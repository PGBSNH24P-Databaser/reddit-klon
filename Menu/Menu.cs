// Login menu
// User menu

public abstract class Menu {

    private List<Command> commands = new List<Command>();

    // Add command to list.
    public void AddCommand(Command command) {
        this.commands.Add(command);
    }

    public void ExecuteCommand(string inputCommand) {
        // Dela upp kommandosträng (e.g login username password) i ord: ["login", "username", "password"]
        // Varje ord blir ett element i resultat arrayen (commandParts).
        string[] commandParts = inputCommand.Split(" ");

        foreach (Command command in commands) {
            if (command.Name.Equals(commandParts[0])) {
                command.Execute(commandParts);
                return;
            }
        }

        throw new ArgumentException("Command not found.");
    }

    public abstract void Display();
}