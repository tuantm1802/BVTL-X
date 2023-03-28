app.controller("UserController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "UserName";
    $scope.ListUserGroup = [];
    $scope.ListUser = [];
    var dataTableUser = null;
    $scope.UserIdSeleted = 0;
    angular.element(document).ready(function () {
        GetBottomAction();

        $scope.LoadPage(1);
    });

    $scope.RoleBtnCreate = false;
    $scope.RoleBtnUpdate = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnView = false;

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
                    });
                }
                $scope.$apply();
            }
        });
    }

    $scope.pageChanged = function () {
        $scope.LoadPage(0);
    };

    $scope.ViewDetail = function () {

        var seletedRow = dataTableUser.rows({ selected: true });
        var count = seletedRow.count();
        if (count > 0) {
            $scope.UserIdSeleted = seletedRow.data()[0].ID;
        } else {
            $scope.UserIdSeleted = 0;
        }

        if ($scope.UserIdSeleted > 0 && $scope.UserIdSeleted != undefined) {
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/User/_View',
                controller: 'view',
                size: 'xl',
                backdrop: 'static',
                resolve: {
                    itemId: function () {
                        return $scope.UserIdSeleted;
                    }
                }
            });

            //kết quả trả về của modal
            modalInstance.result.then(function (response) {
                $scope.LoadPage(0);
            });
        } else {
            toastr.error("Bạn chưa chọn bản ghi nào.");
        }
    };

    $('#dataTableUser').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $scope.LoadPage = function (genTable) {
        showToast();
        //$.ajax({
        //    type: 'post',
        //    url: '/User/GetListUser',
        //    data: $scope.modelSearch,
        //    success: function (data) {
        //        //$scope.modelSearch.totalItems = data.totalItems;
        //        $scope.ListUser = data.data;

        //        $scope.$apply();
        //        if (genTable == 1) {
        //            $('#dataTableUser').DataTable({ "filter": false });
        //        }
        //    }
        //});

        $scope.ListUser = [];
        if (genTable == 1) {
            dataTableUser = $('#dataTableUser').DataTable({
                lengthMenu: [10, 20, 30, 50, 60, 100],
                serverSide: true,
                ordering: false,
                searching: false,
                ajax: function (data, callback, settings) {
                    var dataUser = [];
                    var totalItems = 0;
                    var page = ((data.start / data.length) + 1);

                    $scope.modelSearch.currentPage = page;
                    $scope.modelSearch.pageSize = data.length;

                    $.ajax({
                        type: 'post',
                        url: '/User/GetListUser',
                        cache: false,
                        async: false,
                        data: $scope.modelSearch,
                        success: function (respone) {
                            totalItems = respone.totalItems;
                            $scope.ListUser = respone.data;
                            if (respone.data != null && respone.data.length > 0) {
                                for (var i = 0; i < respone.data.length; i++) {
                                    var tmp = {
                                        STT: i + 1,
                                        UserName: respone.data[i].UserName,
                                        Name: respone.data[i].Name,
                                        IdNumber: (respone.data[i].IdNumber == null || respone.data[i].IdNumber == undefined) ? '' : respone.data[i].IdNumber,
                                        RoleName: respone.data[i].RoleName,
                                        Status: respone.data[i].Status == true ? 'Active' : 'In Active',
                                        ID: respone.data[i].ID
                                    }
                                    dataUser.push(tmp);
                                }
                            }
                        }
                    });

                    setTimeout(function () {
                        callback({
                            draw: data.draw,
                            data: dataUser,
                            recordsTotal: totalItems,
                            recordsFiltered: totalItems
                        });
                    }, 50);
                },
                rowId: 'ID',
                select: {
                    info: false
                },
                "language": {
                    "emptyTable": "Không có dữ liệu trong bản",
                    "info": "Hiển thị _START_ đến _END_ của _TOTAL_ bản ghi",
                    "infoEmpty": "Hiển thị 0 đến 0 của 0 bản ghi",
                    "infoFiltered": "(lọc từ _MAX_ tổng bản ghi)",
                    "lengthMenu": "Hiển thị _MENU_ bản ghi",
                    "loadingRecords": "Đang tải...",
                    "search": "Tìm kiếm:",
                    "zeroRecords": "Không tìm thấy kết quả",
                    "paginate": {
                        "first": "<<",
                        "last": ">>",
                        "next": ">",
                        "previous": "<"
                    },
                },
                columns: [
                    { "data": "STT", },
                    { "data": "UserName" },
                    { "data": "Name" },
                    { "data": "IdNumber" },
                    { "data": "RoleName" },
                    { "data": "Status" },
                    //{
                    //    "data": "ID", "render": function (data) {

                    //        return '<button type="button" class="btn btn-sm btn-primary" title="Cập nhật" style="margin-right: 5px;" ng-click="edit(' + data+')"> <i class="fas fa-pencil-alt" ></i></button>'
                    //            + '<button type="button" class="btn btn-sm btn-danger"  title="Xóa" style="margin-right: 5px;" ng-click="delete(' + data +')"><i class="fas fa-times"></i> </button>'
                    //            + '<button type="button" class="btn btn-sm btn-primary"  title="Xem" ng-click="ViewDetail(' + data +')"> <i class="fas fa-eye"></i></button>'

                    //    }
                    //}
                ],
                dom: "<'row'<'col-sm-12'f>>" +
                    "<'row'<'col-sm-12'tr>>" +
                    "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>",

                // dom: '<"top"f>rt<"bottom"ilp><"clear">',
                // scrollY: 200,
                scroller: {
                    loadingIndicator: true
                },
            });
        } else {
            dataTableUser.ajax.reload();
        }
        hideLoading();
    };

    $scope.Refesh = function () {
        $scope.LoadPage(0);
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
            $scope.LoadPage(0);
        });
    };
    $scope.edit = function () {
        var seletedRow = dataTableUser.rows({ selected: true });
        var count = seletedRow.count();
        if (count > 0) {
            $scope.UserIdSeleted = seletedRow.data()[0].ID;
        } else {
            $scope.UserIdSeleted = 0;
        }

        if ($scope.UserIdSeleted > 0 && $scope.UserIdSeleted != undefined) {
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/User/_Edit',
                controller: 'edit',
                size: 'xl',
                backdrop: 'static',
                resolve: {
                    itemId: function () {
                        return $scope.UserIdSeleted;
                    }
                }
            });

            //kết quả trả về của modal
            modalInstance.result.then(function (response) {
                $scope.LoadPage(0);
            });
        } else {
            toastr.error("Bạn chưa chọn bản ghi nào.");
        }
    };

    $scope.resetPassword = function () {
        var seletedRow = dataTableUser.rows({ selected: true });
        var count = seletedRow.count();
        if (count > 0) {
            $scope.UserIdSeleted = seletedRow.data()[0].ID;
        } else {
            $scope.UserIdSeleted = 0;
        }

        if ($scope.UserIdSeleted > 0 && $scope.UserIdSeleted != undefined) {
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/User/_ResetPassword',
                controller: 'resetPassword',
                size: 'xl',
                backdrop: 'static',
                resolve: {
                    itemId: function () {
                        return $scope.UserIdSeleted;
                    }
                }
            });

            //kết quả trả về của modal
            modalInstance.result.then(function (response) {
                $scope.LoadPage(0);
            });
        } else {
            toastr.error("Bạn chưa chọn bản ghi nào.");
        }
    };

    $scope.cancel = function () {
        $uibModalInstance.close();
    };

    $scope.delete = function (itemId) {
        var seletedRow = dataTableUser.rows({ selected: true });
        var count = seletedRow.count();
        if (count > 0) {
            $scope.UserIdSeleted = seletedRow.data()[0].ID;
        } else {
            $scope.UserIdSeleted = 0;
        }

        if ($scope.UserIdSeleted > 0 && $scope.UserIdSeleted != undefined) {
            var Username = $scope.ListUser.filter(function (item) {
                return item.ID === $scope.UserIdSeleted;
            })[0].UserName;

            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn bỏ hiệu lực người dùng ' + Username + ' không?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Bỏ hiệu lực',
                        btnClass: 'btn-primary',
                        action: function (scope, button) {
                            showToast();
                            $.ajax({
                                type: 'post',
                                url: '/User/Delete',
                                data: { Id: $scope.UserIdSeleted },
                                success: function (data) {
                                    if (data.Error) {
                                        toastr.error(data.Title);
                                    } else {
                                        toastr.success(data.Title);
                                        $scope.LoadPage(0);
                                    }
                                    hideLoading();
                                }
                            });
                        }
                    },
                    close: {
                        text: 'Hủy',
                        btnClass: 'btn-secondary',
                        action: function (scope, button) {

                        }
                    }
                }
            });
        } else {
            toastr.error("Bạn chưa chọn bản ghi nào.");
        }
    };

    $scope.activeUser = function (itemId) {
        var seletedRow = dataTableUser.rows({ selected: true });
        var count = seletedRow.count();
        if (count > 0) {
            $scope.UserIdSeleted = seletedRow.data()[0].ID;
        } else {
            $scope.UserIdSeleted = 0;
        }

        if ($scope.UserIdSeleted > 0 && $scope.UserIdSeleted != undefined) {
            var Username = $scope.ListUser.filter(function (item) {
                return item.ID === $scope.UserIdSeleted;
            })[0].UserName;

            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn cập nhật hiệu lực người dùng ' + Username + ' không?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Hiệu lực',
                        btnClass: 'btn-primary',
                        action: function (scope, button) {
                            showToast();
                            $.ajax({
                                type: 'post',
                                url: '/User/ActiveUser',
                                data: { Id: $scope.UserIdSeleted },
                                success: function (data) {
                                    if (data.Error) {
                                        toastr.error(data.Title);
                                    } else {
                                        toastr.success(data.Title);
                                        $scope.LoadPage(0);
                                    }
                                    hideLoading();
                                }
                            });
                        }
                    },
                    close: {
                        text: 'Hủy',
                        btnClass: 'btn-secondary',
                        action: function (scope, button) {

                        }
                    }
                }
            });
        } else {
            toastr.error("Bạn chưa chọn bản ghi nào.");
        }
    };
});

