document.addEventListener('DOMContentLoaded', function () {
    (function () {
        // Allowed VIP values
        var sliderValues = [0, 13, 14, 15];

        // Map position (0..3) -> VIP value
        function positionToValue(pos) {
            var i = Math.round(Number(pos));
            return sliderValues[Math.max(0, Math.min(sliderValues.length - 1, i))];
        }

        // Map VIP value -> nearest position (0..3)
        function nearestPositionFor(value) {
            var n = Number(value);
            var bestIdx = 0, bestDiff = Infinity;
            for (var i = 0; i < sliderValues.length; i++) {
                var d = Math.abs(sliderValues[i] - n);
                if (d < bestDiff) { bestDiff = d; bestIdx = i; }
            }
            return bestIdx;
        }

        // Tooltip formatter: show "No VIP" for 0, otherwise the numeric value
        var tooltipFormatter = {
            to: function (pos) {
                var v = positionToValue(pos);
                return v === 0 ? 'No VIP' : String(v);
            },
            from: function () { return 0; } // not used
        };

        var slider = document.getElementById('vipSlider');
        var minHidden = document.getElementById('Filter_VipLevelMin');
        var maxHidden = document.getElementById('Filter_VipLevelMax');

        if (slider && minHidden && maxHidden) {
            // Read initial VIP values from hidden fields and map to positions
            var startMinValue = parseInt(minHidden.value, 10);
            var startMaxValue = parseInt(maxHidden.value, 10);
            if (!Number.isFinite(startMinValue)) startMinValue = 0;
            if (!Number.isFinite(startMaxValue)) startMaxValue = 15;

            var startMinPos = nearestPositionFor(startMinValue);
            var startMaxPos = nearestPositionFor(startMaxValue);
            var startA = Math.min(startMinPos, startMaxPos);
            var startB = Math.max(startMinPos, startMaxPos);

            // Create slider over positions 0..3
            noUiSlider.create(slider, {
                start: [startA, startB],
                step: 1,
                connect: true,
                range: { min: 0, max: sliderValues.length - 1 },
                behaviour: 'tap-drag',

                // Tooltips for both handles
                // tooltips: [tooltipFormatter, tooltipFormatter],

                // Pips labeled "No VIP, 13, 14, 15"
                pips: {
                    mode: 'values',
                    values: [0, 1, 2, 3],
                    density: 100,
                    format: {
                        to: function (pos) {
                            var v = sliderValues[pos];
                            return v === 0 ? 'No VIP' : String(v);
                        }
                    }
                }
            });

            // Keep hidden fields updated with ACTUAL VIP values (0, 13, 14, 15)
            function commit(values) {
                // 'values' are positions (as strings) because we didn't set slider 'format'
                var posMin = Math.round(parseFloat(values[0]));
                var posMax = Math.round(parseFloat(values[1]));
                var vMin = positionToValue(posMin);
                var vMax = positionToValue(posMax);
                minHidden.value = String(vMin);
                maxHidden.value = String(vMax);
                // Trigger change to cooperate with client validation
                minHidden.dispatchEvent(new Event('change', { bubbles: true }));
                maxHidden.dispatchEvent(new Event('change', { bubbles: true }));
            }

            // Update continuously and on set/change
            slider.noUiSlider.on('update', commit);
            slider.noUiSlider.on('set', commit);
            slider.noUiSlider.on('change', commit);
        }

        // -------------------------------
        // Query-string + dropdown handling
        // -------------------------------
        function getPageKey() {
            var p = (window.location.pathname || '').toLowerCase();
            if (p.includes('gauntletleaderboard')) return 'gauntletLb';
            if (p.includes('sprintleaderboard')) return 'sprintLb';
            return 'leaderboard';
        }

        function getParams() {
            try {
                return new URLSearchParams(window.location.search || '');
            } catch {
                return new URLSearchParams();
            }
        }

        function getTabFromQuery(params) {
            // accept both ?ActiveTab=bestLaps and ?tab=bestLaps
            return (params.get('ActiveTab') || params.get('tab') || '').trim();
        }

        function setOrDeleteParam(url, key, value) {
            if (value === null || value === undefined || String(value).trim() === '') {
                url.searchParams.delete(key);
            } else {
                url.searchParams.set(key, String(value));
            }
        }

        function replaceUrlKeepingHash(mutator) {
            var url = new URL(window.location.href);
            mutator(url);
            window.history.replaceState({}, '', url.toString());
        }

        // Activate a bootstrap tab button by its data-tab-key (e.g. "byTrack", "bestLaps", "total")
        function activateTabByKey(tabKeyRaw) {
            var tabKey = (tabKeyRaw || '').trim();
            if (!tabKey) return;

            var btn = document.querySelector('#leaderboardTabs [data-bs-toggle="tab"][data-tab-key="' + tabKey + '"]');
            if (!btn) return;

            // Prefer Bootstrap API if present
            if (window.bootstrap && window.bootstrap.Tab) {
                window.bootstrap.Tab.getOrCreateInstance(btn).show();
            } else {
                btn.click();
            }
        }

        // --- Client-side dropdowns: show/hide pre-rendered group tables; persist selection in localStorage; support URL param ---
        function setupDropdown(tabKey, selectId, itemSelector, queryParamName) {
            var pageKey = getPageKey();
            var lsKey = pageKey + '.' + tabKey + '.idx';

            var select = document.getElementById(selectId);
            var items = Array.from(document.querySelectorAll(itemSelector));
            if (!select || items.length === 0) return;

            // Find the hidden input in the form to sync with
            var hiddenInputId = 'Filter_' + queryParamName.charAt(0).toUpperCase() + queryParamName.slice(1);
            var hiddenInput = document.getElementById(hiddenInputId);

            var params = getParams();

            function findIndexFromQuery() {
                var raw = (params.get(queryParamName) || '').trim();
                if (!raw) return null;

                // numeric => index
                var asInt = parseInt(raw, 10);
                if (!Number.isNaN(asInt) && String(asInt) === raw) {
                    if (asInt >= 0 && asInt < items.length) return asInt;
                }

                // otherwise treat as name: match data-name OR option text
                var lower = raw.toLowerCase();
                for (var i = 0; i < items.length; i++) {
                    var dn = (items[i].getAttribute('data-name') || '').trim().toLowerCase();
                    if (dn && dn === lower) return i;
                }

                // fallback: match option label
                for (var j = 0; j < select.options.length; j++) {
                    var t = (select.options[j].text || '').trim().toLowerCase();
                    if (t && t === lower) return j;
                }

                return null;
            }

            function readIndexWithPriority() {
                // 1) URL param wins
                var qIdx = findIndexFromQuery();
                if (qIdx !== null) return qIdx;

                // 2) localStorage fallback
                var saved = localStorage.getItem(lsKey);
                var idx = parseInt(saved, 10);
                if (Number.isNaN(idx) || idx < 0 || idx >= items.length) idx = 0;
                return idx;
            }

            function showIndex(i) {
                items.forEach(function (el) {
                    if (parseInt(el.getAttribute('data-index'), 10) === i) {
                        el.classList.remove('d-none');
                    } else {
                        el.classList.add('d-none');
                    }
                });
            }

            // initial selection
            var idx = readIndexWithPriority();
            select.value = String(idx);
            showIndex(idx);
            if (hiddenInput) hiddenInput.value = String(idx);

            // keep localStorage + URL updated on change
            select.addEventListener('change', function () {
                var newIdx = parseInt(select.value, 10);
                if (Number.isNaN(newIdx)) return;

                showIndex(newIdx);
                localStorage.setItem(lsKey, String(newIdx));

                // Update URL so the selection is shareable/bookmarkable
                replaceUrlKeepingHash(function (url) {
                    setOrDeleteParam(url, queryParamName, String(newIdx));

                    // also keep the currently active tab in the URL if possible
                    var hidden = document.getElementById('Filter_ActiveTab');
                    var tabVal = hidden && hidden.value ? hidden.value : '';
                    if (tabVal) {
                        setOrDeleteParam(url, 'ActiveTab', tabVal);
                        setOrDeleteParam(url, 'tab', null); // avoid duplicates
                    }
                });
            });
        }

        // Use your existing calls, but now with URL param names:
        // ?track=... controls By Track dropdown
        // ?member=... controls Best Laps dropdown
        setupDropdown('byTrack', 'byTrackSelect', '.track-group-table[data-tab="byTrack"]', 'track');
        setupDropdown('bestLaps', 'byMemberSelect', '.member-group-table[data-tab="bestLaps"]', 'member');

        // Persist ActiveTab when switching tabs (and keep URL in sync)
        var hidden = document.getElementById('Filter_ActiveTab');
        document.querySelectorAll('#leaderboardTabs [data-bs-toggle="tab"]').forEach(function (btn) {
            btn.addEventListener('shown.bs.tab', function (e) {
                var newTab = e.target.getAttribute('data-tab-key');
                if (hidden) hidden.value = newTab;

                replaceUrlKeepingHash(function (url) {
                    setOrDeleteParam(url, 'ActiveTab', newTab);
                    setOrDeleteParam(url, 'tab', null); // avoid duplicates
                });
            });
        });

        // On load: if query specifies a tab, activate it (client-side)
        // This complements server-side selection, and helps if someone uses ?tab=... without binding to the server.
        var initialParams = getParams();
        var initialTab = getTabFromQuery(initialParams);
        if (initialTab) {
            activateTabByKey(initialTab);
            if (hidden) hidden.value = initialTab;
        }

        // Leaderboard Date toggle UX
        const use = document.getElementById('Filter_UseLeaderboardDate');
        const date = document.getElementById('Filter_LeaderboardDate');
        function syncDate() {
            const on = use?.checked;
            if (!date) return;
            date.disabled = !on;
            date.required = on;
            if (!on) date.value = '';
        }
        syncDate();
        use?.addEventListener('change', syncDate);
    })();
});