using ExcelLibrary;
using ExcelLibrary.SpreadSheet;
using tema2mvc.Models;

namespace tema2mvc.Services
{
    public class ExcelExporter : IExporter
    {
        private readonly IDataAccess<MenuItemDAO> _items;

        public ExcelExporter(IDataAccess<MenuItemDAO> items)
        {
            _items = items;
        }

        public byte[] Export(OrderDAO order)
        {
            Workbook workbook = new();
            Worksheet sheet = new("Order");

            sheet.Cells[0, 0] = new Cell("Time");
            sheet.Cells[0, 1] = new Cell(order.Time.ToString());
            sheet.Cells[1, 0] = new Cell("Status");
            sheet.Cells[1, 1] = new Cell(order.OrderStatus.ToString());
            sheet.Cells[2, 0] = new Cell("Item name");
            sheet.Cells[2, 1] = new Cell("Qty");

            const int rowOffset = 3;
            int i = 0;
            foreach (var item in order.Items)
            {
                var menuItem = _items.GetById(item.Key) ?? throw new Exception();
                sheet.Cells[i + rowOffset, 0] = new Cell(menuItem.Name);
                sheet.Cells[i + rowOffset, 1] = new Cell(item.Value.ToString());
                i++;
            }

            workbook.Worksheets.Add(sheet);
            using MemoryStream memStream = new();
            workbook.SaveToStream(memStream);
            return memStream.ToArray();
        }
    }
}
