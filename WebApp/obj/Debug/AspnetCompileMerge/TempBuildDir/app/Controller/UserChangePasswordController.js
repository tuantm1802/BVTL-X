app.controller("UserChangePasswordController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.PasswordOld = "";
    $scope.PasswordNew = "";
    $scope.PasswordReNew = "";

    angular.element(document).ready(function () {
        vsblockui.disable({ message: 'Đang tải...' });
        $scope.LoadPage();
    });

    $scope.LoadPage = function () {
        vsblockui.enable();
    };

    $scope.ChangePassword = function () {
        if ($scope.PasswordOld == "" || $scope.PasswordOld == null) {
            toastr.error("Bạn chưa nhập mật khẩu cũ.");
            return false;
        }
        if ($scope.PasswordNew == "" || $scope.PasswordNew == null) {
            toastr.error("Bạn chưa nhập mật khẩu mới.");
            return false;
        }

        if ($scope.PasswordNew !== $scope.PasswordReNew) {
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
                            data: { passOld: $scope.PasswordOld, passNew: $scope.PasswordNew },
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
});
