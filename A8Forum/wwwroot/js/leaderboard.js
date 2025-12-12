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
    if (d < bestDiff) {bestDiff = d; bestIdx = i; }
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
    range: {min: 0, max: sliderValues.length - 1 },
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
    minHidden.dispatchEvent(new Event('change', {bubbles: true }));
    maxHidden.dispatchEvent(new Event('change', {bubbles: true }));
                }

    // Update continuously and on set/change
    slider.noUiSlider.on('update', commit);
    slider.noUiSlider.on('set', commit);
    slider.noUiSlider.on('change', commit);
            }

    // --- Client-side dropdowns: show/hide pre-rendered group tables; persist selection in localStorage ---
    function setupDropdown(tabKey, selectId, itemSelector) {
                var lsKey = 'sprintLb.' + tabKey + '.idx';
    var select = document.getElementById(selectId);
    var items = Array.from(document.querySelectorAll(itemSelector));

    if (!select || items.length === 0) return;

    // Restore selection; fallback to 0 (first item)
    var saved = localStorage.getItem(lsKey);
    var idx = parseInt(saved, 10);
    if (isNaN(idx) || idx < 0 || idx >= items.length) idx = 0;

    select.value = String(idx);
    showIndex(idx);

    select.addEventListener('change', function () {
                    var newIdx = parseInt(select.value, 10);
    if (isNaN(newIdx)) return;
    showIndex(newIdx);
    localStorage.setItem(lsKey, String(newIdx));
                });

    function showIndex(i) {
        items.forEach(function (el) {
            if (parseInt(el.getAttribute('data-index'), 10) === i) {
                el.classList.remove('d-none');
            } else {
                el.classList.add('d-none');
            }
        });
                }
            }

    // Use your existing calls
    setupDropdown('byTrack', 'byTrackSelect', '.track-group-table[data-tab="byTrack"]');
    setupDropdown('bestLaps', 'byMemberSelect', '.member-group-table[data-tab="bestLaps"]');

    // Persist ActiveTab when switching tabs
    var hidden = document.getElementById('Filter_ActiveTab');
    document.querySelectorAll('#leaderboardTabs [data-bs-toggle="tab"]').forEach(function (btn) {
        btn.addEventListener('shown.bs.tab', function (e) {
            hidden && (hidden.value = e.target.getAttribute('data-tab-key'));
        });
            });

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