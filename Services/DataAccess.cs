using LiteDB;
using System.Linq.Expressions;

namespace tema2mvc.Services
{
    public class DataAccess<T> : IDataAccess<T> where T : class
    {
        private readonly ILiteCollection<T> _col;

        public DataAccess(ILiteDatabase db)
        {
            _col = db.GetCollection<T>(typeof(T).Name);
        }

        public T? GetById(string id)
        {
            return _col.FindById(new ObjectId(id));
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> expression)
        {
            var find = _col.Find(expression);
            return find;
        }

        public T? GetFirst(Expression<Func<T, bool>> expression)
        {
            return Get(expression).FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            return _col.FindAll();
        }

        public void Create(T entity)
        {
            _col.Insert(entity);
        }

        public void Update(T entity)
        {
            _col.Update(entity);
        }

        public void RemoveById(string id)
        {
            _col.Delete(new ObjectId(id));
        }
    }
}
