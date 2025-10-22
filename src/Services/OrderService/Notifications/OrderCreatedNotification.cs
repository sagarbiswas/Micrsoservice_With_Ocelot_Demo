using MediatR;

namespace OrderService.Notifications
{
    public class OrderCreatedNotification : INotification
    {
        public Guid OrderId { get; }
        public string CustomerName { get; }
        public decimal TotalAmount { get; }

        public OrderCreatedNotification(Guid orderId, string customerName, decimal totalAmount)
        {
            OrderId = orderId;
            CustomerName = customerName;
            TotalAmount = totalAmount;
        }
    }
}
