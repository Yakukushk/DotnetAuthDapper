namespace DotnetAPI.Models.DTOs
{
    public class PostEditDto
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; } = default!;
        public string PostContent { get; set; } = default!;

        public PostEditDto()
        {
            if (PostTitle is null)
                PostTitle = string.Empty;
            if (PostContent is null)
                PostContent = string.Empty;
        }
    }
}
