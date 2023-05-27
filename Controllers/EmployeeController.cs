using Microsoft.AspNetCore.Mvc;
using tema2mvc.Models;
using tema2mvc.Services;

namespace tema2mvc.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IDataAccess<OrderDAO> _orders;
        private readonly IDataAccess<MenuItemDAO> _menuItems;

        public EmployeeController(
            IDataAccess<OrderDAO> orders,
            IDataAccess<MenuItemDAO> menuItems)
        {
            _orders = orders;
            _menuItems = menuItems;
        }

        [HttpGet]
        public IActionResult Index()
        {
            EmployeePanelModel model = new()
            {
                Orders = _orders.GetAll(),
                MenuItems = _menuItems.GetAll()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult ViewOrder(string id)
        {
            OrderDAO? selected = null;
            if (id is not null)
            {
                selected = _orders.GetById(id);
            }

            EmployeePanelModel model = new()
            {
                Orders = _orders.GetAll(),
                MenuItems = _menuItems.GetAll(),
                SelectedOrder = selected
            };

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult AddOrder(IDictionary<string, int> items)
        {
            Dictionary<string, int> menuItems = new();
            foreach (var item in items)
            {
                var menuItem = _menuItems.GetById(item.Key);
                if (menuItem is null || item.Value > menuItem.Stock)
                    return BadRequest();

                menuItems.Add(item.Key, item.Value);
            }

            OrderDAO order = new()
            {
                Items = menuItems
            };

            _orders.Create(order);
            var id = order.Id;

            return Json(new { success = true, id = id.ToString() });
        }

        [HttpDelete]
        public IActionResult RemoveOrder([FromQuery] string? order)
        {
            if (string.IsNullOrEmpty(order))
                return BadRequest();

            _orders.RemoveById(order);

            return Json(new { success = true });
        }

        [HttpPatch]
        public IActionResult AddToOrder([FromQuery] string? order, [FromQuery] string? item)
        {
            if (string.IsNullOrEmpty(order) || string.IsNullOrEmpty(item))
                return NotFound();

            var o = _orders.GetById(order);
            var i = _menuItems.GetById(item);
            if (o is null || i is null)
                return NotFound();

            o.Items.Add(order, 1);
            _orders.Update(o);

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetItemDetails([FromQuery] string? id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var item = _menuItems.GetById(id);
            if (item is null)
                return NotFound();

            return Json(new
            {
                id,
                name = item.Name,
                stock = item.Stock,
                price = item.Price
            });
        }

        [HttpPatch]
        public IActionResult UpdateOrderStatus([FromQuery] string? order, [FromQuery] int? status)
        {
            if (order is null || status is null)
                return BadRequest();

            var orderDAO = _orders.GetById(order);
            if (orderDAO is null || status.Value < 1 || status.Value > 2)
                return BadRequest();

            orderDAO.OrderStatus = (OrderDAO.Status)status.Value;
            _orders.Update(orderDAO);

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult DownloadOrder([FromQuery] string? id, [FromQuery] string? type)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(type))
                return BadRequest();

            var order = _orders.GetById(id);
            if (order is null)
                return NotFound();

            IExporter? exporter = null;
            string? fileName = null;
            if (type == "excel")
            {
                exporter = ExporterFactory.CreateExcelExporter(_menuItems);
                fileName = "excel.xls";
            }
            else if (type == "csv")
            {
                exporter = ExporterFactory.CreateCsvExporter(_menuItems);
                fileName = "csv.csv";
            }

            if (exporter is null)
                return BadRequest();

            byte[] file = exporter.Export(order);
            return File(file, "application/octet-stream", fileName);
        }
    }
}
