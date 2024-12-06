// Ansvarar för att "hantera" menyer:
// - Hämta aktiv meny
// - Byta till ny (aktiv) meny
public interface IMenuService
{
    void SetMenu(Menu menu);
    Menu GetMenu();
}

public class SimpleMenuService : IMenuService
{
    private Menu menu = new EmptyMenu();

    public Menu GetMenu()
    {
        return menu;
    }

    public void SetMenu(Menu menu)
    {
        this.menu = menu;
        this.menu.Display();
    }
}

// Startmeny som bara används i första början.
// Den skall bytas ut mot en "riktig" menu direkt, som LoginMenu exempelvis.
class EmptyMenu : Menu
{
    public override void Display() { }
}