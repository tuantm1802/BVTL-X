app.controller("QuanLyHopDongController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location, $timeout) {
    var today = moment().format('DD/MM/YYYY');
    const tablePagingBase = { pageIndex: 1, pageSize: 10, totalItems: 0, timKiem: "" }
    const tableNoPagingBase = { timKiem: null, pageIndex: 1, pageSize: 1000 }
    const baseTableAction = { checkedAll: false, timKiem: "" }
    $scope.hdHopDong = [];
    $scope.allSP = [];
    $scope.hdPaging = { ...tablePagingBase, namKP: "", type: "" }
    $scope.isDisableHD = false;
    $scope.isChoDuyet = false;
    $scope.listNamKinhPhi = [];
    $scope.listCongTy = [];

    $scope.listSP = [];
    $scope.listSPOpts = { ...baseTableAction };
    $scope.listSPAdd = [];
    $scope.listSPAddOpts = { ...baseTableAction };

    $scope.listPhieuChuaChot = [];
    $scope.listVatTuPhu = [];
    $scope.listVatTuPhuDuocChon = [];
    $scope.listVatTuPhuBanDau = [];
    $scope.AddSPVTNTBanDauCheckAll = false;
    $scope.AddSPVTNTDuocChonCheckAll = false;
    $scope.listSanPhamCuaKho = [];
    $scope.listSanPhamModal = [];
    $scope.listSanPhamAdd = [];
    $scope.listNguoiPhuTrach = [];
    $scope.listDonVi = [];
    $scope.listLanhDao = [];
    $scope.listPhuongThucVanChuyen = [];
    $scope.listQuyetToanPhieuNhapKho = [];
    $scope.listQuyetToanPhieuGiaoThang = [];
    $scope.listQuyetToanPhieuXuatThang = [];

    //#region button role
    $scope.chotSLTTHopDong = false;
    $scope.chuyenPhieuGiaoThang = false;
    $scope.chuyenPhieuNhapKho = false;
    $scope.chuyenPhieuXuatThang = false;
    $scope.dungCapPhat = false;
    $scope.luuChuyenKinhPhiKetDu = false;
    $scope.luuDieuChuyenVatTuChinhKhongQuaKho = false;
    $scope.luuGiaBaoNghiemThuKho = false;
    $scope.luuGiaoThangVatTuChinhKhongQuaKho = false;
    $scope.luuGiaoVatTuChinhQuaKho = false;
    $scope.luuHopDong = false;
    $scope.luuNghiemThuVatTuPhu = false;
    $scope.luuNhapKhoBo = false;
    $scope.luuNhapTraHopDong = false;
    $scope.luuNhapTraVatTuChinh = false;
    $scope.luuNoiDungBanDau = false;
    $scope.luuPhuLuc = false;
    $scope.luuVaySec = false;
    $scope.luuXuatThangKhoHopDong = false;
    $scope.quyetToan = false;
    $scope.viewDinhMucVatTu = false;
    $scope.viewNoiDungBanDau = false;
    $scope.viewVaySec = false;
    $scope.xoaHopDong = false;
    $scope.xoaChuyenKinhPhiKetDu = false;
    $scope.xoaDieuChuyenVatTuChinhKhongQuaKho = false;
    $scope.xoaGiaBaoNghiemThuKho = false;
    $scope.xoaGiaoThangVatTuChinhKhongQuaKho = false;
    $scope.xoaGiaoVatTuChinhQuaKho = false;
    $scope.xoaNghiemThuVatTuPhu = false;
    $scope.xoaNhapKhoBo = false;
    $scope.xoaNhapTraHopDong = false;
    $scope.xoaNhapTraVatTuChinh = false;
    $scope.xoaNoiDungBanDau = false;
    $scope.xoaPhuLuc = false;
    $scope.xoaVaySec = false;
    $scope.xoaXuatThangKhoHopDong = false;
    //#endregion button role

    $scope.listLoaiHD = [
        { id: "", name: "--Chọn loại hợp đồng--" },
        { id: "1", name: "Gia công - mua bán - sản xuất" },
        { id: "2", name: "Gia công - may đo" }
    ];


    $scope.listTrangThaiHD = [
        { id: "", name: "--Chọn trạng thái--" },
        { id: "T", name: "Tạm" },
        { id: "C", name: "Chính thức" }
    ]

    $scope.modelPhuLucAE = {
        giaTriPL: 0
    }

    var currentYear = moment().year();
    $scope.modelSearch = {
        ngayBatDauHopDong: today,
        ngayKetThucHopDong: today,
        ngayBatDauGiaoHang: today,
        ngayKetThucGiaoHang: today,
        pageIndex: 1,
        pageSize: 10
    }

    $scope.modelPhuluc = { ...tablePagingBase };
    $scope.modelPhulucChiTiet = { ...tablePagingBase };
    $scope.modelNTVatTu = { ...tablePagingBase };
    $scope.modelVatTuQuaKho = { ...tablePagingBase };
    $scope.modelVatTuKhongQuaKho = { ...tablePagingBase };
    $scope.modelDieuChuyenVatTuKhongQuaKho = { ...tablePagingBase };
    $scope.modelVatTuNhapTra = { ...tablePagingBase };

    $scope.modelHopDong = {
        ngayKy: "",
        ngayThanhLy: ""
    }

    $scope.modelHopDongAE = {}

    $scope.modelNTVTAE = {
        id: "",
        soNghiemThu: null,
        ngayNghiemThu: "",
        nguoiNghiemThu: "",
        moTa: ""
    }

    const modelVatTuBase = {
        soPhieu: '',
        ngayThang: today,
        soPhieuH55: '',
        nguoiNhan: '',
        noiNhan: '',
        noiGiao: '',
        moTa: '',
        lanhDao: '',
        phuongThucVanChuyen: '',
        capNhatSLTT: false
    }

    $scope.modelVTAE = {
        type: 1, ...modelVatTuBase
    };
    $scope.modelGTVTKhongQuaKhoAE = {
        type: 2, ...modelVatTuBase
    };
    $scope.modelDieuChuyenVTKhongQuaKhoAE = {
        type: 3, ...modelVatTuBase
    };
    $scope.modelNhapTraVatTuAE = {
        type: 3, ...modelVatTuBase
    };

    const editFieldBase = {
        value: null,
        text: null,
        edit: true
    }
    // #region First Load
    angular.element(document).ready(function () {
        $('body').addClass("hd-overflow");
        for (var i = currentYear; i >= (currentYear - 5); i--) {
            $scope.listNamKinhPhi.push({ key: i.toString(), value: i.toString() });
        }
        $scope.hdPaging.namKP = currentYear.toString();
        $scope.listNamKinhPhi.unshift({ key: "", value: "--Chọn năm kinh phí --" })
        $scope.callAPI('GET', 'GetDanhMucCongTy', {}, $scope.danhMucCongTyListResponse);
        $scope.callAPI('GET', 'GetAllVatTuPhu', {}, $scope.vatTuPhuListResponse);
        $scope.callAPI('GET', 'GetNguoiPhuTrach', {}, $scope.nguoiPhuTrachListResponse);
        $scope.callAPI('GET', 'GetAllHopDong', {}, $scope.danhMucHopDongListResponse);
        $scope.callAPI('GET', 'GetDonVi', {}, $scope.donViListResponse);
        $scope.callAPI('GET', 'GetKho', {}, $scope.khoListResponse);
        $scope.callAPI('GET', 'GetDanhSachLanhDao', {}, $scope.lanhDaoListResponse);
        $scope.callAPI('GET', 'GetDanhMucPhuongThucVanChuyen', {}, $scope.phuongThucVanChuyenListResponse);
        $scope.getListSanPham();
        $scope.callAPI('GET', 'GetBottomAction', {}, $scope.buttonListListResponse);
        $timeout(function () {
            angular.element('a[href="#ds_hop_dong"]').trigger('click');
        });
    });

    $scope.hdDanhSachClicked = (type) => {
        $scope.hdPaging.type = type;
        $scope.hdPaging.pageIndex = 1;
        $scope.hdPaging.namKP = currentYear.toString();
        $scope.hdPaging.timKiem = "";
        $scope.getDanhSachHopDong();
    }
    $scope.danhMucCongTyListResponse = (data) => {
        $scope.listCongTy = data.data;
        $scope.listCongTy.unshift({ ID: "", TEN_CONG_TY: "--Chọn công ty --" })
    }

    //#region set button role
    $scope.buttonListListResponse = (data) => {
        angular.forEach(data.data, function (item) {
            if (item == 'chotSLTTHopDong') $scope.chotSLTTHopDong = true;
            if (item == 'chuyenPhieuGiaoThang') $scope.chuyenPhieuGiaoThang = true;
            if (item == 'chuyenPhieuNhapKho') $scope.chuyenPhieuNhapKho = true;
            if (item == 'chuyenPhieuXuatThang') $scope.chuyenPhieuXuatThang = true;
            if (item == 'dungCapPhat') $scope.dungCapPhat = true;
            if (item == 'luuChuyenKinhPhiKetDu') $scope.luuChuyenKinhPhiKetDu = true;
            if (item == 'luuDieuChuyenVatTuChinhKhongQuaKho') $scope.luuDieuChuyenVatTuChinhKhongQuaKho = true;
            if (item == 'luuGiaBaoNghiemThuKho') $scope.luuGiaBaoNghiemThuKho = true;
            if (item == 'luuGiaoThangVatTuChinhKhongQuaKho') $scope.luuGiaoThangVatTuChinhKhongQuaKho = true;
            if (item == 'luuGiaoVatTuChinhQuaKho') $scope.luuGiaoVatTuChinhQuaKho = true;
            if (item == 'luuHopDong') $scope.luuHopDong = true;
            if (item == 'luuNghiemThuVatTuPhu') $scope.luuNghiemThuVatTuPhu = true;
            if (item == 'luuNhapKhoBo') $scope.luuNhapKhoBo = true;
            if (item == 'luuNhapTraHopDong') $scope.luuNhapTraHopDong = true;
            if (item == 'luuNhapTraVatTuChinh') $scope.luuNhapTraVatTuChinh = true;
            if (item == 'luuNoiDungBanDau') $scope.luuNoiDungBanDau = true;
            if (item == 'luuPhuLuc') $scope.luuPhuLuc = true;
            if (item == 'luuVaySec') $scope.luuVaySec = true;
            if (item == 'luuXuatThangKhoHopDong') $scope.luuXuatThangKhoHopDong = true;
            if (item == 'quyetToan') $scope.quyetToan = true;
            if (item == 'viewDinhMucVatTu') $scope.viewDinhMucVatTu = true;
            if (item == 'viewNoiDungBanDau') $scope.viewNoiDungBanDau = true;
            if (item == 'viewVaySec') $scope.viewVaySec = true;
            if (item == 'xoaHopDong') $scope.xoaHopDong = true;
            if (item == 'xoaChuyenKinhPhiKetDu') $scope.xoaChuyenKinhPhiKetDu = true;
            if (item == 'xoaDieuChuyenVatTuChinhKhongQuaKho') $scope.xoaDieuChuyenVatTuChinhKhongQuaKho = true;
            if (item == 'xoaGiaBaoNghiemThuKho') $scope.xoaGiaBaoNghiemThuKho = true;
            if (item == 'xoaGiaoThangVatTuChinhKhongQuaKho') $scope.xoaGiaoThangVatTuChinhKhongQuaKho = true;
            if (item == 'xoaGiaoVatTuChinhQuaKho') $scope.xoaGiaoVatTuChinhQuaKho = true;
            if (item == 'xoaNghiemThuVatTuPhu') $scope.xoaNghiemThuVatTuPhu = true;
            if (item == 'xoaNhapKhoBo') $scope.xoaNhapKhoBo = true;
            if (item == 'xoaNhapTraHopDong') $scope.xoaNhapTraHopDong = true;
            if (item == 'xoaNhapTraVatTuChinh') $scope.xoaNhapTraVatTuChinh = true;
            if (item == 'xoaNoiDungBanDau') $scope.xoaNoiDungBanDau = true;
            if (item == 'xoaPhuLuc') $scope.xoaPhuLuc = true;
            if (item == 'xoaVaySec') $scope.xoaVaySec = true;
            if (item == 'xoaXuatThangKhoHopDong') $scope.xoaXuatThangKhoHopDong = true;
            
        })
    }
    //#endregion set button role

    $scope.nguoiPhuTrachListResponse = (data) => {
        $scope.listNguoiPhuTrach = data.data;
        $scope.listNguoiPhuTrach.unshift({ USER_ID: "", FULL_NAME: "--Chọn người phụ trách--" })
    }

    $scope.danhMucHopDongListResponse = (data) => {
        $scope.listHopdong = [];
        $scope.listHopdong = data.data;
        $scope.listHopdong.unshift({ ID_HOP_DONG: "", NAME_HOP_DONG: "--Chọn hợp đồng --" })
    }

    $scope.donViListResponse = (data) => {
        $scope.listDonVi = data.data;
        $scope.listDonVi.unshift({ ID: "", TEN_DON_VI: "--Chọn đơn vị --" })
    }

    $scope.lanhDaoListResponse = (data) => {
        $scope.listLanhDao = data.data;
        $scope.listLanhDao.unshift({ USER_ID: "", FULL_NAME: "--Chọn lãnh đạo --" })
    }

    $scope.phuongThucVanChuyenListResponse = (data) => {
        $scope.listPhuongThucVanChuyen = data.data;
        $scope.listPhuongThucVanChuyen.unshift({ FLEX_VALUE_ID: "", DESCRIPTION: "--Chọn phương thức --" })
    }

    $scope.khoListResponse = (data) => {
        $scope.listKho = data.data;
        $scope.listKho.unshift({ ID: "", TEN_DON_VI: "--Chọn kho hàng --" })
    }

    $scope.vatTuPhuListResponse = (data) => {
        $scope.listVatTuPhu = data.data;
        $scope.listVatTuPhu.unshift({ ID: "", TEN_SP: "--Chọn vật tư phụ --" })
    }

    $scope.getDanhSachHopDong = function () {
        showToast();
        const getDanhSachHopDongResponse = (data) => {
            console.log("danh sach hop dong", data.data);
            data.data.forEach(i => {
                i.HD_HOP_DONGS.forEach(j => {
                    j.NGAY_TAO = moment(j.NGAY_TAO).format('DD/MM/YYYY');
                })
            })
            $scope.hdHopDong = data.data;
            $scope.hdPaging.totalRecord = data.totalRecord;
            $scope.modelHopDongView = {};
            if ($scope.hdHopDong.length != 0) {
                $scope.viewDetailHD($scope.hdHopDong[0].HD_HOP_DONGS[0], false);
            }
        }
        $scope.callAPI('POST', 'LoadDanhSachHopDong', {
            model: $scope.hdPaging
        }, getDanhSachHopDongResponse, { hideLoading: true });
    }

    $scope.timkiemHD = (isShowed) => {
        showToast();
        const getDanhSachHopDongResponse = (data) => {
            console.log("danh sach hop dong", data.data);
            $scope.hdTimKiem = data.data;
            $scope.modelSearch.totalRecord = data.totalRecord;
            if (!isShowed) {
                $('#modalHopDongTimKiem').modal('show');
                $('#modalSearch').modal('hide');
            }
        }
        $scope.callAPI('POST', 'GetDanhSachHopDong', {
            model: {
                ...$scope.modelSearch, kieuHD: $scope.hdPaging.type
            }
        }, getDanhSachHopDongResponse, { hideLoading: true });
    }

    $scope.closeTimKiemHD = () => {
        $('#modalHopDongTimKiem').modal('hide');
    }

    $scope.viewDetailHDFromTimKiem = (item, isShowToast) => {
        $('#modalHopDongTimKiem').modal('hide');
        $scope.viewDetailHD(item, isShowToast);
    }

    $scope.viewDetailHD = function (item, isShowToast) {
        if (isShowToast) {
            showToast()
        }
        console.log("[Hop Dong] Detail", item);
        $scope.selectedRow = item.ID_HOP_DONG;
        $scope.selectedRowCT = item.ID_CONG_TY;
        $scope.modelHopDongView = {
            ...item,
            NAM_KP: item.NAM_KP?.toString(),
            NGAY_KY: convertDate4(item.NGAY_KY),
            ID_CONG_TY: item.ID_CONG_TY ? parseInt(item.ID_CONG_TY) : "",
            NGUOI_PHU_TRACH: item.NGUOI_PHU_TRACH ? parseInt(item.NGUOI_PHU_TRACH) : "",
            NGAY_DK_THANH_LY: convertDate4(item.NGAY_DK_THANH_LY),
            NGAY_CHOT_HD: convertDate4(item.NGAY_CHOT_HD),
            TRANG_THAI: item.TRANG_THAI
        };
        $scope.isDisableHD = (item.TRANG_THAI == 'Q');
        $scope.isDungCapPhat = (item.IS_DUNG_CAP_PHAT == 'Y');
        $scope.listSanPhamAddNTVatTuView = [];
        $timeout(function () {
            if ($scope.hdPaging.type == 'HD') {
                $scope.noiDungIsShowToast = false;
                angular.element('a[href="#noi_dung"]').trigger('click');
            } else {
                angular.element('a[href="#kho_hop_dong"]').trigger('click');
            }
        });
    }

    $scope.noidungHDPage = { ...tableNoPagingBase };
    $scope.noidungHopDongGet = () => {
        if ($scope.noiDungIsShowToast) {
            showToast();
        }
        $scope.noiDungIsShowToast = true;
        const data = {
            ...$scope.noidungHDPage,
            idHopDong: $scope.modelHopDongView.ID_HOP_DONG
        };

        const noiDungHopDongResponse = (data) => {
            $scope.noiDungHDSanPhamList = [];
            console.log('[Hop Dong] Noi dung', data);
            data.data.map((item) => {
                var modelSanPham = [{
                    ...item
                }];
                $scope.noiDungHDSanPhamList = [...modelSanPham, ...$scope.noiDungHDSanPhamList];
            })
            $scope.modelHopDongView.GIA_TRI_THUC_TE = $scope.noiDungHDSanPhamList.reduce(
                (count, item) => count += item.THANH_TIEN, 0
            );
            $scope.getVaySecList();
        }

        $scope.callAPI('GET', 'GetNoiDungHopDong', data, noiDungHopDongResponse, { hideLoading: true });
    }
    // #endregion

    // #region Hop Dong
    $scope.validationOptions = generateRequireFiled(
        [genReqField('LOAI_HD', 'Loại hợp đồng'), genReqField('NAM_KP', 'Năm kinh phí'), genReqField('TRANG_THAI', 'Trạng thái'),
        genReqField('SO_HD', 'Số hợp đồng'), genReqField('TEN_HD', 'Tên hợp đồng'), genReqField('ID_CONG_TY', 'Công ty'),
        genReqField('NGUOI_PHU_TRACH', 'Người phụ trách'), genReqField('NGAY_KY', 'Ngày ký')]
    );

    $scope.openAddHDModel = () => {
        $scope.modelHopDongAE = {};
        $scope.listSPAdd = [];
        $scope.listSPAddOpts = { ...baseTableAction };
    }

    $scope.register = function () {

        var flag = $scope.listSanPhamAddValidation([, 'SO_LUONG', 'DON_GIA']);
        fieldValidation($scope.modelHopDongAE, $scope.validationOptions.rules, flag);

        if ($scope.hopdongform.validate() && flag) {
            var model = {
                ...$scope.modelHopDongAE,
                NGAY_KY: convertDate2($scope.modelHopDongAE.NGAY_KY),
                NGAY_DK_THANH_LY: convertDate2($scope.modelHopDongAE.NGAY_DK_THANH_LY),
                NGAY_CHOT_HD: convertDate2($scope.modelHopDongAE.NGAY_CHOT_HD),
                KIEU_HD: $scope.hdPaging.type,
                HD_DETAILS: []
            };
            $scope.listSPAdd.map((item) => {
                var md = {
                    ...item,
                    ID_HANG: item.ID,
                    MA_HANG: item.MA_H55,
                    TEN_HANG: item.TEN_SP,
                    DVT: item.TEN_DVT,
                    SO_LUONG: item.SO_LUONG,
                    DON_GIA: item.DON_GIA,
                    THANH_TIEN: item.SO_LUONG * item.DON_GIA
                }
                model.HD_DETAILS.push(md);
            })
            if (flag && model) {
                if (model.HD_DETAILS.length == 0 && $scope.hdPaging.type == 'HD') {
                    toastr.error("Bạn chưa chọn sản phẩm nào!");
                    return;
                }
                const addHopDongResponse = (data) => {
                    console.log('[Hop dong] Add success');
                    $('#modalAddHopDong').modal('hide');
                    $scope.listSanPhamAdd = [];
                    $scope.getDanhSachHopDong();

                }
                $scope.callAPI('POST', 'AddHopDong', { model: model }, addHopDongResponse, { hideLoading: true });
            }
        }
    }

    $scope.removeHopDong = function () {

        const removeHopDongConfirm = () => {
            const removeHopDongResponse = () => {
                console.log('[Hop dong] Remove success');
                $scope.getDanhSachHopDong();
            }
            $scope.callAPI('POST', 'XoaHopDong', { idHopDong: $scope.modelHopDongView.ID_HOP_DONG }, removeHopDongResponse);
        }
        $scope.confirmDialog('Bạn có muốn xóa hợp đồng này không?', removeHopDongConfirm);
    }

    $scope.saveView = function () {
        var flag = true;
        Object.keys($scope.modelHopDongView).forEach(i => {
            if ($scope.validationOptions.rules[i] && $scope.modelHopDongView[i] == '') {
                $scope.modelHopDongView[`${i}_REQUIRED`] = true;
                flag = false;
            }
        })
        if ($scope.hopdongViewform.validate()) {
            var model = {
                ...$scope.modelHopDongView,
                NGAY_KY: convertDate2($scope.modelHopDongView.NGAY_KY),
                NGAY_DK_THANH_LY: convertDate2($scope.modelHopDongView.NGAY_DK_THANH_LY),
                NGAY_CHOT_HD: convertDate2($scope.modelHopDongView.NGAY_CHOT_HD),
                HD_DETAILS: []
            };

            if (flag && model) {
                const updateHopDongResponse = (data) => {
                    console.log('[Hop dong] Update success');
                    $scope.getDanhSachHopDong();
                }
                $scope.callAPI('POST', 'UpdateHopDong', { model: model }, updateHopDongResponse);
            }
        }
    }
    $scope.chotSLTTHDModal = function () {
        $scope.chotSLTTHDCheckAllCk = false;
        $scope.listPhieuChuaChot = [];
        $scope.callAPI('POST', 'GetListPhieuChuaChot', { idHopDong: $scope.modelHopDongView.ID_HOP_DONG }, $scope.getListPhieuChuaChotResponse);
    }

    $scope.getListPhieuChuaChotResponse = (data) => {
        $scope.listPhieuChuaChot = data.data;
    }

    $scope.chotSLTTHDSelectAll = function () {
        $scope.listPhieuChuaChot.forEach(e => {
            e.CHECKED = $scope.chotSLTTHDCheckAllCk;
        });
    }
    $scope.chotSLTTHD = function () {
        var listChecked = $scope.listPhieuChuaChot.filter(x => x.CHECKED);
        if (listChecked.length == 0) {
            toastr.error("Bạn chưa chọn phiếu để chốt!");
            return;
        }
        var listId = [];
        listChecked.map((item) => {
            listId.push(item.CODE);
        })
        console.log(listId);
        const chotSLTTHDConfirmed = () => {
            const removeNghiemThuVatTuResponse = () => {
                toastr.success("Chốt phiếu thành công!");
                $('#modalChotSLTT').modal('hide');
            };
            $scope.callAPI('POST', 'SaveChotSLTTHD', { idPhieus: listId.join(',') }, removeNghiemThuVatTuResponse);
        }
        $scope.confirmDialog('Bạn xác nhận chốt các phiếu này?', chotSLTTHDConfirmed);
    }
    // #endregion

    // #region San Pham
    $scope.getListSanPham = () => {
        const listSPResponse = (data) => {
            console.log('[List All SP]', data.data)
            $scope.listSP = data.data;
            $scope.allSP = data.data;
        }
        $scope.callAPI('POST', 'GetAllDanhSachSP', {}, listSPResponse, { hideLoading: true })
    }

    $scope.getSPHopDong = () => {
        const data = {
            ...$scope.noidungHDPage,
            idHopDong: $scope.modelHopDongView.ID_HOP_DONG
        };
        const listSPResponse = (data) => {
            data.data.forEach(i => {
                i.MA_H55 = i.MA_HANG;
                i.ID = i.ID_HANG;
                i.TEN_SP = i.TEN_HANG;
                i.TEN_DVT = i.DVT;
            });
            console.log('[List SP Of HD]', data.data)
            $scope.listSP = data.data;
        }
        $scope.callAPI('GET', 'GetNoiDungHopDong', data, listSPResponse, { hideLoading: true });
    }

    $scope.openDanhSachSP = (type, action) => {
        showToast();
        $scope.listSP = [];
        $scope.listSPOpts = { ...baseTableAction, action: action };
        $('#modalHDSanPham').modal('show');
        if ($scope.hdPaging.type == 'UNG') {
            $scope.getListSanPham();
            return;
        }

        switch (type) {
            case 'HD':
                $scope.getSPHopDong();
                break;
            default:
                $scope.getListSanPham();
                break;
        }
    }

    $scope.saveCheckedListSP = () => {
        const checkedList = $scope.listSP.filter(i => i.CHECKED);
        const addList = checkedList.filter(i => {
            const existed = $scope.listSPAdd.find(c => c.ID == i.ID);
            if (existed) return false;
            return true;
        });
        addList.forEach(i => { i.CHECKED = false; i.SO_LUONG = 0 });
        $scope.listSPAdd = [...addList, ...$scope.listSPAdd];
        switch ($scope.listSPOpts.action) {
            case undefined:
                break;
            case null:
                break;
            case 'UPDATE_SL_TON':
                capNhatSoLuongTonlistSPByPhieu('listSPAdd')
                break;
            default:
                break;
        }
    }

    $scope.removeSP = () => {
        if ($scope.listSPAdd.length == 0) {
            return;
        }
        var sp = $scope.listSPAdd.filter(x => x.CHECKED);
        if (sp.length > 0) {
            const xoaSPConfirmed = () => {
                $scope.listSPAdd = $scope.listSPAdd.filter(x => !x.CHECKED);
                $scope.$apply();
            }
            $scope.confirmDialog('Bạn có muốn xóa các sản phầm đã chọn?', xoaSPConfirmed);
        }
    }

    $scope.checkAllSPClicked = () => {
        $scope.listSPAdd.forEach(i => i.CHECKED = $scope.listSPAddOpts.checkedAll);
    }
    // #endregion 


    $scope.addSanPham = function () {
        var modelSanPham = [{
            checked: false,
            maH55: null,
            idHang: null,
            tenHang: { ...editFieldBase },
            dvt: null,
            soLuongHD: { ...editFieldBase },
            donGiaKH: { ...editFieldBase },
            thanhTien: null,
            soLuongTon: null,
            soLuongConLai: { ...editFieldBase },
            soLuong: { ...editFieldBase },
            soLuongTT: { ...editFieldBase },
        }];
        $scope.listSanPhamAdd = [...modelSanPham, ...$scope.listSanPhamAdd];
    }

    $scope.changeSanPham = function (item) {
        if ($scope.listSanPham.length > 0) {
            var sp = $scope.listSanPham.find(x => x.ID === item.tenHang.value);
            if (sp) {
                item.tenHang.text = sp.TEN_SP;
                item.maH55 = sp.MA_H55;
                item.dvt = sp.TEN_DVT;
                item.idHang = sp.ID;
            }
        }
    }

    $scope.changeSanPhamCuaKho = function (item, cb) {
        if ($scope.listSanPhamCuaKho.length > 0) {
            item.soLuongTon = 0;
            var sp = $scope.listSanPhamCuaKho.find(x => x.ID === item.tenHang.value);
            if (sp) {
                item.tenHang.text = sp.TEN_SP;
                item.maH55 = sp.MA_H55;
                item.dvt = sp.DON_VI_TINH;
                item.idHang = sp.ID;
            }
            const data = {
                idKho: $scope.modelVTAE.noiGiao,
                idSanPham: item.tenHang.value
            };
            const spTonResponse = (data) => {
                if (cb) cb();
                item.soLuongTon = data.data;
                $scope.$apply();
            };
            $scope.callAPI('POST', 'SanPhamSoLuongTonTrongKho', data, spTonResponse)
        }
    }

    $scope.onChangeKho = function (id) {
        $scope.listSanPhamAdd = [];
        $scope.listSanPhamCuaKho = [];
        const listSPKhoResponse = (data) => {
            $scope.listSanPhamCuaKho = data.data;
        }
        $scope.callAPI('POST', 'ListSanPhamByKho', { idKho: id }, listSPKhoResponse);
    }

    $scope.onChangeDMHopDong = function (id, type) {
        $scope.listSanPhamAdd = [];
        $.ajax({
            type: 'Post',
            url: '/QuanLyHopDong/ListSanPhamByHopDong',
            data: {
                idHopDong: id,
                type: type,
                nam: $scope.modelHopDongView.NAM_KP
            },
            success: function (data) {
                if (data.status) {
                    $scope.listSanPhamCuaHopDong = data.data;
                } else {
                    $scope.listSanPhamCuaHopDong = [];
                }
                $scope.$apply();

            }
        });
    }

    $scope.changeThanhPhamCuaHD = function (item) {
        if ($scope.listSanPhamCuaHopDong.length > 0) {
            var sp = $scope.listSanPhamCuaHopDong.find(x => x.ID_SP === item.tenHang.value);
            if (sp !== undefined && sp !== null) {
                item.tenHang.text = sp.TEN_SP;
                item.maH55 = sp.MA_H55;
                item.dvt = sp.DVT;
                item.idHang = sp.ID_SP;
                item.soLuongTon = sp.SO_LUONG_XUAT;
            }
        }

    }
    $scope.changeCheckAllSP = function () {
        if ($scope.listSanPhamAdd.length > 0) {
            $scope.listSanPhamAdd.map((item) => {
                item.checked = $scope.isCheckAllSP;
            });
        }
    };
    $scope.removeSanPham = function () {
        if ($scope.listSanPhamAdd.length == 0) {
            return;
        }
        var sp = $scope.listSanPhamAdd.filter(x => x.checked);
        if (sp !== undefined && sp.length > 0) {
            const xoaSPConfirmed = () => {
                $scope.listSanPhamAdd = $scope.listSanPhamAdd.filter(x => !x.checked);
                $scope.$apply();
            }
            $scope.confirmDialog('Bạn có muốn xóa các sản phầm đã chọn?', xoaSPConfirmed);
        }
    }

    $scope.searchSanPham = function () {
        var sp = $scope.txtSearchSanPham;
        if (sp !== null) {
            $scope.listSanPhamModal = $scope.listSanPham.filter(x => (x.MA_H55 + '').toLowerCase().includes(sp.toLowerCase()) || (x.TEN_SP + '').toLowerCase().includes(sp.toLowerCase()))
        } else {
            $scope.listSanPhamModal = $scope.listSanPham;
        }
    }

    $scope.changeCheckAllSPModal = function () {
        if ($scope.listSanPhamModal.length > 0) {
            $scope.listSanPhamModal.map((item) => {
                item.checked = $scope.isCheckAllSPModal;
            });
        }
    };
    $scope.resetDanhSachSP = function (type) {
        $scope.dsType = type;
        $scope.listSanPhamModal.map((item) => {
            item.checked = false;
        })
        if ($scope.dsType === 2) {
            if ($scope.listSanPhamAddView.length > 0) {
                console.log($scope.listSanPhamModal);
                $scope.listSanPhamAddView.map((item) => {
                    $scope.listSanPhamModal.find(x => x.ID === item.idHang).checked = true;
                });
            }
        } else {
            if ($scope.listSanPhamAdd.length > 0) {
                $scope.listSanPhamAdd.map((item) => {
                    $scope.listSanPhamModal.find(x => x.ID === item.idHang).checked = true;
                });
            }
        }

    }
    $scope.getTotal = function () {
        var total = 0;
        $scope.listSanPhamAdd.map((item) => {
            total += (item.soLuongHD.value * item.donGiaKH.value);
        });
        $scope.modelHopDongAE.GIA_TRI_HD = total;
        return total;
    }

    $scope.getTotalPLView = function () {
        var total = 0;
        if ($scope.listSanPhamAdd.length > 0) {
            $scope.listSanPhamAdd.map((item) => {
                total += (item.soLuongHD.value * item.donGiaKH.value);
            });

            $scope.modelPhuLucAE.giaTriPL = total;

        }
        return total;
    }

    function capNhatSoLuongTonNoiDung() {
        const listSPTonResponse = (data) => {
            console.log('[So luong ton SP]', data.data);
            data.data.forEach(item => {
                $scope.noiDungHDSanPhamList.forEach(nd => {
                    if (nd.ID_HANG == item.ID_SP) {
                        nd.SO_LUONG_CON_LAI = item.SO_LUONG_TON
                    }
                })
            })
        }
        $scope.khoTonKhoCallAPI(listSPTonResponse, 'SP')
    }

    function capNhatSoLuongTonlistSPByPhieu(nameOfPhieu) {
        const listSPTonResponse = (data) => {
            data.data.forEach(item => {
                $scope[nameOfPhieu].forEach(nd => {
                    if (nd.ID == item.ID_SP) {
                        nd.SO_LUONG_CON_LAI = item.SO_LUONG_TON
                    }
                })
            })
        }
        $scope.khoTonKhoCallAPI(listSPTonResponse, 'SP')
    }

    // #region NHAP KHO BO - NHAP KHO
    $scope.khoNhapKhoPaging = { ...tablePagingBase }
    $scope.khoNhapKhoListSP = [];
    $scope.khoNhapKhoModel = {}
    $scope.khoNhapKhoValidation = generateRequireFiled([
        genReqField('NGAY_NHAP', 'Ngày nhâp'),
        genReqField('NGUOI_GIAO', 'Người giao'),
        genReqField('ID_KHO', 'Kho'),
        genReqField('PHUONG_THUC_VAN_CHUYEN', 'Phương thức vận chuyển')
    ]);

    $scope.khoNhapKhoOpenModal = function (item) {
        $scope.listSPAdd = [];
        if (item) {
            console.log('[Kho Nhap Kho] Detail', item);
            $scope.khoNhapKhoModel = {
                ...item,
                NOI_GIAO: `HD số ${$scope.modelHopDongView.SO_HD}/${$scope.modelHopDongView.TEN_HD}`
            }
            $scope.khoNhapKhoGetListSP(item);
            return;
        }
        $scope.khoNhapKhoModel = {
            SO_PHIEU: '',
            NGAY_NHAP: today,
            NOI_GIAO: `HD số ${$scope.modelHopDongView.SO_HD}/${$scope.modelHopDongView.TEN_HD}`,
            TRANG_THAI: 'I'
        };
        capNhatSoLuongTonNoiDung();
    }

    $scope.khoNhapKhoGetList = () => {
        showToast();
        $scope.khoNhapKhoListData = [];
        const data = {
            ...$scope.khoNhapKhoPaging,
            idHopDong: $scope.modelHopDongView.ID_HOP_DONG
        };

        const khoNhapKhoGetListReponse = (data) => {
            console.log('[Kho Nhap Kho] list data', data);
            $scope.khoNhapKhoPaging.totalItems = data.totalRecord;
            data.data.forEach(item => {
                $scope.khoNhapKhoListData.push({
                    ...item,
                    IS_SO_LUONG_TT: item.IS_SO_LUONG_TT == 'Y' ? true : false,
                    NGAY_TAO: moment(item.NGAY_TAO).format('DD/MM/YYYY'),
                    NOI_GIAO: `HD số ${$scope.modelHopDongView.SO_HD}/${$scope.modelHopDongView.TEN_HD}`,
                    NOI_NHAN: $scope.listKho.find(i => i.ID == item.ID_KHO)["TEN_DON_VI"]
                })
            });

        }
        $scope.callAPI('POST', 'KhoNhapKhoGetList', data, khoNhapKhoGetListReponse, { hideLoading: true });
    }

    $scope.khoNhapKhoGetListSP = (item) => {
        const data = { ...tableNoPagingBase, id: item.ID }

        const khoNhapKhoListSPResponse = (data) => {
            console.log("[Kho Nhap kho] listSP", data);
            data.data.forEach(item => {
                var modelSanPham = [{
                    ...item,
                    CHECKED: false,
                    ID: item.ID_SP
                }];
                $scope.listSPAdd = [...modelSanPham, ...$scope.listSPAdd];
            })
            capNhatSoLuongTonlistSPByPhieu('listSPAdd')
        }
        $scope.callAPI('POST', 'GetDanhSachKhoNhapKhoChiTiet', data, khoNhapKhoListSPResponse);
    }

    $scope.khoNhapKhoCreateOrUpdate = function () {

        var flag = $scope.listSanPhamAddValidation(['TEN_SP', 'SO_LUONG_KE_HOACH']);
        fieldValidation($scope.khoNhapKhoModel, $scope.khoNhapKhoValidation.rules, flag);

        if ($scope.khoNhapKhoForm.validate() && flag) {
            var model = {
                ...$scope.khoNhapKhoModel,
                ID_HOP_DONG: $scope.modelHopDongView.ID_HOP_DONG,
                SO_HD_CHUYEN: $scope.modelHopDongView.SO_HD,
                IS_SO_LUONG_TT: $scope.khoNhapKhoModel.IS_SO_LUONG_TT ? 'Y' : 'N',
                HD_KHO_NHAP_KHO_DS: []
            };

            $scope.listSPAdd.map((item, index) => {
                var md = {
                    ...item,
                    ID_SP: item.ID,
                    ID_KHO_NHAP_KHO: $scope.khoNhapKhoModel.ID
                }
                model.HD_KHO_NHAP_KHO_DS.push(md);
            })

            if (flag && model) {
                if (model.HD_KHO_NHAP_KHO_DS.length == 0) {
                    toastr.error("Bạn chưa chọn sản phẩm nào!");
                    return;
                }
                const data = {
                    model: model,
                    idHopDong: $scope.modelHopDongView.ID_HOP_DONG
                }
                const createOrUpdateKhoNhapKhoResponse = (data) => {
                    $scope.khoNhapKhoGetList();
                    $('#modalAddNhapKhoBoNhapKho').modal('hide');
                }
                $scope.callAPI('POST', 'KhoNhapKhoAdd', data, createOrUpdateKhoNhapKhoResponse);
            }
        }
    }

    $scope.khoNhapKhoRemove = () => {
        var listChecked = $scope.khoNhapKhoListData.filter(x => x.CHECKED);
        if (listChecked.length == 0) {
            toastr.error("Bạn chưa chọn phiếu vay séc nào!");
            return;
        }
        var listId = [];
        listChecked.map((item) => {
            listId.push(item.ID);
        })

        const removeKhoNhapKhoConfirmed = () => {
            const removeKhoNhapKhoResponse = () => {
                $scope.khoNhapKhoGetList();
            }
            $scope.callAPI('POST', 'XoaKhoNhapKho', { ids: listId.join(',') }, removeKhoNhapKhoResponse);
        }
        $scope.confirmDialog('Bạn có muốn xóa các phiếu nhập kho này không?', removeKhoNhapKhoConfirmed)
    }

    // #endregion

    $scope.inPhieuNhapKho = (id) => {

        showToast()
        $.ajax({
            url: '/QuanLyHopDong/InPhieuNhapXuatKho',
            type: 'post',
            data: {
                id: id
            },
            success: function (result) {
                hideLoading();
                let pdfWindow = window.open("");
                pdfWindow.document.write("<iframe width='100%' height='100%' src='data:application/pdf;base64," + result + "'></iframe>");
            },
            error: function (xhr, status, err) {
                hideLoading();
            }
        });
    }

    // #region KHO TRA HD
    $scope.khoTraHDPaging = { ...tablePagingBase }
    $scope.khoTraHDModel = {}
    $scope.khoTraHDValidation = generateRequireFiled([
        genReqField('NGAY_NHAP', 'Ngày nhâp'),
        genReqField('NGUOI_GIAO', 'Người giao'),
        genReqField('ID_KHO', 'Kho'),
        genReqField('PHUONG_THUC_VAN_CHUYEN', 'Phương thức vận chuyển')
    ]);

    $scope.khoTraHDGetList = () => {
        showToast();
        $scope.khoTraHDListData = [];
        const data = {
            ...$scope.khoTraHDPaging,
            idHopDong: $scope.modelHopDongView.ID_HOP_DONG
        };

        const khoTraHDGetListReponse = (data) => {
            console.log('[Kho TraHD] list data', data);
            $scope.khoTraHDPaging.totalItems = data.totalRecord;
            data.data.forEach(item => {
                $scope.khoTraHDListData.push({
                    ...item,
                    IS_SO_LUONG_TT: item.IS_SO_LUONG_TT == 'Y' ? true : false,
                    NGAY_TAO: moment(item.NGAY_TAO).format('DD/MM/YYYY'),
                    NOI_NHAN: `HD số ${$scope.modelHopDongView.SO_HD}/${$scope.modelHopDongView.TEN_HD}`,
                    NOI_GIAO: $scope.listKho.find(i => i.ID == item.ID_KHO)["TEN_DON_VI"]
                })
            });
        }
        $scope.callAPI('POST', 'KhoTraHDGetList', data, khoTraHDGetListReponse, { hideLoading: true });
    }

    $scope.khoTraHDGetListSP = (item) => {
        const data = {
            ...tableNoPagingBase,
            id: item.ID,
        }

        const khoTraHDListSPResponse = (data) => {
            console.log("[Kho TraHD] listSP", data);
            data.data.forEach(item => {
                var modelSanPham = [{
                    ...item,
                    CHECKED: false,
                    ID: item.ID_SP,
                }];
                $scope.listSPAdd = [...modelSanPham, ...$scope.listSPAdd];
            })
            capNhatSoLuongTonlistSPByPhieu('listSPAdd')

        }
        $scope.callAPI('POST', 'GetDanhSachKhoNhapKhoChiTiet', data, khoTraHDListSPResponse);
    }

    $scope.khoTraHDOpenModal = function (item) {
        $scope.listSPAdd = [];
        if (item) {
            console.log('[Kho Tra HD] Detail', item);
            $scope.khoTraHDModel = {
                ...item,
                NOI_NHAN: `HD số ${$scope.modelHopDongView.SO_HD}/${$scope.modelHopDongView.TEN_HD}`
            }
            $scope.khoTraHDGetListSP(item);
            return;
        }
        $scope.khoTraHDModel = {
            NGAY_NHAP: today,
            NGUOI_GIAO: '',
            NOI_NHAN: `HD số ${$scope.modelHopDongView.SO_HD}/${$scope.modelHopDongView.TEN_HD}`,
            TRANG_THAI: "I",
        };
        capNhatSoLuongTonNoiDung();
    }

    $scope.khoTraHDCreateOrUpdate = function () {
        var flag = $scope.listSanPhamAddValidation(['SO_LUONG_KE_HOACH']);
        fieldValidation($scope.khoTraHDModel, $scope.khoTraHDValidation.rules, flag);

        if ($scope.khoTraHDForm.validate() && flag) {
            var model = {
                ...$scope.khoTraHDModel,
                ID_HOP_DONG: $scope.modelHopDongView.ID_HOP_DONG,
                SO_HD_CHUYEN: $scope.modelHopDongView.SO_HD,
                IS_SO_LUONG_TT: $scope.khoTraHDModel.IS_SO_LUONG_TT ? 'Y' : 'N',
                HD_KHO_NHAP_KHO_DS: []
            };

            $scope.listSPAdd.map((item, index) => {
                var md = {
                    ...item,
                    ID_SP: item.ID,
                    SO_LUONG_KE_HOACH: item.SO_LUONG_KE_HOACH,
                    SO_LUONG_THUC_TE: item.SO_LUONG_THUC_TE,
                    ID_KHO_NHAP_KHO: $scope.khoTraHDModel.ID
                }
                model.HD_KHO_NHAP_KHO_DS.push(md);
            })

            if (flag && model) {
                if (model.HD_KHO_NHAP_KHO_DS.length == 0) {
                    toastr.error("Bạn chưa chọn sản phẩm nào!");
                    return;
                }
                const data = {
                    model: model,
                    idHopDong: $scope.modelHopDongView.ID_HOP_DONG
                }
                const createOrUpdateKhoTraHDResponse = (data) => {
                    $scope.khoTraHDGetList();
                    $('#modalAddNhapKhoBoTraHD').modal('hide');
                }
                $scope.callAPI('POST', 'KhoTraHDAdd', data, createOrUpdateKhoTraHDResponse);
            }
        }
    }

    $scope.khoTraHDRemove = () => {
        var listChecked = $scope.khoTraHDListData.filter(x => x.CHECKED);
        if (listChecked.length == 0) {
            toastr.error("Bạn chưa chọn phiếu trả hợp đồng nào!");
            return;
        }
        var listId = [];
        listChecked.map((item) => {
            listId.push(item.ID);
        })

        const removeKhoTraHDConfirmed = () => {
            const removeKhoTraHDResponse = () => {
                $scope.khoTraHDGetList();
            }
            $scope.callAPI('POST', 'XoaKhoNhapKho', { ids: listId.join(',') }, removeKhoTraHDResponse);
        }
        $scope.confirmDialog('Bạn có muốn xóa các phiếu trả hợp đồng này không?', removeKhoTraHDConfirmed)
    }

    // #endregion

    // #region KHO NGHIEM THU

    $scope.khoNTIsCheckAll = false;

    $scope.khoNTonCheck = function () {
        if ($scope.khoNTListData.length > 0) {
            $scope.khoNTListData.map((item) => {
                item.CHECKED = $scope.khoNTIsCheckAll;
            });
        }
    };

    $scope.khoNTModel = {
    }

    $scope.khoNTModelBase = {
        ID: "",
        SO_PHIEU_NT: null,
        NGAY_NT: "",
        NGUOI_NT: "",
        MO_TA: "",
        PHIEU_KH_ID: ""
    };

    $scope.khoNTPaging = { ...tablePagingBase }

    $scope.khoNTValidation = generateRequireFiled([
        genReqField('SO_PHIEU_NT', 'Số phiếu nghiệm thu')]);

    $scope.khoNTGetList = () => {
        showToast();
        $scope.khoNTListData = [];
        const data = {
            ...$scope.khoNTPaging,
            idHopDong: $scope.modelHopDongView.ID_HOP_DONG
        };

        const khoNghiemThuListReponse = (data) => {
            console.log('[Kho NT] list', data);
            $scope.khoNTPaging.totalItems = data.totalRecord;
            $scope.khoNTListData = data.data;
            if ($scope.khoNTListData.length > 0) {
                $scope.khoNTListSPGet($scope.khoNTListData[0], 0);
            }
        }
        $scope.callAPI('POST', 'GetDanhSachKhoNghiemThu', data, khoNghiemThuListReponse, { hideLoading: true });
    }

    $scope.khoNTListSPGet = function (item, index, flag) {
        console.log('[Kho NT Update]', item);
        if (flag !== undefined)
            showToast();
        $scope.khoNTSelectedRow = index;
        $scope.khoNTModel = item
        const data = {
            ...tableNoPagingBase,
            idVatTu: item.ID
        }
        const danhSachNghiemThuChiTietResponse = (data) => {
            console.log("[Kho Nhiem Thu] chi tiet", data);
            $scope.khoNTListSP = data.data;
        }
        $scope.callAPI('POST', 'GetDanhSachKhoNghiemThuChiTiet', data, danhSachNghiemThuChiTietResponse, { hideLoading: true });
    }

    $scope.khoHopDongClicked = () => {
        $timeout(function () {
            angular.element('a[href="#giaybaonghiemthu"]').trigger('click');
        });
        const danhSachPhieuKeHoachSXResponse = (data) => {
            console.log("[Kho Nhiem Thu] Phieu ke hoach sx", data);
            $scope.listPhieuKeHoachSx = data.data;
            $scope.listPhieuKeHoachSx.unshift({ ID: "", MA_LENH: "--Chọn Kế hoạch --" })
        }
        $scope.callAPI('POST', 'GetDanhSachPhieuKeHoachSx', {}, danhSachPhieuKeHoachSXResponse, { hideLoading: false });
    }
    $scope.khoNTChanged = function () {
        $scope.khoNTGetList();
    }

    $scope.khoNTOpenAddModal = () => {
        $scope.khoNTModel = $scope.khoNTModelBase;
        $scope.listSPAdd = [];
    }

    $scope.khoNTAddNew = function () {

        var model = {
            ...$scope.khoNTModel,
            HD_KHO_NT_DS: []
        };

        $scope.listSPAdd.map((item, index) => {
            var md = {
                ...item,
                SP_ID: item.ID,
            }
            model.HD_KHO_NT_DS.push(md);
        })
        var flag = $scope.listSanPhamAddValidation(['SL']);
        var validMayDo = $scope.khoNTform.validate() && flag;
        var validCoSo = $scope.khoNTPhieuKHForm.validate() && model.PHIEU_KH_ID;
        if (!validMayDo && !validCoSo) {
            if (model.HD_KHO_NT_DS.length == 0) {
                toastr.error("Bạn chưa chọn sản phẩm nào!");
                return;
            }

            if (!model.PHIEU_KH_ID) {
                toastr.error("Bạn chưa chọn phiếu kế hoạch nào!");
                return;
            }
            return;
        }

        const data = {
            model: model,
            idHopDong: $scope.modelHopDongView.ID_HOP_DONG
        };

        const createOrUpdateKhoNTResponse = () => {
            $scope.khoNTGetList();
            $scope.khoNTModel = { ...$scope.khoNTModelBase };
            $scope.listSanPhamAdd = [];
            $('#modalAddGiayBaoNghiemThu').modal('hide');
        };

        if ($scope.khoNTIsEditMode) {
            $scope.callAPI('POST', 'UpdateKhoNghiemThu', data, createOrUpdateKhoNTResponse);
        } else {
            $scope.callAPI('POST', 'AddKhoNghiemThu', data, createOrUpdateKhoNTResponse);
        }

    }

    $scope.khoNTOpenEditModal = function () {
        $scope.khoNTIsEditMode = true;
        if ($scope.khoNTListSP) {
            $scope.listSPAdd = [];
            $scope.khoNTListSP.map((item) => {
                var modelSanPham = [{
                    ...item,
                    checked: false,
                    ID: item.SP_ID
                }];
                $scope.listSPAdd = [...modelSanPham, ...$scope.listSPAdd];
            })
        }
        $scope.onChangeKHSX($scope.khoNTModel.PHIEU_KH_ID);
        $('#modalAddGiayBaoNghiemThu').modal('show');
    }

    $scope.khoNTRemove = function () {
        var listChecked = $scope.khoNTListData.filter(x => x.CHECKED);
        if (listChecked.length == 0) {
            toastr.error("Bạn chưa chọn giấy nghiệm thu nào!");
            return;
        }

        var listId = [];
        listChecked.map((item) => {
            listId.push(item.ID);
        })

        const khoNTRemoveConfirmed = () => {
            const khoNTRemoveResponse = () => {
                $scope.khoNTGetList();
            }
            $scope.callAPI('POST', 'XoaKhoNT', { ids: listId.join(',') }, khoNTRemoveResponse);
        }

        $scope.confirmDialog('Bạn có muốn xóa các giấy báo nghiệm thu này không?', khoNTRemoveConfirmed)
    }

    $scope.onChangeKHSX = function (id) {
        $scope.listPhieuKHSXDetail = [];
        const changeKHSXResponse = (data) => {
            $scope.listPhieuKHSXDetail = data.data;
        }
        $scope.callAPI('POST', 'GetPhieuKeHoachSxChiTiet', { phieuXuatKhoId: id }, changeKHSXResponse);
    }
    // #endregion

    // #region VAY SEC
    $scope.vaySecListCheckAll = false;
    $scope.modelVaySec = {}
    $scope.listVaySec = {}

    $scope.validationVaySec = generateRequireFiled([
        genReqField('SO_TIEN_THANH_TOAN', 'Số tiền thanh toán'), genReqField('NGAY_THANH_TOAN', 'Ngày vay'),
        genReqField('NGUOI_NHAN', 'Người nhận')]);

    $scope.openVaySecModal = function () {
        $scope.listVaySec = [];
        $scope.getVaySecList();
    }

    $scope.addVaySec = function () {
        if ($scope.addVaySecForm.validate()) {
            var model = {
                ...$scope.modelVaySec,
                ID_HOP_DONG: $scope.modelHopDongView.ID_HOP_DONG,
            };
            const addVaySecResponse = (data) => {
                if (data.data.Error) {
                    toastr.error(data.data.Title);
                    return;
                }
                console.log('[Vay Sec] add successful')
                $scope.getVaySecList();
                $('#modalAddVaySec').modal('hide');
            }
            $scope.callAPI('POST', 'AddVaySec', { model: model }, addVaySecResponse);
        }
    }

    $scope.openAddVaySecModal = function (id) {
        $scope.modelVaySec = {};
        if (id) {
            const vaySecGetByIdResponse = (data) => {
                $scope.modelVaySec = {
                    ...data.data,
                    NGAY_THANH_TOAN: moment(data.data.NGAY_THANH_TOAN).format("DD/MM/yyyy"),
                }
            }
            $scope.callAPI('POST', `GetVaySecByID?vaysecId=${id}`, {}, vaySecGetByIdResponse);
        }
    }

    $scope.removeVaySec = function () {
        var listChecked = $scope.listVaySec.filter(x => x.CHECKED);
        if (listChecked.length == 0) {
            toastr.error("Bạn chưa chọn phiếu vay séc nào!");
            return;
        }
        var listId = [];
        listChecked.map((item) => {
            listId.push(item.ID);
        })

        const removeVaySecConfirmed = () => {
            const removeVaySecResponse = () => {
                $scope.getVaySecList();
            }
            $scope.callAPI('POST', 'DeleteVaySec', { idVaySec: listId }, removeVaySecResponse);
        }
        $scope.confirmDialog('Bạn có muốn xóa các phiếu vay séc này không?', removeVaySecConfirmed)
    }

    $scope.getVaySecList = () => {
        showToast();
        const vaySecListResponse = (data) => {
            console.log('[List vay sec]', data.data);
            $scope.listVaySec = [];
            $scope.vaySecListCheckAll = false;
            for (let item of data.data) {
                let obj = {
                    ...item,
                    NGAY_TAO: moment(item.NGAY_TAO).format("DD/MM/yyyy"),
                    CHECKED: false
                };
                $scope.listVaySec.push(obj);
            }
            $scope.modelHopDongView.GT_DA_THANH_TOAN = $scope.listVaySec.reduce((total, item) =>
                total += item.SO_TIEN_THANH_TOAN, 0);
            $scope.modelHopDongView.GT_CON_THANH_TOAN = $scope.modelHopDongView.GIA_TRI_THUC_TE - $scope.modelHopDongView.GT_DA_THANH_TOAN;
        }
        $scope.callAPI('POST', `GetListVaySec?hopdongId=${$scope.modelHopDongView.ID_HOP_DONG}`, {}, vaySecListResponse, { hideLoading: true })
    }

    $scope.vaySecCheckAllOnCheck = () => {
        for (let item of $scope.listVaySec) {
            if (!item.SO_PHIEU_H55) item.CHECKED = $scope.vaySecListCheckAll;
        }
    }

    $scope.vaySecListIsCheckAll = () => {
        $scope.vaySecListCheckAll = $scope.listVaySec.every(i => (i.SO_PHIEU_H55 || i.CHECKED))
        console.log('[Vay Sec] check all list', $scope.vaySecListCheckAll)
    }

    // #endregion

    // #region Phu Luc
    $scope.$watch('listSPAdd', function () {
        $scope.modelPhuLucAE.giaTriPL = $scope.modelHopDongAE.GIA_TRI_HD = $scope.listSPAdd.reduce((total, item) => {
            if (!item.SO_LUONG || !item.DON_GIA) return total;
            total += (+item.SO_LUONG) * (+item.DON_GIA);
            return total;
        }, 0);
    }, true);

    $scope.validationPhuLuc = generateRequireFiled([
        genReqField('soPhuLuc', 'Số phụ lục')]);

    $scope.addPhuLuc = function () {
        var flag = $scope.listSanPhamAddValidation(['SO_LUONG', 'DON_GIA']);
        if (!$scope.phulucform.validate()) {
            return;
        }
        var model = {
            ID_HOP_DONG: $scope.modelPhuLucAE.idPhuLuc,
            SO_HD: $scope.modelPhuLucAE.soPhuLuc,
            NGAY_KY: convertDate2($scope.modelPhuLucAE.ngayPhuLuc),
            GIA_TRI_HD: $scope.modelPhuLucAE.giaTriPL,
            NGAY_CHOT_HD: convertDate2($scope.modelPhuLucAE.ngayChotSLTT),
            HD_DETAILS: []
        };

        $scope.listSPAdd.map((item, index) => {
            var md = {
                STT: index + 1,
                ID_HANG: item.ID,
                MA_HANG: item.MA_H55,
                TEN_HANG: item.TEN_SP,
                DVT: item.TEN_DVT,
                SO_LUONG: item.SO_LUONG,
                DON_GIA: item.DON_GIA,
                THANH_TIEN: item.SO_LUONG * item.DON_GIA
            }
            model.HD_DETAILS.push(md);
        })

        if (flag && model.HD_DETAILS.length > 0) {

            const data = {
                model: model,
                idHopDong: $scope.modelHopDongView.ID_HOP_DONG
            }
            const phuLucAddResponse = () => {
                $scope.getPhucLuc();
                $scope.modelPhuLucAE = {};
                $scope.listSPAdd = [];
                $scope.listSPAddOpts = { ...baseTableAction };
                $('#modalAddPhuLuc').modal('hide');
            }
            $scope.callAPI('POST', 'AddPhuLuc', data, phuLucAddResponse);
        }
        else {
            toastr.error("Bạn chưa chọn sản phẩm nào!");
        }
    }

    $scope.addPhuLucModal = function () {
        $scope.modelPhuLucAE = {};
        $scope.listSPAdd = [];
        $scope.listSPAddOpts = { ...baseTableAction };
    }

    $scope.removePhuLuc = function () {
        var listChecked = $scope.listHDPhuLuc.filter(x => x.checked);
        if (listChecked.length == 0) {
            toastr.error("Bạn chưa chọn phụ lục nào!");
            return;
        }
        var listId = [];
        listChecked.map((item) => {
            listId.push(item.idPhuLuc);
        })
        const phuLucRemove = () => {
            $scope.callAPI('POST', 'XoaPhuLuc', {
                idPhuLuc: listId.join(',')
            }, $scope.getPhucLuc);
        }
        $scope.confirmDialog('Bạn có muốn xóa các phụ lục này không?', phuLucRemove);
    }

    $scope.viewDetailPL = function (item, index, flag, hdf) {
        console.log('[Phu luc] Selected Phu Luc', item)
        if (flag !== undefined)
            showToast();
        $scope.selectedRowPL = index;
        $scope.modelPhuLucView = {
            ...item
        };

        const data = {
            idHopDong: item.idPhuLuc,
            timKiem: null,
            pageIndex: $scope.modelPhulucChiTiet.pageIndex,
            pageSize: $scope.modelPhulucChiTiet.pageSize
        }

        const phuLucDetailResponse = (data) => {
            console.log('[Phu luc] SP Detail', data);
            $scope.listSanPhamPLView = [];
            $scope.modelPhulucChiTiet.totalItems = data.totalRecord;
            if (data.data) {
                data.data.map((item) => {
                    var modelSanPham = [{
                        ...item
                    }];
                    $scope.listSanPhamPLView = [...modelSanPham, ...$scope.listSanPhamPLView];
                })
            }
        }
        $scope.callAPI('GET', 'GetHopDongChiTiet', data, phuLucDetailResponse, { hideLoading: true });
    }
    $scope.changeCheckAllPLView = function () {
        if ($scope.listHDPhuLuc.length > 0) {
            $scope.listHDPhuLuc.map((item) => {
                item.checked = $scope.isCheckAllPLView;
            });
        }
    };
    $scope.changeCheckAllNTVattuView = function () {
        if ($scope.listHDNTVatTu.length > 0) {
            $scope.listHDNTVatTu.map((item) => {
                item.CHECKED = $scope.isCheckAllNtVatTuView;
            });
        }
    };

    $scope.getPhucLuc = () => {
        showToast();
        $scope.modelPhuLucView = {};
        $scope.selectedRowPL = -1;
        const data = {
            ...$scope.modelPhuluc,
            idHopDong: $scope.modelHopDongView.ID_HOP_DONG
        };

        const phuLucListResponse = (data) => {
            $scope.listHDPhuLuc = [];
            $scope.modelPhuluc.totalItems = data.totalRecord;
            data.data.map((item) => {
                var modelSL = [{
                    checked: false,
                    idPhuLuc: item.ID_HOP_DONG,
                    maH55: item.MA_HANG,
                    ngayPhuLuc: convertDate4(item.NGAY_KY),
                    ngayChotSLTT: convertDate4(item.NGAY_CHOT_HD),
                    soPhuLuc: item.SO_HD,
                    giaTriPL: item.GIA_TRI_HD
                }];
                $scope.listHDPhuLuc = [...$scope.listHDPhuLuc, ...modelSL];

            });
            //$scope.modelPhuLucView = {};
            //if ($scope.listHDPhuLuc.length > 0) {
            //    $scope.viewDetailPL($scope.listHDPhuLuc[0], 0, false, true);
            //} else {
            //    $scope.listSanPhamAddPLHDView = [];
            //}
        }
        $scope.callAPI('POST', 'GetDanhSachPhuLuc', data, phuLucListResponse, { hideLoading: true });
    }

    $scope.pagePhuLucChanged = function () {
        getPhucLuc($scope.modelHopDongView.ID_HOP_DONG);
    }
    // #endregion
    $scope.tabVatTuSelect = function () {
        $timeout(function () {
            angular.element('a[href="#vt1"]').trigger('click');
        });
    }
    // #region Nghiem thu vat tu phu

    $scope.validationNTVatTu = generateRequireFiled([
        genReqField('soNghiemThu', 'Số nghiệm thu'), genReqField('ngayNghiemThu', 'Ngày nghiệm thu'),
        genReqField('nguoiNghiemThu', 'Người nghiệm thu')]);

    $scope.pageNTVatTuChanged = function () {
        showToast();
        getNghiemThuVatTu($scope.modelHopDongView.ID_HOP_DONG);
    }
    function getNghiemThuVatTu(idHopDong) {
        var data = {
            idHopDong: idHopDong,
            timKiem: $scope.modelNTVatTu.timKiem,
            pageIndex: $scope.modelNTVatTu.pageIndex,
            pageSize: $scope.modelNTVatTu.pageSize
        };
        $scope.callAPI("POST", "GetDanhSachNghiemThuVatTu", data, $scope.listNTVatTuResponse, { hideLoading: true });
    }

    $scope.listNTVatTuResponse = (data) => {
        $scope.listHDNTVatTu = [];

        $scope.modelNTVatTu.totalItems = data.totalRecord;
        $scope.listHDNTVatTu = data.data;
        if ($scope.listHDNTVatTu.length > 0) {
            $scope.viewDetailNTVatTu($scope.listHDNTVatTu[0], 0);
        } else {
            $scope.viewDetailNTVatTu(null, 0);
        }
    }
    $scope.addNTVatTu = function () {
        var flag = $scope.listSanPhamVTAddValidation(['tenHang', 'soLuongHD']);
        if ($scope.ntvtform.validate() && flag) {
            var model = {
                ID: $scope.modelNTVTAE.id ?? 0,
                SO_PHIEU_NT: $scope.modelNTVTAE.soNghiemThu,
                NGAY_NT: $scope.modelNTVTAE.ngayNghiemThu,//convertDate2($scope.modelNTVTAE.ngayNghiemThu),
                NGUOI_NT: $scope.modelNTVTAE.nguoiNghiemThu,
                MO_TA: $scope.modelNTVTAE.moTa,
                HD_NT_VAT_TU_DS: []
            };
            if ($scope.listSanPhamAdd.length > 0) {
                $scope.listSanPhamAdd.map((item, index) => {
                    var md = {
                        SP_ID: item.tenHang.value,
                        TEN_SP: item.tenHang.text,
                        SL: item.soLuongHD.value,
                        THANH_TIEN: item.soLuongHD.value * 1
                    }
                    model.HD_NT_VAT_TU_DS.push(md);
                })
            }
            if (flag && model !== undefined) {
                // showToast();
                if (model.HD_NT_VAT_TU_DS.length > 0) {
                    var url = '';
                    if ($scope.idEditNTVatTu) {
                        url = 'UpdateNghiemThuVatTu';
                    } else {
                        url = 'AddNghiemThuVatTu';
                    }
                    var data = {
                        model: model,
                        idHopDong: $scope.modelHopDongView.ID_HOP_DONG
                    };
                    $scope.callAPI("POST", url, data, $scope.addNTVatTuResponse);
                } else {
                    toastr.error("Bạn chưa chọn sản phẩm nào!");
                }

            }
        }
    }
    $scope.addNTVatTuResponse = (data) => {
        if (data.status) {
            getNghiemThuVatTu($scope.modelHopDongView.ID_HOP_DONG);
            $scope.modelNTVTAE = {
                id: "",
                soNghiemThu: null,
                ngayNghiemThu: "",
                nguoiNghiemThu: "",
                moTa: ""
            };
            $scope.listSanPhamAdd = [];
            $('#modalAddNTVatTu').modal('hide');
        } else {
            toastr.error(data.message);
        }
        $scope.$apply();
    }
    $scope.addNTVatTuModal = function () {
        $scope.modelNTVTAE = {
            id: "",
            soNghiemThu: null,
            ngayNghiemThu: "",
            nguoiNghiemThu: "",
            moTa: ""
        };
        $scope.listSanPhamAdd = [];
        $scope.idEditNTVatTu = false;
    }
    $scope.updateNTVatTuModal = function (item) {
        $scope.idEditNTVatTu = true;
        $scope.modelNTVTAE = {
            id: item.ID, //$scope.modelNTVatTuView.ID,
            soNghiemThu: item.SO_PHIEU_NT, //$scope.modelNTVatTuView.SO_PHIEU_NT,
            ngayNghiemThu: item.NGAY_NT, //$scope.modelNTVatTuView.NGAY_NT,
            nguoiNghiemThu: item.NGUOI_NT, //$scope.modelNTVatTuView.NGUOI_NT,
            moTa: item.MO_TA // $scope.modelNTVatTuView.MO_TA
        };
        //$scope.callAPI('POST', 'GetDanhSachNghiemThuVatTuChiTiet', {
        //    idVatTu: item.ID
        //}, $scope.updateNTVatTuModalResponse, { hideLoading: true });

        $('#modalAddNTVatTu').modal('show');

    }
    $scope.updateNTVatTuModalResponse = (data) => {
        if (data.status) {
            $scope.modelNTVatTu.totalItems = data.totalRecord;
            $scope.listSanPhamAdd = [];
            data.data.map((item) => {
                var modelSanPham = [{
                    checked: false,
                    idHang: item.SP_ID,
                    tenHang: editableField(item.SP_ID, item.TEN_SP),
                    soLuongHD: editableField(item.SL),
                    donGiaKH: editableField(0),
                    thanhTien: 0,
                }];
                $scope.listSanPhamAdd = [...modelSanPham, ...$scope.listSanPhamAdd];
            })
        } else {
            toastr.error(data.message);
        }
        //  hideLoading();
        $scope.$apply();

    }

    $scope.removeNghiemThuVatTu = function () {
        var listChecked = $scope.listHDNTVatTu.filter(x => x.CHECKED);
        if (listChecked.length == 0) {
            toastr.error("Bạn chưa chọn nghiệm thu vật tư nào!");
            return;
        }
        var listId = [];
        listChecked.map((item) => {
            listId.push(item.ID);
        })
        const removeNghiemThuVatTuConfirmed = () => {
            const removeNghiemThuVatTuResponse = () => getNghiemThuVatTu($scope.modelHopDongView.ID_HOP_DONG);
            $scope.callAPI('POST', 'XoaNghiemThuVatTu', { idVatTu: listId.join(',') }, removeNghiemThuVatTuResponse);
        }
        $scope.confirmDialog('Bạn có muốn xóa các nghiệm thu vật tư này không?', removeNghiemThuVatTuConfirmed);
    }

    $scope.viewDetailNTVatTu = function (item, index, flag) {
        if (flag !== undefined)
            showToast();
        if (item = undefined || item == null) {
            $scope.listSanPhamAddNTVatTuView = [];
            $scope.selectedRowNTVatTu = 0;
            $scope.modelNTVatTuView = {};
            return;
        }
        $scope.selectedRowNTVatTu = index;
        $scope.modelNTVatTuView = item
        const viewDetailNTVatTuResponse = (data) => {
            $scope.listSanPhamAddNTVatTuView = data.data;
            $scope.updateNTVatTuModalResponse(data);
        }
        $scope.callAPI('POST', 'GetDanhSachNghiemThuVatTuChiTiet', {
            ...tableNoPagingBase,
            idVatTu: item.ID
        }, viewDetailNTVatTuResponse, { hideLoading: true });
    }
    $scope.addSPVatTuNghiemThuModal = function () {
        $scope.listVatTuPhu.forEach(e => {
            e.SELECT = false;
        });
    }
    $scope.AddSPVTNTSelectAll = function () {
        $scope.listVatTuPhu.forEach(e => {
            e.SELECT = AddSPVTNTBanDauCheckAll;
        });
    }
    $scope.addSPVatTuNghiemThu = function () {
        $scope.listVatTuPhu.forEach(e => {
            if (e.SELECT) {
                var modelSanPham = [{
                    checked: false,
                    maH55: null,
                    idHang: e.ID,
                    tenHang: {
                        value: e.ID,
                        text: e.TEN_SP,
                        edit: true
                    },
                    dvt: null,
                    soLuongHD: { ...editFieldBase },
                    donGiaKH: { ...editFieldBase },
                    thanhTien: null,
                    soLuongTon: null,
                    soLuongConLai: { ...editFieldBase },
                    soLuong: { ...editFieldBase },
                    soLuongTT: { ...editFieldBase },
                }];
                var arr = $.grep($scope.listSanPhamAdd, function (x) {
                    return x.idHang == e.ID;
                });
                if (arr == null || arr == undefined || arr.length == 0) {
                    $scope.listSanPhamAdd = [...modelSanPham, ...$scope.listSanPhamAdd];
                }

            }
        });
    }
    // #endregion

    // #region Giao vật tư qua kho
    $scope.pageVatTuChanged = function () {
        getVatTu($scope.modelHopDongView.ID_HOP_DONG, 1);
    }
    $scope.changeTabVatTu = function (type) {
        getVatTu($scope.modelHopDongView.ID_HOP_DONG, type);
        if (type == 3 || type == 4) {
            //get danh mục vật tư của hợp đồng hiện tại để giao đi
            $scope.onChangeDMHopDong($scope.modelHopDongView.ID_HOP_DONG, 2); //get list sản phẩm by hd
        }
    }
    $scope.changeCheckAllVatTuQuaKhoView = function () {
        if ($scope.listVatTuQuaKho.length > 0) {
            $scope.listVatTuQuaKho.map((item) => {
                item.CHECKED = $scope.isCheckAllVatTuQuaKhoView;
            });
        }
    };
    $scope.changeCheckAllVatTuKhongQuaKhoView = function () {
        if ($scope.listVatTuKhongQuaKho.length > 0) {
            $scope.listVatTuKhongQuaKho.map((item) => {
                item.CHECKED = $scope.isCheckAllVatTuKhongQuaKhoView;
            });
        }
    };
    $scope.removeVatTu = function () {
        var listChecked = $scope.listVatTuQuaKho.filter(x => x.CHECKED);
        if (listChecked.length == 0) {
            toastr.error("Bạn chưa chọn vật tư nào!");
            return;
        }
        var listId = [];
        listChecked.map((item) => {
            listId.push(item.ID);
        })
        const removeVatTuConfirm = () => {
            const removeVatTuResponse = () => {
                getVatTu($scope.modelHopDongView.ID_HOP_DONG, 1);
            }
            $scope.callAPI('Post', 'XoaVatTu', { idVatTu: listId.join(',') }, removeVatTuResponse);
        }
        $scope.confirmDialog('Bạn có muốn xóa các vật tư này không?', removeVatTuConfirm);
    }

    $scope.addVatTuModal = function () {
        $scope.modelVTAE = { type: 1, soPhieu: '', ngayThang: today, soPhieuH55: '', nguoiNhan: '', noiNhan: '', noiGiao: '', noiNhan: $scope.modelHopDongView.SO_HD, moTa: '', capNhatSLTT: false, lanhDao: '', phuongThucVanChuyen: '' };
        $scope.listSanPhamAdd = [];
        $scope.idEditVatTu = false;
        $scope.isChoDuyet = false;
    }
    $scope.updateVatTuModal = function (item) {
        $scope.idEditVatTu = true;
        $scope.listSanPhamAdd = [];
        $scope.modelVTAE = {
            ...$scope.modelVTAE,
            id: item.ID,
            soPhieu: item.SO_PHIEU,
            ngayThang: item.NGAY_GIAO,//moment(new Date(parseInt($scope.modelVatTuView.NGAY_TAO.substr(6)))).format('DD/MM/YYYY'),  
            soPhieuH55: item.SO_PHIEU_H55,
            nguoiNhan: item.NGUOI_NHAN,
            //noiGiao: $scope.listDonVi.find(x => x.ID === $scope.modelVatTuView.DON_VI_ID),
            noiGiao: item.KHO_ID,
            noiNhan: item.SO_HD_DEN,
            moTa: item.MO_TA,
            capNhatSLTT: item.IS_CAP_NHAT_SLTT == 'Y' ? true : false,
            lanhDao: item.LANH_DAO,
            phuongThucVanChuyen: item.PHUONG_THUC_VAN_CHUYEN
        };
        $scope.isChoDuyet = item.TRANG_THAI == 'U' || item.TRANG_THAI == 'A';
        $scope.onChangeKho(item.KHO_ID); //get list sản phẩm by kho
        $.ajax({
            type: 'POST',
            url: '/QuanLyHopDong/GetDanhSachGiaoVatTuChiTiet',
            data: {
                ...tableNoPagingBase,
                idVatTu: item.ID
            },
            success: function (data) {
                if (data.status) {
                    data.data.map((item) => {
                        var modelSanPham = [{
                            checked: false,
                            id: item.ID,
                            idHang: item.SP_ID,
                            tenHang: {
                                value: item.SP_ID,
                                text: item.TEN_SP,
                                edit: true
                            },
                            dvt: item.TEN_DVT,
                            soLuongTon: item.SO_LUONG_TON,
                            soLuong: {
                                value: item.SO_LUONG,
                                edit: true
                            },
                            soLuongTT: {
                                value: item.SO_LUONG_THUC_TE,
                                edit: true
                            }
                        }];
                        $scope.listSanPhamAdd = [...modelSanPham, ...$scope.listSanPhamAdd];
                    })
                } else {
                    toastr.error(data.message);
                }
                hideLoading();
                $scope.$apply();

            }
        });
        $('#modalAddVatTu').modal('show');

    }
    $scope.validationVatTu = generateRequireFiled([
        //genReqField('soPhieu', 'Số phiếu'),
        genReqField('ngayThang', 'Ngày tháng'),
        genReqField('nguoiNhan', 'Người nhận')]);

    $scope.addVatTu = function () {
        var flag = $scope.listSanPhamVTAddValidation(['tenHang', 'soLuong']);
        if ($scope.modelVTAE.noiGiao == '' || $scope.modelVTAE.noiGiao == null) {
            $scope.modelVTAE.requiredNoiGiao = true;
            flag = flag && false;
        } else {
            $scope.modelVTAE.requiredNoiGiao = false;
            flag = flag && true;
        }
        //if ($scope.modelVTAE.lanhDao == '' || $scope.modelVTAE.lanhDao == null) {
        //    $scope.modelVTAE.requiredLanhDao = true;
        //    flag = flag && false;
        //} else {
        //    $scope.modelVTAE.requiredLanhDao = false;
        //    flag = flag &&  true;
        //}
        if ($scope.vattuform.validate() && flag) {
            var model = {
                ID: $scope.modelVTAE.id ?? 0,
                SO_PHIEU: $scope.modelVTAE.soPhieu,
                NGAY_GIAO: $scope.modelVTAE.ngayThang,
                HOP_DONG_CHINH_ID: $scope.modelHopDongView.ID_HOP_DONG,
                SO_HOP_DONG_CHINH: $scope.modelHopDongView.SO_HD,
                SO_PHIEU_H55: $scope.modelVTAE.soPhieuH55,
                NGUOI_NHAN: $scope.modelVTAE.nguoiNhan,
                LANH_DAO: $scope.modelVTAE.lanhDao,
                PHUONG_THUC_VAN_CHUYEN: $scope.modelVTAE.phuongThucVanChuyen,
                KHO_ID: $scope.modelVTAE.noiGiao,
                TEN_KHO: $scope.listKho.find(x => x.ID === $scope.modelVTAE?.noiGiao).TEN_DON_VI,//$scope.listDonVi.find(x => x.ID === $scope.modelVTAE?.noiGiao).TEN_DON_VI,
                SO_HD: $scope.modelHopDongView.SO_HD,
                HOP_DONG_ID: $scope.modelHopDongView.ID_HOP_DONG,
                SO_HD_GUI: '',
                HOP_DONG_GUI_ID: '',
                MO_TA: $scope.modelVTAE.moTa,
                IS_CAP_NHAT_SLTT: $scope.modelVTAE.capNhatSLTT ? 'Y' : 'N',
                LOAI_XUAT_KHO: 1,
                HD_GIAO_VT_DATA: []
            };
            console.log("MODEL", model);
            if ($scope.listSanPhamAdd.length > 0) {
                $scope.listSanPhamAdd.map((item, index) => {
                    var md = {
                        ID: item.id ?? 0,
                        SP_ID: item.idHang,
                        TEN_SP: item.tenHang.text,
                        TEN_DVT: item.dvt,
                        SO_LUONG_TON: item.soLuongTon,
                        SO_LUONG: item.soLuong.value,
                        SO_LUONG_THUC_TE: item.soLuongTT.value
                    }
                    model.HD_GIAO_VT_DATA.push(md);
                })
            }
            if (flag && model !== undefined) {
                // showToast();
                if (model.HD_GIAO_VT_DATA.length > 0) {
                    var url = '';
                    if ($scope.idEditVatTu) {
                        url = 'UpdateVatTu';
                    } else {
                        console.log('AddVatTu-model', model);
                        url = 'AddVatTu';
                    }
                    var data = {
                        model: model
                    };
                    $scope.callAPI("POST", url, data, $scope.addVatTuResponse);
                } else {
                    toastr.error("Bạn chưa chọn sản phẩm nào!");
                }

            }
        }
    }

    $scope.addVatTuResponse = (data) => {
        if (data.status) {
            getVatTu($scope.modelHopDongView.ID_HOP_DONG, 1);
            $scope.modelVTAE = { type: 1, ngayThang: today, noiGiao: $scope.modelHopDongView.SO_HD };
            $scope.listSanPhamAdd = [];
            $('#modalAddVatTu').modal('hide');
        } else {
            toastr.error(data.message);
        }
        //  hideLoading();
        $scope.$apply();

    }

    $scope.viewDetailVatTu = function (item, type, index, flag) {
        //if (flag !== undefined)
        //    showToast();
        if (type === 1) {
            $scope.selectedRowVatTuQuaKho = index;
        } else if (type === 2) {
            $scope.selectedRowVatTuKhongQuaKho = index;
        } else {
            $scope.selectedRowVatTuChinh = index;
        }
    }

    $scope.addDsVatTuKhoModal = function () {
        $scope.listSanPhamCuaKho.map((item, index) => { item.CHECKED = false; });
    }

    $scope.saveCheckedVatTuKho = function () {
        $scope.listSanPhamCuaKho.map((item, index) => {
            if (item.CHECKED) {
                var sp = {
                    checked: false,
                    id: null,
                    idHang: item.ID,
                    tenHang: {
                        value: item.ID,
                        text: item.TEN_SP,
                        edit: true
                    },
                    dvt: item.DON_VI_TINH,
                    soLuongTon: null,
                    soLuong: {
                        value: null,
                        edit: true
                    },
                    soLuongTT: {
                        value: null,
                        edit: true
                    }
                };
                var arr = $.grep($scope.listSanPhamAdd, function (x) {
                    return x.idHang == sp.idHang;
                });
                if (arr == undefined || arr.length == 0) {
                    $scope.changeSanPhamCuaKho(sp, function () {
                        $scope.listSanPhamAdd.push(sp);
                    });
                }
            }
        })
    }
    function getVatTu(idHopDong, type) {
        var ob = {
            idHopDong: idHopDong,
            type: type,
            timKiem: null,
            pageIndex: type === 1 ? $scope.modelVatTuQuaKho.pageIndex : (type === 2 ? $scope.modelVatTuKhongQuaKho.pageIndex : (type === 3 ? $scope.modelDieuChuyenVatTuKhongQuaKho.pageIndex : $scope.modelVatTuNhapTra.pageIndex)),
            pageSize: type === 1 ? $scope.modelVatTuQuaKho.pageSize : (type === 2 ? $scope.modelVatTuKhongQuaKho.pageSize : (type === 3 ? $scope.modelDieuChuyenVatTuKhongQuaKho.pageSize : $scope.modelVatTuNhapTra.pageSize))
        };
        console.log('ABC', ob);
        $.ajax({
            type: 'Post',
            url: '/QuanLyHopDong/GetDanhSachGiaoVatTu',
            data: {
                idHopDong: idHopDong,
                type: type,
                timKiem: type === 1 ? $scope.modelVatTuQuaKho.timKiem : (type === 2 ? $scope.modelVatTuKhongQuaKho.timKiem : (type === 3 ? $scope.modelDieuChuyenVatTuKhongQuaKho.timKiem : $scope.modelVatTuNhapTra.timKiem)),
                pageIndex: type === 1 ? $scope.modelVatTuQuaKho.pageIndex : (type === 2 ? $scope.modelVatTuKhongQuaKho.pageIndex : (type === 3 ? $scope.modelDieuChuyenVatTuKhongQuaKho.pageIndex : $scope.modelVatTuNhapTra.pageIndex)),
                pageSize: type === 1 ? $scope.modelVatTuQuaKho.pageSize : (type === 2 ? $scope.modelVatTuKhongQuaKho.pageSize : (type === 3 ? $scope.modelDieuChuyenVatTuKhongQuaKho.pageSize : $scope.modelVatTuNhapTra.pageSize))
            },
            success: function (data) {

                if (data.status) {
                    if (type === 1) {
                        $scope.listVatTuQuaKho = [];
                        $scope.modelVatTuQuaKho.totalItems = data.totalRecord;
                        $scope.listVatTuQuaKho = data.data;
                        //if ($scope.modelVTAE.type === 1 && $scope.listVatTuQuaKho.length > 0) {
                        //    $scope.viewDetailVatTu($scope.listVatTuQuaKho[0], 0);
                        //}
                    } else if (type === 2) {
                        $scope.listVatTuKhongQuaKho = [];
                        $scope.modelVatTuKhongQuaKho.totalItems = data.totalRecord;
                        $scope.listVatTuKhongQuaKho = data.data;
                        //if ($scope.modelVTAE.type === 2 && $scope.listVatTuKhongQuaKho.length > 0) {
                        //    $scope.viewDetailVatTu($scope.listVatTuKhongQuaKho[0], 0);
                        //}
                    } else if (type === 3) {
                        $scope.listDieuChuyenVatTuKhongQuaKho = [];
                        $scope.modelVatTuKhongQuaKho.totalItems = data.totalRecord;
                        $scope.listDieuChuyenVatTuKhongQuaKho = data.data;
                        //if ($scope.modelVTAE.type === 2 && $scope.listVatTuKhongQuaKho.length > 0) {
                        //    $scope.viewDetailVatTu($scope.listVatTuKhongQuaKho[0], 0);
                        //}
                    } else {
                        $scope.listVatTuNhapTra = [];
                        $scope.modelVatTuNhapTra.totalItems = data.totalRecord;
                        $scope.listVatTuNhapTra = data.data;
                        //if ($scope.modelVTAE.type === 4 && $scope.listVatTuNhapTra.length > 0) {
                        //    $scope.viewDetailVatTu($scope.listVatTuNhapTra[0], 0);
                        //}
                    }
                } else {
                    toastr.error(data.message);
                }
                // hideLoading();
                $scope.$apply();

            }
        });
    }
    // #endregion

    // #region Giao thẳng vật tư không qua kho
    $scope.tabGTVTKhongQuaKhoSelect = function () {
        $timeout(function () {
            angular.element('a[href="#giaothangvattu"]').trigger('click');
        });
    }

    $scope.pageGTVTKhongQuaKhoChanged = function () {
        getVatTu($scope.modelHopDongView.ID_HOP_DONG, 2);
    }

    $scope.addGTVTKhongQuaKhoModal = function () {
        $scope.modelGTVTKhongQuaKhoAE = { type: $scope.modelGTVTKhongQuaKhoAE.type, soPhieu: '', ngayThang: today, soPhieuH55: '', nguoiNhan: '', noiNhan: '', noiGiao: '', noiNhan: $scope.modelHopDongView.SO_HD, moTa: '', capNhatSLTT: false, lanhDao: '', phuongThucVanChuyen: '' };
        $scope.listSanPhamAdd = [];
        $scope.idEditGTVTKhongQuaKho = false;
        $scope.isChoDuyet = false;
    }

    $scope.validationGTVTKhongQuaKho = generateRequireFiled([
        //genReqField('soPhieu', 'Số phiếu'),
        genReqField('ngayThang', 'Ngày tháng'),
        genReqField('nguoiNhan', 'Người nhận')]);

    $scope.addGTVTKhongQuaKho = function () {
        var flag = $scope.listSanPhamVTAddValidation(['tenHang', 'soLuong']);
        if ($scope.modelGTVTKhongQuaKhoAE.noiGiao == '' || $scope.modelGTVTKhongQuaKhoAE.noiGiao == null) {
            $scope.modelGTVTKhongQuaKhoAE.requiredNoiGiao = true;
            flag = flag && false;
        } else {
            $scope.modelGTVTKhongQuaKhoAE.requiredNoiGiao = false;
            flag = flag && true;
        }
        //if ($scope.modelGTVTKhongQuaKhoAE.lanhDao == '' || $scope.modelGTVTKhongQuaKhoAE.lanhDao == null) {
        //    $scope.modelGTVTKhongQuaKhoAE.requiredLanhDao = true;
        //    flag = flag && false;
        //} else {
        //    $scope.modelGTVTKhongQuaKhoAE.requiredLanhDao = false;
        //    flag = flag && true;
        //}
        if ($scope.gtvtkhongquakhoform.validate() && flag) {
            var model = {
                ID: $scope.modelGTVTKhongQuaKhoAE.id ?? 0,
                SO_PHIEU: $scope.modelGTVTKhongQuaKhoAE.soPhieu,
                NGAY_GIAO: $scope.modelGTVTKhongQuaKhoAE.ngayThang,
                HOP_DONG_CHINH_ID: $scope.modelHopDongView.ID_HOP_DONG,
                SO_HOP_DONG_CHINH: $scope.modelHopDongView.SO_HD,
                SO_PHIEU_H55: $scope.modelGTVTKhongQuaKhoAE.soPhieuH55,
                NGUOI_NHAN: $scope.modelGTVTKhongQuaKhoAE.nguoiNhan,
                KHO_ID: '',
                TEN_KHO: '',
                SO_HD: $scope.modelHopDongView.SO_HD,
                HOP_DONG_ID: $scope.modelHopDongView.ID_HOP_DONG,
                HOP_DONG_GUI_ID: $scope.modelGTVTKhongQuaKhoAE.noiGiao,
                SO_HD_GUI: $scope.listHopdong.find(x => x.ID_HOP_DONG === $scope.modelGTVTKhongQuaKhoAE?.noiGiao).SO_HD,
                MO_TA: $scope.modelGTVTKhongQuaKhoAE.moTa,
                LOAI_XUAT_KHO: 2,
                IS_CAP_NHAT_SLTT: $scope.modelGTVTKhongQuaKhoAE.capNhatSLTT ? 'Y' : 'N',
                LANH_DAO: $scope.modelGTVTKhongQuaKhoAE.lanhDao,
                PHUONG_THUC_VAN_CHUYEN: $scope.modelGTVTKhongQuaKhoAE.phuongThucVanChuyen,
                HD_GIAO_VT_DATA: []
            };
            console.log("MODEL", model);
            if ($scope.listSanPhamAdd.length > 0) {
                $scope.listSanPhamAdd.map((item, index) => {
                    var md = {
                        ID: item.id ?? 0,
                        SP_ID: item.idHang,
                        TEN_SP: item.tenHang.text,
                        TEN_DVT: item.dvt,
                        SO_LUONG_TON: item.soLuongTon,
                        SO_LUONG: item.soLuong.value,
                        SO_LUONG_THUC_TE: item.soLuongTT.value
                    }
                    model.HD_GIAO_VT_DATA.push(md);
                })
            }
            if (flag && model !== undefined) {
                // showToast();
                if (model.HD_GIAO_VT_DATA.length > 0) {
                    var url = '';
                    if ($scope.idEditGTVTKhongQuaKho) {
                        url = 'UpdateVatTu';
                    } else {
                        console.log('AddVatTu-model', model);
                        url = 'AddVatTu';
                    }
                    var data = {
                        model: model
                    };
                    $scope.callAPI("POST", url, data, $scope.addGTVTKhongQuaKhoResponse);
                } else {
                    toastr.error("Bạn chưa chọn sản phẩm nào!");
                }

            }
        }
    }
    $scope.addGTVTKhongQuaKhoResponse = (data) => {
        if (data.status) {
            getVatTu($scope.modelHopDongView.ID_HOP_DONG, 2);
            $scope.modelGTVTKhongQuaKhoAE = { type: 2, ngayThang: today, noiGiao: $scope.modelHopDongView.SO_HD };
            $scope.listSanPhamAdd = [];
            $('#modalAddGiaoThangVatTu').modal('hide');
        } else {
            toastr.error(data.message);
        }
        //  hideLoading();
        $scope.$apply();
    }
    $scope.updateGTVTKhongQuaKhoModal = function (item) {
        $scope.idEditGTVTKhongQuaKho = true;
        $scope.listSanPhamAdd = [];
        $scope.modelGTVTKhongQuaKhoAE = {
            ...$scope.modelGTVTKhongQuaKhoAE,
            id: item.ID,
            soPhieu: item.SO_PHIEU,
            ngayThang: item.NGAY_GIAO,
            soPhieuH55: item.SO_PHIEU_H55,
            nguoiNhan: item.NGUOI_NHAN,
            noiNhan: item.SO_HD_DEN,
            idHopDong: item.HOP_DONG_ID,
            noiGiao: item.HOP_DONG_GUI_ID,
            moTa: item.MO_TA,
            capNhatSLTT: item.IS_CAP_NHAT_SLTT == 'Y' ? true : false,
            lanhDao: item.LANH_DAO,
            phuongThucVanChuyen: item.PHUONG_THUC_VAN_CHUYEN
        };
        $scope.isChoDuyet = item.TRANG_THAI == 'U' || item.TRANG_THAI == 'A';
        $scope.onChangeDMHopDong(item.HOP_DONG_GUI_ID, 1); //get list sản phẩm by hd
        $.ajax({
            type: 'POST',
            url: '/QuanLyHopDong/GetDanhSachGiaoVatTuChiTiet',
            data: {
                ...tableNoPagingBase,
                idVatTu: item.ID
            },
            success: function (data) {
                if (data.status) {
                    data.data.map((item) => {
                        var modelSanPham = [{
                            checked: false,
                            id: item.ID,
                            idHang: item.SP_ID,
                            tenHang: {
                                value: item.SP_ID,
                                text: item.TEN_SP,
                                edit: true
                            },
                            dvt: item.TEN_DVT,
                            soLuongTon: item.SO_LUONG_TON,
                            soLuong: {
                                value: item.SO_LUONG,
                                edit: true
                            },
                            soLuongTT: {
                                value: item.SO_LUONG_THUC_TE,
                                edit: true
                            }
                        }];
                        $scope.listSanPhamAdd = [...modelSanPham, ...$scope.listSanPhamAdd];
                    })
                } else {
                    toastr.error(data.message);
                }
                hideLoading();
                $scope.$apply();

            }
        });
        $('#modalAddGiaoThangVatTu').modal('show');

    }
    $scope.removeGTVTKhongQuaKho = function () {
        var listChecked = $scope.listVatTuKhongQuaKho.filter(x => x.CHECKED);
        if (listChecked.length > 0) {
            var listId = [];
            listChecked.map((item) => {
                listId.push(item.ID);
            })
            $ngConfirm({
                title: 'Xác nhận',
                content: 'Bạn có muốn xóa các vật tư này không?',
                scope: $scope,
                buttons: {
                    sayBoo: {
                        text: 'Xác nhận',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            $.ajax({
                                type: 'Post',
                                url: '/QuanLyHopDong/XoaVatTu',
                                data: {
                                    idVatTu: listId.join(',')
                                },
                                success: function (data) {
                                    if (data.status) {
                                        getVatTu($scope.modelHopDongView.ID_HOP_DONG, 2);
                                    } else {
                                        toastr.error(data.message);
                                    }
                                    hideLoading();
                                    $scope.$apply();

                                }
                            });
                        }
                    },
                    close: {
                        text: 'Hủy',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                        }
                    }
                }
            });
        } else {
            toastr.error("Bạn chưa chọn vật tư nào!");
        }
    }

    $scope.addDsGTVatTuModal = function () {
        $scope.listSanPhamCuaHopDong.map((item, index) => { item.CHECKED = false; });
    }

    $scope.saveCheckedVatTuKhongKho = function () {
        $scope.listSanPhamCuaHopDong.map((item, index) => {
            if (item.CHECKED) {
                var sp = {
                    checked: false,
                    id: null,
                    idHang: item.ID_SP,
                    tenHang: {
                        value: item.ID_SP,
                        text: item.TEN_SP,
                        edit: true
                    },
                    dvt: item.DVT,
                    soLuongTon: item.SO_LUONG_XUAT,
                    soLuong: {
                        value: null,
                        edit: true
                    },
                    soLuongTT: {
                        value: null,
                        edit: true
                    }
                };
                var arr = $.grep($scope.listSanPhamAdd, function (x) {
                    return x.idHang == sp.idHang;
                });
                if (arr == undefined || arr.length == 0) {
                    $scope.listSanPhamAdd.push(sp);
                }

            }
        })
    }
    // #endregion

    // #region Điều chuyển vật tư không qua kho
    $scope.pageDieuChuyenVTKhongQuaKhoChanged = function () {
        getVatTu($scope.modelHopDongView.ID_HOP_DONG, 3);
    }
    $scope.addDieuChuyenVTKhongQuaKhoModal = function () {
        $scope.modelDieuChuyenVTKhongQuaKhoAE = { type: $scope.modelDieuChuyenVTKhongQuaKhoAE.type, soPhieu: '', ngayThang: today, soPhieuH55: '', nguoiNhan: '', noiNhan: '', noiGiao: $scope.modelHopDongView.SO_HD, noiNhan: '', moTa: '', capNhatSLTT: false, lanhDao: '', phuongThucVanChuyen: '' };
        $scope.listSanPhamAdd = [];
        $scope.idEditDieuChuyenVT = false;
        $scope.isChoDuyet = false;
    }
    $scope.validationDieuChuyenVTKhongQuaKho = generateRequireFiled([
        //genReqField('soPhieu', 'Số phiếu'), 
        genReqField('ngayThang', 'Ngày tháng'),
        genReqField('nguoiNhan', 'Người nhận')]);

    $scope.addDieuChuyenVTKhongQuaKho = function () {
        var flag = $scope.listSanPhamVTAddValidation(['tenHang', 'soLuong']);
        if ($scope.modelDieuChuyenVTKhongQuaKhoAE.noiNhan == '' || $scope.modelDieuChuyenVTKhongQuaKhoAE.noiNhan == null) {
            $scope.modelDieuChuyenVTKhongQuaKhoAE.requiredNoiNhan = true;
            flag = flag && false;
        } else {
            $scope.modelDieuChuyenVTKhongQuaKhoAE.requiredNoiNhan = false;
            flag = flag && true;
        }
        //if ($scope.modelDieuChuyenVTKhongQuaKhoAE.lanhDao == '' || $scope.modelDieuChuyenVTKhongQuaKhoAE.lanhDao == null) {
        //    $scope.modelDieuChuyenVTKhongQuaKhoAE.requiredLanhDao = true;
        //    flag = flag && false;
        //} else {
        //    $scope.modelDieuChuyenVTKhongQuaKhoAE.requiredLanhDao = false;
        //    flag = flag && true;
        //}
        if ($scope.dcvtkhongquakhoform.validate() && flag) {
            var model = {
                ID: $scope.modelDieuChuyenVTKhongQuaKhoAE.id ?? 0,
                SO_PHIEU: $scope.modelDieuChuyenVTKhongQuaKhoAE.soPhieu,
                NGAY_GIAO: $scope.modelDieuChuyenVTKhongQuaKhoAE.ngayThang,
                HOP_DONG_CHINH_ID: $scope.modelHopDongView.ID_HOP_DONG,
                SO_HOP_DONG_CHINH: $scope.modelHopDongView.SO_HD,
                SO_PHIEU_H55: $scope.modelDieuChuyenVTKhongQuaKhoAE.soPhieuH55,
                NGUOI_NHAN: $scope.modelDieuChuyenVTKhongQuaKhoAE.nguoiNhan,
                KHO_ID: '',
                TEN_KHO: '',
                SO_HD: $scope.listHopdong.find(x => x.ID_HOP_DONG === $scope.modelDieuChuyenVTKhongQuaKhoAE?.noiNhan).SO_HD,
                HOP_DONG_ID: $scope.modelDieuChuyenVTKhongQuaKhoAE.noiNhan,
                HOP_DONG_GUI_ID: $scope.modelHopDongView.ID_HOP_DONG,
                SO_HD_GUI: $scope.modelHopDongView.SO_HD,
                MO_TA: $scope.modelDieuChuyenVTKhongQuaKhoAE.moTa,
                LOAI_XUAT_KHO: 3,
                IS_CAP_NHAT_SLTT: $scope.modelDieuChuyenVTKhongQuaKhoAE.capNhatSLTT ? 'Y' : 'N',
                LANH_DAO: $scope.modelDieuChuyenVTKhongQuaKhoAE.lanhDao,
                PHUONG_THUC_VAN_CHUYEN: $scope.modelDieuChuyenVTKhongQuaKhoAE.phuongThucVanChuyen,
                HD_GIAO_VT_DATA: []
            };
            console.log("MODEL", model);
            if ($scope.listSanPhamAdd.length > 0) {
                $scope.listSanPhamAdd.map((item, index) => {
                    var md = {
                        ID: item.id ?? 0,
                        SP_ID: item.idHang,
                        TEN_SP: item.tenHang.text,
                        TEN_DVT: item.dvt,
                        SO_LUONG_TON: item.soLuongTon,
                        SO_LUONG: item.soLuong.value,
                        SO_LUONG_THUC_TE: item.soLuongTT.value
                    }
                    model.HD_GIAO_VT_DATA.push(md);
                })
            }
            if (flag && model !== undefined) {
                // showToast();
                if (model.HD_GIAO_VT_DATA.length > 0) {
                    var url = '';
                    if ($scope.idEditDieuChuyenVT) {
                        url = 'UpdateVatTu';
                    } else {
                        console.log('AddVatTu-model', model);
                        url = 'AddVatTu';
                    }
                    var data = {
                        model: model
                    };
                    $scope.callAPI("POST", url, data, $scope.addDieuChuyenVTKhongQuaKhoResponse);
                } else {
                    toastr.error("Bạn chưa chọn sản phẩm nào!");
                }

            }
        }
    }
    $scope.addDieuChuyenVTKhongQuaKhoResponse = (data) => {
        if (data.status) {
            getVatTu($scope.modelHopDongView.ID_HOP_DONG, 3);
            $scope.modelDieuChuyenVTKhongQuaKhoAE = { type: $scope.modelDieuChuyenVTKhongQuaKhoAE.type, soPhieu: '', ngayThang: today, soPhieuH55: '', nguoiNhan: '', noiNhan: '', noiGiao: $scope.modelHopDongView.SO_HD, noiNhan: '', moTa: '' };
            $scope.listSanPhamAdd = [];
            $('#modalAddDieuChuyenVatTu').modal('hide');
        } else {
            toastr.error(data.message);
        }
        //  hideLoading();
        $scope.$apply();
    }
    $scope.updateDieuChuyenVTKhongQuaKhoModal = function (item) {
        $scope.idEditDieuChuyenVT = true;
        $scope.listSanPhamAdd = [];
        $scope.modelDieuChuyenVTKhongQuaKhoAE = {
            ...$scope.modelDieuChuyenVTKhongQuaKhoAE,
            id: item.ID,
            soPhieu: item.SO_PHIEU,
            ngayThang: item.NGAY_GIAO,
            soPhieuH55: item.SO_PHIEU_H55,
            nguoiNhan: item.NGUOI_NHAN,
            noiNhan: item.HOP_DONG_DEN_ID,
            idHopDong: item.HOP_DONG_ID,
            noiGiao: item.SO_HD_GUI,
            moTa: item.MO_TA,
            capNhatSLTT: item.IS_CAP_NHAT_SLTT == 'Y' ? true : false,
            lanhDao: item.LANH_DAO,
            phuongThucVanChuyen: item.PHUONG_THUC_VAN_CHUYEN
        };
        $scope.isChoDuyet = item.TRANG_THAI == 'U' || item.TRANG_THAI == 'A';
        $.ajax({
            type: 'POST',
            url: '/QuanLyHopDong/GetDanhSachGiaoVatTuChiTiet',
            data: {
                ...tableNoPagingBase,
                idVatTu: item.ID
            },
            success: function (data) {
                if (data.status) {
                    data.data.map((item) => {
                        var modelSanPham = [{
                            checked: false,
                            id: item.ID,
                            idHang: item.SP_ID,
                            tenHang: {
                                value: item.SP_ID,
                                text: item.TEN_SP,
                                edit: true
                            },
                            dvt: item.TEN_DVT,
                            soLuongTon: item.SO_LUONG_TON,
                            soLuong: {
                                value: item.SO_LUONG,
                                edit: true
                            },
                            soLuongTT: {
                                value: item.SO_LUONG_THUC_TE,
                                edit: true
                            }
                        }];
                        $scope.listSanPhamAdd = [...modelSanPham, ...$scope.listSanPhamAdd];
                    })
                } else {
                    toastr.error(data.message);
                }
                hideLoading();
                $scope.$apply();

            }
        });
        $('#modalAddDieuChuyenVatTu').modal('show');

    }
    $scope.removeDieuChuyenVTKhongQuaKho = function () {
        var listChecked = $scope.listDieuChuyenVatTuKhongQuaKho.filter(x => x.CHECKED);
        if (listChecked.length > 0) {
            var listId = [];
            listChecked.map((item) => {
                listId.push(item.ID);
            })
            $ngConfirm({
                title: 'Xác nhận',
                content: 'Bạn có muốn xóa các vật tư này không?',
                scope: $scope,
                buttons: {
                    sayBoo: {
                        text: 'Xác nhận',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            $.ajax({
                                type: 'Post',
                                url: '/QuanLyHopDong/XoaVatTu',
                                data: {
                                    idVatTu: listId.join(',')
                                },
                                success: function (data) {
                                    if (data.status) {
                                        getVatTu($scope.modelHopDongView.ID_HOP_DONG, 3);
                                    } else {
                                        toastr.error(data.message);
                                    }
                                    hideLoading();
                                    $scope.$apply();

                                }
                            });
                        }
                    },
                    close: {
                        text: 'Hủy',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                        }
                    }
                }
            });
        } else {
            toastr.error("Bạn chưa chọn vật tư nào!");
        }
    }
    //  #endregion

    // #region Nhập trả vật tư
    $scope.pageVatTuNhapTraChanged = function () {
        getVatTu($scope.modelHopDongView.ID_HOP_DONG, 4);
    }
    $scope.addNhapTraVatTuModal = function () {
        $scope.modelNhapTraVatTuAE = { type: $scope.modelNhapTraVatTuAE.type, soPhieu: '', ngayThang: today, soPhieuH55: '', nguoiNhan: '', noiNhan: '', noiGiao: $scope.modelHopDongView.SO_HD, noiNhan: '', moTa: '', capNhatSLTT: false, lanhDao: '', phuongThucVanChuyen: '' };
        $scope.listSanPhamAdd = [];
        $scope.idEditNhapTraVT = false;
        $scope.isChoDuyet = false;
    }
    $scope.validationNhapTraVatTu = $scope.khoPhieuXTValidation = generateRequireFiled([
        //genReqField('soPhieu', 'Số phiếu'),
        genReqField('ngayThang', 'Ngày tháng'),
        genReqField('nguoiNhan', 'Người nhận')
    ]);

    $scope.addNhapTraVatTu = function () {
        var flag = $scope.listSanPhamVTAddValidation(['tenHang', 'soLuong']);
        if ($scope.modelNhapTraVatTuAE.noiNhan == '' || $scope.modelNhapTraVatTuAE.noiNhan == null) {
            $scope.modelNhapTraVatTuAE.requiredNoiNhan = true;
            flag = flag && false;
        } else {
            $scope.modelNhapTraVatTuAE.requiredNoiNhan = false;
            flag = flag && true;
        }
        //if ($scope.modelNhapTraVatTuAE.lanhDao == '' || $scope.modelNhapTraVatTuAE.lanhDao == null) {
        //    $scope.modelNhapTraVatTuAE.requiredLanhDao = true;
        //    flag = flag && false;
        //} else {
        //    $scope.modelNhapTraVatTuAE.requiredLanhDao = false;
        //    flag = flag && true;
        //}
        if ($scope.nhaptravattuform.validate() && flag) {
            var model = {
                ID: $scope.modelNhapTraVatTuAE.id ?? 0,
                SO_PHIEU: $scope.modelNhapTraVatTuAE.soPhieu,
                NGAY_GIAO: $scope.modelNhapTraVatTuAE.ngayThang,
                HOP_DONG_CHINH_ID: $scope.modelHopDongView.ID_HOP_DONG,
                SO_HOP_DONG_CHINH: $scope.modelHopDongView.SO_HD,
                SO_PHIEU_H55: $scope.modelNhapTraVatTuAE.soPhieuH55,
                NGUOI_NHAN: $scope.modelNhapTraVatTuAE.nguoiNhan,
                KHO_ID: $scope.modelNhapTraVatTuAE.noiNhan,
                TEN_KHO: $scope.listKho.find(x => x.ID === $scope.modelNhapTraVatTuAE?.noiNhan).TEN_DON_VI,
                SO_HD: '',
                HOP_DONG_ID: '',
                HOP_DONG_GUI_ID: $scope.modelHopDongView.ID_HOP_DONG,
                SO_HD_GUI: $scope.modelHopDongView.SO_HD,
                MO_TA: $scope.modelNhapTraVatTuAE.moTa,
                LOAI_XUAT_KHO: 4,
                IS_CAP_NHAT_SLTT: $scope.modelNhapTraVatTuAE.capNhatSLTT ? 'Y' : 'N',
                LANH_DAO: $scope.modelNhapTraVatTuAE.lanhDao,
                PHUONG_THUC_VAN_CHUYEN: $scope.modelNhapTraVatTuAE.phuongThucVanChuyen,
                HD_GIAO_VT_DATA: []
            };
            console.log("MODEL", model);
            if ($scope.listSanPhamAdd.length > 0) {
                $scope.listSanPhamAdd.map((item, index) => {
                    var md = {
                        ID: item.id ?? 0,
                        SP_ID: item.idHang,
                        TEN_SP: item.tenHang.text,
                        TEN_DVT: item.dvt,
                        SO_LUONG_TON: item.soLuongTon,
                        SO_LUONG: item.soLuong.value,
                        SO_LUONG_THUC_TE: item.soLuongTT.value
                    }
                    model.HD_GIAO_VT_DATA.push(md);
                })
            }
            if (flag && model !== undefined) {
                // showToast();
                if (model.HD_GIAO_VT_DATA.length > 0) {
                    var url = '';
                    if ($scope.idEditNhapTraVT) {
                        url = 'UpdateVatTu';
                    } else {
                        console.log('AddVatTu-model', model);
                        url = 'AddVatTu';
                    }
                    var data = {
                        model: model
                    };
                    $scope.callAPI("POST", url, data, $scope.addNhapTraVatTuResponse);
                } else {
                    toastr.error("Bạn chưa chọn sản phẩm nào!");
                }

            }
        }
    }
    $scope.addNhapTraVatTuResponse = (data) => {
        if (data.status) {
            getVatTu($scope.modelHopDongView.ID_HOP_DONG, 4);
            $scope.modelNhapTraVatTuAE = { type: $scope.modelNhapTraVatTuAE.type, soPhieu: '', ngayThang: today, soPhieuH55: '', nguoiNhan: '', noiNhan: '', noiGiao: $scope.modelHopDongView.SO_HD, noiNhan: '', moTa: '' };
            $scope.listSanPhamAdd = [];
            $('#modalAddNhapTraVatTu').modal('hide');
        } else {
            toastr.error(data.message);
        }
        //  hideLoading();
        $scope.$apply();
    }
    $scope.updateVatTuNhapTraModal = function (item) {
        $scope.idEditNhapTraVT = true;
        $scope.listSanPhamAdd = [];
        $scope.modelNhapTraVatTuAE = {
            ...$scope.modelNhapTraVatTuAE,
            id: item.ID,
            soPhieu: item.SO_PHIEU,
            ngayThang: item.NGAY_GIAO,
            soPhieuH55: item.SO_PHIEU_H55,
            nguoiNhan: item.NGUOI_NHAN,
            noiNhan: item.KHO_ID,
            idHopDong: item.HOP_DONG_ID,
            noiGiao: item.SO_HD_GUI,
            moTa: item.MO_TA,
            capNhatSLTT: item.IS_CAP_NHAT_SLTT == 'Y' ? true : false,
            lanhDao: item.LANH_DAO,
            phuongThucVanChuyen: item.PHUONG_THUC_VAN_CHUYEN,
        };
        $scope.isChoDuyet = item.TRANG_THAI == 'U' || item.TRANG_THAI == 'A';
        $.ajax({
            type: 'POST',
            url: '/QuanLyHopDong/GetDanhSachGiaoVatTuChiTiet',
            data: {
                ...tableNoPagingBase,
                idVatTu: item.ID
            },
            success: function (data) {
                if (data.status) {
                    data.data.map((item) => {
                        var modelSanPham = [{
                            checked: false,
                            id: item.ID,
                            idHang: item.SP_ID,
                            tenHang: editableField(item.SP_ID, item.TEN_SP),
                            dvt: item.TEN_DVT,
                            soLuongTon: item.SO_LUONG_TON,
                            soLuong: editableField(item.SO_LUONG),
                            soLuongTT: editableField(item.SO_LUONG_THUC_TE)
                        }];
                        $scope.listSanPhamAdd = [...modelSanPham, ...$scope.listSanPhamAdd];
                    })
                } else {
                    toastr.error(data.message);
                }
                hideLoading();
                $scope.$apply();

            }
        });
        $('#modalAddNhapTraVatTu').modal('show');

    }
    $scope.removeNhapTraVatTu = function () {
        var listChecked = $scope.listVatTuNhapTra.filter(x => x.CHECKED);
        if (listChecked.length == 0) {
            toastr.error("Bạn chưa chọn vật tư nào!");
            return;
        }

        var listId = [];
        listChecked.map((item) => {
            listId.push(item.ID);
        })
        const removeNhapTraVTConfirm = () => {
            const removeNhapTraVTResponse = () => {
                getVatTu($scope.modelHopDongView.ID_HOP_DONG, 4);
            }
            $scope.callAPI('Post', 'XoaVatTu', { idVatTu: listId.join(',') }, removeNhapTraVTResponse);
        }
        $scope.confirmDialog('Bạn có muốn xóa các vật tư này không?', removeNhapTraVTConfirm);
    }
    // #endregion

    // #region Kho Hop Dong Phieu Giao Thang

    $scope.khoPhieuGTPaging = { ...tablePagingBase }
    $scope.khoPhieuGTListSP = [];
    $scope.khoPhieuGTListSPType = {
        CHECKED: false,
        ID_SP: null,
        TEN_SP: { ...editFieldBase },
        DVT: null,
        SO_LUONG: { ...editFieldBase },
    }
    $scope.khoPhieuGTModel = {}
    $scope.khoPhieuGTSTT = -1;
    $scope.khoPhieuGTSelected = {}
    $scope.selectKhoPhieuGTItem = (item,index) => {
        $scope.khoPhieuGTSelected = item;
        $scope.khoPhieuGTSTT = index;
    }

    $scope.khoPhieuGTGetList = () => {
        showToast();
        $scope.khoPhieuGTListData = [];
        const data = {
            ...$scope.khoPhieuGTPaging,
            idHopDong: $scope.modelHopDongView.ID_HOP_DONG
        };

        const khoPhieuGTGetListReponse = (data) => {
            console.log('[Kho PhieuGT] list data', data);
            $scope.khoPhieuGTPaging.totalItems = data.totalRecord;
            data.data.forEach(item => {
                $scope.khoPhieuGTListData.push({
                    ...item
                })
            });
        }
        $scope.callAPI('POST', 'KhoPhieuGiaoThangList', data, khoPhieuGTGetListReponse, { hideLoading: true });
    }

    $scope.khoPhieuGTGetDetail = (item) => {
        console.log('[Kho Phieu Giao Thang] detail', item);
        $scope.khoPhieuGTModel = {
            ...item,
            NOI_GIAO: $scope.modelHopDongView.SO_HD,
            NOI_NHAN: item.SO_HD_DEN
        }
        $scope.khoPhieuGTListSP = [];
        const data = {
            ...tableNoPagingBase,
            id: $scope.khoPhieuGTModel.ID
        };

        const khoPhieuGTGetDetailReponse = (data) => {
            data.data.map(item => {
                var modelSanPham = [{
                    ...item,
                    TEN_SP: $scope.allSP.find(i => i.ID == item.SP_ID)["TEN_SP"]
                }];
                $scope.khoPhieuGTListSP = [...modelSanPham, ...$scope.khoPhieuGTListSP];
            })
        }
        $scope.callAPI('POST', 'KhoPhieuGiaoThangSPDetail', data, khoPhieuGTGetDetailReponse);
    }

    $scope.khoGiaoThangInPhieu = () => {
        if ($scope.khoPhieuGTSelected && $scope.khoPhieuGTSelected.ID)
            $scope.inPhieuVatTuChinh(2, $scope.khoPhieuGTSelected.ID);
    }

    // #endregion

    // #region Kho Phieu Xuat Thang
    $scope.khoPhieuXTPaging = { ...tablePagingBase }
    $scope.khoPhieuXTModel = [];
    $scope.khoPhieuXTValidation = generateRequireFiled([
        genReqField('NGAY_GIAO', 'Ngày giao'),
        genReqField('NGUOI_GIAO', 'Người giao'),
        genReqField('HOP_DONG_DEN_ID', 'Hợp đồng đến'),
        genReqField('PHUONG_THUC_VAN_CHUYEN', 'Phương thức vận chuyển')

    ]);

    $scope.khoPhieuXTOpenModal = function (item, isNotChange) {

        $scope.listSPAdd = [];
        if (item) {
            console.log('[Phieu Xuat Thang] Detail', item);
            $scope.khoPhieuXTModel = {
                ...item,
                SO_HD_GUI: $scope.modelHopDongView.SO_HD,
                HOP_DONG_GUI_ID: $scope.modelHopDongView.ID_HOP_DONG,
                IS_NOT_CHANGE: !!isNotChange
            }
            $scope.khoPhieuXTGetListSP(item);
            $('#modalAddKhoPhieuXuatThang').modal('show');
            return;
        }
        $scope.khoPhieuXTModel = {
            SO_HD_GUI: $scope.modelHopDongView.SO_HD,
            HOP_DONG_GUI_ID: $scope.modelHopDongView.ID_HOP_DONG,
            IS_NOT_CHANGE: !!isNotChange,
            TRANG_THAI: 'I'
        };
        capNhatSoLuongTonNoiDung();
    }

    $scope.khoPhieuXTGetList = () => {
        showToast();
        $scope.khoPhieuXTListData = [];
        const data = {
            ...$scope.khoPhieuXTPaging,
            idHopDong: $scope.modelHopDongView.ID_HOP_DONG,
            type: 5
        };

        const khoPhieuXTGetListReponse = (data) => {
            console.log('[Kho PhieuXT] list data', data);
            $scope.khoPhieuXTPaging.totalItems = data.totalRecord;
            data.data.forEach(item => {
                $scope.khoPhieuXTListData.push({
                    ...item,
                    NGAY_TAO: moment(item.NGAY_TAO).format('DD/MM/YYYY')
                })
            });
        }
        $scope.callAPI('POST', 'GetDanhSachGiaoVatTu', data, khoPhieuXTGetListReponse, { hideLoading: true });
    }

    $scope.khoPhieuXTGetListSP = (item) => {
        const data = {
            ...tableNoPagingBase,
            idVatTu: item.ID,
        }

        const khoPhieuXTListSPResponse = (data) => {
            console.log("[Phieu XT] listSP", data);
            data.data.forEach(item => {
                var modelSanPham = [{
                    ...item,
                    ID: item.SP_ID,
                    ID_RECORD: item.ID,
                    TEN_SP: $scope.allSP.find(i => i.ID == item.SP_ID)['TEN_SP'],
                    DVT: $scope.allSP.find(i => i.ID == item.SP_ID)['TEN_DVT']
                }];
                $scope.listSPAdd = [...modelSanPham, ...$scope.listSPAdd];
            })
            capNhatSoLuongTonlistSPByPhieu('listSPAdd');
        }
        $scope.callAPI('POST', 'GetDanhSachGiaoVatTuChiTiet', data, khoPhieuXTListSPResponse);
    }

    $scope.khoPhieuXTCreateOrUpdate = function () {
        var flag = $scope.listSanPhamAddValidation(['SO_LUONG']);
        fieldValidation($scope.khoPhieuXTModel, $scope.khoPhieuXTValidation.rules, flag);

        if ($scope.khoPhieuXTForm.validate() && flag) {
            var model = {
                ...$scope.khoPhieuXTModel,
                ID_HOP_DONG: $scope.modelHopDongView.ID_HOP_DONG,
                HOP_DONG_ID: $scope.khoPhieuXTModel.HOP_DONG_DEN_ID,
                HOP_DONG_CHINH_ID: $scope.modelHopDongView.ID_HOP_DONG,
                SO_HOP_DONG_CHINH: $scope.modelHopDongView.SO_HD,
                SO_HD: $scope.listHopdong.find(i => i.ID_HOP_DONG == $scope.khoPhieuXTModel.HOP_DONG_DEN_ID)['NAME_HOP_DONG'],
                HD_GIAO_VT_DATA: [],
                LOAI_XUAT_KHO: 5,
                ID: $scope.khoPhieuXTModel.ID
            };

            $scope.listSPAdd.map((item, index) => {
                var md = {
                    ...item,
                    ID: item.ID_RECORD,
                    SP_ID: item.ID,
                    GIAO_VT_CHINH_ID: $scope.khoPhieuXTModel.ID
                }
                model.HD_GIAO_VT_DATA.push(md);
            })

            if (flag && model) {
                if (model.HD_GIAO_VT_DATA.length == 0) {
                    toastr.error("Bạn chưa chọn sản phẩm nào!");
                    return;
                }
                const data = {
                    model: model,
                    idHopDong: $scope.modelHopDongView.ID_HOP_DONG
                }
                const createOrUpdateKhoPhieuXTResponse = (data) => {
                    $scope.khoPhieuXTGetList();
                    $('#modalAddKhoPhieuXuatThang').modal('hide');
                }
                if (model.ID) {
                    $scope.callAPI('POST', 'UpdateVatTu', data, createOrUpdateKhoPhieuXTResponse);
                } else {
                    $scope.callAPI('POST', 'AddVatTu', data, createOrUpdateKhoPhieuXTResponse);
                }
            }
        }

    }

    $scope.khoPhieuXTRemove = () => {
        var listChecked = $scope.khoPhieuXTListData.filter(x => x.CHECKED);
        if (listChecked.length == 0) {
            toastr.error("Bạn chưa chọn phiếu xuất thẳng nào!");
            return;
        }
        var listId = [];
        listChecked.map((item) => {
            listId.push(item.ID);
        })
        const khoPhieuXTRemoveConfirm = () => {
            const khoPhieuXTRemoveResponse = () => {
                $scope.khoPhieuXTGetList();
            }
            $scope.callAPI('POST', 'XoaVatTu', { idVatTu: listId.join(',') }, khoPhieuXTRemoveResponse)
        }
        $scope.confirmDialog('Bạn có muốn xóa các vật tư này không?', khoPhieuXTRemoveConfirm);
    }

    // #endregion

    // #region Kho Ton kho san pham
    $scope.khoTonKhoClicked = () => {
        $timeout(function () {
            $scope.noiDungIsShowToast = false;
            angular.element('a[href="#khoTonKhoSP"]').trigger('click');
        });
    }
    $scope.khoTonKhoSPPage = {
        timKiem: ''
    }
    $scope.khoTonKhoSPList = {}
    $scope.khoTonKhoSPGetList = () => {
        showToast();
        const khoTonKhoSPResponse = (data) => {
            console.log('[Kho Ton Kho SP] list', data.data);
            $scope.khoTonKhoSPList = data.data;
        }
        $scope.khoTonKhoCallAPI(khoTonKhoSPResponse, 'SP');
    }
    // #endregion

    // #region Kho Ton kho vat tu
    $scope.khoTonKhoVTPage = {
        timKiem: ''
    }
    $scope.khoTonKhoVTList = {}
    $scope.khoTonKhoVTGetList = () => {
        const khoTonKhoVTResponse = (data) => {
            console.log('[Kho Ton Kho VT] list', data.data);
            $scope.khoTonKhoVTList = data.data;
        }
        $scope.khoTonKhoCallAPI(khoTonKhoVTResponse, 'VT');
    }
    $scope.khoTonKhoCallAPI = (cb, type) => {
        $scope.callAPI('POST', 'KhoTonKhoHD', {
            id: $scope.modelHopDongView.ID_HOP_DONG,
            type: type,
            nam: $scope.modelHopDongView.NAM_KP,
            timKiem: type == 'SP' ? $scope.khoTonKhoSPPage.timKiem : $scope.khoTonKhoVTPage.timKiem
        }, cb, { hideLoading: true });
    }
    // #endregion

    // #region Chuyen Hop Dong Phieu Nhap Kho
    $scope.chuyenHDClicked = () => {
        $timeout(function () {
            angular.element('a[href="#chuyenphieunhapkho"]').trigger('click');
        });
    }
    $scope.khoChuyenHDNKGetList = () => {
        showToast();
        const khoChuyenHDNKResponse = (data) => {
            console.log('[Kho Chuyen HD Nhap Kho]', data.data);
            $scope.chuyenHDPhieuNKList = data.data;
        }
        $scope.callAPI('POST', 'HDChuyenHD', { id: $scope.modelHopDongView.ID_HOP_DONG, type: 'NK' },
            khoChuyenHDNKResponse, { hideLoading: true });
    }
    $scope.chuyenHDNKConfirm = () => {
        let checkedList = $scope.chuyenHDPhieuNKList.filter(i => i.CHECKED)
        if (checkedList.length == 0) {
            toastr.error('Bạn chưa chọn phiếu nhập kho nào');
            return;
        }
        const chuyenHDNKResponse = () => {
            toastr.success('Chuyển phiếu nhập kho hoàn tất');
            $scope.khoChuyenHDNKGetList();
        }
        $scope.callAPI('POST', 'HDChuyenHDSave',
            {
                id: $scope.modelHopDongView.ID_HOP_DONG,
                idChuyen: $scope.hdChuyenHDNKId,
                idPhieu: checkedList.map(i => i.ID).join(','),
                type: 'NK'
            }, chuyenHDNKResponse)
    }
    // #endregion

    // #region Chuyen Hop Dong Phieu Giao Thang

    $scope.khoChuyenHDGTGetList = () => {
        showToast();
        const khoChuyenHDGTResponse = (data) => {
            console.log('[Kho Chuyen HD Giao Thang]', data.data);
            $scope.chuyenHDPhieuGTList = data.data;
        }
        $scope.callAPI('POST', 'HDChuyenHD', { id: $scope.modelHopDongView.ID_HOP_DONG, type: 'GT' },
            khoChuyenHDGTResponse, { hideLoading: true });
    }
    $scope.chuyenHDGTConfirm = () => {
        let checkedList = $scope.chuyenHDPhieuGTList.filter(i => i.CHECKED)
        if (checkedList.length == 0) {
            toastr.error('Bạn chưa chọn phiếu giao thẳng nào');
            return;
        }
        const chuyenHDGTResponse = () => {
            toastr.success('Chuyển phiếu giao thẳng hoàn tất');
            $scope.khoChuyenHDGTGetList();
        }
        $scope.callAPI('POST', 'HDChuyenHDSave',
            {
                id: $scope.modelHopDongView.ID_HOP_DONG,
                idChuyen: $scope.hdChuyenHDGTId,
                idPhieu: checkedList.map(i => i.ID).join(','),
                type: 'GT'
            }, chuyenHDGTResponse)
    }
    // #endregion

    // #region Chuyen Hop Dong Phieu Xuat Thang

    $scope.khoChuyenHDXTGetList = () => {
        showToast();
        const khoChuyenHDXTResponse = (data) => {
            console.log('[Kho Chuyen HD Xuat Thang]', data.data);
            $scope.chuyenHDPhieuXTList = data.data;
        }
        $scope.callAPI('POST', 'HDChuyenHD', { id: $scope.modelHopDongView.ID_HOP_DONG, type: 'XT' },
            khoChuyenHDXTResponse, { hideLoading: true });
    }
    $scope.chuyenHDXTConfirm = () => {
        let checkedList = $scope.chuyenHDPhieuXTList.filter(i => i.CHECKED)
        if (checkedList.length == 0) {
            toastr.error('Bạn chưa chọn phiếu xuất thẳng nào');
            return;
        }
        const chuyenHDXTResponse = () => {
            toastr.success('Chuyển phiếu xuất thẳng hoàn tất');
            $scope.khoChuyenHDXTGetList();
        }
        $scope.callAPI('POST', 'HDChuyenHDSave',
            {
                id: $scope.modelHopDongView.ID_HOP_DONG,
                idChuyen: $scope.hdChuyenHDXTId,
                idPhieu: checkedList.map(i => i.ID).join(','),
                type: 'XT'
            }, chuyenHDXTResponse)
    }
    // #endregion

    // #region Noi dung hợp đồng ban đầu

    $scope.hopDongNDungBdau = () => {
        showToast();
        const ndungBdauResponse = (data) => {
            console.log('[Hop dong] noi dung ban dau', data.data);
            data.data.forEach(i => {
                i.MA_H55 = i.MA_HANG;
                i.TEN_SP = i.TEN_HANG;
                i.TEN_DVT = i.DVT;

            });
            $scope.listSPAdd = data.data;
        }
        $scope.callAPI('POST', 'GetNoiDungBandau', { id: $scope.modelHopDongView.ID_HOP_DONG }, ndungBdauResponse, { hideLoading: true });
    }

    $scope.hopDongDinhMucVatTu = () => {
        showToast();
        const dinhMucVTResponse = (data) => {
            console.log('[Hop dong] dinh muc vat tu', data.data);
            $scope.listDinhMucVatTu = data.data;
        }
        $scope.callAPI('POST', 'GetDinhMucVatTu', { id: $scope.modelHopDongView.ID_HOP_DONG, nam: $scope.modelHopDongView.NAM_KP }, dinhMucVTResponse, { hideLoading: true });
    }

    $scope.saveNoiDungBanDau = () => {
        var flag = $scope.listSanPhamAddValidation([, 'SO_LUONG', 'DON_GIA']);
        if (!flag) {
            return;
        }
        var model = {
            ID_HOP_DONG: $scope.modelHopDongView.ID_HOP_DONG,
            HD_DETAILS: []
        };
        $scope.listSPAdd.map((item) => {
            var md = {
                ...item,
                ID_HANG: item.ID,
                MA_HANG: item.MA_H55,
                TEN_HANG: item.TEN_SP,
                DVT: item.TEN_DVT,
                SO_LUONG: item.SO_LUONG,
                DON_GIA: item.DON_GIA,
                THANH_TIEN: item.SO_LUONG * item.DON_GIA
            }
            model.HD_DETAILS.push(md);
        })

        if (model.HD_DETAILS.length == 0 && $scope.hdPaging.type == 'HD') {
            toastr.error("Bạn chưa chọn sản phẩm nào!");
            return;
        }
        const addHopDongResponse = (data) => {
            console.log('[Hop dong] Add success');
            $('#modalNoiDungHopDongBanDau').modal('hide');
            $scope.listSanPhamAdd = [];
        }
        $scope.callAPI('POST', 'saveNoiDungBanDau', { model: model }, addHopDongResponse, { hideLoading: true });

    }
    // #endregion

    //#region Quyết toán
    $scope.quyetToanClicked = () => {
        $timeout(function () {
            $scope.noiDungIsShowToast = false;
            angular.element('a[href="#quyettoanphieunhapkho"]').trigger('click');
        });
    }

    $scope.getQuyetToanPhieuNhapKho = function () {
        $scope.listQuyetToanPhieuNhapKho = [];
        $scope.callAPI('POST', 'GetListQuyetToanPhieuNhapKho', { idHopDong: $scope.modelHopDongView.ID_HOP_DONG }, $scope.getQuyetToanPhieuNhapKhoResponse);
    }

    $scope.getQuyetToanPhieuNhapKhoResponse = (data) => {
        $scope.listQuyetToanPhieuNhapKho = data.data;
    }

    $scope.getQuyetToanPhieuGiaoThang = function () {
        $scope.listQuyetToanPhieuGiaoThang = [];
        $scope.callAPI('POST', 'GetListQuyetToanPhieuGiaoThang', { idHopDong: $scope.modelHopDongView.ID_HOP_DONG }, $scope.getQuyetToanPhieuGiaoThangResponse);
    }
    $scope.getQuyetToanPhieuGiaoThangResponse = (data) => {
        $scope.listQuyetToanPhieuGiaoThang = data.data;
    }
    $scope.getQuyetToanPhieuXuatThang = function () {
        $scope.listQuyetToanPhieuXuatThang = [];
        $scope.callAPI('POST', 'GetListQuyetToanPhieuXuatThang', { idHopDong: $scope.modelHopDongView.ID_HOP_DONG }, $scope.getQuyetToanPhieuXuatThangResponse);
    }
    $scope.getQuyetToanPhieuXuatThangResponse = (data) => {
        $scope.listQuyetToanPhieuXuatThang = data.data;
    }
    $scope.saveQuyetToan = function () {
        const saveQuyetToanConfirm = () => {
            const saveQuyetToanResponse = () => {
                /*$scope.getDanhSachHopDong();*/
                $scope.modelHopDongView.TRANG_THAI = 'Q';
                $scope.isDisableHD = true;
                $scope.hdHopDong.forEach(x => {
                    x.HD_HOP_DONGS.forEach(y => {
                        if (y.ID_HOP_DONG == $scope.modelHopDongView.ID_HOP_DONG) {
                            y.TRANG_THAI = 'Q';
                        }
                    })
                })
            }
            $scope.callAPI('POST', 'SaveQuyetToan', { idHopDong: $scope.modelHopDongView.ID_HOP_DONG }, saveQuyetToanResponse)
        }
        $scope.confirmDialog('Bạn xác nhận quyết toán hợp đồng này?', saveQuyetToanConfirm);
    }

    $scope.dungCapPhat = function () {
        const dungCapPhatConfirm = () => {
            const dungCapPhatResponse = (data) => {
                //$scope.isDungCapPhat = (data.data == 'Y');
                //if (data.data == 'Y') {
                //    $('#dung-cap-phat-btn').html('<span>Hủy dừng cấp phát</span>');
                //} else {
                //    $('#dung-cap-phat-btn').html('<span>Dừng cấp phát</span>');
                //}
                //$scope.getDanhSachHopDong();
            }
            $scope.callAPI('POST', 'ChangeDungCapPhat', { idHopDong: $scope.modelHopDongView.ID_HOP_DONG }, dungCapPhatResponse)
        }
        $scope.confirmDialog('Bạn xác nhận thay đổi cấp phát hợp đồng này?', dungCapPhatConfirm);
    }
    //#endregion Quyết toán

    // #region Kinh phi ket du

    $scope.kpKetDuOpenModal = () => {
        modelVaySec = {};
    }
    $scope.kpKetDuGetList = () => {
        showToast();
        const kpKetDuResponse = (data) => {
            console.log('[Kinh phi ket du]', data.data);
            data.data.forEach(i => {
                $scope.listHopdong.forEach(x => {
                    if (x.ID_HOP_DONG == i.ID_CHUYEN) {
                        i.SO_HD_DEN = x.SO_HD;
                        return;
                    }
                });
            });
            $scope.kpKetDuList = data.data;
        }
        $scope.callAPI('POST', 'ListKPKetDu', { id: $scope.modelHopDongView.ID_HOP_DONG }, kpKetDuResponse, { hideLoading: true });
    }

    $scope.removePhieuKetDu = function () {
        var listChecked = $scope.kpKetDuList.filter(x => x.CHECKED);
        if (listChecked.length == 0) {
            toastr.error("Bạn chưa chọn phiếu kết dư nào!");
            return;
        }
        var listId = [];
        listChecked.map((item) => {
            listId.push(item.ID);
        })

        const removeVaySecConfirmed = () => {
            const removeVaySecResponse = () => {
                $scope.kpKetDuGetList();
            }
            $scope.callAPI('POST', 'DeleteVaySec', { idVaySec: listId }, removeVaySecResponse);
        }
        $scope.confirmDialog('Bạn có muốn xóa các phiếu vay séc này không?', removeVaySecConfirmed)
    }

    $scope.addKPKetDu = () => {
        if (!$scope.modelVaySec.ID_CHUYEN) {
            $scope.modelVaySec.ID_CHUYEN_ERROR = true;
            flag = false;
        } else {
            $scope.modelVaySec.ID_CHUYEN_ERROR = false;
            flag = true;
        }
        if ($scope.addKetDuForm.validate() && flag) {
            var model = {
                ...$scope.modelVaySec,
                ID_HOP_DONG: $scope.modelHopDongView.ID_HOP_DONG,
            };
            const addVaySecResponse = (data) => {
                if (data.data.Error) {
                    toastr.error(data.data.Title);
                    return;
                }
                console.log('[Kinh Phi Ket Du] add successful')
                $scope.kpKetDuGetList();
                $('#modalAddKinhPhiKetDu').modal('hide');
            }
            $scope.callAPI('POST', 'AddVaySec', { model: model }, addVaySecResponse);
        }
    }
    // #endregion 
    $scope.resetForm = function (tab) {
    }

    // #region Ultil
    $scope.callAPI = (type, url, data, cb, opts) => {
        $.ajax({
            type: type,
            url: `/QuanLyHopDong/${url}`,
            data: data,
            success: function (data) {
                if (data.status) {
                    cb(data)
                } else {
                    toastr.error(data.message);
                }
                if (opts && opts.hideLoading) {
                    hideLoading();
                }
                $scope.$apply();
            },
            error: function (err) {
                if (opts && opts.hideLoading) {
                    hideLoading();
                }
            }
        });
    }

    $scope.listSanPhamAddValidation = (fieldArray) => {
        let flag = true;
        if ($scope.listSPAdd.length == 0) {
            return flag;
        }
        fieldArray.forEach(field => {
            $scope.listSPAdd.map((item) => {
                if (!item[field]) {
                    item[`${field}_ERROR`] = true;
                    flag = false;
                } else {
                    item[`${field}_ERROR`] = false;
                }
            });
        });
        return flag;
    }

    $scope.listSanPhamVTAddValidation = (fieldArray) => {
        let flag = true;
        if ($scope.listSanPhamAdd.length == 0) {
            return flag;
        }
        fieldArray.forEach(field => {
            $scope.listSanPhamAdd.map((item) => {
                if (!item[field].value) {
                    item[field].error = true;
                    flag = false;
                } else {
                    item[field].error = false;
                }
            });
        });
        return flag;
    }

    $scope.confirmDialog = (content, cb) => {
        $ngConfirm({
            title: 'Xác nhận',
            content: content,
            scope: $scope,
            buttons: {
                sayBoo: {
                    text: 'Xác nhận',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        cb();
                    }
                },
                close: {
                    text: 'Hủy',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                    }
                }
            }
        });
    }

    function convertDate2(date) {
        if (date) {
            var d = moment(date, 'DD/MM/YYYY');
            return d.format('YYYYMMDD');
        } else {
            return "";
        }
    }
    function convertDate4(date) {
        if (date) {
            var d = moment(date, 'YYYYMMDD');
            return d.format('DD/MM/YYYY');
        } else {
            return "";
        }
    }

    function editableField(value, text) {
        if (text) {
            return {
                value: value,
                text: text,
                edit: true
            }
        }
        return {
            value: value,
            edit: true
        }
    }

    function genReqField(field, text) {
        return { field, text }
    }
    function generateRequireFiled(arr) {
        rules = {};
        messages = {};
        arr.forEach(item => {
            rules[item['field']] = { required: true };
            messages[item['field']] = { required: `${item['text']} không được để trống` }
        })
        return {
            rules,
            messages
        }
    }

    function fieldValidation(obj, rules, flagChecking) {
        Object.keys(rules).forEach(i => {
            if (!obj[i] || !obj[i].toString().trim()) {
                obj[`${i}_REQUIRED`] = true;
                flagChecking = false;
            } else {
                obj[`${i}_REQUIRED`] = false;
            }
        })
    }
    $scope.inPhieuVatTuChinh = (type, id) => {
        //const inPhieuVatTuChinhResponse = (data) => {
        //    let pdfWindow = window.open("");
        //    pdfWindow.document.write("<iframe width='100%' height='100%' src='data:application/pdf;base64," + data + "'></iframe>");
        //}
        if (!id) {
            switch (type) {
                case 1:
                    id = $scope.modelVTAE.id;
                    break;
                case 2:
                    id = $scope.modelGTVTKhongQuaKhoAE.id;
                    break;
                case 3:
                    id = $scope.modelDieuChuyenVTKhongQuaKhoAE.id;
                    break;
                case 4:
                    id = $scope.modelNhapTraVatTuAE.id;
                    break;
                case 5:
                    id = $scope.khoPhieuXTModel.ID;
                    break;
                default:
                    break;
            }
        }
        //$scope.callAPI('Post', 'InPhieuVatTuChinh', { id: id, type: type }, inPhieuVatTuChinhResponse);
        showToast()
        $.ajax({
            url: '/QuanLyHopDong/InPhieuVatTuChinh',
            type: 'post',
            data: {
                id: id, type: type
            },
            success: function (result) {
                hideLoading();
                let pdfWindow = window.open("");
                pdfWindow.document.write("<iframe width='100%' height='100%' src='data:application/pdf;base64," + result + "'></iframe>");

            },
            error: function (xhr, status, err) {
                alert(err);
                hideLoading();
            }
        });
    }

    $scope.inPhieuDinhMucVatTu = () => {
        showToast()
        $.ajax({
            url: '/QuanLyHopDong/InPhieuDinhMucVatTu',
            type: 'post',
            data: {
                id: $scope.modelHopDongView.ID_HOP_DONG, nam: $scope.modelHopDongView.NAM_KP, soHd: $scope.modelHopDongView.SO_HD
            },
            success: function (result) {
                hideLoading();
                let pdfWindow = window.open("");
                pdfWindow.document.write("<iframe width='100%' height='100%' src='data:application/pdf;base64," + result + "'></iframe>");

            },
            error: function (xhr, status, err) {
                alert(err);
                hideLoading();
            }
        });
    }

    // #endregion
});