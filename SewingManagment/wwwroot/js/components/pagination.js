// Lightweight Pagination component (ES module)
// Usage:
// import { Pagination } from '/js/components/pagination.js';
// const p = new Pagination({ totalPages: 10, currentPage: 1, onPageChange: (n) => {} });
// p.render(document.getElementById('paginationContainer'))

export class Pagination {
    constructor({ totalPages = 1, currentPage = 1, onPageChange = null, windowSize = 2 } = {}) {
        this.totalPages = Math.max(1, Number(totalPages) || 1);
        this.currentPage = Math.min(Math.max(1, Number(currentPage) || 1), this.totalPages);
        this.onPageChange = typeof onPageChange === 'function' ? onPageChange : null;
        this.container = null;
        // windowSize: how many pages to show on each side of current (e.g., 2 -> current ±2)
        this.windowSize = Math.max(1, Number(windowSize) || 2);
    }

    render(container) {
        let el = null;
        if (typeof container === 'string') el = document.querySelector(container);
        else el = container;
        if (!el) return;

        const nav = document.createElement('nav');
        nav.setAttribute('aria-label', '分頁導航');

        const ul = document.createElement('ul');
        ul.className = 'pagination justify-content-center';
        nav.appendChild(ul);

        const makeItem = (page, text, { disabled = false, active = false } = {}) => {
            const li = document.createElement('li');
            li.className = 'page-item' + (disabled ? ' disabled' : '') + (active ? ' active' : '');

            const a = document.createElement('a');
            a.className = 'page-link';
            a.href = '#';
            a.textContent = text;
            a.addEventListener('click', (e) => {
                e.preventDefault();
                if (disabled || active) return;
                this.goToPage(page);
            });

            li.appendChild(a);
            return li;
        };

        const makeEllipsis = () => {
            const li = document.createElement('li');
            li.className = 'page-item disabled';
            const span = document.createElement('span');
            span.className = 'page-link';
            span.textContent = '…';
            li.appendChild(span);
            return li;
        };

        // previous
        ul.appendChild(makeItem(this.currentPage - 1, '上一頁', { disabled: this.currentPage <= 1 }));

        // If totalPages is small, render all pages
        if (this.totalPages <= (this.windowSize * 2) + 5) {
            for (let i = 1; i <= this.totalPages; i++) {
                ul.appendChild(makeItem(i, String(i), { active: i === this.currentPage }));
            }
        } else {
            // always show first page
            ul.appendChild(makeItem(1, '1', { active: this.currentPage === 1 }));

            const left = Math.max(2, this.currentPage - this.windowSize);
            const right = Math.min(this.totalPages - 1, this.currentPage + this.windowSize);

            if (left > 2) {
                ul.appendChild(makeEllipsis());
            }

            for (let i = left; i <= right; i++) {
                ul.appendChild(makeItem(i, String(i), { active: i === this.currentPage }));
            }

            if (right < this.totalPages - 1) {
                ul.appendChild(makeEllipsis());
            }

            // always show last page
            ul.appendChild(makeItem(this.totalPages, String(this.totalPages), { active: this.currentPage === this.totalPages }));
        }

        // next
        ul.appendChild(makeItem(this.currentPage + 1, '下一頁', { disabled: this.currentPage >= this.totalPages }));

        // replace content
        el.innerHTML = '';
        el.appendChild(nav);

        this.container = el;
        this._updateDataset();
    }

    goToPage(page) {
        const p = Math.min(this.totalPages, Math.max(1, Number(page) || 1));
        if (p === this.currentPage) return;
        this.currentPage = p;
        // re-render to update UI quickly
        this.render(this.container);
        if (this.onPageChange) this.onPageChange(this.currentPage);
    }

    _updateDataset() {
        if (!this.container) return;
        this.container.dataset.totalPages = String(this.totalPages);
        this.container.dataset.currentPage = String(this.currentPage);
    }
}
