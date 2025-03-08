namespace UetdsProgramiNet.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUsername { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedUsername { get; set; }
        public bool IsDeleted { get; set; }
    }
}
