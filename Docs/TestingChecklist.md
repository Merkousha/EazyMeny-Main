# 🧪 Reservation System - Testing Checklist

## 📅 تاریخ: 4 اکتبر 2025
## 🎯 هدف: تست کامل سیستم رزرو (Phase 2 - Task 4)

---

## 🔐 Pre-Test Setup

### داده‌های تست:
```
Admin:
- Email: admin@eazymenu.ir
- Password: Admin@123

Restaurant Owner:
- Email: owner@restaurant.com
- Password: Owner@123
- Restaurant: رستوران زیتون (Zeitoon)

Customer:
- Email: customer@test.com
- Password: Customer@123
```

### URLs:
- Application: http://localhost:5125
- Restaurant Slug: zeitoon

---

## ✅ Test Scenarios

### 1️⃣ Customer - Create Reservation (Public)

**URL:** `/Reservation/Reserve?restaurantId={guid}`

**Steps:**
- [ ] Navigate to Reserve page
- [ ] Fill form:
  - [ ] CustomerName: "علی محمدی"
  - [ ] CustomerPhone: "09121234567" (format validation)
  - [ ] CustomerEmail: "ali@test.com" (optional)
  - [ ] ReservationDate: Tomorrow's date (min: today)
  - [ ] ReservationTime: "19:00" (HH:mm format)
  - [ ] GuestsCount: Select "4" from dropdown (1-20)
  - [ ] SpecialRequests: "میز کنار پنجره لطفاً" (optional)
- [ ] Click "ثبت رزرو"

**Expected:**
- ✅ Form validation works (phone format, date min)
- ✅ Success message: "رزرو شما با موفقیت ثبت شد"
- ✅ SMS sent to customer (check Kavenegar panel or logs)
- ✅ Redirected to MyReservations or Details
- ✅ Reservation created with Status = Pending

**Actual Result:** _____________

---

### 2️⃣ Customer - View My Reservations

**URL:** `/Reservation/MyReservations`

**Steps:**
- [ ] Login as customer@test.com
- [ ] Navigate to MyReservations
- [ ] Check table displays reservation
- [ ] Verify summary cards show counts

**Expected:**
- ✅ Table shows: ReservationNumber, Restaurant, Date, Time, Guests, Table, Status
- ✅ Status badge: "در انتظار تایید" (warning color)
- ✅ Action buttons: "جزئیات" (eye), "لغو رزرو" (x)
- ✅ Summary cards: Pending = 1, others = 0
- ✅ Empty state if no reservations: Icon + "رزرویی یافت نشد"

**Actual Result:** _____________

---

### 3️⃣ Customer - View Reservation Details

**URL:** `/Reservation/Details/{id}`

**Steps:**
- [ ] Click "جزئیات" button from MyReservations
- [ ] Verify all info displayed

**Expected:**
- ✅ Reservation Number badge (blue)
- ✅ Status badge (Pending = warning)
- ✅ Restaurant info: Name, Phone, Address
- ✅ Customer info: Name, Phone, Email
- ✅ Reservation details: Date, Time, Guests, Table (تخصیص نیافته)
- ✅ Special requests displayed
- ✅ Timeline: Created timestamp
- ✅ "لغو رزرو" button visible (if Pending/Confirmed & not past)
- ✅ "بازگشت به لیست" button

**Actual Result:** _____________

---

### 4️⃣ Restaurant Owner - View Reservations Dashboard

**URL:** `/Restaurant/Reservation/Index?restaurantId={guid}`

**Steps:**
- [ ] Login as owner@restaurant.com
- [ ] Navigate to Reservation Index (via sidebar or direct URL)
- [ ] Check filters work
- [ ] Check stats cards

**Expected:**
- ✅ Stats cards (Small Boxes):
  - Pending = 1 (warning)
  - Confirmed = 0 (success)
  - Completed = 0 (primary)
  - Cancelled = 0 (danger)
- ✅ Filters: FromDate, ToDate, Status dropdown
- ✅ Table shows reservation with 9 columns
- ✅ Action buttons: "جزئیات" (eye), "تایید" (check - only Pending), "لغو" (x)
- ✅ "نمای تقویم" button (link to Calendar)

**Actual Result:** _____________

