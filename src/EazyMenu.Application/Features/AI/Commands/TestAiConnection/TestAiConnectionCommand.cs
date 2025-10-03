using MediatR;

namespace EazyMenu.Application.Features.AI.Commands.TestAiConnection;

/// <summary>
/// دستور تست اتصال با سرویس هوش مصنوعی
/// </summary>
public class TestAiConnectionCommand : IRequest<TestConnectionResult>
{
    public Guid RestaurantId { get; set; }
}

/// <summary>
/// نتیجه تست اتصال
/// </summary>
public class TestConnectionResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int ResponseTimeMs { get; set; }
}
