public class CreatePostCommand : Command
{
    public CreatePostCommand(
        IUserService userService, 
        IMenuService menuService, 
        IPostService postService
        ) : base(
            "create-post",
             "Create and upload a post.", 
             userService, 
             menuService, 
             postService
             )
    {
    }

    public override void Execute(string[] args)
    {
        string content = "";// Jag gillar glass
        foreach (var part in args[1..]) {
            content += part + " ";
        }
        
        // args = ["create-post", "Jag", "gillar", "glass"]
        // args[1..] = ["Jag", "gillar", "glass"]

        Post post = postService.CreatePost(content);
        Console.WriteLine("Created post!");
    }
}