using Microsoft.EntityFrameworkCore;

public class GetOrderByIdQueryHandler
{
	public static async Task<Order?> Handle(GetOrderByIdQuery query,
			AppDbContext context)
	{
		return await context.Orders.FirstOrDefaultAsync(
				o => o.Id == query.OrderId);
	}
}
