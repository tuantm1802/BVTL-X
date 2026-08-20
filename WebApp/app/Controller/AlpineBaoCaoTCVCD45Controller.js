document.addEventListener('alpine:init', function () {
    Alpine.data('alpineBaoCaoTCVCD45', function () {
        return {
            items: [],
            isLoading: false,
            thang: new Date().getMonth() + 1,
            nam: new Date().getFullYear(),
            fromDate: '',
            toDate: '',
            selectedCity: '',
            selectedNhom: '',
            selectedTCV: '',
            selectedTCVObj: null,
            listCities: [],
            listNhoms: [],
            filteredNhoms: [],
            listTCVs: [],
            filteredTCVs: [],
            checkedTCVs: [], // For batch export
            isSelectAll: false,

            init: function () {
                var self = this;
                self.updateDateRange();
                self.loadDanhMuc();
            },

            formatNumber: function (val) {
                if (val === null || val === undefined || val === 0 || val === "0") return '-';
                return Number(val).toLocaleString('vi-VN');
            },

            updateDateRange: function () {
                var self = this;
                var y = parseInt(self.nam) || new Date().getFullYear();
                var m = parseInt(self.thang) || (new Date().getMonth() + 1);

                // Chu kỳ: 26 tháng trước đến 25 tháng này
                var prevMonth = m === 1 ? 12 : m - 1;
                var prevYear = m === 1 ? y - 1 : y;
                var strPrevMonth = prevMonth < 10 ? '0' + prevMonth : prevMonth;
                var strM = m < 10 ? '0' + m : m;

                self.fromDate = '26/' + strPrevMonth + '/' + prevYear;
                self.toDate = '25/' + strM + '/' + y;
            },

            loadDanhMuc: function () {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoTCVCD45/GetFilterData',
                    success: function (res) {
                        if (res.Success) {
                            self.listCities = res.Cities || [];
                            self.listNhoms = res.Nhoms || [];
                            self.filteredNhoms = self.listNhoms;
                            self.listTCVs = res.TCVs || [];
                            self.filteredTCVs = self.listTCVs;

                            if (self.filteredTCVs.length > 0) {
                                self.selectedTCV = self.filteredTCVs[0].MA_TCV;
                                self.selectedTCVObj = self.filteredTCVs[0];
                                self.checkedTCVs = self.filteredTCVs.map(function (x) { return x.ID; });
                                self.isSelectAll = true;
                                self.previewData();
                            }
                        }
                    }
                });
            },

            onCityChange: function () {
                var self = this;
                if (!self.selectedCity) {
                    self.filteredNhoms = self.listNhoms;
                } else {
                    self.filteredNhoms = self.listNhoms.filter(function (x) {
                        return x.city_code === self.selectedCity;
                    });
                }
                self.selectedNhom = '';
                self.filterTCVs();
            },

            onNhomChange: function () {
                this.filterTCVs();
            },

            filterTCVs: function () {
                var self = this;
                self.filteredTCVs = self.listTCVs.filter(function (x) {
                    var matchCity = !self.selectedCity || x.CITY_CODE === self.selectedCity;
                    var matchNhom = !self.selectedNhom || x.MA_NHOM === self.selectedNhom;
                    return matchCity && matchNhom;
                });

                if (self.filteredTCVs.length > 0) {
                    self.selectedTCV = self.filteredTCVs[0].MA_TCV;
                    self.selectedTCVObj = self.filteredTCVs[0];
                } else {
                    self.selectedTCV = '';
                    self.selectedTCVObj = null;
                }
                self.checkedTCVs = self.filteredTCVs.map(function (x) { return x.ID; });
                self.isSelectAll = true;
            },

            toggleSelectAll: function () {
                var self = this;
                if (self.isSelectAll) {
                    self.checkedTCVs = self.filteredTCVs.map(function (x) { return x.ID; });
                } else {
                    self.checkedTCVs = [];
                }
            },

            onTCVSelectChange: function () {
                var self = this;
                var found = self.filteredTCVs.find(function (x) { return x.MA_TCV === self.selectedTCV; });
                self.selectedTCVObj = found || null;
                self.previewData();
            },

            previewData: function () {
                var self = this;
                if (!self.selectedTCVObj) {
                    if (window.toastr) toastr.warning("Vui lòng chọn 1 Tiếp cận viên để xem trước.");
                    return;
                }
                self.isLoading = true;

                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoTCVCD45/SearchBaoCao',
                    data: {
                        FromDate: self.fromDate,
                        ToDate: self.toDate,
                        MaNhom: self.selectedTCVObj.MA_NHOM,
                        MaTCV: self.selectedTCVObj.MA_TCV
                    },
                    success: function (res) {
                        self.isLoading = false;
                        if (res.Success) {
                            self.items = res.Data || [];
                        } else {
                            if (window.toastr) toastr.error(res.Message);
                        }
                    },
                    error: function () {
                        self.isLoading = false;
                        if (window.toastr) toastr.error('Có lỗi xảy ra khi tải dữ liệu xem trước.');
                    }
                });
            },

            exportSingle: function () {
                var self = this;
                if (!self.selectedTCVObj) {
                    if (window.toastr) toastr.warning("Vui lòng chọn 1 Tiếp cận viên.");
                    return;
                }
                var url = '/BaoCaoTCVCD45/ExportSingleExcel?FromDate=' + encodeURIComponent(self.fromDate) +
                    '&ToDate=' + encodeURIComponent(self.toDate) +
                    '&MaNhom=' + encodeURIComponent(self.selectedTCVObj.MA_NHOM) +
                    '&MaTCV=' + encodeURIComponent(self.selectedTCVObj.MA_TCV) +
                    '&TenTCV=' + encodeURIComponent(self.selectedTCVObj.TEN_TCV) +
                    '&TenNhom=' + encodeURIComponent(self.selectedTCVObj.TEN_NHOM);
                window.location.href = url;
            },

            exportBatchZip: function () {
                var self = this;
                if (!self.checkedTCVs || self.checkedTCVs.length === 0) {
                    if (window.toastr) toastr.warning("Vui lòng tích chọn ít nhất 1 TCV để xuất file ZIP hàng loạt.");
                    return;
                }

                var selectedObjs = self.listTCVs.filter(function (x) {
                    return self.checkedTCVs.includes(x.ID);
                });

                var form = document.createElement("form");
                form.method = "POST";
                form.action = "/BaoCaoTCVCD45/ExportExcelZip";

                var inputFrom = document.createElement("input");
                inputFrom.type = "hidden";
                inputFrom.name = "FromDate";
                inputFrom.value = self.fromDate;
                form.appendChild(inputFrom);

                var inputTo = document.createElement("input");
                inputTo.type = "hidden";
                inputTo.name = "ToDate";
                inputTo.value = self.toDate;
                form.appendChild(inputTo);

                var inputJson = document.createElement("input");
                inputJson.type = "hidden";
                inputJson.name = "DanhSachTCVJson";
                inputJson.value = JSON.stringify(selectedObjs);
                form.appendChild(inputJson);

                document.body.appendChild(form);
                form.submit();
                document.body.removeChild(form);
            }
        };
    });
});
