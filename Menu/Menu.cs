// Denna klass agerar som basklass till alla menyer
// Den bestämmer vad alla menyer skall innehålla, som exempelvis:
// - Kommandon som tillhör menyn (commands listan)
// - En funktion för att exekvera kommandon (som tillhör menyn)
// - En Display funktion som är till för att "visa upp" menyn
//    - Den är abstrakt eftersom det är dem specifika menyerna (Login, User) som bestämmer hur de ser ut
public abstract class Menu
{

    private List<Command> commands = new List<Command>();

    // Add command to list.
    // Anropa denna för att "registrera", eller "koppla", ett specifikt kommando till menyn.
    public void AddCommand(Command command)
    {
        this.commands.Add(command);
    }

    // Kör ett kommando som är registrerat med "AddCommand", om ett sådant finns.
    public void ExecuteCommand(string inputCommand)
    {
        // Dela upp kommandosträng (e.g login username password) i ord: ["login", "username", "password"]
        // Varje ord blir ett element i resultat arrayen (commandParts).
        string[] commandParts = inputCommand.Split(" ");

        foreach (Command command in commands)
        {
            if (command.Name.Equals(commandParts[0]))
            {
                command.Execute(commandParts);
                return;
            }
        }

        throw new ArgumentException("Command not found.");
    }

    public abstract void Display();
}