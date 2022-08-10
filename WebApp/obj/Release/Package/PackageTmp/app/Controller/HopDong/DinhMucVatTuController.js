app.controller("DinhMucVatTuController", function ($scope, $ngConfirm, showToast, hideLoading, $filter) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "ROLE_TYPE DESC";
    $scope.ListRole = [];
    $scope.AddVatTu_VatTuChinhChons = [];
    $scope.AddVatTu_VatTuPhuChons = [];
    $scope.AddVatTu_VatTus = [];
    $scope.VatTuPhus = [];
    $scope.VatTuChinhs = [];

    $scope.SanPhamId = 0;

    $scope.disabledRollbackData = true;
    $scope.disabledSaveChange = true;
    $scope.dateUpdate = "";
    angular.element(document).ready(function () {
        showToast();

        GetBottomAction();
        GetDanhMuc();
        //$scope.LoadPage();
    });

    $scope.ListSanPhamChon = [];

    $scope.ShowContent = false;

    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDinhMucByNam = false;
    $scope.RoleBtnCreateNDM = false;
    $scope.RoleBtnAddSP = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnCreate = false;
    $scope.RoleBtnRollback = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnDeleteVT = false;
    $scope.ListYear = [];
    $scope.ListYearBH = [];
    $scope.ListSanPham = [];
    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/DinhMucVatTu/GetBottomAction',
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnSearch') {
                            $scope.RoleBtnSearch = true;
                        }
                        if (item == 'btnDinhMucByNam') {
                            $scope.RoleBtnDinhMucByNam = true;
                        }
                        if (item == 'btnCreateNDM') {
                            $scope.RoleBtnCreateNDM = true;
                        }
                        if (item == 'btnAddSP') {
                            $scope.RoleBtnAddSP = true;
                        }
                        if (item == 'btnDelete') {
                            $scope.RoleBtnDelete = true;
                        }
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'btnRollback') {
                            $scope.RoleBtnRollback = true;
                        }
                        if (item == 'btnSave') {
                            $scope.RoleBtnSave = true;
                        }
                        if (item == 'btnDeleteVT') {
                            $scope.RoleBtnDeleteVT = true;
                        }
                    });
                }
                var currentdate = new Date();
                $scope.ListYear = response.ListYear;
                $scope.NamBanHanh = '' + currentdate.getFullYear();
                $scope.ListYearBH = response.ListYearBH;
                $scope.NamBanHanhTheoNT = response.ListYearBH[0].Value;
                $scope.$apply();
            }
        });
    }

    $scope.disabledRollbackData = true;
    $scope.disabledSaveChange = true;

    $scope.pageChanged = function () {
        $scope.LoadPage();
    };

    $scope.LoadPage = function () {
        $.ajax({
            type: 'post',
            url: '/DinhMucVatTu/GetAllByPage',
            data: $scope.modelSearch,
            success: function (data) {
                $scope.modelSearch.totalItems = data.totalItems;
                $scope.ListRole = data.data;
                $scope.modelSearch.pageSize = data.pageSize;
                $scope.$apply();
                hideLoading();
            }
        });
    };


    var setting = {
        check: {
            enable: false
        },
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "pId",
                rootPId: ""
            }
        },
        callback: {
            onClick: onClickSanPham
        }
    };

    $scope.NhomDinhMucId = 0;

    function onClickSanPham(event, treeId, treeNode, clickFlag) {
        $scope.SanPhamId = 0;
        // Lấy danh sách tiêu chuẩn của nhóm tiêu chuẩn đã chọn
        if (treeNode != null ) {
            if (treeNode.name_control == 'SP') {
                
                $scope.SanPhamId = parseInt(treeNode.pace_id);
            }
        }
        $scope.ShowContent = true;
        $scope.NhomDinhMucId = parseInt(treeNode.ContactName);
        $scope.$apply();
        GetDanhSachVatTuCuaSP();
    }

    var treeLucLuong;

    var treeLucLuongSource = [];
    $scope.ListSanPham = [];
    function GetDanhMuc() {
        $scope.VatTuPhus = [];
        $scope.VatTuChinhs = [];

        $scope.ListSanPham = [];
        $.ajax({
            type: 'post',
            url: '/DinhMucVatTu/GetDanhMuc',
            data: {},
            success: function (data) {
                hideLoading();

                $scope.VatTuPhus = data.VatTuPhus;
                $scope.VatTuChinhs = data.VatTuChinhs;
                $scope.ListSanPham = data.TreeTCData;

                $.fn.zTree.init($("#treeRole"), setting, $scope.ListSanPham);
                var zTree = $.fn.zTree.getZTreeObj("treeRole");
                var type = { "Y": "ps", "N": "ps" };
                zTree.setting.check.chkboxType = type;


            }
        });
    }

    // tìm kiếm sản phẩm
    $scope.ChangeSanPham = function () {
        $scope.ListSanPham = [];
        $scope.SanPham_VatTuChinhs = [];
        $scope.SanPham_VatTuPhus = [];
        $scope.disabledSaveChange = true;
        $scope.disabledRollbackData = true;
        $.ajax({
            type: 'post',
            url: '/DinhMucVatTu/SearchTreeSanPham',
            data: { keyword: $scope.SearchTreeSanPhamText, nam: $scope.NamBanHanh },
            success: function (data) {

                $scope.SanPhamId = 0;
                $scope.ListSanPham = data.TreeTCData;

                $.fn.zTree.init($("#treeRole"), setting, $scope.ListSanPham);
                var zTree = $.fn.zTree.getZTreeObj("treeRole");
                var type = { "Y": "ps", "N": "ps" };
                zTree.setting.check.chkboxType = type;

            }
        });
    };



    ///////////////////////// Lấy danh sách sản phẩm bên trái ///////////////////////////////////
    $scope.SanPham_VatTuPhus = [];
    $scope.SanPham_VatTuChinhs = [];
    function GetDanhSachVatTuCuaSP() {
        if ($scope.NhomDinhMucId > 0) {
            $scope.SanPham_VatTuPhus = [];
            $scope.SanPham_VatTuChinhs = [];
            showToast();
            $.ajax({
                type: 'post',
                url: '/DinhMucVatTu/GetVatTu',
                data: {
                    sanPhamId: $scope.SanPhamId,
                    nhomDinhMucId: $scope.NhomDinhMucId,
                    nam: $scope.NamBanHanh,
                    keyword: $scope.SearchVatTuText
                },
                success: function (data) {
                    hideLoading();
                    if (data.VatTuPhus != null && data.VatTuPhus.length > 0) {
                        $scope.SanPham_VatTuPhus = data.VatTuPhus;
                    }
                    if (data.VatTuChinhs != null && data.VatTuChinhs.length > 0) {
                        $scope.SanPham_VatTuChinhs = data.VatTuChinhs;
                    }
                    $scope.$apply();
                }
            });
        }
    }


    $scope.SearchVatTu = function () {
        GetDanhSachVatTuCuaSP();
    }

    /////////////////////////// End Lấy danh sách sản phẩm bên trái///////////////////////////////

    ////////////////////////// Thêm vật tư //////////////////////////////////////////////////
    // Show popup thêm vật tư
    $scope.AddVatTu = function () {
        $scope.AddVatTu_VatTus = [];
        $scope.AddVatTu_VatTuChinhChons = [];
        $scope.AddVatTu_VatTuPhuChons = [];

        $scope.AddVatTu_LoaiVT = 'Y';
        if ($scope.NhomDinhMucId > 0) {
            GetAllVatTu();
            $("#AddVatTu").modal("show");
        }
    }
    $scope.AllVatTus = [];
    function GetAllVatTu() {
        if ($scope.NhomDinhMucId > 0) {
            $scope.AddVatTu_VatTus = [];
            $scope.AllVatTus = [];
            showToast();
            $.ajax({
                type: 'post',
                url: '/DinhMucVatTu/GetAllVatTu',
                data: {
                    sanPhamId: $scope.SanPhamId,
                    nhomDinhMucId: $scope.NhomDinhMucId,
                    nam: $scope.NamBanHanh,
                    keyword: $scope.AddVatTu_Keyword,
                    loaiVT: $scope.AddVatTu_LoaiVT
                },
                success: function (data) {
                    hideLoading();
                    if (data.AllVatTuPhus != null && data.AllVatTuPhus.length > 0) {
                        $scope.AllVatTus = data.AllVatTuPhus;
                    }

                    if (data.AllVatTuPhus != null && data.AllVatTuPhus.length > 0) {
                        $scope.AddVatTu_VatTus = data.AllVatTuPhus;
                    }

                    var vattus = [];

                    if ($scope.AddVatTu_VatTus != null && $scope.AddVatTu_VatTus.length > 0) {
                        for (var i = 0; i < $scope.AddVatTu_VatTus.length; i++) {

                            if ($scope.AddVatTu_LoaiVT == 'Y') {
                                // Kiểm tra xem vật tư đã tồn ại chua
                                // lấy danh sách vật tư chính cần xóa
                                var deleteVTCs = $scope.AddVatTu_VatTuChinhChons.filter(function (x) {
                                    return (x.ID == $scope.AddVatTu_VatTus[i].ID);
                                });

                                if (deleteVTCs == null || deleteVTCs.length == 0) {
                                    vattus.push($scope.AddVatTu_VatTus[i]);
                                }
                            } else {

                                // lấy danh sách vật tư phụ cần xóa
                                var deleteVTPs = $scope.AddVatTu_VatTuPhuChons.filter(function (x) {
                                    return (x.ID == $scope.AddVatTu_VatTus[i].ID);
                                });
                                if (deleteVTPs == null || deleteVTPs.length == 0) {
                                    vattus.push($scope.AddVatTu_VatTus[i]);
                                }
                            }

                        }
                    }
                    $scope.AddVatTu_VatTus = vattus;

                    $scope.$apply();
                }
            });
        }
    }

    // Thay đổi loại VT
    $scope.AddVatTu_LoaiVatTu = function () {
        if ($scope.NhomDinhMucId > 0) {
            GetAllVatTu();
        }
    }

    // Tìm kiếm VT
    $scope.AddVatTu_SearchVatTu = function () {
        if ($scope.NhomDinhMucId > 0) {
            GetAllVatTu();
        }
    }

    $scope.AddVatTu_VatTuChinhChons = [];
    $scope.AddVatTu_VatTuPhuChons = [];

    // Check all vât tư
    $scope.AddVatTu_ChangeCheckAllTP = function () {
        if ($scope.AddVatTu_CheckAllTP == true) {
            $scope.AddVatTu_CheckAllTP = false;
            for (var i = 0; i < $scope.AddVatTu_VatTus.length; i++) {
                $scope.AddVatTu_VatTus[i].SELECT = false;
                // thêm vào danh sách sản phẩm
                if ($scope.AddVatTu_LoaiVT == 'Y') {
                    $scope.AddVatTu_VatTuChinhChons.push($scope.AddVatTu_VatTus[i]);

                } else {
                    $scope.AddVatTu_VatTuPhuChons.push($scope.AddVatTu_VatTus[i]);

                }
            }
            $scope.AddVatTu_VatTus = [];
        }
    }

    // Check vât tư
    $scope.AddVatTu_CheckedVatTu = function (index) {
        $scope.AddVatTu_CheckAllTP = false;
        if ($scope.AddVatTu_VatTus[index].SELECT == true) {
            $scope.AddVatTu_VatTus[index].SELECT = false;
            // thêm vào danh sách sản phẩm
            if ($scope.AddVatTu_LoaiVT == 'Y') {
                $scope.AddVatTu_VatTuChinhChons.push($scope.AddVatTu_VatTus[index]);

            } else {
                $scope.AddVatTu_VatTuPhuChons.push($scope.AddVatTu_VatTus[index]);

            }
            // xóa khỏi danh sách sản phẩm chọn
            $scope.AddVatTu_VatTus.splice(index, 1);
        }
    };

    // Bỏ Check all vât tư
    $scope.AddVatTu_ChangeCheckAllTPChon = function (type) {
        if ($scope.AddVatTu_CheckAllTPChinhChon == true || $scope.AddVatTu_CheckAllTPPhuChon == true) {
            $scope.AddVatTu_CheckAllTPChinhChon = false;
            $scope.AddVatTu_CheckAllTPPhuChon = false;
            if (type == 1) {
                if ($scope.AddVatTu_LoaiVT == 'Y') {
                    for (var i = 0; i < $scope.AddVatTu_VatTuChinhChons.length; i++) {
                        $scope.AddVatTu_VatTuChinhChons[i].SELECT = false;
                        // thêm vào danh sách sản phẩm
                        $scope.AddVatTu_VatTus.push($scope.AddVatTu_VatTuChinhChons[i]);
                    }
                }
                $scope.AddVatTu_VatTuChinhChons = [];
            } else {
                if ($scope.AddVatTu_LoaiVT != 'Y') {
                    for (var i = 0; i < $scope.AddVatTu_VatTuPhuChons.length; i++) {
                        $scope.AddVatTu_VatTuPhuChons[i].SELECT = false;
                        // thêm vào danh sách sản phẩm
                        $scope.AddVatTu_VatTus.push($scope.AddVatTu_VatTuPhuChons[i]);
                    }
                }

                $scope.AddVatTu_VatTuPhuChons = [];
            }
        }
    }

    // Bỏ Check vât tư
    $scope.AddVatTu_CheckedVatTuChon = function (index, type) {
        $scope.AddVatTu_CheckAllTPChinhChon = false;
        $scope.AddVatTu_CheckAllTPPhuChon = false;

        if (type == 1) {
            if ($scope.AddVatTu_VatTuChinhChons[index].SELECT == true) {
                if ($scope.AddVatTu_LoaiVT == 'Y') {
                    $scope.AddVatTu_VatTuChinhChons[index].SELECT = false;
                    // thêm vào danh sách sản phẩm
                    $scope.AddVatTu_VatTus.push($scope.AddVatTu_VatTuChinhChons[index]);
                }

                // xóa khỏi danh sách sản phẩm chọn
                $scope.AddVatTu_VatTuChinhChons.splice(index, 1);
            }
        } else {
            if ($scope.AddVatTu_VatTuPhuChons[index].SELECT == true) {
                if ($scope.AddVatTu_LoaiVT != 'Y') {
                    $scope.AddVatTu_VatTuPhuChons[index].SELECT = false;
                    // thêm vào danh sách sản phẩm
                    $scope.AddVatTu_VatTus.push($scope.AddVatTu_VatTuPhuChons[index]);
                }

                // xóa khỏi danh sách sản phẩm chọn
                $scope.AddVatTu_VatTuPhuChons.splice(index, 1);
            }
        }
        $scope.AddVatTu_VatTus = $filter('orderBy')($scope.AddVatTu_VatTus, 'TEN_SP', true); 
    };

    // Thêm vật tư
    $scope.AddVatTu_ThemVT = function () {

        var dsVatTuChinhs = [];
        var dsVatTuPhus = [];

        // thêm vật tư chính
        if ($scope.AddVatTu_VatTuChinhChons != null && $scope.AddVatTu_VatTuChinhChons.length > 0) {
            for (var i = 0; i < $scope.AddVatTu_VatTuChinhChons.length; i++) {

                // Kiểm tra xem vật tư đã tồn ại chua
                // lấy danh sách vật tư chính cần xóa
                var deleteVTCs = $scope.SanPham_VatTuChinhs.filter(function (x) {
                    return (x.SP_ID == $scope.AddVatTu_VatTuChinhChons[i].ID);
                });

                if (deleteVTCs == null || deleteVTCs.length == 0) {
                    var vt = {
                        ID: 0,
                        DINH_MUC_ID: 0,
                        DINH_MUC_SP_ID: 0,
                        SP_ID: $scope.AddVatTu_VatTuChinhChons[i].ID,
                        DVT_ID: $scope.AddVatTu_VatTuChinhChons[i].DVT_ID,
                        TEN_DVT: $scope.AddVatTu_VatTuChinhChons[i].TEN_DVT,
                        SO_LUONG: 1,
                        IS_VAT_TU_CHINH: 'Y',
                        TEN_VAT_TU: $scope.AddVatTu_VatTuChinhChons[i].TEN_SP,
                    };
                    dsVatTuChinhs.push(vt);
                }
            }
        }

        // thêm vật tu phụ
        if ($scope.AddVatTu_VatTuPhuChons != null && $scope.AddVatTu_VatTuPhuChons.length > 0) {
            for (var i = 0; i < $scope.AddVatTu_VatTuPhuChons.length; i++) {

                // lấy danh sách vật tư phụ cần xóa
                var deleteVTPs = $scope.SanPham_VatTuPhus.filter(function (x) {
                    return (x.SP_ID == $scope.AddVatTu_VatTuPhuChons[i].ID);
                });
                if (deleteVTPs == null || deleteVTPs.length == 0) {
                    var vt = {
                        ID: 0,
                        DINH_MUC_ID: 0,
                        DINH_MUC_SP_ID: 0,
                        SP_ID: $scope.AddVatTu_VatTuPhuChons[i].ID,
                        DVT_ID: $scope.AddVatTu_VatTuPhuChons[i].DVT_ID,
                        TEN_DVT: $scope.AddVatTu_VatTuPhuChons[i].TEN_DVT,
                        SO_LUONG: 1,
                        IS_VAT_TU_CHINH: 'N',
                        TEN_VAT_TU: $scope.AddVatTu_VatTuPhuChons[i].TEN_SP,
                    };

                    dsVatTuPhus.push(vt);
                }
            }
        }
        if ((dsVatTuChinhs == null || dsVatTuChinhs.length == 0) && (dsVatTuPhus == null || dsVatTuPhus.length == 0)) {
            toastr.error("Chưa chọn vật tư hoặc các vật tư đã chọn đã được thêm từ trước. Vui lòng chọn vật tư!");
        }
        else {
            // Thêm mới vật tư
            showToast();
            $.ajax({
                type: 'post',
                url: '/DinhMucVatTu/AddVatTu',
                data: {
                    nam: $scope.NamBanHanh,
                    sanPhamId: $scope.SanPhamId,
                    vatTuChinhs: dsVatTuChinhs,
                    vatTuPhus: dsVatTuPhus,
                    nhomDinhMucId : $scope.NhomDinhMucId
                },
                success: function (data) {
                    hideLoading();
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        $("#AddVatTu").modal("hide");
                        toastr.success(data.Title);
                        GetDanhSachVatTuCuaSP();
                        $scope.disabledRollbackData = true;
                        $scope.disabledSaveChange = true;
                    }
                }
            });
        }
    };


    ////////////////////////// End Thêm vật tư //////////////////////////////////////////////////

    $scope.Sort = function (event, sortRow) {
        if (event.currentTarget.classList.contains('arrow-up')) {
            $scope.modelSearch.SortColumn = sortRow + " DESC";
            $scope.LoadPage();
        }
        if (event.currentTarget.classList.contains('arrow-down')) {
            $scope.modelSearch.SortColumn = sortRow + " ASC";
            $scope.LoadPage();
        }
    };


    // Lấy lại dữ liệu cũ
    $scope.RollbackVatTu = function () {
        GetDanhSachVatTuCuaSP();
        $scope.disabledRollbackData = true;
        $scope.disabledSaveChange = true;
    }

    $scope.ChangeSoLuong = function () {
        $scope.disabledRollbackData = false;
        $scope.disabledSaveChange = false;
    }



    $scope.DeleteVatTu = function () {

        // lấy danh sách vật tư chính cần xóa
        var deleteVTCs = $scope.SanPham_VatTuChinhs.filter(function (x) {
            return (x.SELECT == true);
        });

        // lấy danh sách vật tư phụ cần xóa
        var deleteVTPs = $scope.SanPham_VatTuPhus.filter(function (x) {
            return (x.SELECT == true);
        });

        if ((deleteVTCs != null && deleteVTCs.length > 0) || (deleteVTPs != null && deleteVTPs.length > 0)) {
            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn xóa vật tư đã chọn?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Xóa',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            //// Xóa vật tu chính
                            //var notdeleteVTCs = $scope.SanPham_VatTuChinhs.filter(function (x) {
                            //    return (x.SELECT != true);
                            //});

                            //if (notdeleteVTCs != null && notdeleteVTCs.length > 0) {
                            //    $scope.SanPham_VatTuChinhs = notdeleteVTCs;
                            //} else {
                            //    $scope.SanPham_VatTuChinhs = [];
                            //}

                            //// Xóa vật tư phụ
                            //var notdeleteVTPs = $scope.SanPham_VatTuPhus.filter(function (x) {
                            //    return (x.SELECT != true);
                            //});

                            //if (notdeleteVTPs != null && notdeleteVTPs.length > 0) {
                            //    $scope.SanPham_VatTuPhus = notdeleteVTPs;
                            //} else {
                            //    $scope.SanPham_VatTuPhus = [];
                            //}
                            //$scope.disabledRollbackData = false;
                            //$scope.disabledSaveChange = false;
                            //$scope.$apply();

                            showToast();
                            $.ajax({
                                type: 'post',
                                url: '/DinhMucVatTu/DeleteVatTu',
                                data: {
                                    nam: $scope.NamBanHanh,
                                    sanPhamId: $scope.SanPhamId,
                                    nhomDinhMucId: $scope.NhomDinhMucId,
                                    vatTuChinhs: deleteVTCs,
                                    vatTuPhus: deleteVTPs
                                },
                                success: function (data) {
                                    hideLoading();
                                    if (data.Error) {
                                        toastr.error(data.Title);
                                    }
                                    else {
                                        $scope.SanPham_VatTuChinhs = [];
                                        $scope.SanPham_VatTuPhus = [];

                                        toastr.success(data.Title);
                                        GetDanhSachVatTuCuaSP();
                                    }
                                }
                            });

                        }
                    },
                    close: {
                        text: 'Hủy',
                        action: function (scope, button) {

                        }
                    }
                }
            });
        }
        else {
            toastr.error("Vui lòng chọn vật tư cần xóa!");
        }
    };

    // Lưu thay đổi
    $scope.SaveVatTu = function () {
        showToast();
        $.ajax({
            type: 'post',
            url: '/DinhMucVatTu/EditVatTu',
            data: {
                nam: $scope.NamBanHanh,
                sanPhamId: $scope.SanPhamId,
                vatTuChinhs: $scope.SanPham_VatTuChinhs,
                vatTuPhus: $scope.SanPham_VatTuPhus,
                nhomDinhMucId: $scope.NhomDinhMucId
            },
            success: function (data) {
                hideLoading();
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    toastr.success(data.Title);
                    GetDanhSachVatTuCuaSP();
                    $scope.disabledRollbackData = true;
                    $scope.disabledSaveChange = true;
                }
            }
        });
    };

    //////////////////////Thêm sản phẩm định mức//////////////////////////
    $scope.SanPham_DinhMuc = [];
    $scope.SanPham_DinhMucAll = [];
    $scope.SanPham_DinhMucChon = [];
    $scope.SanPhamDM_Keyword = '';

    $scope.SanPhamDM_CheckAllTP = false;

    function GetSanPhamAddDM() {
        $scope.SanPham_DinhMuc = [];
        showToast();
        $.ajax({
            type: 'post',
            url: '/DinhMucVatTu/GetSanPhamAdd',
            data: {
                nam: $scope.NamBanHanh
            },
            success: function (data) {
                hideLoading();
                $scope.SanPham_DinhMuc = data.data;
                $scope.SanPham_DinhMucAll = data.data;
                $scope.$apply();
            }
        });
    }

    // Thêm mới định mức
    $scope.AddNewDinhMuc = function () {
        if ($scope.NhomDinhMucId == null || $scope.NhomDinhMucId == 0) {
            toastr.error("Vui lòng chọn nhóm định mức!");
        } else {
            GetSanPhamAddDM();
            $scope.SanPham_DinhMucChon = [];
            $("#AddSPDinhMuc").modal("show");
        }
    }

    // Tìm kiếm SP định mức
    $scope.SanPhamDM_Search = function () {
        var sanPhams = $scope.SanPham_DinhMucAll.filter(function (x) {
            return (x.TEN_SP != null && x.TEN_SP.indexOf($scope.SanPhamDM_Keyword) != -1);
        });
        if (sanPhams != null && sanPhams.length > 0)
            $scope.SanPham_DinhMuc = sanPhams;
        else
            $scope.SanPham_DinhMuc = [];
    }

    // Check all SP định mức
    $scope.SanPhamDM_ChangeCheckAllTP = function () {
        if ($scope.SanPhamDM_CheckAllTP == true) {
            $scope.SanPhamDM_CheckAllTP = false;
            for (var i = 0; i < $scope.SanPham_DinhMuc.length; i++) {
                $scope.SanPham_DinhMuc[i].SELECT = false;
                // thêm vào danh sách sản phẩm
                $scope.SanPham_DinhMucChon.push($scope.SanPham_DinhMuc[i]);
            }
            $scope.SanPham_DinhMuc = [];
        }
    }

    // Check SP định mức
    $scope.SanPhamDM_CheckedTrangPhuc = function (index) {
        $scope.SanPhamDM_CheckAllTP = false;
        if ($scope.SanPham_DinhMuc[index].SELECT == true) {
            $scope.SanPham_DinhMuc[index].SELECT = false;
            // thêm vào danh sách sản phẩm
            $scope.SanPham_DinhMucChon.push($scope.SanPham_DinhMuc[index]);

            // xóa khỏi danh sách sản phẩm chọn
            $scope.SanPham_DinhMuc.splice(index, 1);
        }
    };

    // Bỏ Check all SP định mức
    $scope.SanPhamDM_ChangeCheckAllTPChon = function () {
        if ($scope.SanPhamDM_CheckAllTPChon == true) {
            $scope.SanPhamDM_CheckAllTPChon = false;
            for (var i = 0; i < $scope.SanPham_DinhMucChon.length; i++) {
                $scope.SanPham_DinhMucChon[i].SELECT = false;
                // thêm vào danh sách sản phẩm
                $scope.SanPham_DinhMuc.push($scope.SanPham_DinhMucChon[i]);
            }
            $scope.SanPham_DinhMucChon = [];
        }
    }

    // Bỏ Check SP định mức
    $scope.SanPhamDM_CheckedTrangPhucChon = function (index) {
        $scope.SanPhamDM_CheckAllTPChon = false;
        if ($scope.SanPham_DinhMucChon[index].SELECT == true) {
            $scope.SanPham_DinhMucChon[index].SELECT = false;
            // thêm vào danh sách sản phẩm
            $scope.SanPham_DinhMuc.push($scope.SanPham_DinhMucChon[index]);

            // xóa khỏi danh sách sản phẩm chọn
            $scope.SanPham_DinhMucChon.splice(index, 1);
        }
    };

    // Thêm sp định mức
    $scope.SanPhamDM_ThemSP = function () {
        var spChons = $scope.SanPham_DinhMucChon;

        if (spChons != null && spChons.length > 0) {
            showToast();
            $.ajax({
                type: 'post',
                url: '/DinhMucVatTu/AddSanPhamDM',
                data: {
                    nam: $scope.NamBanHanh,
                    sanPhams: spChons,
                    nhomDinhMucId: $scope.NhomDinhMucId,
                    sanPhamId: $scope.SanPhamId
                },
                success: function (data) {
                    hideLoading();
                    if (data.Error) {
                        toastr.error(data.Title);
                    }
                    else {
                        $('#AddSPDinhMuc').modal('hide');
                        toastr.success(data.Title);
                        $scope.ChangeSanPham();
                    }
                }
            });
        }
        else {
            toastr.error("Vui lòng chọn sản phẩm!");
        }
    };

    ////////////////////////////// End Thêm sản phẩm định mức /////////////////////////////////////


    // Thay đổi năm ban hành
    $scope.ChangeNamBanHanh = function () {
        $scope.ChangeSanPham();
    };

    ////////////////////////////// Xóa sản phẩm định mức /////////////////////////////////////////
    $scope.deleteDinhMuc = function () {
        if ($scope.NhomDinhMucId > 0) {
            showToast();
            $.ajax({
                type: 'post',
                url: '/DinhMucVatTu/DeleteSanPhamDM',
                data: {
                    nam: $scope.NamBanHanh,
                    sanPhamId: $scope.SanPhamId,
                    nhomDinhMucId: $scope.NhomDinhMucId
                },
                success: function (data) {
                    hideLoading();
                    if (data.Error) {
                        toastr.error(data.Title);
                    }
                    else {
                        $scope.SanPham_VatTuChinhs = [];
                        $scope.SanPham_VatTuPhus = [];

                        toastr.success(data.Title);
                        $scope.ChangeSanPham();
                    }
                }
            });
        }
        else {
            toastr.error("Vui lòng chọn sản phẩm cần xóa!");
        }
    };
    ////////////////////////////// End Xóa sản phẩm định mức /////////////////////////////////////

    ////////////////////////// Tích chọn vật tư chính //////////////////////////////////////////////////

    $scope.VTChinh_CheckAll = false;

    // Check all vât tư
    $scope.VTChinh_ChangeCheckAll = function () {
        for (var i = 0; i < $scope.SanPham_VatTuChinhs.length; i++) {
            $scope.SanPham_VatTuChinhs[i].SELECT = $scope.VTChinh_CheckAll;
        }
    }

    // Check vât tư
    $scope.VTChinh_CheckVatTu = function (index) {
        $scope.VTChinh_CheckAll = false;
        var sanPhams = $scope.SanPham_VatTuChinhs.filter(function (x) {
            return (x.SELECT == false);
        });

        if (sanPhams == null || sanPhams.length == 0) {
            $scope.VTChinh_CheckAll = true;
        }
    };

    ////////////////////////// End Tích chọn vật tư chính //////////////////////////////////////////////////

    ////////////////////////// Tích chọn vật tư phụ //////////////////////////////////////////////////

    $scope.VTPhu_CheckAll = false;

    // Check all vât tư
    $scope.VTPhu_ChangeCheckAll = function () {
        for (var i = 0; i < $scope.SanPham_VatTuPhus.length; i++) {
            $scope.SanPham_VatTuPhus[i].SELECT = $scope.VTPhu_CheckAll;
        }
    }

    // Check vât tư
    $scope.VTPhu_CheckVatTu = function (index) {
        $scope.VTPhu_CheckAll = false;
        var sanPhams = $scope.SanPham_VatTuPhus.filter(function (x) {
            return (x.SELECT == false);
        });

        if (sanPhams == null || sanPhams.length == 0) {
            $scope.VTPhu_CheckAll = true;
        }
    };

    ////////////////////////// End Tích chọn vật tư phụ //////////////////////////////////////////////////


    ////////////////////// Định mức vật tư theo năm trước /////////////////////////////////////////////////
    // Show popup
    $scope.CoppyDinhMucTheoNamTruoc = function (index) {
        $('#CoppyDinhMucNamTruoc').modal('show');
    };

    // Coppy dữ liệu
    $scope.CoppyDMNamTruoc_CoppyDuLieu = function (index) {
        if ($scope.NamBanHanhTheoNT == null || $scope.NamBanHanhTheoNT == 0) {
            toastr.error("Vui lòng chọn năm ban hành.");
        } else {
            showToast();
            $.ajax({
                type: 'post',
                url: '/DinhMucVatTu/CoppyDinhMucNamTruoc',
                data: {
                    nam: $scope.NamBanHanhTheoNT
                },
                success: function (data) {
                    hideLoading();
                    if (data.Error) {
                        toastr.error(data.Title);
                    }
                    else {
                        $('#CoppyDinhMucNamTruoc').modal('hide');
                        toastr.success(data.Title);
                        $scope.ChangeSanPham();
                    }
                }
            });
        }

    };


    ////////////////////// End Định mức vật tư theo năm trước /////////////////////////////////////////////////

    //////////////////////////// Thêm nhóm định mức //////////////////////////////////////////////////////////
    $scope.AddNhomDM_TenNhomDM = "";
    $scope.ShowPopupThemNhomDM = function () {
        $('#AddNhomDinhMuc').modal('show');
    };

    // Thêm nhóm định mức
    $scope.AddNhomDM_ThemNhomDM = function () {
        if ($scope.AddNhomDM_TenNhomDM == null || $scope.AddNhomDM_TenNhomDM == '') {
            toastr.error("Vui lòng nhập tên nhóm định mức.");
        } else {
            showToast();
            $.ajax({
                type: 'post',
                url: '/DinhMucVatTu/ThemNhomDinhMuc',
                data: {
                    nam: $scope.NamBanHanh,
                    tenNhomDM: $scope.AddNhomDM_TenNhomDM
                },
                success: function (data) {
                    hideLoading();
                    if (data.Error) {
                        toastr.error(data.Title);
                    }
                    else {
                        $scope.AddNhomDM_TenNhomDM = "";
                        $('#AddNhomDinhMuc').modal('hide');
                        toastr.success(data.Title);
                        $scope.ChangeSanPham();
                    }
                }
            });
        }

    };
    /////////////////////////////// End Thêm nhóm định mức ////////////////////////////////////////////////////


});
