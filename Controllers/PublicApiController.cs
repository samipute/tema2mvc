using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using tema2mvc.Models;
using tema2mvc.Services;

namespace tema2mvc.Controllers
{
    public class PublicApiController : Controller
    {
        private readonly IDataAccess<UserDAO> _users;
        private readonly IDataAccess<MenuItemDAO> _menuItems;
        private readonly IDataAccess<OrderDAO> _orders;

        public PublicApiController(
            IDataAccess<UserDAO> users,
            IDataAccess<MenuItemDAO> menuItems,
            IDataAccess<OrderDAO> orders)
        {
            _users = users;
            _menuItems = menuItems;
            _orders = orders;
        }

        [HttpGet]
        public IActionResult GetOrders([FromQuery] string? startDate, [FromQuery] string? endDate)
        {
            var stats = GetStatsBetweenDates(startDate, endDate);
            if (stats is null)
                return NotFound();

            StatisticsModelStripped stripped = new(stats);
            return Json(stripped.Orders);
        }

        [HttpGet]
        public IActionResult GetMostOrdered([FromQuery] string? startDate, [FromQuery] string? endDate)
        {
            var stats = GetStatsBetweenDates(startDate, endDate);
            if (stats is null)
                return NotFound();

            StatisticsModelStripped stripped = new(stats);
            return Json(stripped.MostOrdered);
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] CreateOrderRequestBody createOrderData)
        {
            if (createOrderData.Items.Count == 0)
                return BadRequest();

            Dictionary<string, int> parsedItems = new();
            foreach (var item in createOrderData.Items)
            {
                string itemName = item.Key;
                int itemQty = item.Value;

                var foundItems = _menuItems.Get(mi => mi.Name == itemName);
                if (foundItems.Count() == 0)
                    return BadRequest();

                var foundItem = foundItems.First();
                parsedItems.Add(foundItem.Id.ToString(), itemQty);
            }

            OrderDAO order = new()
            {
                Items = parsedItems
            };

            _orders.Create(order);
            return Json(new { success = true, id = order.Id.ToString() });
        }

        private StatisticsModel? GetStatsBetweenDates(string? startDate, string? endDate)
        {
            if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
                return null;

            var start = ParseDate(startDate);
            var end = ParseDate(endDate);
            if (start is null || end is null)
                return null;

            var stats = GenerateStatistics(start.Value, end.Value);
            return stats;
        }

        private DateTime? ParseDate(string date)
        {
            string[] split = date.Split('-');
            if (split.Length < 3)
                return null;

            if (!int.TryParse(split[0], out int year) ||
                !int.TryParse(split[1], out int month) ||
                !int.TryParse(split[2], out int day))
                return null;

            return new(year, month, day);
        }

        private StatisticsModel? GenerateStatistics(DateTime startDate, DateTime endDate)
        {
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