app.controller('add', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading) {
    $scope.ph_numbr = /(09|01[2|6|8|9])+([0-9]{8})\b/;
    $scope.ListUserGroup = [];
    $scope.ListTestGroup = [];
    $scope.ListTestGroupId = [];
    $scope.ListCity = [];
    $scope.ListCityCode = [];
    $scope.ListDuAn = [];
    $scope.ListMaDuAn = [];
    $scope.FileName = "";
    angular.element(document).ready(function () {
        $scope.ListTestGroupId = [];
        $scope.ListMaDuAn = [];
        showToast();
        GetDanhMuc();
    });

    function GetDanhMuc() {
        $scope.ListUserGroup = [];
        $scope.ListTestGroup = [];
        $scope.ListCity = [];
        $scope.ListDuAn = [];
        $.ajax({
            type: 'post',
            url: '/User/GetDanhMuc',
            cache: false,
            async: false,
            data: {},
            success: function (data) {
                if (data.DataRoles != null && data.DataRoles.length > 0) {
                    $scope.ListUserGroup = data.DataRoles;
                    $scope.model.GroupID = $scope.ListUserGroup[0].ID;
                }

                if (data.DataTestGroup != null && data.DataTestGroup.length > 0) {
                    $scope.ListTestGroup = data.DataTestGroup;
                }

                if (data.Citys != null && data.Citys.length > 0) {
                    $scope.ListCity = data.Citys;
                }
                if (data.DuAns != null && data.DuAns.length > 0) {
                    $scope.ListDuAn = data.DuAns;
                }

                $scope.ListStatus = [{ ID: true, Name: 'Sử dụng' }, { ID: false, Name: 'Không sử dụng' }];
                $scope.model.Status = true;
                $scope.model.Gender = "M";
                $scope.$apply();
                hideLoading();
            }
        });
    }

    $scope.model = {};
    $scope.submit = function () {
        if ($scope.ListMaDuAn == null || $scope.ListMaDuAn.length == 0) {
            toastr.error("Bạn chưa chọn Dự án quản lý");
            return false;
        } else {
            for (var i = 0; i < $scope.ListMaDuAn.length; i++) {
                if ($scope.model.MaDuAn == null || $scope.model.MaDuAn == '')
                    $scope.model.MaDuAn = $scope.ListMaDuAn[i];
                else
                    $scope.model.MaDuAn += ',' + $scope.ListMaDuAn[i];
            }
        }

        showToast();

        $("#formSubmit").validate({
            rules: {
                UserName: {
                    required: true,
                    maxlength: 50
                },
                Status: {
                    required: true
                },
                GroupID: {
                    required: true
                },
                Name: {
                    required: true,
                    maxlength: 250
                }
                //,
                //MaDuAn: {
                //    required: true
                //}
            },
            messages: {
                UserName: {
                    required: "Vui lòng nhập tên đăng nhập",
                    maxlength: "Tên đăng nhập không được vượt quá 50 ký tự"
                },
                Status: {
                    required: "Vui lòng chọn trạng thái"
                },
                Email: {
                    email: "Vui lòng nhập đúng định dạng email"
                }, GroupID: {
                    required: "Vui lòng chọn quyền"
                }, Name: {
                    required: "Vui lòng nhập họ và tên",
                    maxlength: "Họ và tên không được vượt quá 250 ký tự"
                }
                //,
                //MaDuAn: {
                //    required: "Vui lòng chọn Dự án quản lý"
                //}
            }
        });
        if ($("#formSubmit").valid()) {
            $.ajax({
                type: 'post',
                url: '/User/Add',
                data: { user: $scope.model, fileName: $scope.FileName, testGroupMa: $scope.ListTestGroupId, cityCodes: $scope.ListCityCode },
                success: function (data) {
                    if (data.Error) {
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
        $scope.model.Avartar = "";
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
                    $scope.model.Avartar = base64;
                    $('#pathPhoto').attr('src', e1.target.result);
                };
                reader.readAsDataURL(e.target.files[0]);
            }
        }
    };
});

app.controller('edit', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {
    $scope.ListUserGroup = [];
    $scope.ListTestGroup = [];
    $scope.ListTestGroupId = [];
    $scope.ListCity = [];
    $scope.ListCityCode = [];
    $scope.ListDuAn = [];
    $scope.ListMaDuAn = [];
    $scope.FileName = "";
    $scope.model = {};
    angular.element(document).ready(function () {
        showToast();
        GetDanhMuc();
        $.ajax({
            type: 'post',
            url: '/User/GetItemByID',
            cache: false,
            async: false,
            data: { Id: itemId },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.ListStatus = [{ ID: true, Name: 'Sử dụng' }, { ID: false, Name: 'Không sử dụng' }];
                    $scope.model = data.data;
                    $scope.model.GroupID = $scope.model.UserGroupID;
                    $scope.ListTestGroupId = data.TestGroupId;
                    $scope.ListCityCode = data.CityCodes;
                    $scope.ListMaDuAn = data.MaDuAns;
                    hideLoading();
                    $scope.$apply();

                }
            }
        });
    });


    function GetDanhMuc() {
        $scope.ListTestGroup = [];
        $scope.ListUserGroup = [];
        $scope.ListCity = [];
        $scope.ListDuAn = [];
        $.ajax({
            type: 'post',
            url: '/User/GetDanhMuc',
            data: {},
            cache: false,
            async: false,
            success: function (data) {
                if (data.DataRoles != null && data.DataRoles.length > 0) {
                    $scope.ListUserGroup = data.DataRoles;
                }
                if (data.DataTestGroup != null && data.DataTestGroup.length > 0) {
                    $scope.ListTestGroup = data.DataTestGroup;
                }
                if (data.Citys != null && data.Citys.length > 0) {
                    $scope.ListCity = data.Citys;
                }
                if (data.DuAns != null && data.DuAns.length > 0) {
                    $scope.ListDuAn = data.DuAns;
                }
                $scope.$apply();
            }
        });
    }
    $scope.submit = function () {
        if ($scope.ListMaDuAn == null || $scope.ListMaDuAn.length == 0) {
            toastr.error("Bạn chưa chọn Dự án quản lý");
            return false;
        } else {
            $scope.model.MaDuAn = '';
            for (var i = 0; i < $scope.ListMaDuAn.length; i++) {
                if ($scope.model.MaDuAn == null || $scope.model.MaDuAn == '')
                    $scope.model.MaDuAn = $scope.ListMaDuAn[i];
                else
                    $scope.model.MaDuAn +=',' +$scope.ListMaDuAn[i];
            }
        }

        $("#formSubmit").validate({
            rules: {
                UserName: {
                    required: true,
                    maxlength: 50
                },
                Status: {
                    required: true
                },
                GroupID: {
                    required: true
                },
                Name: {
                    required: true,
                    maxlength: 250
                }
                //,
                //MaDuAn: {
                //    required: true
                //}
            },
            messages: {
                UserName: {
                    required: "Vui lòng nhập tên đăng nhập",
                    maxlength: "Tên đăng nhập không được vượt quá 50 ký tự"
                },
                Status: {
                    required: "Vui lòng chọn trạng thái"
                },
                Email: {
                    email: "Vui lòng nhập đúng định dạng email"
                }, GroupID: {
                    required: "Vui lòng chọn quyền"
                }, Name: {
                    required: "Vui lòng nhập họ và tên",
                    maxlength: "Họ và tên không được vượt quá 250 ký tự"
                }
                //,
                //MaDuAn: {
                //    required: "Vui lòng chọn Dự án quản lý"
                //}
            }
        });
        if ($("#formSubmit").valid()) {
            $.ajax({
                type: 'post',
                url: '/User/Edit',
                data: { user: $scope.model, fileName: $scope.FileName, testGroupMa: $scope.ListTestGroupId, cityCodes: $scope.ListCityCode  },
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

    $scope.SelectFile = function (e) {
        $scope.model.Avartar = "";
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
                    $scope.model.Avartar = base64;
                    $('#pathPhoto').attr('src', e1.target.result);
                };
                reader.readAsDataURL(e.target.files[0]);
            }
        }
    };

});

