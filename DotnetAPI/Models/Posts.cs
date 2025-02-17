namespace DotnetAPI.Models
{
    public partial class Posts
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string PostTitle { get; set; } = default!;
        public string PostContent { get; set; } = default!;
        public DateTime PostCreated { get; set; }
        public DateTime PostUpdated { get; set; }

        public Posts()
        {
            if (PostTitle is null)
                PostTitle = string.Empty;
            if (PostContent is null)
                PostContent = string.Empty;
        }
    }
}
