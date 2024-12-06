public class PostMenu : Menu
{

    public Post Post { get; init; }

    IUserService userService;
    IMenuService menuService;
    IPostService postService;

    public PostMenu(Post post, IUserService userService, IMenuService menuService, IPostService postService) {
        this.Post = post;
        this.userService = userService;
        this.menuService = menuService;
        this.postService = postService;

        AddCommand(new CreateCommentCommand(userService, menuService, postService));
    }

    public override void Display()
    {
        Console.Clear();

        if (Post.User == null)
        {
            Console.WriteLine($"<deleted> - {Post.Content}");
        }
        else
        {
            Console.WriteLine($"{Post.User.Name} - {Post.Content}");
        }

        Console.WriteLine("Comments:");

        List<Post> comments = postService.GetAllCommentsForPost(Post.Id);
        foreach (Post comment in comments)
        {
            if (comment.User == null)
            {
                Console.WriteLine($"  <deleted> - {comment.Content}");
            }
            else
            {
                Console.WriteLine($"  {comment.User.Name} - {comment.Content}");
            }
        }
    }
}