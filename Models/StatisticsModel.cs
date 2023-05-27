namespace tema2mvc.Models
{
    public record StatisticsModel
    {
        public required OrderDAO[] Orders { get; set; }
        public required IDictionary<string, int> MostOrdered { get; set; }
    }

    public record StatisticsModelStripped
    { 
        public OrderDAOStripped[] Orders { get; set; }
        public IDictionary<string, int> MostOrdered { get; set; }

        public StatisticsModelStripped(StatisticsModel model)
        {
            Orders = model.Orders.Select(o => new OrderDAOStripped(o)).ToArray();
            MostOrdered = model.MostOrdered;
        }
    }
}
