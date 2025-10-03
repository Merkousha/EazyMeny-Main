# 🤖 پیاده‌سازی User Story 16: هوش مصنوعی در مدیریت و پشتیبانی منو

**تاریخ:** 3 اکتبر 2025  
**وضعیت:** ✅ پیاده‌سازی شده  
**اولویت:** بالا | **فاز:** 2

---

## 📋 خلاصه

User Story 16 با موفقیت پیاده‌سازی شد که شامل قابلیت‌های زیر است:

1. ✅ تولید توضیحات محصول با هوش مصنوعی
2. ✅ تولید تصویر محصول (پایه پیاده‌سازی شده - نیاز به API تصویر)
3. ✅ تنظیمات Semantic Kernel
4. ✅ چت تعاملی با SignalR

---

## 🏗️ ساختار فایل‌ها

### Domain Layer
```
Domain/Entities/
├── AiSettings.cs         # تنظیمات هوش مصنوعی
└── ChatHistory.cs        # تاریخچه گفت‌وگو
```

### Application Layer
```
Application/
├── Common/
│   ├── Interfaces/
│   │   ├── IAiSettingsProvider.cs
│   │   └── IAiContentService.cs
│   └── Models/AI/
│       └── AiModels.cs
├── Features/AI/
│   ├── Commands/
│   │   ├── GenerateProductContent/
│   │   │   ├── GenerateProductContentCommand.cs
│   │   │   ├── GenerateProductContentCommandHandler.cs
│   │   │   └── GenerateProductContentCommandValidator.cs
│   │   ├── GenerateProductImage/
│   │   │   ├── GenerateProductImageCommand.cs
│   │   │   ├── GenerateProductImageCommandHandler.cs
│   │   │   └── GenerateProductImageCommandValidator.cs
│   │   ├── SaveAiSettings/
│   │   │   ├── SaveAiSettingsCommand.cs
│   │   │   ├── SaveAiSettingsCommandHandler.cs
│   │   │   └── SaveAiSettingsCommandValidator.cs
│   │   └── TestAiConnection/
│   │       ├── TestAiConnectionCommand.cs
│   │       └── TestAiConnectionCommandHandler.cs
│   └── Queries/
│       └── GetAiSettings/
│           ├── GetAiSettingsQuery.cs
│           └── GetAiSettingsQueryHandler.cs
```

### Infrastructure Layer
```
Infrastructure/
├── Data/Configurations/
│   ├── AiSettingsConfiguration.cs
│   └── ChatHistoryConfiguration.cs
├── Services/
│   ├── AiSettingsProvider.cs
│   └── AiContentService.cs
└── Hubs/
    └── AiAssistantHub.cs
```

### Web Layer
```
Web/
├── Controllers/Api/
│   └── AiController.cs
└── Areas/Restaurant/
    ├── Controllers/
    │   └── AiSettingsController.cs
    └── Views/AiSettings/
        └── Index.cshtml
```

---

## 📦 پکیج‌های نصب شده

```xml
<!-- EazyMenu.Application -->
<PackageReference Include="Microsoft.SemanticKernel" Version="1.30.0" />
<PackageReference Include="Microsoft.AspNetCore.SignalR" Version="1.1.0" />
```

---

## 🗄️ تغییرات دیتابیس

### Migration: `20251003093004_AddAiEntities`

#### جدول AiSettings
```sql
CREATE TABLE [AiSettings] (
    [Id] uniqueidentifier PRIMARY KEY,
    [RestaurantId] uniqueidentifier NOT NULL,
    [BaseUrl] nvarchar(500) NOT NULL,
    [ApiKey] nvarchar(500) NOT NULL,
    [ModelName] nvarchar(100) NOT NULL,
    [TimeoutSeconds] int DEFAULT 30,
    [IsActive] bit DEFAULT 1,
    [Environment] nvarchar(50) DEFAULT 'Production',
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit DEFAULT 0,
    CONSTRAINT [FK_AiSettings_Restaurants] 
        FOREIGN KEY ([RestaurantId]) 
        REFERENCES [Restaurants]([Id]) 
        ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_AiSettings_RestaurantId] 
    ON [AiSettings]([RestaurantId]) 
    WHERE [IsDeleted] = 0;

CREATE INDEX [IX_AiSettings_IsActive] 
    ON [AiSettings]([IsActive]);
```