---

### 5️⃣ Restaurant Owner - Filter Reservations

**Steps:**
- [ ] Test FromDate filter: Select tomorrow → Submit
- [ ] Test ToDate filter: Select next week → Submit
- [ ] Test Status filter: Select "Pending" → Submit
- [ ] Test Clear filters (X button)

**Expected:**
- ✅ Filters persist in ViewBag (show in inputs)
- ✅ Table updates based on filters
- ✅ URL params update: ?fromDate=...&toDate=...&status=...
- ✅ Clear filters returns to Index without params

**Actual Result:** _____________

---

### 6️⃣ Restaurant Owner - View Reservation Details

**URL:** `/Restaurant/Reservation/Details/{id}`

**Steps:**
- [ ] Click "جزئیات" from Index table
- [ ] Verify AdminLTE layout
- [ ] Check all sections

**Expected:**
- ✅ Breadcrumbs: رزروها > جزئیات
- ✅ Reservation Number badge (alert-info)
- ✅ Info Boxes (4): Date, Time, Guests, Table (با icons)
- ✅ Customer info table: Name, Phone, Email
- ✅ Special requests (if any)
- ✅ Timeline sidebar: Created → (states)
- ✅ Restaurant info card: Name, Address, Phone
- ✅ Action buttons: "تایید رزرو" (if Pending), "لغو رزرو"
- ✅ "بازگشت" button to Index

**Actual Result:** _____________

---

### 7️⃣ Restaurant Owner - Confirm Reservation

**Steps:**
- [ ] From Index or Details, click "تایید" button
- [ ] Modal opens: "تایید رزرو"
- [ ] Enter TableNumber: "5" (optional)
- [ ] Click "تایید رزرو" (green button)

**Expected:**
- ✅ Modal closes
- ✅ Success message: "رزرو با موفقیت تایید شد"
- ✅ Status changes: Pending → Confirmed
- ✅ TableNumber saved: میز 5
- ✅ SMS sent to customer: "رزرو شما... تایید شد. میز شماره: 5"
- ✅ ReminderSent = false (for future job)
- ✅ "تایید" button disappears
- ✅ "لغو" button still visible

**Actual Result:** _____________

---

### 8️⃣ Customer - View Confirmed Reservation

**Steps:**
- [ ] Customer navigates to MyReservations
- [ ] Clicks "جزئیات" on confirmed reservation

**Expected:**
- ✅ Status badge: "تایید شده" (success color)
- ✅ Table number: "میز 5" (info badge)
- ✅ Timeline: Created → Confirmed (check icon)
- ✅ "لغو رزرو" button still visible (customer can cancel confirmed)

**Actual Result:** _____________

---

### 9️⃣ Customer - Cancel Reservation

**Steps:**
- [ ] From Details, click "لغو رزرو" button
- [ ] Modal opens
- [ ] Enter cancellation reason: "تغییر برنامه"
- [ ] Click "لغو رزرو" (red button)

**Expected:**
- ✅ Modal closes
- ✅ Success message: "رزرو شما لغو شد"
- ✅ Status changes: Confirmed → Cancelled
- ✅ CancellationReason saved
- ✅ CancelledAt timestamp set
- ✅ SMS sent to restaurant: "رزرو... لغو شد"
- ✅ "لغو رزرو" button disappears
- ✅ Timeline: Created → Confirmed → Cancelled (with reason)

**Actual Result:** _____________

---

### 🔟 Restaurant Owner - Cancel Reservation

**Steps:**
- [ ] Create new reservation (repeat Step 1)
- [ ] Owner views in Index
- [ ] Click "لغو" button
- [ ] Modal: Enter reason: "رستوران تعطیل است" (required)
- [ ] Submit

**Expected:**
- ✅ Reason is required (validation)
- ✅ Success message
- ✅ Status: Cancelled
- ✅ SMS sent to customer with reason
- ✅ Action buttons disappear

**Actual Result:** _____________

---

### 1️⃣1️⃣ Past Reservations (Read-Only)

**Steps:**
- [ ] Create reservation for yesterday (manual DB edit or wait)
- [ ] View as customer & owner

