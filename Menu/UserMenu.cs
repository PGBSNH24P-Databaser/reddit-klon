public class UserMenu : Menu
{
    public UserMenu(IUserService userService, IMenuService menuService, IPostService postService) {
        AddCommand(new CreatePostCommand(userService, menuService, postService));
    }

    public override void Display()
    {
        Console.WriteLine("Type 'help' for a list of commands.");
    }
}