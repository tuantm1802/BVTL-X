app.controller("KiemKeKhoController",function ($scope, $uibModal, $ngConfirm, showToast, hideLoading,$rootScope) {
    $scope.ListKhoVL = [];
    $scope.ListKhoHD = [];
    $scope.selectedRowKhoVL = -1;
    $scope.selectedRowKhoHD = -1;
    $scope.ListHangHoa = [];
    $scope.ListHangHoaHD = [];
    $scope.SelectedKho = '';
    $scope.SelectedHangHoa = {};
    $scope.ShowKhoVLContent = false;
    $scope.ShowKhoHDContent = false;
    $scope.TongTang = 0;
    $scope.TongGiam = 0;
    $scope.DuDauKy = 0;
    $scope.DuCuoiKy = 0;
    var today = new Date();
    today.setDate(today.getDate());
    $scope.StartDate = moment(today).subtract(1,'months').format('DD/MM/YYYY');
    $scope.EndDate = moment(today).format('DD/MM/YYYY');
    angular.element(document).ready(function(){
        $scope.GetListKhoVL();
        $scope.GetListKhoHD();
    });
    $scope.Title = '';
    $scope.model = {
        ID: '',
        ID_KHO: '',
        SO_PHIEU: '',
        LY_DO:''
    };
    $scope.SearchDCP = '';
    $scope.pageChanged = function () {
        $scope.GetListKhoVL();
        $scope.GetListKhoHD();
    };
    $scope.kho_vl = function () {
        if ($scope.selectedRowKhoVL != -1) {
            $scope.ShowKhoVLContent = true;
        }
        $scope.ShowKhoHDContent = false;
    }
    $scope.kho_hd = function () {
        
        if ($scope.selectedRowKhoHD != -1) {
            $scope.ShowKhoHDContent = true;
        }
        $scope.ShowKhoVLContent = false;
    }
    $scope.GetListKhoVL = () =>{
        $.ajax({
            type: 'POST',
            url: '/KiemKeKho/GetListKhoVL',
            data: {},
            success: function (response) {
                if(response.Status === 200){
                    $scope.ListKhoVL = response.Data;
                }
                else{
                    toastr.error(response.Title);
                }
                //console.log(response);
                $scope.$apply();
            }
        });
    };
    $scope.GetListKhoHD = () =>{
        $.ajax({
            type: 'POST',
            url: '/KiemKeKho/GetListKhoHD',
            data: {},
            success: function (response) {
                if(response.Status === 200){
                    $scope.ListKhoHD = response.Data;
                }
                else{
                    toastr.error(response.Title);
                }
                //console.log(response);
                $scope.$apply();
            }
        })
    }
    $scope.GetTheKhoVL = (item) => {
      
        $scope.SelectedHangHoa = item;
        $('#TheKhoVL').modal('show');
        $scope.GetDuDauKy();
        $scope.GetChiTietTheKho();
    };
    $scope.GetDuDauKy = ()=>{
        $.ajax({
            type: 'POST',
            url: '/KiemKeKho/GetDuDauKy',
            data: {
                idKho: $scope.SelectedHangHoa.ID_KHO,
                tenKho: $scope.SelectedKho,
                idHang: $scope.SelectedHangHoa.ID_HANG,
                startDate: $scope.StartDate
            },
            success: function (response) {
                if(response.Status === 200){
                    $scope.DuDauKy = response.Data[0].SO_LUONG_TON;
                }
                $scope.$apply();
            }
        });
    }
    $scope.GetChiTietTheKho = () =>{
        $scope.TongTang = 0;
        $scope.TongGiam = 0;
        $scope.DuDauKy = 0;
        $scope.DuCuoiKy = 0;
        $.ajax({
            type: 'POST',
            url: '/KiemKeKho/GetChiTietTheKho',
            data: {
                idKho: $scope.SelectedHangHoa.ID_KHO,
                tenKho: $scope.SelectedKho,
                idHang: $scope.SelectedHangHoa.ID_HANG,
                startDate: $scope.StartDate,
                endDate: $scope.EndDate
            },
            success: function (response) {
                if(response.Status === 200){
                    $scope.ListTheKho = response.Data;
                    $scope.ListTheKho.forEach(x => x.NGAY_CT = moment(x.NGAY_CT, 'YYYYMMDD').format('DD/MM/YYYY'));
                    $scope.TongTang = $scope.ListTheKho.filter(x => x.SL_TANG > 0).sum("SL_TANG");
                    $scope.TongGiam = $scope.ListTheKho.filter(x => x.SL_GIAM > 0).sum("SL_GIAM");
                    $scope.DuCuoiKy = $scope.DuDauKy + $scope.TongTang - $scope.TongGiam;

                }
                else{
                    toastr.error(response.Title);
                }
                console.log(response);
                $scope.$apply();
            }
        });
    };
    $scope.Cancel = ()=>{
        $uibModalInstance.close();
    }
    $scope.viewDetailKhoHD =(item,index)=>{
        $scope.ShowKhoVLContent = false;
        $scope.ShowKhoHDContent = true;
        $scope.selectedRowKhoHD = item.ID_HOP_DONG;
        $scope.SelectedKho =  "Kho: " + item.TEN_HD + "( "+item.TEN_CONG_TY+" )";
        console.log($scope.selectedRowKhoHD);
        GetDSHangHoaKhoHD(item);
    }
    GetDSHangHoaKhoHD = (item)=>{
        $.ajax({
            type:'POST',
            url: '/KiemKeKho/GetDSHangHoaKhoHD',
            data: {idHopDong: item.ID_HOP_DONG},
            success: function (response) {
                if(response.status){
                    $scope.ListHangHoaHD = response.data;
                }
                else{
                    $scope.ListHangHoaHD = [];
                }
                console.log(response);
                $scope.$apply();
            }
        });
    }
    $scope.SelectKhoVL = (index,item) =>{
        $scope.ShowKhoHDContent = false;
        $scope.ShowKhoVLContent = true;
        $scope.selectedRowKhoVL = index;
        $scope.SelectedKho = item.TEN_DON_VI;
        $scope.model.ID_KHO = item.ID;
        GetDSHangHoaKhoVL(item);
        GetDSPhieuGH($scope.model.ID_KHO);
        GetDS_DCP($scope.model.ID_KHO, $scope.SearchDCP);
    };
    GetDSHangHoaKhoVL = (item) =>{
        $.ajax({
            type: 'POST',
            url: '/KiemKeKho/GetDSHangHoa',
            data: {index: item.ID},
            success: function (response) {
                if(response.Status === 200){
                    $scope.ListHangHoa = response.Data;
                }
                else{
                    toastr.error(response.Title);
                }
                $scope.$apply();
            }
        })
    }
    GetDSPhieuGH = (id_kho) => {
        $scope.DSDuHang = [];
        $.ajax({
            type: 'POST',
            url: '/KiemKeKho/dsPhieuGH',
            data: { id_kho: id_kho },
            success: function (response) {
      
                if (response.Error == false) {
                    $scope.DSDuHang = response.data;
                }
                else {
                    toastr.error(response.Title);
                }
                $scope.$apply();
            }
        })
    }
    GetDS_DCP = (id_kho, search) => {
        $scope.DSDungCP = [];
        $.ajax({
            type: 'POST',
            url: '/KiemKeKho/dsDCP',
            data: { id_kho: id_kho, search: search},
            success: function (response) {

                if (response.Error == false) {
                    $scope.DSDungCP = response.data;
                }
                else {
                    toastr.error(response.Title);
                }
                $scope.$apply();
            }
        })
    }
    GetDSPhieuGHCT = (id_ch) => {
        $scope.DSSanPhamNhan = [];
        $.ajax({
            type: 'POST',
            url: '/KiemKeKho/dsPhieuGHCT',
            data: { id_ch: id_ch },
            success: function (response) {
                if (response.Error === false) {
                    $scope.DSSanPhamNhan = response.data;
                }
                else {
                    toastr.error(response.Title);
                }
                $scope.$apply();
            }
        })
    }


    $scope.SPN_DSSanPham = function () {
        
        $scope.AddSPNhan_SanPhamChons = [];
        $scope.AddSPNhan_SanPhams = [];
        for (var i = 0; i < $scope.ListHangHoa.length; i++) {
            var count_hh = $scope.DSSanPhamNhan.filter(x => x.ID_SP == $scope.ListHangHoa[i].ID_HANG);
            if (count_hh.length == 0) {
                var spn = {
                    SELECT: false,
                    ID_SP: $scope.ListHangHoa[i].ID_HANG,
                    NAME: $scope.ListHangHoa[i].NAME,
                    DVT: $scope.ListHangHoa[i].DVT,
                    SO_LUONG_TON: $scope.ListHangHoa[i].SO_LUONG_TON
                };
                $scope.AddSPNhan_SanPhams.push(spn);
            }

        }

            $scope.AddSPNhan_Keyword = "";
            $('#AddCHSPDK_SPDK').modal('show');
    };

    $scope.AddSPNhan_SearchSanPham = function () {
        $scope.AddSPNhan_SanPhams = [];
        var sanPhams = $scope.ListHangHoa.filter(function (x) {
            return (x.NAME != null && x.NAME.indexOf($scope.AddSPNhan_Keyword) != -1);
        });
        if (sanPhams != null && sanPhams.length > 0) {
            $scope.AddSPNhan_SanPhams = sanPhams;
        }
    };

    $scope.AddSPNhan_ChangeCheckAllTP = function () {
        if ($scope.AddSPNhan_CheckAllTP == true) {
            $scope.AddSPNhan_CheckAllTP = false;
            for (var i = 0; i < $scope.AddSPNhan_SanPhams.length; i++) {
                $scope.AddSPNhan_SanPhams[i].SELECT = false;
                // thêm vào danh sách sản phẩm
                $scope.AddSPNhan_SanPhamChons.push($scope.AddSPNhan_SanPhams[i]);
            }
            $scope.AddSPNhan_SanPhams = [];
        }

    };
    // Chọn trang phục nhận
    $scope.AddSPNhan_CheckedTrangPhuc = function (index) {
        if ($scope.AddSPNhan_SanPhams[index].SELECT == true) {
            $scope.AddSPNhan_SanPhams[index].SELECT = false;
            // thêm vào danh sách chọn
            $scope.AddSPNhan_SanPhamChons.push($scope.AddSPNhan_SanPhams[index]);

            // xóa khỏi danh sách sản phẩm
            $scope.AddSPNhan_SanPhams.splice(index, 1);
        }
    };
    // Bỏ Chọn tất cả sản phẩm để add vào sp nhận
    $scope.AddSPNhan_ChangeCheckAllTPChon = function () {
        if ($scope.AddSPNhan_CheckAllTPChon == true) {
            $scope.AddSPNhan_CheckAllTPChon = false;
            for (var i = 0; i < $scope.AddSPNhan_SanPhamChons.length; i++) {
                $scope.AddSPNhan_SanPhamChons[i].SELECT = false;
                // thêm vào danh sách sản phẩm
                $scope.AddSPNhan_SanPhams.push($scope.AddSPNhan_SanPhamChons[i]);
            }
            $scope.AddSPNhan_SanPhamChons = [];
        }

    };
    // Bỏ Chọn trang phục nhận
    $scope.AddSPNhan_CheckedTrangPhucChon = function (index) {
        if ($scope.AddSPNhan_SanPhamChons[index].SELECT == true) {
            $scope.AddSPNhan_SanPhamChons[index].SELECT = false;
            // thêm vào danh sách chọn
            $scope.AddSPNhan_SanPhams.push($scope.AddSPNhan_SanPhamChons[index]);

            // xóa khỏi danh sách sản phẩm
            $scope.AddSPNhan_SanPhamChons.splice(index, 1);
        }
    };
    $scope.AddSPNhan_Luu = function () {
        for (var i = 0; i < $scope.AddSPNhan_SanPhamChons.length; i++) {
            
            $scope.DSSanPhamNhan.push($scope.AddSPNhan_SanPhamChons[i]);

        }
        $('#AddCHSPDK_SPDK').modal('hide');
    };

    $scope.ThemGiuHang = function () {
        $scope.Title = 'Thêm mới phiếu giữ hàng trong kho';
        $scope.model.ID = '';
        $scope.model.SO_PHIEU = '';
        $scope.model.LY_DO = '';
        $scope.DSSanPhamNhan = [];
        $('#modelThem').modal('show');
    }
    $scope.ChiTiet = function (item) {
        $scope.Title = 'Sửa phiếu giữ hàng trong kho';
        $scope.model.ID = item.ID;
        $scope.model.SO_PHIEU = item.SO_PHIEU;
        $scope.model.LY_DO = item.LY_DO;
        $scope.DSSanPhamNhan = [];
        GetDSPhieuGHCT($scope.model.ID);
        $('#modelThem').modal('show');
    }
    $scope.HuySP = function () {
        if ($scope.model.ID == '') {
            $scope.model.ID = '';
            $scope.model.SO_PHIEU = '';
            $scope.model.LY_DO = '';
            $scope.DSSanPhamNhan = [];
        } else {
            GetDSPhieuGHCT($scope.model.ID);
        }
    }
    $scope.HuyDCP = function () {
        GetDS_DCP($scope.model.ID_KHO, $scope.SearchDCP);
    }
   
    $scope.XoaDCP = function () {
        var lisXoa = []; 
        // Kiểm tra xem đã chọn sản phẩm cần xóa chưa
        var countDelete = 0; 
        for (var i = 0; i < $scope.DSDungCP.length; i++) {
            if ($scope.DSDungCP[i].CHECKEDDCP == true) {
                countDelete++;
                var spn = {
                    ID: $scope.DSDungCP[i].ID
                };
                lisXoa.push(spn)
            }

        }
        console.log(lisXoa);
        if (countDelete != null && countDelete > 0) {
            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn xóa sản phẩm đã chọn?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Có',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            //showToast();
                            $.ajax({
                                type: 'post',
                                url: '/KiemKeKho/XoaDCP',
                                data: {
                                    lisXoa: lisXoa
                                },
                                success: function (data) {
                                    //hideLoading();
                                    if (data.Error) {
                                        toastr.error(data.Title);
                                    }
                                    else {
                                        GetDS_DCP($scope.model.ID_KHO, $scope.SearchDCP);
                                        toastr.success(data.Title);
                                    }
                                }
                            });
                        }
                    },
                    close: {
                        text: 'Không',
                        action: function (scope, button) {

                        }
                    }
                }
            });
        }
        else {
            toastr.error("Chưa có sản phẩm nào được chọn!");
        }

    };
    $scope.XoaGiuHang = function () {
        var lisXoa = [];
        // Kiểm tra xem đã chọn sản phẩm cần xóa chưa
        var countDelete = 0;
        for (var i = 0; i < $scope.DSDuHang.length; i++) {
            if ($scope.DSDuHang[i].CHECKEDKH == true) {
                countDelete++;
                var spn = {
                    ID: $scope.DSDuHang[i].ID
                };
                lisXoa.push(spn)
            }

        }

        if (countDelete != null && countDelete > 0) {
            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn xóa sản phẩm đã chọn?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Có',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            showToast();
                            $.ajax({
                                type: 'post',
                                url: '/KiemKeKho/XoaCH_GIU',
                                data: {
                                    lisXoa: lisXoa
                                },
                                success: function (data) {
                                    hideLoading();
                                    if (data.Error) {
                                        toastr.error(data.Title);
                                    }
                                    else {
                                        GetDSPhieuGH($scope.model.ID_KHO);
                                        toastr.success(data.Title);
                                    }
                                }
                            });
                        }
                    },
                    close: {
                        text: 'Không',
                        action: function (scope, button) {

                        }
                    }
                }
            });
        }
        else {
            toastr.error("Chưa có sản phẩm nào được chọn!");
        }

    };
    $scope.XoaSP = function () {
        // Kiểm tra xem đã chọn sản phẩm cần xóa chưa
        var countDelete = 0;
        for (var i = 0; i < $scope.DSSanPhamNhan.length; i++) {
            if ($scope.DSSanPhamNhan[i].SELECT == true) {
                countDelete++;
            }
           
        }

        if (countDelete != null && countDelete > 0) {
            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn xóa sản phẩm đã chọn?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Có',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            
                            for (var i = 0; i < $scope.DSSanPhamNhan.length; i++) {
                                if ($scope.DSSanPhamNhan[i].SELECT == true) {
                                    $scope.DSSanPhamNhan.splice(i, 1);
                                    i--;
                                }
                                   
                            }
                            $scope.$apply();
                        }
                    },
                    close: {
                        text: 'Không',
                        action: function (scope, button) {

                        }
                    }
                }
            });
        }
        else {
            toastr.error("Chưa có sản phẩm nào được chọn!");
        }

    };

    // thêm mới sản phẩm nhận
    $scope.ThemSP = function () {
            var spn = {
                SELECT: false,
                ID_SP: null,
                DVT: '',
                SO_LUONG_TON: 0,
                SO_LUONG: 0
            };
        $scope.DSSanPhamNhan.push(spn);
        
    };
    $scope.ThemDCP = function () {
        var spn = {
            ID: 0,
            CHECKEDDCP: false,
            ID_SP: '',
            MA_H55: ''
        };
        $scope.DSDungCP.push(spn);

    };
    $scope.changeMatHang = function (index, id) {
        var count_hh = $scope.DSSanPhamNhan.filter(x => x.ID_SP == id);
        if (count_hh.length > 1) {
            $scope.DSSanPhamNhan[index].ID_SP = null;
            toastr.error('Mặt hàng đã được chọn');
        } else {
            var hang = $scope.ListHangHoa.filter(x => x.ID_HANG == id);
            if (hang.length > 0) {
                $scope.DSSanPhamNhan[index].DVT = hang[0].DVT;
                $scope.DSSanPhamNhan[index].SO_LUONG_TON = hang[0].SO_LUONG_TON;
            }
        }
       
        
    }
    $scope.changeHangDCP = function (index, id) {

        var count_hh = $scope.DSDungCP.filter(x => x.ID_SP == id);
        if (count_hh.length > 1) {
            $scope.DSDungCP[index].ID_SP = null;
            toastr.error('Mặt hàng đã được chọn');
        } else {
            var hang = $scope.ListHangHoa.filter(x => x.ID_HANG == id);
            if (hang.length > 0) {
                $scope.DSDungCP[index].MA_H55 = hang[0].CODE;
                $scope.DSDungCP[index].SUA = true;
            }
        }
    }
    $scope.LuuSP = function () {

        if ($scope.model.SO_PHIEU == '' || $scope.model.SO_PHIEU == undefined || $scope.model.SO_PHIEU == null) {
            toastr.error('Chưa nhập Số phiếu');
            return;
        }
        var sp = true;
        var sl = true;
        for (var i = 0; i < $scope.DSSanPhamNhan.length; i++) {
            if ($scope.DSSanPhamNhan[i].ID_SP == '' || $scope.DSSanPhamNhan[i].ID_SP == undefined || $scope.DSSanPhamNhan[i].ID_SP == null) {
                sp = false;
            }
            if ($scope.DSSanPhamNhan[i].SO_LUONG <= 0 || $scope.DSSanPhamNhan[i].SO_LUONG > $scope.DSSanPhamNhan[i].SO_LUONG_TON || $scope.DSSanPhamNhan[i].SO_LUONG == undefined) {
                sl = false;
            }

        }
        if (sp == false) {
            toastr.error('Chưa nhập Sản phẩm');
            return;
        }
        if (sl == false) {
            toastr.error('Số lượng phải lớn hơn 0 và nhỏ hơn hoặc bằng Số lượng tồn.');
            return;
        }
            showToast();
            $.ajax({
                type: 'post',
                url: '/KiemKeKho/LuuCH_GIU',
                data: {
                    sanPhamNhans: $scope.DSSanPhamNhan,
                    model: $scope.model
                },
                success: function (data) {
                    hideLoading();
                    if (data.Error) {
                        toastr.error(data.Title);
                    }
                    else {
                        $('#modelThem').modal('hide');
                        GetDSPhieuGH($scope.model.ID_KHO);
                        toastr.success(data.Title);
                    }
                }
            });
      
    };

    $scope.LuuDCP = function () {
        var them_sua = [];
        var sp = true;
        for (var i = 0; i < $scope.DSDungCP.length; i++) {
            if ($scope.DSDungCP[i].ID == 0 || $scope.DSDungCP[i].SUA == true){
                if ($scope.DSDungCP[i].ID_SP == '' || $scope.DSDungCP[i].ID_SP == undefined) {
                    sp = false;
                } else {
                    them_sua.push($scope.DSDungCP[i]);
                }
            }
            
        }
        console.log($scope.DSDungCP);
        console.log(them_sua);
        console.log(sp);
        if (sp == false) {
            toastr.error('Chưa chọn Sản phẩm');
            return;
        }
        
        showToast();
        $.ajax({
            type: 'post',
            url: '/KiemKeKho/LuuDCP',
            data: {
                sanPhamNhans: them_sua,
                id_kho: $scope.model.ID_KHO
            },
            success: function (data) {
                hideLoading();
                if (data.Error) {
                    toastr.error(data.Title);
                }
                else {
                    $('#modelThem').modal('hide');
                    GetDS_DCP($scope.model.ID_KHO, $scope.SearchDCP);
                    toastr.success(data.Title);
                }
            }
        });

    };

    $scope.TimKiemDCP = function () {
        GetDS_DCP($scope.model.ID_KHO, $scope.SearchDCP);
    }
    $scope.toggleAllSP = function () {

        var toggleStatus = $scope.isAllSelectedSP;
        angular.forEach($scope.DSSanPhamNhan, function (itm) { itm.SELECT = toggleStatus; });

    }
    $scope.optionToggledSP = function () {
        $scope.isAllSelectedSP = $scope.DSSanPhamNhan.every(function (itm) { return itm.SELECT; })

    }
    $scope.toggleAllKH = function () {

        var toggleStatus = $scope.isAllSelectedKH;
        angular.forEach($scope.DSDuHang, function (itm) { itm.CHECKEDKH = toggleStatus; });

    }
    $scope.optionToggledKH = function () {
        $scope.isAllSelectedKH = $scope.DSDuHang.every(function (itm) { return itm.CHECKEDKH; })

    }

    $scope.toggleAllDCP = function () {

        var toggleStatus = $scope.isAllSelectedDCP;
        angular.forEach($scope.DSDungCP, function (itm) { itm.CHECKEDDCP = toggleStatus; });

    }
    $scope.optionToggledDCP = function () {
        $scope.isAllSelectedDCP = $scope.DSDungCP.every(function (itm) { return itm.CHECKEDDCP; })

    }
})