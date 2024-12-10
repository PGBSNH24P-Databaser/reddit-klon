public class CreateCommentCommand : Command
{
    public CreateCommentCommand(IUserService userService, IMenuService menuService, IPostService postService) : base("comment", "Comment on a post or another comment.", userService, menuService, postService)
    {
    }

    // id-comment <index> <content>

    public override void Execute(string[] args)
    {
        PostMenu menu = (PostMenu)menuService.GetMenu();
        Post post = menu.Post;

        string content = "";// Jag gillar glass
        foreach (var part in args[1..])
        {
            content += part + " ";
        }

        postService.AddCommentToPost(content, post.Id, post.Id);
        menu.Display();
    }
}