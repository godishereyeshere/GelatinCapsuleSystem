// =====================================================
// Personnel Select2 Helper
// =====================================================

(function () {
    'use strict';

    function initPersonnelSelect(selector, position) {
        const $el = $(selector);
        if (!$el.length) return;

        $el.select2({
            placeholder: '— جستجو و انتخاب —',
            allowClear: true,
            minimumInputLength: 0,
            language: {
                inputTooShort: function () { return 'تایپ کنید...'; },
                noResults: function () { return 'نتیجه‌ای یافت نشد'; },
                searching: function () { return 'در حال جستجو...'; },
                loadingMore: function () { return 'در حال بارگذاری...'; }
            },
            ajax: {
                url: '/api/personnel/search',
                dataType: 'json',
                delay: 300,
                data: function (params) {
                    return {
                        q: params.term || '',
                        position: position || '',
                        page: params.page || 1,
                        pageSize: 30
                    };
                },
                processResults: function (data, params) {
                    params.page = params.page || 1;
                    return {
                        results: data.results,
                        pagination: { more: data.pagination.more }
                    };
                },
                cache: true
            }
        });
    }

    window.initPersonnelSelect = initPersonnelSelect;
})();