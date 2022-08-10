app.controller("UserController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "FULL_NAME DESC";
    $scope.ListUserGroup = [];
    $scope.ListUser = [];

    angular.element(document).ready(function () {
        GetBottomAction();
        $scope.LoadPage();
    });

    $scope.RoleBtnCreate = false;
    $scope.RoleBtnUpdate = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnView = false;
    $scope.RoleBtnLock = false;
    $scope.RoleBtnUnlock = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/User/GetBottomAction',
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

                        if (item == 'btnLock') {
                            $scope.RoleBtnLock = true;
                        }
                        if (item == 'btnUnlock') {
                            $scope.RoleBtnUnlock = true;
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
            url: '/User/GetListUser',
            data: $scope.modelSearch,
            success: function (data) {
                $scope.modelSearch.totalItems = data.totalItems;
                $scope.ListUser = data.data;
                $scope.$apply();
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
            templateUrl: '/User/_Add',
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
            templateUrl: '/User/_Edit',
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

    $scope.cancel = function () {
        $uibModalInstance.close();
    };

    $scope.delete = function (itemId, Username) {
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn xóa tài khoản ' + Username + ' không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        showToast();
                        $.ajax({
                            type: 'post',
                            url: '/User/Delete',
                            data: { Id: itemId },
                            success: function (data) {
                                if (data.Error) {
                                    toastr.error(data.Title);
                                } else {
                                    toastr.success(data.Title);
                                    $scope.LoadPage();
                                }
                                hideLoading();
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

    $scope.lockUser = function (itemId) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/User/_LockUser',
            controller: 'lockUser',
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

    $scope.unlockUser = function (itemId, Username) {
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn mở khóa tài khoản ' + Username + ' không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        showToast();
                        $.ajax({
                            type: 'post',
                            url: '/User/UnLockUser',
                            data: { userId: itemId },
                            success: function (data) {
                                if (data.Error) {
                                    toastr.error(data.Title);
                                } else {
                                    toastr.success(data.Title);
                                    $scope.LoadPage();
                                }
                                hideLoading();
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

app.controller('add', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading) {
    $scope.ph_numbr = /(09|01[2|6|8|9])+([0-9]{8})\b/;
    $scope.ListUserGroup = [];
    $scope.ListRole = [];
    $scope.ListUnit = [];
    $scope.ListKho = [];
    $scope.IsShowChooseKho = false;
    $scope.FileName = "";
    $scope.ListSCOPE = [{ ID: 1, Name: 'Chức danh 1' }, { ID: 2, Name: 'Chức danh 2' }];
    $scope.ListLEVEL = [];
    angular.element(document).ready(function () {
        showToast();
        GetDanhMuc();
        initValidateAddUser();
    });

    function GetDanhMuc() {
        $scope.ListLEVEL = [];
        $.ajax({
            type: 'post',
            url: '/User/GetDanhMuc',
            data: {},
            success: function (data) {
                $scope.ListUserGroup = data.Roles;
                $scope.ListUnit = data.Units;
                $scope.RoleId = $scope.ListUserGroup[0].ID;
                $scope.model.UNIT_ID = $scope.ListUnit[0].UNIT_ID;
                $scope.ListLEVEL = data.CapBacs;
                $scope.model.LEVEL_ID = $scope.ListLEVEL[0].ID;
                $scope.ListKho = data.Khos;
                $scope.model.ID_KHO = $scope.ListKho[0].ID;
                $scope.model.Status = true;
                $scope.model.IS_APPROVER = false;
                $scope.$apply();
                hideLoading();
                $scope.IsShowChooseKho = false;
                // Kiểm tra xem có phải phòng 8 không
                angular.forEach($scope.ListUnit, function (value, key) {
                    if (value.UNIT_ID == $scope.model.UNIT_ID && value.UNIT_CODE == "P8") {
                        $scope.IsShowChooseKho = true;
                    }
                });
            }
        });
    }
    function initValidateAddUser() {
        $.validator.addMethod(
            "regex",
            function (value, element, regexp) {
                var check = false;
                return this.optional(element) || regexp.test(value);
            },
            "Tên đăng nhập không chứa ký tự đặc biệt."
        );
        $("#formSubmit").validate({
            rules: {
                LOGIN_NAME: {
                    required: true,
                    maxlength: 30,
                    regex: /^[a-zA-Z0-9]+$/
                },
                //RoleId: {
                //    required: true
                //},
                PASSWORD: {
                    required: true,
                    minlength: 6
                },
                RePASSWORD: {
                    required: true,
                    minlength: 6,
                    equalTo: "#PASSWORD-ADD"
                },
                FULL_NAME: {
                    required: true,
                    maxlength: 150
                },
                UNIT_ID: {
                    required: true
                },
                USER_GROUP: {
                    required: true
                },
                USER_CLASS: {
                    required: true
                },
                ListRole: {
                    required: true
                }
            },
            messages: {
                LOGIN_NAME: {
                    required: "Vui lòng nhập tên truy cập",
                    maxlength: "Tên truy cập không được vượt quá 30 ký tự"
                },
                //RoleId: {
                //    required: "Vui lòng chọn Nhóm quyền"
                //},
                PASSWORD: {
                    required: "Vui lòng nhập mật khẩu",
                    minlength: "Mật khẩu ít nhất là 6 ký tự"
                },
                RePASSWORD: {
                    required: "Vui lòng nhập mật khẩu nhắc lại",
                    minlength: "Mật khẩu ít nhất là 6 ký tự",
                    equalTo: "Mật khẩu nhắc lại không chính xác"
                },
                FULL_NAME: {
                    required: "Vui lòng nhập họ và tên",
                    maxlength: "Họ và tên không được vượt quá 150 ký tự"
                },
                UNIT_ID: {
                    required: "Vui lòng chọn Đơn vị"
                },
                USER_GROUP: {
                    required: "Vui lòng chọn Nhóm người dùng"
                },
                USER_CLASS: {
                    required: "Vui lòng chọn Cấp"
                },
                ListRole: {
                    required: "Vui lòng chọn nhóm quyền"
                }
            }
        });
    }
    $scope.model = { ENABLED_FLAG: 'Y' };
    $scope.submit = function () {
        if ($("#formSubmit").valid()) {
            var roleId = "";

            if ($scope.ListRole == null || $scope.ListRole.length == 0) {
                toastr.error("Bạn chưa chọn quyền cho người dùng.");
                return;
            }
            else {
                for (var i = 0; i < $scope.ListRole.length; i++) {
                    if (roleId == null || roleId == "")
                        roleId = $scope.ListRole[i];
                    else
                        roleId += "," + $scope.ListRole[i];
                }
            }
            var startDate = moment($scope.model.START_DATE, 'DD/MM/YYYY');
            var endDate = moment($scope.model.END_DATE, 'DD/MM/YYYY');
            if (startDate >= endDate) {
                toastr.error("Hiệu lực từ ngày phải nhỏ hơn hiệu lực đến ngày.");
                return;
            }
            if ($scope.model.START_DATE != null || $scope.model.START_DATE != "") {
                $scope.model.START_DATE = convertDate2($scope.model.START_DATE);
            }
            if ($scope.model.END_DATE != null || $scope.model.END_DATE != "") {
                $scope.model.END_DATE = convertDate2($scope.model.END_DATE);
            }
            if ($scope.model.IS_APPROVER == true) {
                $scope.model.IS_APPROVER = 'Y';
            } else {
                $scope.model.IS_APPROVER = 'N';
            }
            showToast();
            $.ajax({
                type: 'post',
                url: '/User/Add',
                data: { user: $scope.model, fileName: $scope.FileName, roleId: roleId },
                success: function (data) {
                    if (data.Error) {
                        $scope.model.START_DATE = convertDate4($scope.model.START_DATE);
                        $scope.model.END_DATE = convertDate4($scope.model.END_DATE);
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.cancel();
                    }
                    hideLoading();
                }
            });
        }
    };


    //  thay đổi đơn vị
    $scope.ChangeUnit = function () {
        $scope.IsShowChooseKho = false;
        // Kiểm tra xem có phải phòng 8 không
        angular.forEach($scope.ListUnit, function (value, key) {
            if (value.UNIT_ID == $scope.model.UNIT_ID && value.UNIT_CODE == "P8") {
                $scope.IsShowChooseKho = true;
            }
        });

        $.ajax({
            type: 'post',
            url: '/User/GetRoleByUnit',
            data: { unitId: $scope.model.UNIT_ID },
            success: function (data) {
                $scope.ListUserGroup = data.Roles;
                $scope.RoleId = $scope.ListUserGroup[0].ID;
                $scope.$apply();
            }
        });
    };

    $scope.cancel = function () {
        $uibModalInstance.close();
    };

    $scope.SelectFile = function (e) {
        $scope.model.IMG_PATH = "";
        $scope.FileName = "";
        if (e.target.files[0]) {
            if (e.target.files[0].size > 5242880) {
                toastr.error("Bạn không được tải file lên lớn quá 5M.");
            } else {
                $scope.FileName = e.target.files[0].name;
                $("#lableFile").text($scope.FileName);
                var reader = new FileReader();
                reader.onload = function (e1) {
                    var base64 = "";
                    var checkPNG = e1.target.result.split(',');
                    if (checkPNG != null && checkPNG.length > 0) {
                        base64 = checkPNG[checkPNG.length - 1]
                    }
                    $scope.model.IMG_PATH = base64;
                    $('#pathPhoto').attr('src', e1.target.result);
                };
                reader.readAsDataURL(e.target.files[0]);
            }
        }
    };
    function convertDate2(date) {
        if (date && date !== undefined && date !== null && date !== '') {
            var d = moment(date, 'DD/MM/YYYY');
            return d.format('YYYYMMDD');
        } else {
            return "";
        }
    }
    function convertDate4(date) {
        if (date !== undefined && date !== null) {
            var d = moment(date, 'YYYYMMDD');
            return d.format('DD/MM/YYYY');
        } else {
            return "";
        }
    }
});

app.controller('edit', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {
    $scope.ph_numbr = /(09|01[2|6|8|9])+([0-9]{8})\b/;
    $scope.ListUserGroup = [];
    $scope.ListUnit = [];
    $scope.ListRole = [];
    $scope.ListKho = [];
    $scope.FileName = "";
    $scope.model = {};
    $scope.ListTITLE = [{ ID: 1, Name: 'Chức danh 1' }, { ID: 2, Name: 'Chức danh 2' }];
    $scope.ListLEVEL = [];
    angular.element(document).ready(function () {
        showToast();
        GetDanhMuc();
        $scope.ListRole = [];
        setTimeout(function () {
            $.ajax({
                type: 'post',
                url: '/User/GetItemByID',
                data: { Id: itemId },
                success: function (data) {
                    hideLoading();
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        $scope.model = data.data;

                        for (var j = 0; j < data.RoleId.length; j++) {
                            $scope.ListRole.push(data.RoleId[j]);
                        }
                        //$scope.ListRole = data.RoleId;
                        if (data.START_DATE != null || data.START_DATE != "") {
                            $scope.model.START_DATE = convertDate4($scope.model.START_DATE);
                        }
                        if (data.END_DATE != null || data.END_DATE != "") {
                            $scope.model.END_DATE = convertDate4($scope.model.END_DATE);
                        }
                        if ($scope.model.IMG_PATH != null || $scope.model.IMG_PATH != "") {
                            ConvertImageToBase64($scope.model.IMG_PATH);
                        }

                        if ($scope.model.IS_APPROVER == 'Y') {
                            $scope.model.IS_APPROVER = true;
                        } else {
                            $scope.model.IS_APPROVER = false;
                        }
                        $scope.IsShowChooseKho = false;
                        // Kiểm tra xem có phải phòng 8 không
                        angular.forEach($scope.ListUnit, function (value, key) {
                            if (value.UNIT_ID == $scope.model.UNIT_ID && value.UNIT_CODE == "P8") {
                                $scope.IsShowChooseKho = true;
                            }
                        });
                        $scope.$apply();
                        $('#formSubmitEdit select[name=ListRole]').change();//fix bug không hiển thị nhóm quyền
                    }
                }
            });
        }, 10);
        initValidateEditUser();
    });

    $scope.IsShowChooseKho = false;

    //  thay đổi đơn vị
    $scope.ChangeUnit = function () {
        $scope.IsShowChooseKho = false;
        // Kiểm tra xem có phải phòng 8 không
        angular.forEach($scope.ListUnit, function (value, key) {
            if (value.UNIT_ID == $scope.model.UNIT_ID && value.UNIT_CODE == "P8") {
                $scope.IsShowChooseKho = true;
            }
        });

        $.ajax({
            type: 'post',
            url: '/User/GetRoleByUnit',
            data: { unitId: $scope.model.UNIT_ID },
            success: function (data) {
                $scope.ListUserGroup = data.Roles;
                $scope.RoleId = $scope.ListUserGroup[0].ID;
                $scope.$apply();
            }
        });
    };

    function GetDanhMuc() {
        $.ajax({
            type: 'post',
            url: '/User/GetDanhMuc',
            data: {},
            success: function (data) {
                $scope.ListUserGroup = data.Roles;
                $scope.ListUnit = data.Units;
                $scope.ListLEVEL = data.CapBacs;
                $scope.ListKho = data.Khos;
                $scope.model.Status = true;
                $scope.$apply();
                hideLoading();
            }
        });
    }
    function initValidateEditUser() {
        $.validator.addMethod(
            "regex",
            function (value, element, regexp) {
                var check = false;
                return this.optional(element) || regexp.test(value);
            },
            "Tên đăng nhập không chứa ký tự đặc biệt."
        );
        $("#formSubmitEdit").validate({
            rules: {
                LOGIN_NAME: {
                    required: true,
                    maxlength: 30,
                    //regex: /^[a-zA-Z0-9]+$/
                },
                //RoleId: {
                //    required: true
                //},
                PASSWORD: {
                    minlength: 6
                },
                RePASSWORD: {
                    minlength: 6,
                    equalTo: "#PASSWORD-EDIT"
                },
                FULL_NAME: {
                    required: true,
                    maxlength: 150
                },
                UNIT_ID: {
                    required: true
                },
                USER_GROUP: {
                    required: true
                },
                USER_CLASS: {
                    required: true
                },
                ListRole: {
                    required: true
                }
            },
            messages: {
                LOGIN_NAME: {
                    required: "Vui lòng nhập tên truy cập",
                    maxlength: "Tên truy cập không được vượt quá 30 ký tự"
                },
                //RoleId: {
                //    required: "Vui lòng chọn Nhóm quyền"
                //},
                PASSWORD: {
                    minlength: "Mật khẩu ít nhất là 6 ký tự"
                },
                RePASSWORD: {
                    minlength: "Mật khẩu ít nhất là 6 ký tự",
                    equalTo: "Mật khẩu nhắc lại không chính xác"
                },
                FULL_NAME: {
                    required: "Vui lòng nhập họ và tên",
                    maxlength: "Họ và tên không được vượt quá 150 ký tự"
                },
                UNIT_ID: {
                    required: "Vui lòng chọn Đơn vị"
                },
                USER_GROUP: {
                    required: "Vui lòng chọn Nhóm người dùng"
                },
                USER_CLASS: {
                    required: "Vui lòng chọn Cấp"
                },
                ListRole: {
                    required: "Vui lòng chọn nhóm quyền"
                }
            }
        });
    }
    $scope.submit = function () {
        if ($("#formSubmitEdit").valid()) {

            var roleId = "";

            if ($scope.ListRole == null || $scope.ListRole.length == 0) {
                toastr.error("Bạn chưa chọn quyền cho người dùng.");
                return;
            }
            else {
                for (var i = 0; i < $scope.ListRole.length; i++) {
                    if (roleId == null || roleId == "")
                        roleId = $scope.ListRole[i];
                    else
                        roleId += "," + $scope.ListRole[i];
                }
            }
            var startDate = moment($scope.model.START_DATE, 'DD/MM/YYYY');
            var endDate = moment($scope.model.END_DATE, 'DD/MM/YYYY');
            if (startDate >= endDate) {
                toastr.error("Hiệu lực từ ngày phải nhỏ hơn hiệu lực đến ngày.");
                return;
            }
            if ($scope.model.START_DATE != null || $scope.model.START_DATE != "") {
                if ($scope.model.START_DATE.indexOf("/") >= 0) {
                    var array = $scope.model.START_DATE.split('/');
                    $scope.model.START_DATE = array[2] + array[1] + array[0];
                }
                else {
                    $scope.model.START_DATE = moment($scope.model.START_DATE).format("YYYYMMDD");
                }
            }
            if ($scope.model.END_DATE != null || $scope.model.END_DATE != "") {
                if ($scope.model.END_DATE.indexOf("/") >= 0) {
                    var array = $scope.model.END_DATE.split('/');
                    $scope.model.END_DATE = array[2] + array[1] + array[0];
                }
                else {
                    $scope.model.END_DATE = moment($scope.model.END_DATE).format("YYYYMMDD");
                }
            }

            if ($scope.model.IS_APPROVER == true) {
                $scope.model.IS_APPROVER = 'Y';
            } else {
                $scope.model.IS_APPROVER = 'N';
            }
            showToast();
            $.ajax({
                type: 'post',
                url: '/User/Edit',
                data: { user: $scope.model, fileName: $scope.FileName, roleId: roleId },
                success: function (data) {
                    if (data.Error) {
                        $scope.model.START_DATE = convertDate4($scope.model.START_DATE);
                        $scope.model.END_DATE = convertDate4($scope.model.END_DATE);
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.cancel();
                    }
                    hideLoading();
                }
            });
        }
    };

    $scope.cancel = function () {
        $uibModalInstance.close();
    };

    $scope.SelectFile = function (e) {
        $scope.model.IMG_PATH = "";
        $scope.FileName = "";
        if (e.target.files[0]) {
            if (e.target.files[0].size > 5242880) {
                toastr.error("Bạn không được tải file lên lớn quá 5M.");
            } else {
                $scope.FileName = e.target.files[0].name;
                $("#lableFile").text($scope.FileName);
                var reader = new FileReader();
                reader.onload = function (e1) {
                    var base64 = "";
                    var checkPNG = e1.target.result.split(',');
                    if (checkPNG != null && checkPNG.length > 0) {
                        base64 = checkPNG[checkPNG.length - 1]
                    }
                    $scope.model.IMG_PATH = base64;
                    $('#pathPhoto').attr('src', e1.target.result);
                };
                reader.readAsDataURL(e.target.files[0]);
            }
        }
    };

    //$scope.ChangeEmployee = function () {

    //    var documentCheck = $scope.ListEmployee.filter(function (x) {
    //        return (x.Id == $scope.model.EmployeeId);
    //    });
    //    if (documentCheck != null && documentCheck.length > 0) {
    //        $scope.model.Name = documentCheck[0].FullName;
    //        $scope.model.Phone = documentCheck[0].Phone;
    //        $scope.model.Email = documentCheck[0].Email;
    //        if (documentCheck[0].AvatarPath != null && documentCheck[0].AvatarPath != "") {
    //            $.ajax({
    //                type: 'POST',
    //                dataType: 'json',
    //                cache: false,
    //                async: true,
    //                url: '/User/ConvertPathImageToBase64',
    //                data: {
    //                    path: documentCheck[0].AvatarPath
    //                },
    //                success: function (response) {
    //                    var base64File = "";
    //                    if (response.data != null && response.data != "") {
    //                        $scope.model.IMG_PATH = response.data;
    //                        base64File = 'data:image/jpeg;base64,' + response.data;
    //                    }
    //                    else {
    //                        base64File = '/Content/Images/noimg.png';
    //                    }
    //                    $('#pathPhoto').attr('src', base64File);
    //                }
    //                , error: function (xhr) {
    //                }
    //            });
    //        }
    //    }
    //};

    //type: 1 là create, 2 là update, 3 là detail
    function ConvertImageToBase64(path) {
        $.ajax({
            type: 'POST',
            dataType: 'json',
            cache: false,
            async: true,
            url: '/User/ConvertPathImageToBase64',
            data: {
                path: path
            },
            success: function (response) {
                var base64File = "";
                if (response.data != null && response.data != "") {
                    base64File = 'data:image/jpeg;base64,' + response.data;
                }
                else {
                    base64File = '/Content/Images/noimg.png';
                }
                $('#pathPhoto').attr('src', base64File);
            }
            , error: function (xhr) {
            }
        });
    }
    function convertDate4(date) {
        if (date !== undefined && date !== null) {
            var d = moment(date, 'YYYYMMDD');
            return d.format('DD/MM/YYYY');
        } else {
            return "";
        }
    }
});

app.controller('lockUser', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {
    $scope.Content = "";
    $scope.ItemId = 0;

    angular.element(document).ready(function () {
        $scope.ItemId = itemId;
    });

    $scope.submit = function () {
        $("#formSubmitLock").validate({
            rules: {

            },
            messages: {

            }
        });
        if ($("#formSubmitLock").valid()) {
            if ($scope.Content == null || $scope.Content == "") {
                toastr.error("Bạn chưa nhập ý kiến khóa người dùng.");
                return;
            }
            $.ajax({
                type: 'post',
                url: '/User/LockUser',
                data: { userId: $scope.ItemId, content: $scope.Content },
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