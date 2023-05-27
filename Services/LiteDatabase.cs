using LiteDB;

namespace tema2mvc.Services
{
    public class LiteDatabaseObject : LiteDatabase
    {
        public LiteDatabaseObject() : base("db") { }
    }
}
