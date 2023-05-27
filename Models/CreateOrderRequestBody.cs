namespace tema2mvc.Models
{
    public record CreateOrderRequestBody
    {
        public Dictionary<string, int> Items { get; set; } = new();
    }
}
