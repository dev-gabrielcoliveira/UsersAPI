using MassTransit;

namespace FCG.Application.Events
{
    [EntityName("UserCreated")]
    public record UserCreatedEvent
    (
        int Id,
        string Nome,
        string Email
    );
}
