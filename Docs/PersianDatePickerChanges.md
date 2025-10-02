# تغییرات لازم برای Date Picker شمسی

## فایل: Calendar.cshtml

### 1. اضافه کردن Styles Section (بعد از Layout):

```cshtml
@section Styles {
    <link href="~/lib/multi-calendar-datepicker/multi-calendar-datepicker.css" rel="stylesheet" />
}
```

### 2. تغییر Input تاریخ (در قسمت "انتخاب تاریخ"):

**قبل:**
```cshtml
<input type="date" id="selectedDate" class="form-control" 
       value="@ViewBag.SelectedDate.ToString("yyyy-MM-dd")" />
```

**بعد:**
```cshtml
<input type="text" id="selectedDate" class="form-control" 
       placeholder="1403/07/12" readonly />
<input type="hidden" id="selectedDateGregorian" 
       value="@ViewBag.SelectedDate.ToString("yyyy-MM-dd")" />
```

### 3. تغییر Scripts Section (کل section را جایگزین کنید):

```cshtml
@section Scripts {
    <script src="~/lib/multi-calendar-datepicker/moment-bundled.js"></script>
    <script src="~/lib/multi-calendar-datepicker/multi-calendar-datepicker.js"></script>
    <script>
        const slug = '@ViewBag.RestaurantSlug';
        const selectedDateInput = document.getElementById('selectedDate');
        const selectedDateGregorian = document.getElementById('selectedDateGregorian');
        const displayDate = document.getElementById('displayDate');
        const loadingSpinner = document.getElementById('loadingSpinner');
        const reservationsTable = document.getElementById('reservationsTable');

        // Initialize Persian Date Picker
        $(document).ready(function() {
            $('#selectedDate').multiCalendarDatePicker({
                calendar: 'persian',
                locale: 'fa',
                format: 'YYYY/MM/DD',
                rtl: true,
                theme: 'light',
                todayHighlight: true,
                showToday: true,
                showClear: false,
                autoClose: true
            });
            
            // Set initial date to today (Persian)
            const today = moment();
            const persianDate = today.format('jYYYY/jMM/jDD');
            $('#selectedDate').val(persianDate);
            $('#selectedDateGregorian').val(today.format('YYYY-MM-DD'));
            if (displayDate) displayDate.textContent = persianDate;
            
            // Load today's reservations
            loadReservations(today.format('YYYY-MM-DD'));
            
            // Handle date change from picker
            $('#selectedDate').on('mcdp:change', function(e, data) {
                const persianParts = data.formatted.split('/');
                const gregorianMoment = moment(`${persianParts[0]}/${persianParts[1]}/${persianParts[2]}`, 'jYYYY/jMM/jDD');
                $('#selectedDateGregorian').val(gregorianMoment.format('YYYY-MM-DD'));
                if (displayDate) displayDate.textContent = data.formatted;
            });
        });

        // تابع بارگذاری رزروها
        async function loadReservations(date) {
            loadingSpinner.style.display = 'block';
            reservationsTable.style.display = 'none';

            try {
                const response = await fetch(`/Restaurant/Reservation/Calendar/${slug}/GetByDate?date=${date}`);
                const result = await response.json();

                if (result.success) {
                    updateReservationsTable(result.data);
                    updateStats(result.data);
                } else {
                    alert(result.message || 'خطا در دریافت اطلاعات');
                }
            } catch (error) {
                console.error('Error loading reservations:', error);
                alert('خطا در بارگذاری رزروها');
            } finally {
                loadingSpinner.style.display = 'none';
                reservationsTable.style.display = 'block';
            }
        }

        // تابع به‌روزرسانی جدول
        function updateReservationsTable(reservations) {
            if (reservations.length === 0) {
                reservationsTable.innerHTML = `
                    <div class="text-center py-5">
                        <i class="bi bi-calendar-x display-1 text-muted"></i>
                        <p class="text-muted mt-3">رزرویی برای این تاریخ یافت نشد</p>
                    </div>
                `;
                return;
            }

            reservations.sort((a, b) => a.reservationTime.localeCompare(b.reservationTime));

            let tableHtml = `
                <table class="table table-hover">
                    <thead class="table-light">
                        <tr>
                            <th>ساعت</th>
                            <th>شماره رزرو</th>
                            <th>مشتری</th>
                            <th>تلفن</th>
                            <th>تعداد نفرات</th>
                            <th>میز</th>
                            <th>وضعیت</th>
                            <th>عملیات</th>
                        </tr>
                    </thead>
                    <tbody>
            `;

            reservations.forEach(r => {
                tableHtml += `
                    <tr>
                        <td><strong dir="ltr">${r.formattedTime}</strong></td>
                        <td>${r.reservationNumber}</td>
                        <td>${r.customerName}</td>
                        <td dir="ltr">${r.customerPhone}</td>
                        <td><span class="badge bg-info">${r.guestsCount} نفر</span></td>
                        <td>${r.tableNumber ? `<span class="badge bg-secondary">میز ${r.tableNumber}</span>` : '<span class="text-muted">-</span>'}</td>
                        <td><span class="${r.statusBadgeClass}">${r.statusDisplay}</span></td>
                        <td>
                            <a href="/Restaurant/Reservation/Details/${r.id}" class="btn btn-sm btn-outline-primary">
                                <i class="bi bi-eye"></i>
                            </a>
                        </td>
                    </tr>
                `;
            });

            tableHtml += '</tbody></table>';
            reservationsTable.innerHTML = tableHtml;
        }

        // تابع به‌روزرسانی آمار
        function updateStats(reservations) {
            document.getElementById('totalCount').textContent = reservations.length;
            document.getElementById('pendingCount').textContent = reservations.filter(r => r.status === 0).length;
            document.getElementById('confirmedCount').textContent = reservations.filter(r => r.status === 1).length;
            document.getElementById('totalGuests').textContent = reservations.reduce((sum, r) => sum + r.guestsCount, 0);
        }

        // Event listeners - استفاده از تاریخ میلادی hidden
        document.getElementById('btnLoadDate').addEventListener('click', () => {
            const date = selectedDateGregorian.value;
            if (date) {
                loadReservations(date);
            }
        });

        document.getElementById('btnToday').addEventListener('click', () => {
            const today = moment();
            const persianDate = today.format('jYYYY/jMM/jDD');
            $('#selectedDate').val(persianDate);
            $('#selectedDateGregorian').val(today.format('YYYY-MM-DD'));
            if (displayDate) displayDate.textContent = persianDate;
            loadReservations(today.format('YYYY-MM-DD'));
        });

        document.getElementById('btnPrevDay').addEventListener('click', () => {
            const gregorianDate = moment(selectedDateGregorian.value);
            const prevDay = gregorianDate.subtract(1, 'days');
            const persianDate = prevDay.format('jYYYY/jMM/jDD');
            $('#selectedDate').val(persianDate);
            $('#selectedDateGregorian').val(prevDay.format('YYYY-MM-DD'));
            if (displayDate) displayDate.textContent = persianDate;
            loadReservations(prevDay.format('YYYY-MM-DD'));
        });

        document.getElementById('btnNextDay').addEventListener('click', () => {
            const gregorianDate = moment(selectedDateGregorian.value);
            const nextDay = gregorianDate.add(1, 'days');
            const persianDate = nextDay.format('jYYYY/jMM/jDD');
            $('#selectedDate').val(persianDate);
            $('#selectedDateGregorian').val(nextDay.format('YYYY-MM-DD'));
            if (displayDate) displayDate.textContent = persianDate;
            loadReservations(nextDay.format('YYYY-MM-DD'));
        });

        // Enter key on date input
        selectedDateInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                document.getElementById('btnLoadDate').click();
            }
        });
    </script>
}
```

---

## نتیجه:

✅ Date Picker شمسی با امکانات:
- انتخاب تاریخ شمسی (1403/07/12)
- تبدیل خودکار به میلادی برای ارسال به سرور
- دکمه‌های "امروز"، "روز قبل"، "روز بعد" با تاریخ شمسی
- نمایش رزروها بر اساس تاریخ انتخاب شده
- RTL support کامل
- تم روشن و تیره

🔗 GitHub Repo: https://github.com/delphiassistant/MultiCalendarDatePicker
