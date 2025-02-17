namespace DotnetAPI.Models.DTOs
{
    public class PostDto
    {
        public string PostTitle { get; set; } = default!;
        public string PostContent { get; set; } = default!;

        public PostDto()
        {
            if (PostTitle is null)
                PostTitle = string.Empty;
            if (PostContent is null)
                PostContent = string.Empty;
        }
    }
}
