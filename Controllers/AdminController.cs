using LiteDB;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using tema2mvc.Models;
using tema2mvc.Services;
using XSerializer;

namespace tema2mvc.Controllers
{
    public class AdminController : Controller
    {
        private readonly IDataAccess<UserDAO> _users;
        private readonly IDataAccess<MenuItemDAO> _menuItems;
        private readonly IDataAccess<OrderDAO> _orders;

        public AdminController(
            IDataAccess<UserDAO> users,
            IDataAccess<MenuItemDAO> menuItems,
            IDataAccess<OrderDAO> orders)
        {
            _users = users;
            _menuItems = menuItems;
            _orders = orders;
        }

        public IActionResult Index()
        {
            AdminPanelModel model = new()
            {
                Users = _users.GetAll(),
                MenuItems = _menuItems.GetAll()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AddUser(IDictionary<string, string> data)
        {
            UserDAO user = new()
            {
                Name = data["name"],
                UserRole = data["admin"] == "true" ? UserDAO.Role.Admin : UserDAO.Role.Employee,
            };

            _users.Create(user);
            return Json(new { success = true });
        }

        [HttpDelete]
        public IActionResult RemoveUser(IDictionary<string, string> data)
        {
            _users.RemoveById(data["id"]);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult AddItem(IDictionary<string, string> data)
        {
            string name = data["name"];
            int stock = int.Parse(data["stock"]);
            float price = float.Parse(data["price"]);
            MenuItemDAO itemDAO = new()
            {
                Name = name,
                Stock = stock,
                Price = price
            };

            _menuItems.Create(itemDAO);
            return Json(new { success = true });
        }

        [HttpPatch]
        public IActionResult UpdateItem(IDictionary<string, string> data)
        {
            var item = _menuItems.GetById(data["id"]);
            if (item is null)
                return NotFound();

            _menuItems.Update(new()
            {
                Id = item.Id,
                Name = data["name"],
                Stock = int.Parse(data["stock"]),
                Price = float.Parse(data["price"])
            });

            return Json(new { success = true });
        }

        [HttpDelete]
        public IActionResult RemoveItem([FromQuery] string id)
        {
            _menuItems.RemoveById(id);
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult ShowReport([FromQuery] long? start, [FromQuery] long? end)
        {
            if (start is null || end is null)
                return Index();

            var stats = GenerateStatistics(start.Value, end.Value);
            if (stats is null)
                return Index();

            AdminPanelModel model = new()
            {
                Users = _users.GetAll(),
                MenuItems = _menuItems.GetAll(),
                Statistics = stats
            };

            ViewData["startDate"] = start;
            ViewData["endDate"] = end;
            return View("Index", model);
        }

        [HttpGet]
        public IActionResult DownloadReport([FromQuery] long? start, [FromQuery] long? end)
        {
            if (start is null || end is null)
                return BadRequest();

            var stats = GenerateStatistics(start.Value, end.Value);
            if (stats is null)
                return NotFound();

            var xml = XmlSerializer.Create(typeof(StatisticsModelStripped));
            StatisticsModelStripped strippedStats = new(stats);

            byte[] xmlBytes = Encoding.ASCII.GetBytes(xml.Serialize(strippedStats));
            return File(xmlBytes, "application/octet-stream", "report.xml");
        }

        private StatisticsModel? GenerateStatistics(long start, long end)
        {
            DateTime bigBang = new(1970, 1, 1);
            var startDate = bigBang.AddMilliseconds(start).ToLocalTime();
            var endDate = bigBang.AddMilliseconds(end).ToLocalTime();

            var orders = _orders.GetAll()
                .Where(o => o.Time > startDate && o.Time < endDate);

            var items = orders.SelectMany(o => o.Items)
                .GroupBy(i => i.Key, (id, i) => new
                {
                    MenuItem = _menuItems.GetById(id),
                    MenuItemCount = i.Sum(mi => mi.Value)
                })
                .OrderByDescending(i => i.MenuItemCount)
                .Take(10)
                .Where(i => i.MenuItem is not null)
                .ToDictionary(i => i.MenuItem!.Name, i => i.MenuItemCount);

            if (items is null)
                return null;

            return new()
            {
                Orders = orders.ToArray(),
                MostOrdered = items
            };
        }
    }
}
