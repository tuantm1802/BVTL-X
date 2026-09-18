document.addEventListener('alpine:init', function () {
    Alpine.data('alpineCity', function () {
        return {
            items: [],
            totalItems: 0,
            keyword: '',
            isLoading: false,
            currentPage: 1,
            pageSize: 20,
            sortColumn: 'Code',
            pageSizeOptions: [10, 20, 30, 50, 63],

            init: function () {
                this.loadData();
            },

            get totalPages() {
                if (!this.totalItems || this.totalItems <= 0) return 1;
                return Math.max(1, Math.ceil(this.totalItems / this.pageSize));
            },

            get fromRecord() {
                if (!this.totalItems || this.totalItems <= 0) return 0;
                return (this.currentPage - 1) * this.pageSize + 1;
            },

            get toRecord() {
                if (!this.totalItems || this.totalItems <= 0) return 0;
                return Math.min(this.totalItems, this.currentPage * this.pageSize);
            },

            get visiblePages() {
                var pages = [];
                var total = this.totalPages;
                var current = this.currentPage;
                var start = Math.max(1, current - 2);
                var end = Math.min(total, current + 2);

                for (var i = start; i <= end; i++) {
                    pages.push(i);
                }
                return pages;
            },

            loadData: function () {
                var self = this;
                self.isLoading = true;

                $.ajax({
                    type: 'POST',
                    url: '/City/GetAll',
                    data: {
                        KeyWord: self.keyword || '',
                        currentPage: self.currentPage,
                        pageSize: self.pageSize,
                        SortColumn: self.sortColumn
                    },
                    success: function (response) {
                        if (response && !response.Error) {
                            self.items = response.data || [];
                            self.totalItems = response.totalItems || self.items.length;
                        } else {
                            self.items = [];
                            self.totalItems = 0;
                        }
                    },
                    error: function () {
                        self.items = [];
                        self.totalItems = 0;
                    },
                    complete: function () {
                        self.isLoading = false;
                    }
                });
            },

            changePage: function (page) {
                if (page < 1 || page > this.totalPages || page === this.currentPage) {
                    return;
                }
                this.currentPage = page;
                this.loadData();
            },

            changePageSize: function (size) {
                this.pageSize = parseInt(size, 10);
                this.currentPage = 1;
                this.loadData();
            },

            search: function () {
                this.currentPage = 1;
                this.loadData();
            },

            resetSearch: function () {
                this.keyword = '';
                this.currentPage = 1;
                this.loadData();
            }
        };
    });
});
