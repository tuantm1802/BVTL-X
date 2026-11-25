function alpineBaoCaoChiSoCD45() {
    const currentYear = new Date().getFullYear();
    const pad2 = n => (n < 10 ? '0' + n : '' + n);

    return {
        isLoading: false,
        isAdmin: false,
        warningMsg: '',
        cities: [],
        nhoms: [],
        rows: [],
        yearOptions: [2025, 2026, 2027, 2028],

        filters: {
            periodType: '12T',
            nam: currentYear >= 2025 && currentYear <= 2028 ? currentYear : 2026,
            halfYear: new Date().getMonth() < 6 ? 'H1' : 'H2',
            fromDate: `01/01/${currentYear >= 2025 && currentYear <= 2028 ? currentYear : 2026}`,
            toDate: `31/12/${currentYear >= 2025 && currentYear <= 2028 ? currentYear : 2026}`,
            cityMode: 'NEW34',
            maTinh: '',
            maNhom: '',
            isCumulative: true
        },

        viewOptions: {
            colMode: 'ALL',
            showSubRows: true,
            showMetaCols: false
        },

        drillDown: {
            isLoading: false,
            title: '',
            chiTieuCode: '',
            doiTuong: null,
            gioiTinh: null,
            nhomTuoi: null,
            searchText: '',
            items: []
        },

        configModal: {
            isLoading: false,
            isSaving: false,
            nam: currentYear >= 2025 && currentYear <= 2028 ? currentYear : 2026,
            maKy: '12T',
            items: []
        },

        init() {
            const self = this;
            if (!this.yearOptions.includes(currentYear)) {
                this.yearOptions.push(currentYear);
                this.yearOptions.sort((a, b) => a - b);
            }
            this.onPeriodChange(false);
            this.loadFilterData().then(() => {
                self.search();
            });
        },

        get filteredNhoms() {
            if (!this.filters.maTinh) return this.nhoms;
            const city = this.cities.find(c => c.CityCode === this.filters.maTinh);
            const allowedOldCodes = city && Array.isArray(city.OldCodes) ? city.OldCodes : [this.filters.maTinh];
            return this.nhoms.filter(n => n.city_code && allowedOldCodes.includes(n.city_code.trim()));
        },

        get visibleRows() {
            if (this.viewOptions.showSubRows) return this.rows;
            return this.rows.filter(r => r.RowType !== 'SUB');
        },

        get kpiCards() {
            const defs = [
                { Code: 'MTCT1.1', Title: 'Sàng lọc SKTT (QST / F1)' },
                { Code: 'MTCT1.2', Title: 'Tiếp cận >=1 can thiệp CBO' },
                { Code: 'MTCT1.3', Title: 'Có can thiệp sau QST' },
                { Code: 'MTCT1.4', Title: 'QST(+) chuyển gửi CSYT' },
                { Code: 'MTCT1.5', Title: 'SKTT cải thiện (F5)' }
            ];
            return defs.map(def => {
                const row = this.rows.find(r => r.Code === def.Code);
                if (!row) {
                    return { Code: def.Code, Title: def.Title, PctText: '-', RatioText: '' };
                }
                if (row.RowType === 'PCT') {
                    const num = row.Tong || 0;
                    const den = row.Denom_Tong || 0;
                    const pct = den > 0 ? ((num / den) * 100).toFixed(1) + '%' : '0.0%';
                    return {
                        Code: def.Code,
                        Title: def.Title,
                        PctText: pct,
                        RatioText: `${num.toLocaleString('vi-VN')} / ${den.toLocaleString('vi-VN')}`
                    };
                }
                const f1Row = this.rows.find(r => r.Code === 'MTCT1.1_DEN');
                const val = row.Tong != null ? row.Tong : 0;
                return {
                    Code: def.Code,
                    Title: def.Title,
                    PctText: val.toLocaleString('vi-VN'),
                    RatioText: f1Row && f1Row.Tong ? `Trên ${f1Row.Tong.toLocaleString('vi-VN')} F1` : 'Số người (KH)'
                };
            });
        },

        get filteredDrillDown() {
            const q = (this.drillDown.searchText || '').trim().toLowerCase();
            if (!q) return this.drillDown.items;
            return this.drillDown.items.filter(d =>
                (d.RECORD_ID && d.RECORD_ID.toLowerCase().includes(q)) ||
                (d.TEN_TINH && d.TEN_TINH.toLowerCase().includes(q)) ||
                (d.TEN_NHOM && d.TEN_NHOM.toLowerCase().includes(q)) ||
                (d.TEN_TCV && d.TEN_TCV.toLowerCase().includes(q)) ||
                (d.DOI_TUONG_TEXT && d.DOI_TUONG_TEXT.toLowerCase().includes(q)) ||
                (d.CHI_TIET && d.CHI_TIET.toLowerCase().includes(q))
            );
        },

        showGroup(grp) {
            return this.viewOptions.colMode === 'ALL' || this.viewOptions.colMode === grp;
        },

        tableColSpan() {
            let cols = 4; // Code, Name, ĐVT, Tổng
            if (this.showGroup('KP')) cols += 5;
            if (this.showGroup('GT')) cols += 3;
            if (this.showGroup('AGE')) cols += 3;
            if (this.viewOptions.showMetaCols) cols += 2;
            return cols;
        },

        rowClass(item) {
            if (!item) return '';
            if (item.RowType === 'SEC') return 'row-sec';
            if (item.RowType === 'PCT') return 'row-pct';
            if (item.RowType === 'SUB') return 'row-sub';
            if (item.RowType === 'ORG' || item.RowType === 'TCV') return 'row-org';
            return 'row-main';
        },

        syncDateInputs() {
            const txtFrom = document.getElementById('txtFromDate');
            const txtTo = document.getElementById('txtToDate');
            if (txtFrom) txtFrom.value = this.filters.fromDate || '';
            if (txtTo) txtTo.value = this.filters.toDate || '';
        },

        readDateInputsIfCustom() {
            if (this.filters.periodType === 'TuyChon') {
                const txtFrom = document.getElementById('txtFromDate');
                const txtTo = document.getElementById('txtToDate');
                if (txtFrom) this.filters.fromDate = (txtFrom.value || '').trim();
                if (txtTo) this.filters.toDate = (txtTo.value || '').trim();
            }
        },

        onPeriodChange(autoSearch = true) {
            const y = parseInt(this.filters.nam, 10) || currentYear;
            if (this.filters.periodType === '12T') {
                this.filters.fromDate = `01/01/${y}`;
                this.filters.toDate = `31/12/${y}`;
            } else if (this.filters.periodType === '6T') {
                if (this.filters.halfYear === 'H1') {
                    this.filters.fromDate = `01/01/${y}`;
                    this.filters.toDate = `30/06/${y}`;
                } else {
                    this.filters.fromDate = `01/07/${y}`;
                    this.filters.toDate = `31/12/${y}`;
                }
            } else if (this.filters.periodType === 'ALL') {
                this.filters.fromDate = '';
                this.filters.toDate = '';
            }
            this.syncDateInputs();
            if (autoSearch && this.filters.periodType !== 'TuyChon') {
                this.search();
            }
        },

        onCityChange() {
            if (this.filters.maNhom) {
                const stillValid = this.filteredNhoms.some(n => n.manhom_tbh === this.filters.maNhom);
                if (!stillValid) this.filters.maNhom = '';
            }
        },

        switchCityMode(mode) {
            if (this.filters.cityMode === mode) return;
            this.filters.cityMode = mode;
            this.filters.maTinh = '';
            this.filters.maNhom = '';
            this.loadFilterData();
        },

        loadFilterData() {
            const self = this;
            return $.ajax({
                url: '/BaoCaoChiSoCD45/GetFilterData',
                type: 'POST',
                data: { cityMode: self.filters.cityMode }
            }).done(res => {
                if (res && res.Success) {
                    self.cities = res.Cities || [];
                    self.nhoms = res.Nhoms || [];
                    self.isAdmin = !!res.IsAdmin;
                }
            });
        },

        search() {
            const self = this;
            this.readDateInputsIfCustom();
            this.isLoading = true;
            this.warningMsg = '';

            $.ajax({
                url: '/BaoCaoChiSoCD45/SearchBaoCao',
                type: 'POST',
                data: {
                    FromDate: self.filters.fromDate,
                    ToDate: self.filters.toDate,
                    MaTinh: self.filters.maTinh,
                    MaNhom: self.filters.maNhom,
                    IsCumulative: self.filters.isCumulative,
                    Nam: self.filters.periodType === 'ALL' ? null : self.filters.nam,
                    MaKy: self.filters.periodType
                }
            }).done(res => {
                if (res && res.Success) {
                    self.rows = res.Data || [];
                    self.warningMsg = res.Warning || '';
                } else {
                    if (typeof toastr !== 'undefined') toastr.error((res && res.Message) || 'Không thể tải báo cáo');
                    else alert((res && res.Message) || 'Không thể tải báo cáo');
                }
            }).fail(() => {
                if (typeof toastr !== 'undefined') toastr.error('Lỗi kết nối máy chủ khi tải báo cáo.');
            }).always(() => {
                self.isLoading = false;
            });
        },

        resetFilters() {
            const defYear = currentYear >= 2025 && currentYear <= 2028 ? currentYear : 2026;
            this.filters.periodType = '12T';
            this.filters.nam = defYear;
            this.filters.halfYear = new Date().getMonth() < 6 ? 'H1' : 'H2';
            this.filters.maTinh = '';
            this.filters.maNhom = '';
            this.filters.isCumulative = true;
            this.onPeriodChange(true);
        },

        denomFieldFor(field) {
            return 'Denom_' + field;
        },

        formatCell(item, field) {
            if (!item || item.RowType === 'SEC') return '';
            if (item.RowType === 'PCT') {
                const num = item[field];
                const den = item[this.denomFieldFor(field)];
                if (den == null) return '-';
                if (den === 0) return '0.0% (0/0)';
                const nVal = num || 0;
                const pct = ((nVal / den) * 100).toFixed(1);
                return `${pct}% (${nVal.toLocaleString('vi-VN')}/${den.toLocaleString('vi-VN')})`;
            }
            const val = item[field];
            if (val == null) return '-';
            return Number(val).toLocaleString('vi-VN');
        },

        canClickCell(item, field) {
            if (!item || !item.CanDrillDown || item.RowType === 'SEC') return false;
            const val = item[field];
            return val != null && val > 0;
        },

        resolveColumnFilter(field) {
            const kpMap = { PUD: 1, PLHIV: 2, TG: 3, MSM: 4, SW: 5 };
            const gtMap = { Nam: 1, Nu: 2, Khac: 3 };
            const ageMap = { Tuoi_18_25: 1, Tuoi_26_35: 2, Tuoi_Tren35: 3 };
            return {
                doiTuong: kpMap[field] || null,
                gioiTinh: gtMap[field] || null,
                nhomTuoi: ageMap[field] || null
            };
        },

        handleCellClick(item, field, colLabel) {
            if (!this.canClickCell(item, field)) return;
            const colFilter = this.resolveColumnFilter(field);
            this.drillDown.chiTieuCode = item.Code;
            this.drillDown.title = `${item.Code} - ${item.ChiTieu} (${colLabel})`;
            this.drillDown.doiTuong = colFilter.doiTuong;
            this.drillDown.gioiTinh = colFilter.gioiTinh;
            this.drillDown.nhomTuoi = colFilter.nhomTuoi;
            this.drillDown.searchText = '';
            this.drillDown.items = [];
            this.drillDown.isLoading = true;

            this.openModal('#modalChiSoDrillDown');

            const self = this;
            this.readDateInputsIfCustom();
            $.ajax({
                url: '/BaoCaoChiSoCD45/GetDrillDownData',
                type: 'POST',
                data: {
                    ChiTieuCode: self.drillDown.chiTieuCode,
                    FromDate: self.filters.fromDate,
                    ToDate: self.filters.toDate,
                    MaTinh: self.filters.maTinh,
                    MaNhom: self.filters.maNhom,
                    IsCumulative: self.filters.isCumulative,
                    DoiTuong: self.drillDown.doiTuong,
                    GioiTinh: self.drillDown.gioiTinh,
                    NhomTuoi: self.drillDown.nhomTuoi
                }
            }).done(res => {
                if (res && res.Success) {
                    self.drillDown.items = res.Data || [];
                } else if (typeof toastr !== 'undefined') {
                    toastr.error((res && res.Message) || 'Không tải được dữ liệu chi tiết');
                }
            }).fail(() => {
                if (typeof toastr !== 'undefined') toastr.error('Lỗi kết nối khi tải danh sách chi tiết.');
            }).always(() => {
                self.drillDown.isLoading = false;
            });
        },

        exportExcel() {
            this.readDateInputsIfCustom();
            const params = new URLSearchParams({
                FromDate: this.filters.fromDate || '',
                ToDate: this.filters.toDate || '',
                MaTinh: this.filters.maTinh || '',
                MaNhom: this.filters.maNhom || '',
                IsCumulative: this.filters.isCumulative ? 'true' : 'false',
                Nam: this.filters.periodType === 'ALL' ? '' : (this.filters.nam || ''),
                MaKy: this.filters.periodType || '12T',
                IncludeSubRows: this.viewOptions.showSubRows ? 'true' : 'false',
                IncludeMetaCols: this.viewOptions.showMetaCols ? 'true' : 'false'
            });
            window.location.href = '/BaoCaoChiSoCD45/ExportExcel?' + params.toString();
        },

        exportDrillDownExcel() {
            this.readDateInputsIfCustom();
            const params = new URLSearchParams({
                ChiTieuCode: this.drillDown.chiTieuCode || '',
                FromDate: this.filters.fromDate || '',
                ToDate: this.filters.toDate || '',
                MaTinh: this.filters.maTinh || '',
                MaNhom: this.filters.maNhom || '',
                IsCumulative: this.filters.isCumulative ? 'true' : 'false',
                DoiTuong: this.drillDown.doiTuong != null ? this.drillDown.doiTuong : '',
                GioiTinh: this.drillDown.gioiTinh != null ? this.drillDown.gioiTinh : '',
                NhomTuoi: this.drillDown.nhomTuoi != null ? this.drillDown.nhomTuoi : ''
            });
            window.location.href = '/BaoCaoChiSoCD45/ExportDrillDownExcel?' + params.toString();
        },

        openConfigModal() {
            this.configModal.nam = this.filters.nam || 2026;
            this.configModal.maKy = (this.filters.periodType === 'TuyChon' ? '12T' : this.filters.periodType) || '12T';
            this.openModal('#modalChiSoConfig');
            this.loadConfig();
        },

        loadConfig() {
            const self = this;
            this.configModal.isLoading = true;
            $.ajax({
                url: '/BaoCaoChiSoCD45/GetConfig',
                type: 'POST',
                data: { Nam: self.configModal.nam, MaKy: self.configModal.maKy }
            }).done(res => {
                if (res && res.Success) {
                    self.configModal.items = res.Data || [];
                } else if (typeof toastr !== 'undefined') {
                    toastr.error((res && res.Message) || 'Không tải được cấu hình');
                }
            }).always(() => {
                self.configModal.isLoading = false;
            });
        },

        saveConfig() {
            const self = this;
            this.configModal.isSaving = true;
            $.ajax({
                url: '/BaoCaoChiSoCD45/SaveConfig',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    Nam: self.configModal.nam,
                    MaKy: self.configModal.maKy,
                    Items: self.configModal.items
                })
            }).done(res => {
                if (res && res.Success) {
                    if (typeof toastr !== 'undefined') toastr.success(res.Message || 'Đã lưu cấu hình!');
                    self.closeModal('#modalChiSoConfig');
                    self.search();
                } else if (typeof toastr !== 'undefined') {
                    toastr.error((res && res.Message) || 'Lưu cấu hình thất bại');
                }
            }).fail(() => {
                if (typeof toastr !== 'undefined') toastr.error('Lỗi kết nối khi lưu cấu hình.');
            }).always(() => {
                self.configModal.isSaving = false;
            });
        },

        openModal(selector) {
            if (window.bootstrap && window.bootstrap.Modal) {
                const el = document.querySelector(selector);
                if (el) {
                    window.bootstrap.Modal.getOrCreateInstance(el).show();
                    return;
                }
            }
            if (window.$ && $.fn.modal) {
                $(selector).modal('show');
            }
        },

        closeModal(selector) {
            if (window.bootstrap && window.bootstrap.Modal) {
                const el = document.querySelector(selector);
                if (el) {
                    const inst = window.bootstrap.Modal.getInstance(el);
                    if (inst) { inst.hide(); return; }
                }
            }
            if (window.$ && $.fn.modal) {
                $(selector).modal('hide');
            }
        }
    };
}
