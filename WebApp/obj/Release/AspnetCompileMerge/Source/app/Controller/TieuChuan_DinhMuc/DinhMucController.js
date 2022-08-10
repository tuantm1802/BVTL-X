app.controller("DinhMucController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "ROLE_TYPE DESC";

    $scope.TC_MaDoiTieuChuan = "";
    $scope.DSTieuChuan = [];
    $scope.selectedRowTieuChuan = -1;
    $scope.selectedRowSPD = -1;
    $scope.MaTieuChuanChon = "";
    $scope.SPD_TenSanPham = "";
    $scope.SPD_CheckAllSPDoi = false;
    $scope.DSSanPhamDoi = [];
    $scope.TenSanPhamDoi = "";
    $scope.SPN_TenSanPham = "";
    $scope.SPN_CheckAll = false;
    $scope.ShowBoxRight = false;
    $scope.DSSanPhamNhan = [];
    $scope.selectedRowSPN = -1;
    angular.element(document).ready(function () {
        GetBottomAction();
        GetDanhMuc();
        GetTieuChuan();
    });

    $scope.RoleBtnCreate = false;
    $scope.RoleBtnUpdate = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;
    $scope.ListSanPham = [];
    $scope.ListKhuVuc = [];
    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/DinhMuc/GetBottomAction',
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'btnUpdate') {
                            $scope.RoleBtnUpdate = true;
                        }
                        if (item == 'btnSearch') {
                            $scope.RoleBtnSearch = true;
                        }
                        if (item == 'btnDelete') {
                            $scope.RoleBtnDelete = true;
                        }
                    });
                }
                $scope.$apply();
            }
        });
    }

    function GetDanhMuc() {
        $scope.ListSanPham = [];
        $scope.ListKhuVuc = [];
        $.ajax({
            type: 'post',
            url: '/DinhMuc/GetDanhMuc',
            data: {},
            success: function (data) {
                $scope.ListSanPham = data.sanPhams;
                $scope.ListKhuVuc = data.khuVucs;
                $scope.$apply();
            }
        });
    }


    //// Danh sách function

    // Tìm kiếm tiêu chuẩn
    $scope.TC_TimKiem = function () {
        if ($scope.TC_MaDoiTieuChuan == null || $scope.TC_MaDoiTieuChuan == '') {
            toastr.error('Bạn chưa nhập điều kiện tìm kiếm!');
        } else {
            GetTieuChuan();
        }
    };

    function GetTieuChuan() {
        $scope.selectedRowTieuChuan = -1;
        showToast();
        $scope.DSTieuChuan = [];
        $.ajax({
            type: 'post',
            url: '/DinhMuc/GetTieuChuan',
            data: {
                maTieuChuan: $scope.TC_MaDoiTieuChuan
            },
            success: function (data) {
                hideLoading();
                $scope.DSTieuChuan = data.data;
                $scope.$apply();
            }
        });
    }

    $scope.TC_TieuChuanSelect = {};
    // Chọn tiêu chuẩn
    $scope.TC_SelectTieuChuan = function (item, index) {
        $scope.selectedRowTieuChuan = index;
        $scope.TC_TieuChuanSelect = item;
        $scope.ShowBoxRight = true;

        // Lấy danh sách sản phẩm đổi
        $scope.DSSanPhamDoi = $scope.ListSanPham;
    };

    // Làm mới danh sách tiêu chuẩn
    $scope.TC_LamMoi = function () {
        GetTieuChuan();
    };

    // thêm mới tiêu chuẩn
    $scope.TC_ThemMoi = function () {
        $('#AddTieuChuan').modal('show');
    };

    // Lưu tiêu chuẩn
    $scope.TC_LuuTieuChuan = function () {
        if ($scope.TC_SO_QUYET_DINH == null || $scope.TC_SO_QUYET_DINH == '') {
            toastr.error('Bạn chưa nhập mô tả!');
        } else {
            $.ajax({
                type: 'post',
                url: '/DinhMuc/AddTieuChuan',
                data: {
                    soQuyetDinh: $scope.TC_SO_QUYET_DINH
                },
                success: function (data) {
                    hideLoading();
                    GetTieuChuan();
                }
            });
        }
    };

    // thêm mới sản phẩm đổi
    $scope.SPD_ThemMoi = function () {
    };

    // Xóa sản phẩm đổi
    $scope.SPD_Xoa = function () {
    };

    // Tìm kiếm sản phẩm đổi

    $scope.SPD_TimKiem = function () {
    };

    // Chọn, bỏ chọn tất cả sản phẩm đổi
    $scope.SPD_ChangeSPDoi = function () {
    };

    // Chọn sản phẩm đổi
    $scope.SPD_ChooseSPD = function () {
    };

    // Hủy thay đổi sản phẩm đổi
    $scope.SPD_HuyThayDoi = function () {
    };

    // Lưu sản phẩm đổi
    $scope.SPD_Luu = function () {
    };

    // thêm mới sản phẩm nhận
    $scope.SPN_ThemMoi = function () {
    };

    // xóa sản phẩm nhận
    $scope.SPN_Xoa = function () {
    };


    // Tìm kiếm SPN
    $scope.SPN_TimKiem = function () {
    };

    // Chọn, bỏ chọn tất cả SPN
    $scope.SPN_ChangeCheckAll = function () {
    };

    //Chọn SPN
    $scope.SPN_ChooseSanPhamNhan = function () {
    };

    //Danh sách SPN
    $scope.SPN_DSSanPham = function () {
    };

    //hủy thay đổi SPN
    $scope.SPN_HuyThayDoi = function () {
    };

    //Lưu SPN
    $scope.SPN_Luu = function () {
    };

    //// Kết thúc

    $scope.pageChanged = function () {
        $scope.LoadPage();
    };

    $scope.ChangeSoLuong = function () {
        $scope.disabledRollbackData = false;
        $scope.disabledSaveChange = false;
    };

    $scope.ViewDetail = function (ID, rowIndex) {
        $scope.selectedRow = rowIndex;
    };

    $scope.LoadPage = function () {
        $.ajax({
            type: 'post',
            url: '/DinhMuc/GetAllByPage',
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

    $scope.AddTrangPhuc = function () {
        var trangPhucSelected = $scope.ListSanPhamSearch.filter(function (x) {
            return (x.SELECT == true);
        });
        if (trangPhucSelected != null && trangPhucSelected.length > 0) {
            $.ajax({
                type: 'post',
                url: '/DinhMuc/Add',
                data: {
                    loaiTPs: trangPhucSelected,
                    lucLuongId: $scope.LucLuongId,
                    nhomTCId: $scope.NhomTCId,
                    nhomCBId: $scope.NhomCBId,
                    mua: $scope.Mua,
                    nam: $scope.NamBanHanh
                    //, ngayCapNhat: moment($scope.dateUpdate).format()
                },
                success: function (data) {
                    if (data.Error == false) {
                        toastr.success("Thêm mới thành công!");
                        GetTieuChuanCBCS();
                    } else {
                        toastr.error(data.Title);
                    }
                }
            });
        } else {
            toastr.error('Bạn chưa chọn Loại trang phục!');
        }
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
            onClick: onClickTCCBCS
        }
    };

    function onClickTCCBCS(event, treeId, treeNode, clickFlag) {
        $scope.NhomCBId = 0;
        $scope.NhomTCId = 0;
        $scope.NhomTCOrNhomCB = "";
        $scope.Mua = "";
        // Lấy danh sách tiêu chuẩn của nhóm tiêu chuẩn đã chọn
        if (treeNode != null) {
            $scope.Mua = treeNode.mua;
            $scope.NhomTCOrNhomCB = treeNode.name_control;
            var arrayId = treeNode.id.split('_');
            if (arrayId != null) {

                if (arrayId.length == 2) {
                    $scope.NhomCBId = 0;
                    $scope.NhomTCId = 0;
                }

                if (arrayId.length == 3) {
                    $scope.NhomCBId = 0;
                    $scope.NhomTCId = parseInt(arrayId[1]);
                }

                if (arrayId.length > 3) {
                    $scope.NhomCBId = parseInt(arrayId[arrayId.length - 2]);;
                    $scope.NhomTCId = parseInt(arrayId[1]);
                }
            } else {
                $scope.NhomCBId = 0;
                $scope.NhomTCId = 0;
            }
        }
        GetTieuChuanCBCS();
    }

    var treeLucLuong;

    var treeLucLuongSource = [];



    function GetTieuChuanCBCS() {
        if ($scope.LucLuongName == 'Lực lượng' || $scope.LucLuongId == 0) {
            toastr.error('Lực lượng chọn không được là lực lượng cha');
        } else {
            showToast();
            $scope.ListData = [];
            $.ajax({
                type: 'post',
                url: '/DinhMuc/GetTieuChuanCBCS',
                data: {
                    nhomCBId: $scope.NhomCBId,
                    nhomTCId: $scope.NhomTCId,
                    lucluongId: $scope.LucLuongId,
                    mua: $scope.Mua,
                    nam: $scope.NamBanHanh
                },
                success: function (data) {
                    hideLoading();

                    $scope.ListData = data.data;
                    $scope.$apply();
                }
            });
        }
    }

    function ConvertTreeLucLuong(lstTreeModel, lucLuongs, Id) {
        var lstPageMenu = lucLuongs.filter(function (x) {
            return (x.LL_CHA_ID == Id);
        });

        if (lstPageMenu != null && lstPageMenu.length > 0) {
            lstTreeModel.subs = [];
            for (var i = 0; i < lstPageMenu.length; i++) {
                var tree =
                {
                    id: lstPageMenu[i].ID,
                    title: lstPageMenu[i].TEN_LL
                };
                // Kiểm tra xem có con không
                var lucLuongChilds = lucLuongs.filter(function (x) {
                    return (x.LL_CHA_ID == lstPageMenu[i].ID);
                });
                if (lucLuongChilds != null && lucLuongChilds.length > 0) {
                    ConvertTreeLucLuong(tree, lucLuongs, lstPageMenu[i].ID);
                }
                lstTreeModel.subs.push(tree);
            }

        }
    }


    $scope.ChangeLucLuong = function () {
        $scope.LucLuongName = treeLucLuong.getSelectedNames();
        var id = treeLucLuong.getSelectedIds();
        if (id > 0) {
            $scope.LucLuongId = id;
        } else {
            $scope.LucLuongId = 0;
        }
        GetTieuChuanCBCS();
    };

    // tìm kiếm nhóm tc
    $scope.ChangeNhomTieuChuan = function () {
        $scope.ListPageMenu = [];
        $.ajax({
            type: 'post',
            url: '/DinhMuc/SearchNhomTieuChuan',
            data: { keyword: $scope.NhomTieuChuanText },
            success: function (data) {

                $scope.NhomCBId = 0;
                $scope.NhomTCId = 0;
                $scope.NhomTCOrNhomCB = "";
                $scope.Mua = "";

                $scope.ListPageMenu = data.TreeTCData;
                $.fn.zTree.init($("#treeRole"), setting, $scope.ListPageMenu);
                var zTree = $.fn.zTree.getZTreeObj("treeRole");
                var type = { "Y": "ps", "N": "ps" };
                zTree.setting.check.chkboxType = type;
            }
        });
    };


    $scope.AddNew = function () {
        if ($scope.LucLuongName == 'Lực lượng' || $scope.LucLuongId == 0) {
            toastr.error('Lực lượng chọn không được là lực lượng cha');
        } else {
            $scope.ListSanPhamSearch = $scope.ListSanPham;
            $('#AddSanPham').modal('show');
        }
    };

    // Thêm trang phục
    $scope.LoaiSanPham = "NEW";
    $scope.AddNew = function () {
        if ($scope.LucLuongName == 'Lực lượng' || $scope.LucLuongId == 0) {
            toastr.error('Lực lượng chọn không được là lực lượng cha');
        } else {
            $scope.ChangeLoaiTrangPhuc();
            $('#AddSanPham').modal('show');
        }
    };

    // thay đổi loại trang phục
    $scope.ChangeLoaiTrangPhuc = function () {
        $.ajax({
            type: 'post',
            url: '/DinhMuc/SearchSanPham',
            data: {
                keyword: $scope.TrangPhucText,
                nhomTCId: $scope.NhomTCId,
                loaiTrangPhuc: $scope.LoaiSanPham,
                nhomCBId: $scope.NhomCBId,
                lucLuongId: $scope.LucLuongId,
                mua: $scope.Mua,
                nam: $scope.NamBanHanh
            },
            success: function (data) {
                $scope.ListSanPhamSearch = data.SanPhamS;
                $scope.$apply()
            }
        });
    };

    // Chọn trang ohuc
    $scope.CheckedTrangPhuc = function () {
        var lstPageMenu = $scope.ListSanPhamSearch.filter(function (x) {
            return (x.SELECT == true);
        });
        if (lstPageMenu != null && lstPageMenu.length == $scope.ListSanPhamSearch.length) {
            $scope.CheckAllTP = true;
        } else {
            $scope.CheckAllTP = false;
        }

    };

    // Chọn bỏ chọn all trang phục
    $scope.ChangeCheckAllTP = function () {
        for (var i = 0; i < $scope.ListSanPhamSearch.length; i++) {
            $scope.ListSanPhamSearch[i].SELECT = $scope.CheckAllTP;
        }
    };



    $scope.Refesh = function () {
        $scope.LoadPage();
    };

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

    $scope.add = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/DinhMuc/_Add',
            controller: 'add',
            size: 'xl',
            backdrop: 'static'
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    };
    $scope.edit = function (itemId) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/DinhMuc/_Edit',
            controller: 'edit',
            size: 'xl',
            backdrop: 'static',
            resolve: {
                itemId: function () {
                    return itemId;
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    };


    // Lấy lại dữ liệu cũ
    $scope.RollbackData = function () {
        GetTieuChuanCBCS();
        $scope.disabledRollbackData = true;
        $scope.disabledSaveChange = true;
    }

    $scope.delete = function (itemId, name) {

        // lấy danh sách id tieu chuan cbcs can xoa
        var tccbcsIds = $scope.ListData.filter(function (x) {
            return (x.SELECT == true);
        });

        if (tccbcsIds != null && tccbcsIds.length > 0) {
            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn xóa trang phục này?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Xóa',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            $.ajax({
                                type: 'post',
                                url: '/DinhMuc/Delete',
                                data: { cbcss: tccbcsIds },
                                success: function (data) {
                                    if (data.Error) {
                                        toastr.error(data.Title);
                                    } else {
                                        toastr.success(data.Title);
                                        //$scope.cancel();
                                        GetTieuChuanCBCS();
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
            toastr.error("Chưa có hồ sơ nào được chọn!");
        }
    };

    // Lưu thay đổi
    $scope.SaveChange = function () {
        $.ajax({
            type: 'post',
            url: '/DinhMuc/Edit',
            data: { loaiTPs: $scope.ListData },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    toastr.success(data.Title);
                    GetTieuChuanCBCS();
                    $scope.disabledRollbackData = true;
                    $scope.disabledSaveChange = true;
                }
            }
        });
    };
});

