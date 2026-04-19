using BetRoyale.API.Data;
using BetRoyale.API.Data.Seed;
using BetRoyale.API.DTOs.Subscriptions;
using BetRoyale.API.Entities;
using BetRoyale.API.Services.Exceptions;
using BetRoyale.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BetRoyale.API.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _dbContext;

    public SubscriptionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SubscriptionDetailsDto> SubscribeAsync(
        SubscriptionRequestDto request,
        Guid subscriberId,
        CancellationToken cancellationToken = default)
    {
        ValidateSubscriberId(subscriberId);

        var analystId = ValidateAnalystId(request.AnalystId);

        if (subscriberId == analystId)
        {
            throw new InvalidSubscriptionException("Users cannot subscribe to themselves.");
        }

        var subscriberExists = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == subscriberId, cancellationToken);

        if (!subscriberExists)
        {
            throw new SubscriptionUserNotFoundException($"Subscription user '{subscriberId}' was not found.");
        }

        var analyst = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Id == analystId, cancellationToken);

        if (analyst is null)
        {
            throw new UserNotFoundException($"User '{analystId}' was not found.");
        }

        if (analyst.RoleId != RoleSeedData.AnalystId)
        {
            throw new InvalidSubscriptionException("Subscriptions are allowed only to users with role 'Analyst'.");
        }

        var subscriptionExists = await _dbContext.Subscriptions
            .AsNoTracking()
            .AnyAsync(
                subscription => subscription.SubscriberId == subscriberId && subscription.AnalystId == analystId,
                cancellationToken);

        if (subscriptionExists)
        {
            throw new InvalidSubscriptionException("User is already subscribed to this analyst.");
        }

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            SubscriberId = subscriberId,
            AnalystId = analystId
        };

        _dbContext.Subscriptions.Add(subscription);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDetailsDto(subscription);
    }

    public async Task UnsubscribeAsync(
        Guid analystId,
        Guid subscriberId,
        CancellationToken cancellationToken = default)
    {
        ValidateSubscriberId(subscriberId);

        var normalizedAnalystId = ValidateAnalystId(analystId);

        var subscription = await _dbContext.Subscriptions
            .SingleOrDefaultAsync(
                currentSubscription =>
                    currentSubscription.SubscriberId == subscriberId &&
                    currentSubscription.AnalystId == normalizedAnalystId,
                cancellationToken);

        if (subscription is null)
        {
            throw new SubscriptionNotFoundException(
                $"Subscription to analyst '{normalizedAnalystId}' for user '{subscriberId}' was not found.");
        }

        _dbContext.Subscriptions.Remove(subscription);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SubscribedAnalystListItemDto>> GetMySubscriptionsAsync(
        Guid subscriberId,
        CancellationToken cancellationToken = default)
    {
        ValidateSubscriberId(subscriberId);

        var subscriberExists = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == subscriberId, cancellationToken);

        if (!subscriberExists)
        {
            throw new SubscriptionUserNotFoundException($"Subscription user '{subscriberId}' was not found.");
        }

        return await _dbContext.Subscriptions
            .AsNoTracking()
            .Where(subscription => subscription.SubscriberId == subscriberId)
            .OrderBy(subscription => subscription.Analyst.Username)
            .Select(subscription => new SubscribedAnalystListItemDto
            {
                SubscriptionId = subscription.Id,
                AnalystId = subscription.AnalystId,
                AnalystUsername = subscription.Analyst.Username
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SubscriberListItemDto>> GetSubscribersByAnalystIdAsync(
        Guid analystId,
        CancellationToken cancellationToken = default)
    {
        var normalizedAnalystId = ValidateAnalystId(analystId);

        var analyst = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Id == normalizedAnalystId, cancellationToken);

        if (analyst is null)
        {
            throw new UserNotFoundException($"User '{normalizedAnalystId}' was not found.");
        }

        if (analyst.RoleId != RoleSeedData.AnalystId)
        {
            throw new InvalidSubscriptionException("Subscriptions are allowed only to users with role 'Analyst'.");
        }

        return await _dbContext.Subscriptions
            .AsNoTracking()
            .Where(subscription => subscription.AnalystId == normalizedAnalystId)
            .OrderBy(subscription => subscription.Subscriber.Username)
            .Select(subscription => new SubscriberListItemDto
            {
                SubscriptionId = subscription.Id,
                SubscriberId = subscription.SubscriberId,
                SubscriberUsername = subscription.Subscriber.Username
            })
            .ToListAsync(cancellationToken);
    }

    private static SubscriptionDetailsDto MapToDetailsDto(Subscription subscription)
    {
        return new SubscriptionDetailsDto
        {
            Id = subscription.Id,
            SubscriberId = subscription.SubscriberId,
            AnalystId = subscription.AnalystId
        };
    }

    private static void ValidateSubscriberId(Guid subscriberId)
    {
        if (subscriberId == Guid.Empty)
        {
            throw new InvalidSubscriptionException("SubscriberId is required.");
        }
    }

    private static Guid ValidateAnalystId(Guid analystId)
    {
        if (analystId == Guid.Empty)
        {
            throw new InvalidSubscriptionException("AnalystId is required.");
        }

        return analystId;
    }
}
