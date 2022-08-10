app.controller("TrangPhucController", function ($scope, $uibModal, $ngConfirm, hideLoading) {
    $scope.level = 0;
    $scope.ShowContent = false;
    $scope.dataTrangPhuc = [];
    $scope.Years = [];
    $scope.NhomCBTreeInit = [];
    $scope.NtrangPhuc = [];
    $scope.ckc = false;
    $scope.ListNam = [];
    var currYear = new Date().getFullYear();
    for (var i = currYear; i >= (currYear - 10); i--)
        $scope.ListNam.push({ Id: i, Text: i });
    $scope.Years = currYear.toString();

    //Dữ liệu hiển thị khi load trang lần đầu  
    angular.element(document).ready(function () {
        $scope.GetDataNhomTrangPhucController($scope.Years);

    });

    //Phân quyền các nút
    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnCancel = false;
    function GetButtonAction() {
        $.ajax({
            type: 'post',
            url: '/TrangPhuc/GetButtonAction',
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
                $scope.$apply();
            }
        });
    }
    //lấy dữ liệu nhóm trang phục
    $scope.GetDataNhomTrangPhucController = function () {
        $scope.ListNhomTrangPhuc = [];
        $.ajax({
            type: 'post',
            url: '/TrangPhuc/GetDataNhomTrangPhucController',
            data: { key: $scope.searchNhomTP },
            success: function (data) {
                if (data.Error == true) {
                    toastr.error("Lỗi lấy danh sách Nhóm trang phục");
                } else {
                    $scope.ListNhomTrangPhuc = data.data;
                    $.fn.zTree.init($("#treeViewNhomTrangPhuc"), setting, $scope.ListNhomTrangPhuc);
                    var zTree = $.fn.zTree.getZTreeObj("treeViewNhomTrangPhuc");
                    //var type = { "Y": "s", "N": "ps" };
                    //zTree.setting.check.chkboxType = type;
                    zTree.expandAll(false);
                    $scope.$apply();
                    hideLoading();
                }
            },

        });
    }
    function filter(node) {
        var value = node.name.toLowerCase()/*conver_tvkhongdau(node.name.toLowerCase())*/;
        var keyword = $scope.searchNhomTP.toLowerCase()/*conver_tvkhongdau($scope.searchNhomTP.toLowerCase())*/;
        if (!$scope.searchNhomTP || value.indexOf(keyword)>=0)
        return true;
    }
    function addParentNode(node) {

        if (node.children) {
            node.children.forEach(function (_node, index) {
                if (filter(_node)) {
                    _node.isHidden = false;
                    node.isHidden = false;
                }
                else
                    _node.isHidden = true;
                addParentNode(_node, _node.isHidden);
                if (!_node.isHidden)
                    node.isHidden = _node.isHidden;
            });
        }
    }
    var parentShow = { value: false };
    //Tìm kiếm trong nhóm trang phục
    $scope.ChangeSearch = function () {
        showNodes = [];
        var treeObj = $.fn.zTree.getZTreeObj("treeViewNhomTrangPhuc");
        var nodes = treeObj.getNodes();
        nodes.forEach(function (value, index) {
            parentShow.value = false;
            if (filter(value)) {
                value.isHidden = false;
            }  
            else
                value.isHidden = true;
            addParentNode(value);
        });
        $scope.ListNhomTrangPhuc = nodes;
        //Cần import <script src="~/Content/js/jquery.ztree.exhide.min.js"></script> thì mới reload được dữ liệu
        $.fn.zTree.init($("#treeViewNhomTrangPhuc"), setting, $scope.ListNhomTrangPhuc);
        if ($scope.searchNhomTP.length>0)
            treeObj.expandAll(true);
        else
            treeObj.expandAll(false);
    }

    //Lấy dữ liệu chính của bảng
    $scope.GetDataTrangPhucController = function () {
        var obj = $.fn.zTree.getZTreeObj("treeViewNhomTrangPhuc").getSelectedNodes();
        $scope.NtrangPhuc = [];
        $scope.NtrangPhuc.push({ ID: obj[0].id })
        $.ajax({
            type: 'post',
            url: '/TrangPhuc/GetDataTrangPhucController',
            data: { Id: obj[0].id, Years: $scope.Years, Keyword:$scope.search },
            success: function (data) {
                if (data.Error == true) {
                    toastr.Error('Lỗi lấy dữ liệu trang phục');
                } else {
                    //toastr.success('Lấy dữ liệu thành công');
                    $scope.dataTrangPhuc = data.data;
                    $scope.$apply();
                    //LoadItemOpen();
                }

            }
        })
    }

    // treeview nhóm trang phục
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
            onClick: onClick
        }
    };

    //Chọn nhóm trang phục để show data
    function onClick() {
        $scope.ShowContent = true;
        var obj = $.fn.zTree.getZTreeObj("treeViewNhomTrangPhuc").getSelectedNodes();
        //$scope.selectedItem =obj[0].getParentNode();
        $scope.search = "";
        $scope.NtrangPhuc = [];
        $scope.NtrangPhuc.push({ ID: obj[0].id, Name: obj[0].name })
        $scope.Years = currYear.toString();
        $.ajax({
            type: 'post',
            url: '/TrangPhuc/GetDataTrangPhucController',
            data: { Id: obj[0].id, Years: $scope.Years },
            success: function (data) {
                if (data.Error == true) {
                    toastr.Error('Lỗi lấy dữ liệu trang phục');
                } else {
                    $scope.dataTrangPhuc = data.data;
                    $scope.$apply();
                    GetButtonAction();
                }

            }
        })
    }

    //Xử lý thêm mới
    $scope.selectedItem = [];
    $scope.addData = function () {
        if ($scope.checkChild() == false) {
            toastr.error("Trang phục bạn chọn không thể là cha của trang phục mới");
            $scope.count = 0;
            $scope.ckc = false;
        } else if ($scope.checkdata() == false) {
            toastr.error("Bạn không thể cùng lúc thêm mới nhiều nhóm trang phục cha");
            $scope.count = 0;
            $scope.ckc = false;
        } else {
            $scope.count = 0;
            $scope.ckc = false;
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/TrangPhuc/AddData',
                controller: 'addData',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    // dữ liệu sẽ được truyền từ đây xuống UI
                    itemNTP: function () {
                        return $scope.NtrangPhuc[0];
                    },
                    itemParent: function () {
                        return $scope.selectedItem[0];
                    }
                }
            });
            modalInstance.result.then(function (result) {
                if (result)
                    onClick();
            });
            $scope.selectedItem = [];
        }
    };

    // Xử lý nút chỉnh sửa thông tin
    $scope.ttChiTiet = function (item) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/TrangPhuc/EditView',
            controller: 'editData',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                // dữ liệu sẽ được truyền từ đây xuống UI
                itemNTP: function () {
                    return $scope.NtrangPhuc[0];
                },
                itemParent: function () {
                    return $scope.selectedItem[0];
                },
                itemList: function () {
                    return item;
                }
            }
        });
        modalInstance.result.then(function (result) {
            if (result)
                onClick();
        });
        $scope.selectedItem = [];
    }

    // check all tất cả các dữ liệu con khi chọn dũ liệu cha
    $scope.checkAllchild = function (item) {
        if (item.children != null) {
            angular.forEach(item.children, function (data) {
                data.selected = !item.selected;
                data.status = !item.selected;
            });
        }
    }

    $scope.count = 0;
    $scope.ckc = false;
    // kiểm tra có dữ liệu con nào được chọn để thêm mới không
    $scope.checkChild = function () {
        $scope.ckc = false;
        angular.forEach($scope.dataTrangPhuc, function (data) {
            if (data.selected == false) {
                if (data.children != null) {
                    var i;
                    for (i = 0; i < data.children.length; i++) {
                        if (data.children[i].selected == true && data.children[i].status == false) {
                            $scope.ckc = true;
                        }
                    }
                }
            }
        });
        if ($scope.ckc == true) {
            return false
        } else return true;
    }
    // kiểm tra xem có 2 thằng cha nào được chọn để thêm mới không
    $scope.checkdata = function () {
        angular.forEach($scope.dataTrangPhuc, function (data) {
            if (data.selected == true && data.status == false) {
                $scope.selectedItem = [];
                $scope.count++;
                $scope.selectedItem.push({ ID: data.ID, Name: data.TEN_SP, LOAI_HAM: data.LOAI_HAM, CAP_BAC_ID: data.CAP_BAC_ID, GIOI_TINH: data.GIOI_TINH, DVT_ID: data.DVT_ID, INT: data.INT, ITP: data.ITP });
            }
            if (data.selected == false) {
                if (data.children != null) {
                    var i;
                    for (i = 0; i < data.children.length; i++) {
                        if (data.children[i].selected == true) {
                            $scope.checkChild = true;
                        }
                    }
                }
            }
        });
        if ($scope.count > 1) {
            return false;
        } else return true;
    }

    //Xử lý xóa dữ liệu
    $scope.Delete = function () {
        $scope.listDelete = [];
        angular.forEach($scope.dataTrangPhuc, function (data) {
            if (data.selected == true && data.status == false) {
                $scope.listDelete.push(data);
            }
            if (data.selected == false) {
                if (data.children != null) {
                    var i;
                    for (i = 0; i < data.children.length; i++) {
                        if (data.children[i].selected == true) {
                            $scope.listDelete.push(data.children[i]);
                        }
                    }
                }
            }
        })
        if ($scope.listDelete.length == 0) {
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
                            url: '/TrangPhuc/DeleteController',
                            data: { dt: $scope.listDelete },
                            success: function (data) {
                                if (!data.Error) {
                                    toastr.success(data.notifyTitle);
                                    onClick();
                                    $scope.selectAll = false;
                                } else {
                                    toastr.warning(data.notifyTitle);
                                }
                                hideLoading();
                                
                               
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
    $scope.showGioiTinh = function (value) {
        if (value == "M")
            return "Nam";
        else if (value == "F")
            return "Nữ";
        else if (value == "B")
            return "Chung";
        else
            return "";
    };
});

app.controller('addData', function ($scope, $uibModalInstance, itemNTP, itemParent, $ngConfirm, showToast, hideLoading) {
    $scope.isDisabled = false;
    $scope.success = false;
    //Load dữ liệu
    angular.element(document).ready(function () {

        var promise1 = GetCapBacController();
        var promise2 = GetDonViTinhController();
        GetButtonAction();

        // Kiểm tra liệu trang phục cha có được chọn hay không
        if (itemParent == undefined) {
            $scope.model.PText = "";
        } else {
            $scope.model.PText = itemParent.Name;
            $scope.model.LOAI_HAM = itemParent.LOAI_HAM;
            $scope.model.GIOI_TINH = itemParent.GIOI_TINH;
            $scope.model.INT = itemParent.INT;
            $scope.model.ITP = itemParent.ITP;
            //Chờ cho danh mục cấp bậc và đơn vị tính load dữ liệu xong thì mới
            Promise.all([promise1, promise2]).then((values) => {
                $scope.model.CAP_BAC_ID = itemParent.CAP_BAC_ID;
                $scope.model.DVT_ID = itemParent.DVT_ID;
                $scope.$apply();
            });
            $scope.isDisabled = true;
        }
    });

    //Phân quyền các nút
    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnCancel = false;
    function GetButtonAction() {
        $.ajax({
            type: 'post',
            url: '/TrangPhuc/GetButtonAction',
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
                $scope.$apply();
            }
        });
    }

    // Button "Hủy thay đổi" sẽ clear các trường đã nhập

    $scope.hiden = function () {
        $scope.model.TEN_SP = "";
        $scope.model.MA_H55 = "";
        $scope.model.TRO_GIA = "";
        $scope.model.IMD = false;
        $scope.model.ITP = false;
        $scope.model.INT = false;
        $scope.model.ICS = false;
        $scope.model.IKHAC = false;
        $scope.model.LOAI_HAM = "";
        $scope.model.GIOI_TINH = "";
        $scope.model.NGAY_HUY_TEXT = "";
        $scope.model.MO_TA = "";
        $scope.model.DVT_ID = "";
        $scope.model.CAP_BAC_ID = "";

    }

    //Đóng pop-up
    $scope.cancel = function () {
        $uibModalInstance.close($scope.success);
    };

    $scope.model = {};
    // lấy dữ liệu truyền từ bên trên xuống thông qua itemNTP
    $scope.model.TEN_NHOM_SP = itemNTP.Name;
    $scope.listHam = [
        { id: 1, text: "Vạch xanh" },
        { id: 2, text: "Vạch vàng" }
    ]
    $scope.listGioiTinh = [
        { id: "M", text: "Nam" },
        { id: "F", text: "Nữ" },
        { id: "B", text: "Chung" }
    ]


    // Xử lý sự kiện Lưu button
    $scope.save = function () {
        var flag = true;
        if (itemParent == undefined) {
            $scope.model.SP_CHA_ID = "";
        } else $scope.model.SP_CHA_ID = itemParent.ID;
        $scope.model.NHOM_SP_ID = itemNTP.ID;

        if ($scope.model.SP_CHA_ID != "") {
            if ($scope.model.MA_H55 == undefined) {
                $scope.model.requiredMaH55 = true;
                flag = false;
            } else {
                $scope.model.requiredMaH55 = false;
            }
            $scope.model.ICS = true;
        } else {
            $scope.model.ICS = false;
        }
        if (!$scope.model.ICS && !$scope.model.IMD) {
            $scope.model.IKHAC = true;
        } else {
            $scope.model.IKHAC = false;
        }

        if ($scope.model.TEN_SP == undefined) {
            $scope.model.requiredTenSP = true;
            flag = false;
        } else {
            $scope.model.requiredTenSP = false;
        }
        if ($scope.model.GIOI_TINH == undefined) {
            $scope.model.requiredSex = true;
            flag = false;
        } else {
            $scope.model.requiredSex = false;
        }
        if ($scope.model.DVT_ID == undefined) {
            $scope.model.requiredDvt = true;
            flag = false;
        } else {
            $scope.model.requiredDvt = false;
        }
        if (!$scope.model.INT && !$scope.model.ITP) {
            $scope.model.requiredChooseINTOrITP = true;
            flag = false;
        } else
            $scope.model.requiredChooseINTOrITP = false;

        if (flag == true) {
            showToast();
            $.ajax({
                type: 'post',
                url: '/TrangPhuc/AddController',
                data: { ds: $scope.model },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success('Thêm mới dữ liệu thành công');
                        $scope.success = true;
                        $uibModalInstance.close($scope.success);
                    }
                    hideLoading();
                },
                error: function () {
                    toastr.error('Thêm mới dữ liệu thất bại');
                    hideLoading();
                }
            })
        }
    }

    $scope.dataCapBac = [];
    function GetCapBacController() {
        return new Promise(function (resolve, reject) {
            if ($scope.dataCapBac.length == 0) {
                $.ajax({
                    type: 'post',
                    url: '/TrangPhuc/GetCapBacController',
                    data: {},
                    success: function (data) {
                        $scope.dataCapBac = data.dataCapBac;
                        $scope.$apply();
                        resolve(data);
                    },
                    error: function (xhr, status, error) {
                        toastr.error("Lỗi lấy danh sách");
                        reject(xhr);
                    }
                });
            }
        });
    }

    $scope.listdvt = [];
    function GetDonViTinhController() {
        return new Promise(function (resolve, reject) {
            if ($scope.listdvt.length == 0) {
                $.ajax({
                    type: 'post',
                    url: '/TrangPhuc/GetDonViTinhController',
                    data: {},
                    success: function (data) {
                        $scope.listdvt = data.data;
                        $scope.$apply();
                        resolve(data);
                    },
                    error: function (xhr, status, error) {
                        toastr.error("Lỗi lấy danh sách");
                        reject(xhr);
                    }
                });
            }
        });
    }
});