#### جدول ChatHistories
```sql
CREATE TABLE [ChatHistories] (
    [Id] uniqueidentifier PRIMARY KEY,
    [RestaurantId] uniqueidentifier NOT NULL,
    [SessionId] nvarchar(100) NOT NULL,
    [UserMessage] nvarchar(2000) NOT NULL,
    [AiResponse] nvarchar(2000) NOT NULL,
    [MessageTime] datetime2 NOT NULL,
    [IsUserMessage] bit DEFAULT 1,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit DEFAULT 0,
    CONSTRAINT [FK_ChatHistories_Restaurants] 
        FOREIGN KEY ([RestaurantId]) 
        REFERENCES [Restaurants]([Id]) 
        ON DELETE CASCADE
);

CREATE INDEX [IX_ChatHistories_RestaurantId] 
    ON [ChatHistories]([RestaurantId]);

CREATE INDEX [IX_ChatHistories_SessionId] 
    ON [ChatHistories]([SessionId]);

CREATE INDEX [IX_ChatHistories_MessageTime] 
    ON [ChatHistories]([MessageTime]);

CREATE INDEX [IX_ChatHistories_RestaurantId_SessionId_MessageTime] 
    ON [ChatHistories]([RestaurantId], [SessionId], [MessageTime]);
```

---

## 🔌 API Endpoints

### REST API

```
POST   /api/ai/menu-items/{id}/generate-content
POST   /api/ai/menu-items/{id}/generate-image
GET    /api/ai/settings
PUT    /api/ai/settings
POST   /api/ai/settings/test-connection
```

### SignalR Hub

```
Hub: /hubs/ai-assistant

Methods:
- JoinRestaurantChat(restaurantId)
- SendMessage(restaurantId, sessionId, message)
- GetChatHistory(restaurantId, sessionId, count)

Events:
- ReceiveMessage(message)
- ReceiveHistory(messages)
- ReceiveError(error)
- TypingStarted()
- TypingStopped()
```

---

## 🎯 نحوه استفاده

### 1. تنظیمات هوش مصنوعی

مدیر رستوران باید ابتدا به صفحه تنظیمات برود:

```
/Restaurant/AiSettings
```

و موارد زیر را وارد کند:

- **Base URL**: آدرس API (مثلاً `https://api.openai.com/v1`)
- **API Key**: کلید API
- **Model Name**: نام مدل (مثلاً `gpt-4`)
- **Timeout**: زمان انقضا (پیش‌فرض 30 ثانیه)
- **Environment**: Production یا Development

### 2. تولید محتوای محصول

در صفحه ایجاد یا ویرایش محصول:

```
/Restaurant/Product/Create
/Restaurant/Product/Edit/{id}
```

1. نام محصول را وارد کنید
2. روی دکمه "تولید با AI" کلیک کنید
3. لحن محتوا را انتخاب کنید (صمیمی، رسمی، خلاقانه)
4. مواد تشکیل‌دهنده را وارد کنید (اختیاری)
5. روی "تولید" کلیک کنید
6. محتوای تولید شده را بررسی کنید
7. روی "اعمال محتوا" کلیک کنید
8. در صورت نیاز ویرایش کنید
9. محصول را ذخیره کنید

**درخواست API:**
```javascript
POST /Restaurant/Product/GenerateContent
{
    "productId": "guid",
    "productName": "کباب کوبیده",
    "ingredients": "گوشت چرخ کرده، پیاز، ادویه",
    "tone": "صمیمی"
}
```

**پاسخ:**
```json
{
    "success": true,
    "data": {
        "title": "کباب کوبیده سنتی",
        "shortDescription": "کباب کوبیده عصرانه با گوشت تازه",
        "longDescription": "کباب کوبیده ما با گوشت تازه گوسفند...",
        "keywords": ["کباب", "ایرانی", "سنتی"]
    }
}
```

### 3. چت تعاملی با AI

در صفحه منوی عمومی رستوران:

```
/menu/{restaurant-slug}
```

**مراحل استفاده:**

