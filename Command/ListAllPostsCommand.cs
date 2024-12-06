public class ListAllPostsCommand : Command
{
    public ListAllPostsCommand(IUserService userService, IMenuService menuService, IPostService postService) : base("list-posts", "List all created posts.", userService, menuService, postService)
    {
    }

    public override void Execute(string[] args)
    {
        List<Post> posts = postService.GetAllPosts();

        foreach (var post in posts)
        {
            if (post.User == null)
            {
                Console.WriteLine($"<deleted> - {post.Content}");
            }
            else
            {
                Console.WriteLine($"{post.User.Name} - {post.Content}");
            }
        }
    }
}