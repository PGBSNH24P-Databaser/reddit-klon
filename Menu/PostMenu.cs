using System.Reflection.Metadata.Ecma335;

public class PostMenu : Menu
{

    public Post Post { get; init; }

    IUserService userService;
    IMenuService menuService;
    IPostService postService;

    public PostMenu(Post post, IUserService userService, IMenuService menuService, IPostService postService)
    {
        this.Post = post;
        this.userService = userService;
        this.menuService = menuService;
        this.postService = postService;

        AddCommand(new CreateCommentCommand(userService, menuService, postService));
        AddCommand(new CommentOnCommentCommand(userService, menuService, postService));
    }

    public override void Display()
    {
        Console.Clear();

        if (Post.User == null)
        {
            Console.WriteLine($"<deleted> - {Post.Content}");
        }
        else
        {
            Console.WriteLine($"{Post.User.Name} - {Post.Content}");
        }

        Console.WriteLine("Comments:");

        /*
            inlägg
            kommentar
                kommentar
                    kommentar
                kommentar

            0
            1
            2-
            3--
            4
            5-
            List<Post> sorted
            0
                2
            1
            4
        */

        // -
        // 3 4 5

        List<Post> comments = postService.GetAllCommentsForPost(Post.Id);
        List<SubPosts> subComments = new List<SubPosts>();

        Dictionary<Guid, int> postIndex = new Dictionary<Guid, int>();
        for (int i = 0; i < comments.Count; i++)
        {
            postIndex.Add(comments[i].Id, i);
        }

        /*
        
        A
            D
                B
                G
        C
            F
        E

        */

        for (int i = 0; i < comments.Count; i++)
        {
            var comment = comments[i];
            if (comment.ParentPostId == Post.Id)
            {
                subComments.Add(new SubPosts
                {
                    Post = comment,
                    Nested = new List<SubPosts>()
                });

                comments.RemoveAt(i);
                i--;
            }
        }

        while (comments.Count > 0)
        {
            foreach (var comment in subComments)
            {
                AddSubComments(comment, comments);
            }
        }

        Console.WriteLine(subComments.Count);
        foreach (SubPosts comment in subComments)
        {
            PrintComment(comment, 1, postIndex);
        }
    }

    private static void AddSubComments(SubPosts comment, List<Post> comments)
    {
        for (int i = 0; i < comments.Count; i++)
        {
            var subComment = comments[i];
            if (subComment.ParentPostId == comment.Post.Id)
            {
                comment.Nested.Add(new SubPosts
                {
                    Post = subComment,
                    Nested = new List<SubPosts>()
                });

                comments.RemoveAt(i);
                i--;
            }
        }

        foreach (SubPosts subComment in comment.Nested)
        {
            AddSubComments(subComment, comments);
        }
    }

    private static void PrintComment(SubPosts comment, int level, Dictionary<Guid, int> postIndex)
    {
        for (int i = 0; i < level; i++)
        {
            Console.Write("  ");
        }

        int index = postIndex[comment.Post.Id];

        if (comment.Post.User == null)
        {
            Console.WriteLine($"  {index} <deleted> - {comment.Post.Content}");
        }
        else
        {
            Console.WriteLine($"  {index} {comment.Post.User.Name} - {comment.Post.Content}");
        }

        foreach (SubPosts subComment in comment.Nested)
        {
            PrintComment(subComment, level + 1, postIndex);
        }
    }
}

class SubPosts
{
    public Post Post { get; init; }
    public List<SubPosts> Nested { get; init; }
}