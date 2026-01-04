public class OrderCreatedProjectionHandler : IEventHandler<OrderCreatedEvent>
{
	private readonly ReadDbContext _context;

	public OrderCreatedProjectionHandler(ReadDbContext context)
	{
		_context = context;
	}

	public async Task HandleAsync(OrderCreatedEvent evt)
	{
		var order = new Order
		{
			Id = evt.OrderId,
			FirstName = evt.FirstName,
			LastName = evt.LastName,
			Status = "Created",
			CreatedAt = DateTime.Now,
			TotalCost = evt.TotalCost
		};

		await _context.Orders.AddAsync(order);
		await _context.SaveChangesAsync();
	}
}
