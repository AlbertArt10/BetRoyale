namespace BetRoyale.API.DTOs.ArticleLikes;

public class ArticleLikeSummaryDto
{
    public Guid ArticleId { get; set; }

    public int LikesCount { get; set; }

    public bool IsLikedByCurrentUser { get; set; }
}
