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
    Alpine.data('alpineBaoCaoTuVanCD45', function () {
        return {
            listL1: [],
            listL2: [],
            filteredL1: [],
            filteredL2: [],
            searchL1Keyword: '',
            searchL2Keyword: '',
            activeTab: 'l1', // 'l1' | 'l2'
            isLoading: false,

            loaiKy: 'Thang', // Thang, Quy, 6Thang, 12Thang, TuyChon
            thang: (new Date().getMonth() + 1).toString(),
            nam: new Date().getFullYear(),
            quy: 'I',
            ky6Thang: '1',
            fromDate: '',
            toDate: '',
            dateError: '',

            cityMode: 'NEW34',
            selectedCity: '',
            selectedNhom: '',
            selectedTCV: '',
            selectedTCVObj: null,
            loaiTuVan: 'ALL', // 'ALL' | 'L1' | 'L2'

            listCities: [],
            listNhoms: [],
            filteredNhoms: [],
            listTCVs: [],
            filteredTCVs: [],
            isAdmin: true,

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

            formatNumber: function (val) {
                if (val === null || val === undefined || val === 0 || val === '0') return '-';
                return Number(val).toLocaleString('vi-VN');
            },

            updateDateRange: function () {
                var self = this;
                var y = parseInt(self.nam) || new Date().getFullYear();

                if (self.loaiKy === 'Thang') {
                    var m = parseInt(self.thang) || (new Date().getMonth() + 1);
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
            },

            validateDateRange: function (showAlert) {
                var self = this;
                if (!self.fromDate || !self.toDate) {
                    self.dateError = 'Vui lòng chọn đầy đủ Từ ngày và Đến ngày!';
                    if (showAlert && window.toastr) toastr.warning(self.dateError);
                    return false;
                }
                var d1 = self.parseDateVN(self.fromDate);
                var d2 = self.parseDateVN(self.toDate);
                if (!d1 || !d2) {
                    self.dateError = 'Định dạng ngày không hợp lệ (dd/MM/yyyy)!';
                    if (showAlert && window.toastr) toastr.warning(self.dateError);
                    return false;
                }
                if (d1 > d2) {
                    self.dateError = 'Từ ngày không được lớn hơn Đến ngày!';
                    if (showAlert && window.toastr) toastr.warning(self.dateError);
                    return false;
                }
                self.dateError = '';
                return true;
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

            setCityMode: function (mode) {
                if (this.cityMode !== mode) {
                    this.cityMode = mode;
                    this.selectedCity = '';
                    this.selectedNhom = '';
                    this.selectedTCV = '';
                    this.selectedTCVObj = null;
                    this.loadDanhMuc();
                }
            },

            findCityCodeByRawCode: function (rawCode) {
                if (!rawCode) return '';
                var code = String(rawCode).trim().toUpperCase();
                var self = this;

                // 1. Tìm trực tiếp theo CityCode
                var direct = self.listCities.find(function (c) {
                    return (c.CityCode || '').trim().toUpperCase() === code;
                });
                if (direct) return direct.CityCode;

                // 2. Tìm trong danh sách OldCodes (nếu đang ở chế độ tỉnh gộp NEW34)
                var oldMatch = self.listCities.find(function (c) {
                    if (!c.OldCodes || !c.OldCodes.length) return false;
                    return c.OldCodes.some(function (oc) {
                        return (oc || '').trim().toUpperCase() === code;
                    });
                });
                if (oldMatch) return oldMatch.CityCode;

                return '';
            },

            getMappedCodesForCity: function (cityCode) {
                if (!cityCode) return [];
                var self = this;
                var foundCity = self.listCities.find(function (c) {
                    return (c.CityCode || '').trim().toUpperCase() === String(cityCode).trim().toUpperCase();
                });
                if (foundCity && foundCity.OldCodes && foundCity.OldCodes.length) {
                    return foundCity.OldCodes.map(function (c) {
                        return (c || '').trim().toUpperCase();
                    });
                }
                return [String(cityCode).trim().toUpperCase()];
            },

            loadDanhMuc: function () {
                var self = this;
                $.ajax({
                    url: '/BaoCaoTuVanCD45/GetFilterData',
                    type: 'POST',
                    data: { cityMode: self.cityMode },
                    success: function (res) {
                        if (res.Success) {
                            self.listCities = res.Cities || [];
                            self.listNhoms = res.Nhoms || [];
                            self.listTCVs = res.TCVs || [];
                            self.isAdmin = res.IsAdmin !== false;
                            self.applyFilterCascades();
                            self.loadData();
                        } else {
                            if (window.toastr) toastr.error(res.Message || 'Lỗi tải danh mục');
                        }
                    },
                    error: function (xhr) {
                        if (window.toastr) toastr.error('Lỗi kết nối máy chủ');
                    }
                });
            },

            onCityChange: function (newCity) {
                if (newCity !== undefined) this.selectedCity = newCity;
                this.selectedNhom = '';
                this.selectedTCV = '';
                this.selectedTCVObj = null;
                this.applyFilterCascades();
            },

            onNhomChange: function (newNhom) {
                var self = this;
                if (newNhom !== undefined) self.selectedNhom = newNhom;
                if (self.selectedNhom) {
                    var nhomObj = self.listNhoms.find(function (n) {
                        var std = (n.manhom_tbh || '').trim().toLowerCase();
                        var map = (n.manhom_tbh_map || '').trim().toLowerCase();
                        var cur = self.selectedNhom.trim().toLowerCase();
                        return std === cur || map === cur;
                    });
                    if (nhomObj && nhomObj.city_code) {
                        var matchedCity = self.findCityCodeByRawCode(nhomObj.city_code);
                        if (matchedCity) {
                            self.selectedCity = matchedCity;
                        }
                    }
                }
                self.selectedTCV = '';
                self.selectedTCVObj = null;
                self.applyFilterCascades();
            },

            onTCVChange: function (newTCV) {
                var self = this;
                if (newTCV !== undefined) self.selectedTCV = newTCV;
                var found = self.filteredTCVs.find(function (t) {
                    return String(t.ID) === String(self.selectedTCV);
                });
                self.selectedTCVObj = found || null;

                if (found) {
                    var needCascade = false;
                    if (!self.selectedCity && found.CITY_CODE) {
                        self.selectedCity = found.CITY_CODE;
                        needCascade = true;
                    }
                    if (!self.selectedNhom && found.MA_NHOM) {
                        var g = self.listNhoms.find(function (n) {
                            var std = (n.manhom_tbh || '').trim().toLowerCase();
                            var map = (n.manhom_tbh_map || '').trim().toLowerCase();
                            var m = (found.MA_NHOM || '').trim().toLowerCase();
                            return std === m || map === m;
                        });
                        if (g) {
                            self.selectedNhom = g.manhom_tbh;
                            needCascade = true;
                        }
                    }
                    if (needCascade) {
                        self.applyFilterCascades();
                        self.selectedTCV = String(found.ID);
                        self.selectedTCVObj = found;
                    }
                }
            },

            formatTCVOptionLabel: function (t) {
                var self = this;
                if (self.selectedNhom) {
                    return t.TEN_TCV + ' (' + t.MA_TCV + ')';
                }
                var prefix = t.SHORT_PREFIX ? t.SHORT_PREFIX + ' ' : '';
                var groupName = t.TEN_NHOM || t.MA_NHOM;
                return t.TEN_TCV + ' (' + t.MA_TCV + ') - [' + prefix + groupName + ']';
            },

            applyFilterCascades: function () {
                var self = this;

                // 1. Lọc danh sách Nhóm theo selectedCity
                if (!self.selectedCity) {
                    self.filteredNhoms = self.listNhoms;
                } else {
                    var mappedCodes = self.getMappedCodesForCity(self.selectedCity);
                    self.filteredNhoms = self.listNhoms.filter(function (n) {
                        if (!n.city_code) return true;
                        return mappedCodes.indexOf(String(n.city_code).trim().toUpperCase()) !== -1;
                    });
                }

                // Nếu nhóm đã chọn trước đó không còn nằm trong filteredNhoms thì reset
                if (self.selectedNhom) {
                    var nhomExists = self.filteredNhoms.some(function (n) {
                        var std = (n.manhom_tbh || '').trim().toLowerCase();
                        var map = (n.manhom_tbh_map || '').trim().toLowerCase();
                        var cur = self.selectedNhom.trim().toLowerCase();
                        return std === cur || map === cur;
                    });
                    if (!nhomExists) {
                        self.selectedNhom = '';
                    }
                }

                // 2. Lọc danh sách TCV theo selectedCity và selectedNhom
                var mappedCodes = self.selectedCity ? self.getMappedCodesForCity(self.selectedCity) : [];
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
                    var matchCity = !self.selectedCity || mappedCodes.indexOf(tcvCity) !== -1;

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

                // Kiểm tra xem TCV đang chọn có còn trong danh sách không
                if (self.selectedTCV) {
                    var tcvExists = self.filteredTCVs.some(function (t) {
                        return String(t.ID) === String(self.selectedTCV);
                    });
                    if (!tcvExists) {
                        self.selectedTCV = '';
                        self.selectedTCVObj = null;
                    } else {
                        self.selectedTCVObj = self.filteredTCVs.find(function (t) {
                            return String(t.ID) === String(self.selectedTCV);
                        }) || null;
                    }
                } else {
                    self.selectedTCVObj = null;
                }
            },

            clearFilter: function () {
                var self = this;
                var now = new Date();
                var curMonth = now.getMonth() + 1;
                self.loaiKy = 'Thang';
                self.thang = curMonth.toString();
                self.nam = now.getFullYear();
                if (curMonth >= 1 && curMonth <= 3) self.quy = 'I';
                else if (curMonth >= 4 && curMonth <= 6) self.quy = 'II';
                else if (curMonth >= 7 && curMonth <= 9) self.quy = 'III';
                else if (curMonth >= 10 && curMonth <= 12) self.quy = 'IV';
                self.ky6Thang = curMonth <= 6 ? '1' : '2';
                self.updateDateRange();

                self.selectedCity = '';
                self.selectedNhom = '';
                self.selectedTCV = '';
                self.selectedTCVObj = null;
                self.loaiTuVan = 'ALL';
                self.searchL1Keyword = '';
                self.searchL2Keyword = '';
                self.dateError = '';

                self.applyFilterCascades();
                self.loadData();
            },

            loadData: function () {
                var self = this;
                if (!self.validateDateRange(true)) return;

                var queryCity = self.selectedCity || '';
                var queryNhom = self.selectedNhom || '';
                var queryTCV = '';
                var tcvId = null;

                if (self.selectedTCVObj) {
                    tcvId = self.selectedTCVObj.ID;
                    queryTCV = self.selectedTCVObj.MA_TCV;
                    if (!queryNhom) {
                        queryNhom = self.selectedTCVObj.MA_NHOM;
                    }
                    if (!queryCity) {
                        queryCity = self.selectedTCVObj.CITY_CODE;
                    }
                }

                self.isLoading = true;
                $.ajax({
                    url: '/BaoCaoTuVanCD45/SearchTuVan',
                    type: 'POST',
                    data: {
                        fromDate: self.fromDate,
                        toDate: self.toDate,
                        cityCode: queryCity,
                        maNhom: queryNhom,
                        maTCV: queryTCV,
                        loaiTuVan: self.loaiTuVan,
                        tcvId: tcvId
                    },
                    success: function (res) {
                        self.isLoading = false;
                        if (res.Success) {
                            self.listL1 = res.L1 || [];
                            self.listL2 = res.L2 || [];
                            self.filterL1();
                            self.filterL2();

                            if (self.loaiTuVan === 'L2' && self.activeTab === 'l1') {
                                self.activeTab = 'l2';
                            } else if (self.loaiTuVan === 'L1' && self.activeTab === 'l2') {
                                self.activeTab = 'l1';
                            }
                        } else {
                            if (window.toastr) toastr.error(res.Message || 'Lỗi tra cứu báo cáo tư vấn');
                        }
                    },
                    error: function (xhr) {
                        self.isLoading = false;
                        if (window.toastr) toastr.error('Lỗi kết nối khi tải dữ liệu');
                    }
                });
            },

            filterL1: function () {
                var self = this;
                var kw = removeVietnameseTones(self.searchL1Keyword.trim());
                if (!kw) {
                    self.filteredL1 = self.listL1;
                    return;
                }
                self.filteredL1 = self.listL1.filter(function (item) {
                    var str = (item.RECORD_ID || '') + ' ' +
                              (item.MA_KH_NGHIEN_CUU || '') + ' ' +
                              (item.CITY_NAME || '') + ' ' +
                              (item.TEN_NHOM || '') + ' ' +
                              (item.TEN_TCV || '') + ' ' +
                              (item.DIA_DIEM_TEXT || '') + ' ' +
                              (item.Q3A_TEXT || '') + ' ' +
                              (item.Q3B_TEXT || '') + ' ' +
                              (item.Q3C_NHU_CAU || '') + ' ' +
                              (item.Q41_CAN_THIEP_GTH || '') + ' ' +
                              (item.Q42_NHU_CAU_TG || '');
                    return removeVietnameseTones(str).indexOf(kw) >= 0;
                });
            },

            filterL2: function () {
                var self = this;
                var kw = removeVietnameseTones(self.searchL2Keyword.trim());
                if (!kw) {
                    self.filteredL2 = self.listL2;
                    return;
                }
                self.filteredL2 = self.listL2.filter(function (item) {
                    var str = (item.RECORD_ID || '') + ' ' +
                              (item.MA_KH_NGHIEN_CUU || '') + ' ' +
                              (item.LAN_TU_VAN || '') + ' ' +
                              (item.CITY_NAME || '') + ' ' +
                              (item.TEN_NHOM || '') + ' ' +
                              (item.TEN_TCV || '') + ' ' +
                              (item.DIA_DIEM_TEXT || '') + ' ' +
                              (item.Q21A_CAM_THAY || '') + ' ' +
                              (item.Q22_VAN_DE || '') + ' ' +
                              (item.Q31A_CAM_XUC || '') + ' ' +
                              (item.Q31B_CANG_THANG_NGAN || '') + ' ' +
                              (item.Q31C_CANG_THANG_DAI || '') + ' ' +
                              (item.Q31D_THOI_QUEN || '') + ' ' +
                              (item.Q31E_NOI_DUNG_KHAC || '') + ' ' +
                              (item.Q32_CAN_THIEP_CHINH || '') + ' ' +
                              (item.Q41_DONG_Y || '') + ' ' +
                              (item.Q421_VIEC_LAM || '');
                    return removeVietnameseTones(str).indexOf(kw) >= 0;
                });
            },

            exportExcel: function () {
                var self = this;
                if (!self.validateDateRange(true)) return;

                var queryCity = self.selectedCity || '';
                var queryNhom = self.selectedNhom || '';
                var queryTCV = '';
                var tenTCV = '';
                var tenNhom = '';
                var tcvId = '';

                if (self.selectedTCVObj) {
                    tcvId = self.selectedTCVObj.ID;
                    queryTCV = self.selectedTCVObj.MA_TCV;
                    tenTCV = self.selectedTCVObj.TEN_TCV;
                    if (!queryNhom) {
                        queryNhom = self.selectedTCVObj.MA_NHOM;
                    }
                    if (!queryCity) {
                        queryCity = self.selectedTCVObj.CITY_CODE;
                    }
                    tenNhom = self.selectedTCVObj.TEN_NHOM;
                } else if (self.selectedNhom) {
                    var selectedNhomObj = self.listNhoms.find(function (n) { return n.manhom_tbh === self.selectedNhom; });
                    tenNhom = selectedNhomObj ? selectedNhomObj.tennhom_tbh : '';
                }

                var url = '/BaoCaoTuVanCD45/ExportExcel?' +
                    'FromDate=' + encodeURIComponent(self.fromDate) +
                    '&ToDate=' + encodeURIComponent(self.toDate) +
                    '&CityCode=' + encodeURIComponent(queryCity || '') +
                    '&MaNhom=' + encodeURIComponent(queryNhom || '') +
                    '&MaTCV=' + encodeURIComponent(queryTCV || '') +
                    '&LoaiTuVan=' + encodeURIComponent(self.loaiTuVan || 'ALL') +
                    '&TenNhom=' + encodeURIComponent(tenNhom || '') +
                    '&TenTCV=' + encodeURIComponent(tenTCV || '') +
                    (tcvId ? ('&tcvId=' + encodeURIComponent(tcvId)) : '');

                window.location.href = url;
            }
        };
    });
});