app.controller('add', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading) {
    $scope.ListPageMenu = [];
    $scope.ListUnit = [];
    $scope.ListSCOPE = [{ ID: "A", Name: 'Toàn hệ thống' }, { ID: "D", Name: 'Áp dụng theo Domain_code' }, { ID: "U", Name: 'Áp dụng theo đơn vị' }];
    $scope.ListAppCode = [];
    angular.element(document).ready(function () {
        showToast();
        GetDanhMuc();
    });

    var setting = {
        check: {
            enable: true
        },
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "pId",
                rootPId: ""
            }
        }
    };


    function GetDanhMuc() {
        $scope.ListPageMenu = [];
        $scope.ListUnit = [];
        $scope.ListAppCode = [];
        $.ajax({
            type: 'post',
            url: '/DinhMuc/GetDanhMuc',
            data: {},
            success: function (data) {
                hideLoading();
                $scope.ListUnit = data.Units;
                $scope.ListAppCode = data.AppCodes;
                $scope.model.SCOPE = $scope.ListSCOPE[0].ID;
                $scope.model.APP_CODE = $scope.ListAppCode[0].APP_CODE;
                $scope.$apply();
                $scope.ListPageMenu = data.TreeDatas;
                $.fn.zTree.init($("#treeRole"), setting, $scope.ListPageMenu);
                var zTree = $.fn.zTree.getZTreeObj("treeRole");
                var type = { "Y": "ps", "N": "ps" };
                zTree.setting.check.chkboxType = type;

            }
        });
    }

    $scope.model = {};
    $scope.submit = function () {
        $scope.model.Status = $scope.model.StatusTemp === '1' ? true : false;
        $("#formSubmit").validate({
            rules: {
                ROLE_TYPE: {
                    required: true,
                    maxlength: 50
                },
                SCOPE: {
                    required: true
                },
                APP_CODE: {
                    required: true
                }

            },
            messages: {
                ROLE_TYPE: {
                    required: "Vui lòng nhập mã",
                    maxlength: "Mã không được vượt quá 50 ký tự"
                },
                SCOPE: {
                    required: "Vui lòng nhập Phạm vi"
                },
                APP_CODE: {
                    required: "Vui lòng nhập Phòng ban"
                }
            }
        });
        if ($("#formSubmit").valid()) {

            var treeObj = $.fn.zTree.getZTreeObj("treeRole");
            var nodes = treeObj.getCheckedNodes();

            angular.forEach($scope.ListPageMenu, function (pageMenu) {

                var dataSearch = nodes.filter(function (item) {
                    return item.id === pageMenu.id;
                });

                if (dataSearch !== null && dataSearch.length > 0) {
                    pageMenu.checked = true;
                }

            })

            if ($scope.model.SCOPE == "D" && ($scope.model.DOMAIN_CODE == null || $scope.model.DOMAIN_CODE == "")) {
                toastr.error("Bạn chưa nhập DOMAIN_CODE!");
                return;
            }
            if ($scope.model.SCOPE == "U" && ($scope.model.UNIT_ID == null || $scope.model.UNIT_ID == "" || $scope.model.UNIT_ID == 0)) {
                toastr.error("Bạn chưa chọn Đơn vị!");
                return;
            }

            $.ajax({
                type: 'post',
                url: '/DinhMuc/Add',
                data: { role: $scope.model, pageMenus: $scope.ListPageMenu },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.cancel();
                    }
                }
            });
        }
    };


    $scope.cancel = function () {
        $uibModalInstance.close();
    };

    $scope.SelectFile = function (e) {
        $scope.model.Avartar = e.target.files[0];
        document.getElementById("pathPhoto").src = e.target.files[0];
    };
});

