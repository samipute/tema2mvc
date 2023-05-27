using LiteDB;

namespace tema2mvc.Models
{
#nullable disable
    public record OrderDAO
    {
        public enum Status
        {
            NewOrder,
            Preparing,
            Ready
        };

        [BsonId]
        [XSerializer.JsonIgnore]
        public ObjectId Id { get; set; }

        public Dictionary<string, int> Items { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public Status OrderStatus { get; set; } = Status.NewOrder;
    }

    public record OrderDAOStripped
    {
        public Dictionary<string, int> Items { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public OrderDAO.Status OrderStatus { get; set; } = OrderDAO.Status.NewOrder;

        public OrderDAOStripped(OrderDAO order)
        {
            Items = order.Items;
            Time = order.Time;
            OrderStatus = order.OrderStatus;
        }
    }
}
