using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WriteDbContext>(
			opt => opt.UseSqlite(builder
			.Configuration.GetConnectionString("WriteDbConnection")));

builder.Services.AddDbContext<ReadDbContext>(
			opt => opt.UseSqlite(builder
			.Configuration.GetConnectionString("ReadDbConnection")));

builder.Services.AddScoped<IValidator<CreateOrderCommand>,
	CreateOrderCommandValidator>();

builder.Services.AddMediatR(cfg =>
		cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

app.MapPost("/api/orders",
		async (IMediator mediator, CreateOrderCommand command) =>
{
	try
	{
		var createdOrder = await mediator.Send(command);

		if (createdOrder == null)
		{
			return Results.BadRequest("Failed to create order");
		}

		return Results.Created($"/api/orders/{createdOrder.Id}", createdOrder);
	}
	catch (ValidationException ex)
	{
		var errors = ex.Errors.Select(e => new
		{
			e.PropertyName,
			e.ErrorMessage
		});
		return Results.BadRequest(errors);
	}
});

app.MapGet("/api/orders/{id}", async (
			IMediator mediator, int id) =>
{
	var order = await mediator.Send(new GetOrderByIdQuery(id));

	if (order == null)
		return Results.NotFound();

	return Results.Ok(order);
});

app.MapGet("/api/orders", async (
			IMediator mediator) =>
	{
		var summaries = await mediator.Send(new GetOrderSummariesQuery());
		return Results.Ok(summaries);
	});

app.Run();
