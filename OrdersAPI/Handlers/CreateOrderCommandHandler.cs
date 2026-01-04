using FluentValidation;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderDto>
{
	private readonly WriteDbContext _context;
	private readonly IValidator<CreateOrderCommand> _validator;
	private readonly IEventPublisher _eventPublisher;

	public CreateOrderCommandHandler(WriteDbContext context,
			IValidator<CreateOrderCommand> validator,
			IEventPublisher eventPublisher)
	{
		_context = context;
		_validator = validator;
		_eventPublisher = eventPublisher;
	}

	// public static async Task<Order> Handle(CreateOrderCommand command,
	// 		AppDbContext context)
	// {
	// var order = new Order
	// {
	// 	FirstName = command.FirstName,
	// 	LastName = command.LastName,
	// 	Status = command.Status,
	// 	CreatedAt = DateTime.Now,
	// 	TotalCost = command.TotalCost
	// };

	// await context.Orders.AddAsync(order);
	// await context.SaveChangesAsync();
	//
	// 	return order;
	// }

	public async Task<OrderDto> HandleAsync(CreateOrderCommand command)
	{
		var validationResult = await _validator.ValidateAsync(command);
		if (!validationResult.IsValid)
		{
			throw new ValidationException(validationResult.Errors);
		}

		var order = new Order
		{
			FirstName = command.FirstName,
			LastName = command.LastName,
			Status = command.Status,
			CreatedAt = DateTime.Now,
			TotalCost = command.TotalCost
		};

		await _context.Orders.AddAsync(order);
		await _context.SaveChangesAsync();

		var orderCreatedEvent = new OrderCreatedEvent
		(
			order.Id,
			order.FirstName,
			order.LastName,
			order.TotalCost
		);

		await _eventPublisher.PublishAsync(orderCreatedEvent);

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
