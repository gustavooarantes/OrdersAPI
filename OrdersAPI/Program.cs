using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
		opt => opt.UseSqlite(builder
			.Configuration.GetConnectionString("BaseConnection")));
builder.Services.AddScoped<ICommandHandler<CreateOrderCommand, OrderDto>,
	CreateOrderCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetOrderByIdQuery, OrderDto>,
	GetOrderByIdQueryHandler>();

var app = builder.Build();

app.MapPost("api/orders",
		async (ICommandHandler<CreateOrderCommand, OrderDto> handler,
			CreateOrderCommand command) =>
{
	// await context.Orders.AddAsync(order);
	// await context.SaveChangesAsync();
	// var createdOrder = await CreateOrderCommandHandler
	// 	.Handle(command, context);

	var createdOrder = await handler.HandleAsync(command);

	if (createdOrder == null)
	{
		return Results.BadRequest("Failed to create order");
	}

	return Results.Created($"/api/orders/{createdOrder.Id}", createdOrder);
});

app.MapGet("api/orders/{id}", async (
			IQueryHandler<GetOrderByIdQuery, OrderDto> handler, int id) =>
{
	// var order = await context
	// 	.Orders.FirstOrDefaultAsync(o => o.Id == id);
	// var order = await GetOrderByIdQueryHandler.Handle(
	// 		new GetOrderByIdQuery(id), context);

	var order = await handler.HandleAsync(new GetOrderByIdQuery(id));

	if (order == null)
		return Results.NotFound();

	return Results.Ok(order);
});

app.Run();
