-- بررسی تعداد پلن‌های موجود در جدول SubscriptionPlans
SELECT COUNT(*) AS TotalPlans FROM SubscriptionPlans;

-- نمایش تمام پلن‌ها
SELECT 
    Id,
    PlanType,
    Name,
    PriceMonthly,
    PriceYearly,
    MaxProducts,
    IsActive,
    IsPopular
FROM SubscriptionPlans
ORDER BY DisplayOrder;
