namespace tema2mvc.Models
{
    public record AdminPanelModel
    {
        public required IEnumerable<UserDAO> Users { get; set; }
        public required IEnumerable<MenuItemDAO> MenuItems { get; set; }
        public StatisticsModel? Statistics { get; set; }
    }
}
