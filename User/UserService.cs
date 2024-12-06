// Ansvarar för att "hantera" användare, vilket innebär:
// - Att kunna registrera/skapa en användare
// - Att kunna logga in på/som användare
// - Att kunna logga ut från användare
// - Att kunna hämta användare som är inloggad
// 
// Vi gör det till ett interface så att vi får möjligheten att vara flexibla.
//
// Tanken med UserService är att den bara hanterar logik - ingen frontend, inga console writelines, inga console readlines och så vidare. 
// Detta kallas "business logic". Fördelen är att klassen blir mycket mer flexibel. 
public interface IUserService {
    User RegisterUser(string username, string password);
    User? Login(string username, string password);
    void Logout();
    User? GetLoggedInUser();
}