**Expected:**
- ✅ Customer Details: No "لغو رزرو" button
- ✅ Owner Index: No "تایید" or "لغو" buttons
- ✅ Table row: class="table-secondary" (greyed out)
- ✅ Status: Completed or NoShow (manual change)

**Actual Result:** _____________

---

### 1️⃣2️⃣ Validation Tests

**Test Invalid Inputs:**
- [ ] Phone: "0912" (too short) → Error: "شماره تلفن باید 11 رقم باشد"
- [ ] Phone: "1234567890" (doesn't start with 09) → Error
- [ ] Date: Yesterday → HTML5 min validation
- [ ] GuestsCount: 0 or 21 → Dropdown limited (1-20)
- [ ] Empty fields → Required validation messages
- [ ] Cancel without reason (restaurant) → Required validation

**Expected:**
- ✅ All validations work client-side (HTML5 + jQuery)
- ✅ All validations work server-side (FluentValidation)
- ✅ Persian error messages

**Actual Result:** _____________

---

### 1️⃣3️⃣ Edge Cases

**Test Scenarios:**
- [ ] Double confirm (click تایید twice) → Should fail (already confirmed)
- [ ] Cancel already cancelled → Button hidden
- [ ] Confirm past reservation → Button hidden
- [ ] Customer cancel others' reservation → Access denied (check auth)
- [ ] Owner manage other restaurants' reservations → Should filter by restaurantId

**Expected:**
- ✅ Business rules enforced
- ✅ No crashes or exceptions
- ✅ Appropriate error messages

**Actual Result:** _____________

---

### 1️⃣4️⃣ UI/UX Tests

**Responsive Design:**
- [ ] Mobile (375px): Forms & tables responsive
- [ ] Tablet (768px): 2-column layouts work
- [ ] Desktop (1200px): Full layout

**RTL Support:**
- [ ] All text right-aligned
- [ ] Buttons in correct position
- [ ] Modals RTL
- [ ] Forms RTL

**AdminLTE Components:**
- [ ] Small boxes (stats) display correctly
- [ ] Info boxes (date, time, etc.) styled
- [ ] Timeline sidebar renders
- [ ] Cards have proper shadows
- [ ] Sidebar navigation active state

**Expected:**
- ✅ Fully responsive
- ✅ Persian RTL support
- ✅ Bootstrap Icons render
- ✅ No layout breaks

**Actual Result:** _____________

---

### 1️⃣5️⃣ SMS Integration (Optional - if Kavenegar configured)

**Check SMS Logs:**
- [ ] Create reservation → SMS to customer
- [ ] Confirm reservation → SMS to customer (with table)
- [ ] Cancel (customer) → SMS to restaurant
- [ ] Cancel (restaurant) → SMS to customer (with reason)

**Expected:**
- ✅ ISmsService.SendAsync called
- ✅ Persian message template
- ✅ Correct recipient phone number
- ✅ If mock: Check logs (console output)
- ✅ If real: Check Kavenegar panel

**Actual Result:** _____________

---

## 🐛 Bugs Found

### Bug #1:
- **Severity:** Critical / Major / Minor
- **Description:** _____________
- **Steps to Reproduce:** _____________
- **Expected:** _____________
- **Actual:** _____________
- **Fix:** _____________

### Bug #2:
- **Severity:** _____________
- **Description:** _____________
- **Fix:** _____________

---

## ✨ Improvement Suggestions

### UI/UX:
- [ ] _____________
- [ ] _____________

### Performance:
- [ ] _____________
- [ ] _____________

### Features:
- [ ] _____________
- [ ] _____________

---

## 📊 Test Summary

**Total Test Cases:** 15  
**Passed:** _____ / 15  
**Failed:** _____ / 15  
**Bugs Found:** _____  
**Critical Bugs:** _____  

**Overall Status:** ✅ Pass / ⚠️ Pass with Issues / ❌ Fail

**Tested By:** _____________  
**Date:** 4 اکتبر 2025  
**Duration:** _____ minutes  

---

## 🎯 Sign-Off

- [ ] All critical bugs fixed
- [ ] All test cases passed
- [ ] Documentation updated
- [ ] Ready for production

**Approved By:** _____________  
**Date:** _____________

---

## 📝 Notes

_____________
_____________
_____________
