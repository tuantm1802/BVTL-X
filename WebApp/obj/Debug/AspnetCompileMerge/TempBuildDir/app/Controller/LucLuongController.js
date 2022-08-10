app.controller("LucLuongController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {

    var setting = {};
    function suaLL(event, treeId, treeNode, clickFlag) {
        $scope.edit();
    }
    angular.element(document).ready(function () {
        $scope.GetLucLuong();
        $scope.modelAdd = {};
        GetAllLucLuong();
        GetButtonAction();
    });
    $scope.tenLLerror = false;
    $scope.tenVietTaterror = false;
    $scope.STTerror = false;
    $scope.STTstringerror = {};
    $scope.TenLLstringerror = {};
    // Phân quyền nút chức năng
    $scope.DSA = true;
    $scope.RoleBtnCreate = false;
    $scope.RoleBtnEdit = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnCancel = false;
    $scope.isDisable = true;
    function GetButtonAction() {
        $.ajax({
            type: 'post',
            url: '/LucLuong/GetButtonAction',
            data: {},
            success: function (response) {
                if (response.buttons != null) {
                    angular.forEach(response.buttons, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'btnEdit') {
                            $scope.RoleBtnEdit = true;
                        }
                        if (item == 'btnSave') {
                            $scope.RoleBtnSave = true;
                            $scope.isDisable = false;
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

    function GetAllLucLuong() {
        $.ajax({
            type: 'post',
            url: '/LucLuong/GetAllLucLuong',
            data: {},
            success: function (data) {
                $scope.ListLucLuong = data.ListLucLuong;
                $scope.ListLucLuong.unshift({ ID: null, TEN_LL: '--Chọn tên lực lượng cha--', SO_THU_TU: 0})
                $scope.model.ID = $scope.ListLucLuong[0].ID;
                $scope.$apply();
            },
            error: function (xhr, status, error) {
                toastr.error("Lỗi lấy danh sách lực lượng");
            }
        });
    }
    $scope.addLucLuong = function () {
        $scope.DSA = false;
        $scope.chgLL = false;
        $scope.chgstt = false;
        $scope.model = {};
        var obj = $.fn.zTree.getZTreeObj("treeLucLuong").getSelectedNodes();
        if (obj.length != 0) {
            for (var i = 0; i < $scope.ListLucLuong.length; i++) {
                if ($scope.ListLucLuong[i].ID == obj[0].id) {
                    $scope.model.ID = $scope.ListLucLuong[i].ID;
                    break;
                }
            }
        } else {
            $scope.model.ID = $scope.ListLucLuong.ID;
        }
       
    }

    $scope.cancel = function () {
        $scope.DSA = true;
        $scope.chgLL = false;
        $scope.chgstt = false;
        $scope.tenLLerror = false;
        $scope.tenVietTaterror = false;
        $scope.STTerror = false;
        $scope.GetLucLuong();
        $scope.searchLucLuong = "";
        $scope.showhideForm = true;
        $scope.showhideHuy = true;
        $scope.showhideLuu = true;
        $scope.model = {};
    }

    $scope.GetLucLuong = function () {
        $scope.ListPageMenu = [];
        $scope.chgLL = false;
        $scope.chgstt = false;
        $.ajax({
            type: 'post',
            url: '/LucLuong/GetLucLuong',
            data: { key: $scope.searchLucLuong },
            success: function (data) {
                $scope.ListPageMenu = data.TreeDatas;
                if ($scope.searchLucLuong != "" && $scope.searchLucLuong != null) {
                    setting = {
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
                        },
                        callback: {
                            onClick: suaLL
                        }
                    };
                } else {
                    setting = {
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
                            onClick: suaLL
                        }
                    };
                }
                $.fn.zTree.init($("#treeLucLuong"), setting, $scope.ListPageMenu);
                var zTree = $.fn.zTree.getZTreeObj("treeLucLuong");
                var type = { "Y": "s", "N": "ps" };
                zTree.setting.check.chkboxType = type;
                zTree.expandAll(false);
                $scope.$apply();
                hideLoading();
            },
            error: function (xhr, status, error) {
                toastr.error("Lỗi lấy danh sách lực lượng");
            }
        });
    }
    $scope.chgLL = false;
    $scope.chgstt = false;
    $scope.changeLL = function () {
        $scope.chgLL = true;
    }
    $scope.changeStt = function () {
        $scope.chgstt = true;
    }
    $scope.model = {};
    $scope.submit = function () {
        var flag = true;
        $scope.tenLLerror = false;
        $scope.tenVietTaterror = false;
        $scope.STTerror = false;
        if ($scope.model.ID > 0 && !$scope.RoleBtnEdit) {
            flag = false;
            toastr.error("Bạn không có quyền sửa lực lượng!");
        }
        if ($scope.model.TEN_LL == null || $scope.model.TEN_LL == "") {
            $scope.tenLLerror = true;
            flag = false;
            $scope.TenLLstringerror = "Bạn chưa nhập Tên lực lượng!";
            //toastr.error("Bạn chưa nhập Tên lực lượng!");
        }
        if ($scope.model.SO_THU_TU == null || $scope.model.SO_THU_TU == "") {
            $scope.STTerror = true;
            flag = false;
            $scope.STTstringerror = "Bạn chưa nhập số thứ tự!";
            //toastr.error("Bạn chưa nhập Tên lực lượng!");
        }
        if ($scope.model.KY_HIEU_VIET_TAT == null || $scope.model.KY_HIEU_VIET_TAT == "") {
            flag = false;
            $scope.tenVietTaterror = true;
            //toastr.error("Bạn chưa nhập ký hiệu viết tắt!");
        }
        if ($scope.model.SO_THU_TU < 0) {
            flag = false;
            $scope.STTerror = true;
            $scope.STTstringerror = "Bạn đang nhập số thứ tự âm, vui lòng nhập lại!";
            //toastr.error("Bạn đang nhập số thứ tự âm, vui lòng nhập lại!");
        } else {
            if ($scope.DSA == false) {
                $scope.ListLucLuong.map((item) => {
                    if (item.SO_THU_TU == $scope.model.SO_THU_TU) {
                        flag = false;
                        $scope.STTerror = true;
                        $scope.STTstringerror = "Số thứ tự đã tồn tại, vui lòng nhập lại!";
                        //toastr.error("Số thứ tự đã tồn tại, vui lòng nhập lại!");
                    }
                    if (item.TEN_LL == $scope.model.TEN_LL) {
                        flag = false;
                        $scope.tenLLerror = true;
                        $scope.TenLLstringerror = "Tên lực lượng đã tồn tại, vui lòng nhập lại!";
                        //toastr.error("Tên lực lượng đã tồn tại, vui lòng nhập lại!");
                    }
                });
            } else {
                if ($scope.chgLL == true) {
                    $scope.ListLucLuong.map((item) => {
                        if (item.TEN_LL == $scope.model.TEN_LL && $scope.model.EditId != item.ID) {
                            flag = false;
                            $scope.tenLLerror = true;
                            $scope.TenLLstringerror = "Tên lực lượng đã tồn tại, vui lòng nhập lại!";
                            //toastr.error("Tên lực lượng đã tồn tại, vui lòng nhập lại!");
                        }
                    });
                }
                if ($scope.chgstt == true) {
                    $scope.ListLucLuong.map((item) => {
                        if (item.SO_THU_TU == $scope.model.SO_THU_TU && $scope.model.EditId != item.ID) {
                            flag = false;
                            $scope.STTerror = true;
                            $scope.STTstringerror = "Số thứ tự đã tồn tại, vui lòng nhập lại!";
                            //toastr.error("Số thứ tự đã tồn tại, vui lòng nhập lại!");
                        }
                    });
                }
            }
            
        }
        if ($scope.model.EditId == undefined) {
            $scope.model.EditId = 0;
        }
        if (flag == true) {
                $.ajax({
                    type: 'post',
                    url: '/LucLuong/Add',
                    data: { lucLuong: $scope.model, idEdit: $scope.model.EditId },
                    success: function (data) {
                        if (data.Error) {
                            toastr.error(data.Title);
                        } else {
                            toastr.success(data.Title);
                            $scope.GetLucLuong();
                            GetAllLucLuong();
                            $scope.model = {};
                            $scope.DSA = true;
                            $scope.chgLL = false;
                            $scope.chgstt = false;
                            hideLoading();
                        }
                    }
                });
        }
    };
    $scope.delete = function () {
        var obj = $.fn.zTree.getZTreeObj("treeLucLuong").getSelectedNodes();
        if (obj[0].id == null || obj[0].id == "") {
            toastr.error("Bạn chưa chọn Lực Lượng để Xóa!");
            return;
        }
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn xóa dữ liệu đã chọn không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-red',
                    action: function (scope, button) {
                        $.ajax({
                            type: 'post',
                            url: '/LucLuong/Delete',
                            data: { Id: obj[0].id },
                            success: function (data) {
                                toastr.warning(data.notifyTitle);
                                $scope.GetLucLuong();
                                GetAllLucLuong();
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
    $scope.edit = function () {
        var obj = $.fn.zTree.getZTreeObj("treeLucLuong").getSelectedNodes();
        if (obj.length == 0) {
            toastr.error("Bạn chưa chọn Lực Lượng để Sửa!");
            return;
        }
        $.ajax({
            type: 'post',
            url: '/LucLuong/Edit',
            data: { Id: obj[0].id },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.model.TEN_LL = data.ObjectData[0].TEN_LL;
                    $scope.model.MO_TA = data.ObjectData[0].MO_TA;
                    $scope.model.ID = data.ObjectData[0].LL_CHA_ID;
                    $scope.model.KY_HIEU_VIET_TAT = data.ObjectData[0].KY_HIEU_VIET_TAT;
                    $scope.model.SO_THU_TU = data.ObjectData[0].SO_THU_TU;
                    $scope.model.IS_NHANTIEN = data.ObjectData[0].IS_NHANTIEN;
                    $scope.model.EditId = data.ObjectData[0].ID;
                    $scope.$apply();
                }
            }
        });
        
    }
});