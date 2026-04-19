namespace BetRoyale.API.DTOs.Comments;

public class CreateCommentRequestDto
{
    public string Content { get; set; } = string.Empty;

    public Guid ArticleId { get; set; }
}
