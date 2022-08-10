app.controller("QSHocVienController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location, $rootScope) {
    //#region giá trị mặc định
    $scope.IsShowRight = false;
    $scope.IsShowHocVien = false;
    $scope.HocVien = false;
    $scope.ky0 = false;
    $scope.ky1 = false;
    $scope.ky2 = false;
    $scope.ky3 = false;
    $scope.ky4 = false;
    $scope.ky5 = false;
    $scope.ListNam = [];
    var currentYear = new Date().getFullYear();
    for (var i = currentYear; i > currentYear - 7; i--) {
        $scope.ListNam.push({ Id: i, Name: i });
    }
    $scope.NAM = currentYear.toString();
    $scope.KY = 'XH';
    $scope.HocVienModel = {};
    $scope.HocVienModel.NAM_HOC = 0;
    $scope.dsLoaiHocVien = [
        { Id : 1, TEN_LOAI_HOC_VIEN: 'Học viên CAND'},
        { Id : 2, TEN_LOAI_HOC_VIEN: 'Cán bộ đi học'}
    ];

    $scope.disabledDelHocVien = true;
    $scope.disabledEditHocVien = true;
    $scope.actionCRUD = '';
	$scope.actionNienKhoaCRUD = '';
	$scope.actionKhoaHocCRUD = '';

    $scope.GetDanhMuc = function () {
        $.ajax({
            type: 'get',
            async: false,
            url: '/ChotQuanSoDauKy/GetDanhMuc',
            success: function (res) {
                //#region Lực lượng
                $scope.LucLuongTreeInit = [];
                $scope.LucLuongTreeInit.data = res.lucLuong;

                $scope.LucLuongCallback = function (data) {
                    $scope.LucLuongComboTree = data;
                };
                //#endregion
                //#region Lực lượng
                $scope.NhomCBTreeInit = [];
                $scope.NhomCBTreeInit.data = res.nhomCapBac;
                $scope.NhomCBCallback = function (data) {
                    $scope.NhomCBComboTree = data;
                };
                //#endregion

                $scope.ListCapBacs = res.capBac;
                //$scope.$apply();
            }
        })
    }
    $scope.GetDanhMuc();

    $scope.ChangeLucLuong = function (data) {
        if ($scope.LucLuongComboTree !== undefined) {
            $scope.LucLuongId = $scope.LucLuongComboTree._selectedItem.id;
            //$scope.HocVienModel.LUC_LUONG_ID = $scope.LucLuongComboTree._selectedItem.id;
        }
    }
    $scope.ChangeNhomCapBac = function (data) {
        if ($scope.NhomCBComboTree !== undefined) {
            $scope.NhomCBId = $scope.NhomCBComboTree._selectedItem.id;
        }
    }

    //#endregion

    //#region tạo treeview
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
        showToast();
        if (treeNode !== undefined && treeNode.id !== undefined && !treeNode.isParent) {
            $scope.TEN_DON_VI = treeNode.name;
            $scope.DON_VI_ID = treeNode.id.split('_')[1];
            $scope.IsShowRight = true;
            $scope.ListKhoaHoc = [];
            $scope.ListNienKhoa = [];
            $scope.dsHocVien = [];
            
            $scope.GetKhoaHoc(treeNode.id.split('_')[1], $scope.NAM);
            $scope.khoaHocId = 0;
            $scope.itemKhoaHocSelect = -1;
        }
        hideLoading();
    }
    $scope.ChangeNam = function () {
        showToast();
        $scope.LayDanhSachHocVien();
        //$scope.GetKhoaHoc($scope.DON_VI_ID, $scope.NAM);
        //$scope.khoaHocId = 0;
        //$scope.itemKhoaHocSelect = 0;
        //$scope.LoadDataHocVien();
        //$scope.LoadDataHocVien_12();
        //$scope.LoadDataHocVien_23();
        //$scope.LoadDataHocVien_34();
        //$scope.LoadDataHocVien_45();
        //$scope.LoadDataHocVien_56();
        hideLoading();
    }
    $scope.treeData = {};
    $scope.treeData.data = {};
    $scope.ListDonVi;
    $.ajax({
        type: 'get',
        async: false,
        url: '/QSHocVien/GetTreeData',
        success: function (res) {
            if (!res.Error) {
                $scope.ListDonVi = res;
                $.fn.zTree.init($("#treeRole"), setting, $scope.ListDonVi.data);
                var zTree = $.fn.zTree.getZTreeObj("treeRole");
                var type = { "Y": "ps", "N": "ps" };
                zTree.setting.check.chkboxType = type;
            }
        }
    });
    //#endregion tạo treeview

    //#region Học viên kỳ đầu
    $scope.ch_tree_Table_Checkbox = true;
    $scope.hv_tree_data = [];
    $scope.LoadDataHocVien = function (donVi) {
        $.ajax({
            type: 'post',
            url: '/QSHocVien/DanhSachTp_HocVien',
            //data: { khoaHoc: $scope.khoaHocId, namHoc: 0 },
            data: { khoaHoc: $scope.khoaHocId, nienKhoa: $scope.NIEN_KHOA_ID, nam: $scope.NAM},
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.hv_tree_data = res.data;
                    if (res.data.length > 0)
                        $scope.ky0 = true;
                    else
                        $scope.ky0 = false;
                    if (!$scope.$$phase)
                        $scope.$apply();
                }
            }
        })
    }
    //$scope.LoadDataHocVien();

    //$scope.ch_tree_data = [];
    // cấu hình hiển thị cùng với icon
    $scope.ch_expanding_property = {
        field: "capBac",
        displayName: "Cấp bậc",
        width: "30%",
        rowspan: 3,
        colspan: 2
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.hv_col_defs = [[
        { field: "capBac", displayName: "Cấp bậc", width: "15%", rowspan: 1, colspan: 1 },
        { field: "loaiHocVien", displayName: "Loại học viên", width: "20%", rowspan: 1, colspan: 1 },
        { field: "slNam", displayName: "Nam", width: "10%", rowspan: 1, colspan: 1 },
        { field: "slNu", displayName: "Nữ", width: "10%", rowspan: 1, colspan: 1 },
        { field: "slTong", displayName: "Tổng cộng", width: "10%", rowspan: 1, colspan: 1 },
    ]];

    $scope.hv_body_defs = [{ field: "capBac" }, { field: "loaiHocVien" }, { field: "slNam" }, { field: "slNu" }, { field: "slTong" }];
    // event when change value checkbox
    $scope.hv_ChangeTreeTableCheckbox = function (item) {
        console.log(item);
    };

    // event when click choose row
    $scope.hv_ClickTreeTableRow = function (item) {
        console.log(item);
    };
    //#endregion
    //#region Học viên năm 1 - 2
    $scope.ch_tree_Table_Checkbox_12 = false;
    // Model data
    $scope.hv_tree_data_12 = [];
    $scope.LoadDataHocVien_12 = function (donVi) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/QSHocVien/DanhSachTp_HocVien',
            data: { khoaHoc: $scope.khoaHocId, namHoc: 1 },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.hv_tree_data_12 = res.data;
                    if (res.data.length > 0)
                        $scope.ky1 = true;
                    else
                        $scope.ky1 = false;
                    if (!$scope.$$phase)
                        $scope.$apply();
                }
            }
        })
    }
    //$scope.LoadDataHocVien_12();

    //$scope.ch_tree_data = [];
    // cấu hình hiển thị cùng với icon
    $scope.ch_expanding_property_12 = {
        field: "capBac",
        displayName: "Cấp bậc",
        width: "30%",
        rowspan: 3,
        colspan: 2
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.hv_col_defs_12 = [[
        { field: "capBac", displayName: "Cấp bậc", width: "15%", rowspan: 1, colspan: 1 },
        { field: "loaiHocVien", displayName: "Loại học viên", width: "20%", rowspan: 1, colspan: 1 },
        { field: "slNam", displayName: "Nam", width: "10%", rowspan: 1, colspan: 1 },
        { field: "slNu", displayName: "Nữ", width: "10%", rowspan: 1, colspan: 1 },
        { field: "slTong", displayName: "Tổng cộng", width: "10%", rowspan: 1, colspan: 1 },
    ]];

    $scope.hv_body_defs_12 = [{ field: "capBac" }, { field: "loaiHocVien" }, { field: "slNam" }, { field: "slNu" }, { field: "slTong" }];
    // event when change value checkbox
    $scope.hv_ChangeTreeTableCheckbox_12 = function (item) {
        console.log(item);
    };

    // event when click choose row
    $scope.hv_ClickTreeTableRow_12 = function (item) {
        console.log(item);
    };
    //#endregion
    //#region Học viên năm 2 - 3
    $scope.ch_tree_Table_Checkbox_23 = false;
    // Model data
    $scope.hv_tree_data_23 = [];
    $scope.LoadDataHocVien_23 = function (donVi) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/QSHocVien/DanhSachTp_HocVien',
            data: { khoaHoc: $scope.khoaHocId, namHoc: 2 },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.hv_tree_data_23 = res.data;
                    if (res.data.length > 0)
                        $scope.ky2 = true;
                    else
                        $scope.ky2 = false;
                    if (!$scope.$$phase)
                        $scope.$apply();
                }
            }
        })
    }
    //$scope.LoadDataHocVien_23();

    //$scope.ch_tree_data = [];
    // cấu hình hiển thị cùng với icon
    $scope.ch_expanding_property = {
        field: "capBac",
        displayName: "Cấp bậc",
        width: "30%",
        rowspan: 3,
        colspan: 2
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.hv_col_defs_23 = [[
        { field: "capBac", displayName: "Cấp bậc", width: "15%", rowspan: 1, colspan: 1 },
        { field: "loaiHocVien", displayName: "Loại học viên", width: "20%", rowspan: 1, colspan: 1 },
        { field: "slNam", displayName: "Nam", width: "10%", rowspan: 1, colspan: 1 },
        { field: "slNu", displayName: "Nữ", width: "10%", rowspan: 1, colspan: 1 },
        { field: "slTong", displayName: "Tổng cộng", width: "10%", rowspan: 1, colspan: 1 },
    ]];

    $scope.hv_body_defs_23 = [{ field: "capBac" }, { field: "loaiHocVien" }, { field: "slNam" }, { field: "slNu" }, { field: "slTong" }];
    // event when change value checkbox
    $scope.hv_ChangeTreeTableCheckbox_23 = function (item) {
        console.log(item);
    };

    // event when click choose row
    $scope.hv_ClickTreeTableRow_23 = function (item) {
        console.log(item);
    };
    //#endregion
    //#region Học viên năm 3 - 4
    $scope.ch_tree_Table_Checkbox_34 = false;
    // Model data
    $scope.hv_tree_data_34 = [];
    $scope.LoadDataHocVien_34 = function (donVi) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/QSHocVien/DanhSachTp_HocVien',
            data: { khoaHoc: $scope.khoaHocId, namHoc: 3 },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.hv_tree_data_34 = res.data;
                    if (res.data.length > 0)
                        $scope.ky3 = true;
                    else
                        $scope.ky3 = false;
                    if (!$scope.$$phase)
                        $scope.$apply();
                }
            }
        })
    }
    //$scope.LoadDataHocVien_34();

    //$scope.ch_tree_data = [];
    // cấu hình hiển thị cùng với icon
    $scope.ch_expanding_property_34 = {
        field: "capBac",
        displayName: "Cấp bậc",
        width: "30%",
        rowspan: 3,
        colspan: 2
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.hv_col_defs_34 = [[
        { field: "capBac", displayName: "Cấp bậc", width: "15%", rowspan: 1, colspan: 1 },
        { field: "loaiHocVien", displayName: "Loại học viên", width: "20%", rowspan: 1, colspan: 1 },
        { field: "slNam", displayName: "Nam", width: "10%", rowspan: 1, colspan: 1 },
        { field: "slNu", displayName: "Nữ", width: "10%", rowspan: 1, colspan: 1 },
        { field: "slTong", displayName: "Tổng cộng", width: "10%", rowspan: 1, colspan: 1 },
    ]];

    $scope.hv_body_defs_34 = [{ field: "capBac" }, { field: "loaiHocVien" }, { field: "slNam" }, { field: "slNu" }, { field: "slTong" }];
    // event when change value checkbox
    $scope.hv_ChangeTreeTableCheckbox_34 = function (item) {
        console.log(item);
    };

    // event when click choose row
    $scope.hv_ClickTreeTableRow_34 = function (item) {
        console.log(item);
    };
    //#endregion
    //#region Học viên năm 4 - 5
    $scope.ch_tree_Table_Checkbox_45 = false;
    // Model data
    $scope.hv_tree_data_45 = [];
    $scope.LoadDataHocVien_45 = function (donVi) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/QSHocVien/DanhSachTp_HocVien',
            data: { khoaHoc: $scope.khoaHocId, namHoc: 4 },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.hv_tree_data_45 = res.data;
                    if (res.data.length > 0)
                        $scope.ky4 = true;
                    else
                        $scope.ky4 = false;
                    if (!$scope.$$phase)
                        $scope.$apply();
                }
            }
        })
    }
    //$scope.LoadDataHocVien_45();

    //$scope.ch_tree_data = [];
    // cấu hình hiển thị cùng với icon
    $scope.ch_expanding_property_45 = {
        field: "capBac",
        displayName: "Cấp bậc",
        width: "30%",
        rowspan: 3,
        colspan: 2
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.hv_col_defs_45 = [[
        { field: "capBac", displayName: "Cấp bậc", width: "15%", rowspan: 1, colspan: 1 },
        { field: "loaiHocVien", displayName: "Loại học viên", width: "20%", rowspan: 1, colspan: 1 },
        { field: "slNam", displayName: "Nam", width: "10%", rowspan: 1, colspan: 1 },
        { field: "slNu", displayName: "Nữ", width: "10%", rowspan: 1, colspan: 1 },
        { field: "slTong", displayName: "Tổng cộng", width: "10%", rowspan: 1, colspan: 1 },
    ]];

    $scope.hv_body_defs_45 = [{ field: "capBac" }, { field: "loaiHocVien" }, { field: "slNam" }, { field: "slNu" }, { field: "slTong" }];
    // event when change value checkbox
    $scope.hv_ChangeTreeTableCheckbox_45 = function (item) {
        console.log(item);
    };

    // event when click choose row
    $scope.hv_ClickTreeTableRow_45 = function (item) {
        console.log(item);
    };
    //#endregion
    //#region Học viên năm 5 - 6
    $scope.ch_tree_Table_Checkbox_56 = false;
    // Model data
    $scope.hv_tree_data_56 = [];
    $scope.LoadDataHocVien_56 = function (donVi) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/QSHocVien/DanhSachTp_HocVien',
            data: { khoaHoc: $scope.khoaHocId, namHoc: 5 },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.hv_tree_data_56 = res.data;
                    if (res.data.length > 0)
                        $scope.ky5 = true;
                    else
                        $scope.ky5 = false;
                    if (!$scope.$$phase)
                        $scope.$apply();
                }
            }
        })
    }
    //$scope.LoadDataHocVien_56();

    //$scope.ch_tree_data = [];
    // cấu hình hiển thị cùng với icon
    $scope.ch_expanding_property_56 = {
        field: "capBac",
        displayName: "Cấp bậc",
        width: "30%",
        rowspan: 3,
        colspan: 2
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.hv_col_defs_56 = [[
        { field: "capBac", displayName: "Cấp bậc", width: "15%", rowspan: 1, colspan: 1 },
        { field: "loaiHocVien", displayName: "Loại học viên", width: "20%", rowspan: 1, colspan: 1 },
        { field: "slNam", displayName: "Nam", width: "10%", rowspan: 1, colspan: 1 },
        { field: "slNu", displayName: "Nữ", width: "10%", rowspan: 1, colspan: 1 },
        { field: "slTong", displayName: "Tổng cộng", width: "10%", rowspan: 1, colspan: 1 },
    ]];

    $scope.hv_body_defs_56 = [{ field: "capBac" }, { field: "loaiHocVien" }, { field: "slNam" }, { field: "slNu" }, { field: "slTong" }];
    // event when change value checkbox
    $scope.hv_ChangeTreeTableCheckbox_56 = function (item) {
        console.log(item);
    };

    // event when click choose row
    $scope.hv_ClickTreeTableRow_56 = function (item) {
        console.log(item);
    };
    //#endregion

    //#region Khóa học
    $scope.GetKhoaHoc = function (donViId, nam) {
        //showToast();
        $scope.hv_tree_data = [];
        //$scope.hv_tree_data_12 = [];
        //$scope.hv_tree_data_23 = [];
        //$scope.hv_tree_data_34 = [];
        //$scope.hv_tree_data_45 = [];
        //$scope.hv_tree_data_56 = [];

        $.ajax({
            type: 'post',
            url: '/QSHocVien/GetKhoaHoc',
            data: { donViId: donViId, nam: nam },
            success: function (res) {
				
                $scope.ListKhoaHoc = res;
                console.log($scope.khoaHocId);
                console.log($scope.itemKhoaHocSelect);

                if (res.length > 0) {
                    $scope.khoaHocId = ($scope.khoaHocId != null && $scope.khoaHocId != '' && $scope.khoaHocId > 0) ? $scope.khoaHocId : 0;
                    $scope.itemKhoaHocSelect = ($scope.itemKhoaHocSelect != null && $scope.itemKhoaHocSelect != '' && $scope.itemKhoaHocSelect > 0) ? $scope.itemKhoaHocSelect : 0;

                    $scope.SelectedKhoaHoc(res[$scope.itemKhoaHocSelect], $scope.khoaHocId)
                }
					
                if (!$scope.$$phase)
                    $scope.$apply();
            }
        })

        //hideLoading();
    }
    $scope.khoaHoc = {};
    $scope.AddKhoaHoc = function () {
        $scope.khoaHoc = {};
        $scope.actionKhoaHocCRUD = 'C';
        $('#add-khoa-hoc').modal('show');
    }

    $scope.EditKhoaHoc = function () {

        $scope.khoaHoc = $scope.ListKhoaHoc.find(x => x.ID === $scope.khoaHocId);
        console.log($scope.khoaHoc);
        $scope.actionKhoaHocCRUD = 'E';
        $('#add-khoa-hoc').modal('show');
    }

    $scope.LuuKhoaHoc = function () {
        $("#form-hoc-vien").validate({
            rules: {
                //NAM_HOC: {
                //    required: true
                //},
                TEN_KHOA_HOC: {
                    required: true,
                },
                MA_KHOA_HOC: {
                    required: true,
                },

            },
            messages: {
                //NAM_HOC: {
                //    required: "Vui lòng chọn năm học",
                //},
                TEN_KHOA_HOC: {
                    required: "Vui lòng nhập Tên khóa học",
                },
                MA_KHOA_HOC: {
                    required: "Vui lòng nhập Mã Khóa học",
                },
            }
        });
        if ($("#form-hoc-vien").valid()) {
            $scope.khoaHoc.DON_VI_ID = $scope.DON_VI_ID;
            //$scope.khoaHoc.NAM = $scope.NAM;
            if ($scope.actionKhoaHocCRUD == 'E') {
                $.ajax({
                    type: 'post',
                    url: '/QSHocVien/EditKhoaHoc',
                    data: $scope.khoaHoc,
                    success: function (res) {
                        if (res.Error) {
                            toastr.error(res.Title);
                        } else {
                            
                            toastr.success(res.Title);
                            $('#add-khoa-hoc').modal('hide');
                            
                            $scope.GetKhoaHoc($scope.DON_VI_ID, $scope.NAM)
                        }
                    }
                })
            } else {
                $.ajax({
                    type: 'post',
                    url: '/QSHocVien/AddKhoaHoc',
                    data: $scope.khoaHoc,
                    success: function (res) {
                        if (res.Error) {
                            toastr.error(res.Title);
                        } else {
                            hideLoading();
                            toastr.success(res.Title);
                            $('#add-khoa-hoc').modal('hide');
                            $scope.GetKhoaHoc($scope.DON_VI_ID, $scope.NAM)
                        }
                    }
                })
            }
            
        }
    }
    $scope.itemKhoaHocSelect = -1;
    $scope.DeleteKhoaHoc = function () {
        var idKhoaHoc = $('.selected-khoa-hoc').data('id');
        $ngConfirm({
            title: 'Xác nhận',
            content: 'Bạn có chắc chắn muốn xóa khóa học?',
            scope: $scope,
            buttons: {
                OK: {
                    text: 'Đồng ý',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        $.ajax({
                            type: 'post',
                            url: '/QSHocVien/XoaKhoaHoc',
                            data: { id: idKhoaHoc },
                            success: function (res) {
                                if (res.Error) {
                                    toastr.error(res.Title);
                                } else {
                                    
                                    toastr.success(res.Title);
                                    $scope.khoaHocId = 0;
                                    $scope.itemKhoaHocSelect = 0;
                                    $scope.GetKhoaHoc($scope.DON_VI_ID, $scope.NAM);
									
                                }

                            }
                        });
                    }
                },
                Hủy: function (scope, button) {
                },
            }
        });

    }
    $scope.SelectedKhoaHoc = function (item, index) {
        //showToast();
        $scope.khoaHocId = item.ID;
        $scope.itemKhoaHocSelect = index;
        $scope.TEN_KHOA_HOC = $scope.ListKhoaHoc.find(x => x.ID == $scope.khoaHocId).TEN_KHOA_HOC;

        
        //$scope.LoadDataHocVien_12();
        //$scope.LoadDataHocVien_23();
        //$scope.LoadDataHocVien_34();
        //$scope.LoadDataHocVien_45();
        //$scope.LoadDataHocVien_56();

        //Hiển thị Niên khóa theo Khóa học đã chọn
        $scope.ListNienKhoa = [];
        $scope.GetNienKhoa($scope.khoaHocId);

        //hideLoading();
    }
    //#endregion

    //#region NIÊN KHÓA #nienkhoan
    $scope.nienKhoa = {};
    $scope.GetNienKhoa = function (khoaHocId) {
        
        $.ajax({
            type: 'post',
            url: '/QSHocVien/GetNienKhoa',
            data: { khoaHocId: khoaHocId },
            success: function (res) {
                $scope.ListNienKhoa = res;

                if (res.length > 0)
                    $scope.SelectedNienKhoa(res[0], 0)
                    if (!$scope.$$phase)
                        $scope.$apply();
            }
        })
    }

    $scope.SelectedNienKhoa = function (item, index) {
        showToast();
        $scope.itemNienKhoaSelect = index;
        $scope.NIEN_KHOA_ID = item.ID;
        $scope.NIEN_KHOA = $scope.ListNienKhoa.find((x, idx) => idx == index).TU_NAM + ' - ' + $scope.ListNienKhoa.find((x, idx) => idx == index).DEN_NAM;

        $scope.IsShowHocVien = true;
        $scope.LayDanhSachHocVien();
        hideLoading();

    }


    $scope.AddNienKhoa = function () {
        $scope.nienKhoa.TitleModal = 'Thêm mới';
                
        $scope.TEN_KHOA_HOC = $scope.ListKhoaHoc.find(x => x.ID == $scope.khoaHocId).TEN_KHOA_HOC;
        $scope.nienKhoa.TU_NAM = '';
        $scope.nienKhoa.DEN_NAM = '';
        $scope.nienKhoa.MO_TA = '';
        $scope.actionNienKhoaCRUD = 'C';

        $('#add_nien_khoa').modal('show');
        //$scope.$evalAsync();
    }
    $scope.EditNienKhoa = function () {
        $scope.nienKhoa.TitleModal = 'Sửa';
        $scope.TEN_KHOA_HOC = $scope.ListKhoaHoc.find(x => x.ID == $scope.khoaHocId).TEN_KHOA_HOC;

        $scope.nienKhoa = $scope.ListNienKhoa.find(x => x.ID === $scope.NIEN_KHOA_ID);
        $scope.actionNienKhoaCRUD = 'E';
        $('#add_nien_khoa').modal('show');
        //$scope.$evalAsync();
    }

    $scope.LuuNienKhoa = function () {
        $("#form-nien-khoa").validate({
            rules: {
                
                TU_NAM: {
                    required: true,
                },
                DEN_NAM: {
                    required: true,
                },

            },
            messages: {
                
                TU_NAM: {
                    required: "Vui lòng nhập Từ năm",
                },
                DEN_NAM: {
                    required: "Vui lòng nhập Đến năm",
                },
            }
        });

        if ($("#form-nien-khoa").valid()) {
            $scope.nienKhoa.ID_KHOA_HOC = $scope.khoaHocId;
            if ($scope.actionNienKhoaCRUD == 'E') {
                $.ajax({
                    type: 'post',
                    url: '/QSHocVien/EditNienKhoa',
                    data: $scope.nienKhoa,
                    success: function (res) {
                        if (res.Error) {
                            toastr.error(res.Title);
                        } else {
                            toastr.success(res.Title);
                            $('#add_nien_khoa').modal('hide');
                            $scope.GetNienKhoa($scope.khoaHocId)
                        }
                    }
                })
            } else {
                $.ajax({
                    type: 'post',
                    url: '/QSHocVien/AddNienKhoa',
                    data: $scope.nienKhoa,
                    success: function (res) {
                        if (res.Error) {
                            toastr.error(res.Title);
                        } else {
                            toastr.success(res.Title);
                            $('#add_nien_khoa').modal('hide');
                            $scope.GetNienKhoa($scope.khoaHocId)
                        }
                    }
                })
            }
            
        }
    }

    $scope.DeleteNienKhoa = function () {
        console.log($scope.NIEN_KHOA_ID);
        if ($scope.NIEN_KHOA_ID != '' && $scope.NIEN_KHOA_ID != undefined) {
            $ngConfirm({
                title: 'Xác nhận',
                content: 'Bạn có chắc chắn muốn xóa niên khóa đã chọn?',
                scope: $scope,
                buttons: {
                    OK: {
                        text: 'Đồng ý',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            $.ajax({
                                type: 'post',
                                url: '/QSHocVien/DeleteNienKhoa',
                                data: { nienkhoaId: $scope.NIEN_KHOA_ID },
                                success: function (res) {
                                    if (res.Error) {
                                        toastr.error(res.Title);
                                    } else {
                                        toastr.success(res.Title);
                                        $scope.GetNienKhoa($scope.khoaHocId)
                                    }
                                }
                            });
                        }
                    },
                    Hủy: function (scope, button) {
                    },
                }
            });
        }
    }
    //#endregion
    //#region học viên

    $scope.AddHocVien = function () {
        $scope.HocVienModel = {};
        $scope.lucLuong = "";
        //$scope.HocVienModel.CAP_BAC = "1";
        $scope.HocVienModel.LOAI_HOC_VIEN = "1";
        $scope.actionCRUD = 'C';
        $('#add-hoc-vien').modal('show');
    }

    $scope.EditHocVien = function () {
        if ($scope.hocVienIdToEdit > 0) {
            var itemHV = $scope.dsHocVien.find(x => x.ID === $scope.hocVienIdToEdit);
            $scope.HocVienModel.ID = $scope.hocVienIdToEdit;
            $scope.HocVienModel.LOAI_HOC_VIEN = itemHV.LOAI_HOC_VIEN ?.toString();
            $scope.HocVienModel.LUC_LUONG_ID_TXT = itemHV.TEN_LUC_LUONG;
            $scope.LucLuongId = itemHV.LUC_LUONG_ID;
            $scope.HocVienModel.CAP_BAC = itemHV.CAP_BAC;
            $scope.HocVienModel.SL_NAM = itemHV.SL_NAM;
            $scope.HocVienModel.SL_NU = itemHV.SL_NU;
            $scope.HocVienModel.SL_TONG = itemHV.SL_TONG;
            $scope.actionCRUD = 'E';
            $('#add-hoc-vien').modal('show');
        } else {
        }
    }

    $scope.sumSoLuongNamNu = function () {
        $scope.HocVienModel.SL_NAM = $scope.HocVienModel.SL_NAM ?? 0;
        $scope.HocVienModel.SL_NU = $scope.HocVienModel.SL_NU ?? 0;
        $scope.HocVienModel.SL_TONG = $scope.HocVienModel.SL_NAM + $scope.HocVienModel.SL_NU;
    }

    $scope.LuuHocVien = function () {
        showToast();
        $scope.HocVienModel.KHOA_HOC_ID = $scope.khoaHocId;
        $scope.HocVienModel.NAM = $scope.NAM;
        $scope.HocVienModel.NIEN_KHOA_ID = $scope.NIEN_KHOA_ID;
        $scope.HocVienModel.LUC_LUONG_ID = $scope.LucLuongId;

        $("#form-them-hoc-vien").validate({
            rules: {
                LOAI_HOC_VIEN: { required: true},
                LUC_LUONG: {required: true},
                CAP_BAC: {required: true},
                SL_NAM: {required: true},
                SL_NU: {required: true},

            },
            messages: {                
                LOAI_HOC_VIEN: {required: "Vui lòng nhập Loại học viên"},
                LUC_LUONG: {required: "Vui lòng nhập Lực lượng"},
                CAP_BAC: {required: "Vui lòng nhập Cấp bậc"},
                SL_NAM: {required: "Vui lòng nhập Số lượng nam"},
                SL_NU: {required: "Vui lòng nhập Số lượng nữ"},
            }
        });

        if ($("#form-them-hoc-vien").valid()) {
            console.log($scope.actionCRUD);
            if ($scope.actionCRUD === 'E') {
                $.ajax({
                    type: 'post',
                    url: '/QSHocVien/EditHocVien',
                    data: $scope.HocVienModel,
                    success: function (res) {
                        if (res.Error) {
                            toastr.error(res.Title);
                        } else {
                            hideLoading();
                            toastr.success(res.Title);
                            $scope.SelectedKhoaHoc($scope.ListKhoaHoc.find(x => x.ID === $scope.khoaHocId), $scope.itemKhoaHocSelect)
                            $('#add-hoc-vien').modal('hide');
                        }
                    },
                    error: function (error) {
                        hideLoading();
                        toastr.error("Có lỗi xảy ra. Xin vui lòng liên hệ với quản trị viên!");
                    }
                })
            } else {
                $.ajax({
                    type: 'post',
                    url: '/QSHocVien/LuuHocVien',
                    data: $scope.HocVienModel,
                    success: function (res) {
                        if (res.Error) {
                            toastr.error(res.Title);
                        } else {
                            hideLoading();
                            toastr.success(res.Title);
                            $scope.SelectedKhoaHoc($scope.ListKhoaHoc.find(x => x.ID === $scope.khoaHocId), $scope.itemKhoaHocSelect)
                            $('#add-hoc-vien').modal('hide');
                        }
                    },
                    error: function (error) {
                        hideLoading();
                        toastr.error("Có lỗi xảy ra. Xin vui lòng liên hệ với quản trị viên!");
                    }
                })
            }
            
        }
        hideLoading();

    }
	
	//Lấy danh sách học viên
    $scope.kyHocVien = '';
	$scope.LayDanhSachHocVien = function(){
		$.ajax({
            type: 'post',
            url: '/QSHocVien/LayDanhSachHocVien',
            data: { khoaHoc: $scope.khoaHocId, nienKhoa: $scope.NIEN_KHOA_ID, nam: $scope.NAM},
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    
                    $scope.dsHocVien = res.data;
                    console.log($scope.dsHocVien);
					$scope.dsHocVien.forEach( x=> {
						x.TEN_LUC_LUONG = $scope.LucLuongTreeInit.data.find(ll => ll.Id === x.LUC_LUONG_ID)?.Name,
						x.TEN_CAP_BAC = $scope.ListCapBacs.find(cb => cb.Id === x.CAP_BAC)?.Name
						x.TEN_LOAI_HOC_VIEN = $scope.dsLoaiHocVien.find(cb => cb.Id === x.LOAI_HOC_VIEN)?.TEN_LOAI_HOC_VIEN
					})
                    if (res.data.length > 0)
                        $scope.ky0 = true;
                    else
                        $scope.ky0 = false;
                    if (!$scope.$$phase)
                        $scope.$apply();
                }
            }
        });
		//Hiển thị text KyHocVien		
        var nienKhoa_TuNam = Number($scope.ListNienKhoa.find((x, idx) => idx == $scope.itemNienKhoaSelect).TU_NAM);
        var nienKhoa_DenNam = Number($scope.ListNienKhoa.find((x, idx) => idx == $scope.itemNienKhoaSelect).DEN_NAM)
        var selectedYear = Number($scope.NAM);
        if ($scope.NAM == nienKhoa_TuNam) {
            $scope.kyHocVien = 'HV nhập học lần đầu';
        } else if ($scope.NAM == nienKhoa_DenNam) {
            $scope.kyHocVien = 'HV kỳ cuối';
        } else if ($scope.NAM < nienKhoa_DenNam && $scope.NAM > nienKhoa_TuNam) {
            $scope.kyHocVien = 'HV năm' + ($scope.NAM - nienKhoa_TuNam) + ' -> ' + ($scope.NAM - nienKhoa_TuNam + 1);
        } else {
            $scope.kyHocVien = '';
        }
    }

    $scope.CopySoLieuNamTruoc = function () {
        $ngConfirm({
            title: 'Xác nhận',
            content: 'Nếu copy dữ liệu năm trước sẽ xóa bỏ dữ liệu của năm đang chọn, bạn có chắc chắn muốn copy hay không?',
            scope: $scope,
            buttons: {
                OK: {
                    text: 'Đồng ý',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        $scope.CopySoLieuNamTruocProcess();
                        $('#check-pass').modal('show');
                    }
                },
                Hủy: function (scope, button) {
                },
            }
        });
    }
    $scope.CopySoLieuNamTruocProcess = function () {
        console.log($scope.khoaHocId + '|' + $scope.NIEN_KHOA_ID + '|' + $scope.NAM);
        $.ajax({
            type: 'post',
            url: '/QSHocVien/CopySoLieuNamTruoc',
            data: { khoaHoc: $scope.khoaHocId, nienKhoa: $scope.NIEN_KHOA_ID, nam: $scope.NAM },
            success: function (res) {
                
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    toastr.success(res.Title);
                    $scope.LayDanhSachHocVien();
                }
            }
        })
    }

    $scope.ChonTatCa = () => {
        $scope.dsHocVien.forEach(x => x.Selected = !$scope.checkAll);
        if ($scope.dsHocVien.some(x => x.Selected === true)) {
            $scope.disabledDelHocVien = false;
        } else {
            $scope.disabledDelHocVien = true;
        }
    }
    $scope.TichChonHocVien = (index) => {
        $scope.dsHocVien[index].Selected = !$scope.dsHocVien[index].Selected;
        if ($scope.dsHocVien.length > 0) {
            if ($scope.dsHocVien.every(x => x.Selected === true)) {
                $scope.checkAll = true;
            } else {
                $scope.checkAll = false;
            }
        } else {
            $scope.checkAll = false;
        }

        if ($scope.dsHocVien.some(x => x.Selected === true)) {
            $scope.disabledDelHocVien = false;
        } else {
            $scope.disabledDelHocVien = true;
        }
        if ($scope.dsHocVien[index].Selected) {
            $scope.hocVienIdToEdit = $scope.dsHocVien[index].ID;
            $scope.disabledEditHocVien = false;

        } else {
            $scope.hocVienIdToEdit = '';
            $scope.disabledEditHocVien = true;

        }
    }

   

    $scope.DeleteHocVien = function () {
        $scope.dsHocVienXoa = $scope.dsHocVien.filter(x => x.Selected === true);
        if ($scope.dsHocVienXoa ?.length > 0) {
            $ngConfirm({
                title: 'Xác nhận',
                content: 'Bạn có chắc chắn muốn xóa học viên?',
                scope: $scope,
                buttons: {
                    OK: {
                        text: 'Đồng ý',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            $.ajax({
                                type: 'post',
                                url: '/QSHocVien/DeleteHocVien',
                                data: { hocviens: $scope.dsHocVienXoa },
                                success: function (res) {
                                    if (res.Error) {
                                        toastr.error(res.Title);
                                    } else {
                                        toastr.success(res.Title);
                                        $scope.LayDanhSachHocVien();
                                    }
                                }
                            });
                        }
                    },
                    Hủy: function (scope, button) {
                    },
                }
            });
        } else {
            toastr.warning('Bạn cần chọn học viên để xóa!');
        }

    }
    //#endregion
    // Thay đổi cấp bậc
    $scope.ChangeCapBac = function () {
        if ($scope.HocVienModel.CAP_BAC == "1" || $scope.HocVienModel.CAP_BAC == "2") {
            $scope.HocVienModel.LOAI_HOC_VIEN = "1";
        } else {
            $scope.HocVienModel.LOAI_HOC_VIEN = "2";
        }
    }
});