app.controller('view', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {
    $scope.ListUserGroup = [];
    $scope.ListTestGroup = [];
    $scope.ListTestGroupId = [];
    $scope.ListCity = [];
    $scope.ListCityCode = [];
    $scope.FileName = "";
    $scope.model = {};
    angular.element(document).ready(function () {
        showToast();
        GetDanhMuc();
        $.ajax({
            type: 'post',
            url: '/User/GetItemByID',
            cache: false,
            async: false,
            data: { Id: itemId },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.ListStatus = [{ ID: true, Name: 'Sử dụng' }, { ID: false, Name: 'Không sử dụng' }];
                    $scope.model = data.data;
                    $scope.model.GroupID = $scope.model.UserGroupID;
                    $scope.ListTestGroupId = data.TestGroupId;
                    $scope.ListCityCode = data.CityCodes;
                    hideLoading();
                    $scope.$apply();

                }
            }
        });
    });


    function GetDanhMuc() {
        $scope.ListTestGroup = [];
        $scope.ListUserGroup = [];
        $.ajax({
            type: 'post',
            url: '/User/GetDanhMuc',
            data: {},
            cache: false,
            async: false,
            success: function (data) {
                if (data.DataRoles != null && data.DataRoles.length > 0) {
                    $scope.ListUserGroup = data.DataRoles;
                }
                if (data.DataTestGroup != null && data.DataTestGroup.length > 0) {
                    $scope.ListTestGroup = data.DataTestGroup;
                }
                if (data.Citys != null && data.Citys.length > 0) {
                    $scope.ListCity = data.Citys;
                }
                $scope.$apply();
            }
        });
    }

    $scope.cancel = function () {
        $uibModalInstance.close();
    };

});

app.controller('resetPassword', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {
   
    $scope.Password = "";

    $scope.submit = function () {
        $("#formSubmit").validate({
            rules: {
                Password: {
                    required: true
                }
            },
            messages: {
                Password: {
                    required: "Vui lòng nhập mật khẩu mới"
                }
            }
        });
        if ($("#formSubmit").valid()) {
            $.ajax({
                type: 'post',
                url: '/User/ResetPassword',
                data: { userId: itemId, password: $scope.Password },
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