using tema2mvc.Models;

namespace tema2mvc.Services
{
    public interface IExporter
    {
        public byte[] Export(OrderDAO order);
    }
}
