using System.Text;
using tema2mvc.Models;

namespace tema2mvc.Services
{
    public class CsvExporter : IExporter
    {
        private readonly IDataAccess<MenuItemDAO> _items;

        public CsvExporter(IDataAccess<MenuItemDAO> items)
        {
            _items = items;
        }

        public byte[] Export(OrderDAO order)
        {
            List<string> header = new();
            List<string> status = new();
            List<string> tableHead = new();

            header.Add("Time");
            header.Add(order.Time.ToString());
            status.Add("Status");
            status.Add(order.OrderStatus.ToString());
            tableHead.Add("Item name");
            tableHead.Add("Qty");

            List<string> rows = new();
            foreach (var item in order.Items)
            {
                var menuItem = _items.GetById(item.Key) ?? throw new Exception();
                string row = $"{menuItem.Name},{item.Value},";
                rows.Add(row);
            }

            StringBuilder sb = new();
            sb.AppendJoin(',', header);
            sb.Append(",\n");
            sb.AppendJoin(',', status);
            sb.Append(",\n");
            sb.AppendJoin(',', tableHead);
            sb.Append(",\n");
            sb.AppendJoin('\n', rows);

            string content = sb.ToString();
            return Encoding.UTF8.GetBytes(content);
        }
    }
}