app.controller('editData', function ($scope, $uibModalInstance, itemNTP, itemParent, itemList, $ngConfirm, showToast, hideLoading) {
    $scope.isDisabled = false;
    $scope.success = false;
    angular.element(document).ready(function () {
        GetCapBacController();
        GetDonViTinhController();
        GetButtonAction();
        GetDataSpChaController(itemList.ID, itemList.NHOM_SP_ID);
        GetListNhomTrangPhucController()
    });

    //Phân quyền các nút
    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnCancel = false;
    function GetButtonAction() {
        $.ajax({
            type: 'post',
            url: '/TrangPhuc/GetButtonAction',
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
                $scope.$apply();
            }
        });
    }

    // Bấm nút "Hủy thay đổi" sẽ clear các trường đã nhập

    $scope.hiden = function () {
        $scope.model.TEN_NHOM_SP = itemNTP.Name;
        $scope.model.TEN_SP = itemList.TEN_SP;
        $scope.model.MA_H55 = itemList.MA_H55;
        $scope.model.TRO_GIA = itemList.TRO_GIA;
        $scope.model.IMD = itemList.IMD;
        $scope.model.ITP = itemList.ITP;
        $scope.model.INT = itemList.INT;
        $scope.model.ICS = itemList.ICS;
        $scope.model.IKHAC = itemList.IKHAC;
        $scope.model.LOAI_HAM = itemList.LOAI_HAM;
        $scope.model.GIOI_TINH = itemList.GIOI_TINH;
        $scope.model.MO_TA = itemList.MO_TA;
        $scope.model.NGAY_HUY_TEXT = itemList.NGAY_HUY_TEXT;
        $scope.model.DVT_ID = itemList.DVT_ID;
        $scope.model.edit_CHA_ID = itemList.SP_CHA_ID;
        $scope.model.CAP_BAC_ID = itemList.CAP_BAC_ID;
        $scope.model.NHOM_SP_ID = itemList.NHOM_SP_ID;
        $scope.model.ID = itemList.ID;
    }

    // Đóng popup
    $scope.cancel = function () {
        $uibModalInstance.close($scope.success);
    };

    $scope.model = {};
    // lấy dữ liệu truyền từ bên trên xuống thông qua itemNTP
    $scope.model.TEN_NHOM_SP = itemNTP.Name;
    $scope.model.TEN_SP = itemList.TEN_SP;
    $scope.model.MA_H55 = itemList.MA_H55;
    $scope.model.TRO_GIA = itemList.TRO_GIA;
    $scope.model.IMD = itemList.IMD;
    $scope.model.ITP = itemList.ITP;
    $scope.model.INT = itemList.INT;
    $scope.model.ICS = itemList.ICS;
    $scope.model.IKHAC = itemList.IKHAC;
    $scope.model.LOAI_HAM = itemList.LOAI_HAM;
    $scope.model.GIOI_TINH = itemList.GIOI_TINH;
    $scope.model.MO_TA = itemList.MO_TA;
    $scope.model.DVT_ID = itemList.DVT_ID;
    $scope.model.CAP_BAC_ID = itemList.CAP_BAC_ID;
    $scope.model.NHOM_SP_ID = itemList.NHOM_SP_ID;
    $scope.model.edit_CHA_ID = itemList.SP_CHA_ID;
    if (itemList.SP_CHA_ID) {
        $scope.isDisabled = true;
    }
    $scope.model.NGAY_HUY_TEXT = itemList.NGAY_HUY_TEXT;
    $scope.model.ID = itemList.ID;
    $scope.model.children = itemList.children;

    $scope.listHam = [
        { id: 1, text: "Vạch xanh" },
        { id: 2, text: "Vạch vàng" }
    ]
    $scope.listGioiTinh = [
        { id: "M", text: "Nam" },
        { id: "F", text: "Nữ" },
        { id: "B", text: "Chung" }
    ]

    $scope.save = function () {
        var flag = true;
        var _models = [];
        if ($scope.model.TEN_SP == undefined || $scope.model.TEN_SP == "") {
            $scope.model.requiredTenSP = true;
            flag = false;
        } else {
            $scope.model.requiredTenSP = false;
        }
        if ($scope.model.GIOI_TINH == undefined) {
            $scope.model.requiredSex = true;
            flag = false;
        } else {
            $scope.model.requiredSex = false;
        }
        if ($scope.model.SP_CHA_ID != "") {
            $scope.model.ICS = true;
        } else {
            $scope.model.ICS = false;
        }
        if (!$scope.model.ICS && !$scope.model.IMD) {
            $scope.model.IKHAC = true;
        } else {
            $scope.model.IKHAC = false;
        }
        if ($scope.model.DVT_ID == undefined) {
            $scope.model.requiredDvt = true;
            flag = false;
        } else {
            $scope.model.requiredDvt = false;
        }
        if (!$scope.model.INT && !$scope.model.ITP) {
            $scope.model.requiredChooseINTOrITP = true;
            flag = false;
        } else
            $scope.model.requiredChooseINTOrITP = false;
        if (flag == true) {
            showToast();
            $.ajax({
                type: 'post',
                url: '/TrangPhuc/EditController',
                data: { model: $scope.model },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success('Thay đổi  dữ liệu thành công');
                        $scope.success = true;
                    }
                    hideLoading();
                    $uibModalInstance.close($scope.success);
                },
                error: function () {
                    toastr.error('Thay đổi dữ liệu thất bại');
                    hideLoading();
                }
            })
        }
    }

    function GetCapBacController() {
        $.ajax({
            type: 'post',
            url: '/TrangPhuc/GetCapBacController',
            data: {},
            success: function (data) {
                $scope.dataCapBac = data.dataCapBac;
                $scope.$apply();
            },
            error: function (xhr, status, error) {
                toastr.error("Lỗi lấy danh sách");
            }
        });
    }

    function GetDonViTinhController() {
        $.ajax({
            type: 'post',
            url: '/TrangPhuc/GetDonViTinhController',
            data: {},
            success: function (data) {
                $scope.listdvt = data.data;
                $scope.$apply();
            },
            error: function (xhr, status, error) {
                toastr.error("Lỗi lấy danh sách");
            }
        });
    }

    function GetDataSpChaController(currentId, nhomspid) {
        $.ajax({
            type: 'post',
            url: '/TrangPhuc/GetDataSPChaController',
            data: { currentId: currentId, nhomspid: nhomspid },
            success: function (data) {
                $scope.dataSpCha = data.dataSpCha;
                $scope.$apply();
            },
            error: function () {
                toastr.error("Lỗi lấy danh sách");
            }
        });
    }


    function GetListNhomTrangPhucController() {
        $.ajax({
            type: 'post',
            url: '/TrangPhuc/GetListNhomTrangPhucController',
            data: {},
            success: function (data) {
                $scope.lstNhomTrangPhuc = data.lstNhomTrangPhuc;
                $scope.$apply();
            },
            error: function () {
                toastr.error("Lỗi lấy danh sách");
            }
        });
    }



});