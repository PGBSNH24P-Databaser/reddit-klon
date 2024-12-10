public class CommentOnCommentCommand : Command
{
    // Man skulle även kunna kalla kommandot för "nested-comment"
    public CommentOnCommentCommand(IUserService userService, IMenuService menuService, IPostService postService) : base("id-comment", "Add a comment to another comment.", userService, menuService, postService)
    {
    }

    public override void Execute(string[] args)
    {
        PostMenu menu = (PostMenu)menuService.GetMenu();
        Post post = menu.Post;

        int commentIndex = int.Parse(args[1]);
        List<Post> comments = postService.GetAllCommentsForPost(post.Id);

        Post comment = comments[commentIndex];

        string content = "";// Jag gillar glass
        foreach (var part in args[2..])
        {
            content += part + " ";
        }

        postService.AddCommentToPost(content, comment.Id, post.Id);
        menu.Display();
    }
}