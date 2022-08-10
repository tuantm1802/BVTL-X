app.controller("NhomTieuChuanController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $rootScope) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 1000;
    $scope.tckv = false;
    $scope.tccb = false;
    $scope.rows = [];
    $scope.loaitc = [];
    $scope.loaitcAll = [];
    $rootScope.checkadd = false;
    $scope.currentPage = 1;
    $scope.mua = [
        { Name: '' },
        { Name: 'Xuân hạ' },
        { Name: 'Thu đông' },
        { Name: 'Khác' }
    ];
    $scope.nienhan = [
        { id: 0, text: '0' },
        { id: 1, text: '1' },
        { id: 2, text: '2' },
        { id: 3, text: '3' },
        { id: 4, text: '4' },
        { id: 5, text: '5' }
    ];

    $scope.loaiNhomTieuChuan = [
        { text: 'Tiêu chuẩn thường xuyên' },
        { text: 'Tiêu chuẩn tăng thêm' },
        //{ text: 'Tiêu chuẩn cấp 1 lần' },
        { text: 'Tiêu chuẩn khác' },
    ]

    var settingLoaiKH = {
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
            onClick: onClickLoaiKH
        }
    };

    $scope.NhomTCGoc = "";

    function onClickLoaiKH(event, treeId, treeNode, clickFlag) {
        $scope.NhomTCGoc = "";
        // Lấy danh sách tiêu chuẩn của nhóm tiêu chuẩn đã chọn
        if (treeNode != null) {
            $scope.NhomTCGoc = treeNode.name_control;
        }

        // Lấy danh sách loại tiêu chuẩn 
        $scope.loaitc = [];
        var childs = $scope.loaitcAll.filter(function (x) {
            return (x.LOAI_KH_ID == parseInt(treeNode.id));
        });

        if (childs != null && childs.length > 0) {
            $scope.loaitc = childs;
        }

        $scope.modelSearch.NHOM_GOC = $scope.NhomTCGoc;
        $scope.ShowContent = true;
        $scope.currentPage = 1;

        LoadPage();
    }
    $scope.ListYear = [];
    $scope.modelSearch.NAM = 0;
    angular.element(document).ready(function () {
        var date = new Date();
        var currentYear = date.getFullYear();
        for (var i = currentYear; i > currentYear - 5; i--) {
            var temp = {
                Id: i,
                Name: i + ''
            }
            $scope.ListYear.push(temp);
        }
        $scope.modelSearch.NAM = $scope.ListYear[0].Id;
        GetButtonAction();
        $scope.GetLoaitc();

    });

    // Phân quyền các nút chức năng
    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnCancel = false;

    $scope.loaiKHTrees = [];

    function GetButtonAction() {
        $scope.loaiKHTrees = [];
        $.ajax({
            type: 'post',
            url: '/NhomTieuChuan/GetButtonAction',
            data: {},
            success: function (response) {
                if (response.buttons != null) {
                    angular.forEach(response.buttons, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'btnSave') {
                            $scope.RoleBtnSave = true;
                        }
                        if (item == 'btnDelete') {
                            $scope.RoleBtnDelete = true;
                        }
                        if (item == 'btnCancel') {
                            $scope.RoleBtnCancel = true;
                        }

                    });
                }
                $scope.loaiKHTrees = response.loaiKHs;

                $.fn.zTree.init($("#treeRole"), settingLoaiKH, $scope.loaiKHTrees);
                var zTree = $.fn.zTree.getZTreeObj("treeRole");
                var type = { "Y": "ps", "N": "ps" };
                zTree.setting.check.chkboxType = type;

                $scope.$apply();
            }
        });
    }

    $scope.Hiden = function () {
        $scope.rows = [];
        LoadPage();
    };


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

    $scope.Addmm = function () {
        $rootScope.checkadd = true;
        $scope.rows.push({
            TEN_NHOM_TC: '',
            MO_TA: '',
            MUA: '',
            LOAI_TC_ID: '',
            NIEN_HAN: '',
            TN: false,
            MA: false,
            LTC: false,
            NH: false,
            NHOM_GOC: $scope.NhomTCGoc
        });
    };
    $scope.GetLoaitc = function () {
        $.ajax({
            type: 'post',
            url: '/NhomTieuChuan/GetLoaitc',
            data: {},
            success: function (data) {
                hideLoading();
                if (data.Error == true) {
                    toastr.error(data.Title);
                } else {

                    $scope.loaitcAll = data.data;
                    $scope.$apply();
                }
            }
        });
    }
    $scope.TimKiemNhomTC = function () {
        $scope.modelSearch.currentPage = 1;
        LoadPage();
    }
    $scope.ChangeYear = function () {
        $scope.modelSearch.currentPage = 1;
        LoadPage();
    }

    LoadPage = () => {
        //$scope.modelSearch.currentPage = $scope.currentPage;
        $scope.modelSearch.totalItems = 0;
        if ($scope.modelSearch.NHOM_GOC != null && $scope.modelSearch.NHOM_GOC != '') {


            // Lấy danh sách nhóm tiêu chuẩn
            $.ajax({
                type: 'post',
                url: '/NhomTieuChuan/DanhSach',
                cache: false,
                async: true,
                data: $scope.modelSearch,
                success: function (data) {
                    hideLoading();
                    $scope.modelSearch.totalItems = data.totalItems;
                    //$scope.modelSearch.pageSize = data.pageSize;
                    if (data.Error == true) {
                        toastr.error(data.Title);
                    } else {
                        //toastr.success(data.Title);
                        $scope.DanhSach = data.data;
                        $scope.DanhSach.Edit = false;
                        angular.forEach($scope.DanhSach, function (obj) {
                            obj["EditMode"] = false;
                            if (obj.NGAY_HET_HAN != null && obj.NGAY_HET_HAN != '') {
                                obj.NGAY_HET_HAN_E = new Date(moment(obj.NGAY_HET_HAN).format());
                                console.log(obj.NGAY_HET_HAN_E);
                            } 
                        })
                        $scope.rows = [];
                        $scope.$apply();
                    }
                }
            });
        }
        else {
            toastr.error("Vui lòng chọn Loại tiêu chuẩn!");
        }
    }

    $scope.loadTemplate = function (index) {
        if ($scope.RoleBtnSave)
            $scope.DanhSach[index].Edit = true;
    }
    $scope.pageChanged1 = function () {
        LoadPage();
    };

    $scope.submit = function () {
        var flag = true;
        if ($scope.rows != "") {
            angular.forEach($scope.rows, function (data) {
                if (data.TEN_NHOM_TC == null || data.TEN_NHOM_TC == "") {
                    data.TN = true;
                    flag = false;

                } else {
                    data.TN = false;
                }
                //if (data.PHAN_LOAI_TC == null || data.PHAN_LOAI_TC == "") {
                //    data.loaiNTC = true;
                //    flag = false;

                //} else {
                //    data.loaiNTC = false;
                //}
                //if (data.MUA == null || data.MUA == "") {
                //    data.MA = true;
                //    flag = false;

                //} else {
                //    data.MA = false;
                //}
                if (data.LOAI_TC_ID == null || data.LOAI_TC_ID == "") {
                    data.LTC = true;
                    flag = false;

                } else {
                    data.LTC = false;
                }
                if (data.NIEN_HAN == null || data.NIEN_HAN == "") {
                    data.NH = true;
                    flag = false;

                } else {
                    data.NH = false;
                }

                if (data.NGAY_HET_HAN != null && data.NGAY_HET_HAN != '') {
                    data.NGAY_HET_HAN = moment(data.NGAY_HET_HAN).format('DD/MM/YYYY');
                } else {
                    data.NGAY_HET_HAN = "";
                }
            });
            if (flag == true) {

                $.ajax({
                    type: 'post',
                    url: '/NhomTieuChuan/Add',
                    data: { ds: $scope.rows, nhomTCGoc: $scope.NhomTCGoc },
                    success: function (data) {
                        if (data.Error) {
                            toastr.error(data.Title);
                        } else {
                            //$scope.ListHangHoa = [];
                            toastr.success(data.Title);
                            $scope.ItemShow = false;
                            $rootScope.checkadd = false;
                            LoadPage();
                        }
                        hideLoading();
                    }
                });
            }
            $scope.ck = false;
        } else {
            var nhomTCS = $scope.DanhSach.filter(function (x) {
                return (x.Edit == true);
            });
            if (nhomTCS != null && nhomTCS.length > 0) {
                angular.forEach(nhomTCS, function (data) {
                    if (data.NGAY_HET_HAN_E != null && data.NGAY_HET_HAN_E != '') {
                        data.NGAY_HET_HAN = moment(data.NGAY_HET_HAN_E).format('DD/MM/YYYY');
                    } else {
                        data.NGAY_HET_HAN = "";
                    }
                });
                $.ajax({
                    type: 'post',
                    url: '/NhomTieuChuan/edit',
                    data: { ds: nhomTCS, nhomTCGoc: $scope.NhomTCGoc },
                    success: function (data) {
                        if (data.Error) {
                            toastr.error(data.Title);
                        } else {
                            toastr.success(data.Title);
                            LoadPage();
                        }
                        hideLoading();
                    }
                });
            }

        }
    };
    $scope.tckhuvuc = function (itemId) {
        if ($rootScope.checkadd == true) {
            toastr.error("Vui lòng lưu dữ liệu trước");
        } else {
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/NhomTieuChuan/tckhuvuc',
                controller: 'tckhuvuc',
                size: 'xs',
                backdrop: 'static',
                resolve: {
                    itemId: function () {
                        return itemId;
                    }
                }
            });
            //kết quả trả về của modal
            modalInstance.result.then(function (response) {
                LoadPage();
            });
        }
    };
    $scope.tccapbac = function (itemId) {
        if ($rootScope.checkadd == true) {
            toastr.error("Vui lòng lưu dữ liệu trước");
        } else {
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/NhomTieuChuan/tccapbac',
                controller: 'tccapbac',
                size: 'xs',
                backdrop: 'static',
                resolve: {
                    itemId: function () {
                        return itemId;
                    }
                }
            });
            //kết quả trả về của modal
            modalInstance.result.then(function (response) {
                LoadPage();
            });
        }

    };
    $scope.selectAll = false;
    $scope.checkAll = () => {
        angular.forEach($scope.DanhSach, function (obj) {
            obj["SELECT"] = !$scope.selectAll;
            if (obj["SELECT"] === true) obj["EditMode"] = true;
            else obj["EditMode"] = false;
        });
    };
    $scope.listXoa = [];
    $scope.delete = function () {
        angular.forEach($scope.DanhSach, function (data) {
            if (data.SELECT == true) {
                $scope.listXoa.push(data);
            }
        })
        if ($scope.listXoa.length == 0) {
            toastr.error('Chưa chọn mục nào để xóa.');
            return;
        }
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn xóa các bản ghi đã chọn không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-red',
                    action: function (scope, button) {
                        $.ajax({
                            type: 'post',
                            url: '/NhomTieuChuan/delete',
                            data: { dt: $scope.listXoa },
                            success: function (data) {
                                if (data.Error) {
                                    toastr.error(data.Title);
                                } else {
                                    toastr.success(data.Title);
                                    LoadPage();
                                    $scope.selectAll = false;
                                    $scope.listXoa = [];
                                }
                            }
                        });
                    }
                },
                close: {
                    text: 'Hủy',
                    btnClass: 'btn-green',
                    action: function (scope, button) {

                    }
                }
            }
        });
    };
    $scope.Loc = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/NhomTieuChuan/_Loc',
            controller: 'Loc',
            size: 'xs',
            backdrop: 'static'
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            LoadPage();
        });
    };
    $rootScope.stt = true;
    $rootScope.tn = true;
    $rootScope.mt = true;
    $rootScope.mm = true;
    $rootScope.ltc = true;
    $rootScope.nh = true;
    $rootScope.ntckv = true;
    $rootScope.ntccb = true;
})
app.controller('tckhuvuc', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading, $rootScope) {
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
    $scope.khuvuc = [];
    angular.element(document).ready(function () {
        if ($rootScope.checkadd == false) {
            $scope.Load();
        } else {
            $.ajax({
                type: 'post',
                url: '/NhomTieuChuan/GetAllkhuvuc',
                data: {},
                success: function (data) {
                    hideLoading();
                    if (data.Error == true) {
                        toastr.error(data.Title);
                    } else {
                        $scope.khuvuc = data.data;
                        //toastr.success(data.Title);
                        $scope.$apply();
                    }
                }
            });
        }
    });
    $scope.Load = function () {
        $.ajax({
            type: 'post',
            url: '/NhomTieuChuan/Getkhuvuc',
            data: { idn: itemId },
            success: function (data) {
                hideLoading();
                if (data.Error == true) {
                    toastr.error(data.Title);
                } else {
                    $scope.khuvuc = data.data;
                    //toastr.success(data.Title);
                    $scope.$apply();
                }
            }
        });
    }
    $scope.submit = function () {
        if ($rootScope.checkadd == false) {
            $.ajax({
                type: 'post',
                url: '/NhomTieuChuan/Edittckhuvuc',
                data: { ds: $scope.khuvuc, dt: itemId },
                success: function (data) {
                    hideLoading();
                    if (data.Error == true) {
                        toastr.error(data.Title);
                    } else {
                        $scope.Load();
                        toastr.success(data.Title);
                        $scope.$apply();
                    }
                }
            });
        }
    }
});
app.controller('tccapbac', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading, $rootScope) {
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
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
    angular.element(document).ready(function () {
        $scope.GetLtccapbac();
    });
    $scope.GetLtccapbac = function () {
        $scope.ListPageMenu = [];
        $.ajax({
            type: 'post',
            url: '/NhomTieuChuan/Gettccapbac',
            data: { dt: itemId },
            success: function (data) {
                $scope.ListPageMenu = data.data;
                $.fn.zTree.init($("#treeNhomCapBac"), setting, $scope.ListPageMenu);
                var zTree = $.fn.zTree.getZTreeObj("treeNhomCapBac");
                zTree.expandAll(false);
                var type = { "Y": "ps", "N": "ps" };
                zTree.setting.check.chkboxType = type;
                $scope.$apply();
                hideLoading();
            },
            error: function (xhr, status, error) {
                toastr.error("Lỗi lấy danh sách nhóm cấp bậc");
            }
        });
    }
    $scope.submit = function () {
        var obj = $.fn.zTree.getZTreeObj("treeNhomCapBac").getCheckedNodes();
        $.ajax({
            type: 'post',
            url: '/NhomTieuChuan/editncapbac',
            data: { item: obj, dt: itemId },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    toastr.success(data.Title);
                    $scope.GetLtccapbac();
                    hideLoading();
                }
            }
        });
    }
});
app.controller('Loc', function ($scope, $uibModalInstance, $rootScope) {
    $scope.cancel = function () {
        $scope.listA = [];
        $scope.listB = [];
        $uibModalInstance.close();
    };
    $scope.listt = [
        { id: 1, Name: 'STT' },
        { id: 2, Name: 'Tên nhóm cấp bậc' },
        { id: 3, Name: 'Mô tả' },
        { id: 4, Name: 'Mùa' },
        { id: 5, Name: 'Loại tiêu chuẩn' },
        { id: 6, Name: 'Niên hạn' },
        { id: 7, Name: 'Nhóm tiêu chuẩn khu vực' },
        { id: 8, Name: 'Nhóm tiêu chuẩn cấp bậc' }
    ];
    $scope.models = [
        { listName: "Dữ liệu được hiển thị", items: [], dragging: false },
        { listName: "Dữ liệu sẵn có", items: [], dragging: false }
    ];
    $scope.listC = [];
    $scope.listA = [];
    $scope.listB = [];
    if ($rootScope.stt == true) {
        $scope.models[0].items.push({ label: 'STT', selected: false });
    } else $scope.models[1].items.push({ label: 'STT', selected: false });
    if ($rootScope.tn == true) {
        $scope.models[0].items.push({ label: 'Tên nhóm cấp bậc', selected: false });
    } else $scope.models[1].items.push({ label: 'Tên nhóm cấp bậc', selected: false });
    if ($rootScope.mt == true) {
        $scope.models[0].items.push({ label: 'Mô tả', selected: false });
    } else $scope.models[1].items.push({ label: 'Mô tả', selected: false });
    if ($rootScope.mm == true) {
        $scope.models[0].items.push({ label: 'Mùa', selected: false });
    } else $scope.models[1].items.push({ label: 'Mùa', selected: false });
    if ($rootScope.ltc == true) {
        $scope.models[0].items.push({ label: 'Loại tiêu chuẩn', selected: false });
    } else $scope.models[1].items.push({ label: 'Loại tiêu chuẩn', selected: false });
    if ($rootScope.nh == true) {
        $scope.models[0].items.push({ label: 'Niên hạn', selected: false });
    } else $scope.models[1].items.push({ label: 'Niên hạn', selected: false });
    if ($rootScope.ntckv == true) {
        $scope.models[0].items.push({ label: 'Nhóm tiêu chuẩn khu vực', selected: false });
    } else $scope.models[1].items.push({ label: 'Nhóm tiêu chuẩn khu vực', selected: false });
    if ($rootScope.ntccb == true) {
        $scope.models[0].items.push({ label: 'Nhóm tiêu chuẩn cấp bậc', selected: false });
    } else $scope.models[1].items.push({ label: 'Nhóm tiêu chuẩn cấp bậc', selected: false });
    //if ($rootScope.stt == true) {
    //    $scope.listA.push({ id: 1, Name: 'STT' });
    //} else $scope.listB.push({ id: 1, Name: 'STT' });
    //if ($rootScope.tn == true) {
    //    $scope.listA.push({ id: 2, Name: 'Tên nhóm cấp bậc' });
    //} else $scope.listB.push({ id: 2, Name: 'Tên nhóm cấp bậc' });
    //if ($rootScope.mt == true) {
    //    $scope.listA.push({ id: 3, Name: 'Mô tả' });
    //} else $scope.listB.push({ id: 3, Name: 'Mô tả' });
    //if ($rootScope.mm == true) {
    //    $scope.listA.push({ id: 4, Name: 'Mùa' });
    //} else $scope.listB.push({ id: 4, Name: 'Mùa' });
    //if ($rootScope.ltc == true) {
    //    $scope.listA.push({ id: 5, Name: 'Loại tiêu chuẩn' });
    //} else $scope.listB.push({ id: 5, Name: 'Loại tiêu chuẩn' });
    //if ($rootScope.nh == true) {
    //    $scope.listA.push({ id: 6, Name: 'Niên hạn' });
    //} else $scope.listB.push({ id: 6, Name: 'Niên hạn' });
    //if ($rootScope.ntckv == true) {
    //    $scope.listA.push({ id: 7, Name: 'Nhóm tiêu chuẩn khu vực' });
    //} else $scope.listB.push({ id: 7, Name: 'Nhóm tiêu chuẩn khu vực' });
    //if ($rootScope.ntccb == true) {
    //    $scope.listA.push({ id: 8, Name: 'Nhóm tiêu chuẩn cấp bậc' });
    //} else $scope.listB.push({ id: 8, Name: 'Nhóm tiêu chuẩn cấp bậc' });
    //$scope.stt = true;
    //$scope.mdvt = true;
    //$scope.dvt = true;
    //$scope.Mota = true;
    //$scope.listA = [
    //    { id: 1, Name: 'STT' },
    //    { id: 2, Name: 'Tên đơn vị tính' },
    //    { id: 3, Name: 'Mã Đơn vị tính' },
    //    { id: 4, Name: 'Mô tả' }
    //];
    //$scope.listB = [];
    $scope.getSelectedItemsIncluding = function (list, item) {
        item.selected = true;
        return list.items.filter(list.items, ['selected', true]);
    };
    $scope.onDragstart = function (list, event) {
        list.dragging = true;
    };
    $scope.onDrop = function (list, items, index) {
        angular.forEach(items, function (item) { item.selected = false; });
        list.items = list.items.slice(0, index)
            .concat(items)
            .concat(list.items.slice(index));
        return true;
    }
    $scope.onMoved = function (list) {
        list.items = list.items.filter(list.items, ['selected', true]);
    };

    $scope.selectedA = [];
    $scope.selectedB = [];
    $scope.selectA = function (i) {
        $scope.selectedA.push(i);
    };

    $scope.selectB = function (i) {
        $scope.selectedB.push(i);
    };
    $scope.toggleA = function () {
        angular.forEach($scope.listA, function (data) {
            $scope.selectedA.push(data);
        });
    };
    $scope.toggleB = function () {
        angular.forEach($scope.listB, function (data) {
            $scope.selectedB.push(data);
        });
    };
    $scope.BTB = function () {

        angular.forEach($scope.selectedA, function (data) {
            if (data.id != '') {
                $scope.listB.push(data);
                var index = $scope.listA.findIndex(x => x.id === data.id);
                $scope.listA.splice(index, 1);
            }
        });
        reset();
    };
    $scope.BTA = function () {
        angular.forEach($scope.selectedB, function (data) {
            if (data.id != '') {
                $scope.listA.push(data);
                var index = $scope.listB.findIndex(x => x.id === data.id);
                $scope.listB.splice(index, 1);

            }
        });
        reset();
    };
    function reset() {
        $scope.selectedA = [];
        $scope.selectedB = [];
    };
    $scope.checka = false;
    $scope.checkb = false;
    $scope.checkc = false;
    $scope.checkd = false;
    $scope.checke = false;
    $scope.checkf = false;
    $scope.checkg = false;
    $scope.checkh = false;
    $scope.Save = function () {
        if ($scope.listB == '') {
            $rootScope.stt = true;
            $rootScope.tn = true;
            $rootScope.mt = true;
            $rootScope.mm = true;
            $rootScope.ltc = true;
            $rootScope.nh = true;
            $rootScope.ntckv = true;
            $rootScope.ntccb = true;
        } else {
            angular.forEach($scope.listB, function (data) {
                if ($scope.checka == false) {
                    if (data.id == 1) {
                        $rootScope.stt = false;
                        $scope.checka = true;

                    } else $rootScope.stt = true;
                }
                if ($scope.checkb == false) {
                    if (data.id == 2) {
                        $rootScope.tn = false;
                        $scope.checkb = true;
                    } else $rootScope.tn = true;
                }
                if ($scope.checkc == false) {
                    if (data.id == 3) {
                        $rootScope.mt = false;
                        $scope.checkc = true;
                    } else $rootScope.mt = true;
                }
                if ($scope.checkd == false) {
                    if (data.id == 4) {
                        $rootScope.mm = false;
                        $scope.checkd = true;
                    } else $rootScope.mm = true;
                }
                if ($scope.checke == false) {
                    if (data.id == 5) {
                        $rootScope.ltc = false;
                        $scope.checke = true;

                    } else $rootScope.ltc = true;
                }
                if ($scope.checkf == false) {
                    if (data.id == 6) {
                        $rootScope.nh = false;
                        $scope.checkf = true;
                    } else $rootScope.nh = true;
                }
                if ($scope.checkg == false) {
                    if (data.id == 7) {
                        $rootScope.ntckv = false;
                        $scope.checkg = true;
                    } else $rootScope.ntckv = true;
                }
                if ($scope.checkh == false) {
                    if (data.id == 8) {
                        $rootScope.ntccb = false;
                        $scope.checkh = true;
                    } else $rootScope.ntccb = true;
                }
            });
        }
        $scope.checka = false;
        $scope.checkb = false;
        $scope.checkc = false;
        $scope.checkd = false;
        $scope.checke = false;
        $scope.checkf = false;
        $scope.checkg = false;
        $scope.checkh = false;
        $scope.listA = [];
        $scope.listB = [];
        $uibModalInstance.close();
    };
});
