(function initTrackSelects() {
    const $ = window.jQuery;

    function init($el) {
        const ajaxUrl = $el.data('ajax-url');
        const resolveUrl = $el.data('resolve-url');
        const placeholder = $el.data('placeholder') || 'Select track';
        const allowClear = String($el.data('allow-clear')) === 'true';
        const width = $el.data('width') || '100%';

        $el.select2({
            width,
            placeholder,
            allowClear,
            theme: 'bootstrap-5',  // if you use the bootstrap theme
            ajax: {
                url: ajaxUrl,
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term || '',
                        page: params.page || 1
                    };
                },
                processResults: function (data, params) {
                    params.page = params.page || 1;
                    return {
                        results: data.results,
                        pagination: { more: !!data.pagination?.more }
                    };
                },
                cache: true
            },
            // Let Select2 create tags only from server results
            minimumInputLength: 0
        });

        // Resolve preselected values: if there are empty <option selected>, fetch text
        const selectedVals = ($el.val() || []).filter(Boolean);
        const isMultiple = $el.prop('multiple') === true;

        if (selectedVals.length > 0) {
            // Some SSR options might have text="" because TagHelper didn’t know display text
            const needResolve = [].some.call($el.find('option:selected'), o => !o.text || o.text.trim() === '');
            if (needResolve) {
                const url = new URL(resolveUrl, window.location.origin);
                selectedVals.forEach(v => url.searchParams.append('ids', v));

                fetch(url.toString())
                    .then(r => r.json())
                    .then(items => {
                        // Replace placeholder selected options with resolved text
                        if (isMultiple) {
                            $el.find('option:selected').remove();
                            items.forEach(it => {
                                const opt = new Option(it.text, it.id, true, true);
                                $el.append(opt);
                            });
                        } else {
                            const it = items[0];
                            if (it) {
                                $el.find('option:selected').remove();
                                const opt = new Option(it.text, it.id, true, true);
                                $el.append(opt);
                            }
                        }
                        $el.trigger('change', { resolved: true });
                    })
                    .catch(() => {/* ignore */ });
            }
        }
    }

    function initAll(scope) {
        const $scope = scope ? $(scope) : $(document);
        $scope.find('select[data-select2="track"]').each(function () { init($(this)); });
    }

    // Initial page load
    $(document).ready(() => initAll());
})();
``
