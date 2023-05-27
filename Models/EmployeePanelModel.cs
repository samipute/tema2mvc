namespace tema2mvc.Models
{
    public class EmployeePanelModel
    {
        public required IEnumerable<OrderDAO> Orders { get; set; }
        public required IEnumerable<MenuItemDAO> MenuItems { get; set; }
        public OrderDAO? SelectedOrder { get; set; }
    }
}
