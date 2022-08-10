app.controller("CauHinhCapBacController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $rootScope) {
    $scope.LoaiQuanSos = [
        { Id: 'CK', Name: 'Cuối kỳ' },
        { Id: 'HUU', Name: 'Nghỉ hưu' },
        { Id: 'HV', Name: 'Học viên' },
        { Id: 'DH', Name: 'Đi học' },
        { Id: 'TB', Name: 'Tân binh' },
        { Id: 'TM', Name: 'Tuyển mới' },
        { Id: 'CLL', Name: 'Chuyển lực lượng' },
        { Id: 'TG', Name: 'QS tăng giảm' },
        { Id: 'TBN', Name: 'Tiêu binh nghi lễ' },
        { Id: 'PH', Name: 'Phong hàm' },
    ];

    $scope.LucLuongs = [];
    $scope.NhomCapBacs = [];
    $scope.LoaiHams = [];
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 15;

    // Hiển thị dữ liệu
    angular.element(document).ready(function () {
        GetButtonAction();
        GetDanhMuc();
    });

    //Phân quyền các nút chắc năng
    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;
    function GetButtonAction() {

        $.ajax({
            type: 'post',
            url: '/CauHinhCapBac/GetButtonAction',
            data: {},
            success: function (response) {
                if (response.buttons != null) {
                    angular.forEach(response.buttons, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'btnSearch') {
                            $scope.RoleBtnSearch = true;
                        }
                        if (item == 'btnDelete') {
                            $scope.RoleBtnDelete = true;
                        }
                    });
                }
                $scope.$apply();
            }
        });
    }


    var treeLucLuong;
    var treeLucLuongAdd;
    var treeNhomCB;

    var treeLucLuongSource = [];
    var treeNhomCBSource = [];

    function GetDanhMuc() {
        showToast();
        $scope.ListPageMenu = [];
        $scope.ListUnit = [];
        $scope.ListAppCode = [];
        $.ajax({
            type: 'post',
            headers: {
                'Access-Control-Allow-Origin': '*',
                'Content-Type': 'application/json'
            },
            url: '/CauHinhCapBac/GetDanhMuc',
            data: {},
            success: function (data) {
                hideLoading();

                $scope.NhomCapBacs = data.NhomCapBacs;
                $scope.LoaiHams = data.LoaiHams;
                $scope.LucLuongs = data.LucLuongs;

                // Lực lượng
                treeLucLuongSource = [];
                if (data.LucLuongs != null && data.LucLuongs.length > 0) {
                    var lucLuong0s = data.LucLuongs.filter(function (x) {
                        return (x.LL_CHA_ID == 0);
                    });
                    if (lucLuong0s != null && lucLuong0s.length > 0) {
                        for (var i = 0; i < lucLuong0s.length; i++) {

                            var comboTree = { id: lucLuong0s[i].ID, title: lucLuong0s[i].TEN_LL /*'Lực lượng'*/ };
                            // kiểm tra có con không
                            var childs = data.LucLuongs.filter(function (x) {
                                return (x.LL_CHA_ID == lucLuong0s[i].ID);
                            });

                            if (childs != null && childs.length > 0) {
                                ConvertTreeLucLuong(comboTree, data.LucLuongs, lucLuong0s[i].ID);
                            }
                            treeLucLuongSource.push(comboTree);

                        }
                    }
                }

                treeLucLuong = $('#justAnotherInputBox').comboTree({
                    source: treeLucLuongSource,
                    isMultiple: false
                });

                treeLucLuongAdd = $('#justAnotherLLAdd').comboTree({
                    source: treeLucLuongSource,
                    isMultiple: false
                });

                // Nhóm CB
                treeNhomCBSource = [];
                if (data.NhomCapBacs != null && data.NhomCapBacs.length > 0) {
                    var lucLuong0s = data.NhomCapBacs.filter(function (x) {
                        return (x.NHOM_CB_CHA_ID == 0 || x.NHOM_CB_CHA_ID == null);
                    });
                    if (lucLuong0s != null && lucLuong0s.length > 0) {
                        for (var i = 0; i < lucLuong0s.length; i++) {

                            var comboTree = { id: lucLuong0s[i].ID, title: lucLuong0s[i].TEN_NHOM_CB /*'Lực lượng'*/ };
                            // kiểm tra có con không
                            var childs = data.LucLuongs.filter(function (x) {
                                return (x.NHOM_CB_CHA_ID == lucLuong0s[i].ID);
                            });

                            if (childs != null && childs.length > 0) {
                                ConvertNhomCB(comboTree, data.NhomCapBacs, lucLuong0s[i].ID);
                            }
                            treeNhomCBSource.push(comboTree);

                        }
                    }
                }

                treeNhomCB = $('#justAnotherNhomCB').comboTree({
                    source: treeNhomCBSource,
                    isMultiple: false
                });
            }
        });
    }

    function ConvertTreeLucLuong(lstTreeModel, lucLuongs, Id) {
        var lstPageMenu = lucLuongs.filter(function (x) {
            return (x.LL_CHA_ID == Id);
        });

        if (lstPageMenu != null && lstPageMenu.length > 0) {
            lstTreeModel.subs = [];
            for (var i = 0; i < lstPageMenu.length; i++) {
                var tree =
                {
                    id: lstPageMenu[i].ID,
                    title: lstPageMenu[i].TEN_LL
                };
                // Kiểm tra xem có con không
                var lucLuongChilds = lucLuongs.filter(function (x) {
                    return (x.LL_CHA_ID == lstPageMenu[i].ID);
                });
                if (lucLuongChilds != null && lucLuongChilds.length > 0) {
                    ConvertTreeLucLuong(tree, lucLuongs, lstPageMenu[i].ID);
                }
                lstTreeModel.subs.push(tree);
            }

        }
    }

    function ConvertNhomCB(lstTreeModel, lucLuongs, Id) {
        var lstPageMenu = lucLuongs.filter(function (x) {
            return (x.NHOM_CB_CHA_ID == Id);
        });

        if (lstPageMenu != null && lstPageMenu.length > 0) {
            lstTreeModel.subs = [];
            for (var i = 0; i < lstPageMenu.length; i++) {
                var tree =
                {
                    id: lstPageMenu[i].ID,
                    title: lstPageMenu[i].TEN_NHOM_CB
                };
                // Kiểm tra xem có con không
                var lucLuongChilds = lucLuongs.filter(function (x) {
                    return (x.NHOM_CB_CHA_ID == lstPageMenu[i].ID);
                });
                if (lucLuongChilds != null && lucLuongChilds.length > 0) {
                    ConvertNhomCB(tree, lucLuongs, lstPageMenu[i].ID);
                }
                lstTreeModel.subs.push(tree);
            }

        }
    }

    $scope.ChangeLucLuong = function () {
        //$scope.LucLuongName = treeLucLuong.getSelectedNames();
        var id = treeLucLuong.getSelectedIds();
        if (id > 0) {
            $scope.modelSearch.LucLuongId = id;
        } else {
            $scope.modelSearch.LucLuongId = 0;
        }
    };

    $scope.ChangeNhomCB = function () {
        //$scope.LucLuongName = treeNhomCB.getSelectedNames();
        var id = treeNhomCB.getSelectedIds();
        if (id > 0) {
            $scope.modelSearch.NhomCBId = id;
        } else {
            $scope.modelSearch.NhomCBId = 0;
        }
    };

    

    $scope.tableEdit = function (index) {
        if ($scope.RoleBtnSave)
            $scope.GetListCapBac[index].Edit = true;

    };

    

    $scope.SearchCapBac = function () {
        $scope.modelSearch.currentPage = 1;
        $scope.LoadPage();
    };

    $scope.totalItems =0;
    $scope.LoadPage = function () {
        if ($scope.modelSearch.LoaiQuanSo == null || $scope.modelSearch.LoaiQuanSo == '') {
            toastr.error("Vui lòng chọn loại quân số.");
        } else {
            $scope.totalItems = 0;
            $scope.ListData =[];
            $.ajax({
                type: 'post',
                async: true,
                cache: false,
                url: '/CauHinhCapBac/GetListCHCapBac',
                data: $scope.modelSearch,
                success: function (data) {
                    $scope.modelSearch.totalItems = data.totalItems;
                    $scope.modelSearch.pageSize = data.pageSize;
                    $scope.ListData = data.data;
                    $scope.$apply();
                }
            });
        }
        
    };
    $scope.pageChanged = function () {
        $scope.LoadPage();
    };
    $scope.Add_LucLuongId = 0;
    $scope.Add_NhomCBId = 0;
    $scope.Add_LoaiHamId = 0;
    $scope.Add_CheckAll = false;
    $scope.addData = function () {
        if ($scope.modelSearch.LoaiQuanSo == null || $scope.modelSearch.LoaiQuanSo == '') {
            toastr.error("Vui lòng chọn loại quân số.");
        } else {
            $scope.Add_LucLuongId = 0;
            $scope.Add_NhomCBId = 0;
            $scope.Add_LoaiHamId = 0;
            $scope.Add_CheckAll = false;
            GetDataAdd(0,0,0);
            $('#AddModal').modal('show');
        }
    };

    $scope.DataAdd = [];
    // Lấy danh sách sữ liệu để thêm mới
    function GetDataAdd(lucLuongId, nhomCBId, loaiHamId) {
        showToast();
        var modelSearch = {
            LucLuongId: lucLuongId,
            NhomCBId: nhomCBId,
            LoaiHamId: loaiHamId,
        };

        $scope.DataAdd = [];
        $.ajax({
            type: 'post',
            async: true,
            cache: false,
            url: '/CauHinhCapBac/GetListCHCapBacAdd',
            data: modelSearch,
            success: function (data) {
                hideLoading();
                $scope.DataAdd = data.data;
                $scope.$apply();
            }, error: (e) => {
                toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
            },
            complete: () => {
                hideLoading();
            }
        });
    }

    $scope.SearchDataAdd = function () {
        GetDataAdd($scope.Add_LucLuongId, $scope.Add_NhomCBId, $scope.Add_LoaiHamId);
    };


    $scope.Add_ChangeLucLuong = function () {
        //$scope.LucLuongName = treeLucLuong.getSelectedNames();
        var id = treeLucLuongAdd.getSelectedIds();
        if (id > 0) {
            $scope.Add_LucLuongId = id;
        } else {
            $scope.Add_LucLuongId = 0;
        }
    };

    $scope.Add_ChangeNhomCB = function () {
        //$scope.LucLuongName = treeNhomCB.getSelectedNames();
        var id = treeNhomCB.getSelectedIds();
        if (id > 0) {
            $scope.Add_NhomCBId = id;
        } else {
            $scope.Add_NhomCBId = 0;
        }
    };

    $scope.Add_ChangeCheckAll = function () {
        $scope.DataAdd.forEach(x => x.selected = $scope.Add_CheckAll);
    };

    $scope.Add_ChangeCheck = function () {
        $scope.Add_CheckAll = false;
        var dataSelected = $scope.DataAdd.filter(x => x.selected == true);
        if (dataSelected != null && dataSelected.length == $scope.DataAdd.length)
            $scope.Add_CheckAll = true;
    }

    

    $scope.Luu = function () {

        // Lấy các dòng được chọn
        var dataSelected = $scope.DataAdd.filter(x => x.selected == true);
        if (dataSelected != null && dataSelected.length > 0) {
            showToast();
            $('#AddModal').modal('hide');
            $.ajax({
                type: 'post',
                async: true,
                cache: false,
                url: '/CauHinhCapBac/Add',
                data: {
                    models: dataSelected,
                    loaiQS: $scope.modelSearch.LoaiQuanSo
                },
                success: function (res) {
                    if (res.Error) {
                        toastr.error(res.Title);
                    } else {
                        toastr.success(res.Title);
                        //$scope.modelSearch.currentPage = 1;
                        $scope.LoadPage();
                    }
                }, error: (e) => {
                    toastr.error('Có lỗi xảy ra trong quá trình lưu!');
                },
                complete: () => {
                    hideLoading();
                }
            });
        }
        else {
            toastr.error('Vui lòng chọn các bản ghi cần thêm mới!');
        }
    }

    $scope.ChangeCheckAll = function () {
        $scope.ListData.forEach(x => x.selected = $scope.CheckAll);
    };

    $scope.ChangeCheck = function () {
        $scope.CheckAll = false;
        var dataSelected = $scope.ListData.filter(x => x.selected == true);
        if (dataSelected != null && dataSelected.length == $scope.ListData.length)
            $scope.CheckAll = true;
    }


    $scope.deleteData = function () {
        // Lấy các dòng được chọn
        var dataSelected = $scope.ListData.filter(x => x.selected == true);
        if (dataSelected != null && dataSelected.length > 0) {
            showToast();
            $.ajax({
                type: 'post',
                url: '/CauHinhCapBac/delete',
                data: {
                    models: dataSelected,
                    loaiQS: $scope.modelSearch.LoaiQuanSo
                },
                success: function (res) {
                    if (res.Error) {
                        toastr.error(res.notifyTitle);
                    } else {
                        toastr.success(res.notifyTitle);
                        //$scope.modelSearch.currentPage = 1;
                        $scope.LoadPage();
                    }
                }, error: (e) => {
                    toastr.error('Có lỗi xảy ra trong quá trình xóa!');
                },
                complete: () => {
                    hideLoading();
                }
            });
        }
        else {
            toastr.error('Vui lòng chọn các bản ghi cần xóa!');
        }
    }


});
