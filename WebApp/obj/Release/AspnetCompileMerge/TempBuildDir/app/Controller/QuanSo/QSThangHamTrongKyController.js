app.controller("QSThangHamTrongKyController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location, $rootScope) {
    //#region Khởi tạo
    $scope.treeLucLuong;
    $scope.treeCapBac;
    $scope.IsShowRight = false;

    $scope.ListNam = [];
    var currentYear = new Date().getFullYear();
    for (var i = currentYear; i > currentYear - 7; i--) {
        $scope.ListNam.push({ Id: i, Name: i });
    }
    $scope.NAM = currentYear.toString();
    $scope.KY = '1';


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

                $scope.treeLucLuong = $('#luc-luong-index').comboTree({
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

                $scope.treeCapBac = $('#nhom-cap-bac-index').comboTree({
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

    $scope.ChangeLucLuong = function () {
        if ($scope.treeLucLuong !== undefined)
            $scope.LucLuongId = $scope.treeLucLuong._selectedItem != undefined ? $scope.treeLucLuong._selectedItem.id : null;
    }
    $scope.ChangeNhomCapBac = function () {
        if ($scope.treeCapBac !== undefined)
            $scope.NhomCapBacId = $scope.treeCapBac._selectedItem !== undefined ? $scope.treeCapBac._selectedItem.id : null;
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
    //#endregion
    $scope.AddThangHamTrongKy = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/QSThangHamTrongKy/_AddThangHamTrongKy',
            controller: 'addThangHamTrongKy',
            size: 'xl',
            backdrop: 'static'
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            var data = $scope.Json2Arrary($rootScope.JxlQSThangHamTrKy.data);
            $scope.tblThangHam.setData(data);
        });
    };

    //#region tạo treeview
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
            $scope.LoadDataThangHam($scope.DON_VI_ID);
            $scope.IsShowRight = true;
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
    function getSearchParams(k) {
        var p = {};
        location.search.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (s, k, v) { p[k] = v })
        return k ? p[k] : p;
    }
    $scope.txnId = 0;
    $scope.txnId = getSearchParams('txnid');
    var ListUnit = [];

    //#region QS thăng hàm trong kỳ
    $scope.JxlQSThangHamTrKy = {};
    $scope.LoadDataThangHam = function (donViId) {
        $scope.JxlQSThangHamTrKy.data = [];
        $.ajax({
            type: 'post',
            async: false,
            url: '/QSThangHamTrongKy/GetDanhSach',
            data: { donViId: donViId, lucLuongId: $scope.LucLuongId, capBacId: $scope.capBacId },
            success: function (response) {
                if (response.Error) {
                    toastr.error(response.Title);
                } else {
                    if (response.data.length > 0) {
                        angular.forEach(response.data, function (val, key) {
                            $scope.ItemJexcel = [val.ID, val.LUC_LUONG_ID_CU, val.CAP_BAC_ID_MOI, val.LOAI_HAM_CU_ID, val.DK_PHONG_HAM, 0, 0, 0];
                            $scope.JxlQSThangHamTrKy.data.push($scope.ItemJexcel);
                            $scope.tblThangHam.setData($scope.Json2Arrary($scope.JxlQSThangHamTrKy.data));
                        });
                    } else {
                        $scope.JxlQSThangHamTrKy.data = [];
                        $scope.tblThangHam.setData($scope.JxlQSThangHamTrKy.data);
                    }
                }
            }
        });
    }

    $scope.JxlQSThangHamTrKy.colHeaders = ['', 'A', 'B', 'C', 'D', 'E', 'F', 'G'];
    $scope.JxlQSThangHamTrKy.column = [
        { type: 'text', width: '0' },
        { type: 'dropdown', autocomplete: true, width: '300', url: '/ChotQuanSoDauKy/GetDMLucLuongJexcel' },
        { type: 'dropdown', autocomplete: true, width: '300', url: '/ChotQuanSoDauKy/GetDMCapBacJexcel' },
        { type: 'dropdown', autocomplete: true, width: '120', title: 'D', url: '/ChotQuanSoDauKy/GetDMLoaiHamJexcel' },
        { type: 'text', width: '150' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '100' }
    ];
    $scope.JxlQSThangHamTrKy.nestedHeaders = [
        [
            { title: '', rowspan: '2' },
            { title: 'Lực lượng ', rowspan: '2' },
            { title: 'Cấp bậc mới ', rowspan: '2' },
            { title: 'Loại hàm ', rowspan: '2' },
            { title: 'Dự kiến phong hàm ', rowspan: '2' },
            { title: 'Thực tế', colspan: '3' }
        ], [
            { title: 'Nữ' },
            { title: 'Nam' },
            { title: 'Tổng cộng' }
        ]
    ];

    $scope.JxlQSThangHamTrKy.footers = [['', 'Tổng', '', '', '', '', '', '']];
    $scope.JxlQSThangHamTrKy.changed = function (instance, cell, x, y, value) {
    };
    //#endregion  

    $scope.ThangHamCallBack = function (data) {
        $scope.tblThangHam = data;
    }

    $.ajax({
        type: 'post',
        url: '/User/GetDanhMuc',
        data: {},
        success: function (data) {
            angular.forEach(data.Units, function (val, key) {
                ListUnit.push({ id: val.UNIT_ID, name: val.UNIT_NAME });
            });
        }
    });
    $scope.SearhData = function () {
        $scope.JxlQSThangHamTrKy.data = [];
        $.ajax({
            type: 'post',
            async: false,
            url: '/QSThangHamTrongKy/GetDanhSach',
            data: { donViId: $scope.DON_VI_ID, lucLuongId: $scope.LucLuongId, capBacId: $scope.capBacId },
            success: function (response) {
                if (response.Error) {
                    toastr.error(response.Title);
                } else {
                    if (response.data.length > 0) {
                        angular.forEach(response.data, function (val, key) {
                            $scope.ItemJexcel = [val.ID, val.LUC_LUONG_ID_CU, val.CAP_BAC_ID_MOI, val.LOAI_HAM_CU_ID, val.DK_PHONG_HAM, 0, 0, 0];
                            $scope.JxlQSThangHamTrKy.data.push($scope.ItemJexcel);
                            
                        });
                    } else {
                        $scope.JxlQSThangHamTrKy.data = [];
                    }
                    $scope.tblThangHam.setData($scope.Json2Arrary($scope.JxlQSThangHamTrKy.data));
                }
            }
        });
    }
    $scope.Save = function () {
        $scope.ListInsert = [];
        $scope.ListUpdate = [];
        $scope.ListDelete = [];
        angular.forEach($scope.tblThangHam.getJson(), function (val, key) {
            $scope.ItemData = {};
            $scope.ItemData.ID = val[0];
            $scope.ItemData.SO_HIEU = '000-000';
            $scope.ItemData.DON_VI_CU_ID = $scope.DON_VI_ID;
            $scope.ItemData.LUC_LUONG_ID_CU = val[1];
            $scope.ItemData.CAP_BAC_ID_MOI = val[2];
            $scope.ItemData.LOAI_HAM_CU_ID = val[3];
            $scope.ItemData.DK_PHONG_HAM = val[4];
            $scope.ItemData.NAM = $scope.NAM;
            $scope.ItemData.KY = $scope.KY;
            $scope.ItemData.NGAY_TAO = moment(new Date()).format();
            $scope.ItemData.NGAY_CAP_NHAT = moment(new Date()).format();
            if (val[0] === '' || val[0] === 0)
                $scope.ListInsert.push($scope.ItemData);
            else
                $scope.ListUpdate.push($scope.ItemData);
        });
        $.ajax({
            type: 'post',
            url: '/QSThangHamTrongKy/UpdateData',
            data: { listThem: $scope.ListInsert, listSua: $scope.ListUpdate, listXoa: $scope.ListDelete },
            success: function (res) {
                if (res.Error) {
                    toastr.error('Có lỗi xảy ra');
                } else {
                    toastr.success('Cập nhật dữ liệu thành công');
                    $scope.LoadDataThangHam($scope.DON_VI_ID);
                }
            }
        })
    }
    $scope.Json2Arrary = function (data) {
        var lstJson = [];
        angular.forEach(data, function (val, key) {
            var itemData = {};
            angular.forEach(val, function (v, k) {
                itemData[k] = v;
            });
            lstJson.push(itemData);
        });
        return lstJson;
    }
});

