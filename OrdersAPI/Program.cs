using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
		opt => opt.UseSqlite(builder
			.Configuration.GetConnectionString("BaseConnection")));

var app = builder.Build();

app.MapPost("api/orders",
		async (AppDbContext context, CreateOrderCommand command) =>
{
	// await context.Orders.AddAsync(order);
	// await context.SaveChangesAsync();

	var createdOrder = await CreateOrderCommandHandler
		.Handle(command, context);
	if (createdOrder == null)
	{
		return Results.BadRequest("Failed to create order");
	}

	return Results.Created($"/api/orders/{createdOrder.Id}", createdOrder);
});

app.MapGet("api/orders/{id}", async (AppDbContext context, int id) =>
{
	// var order = await context
	// 	.Orders.FirstOrDefaultAsync(o => o.Id == id);

	var order = await GetOrderByIdQueryHandler.Handle(
			new GetOrderByIdQuery(id), context);
	if (order == null)
		return Results.NotFound();

	return Results.Ok(order);
});

app.Run();
