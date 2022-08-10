app.controller("QSChuyenDoiLLTrongKyController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location) {
    //#region Khởi tạo
    $scope.treeLucLuong;
    $scope.treeCapBac;

    angular.element(document).ready(function () {
        $scope.GetDanhMuc();
        $scope.IsShowRight = false;

        $scope.ListNam = [];
        var currentYear = new Date().getFullYear();
        for (var i = currentYear; i > currentYear - 7; i--) {
            $scope.ListNam.push({ Id: i, Name: i });
        }
        $scope.NAM = currentYear.toString();
        $scope.KY = '1';
    })
    $scope.GetDanhMuc = function () {
        $scope.ListPageMenu = [];
        $scope.ListUnit = [];
        $scope.ListAppCode = [];
        $.ajax({
            type: 'post',
            url: '/QSTanBinhTrongKy/GetDanhMuc',
            data: {},
            success: function (data) {
                hideLoading();
                //#region Lực lượng
                treeLucLuongSource = [];
                if (data.LucLuongs != null && data.LucLuongs.length > 0) {
                    var lucLuong0s = data.LucLuongs.filter(function (x) {
                        return (x.ParentId == 0);
                    });
                    if (lucLuong0s != null && lucLuong0s.length > 0) {
                        for (var i = 0; i < lucLuong0s.length; i++) {

                            var comboTree = { id: 0, title: 'Lực lượng' };
                            // kiểm tra có con không
                            var childs = data.LucLuongs.filter(function (x) {
                                return (x.ParentId == lucLuong0s[i].Id);
                            });

                            if (childs != null && childs.length > 0) {
                                $scope.ConvertTree(comboTree, data.LucLuongs, lucLuong0s[i].Id);
                            }
                            treeLucLuongSource.push(comboTree);

                        }
                    }
                }

                $scope.treeLucLuong = $('#luc-luong').comboTree({
                    source: treeLucLuongSource,
                    isMultiple: false
                });
                $scope.treeLucLuongTab2 = $('#luc-luong-tab2').comboTree({
                    source: treeLucLuongSource,
                    isMultiple: false
                });

                //#endregion
                //#region nhóm cấp bậc
                treeNhomCapBacSource = [];
                if (data.NhomCapBacs != null && data.NhomCapBacs.length > 0) {
                    var nhomCapBac0s = data.NhomCapBacs.filter(function (x) {
                        return (x.ParentId == 0);
                    });
                    if (nhomCapBac0s != null && nhomCapBac0s.length > 0) {
                        for (var i = 0; i < nhomCapBac0s.length; i++) {

                            var comboTree = { id: nhomCapBac0s[i].Id, title: nhomCapBac0s[i].Name };
                            // kiểm tra có con không
                            var childNhomCB = data.NhomCapBacs.filter(function (x) {
                                return (x.ParentId == nhomCapBac0s[i].Id);
                            });

                            if (childNhomCB != null && childNhomCB.length > 0) {
                                $scope.ConvertTree(comboTree, data.NhomCapBacs, nhomCapBac0s[i].Id);
                            }
                            treeNhomCapBacSource.push(comboTree);

                        }
                    }
                }

                $scope.treeCapBac = $('#nhom-cap-bac').comboTree({
                    source: treeNhomCapBacSource,
                    isMultiple: false
                });
                $scope.treeCapBacTab2 = $('#nhom-cap-bac-tab2').comboTree({
                    source: treeNhomCapBacSource,
                    isMultiple: false
                });
                //#endregion
                $scope.ListLoaiHam = data.LoaiHams;
                $scope.$apply();
            }
        });
    }
    $scope.GetDanhMuc();

    $scope.ChangeLucLuong = function ($event) {
        if ($scope.treeLucLuong !== undefined)
            $scope.LucLuongId = $scope.treeLucLuong._selectedItem != undefined ? $scope.treeLucLuong._selectedItem.id : null;
    }
    $scope.ChangeNhomCapBac = function () {
        if ($scope.treeCapBac !== undefined)
            $scope.NhomCapBacId = $scope.treeCapBac._selectedItem !== undefined ? $scope.treeCapBac._selectedItem.id : null;
    }

    $scope.ChangeLucLuongTab2 = function ($event) {
        if ($scope.treeLucLuongTab2 !== undefined)
            $scope.LucLuongIdTab2 = $scope.treeLucLuongTab2._selectedItem != undefined ? $scope.treeLucLuongTab2._selectedItem.id : null;
    }
    $scope.ChangeNhomCapBacTab2 = function () {
        if ($scope.treeCapBacTab2 !== undefined)
            $scope.NhomCapBacIdTab2 = $scope.treeCapBacTab2._selectedItem !== undefined ? $scope.treeCapBacTab2._selectedItem.id : null;
    }
    $scope.ConvertTree = function (lstTreeModel, data, Id) {
        var lstPageMenu = data.filter(function (x) {
            return (x.ParentId == Id);
        });

        if (lstPageMenu != null && lstPageMenu.length > 0) {
            lstTreeModel.subs = [];
            for (var i = 0; i < lstPageMenu.length; i++) {
                var tree =
                {
                    id: lstPageMenu[i].Id,
                    title: lstPageMenu[i].Name
                };
                // Kiểm tra xem có con không
                var dataChilds = data.filter(function (x) {
                    return (x.ParentId == lstPageMenu[i].Id);
                });
                if (dataChilds != null && dataChilds.length > 0) {
                    $scope.ConvertTree(tree, data, lstPageMenu[i].Id);
                }
                lstTreeModel.subs.push(tree);
            }

        }
    }

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
        if (treeNode !== undefined && treeNode.id !== undefined) {
            $scope.TEN_DON_VI = treeNode.name;
            $scope.DON_VI_ID = treeNode.id.split('_')[1];
            $scope.IsShowRight = true;
            $scope.LoadDataChuyenLLTrongKy($scope.DON_VI_ID);
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

    //#region Quân số chuyển đổi lực lượng trong kỳ
    $scope.LoadDataChuyenLLTrongKy = function (donViId) {
        $.ajax({
            type: 'post',
            url: '/QSChuyenDoiLLTrongKy/GetDanhSach',
            data: { donViId: donViId, lucLuongId: $scope.LucLuongId, capBacId: $scope.capBacId},
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.ListData = res.data;
                    $scope.ListDataNB = res.dataNB;
                    $scope.$apply();
                }
            }, error: function (xhr, ajaxOptions, thrownError) {
                console.log(xxhr.responseText)
            }
        })
    }
   //#enndregion
    //$.ajax({
    //    type: 'post',
    //    url: '/User/GetDanhMuc',
    //    data: {},
    //    success: function (data) {
    //        angular.forEach(data.Units, function (val, key) {
    //            ListUnit.push({ id: val.UNIT_ID, name: val.UNIT_NAME });
    //        });
    //    }
    //});
    $scope.fdate = function (date) {
        return moment(date).format('DD/MM/YYYY');
    }
}); 