public class CreateCommentCommand : Command
{
    public CreateCommentCommand(IUserService userService, IMenuService menuService, IPostService postService) : base("comment", "Comment on a post or another comment.", userService, menuService, postService)
    {
    }

    public override void Execute(string[] args)
    {
        PostMenu menu = (PostMenu) menuService.GetMenu();
        Post post = menu.Post;
    }
}