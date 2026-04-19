namespace BetRoyale.API.DTOs.Comments;

public class CommentListItemDto
{
    public Guid Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public Guid ArticleId { get; set; }
}
