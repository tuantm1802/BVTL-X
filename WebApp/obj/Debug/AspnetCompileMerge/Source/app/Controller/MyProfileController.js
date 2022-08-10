app.controller("MyProfileController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.MyProfile = {
        USER_ID:0,
        FULL_NAME: '',
        LOGIN_NAME: '',
        TEL_NO: '',
        LEVEL_ID: 0,
        USER_DESC: '',
        TITLE_NAME:''
    };
    $scope.ModelPassword = {
        PasswordOld: "",
        PasswordNew: "",
        PasswordReNew:""
    };
    $scope.IsInit = false;
    $scope.IsShowPassword = {
        passOld: false,
        passNew: false,
        passReNew: false,
    };
    angular.element(document).ready(function () {
        $scope.InitInfo();
    });
    $scope.anyCapBac = { ID: 0, TEN_CB: '-- Chọn cấp bậc --' };
    $scope.InitInfo = function () {
        $.get("/User/GetUserInfo", function (data) {
            if (data) {
                $scope.MyProfile = data.user;
                $scope.DmCapBac = data.capbac;
                $scope.DmCapBac.unshift($scope.anyCapBac);
                $scope.$apply();
            }
        });
    };
    $scope.validationOptions = {
        rules: {
            FULL_NAME: {
                required: true
            },
            LOGIN_NAME: {
                required: true
            },
        },
        messages: {
            FULL_NAME: {
                required: "Họ và tên không được để trống."
            },
            LOGIN_NAME: {
                required: "Tên đăng nhập không được để trống."
            }
        }
    };
    $scope.UpdateMyProfile = function () {
        if (!$scope.IsInit) {
            $scope.IsInit = true;
            return;
        }
        if ($scope.frmProfileInfo.validate()) {
            $.ajax({
                type: 'post',
                url: '/User/UpdateMyProfile',
                data: { user: $scope.MyProfile },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success("Cập nhật thông tin tài khoản thành công.");
                    }
                }
            });
        }
    };
    $scope.ChangePassword = function () {
        if ($scope.ModelPassword.PasswordOld == "" || $scope.ModelPassword.PasswordOld == null) {
            toastr.error("Bạn chưa nhập mật khẩu cũ.");
            return false;
        }
        if ($scope.ModelPassword.PasswordNew == "" || $scope.ModelPassword.PasswordNew == null) {
            toastr.error("Bạn chưa nhập mật khẩu mới.");
            return false;
        }

        if ($scope.ModelPassword.PasswordNew !== $scope.ModelPassword.PasswordReNew) {
            toastr.error("Bạn chưa nhập lại mật khẩu mới không đúng.");
            return false;
        }
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn thay đổi mật khẩu không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Có',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        $.ajax({
                            type: 'post',
                            url: '/User/PostChangePassword',
                            data: { passOld: $scope.ModelPassword.PasswordOld, passNew: $scope.ModelPassword.PasswordNew },
                            success: function (data) {
                                if (data.Error) {
                                    toastr.error(data.Title);
                                } else {
                                    toastr.success(data.Title);
                                    window.location.href = '/Login/Index';
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
    $scope.ResetModalChangePass = function () {
        $scope.ModelPassword = {
            PasswordOld: "",
            PasswordNew: "",
            PasswordReNew: ""
        };
    }
});
