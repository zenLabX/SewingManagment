class AdvancedSearchManager {
    constructor(form) {
        this.advForm = form;
        if (!this.advForm) return;

        this.conditionContainer = document.getElementById('fieldSearchContainer');
        this.addConditionBtn = document.getElementById('addConditionBtn');
        this.conditionTemplate = document.getElementById('condition-template');

        this.sortContainer = document.getElementById('sortContainer');
        this.addSortBtn = document.getElementById('addSortBtn');
        this.sortTemplate = document.getElementById('sort-template');

        this.advancedResetBtn = document.getElementById('advancedResetBtn');
        this.modeField = document.getElementById('modeField');
        this.modeMulti = document.getElementById('modeMulti');
        this.fieldModeContainer = document.getElementById('field-search-mode');
        this.multiModeContainer = document.getElementById('multi-search-mode');
    }

    init() {
        if (!this.advForm) return;
        this.attachRowHandlers(document);
        this.updateIndexes();
        this.setMode(this.modeField && this.modeField.checked ? 'field' : 'multi');

        if (this.modeField) this.modeField.addEventListener('change', (e) => this.setMode(e.target.value));
        if (this.modeMulti) this.modeMulti.addEventListener('change', (e) => this.setMode(e.target.value));

        if (this.addConditionBtn) this.addConditionBtn.addEventListener('click', () => this.addCondition());
        if (this.addSortBtn) this.addSortBtn.addEventListener('click', () => this.addSort());
        if (this.advancedResetBtn) this.advancedResetBtn.addEventListener('click', () => this.reset());

        this.advForm.addEventListener('submit', (e) => this.onSubmit(e));
    }

    updateIndexes() {
        const condRows = this.conditionContainer.querySelectorAll('.condition-row');
        condRows.forEach((row, idx) => {
            const field = row.querySelector('.condition-field');
            const op = row.querySelector('.condition-operator');
            const val = row.querySelector('.condition-value');
            if (field) field.name = `Conditions[${idx}].Field`;
            if (op) op.name = `Conditions[${idx}].Operator`;
            if (val) val.name = `Conditions[${idx}].Value`;
        });

        const sortRows = this.sortContainer.querySelectorAll('.sort-row');
        sortRows.forEach((row, idx) => {
            const field = row.querySelector('.sort-field');
            const dir = row.querySelector('.sort-direction');
            if (field) field.name = `Sorts[${idx}].Field`;
            if (dir) dir.name = `Sorts[${idx}].Direction`;
        });
    }

    attachRowHandlers(container) {
        container.querySelectorAll('.btn-remove-condition').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const row = e.target.closest('.condition-row');
                if (row) {
                    row.remove();
                    this.updateIndexes();
                }
            });
        });

        container.querySelectorAll('.btn-remove-sort').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const row = e.target.closest('.sort-row');
                if (row) {
                    row.remove();
                    this.updateIndexes();
                }
            });
        });
    }

    setMode(mode) {
        if (mode === 'field') {
            this.fieldModeContainer.style.display = '';
            this.multiModeContainer.style.display = 'none';
        } else {
            this.fieldModeContainer.style.display = 'none';
            this.multiModeContainer.style.display = '';
        }
        this.sanitizeHiddenModeInputs();
    }

    sanitizeHiddenModeInputs() {
        if (this.fieldModeContainer.style.display === 'none') {
            this.fieldModeContainer.querySelectorAll('select, input').forEach(el => el.name = '');
        } else {
            const firstCond = this.conditionContainer.querySelector('.condition-row');
            if (firstCond) {
                const field = firstCond.querySelector('.condition-field');
                const op = firstCond.querySelector('.condition-operator');
                const val = firstCond.querySelector('.condition-value');
                if (field) field.name = 'Conditions[0].Field';
                if (op) op.name = 'Conditions[0].Operator';
                if (val) val.name = 'Conditions[0].Value';
            }
        }

        if (this.multiModeContainer.style.display === 'none') {
            const gc = document.getElementById('globalConnector');
            if (gc) gc.name = '';
            this.sortContainer.querySelectorAll('select').forEach(el => el.name = '');
        } else {
            const gc = document.getElementById('globalConnector');
            if (gc) gc.name = 'GlobalConnector';
            this.updateIndexes();
        }
    }

    addCondition() {
        const clone = this.conditionTemplate.content.cloneNode(true);
        this.conditionContainer.appendChild(clone);
        this.updateIndexes();
        this.attachRowHandlers(this.conditionContainer);
    }

    addSort() {
        const clone = this.sortTemplate.content.cloneNode(true);
        this.sortContainer.appendChild(clone);
        this.updateIndexes();
        this.attachRowHandlers(this.sortContainer);
    }

    reset() {
        const condRows = this.conditionContainer.querySelectorAll('.condition-row');
        condRows.forEach((r, i) => { if (i > 0) r.remove(); else {
            r.querySelectorAll('select, input').forEach(el => el.value = '');
        }});

        const sortRows = this.sortContainer.querySelectorAll('.sort-row');
        sortRows.forEach((r, i) => { if (i > 0) r.remove(); else {
            r.querySelectorAll('select').forEach(el => el.value = '');
        }});

        const gc = document.getElementById('globalConnector');
        if (gc) gc.value = 'AND';
        this.updateIndexes();
        this.sanitizeHiddenModeInputs();
    }

    onSubmit(e) {
        this.updateIndexes();
        const condRows = this.conditionContainer.querySelectorAll('.condition-row');
        const hasValid = Array.from(condRows).some(r => {
            const f = r.querySelector('.condition-field');
            const v = r.querySelector('.condition-value');
            return f && f.value && v && v.value;
        });

        if (!hasValid) {
            condRows.forEach((r) => {
                r.querySelectorAll('select, input').forEach(el => el.name = '');
            });
        }

        const sortRows = this.sortContainer.querySelectorAll('.sort-row');
        const hasSort = Array.from(sortRows).some(r => r.querySelector('.sort-field') && r.querySelector('.sort-field').value);
        if (!hasSort) sortRows.forEach(r => r.querySelectorAll('select').forEach(el => el.name = ''));

        this.sanitizeHiddenModeInputs();
    }
}

