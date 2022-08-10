app.controller("FlexValueController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $rootScope) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.FLEX_VALUE_SET_ID = 0;

    $scope.modelSearch.SortColumn = "FLEX_VALUE_ID DESC";
    $scope.ListData = [];
    $scope.ListFlexValueSet = [];
    $scope.ListColumn = [];
    $scope.FlexValueSetName = "";
    angular.element(document).ready(function () {
        showToast();
        GetBottomAction();

    });

    // Quyền chỉ được view dữ liệu đối với 1 danh mục động 
    $scope.RoleViewDataByFLS = true;
    $scope.CurrentAppCode = "";
    $scope.CurrentUserId = 0;

    $scope.RoleBtnCreate = false;
    $scope.RoleBtnUpdate = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnView = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/FlexValue/GetBottomAction',
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'btnUpdate') {
                            $scope.RoleBtnUpdate = true;
                        }
                        if (item == 'btnSearch') {
                            $scope.RoleBtnSearch = true;
                        }
                        if (item == 'btnDelete') {
                            $scope.RoleBtnDelete = true;
                        }
                        if (item == 'btnView') {
                            $scope.RoleBtnView = true;
                        }
                    });
                }
                $scope.CurrentAppCode = response.CurrentAppCode;
                $scope.CurrentUserId = response.UserId;
                $scope.RoleViewDataByFLS = true;
                if (response.FlexValueSets != null && response.FlexValueSets.length > 0) {
                    $scope.ListFlexValueSet = response.FlexValueSets;
                    $scope.modelSearch.FLEX_VALUE_SET_ID = $scope.ListFlexValueSet[0].FLEX_VALUE_SET_ID;
                    $scope.FlexValueSetName = response.FlexValueSets[0].DESCRIPTION;
                    $scope.ListColumn = response.FlexValueColumns;

                    CheckViewButtonCreate($scope.ListFlexValueSet[0]);
                }
                $scope.$apply();
                $scope.LoadPage();
            }
        });
    }

    // check quyền button thêm
    function CheckViewButtonCreate(fls) {
        $scope.RoleViewDataByFLS = true;

        // dm động có appcode null hoặc bằng của P1 thì người dùng sẽ được thêm, còn khác thì chỉ đúng app code mới dc thêm
        if (fls.APP_CODE == null || fls.APP_CODE == $rootScope.AppCodeP1) {
            $scope.RoleViewDataByFLS = false;
        } else {
            if ($scope.CurrentAppCode == fls.APP_CODE) {
                $scope.RoleViewDataByFLS = false;
            }
        }
    }

    // Show button sửa, xóa
    $scope.CheckShowEditDelete = function (item) {
        var role = false;
        for (var i = 0; i < $scope.ListFlexValueSet.length; i++) {
            if ($scope.ListFlexValueSet[i].FLEX_VALUE_SET_ID == $scope.modelSearch.FLEX_VALUE_SET_ID) {
                var fls = $scope.ListFlexValueSet[i];
                if (fls.APP_CODE == null || fls.APP_CODE == $rootScope.AppCodeP1) {
                    if ($scope.CurrentUserId == item.CREATED_BY || $scope.CurrentAppCode == $rootScope.AppCodeP1) {
                        role = true;
                    }
                } else {
                    if ($scope.CurrentAppCode == fls.APP_CODE) {
                        role = true;
                    }
                }
                break;
            }
        }

        return role;
    };

    $scope.pageChanged = function () {
        $scope.LoadPage();
    };

    $scope.ViewDetail = function (ID, rowIndex) {
        $scope.selectedRow = rowIndex;
    };

    $scope.LoadPage = function () {


        for (var i = 0; i < $scope.ListColumn.length; i++) {
            if ($scope.ListColumn[i].CONTROL_NAME == 'input' && $scope.ListColumn[i].CONTROL_TYPE == 'checkbox') {
                if ($scope.modelSearch[$scope.ListColumn[i].FLEX_COLUMN_NAME] == null || $scope.modelSearch[$scope.ListColumn[i].FLEX_COLUMN_NAME] == "") {
                    if ($scope.ListColumn[i].FLEX_COL_DATA_TYPE == 'char') {
                        $scope.modelSearch[$scope.ListColumn[i].FLEX_COLUMN_NAME] = 'N';
                    } else {
                        $scope.modelSearch[$scope.ListColumn[i].FLEX_COLUMN_NAME] = false;
                    }
                }
                else {
                    if ($scope.ListColumn[i].FLEX_COL_DATA_TYPE == 'char') {
                        if ($scope.modelSearch[$scope.ListColumn[i].FLEX_COLUMN_NAME] == true) {
                            $scope.modelSearch[$scope.ListColumn[i].FLEX_COLUMN_NAME] = 'Y';

                        } else if ($scope.modelSearch[$scope.ListColumn[i].FLEX_COLUMN_NAME] == false) {
                            $scope.modelSearch[$scope.ListColumn[i].FLEX_COLUMN_NAME] = 'N';
                        }
                    }
                }
            }

            if ($scope.ListColumn[i].CONTROL_NAME == 'select' && $scope.ListColumn[i].FLEX_COL_DATA_TYPE == 'int') {
                if ($scope.modelSearch[$scope.ListColumn[i].FLEX_COLUMN_NAME] != null && $scope.modelSearch[$scope.ListColumn[i].FLEX_COLUMN_NAME] != "") {
                    $scope.modelSearch[$scope.ListColumn[i].FLEX_COLUMN_NAME] = parseInt($scope.modelSearch[$scope.ListColumn[i].FLEX_COLUMN_NAME]);
                }
            }
        }
        $.ajax({
            type: 'post',
            url: '/FlexValue/GetAllByPage',
            data: $scope.modelSearch,
            success: function (data) {
                $scope.modelSearch.totalItems = data.totalItems;
                $scope.ListData = data.data;
                for (var i = 0; i < $scope.ListColumn.length; i++) {
                    if ($scope.ListColumn[i].CONTROL_NAME == 'input' && $scope.ListColumn[i].CONTROL_TYPE == 'checkbox' && $scope.ListColumn[i].FLEX_COL_DATA_TYPE == 'char') {
                        if ($scope.modelSearch[$scope.ListColumn[i].FLEX_COLUMN_NAME] == 'Y') {
                            $scope.modelSearch[$scope.ListColumn[i].FLEX_COLUMN_NAME] = true;

                        } else if ($scope.modelSearch[$scope.ListColumn[i].FLEX_COLUMN_NAME] == 'N') {
                            $scope.modelSearch[$scope.ListColumn[i].FLEX_COLUMN_NAME] =false ;
                        }
                    }
                }
                $scope.$apply();
                hideLoading();
            }
        });
    };

    $scope.ChangeFlexValueSet = function () {
        var FLEX_VALUE_SET_ID = $scope.modelSearch.FLEX_VALUE_SET_ID;
        $scope.modelSearch = {};
        $scope.modelSearch.FLEX_VALUE_SET_ID = FLEX_VALUE_SET_ID;
        showToast();

        for (var i = 0; i < $scope.ListFlexValueSet.length; i++) {
            if ($scope.ListFlexValueSet[i].FLEX_VALUE_SET_ID == $scope.modelSearch.FLEX_VALUE_SET_ID) {
                $scope.FlexValueSetName = $scope.ListFlexValueSet[i].DESCRIPTION;

                CheckViewButtonCreate($scope.ListFlexValueSet[i]);
            }
        }
        $.ajax({
            type: 'post',
            url: '/FlexValue/GetColumnByFlexValueSet',
            data: { flexValueSetId: $scope.modelSearch.FLEX_VALUE_SET_ID },
            success: function (data) {
                $scope.ListColumn = data.FlexValueColumns;
                
                $scope.$apply();
                $scope.LoadPage();
                hideLoading();
            }
        });
    };

    

    $scope.Refesh = function () {
        $scope.ChangeFlexValueSet();
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
            templateUrl: '/FlexValue/_Add',
            controller: 'add',
            size: 'xl',
            backdrop: 'static',
            resolve: {
                flexValueSetId: function () {
                    return $scope.modelSearch.FLEX_VALUE_SET_ID;
                }, flexValueSetName: function () {
                    return $scope.FlexValueSetName;
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    };
    $scope.edit = function (itemId) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/FlexValue/_Edit',
            controller: 'edit',
            size: 'xl',
            backdrop: 'static',
            resolve: {
                itemId: function () {
                    return itemId;
                },
                flexValueSetId: function () {
                    return $scope.modelSearch.FLEX_VALUE_SET_ID;
                }, flexValueSetName: function () {
                    return $scope.FlexValueSetName;
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    };
    $scope.delete = function (itemId, name) {
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn xóa ' + name + ' không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        $.ajax({
                            type: 'post',
                            url: '/FlexValue/Delete',
                            data: { Id: itemId },
                            success: function (data) {
                                if (data.Error) {
                                    toastr.error(data.Title);
                                } else {
                                    toastr.success(data.Title);
                                    //$scope.cancel();
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
    $scope.GetValue = function (columnName, item) {
        if (columnName == 'FLEX_VALUE_ID')
            return item.FLEX_VALUE_ID;

        if (columnName == 'FLEX_VALUE_SET_ID')
            return item.FLEX_VALUE_SET_ID;

        if (columnName == 'FLEX_VALUE')
            return item.FLEX_VALUE;

        if (columnName == 'LAST_UPDATE_DATE')
            return item.LAST_UPDATE_DATE;

        if (columnName == 'LAST_UPDATED_BY')
            return item.LAST_UPDATED_BY;

        if (columnName == 'CREATION_DATE')
            return item.CREATION_DATE;

        if (columnName == 'CREATED_BY')
            return item.CREATED_BY;

        if (columnName == 'DESCRIPTION')
            return item.DESCRIPTION;

        if (columnName == 'ENABLED_FLAG')
            return item.ENABLED_FLAG;

        if (columnName == 'SUMMARY_FLAG')
            return item.SUMMARY_FLAG;

        if (columnName == 'PARENT_FLEX_VALUE_ID')
            return item.PARENT_FLEX_VALUE_ID;

        if (columnName == 'HIERARCHY_LEVEL')
            return item.HIERARCHY_LEVEL;

        if (columnName == 'FLEX_VALUE_CATEGORY')
            return item.FLEX_VALUE_CATEGORY;

        if (columnName == 'ATTRIBUTE1')
            return item.ATTRIBUTE1;

        if (columnName == 'ATTRIBUTE2')
            return item.ATTRIBUTE2;

        if (columnName == 'ATTRIBUTE3')
            return item.ATTRIBUTE3;

        if (columnName == 'ATTRIBUTE4')
            return item.ATTRIBUTE4;

        if (columnName == 'ATTRIBUTE5')
            return item.ATTRIBUTE5;

        if (columnName == 'ATTRIBUTE6')
            return item.ATTRIBUTE6;

        if (columnName == 'ATTRIBUTE7')
            return item.ATTRIBUTE7;

        if (columnName == 'ATTRIBUTE8')
            return item.ATTRIBUTE8;

        if (columnName == 'ATTRIBUTE9')
            return item.ATTRIBUTE9;

        if (columnName == 'ATTRIBUTE10')
            return item.ATTRIBUTE10;
    };
});

app.controller('add', function ($scope, $uibModalInstance, flexValueSetId, flexValueSetName, $ngConfirm, showToast, hideLoading) {
    $scope.ListColumn = [];
    $scope.model = {};
    $scope.FlexValueSetName = "";
    angular.element(document).ready(function () {
        showToast();
        $scope.model.FLEX_VALUE_SET_ID = flexValueSetId;
        $scope.FlexValueSetName = "Thêm mới " + flexValueSetName;
        GetDanhMuc();
    });

    function GetDanhMuc() {
        $scope.ListColumn = {};

        $.ajax({
            type: 'post',
            url: '/FlexValue/GetDanhMuc',
            data: { flexValueSetId: $scope.model.FLEX_VALUE_SET_ID },
            success: function (data) {
                hideLoading();
                $scope.ListColumn = data.FlexValueColumns;
                $scope.$apply();
            }
        });
    }


    $scope.submit = function () {
        $("#formSubmit").validate({
            rules: {
                //FLEX_VALUE_SET_ID: {
                //    required: true
                //}
            },
            messages: {
                //FLEX_VALUE_SET_ID: {
                //    required: "Vui lòng chọn Danh mục"
                //}
            }
        });
        if ($("#formSubmit").valid()) {
            // Kiểm tra dữ liệu các trường bắt buộc nhập
            for (var i = 0; i < $scope.ListColumn.length; i++) {
                if ($scope.ListColumn[i].REQUIRED_FLAG == 'Y') {
                    if ($scope.ListColumn[i].CONTROL_NAME == 'input' && $scope.ListColumn[i].CONTROL_TYPE == 'checkbox') {
                        if ($scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] == null || $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] == "") {
                            if ($scope.ListColumn[i].FLEX_COL_DATA_TYPE == 'char') {
                                $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] = 'N';
                            } else {
                                $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] = false;
                            }
                        }
                        else {
                            if ($scope.ListColumn[i].FLEX_COL_DATA_TYPE == 'char') {
                                $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] = 'Y';
                            } else {
                                $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] = true;
                            }
                        }
                    } else {
                        if ($scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] == null || $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] == "") {
                            toastr.error("Bạn chưa nhập dữ liệu trường " + $scope.ListColumn[i].END_USER_COLUMN_NAME);
                            return;
                        } else {
                            if (($scope.ListColumn[i].FLEX_COL_DATA_TYPE == 'nvarchar' || $scope.ListColumn[i].FLEX_COL_DATA_TYPE == 'varchar') && $scope.ListColumn[i].FLEX_COL_DATA_LENGTH != null) {

                            }
                        }
                    }
                }

                // Check max length
                if ($scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] != null || $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] != "") {
                    if (($scope.ListColumn[i].FLEX_COL_DATA_TYPE == 'nvarchar' || $scope.ListColumn[i].FLEX_COL_DATA_TYPE == 'varchar') && $scope.ListColumn[i].FLEX_COL_DATA_LENGTH != null) {
                        var maxLength = parseInt($scope.ListColumn[i].FLEX_COL_DATA_LENGTH);
                        if (maxLength < $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME].length) {
                            toastr.error("Trường " + $scope.ListColumn[i].END_USER_COLUMN_NAME + " không được nhập quá " + maxLength + " ký tự!");
                            return;
                        }
                    }
                }

                if ($scope.ListColumn[i].CONTROL_NAME == 'select' && $scope.ListColumn[i].FLEX_COL_DATA_TYPE == 'int') {
                    if ($scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] != null && $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] != "") {
                        $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] = parseInt($scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME]);
                    }
                }
            }
            $.ajax({
                type: 'post',
                url: '/FlexValue/Add',
                data: { model: $scope.model },
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

app.controller('edit', function ($scope, $uibModalInstance, itemId, flexValueSetId, flexValueSetName, $ngConfirm, showToast, hideLoading) {
    $scope.ListColumn = [];
    $scope.FlexValueSetName = "";
    $scope.model = {};
    angular.element(document).ready(function () {
        $scope.ListColumn = [];
        $scope.model.FLEX_VALUE_SET_ID = flexValueSetId;
        $scope.FlexValueSetName = "Cập nhật " + flexValueSetName;
        $.ajax({
            type: 'post',
            url: '/FlexValue/GetItemByID',
            data: { Id: itemId },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.ListColumn = data.FlexValueColumns;
                    $scope.model = data.data;
                    $scope.$apply();
                }
            }
        });
    });

    $scope.submit = function () {
        $("#formSubmit").validate({
            rules: {
                //FLEX_VALUE_SET_ID: {
                //    required: true
                //}
            },
            messages: {
                //FLEX_VALUE_SET_ID: {
                //    required: "Vui lòng chọn Danh mục"
                //}
            }
        });
        if ($("#formSubmit").valid()) {

            // Kiểm tra dữ liệu các trường bắt buộc nhập
            for (var i = 0; i < $scope.ListColumn.length; i++) {
                if ($scope.ListColumn[i].REQUIRED_FLAG == 'Y') {
                    if ($scope.ListColumn[i].CONTROL_NAME == 'input' && $scope.ListColumn[i].CONTROL_TYPE == 'checkbox') {
                        if ($scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] == null || $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] == "") {
                            if ($scope.ListColumn[i].FLEX_COL_DATA_TYPE == 'char') {
                                $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] = 'N';
                            } else {
                                $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] = false;
                            }
                        }
                        else {
                            if ($scope.ListColumn[i].FLEX_COL_DATA_TYPE == 'char') {
                                $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] = 'Y';
                            } else {
                                $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] = true;
                            }
                        }
                    } else {
                        if ($scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] == null || $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] == "") {
                            toastr.error("Bạn chưa nhập dữ liệu trường " + $scope.ListColumn[i].END_USER_COLUMN_NAME);
                            return;
                        }
                    }
                }

                // Check max length
                if ($scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] != null || $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] != "") {
                    if (($scope.ListColumn[i].FLEX_COL_DATA_TYPE == 'nvarchar' || $scope.ListColumn[i].FLEX_COL_DATA_TYPE == 'varchar') && $scope.ListColumn[i].FLEX_COL_DATA_LENGTH != null) {
                        var maxLength = parseInt($scope.ListColumn[i].FLEX_COL_DATA_LENGTH);
                        if (maxLength < $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME].length) {
                            toastr.error("Trường " + $scope.ListColumn[i].END_USER_COLUMN_NAME + " không được nhập quá " + maxLength + " ký tự!");
                            return;
                        }
                    }
                }

                if ($scope.ListColumn[i].CONTROL_NAME == 'select' && $scope.ListColumn[i].FLEX_COL_DATA_TYPE == 'int') {
                    if ($scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] != null && $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] != "") {
                        $scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME] = parseInt($scope.model[$scope.ListColumn[i].FLEX_COLUMN_NAME]);
                    }
                }
            }

            $.ajax({
                type: 'post',
                url: '/FlexValue/Edit',
                data: { model: $scope.model },
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