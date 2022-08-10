app.controller("TieuChuanCBCSPhamController", function ($scope, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "ROLE_TYPE DESC";
    $scope.ListRole = [];

    $scope.LucLuongId = 0;
    $scope.LucLuongName = "";
    $scope.NhomTCId = 0;
    $scope.NhomCBId = 0;
    $scope.NhomTCOrNhomCB = "";
    $scope.NhomTieuChuanText = "";
    $scope.Mua = "";

    $scope.disabledRollbackData = true;
    $scope.disabledSaveChange = true;
    $scope.dateUpdate = "";
    angular.element(document).ready(function () {
        showToast();

        GetBottomAction();
        GetDanhMuc();
        $scope.LoadPage();
    });

    $scope.ListSanPhamChon = [];

    $scope.ShowContent = false;

    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnView = false;
    $scope.RoleBtnRollback = false;
    $scope.ListYear = [];
    $scope.ListSanPham = [];
    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/TieuChuanCBCSPham/GetBottomAction',
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'btnSave') {
                            $scope.RoleBtnSave = true;
                        }
                        if (item == 'btnDelete') {
                            $scope.RoleBtnDelete = true;
                        }
                        if (item == 'btnView') {
                            $scope.RoleBtnView = true;
                        }
                        if (item == 'btnRollback') {
                            $scope.RoleBtnRollback = true;
                        }
                    });
                }
                $scope.ListYear = response.ListYear;
                $scope.NamBanHanh = response.ListYear[0].Value;
                $scope.$apply();
            }
        });
    }



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
            url: '/TieuChuanCBCSPham/GetAllByPage',
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
        showToast();
        if ($scope.ListSanPhamChon != null && $scope.ListSanPhamChon.length > 0) {
            $.ajax({
                type: 'post',
                url: '/TieuChuanCBCSPham/Add',
                data: {
                    loaiTPs: $scope.ListSanPhamChon,
                    lucLuongId: $scope.LucLuongId,
                    nhomTCId: $scope.NhomTCId,
                    nhomCBId: $scope.NhomCBId,
                    mua: $scope.Mua,
                    nam: $scope.NamBanHanh,
                    loaiSanPham: $scope.LoaiSanPham
                    //, ngayCapNhat: moment($scope.dateUpdate).format()
                },
                success: function (data) {
                    hideLoading();
                    if (data.Error == false) {
                        toastr.success("Thêm mới thành công!");
                        $('#AddSanPham').modal('hide');
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
            if (treeNode.name_control == "NHOMTC" || treeNode.name_control == "NHOMCB") {

                $scope.NhomCBId = 0;
                $scope.NhomTCId = 0;

                if (treeNode.name_control == "NHOMTC") {
                    $scope.NhomCBId = 0;
                    $scope.NhomTCId = parseInt(treeNode.NhomTCId);
                }

                if (treeNode.name_control == "NHOMCB") {
                    $scope.NhomCBId = parseInt(treeNode.NhomCBId);
                    $scope.NhomTCId = parseInt(treeNode.NhomTCId);
                    $scope.ShowContent = true;
                }
            } else {
                $scope.NhomCBId = 0;
                $scope.NhomTCId = 0;
            }
        }
        $scope.$apply();
        if ($scope.LucLuongName != "" && $scope.LucLuongName != 'Lực lượng' && $scope.LucLuongId != 0) {
            GetTieuChuanCBCS();
        }

    }

    var treeLucLuong;

    var treeLucLuongSource = [];

    function GetDanhMuc() {
        $scope.ListPageMenu = [];
        $scope.ListUnit = [];
        $scope.ListAppCode = [];
        $.ajax({
            type: 'post',
            headers: {
                'Access-Control-Allow-Origin': '*',
                'Content-Type': 'application/json'
            },
            url: '/TieuChuanCBCSPham/GetDanhMuc',
            data: {},
            success: function (data) {
                hideLoading();

                $scope.dateUpdate = data.CurrentDate;
                //$scope.ListSanPham = data.SanPhams;
                $scope.ListPageMenu = data.TreeTCData;
                $.fn.zTree.init($("#treeRole"), setting, $scope.ListPageMenu);
                var zTree = $.fn.zTree.getZTreeObj("treeRole");
                var type = { "Y": "ps", "N": "ps" };
                zTree.setting.check.chkboxType = type;

                treeLucLuongSource = [];
                if (data.LucLuongs != null && data.LucLuongs.length > 0) {
                    var lucLuong0s = data.LucLuongs.filter(function (x) {
                        return (x.LL_CHA_ID == 0);
                    });
                    if (lucLuong0s != null && lucLuong0s.length > 0) {
                        for (var i = 0; i < lucLuong0s.length; i++) {

                            var comboTree = { id: lucLuong0s[i].ID, title: lucLuong0s[i].TEN_LL /*'Lực lượng'*/ };
                            // kiểm tra có con không
                            var childs = data.LucLuongs.filter(function (x) {
                                return (x.LL_CHA_ID == lucLuong0s[i].ID);
                            });

                            if (childs != null && childs.length > 0) {
                                ConvertTreeLucLuong(comboTree, data.LucLuongs, lucLuong0s[i].ID);
                            }
                            treeLucLuongSource.push(comboTree);

                        }
                    }
                }

                treeLucLuong = $('#justAnotherInputBox').comboTree({
                    source: treeLucLuongSource,
                    isMultiple: false
                });
            }
        });
    }

    function GetTieuChuanCBCS() {
        if ($scope.NhomCBId > 0 && $scope.NhomTCId != null) {
            if ($scope.LucLuongName == 'Lực lượng' || $scope.LucLuongId == 0) {
                toastr.error('Lực lượng chọn không được là lực lượng cha');
            } else {
                showToast();
                $scope.ListData = [];
                $.ajax({
                    type: 'post',
                    url: '/TieuChuanCBCSPham/GetTieuChuanCBCS',
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
            url: '/TieuChuanCBCSPham/SearchNhomTieuChuan',
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


    // Thêm trang phục
    $scope.LoaiSanPham = "NEW";
    $scope.AddNew = function () {
        $scope.ListSanPhamChon = [];
        $scope.ListSanPhamSearch = [];
        if ($scope.NhomCBId > 0 && $scope.NhomTCId != null) {
            if ($scope.LucLuongName == 'Lực lượng' || $scope.LucLuongId == 0) {
                toastr.error('Lực lượng chọn không được là lực lượng cha.');
            } else {
                $scope.ChangeLoaiTrangPhuc();
                $('#AddSanPham').modal('show');
            }
        }
        else {
            toastr.error('Bạn chưa chọn nhóm cấp bậc.');
        }
    };

    // thay đổi loại trang phục
    $scope.ChangeLoaiTrangPhuc = function () {
        $.ajax({
            type: 'post',
            url: '/TieuChuanCBCSPham/SearchSanPham',
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

    // Chọn chọn all trang phục
    $scope.ChangeCheckAll = function () {
        for (var i = 0; i < $scope.ListData.length; i++) {
            $scope.ListData[i].SELECT = $scope.CheckAll;
        }
    };

    // Chọn trang ohuc
    $scope.CheckLLTrangPhuc = function () {
        $scope.CheckAll = false;
        var lucLuong0s = $scope.ListData.filter(function (x) {
            return (x.SELECT == true);
        });

        if (lucLuong0s != null && lucLuong0s.length == $scope.ListData.length) {
            $scope.CheckAll = true;
        }
    };



    // Chọn trang ohuc
    $scope.CheckedTrangPhuc = function (index) {
        //var lstPageMenu = $scope.ListSanPhamSearch.filter(function (x) {
        //    return (x.SELECT == true);
        //});
        //if (lstPageMenu != null && lstPageMenu.length == $scope.ListSanPhamSearch.length) {
        //    $scope.CheckAllTP = true;
        //} else {
        //    $scope.CheckAllTP = false;
        //}
        if ($scope.ListSanPhamSearch[index].SELECT == true) {
            $scope.ListSanPhamSearch[index].SELECT = false;
            // thêm vào danh sách chọn
            $scope.ListSanPhamChon.push($scope.ListSanPhamSearch[index]);

            // xóa khỏi danh sách sản phẩm
            $scope.ListSanPhamSearch.splice(index, 1);
        }
    };



    // Chọn chọn all trang phục
    $scope.ChangeCheckAllTP = function () {
        if ($scope.CheckAllTP == true) {
            for (var i = 0; i < $scope.ListSanPhamSearch.length; i++) {
                $scope.ListSanPhamSearch[i].SELECT = false;
                // thêm vào danh sách chọn
                $scope.ListSanPhamChon.push($scope.ListSanPhamSearch[i]);
            }
            $scope.ListSanPhamSearch = [];
            $scope.CheckAllTP = false;
        }
    };

    // Bỏ chọn trang ohuc
    $scope.CheckedTrangPhucChon = function (index) {
        if ($scope.ListSanPhamChon[index].SELECT == true) {
            $scope.ListSanPhamChon[index].SELECT = false;
            // thêm vào danh sách sản phẩm
            $scope.ListSanPhamSearch.push($scope.ListSanPhamChon[index]);

            // xóa khỏi danh sách sản phẩm chọn
            $scope.ListSanPhamChon.splice(index, 1);
        }
    };

    // Bỏ chọn all trang phục
    $scope.ChangeCheckAllTPChon = function () {
        if ($scope.CheckAllTPChon == true) {
            for (var i = 0; i < $scope.ListSanPhamChon.length; i++) {
                $scope.ListSanPhamChon[i].SELECT = false;
                // thêm vào danh sách chọn
                $scope.ListSanPhamSearch.push($scope.ListSanPhamChon[i]);
            }
            $scope.ListSanPhamChon = [];
            $scope.CheckAllTPChon = false;
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
            templateUrl: '/TieuChuanCBCSPham/_Add',
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
            templateUrl: '/TieuChuanCBCSPham/_Edit',
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
                            showToast();
                            $.ajax({
                                type: 'post',
                                url: '/TieuChuanCBCSPham/Delete',
                                data: { cbcss: tccbcsIds },
                                success: function (data) {
                                    if (data.Error) {
                                        toastr.error(data.Title);
                                    } else {
                                        toastr.success(data.Title);
                                        $scope.CheckAll = false;
                                        hideLoading();
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
        // Kiểm tra xem số lượng có nhỏ hơn 0 không
        var checkSL = $scope.ListData.filter(function (x) {
            return (x.SO_LUONG_TC < 0 || x.SO_LUONG_TC == 0);
        });
        if (checkSL == null || checkSL.length == 0) {
            showToast();
            $.ajax({
                type: 'post',
                url: '/TieuChuanCBCSPham/Edit',
                data: { loaiTPs: $scope.ListData },
                success: function (data) {
                    hideLoading();
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
        }
        else {
            toastr.error("Số lượng không được nhỏ hơn hoặc bằng 0.");
        }
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
            url: '/TieuChuanCBCSPham/GetDanhMuc',
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
                url: '/TieuChuanCBCSPham/Add',
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
            url: '/TieuChuanCBCSPham/GetItemByID',
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
            url: '/TieuChuanCBCSPham/GetDanhMuc',
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
                url: '/TieuChuanCBCSPham/Edit',
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