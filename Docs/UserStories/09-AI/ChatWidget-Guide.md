# 💬 راهنمای استفاده از Chat Widget هوش مصنوعی

**نسخه:** 1.0  
**تاریخ:** 3 اکتبر 2025

---

## 🎯 معرفی

Chat Widget یک ابزار تعاملی است که به مشتریان رستوران اجازه می‌دهد با هوش مصنوعی درباره منو، محصولات و خدمات رستوران گفتگو کنند.

---

## 📍 محل نمایش

Chat Widget به صورت خودکار در صفحات زیر نمایش داده می‌شود:

- ✅ صفحه منوی عمومی رستوران (`/menu/{slug}`)
- ⏳ صفحه جزئیات محصول (قابل اضافه کردن)
- ⏳ صفحه رزرو (قابل اضافه کردن)

---

## 🎨 ظاهر Widget

### حالت بسته (پیش‌فرض)
```
┌─────────┐
│    💬   │  ← آیکن شناور در گوشه پایین چپ
└─────────┘
```

### حالت باز
```
┌─────────────────────────────────────┐
│  🤖 دستیار هوشمند        [آنلاین] ✕│
├─────────────────────────────────────┤
│                                      │
│  🤖 سلام! 👋                        │
│     من دستیار هوشمند رستوران هستم  │
│                                      │
│                  سلام، قابلیت‌ها چیه؟│
│                                      │
│  🤖 می‌توانم در مورد منو، محصولات   │
│     و رزرو میز به شما کمک کنم        │
│                                      │
│  • • •  (در حال تایپ...)           │
│                                      │
├─────────────────────────────────────┤
│  پیام خود را بنویسید...      [📤]  │
└─────────────────────────────────────┘
```

---

## 🔧 نصب و راه‌اندازی

### 1. افزودن به View

در فایل `.cshtml` خود، Component را فراخوانی کنید:

```razor
@* در انتهای صفحه، قبل از Scripts *@
@await Component.InvokeAsync("ChatWidget", Model.RestaurantId)
```

### 2. وابستگی‌ها

مطمئن شوید فایل‌های زیر لود شده‌اند:

```html
<!-- CSS -->
<link rel="stylesheet" href="~/css/ai-chat-widget.css" />

<!-- SignalR Library -->
<script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js"></script>
```

**نکته:** Component خودکار این فایل‌ها را include می‌کند.

---

## 📝 نحوه استفاده (مشتری)

### مرحله 1: باز کردن چت
1. به صفحه منوی رستوران بروید
2. آیکن 💬 را در گوشه پایین چپ صفحه پیدا کنید
3. روی آیکن کلیک کنید

### مرحله 2: ارسال پیام
1. در کادر "پیام خود را بنویسید..." تایپ کنید
2. Enter بزنید یا روی دکمه 📤 کلیک کنید
3. پیام شما نمایش داده می‌شود

### مرحله 3: دریافت پاسخ
1. نشانگر "در حال تایپ..." نمایش داده می‌شود
2. پاسخ AI ظاهر می‌شود
3. می‌توانید سوالات بیشتری بپرسید

### مرحله 4: بستن چت
- روی دکمه ✕ در گوشه بالا کلیک کنید
- یا دوباره روی آیکن 💬 کلیک کنید

---

## 💡 نمونه سوالات

### درباره منو
- "چه غذاهایی دارید؟"
- "محبوب‌ترین غذا چیه؟"
- "غذای گیاهی دارید؟"
- "پیتزا چند قیمته؟"

### درباره رستوران
- "ساعت کاری شما چیه؟"
- "آدرس رستوران کجاست؟"
- "میز خالی دارید؟"
- "چطور رزرو کنم؟"

### توصیه‌ها
- "چی پیشنهاد می‌کنید؟"
- "برای 4 نفر چی بخوریم؟"
- "غذای تند دارید؟"
- "دسر چی دارید؟"

---

## 🔧 تنظیمات (برای توسعه‌دهندگان)

### پارامترهای Component

```csharp
@await Component.InvokeAsync("ChatWidget", new
{
    restaurantId = Model.RestaurantId,
    // Optional parameters:
    theme = "light",           // light | dark
    position = "bottom-left",   // bottom-left | bottom-right
    maxHeight = 500,           // px
    maxWidth = 380             // px
})
```

### تغییر موقعیت Widget

در فایل `ai-chat-widget.css`:

```css
.ai-chat-widget {
    /* برای پایین راست: */
    right: 20px;
    left: auto;
    
    /* برای بالا چپ: */
    top: 20px;
    bottom: auto;
    left: 20px;
}
```

### تغییر رنگ‌ها

```css
:root {
    --chat-primary: #667eea;
    --chat-secondary: #764ba2;
    --chat-bg: #f5f5f5;
    --chat-user-bubble: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    --chat-ai-bubble: #ffffff;
}
```

### تغییر ابعاد

```css
.ai-chat-window {
    width: 400px;      /* عرض */
    height: 600px;     /* ارتفاع */
}
```

---

## 🎨 سفارشی‌سازی پیشرفته

### تغییر پیام خوش‌آمدگویی

در `Default.cshtml`:

```html
<div class="ai-chat-message ai-message">
    <div class="ai-chat-message-content">
        <p>به رستوران ما خوش آمدید! 🍽️</p>
        <p>چطور می‌تونم کمکتون کنم؟</p>
        <small>همین الان</small>
    </div>
</div>
```

### افزودن دکمه‌های پیشنهادی

