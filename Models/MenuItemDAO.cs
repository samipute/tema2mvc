using LiteDB;

namespace tema2mvc.Models
{
#nullable disable
    public record MenuItemDAO
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }
        public int Stock { get; set; }
        public float Price { get; set; }
    }
}
