public class LoginMenu : Menu
{
    public LoginMenu(IUserService userService, IMenuService menuService) {
        AddCommand(new LoginCommand(userService, menuService));
        AddCommand(new RegisterUserCommand(userService, menuService));
    }

    public override void Display()
    {
        Console.WriteLine("Welcome! Please login by typing 'login'");
    }
}