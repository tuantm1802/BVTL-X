document.addEventListener('alpine:init', function () {
    Alpine.data('alpineCity', function () {
        return {
            items: [],
            totalItems: 0,
            keyword: '',
            isLoading: false,
            currentPage: 1,
            pageSize: 20,
            sortColumn: 'KeyFirst',
            filterScope: 'all', // 'all' hoặc 'key'
            cityMode: 'NEW34',  // 'NEW34' (NQ 202/2025) hoặc 'OLD63' (Lịch sử)
            pageSizeOptions: [10, 20, 34, 50, 63],
            summary: {
                totalNew: 34,
                totalOld: 63,
                keyNew: 6,
                keyOld: 6
            },

            editModal: {
                isOpen: false,
                isSaving: false,
                code: '',
                name: '',
                isKey: false,
                codeMap: '',
                errorMessage: ''
            },

            init: function () {
                this.loadSummary();
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

            setCityMode: function (mode) {
                if (this.cityMode !== mode) {
                    this.cityMode = mode;
                    this.currentPage = 1;
                    this.filterScope = 'all';
                    this.loadData();
                }
            },

            setFilterScope: function (scope) {
                if (this.filterScope !== scope) {
                    this.filterScope = scope;
                    this.currentPage = 1;
                    this.loadData();
                }
            },

            loadSummary: function () {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/City/GetCitySummary',
                    success: function (res) {
                        if (res && !res.Error) {
                            self.summary.totalNew = res.TotalNew || 34;
                            self.summary.totalOld = res.TotalOld || 63;
                            self.summary.keyNew = res.KeyNew || 6;
                            self.summary.keyOld = res.KeyOld || 6;
                        }
                    }
                });
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
                        SortColumn: self.sortColumn,
                        CityCodes: self.filterScope === 'key' ? 'KEY_ONLY' : '',
                        CityMode: self.cityMode
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
                this.filterScope = 'all';
                this.currentPage = 1;
                this.loadData();
            },

            openEditModal: function (item) {
                this.editModal.code = item.Code;
                this.editModal.name = item.Name;
                this.editModal.isKey = !!(item.Code_Map && item.Code_Map.trim());
                this.editModal.codeMap = item.Code_Map || '';
                this.editModal.errorMessage = '';
                this.editModal.isSaving = false;
                this.editModal.isOpen = true;
            },

            closeEditModal: function () {
                this.editModal.isOpen = false;
                this.editModal.errorMessage = '';
            },

            saveKeyProvince: function () {
                var self = this;
                self.editModal.errorMessage = '';

                if (self.editModal.isKey) {
                    var map = (self.editModal.codeMap || '').trim().toUpperCase();
                    if (!map) {
                        self.editModal.errorMessage = 'Vui lòng nhập Mã viết tắt (2 ký tự) khi thiết lập làm tỉnh trọng điểm.';
                        return;
                    }
                    if (map.length > 5) {
                        self.editModal.errorMessage = 'Mã viết tắt không nên vượt quá 5 ký tự (chuẩn 2 chữ cái in hoa, ví dụ: NT, CT, BD).';
                        return;
                    }
                    self.editModal.codeMap = map;
                }

                self.editModal.isSaving = true;

                $.ajax({
                    type: 'POST',
                    url: '/City/UpdateKeyProvince',
                    data: {
                        code: self.editModal.code,
                        codeMap: self.editModal.codeMap,
                        isKey: self.editModal.isKey
                    },
                    success: function (res) {
                        self.editModal.isSaving = false;
                        if (res && !res.Error) {
                            if (window.toastr) {
                                toastr.success(res.Title || 'Cập nhật thành công!');
                            }
                            self.closeEditModal();
                            self.loadSummary();
                            self.loadData();
                        } else {
                            self.editModal.errorMessage = (res && res.Title) ? res.Title : 'Lỗi khi cập nhật tỉnh trọng điểm.';
                            if (window.toastr) {
                                toastr.error(self.editModal.errorMessage);
                            }
                        }
                    },
                    error: function () {
                        self.editModal.isSaving = false;
                        self.editModal.errorMessage = 'Không thể kết nối đến máy chủ.';
                        if (window.toastr) {
                            toastr.error(self.editModal.errorMessage);
                        }
                    }
                });
            }
        };
    });
});