app.controller('addThangHamTrongKy', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading, $rootScope) {
    $scope.model = {};
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;

    $scope.treeLucLuong;
    $scope.treeCapBac;

    $scope.LoadPage = function () {
        showToast();
        $.ajax({
            type: 'post',
            url: '/QSTanBinhTrongKy/DsDmCapBacLoaiHam',
            data: { currentPage: $scope.modelSearch.currentPage, recordPerPage: $scope.modelSearch.pageSize, LucLuongId: $scope.LucLuongId, NhomCapBacId: $scope.NhomCapBacId, LoaiHamId: $scope.LoaiHamId },
            success: function (response) {
                if (response != null) {
                    $scope.dsDmCBacLHam = response.data;
                    $scope.modelSearch.totalItems = response.total;
                    $scope.$apply();
                }
                else {
                    toastr.success('Có lỗi xảy ra trong quá trình lấy dữ liệu!');
                }
                hideLoading();
            }
        });
    }
    $scope.LoadPage();
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

                //#endregion
                $scope.ListLoaiHam = data.LoaiHams;
                $scope.$apply();
            }
        });
    }
    $scope.GetDanhMuc();
    $scope.ChangeLucLuong = function () {
        if ($scope.treeLucLuong !== undefined)
            $scope.LucLuongId = $scope.treeLucLuong._selectedItem != undefined ? $scope.treeLucLuong._selectedItem.id : null;
    }
    $scope.ChangeNhomCapBac = function () {
        if ($scope.treeCapBac !== undefined)
            $scope.NhomCapBacId = $scope.treeCapBac._selectedItem !== undefined ? $scope.treeCapBac._selectedItem.id : null;
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

    $scope.pageChanged = function () {
        $scope.LoadPage();
    }
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
    $scope.SearchTieuChuan = function () {
        $scope.LoadPage();
    }
    $scope.ChonTieuChuan = function () {

        $scope.cBacLoaiHamArray = [];
        $rootScope.JxlQSThangHamTrKy = {};
        $rootScope.JxlQSThangHamTrKy.data = [];
        angular.forEach($scope.dsDmCBacLHam, function (dsDmCBacLHam) {
            if (!!dsDmCBacLHam.Selected) {
                $scope.cBacLoaiHamArray.push(dsDmCBacLHam.ID);
            }
        })

        //Add dữ liệu tiêu chuẩn
        angular.forEach($scope.cBacLoaiHamArray, function (val, key) {
            var item = $scope.dsDmCBacLHam.find(x => x.ID === val);
            $rootScope.ItemJexcel = ['', item.LUC_LUONG_ID, item.CAP_BAC_ID, item.LOAI_HAM_ID, 0, 0, 0, 0];
            $rootScope.JxlQSThangHamTrKy.data.push($rootScope.ItemJexcel);
        });
        $uibModalInstance.close();
    };
    $scope.checkAll = function () {
        angular.forEach($scope.dsDmCBacLHam, function (item) {
            item.Selected = event.target.checked;
        });
    };
});