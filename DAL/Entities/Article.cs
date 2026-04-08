namespace DAL.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int AuthorId { get; set; }
        public virtual User Author { get; set; }

        // ОБОВ'ЯЗКОВО ДОДАЙ ЦІ ДВА РЯДКИ:
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}