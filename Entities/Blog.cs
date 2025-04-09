namespace UetdsProgramiNet.Entities
{
    public class Blog : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SubDescription { get; set; }
        public string InfoUrl { get; set; }
        public string ImgUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime? PublishedDate { get; set; }
    }
}
