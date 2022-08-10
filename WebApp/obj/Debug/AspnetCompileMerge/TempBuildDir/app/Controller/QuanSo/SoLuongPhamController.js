app.controller("SoLuongPhamController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location, constant) {
    //#region giá trị mặc định
    $scope.IsShowRight = false;
    $scope.HocVien = false;
    $scope.ListNam = [];
    var currentYear = new Date().getFullYear();
    for (var i = currentYear; i > currentYear - 7; i--) {
        $scope.ListNam.push({ Id: i, Name: i });
    }
    $scope.NAM = currentYear.toString();
    $scope.KY = 'XH';


    $scope.GetDanhMuc = function () {
        $.ajax({
            type: 'get',
            async: false,
            url: '/ChotQuanSoDauKy/GetDanhMuc',
            success: function (res) {
                if (res.phamVi !== null)
                    $scope.RoleQuanSo = res.phamVi.DUOC_SUA === 'Y' ? false : true;
                else
                    $scope.RoleQuanSo = true;
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
            $scope.LoadPham($scope.DON_VI_ID, $scope.NAM);
            $scope.$apply();
        }
        hideLoading();
    }

    $scope.treeData = {};
    $scope.treeData.data = {};
    $scope.ListDonVi;
    $.ajax({
        type: 'get',
        async: false,
        url: '/ChotQuanSoDauKy/GetTreeData',
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
    //#region Dự kiến thăng hàm
    $.ajax({
        type: 'get',
        async: false,
        url: '/QSSoLuongPham/GetDMPham',
        success: function (res) {
            $scope.ListLoaiPham = res;
        }
    });
    $scope.ListPham = [];
    $scope.LoadPham = function (donViId, nam) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/QSSoLuongPham/GetPham',
            data: { donVi: $scope.DON_VI_ID, nam: $scope.NAM },
            success: function (res) {
                $scope.ListPham = res;
                if (res.length === 0) {
                    $scope.ListPham = [];
                    angular.forEach($scope.ListLoaiPham, function (val, key) {
                        $scope.ListPham.push({
                            ID: 0,
                            DON_VI_ID: $scope.DON_VI_ID,
                            LOAI_PHAM_ID: val.ID,
                            NAM: $scope.NAM,
                            KY: $scope.KY,
                            SL_NAM: 0,
                            SL_NU: 0,
                            TONG: 0,
                        })
                    })

                }
            }
        });
    }
    $scope.Them = function () {
        $scope.ListPham.push(
            {
                'ID': 0,
                'LOAI_PHAM_ID': '',
                'SL_NAM': '',
                'SL_NU': '',
                'TONG': '',
                'DON_VI_ID': $scope.DON_VI_ID,
                'KY': $scope.KY,
                'NAM': $scope.NAM,
            });
    }

    $scope.CheckAll = function () {
        angular.forEach($scope.ListPham, function (val, key) {
            if ($scope.SeclectAll)
                val.selected = false;
            else
                val.selected = true;
        });
    }
    $scope.Xoa = function () {
        $scope.ListPhamDelete = $scope.ListPham.filter(x => x.selected);
        $scope.ListPham = $scope.ListPham.filter(x => !x.selected);
        if ($scope.ListPham.length === 0)
            $scope.SeclectAll = false;
    }
    $scope.Luu = function () {
        $scope.checkSave = true;
        angular.forEach($scope.ListPham, function (val, key) {
            if (val.SL_NAM <= 0) {
                toastr.error("Số lượng nam không được < 0");
                $scope.checkSave = false;
            }
            if (val.SL_NU <= 0) {
                toastr.error("Số lượng nữ không được < 0");
                $scope.checkSave = false;
            }
        });
        if ($scope.checkSave === true) {
            $.ajax({
                type: 'post',
                async: false,
                url: '/QSSoLuongPham/Save',
                data: { listItem: $scope.ListPham, listItemDelete: $scope.ListPhamDelete },
                success: function (res) {
                    if (res.Error) {
                        toastr.error(res.Title);
                    } else {
                        toastr.success(res.Title);
                    }
                }
            });
        }
    }
    //#endregion
});