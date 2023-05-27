using System.Linq.Expressions;

namespace tema2mvc.Services
{
    public interface IDataAccess<T> where T : class
    {
        T? GetById(string id);
        IEnumerable<T> Get(Expression<Func<T, bool>> expression);
        T? GetFirst(Expression<Func<T, bool>> expression);
        IEnumerable<T> GetAll();
        void Create(T entity);
        void Update(T entity);
        void RemoveById(string id);
    }
}
