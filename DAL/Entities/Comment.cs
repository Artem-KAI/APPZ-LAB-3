namespace DAL.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int AuthorId { get; set; }
        public int ArticleId { get; set; }
        public virtual User Author { get; set; }
        public int? ParentId { get; set; }
    }
}