using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace EazyMenu.Infrastructure.Hubs;

/// <summary>
/// SignalR Hub برای چت با هوش مصنوعی
/// </summary>
public class AiAssistantHub : Hub
{
    private readonly IAiContentService _aiContentService;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<ChatHistory> _chatHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AiAssistantHub> _logger;
    
    // صف داخلی برای مدیریت درخواست‌ها و جلوگیری از محدودیت نرخ
    private static readonly Channel<ChatRequest> _requestQueue = Channel.CreateBounded<ChatRequest>(
        new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        });

    public AiAssistantHub(
        IAiContentService aiContentService,
        IRepository<Product> productRepository,
        IRepository<ChatHistory> chatHistoryRepository,
        IUnitOfWork unitOfWork,
        ILogger<AiAssistantHub> logger)
    {
        _aiContentService = aiContentService;
        _productRepository = productRepository;
        _chatHistoryRepository = chatHistoryRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// اتصال کاربر به Hub
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        _logger.LogInformation("کاربر متصل شد: {ConnectionId}", connectionId);
        
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// قطع اتصال کاربر
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        _logger.LogInformation("کاربر قطع شد: {ConnectionId}", connectionId);
        
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// پیوستن به گروه رستوران
    /// </summary>
    public async Task JoinRestaurantChat(string restaurantId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"restaurant_{restaurantId}");
        _logger.LogInformation("کاربر {ConnectionId} به گروه رستوران {RestaurantId} پیوست", 
            Context.ConnectionId, restaurantId);
    }

    /// <summary>
    /// ارسال پیام به هوش مصنوعی
    /// </summary>
    public async Task SendMessage(string restaurantId, string sessionId, string message)
    {
        try
        {
            _logger.LogInformation("پیام دریافت شد از {ConnectionId} برای رستوران {RestaurantId}", 
                Context.ConnectionId, restaurantId);

            // ارسال وضعیت در حال تایپ
            await Clients.Caller.SendAsync("TypingStarted");

            // اضافه کردن پیام کاربر به صف
            var request = new ChatRequest
            {
                RestaurantId = Guid.Parse(restaurantId),
                SessionId = sessionId,
                Message = message,
                ConnectionId = Context.ConnectionId
            };

            await _requestQueue.Writer.WriteAsync(request);

            // پردازش درخواست
            await ProcessChatRequest(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ارسال پیام برای رستوران {RestaurantId}", restaurantId);
            
            await Clients.Caller.SendAsync("ReceiveError", new
            {
                message = "خطا در ارسال پیام. لطفاً دوباره تلاش کنید.",
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// دریافت تاریخچه چت
    /// </summary>
    public async Task GetChatHistory(string restaurantId, string sessionId, int count = 20)
    {
        try
        {
            var restaurantGuid = Guid.Parse(restaurantId);
            
            var history = await _chatHistoryRepository.FindAsync(
                h => h.RestaurantId == restaurantGuid 
                    && h.SessionId == sessionId 
                    && !h.IsDeleted,
                CancellationToken.None);

            var messages = history
                .OrderByDescending(h => h.MessageTime)
                .Take(count)
                .Select(h => new
                {
                    message = h.IsUserMessage ? h.UserMessage : h.AiResponse,
                    isUserMessage = h.IsUserMessage,
                    timestamp = h.MessageTime
                })
                .Reverse()
                .ToList();

            await Clients.Caller.SendAsync("ReceiveHistory", messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت تاریخچه چت");
        }
    }

    #region Private Methods

    private async Task ProcessChatRequest(ChatRequest request)
    {
        try
        {
            // دریافت اطلاعات منو
            var menuContext = await BuildMenuContext(request.RestaurantId);

            // تولید پاسخ از AI
            var aiResponse = await _aiContentService.GenerateChatResponseAsync(
                request.RestaurantId,
                request.Message,
                menuContext);

            // ذخیره در تاریخچه
            await SaveChatHistory(request.RestaurantId, request.SessionId, request.Message, aiResponse);

            // ارسال پاسخ به کلاینت
            await Clients.Client(request.ConnectionId).SendAsync("ReceiveMessage", new
            {
                message = aiResponse,
                isUserMessage = false,
                timestamp = DateTime.UtcNow
            });

            // توقف وضعیت تایپ
            await Clients.Client(request.ConnectionId).SendAsync("TypingStopped");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در پردازش درخواست چت");
            
            await Clients.Client(request.ConnectionId).SendAsync("ReceiveError", new
            {
                message = "متاسفانه نتوانستم پاسخ دهم. لطفاً دوباره تلاش کنید.",
                timestamp = DateTime.UtcNow
            });
        }
    }

    private async Task<string> BuildMenuContext(Guid restaurantId)
    {
        try
        {
            var products = await _productRepository.FindAsync(
                p => p.RestaurantId == restaurantId && p.IsAvailable && !p.IsDeleted,
                CancellationToken.None);

            var orderedProducts = products
                .OrderBy(p => p.DisplayOrder)
                .Take(20)
                .ToList();

            if (!orderedProducts.Any())
            {
                return "هیچ محصولی در منو موجود نیست.";
            }

            var menuText = "محصولات موجود:\n\n";
            foreach (var product in orderedProducts)
            {
                menuText += $"- {product.Name}: {product.Description}\n";
                menuText += $"  قیمت: {product.Price:N0} تومان\n";
                menuText += "\n";
            }

            return menuText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ساخت context منو");
            return "اطلاعات منو در دسترس نیست.";
        }
    }

    private async Task SaveChatHistory(Guid restaurantId, string sessionId, string userMessage, string aiResponse)
    {
        try
        {
            // ذخیره پیام کاربر
            await _chatHistoryRepository.AddAsync(new ChatHistory
            {
                RestaurantId = restaurantId,
                SessionId = sessionId,
                UserMessage = userMessage,
                AiResponse = string.Empty,
                MessageTime = DateTime.UtcNow,
                IsUserMessage = true
            });

            // ذخیره پاسخ AI
            await _chatHistoryRepository.AddAsync(new ChatHistory
            {
                RestaurantId = restaurantId,
                SessionId = sessionId,
                UserMessage = string.Empty,
                AiResponse = aiResponse,
                MessageTime = DateTime.UtcNow,
                IsUserMessage = false
            });

            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ذخیره تاریخچه چت");
        }
    }

    #endregion

    private class ChatRequest
    {
        public Guid RestaurantId { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
    }
}
