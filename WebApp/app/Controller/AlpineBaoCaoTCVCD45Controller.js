function removeVietnameseTones(str) {
    if (!str) return '';
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, "");
    str = str.replace(/\u02C6|\u0306|\u031B/g, "");
    return str.trim();
}

document.addEventListener('alpine:init', function () {
    Alpine.data('alpineBaoCaoTCVCD45', function () {
        return {
            items: [],
            isLoading: false,
            loaiKy: 'Thang', // Thang, Quy, 6Thang, 12Thang, TuyChon
            thang: (new Date().getMonth() + 1).toString(),
            nam: new Date().getFullYear(),
            quy: 'I',
            ky6Thang: '1',
            fromDate: '',
            toDate: '',
            dateError: '',
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
                var now = new Date();
                var curMonth = now.getMonth() + 1;
                self.thang = curMonth.toString();
                self.nam = now.getFullYear();

                if (curMonth >= 1 && curMonth <= 3) self.quy = 'I';
                else if (curMonth >= 4 && curMonth <= 6) self.quy = 'II';
                else if (curMonth >= 7 && curMonth <= 9) self.quy = 'III';
                else if (curMonth >= 10 && curMonth <= 12) self.quy = 'IV';

                self.ky6Thang = curMonth <= 6 ? '1' : '2';

                self.updateDateRange();
                self.loadDanhMuc();
            },

            getLoaiBaoCao: function () {
                var loaiBaoCaoMap = { 'Thang': 'Thang', 'Quy': 'Quy', '6Thang': '6T', '12Thang': '12T', 'TuyChon': 'TuyChon' };
                return loaiBaoCaoMap[this.loaiKy] || 'TuyChon';
            },

            formatNumber: function (val) {
                if (val === null || val === undefined || val === 0 || val === "0") return '-';
                return Number(val).toLocaleString('vi-VN');
            },

            parseDateVN: function (dateStr) {
                if (!dateStr || typeof dateStr !== 'string') return null;
                var parts = dateStr.trim().split('/');
                if (parts.length === 3) {
                    var day = parseInt(parts[0], 10);
                    var month = parseInt(parts[1], 10) - 1;
                    var year = parseInt(parts[2], 10);
                    if (!isNaN(day) && !isNaN(month) && !isNaN(year)) {
                        var d = new Date(year, month, day);
                        if (d.getFullYear() === year && d.getMonth() === month && d.getDate() === day) {
                            return d;
                        }
                    }
                }
                var isoParts = dateStr.trim().split('-');
                if (isoParts.length === 3) {
                    var year = parseInt(isoParts[0], 10);
                    var month = parseInt(isoParts[1], 10) - 1;
                    var day = parseInt(isoParts[2], 10);
                    if (!isNaN(day) && !isNaN(month) && !isNaN(year)) {
                        var d = new Date(year, month, day);
                        if (d.getFullYear() === year && d.getMonth() === month && d.getDate() === day) {
                            return d;
                        }
                    }
                }
                return null;
            },

            validateDateRange: function (showToast) {
                var self = this;
                if (showToast === undefined) showToast = true;

                if (!self.fromDate || !self.fromDate.trim()) {
                    self.dateError = "Vui lòng nhập 'Từ ngày'!";
                    if (showToast && window.toastr) toastr.error(self.dateError);
                    return false;
                }
                if (!self.toDate || !self.toDate.trim()) {
                    self.dateError = "Vui lòng nhập 'Đến ngày'!";
                    if (showToast && window.toastr) toastr.error(self.dateError);
                    return false;
                }

                var dFrom = self.parseDateVN(self.fromDate);
                if (!dFrom) {
                    self.dateError = "'Từ ngày' không đúng định dạng dd/mm/yyyy!";
                    if (showToast && window.toastr) toastr.error(self.dateError);
                    return false;
                }

                var dTo = self.parseDateVN(self.toDate);
                if (!dTo) {
                    self.dateError = "'Đến ngày' không đúng định dạng dd/mm/yyyy!";
                    if (showToast && window.toastr) toastr.error(self.dateError);
                    return false;
                }

                if (dTo < dFrom) {
                    self.dateError = "Khoảng thời gian không hợp lệ: 'Đến ngày' không được nhỏ hơn 'Từ ngày' (Từ ngày phải nhỏ hơn hoặc bằng Đến ngày)!";
                    if (showToast && window.toastr) toastr.error(self.dateError);
                    return false;
                }

                self.dateError = '';
                return true;
            },

            updateDateRange: function () {
                var self = this;
                var y = parseInt(self.nam) || new Date().getFullYear();

                if (self.loaiKy === 'Thang') {
                    var m = parseInt(self.thang) || (new Date().getMonth() + 1);
                    // Chu kỳ REDCap: 26 tháng trước đến 25 tháng này
                    var prevMonth = m === 1 ? 12 : m - 1;
                    var prevYear = m === 1 ? y - 1 : y;
                    var strPrevMonth = prevMonth < 10 ? '0' + prevMonth : prevMonth;
                    var strM = m < 10 ? '0' + m : m;

                    self.fromDate = '26/' + strPrevMonth + '/' + prevYear;
                    self.toDate = '25/' + strM + '/' + y;
                } else if (self.loaiKy === 'Quy') {
                    if (self.quy === 'I') { self.fromDate = '26/12/' + (y - 1); self.toDate = '25/03/' + y; }
                    else if (self.quy === 'II') { self.fromDate = '26/03/' + y; self.toDate = '25/06/' + y; }
                    else if (self.quy === 'III') { self.fromDate = '26/06/' + y; self.toDate = '25/09/' + y; }
                    else if (self.quy === 'IV') { self.fromDate = '26/09/' + y; self.toDate = '25/12/' + y; }
                } else if (self.loaiKy === '6Thang') {
                    if (self.ky6Thang === '1') { self.fromDate = '26/12/' + (y - 1); self.toDate = '25/06/' + y; }
                    else { self.fromDate = '26/06/' + y; self.toDate = '25/12/' + y; }
                } else if (self.loaiKy === '12Thang') {
                    self.fromDate = '26/12/' + (y - 1);
                    self.toDate = '25/12/' + y;
                }
                self.dateError = '';

                if (self.selectedTCVObj) {
                    self.previewData();
                }
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
                                self.selectedTCV = self.filteredTCVs[0].ID;
                                self.selectedTCVObj = self.filteredTCVs[0];
                                self.checkedTCVs = self.filteredTCVs.map(function (x) { return x.ID; });
                                self.isSelectAll = true;
                                self.syncSelect2();
                                self.previewData();
                            } else {
                                self.syncSelect2();
                            }
                        }
                    }
                });
            },

            onCityChange: function (city) {
                var self = this;
                if (city !== undefined) self.selectedCity = city;
                var selCity = (self.selectedCity || '').trim().toUpperCase();
                if (!selCity) {
                    self.filteredNhoms = self.listNhoms;
                } else {
                    self.filteredNhoms = self.listNhoms.filter(function (x) {
                        return (x.city_code || '').trim().toUpperCase() === selCity;
                    });
                }
                self.selectedNhom = '';
                self.filterTCVs();
            },

            onNhomChange: function (nhom) {
                var self = this;
                if (nhom !== undefined) self.selectedNhom = nhom;
                self.filterTCVs();
            },

            filterTCVs: function () {
                var self = this;
                var selCity = (self.selectedCity || '').trim().toUpperCase();
                var selNhom = (self.selectedNhom || '').trim().toLowerCase();

                var selectedNhomObj = null;
                if (selNhom) {
                    selectedNhomObj = self.listNhoms.find(function (n) {
                        var std = (n.manhom_tbh || '').trim().toLowerCase();
                        var map = (n.manhom_tbh_map || '').trim().toLowerCase();
                        return std === selNhom || map === selNhom;
                    });
                }

                self.filteredTCVs = self.listTCVs.filter(function (x) {
                    var tcvCity = (x.CITY_CODE || '').trim().toUpperCase();
                    var matchCity = !selCity || tcvCity === selCity;

                    var matchNhom = true;
                    if (selNhom) {
                        var tcvNhom = (x.MA_NHOM || '').trim().toLowerCase();
                        if (selectedNhomObj) {
                            var codeStd = (selectedNhomObj.manhom_tbh || '').trim().toLowerCase();
                            var codeMap = (selectedNhomObj.manhom_tbh_map || '').trim().toLowerCase();
                            matchNhom = (tcvNhom === codeStd || tcvNhom === codeMap || tcvNhom === selNhom);
                        } else {
                            matchNhom = (tcvNhom === selNhom);
                        }
                    }
                    return matchCity && matchNhom;
                });

                var stillExists = self.selectedTCVObj && self.filteredTCVs.some(function (x) {
                    return String(x.ID) === String(self.selectedTCVObj.ID);
                });

                if (!stillExists) {
                    if (self.filteredTCVs.length > 0) {
                        self.selectedTCV = self.filteredTCVs[0].ID;
                        self.selectedTCVObj = self.filteredTCVs[0];
                        self.previewData();
                    } else {
                        self.selectedTCV = '';
                        self.selectedTCVObj = null;
                        self.items = [];
                    }
                } else if (self.selectedTCVObj) {
                    self.previewData();
                }

                self.checkedTCVs = self.filteredTCVs.map(function (x) { return x.ID; });
                self.isSelectAll = true;
                self.syncSelect2();
            },

            syncSelect2: function () {
                var self = this;
                setTimeout(function () {
                    var $select = $('#cboSelectedTCV');
                    if (!$select.length) return;

                    if ($select.hasClass('select2-hidden-accessible')) {
                        $select.select2('destroy');
                    }

                    $select.empty();
                    if (self.filteredTCVs.length === 0) {
                        $select.append(new Option('-- Không có TCV nào --', '', true, true));
                    } else {
                        self.filteredTCVs.forEach(function (t) {
                            var text = '[' + t.MA_TCV + '] ' + t.TEN_TCV + ' - ' + t.TEN_NHOM + ' (' + t.CITY_CODE + ')';
                            var isSelected = String(t.ID) === String(self.selectedTCV);
                            var opt = new Option(text, t.ID, isSelected, isSelected);
                            $select.append(opt);
                        });
                    }

                    $select.select2({
                        theme: 'bootstrap4',
                        width: '100%',
                        placeholder: '-- Gõ họ tên TCV để tìm kiếm --',
                        allowClear: false,
                        matcher: function (params, data) {
                            if ($.trim(params.term) === '') return data;
                            if (typeof data.text === 'undefined') return null;
                            var term = removeVietnameseTones(params.term);
                            var text = removeVietnameseTones(data.text);
                            if (text.indexOf(term) > -1) return data;
                            return null;
                        }
                    });

                    if (self.selectedTCV) {
                        $select.val(self.selectedTCV).trigger('change.select2');
                    }

                    // Dùng namespaced events để tránh hủy mất internal listeners của Select2
                    $select.off('.tcvSync');
                    $select.on('select2:select.tcvSync', function (e) {
                        var val = e.params && e.params.data ? e.params.data.id : $(this).val();
                        if (val && String(val) !== String(self.selectedTCV)) {
                            self.onTCVSelectChange(val);
                        }
                    });
                    $select.on('change.tcvSync', function () {
                        var val = $(this).val();
                        if (val && String(val) !== String(self.selectedTCV)) {
                            self.onTCVSelectChange(val);
                        }
                    });
                }, 20);
            },

            toggleSelectAll: function () {
                var self = this;
                if (self.isSelectAll) {
                    self.checkedTCVs = self.filteredTCVs.map(function (x) { return x.ID; });
                } else {
                    self.checkedTCVs = [];
                }
            },

            selectTCVForPreview: function (id) {
                var self = this;
                if (!id) return;
                self.selectedTCV = id;
                var found = self.filteredTCVs.find(function (x) {
                    return String(x.ID) === String(id);
                });
                self.selectedTCVObj = found || null;
                var $select = $('#cboSelectedTCV');
                if ($select.length) {
                    $select.val(id).trigger('change.select2');
                }
                if (self.selectedTCVObj) {
                    self.previewData();
                } else {
                    self.items = [];
                }
            },

            onTCVSelectChange: function (newVal) {
                var self = this;
                if (newVal !== undefined && newVal !== null && newVal !== '') {
                    self.selectedTCV = newVal;
                }
                var found = self.filteredTCVs.find(function (x) {
                    return String(x.ID) === String(self.selectedTCV);
                });
                self.selectedTCVObj = found || null;
                if (self.selectedTCVObj) {
                    self.previewData();
                } else {
                    self.items = [];
                }
            },

            previewData: function () {
                var self = this;
                if (!self.validateDateRange(true)) {
                    return;
                }
                if (!self.selectedTCVObj) {
                    if (window.toastr) toastr.warning("Vui lòng chọn 1 Tiếp cận viên để xem trước.");
                    return;
                }
                self.isLoading = true;

                var loaiBaoCao = self.getLoaiBaoCao();

                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoTCVCD45/SearchBaoCao',
                    data: {
                        FromDate: self.fromDate,
                        ToDate: self.toDate,
                        MaNhom: self.selectedTCVObj.MA_NHOM,
                        MaTCV: self.selectedTCVObj.MA_TCV,
                        LoaiBaoCao: loaiBaoCao
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
                if (!self.validateDateRange(true)) {
                    return;
                }
                if (!self.selectedTCVObj) {
                    if (window.toastr) toastr.warning("Vui lòng chọn 1 Tiếp cận viên.");
                    return;
                }

                var loaiBaoCao = self.getLoaiBaoCao();

                var form = document.createElement("form");
                form.method = "POST";
                form.action = "/BaoCaoTCVCD45/ExportSingleExcel";

                var fields = {
                    FromDate: self.fromDate,
                    ToDate: self.toDate,
                    MaNhom: self.selectedTCVObj.MA_NHOM,
                    MaTCV: self.selectedTCVObj.MA_TCV,
                    TenTCV: self.selectedTCVObj.TEN_TCV,
                    TenNhom: self.selectedTCVObj.TEN_NHOM,
                    LoaiBaoCao: loaiBaoCao
                };

                for (var key in fields) {
                    var input = document.createElement("input");
                    input.type = "hidden";
                    input.name = key;
                    input.value = fields[key] || "";
                    form.appendChild(input);
                }

                document.body.appendChild(form);
                form.submit();
                document.body.removeChild(form);
            },

            exportExcel: function () {
                this.exportSingle();
            },

            exportBatchZip: function () {
                var self = this;
                if (!self.validateDateRange(true)) {
                    return;
                }
                if (!self.checkedTCVs || self.checkedTCVs.length === 0) {
                    if (window.toastr) toastr.warning("Vui lòng tích chọn ít nhất 1 TCV để xuất file ZIP hàng loạt.");
                    return;
                }

                var loaiBaoCao = self.getLoaiBaoCao();

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

                var inputLoai = document.createElement("input");
                inputLoai.type = "hidden";
                inputLoai.name = "LoaiBaoCao";
                inputLoai.value = loaiBaoCao;
                form.appendChild(inputLoai);

                document.body.appendChild(form);
                form.submit();
                document.body.removeChild(form);
            }
        };
    });
});
