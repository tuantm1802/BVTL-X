(function () {
    function registerController() {
        if (typeof Alpine === 'undefined') return;

        Alpine.data('alpineNhomTCVCD45', function () {
            return {
                items: [],
                kpi: {
                    TongTCV: 0,
                    TongNhom: 0,
                    TongTinh: 0,
                    TcvActive: 0
                },
                isLoading: false,
                keyword: '',
                selectedCity: '',
                selectedNhom: '',
                listCities: [],
                listNhoms: [],
                filteredNhoms: [],

                editModal: {
                    show: false,
                    maNhom: '',
                    tenNhom: '',
                    cityName: '',
                    prefix: 'Nhóm',
                    shortPrefix: 'Nhóm',
                    chucDanh: 'Trưởng nhóm',
                    isSaving: false,
                    errorMsg: ''
                },

                pageIndex: 1,
                pageSize: 15,

                init: function () {
                    var self = this;
                    self.loadDanhMuc();
                },

                get totalRecords() {
                    return this.items ? this.items.length : 0;
                },

                get totalPages() {
                    if (!this.items || this.items.length === 0) return 1;
                    return Math.ceil(this.items.length / parseInt(this.pageSize));
                },

                get pagedItems() {
                    if (!this.items) return [];
                    var size = parseInt(this.pageSize) || 15;
                    var start = (this.pageIndex - 1) * size;
                    return this.items.slice(start, start + size);
                },

                get fromRecord() {
                    if (!this.items || this.items.length === 0) return 0;
                    var size = parseInt(this.pageSize) || 15;
                    return (this.pageIndex - 1) * size + 1;
                },

                get toRecord() {
                    if (!this.items || this.items.length === 0) return 0;
                    var size = parseInt(this.pageSize) || 15;
                    return Math.min(this.pageIndex * size, this.items.length);
                },

                get visiblePages() {
                    var total = this.totalPages;
                    var current = this.pageIndex;
                    var pages = [];
                    var start = Math.max(1, current - 2);
                    var end = Math.min(total, current + 2);
                    for (var i = start; i <= end; i++) {
                        pages.push(i);
                    }
                    return pages;
                },

                changePage: function (page) {
                    var p = parseInt(page);
                    if (isNaN(p) || p < 1) p = 1;
                    if (p > this.totalPages) p = this.totalPages;
                    this.pageIndex = p;
                },

                loadDanhMuc: function () {
                    var self = this;
                    $.ajax({
                        type: 'POST',
                        url: '/NhomTCVCD45/GetFilterData',
                        success: function (res) {
                            if (res.Success) {
                                self.listCities = res.Cities || [];
                                self.listNhoms = res.Nhoms || [];
                                self.filteredNhoms = self.listNhoms;
                            }
                            self.loadData();
                        },
                        error: function () {
                            self.loadData();
                        }
                    });
                },

                onCityChange: function () {
                    var self = this;
                    self.pageIndex = 1;
                    if (!self.selectedCity) {
                        self.filteredNhoms = self.listNhoms;
                    } else {
                        self.filteredNhoms = self.listNhoms.filter(function (x) {
                            return x.CityCode === self.selectedCity;
                        });
                    }
                    self.selectedNhom = '';
                    self.loadData();
                },

                resetFilters: function () {
                    var self = this;
                    self.keyword = '';
                    self.selectedCity = '';
                    self.selectedNhom = '';
                    self.pageIndex = 1;
                    self.filteredNhoms = self.listNhoms;
                    self.loadData();
                },

                loadData: function () {
                    var self = this;
                    self.isLoading = true;
                    self.pageIndex = 1;

                    $.ajax({
                        type: 'POST',
                        url: '/NhomTCVCD45/GetList',
                        data: {
                            cityCode: self.selectedCity || '',
                            maNhom: self.selectedNhom || '',
                            keyword: self.keyword || ''
                        },
                        success: function (res) {
                            self.isLoading = false;
                            if (res.Success) {
                                self.items = res.Data || [];
                                if (res.Kpi) {
                                    self.kpi = res.Kpi;
                                }
                            } else {
                                if (window.toastr) toastr.error(res.Message);
                            }
                        },
                        error: function (xhr, status, error) {
                            self.isLoading = false;
                            console.error('Loi khi nap du lieu TCV:', error);
                            if (window.toastr) toastr.error('Có lỗi xảy ra khi tải danh sách tiếp cận viên!');
                        }
                    });
                },

                exportExcel: function () {
                    var self = this;
                    var url = '/NhomTCVCD45/ExportExcel?cityCode=' + encodeURIComponent(self.selectedCity || '') +
                        '&maNhom=' + encodeURIComponent(self.selectedNhom || '');
                    window.location.href = url;
                },

                openEditPrefixModal: function (item) {
                    var self = this;
                    if (!item) return;
                    self.editModal.show = true;
                    self.editModal.maNhom = item.MA_NHOM || item.MaNhom || '';
                    self.editModal.tenNhom = item.TEN_NHOM || item.TenNhom || '';
                    self.editModal.cityName = item.CityName || item.CityCode || '';
                    self.editModal.prefix = item.PREFIX || item.Prefix || 'Nhóm';
                    self.editModal.shortPrefix = item.SHORT_PREFIX || item.ShortPrefix || self.editModal.prefix || 'Nhóm';
                    self.editModal.chucDanh = item.CHUC_DANH || item.ChucDanh || (self.editModal.cityName === 'HCM' || self.editModal.maNhom === 'HN_TT' ? 'Giám đốc' : 'Trưởng nhóm');
                    self.editModal.errorMsg = '';
                    self.editModal.isSaving = false;
                },

                openEditPrefixModalByMaNhom: function (maNhom) {
                    var self = this;
                    if (!maNhom) return;
                    var nhom = self.listNhoms.find(function (x) { return x.MaNhom === maNhom; });
                    if (nhom) {
                        self.openEditPrefixModal({
                            MA_NHOM: nhom.MaNhom,
                            TEN_NHOM: nhom.TenNhom,
                            CityName: nhom.CityCode,
                            PREFIX: nhom.Prefix || 'Nhóm',
                            SHORT_PREFIX: nhom.ShortPrefix || 'Nhóm',
                            CHUC_DANH: nhom.ChucDanh || 'Trưởng nhóm'
                        });
                    }
                },

                closeEditPrefixModal: function () {
                    this.editModal.show = false;
                    this.editModal.errorMsg = '';
                    this.editModal.isSaving = false;
                },

                setPresetPrefix: function (prefix, shortPrefix, chucDanh) {
                    this.editModal.prefix = prefix;
                    this.editModal.shortPrefix = shortPrefix;
                    if (chucDanh) {
                        this.editModal.chucDanh = chucDanh;
                    }
                },

                saveNhomPrefix: function () {
                    var self = this;
                    if (!self.editModal.prefix || !self.editModal.prefix.trim()) {
                        self.editModal.errorMsg = 'Vui lòng nhập xưng danh / loại hình!';
                        return;
                    }
                    if (!self.editModal.shortPrefix || !self.editModal.shortPrefix.trim()) {
                        self.editModal.shortPrefix = self.editModal.prefix;
                    }

                    self.editModal.isSaving = true;
                    self.editModal.errorMsg = '';

                    $.ajax({
                        type: 'POST',
                        url: '/NhomTCVCD45/UpdateNhomPrefix',
                        data: {
                            maNhom: self.editModal.maNhom,
                            prefix: self.editModal.prefix.trim(),
                            shortPrefix: self.editModal.shortPrefix.trim(),
                            chucDanh: (self.editModal.chucDanh || '').trim() || 'Trưởng nhóm'
                        },
                        success: function (res) {
                            self.editModal.isSaving = false;
                            if (res.Success) {
                                if (window.toastr) {
                                    toastr.success(res.Message || 'Cập nhật xưng danh nhóm thành công!');
                                } else {
                                    alert(res.Message || 'Cập nhật xưng danh nhóm thành công!');
                                }
                                self.closeEditPrefixModal();
                                self.loadDanhMuc();
                            } else {
                                self.editModal.errorMsg = res.Message || 'Lỗi khi cập nhật!';
                                if (window.toastr) toastr.error(self.editModal.errorMsg);
                            }
                        },
                        error: function (xhr, status, error) {
                            self.editModal.isSaving = false;
                            self.editModal.errorMsg = 'Lỗi kết nối máy chủ: ' + error;
                            if (window.toastr) toastr.error(self.editModal.errorMsg);
                        }
                    });
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