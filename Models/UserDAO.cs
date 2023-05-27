using LiteDB;

namespace tema2mvc.Models
{
#nullable disable
    public record UserDAO
    {
        public enum Role
        { 
            Admin,
            Employee
        };

        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }
        public Role UserRole { get; set; }
    }
}
