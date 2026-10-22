document.addEventListener('alpine:init', function () {
    Alpine.data('alpineBaoCaoBacSiCD45', function () {
        return {
            summary: [],
            details: [],
            filteredDetails: [],
            detailSearch: '',
            activeTab: 'summary', // 'summary' | 'details'
            isLoading: false,

            loaiKy: 'Thang', // 'Thang' | 'TuyChon'
            thang: (new Date().getMonth() + 1).toString(),
            nam: new Date().getFullYear(),
            fromDate: '',
            toDate: '',
            dateError: '',

            cityMode: 'NEW34',
            selectedCity: '',
            selectedDoctor: '',
            selectedNhom: '',

            listCities: [],
            listDoctors: [],
            listNhoms: [],
            filteredDoctors: [],
            filteredNhoms: [],
            isAdmin: true,

            init: function () {
                var self = this;
                var now = new Date();
                self.thang = (now.getMonth() + 1).toString();
                self.nam = now.getFullYear();

                self.updateDateRange();
                self.loadDanhMuc();
            },

            formatNumber: function (val) {
                if (val === null || val === undefined || val === 0 || val === '0') return '-';
                return Number(val).toLocaleString('vi-VN');
            },

            setCityMode: function (mode) {
                if (this.cityMode !== mode) {
                    this.cityMode = mode;
                    this.selectedCity = '';
                    this.selectedDoctor = '';
                    this.selectedNhom = '';
                    this.loadDanhMuc();
                }
            },

            updateDateRange: function () {
                var self = this;
                var y = parseInt(self.nam) || new Date().getFullYear();

                if (self.loaiKy === 'Thang') {
                    var m = parseInt(self.thang) || (new Date().getMonth() + 1);
                    // Báo cáo Bác sĩ chạy từ ngày 1 tới ngày cuối cùng của tháng
                    var strM = m < 10 ? '0' + m : m;
                    var lastDay = new Date(y, m, 0).getDate();
                    var strLastDay = lastDay < 10 ? '0' + lastDay : lastDay;

                    self.fromDate = '01/' + strM + '/' + y;
                    self.toDate = strLastDay + '/' + strM + '/' + y;
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
                    var d = new Date(year, month, day);
                    if (d.getFullYear() === year && d.getMonth() === month && d.getDate() === day) {
                        return d;
                    }
                }
                return null;
            },

            loadDanhMuc: function () {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoBacSiCD45/GetFilterData',
                    data: { cityMode: self.cityMode },
                    success: function (res) {
                        if (res.Success) {
                            self.listCities = res.Cities || [];
                            self.listDoctors = res.Doctors || [];
                            self.listNhoms = res.Nhoms || [];
                            self.filteredDoctors = self.listDoctors;
                            self.filteredNhoms = self.listNhoms;
                            self.isAdmin = res.IsAdmin !== undefined ? res.IsAdmin : true;
                        }
                        self.loadData();
                    },
                    error: function () {
                        self.loadData();
                    }
                });
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

            onCityChange: function () {
                var self = this;
                if (!self.selectedCity) {
                    self.filteredDoctors = self.listDoctors;
                    self.filteredNhoms = self.listNhoms;
                } else {
                    var mappedCodes = self.getMappedCodesForCity(self.selectedCity);

                    self.filteredDoctors = self.listDoctors.filter(function (d) {
                        if (!d.CITY_CODE) return true;
                        return mappedCodes.indexOf(String(d.CITY_CODE).trim().toUpperCase()) !== -1;
                    });
                    self.filteredNhoms = self.listNhoms.filter(function (n) {
                        if (!n.city_code) return true;
                        return mappedCodes.indexOf(String(n.city_code).trim().toUpperCase()) !== -1;
                    });
                }

                // Nếu Bác sĩ đang chọn không thuộc Tỉnh/TP mới thì reset
                if (self.selectedDoctor) {
                    var docExists = self.filteredDoctors.some(function (d) {
                        return d.MA_BAC_SI === self.selectedDoctor;
                    });
                    if (!docExists) {
                        self.selectedDoctor = '';
                    }
                }

                // Nếu Nhóm đang chọn không thuộc Tỉnh/TP mới thì reset
                if (self.selectedNhom) {
                    var nhomExists = self.filteredNhoms.some(function (n) {
                        return n.manhom_tbh === self.selectedNhom;
                    });
                    if (!nhomExists) {
                        self.selectedNhom = '';
                    }
                }
            },

            onDoctorChange: function () {
                var self = this;
                if (!self.selectedDoctor) {
                    // Khi người dùng chọn lại "-- Tất cả bác sĩ --"
                    if (self.selectedCity) {
                        var mappedCodes = self.getMappedCodesForCity(self.selectedCity);
                        self.filteredDoctors = self.listDoctors.filter(function (d) {
                            if (!d.CITY_CODE) return true;
                            return mappedCodes.indexOf(String(d.CITY_CODE).trim().toUpperCase()) !== -1;
                        });
                        self.filteredNhoms = self.listNhoms.filter(function (n) {
                            if (!n.city_code) return true;
                            return mappedCodes.indexOf(String(n.city_code).trim().toUpperCase()) !== -1;
                        });
                    } else {
                        self.filteredDoctors = self.listDoctors;
                        if (self.selectedNhom) {
                            var nhomObj = self.listNhoms.find(function (n) { return n.manhom_tbh === self.selectedNhom; });
                            if (nhomObj && nhomObj.city_code) {
                                var nCodes = self.getMappedCodesForCity(self.findCityCodeByRawCode(nhomObj.city_code) || nhomObj.city_code);
                                self.filteredDoctors = self.listDoctors.filter(function (d) {
                                    if (!d.CITY_CODE) return true;
                                    return nCodes.indexOf(String(d.CITY_CODE).trim().toUpperCase()) !== -1;
                                });
                            }
                        } else {
                            self.filteredNhoms = self.listNhoms;
                        }
                    }
                    return;
                }

                var doc = self.listDoctors.find(function (d) { return d.MA_BAC_SI === self.selectedDoctor; });
                if (doc && doc.CITY_CODE) {
                    var matchedCity = self.findCityCodeByRawCode(doc.CITY_CODE);
                    if (matchedCity) {
                        self.selectedCity = matchedCity;
                    }

                    var mappedCodes = matchedCity ? self.getMappedCodesForCity(matchedCity) : [String(doc.CITY_CODE).trim().toUpperCase()];

                    // Lọc nhóm theo Tỉnh/TP của bác sĩ
                    self.filteredNhoms = self.listNhoms.filter(function (n) {
                        if (!n.city_code) return true;
                        return mappedCodes.indexOf(String(n.city_code).trim().toUpperCase()) !== -1;
                    });

                    // Lọc danh sách bác sĩ cùng Tỉnh/TP (vẫn giữ bác sĩ hiện tại)
                    self.filteredDoctors = self.listDoctors.filter(function (d) {
                        if (!d.CITY_CODE) return true;
                        return mappedCodes.indexOf(String(d.CITY_CODE).trim().toUpperCase()) !== -1;
                    });

                    // Nếu nhóm đang chọn không thuộc Tỉnh/TP này thì reset
                    if (self.selectedNhom) {
                        var nhomExists = self.filteredNhoms.some(function (n) {
                            return n.manhom_tbh === self.selectedNhom;
                        });
                        if (!nhomExists) {
                            self.selectedNhom = '';
                        }
                    }
                }
            },

            onNhomChange: function () {
                var self = this;
                if (!self.selectedNhom) {
                    // Khi người dùng chọn lại "-- Tất cả nhóm --"
                    if (self.selectedCity) {
                        var mappedCodes = self.getMappedCodesForCity(self.selectedCity);
                        self.filteredDoctors = self.listDoctors.filter(function (d) {
                            if (!d.CITY_CODE) return true;
                            return mappedCodes.indexOf(String(d.CITY_CODE).trim().toUpperCase()) !== -1;
                        });
                        self.filteredNhoms = self.listNhoms.filter(function (n) {
                            if (!n.city_code) return true;
                            return mappedCodes.indexOf(String(n.city_code).trim().toUpperCase()) !== -1;
                        });
                    } else {
                        self.filteredNhoms = self.listNhoms;
                        if (self.selectedDoctor) {
                            var docObj = self.listDoctors.find(function (d) { return d.MA_BAC_SI === self.selectedDoctor; });
                            if (docObj && docObj.CITY_CODE) {
                                var dCodes = self.getMappedCodesForCity(self.findCityCodeByRawCode(docObj.CITY_CODE) || docObj.CITY_CODE);
                                self.filteredNhoms = self.listNhoms.filter(function (n) {
                                    if (!n.city_code) return true;
                                    return dCodes.indexOf(String(n.city_code).trim().toUpperCase()) !== -1;
                                });
                            }
                        } else {
                            self.filteredDoctors = self.listDoctors;
                        }
                    }
                    return;
                }

                var nhom = self.listNhoms.find(function (n) { return n.manhom_tbh === self.selectedNhom; });
                if (nhom && nhom.city_code) {
                    var matchedCity = self.findCityCodeByRawCode(nhom.city_code);
                    if (matchedCity) {
                        self.selectedCity = matchedCity;
                    }

                    var mappedCodes = matchedCity ? self.getMappedCodesForCity(matchedCity) : [String(nhom.city_code).trim().toUpperCase()];

                    // Lọc bác sĩ theo Tỉnh/TP của nhóm
                    self.filteredDoctors = self.listDoctors.filter(function (d) {
                        if (!d.CITY_CODE) return true;
                        return mappedCodes.indexOf(String(d.CITY_CODE).trim().toUpperCase()) !== -1;
                    });

                    // Lọc danh sách nhóm cùng Tỉnh/TP (vẫn giữ nhóm hiện tại)
                    self.filteredNhoms = self.listNhoms.filter(function (n) {
                        if (!n.city_code) return true;
                        return mappedCodes.indexOf(String(n.city_code).trim().toUpperCase()) !== -1;
                    });

                    // Nếu bác sĩ đang chọn không thuộc Tỉnh/TP này thì reset
                    if (self.selectedDoctor) {
                        var docExists = self.filteredDoctors.some(function (d) {
                            return d.MA_BAC_SI === self.selectedDoctor;
                        });
                        if (!docExists) {
                            self.selectedDoctor = '';
                        }
                    }
                }
            },

            clearFilter: function () {
                var self = this;
                var now = new Date();
                self.loaiKy = 'Thang';
                self.thang = (now.getMonth() + 1).toString();
                self.nam = now.getFullYear();
                self.updateDateRange();

                self.selectedCity = '';
                self.selectedDoctor = '';
                self.selectedNhom = '';
                self.detailSearch = '';
                self.dateError = '';

                self.filteredDoctors = self.listDoctors;
                self.filteredNhoms = self.listNhoms;

                self.loadData();
            },

            loadData: function () {
                var self = this;
                if (!self.validateDateRange(true)) return;
                self.isLoading = true;

                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoBacSiCD45/SearchBaoCao',
                    data: {
                        FromDate: self.fromDate,
                        ToDate: self.toDate,
                        MaTinh: self.selectedCity,
                        DoctorId: self.selectedDoctor,
                        MaNhom: self.selectedNhom
                    },
                    success: function (res) {
                        self.isLoading = false;
                        if (res.Success) {
                            self.summary = res.Summary || [];
                            self.details = res.Details || [];
                            self.filterDetails();
                        } else {
                            if (window.toastr) toastr.error(res.Message);
                        }
                    },
                    error: function () {
                        self.isLoading = false;
                        if (window.toastr) toastr.error('Có lỗi xảy ra khi tải dữ liệu báo cáo bác sĩ!');
                    }
                });
            },

            filterDetails: function () {
                var self = this;
                var q = (self.detailSearch || '').trim().toLowerCase();
                if (!q) {
                    self.filteredDetails = self.details;
                    return;
                }
                self.filteredDetails = self.details.filter(function (d) {
                    return (d.RECORD_ID && d.RECORD_ID.toLowerCase().indexOf(q) !== -1) ||
                           (d.TEN_BAC_SI && d.TEN_BAC_SI.toLowerCase().indexOf(q) !== -1) ||
                           (d.TEN_TINH && d.TEN_TINH.toLowerCase().indexOf(q) !== -1) ||
                           (d.TEN_NHOM && d.TEN_NHOM.toLowerCase().indexOf(q) !== -1) ||
                           (d.TEN_CO_SO_Y_TE && d.TEN_CO_SO_Y_TE.toLowerCase().indexOf(q) !== -1) ||
                           (d.TEN_CHAN_DOAN && d.TEN_CHAN_DOAN.toLowerCase().indexOf(q) !== -1) ||
                           (d.TEN_HINH_THUC_DIEU_TRI && d.TEN_HINH_THUC_DIEU_TRI.toLowerCase().indexOf(q) !== -1);
                });
            },

            getTreatmentBadgeClass: function (val) {
                if (!val) return 'badge-secondary';
                var s = String(val).replace(/\s+/g, '');
                if (s.indexOf('2') !== -1) return 'badge-danger'; // Nội trú
                if (s.indexOf('1') !== -1) return 'badge-success'; // Ngoại trú (hoặc Ngoại trú kết hợp)
                if (s.indexOf('3') !== -1) return 'badge-info'; // Tư vấn tâm lý
                return 'badge-secondary';
            },

            formatTreatment: function (val, defaultTen) {
                if (defaultTen && defaultTen !== '-' && defaultTen !== val) {
                    return defaultTen;
                }
                if (!val) return '-';
                var dict = {
                    "1": "Ngoại trú",
                    "2": "Nội trú",
                    "3": "Tư vấn tâm lý",
                    "4": "Khác"
                };
                var parts = String(val).split(/[,;]/);
                var names = [];
                for (var i = 0; i < parts.length; i++) {
                    var k = parts[i].trim();
                    if (k) {
                        names.push(dict[k] || k);
                    }
                }
                return names.length > 0 ? names.join(', ') : '-';
            },

            exportExcel: function () {
                var self = this;
                if (!self.validateDateRange(true)) return;

                var url = '/BaoCaoBacSiCD45/ExportExcel?FromDate=' + encodeURIComponent(self.fromDate) +
                    '&ToDate=' + encodeURIComponent(self.toDate) +
                    '&MaTinh=' + encodeURIComponent(self.selectedCity) +
                    '&DoctorId=' + encodeURIComponent(self.selectedDoctor) +
                    '&MaNhom=' + encodeURIComponent(self.selectedNhom);
                window.location.href = url;
            },

            getTotalSummary: function (key) {
                var self = this;
                if (!self.summary || self.summary.length === 0) return 0;
                return self.summary.reduce(function (sum, item) {
                    return sum + (item[key] || 0);
                }, 0);
            }
        };
    });
});
