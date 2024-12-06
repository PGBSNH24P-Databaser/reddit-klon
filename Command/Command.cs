// Basklassen för alla kommandon. Alla kommandon har ett namn och en beskrivning som tas in genom constructorn.
// Kommandon (exempelvis LoginCommand) behöver ha tillgång till services och de tas också in genom constructorn.
public abstract class Command
{
    public string Name { get; init; }
    public string Description { get; init; }

    protected IUserService userService;
    protected IMenuService menuService;
    protected IPostService postService;

    public Command(string name, string description, IUserService userService, IMenuService menuService, IPostService postService)
    {
        this.Name = name;
        this.Description = description;
        this.userService = userService;
        this.menuService = menuService;
        this.postService = postService;
    }

    public abstract void Execute(string[] args);
}