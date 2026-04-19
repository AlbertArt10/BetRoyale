using BetRoyale.API.DTOs.Subscriptions;

namespace BetRoyale.API.Services.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionDetailsDto> SubscribeAsync(
        SubscriptionRequestDto request,
        Guid subscriberId,
        CancellationToken cancellationToken = default);

    Task UnsubscribeAsync(
        Guid analystId,
        Guid subscriberId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SubscribedAnalystListItemDto>> GetMySubscriptionsAsync(
        Guid subscriberId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SubscriberListItemDto>> GetSubscribersByAnalystIdAsync(
        Guid analystId,
        CancellationToken cancellationToken = default);
}
