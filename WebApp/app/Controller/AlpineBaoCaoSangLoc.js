function ketQuaSangLocComponent() {
    return {
        modelSearch: {
            FromDate: '',
            ToDate: '',
            CityCodes: null,
            MaNhomTBH: []
        },
        ListCity: [],
        ListNhomTBH: [],
        RoleBtnSearch: false,
        RoleBtnExportExcel: false,
        
        KetQuaTTCBHanhViNguyCos: [],
        CacLoaiChatGayNghienAssists: [],
        KetQuaQSTs: [],
        KetQuaACEs: [],

        init() {
            const date = new Date();
            this.modelSearch.FromDate = date.toISOString().slice(0, 10);
            this.modelSearch.ToDate = date.toISOString().slice(0, 10);
            
            this.GetBottomAction();

            // Khởi tạo Select2 cho các thẻ select
            this.$nextTick(() => {
                $('#CityCodes').select2({
                    placeholder: "Chọn tỉnh",
                    allowClear: true
                }).on('change', (e) => {
                    this.modelSearch.CityCodes = $(e.target).val();
                });

                $('#MaNhomTBH').select2({
                    placeholder: "Chọn nhóm",
                    allowClear: true
                }).on('change', (e) => {
                    this.modelSearch.MaNhomTBH = $(e.target).val() || [];
                });
                
                // Tự động load khi vào trang
                this.LoadPage();
            });
            
            // Theo dõi data source của Select2 để re-init nếu cần (CH07 Pattern)
            this.$watch('ListCity', () => {
                this.$nextTick(() => {
                    $('#CityCodes').select2({ placeholder: "Chọn tỉnh", allowClear: true });
                });
            });
            this.$watch('ListNhomTBH', () => {
                this.$nextTick(() => {
                    $('#MaNhomTBH').select2({ placeholder: "Chọn nhóm", allowClear: true });
                });
            });
        },

        GetBottomAction() {
            $.ajax({
                type: 'post',
                url: '/KetQuaSangLoc/GetBottomAction',
                cache: false,
                async: false,
                data: {},
                success: (response) => {
                    if (response.Buttoms != null) {
                        response.Buttoms.forEach(item => {
                            if (item == 'btnExportExcel') {
                                this.RoleBtnExportExcel = true;
                            }
                            if (item == 'btnSearch') {
                                this.RoleBtnSearch = true;
                            }
                        });

                        this.ListCity = response.Citis || [];
                        this.ListNhomTBH = response.NhomTBHs || [];
                    }
                }
            });
        },

        LoadPage() {
            if (!this.modelSearch.FromDate) {
                toastr.error("Vui lòng chọn Từ ngày!");
                return;
            }

            if (!this.modelSearch.ToDate) {
                toastr.error("Vui lòng chọn Đến ngày!");
                return;
            }

            // showLoading() có thể là hàm global trong hệ thống của bạn (tương tự showToast)
            if (typeof showToast === "function") showToast();

            var inputSearch = {
                FromDate: this.modelSearch.FromDate.replace(/-/g, ""),
                ToDate: this.modelSearch.ToDate.replace(/-/g, ""),
                CityCodes: this.modelSearch.CityCodes,
                MaNhomTBH: this.modelSearch.MaNhomTBH ? this.modelSearch.MaNhomTBH.join(',') : '', // Multiple select, cần join
            };

            $.ajax({
                type: 'post',
                url: '/KetQuaSangLoc/GetAllCD43',
                cache: false,
                async: false,
                data: inputSearch,
                success: (response) => {
                    this.KetQuaTTCBHanhViNguyCos = response.KetQuaTTCBHanhViNguyCos || [];
                    this.CacLoaiChatGayNghienAssists = response.CacLoaiChatGayNghienAssists || [];
                    this.KetQuaQSTs = response.KetQuaQSTs || [];
                    this.KetQuaACEs = response.KetQuaACEs || [];
                }
            });

            if (typeof hideLoading === "function") hideLoading();
        },

        ExportDataSangLoc() {
            if (!this.modelSearch.FromDate) {
                toastr.error("Vui lòng chọn Từ ngày!");
                return;
            }

            if (!this.modelSearch.ToDate) {
                toastr.error("Vui lòng chọn Đến ngày!");
                return;
            }

            let maNhomTBH = this.modelSearch.MaNhomTBH ? this.modelSearch.MaNhomTBH.join(',') : '';

            window.location.href = '/KetQuaSangLoc/ExportDataSangLoc?FromDate=' + this.modelSearch.FromDate.replace(/-/g, "")
                + '&ToDate=' + this.modelSearch.ToDate.replace(/-/g, "")
                + '&CityCodes=' + (this.modelSearch.CityCodes || '')
                + '&maNhomTBHs=' + maNhomTBH;
        }
    }
}
