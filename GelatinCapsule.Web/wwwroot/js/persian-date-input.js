// =====================================================
// Persian Date Input Formatter
// خودکار تایپ کاربر رو به فرمت 1405/05/06 تبدیل می‌کنه
// =====================================================

(function () {
    'use strict';

    function formatPersianDate(value) {
        // فقط اعداد رو نگه دار
        let digits = value.replace(/\D/g, '');

        // حداکثر 8 رقم
        if (digits.length > 8) {
            digits = digits.substring(0, 8);
        }

        // اسلش خودکار
        if (digits.length >= 5) {
            return digits.substring(0, 4) + '/' + digits.substring(4, 6) +
                (digits.length >= 7 ? '/' + digits.substring(6, 8) : '');
        }

        return digits;
    }

    function applyToInput(input) {
        if (input.dataset.persianDateInitialized === '1') return;
        input.dataset.persianDateInitialized = '1';

        input.addEventListener('input', function (e) {
            const cursorPos = e.target.selectionStart;
            const oldValue = e.target.value;
            const newValue = formatPersianDate(oldValue);
            e.target.value = newValue;

            // اگه اسلش اضافه شد، مکان cursor رو تنظیم کن
            const diff = newValue.length - oldValue.length;
            if (diff > 0 && cursorPos !== null) {
                e.target.setSelectionRange(cursorPos + diff, cursorPos + diff);
            }
        });

        input.addEventListener('keydown', function (e) {
            // اجازه به Backspace, Delete, Arrow, Tab
            const allowedKeys = ['Backspace', 'Delete', 'ArrowLeft', 'ArrowRight',
                'Tab', 'Enter', 'Home', 'End'];
            if (allowedKeys.includes(e.key)) return;
            if (e.ctrlKey || e.metaKey) return;

            // فقط اعداد اجازه داشته باشن
            if (!/^\d$/.test(e.key)) {
                e.preventDefault();
            }
        });

        // input های موجود رو هم فرمت کن
        if (input.value) {
            input.value = formatPersianDate(input.value);
        }
    }

    function initAll() {
        document.querySelectorAll('[data-persian-date]').forEach(applyToInput);
    }

    // در DOMContentLoaded
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initAll);
    } else {
        initAll();
    }

    // برای input هایی که داینامیک اضافه میشن
    window.initPersianDateInputs = initAll;
})();