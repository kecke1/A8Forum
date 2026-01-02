
// Gauntlet Opponents SPA (Local Storage only)
// - Name: Select2 with local suggestions + tagging
// - Rating: button-styled radios (Bootstrap 5 btn-check)
// - Track: Select2 via AJAX to /Tracks/Select2 (service-backed)
// - Time: optional
// - Reset database: clears Local Storage only

(function () {
    const STORAGE_KEY = 'A8FORUM.gauntletOpponents';

    // Cache DOM elements
    const $name = $('#op-name');
    const $time = $('#op-time');
    const $track = $('#op-track');
    const $save = $('#save-btn');
    const $clear = $('#clear-btn');
    const $reset = $('#reset-db-btn');
    const $list = $('#op-list');
    const $count = $('#op-count');

    // --- Local Storage helpers ---
    function loadOpponents() {
        try {
            const raw = localStorage.getItem(STORAGE_KEY);
            return raw ? JSON.parse(raw) : [];
        } catch {
            return [];
        }
    }
    function saveOpponents(items) {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
    }
    function resetDatabase() {
        localStorage.removeItem(STORAGE_KEY);
        render();
        clearForm();
        refreshNameSelect2Data(); // keep Name suggestions in sync
    }

    // --- Select2: Track via AJAX (/Tracks/Select2) ---
    function initTrackSelect2() {
        $track.select2({
            theme: 'bootstrap-5',
            width: '100%',
            placeholder: $track.data('placeholder') || 'Select a track',
            allowClear: true,
            minimumInputLength: 0,
            ajax: {
                url: '/Tracks/Select2',
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return { term: params.term };
                },
                processResults: function (data) {
                    return { results: data };
                },
                cache: true
            }
        });
    }

    // --- Select2: Name with local suggestions + tagging ---
    function initNameSelect2() {
        const data = buildNameDataFromLocal();

        $name.select2({
            theme: 'bootstrap-5',
            width: '100%',
            placeholder: $name.data('placeholder') || 'Search or add a name',
            allowClear: true,
            tags: true,
            data: data,
            matcher: function (params, dataItem) {
                const term = (params.term || '').trim().toLowerCase();
                if (term === '') return dataItem;
                const text = (dataItem.text || '').toLowerCase();
                return text.indexOf(term) > -1 ? dataItem : null;
            },
            createTag: function (params) {
                const term = (params.term || '').trim();
                if (!term) return null;
                const exists = data.some(d => d.id.toLowerCase() === term.toLowerCase());
                return exists ? null : { id: term, text: term, newTag: true };
            }
        });

        // When a name is selected (existing/new), load the opponent if it exists
        $name.on('change', function () {
            const selectedName = ($name.val() || '').trim();

            if (!selectedName) {
                clearRatingRadios(); // [rating radio change]
                $time.val('');
                $track.val(null).trigger('change');
                $save.text('Save');
                return;
            }

            const match = loadOpponents().find(x => equalsIgnoreCase(x.name, selectedName));
            if (match) {
                // populate for edit
                setRatingRadio(match.rating);   // [rating radio change]
                $time.val(match.time || '');
                setTrackById(match.trackId, match.trackName);
                $save.text('Update');
            } else {
                // new name → clear other fields
                clearRatingRadios();            // [rating radio change]
                $time.val('');
                $track.val(null).trigger('change');
                $save.text('Save');
            }
        });
    }

    // Build Select2 data for Name from Local Storage
    function buildNameDataFromLocal() {
        const unique = {};
        loadOpponents().forEach(o => {
            const key = (o.name || '').toLowerCase();
            if (key && !unique[key]) unique[key] = { id: o.name, text: o.name };
        });
        return Object.values(unique);
    }

    // Refresh Name Select2 when Local Storage changes (after add/update/delete/reset)
    function refreshNameSelect2Data() {
        const current = $name.val(); // keep current selection
        const newData = buildNameDataFromLocal();

        $name.find('option').not(':first').remove();
        newData.forEach(d => $name.append(new Option(d.text, d.id, false, false)));
        $name.trigger('change.select2');

        if (current) {
            const exists = newData.some(d => d.id.toLowerCase() === current.toLowerCase());
            if (exists) $name.val(current).trigger('change');
        }
    }

    // --- Rating radio helpers ---  [rating radio change]
    function getSelectedRating() {
        return $('input[name="rating"]:checked').val() || '';
    }
    function setRatingRadio(value) {
        const val = (value || '').toLowerCase();
        $('input[name="rating"]').prop('checked', false);
        if (val === 'beatable') $('#rating-beatable').prop('checked', true);
        else if (val === 'difficult') $('#rating-difficult').prop('checked', true);
        else if (val === 'impossible') $('#rating-impossible').prop('checked', true);
    }
    function clearRatingRadios() {
        $('input[name="rating"]').prop('checked', false);
    }

    // --- Utilities ---
    const equalsIgnoreCase = (a, b) => (a ?? '').toLowerCase() === (b ?? '').toLowerCase();

    function validate() {
        const name = ($name.val() || '').trim();
        const rating = getSelectedRating();     // [rating radio change]
        if (!name) { alert('Name is required.'); $name.select2('open'); return false; }
        if (!rating) { alert('Rating is required.'); $('#rating-beatable').focus(); return false; }
        return true;
    }

    function clearForm() {
        $name.val(null).trigger('change');
        clearRatingRadios();                    // [rating radio change]
        $time.val('');
        $track.val(null).trigger('change');
        $save.text('Save');
    }

    function toBadge(rating) {
        switch ((rating || '').toLowerCase()) {
            case 'beatable': return '<span class="badge bg-success">Beatable</span>';
            case 'difficult': return '<span class="badge bg-warning text-dark">Difficult</span>';
            case 'impossible': return '<span class="badge bg-danger">Impossible</span>';
            default: return '';
        }
    }

    // --- CRUD ---
    function upsertOpponent(op) {
        const items = loadOpponents();
        const idx = items.findIndex(x => equalsIgnoreCase(x.name, op.name));
        if (idx >= 0) { items[idx] = op; } else { items.push(op); }
        saveOpponents(items);
        render();
        refreshNameSelect2Data();
    }

    function removeOpponent(name) {
        const items = loadOpponents().filter(x => !equalsIgnoreCase(x.name, name));
        saveOpponents(items);
        render();
        refreshNameSelect2Data();
        if (equalsIgnoreCase(($name.val() || '').trim(), name)) {
            clearForm();
        }
    }

    // --- Rendering list ---
    function render() {
        const items = loadOpponents()
            .sort((a, b) => a.name.localeCompare(b.name, undefined, { sensitivity: 'base' }));

        $list.empty();
        $count.text(items.length);

        items.forEach(op => {
            const li = $(`
<li class="list-group-item d-flex justify-content-between align-items-center">
    <div class="me-3">
        <div class="fw-semibold">${escapeHtml(op.name)} ${toBadge(op.rating)}</div>
        <div class="small text-muted">
            ${op.time ? `Time: ${escapeHtml(op.time)}; ` : ''}
            ${op.trackName ? `Track: ${escapeHtml(op.trackName)}` : ''}
        </div>
    </div>
    <div class="btn-group btn-group-sm">
        <button class="btn btn-outline-primary edit-btn">Edit</button>
        <button class="btn btn-outline-danger delete-btn">Delete</button>
    </div>
</li>`);
            li.find('.edit-btn').on('click', () => {
                $name.val(op.name).trigger('change');
                setRatingRadio(op.rating);      // [rating radio change]
                $time.val(op.time || '');
                setTrackById(op.trackId, op.trackName);
                $save.text('Update');
                //$name.select2('open');
            });
            li.find('.delete-btn').on('click', () => {
                if (confirm(`Delete opponent "${op.name}"?`)) {
                    removeOpponent(op.name);
                }
            });
            $list.append(li);
        });
    }

    // Ensure Track Select2 shows a selected track even if it isn’t in the DOM yet
    function setTrackById(trackId, trackName) {
        if (!trackId) { $track.val(null).trigger('change'); return; }
        const exists = $track.find(`option[value="${trackId}"]`).length > 0;
        if (!exists) {
            const opt = new Option(trackName || trackId, trackId, true, true);
            $track.append(opt).trigger('change');
        } else {
            $track.val(trackId).trigger('change');
        }
    }

    function escapeHtml(s) {
        return (s ?? '').replace(/[&<>"']/g, m => ({
            '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;'
        }[m]));
    }

    // --- Buttons ---
    function wireButtons() {
        $save.on('click', () => {
            if (!validate()) return;

            const opponent = {
                name: ($name.val() || '').trim(),
                rating: getSelectedRating(),    // [rating radio change]
                time: ($time.val() || '').trim(),
                trackId: $track.val(),
                trackName: $track.find(':selected').text() || ''
            };

            const exists = loadOpponents().some(x => equalsIgnoreCase(x.name, opponent.name));
            upsertOpponent(opponent);
            if (!exists) {
                clearForm();
            } else {
                $save.text('Update');
            }
        });

        $clear.on('click', clearForm);

        $reset.on('click', () => {
            if (confirm('This will remove all opponents stored in your browser. Continue?')) {
                resetDatabase();
            }
        });
    }

    // --- Init ---
    $(document).ready(function () {
        initTrackSelect2();  // Track (remote via /Tracks/Select2)
        initNameSelect2();   // Name (local suggestions + tagging)
        wireButtons();
        render();
    });
})();
