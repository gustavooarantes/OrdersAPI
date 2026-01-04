using Microsoft.EntityFrameworkCore;

public class GetOrderSummariesQueryHandler : IQueryHandler<GetOrderSummariesQuery, List<OrderSummaryDto>>
{
	private readonly AppDbContext _context;

	public GetOrderSummariesQueryHandler(AppDbContext context)
	{
		_context = context;
	}

	public async Task<List<OrderSummaryDto>?> HandleAsync(GetOrderSummariesQuery query)
	{
		return await _context.Orders
			.Select(o => new OrderSummaryDto(
						o.Id,
						o.FirstName + " " + o.LastName,
						o.Status,
						o.TotalCost
			)).ToListAsync();
	}
}
