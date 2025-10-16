import { Pagination } from '/js/components/pagination.js';

function submitForm(pageNumber) {
    const pageInput = document.getElementById('pageNumberInput');
    if (pageInput) pageInput.value = pageNumber;
    const form = document.getElementById('paginationForm');
    if (form) form.submit();
}

document.querySelectorAll('.btn-delete').forEach(btn => {
    btn.addEventListener('click', async (e) => {
        e.preventDefault();

        const id = btn.getAttribute('data-id');
        if (!confirm(`確定要刪除 ID=${id} 的員工嗎？`)) return;

        const response = await fetch(`/Employee/Delete/${id}`, {
            method: 'DELETE'
        });

        console.log('response', response)

        if (response.ok) {
            alert('刪除成功');
            // 重新載入頁面以更新列表，因為刪除可能會影響分頁
            // 取得當前頁數，若有 #paginationContainer 則以 dataset 為主，否則 fallback 到隱藏 input
            const container = document.getElementById('paginationContainer');
            let currentPage = null;
            if (container && container.dataset && container.dataset.currentPage) {
                currentPage = parseInt(container.dataset.currentPage, 10);
            }
            if (!currentPage) {
                const pageInput = document.getElementById('pageNumberInput');
                if (pageInput) currentPage = parseInt(pageInput.value, 10) || 1;
            }
            submitForm(currentPage || 1);
        } else {
            alert('刪除失敗');
        }
    });
});

//畫面初始化行為: 進階搜尋、pagination 渲染
document.addEventListener('DOMContentLoaded', function () {
    //渲染進階搜尋
    const form = document.getElementById('advancedForm');
    if (form) new AdvancedSearchManager(form).init();

    //渲染pagination
    const container = document.getElementById('paginationContainer');
    if (container) {
        const totalPages = parseInt(container.dataset.totalPages, 10) || 1;
        const currentPage = parseInt(container.dataset.currentPage, 10) || 1;
        // Optional: allow view to configure windowSize via data-window-size
        // Example usage (also shown in module comment):
        // import { Pagination } from '/js/components/pagination.js';
        // const p = new Pagination({ totalPages: 10, currentPage: 1, onPageChange: (n) => {} });
        // p.render(document.getElementById('paginationContainer'))
        const windowSize = parseInt(container.dataset.windowSize, 10) || 2;

        const pagination = new Pagination({
            totalPages: totalPages,
            currentPage: currentPage,
            windowSize: windowSize,
            onPageChange: (pageNumber) => {
                alert(`要載入第 ${pageNumber} 頁資料`)
                submitForm(pageNumber);
            }
        });

        pagination.render(container);
    }
});