function baoCaoQuyCH07Component() {
    return {
        modelSearch: {
            Year: new Date().getFullYear(),
            CityCodes: '',
            MaNhomTBH: '',
            Months: '',
            MaDuAn: ''
        },
        Quy: '',
        ListYear: [],
        ListCity: [],
        ListCityCode: [],
        ListMaNhomTBH: [],
        ListNhomTBH: [],
        ListDuAn: [],
        ListData: [],
        RoleBtnSearch: false,
        RoleBtnUpdate: false,

        init() {
            // Generate years
            const currentYear = new Date().getFullYear();
            for (let i = currentYear - 5; i < currentYear + 5; i++) {
                this.ListYear.push({ Id: i, Name: i.toString() });
            }

            // Init quarter based on current month
            const month = new Date().getMonth() + 1;
            if (month >= 1 && month <= 3) this.Quy = "I";
            else if (month >= 4 && month <= 6) this.Quy = "II";
            else if (month >= 7 && month <= 9) this.Quy = "III";
            else this.Quy = "IV";

            this.getBottomAction().then(() => {
                this.changeCity().then(() => {
                    this.loadPage();
                });
            });
        },

        async getBottomAction() {
            try {
                const response = await fetch('/BaoCaoQuyCH07/GetBottomAction', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                });
                const data = await response.json();
                if (data.Buttoms) {
                    data.Buttoms.forEach(item => {
                        if (item === 'btnUpdate') this.RoleBtnUpdate = true;
                        if (item === 'btnSearch') this.RoleBtnSearch = true;
                    });
                }
                this.ListCity = data.Citys || [];
                this.ListDuAn = data.DuAns || [];
                if (this.ListDuAn.length > 0) {
                    this.modelSearch.MaDuAn = this.ListDuAn[0].maduan;
                }
            } catch (err) {
                console.error("GetBottomAction error", err);
            }
        },

        async changeCity() {
            let citys = this.ListCityCode.join(',');
            try {
                const formData = new URLSearchParams();
                formData.append('CityCodes', citys);

                const response = await fetch('/BaoCaoQuyCH07/GetNhomTBHByCityCodes', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    body: formData
                });
                const data = await response.json();
                this.ListNhomTBH = data.NhomTBHs || [];
                this.ListMaNhomTBH = []; // Reset selected groups when city changes
            } catch (err) {
                console.error("GetNhomTBHByCityCodes error", err);
            }
        },

        async loadPage() {
            if (!this.modelSearch.Year) {
                if(typeof toastr !== 'undefined') toastr.error("Vui lòng chọn năm!");
                return;
            }

            if (!this.Quy) {
                if(typeof toastr !== 'undefined') toastr.error("Vui lòng chọn quý!");
                return;
            }

            switch(this.Quy) {
                case 'I': this.modelSearch.Months = '1,2,3'; break;
                case 'II': this.modelSearch.Months = '4,5,6'; break;
                case 'III': this.modelSearch.Months = '7,8,9'; break;
                case 'IV': this.modelSearch.Months = '10,11,12'; break;
            }

            this.modelSearch.CityCodes = this.ListCityCode.join(',');
            this.modelSearch.MaNhomTBH = this.ListMaNhomTBH.join(',');

            try {
                const formData = new URLSearchParams();
                for (const key in this.modelSearch) {
                    formData.append(key, this.modelSearch[key] || '');
                }

                const response = await fetch('/BaoCaoQuyCH07/SearchData', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    body: formData
                });
                const data = await response.json();
                this.ListData = data.data || [];
            } catch (err) {
                console.error("SearchData error", err);
            }
        },

        exportExcel() {
            if (!this.modelSearch.Year) {
                if(typeof toastr !== 'undefined') toastr.error("Vui lòng chọn năm!");
                return;
            }
            if (!this.modelSearch.MaDuAn) {
                if(typeof toastr !== 'undefined') toastr.error("Vui lòng chọn dự án!");
                return;
            }

            this.modelSearch.CityCodes = this.ListCityCode.join(',');
            this.modelSearch.MaNhomTBH = this.ListMaNhomTBH.join(',');

            let url = '/BaoCaoQuyCH07/ExportData?Months=' + this.modelSearch.Months 
                + '&Year=' + this.modelSearch.Year
                + '&CityCodes=' + this.modelSearch.CityCodes 
                + '&quy=' + this.Quy
                + '&maNhomTBHs=' + this.modelSearch.MaNhomTBH
                + '&maDuAn=' + this.modelSearch.MaDuAn;

            window.location.href = url;
        }
    }
}
