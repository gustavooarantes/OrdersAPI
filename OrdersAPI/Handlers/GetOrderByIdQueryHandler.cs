using Microsoft.EntityFrameworkCore;

public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDto>
{
	private readonly AppDbContext _context;

	// public static async Task<Order?> Handle(GetOrderByIdQuery query,
	// 		AppDbContext context)
	// {
	// 	return await context.Orders.FirstOrDefaultAsync(
	// 			o => o.Id == query.OrderId);
	// }

	public GetOrderByIdQueryHandler(AppDbContext context)
	{
		_context = context;
	}

	public async Task<OrderDto?> HandleAsync(GetOrderByIdQuery query)
	{
		var order = await _context.Orders.FirstOrDefaultAsync(
				o => o.Id == query.OrderId);

		if (order == null)
			return null;

		return new OrderDto(
			order.Id,
			order.FirstName,
			order.LastName,
			order.Status,
			order.CreatedAt,
			order.TotalCost
		);
	}
}
