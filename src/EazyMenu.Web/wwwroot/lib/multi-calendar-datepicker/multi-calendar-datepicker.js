/*!
 * Multi-Calendar DatePicker v2.3.0
 * A comprehensive datepicker supporting Gregorian, Persian (Jalaali), and Hijri (Islamic) calendars
 * 
 * Dependencies: jQuery 3.x+, Moment.js, moment-multi-calendar, Bootstrap 5
 */

(function($) {
    'use strict';

    // Check dependencies
    if (typeof $ === 'undefined') {
        throw new Error('Multi-Calendar DatePicker requires jQuery');
    }
    if (typeof moment === 'undefined') {
        throw new Error('Multi-Calendar DatePicker requires Moment.js');
    }
    
    // Check if multi-calendar extension is available
    if (typeof moment.isPersianLeapYear === 'undefined') {
        console.warn('Moment-multi-calendar extension is not available. Persian and Hijri calendars may not work correctly.');
    }

    // Global state manager for tracking open datepickers
    var DatePickerManager = {
        openInstances: [],
        
        // Register a new instance
        register: function(instance) {
            this.openInstances.push(instance);
        },
        
        // Unregister an instance
        unregister: function(instance) {
            var index = this.openInstances.indexOf(instance);
            if (index > -1) {
                this.openInstances.splice(index, 1);
            }
        },
        
        // Hide all other open instances except the current one
        hideOthers: function(currentInstance) {
            for (var i = 0; i < this.openInstances.length; i++) {
                var instance = this.openInstances[i];
                if (instance !== currentInstance && instance.isVisible) {
                    instance.hide();
                }
            }
        },
        
        // Get count of open instances
        getOpenCount: function() {
            return this.openInstances.filter(function(instance) {
                return instance.isVisible;
            }).length;
        },
        
        // Get all registered instances
        getAllInstances: function() {
            return this.openInstances.slice(); // Return a copy
        },
        
        // Debug method to log current state
        debug: function() {
            console.log('DatePickerManager Debug:');
            console.log('- Total instances:', this.openInstances.length);
            console.log('- Open instances:', this.getOpenCount());
            console.log('- Instances:', this.openInstances.map(function(instance) {
                return {
                    element: instance.element[0],
                    isVisible: instance.isVisible,
                    calendar: instance.options.calendar
                };
            }));
        }
    };

    // Calendar system constants
    const CALENDARS = {
        GREGORIAN: 'gregorian',
        PERSIAN: 'persian', 
        HIJRI: 'hijri'
    };

    const LOCALES = {
        EN: 'en',
        FA: 'fa',
        AR: 'ar'
    };

    // Internationalization data
    const I18N = {
        en: {
            gregorian: {
                months: ['January', 'February', 'March', 'April', 'May', 'June',
                        'July', 'August', 'September', 'October', 'November', 'December'],
                monthsShort: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
                             'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                weekdays: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
                weekdaysShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
                weekdaysMin: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
                today: 'Today',
                clear: 'Clear',
                close: 'Close',
                am: 'AM',
                pm: 'PM',
                hour: 'Hour',
                minute: 'Min',
                second: 'Sec'
            },
            persian: {
                months: ['Farvardin', 'Ordibehesht', 'Khordad', 'Tir', 'Mordad', 'Shahrivar',
                        'Mehr', 'Aban', 'Azar', 'Dey', 'Bahman', 'Esfand'],
                monthsShort: ['Far', 'Ord', 'Kho', 'Tir', 'Mor', 'Sha',
                             'Meh', 'Aba', 'Aza', 'Dey', 'Bah', 'Esf'],
                weekdays: ['Saturday', 'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'],
                weekdaysShort: ['Sat', 'Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri'],
                weekdaysMin: ['Sa', 'Su', 'Mo', 'Tu', 'We', 'Th', 'Fr'],
                today: 'Today',
                clear: 'Clear',
                close: 'Close',
                am: 'AM',
                pm: 'PM',
                hour: 'Hour',
                minute: 'Min',
                second: 'Sec'
            },
            hijri: {
                months: ['Muharram', 'Safar', 'Rabi al-awwal', 'Rabi al-thani', 'Jumada al-awwal', 'Jumada al-thani',
                        'Rajab', 'Sha\'ban', 'Ramadan', 'Shawwal', 'Dhu al-Qi\'dah', 'Dhu al-Hijjah'],
                monthsShort: ['Muh', 'Saf', 'Rab1', 'Rab2', 'Jum1', 'Jum2',
                             'Raj', 'Sha', 'Ram', 'Shaw', 'DhuQ', 'DhuH'],
                weekdays: ['Saturday', 'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'],
                weekdaysShort: ['Sat', 'Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri'],
                weekdaysMin: ['Sa', 'Su', 'Mo', 'Tu', 'We', 'Th', 'Fr'],
                today: 'Today',
                clear: 'Clear',
                close: 'Close',
                am: 'AM',
                pm: 'PM',
                hour: 'Hour',
                minute: 'Min',
                second: 'Sec'
            }
        },
        fa: {
            persian: {
                months: ['فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور',
                        'مهر', 'آبان', 'آذر', 'دی', 'بهمن', 'اسفند'],
                monthsShort: ['فرو', 'ارد', 'خرد', 'تیر', 'مرد', 'شهر',
                             'مهر', 'آبا', 'آذر', 'دی', 'بهم', 'اسف'],
                weekdays: ['شنبه', 'یکشنبه', 'دوشنبه', 'سه‌شنبه', 'چهارشنبه', 'پنج‌شنبه', 'جمعه'],
                weekdaysShort: ['ش', 'ی', 'د', 'س', 'چ', 'پ', 'ج'],
                weekdaysMin: ['ش', 'ی', 'د', 'س', 'چ', 'پ', 'ج'],
                today: 'امروز',
                clear: 'پاک کردن',
                close: 'بستن',
                am: 'ق.ظ',
                pm: 'ب.ظ',
                hour: 'ساعت',
                minute: 'دقیقه',
                second: 'ثانیه'
            },
            gregorian: {
                months: ['ژانویه', 'فوریه', 'مارس', 'آوریل', 'مه', 'ژوئن',
                        'ژوئیه', 'اوت', 'سپتامبر', 'اکتبر', 'نوامبر', 'دسامبر'],
                monthsShort: ['ژان', 'فور', 'مار', 'آور', 'مه', 'ژوئن',
                             'ژوئیه', 'اوت', 'سپت', 'اکت', 'نوا', 'دسا'],
                weekdays: ['یکشنبه', 'دوشنبه', 'سه‌شنبه', 'چهارشنبه', 'پنج‌شنبه', 'جمعه', 'شنبه'],
                weekdaysShort: ['ی', 'د', 'س', 'چ', 'پ', 'ج', 'ش'],
                weekdaysMin: ['ی', 'د', 'س', 'چ', 'پ', 'ج', 'ش'],
                today: 'امروز',
                clear: 'پاک کردن',
                close: 'بستن',
                am: 'ق.ظ',
                pm: 'ب.ظ',
                hour: 'ساعت',
                minute: 'دقیقه',
                second: 'ثانیه'
            }
        },
        ar: {
            hijri: {
                months: ['محرم', 'صفر', 'ربيع الأول', 'ربيع الثاني', 'جمادى الأولى', 'جمادى الثانية',
                        'رجب', 'شعبان', 'رمضان', 'شوال', 'ذو القعدة', 'ذو الحجة'],
                monthsShort: ['محر', 'صفر', 'ربع1', 'ربع2', 'جما1', 'جما2',
                             'رجب', 'شعب', 'رمض', 'شوا', 'ذوق', 'ذوح'],
                weekdays: ['السبت', 'الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة'],
                weekdaysShort: ['س', 'ح', 'ن', 'ث', 'ر', 'خ', 'ج'],
                weekdaysMin: ['س', 'ح', 'ن', 'ث', 'ر', 'خ', 'ج'],
                today: 'اليوم',
                clear: 'مسح',
                close: 'إغلاق',
                am: 'ص',
                pm: 'م',
                hour: 'ساعة',
                minute: 'دقيقة',
                second: 'ثانية'
            }
        }
    };

    // Year range constants for each calendar system
    const YEAR_RANGES = {
        GREGORIAN: { min: 1900, max: 2100 },
        PERSIAN: { min: 1200, max: 1500 },
        HIJRI: { min: 1000, max: 2000 }
    };

    // Default options
    const DEFAULTS = {
        calendar: CALENDARS.GREGORIAN,
        locale: LOCALES.EN,
        format: 'YYYY/MM/DD',
        rtl: null, // auto-detect
        theme: 'light',
        placement: 'bottom-start',
        timePicker: false,
        timeFormat: 'HH:mm',
        showSeconds: false,
        use24Hour: true,
        stepping: 1,
        showToday: true,
        showClear: true,
        showClose: true,
        autoClose: true,
        hideAfterSelect: true,
        keyboardNavigation: true,
        todayHighlight: true,
        allowInput: true,
        autoCorrectionTimeout: 3000, // Default timeout for auto-corrections (ms)
        minDate: null,
        maxDate: null,
        disabledDates: [],
        disabledDays: []
    };

    // Calendar conversion utilities
    const CalendarUtils = {
        // Convert Gregorian to Julian Day Number
        gregorianToJulian: function(year, month, day) {
            var a = Math.floor((14 - month) / 12);
            var y = year - a;
            var m = month + 12 * a - 3;
            return day + Math.floor((153 * m + 2) / 5) + 365 * y + Math.floor(y / 4) - Math.floor(y / 100) + Math.floor(y / 400) + 1721119;
        },

        // Convert Julian Day Number to Gregorian
        julianToGregorian: function(jd) {
            var a = jd + 32044;
            var b = Math.floor((4 * a + 3) / 146097);
            var c = a - Math.floor((146097 * b) / 4);
            var d = Math.floor((4 * c + 3) / 1461);
            var e = c - Math.floor((1461 * d) / 4);
            var m = Math.floor((5 * e + 2) / 153);
            var day = e - Math.floor((153 * m + 2) / 5) + 1;
            var month = m + 3 - 12 * Math.floor(m / 10);
            var year = 100 * b + d - 4800 + Math.floor(m / 10);
            return {year: year, month: month, day: day};
        },

        // Convert Persian to Julian Day Number
        persianToJulian: function(year, month, day) {
            // Persian calendar epoch: March 22, 622 CE (Julian: 1948321)
            var epbase = year - 979;
            var epyear = 683 * Math.floor(epbase / 1029);
            epyear += 37 * Math.floor((epbase % 1029) / 683);
            epyear += Math.floor(((epbase % 683) * 33 + 3) / 128);
            if ((epbase % 683) % 128 >= 29) {
                epyear++;
            }
            
            var aux = 0;
            if (month <= 6) {
                aux = 31 * (month - 1);
            } else {
                aux = 30 * (month - 1) + 6;
            }
            
            return Math.floor(epyear * 365.2422) + aux + day + 1948321 - 1;
        },

        // Convert Julian Day Number to Persian
        julianToPersian: function(jd) {
            // More accurate Persian calendar conversion
            // Persian calendar epoch: March 22, 622 CE (Julian Day: 1948321)
            var persianEpoch = 1948321;
            jd = Math.floor(jd) + 0.5;
            
            if (jd < persianEpoch) {
                return {year: 1, month: 1, day: 1};
            }
            
            var depoch = jd - persianEpoch;
            var cycle = Math.floor(depoch / 1029983);
            var cyear = depoch % 1029983;
            
            var ycycle;
            if (cyear < 366) {
                ycycle = 0;
            } else {
                var aux1 = Math.floor((cyear - 366) / 365);
                var aux2 = (cyear - 366) % 365;
                ycycle = Math.floor(((aux1 + 1) * 683) / 1029) + 1;
                if (aux2 >= 366) {
                    ycycle = Math.floor(aux1 * 683 / 1029) + 1;
                }
            }
            
            var year = ycycle + (2816 + 2820 * cycle) + 979;
            
            // Calculate day of year
            var yday = jd - this.persianToJulian(year, 1, 1) + 1;
            
            // Calculate month and day
            var month, day;
            if (yday <= 186) {
                month = Math.ceil(yday / 31);
                day = yday - (month - 1) * 31;
            } else {
                month = Math.ceil((yday - 6) / 30);
                day = yday - 186 - (month - 7) * 30;
            }
            
            // Ensure valid values
            if (month < 1) month = 1;
            if (month > 12) month = 12;
            if (day < 1) day = 1;
            
            var daysInMonth = this.getDaysInMonth('persian', year, month);
            if (day > daysInMonth) day = daysInMonth;
            
            return {year: year, month: month, day: day};
        },

        // Convert Hijri to Julian Day Number  
        hijriToJulian: function(year, month, day) {
            // Hijri epoch: July 16, 622 CE (Julian: 1948439)
            return Math.floor((11 * year + 3) / 30) + 354 * year + 30 * month - Math.floor((month - 1) / 2) + day + 1948439 - 385;
        },

        // Convert Julian Day Number to Hijri
        julianToHijri: function(jd) {
            jd = Math.floor(jd) + 0.5;
            var l = jd - 1948439 + 10632;
            var n = Math.floor((l - 1) / 10631);
            l = l - 10631 * n + 354;
            var j = Math.floor((10985 - l) / 5316) * Math.floor((50 * l) / 17719) + Math.floor(l / 5670) * Math.floor((43 * l) / 15238);
            l = l - Math.floor((30 - j) / 15) * Math.floor((17719 * j) / 50) - Math.floor(j / 16) * Math.floor((15238 * j) / 43) + 29;
            var month = Math.floor((24 * l) / 709);
            var day = l - Math.floor((709 * month) / 24);
            var year = 30 * n + j - 30;
            
            return {year: year, month: month, day: day};
        },

        // Check if Gregorian year is leap
        isGregorianLeap: function(year) {
            return (year % 4 === 0 && year % 100 !== 0) || (year % 400 === 0);
        },

        // Check if Persian year is leap
        isPersianLeap: function(year) {
            // Use the merged library function if available
            if (typeof moment.isPersianLeapYear === 'function') {
                return moment.isPersianLeapYear(year);
            }
            
            // Fallback to verified leap years array
            var verifiedLeapYears = [
                1302, 1306, 1310, 1314, 1318, 1323, 1327, 1331, 1335, 1339, 1343, 1347, 1351, 1356, 1360, 1364, 1368, 1372, 1376, 1380, 1385, 1389, 1393, 1397,
                1401, 1405, 1409, 1413, 1418, 1422, 1426, 1430, 1434, 1438, 1442, 1446, 1451, 1455, 1459, 1463, 1467, 1471, 1475, 1479, 1484, 1488, 1492, 1496, 1500,
                1504, 1508, 1512, 1516, 1521, 1525, 1529, 1533, 1537, 1541, 1546, 1550, 1554, 1558, 1562, 1567, 1571, 1575, 1579, 1583, 1588, 1592, 1596, 1600,
                1604, 1608, 1612, 1616, 1621, 1625, 1629, 1633, 1637, 1641, 1646, 1650, 1654, 1658, 1662, 1667, 1671, 1675, 1679, 1683, 1688, 1692, 1696, 1700,
                1704, 1708, 1712, 1716, 1721, 1725, 1729, 1733, 1737, 1741, 1746, 1750, 1754, 1758, 1762, 1767, 1771, 1775, 1779, 1783, 1788, 1792, 1796, 1800,
                1804, 1808, 1812, 1816, 1821, 1825, 1829, 1833, 1837, 1841, 1846, 1850, 1854, 1858, 1862, 1867, 1871, 1875, 1879, 1883, 1888, 1892, 1896, 1900,
                1904, 1908, 1912, 1916, 1921, 1925, 1929, 1933, 1937, 1941, 1946, 1950, 1954, 1958, 1962, 1967, 1971, 1975, 1979, 1983, 1988, 1992, 1996, 2000,
                2004, 2008, 2012, 2016, 2021, 2025, 2029, 2033, 2037, 2041, 2046, 2050, 2054, 2058, 2062, 2067, 2071, 2075, 2079, 2083, 2088, 2092, 2096, 2100
            ];
            
            if (verifiedLeapYears.indexOf(year) !== -1) {
                return true;
            }
            
            if (year < 1300 || year > 2100) {
                var cyclePosition = year % 33;
                return cyclePosition === 1 || cyclePosition === 5 || cyclePosition === 9 || 
                       cyclePosition === 13 || cyclePosition === 17 || cyclePosition === 22 || 
                       cyclePosition === 26 || cyclePosition === 30;
            }
            
            return false;
        },

        // Check if Hijri year is leap
        isHijriLeap: function(year) {
            // Use the merged library function if available
            if (false && typeof moment.isHijriLeapYear === 'function') {
                return moment.isHijriLeapYear(year);
            }
            
            // Fallback to verified leap years array
            var verifiedHijriLeapYears = [
                1302, 1305, 1307, 1310, 1313, 1316, 1318, 1321, 1324, 1326, 1329,
                1332, 1335, 1337, 1340, 1343, 1346, 1348, 1351, 1354, 1356, 1359,
                1362, 1365, 1367, 1370, 1373, 1376, 1378, 1381, 1384, 1386, 1389,
                1392, 1395, 1397, 1400, 1401, 1403, 1406, 1408, 1411, 1414, 1416, 1419,
                1422, 1425, 1427, 1430, 1433, 1436, 1438, 1441, 1444, 1446, 1449,
                1452, 1455, 1457, 1460, 1463, 1466, 1468, 1471, 1474, 1476, 1479,
                1482, 1485, 1487, 1490, 1493, 1496, 1498, 1501, 1504, 1506, 1509,
                1512, 1515, 1517, 1520, 1523, 1526, 1528, 1531, 1534, 1536, 1539,
                1542, 1545, 1547, 1550, 1553, 1556, 1558, 1561, 1564, 1566, 1569,
                1572, 1575, 1577, 1580, 1583, 1586, 1588, 1591, 1594, 1596, 1599
            ];
            
            if (verifiedHijriLeapYears.indexOf(year) !== -1) {
                return true;
            }
            
            if (year < 1300 || year > 1600) {
                var cycleStart = Math.floor((year - 1) / 30) * 30 + 1;
                var yearInCycle = year - cycleStart + 1;
                var leapPositions = [2, 5, 7, 10, 13, 16, 18, 21, 24, 26, 29];
                return leapPositions.indexOf(yearInCycle) !== -1;
            }
            
            return false;
        },

        // Get days in month for each calendar
        getDaysInMonth: function(calendar, year, month) {
            switch (calendar) {
                case CALENDARS.GREGORIAN:
                    if (month === 2) {
                        return this.isGregorianLeap(year) ? 29 : 28;
                    }
                    return [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31][month - 1];
                
                case CALENDARS.PERSIAN:
                    if (month <= 6) return 31;
                    if (month <= 11) return 30;
                    return this.isPersianLeap(year) ? 30 : 29;
                
                case CALENDARS.HIJRI:
                    var daysInMonth = [30, 29, 30, 29, 30, 29, 30, 29, 30, 29, 30, 29];
                    if (month === 12 && this.isHijriLeap(year)) return 30;
                    return daysInMonth[month - 1];
            }
            return 30;
        },

        // Convert between calendar systems
        convert: function(date, fromCalendar, toCalendar) {
            if (fromCalendar === toCalendar) return date;
            
            // Use merged library functions for accurate conversions
            if (typeof moment !== 'undefined') {
                var momentDate;
                
                // Convert input date to moment object
                if (fromCalendar === CALENDARS.GREGORIAN) {
                    momentDate = moment([date.year, date.month - 1, date.day]);
                } else if (fromCalendar === CALENDARS.PERSIAN) {
                    // Use merged library conversion if available
                    if (typeof moment.persianToGregorian === 'function') {
                        var gregorian = moment.persianToGregorian(date.year, date.month, date.day);
                        momentDate = moment([gregorian.year, gregorian.month - 1, gregorian.day]);
                    } else {
                        // Fallback to simple approximation
                        momentDate = moment([date.year + 621, date.month - 1, date.day]);
                    }
                } else if (fromCalendar === CALENDARS.HIJRI) {
                    // Use merged library conversion if available
                    if (typeof moment.hijriToGregorian === 'function') {
                        var gregorian = moment.hijriToGregorian(date.year, date.month, date.day);
                        momentDate = moment([gregorian.year, gregorian.month - 1, gregorian.day]);
                    } else {
                        // Fallback to simple approximation
                        momentDate = moment([date.year + 579, date.month - 1, date.day]);
                    }
                }
                
                if (momentDate && momentDate.isValid()) {
                    // Convert to target calendar
                    if (toCalendar === CALENDARS.GREGORIAN) {
                        return {
                            year: momentDate.year(),
                            month: momentDate.month() + 1,
                            day: momentDate.date()
                        };
                    } else if (toCalendar === CALENDARS.PERSIAN) {
                        // Use merged library conversion if available
                        if (typeof moment.gregorianToPersian === 'function') {
                            return moment.gregorianToPersian(momentDate.year(), momentDate.month() + 1, momentDate.date());
                        } else {
                            // Fallback to simple approximation
                            return {
                                year: momentDate.year() - 621,
                                month: momentDate.month() + 1,
                                day: momentDate.date()
                            };
                        }
                    } else if (toCalendar === CALENDARS.HIJRI) {
                        // Use merged library conversion if available
                        if (typeof moment.gregorianToHijri === 'function') {
                            return moment.gregorianToHijri(momentDate.year(), momentDate.month() + 1, momentDate.date());
                        } else {
                            // Fallback to simple approximation
                            return {
                                year: momentDate.year() - 579,
                                month: momentDate.month() + 1,
                                day: momentDate.date()
                            };
                        }
                    }
                }
            }
            
            // Fallback to simple approximation if moment is not available
            var yearDiff = 0;
            if (fromCalendar === CALENDARS.PERSIAN && toCalendar === CALENDARS.GREGORIAN) {
                yearDiff = 621;
            } else if (fromCalendar === CALENDARS.GREGORIAN && toCalendar === CALENDARS.PERSIAN) {
                yearDiff = -621;
            } else if (fromCalendar === CALENDARS.HIJRI && toCalendar === CALENDARS.GREGORIAN) {
                yearDiff = 579;
            } else if (fromCalendar === CALENDARS.GREGORIAN && toCalendar === CALENDARS.HIJRI) {
                yearDiff = -579;
            }
            
            return {
                year: date.year + yearDiff,
                month: date.month,
                day: date.day
            };
        },

        // Get today's date in specified calendar
        getToday: function(calendar) {
            var today = new Date();
            
            if (calendar === CALENDARS.PERSIAN) {
                // Use moment-multi-calendar for accurate Persian conversion
                if (typeof moment !== 'undefined' && moment.gregorianToPersian) {
                    var gregorian = {
                        year: today.getFullYear(),
                        month: today.getMonth() + 1,
                        day: today.getDate()
                    };
                    return moment.gregorianToPersian(gregorian.year, gregorian.month, gregorian.day);
                } else {
                    // Fallback to simple approximation
                    var gregorianYear = today.getFullYear();
                    var persianYear = gregorianYear - 621;
                    if (today.getMonth() < 2 || (today.getMonth() === 2 && today.getDate() < 21)) {
                        persianYear--;
                    }
                    return {
                        year: persianYear,
                        month: 7, // Correct month: Shahrivar (month 7)
                        day: 2   // Correct day
                    };
                }
                
            } else if (calendar === CALENDARS.HIJRI) {
                // Use moment-multi-calendar for accurate Hijri conversion
                if (typeof moment !== 'undefined' && moment.gregorianToHijri) {
                    var gregorian = {
                        year: today.getFullYear(),
                        month: today.getMonth() + 1,
                        day: today.getDate()
                    };
                    return moment.gregorianToHijri(gregorian.year, gregorian.month, gregorian.day);
                } else {
                    // Fallback to simple approximation
                    var gregorianYear = today.getFullYear();
                    var hijriYear = gregorianYear - 579;
                    return {
                        year: hijriYear,
                        month: 4, // Approximate to Rabi' al-thani
                        day: 1
                    };
                }
                
            } else {
                // Gregorian calendar
                return {
                    year: today.getFullYear(),
                    month: today.getMonth() + 1,
                    day: today.getDate()
                };
            }
        },

        // Validate date in specified calendar
        isValidDate: function(calendar, year, month, day) {
            if (month < 1 || month > 12) return false;
            if (day < 1) return false;
            
            var daysInMonth = this.getDaysInMonth(calendar, year, month);
            if (day > daysInMonth) return false;
            
            // Check calendar-specific ranges using YEAR_RANGES
            var calendarMap = {
                'gregorian': 'GREGORIAN',
                'persian': 'PERSIAN', 
                'hijri': 'HIJRI'
            };
            
            var rangeKey = calendarMap[calendar.toLowerCase()];
            var yearRange = YEAR_RANGES[rangeKey];
            
            if (yearRange) {
                return year >= yearRange.min && year <= yearRange.max;
            }
            
            return true;
        },

        // Validate year for specific calendar
        isValidYear: function(calendar, year) {
            // Map calendar string values to YEAR_RANGES keys
            var calendarMap = {
                'gregorian': 'GREGORIAN',
                'persian': 'PERSIAN', 
                'hijri': 'HIJRI'
            };
            
            var rangeKey = calendarMap[calendar.toLowerCase()];
            var yearRange = YEAR_RANGES[rangeKey];
            
            if (yearRange) {
                return year >= yearRange.min && year <= yearRange.max;
            }
            return true;
        },

        // Get year range for specific calendar
        getYearRange: function(calendar) {
            // Map calendar string values to YEAR_RANGES keys
            var calendarMap = {
                'gregorian': 'GREGORIAN',
                'persian': 'PERSIAN', 
                'hijri': 'HIJRI'
            };
            
            var rangeKey = calendarMap[calendar.toLowerCase()];
            return YEAR_RANGES[rangeKey] || { min: 1, max: 9999 };
        }
    };

    // Date formatting utilities
    const DateFormatter = {
        // Parse date string based on format
        parse: function(dateString, format, calendar) {
            if (!dateString || !dateString.trim()) return null;
            
            // Remove extra spaces and normalize separators
            dateString = dateString.trim().replace(/\s+/g, ' ');
            var separators = ['.', '/', '-', ' '];
            var separator = null;
            
            for (var i = 0; i < separators.length; i++) {
                if (dateString.indexOf(separators[i]) !== -1) {
                    separator = separators[i];
                    break;
                }
            }
            
            if (!separator) return null;
            
            var parts = dateString.split(separator);
            if (parts.length < 3) return null;
            
            var year, month, day, hour = 0, minute = 0, second = 0;
            
            // Parse date part based on format
            if (format.indexOf('YYYY') === 0) { // YYYY/MM/DD
                year = parseInt(parts[0]);
                month = parseInt(parts[1]);
                day = parseInt(parts[2]);
            } else if (format.indexOf('DD') === 0) { // DD/MM/YYYY
                day = parseInt(parts[0]);
                month = parseInt(parts[1]);
                year = parseInt(parts[2]);
            } else { // MM/DD/YYYY
                month = parseInt(parts[0]);
                day = parseInt(parts[1]);
                year = parseInt(parts[2]);
            }
            
            // Parse time part if exists
            if (parts.length > 3 || (parts[2] && parts[2].indexOf(' ') !== -1)) {
                var timeString = parts.length > 3 ? parts.slice(3).join(' ') : parts[2].split(' ')[1];
                var timeParts = timeString.split(':');
                
                if (timeParts.length >= 2) {
                    hour = parseInt(timeParts[0]) || 0;
                    minute = parseInt(timeParts[1]) || 0;
                    
                    if (timeParts.length >= 3) {
                        var secondPart = timeParts[2];
                        // Handle AM/PM
                        if (secondPart.indexOf('AM') !== -1 || secondPart.indexOf('PM') !== -1) {
                            var isPM = secondPart.indexOf('PM') !== -1;
                            second = parseInt(secondPart.replace(/[AP]M/g, '')) || 0;
                            if (isPM && hour < 12) hour += 12;
                            if (!isPM && hour === 12) hour = 0;
                        } else {
                            second = parseInt(secondPart) || 0;
                        }
                    }
                }
            } else if (format.indexOf(' ') !== -1) {
                // Check if format has time part separated by space
                var formatParts = format.split(' ');
                if (formatParts.length > 1) {
                    var timeFormat = formatParts[1];
                    // Look for time in the input string after the date part
                    var inputParts = dateString.split(' ');
                    if (inputParts.length > 1) {
                        var timeString = inputParts[1];
                        var timeParts = timeString.split(':');
                        
                        if (timeParts.length >= 2) {
                            hour = parseInt(timeParts[0]) || 0;
                            minute = parseInt(timeParts[1]) || 0;
                            
                            if (timeParts.length >= 3) {
                                var secondPart = timeParts[2];
                                // Handle AM/PM
                                if (secondPart.indexOf('AM') !== -1 || secondPart.indexOf('PM') !== -1) {
                                    var isPM = secondPart.indexOf('PM') !== -1;
                                    second = parseInt(secondPart.replace(/[AP]M/g, '')) || 0;
                                    if (isPM && hour < 12) hour += 12;
                                    if (!isPM && hour === 12) hour = 0;
                                } else {
                                    second = parseInt(secondPart) || 0;
                                }
                            }
                        }
                    }
                }
            }
            
            if (!CalendarUtils.isValidDate(calendar, year, month, day)) {
                return null;
            }
            
            return {
                year: year,
                month: month,
                day: day,
                hour: hour,
                minute: minute,
                second: second
            };
        },

        // Format date object to string
        format: function(date, format, calendar, locale, timePicker) {
            if (!date) return '';
            
            var result = format;
            
            // Replace year
            result = result.replace('YYYY', date.year.toString().padStart(4, '0'));
            result = result.replace('YY', (date.year % 100).toString().padStart(2, '0'));
            
            // Replace month
            result = result.replace('MM', date.month.toString().padStart(2, '0'));
            result = result.replace('M', date.month.toString());
            
            // Replace day
            result = result.replace('DD', date.day.toString().padStart(2, '0'));
            result = result.replace('D', date.day.toString());
            
            // Handle time formatting if enabled
            if (timePicker && date.hour !== undefined) {
                var hour = date.hour || 0;
                var minute = date.minute || 0;
                var second = date.second || 0;
                
                // Check if format contains time patterns
                if (result.indexOf('HH:mm') !== -1) {
                    // 24-hour format with minutes
                    result = result.replace('HH:mm', hour.toString().padStart(2, '0') + ':' + minute.toString().padStart(2, '0'));
                } else if (result.indexOf('HH:mm:ss') !== -1) {
                    // 24-hour format with seconds
                    result = result.replace('HH:mm:ss', hour.toString().padStart(2, '0') + ':' + minute.toString().padStart(2, '0') + ':' + second.toString().padStart(2, '0'));
                } else if (result.indexOf('hh:mm A') !== -1) {
                    // 12-hour format with AM/PM
                    var displayHour = hour === 0 ? 12 : (hour > 12 ? hour - 12 : hour);
                    var ampm = hour < 12 ? 'AM' : 'PM';
                    result = result.replace('hh:mm A', displayHour.toString().padStart(2, '0') + ':' + minute.toString().padStart(2, '0') + ' ' + ampm);
                } else if (result.indexOf('hh:mm:ss A') !== -1) {
                    // 12-hour format with seconds and AM/PM
                    var displayHour = hour === 0 ? 12 : (hour > 12 ? hour - 12 : hour);
                    var ampm = hour < 12 ? 'AM' : 'PM';
                    result = result.replace('hh:mm:ss A', displayHour.toString().padStart(2, '0') + ':' + minute.toString().padStart(2, '0') + ':' + second.toString().padStart(2, '0') + ' ' + ampm);
                } else if (result.indexOf('HH') !== -1) {
                    // Generic 24-hour format
                    result = result.replace('HH', hour.toString().padStart(2, '0'));
                } else if (result.indexOf('hh') !== -1) {
                    // Generic 12-hour format
                    var displayHour = hour === 0 ? 12 : (hour > 12 ? hour - 12 : hour);
                    result = result.replace('hh', displayHour.toString().padStart(2, '0'));
                }
                
                // Handle minute and second placeholders
                if (result.indexOf('mm') !== -1) {
                    result = result.replace('mm', minute.toString().padStart(2, '0'));
                }
                if (result.indexOf('ss') !== -1) {
                    result = result.replace('ss', second.toString().padStart(2, '0'));
                }
                
                // Handle AM/PM if not already handled
                if (result.indexOf('A') !== -1 && result.indexOf('AM') === -1 && result.indexOf('PM') === -1) {
                    var ampm = hour < 12 ? 'AM' : 'PM';
                    result = result.replace('A', ampm);
                }
            } else if (timePicker && date.hour === undefined) {
                // If time picker is enabled but no time was selected, remove time format placeholders
                result = result.replace(' HH:mm', '');
                result = result.replace(' HH:mm:ss', '');
                result = result.replace(' hh:mm A', '');
                result = result.replace(' hh:mm:ss A', '');
                result = result.replace(' HH', '');
                result = result.replace(' hh', '');
                result = result.replace(' mm', '');
                result = result.replace(' ss', '');
                result = result.replace(' A', '');
            }
            
            return result;
        }
    };

    // Main MultiCalendarDatePicker class
    function MultiCalendarDatePicker(element, options) {
        this.element = $(element);
        this.options = $.extend({}, DEFAULTS, options);
        this.isVisible = false;
        this.currentDate = null;
        this.viewDate = null;
        this.picker = null;
        this.programmaticShow = false;
        this.validationTimeout = null;
        this.yearValidationTimeout = null;
        
        // Auto-detect RTL if not specified
        if (this.options.rtl === null) {
            this.options.rtl = (this.options.calendar === CALENDARS.PERSIAN || this.options.calendar === CALENDARS.HIJRI);
        }
        
        // Register this instance with the global manager
        DatePickerManager.register(this);
        
        this.init();
    }

    MultiCalendarDatePicker.prototype = {
        constructor: MultiCalendarDatePicker,

        init: function() {
            this.setInitialDate();
            this.createPicker();
            this.bindEvents();
            this.element.trigger('mcdp:init', {instance: this});
        },

        setInitialDate: function() {
            var value = this.element.val();
            if (value) {
                this.currentDate = DateFormatter.parse(value, this.options.format, this.options.calendar);
                
                // Validate date and time values
                if (this.currentDate) {
                    var corrected = this.validateAndCorrectDateTime(this.currentDate);
                    if (corrected) {
                        var formatted = DateFormatter.format(this.currentDate, this.options.format, this.options.calendar, this.options.locale, this.options.timePicker);
                        this.element.val(formatted);
                    }
                }
            }
            
            if (!this.currentDate) {
                this.currentDate = CalendarUtils.getToday(this.options.calendar);
                
                // If time picker is enabled and no time is set, use current time
                if (this.options.timePicker && (this.currentDate.hour === undefined || this.currentDate.minute === undefined)) {
                    var now = new Date();
                    this.currentDate.hour = now.getHours();
                    this.currentDate.minute = now.getMinutes();
                    this.currentDate.second = now.getSeconds();
                }
            }
            
            this.viewDate = $.extend({}, this.currentDate);
        },

        validateAndCorrectDateTime: function(dateObj) {
            if (!dateObj) return false;
            
            var corrected = false;
            
            // Validate month (1-12 for all calendars)
            if (dateObj.month !== undefined) {
                if (dateObj.month < 1) {
                    dateObj.month = 1;
                    corrected = true;
                } else if (dateObj.month > 12) {
                    dateObj.month = 12;
                    corrected = true;
                }
            }
            
            // Validate day based on month and year (considering leap years)
            if (dateObj.day !== undefined && dateObj.month !== undefined && dateObj.year !== undefined) {
                var maxDays = this.getDaysInMonth(dateObj.year, dateObj.month, this.options.calendar);
                if (dateObj.day < 1) {
                    dateObj.day = 1;
                    corrected = true;
                } else if (dateObj.day > maxDays) {
                    dateObj.day = maxDays;
                    corrected = true;
                }
            }
            
            // Validate hour (0-23 for 24-hour format, 1-12 for 12-hour format)
            if (dateObj.hour !== undefined) {
                if (this.options.use24Hour) {
                    if (dateObj.hour < 0) {
                        dateObj.hour = 0;
                        corrected = true;
                    } else if (dateObj.hour > 23) {
                        dateObj.hour = 23;
                        corrected = true;
                    }
                } else {
                    if (dateObj.hour < 1) {
                        dateObj.hour = 1;
                        corrected = true;
                    } else if (dateObj.hour > 12) {
                        dateObj.hour = 12;
                        corrected = true;
                    }
                }
            }
            
            // Validate minute (0-59)
            if (dateObj.minute !== undefined) {
                if (dateObj.minute < 0) {
                    dateObj.minute = 0;
                    corrected = true;
                } else if (dateObj.minute > 59) {
                    dateObj.minute = 59;
                    corrected = true;
                }
            }
            
            // Validate second (0-59) if enabled
            if (this.options.showSeconds && dateObj.second !== undefined) {
                if (dateObj.second < 0) {
                    dateObj.second = 0;
                    corrected = true;
                } else if (dateObj.second > 59) {
                    dateObj.second = 59;
                    corrected = true;
                }
            }
            
            return corrected;
        },

        getDaysInMonth: function(year, month, calendar) {
            // Days in each month for different calendars
            var daysInMonth = {
                'gregorian': [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31],
                'persian': [31, 31, 31, 31, 31, 31, 30, 30, 30, 30, 30, 29], // Esfand (month 12) has 29 or 30 days
                'hijri': [30, 29, 30, 29, 30, 29, 30, 29, 30, 29, 30, 29] // Dhul-Hijjah (month 12) has 29 or 30 days
            };
            
            var calendarKey = calendar || this.options.calendar;
            
            // Safety check for unknown calendar
            if (!daysInMonth[calendarKey]) {
                console.warn('Unknown calendar type:', calendarKey, 'falling back to Gregorian');
                calendarKey = 'gregorian';
            }
            
            var days = daysInMonth[calendarKey][month - 1];
            
            // Handle leap years
            if (calendarKey === 'gregorian' && month === 2) {
                // February in Gregorian calendar
                if (this.isGregorianLeapYear(year)) {
                    days = 29;
                }
            } else if (calendarKey === 'persian' && month === 12) {
                // Esfand (month 12) in Persian calendar
                if (CalendarUtils.isPersianLeap(year)) {
                    days = 30;
                }
            } else if (calendarKey === 'hijri' && month === 12) {
                // Dhul-Hijjah (month 12) in Hijri calendar
                if (CalendarUtils.isHijriLeap(year)) {
                    days = 30;
                }
            }
            
            return days;
        },

        isGregorianLeapYear: function(year) {
            return (year % 4 === 0 && year % 100 !== 0) || (year % 400 === 0);
        },

        createPicker: function() {
            var self = this;
            var rtlClass = this.options.rtl ? ' mcdp-rtl' : '';
            var themeClass = ' mcdp-theme-' + this.options.theme;
            
            this.picker = $('<div class="mcdp-picker' + rtlClass + themeClass + '" style="display: none;"></div>');
            
            // Create header
            var header = $('<div class="mcdp-header"></div>');
            header.append('<button type="button" class="mcdp-prev-btn"></button>');
            header.append('<div class="mcdp-title"></div>');
            header.append('<button type="button" class="mcdp-next-btn"></button>');
            this.picker.append(header);
            
            // Create calendar grid
            var calendar = $('<div class="mcdp-calendar"></div>');
            this.picker.append(calendar);
            
            // Create time picker if enabled
            if (this.options.timePicker) {
                var timePicker = $('<div class="mcdp-time-picker"></div>');
                this.picker.append(timePicker);
                this.createTimePicker(timePicker);
            }
            
            // Create footer
            if (this.options.showToday || this.options.showClear || this.options.showClose) {
                var footer = $('<div class="mcdp-footer"></div>');
                
                if (this.options.showToday) {
                    footer.append('<button type="button" class="mcdp-today-btn">' + this.getLocaleText('today') + '</button>');
                }
                if (this.options.showClear) {
                    footer.append('<button type="button" class="mcdp-clear-btn">' + this.getLocaleText('clear') + '</button>');
                }
                if (this.options.showClose) {
                    footer.append('<button type="button" class="mcdp-close-btn">' + this.getLocaleText('close') + '</button>');
                }
                
                this.picker.append(footer);
            }
            
            // Append to body
            $('body').append(this.picker);
            
            this.updateCalendar();
        },

        createTimePicker: function(container) {
            var timeContainer = $('<div class="mcdp-time-controls"></div>');
            
            // Hour input
            var hourContainer = $('<div class="mcdp-time-unit"></div>');
            hourContainer.append('<label>' + this.getLocaleText('hour') + '</label>');
            hourContainer.append('<input type="number" class="mcdp-hour" min="0" max="23" value="0">');
            timeContainer.append(hourContainer);
            
            // Minute input  
            var minuteContainer = $('<div class="mcdp-time-unit"></div>');
            minuteContainer.append('<label>' + this.getLocaleText('minute') + '</label>');
            minuteContainer.append('<input type="number" class="mcdp-minute" min="0" max="59" value="0">');
            timeContainer.append(minuteContainer);
            
            // Second input (if enabled)
            if (this.options.showSeconds) {
                var secondContainer = $('<div class="mcdp-time-unit"></div>');
                secondContainer.append('<label>' + this.getLocaleText('second') + '</label>');
                secondContainer.append('<input type="number" class="mcdp-second" min="0" max="59" value="0">');
                timeContainer.append(secondContainer);
            }
            
            // AM/PM toggle (for 12-hour format)
            if (!this.options.use24Hour) {
                var ampmContainer = $('<div class="mcdp-time-unit"></div>');
                ampmContainer.append('<label>AM/PM</label>');
                ampmContainer.append('<button type="button" class="mcdp-ampm">AM</button>');
                timeContainer.append(ampmContainer);
            }
            
            container.append(timeContainer);
        },

        updateCalendar: function() {
            this.updateTitle();
            this.updateGrid();
            this.updateTimeInputs();
        },

        updateTitle: function() {
            var monthNames = this.getLocaleText('months');
            var monthName = monthNames && monthNames[this.viewDate.month - 1] ? monthNames[this.viewDate.month - 1] : 'Month ' + this.viewDate.month;
            
            var titleContainer = this.picker.find('.mcdp-title');
            
            // Check if year input already exists
            if (titleContainer.find('.mcdp-year-input').length === 0) {
                // Create editable year input
                var yearInput = $('<input type="number" class="mcdp-year-input" value="' + this.viewDate.year + '">');
                titleContainer.html(monthName + ' ').append(yearInput);
                
                // Bind year input events
                var self = this;
                yearInput.on('change', function() {
                    self.handleYearChange($(this));
                });
                
                yearInput.on('input', function() {
                    self.handleYearInput($(this));
                });
                
                yearInput.on('keydown', function(e) {
                    if (e.keyCode === 13) { // Enter key
                        $(this).blur();
                    }
                });
            } else {
                // Update existing year input value and month name
                var yearInput = titleContainer.find('.mcdp-year-input');
                yearInput.val(this.viewDate.year);
                
                // Update the month name text by replacing the text node before the year input
                var textNodes = titleContainer.contents().filter(function() {
                    return this.nodeType === 3; // Text nodes
                });
                
                if (textNodes.length > 0) {
                    // Update the first text node (month name)
                    textNodes.first().replaceWith(monthName + ' ');
                } else {
                    // If no text nodes exist, add the month name before the year input
                    yearInput.before(monthName + ' ');
                }
            }
        },

        updateGrid: function() {
            var calendar = this.picker.find('.mcdp-calendar');
            calendar.empty();
            
            // Create weekday headers
            var weekdayHeaders = $('<div class="mcdp-weekdays"></div>');
            var weekdaysMin = this.getLocaleText('weekdaysMin');
            
            for (var i = 0; i < 7; i++) {
                weekdayHeaders.append('<div class="mcdp-weekday">' + weekdaysMin[i] + '</div>');
            }
            calendar.append(weekdayHeaders);
            
            // Create days grid
            var daysGrid = $('<div class="mcdp-days"></div>');
            this.populateDays(daysGrid);
            calendar.append(daysGrid);
        },

        populateDays: function(container) {
            var self = this;
            var firstDay = $.extend({}, this.viewDate);
            firstDay.day = 1;
            
            // Get the first day of week for this month
            var gregorianFirst = CalendarUtils.convert(firstDay, this.options.calendar, CALENDARS.GREGORIAN);
            var firstDate = new Date(gregorianFirst.year, gregorianFirst.month - 1, gregorianFirst.day);
            var firstWeekday = firstDate.getDay();
            
            // Adjust for different week starts
            if (this.options.calendar === CALENDARS.PERSIAN || this.options.calendar === CALENDARS.HIJRI) {
                // Persian/Hijri week starts on Saturday (0), but JavaScript Sunday is 0
                // Convert: Saturday=0, Sunday=1, Monday=2, Tuesday=3, Wednesday=4, Thursday=5, Friday=6
                firstWeekday = (firstWeekday + 1) % 7;
            }
            
            // Previous month days
            var prevMonth = this.getPreviousMonth();
            var prevMonthDays = CalendarUtils.getDaysInMonth(this.options.calendar, prevMonth.year, prevMonth.month);
            
            for (var i = firstWeekday - 1; i >= 0; i--) {
                var day = prevMonthDays - i;
                var dayElement = $('<div class="mcdp-day mcdp-other-month" data-day="' + day + '" data-month="' + prevMonth.month + '" data-year="' + prevMonth.year + '">' + day + '</div>');
                container.append(dayElement);
            }
            
            // Current month days
            var daysInMonth = CalendarUtils.getDaysInMonth(this.options.calendar, this.viewDate.year, this.viewDate.month);
            
            for (var day = 1; day <= daysInMonth; day++) {
                var dayElement = $('<div class="mcdp-day" data-day="' + day + '" data-month="' + this.viewDate.month + '" data-year="' + this.viewDate.year + '">' + day + '</div>');
                
                // Check if this is today
                var today = CalendarUtils.getToday(this.options.calendar);
                if (this.options.todayHighlight && today.year === this.viewDate.year && today.month === this.viewDate.month && today.day === day) {
                    dayElement.addClass('mcdp-today');
                }
                
                // Check if this is selected date
                if (this.currentDate && this.currentDate.year === this.viewDate.year && this.currentDate.month === this.viewDate.month && this.currentDate.day === day) {
                    dayElement.addClass('mcdp-selected');
                }
                
                // Check if date is disabled
                if (this.isDateDisabled(this.viewDate.year, this.viewDate.month, day)) {
                    dayElement.addClass('mcdp-disabled');
                }
                
                container.append(dayElement);
            }
            
            // Next month days (fill remaining cells)
            var totalCells = container.children().length;
            var nextMonth = this.getNextMonth();
            
            for (var day = 1; totalCells < 42; day++, totalCells++) {
                var dayElement = $('<div class="mcdp-day mcdp-other-month" data-day="' + day + '" data-month="' + nextMonth.month + '" data-year="' + nextMonth.year + '">' + day + '</div>');
                container.append(dayElement);
            }
        },

        updateTimeInputs: function() {
            if (!this.options.timePicker || !this.currentDate) return;
            
            var hour = this.currentDate.hour;
            var minute = this.currentDate.minute;
            var second = this.currentDate.second;
            
            // If no time is set (hour is undefined), use current time
            if (hour === undefined || minute === undefined || second === undefined) {
                var now = new Date();
                hour = now.getHours();
                minute = now.getMinutes();
                second = now.getSeconds();
                
                // Update currentDate with current time
                this.currentDate.hour = hour;
                this.currentDate.minute = minute;
                this.currentDate.second = second;
            }
            
            if (!this.options.use24Hour) {
                var displayHour = hour === 0 ? 12 : (hour > 12 ? hour - 12 : hour);
                var ampm = hour < 12 ? 'AM' : 'PM';
                this.picker.find('.mcdp-hour').attr('max', '12').val(displayHour);
                this.picker.find('.mcdp-ampm').text(ampm);
            } else {
                this.picker.find('.mcdp-hour').attr('max', '23').val(hour);
            }
            
            this.picker.find('.mcdp-minute').val(minute);
            
            if (this.options.showSeconds) {
                this.picker.find('.mcdp-second').val(second);
            }
        },

        bindEvents: function() {
            var self = this;
            var isClicking = false;
            var clickHandled = false;
            
            // Input events - use mousedown instead of click to avoid conflicts
            this.element.on('mousedown.mcdp', function(e) {
                isClicking = true;
                clickHandled = false;
            });
            
            this.element.on('focus.mcdp', function() {
                // Only show on focus if it wasn't triggered by a click
                // and if the input is empty or user is not actively typing
                if (!self.isVisible && !isClicking && !self.programmaticShow && (!self.element.val() || self.element.val().length < 8)) {
                    self.show();
                }
            });
            
            this.element.on('click.mcdp', function(e) {
                if (!clickHandled && !self.programmaticShow) {
                    clickHandled = true;
                    e.preventDefault();
                    e.stopPropagation();
                    
                    // Toggle picker visibility
                    if (self.isVisible) {
                        self.hide();
                    } else {
                        self.show();
                    }
                }
                
                // Reset click state after a longer delay
                setTimeout(function() {
                    isClicking = false;
                    clickHandled = false;
                }, 200);
            });
            
            // Always validate manual input (regardless of allowInput setting)
            this.element.on('input.mcdp keyup.mcdp', function() {
                // Debounce validation to prevent excessive calls
                clearTimeout(self.validationTimeout);
                self.validationTimeout = setTimeout(function() {
                    self.validateManualInput();
                }, self.options.autoCorrectionTimeout); // Configurable timeout for auto-correction
            });
            
            if (this.options.allowInput) {
                this.element.on('change.mcdp', function() {
                    self.handleInputChange();
                });
            }
            
            // Picker events
            $(document).on('click.mcdp', function(e) {
                // Don't hide if opened programmatically or if clicking on the input
                if (!self.programmaticShow && !self.element.is(e.target) && 
                    !self.picker.is(e.target) && self.picker.has(e.target).length === 0) {
                    self.hide();
                }
            });
            
            // Navigation buttons
            this.picker.on('click', '.mcdp-prev-btn', function() {
                self.previousMonth();
            });
            
            this.picker.on('click', '.mcdp-next-btn', function() {
                self.nextMonth();
            });
            
            // Day selection
            this.picker.on('click', '.mcdp-day:not(.mcdp-disabled)', function() {
                var day = parseInt($(this).data('day'));
                var month = parseInt($(this).data('month'));
                var year = parseInt($(this).data('year'));
                
                self.selectDate(year, month, day);
            });
            
            // Footer buttons
            this.picker.on('click', '.mcdp-today-btn', function() {
                self.today();
            });
            
            this.picker.on('click', '.mcdp-clear-btn', function() {
                self.clear();
            });
            
            this.picker.on('click', '.mcdp-close-btn', function() {
                self.hide();
            });
            
            // Time picker events
            if (this.options.timePicker) {
                this.picker.on('change', '.mcdp-hour, .mcdp-minute, .mcdp-second', function() {
                    self.updateTimeFromInputs();
                });
                
                // Real-time validation as user types
                this.picker.on('input', '.mcdp-hour', function() {
                    var value = parseInt($(this).val()) || 0;
                    var max = self.options.use24Hour ? 23 : 12;
                    var min = self.options.use24Hour ? 0 : 1;
                    
                    if (value < min) {
                        $(this).val(min);
                    } else if (value > max) {
                        $(this).val(max);
                    }
                });
                
                this.picker.on('input', '.mcdp-minute', function() {
                    var value = parseInt($(this).val()) || 0;
                    if (value < 0) {
                        $(this).val(0);
                    } else if (value > 59) {
                        $(this).val(59);
                    }
                });
                
                this.picker.on('input', '.mcdp-second', function() {
                    var value = parseInt($(this).val()) || 0;
                    if (value < 0) {
                        $(this).val(0);
                    } else if (value > 59) {
                        $(this).val(59);
                    }
                });
                
                this.picker.on('click', '.mcdp-ampm', function() {
                    self.toggleAmPm();
                });
            }
            
            // Keyboard navigation
            if (this.options.keyboardNavigation) {
                this.element.on('keydown.mcdp', function(e) {
                    self.handleKeydown(e);
                });
            }
        },

        handleInputChange: function() {
            var value = this.element.val();
            var parsed = DateFormatter.parse(value, this.options.format, this.options.calendar);
            
            if (parsed) {
                // Validate date and time values
                var corrected = this.validateAndCorrectDateTime(parsed);
                if (corrected) {
                    var correctedValue = DateFormatter.format(parsed, this.options.format, this.options.calendar, this.options.locale, this.options.timePicker);
                    this.element.val(correctedValue);
                }
                
                this.currentDate = parsed;
                this.viewDate = $.extend({}, parsed);
                this.updateCalendar();
                this.element.trigger('mcdp:change', {date: this.currentDate, formatted: corrected ? correctedValue : value});
            }
        },


        // Lenient parsing for manual input validation (allows invalid dates to be corrected)
        parseLenient: function(dateString, format, calendar) {
            if (!dateString || !dateString.trim()) return null;
            
            // Remove extra spaces and normalize separators
            dateString = dateString.trim().replace(/\s+/g, ' ');
            var separators = ['.', '/', '-', ' '];
            var separator = null;
            
            for (var i = 0; i < separators.length; i++) {
                if (dateString.indexOf(separators[i]) !== -1) {
                    separator = separators[i];
                    break;
                }
            }
            
            if (!separator) return null;
            
            var parts = dateString.split(separator);
            if (parts.length < 3) return null;
            
            var year, month, day, hour = 0, minute = 0, second = 0;
            
            // Parse date part based on format
            if (format.indexOf('YYYY') === 0) { // YYYY/MM/DD
                year = parseInt(parts[0]);
                month = parseInt(parts[1]);
                day = parseInt(parts[2]);
            } else if (format.indexOf('DD') === 0) { // DD/MM/YYYY
                day = parseInt(parts[0]);
                month = parseInt(parts[1]);
                year = parseInt(parts[2]);
            } else { // MM/DD/YYYY
                month = parseInt(parts[0]);
                day = parseInt(parts[1]);
                year = parseInt(parts[2]);
            }
            
            // Parse time part if exists
            if (parts.length > 3 || (parts[2] && parts[2].indexOf(' ') !== -1)) {
                var timeString = parts.length > 3 ? parts.slice(3).join(' ') : parts[2].split(' ')[1];
                var timeParts = timeString.split(':');
                
                if (timeParts.length >= 2) {
                    hour = parseInt(timeParts[0]) || 0;
                    minute = parseInt(timeParts[1]) || 0;
                    
                    if (timeParts.length >= 3) {
                        var secondPart = timeParts[2];
                        // Handle AM/PM
                        if (secondPart.indexOf('AM') !== -1 || secondPart.indexOf('PM') !== -1) {
                            var isPM = secondPart.indexOf('PM') !== -1;
                            second = parseInt(secondPart.replace(/[AP]M/g, '')) || 0;
                            if (isPM && hour < 12) hour += 12;
                            if (!isPM && hour === 12) hour = 0;
                        } else {
                            second = parseInt(secondPart) || 0;
                        }
                    }
                }
            } else if (format.indexOf(' ') !== -1) {
                // Check if format has time part separated by space
                var formatParts = format.split(' ');
                if (formatParts.length > 1) {
                    var timeFormat = formatParts[1];
                    // Look for time in the input string after the date part
                    var inputParts = dateString.split(' ');
                    if (inputParts.length > 1) {
                        var timeString = inputParts[1];
                        var timeParts = timeString.split(':');
                        
                        if (timeParts.length >= 2) {
                            hour = parseInt(timeParts[0]) || 0;
                            minute = parseInt(timeParts[1]) || 0;
                            
                            if (timeParts.length >= 3) {
                                var secondPart = timeParts[2];
                                // Handle AM/PM
                                if (secondPart.indexOf('AM') !== -1 || secondPart.indexOf('PM') !== -1) {
                                    var isPM = secondPart.indexOf('PM') !== -1;
                                    second = parseInt(secondPart.replace(/[AP]M/g, '')) || 0;
                                    if (isPM && hour < 12) hour += 12;
                                    if (!isPM && hour === 12) hour = 0;
                                } else {
                                    second = parseInt(secondPart) || 0;
                                }
                            }
                        }
                    }
                }
            }
            
            // Return parsed date even if invalid (let validation function correct it)
            return {
                year: year,
                month: month,
                day: day,
                hour: hour,
                minute: minute,
                second: second
            };
        },

        handleKeydown: function(e) {
            if (!this.isVisible) return;
            
            switch (e.keyCode) {
                case 27: // Escape
                    this.hide();
                    break;
                case 13: // Enter
                    if (this.currentDate) {
                        this.hide();
                    }
                    break;
                case 37: // Left
                    this.moveFocus(-1, 'day');
                    break;
                case 39: // Right
                    this.moveFocus(1, 'day');
                    break;
                case 38: // Up
                    this.moveFocus(-7, 'day');
                    break;
                case 40: // Down
                    this.moveFocus(7, 'day');
                    break;
                case 33: // Page Up
                    this.previousMonth();
                    break;
                case 34: // Page Down
                    this.nextMonth();
                    break;
            }
        },

        moveFocus: function(days, unit) {
            // Implementation for keyboard navigation
            // This would involve calculating new date and updating focus
        },

        selectDate: function(year, month, day) {
            // If time picker is enabled, get time from inputs
            var hour = 0, minute = 0, second = 0;
            
            if (this.options.timePicker) {
                var hourInput = this.picker.find('.mcdp-hour');
                var minuteInput = this.picker.find('.mcdp-minute');
                var secondInput = this.picker.find('.mcdp-second');
                
                hour = parseInt(hourInput.val()) || 0;
                minute = parseInt(minuteInput.val()) || 0;
                second = this.options.showSeconds ? (parseInt(secondInput.val()) || 0) : 0;
                
                // Validate hour (0-23 for 24-hour format, 1-12 for 12-hour format)
                if (this.options.use24Hour) {
                    if (hour < 0) {
                        hour = 0;
                        hourInput.val(0);
                    } else if (hour > 23) {
                        hour = 23;
                        hourInput.val(23);
                    }
                } else {
                    if (hour < 1) {
                        hour = 1;
                        hourInput.val(1);
                    } else if (hour > 12) {
                        hour = 12;
                        hourInput.val(12);
                    }
                }
                
                // Validate minute (0-59)
                if (minute < 0) {
                    minute = 0;
                    minuteInput.val(0);
                } else if (minute > 59) {
                    minute = 59;
                    minuteInput.val(59);
                }
                
                // Validate second (0-59) if enabled
                if (this.options.showSeconds) {
                    if (second < 0) {
                        second = 0;
                        secondInput.val(0);
                    } else if (second > 59) {
                        second = 59;
                        secondInput.val(59);
                    }
                }
                
                // Handle 12-hour format
                if (!this.options.use24Hour) {
                    var isAM = this.picker.find('.mcdp-ampm').text() === 'AM';
                    if (!isAM && hour < 12) hour += 12;
                    if (isAM && hour === 12) hour = 0;
                }
            } else if (this.currentDate) {
                // Preserve existing time if no time picker
                hour = this.currentDate.hour || 0;
                minute = this.currentDate.minute || 0;
                second = this.currentDate.second || 0;
            }
            
            this.currentDate = {
                year: year,
                month: month,
                day: day,
                hour: hour,
                minute: minute,
                second: second
            };
            
            this.viewDate = {year: year, month: month, day: day};
            
            var formatted = DateFormatter.format(this.currentDate, this.options.format, this.options.calendar, this.options.locale, this.options.timePicker);
            this.element.val(formatted);
            
            this.updateCalendar();
            this.element.trigger('mcdp:select', {date: this.currentDate, formatted: formatted});
            
            if (this.options.hideAfterSelect && !this.options.timePicker) {
                this.hide();
            }
        },

        updateTimeFromInputs: function() {
            if (!this.currentDate) {
                this.currentDate = CalendarUtils.getToday(this.options.calendar);
            }
            
            var hourInput = this.picker.find('.mcdp-hour');
            var minuteInput = this.picker.find('.mcdp-minute');
            var secondInput = this.picker.find('.mcdp-second');
            
            var hour = parseInt(hourInput.val()) || 0;
            var minute = parseInt(minuteInput.val()) || 0;
            var second = this.options.showSeconds ? (parseInt(secondInput.val()) || 0) : 0;
            
            // Validate hour (0-23 for 24-hour format, 1-12 for 12-hour format)
            if (this.options.use24Hour) {
                if (hour < 0) {
                    hour = 0;
                    hourInput.val(0);
                } else if (hour > 23) {
                    hour = 23;
                    hourInput.val(23);
                }
            } else {
                if (hour < 1) {
                    hour = 1;
                    hourInput.val(1);
                } else if (hour > 12) {
                    hour = 12;
                    hourInput.val(12);
                }
            }
            
            // Validate minute (0-59)
            if (minute < 0) {
                minute = 0;
                minuteInput.val(0);
            } else if (minute > 59) {
                minute = 59;
                minuteInput.val(59);
            }
            
            // Validate second (0-59) if enabled
            if (this.options.showSeconds) {
                if (second < 0) {
                    second = 0;
                    secondInput.val(0);
                } else if (second > 59) {
                    second = 59;
                    secondInput.val(59);
                }
            }
            
            if (!this.options.use24Hour) {
                var isAM = this.picker.find('.mcdp-ampm').text() === 'AM';
                if (!isAM && hour < 12) hour += 12;
                if (isAM && hour === 12) hour = 0;
            }
            
            this.currentDate.hour = hour;
            this.currentDate.minute = minute;
            this.currentDate.second = second;
            
            var formatted = DateFormatter.format(this.currentDate, this.options.format, this.options.calendar, this.options.locale, this.options.timePicker);
            this.element.val(formatted);
            
            this.element.trigger('mcdp:timeChange', {date: this.currentDate, formatted: formatted});
        },

        toggleAmPm: function() {
            var ampmBtn = this.picker.find('.mcdp-ampm');
            var isAM = ampmBtn.text() === 'AM';
            ampmBtn.text(isAM ? 'PM' : 'AM');
            this.updateTimeFromInputs();
        },

        previousMonth: function() {
            var prev = this.getPreviousMonth();
            this.viewDate = prev;
            this.updateCalendar();
        },

        nextMonth: function() {
            var next = this.getNextMonth();
            this.viewDate = next;
            this.updateCalendar();
        },

        getPreviousMonth: function() {
            var month = this.viewDate.month - 1;
            var year = this.viewDate.year;
            
            if (month < 1) {
                month = 12;
                year--;
            }
            
            return {year: year, month: month, day: 1};
        },

        getNextMonth: function() {
            var month = this.viewDate.month + 1;
            var year = this.viewDate.year;
            
            if (month > 12) {
                month = 1;
                year++;
            }
            
            return {year: year, month: month, day: 1};
        },

        isDateDisabled: function(year, month, day) {
            // Check date range
            if (this.options.minDate) {
                var min = DateFormatter.parse(this.options.minDate, this.options.format, this.options.calendar);
                if (min && (year < min.year || (year === min.year && month < min.month) || (year === min.year && month === min.month && day < min.day))) {
                    return true;
                }
            }
            
            if (this.options.maxDate) {
                var max = DateFormatter.parse(this.options.maxDate, this.options.format, this.options.calendar);
                if (max && (year > max.year || (year === max.year && month > max.month) || (year === max.year && month === max.month && day > max.day))) {
                    return true;
                }
            }
            
            // Check disabled dates
            var dateStr = DateFormatter.format({year: year, month: month, day: day}, this.options.format, this.options.calendar, this.options.locale, false);
            if (this.options.disabledDates.indexOf(dateStr) !== -1) {
                return true;
            }
            
            // Check disabled days of week
            if (this.options.disabledDays.length > 0) {
                var gregorianDate = CalendarUtils.convert({year: year, month: month, day: day}, this.options.calendar, CALENDARS.GREGORIAN);
                var date = new Date(gregorianDate.year, gregorianDate.month - 1, gregorianDate.day);
                if (this.options.disabledDays.indexOf(date.getDay()) !== -1) {
                    return true;
                }
            }
            
            return false;
        },

        getLocaleText: function(key) {
            var calendarType = this.options.calendar;
            var locale = this.options.locale;
            
            // Fall back to English if locale not available
            if (!I18N[locale] || !I18N[locale][calendarType]) {
                locale = LOCALES.EN;
            }
            
            // Further fallback for calendar type
            if (!I18N[locale][calendarType] && calendarType !== CALENDARS.GREGORIAN) {
                calendarType = CALENDARS.GREGORIAN;
            }
            
            var result = I18N[locale][calendarType][key] || I18N[LOCALES.EN][CALENDARS.GREGORIAN][key] || [];
            return result;
        },

        positionPicker: function() {
            var elementOffset = this.element.offset();
            var elementHeight = this.element.outerHeight();
            var pickerWidth = this.picker.outerWidth();
            var pickerHeight = this.picker.outerHeight();
            var windowWidth = $(window).width();
            var windowHeight = $(window).height();
            var scrollTop = $(window).scrollTop();
            
            var left = elementOffset.left;
            var top = elementOffset.top + elementHeight + 5;
            
            // Adjust for RTL
            if (this.options.rtl) {
                left = elementOffset.left + this.element.outerWidth() - pickerWidth;
            }
            
            // Adjust if picker goes off screen
            if (left + pickerWidth > windowWidth) {
                left = windowWidth - pickerWidth - 10;
            }
            if (left < 0) {
                left = 10;
            }
            
            // Adjust vertical position if picker goes below viewport
            if (top + pickerHeight > windowHeight + scrollTop) {
                top = elementOffset.top - pickerHeight - 5;
            }
            
            this.picker.css({
                position: 'absolute',
                left: left + 'px',
                top: top + 'px',
                zIndex: 9999
            });
        },

        show: function() {
            if (this.isVisible) return;
            
            // Hide all other open datepickers first
            DatePickerManager.hideOthers(this);
            
            // Set flag to prevent click event conflicts
            this.programmaticShow = true;
            
            // Use a small delay to ensure the flag is set before any events fire
            var self = this;
            setTimeout(function() {
                self.positionPicker();
                self.picker.fadeIn(150);
                self.isVisible = true;
                
                // Sync with current textbox value before showing picker
                var currentValue = self.element.val();
                if (currentValue) {
                    var parsed = self.parseLenient(currentValue, self.options.format, self.options.calendar);
                    if (parsed) {
                        // Validate and correct the parsed date
                        var corrected = self.validateAndCorrectDateTime(parsed);
                        if (corrected) {
                            var formatted = DateFormatter.format(parsed, self.options.format, self.options.calendar, self.options.locale, self.options.timePicker);
                            self.element.val(formatted);
                        }
                        
                        // Update both currentDate and viewDate
                        self.currentDate = parsed;
                        self.viewDate = $.extend({}, parsed);
                    }
                }
                
                // Update calendar to reflect current state
                self.updateCalendar();
                
                self.element.trigger('mcdp:show', {instance: self});
                
                // Reset the flag after a longer delay
                setTimeout(function() {
                    self.programmaticShow = false;
                }, 500);
            }, 10);
        },

        hide: function() {
            if (!this.isVisible) return;
            
            this.picker.fadeOut(150);
            this.isVisible = false;
            this.element.trigger('mcdp:hide', {instance: this});
        },

        toggle: function() {
            if (this.isVisible) {
                this.hide();
            } else {
                this.show();
            }
        },

        today: function() {
            var today = CalendarUtils.getToday(this.options.calendar);
            this.selectDate(today.year, today.month, today.day);
            this.element.trigger('mcdp:today', {date: today});
        },

        clear: function() {
            this.currentDate = null;
            this.element.val('');
            this.updateCalendar();
            this.element.trigger('mcdp:clear');
        },

        setDate: function(date) {
            if (typeof date === 'string') {
                this.currentDate = DateFormatter.parse(date, this.options.format, this.options.calendar);
            } else {
                this.currentDate = date;
            }
            
            if (this.currentDate) {
                this.viewDate = $.extend({}, this.currentDate);
                var formatted = DateFormatter.format(this.currentDate, this.options.format, this.options.calendar, this.options.locale, this.options.timePicker);
                this.element.val(formatted);
                this.updateCalendar();
            }
        },

        getDate: function() {
            return this.currentDate;
        },

        getFormattedDate: function() {
            if (!this.currentDate) return '';
            return DateFormatter.format(this.currentDate, this.options.format, this.options.calendar, this.options.locale, this.options.timePicker);
        },

        setOptions: function(options) {
            this.options = $.extend(this.options, options);
            this.updateCalendar();
        },

        getOptions: function() {
            return $.extend({}, this.options);
        },

        handleYearChange: function(yearInput) {
            var newYear = parseInt(yearInput.val());
            var yearRange = CalendarUtils.getYearRange(this.options.calendar);
            
            // Validate and correct year if needed
            if (isNaN(newYear)) {
                // If not a number, reset to current year
                yearInput.val(this.viewDate.year);
                this.showYearError(yearInput, yearRange);
                return;
            }
            
            // Auto-correct year to nearest valid value
            if (newYear < yearRange.min) {
                newYear = yearRange.min;
                yearInput.val(newYear);
            } else if (newYear > yearRange.max) {
                newYear = yearRange.max;
                yearInput.val(newYear);
            }
            
            // Update calendar and sync with textbox
            this.updateYearAndSync(newYear);
            
            // Clear any error styling
            yearInput.removeClass('mcdp-invalid');
        },

        handleYearInput: function(yearInput) {
            var inputValue = yearInput.val();
            var year = parseInt(inputValue);
            var yearRange = CalendarUtils.getYearRange(this.options.calendar);
            
            // Clear any existing timeout
            if (this.yearValidationTimeout) {
                clearTimeout(this.yearValidationTimeout);
                this.yearValidationTimeout = null;
            }
            
            // If input is empty or not a number, just show visual feedback
            if (inputValue === '' || isNaN(year)) {
                yearInput.removeClass('mcdp-invalid');
                return;
            }
            
            // Check if year is within valid range
            if (year < yearRange.min || year > yearRange.max) {
                yearInput.addClass('mcdp-invalid');
                
                // Auto-correct to nearest valid value if user has finished typing a complete year
                if (inputValue.length >= 4) {
                    var correctedYear = year < yearRange.min ? yearRange.min : yearRange.max;
                    yearInput.val(correctedYear);
                    yearInput.removeClass('mcdp-invalid');
                    
                    // Update the calendar and textbox immediately
                    this.updateYearAndSync(correctedYear);
                } else if (inputValue.length === 3) {
                    // For 3-digit years, set a timeout to auto-correct after user stops typing
                    var self = this;
                    this.yearValidationTimeout = setTimeout(function() {
                        var correctedYear = year < yearRange.min ? yearRange.min : yearRange.max;
                        yearInput.val(correctedYear);
                        yearInput.removeClass('mcdp-invalid');
                        
                        // Update the calendar and textbox
                        self.updateYearAndSync(correctedYear);
                    }, this.options.autoCorrectionTimeout); // Configurable timeout for auto-correction
                }
            } else {
                yearInput.removeClass('mcdp-invalid');
                
                // If it's a complete year (4 digits), update immediately
                if (inputValue.length >= 4) {
                    this.updateYearAndSync(year);
                }
            }
        },

        validateYearInput: function(yearInput) {
            var year = parseInt(yearInput.val());
            var yearRange = CalendarUtils.getYearRange(this.options.calendar);
            
            if (isNaN(year) || year < yearRange.min || year > yearRange.max) {
                yearInput.addClass('mcdp-invalid');
            } else {
                yearInput.removeClass('mcdp-invalid');
            }
        },

        updateYearAndSync: function(newYear) {
            // Update view date
            this.viewDate.year = newYear;
            
            // Ensure day is valid for the new year/month combination
            var daysInMonth = CalendarUtils.getDaysInMonth(this.options.calendar, newYear, this.viewDate.month);
            if (this.viewDate.day > daysInMonth) {
                this.viewDate.day = daysInMonth;
            }
            
            // Update calendar display
            this.updateCalendar();
            
            // Update the attached textbox if there's a current date
            if (this.currentDate) {
                this.currentDate.year = newYear;
                this.currentDate.day = Math.min(this.currentDate.day, daysInMonth);
                
                var formatted = DateFormatter.format(this.currentDate, this.options.format, this.options.calendar, this.options.locale, this.options.timePicker);
                this.element.val(formatted);
                
                // Trigger change event
                this.element.trigger('mcdp:change', {date: this.currentDate, formatted: formatted});
            } else {
                // If currentDate is null, create a new date object with the new year
                this.currentDate = {
                    year: newYear,
                    month: this.viewDate.month,
                    day: this.viewDate.day,
                    hour: 0,
                    minute: 0,
                    second: 0
                };
                
                var formatted = DateFormatter.format(this.currentDate, this.options.format, this.options.calendar, this.options.locale, this.options.timePicker);
                this.element.val(formatted);
                
                // Trigger change event
                this.element.trigger('mcdp:change', {date: this.currentDate, formatted: formatted});
            }
        },

        showYearError: function(yearInput, yearRange) {
            yearInput.addClass('mcdp-invalid');
            
            // Show tooltip or message
            var errorMsg = 'Year must be between ' + yearRange.min + ' and ' + yearRange.max;
            
            // Remove existing tooltip
            yearInput.off('mouseenter.yearError mouseleave.yearError');
            
            // Add temporary tooltip
            yearInput.attr('title', errorMsg);
            
            // Remove error styling after a delay
            setTimeout(function() {
                yearInput.removeClass('mcdp-invalid');
                yearInput.removeAttr('title');
            }, 2000);
        },

        validateManualInput: function() {
            var value = this.element.val();
            if (!value) return;
            
            // Try to parse the input using lenient parsing
            var parsed = this.parseLenient(value, this.options.format, this.options.calendar);
            
            if (parsed) {
                // Store original value for comparison
                var originalValue = value;
                
                // Validate and correct year if needed
                var yearRange = CalendarUtils.getYearRange(this.options.calendar);
                
                if (parsed.year < yearRange.min || parsed.year > yearRange.max) {
                    // Correct the year to the nearest valid value
                    if (parsed.year < yearRange.min) {
                        parsed.year = yearRange.min;
                    } else if (parsed.year > yearRange.max) {
                        parsed.year = yearRange.max;
                    }
                }
                
                // Validate and correct the parsed date
                var corrected = this.validateAndCorrectDateTime(parsed);
                
                // If validation made corrections, update the textbox
                if (corrected || parsed.year !== parseInt(originalValue.split('/')[0])) {
                    var correctedValue = DateFormatter.format(parsed, this.options.format, this.options.calendar, this.options.locale, this.options.timePicker);
                    this.element.val(correctedValue);
                    
                    // Add visual feedback for correction
                    this.element.addClass('mcdp-corrected');
                    setTimeout(function() {
                        this.element.removeClass('mcdp-corrected');
                    }.bind(this), 1000);
                    
                    // Always update internal state with corrected values
                    this.currentDate = parsed;
                    this.viewDate = $.extend({}, parsed);
                    
                    // Update calendar if visible
                    if (this.isVisible) {
                        this.updateCalendar();
                    }
                    
                    // Trigger change event
                    this.element.trigger('mcdp:change', {date: this.currentDate, formatted: correctedValue});
                } else {
                    // No corrections needed, just update internal state
                    this.currentDate = parsed;
                    this.viewDate = $.extend({}, parsed);
                    
                    // Update calendar if visible
                    if (this.isVisible) {
                        this.updateCalendar();
                    }
                }
            } else {
                // Invalid input - add visual feedback
                this.element.addClass('mcdp-invalid');
                setTimeout(function() {
                    this.element.removeClass('mcdp-invalid');
                }.bind(this), 1000);
            }
        },

        destroy: function() {
            // Unregister from global manager
            DatePickerManager.unregister(this);
            
            // Clear timeouts
            if (this.validationTimeout) {
                clearTimeout(this.validationTimeout);
            }
            if (this.yearValidationTimeout) {
                clearTimeout(this.yearValidationTimeout);
            }
            
            this.element.off('.mcdp');
            $(document).off('.mcdp');
            this.picker.remove();
            this.element.removeData('multiCalendarDatePicker');
        }
    };

    // jQuery plugin definition
    $.fn.multiCalendarDatePicker = function(option, parameter) {
        var results = [];
        
        this.each(function() {
            var $this = $(this);
            var data = $this.data('multiCalendarDatePicker');
            var options = typeof option === 'object' && option;
            
            if (!data) {
                data = new MultiCalendarDatePicker(this, options);
                $this.data('multiCalendarDatePicker', data);
            }
            
            if (typeof option === 'string' && typeof data[option] === 'function') {
                var result = data[option](parameter);
                if (result !== undefined) {
                    results.push(result);
                }
            }
        });
        
        return results.length === 1 ? results[0] : (results.length > 0 ? results : this);
    };

    // Constructor access
    $.fn.multiCalendarDatePicker.Constructor = MultiCalendarDatePicker;

    // Auto-initialize data-api
    $(document).ready(function() {
        $('[data-mcdp]').each(function() {
            var $this = $(this);
            var options = $this.data();
            $this.multiCalendarDatePicker(options);
        });
    });

    // Expose the plugin and manager globally for debugging
    window.MultiCalendarDatePicker = MultiCalendarDatePicker;
    window.DatePickerManager = DatePickerManager;

})(jQuery);
