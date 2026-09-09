(function () {
    function registerController() {
        if (typeof Alpine === 'undefined') return;

        Alpine.data('alpineKhachHangCD45', function () {
            return {
                items: [],
                totalRecords: 0,
                isLoading: false,
                isDetailLoading: false,
                listCities: [],
                listNhoms: [],
                filteredNhoms: [],
                selectedCustomer: null,

                filter: {
                    Keyword: '',
                    CityCode: '',
                    MaNhom: '',
                    DoiTuong: '',
                    CoBHYT: '',
                    CoCCCD: '',
                    FromDate: '',
                    ToDate: '',
                    PageIndex: 1,
                    PageSize: 25
                },

                init: function () {
                    var self = this;
                    self.loadDanhMuc();
                },

                get totalPages() {
                    var self = this;
                    if (!self.totalRecords || self.totalRecords <= 0) return 1;
                    return Math.ceil(self.totalRecords / self.filter.PageSize);
                },

                get visiblePages() {
                    var self = this;
                    var total = self.totalPages;
                    var current = self.filter.PageIndex;
                    var pages = [];
                    var start = Math.max(1, current - 2);
                    var end = Math.min(total, current + 2);
                    for (var i = start; i <= end; i++) {
                        pages.push(i);
                    }
                    return pages;
                },

                loadDanhMuc: function () {
                    var self = this;
                    $.ajax({
                        type: 'POST',
                        url: '/KhachHangCD45/GetFilterData',
                        success: function (res) {
                            if (res.Success) {
                                self.listCities = res.Cities || [];
                                self.listNhoms = res.Nhoms || [];
                                self.filteredNhoms = self.listNhoms;
                            }
                            self.searchCustomers(1);
                        },
                        error: function () {
                            self.searchCustomers(1);
                        }
                    });
                },

                onCityChange: function () {
                    var self = this;
                    if (!self.filter.CityCode) {
                        self.filteredNhoms = self.listNhoms;
                    } else {
                        self.filteredNhoms = self.listNhoms.filter(function (x) {
                            return x.CityCode === self.filter.CityCode;
                        });
                    }
                    self.filter.MaNhom = '';
                    self.searchCustomers(1);
                },

                resetFilters: function () {
                    var self = this;
                    self.filter.Keyword = '';
                    self.filter.CityCode = '';
                    self.filter.MaNhom = '';
                    self.filter.DoiTuong = '';
                    self.filter.CoBHYT = '';
                    self.filter.CoCCCD = '';
                    self.filter.FromDate = '';
                    self.filter.ToDate = '';
                    self.filter.PageIndex = 1;
                    self.filteredNhoms = self.listNhoms;
                    self.searchCustomers(1);
                },

                searchCustomers: function (pageIndex) {
                    var self = this;
                    if (pageIndex) self.filter.PageIndex = pageIndex;
                    self.isLoading = true;

                    var reqData = {
                        Keyword: self.filter.Keyword || '',
                        CityCode: self.filter.CityCode || '',
                        MaNhom: self.filter.MaNhom || '',
                        DoiTuong: self.filter.DoiTuong ? parseInt(self.filter.DoiTuong) : null,
                        CoBHYT: self.filter.CoBHYT === '' ? null : (self.filter.CoBHYT === 'true'),
                        CoCCCD: self.filter.CoCCCD === '' ? null : (self.filter.CoCCCD === 'true'),
                        FromDate: self.filter.FromDate || '',
                        ToDate: self.filter.ToDate || '',
                        PageIndex: self.filter.PageIndex,
                        PageSize: parseInt(self.filter.PageSize) || 25
                    };

                    $.ajax({
                        type: 'POST',
                        url: '/KhachHangCD45/SearchCustomers',
                        data: reqData,
                        success: function (res) {
                            self.isLoading = false;
                            if (res.Success) {
                                self.items = res.Data || [];
                                self.totalRecords = res.Total || 0;
                            } else {
                                if (window.toastr) toastr.error(res.Message);
                            }
                        },
                        error: function (xhr, status, error) {
                            self.isLoading = false;
                            console.error('Loi khi tim kiem khach hang:', error);
                            if (window.toastr) toastr.error('Có lỗi xảy ra khi tải danh sách khách hàng!');
                        }
                    });
                },

                activeDetailTab: 'tab-thongtin',

                switchDetailTab: function (tabId) {
                    this.activeDetailTab = tabId;
                },

                copyRecordId: function (recordId) {
                    if (!recordId) return;
                    if (navigator.clipboard && window.isSecureContext) {
                        navigator.clipboard.writeText(recordId).then(function () {
                            if (window.toastr) toastr.success('Đã sao chép mã hồ sơ: ' + recordId);
                        });
                    } else {
                        var textArea = document.createElement("textarea");
                        textArea.value = recordId;
                        document.body.appendChild(textArea);
                        textArea.select();
                        try {
                            document.execCommand('copy');
                            if (window.toastr) toastr.success('Đã sao chép mã hồ sơ: ' + recordId);
                        } catch (err) {
                            if (window.toastr) toastr.info('Mã hồ sơ: ' + recordId);
                        }
                        document.body.removeChild(textArea);
                    }
                },

                openDetailModal: function (recordId) {
                    var self = this;
                    if (!recordId) return;

                    self.selectedCustomer = null;
                    self.isDetailLoading = true;
                    self.activeDetailTab = 'tab-thongtin';

                    var modalEl = document.getElementById('modalCustomerDetail');
                    if (window.bootstrap && bootstrap.Modal) {
                        var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                        modal.show();
                    } else if (window.jQuery) {
                        window.jQuery(modalEl).modal('show');
                    }

                    // Kích hoạt lại tab đầu tiên nếu có
                    var firstTabBtn = document.querySelector('#modalCustomerDetail button[data-bs-target="#tab-thongtin"]');
                    if (firstTabBtn && window.bootstrap && bootstrap.Tab) {
                        var tabInstance = bootstrap.Tab.getInstance(firstTabBtn) || new bootstrap.Tab(firstTabBtn);
                        tabInstance.show();
                    }

                    $.ajax({
                        type: 'GET',
                        url: '/KhachHangCD45/GetCustomerDetail?recordId=' + encodeURIComponent(recordId),
                        success: function (res) {
                            self.isDetailLoading = false;
                            if (res.Success) {
                                self.selectedCustomer = res.Data;
                            } else {
                                if (window.toastr) toastr.error(res.Message);
                            }
                        },
                        error: function (xhr, status, error) {
                            self.isDetailLoading = false;
                            console.error('Loi khi lay chi tiet:', error);
                            if (window.toastr) toastr.error('Có lỗi khi tải thông tin hồ sơ!');
                        }
                    });
                },

                exportExcel: function () {
                    var self = this;
                    var p = self.filter;
                    var url = '/KhachHangCD45/ExportExcel?keyword=' + encodeURIComponent(p.Keyword || '') +
                        '&cityCode=' + encodeURIComponent(p.CityCode || '') +
                        '&maNhom=' + encodeURIComponent(p.MaNhom || '') +
                        '&doiTuong=' + encodeURIComponent(p.DoiTuong || '') +
                        '&coBHYT=' + encodeURIComponent(p.CoBHYT || '') +
                        '&coCCCD=' + encodeURIComponent(p.CoCCCD || '') +
                        '&fromDate=' + encodeURIComponent(p.FromDate || '') +
                        '&toDate=' + encodeURIComponent(p.ToDate || '');
                    window.location.href = url;
                },

                parseDate: function (val) {
                    if (!val) return null;
                    if (typeof val === 'number') return new Date(val);
                    if (typeof val === 'string') {
                        var matches = val.match(/\d+/);
                        if (matches && val.indexOf('/Date(') !== -1) {
                            return new Date(parseInt(matches[0], 10));
                        }
                        var d = new Date(val);
                        if (!isNaN(d.getTime())) return d;
                    }
                    return null;
                },

                formatDate: function (val) {
                    if (!val) return '-';
                    var d = this.parseDate(val);
                    if (!d || isNaN(d.getTime())) return '-';
                    var day = ('0' + d.getDate()).slice(-2);
                    var month = ('0' + (d.getMonth() + 1)).slice(-2);
                    return day + '/' + month + '/' + d.getFullYear();
                },

                formatDateTime: function (val) {
                    if (!val) return '-';
                    var d = this.parseDate(val);
                    if (!d || isNaN(d.getTime())) return '-';
                    var day = ('0' + d.getDate()).slice(-2);
                    var month = ('0' + (d.getMonth() + 1)).slice(-2);
                    var hour = ('0' + d.getHours()).slice(-2);
                    var min = ('0' + d.getMinutes()).slice(-2);
                    return day + '/' + month + '/' + d.getFullYear() + ' ' + hour + ':' + min;
                },

                getBadgeClassDoiTuong: function (doiTuong) {
                    switch (parseInt(doiTuong)) {
                        case 1: return 'badge bg-success-subtle text-success border border-success'; // PUD
                        case 2: return 'badge bg-danger-subtle text-danger border border-danger';   // PLHIV
                        case 3: return 'badge bg-info-subtle text-info border border-info';          // TG
                        case 4: return 'badge bg-primary-subtle text-primary border border-primary'; // MSM
                        case 5: return 'badge bg-warning-subtle text-warning-emphasis border border-warning'; // SW
                        default: return 'badge bg-secondary-subtle text-secondary border';
                    }
                },

                getBadgeClassQst: function (muc) {
                    switch (parseInt(muc)) {
                        case 1: return 'badge bg-danger-subtle text-danger border border-danger'; // Nguy cơ rất cao
                        case 2: return 'badge bg-warning-subtle text-warning-emphasis border border-warning'; // Nguy cơ cao
                        case 3: return 'badge bg-info-subtle text-info border border-info'; // Nguy cơ trung bình
                        case 4: return 'badge bg-success-subtle text-success border border-success'; // Nguy cơ thấp
                        default: return 'badge bg-secondary-subtle text-secondary border';
                    }
                },

                formatKetQuaHivText: function (val) {
                    if (val === 1 || val === '1') return 'Âm tính';
                    if (val === 2 || val === '2') return 'Dương tính';
                    if (val === 3 || val === '3') return 'Không rõ / Chờ KQ';
                    return '-';
                },

                getBadgeClassHiv: function (val) {
                    if (val === 1 || val === '1') return 'badge bg-success-subtle text-success border border-success';
                    if (val === 2 || val === '2') return 'badge bg-danger-subtle text-danger border border-danger fw-bold';
                    if (val === 3 || val === '3') return 'badge bg-warning-subtle text-warning-emphasis border border-warning';
                    return 'badge bg-secondary-subtle text-secondary border';
                }
            };
        });
    }

    if (window.Alpine) {
        registerController();
    } else {
        document.addEventListener('alpine:init', registerController);
    }
})();