app.controller('edit', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {
    $scope.ListPageMenu = [];
    $scope.ListUnit = [];
    $scope.ListAppCode = [];
    $scope.ListSCOPE = [{ ID: "A", Name: 'Toàn hệ thống' }, { ID: "D", Name: 'Áp dụng theo Domain_code' }, { ID: "U", Name: 'Áp dụng theo đơn vị' }];
    $scope.ListRole = [];
    var setting = {
        check: {
            enable: true
        },
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "pId",
                rootPId: ""
            }
        }
    };


    $scope.model = {};
    angular.element(document).ready(function () {
        GetDanhMuc();
        $.ajax({
            type: 'post',
            url: '/DinhMuc/GetItemByID',
            data: { Id: itemId },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.model = data.data;

                    $scope.ListPageMenu = data.TreeDatas;
                    $.fn.zTree.init($("#treeRole"), setting, $scope.ListPageMenu);
                    var zTree = $.fn.zTree.getZTreeObj("treeRole");
                    var type = { "Y": "ps", "N": "ps" };
                    zTree.setting.check.chkboxType = type;
                    $scope.$apply();
                }
            }
        });
    });

    function GetDanhMuc() {
        $scope.ListUnit = [];
        $scope.ListAppCode = [];
        $.ajax({
            type: 'post',
            url: '/DinhMuc/GetDanhMuc',
            data: {},
            success: function (data) {
                hideLoading();
                $scope.ListUnit = data.Units;
                $scope.ListAppCode = data.AppCodes;
            }
        });
    }

    $scope.submit = function () {
        $scope.model.Status = $scope.model.StatusTemp === '1' ? true : false;
        $("#formSubmit").validate({
            rules: {
                ROLE_TYPE: {
                    required: true,
                    maxlength: 50
                },
                SCOPE: {
                    required: true
                },
                APP_CODE: {
                    required: true
                }

            },
            messages: {
                ROLE_TYPE: {
                    required: "Vui lòng nhập mã",
                    maxlength: "Mã không được vượt quá 50 ký tự"
                },
                SCOPE: {
                    required: "Vui lòng nhập Phạm vi"
                },
                APP_CODE: {
                    required: "Vui lòng nhập Phòng ban"
                }
            }
        });
        if ($("#formSubmit").valid()) {

            var treeObj = $.fn.zTree.getZTreeObj("treeRole");
            var nodes = treeObj.getCheckedNodes();

            angular.forEach($scope.ListPageMenu, function (pageMenu) {

                var dataSearch = nodes.filter(function (item) {
                    return item.id === pageMenu.id;
                });

                if (dataSearch !== null && dataSearch.length > 0) {
                    pageMenu.checked = true;
                }

            })

            if ($scope.model.SCOPE == "D" && ($scope.model.DOMAIN_CODE == null || $scope.model.DOMAIN_CODE == "")) {
                toastr.error("Bạn chưa nhập DOMAIN_CODE!");
                return;
            }
            if ($scope.model.SCOPE == "U" && ($scope.model.UNIT_ID == null || $scope.model.UNIT_ID == "" || $scope.model.UNIT_ID == 0)) {
                toastr.error("Bạn chưa chọn Đơn vị!");
                return;
            }

            $.ajax({
                type: 'post',
                url: '/DinhMuc/Edit',
                data: { role: $scope.model, pageMenus: $scope.ListPageMenu },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.cancel();
                        $scope.LoadPage();
                    }
                }
            });
        }
    };

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});