```html
<div class="ai-chat-suggestions">
    <button class="suggestion-btn" data-message="منوی امروز چیه؟">
        منوی امروز 📋
    </button>
    <button class="suggestion-btn" data-message="چه غذاهای گیاهی دارید؟">
        غذای گیاهی 🌱
    </button>
    <button class="suggestion-btn" data-message="چطور رزرو کنم؟">
        رزرو میز 📅
    </button>
</div>

<script>
    document.querySelectorAll('.suggestion-btn').forEach(btn => {
        btn.addEventListener('click', function() {
            const message = this.getAttribute('data-message');
            chatInput.value = message;
            chatForm.dispatchEvent(new Event('submit'));
        });
    });
</script>
```

### افزودن صدا (اختیاری)

```javascript
function playNotificationSound() {
    const audio = new Audio('/sounds/notification.mp3');
    audio.play();
}

connection.on('ReceiveMessage', function(data) {
    playNotificationSound();
    addMessage(data.message, false);
});
```

---

## 🔌 API و Event ها

### JavaScript API

```javascript
// دسترسی به وضعیت اتصال
const isConnected = connection.state === signalR.HubConnectionState.Connected;

// ارسال پیام برنامه‌نویسی
await connection.invoke('SendMessage', restaurantId, sessionId, 'سلام');

// دریافت تاریخچه
await connection.invoke('GetChatHistory', restaurantId, sessionId, 50);

// قطع اتصال
await connection.stop();
```

### Event Listeners

```javascript
// پیام دریافت شد
connection.on('ReceiveMessage', (data) => {
    console.log('AI Response:', data.message);
});

// شروع تایپ
connection.on('TypingStarted', () => {
    console.log('AI is typing...');
});

// پایان تایپ
connection.on('TypingStopped', () => {
    console.log('AI stopped typing');
});

// خطا
connection.on('ReceiveError', (data) => {
    console.error('Chat Error:', data.message);
});

// تاریخچه دریافت شد
connection.on('ReceiveHistory', (messages) => {
    console.log('History:', messages);
});
```

---

## 🐛 عیب‌یابی

### مشکل: Widget نمایش داده نمی‌شود

**راه‌حل:**
1. مطمئن شوید CSS لود شده:
   ```html
   <link rel="stylesheet" href="~/css/ai-chat-widget.css" />
   ```
2. Component را صحیح فراخوانی کرده‌اید:
   ```razor
   @await Component.InvokeAsync("ChatWidget", Model.RestaurantId)
   ```

### مشکل: اتصال SignalR برقرار نمی‌شود

**راه‌حل:**
1. مطمئن شوید SignalR Hub در `Program.cs` map شده:
   ```csharp
   app.MapHub<AiAssistantHub>("/hubs/ai-assistant");
   ```
2. Console log را بررسی کنید:
   ```
   F12 > Console
   ```

### مشکل: پاسخ AI دریافت نمی‌شود

**راه‌حل:**
1. تنظیمات AI را بررسی کنید:
   ```
   /Restaurant/AiSettings/Index
   ```
2. Test Connection را انجام دهید
3. API Key و Base URL را چک کنید

### مشکل: CSS به درستی اعمال نمی‌شود

**راه‌حل:**
1. Cache مرورگر را پاک کنید (Ctrl+Shift+Delete)
2. فایل CSS را با `?v=` version کنید:
   ```html
   <link href="~/css/ai-chat-widget.css?v=1.0" rel="stylesheet" />
   ```

---

## 📱 Responsive Design

Widget به صورت خودکار برای موبایل بهینه شده است:

### Desktop (> 768px)
- عرض: 380px
- ارتفاع: 500px
- موقعیت: پایین چپ

### Mobile (< 768px)
- عرض: calc(100vw - 40px)
- ارتفاع: calc(100vh - 120px)
- موقعیت: تمام صفحه (با فاصله)

---

## 🔐 امنیت

### محدودیت‌ها
- ✅ نیاز به `RestaurantId` معتبر
- ✅ Rate limiting توسط SignalR
- ⚠️ Session محلی (پاک شدن با refresh)

### توصیه‌ها برای Production
1. افزودن Rate Limiting سمت سرور
2. محدود کردن تعداد پیام‌ها در هر session
3. فیلتر کردن محتوای نامناسب
4. لاگ‌گیری تمام مکالمات

---

## 📊 آمار عملکرد

### زمان پاسخ
- اتصال SignalR: < 1s
- ارسال پیام: < 500ms
- دریافت پاسخ AI: 2-10s (بستگی به API)

### استفاده از منابع
- CSS: 6 KB
- JavaScript: 4 KB
- SignalR: WebSocket connection

---

## 🚀 ویژگی‌های آینده

- [ ] پشتیبانی از فایل و تصویر
- [ ] Voice Input (تشخیص صدا)
- [ ] پیشنهاد محصولات با کارت
- [ ] ذخیره تاریخچه در دیتابیس
- [ ] پشتیبانی چندزبانه
- [ ] Dark Mode
- [ ] Emoji Picker
- [ ] Quick Replies

---

## 📚 منابع مفید

- [Microsoft SignalR Docs](https://learn.microsoft.com/en-us/aspnet/core/signalr/)
- [Semantic Kernel GitHub](https://github.com/microsoft/semantic-kernel)
- [OpenAI API Docs](https://platform.openai.com/docs/)

---

**آخرین به‌روزرسانی:** 3 اکتبر 2025  
**نگهداری:** EazyMenu Team  
**پشتیبانی:** support@eazymenu.ir
