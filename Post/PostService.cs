// Ansvarar för att "hantera" inlägg, vilket innebär:
// - Att kunna skapa inlägg
// - Att kunna hämta alla inlägg
// - Att kunna hämta kommentarer för inlägg
// - (inte gjort än) Att kommentera på inlägg och andra kommentarer
// - (inte gjort än) Att gilla och ogilla inlägg
// 
// Vi gör det till ett interface så att vi får möjligheten att vara flexibla.
//
// Tanken med PostService är att den bara hanterar logik - ingen frontend, inga console writelines, inga console readlines och så vidare. 
// Detta kallas "business logic". Fördelen är att klassen blir mycket mer flexibel. 
public interface IPostService
{
    Post CreatePost(string content);
    List<Post> GetAllPosts();
    List<Post> GetAllCommentsForPost(Guid postId);
    void AddCommentToPost(string content, Guid parentPostId, Guid originalPostId);
}