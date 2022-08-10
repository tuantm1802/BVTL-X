app.controller("SysParameterController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "PARAM_CODE DESC";

    angular.element(document).ready(function () {
        showToast();
        GetBottomAction();
        $scope.LoadPage();
    });

    $scope.RoleBtnUpdate = false;
    $scope.RoleBtnSearch = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/SysParameter/GetBottomAction',
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnUpdate') {
                            $scope.RoleBtnUpdate = true;
                        }
                        if (item == 'btnSearch') {
                            $scope.RoleBtnSearch = true;
                        }
                    });
                }
                $scope.$apply();
            }
        });
    }

    $scope.pageChanged = function () {
        $scope.LoadPage();
    };

    $scope.ViewDetail = function (ID, rowIndex) {
        $scope.selectedRow = rowIndex;
    };

    $scope.LoadPage = function () {
        $.ajax({
            type: 'post',
            url: '/SysParameter/GetAll',
            data: $scope.modelSearch,
            success: function (data) {
                $scope.modelSearch.totalItems = data.totalItems;
                $scope.ListSysParameter = data.data;
                $scope.modelSearch.pageSize = data.pageSize;
                $scope.$apply();
                hideLoading();
            }
        });
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
            templateUrl: '/SysParameter/_Add',
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
            templateUrl: '/SysParameter/_Edit',
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
    $scope.delete = function (itemId, Username) {
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn xóa tham số ' + Username + ' không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        $.ajax({
                            type: 'post',
                            url: '/SysParameter/Delete',
                            data: { Id: itemId },
                            success: function (data) {
                                if (data.Error) {
                                    toastr.error(data.Title);
                                } else {
                                    toastr.success(data.Title);
                                    $scope.LoadPage();
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
    };
});

app.controller('edit', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {

    $scope.model = {};
    angular.element(document).ready(function () {
        $.ajax({
            type: 'post',
            url: '/SysParameter/GetItemByID',
            data: { Id: itemId },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.model = data.data;
                    $scope.$apply();
                }
            }
        });
    });
    $scope.ListValueType = [{ value: 'STRING', text: 'Kiểu chữ' }, { value: 'NUMBER', text: 'Kiểu số' }];
    $scope.submit = function () {
        $("#formSubmit").validate({
            rules: {
                ParamCode: {
                    required: true,
                    maxlength: 50
                },
                ParamValue: {
                    required: true,
                    maxlength: 500
                },
                ParamValueType: {
                    required: true
                },
                Desctiption: {
                    maxlength: 1000
                }
            },
            messages: {
                ParamCode: {
                    required: "Vui lòng nhập mã",
                    maxlength: "Mã không được vượt quá 50 ký tự"
                },
                ParamValue: {
                    required: "Vui lòng nhập giá trị",
                    maxlength: "Mã không được vượt quá 500 ký tự"
                },
                ParamValueType: {
                    required: "Vui lòng chọn kiểu dữ liệu"
                }
            }
        });
        if ($("#formSubmit").valid()) {
            // kiểm tra xem có nhập đúng kiểu dữ liệu không
            if ($scope.model.PARAM_VALUE_TYPE == 'NUMBER' && !(!isNaN($scope.model.PARAM_VALUE) && angular.isNumber(+$scope.model.PARAM_VALUE))) {
                toastr.error("Bạn nhập không đúng kiểu số.");
                return;
            }

            $.ajax({
                type: 'post',
                url: '/SysParameter/Edit',
                data: $scope.model,
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
});