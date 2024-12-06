using System.Diagnostics;

public class ViewPostCommand : Command
{

    public ViewPostCommand(IUserService userService, IMenuService menuService, IPostService postService) : base("view-post", "View a specific post.", userService, menuService, postService)
    {
    }

    public override void Execute(string[] args)
    {
        List<Post> posts = postService.GetAllPosts();
        int postIndex = int.Parse(args[1]);
        Post post = posts[postIndex];

        menuService.SetMenu(new PostMenu(post, userService, menuService, postService));
    }
}