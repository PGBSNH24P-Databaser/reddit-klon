public abstract class Command {
    public string Name { get; init; }
    public string Description { get; init; }

    protected IUserService userService;
    protected IMenuService menuService;

    public Command(string name, string description, IUserService userService, IMenuService menuService) {
        this.Name = name;
        this.Description = description;
        this.userService = userService;
        this.menuService = menuService;
    } 

    public abstract void Execute(string[] args);
}