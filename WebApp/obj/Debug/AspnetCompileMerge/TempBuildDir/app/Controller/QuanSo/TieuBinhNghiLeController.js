app.controller("TieuBinhNghiLeController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location, $rootScope, constant) {
    $scope.treeLucLuong;
    $scope.treeCapBac;
    $scope.rdoDonVi = 1;

    $scope.SLNamTong = 0;
    $scope.SLNuTong = 0;

    $scope.ListNam = [];
    var currentYear = new Date().getFullYear();
    for (var i = currentYear; i > currentYear - 7; i--) {
        $scope.ListNam.push({ Id: i, Name: i });
    }
    $scope.cbbNam = currentYear.toString();
    $scope.NAM_HIEN_TAI = currentYear.toString();

    $scope.IsVisibleTab = false;

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
        if (treeNode !== undefined && treeNode.id !== undefined && treeNode.isParent === false) {
            $scope.TEN_DON_VI = treeNode.name;
            $scope.DON_VI_ID = treeNode.id.split('_')[1];
            //Load danh sách theo đơn vị, năm, kỳ
            $scope.LoadDsTieuBinh();
            $scope.IsVisibleTab = true;
            $scope.$apply();
        }
        else {
            hideLoading();
            $scope.IsVisibleTab = false;
            $scope.$apply();
        }
    }
    $scope.treeData = {};
    $scope.treeData.data = {};
    $scope.ListDonVi;

    $.ajax({
        type: 'get',
        async: false,
        url: '/TieuBinhNghiLe/GetTreeData',
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

    //Check kiểu hiển thị tree Đơn vị
    $scope.ChangeTreeViewDmDonVi = function (type) {
        var api = '/TieuBinhNghiLe/GetTreeData';
        if (type === 1) {
            $scope.rdoDonVi = type;
            api = '/TieuBinhNghiLe/GetTreeData';
        }
        else {
            $scope.rdoDonVi = type;
            api = '/TieuBinhNghiLe/GetTreeDataKhuVuc';
        }
        $.ajax({
            type: 'get',
            async: false,
            url: api,
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
    }

    $scope.btnSearchDmDonVi = function () {
        var sp = $scope.txtSearchDmDonVi;
        if (sp !== null) {
            console.log(sp);

        } else {
            console.log("Đang xử lý");
        }
    }
    //#endregion tạo treeview

    $scope.ShowHideSearch = function (show) {
        if (show) {
            $(".cHide").show();
            $(".cShow").hide();
        } else {
            $(".cShow").show();
            $(".cHide").hide();
        }
    };

    $scope.NhomCapBacId = null;
    //#region Danh mục tìm kiếm cấp bậc, loại hàm
    $scope.GetDanhMuc = function () {
        $.ajax({
            type: 'get',
            url: '/TieuBinhNghiLe/GetDanhMuc',
            success: function (data) {
                hideLoading();
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

                $scope.treeCapBac = $('#nhom-cap-bac-tb').comboTree({
                    source: treeNhomCapBacSource,
                    isMultiple: false
                });

                //#endregion
                $scope.ListLoaiHam = data.LoaiHams;
                $scope.ListCapBac = data.CapBacs;
                $scope.$apply();
            }
        });
    }
    $scope.GetDanhMuc();
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


    function getSearchParams(k) {
        var p = {};
        location.search.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (s, k, v) { p[k] = v })
        return k ? p[k] : p;
    }
    $scope.txnId = 0;
    $scope.txnId = getSearchParams('txnid');

    $scope.SearchTieuBinhDs = function () {
        $scope.LoadDsTieuBinh();
    }
    //#region QS tiêu binh nghi lễ 
    $scope.LoadDsTieuBinh = function () {
        $.ajax({
            type: 'post',
            async: false,
            url: '/TieuBinhNghiLe/DsTieuBinhNghiLeTheoKy',
            data: { dvId: $scope.DON_VI_ID, iNam: $scope.cbbNam, NhomCapBacId: $scope.NhomCapBacId, LoaiHamId: $scope.LoaiHamId },
            success: function (response) {
                if (response.Error) {
                    toastr.error(response.Title);
                } else {
                    $rootScope.ListTieuBinhNghiLe = response.data;
                    $scope.SLNuTong = 0;
                    $scope.SLNamTong = 0;
                    angular.forEach($rootScope.ListTieuBinhNghiLe, function (val, key) {
                        $scope.SLNamTong += val.SL_NAM_HT;
                        $scope.SLNuTong += val.SL_NU_HT;
                    });
                }
                hideLoading();
            }
        });
    }

    $scope.GetCapBacText = function (itemId) {
        $scope.CapbacText = $scope.ListCapBac.find(x => x.id === itemId);
        if ($scope.CapbacText !== null && $scope.CapbacText !== undefined)
            return $scope.CapbacText.name;
        else
            return "";
    }

    $scope.GetLoaiHamText = function (itemId) {
        $scope.ListLoaiHamText = $scope.ListLoaiHam.find(x => x.ID === itemId);
        if ($scope.ListLoaiHamText !== null && $scope.ListLoaiHamText !== undefined)
            return $scope.ListLoaiHamText.LOAI_HAM;
        else
            return "";
    }

    $scope.btnHuyQsTieuBinh = function () {
        $scope.ListData = [];
        if ($rootScope.JxlQuanSoTBNL.data.length > 0) {
            angular.forEach($rootScope.JxlQuanSoTBNL.data, function (val, key) {
                if (val[0] !== '' && val[0] !== 0) {
                    $scope.ListData.push(val);
                }
            });
            $scope.tblTieBinh.setData($scope.Json2Arrary($scope.ListData));
        }
    }

    $scope.btnLuuQsTieuBinh = function () {
        if ($rootScope.ListTieuBinhNghiLe.length > 0) {
            $scope.ListInsert = [];
            $scope.ListUpdate = [];

            angular.forEach($rootScope.ListTieuBinhNghiLe, function (value, key) {
                var k = key + 1;
                $scope.itemModel = {};
                $scope.itemModel.ID = value.ID;
                $scope.itemModel.DON_VI_ID = $scope.DON_VI_ID;
                $scope.itemModel.LUC_LUONG_ID = 0;
                $scope.itemModel.CAP_BAC_ID = value.CAP_BAC_ID;
                $scope.itemModel.LOAI_HAM_ID = value.LOAI_HAM_ID;
                $scope.itemModel.NAM = $scope.cbbNam;
                $scope.itemModel.KY = $scope.cbbKy;
                $scope.itemModel.SL_NAM_DK = 0;
                $scope.itemModel.SL_NU_DK = 0;
                $scope.itemModel.SL_NAM_HT = value.SL_NAM_HT;
                $scope.itemModel.SL_NU_HT = value.SL_NU_HT;
                $scope.itemModel.SL_TONG_HT = value.SL_NAM_HT + value.SL_NU_HT;
                $scope.itemModel.SL_NAM_CK = 0;
                $scope.itemModel.SL_NU_CK = 0;
                $scope.itemModel.SL_TONG_CK = 0;
                $scope.itemModel.SL_NAM_REAL = 0;
                $scope.itemModel.SL_NU_REAL = 0;
                $scope.itemModel.SL_TONG_REAL = 0;

                if (value.ID === '' || value.ID === 0 || value.ID === undefined)
                    $scope.ListInsert.push($scope.itemModel);
                else
                    $scope.ListUpdate.push($scope.itemModel);
            });
            if ($scope.ListUpdate.length > 0 || $scope.ListInsert.length > 0) {
                $.ajax({
                    type: 'post',
                    url: '/TieuBinhNghiLe/DieuChinhDSTieuBinh',
                    data: { listThem: $scope.ListInsert, listSua: $scope.ListUpdate, listXoa: $scope.ListDelete },
                    success: function (rs) {
                        if (rs.Error) {
                            toastr.error(rs.Title);
                        }
                        else {
                            toastr.success(rs.Title);
                            $scope.ListDelete = [];
                            $scope.LoadDsTieuBinh();
                        }
                    }
                })
            }
            else {
                toastr.error("Danh sách trống xin vui lòng kiểm tra lại!");
            }
        }
        else {
            toastr.error("Lưu thất bại!");
        }
    }

    $scope.Delete = function () {
        $scope.ListDelete = [];
        angular.forEach($rootScope.ListTieuBinhNghiLe, function (val, key) {
            if (val.Selected) {
                if (val.ID > 0)
                    $scope.ListDelete.push(val.ID);
                $rootScope.ListTieuBinhNghiLe.splice(key, 1)
                $.ajax({
                    type: 'post',
                    url: '/TieuBinhNghiLe/DieuChinhDSTieuBinh',
                    data: { listThem: [], listSua: [], listXoa: $scope.ListDelete },
                    success: function (rs) {
                        if (rs.Error) {
                            toastr.error(rs.Title);
                        }
                        else {
                            toastr.success("Xóa thành công");
                            $scope.ListDelete = [];
                        }
                    }
                })
            }
        });
        console.log($scope.ListDelete);
    }

    $scope.ChangeSoLuong = function (gender) {
        if (gender === 'nam') {
            $scope.SLNamTong = 0;
            angular.forEach($rootScope.ListTieuBinhNghiLe, function (val, key) {
                $scope.SLNamTong += val.SL_NAM_HT;
            });
        }
        if (gender === 'nu') {
            $scope.SLNuTong = 0;
            angular.forEach($rootScope.ListTieuBinhNghiLe, function (val, key) {
                $scope.SLNuTong += val.SL_NU_HT;
            });
        }
    }
    //#endregion  

    $scope.ExportTemplate = function () {
        window.location.href = '/TieuBinhNghiLe/ExportTemplate';
    }

    //Thêm mới danh sách tiêu binh
    $scope.AddTBTheoCapBacLoaiHamPartial = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/TieuBinhNghiLe/_DmCapBacLoaiHamPartial',
            controller: 'AddTBTheoCapBacLoaiHamPartial',
            size: 'lg',
            backdrop: 'static'
        });
        modalInstance.result.then(function (response) {

        });
    };
    $scope.ImpFileTieuBinh = function (donViId, nam) {
        console.log(donViId, nam);
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            modalTemplate: '<div class="" ng-transclude></div>',
            templateUrl: '/TieuBinhNghiLe/_ImpFileTieuBinh',
            controller: 'ImpFileTieuBinhController',
            size: 'xl',
            backdrop: 'show',
            resolve: {
                data: function () {
                    return {
                        donViId: donViId,
                        nam: nam
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function () {
            $scope.LoadDsTieuBinh();
            $scope.$apply();
        });
    };
    $scope.checkAll = function () {
        angular.forEach($rootScope.ListTieuBinhNghiLe, function (item) {
            item.Selected = event.target.checked;
        });
    };
});
//Thêm mới danh sách tiêu binh
app.controller('AddTBTheoCapBacLoaiHamPartial', function ($scope, $rootScope, $uibModalInstance, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.model = {};

    //$scope.treeLucLuong;
    $scope.treeCapBac;


    //#region Danh mục tìm kiếm cấp bậc, loại hàm
    $scope.GetDanhMuc = function () {
        $.ajax({
            type: 'post',
            url: '/TieuBinhNghiLe/GetDanhMuc',
            data: {},
            success: function (data) {
                hideLoading();
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
    $scope.SearchTieuChuan = function () {
        $scope.LoadPage();
    }
    //#endregion

    $scope.LoadPage = function () {
        showToast();
        $.ajax({
            type: 'post',
            url: '/TieuBinhNghiLe/DsDmCapBacLoaiHam',
            data: { currentPage: $scope.modelSearch.currentPage, recordPerPage: $scope.modelSearch.pageSize, LucLuongId: $scope.LucLuongId, NhomCapBacId: $scope.NhomCapBacId, LoaiHamId: $scope.LoaiHamId },
            success: function (response) {
                if (response != null) {
                    $scope.dsDmCBacLHam = response.data;
                    $scope.modelSearch.totalItems = response.total;
                    $scope.$apply();
                }
                else {
                    toastr.error("Trùng số liệu: [" + item.TEN_CB + "]  [" + item.LOAI_HAM + "]");
                }
                hideLoading();
            }
        });
    }
    $scope.LoadPage();

    $scope.pageChanged = function () {
        $scope.LoadPage();
    }

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
    $scope.btnChoiceCapBacLoaiHam = function () {
        $scope.cBacLoaiHamArray = [];


        angular.forEach($scope.dsDmCBacLHam, function (dsDmCBacLHam) {
            if (!!dsDmCBacLHam.Selected) {
                $scope.cBacLoaiHamArray.push(dsDmCBacLHam.ID);
            }
        })
        //Add dữ liệu tiêu binh nghi lễ
        angular.forEach($scope.cBacLoaiHamArray, function (val, key) {
            var item = $scope.dsDmCBacLHam.find(x => x.ID === val);

            if ($rootScope.ListTieuBinhNghiLe.length === 0) {
                $scope.itemModel = {};
                $scope.itemModel.CAP_BAC_ID = item.CAP_BAC_ID;
                $scope.itemModel.LOAI_HAM_ID = item.LOAI_HAM_ID;
                $scope.itemModel.SL_NAM_HT = 0;
                $scope.itemModel.SL_NU_HT = 0;
                $rootScope.ListTieuBinhNghiLe.push($scope.itemModel);
            }
            else {
                var CheckTrungData = $rootScope.ListTieuBinhNghiLe.filter(x => x.CAP_BAC_ID === item.CAP_BAC_ID && x.LOAI_HAM_ID === item.LOAI_HAM_ID);
                if (CheckTrungData.length === 0) {
                    $scope.itemModel = {};
                    $scope.itemModel.CAP_BAC_ID = item.CAP_BAC_ID;
                    $scope.itemModel.LOAI_HAM_ID = item.LOAI_HAM_ID;
                    $scope.itemModel.SL_NAM_HT = 0;
                    $scope.itemModel.SL_NU_HT = 0;
                    $rootScope.ListTieuBinhNghiLe.push($scope.itemModel);
                }
                else {
                    toastr.error("Trùng số liệu: [" + item.TEN_CB + "]  [" + item.LOAI_HAM + "]");
                    return false;
                }
            }
        });
        $scope.cBacLoaiHamArray = [];
        $uibModalInstance.close();
    };
    $scope.checkAll = function () {
        angular.forEach($scope.dsDmCBacLHam, function (item) {
            item.Selected = event.target.checked;
        });
    };

});
app.controller('ImpFileTieuBinhController', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading, data) {
    $scope.model = {};

    $scope.Upload = function () {
        showToast();
        var fileUpload = $("#file-input").get(0);
        var files = fileUpload.files;
        var fileData = new FormData();
        fileData.append(files[0].name, files[0]);
        fileData.append('donViId', data.donViId);
        fileData.append('nam', data.nam);
        $.ajax({
            url: '/TieuBinhNghiLe/ImportFileExel',
            type: "POST",
            dataType: 'json',
            contentType: false, // Not to set any content header
            processData: false, // Not to process data
            data: fileData,
            success: function (result) {
                if (result.Error) {
                    toastr.error(result.Title);
                } else {
                    toastr.success(result.Title);
                    $scope.cancel();
                    hideLoading();
                    $uibModalInstance.close();
                }
            },
            error: function (err) {
                toastr.error("Lỗi nhập dữ liệu.")
            }
        });
    }
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});