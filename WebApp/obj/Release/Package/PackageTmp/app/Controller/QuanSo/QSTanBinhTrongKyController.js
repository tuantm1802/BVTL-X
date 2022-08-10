app.controller("QSTanBinhTrongKyController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location, $rootScope) {
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
    $scope.AddTanBinhTrongKy = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/QSTanBinhTrongKy/_AddTanBinhTrongKy',
            controller: 'addTanBinhTrongKy',
            size: 'xl',
            backdrop: 'static'
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
           
        });
    };
    //#region tạo treeview
    $scope.treeData = {};
    $scope.treeData.data = {};
    $scope.ListDonVi;
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
            $scope.LoadDataQuanSoTBTrongKy($scope.DON_VI_ID);
            $scope.IsShowRight = true;
            $scope.$apply();
        }
        hideLoading();
    }
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

    //#region Quân số tân binh trong kỳ
    $scope.tree_Table_Checkbox = true;
    $rootScope.tree_data = [];
    $scope.LoadDataQuanSoTBTrongKy = function (donVi) {
        $.ajax({
            type: 'post',
            url: '/QSTanBinhTrongKy/DanhSachTanBinh',
            data: { donViId: $scope.DON_VI_ID },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $rootScope.tree_data = res.data;
                    if (!$scope.$$phase)
                        $scope.$apply();
                }
            }
        })
    }
    $scope.LoadDataQuanSoTBTrongKy();
    // cấu hình hiển thị cùng với icon
    $scope.expanding_property = {
        field: "lucLuong",
        displayName: "Lực lượng",
        width: "30%",
        rowspan: 1,
        colspan: 1
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.col_defs = [[
        { field: "capBac", displayName: "Cấp bậc", width: "15%", rowspan: 1, colspan: 1 },
        { field: "loaiHam", displayName: "Loại hàm", width: "20%", rowspan: 1, colspan: 1 },
        { field: "loaiTanBinh", displayName: "Loai tân binh", width: "10%", rowspan: 1, colspan: 1 },
        { field: "slNam", displayName: "Nam", width: "10%", rowspan: 1, colspan: 1 },
        { field: "slNu", displayName: "Nữ", width: "10%", rowspan: 1, colspan: 1 },
    ]];

    $scope.body_defs = [{ field: "capBac" }, { field: "loaiHam" }, { field: "loaiTanBinh" }, { field: "slNam" }, { field: "slNu" }];
    // event when change value checkbox
    $scope.ChangeTreeTableCheckbox = function (item) {
        console.log(item);
    };

    // event when click choose row
    $scope.ClickTreeTableRow = function (item) {
        console.log(item);
    };
    //#endregion  

    $scope.ExportTemplate = function () {
        window.location.href = '/QSTanBinhTrongKy/ExportTemplate';
    }

    $scope.Save = function () {
        $scope.ListInsert = [];
        $scope.ListUpdate = [];
        $scope.ListDelete = [];
        angular.forEach($scope.tblTanBinh.getJson(), function (val, key) {
            $scope.ItemData = {};
            $scope.ItemData.DU_TOAN_ID = val[0];
            $scope.ItemData.DON_VI_ID = $scope.DON_VI_ID;
            $scope.ItemData.NGAY_TAO = moment(val[1].split('/')[2] + '-' + val[1].split('/')[1] + '-' + val[1].split('/')[0]).format();
            $scope.ItemData.LOAI_TAN_BINH = val[2];
            $scope.ItemData.LUC_LUONG_ID = val[3];
            $scope.ItemData.CAP_BAC_ID = val[4];
            $scope.ItemData.NAM = $scope.NAM;
            $scope.ItemData.KY = $scope.KY;
            $scope.ItemData.SL_NAM = val[5];
            $scope.ItemData.SL_NU = val[6];
            $scope.ItemData.TONG = parseInt(val[5]) + parseInt(val[6]);
            if (val.DU_TOAN_ID === '' || val.DU_TOAN_ID === 0 && val.DU_TOAN_ID !== undefined)
                $scope.ListInsert.push($scope.ItemData);
            else
                $scope.ListUpdate.push($scope.ItemData);
        });
        $.ajax({
            type: 'post',
            url: '/QSTanBinhTrongKy/UpdateData',
            data: { listThem: $scope.ListInsert, listSua: $scope.ListUpdate, listXoa: $scope.ListDelete },
            success: function (res) {
                if (res.Error) {
                    toastr.error('Có lỗi xảy ra');
                } else {
                    toastr.success('Cập nhật dữ liệu thành công');
                    $scope.LoadDataQuanSoTBTrongKy($scope.DON_VI_ID);
                }
            }
        })
    }
    $scope.SearhData = function () {
        $scope.JxlQSTanBinhTrKy.data = [];
        $.ajax({
            type: 'post',
            async: false,
            url: '/QSTanBinhTrongKy/GetDanhSach',
            data: { donViId: $scope.DON_VI_ID, lucLuongId: $scope.LucLuongId, capBacId: $scope.capBacId },
            success: function (response) {
                if (response.Error) {
                    toastr.error(response.Title);
                } else {
                    angular.forEach(response.data, function (val, key) {
                        $scope.ItemJexcel = [val.DU_TOAN_ID, moment(val.NGAY_TAO).format('DD/MM/YYYY'), val.LOAI_TAN_BINH, val.LUC_LUONG_ID, val.CAP_BAC_ID, val.SL_NAM, val.SL_NU, val.TONG]
                        $scope.JxlQSTanBinhTrKy.data.push($scope.ItemJexcel);
                    });
                    $scope.tblTanBinh.setData($scope.Json2Arrary($scope.JxlQSTanBinhTrKy.data));
                }
            }
        });
    }
});
app.controller('addTanBinhTrongKy', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading, $rootScope) {
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

    $scope.ChangeLucLuong = function ($event) {
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
    $scope.LoadPage();

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
        if ($scope.LoaiTanBinh === null || $scope.LoaiTanBinh === undefined) {
            toastr.error('Bạn phải chọn loại tân binh');
        } else {
            $rootScope.cBacLoaiHamArray = [];
            angular.forEach($scope.dsDmCBacLHam, function (dsDmCBacLHam) {
                if (!!dsDmCBacLHam.Selected) {
                    $scope.cBacLoaiHamArray.push(dsDmCBacLHam.ID);
                    //Add dữ liệu tiêu chuẩn
                    $scope.ItemData = {};
                    var checkLL = $rootScope.tree_data.find(x => x.lucLuong === dsDmCBacLHam.TEN_LUC_LUONG);
                    if (checkLL == null || checkLL == undefined) {

                        if (checkDate !== null && checkDate !== undefined) {
                            $scope.ItemData.url = '';
                            $scope.ItemData.expanded = false;
                            $scope.ItemData.type = 2;
                            $scope.ItemData.checkbox = false;
                            $scope.ItemData.selected = false;
                            $scope.ItemData.lucLuong = currentDate;
                            $scope.ItemData.children = $scope.CreateChild($scope.cBacLoaiHamArray);
                        }
                        else {
                            $scope.ItemData.url = '';
                            $scope.ItemData.expanded = false;
                            $scope.ItemData.type = 2;
                            $scope.ItemData.checkbox = false;
                            $scope.ItemData.selected = false;
                            $scope.ItemData.lucLuong = currentDate;
                            $scope.ItemData.children = $scope.CreateChild($scope.cBacLoaiHamArray);
                        }
                        $rootScope.tree_data.push($scope.ItemData);
                    }
                }
            })

           
            $uibModalInstance.close();
        }
    };
    $scope.CreateDate = function () {
        var currentDate = moment(new Date).format('DD/MM/YYYY');
        var checkDate = $rootScope.tree_data.find(x => x.lucLuong === currentDate);
        if (checkDate !== null && checkDate !== undefined) {
            $scope.ItemData.url = '';
            $scope.ItemData.expanded = false;
            $scope.ItemData.type = 2;
            $scope.ItemData.checkbox = false;
            $scope.ItemData.selected = false;
            $scope.ItemData.lucLuong = currentDate;
            $scope.ItemData.children = $scope.CreateChild($scope.cBacLoaiHamArray);
        }
        else {
            $scope.ItemData.url = '';
            $scope.ItemData.expanded = false;
            $scope.ItemData.type = 2;
            $scope.ItemData.checkbox = false;
            $scope.ItemData.selected = false;
            $scope.ItemData.lucLuong = currentDate;
            $scope.ItemData.children = $scope.CreateChild($scope.cBacLoaiHamArray);
        }
    }
    $scope.CreateChild = function (data) {
        $rootScope.tree_data_item = [];
        angular.forEach(data, function (val, key) {
            var item = $scope.dsDmCBacLHam.find(x => x.ID === val);
            $scope.ItemData = {};
            $scope.ItemData.url = '';
            $scope.ItemData.expanded = false;
            $scope.ItemData.type = 2;
            $scope.ItemData.checkbox = false;
            $scope.ItemData.selected = false;
            $scope.ItemData.lucLuong = "";
            $scope.ItemData.capBac = item.TEN_CB;
            $scope.ItemData.loaiTanBinh = $scope.LoaiTanBinh === 1 ? "Tân binh" : "Tân binh ra trường";
            $scope.ItemData.slNam = 0;
            $scope.ItemData.slNu = 0;

            $rootScope.tree_data_item.push($scope.ItemData);
        });
        return $rootScope.tree_data_item;
    }
    $scope.checkAll = function () {
        angular.forEach($scope.dsDmCBacLHam, function (item) {
            item.Selected = event.target.checked;
        });
    };
});