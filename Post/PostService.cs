public interface IPostService {
    Post CreatePost(string content);
    List<Post> GetAllPosts();
    List<Post> GetAllCommentsForPost(Guid postId);
}