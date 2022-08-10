app.controller("ChotQuanSoDauKyController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location, $sce) {
    //#region giá trị mặc định
    $scope.IsShowRight = false;
    $scope.HocVien = false;
    $scope.ListNam = [];
    var currentYear = new Date().getFullYear();
    for (var i = currentYear; i > currentYear - 7; i--) {
        $scope.ListNam.push({ Id: i, Name: i });
    }
    $scope.NAM = currentYear.toString();
    $scope.KY = 'XH';
    $scope.KiemTraTrongKy = false;
    $scope.cnvca = [];

    $scope.KyHienTai = function () {
        const d = new Date();
        let month = d.getMonth();
        if (month > 6)
            $scope.KY = 'TD';
        else
            $scope.KY = 'XH';
    }
    $scope.KyHienTai();

    $scope.GetDanhMuc = function () {
        $.ajax({
            type: 'get',
            async: false,
            url: '/ChotQuanSoDauKy/GetDanhMuc',
            success: function (res) {
                console.log(res);
                if (res.phamVi !== null)
                    $scope.RoleQuanSo = res.phamVi.DUOC_SUA === 'Y' ? false : true;
                else
                    $scope.RoleQuanSo = true;
                //#region Lực lượng
                $scope.LucLuongTreeInit = [];
                $scope.LucLuongTreeInit.data = res.lucLuong;
                $scope.LucLuongCallback = function (data) {
                    $scope.LucLuongComboTree = data;
                };
                //#endregion
                //#region Lực lượng
                $scope.NhomCBTreeInit = [];
                $scope.NhomCBTreeInit.data = res.nhomCapBac;
                console.log(res.nhomCapBac);

                $scope.NhomCBCallback = function (data) {
                    console.log(data);

                    $scope.NhomCBComboTree = data;
                };
                //#endregion
                $scope.ListCapBacs = res.capBac;
                $scope.ListLoaiHams = res.loaiHam;
                //$scope.$apply();
            }
        })
    }
    $scope.GetDanhMuc();

    $scope.KiemTraNutDauKy = function (key, nam, donVi) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/ChotQuanSoDauKy/KiemTraTrongKy',
            data: { ky: $scope.KY, nam: nam, donVi: donVi },
            success: function (res) {
                $scope.KiemTraTrongKy = res;
                $scope.$apply();
            }
        })
    }

    $scope.ChangeLucLuong = function (data) {
        if ($scope.LucLuongComboTree !== undefined) {
            $scope.LucLuongId = $scope.LucLuongComboTree._selectedItem.id;
            $scope.HocVienModel.LUC_LUONG_ID = $scope.LucLuongComboTree._selectedItem.id;
        }
    }
    $scope.ChangeNhomCapBac = function (data) {
        if ($scope.NhomCBComboTree !== undefined) {
            $scope.NhomCBId = $scope.NhomCBComboTree._selectedItem.id;
        }
    }
    //#endregion
    //#region mở popup
    $scope.TypeModal = "DK";
    $scope.AddFileChotQuanSoDauKy = function () {
        $scope.TypeModal = "DK";
        $ngConfirm({
            title: 'Xác nhận',
            content: 'Bạn có chắc chắn muốn tải quân số đầu kỳ?',
            scope: $scope,
            buttons: {
                OK: {
                    text: 'Đồng ý',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        $('#check-pass').modal('show');
                    }
                },
                Hủy: function (scope, button) {
                },
            }
        });

    };
    $scope.AddFileChotQuanSoTrongKy = function () {
        $scope.TypeModal = "TK";
        $ngConfirm({
            title: 'Xác nhận',
            content: 'Bạn có chắc chắn muốn tải quân số trong kỳ?',
            scope: $scope,
            buttons: {
                OK: {
                    text: 'Đồng ý',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        $scope.TypeModal = "TK_OK";
                        $('#check-pass').modal('show');
                    }
                },
                //Rollback: {
                //    text: 'Phục hồi dữ liệu của lần đẩy trước',
                //    btnClass: 'btn-blue',
                //    action: function (scope, button) {
                //        $scope.TypeModal = "TK_RB";
                //        $('#check-pass').modal('show');
                //    }
                //},
                Hủy: function (scope, button) {
                },
            }
        });

    };
    $scope.DSLGN_IS_DAU_KY = "Y";
    $scope.DanhSachLoi = function () {
        $scope.DSLGN_IS_DAU_KY = "Y";
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/ChotQuanSoDauKy/_DanhSachFileLoiGanNhat',
            controller: 'danhSachFileLoiGanNhat',
            size: 'xl',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        donVi: $scope.DON_VI_ID,
                        nam: $scope.NAM,
                        ky: $scope.KY
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            //$scope.LoadPage();
        });
    };
    $scope.DanhSachChiTietQuanSo = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/ChotQuanSoDauKy/_DanhSachChiTietQuanSo',
            controller: 'danhSachChiTietQuanSo',
            size: 'xl',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        donVi: $scope.DON_VI_ID,
                        nam: $scope.NAM,
                        ky: $scope.KY
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            //$scope.LoadPage();
        });
    };
    $scope.CheckPass = function () {
        $.ajax({
            type: 'post',
            async: false,
            url: '/ChotQuanSoDauKy/CheckPass',
            data: { pass: $scope.Password },
            success: function (res) {
                if (res.Check === 1) {
                    $('#check-pass').modal('hide');
                    if ($scope.TypeModal === 'DK') {
                        var modalInstance = $uibModal.open({
                            animation: $scope.animationsEnabled,
                            modalTemplate: '<div class="" ng-transclude></div>',
                            templateUrl: '/ChotQuanSoDauKy/_AddFileChotQuanSoDauKy',
                            controller: 'addFileChotQuanSoDauKy',
                            size: 'xl',
                            backdrop: 'show',
                            resolve: {
                                data: function () {
                                    return {
                                        donViId: $scope.DON_VI_ID,
                                        ky: $scope.KY,
                                        nam: $scope.NAM
                                    };
                                }
                            }
                        });

                        //kết quả trả về của modal
                        modalInstance.result.then(function () {
                            $scope.LoadDataCBChoHuu($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                            $scope.LoadDataCBBietPhaiDiHoc($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                            $scope.LoadDataCBTuyenMoi($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                            $scope.LoadDataCBCSDauKy($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                            $scope.LoadThangHam($scope.DON_VI_ID, $scope.NAM);
                        });
                    } else if ($scope.TypeModal === 'TK_OK') {
                        $.ajax({
                            type: 'post',
                            async: false,
                            url: '/ChotQuanSoDauKy/KiemTraDuLieuDauKy',
                            data: { donVi: $scope.DON_VI_ID, nam: $scope.NAM, ky: $scope.KY },
                            success: function (response) {
                                if (response.Error) {
                                    toastr.error(response.Title);
                                } else {
                                    if (response.ObjectData) {
                                        $ngConfirm({
                                            title: 'Xác nhận',
                                            content: 'Chưa có dữ liệu trong kỳ, hệ thống sẽ lấy dữ liệu hiện tại của kỳ trước làm dữ liệu đầu kỳ, bạn có chắc chắn không?',
                                            scope: $scope,
                                            buttons: {
                                                OK: {
                                                    text: 'Đồng ý',
                                                    btnClass: 'btn-blue',
                                                    action: function (scope, button) {
                                                        $.ajax({
                                                            type: 'post',
                                                            async: false,
                                                            url: '/ChotQuanSoDauKy/SaoChepDuLieuDauKy',
                                                            data: { donVi: $scope.DON_VI_ID, nam: $scope.NAM, ky: $scope.KY },
                                                            success: function (response) {
                                                                if (response.Error) {
                                                                    toastr.error(response.Title);
                                                                } else {
                                                                    toastr.success('Sao chép dữ liệu thành công.');
                                                                    var modalInstance1 = $uibModal.open({
                                                                        animation: $scope.animationsEnabled,
                                                                        templateUrl: '/ChotQuanSoDauKy/_AddFileChotQuanSoTrongKy',
                                                                        controller: 'addFileChotQuanSoTrongKy',
                                                                        size: 'xl',
                                                                        backdrop: 'static',
                                                                        resolve: {
                                                                            data: function () {
                                                                                return {
                                                                                    donViId: $scope.DON_VI_ID,
                                                                                    ky: $scope.KY,
                                                                                    nam: $scope.NAM
                                                                                };
                                                                            }
                                                                        }
                                                                    });

                                                                    //kết quả trả về của modal
                                                                    modalInstance1.result.then(function (response) {
                                                                        $scope.LoadDataCBChoHuu($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                                                        $scope.LoadDataCBBietPhaiDiHoc($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                                                        $scope.LoadDataCBTuyenMoi($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                                                        $scope.LoadDataCBCSDauKy($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                                                        $scope.LoadThangHam($scope.DON_VI_ID, $scope.NAM);
                                                                    });
                                                                }
                                                            }
                                                        });
                                                    }
                                                },
                                                Hủy: function (scope, button) {
                                                },
                                            }
                                        });
                                    } else {
                                        var modalInstance1 = $uibModal.open({
                                            animation: $scope.animationsEnabled,
                                            templateUrl: '/ChotQuanSoDauKy/_AddFileChotQuanSoTrongKy',
                                            controller: 'addFileChotQuanSoTrongKy',
                                            size: 'xl',
                                            backdrop: 'static',
                                            resolve: {
                                                data: function () {
                                                    return {
                                                        donViId: $scope.DON_VI_ID,
                                                        ky: $scope.KY,
                                                        nam: $scope.NAM
                                                    };
                                                }
                                            }
                                        });

                                        //kết quả trả về của modal
                                        modalInstance1.result.then(function (response) {
                                            $scope.LoadDataCBChoHuu($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                            $scope.LoadDataCBBietPhaiDiHoc($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                            $scope.LoadDataCBTuyenMoi($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                            $scope.LoadDataCBCSDauKy($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                            $scope.LoadThangHam($scope.DON_VI_ID, $scope.NAM);
                                        });
                                    }
                                }
                            }
                        });

                    } else if ($scope.TypeModal === 'TK_DB') {
                        $.ajax({
                            type: 'post',
                            async: false,
                            url: '/ChotQuanSoDauKy/RollbackTrongKy',
                            data: { donViId: $scope.DON_VI_ID, loai: 'Y', nam: $scope.NAM },
                            success: function (response) {
                                if (response.Error) {
                                    toastr.error(response.Title);
                                } else {
                                    $scope.LoadDataCBChoHuu($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                    $scope.LoadDataCBBietPhaiDiHoc($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                    //$scope.LoadDataCSNghiaVu($scope.DON_VI_ID);
                                    $scope.LoadDataCBTuyenMoi($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                    $scope.LoadDataCBCSDauKy($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                    $scope.LoadThangHam($scope.DON_VI_ID, $scope.NAM);

                                    $scope.GetKhoaHoc();
                                    toastr.success('Xóa toàn bộ quân số thành công.');
                                }
                            }
                        });
                    } else if ($scope.TypeModal === 'X') {
                        $ngConfirm({
                            title: 'Xác nhận',
                            content: 'Bạn có chắc chăn xóa tất cả quân số?',
                            scope: $scope,
                            buttons: {
                                OKDK: {
                                    text: 'Đầu kỳ',
                                    btnClass: 'btn-blue',
                                    action: function (scope, button) {
                                        $.ajax({
                                            type: 'post',
                                            async: false,
                                            url: '/ChotQuanSoDauKy/XoaToanBoQuanSo',
                                            data: { donViId: $scope.DON_VI_ID, isDauKy: 'Y', ky: $scope.KY, nam: $scope.NAM },
                                            success: function (response) {
                                                if (response.Error) {
                                                    toastr.error(response.Title);
                                                } else {
                                                    $scope.LoadDataCBChoHuu($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                                    $scope.LoadDataCBBietPhaiDiHoc($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                                    //$scope.LoadDataCSNghiaVu($scope.DON_VI_ID);
                                                    $scope.LoadDataCBTuyenMoi($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                                    $scope.LoadDataCBCSDauKy($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                                    $scope.LoadThangHam($scope.DON_VI_ID, $scope.NAM);

                                                    $scope.GetKhoaHoc();
                                                    toastr.success('Xóa toàn bộ quân số thành công.');
                                                }
                                            }
                                        });
                                    }
                                },
                                OKTK: {
                                    text: 'Trong kỳ',
                                    btnClass: 'btn-blue',
                                    action: function (scope, button) {
                                        $.ajax({
                                            type: 'post',
                                            async: false,
                                            url: '/ChotQuanSoDauKy/XoaToanBoQuanSo',
                                            data: { donViId: $scope.DON_VI_ID, isDauKy: 'N', ky: $scope.KY, nam: $scope.NAM },
                                            success: function (response) {
                                                if (response.Error) {
                                                    toastr.error(response.Title);
                                                } else {
                                                    toastr.success('Xóa toàn bộ quân số thành công.');
                                                    $scope.LoadDataCBChoHuu($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                                    $scope.LoadDataCBBietPhaiDiHoc($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                                    $scope.LoadDataCBTuyenMoi($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                                    $scope.LoadDataCBCSDauKy($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                                                    $scope.LoadThangHam($scope.DON_VI_ID, $scope.NAM);

                                                    $scope.GetKhoaHoc();
                                                    toastr.success('Xóa toàn bộ quân số thành công.');
                                                }
                                            }
                                        });
                                    }
                                },
                                Hủy: function (scope, button) {
                                },
                            }
                        });
                    }
                    $scope.Password = '';
                } else {
                    toastr.error("Mật khẩu không chính xác");
                }
            }
        });
    }
    //#endregion
    //#region tạo treeview
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
            onClick: onClickTCCBCS
        }
    };
    function onClickTCCBCS(event, treeId, treeNode, clickFlag) {
        if (treeNode !== undefined && treeNode.id !== undefined && !treeNode.isParent) {
            showToast();
            $scope.TEN_DON_VI = treeNode.name;
            $scope.DON_VI_ID = treeNode.id.split('_')[1];
            $scope.IsShowRight = true;
            if (treeNode.mua === 'KHOI_TRUONG' || (treeNode.getParentNode() !== null && treeNode.getParentNode().mua === 'KHOI_TRUONG'))
                $scope.HocVien = true;
            else
                $scope.HocVien = false;
            setTimeout(function () {
                $scope.LoadDataCBCSDauKy($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                $scope.LoadDataCBChoHuu($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                $scope.LoadDataCBBietPhaiDiHoc($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                $scope.LoadDataCBTuyenMoi($scope.DON_VI_ID, $scope.NAM, $scope.KY);
                $scope.LoadDataCBChuyenLL();
                $scope.LoadThangHam($scope.DON_VI_ID, $scope.NAM);
                $scope.KiemTraNutDauKy($scope.KY, $scope.NAM, $scope.DON_VI_ID);
                $scope.LoadDataCBTangGiam();
                $scope.GetKhoaHoc();
            }, 100);
        }

    }
    $scope.ChangeNam = function () {
        showToast();
        $scope.LoadDataCBChoHuu($scope.DON_VI_ID, $scope.NAM, $scope.KY);
        $scope.LoadDataCBBietPhaiDiHoc($scope.DON_VI_ID, $scope.NAM, $scope.KY);
        //$scope.LoadDataCBDiHoc($scope.DON_VI_ID);
        //$scope.LoadDataCSNghiaVu($scope.DON_VI_ID);
        $scope.LoadDataCBTuyenMoi($scope.DON_VI_ID);
        $scope.LoadDataCBCSDauKy($scope.DON_VI_ID, $scope.NAM, $scope.KY);
        $scope.LoadThangHam($scope.DON_VI_ID, $scope.NAM);
        $scope.GetKhoaHoc();
        hideLoading();
    }
    $scope.ChangeKy = function () {
        showToast();
        $scope.LoadDataCBChoHuu($scope.DON_VI_ID, $scope.NAM, $scope.KY);
        $scope.LoadDataCBBietPhaiDiHoc($scope.DON_VI_ID, $scope.NAM, $scope.KY);
        $scope.LoadDataCBTuyenMoi($scope.DON_VI_ID, $scope.NAM, $scope.KY);
        $scope.LoadDataCBCSDauKy($scope.DON_VI_ID, $scope.NAM, $scope.KY);
        $scope.LoadThangHam($scope.DON_VI_ID, $scope.NAM, $scope.KY);
        //$scope.GetKhoaHoc();
        hideLoading();
    }
    $scope.txtSearchDmDonVi = "";
    $scope.treeData = {};
    $scope.treeData.data = {};
    $scope.ListDonVi;
    $.ajax({
        type: 'get',
        async: false,
        url: '/ChotQuanSoDauKy/GetTreeData',
        data: {
            keyword: $scope.txtSearchDmDonVi,
            isOpen: false
        },
        success: function (res) {
            if (!res.Error) {
                $scope.ListDonVi = res;
                $.fn.zTree.init($("#treeRole"), setting, $scope.ListDonVi.data);
                var zTree = $.fn.zTree.getZTreeObj("treeRole");
                var type = { "Y": "ps", "N": "ps" };
                zTree.setting.check.chkboxType = type;
            }
        }
    });

    $scope.btnSearchDmDonVi = function () {
        var api = '/ChotQuanSoDauKy/GetTreeData';
        $.ajax({
            type: 'get',
            async: false,
            url: api,
            data: {
                keyword: $scope.txtSearchDmDonVi,
                isOpen: true
            },
            success: function (res) {
                if (!res.Error) {
                    $scope.ListDonVi = res;
                    $.fn.zTree.init($("#treeRole"), setting, $scope.ListDonVi.data);
                    var zTree = $.fn.zTree.getZTreeObj("treeRole");
                    var type = { "Y": "ps", "N": "ps" };
                    zTree.setting.check.chkboxType = type;
                }
            }
        });
    }

    //#endregion tạo treeview
    //#region validate dữ liệu từ bảng temp
    $scope.ValidateSoHieu = function (data, key) {
        var lstError = [];
        if (data.SO_HIEU.indexOf('-') === -1 || data.SO_HIEU.length !== 7) {
            lstError.push({ col: 1, row: key, id: data.ID });
        } else {
            if (data.SO_HIEU.split('-')[0].length !== 3 || data.SO_HIEU.split('-')[1].length !== 3) {
                lstError.push({ col: 1, row: key, id: data.ID });
            }
        }
        return lstError;
    }
    //#endregion
    //#region Tìm kiếm đơn vị tree
    $scope.SearchDonVi = function () {
        console.log($scope.TenDonVi);
    }
    //#endregion
    $scope.CNVCA = function () {

        $('#addCNVCA').modal('show');

    }
    $scope.AddCNVCA = function () {
        //Tạm thời fix giá trị:
        //Cap_bac_id = 36;
        //luc_luong_id = 270;
        //lucLuongId: $scope.LucLuongId,
        //nhomCBId: $scope.NhomCBId,
        //capBacId: $scope.capBac        

        $.ajax({
            type: 'post',
            url: '/ChotQuanSoDauKy/AddCNVCA',
            data: {
                donViId: $scope.DON_VI_ID, nam: $scope.NAM, ky: $scope.KY,
                sl_Nam: $scope.cnvca.SL_NAM_DK,
                sl_Nu: $scope.cnvca.SL_NU_DK,
                //lucLuongId: $scope.LucLuongId,
                //capBacId: $scope.capBac
            },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    console.log(res);
                    toastr.success(res.Title);
                }
                hideLoading();
            }
        })
    }
    $scope.GetCNVCA = function () {
        $.ajax({
            type: 'get',
            url: '/ChotQuanSoDauKy/GetCNVCA',
            data: {
                donViId: $scope.DON_VI_ID, nam: $scope.NAM, ky: $scope.KY
            },
            success: function (res) {
                $scope.cnvca.SL_NAM = res.data?.SL_NAM;
                $scope.cnvca.SL_NU = res.data?.SL_NU;

            }
        })
    }


    //#region Cán bộ chiến sỹ đầu kỳ full
    // Có hiển thị cột checkbox ở đầu không
    $scope.tree_Table_Checkbox = false;
    // Model data
    $scope.cbcs_tree_data = [];
    $scope.LoadDataCBCSDauKy = function (donVi, nam, ky) {
        $scope.GetCNVCA();

        $.ajax({
            type: 'post',
            url: '/ChotQuanSoDauKy/DanhSachCBCSDauKy',
            data: { donViId: donVi, nam: nam, ky: ky },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.cbcs_tree_data = res.data;
                    $scope.cbcs_tree_data = $scope.TinhTongLastRowCBCS($scope.cbcs_tree_data);
                    $scope.cbcs_tree_data_before = res.data;
                    $scope.$apply();
                }
                hideLoading();
            }
        })
    }
    // cấu hình hiển thị cùng với icon
    $scope.expanding_property = {
        field: "lucLuong",
        displayName: "Cấp Bậc",
        width: "20%",
        rowspan: 3,
        colspan: 2
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.col_defs = [[
        //{ field: "capBac", displayName: "Cấp bậc", width: "15%", rowspan: 3, colspan: 1 },
        { field: "loaiHam", displayName: "Loại hàm", width: "11%", rowspan: 3, colspan: 1, filterable: true },
        { field: "quanSoKyTruoc", displayName: "Quân số kỳ trước", width: "18%", rowspan: 1, colspan: 8 },
        { field: "bienDong", displayName: "Biến động", width: "6%", rowspan: 2, colspan: 2 },
        { field: "quanSoDauKy", displayName: "Quân số đầu kỳ", width: "18%", rowspan: 1, colspan: 8 },
        { field: "bienDongTrongKy", displayName: $sce.trustAsHtml("Biến động <br> trong kỳ"), width: "6%", rowspan: 2, colspan: 2, isHtml: true },
        { field: "quanSoHienTai", displayName: "Quân số hiện tại", width: "21%", rowspan: 1, colspan: 9 }
    ], [
        { field: "ktCongTac", displayName: "Công tác", width: "25%", rowspan: 1, colspan: 2 },
        { field: "ktNghiaVu", displayName: "Nghĩa Vụ", width: "25%", rowspan: 1, colspan: 2 },
        { field: "ktBietPhai", displayName: "Đi học/B.phái", width: "25%", rowspan: 1, colspan: 2 },
        { field: "ktChoHuu", displayName: "Chờ hưu", width: "25%", rowspan: 1, colspan: 2 },
        { field: "tkCongTac", displayName: "Công tác", width: "25%", rowspan: 1, colspan: 2 },
        { field: "tkNghiaVu", displayName: "Nghĩa vụ", width: "25%", rowspan: 1, colspan: 2 },
        { field: "tkBietPhai", displayName: "Đi học/B.phái", width: "25%", rowspan: 1, colspan: 2 },
        { field: "tkChoHuu", displayName: "Chờ hưu", width: "25%", rowspan: 1, colspan: 2 },
        { field: "htCongTac", displayName: "Công tác", width: "25%", rowspan: 1, colspan: 2 },
        { field: "htNghiaVu", displayName: "Nghĩa vụ", width: "25%", rowspan: 1, colspan: 2 },
        { field: "htBietPhai", displayName: "Đi học/B.phái", width: "25%", rowspan: 1, colspan: 2 },
        { field: "htChoHuu", displayName: "Chờ hưu", width: "25%", rowspan: 1, colspan: 2 },
        { field: "tong", displayName: "Tổng cộng", width: "25%", rowspan: 2, colspan: 1 },
    ], [

        { field: "ktCongTacNam", displayName: "Nam", width: "3%", rowspan: 1, colspan: 1 },
        { field: "ktCongTacNu", displayName: "Nữ", width: "3%", rowspan: 1, colspan: 1 },
        { field: "ktNghiaVuNam", displayName: "Nam", width: "3%", rowspan: 1, colspan: 1 },
        { field: "ktNghiaVuNu", displayName: "Nữ", width: "3%", rowspan: 1, colspan: 1 },
        { field: "ktBietPhaiNam", displayName: "Nam", width: "3%", rowspan: 1, colspan: 1 },
        { field: "ktBietPhaiNu", displayName: "Nữ", width: "3%", rowspan: 1, colspan: 1 },
        { field: "ktChoHuuNam", displayName: "Nam", width: "3%", rowspan: 1, colspan: 1 },
        { field: "ktChoHuuNu", displayName: "Nữ", width: "3%", rowspan: 1, colspan: 1 },
        { field: "ktBienDongNam", displayName: "Nam", width: "3%", rowspan: 1, colspan: 1 },
        { field: "ktBienDongNu", displayName: "Nữ", width: "3%", rowspan: 1, colspan: 1 },
        { field: "tkCongTacNam", displayName: "Nam", width: "3%", rowspan: 1, colspan: 1 },
        { field: "tkCongTacNu", displayName: "Nữ", width: "3%", rowspan: 1, colspan: 1 },
        { field: "tkNghiaVuNam", displayName: "Nam", width: "3%", rowspan: 1, colspan: 1 },
        { field: "tkNghiaVuNu", displayName: "Nữ", width: "3%", rowspan: 1, colspan: 1 },
        { field: "tkBietPhaiNam", displayName: "Nam", width: "3%", rowspan: 1, colspan: 1 },
        { field: "tkBietPhaiNu", displayName: "Nữ", width: "3%", rowspan: 1, colspan: 1 },
        { field: "tkChoHuuNam", displayName: "Nam", width: "3%", rowspan: 1, colspan: 1 },
        { field: "tkChoHuuNu", displayName: "Nữ", width: "3%", rowspan: 1, colspan: 1 },
        { field: "bienDongNam", displayName: "Nam", width: "3%", rowspan: 1, colspan: 1 },
        { field: "bienDongNu", displayName: "Nữ", width: "3%", rowspan: 1, colspan: 1 },
        { field: "htCongTacNam", displayName: "Nam", width: "3%", rowspan: 1, colspan: 1 },
        { field: "htCongTacNu", displayName: "Nữ", width: "3%", rowspan: 1, colspan: 1 },
        { field: "htNghiaVuNam", displayName: "Nam", width: "3%", rowspan: 1, colspan: 1 },
        { field: "htNghiaVuNu", displayName: "Nữ", width: "3%", rowspan: 1, colspan: 1 },
        { field: "htBietPhaiNam", displayName: "Nam", width: "3%", rowspan: 1, colspan: 1 },
        { field: "htBietPhaiNu", displayName: "Nữ", width: "3%", rowspan: 1, colspan: 1 },
        { field: "htChoHuuNam", displayName: "Nam", width: "3%", rowspan: 1, colspan: 1 },
        { field: "htChoHuuNu", displayName: "Nữ", width: "3%", rowspan: 1, colspan: 1 },
    ]];

    $scope.body_defs = [{ field: "capBac", rowspan: 1, colspan: 2 }, { field: "loaiHam" }, { field: "ktCongTacNam" }, { field: "ktCongTacNu" }, { field: "ktNghiaVuNam" }, { field: "ktNghiaVuNu" }
        , { field: "ktBietPhaiNam" }, { field: "ktBietPhaiNu" }, { field: "ktChoHuuNam" }, { field: "ktChoHuuNu" }, { field: "ktBienDongNam" }, { field: "ktBienDongNu" }
        , { field: "tkCongTacNam" }, { field: "tkCongTacNu" }, { field: "tkNghiaVuNam" }, { field: "tkNghiaVuNu" }, { field: "tkBietPhaiNam" }, { field: "tkBietPhaiNu" }, { field: "tkChoHuuNam" }, { field: "tkChoHuuNu" }
        , { field: "bienDongNam" }, { field: "bienDongNu" }, { field: "htCongTacNam" }, { field: "htCongTacNu" }, { field: "htNghiaVuNam" }, { field: "htNghiaVuNu" }
        , { field: "htBietPhaiNam" }, { field: "htBietPhaiNu" }, { field: "htChoHuuNam" }, { field: "htChoHuuNu" }, { field: "tong" }];
    // event when change value checkbox
    $scope.ChangeTreeTableCheckbox = function (item) {
        console.log(item);
    };

    // event when click choose row
    $scope.ClickTreeTableRow = function (item) {
        console.log(item);
    };
    //#endregion 
    //#region Nghỉ chờ hưu
    $scope.ch_tree_Table_Checkbox = false;
    // Model data
    $scope.ch_tree_data = [];
    $scope.ch_tree_data_before = [];
    $scope.LoadDataCBChoHuu = function (donVi, nam, ky) {
        console.log(donVi);
        $.ajax({
            type: 'post',
            url: '/ChotQuanSoDauKy/DanhSachTp_CbChoHuu',
            data: { donViId: donVi, nam: nam, ky: ky },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.ch_tree_data = res.data;
                    $scope.ch_tree_data_before = res.data;

                }
            }
        })
    }
    $scope.TinhTongLastRowCBCS = function (cbcs_tree_data) {
        var ColumnTotals = {
            lucLuong: 'Tổng cộng'
        };
        var columns = ['ktNghiaVuNam', 'ktNghiaVuNu', 'ktCongTacNam', 'ktCongTacNu', 'ktBietPhaiNam', 'ktBietPhaiNu', 'ktChoHuuNam', 'ktChoHuuNu', 'ktBienDongNam', 'ktBienDongNu', 'tkNghiaVuNam', 'tkNghiaVuNu', 'tkCongTacNam', 'tkCongTacNu', 'tkBietPhaiNam', 'tkBietPhaiNu', 'tkChoHuuNam', 'tkChoHuuNu', 'htNghiaVuNam', 'htNghiaVuNu', 'htCongTacNam', 'htCongTacNu', 'htBietPhaiNam', 'htBietPhaiNu', 'htChoHuuNam', 'htChoHuuNu', 'bienDongNam', 'bienDongNu', 'tong'];
        cbcs_tree_data.forEach(x => {

            var rowTotal = 0;
            columns.forEach(i => {

                if (x[i] === undefined) {
                    return;
                }

                ColumnTotals[i] = ColumnTotals[i] || 0;
                ColumnTotals[i] += x[i];
                rowTotal += x[i];
            })
            x.RowTotal = rowTotal;

        });

        columns.forEach(i => {
            ColumnTotals.RowTotal = ColumnTotals.RowTotal || 0;
            ColumnTotals.RowTotal += ColumnTotals[i];
        });

        cbcs_tree_data = cbcs_tree_data.filter(x => x.lucLuong !== ColumnTotals.lucLuong);
        cbcs_tree_data.push(ColumnTotals);
        //console.log('cbcs_tree_data: ', cbcs_tree_data);
        //console.table(cbcs_tree_data);
        return cbcs_tree_data;
    }
    $scope.TinhLaiTongCong = function (tree_data) {
        //Tính lại Tổng Cộng
        tree_data.forEach(x => {

            var sumNam = x.children.reduce((sumNam, elem) => {
                return sumNam + elem.SL_NAM;
            }, 0);
            x.SL_NAM = sumNam;
            var sumNu = x.children.reduce((sumNu, elem) => {
                return sumNu + elem.SL_NU;
            }, 0);
            x.SL_NU = sumNu;

            x.TONG_CONG = sumNam + sumNu;
        });
        return tree_data;
    }
    $scope.SearchData = function (tabSelected) {

        if (tabSelected == 'cbcs') {

            if ($scope.lucLuong != null && $scope.lucLuong != '') {
                $scope.cbcs_tree_data = $scope.cbcs_tree_data_before.filter(
                    x => x.lucLuong?.toLowerCase().includes($scope.lucLuong.toLowerCase())
                );
            } else {
                $scope.cbcs_tree_data = $scope.cbcs_tree_data_before;
            }

            $scope.cbcs_tree_data_before_tmp = $scope.cbcs_tree_data;
            if ($scope.loaiHam != null && $scope.loaiHam != '') {
                var tenLoaiHam = $scope.ListLoaiHams.find(x => x.Id === $scope.loaiHam);

                $scope.cbcs_tree_data = $scope.cbcs_tree_data_before_tmp
                    .filter(x =>
                        x.children.some(y => y.loaiHam.toLowerCase() == tenLoaiHam?.Name.toLowerCase()))
                    .map(
                        x => {
                            return Object.assign({}, x, { children: x.children.filter(children => children.loaiHam.toLowerCase() == tenLoaiHam?.Name.toLowerCase()) });
                        });

            } else {
                $scope.cbcs_tree_data = $scope.cbcs_tree_data_before_tmp;
            }

            $scope.cbcs_tree_data_before_tmp = $scope.cbcs_tree_data;
            if ($scope.capBac != null && $scope.capBac != '') {
                var tenCapBac = $scope.ListCapBacs.find(x => x.Id === $scope.capBac);

                $scope.cbcs_tree_data = $scope.cbcs_tree_data_before_tmp
                    .filter(x =>
                        x.children.some(y => y.capBac.toLowerCase() == tenCapBac ?.Name.toLowerCase()))
                    .map(
                        x => {
                            return Object.assign({}, x, { children: x.children.filter(children => children.capBac.toLowerCase() == tenCapBac ?.Name.toLowerCase()) });
                        });
            } else {
                $scope.cbcs_tree_data = $scope.cbcs_tree_data_before_tmp;
            }

            //Tính lại Tổng Cộng
            //$scope.cbcs_tree_data = $scope.TinhLaiTongCong($scope.cbcs_tree_data);     
            $scope.cbcs_tree_data = $scope.TinhTongLastRowCBCS($scope.cbcs_tree_data);

        }

        if (tabSelected == 'ch') {

            if ($scope.lucLuongCH != null && $scope.lucLuongCH != '') {
                $scope.ch_tree_data = $scope.ch_tree_data_before.filter(
                    x => x.TEN_LL?.toLowerCase().includes($scope.lucLuongCH.toLowerCase())
                );
            } else {
                $scope.ch_tree_data = $scope.ch_tree_data_before;
            }

            $scope.ch_tree_data_before_tmp = $scope.ch_tree_data;
            if ($scope.loaiHamCH != null && $scope.loaiHamCH != '') {
                var tenLoaiHam = $scope.ListLoaiHams.find(x => x.Id === $scope.loaiHamCH);

                //$scope.ch_tree_data = $scope.ch_tree_data_before_tmp.filter(x =>
                //    (tenLoaiHam ?.Name !== null && tenLoaiHam ?.Name !== undefined && x.children.some(y => y.LOAI_HAM.toLowerCase().includes(tenLoaiHam ?.Name.toLowerCase())))
                //);
                $scope.ch_tree_data = $scope.ch_tree_data_before_tmp
                    .filter(x =>
                        x.children.some(y => y.LOAI_HAM.toLowerCase() == tenLoaiHam?.Name.toLowerCase()))
                    .map(
                        x => {
                            return Object.assign({}, x, { children: x.children.filter(children => children.LOAI_HAM.toLowerCase() == tenLoaiHam?.Name.toLowerCase()) });
                        });

            } else {
                $scope.ch_tree_data = $scope.ch_tree_data_before_tmp;
            }

            $scope.ch_tree_data_before_tmp = $scope.ch_tree_data;
            if ($scope.capBacCH != null && $scope.capBacCH != '') {
                var tenCapBac = $scope.ListCapBacs.find(x => x.Id === $scope.capBacCH);

                //$scope.ch_tree_data = $scope.ch_tree_data_before_tmp.filter(x =>
                //    (tenCapBac?.Name !== null && tenCapBac?.Name !== undefined && x.children.find(y => y.TEN_CB.includes(tenCapBac?.Name)))
                //);
                $scope.ch_tree_data = $scope.ch_tree_data_before_tmp
                    .filter(x =>
                        x.children.some(y => y.capBac.toLowerCase() == tenCapBac ?.Name.toLowerCase()))
                    .map(
                        x => {
                            return Object.assign({}, x, { children: x.children.filter(children => children.capBac.toLowerCase() == tenCapBac ?.Name.toLowerCase()) });
                        });

            } else {
                $scope.ch_tree_data = $scope.ch_tree_data_before_tmp;
            }

            //Tính lại Tổng Cộng
            $scope.ch_tree_data = $scope.TinhLaiTongCong($scope.ch_tree_data);

        }

        if (tabSelected == 'dh') {

            if ($scope.lucLuongDH != null && $scope.lucLuongDH != '') {
                $scope.dh_tree_data = $scope.dh_tree_data_before.filter(
                    x => x.TEN_LL?.toLowerCase().includes($scope.lucLuongDH.toLowerCase())
                );
            } else {
                $scope.dh_tree_data = $scope.dh_tree_data_before;
            }

            $scope.dh_tree_data_before_tmp = $scope.dh_tree_data;
            if ($scope.loaiHamDH != null && $scope.loaiHamDH != '') {
                var tenLoaiHam = $scope.ListLoaiHams.find(x => x.Id === $scope.loaiHamDH);

                //$scope.dh_tree_data = $scope.dh_tree_data_before_tmp.filter(x =>
                //    (tenLoaiHam ?.Name !== null && tenLoaiHam ?.Name !== undefined && x.children.find(y => y.LOAI_HAM.includes(tenLoaiHam ?.Name)))
                //);
                $scope.dh_tree_data = $scope.dh_tree_data_before_tmp
                    .filter(x =>
                        x.children.some(y => y.LOAI_HAM.toLowerCase() == tenLoaiHam?.Name.toLowerCase()))
                    .map(
                        x => {
                            return Object.assign({}, x, { children: x.children.filter(children => children.LOAI_HAM.toLowerCase() == tenLoaiHam?.Name.toLowerCase()) });
                        });

            } else {
                $scope.dh_tree_data = $scope.dh_tree_data_before_tmp;
            }

            $scope.dh_tree_data_before_tmp = $scope.dh_tree_data;
            if ($scope.capBacDH != null && $scope.capBacDH != '') {
                var tenCapBac = $scope.ListCapBacs.find(x => x.Id === $scope.capBacDH);

                //$scope.dh_tree_data = $scope.dh_tree_data_before_tmp.filter(x =>
                //    (tenCapBac?.Name !== null && tenCapBac?.Name !== undefined && x.children.find(y => y.TEN_CB.includes(tenCapBac?.Name)))
                //);
                $scope.dh_tree_data = $scope.dh_tree_data_before_tmp
                    .filter(x =>
                        x.children.some(y => y.capBac.toLowerCase() == tenCapBac ?.Name.toLowerCase()))
                    .map(
                        x => {
                            return Object.assign({}, x, { children: x.children.filter(children => children.capBac.toLowerCase() == tenCapBac ?.Name.toLowerCase()) });
                        });
            } else {
                $scope.dh_tree_data = $scope.dh_tree_data_before_tmp;
            }

            //Tính lại Tổng Cộng
            $scope.dh_tree_data = $scope.TinhLaiTongCong($scope.dh_tree_data);

        }

        if (tabSelected == 'tm') {

            if ($scope.lucLuongTM != null && $scope.lucLuongTM != '') {
                $scope.tm_tree_data = $scope.tm_tree_data_before.filter(
                    x => x.TEN_LL?.toLowerCase().includes($scope.lucLuongTM.toLowerCase())
                );
            } else {
                $scope.tm_tree_data = $scope.tm_tree_data_before;
            }

            $scope.tm_tree_data_before_tmp = $scope.tm_tree_data;
            if ($scope.loaiHamTM != null && $scope.loaiHamTM != '') {
                var tenLoaiHam = $scope.ListLoaiHams.find(x => x.Id === $scope.loaiHamTM);

                //$scope.tm_tree_data = $scope.tm_tree_data_before_tmp.filter(x =>
                //    (tenLoaiHam ?.Name !== null && tenLoaiHam ?.Name !== undefined && x.children.find(y => y.LOAI_HAM.includes(tenLoaiHam ?.Name)))
                //);
                $scope.tm_tree_data = $scope.tm_tree_data_before_tmp
                    .filter(x =>
                        x.children.some(y => y.LOAI_HAM.toLowerCase() == tenLoaiHam?.Name.toLowerCase()))
                    .map(
                        x => {
                            return Object.assign({}, x, { children: x.children.filter(children => children.LOAI_HAM.toLowerCase() == tenLoaiHam?.Name.toLowerCase()) });
                        });


            } else {
                $scope.tm_tree_data = $scope.tm_tree_data_before_tmp;
            }

            $scope.tm_tree_data_before_tmp = $scope.tm_tree_data;
            if ($scope.capBacTM != null && $scope.capBacTM != '') {
                var tenCapBac = $scope.ListCapBacs.find(x => x.Id === $scope.capBacTM);

                //$scope.tm_tree_data = $scope.tm_tree_data_before_tmp.filter(x =>
                //    (tenCapBac?.Name !== null && tenCapBac?.Name !== undefined && x.children.find(y => y.TEN_CB.includes(tenCapBac?.Name)))
                //);
                $scope.tm_tree_data = $scope.tm_tree_data_before_tmp
                    .filter(x =>
                        x.children.some(y => y.capBac.toLowerCase() == tenCapBac ?.Name.toLowerCase()))
                    .map(
                        x => {
                            return Object.assign({}, x, { children: x.children.filter(children => children.capBac.toLowerCase() == tenCapBac ?.Name.toLowerCase()) });
                        });
            } else {
                $scope.tm_tree_data = $scope.tm_tree_data_before_tmp;
            }

            //Tính lại Tổng Cộng
            $scope.tm_tree_data = $scope.TinhLaiTongCong($scope.tm_tree_data);

        }


    }
    //$scope.ch_tree_data = [];
    // cấu hình hiển thị cùng với icon
    $scope.ch_expanding_property = {
        field: "TEN_LL",
        displayName: "Cấp bậc",
        width: "30%",
        rowspan: 3,
        colspan: 2
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.ch_col_defs = [[
        //{ field: "TEN_CB", displayName: "Cấp bậc", width: "15%", rowspan: 1, colspan: 1 },
        { field: "LOAI_HAM", displayName: "Loại hàm", width: "20%", rowspan: 1, colspan: 1 },
        { field: "LOAI_NGHI_CHO_HUU_TEXT", displayName: "Loại nghỉ chờ hưu", width: "20%", rowspan: 1, colspan: 1 },
        { field: "SL_NAM", displayName: "Nam", width: "10%", rowspan: 1, colspan: 1 },
        { field: "SL_NU", displayName: "Nữ", width: "10%", rowspan: 1, colspan: 1 },
        { field: "TONG_CONG", displayName: "Tổng cộng", width: "10%", rowspan: 1, colspan: 1 },
    ]];

    $scope.ch_body_defs = [{ field: "TEN_CB", rowspan: 1, colspan: 2 }, { field: "LOAI_HAM" }, { field: "LOAI_NGHI_CHO_HUU_TEXT" }, { field: "SL_NAM" }, { field: "SL_NU" }, { field: "TONG_CONG" }];
    // event when change value checkbox
    $scope.ch_ChangeTreeTableCheckbox = function (item) {
        console.log(item);
    };

    // event when click choose row
    $scope.ch_ClickTreeTableRow = function (item) {
        console.log(item);
    };
    //#endregion
    //#region Đi học biệt phái
    $scope.dh_tree_Table_Checkbox = false;
    // Model data
    $scope.dh_tree_data = [];
    $scope.dh_tree_data_before = [];
    $scope.LoadDataCBBietPhaiDiHoc = function (donVi, nam, ky) {
        $.ajax({
            type: 'post',
            url: '/ChotQuanSoDauKy/DanhSachTp_BietPhai_DiHoc',
            data: { donViId: donVi, nam: nam, ky: ky },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.dh_tree_data = res.data;
                    $scope.dh_tree_data_before = res.data;
                }
            }
        })
    }

    //$scope.ch_tree_data = [];
    // cấu hình hiển thị cùng với icon
    $scope.dh_expanding_property = {
        field: "TEN_LL",
        displayName: "Cấp bậc",
        width: "30%",
        rowspan: 3,
        colspan: 2
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.dh_col_defs = [[
        //{ field: "TEN_CB", displayName: "Cấp bậc", width: "15%", rowspan: 1, colspan: 1 },
        { field: "LOAI_HAM", displayName: "Loại hàm", width: "20%", rowspan: 1, colspan: 1 },
        { field: "LOAI_NGHI_CHO_HUU", displayName: "Thông tin bổ sung", width: "20%", rowspan: 1, colspan: 1 },
        { field: "SL_NAM", displayName: "Nam", width: "10%", rowspan: 1, colspan: 1 },
        { field: "SL_NU", displayName: "Nữ", width: "10%", rowspan: 1, colspan: 1 },
        { field: "TONG_CONG", displayName: "Tổng cộng", width: "10%", rowspan: 1, colspan: 1 },
    ]];

    $scope.dh_body_defs = [{ field: "TEN_CB", rowspan: 1, colspan: 2 }, { field: "LOAI_HAM" }, { field: "LOAI_NGHI_CHO_HUU" }, { field: "SL_NAM" }, { field: "SL_NU" }, { field: "TONG_CONG" }];
    //#endregion
    //#region Tuyển mới
    $scope.tm_tree_Table_Checkbox = false;
    // Model data
    $scope.tm_tree_data = [];
    $scope.tm_tree_data_before = [];
    $scope.LoadDataCBTuyenMoi = function (donVi, nam, ky) {
        $.ajax({
            type: 'post',
            url: '/ChotQuanSoDauKy/DanhSachTp_TuyenMoi',
            data: { donViId: donVi, nam: nam, ky: ky },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.tm_tree_data = res.data;
                    $scope.tm_tree_data_before = res.data;
                }
            }
        })
    }

    //$scope.ch_tree_data = [];
    // cấu hình hiển thị cùng với icon
    $scope.tm_expanding_property = {
        field: "TEN_LL",
        displayName: "Cấp bậc",
        width: "30%",
        rowspan: 3,
        colspan: 2
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.tm_col_defs = [[
        //{ field: "TEN_CB", displayName: "Cấp bậc", width: "15%", rowspan: 1, colspan: 1 },
        { field: "LOAI_HAM", displayName: "Loại hàm", width: "20%", rowspan: 1, colspan: 1 },
        { field: "LOAI_NGHI_CHO_HUU", displayName: "Thông tin bổ sung", width: "20%", rowspan: 1, colspan: 1 },
        { field: "SL_NAM", displayName: "Nam", width: "10%", rowspan: 1, colspan: 1 },
        { field: "SL_NU", displayName: "Nữ", width: "10%", rowspan: 1, colspan: 1 },
        { field: "TONG_CONG", displayName: "Tổng cộng", width: "10%", rowspan: 1, colspan: 1 },
    ]];

    $scope.tm_body_defs = [{ field: "TEN_CB", rowspan: 1, colspan: 2 }, { field: "LOAI_HAM" }, { field: "LOAI_NGHI_CHO_HUU" }, { field: "SL_NAM" }, { field: "SL_NU" }, { field: "TONG_CONG" }];
    //#endregion
    //#region Học viên
    $scope.hv_tree_Table_Checkbox = true;
    // Model data
    $scope.HocVienModel = {};
    $scope.HocVienModel.NAM_HOC = 0;
    $scope.hv_tree_data = [];
    $scope.LoadDataHocVien = function (donVi) {
        $.ajax({
            type: 'post',
            url: '/ChotQuanSoDauKy/DanhSachTp_HocVien',
            data: { donViId: donVi, khoaHoc: $scope.khoaHocId, namHoc: $scope.HocVienModel.NAM_HOC },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.tm_tree_data = res.data;
                }
            }
        })
    }

    //$scope.ch_tree_data = [];
    // cấu hình hiển thị cùng với icon
    $scope.hv_expanding_property = {
        field: "TEN_LL",
        displayName: "Cấp bậc",
        width: "40%",
        rowspan: 3,
        colspan: 2
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.hv_col_defs = [[
        //{ field: "TEN_CB", displayName: "Cấp bậc", width: "15%", rowspan: 1, colspan: 1 },
        { field: "SL_NAM", displayName: "Nam", width: "20%", rowspan: 1, colspan: 1 },
        { field: "SL_NU", displayName: "Nữ", width: "20%", rowspan: 1, colspan: 1 },
        { field: "SL_TONG", displayName: "Tổng cộng", width: "20%", rowspan: 1, colspan: 1 },
    ]];

    $scope.hv_body_defs = [{ field: "TEN_CB" }, { field: "SL_NAM" }, { field: "SL_NU" }, { field: "SL_TONG" }];
    //#endregion
    //#region Khóa học
    $scope.GetKhoaHoc = function () {
        $.ajax({
            type: 'get',
            url: '/ChotQuanSoDauKy/GetKhoaHoc',
            success: function (res) {
                $scope.ListKhoaHoc = res;
                $scope.$apply();
                hideLoading();
            }
        })
    }
    $scope.khoaHoc = {};
    $scope.AddKhoaHoc = function () {
        $('#add-khoa-hoc').modal('show');
    }
    $scope.LuuKhoaHoc = function () {
        $("#form-khoa-hoc").validate({
            rules: {
                NAM_HOC: {
                    required: true
                },
                TEN_KHOA_HOC: {
                    required: true,
                },

            },
            messages: {
                NAM_HOC: {
                    required: "Vui lòng chọn năm học",
                },
                TEN_KHOA_HOC: {
                    required: "Vui lòng nhập Tên khóa học",
                }
            }
        });
        if ($("#form-khoa-hoc").valid()) {
            $.ajax({
                type: 'post',
                url: '/ChotQuanSoDauKy/AddKhoaHoc',
                data: $scope.khoaHoc,
                success: function (res) {
                    if (res.Error) {
                        toastr.error(res.Title);
                    } else {
                        toastr.success(res.Title);
                        $('#add-khoa-hoc').modal('hide');
                        $scope.GetKhoaHoc();
                    }
                }
            })
        }
    }
    $scope.DeleteKhoaHoc = function () {

    }
    $scope.SelectedKhoaHoc = function (item) {
        $scope.khoaHocId = item.ID;
    }
    $scope.SelectedNamHoc = function (item) {
        $scope.HocVienModel.NAM_HOC = item;
    }
    $scope.AddHocVien = function () {
        $('#add-hoc-vien').modal('show');
        //$scope.GetDanhMuc();
    }
    $scope.LuuHocVien = function () {
        console.log($scope.HocVienModel);
    }
    //#endregion
    //#region Chuyển lực lượng
    // cấu hình hiển thị cùng với icon
    $scope.cll_expanding_property = {
        field: "lucLuongMoi",
        displayName: "Thông tin chuyển đến",
        width: "40%",
        rowspan: 1,
        colspan: 3
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.cll_col_defs = [
        [
            { field: "chuyenDi", displayName: "Thông tin chuyển đi", width: "40%", rowspan: 1, colspan: 2 },
            { field: "soLuongNam", displayName: "Nam", width: "6%", rowspan: 2, colspan: 1 },
            { field: "soLuongNu", displayName: "Nữ", width: "6%", rowspan: 2, colspan: 1 },
            { field: "soLuongTong", displayName: "Tổng cộng", width: "8%", rowspan: 2, colspan: 1 }
        ], [
            { field: "lucLuongMoi", displayName: "Lực lượng", width: "20%", rowspan: 1, colspan: 1 },
            { field: "capBac", displayName: "Cấp bậc", width: "10%", rowspan: 1, colspan: 1 },
            { field: "loaiHam", displayName: "Loại hàm", width: "10%", rowspan: 1, colspan: 1 },
            { field: "lucLuongCu", displayName: "Lực lượng", width: "20%", rowspan: 1, colspan: 1 },
            { field: "noiChuyenDen", displayName: "Nơi chuyển đến", width: "10%", rowspan: 1, colspan: 1 },
        ]];

    $scope.cll_body_defs = [{ field: "capBac" }, { field: "loaiHam" }, { field: "lucLuongCu" }, { field: "noiChuyenDen" }, { field: "soLuongNam" }, { field: "soLuongNu" }, { field: "soLuongTong" }];


    $scope.cll_tree_Table_Checkbox = false;
    // Model data
    $scope.cll_tree_data = [];
    $scope.DauKy_TrongKy = 'DK';
    $scope.LoadDataCBChuyenLL = function () {

        $.ajax({
            type: 'post',
            url: '/ChotQuanSoDauKy/GetDanhSachChuyenLucLuong',
            data: {
                donViId: $scope.DON_VI_ID,
                nam: $scope.NAM,
                IsDauKy: ($scope.DauKy_TrongKy == 'DK' ? 'Y' : 'N'),
                lucLuongId: $scope.LucLuongId,
                nhomCBId: $scope.NhomCBId,
                capBacId: $scope.capBac
            },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.cll_tree_data = res.data;
                    $scope.$apply();
                }
            }
        })
    }
    $scope.LoadDataCBChuyenLL();

    //$scope.ch_tree_data = [];

    //#endregion
    //#region tăng giảm
    $scope.tg_tree_Table_Checkbox = false;
    // Model data
    $scope.tg_tree_data = [];
    $scope.DauKy_TrongKy = 'DK';
    $scope.LoadDataCBTangGiam = function () {
        $.ajax({
            type: 'post',
            url: '/ChotQuanSoDauKy/GetDanhSachTangGiam',
            data: {
                donViId: $scope.DON_VI_ID,
                nam: $scope.NAM,
                IsDauKy: ($scope.DauKy_TrongKy == 'DK' ? 'Y' : 'N'),
                lucLuongId: $scope.LucLuongId,
                nhomCBId: $scope.NhomCBId,
                capBacId: $scope.capBac
            },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.tg_tree_data = res.data;
                    $scope.$apply();
                }
            }
        })
    }

    //$scope.ch_tree_data = [];
    // cấu hình hiển thị cùng với icon
    $scope.tg_expanding_property = {
        field: "TEN_LL",
        displayName: "Cấp bậc",
        width: "25%",
        rowspan: 3,
        colspan: 1
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.tg_col_defs = [
        [
            { field: "LOAI_HAM", displayName: "Loại hàm", width: "19%", rowspan: 3, colspan: 1 },
            { field: "CHUYEN_DEN", displayName: "Quân số chuyển đến", width: "12%", rowspan: 2, colspan: 2 },
            { field: "CHUYEN_DI", displayName: "Quân số chuyển đi", width: "48%", rowspan: 1, colspan: 8 },
            { field: "TONG_CONG", displayName: "Tổng cộng", width: "6%", rowspan: 3, colspan: 1 }
        ], [
            { field: "CAT_QS", displayName: "Cắt quân số", width: "12%", rowspan: 1, colspan: 2 },
            { field: "GIAM_TM", displayName: "Giảm t.mới", width: "12%", rowspan: 1, colspan: 2 },
            { field: "GIAM_CH", displayName: "Giảm ch.hưu", width: "12%", rowspan: 1, colspan: 2 },
            { field: "CHUYEN_DI", displayName: "Giảm ch.hưu", width: "12%", rowspan: 1, colspan: 2 },
        ], [
            { field: "NAM1", displayName: "Nam", width: "6%", rowspan: 1, colspan: 1 },
            { field: "NU1", displayName: "Nữ", width: "6%", rowspan: 1, colspan: 1 },
            { field: "NAM2", displayName: "Nam", width: "6%", rowspan: 1, colspan: 1 },
            { field: "NU2", displayName: "Nữ", width: "6%", rowspan: 1, colspan: 1 },
            { field: "NAM3", displayName: "Nam", width: "6%", rowspan: 1, colspan: 1 },
            { field: "NU3", displayName: "Nữ", width: "6%", rowspan: 1, colspan: 1 },
            { field: "NAM4", displayName: "Nam", width: "6%", rowspan: 1, colspan: 1 },
            { field: "NU4", displayName: "Nữ", width: "6%", rowspan: 1, colspan: 1 },
            { field: "NAM5", displayName: "Nam", width: "6%", rowspan: 1, colspan: 1 },
            { field: "NAM5", displayName: "Nữ", width: "6%", rowspan: 1, colspan: 1 },
        ]];

    $scope.tg_body_defs = [{ field: "CAP_BAC", rowspan: 1, colspan: 2 }, { field: "LOAI_HAM" },
    { field: "NAM1" }, { field: "NU1" }, { field: "NAM2" },
    { field: "NU3" }, { field: "NAM3" }, { field: "NU3" },
    { field: "NAM4" }, { field: "NU4" }, { field: "NAM5" }, { field: "NU5" }];
    //#endregion
    //#region Dự kiến thăng hàm
    $scope.ListDuKienThangHam = [];
    $scope.LoadThangHam = function (donViId, nam) {
        $.ajax({
            type: 'post',
            //async: false,
            url: '/ChotQuanSoDauKy/GetThangHam',
            data: { donViId: donViId, nam: nam },
            success: function (res) {
                if (!res.Error) {
                    $scope.ListDuKienThangHam = res.data;
                }
            }
        });
    }
    $scope.ThemMoiThangHam = function () {
        $scope.ListDuKienThangHam.push(
            {
                'ID': 0,
                'CAP_BAC_ID': '',
                'LOAI_HAM': '',
                'DU_KIEN_DE_XUAT': '',
                'THUC_TE_NAM': '',
                'THUC_TE_NU': '',
                'ID_DON_VI': $scope.DON_VI_ID,
                'NAM': $scope.NAM,
            });
    }
    $scope.GetDMThangHam = function () {
        $.ajax({
            type: 'get',
            async: false,
            url: '/ChotQuanSoDauKy/GetDMThangHam',
            success: function (res) {
                if (!res.Error) {
                    $scope.ListCapBac = res.capBacs;
                    $scope.ListLoaiHam = res.loaiHams;
                    //$scope.$apply();
                }
            }
        });
    }
    $scope.GetDMThangHam();
    $scope.CheckAllThangHam = function () {
        angular.forEach($scope.ListDuKienThangHam, function (val, key) {
            if ($scope.itemCheckAllThangham)
                val.selected = false;
            else
                val.selected = true;
        });
    }
    $scope.XoaThangHam = function () {
        $scope.ListDuKienThangHamDelete = $scope.ListDuKienThangHam.filter(x => x.selected);
        $scope.ListDuKienThangHam = $scope.ListDuKienThangHam.filter(x => !x.selected);
        if ($scope.ListDuKienThangHam.length === 0)
            $scope.itemCheckAllThangham = false;
    }
    $scope.SumDataThangHam = function (slNam, slNu, index) {
        var nam = 0, nu = 0;
        if (slNam === null && slNam === undefined)
            nam = 0;
        else
            nam = parseInt(slNam);
        if (slNu === null && slNu === undefined)
            nu = 0;
        else
            nu = parseInt(slNu);
        //$scope.ListDuKienThangHam[index].TONG = nam + nu;
        return nam + nu;
    }

    $scope.checkHasDuplicate = function (arr) {

        var arrUnique = arr.filter((item, pos, self) =>
            self.findIndex(v => v.CAP_BAC_ID === item.CAP_BAC_ID && v.LOAI_HAM === item.LOAI_HAM) === pos
        );
        //console.log(arrUnique);

        if (arrUnique.length < arr.length) {
            return true;
        }
        return false;

    }

    $scope.LuuThangHam = function () {

        if ($scope.checkHasDuplicate($scope.ListDuKienThangHam)) {
            toastr.error("Không thể lưu do Cấp bậc và Loại hàm bị trùng!");
            return;
        }

        $.ajax({
            type: 'post',
            async: false,
            url: '/ChotQuanSoDauKy/SaveThangHam',
            data: { qS_DU_KIEN_THANG_HAM: $scope.ListDuKienThangHam, qsXoa: $scope.ListDuKienThangHamDelete },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    toastr.success(res.Title);
                }
            }
        });
    }
    $scope.CapNhatDLTuDongToDKThangHam = function (dulieuTuDong) {
        if (dulieuTuDong != null && dulieuTuDong.length > 0) {
            angular.forEach(dulieuTuDong, function (value, key) {
                var k = key + 1;
                $scope.itemModel = {};
                $scope.itemModel.CAP_BAC_ID = value.CAP_BAC_ID_MOI ?? '';
                $scope.itemModel.LOAI_HAM = value.LOAI_HAM_MOI_ID ?? '';
                $scope.itemModel.THUC_TE_NAM = value.THUC_TE_NAM ?? 0;
                $scope.itemModel.THUC_TE_NU = value.THUC_TE_NU ?? 0;
                $scope.itemModel.ID_DON_VI = $scope.DON_VI_ID;
                $scope.itemModel.NAM = $scope.NAM;

                //Kiểm tra nếu đã có đã có Cấp bậc và Loại hàm thì cập nhật Số lượng thực tế Nam, Nữ
                var index = -1;
                $scope.ListDuKienThangHam.filter((x, pos) => {
                    if (x.CAP_BAC_ID === value.CAP_BAC_ID_MOI && x.LOAI_HAM === value.LOAI_HAM_MOI_ID) {
                        index = pos;
                    }
                    return true;
                });

                if (index == -1) {
                    $scope.ListDuKienThangHam.push($scope.itemModel);
                } else {
                    $scope.ListDuKienThangHam[index] = $scope.itemModel;
                }
            });
        }
    }

    $scope.TaoDLTuDong = function () {
        $.ajax({
            type: 'post',
            async: false,
            url: '/ChotQuanSoDauKy/TaoDLTuDong',
            data: { donViId: $scope.DON_VI_ID, nam: $scope.NAM },
            success: function (res) {
                console.log(res);
                if (res.data != null & res.data.length > 0) {
                    $scope.CapNhatDLTuDongToDKThangHam(res.data);
                    toastr.success(res.Title);
                } else {
                    toastr.warning('Không có dữ liệu!');
                }
            }
        });
    }
    //#endregion
    $scope.Json2Arrary = function (data) {
        var lstJson = [];
        angular.forEach(data, function (val, key) {
            var itemData = {};
            angular.forEach(val, function (v, k) {
                itemData[k] = v;
            });
            lstJson.push(itemData);
        });
        return lstJson;
    }
    $scope.XoaToanBoQuanSo = function () {
        $scope.TypeModal = 'X'
        $('#check-pass').modal('show');
    }
    $scope.SaveToMainTable = function () {
        $.ajax({
            type: 'post',
            async: false,
            url: '/ChotQuanSoDauKy/SyncToMainTable',
            data: { nam: $scope.NAM, ky: $scope.KY, donViId: $scope.DON_VI_ID },
            success: function (response) {
                if (response.Error) {
                    toastr.error(response.Title);
                } else {
                    toastr.success(response.Title);
                }
            }
        });
    }
    $scope.ExportTemplate = function () {
        window.location.href = '/ChotQuanSoDauKy/ExportTemplate';
    }
});
app.controller('addFileChotQuanSoDauKy', function ($scope, $uibModal, $uibModalInstance, $ngConfirm, showToast, hideLoading, data) {
    $scope.model = {};
    $scope.SelectFile = function (e) {
        $('.custom-file-label').text(e.target.files[0].name);
    }
    $scope.Upload = function () {
        showToast();
        var fileUpload = $("#file-input").get(0);
        var files = fileUpload.files;
        var fileData = new FormData();
        fileData.append(files[0].name, files[0]);
        fileData.append('donViId', data.donViId);
        fileData.append('ky', data.ky);
        fileData.append('nam', data.nam);
        fileData.append('loai', 'dk');
        $.ajax({
            url: '/ChotQuanSoDauKy/ImportFileExel',
            type: "POST",
            dataType: 'json',
            contentType: false, // Not to set any content header
            processData: false, // Not to process data
            data: fileData,
            success: function (result) {
                if (result.Error) {
                    toastr.error(result.Title);
                } else {
                    toastr.success(result.Title);
                    $scope.DanhSachLoi = result.Loi;
                    var modalInstance = $uibModal.open({
                        animation: $scope.animationsEnabled,
                        templateUrl: '/ChotQuanSoDauKy/_ThongBaoLoi',
                        controller: 'thongBaoLoi',
                        size: 'xl',
                        backdrop: 'static',
                        resolve: {
                            data: function () {
                                return {
                                    danhSachLoi: result.Loi
                                };
                            }
                        }
                    });

                    //kết quả trả về của modal
                    modalInstance.result.then(function (response) {
                        //$scope.LoadPage();
                    });
                    $scope.cancel();
                    $uibModalInstance.close();
                }
            },
            error: function (err) {
                toastr.error("Lỗi nhập dữ liệu.")
            }
        });
    }
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller('addFileChotQuanSoTrongKy', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading, data) {
    $scope.model = {};
    $scope.SelectFile = function (e) {
        $('.custom-file-label').text(e.target.files[0].name);
    }
    $scope.Upload = function () {
        showToast();
        var fileUpload = $("#file-input").get(0);
        var files = fileUpload.files;
        var fileData = new FormData();
        fileData.append(files[0].name, files[0]);
        fileData.append('donViId', data.donViId);
        fileData.append('ky', data.ky);
        fileData.append('nam', data.nam);
        fileData.append('loai', 'tk');
        $.ajax({
            url: '/ChotQuanSoDauKy/ImportFileExel',
            type: "POST",
            dataType: 'json',
            contentType: false, // Not to set any content header
            processData: false, // Not to process data
            data: fileData,
            success: function (result) {
                hideLoading();
                if (result.Error) {
                    toastr.error(result.Title);
                } else {
                    toastr.success(result.Title);
                    $scope.cancel();
                    //window.location.href = '/ChotQuanSoDauKy/Index?txnid=' + result.txId
                    $uibModalInstance.close();
                }
            },
            error: function (err) {
                toastr.error("Lỗi nhập dữ liệu.")
            }
        });
    }
    $scope.cancel = function () {
        $uibModalInstance.close();
    };


});
app.controller('danhSachFileLoiGanNhat', function ($scope, $uibModal, $uibModalInstance, $ngConfirm, showToast, hideLoading, data) {
    $scope.model = {};
    $scope.modelSearch = {};
    $scope.modelSearch.totalItemCT = 0;
    $scope.modelSearch.totalItemCH = 0;
    $scope.modelSearch.totalItemBP = 0;
    $scope.modelSearch.totalItemDH = 0;
    $scope.modelSearch.totalItemNV = 0;
    $scope.modelSearch.totalItemTM = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.DSLGN_IS_DAU_KY = "Y";
    $scope.pageChanged = function () {
        $scope.LoadPage();
    }
    showToast();
    $scope.LoadPage = function () {
        $.ajax({
            type: 'post',
            url: '/ChotQuanSoDauKy/GetDanhSachLoiGanNhat',
            data: { donVi: data.donVi, nam: data.nam, pageSize: $scope.modelSearch.pageSize, pageNumber: $scope.modelSearch.currentPage, isDauKy: $scope.DSLGN_IS_DAU_KY },
            success: function (res) {
                $scope.ListCBCongTac = res.error.CongTac;
                $scope.ListCBBietPhai = res.error.BietPhai;
                $scope.ListCBChoHuu = res.error.ChoHuu;
                $scope.ListCBDiHoc = res.error.diHoc;
                $scope.ListCBTuyenMoi = res.error.tuyenMoi;
                $scope.ListCSNgiaVu = res.error.nghiaVu;

                $scope.modelSearch.totalItemCT = res.error.ToTalCT;
                $scope.modelSearch.totalItemCH = res.error.ToTalCH;
                $scope.modelSearch.totalItemBP = res.error.ToTalBP;
                $scope.modelSearch.totalItemDH = res.error.ToTalDH;
                $scope.modelSearch.totalItemNV = res.error.ToTalTM;
                $scope.modelSearch.totalItemTM = res.error.ToTalNV;

                $scope.$apply();
            }
        })
    }
    $scope.LoadPage();

    $scope.ChangeDSLGN_IS_DAU_KY = function () {
        $scope.modelSearch.currentPage = 1;
        $scope.LoadPage();
    }
    $scope.CapNhatCongTac = function (itemData) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/ChotQuanSoDauKy/_CapNhatChiTietLoi',
            controller: 'CapNhatChiTietLoi',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        item: itemData,
                        type: 'CT',
                        ky: data.ky
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    }

    $scope.CapNhatChoHuu = function (itemData) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/ChotQuanSoDauKy/_CapNhatChiTietLoi',
            controller: 'CapNhatChiTietLoi',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        item: itemData,
                        type: 'CH',
                        ky: data.ky
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    }

    $scope.CapNhatBietPhai = function (itemData) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/ChotQuanSoDauKy/_CapNhatChiTietLoi',
            controller: 'CapNhatChiTietLoi',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        item: itemData,
                        type: 'BP',
                        ky: data.ky
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    }

    $scope.CapNhatDiHoc = function (itemData) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/ChotQuanSoDauKy/_CapNhatChiTietLoi',
            controller: 'CapNhatChiTietLoi',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        item: itemData,
                        type: 'DH',
                        ky: data.ky
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    }

    $scope.CapNhatNghiaVu = function (itemData) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/ChotQuanSoDauKy/_CapNhatChiTietLoi',
            controller: 'CapNhatChiTietLoi',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        item: itemData,
                        type: 'NV',
                        ky: data.ky
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    }

    $scope.CapNhatTuyenMoi = function (itemData) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/ChotQuanSoDauKy/_CapNhatChiTietLoi',
            controller: 'CapNhatChiTietLoi',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        item: itemData,
                        type: 'TM',
                        ky: data.ky
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    }

    hideLoading();
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller('danhSachChiTietQuanSo', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading, data) {
    $scope.model = {};
    $scope.checkKy = 'Y';

    $scope.ListCBCongTac = [];
    $scope.ListCBBietPhai = [];
    $scope.ListCBChoHuu = [];
    $scope.ListCBDiHoc = [];
    $scope.ListCBTuyenMoi = [];
    $scope.ListCSNgiaVu = [];

    $scope.model = {};
    $scope.modelSearch = {};
    $scope.modelSearch.totalItemCT = 0;
    $scope.modelSearch.totalItemCH = 0;
    $scope.modelSearch.totalItemBP = 0;
    $scope.modelSearch.totalItemDH = 0;
    $scope.modelSearch.totalItemNV = 0;
    $scope.modelSearch.totalItemTM = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;

    $scope.GetDanhMuc = function () {
        $.ajax({
            type: 'get',
            async: false,
            url: '/ChotQuanSoDauKy/GetDanhMuc',
            success: function (res) {
                console.log(res);
                if (res.phamVi !== null)
                    $scope.RoleQuanSo = res.phamVi.DUOC_SUA === 'Y' ? false : true;
                else
                    $scope.RoleQuanSo = true;
                //#region Lực lượng
                $scope.LucLuongTreeInit = [];
                $scope.LucLuongTreeInit.data = res.lucLuong;
                $scope.LucLuongCallback = function (data) {
                    $scope.LucLuongComboTree = data;
                };
                //#endregion
                //#region Lực lượng
                $scope.NhomCBTreeInit = [];
                $scope.NhomCBTreeInit.data = res.nhomCapBac;
                console.log(res.nhomCapBac);

                $scope.NhomCBCallback = function (data) {
                    $scope.NhomCBComboTree = data;
                };
                //#endregion
                $scope.ListCapBacs = res.capBac;
                $scope.ListLoaiHams = res.loaiHam;
                //$scope.$apply();
            }
        })
    }
    $scope.GetDanhMuc();

    $scope.ChangeLucLuong = function () {
        if ($scope.LucLuongComboTree !== undefined) {
            $scope.LucLuongId = $scope.LucLuongComboTree._selectedItem.id;
        }
        $scope.LoadPage();
    }

    $scope.ChangeSoHieu = function () {
        setTimeout($scope.LoadPage(), 2000);
    }

    showToast();
    $scope.pageChanged = function () {
        $scope.LoadPage();
    }
    $scope.LoadPage = function () {
        $.ajax({
            type: 'post',
            async: false,
            url: '/ChotQuanSoDauKy/GetDanhSachChiTiet',
            data: {
                donVi: data.donVi,
                nam: data.nam,
                pageSize: $scope.modelSearch.pageSize,
                pageNumber: $scope.modelSearch.currentPage,
                ky: data.ky,
                isDauKy: $scope.checkKy,
                soHieu: $scope.soHieu,
                capBac: $scope.capBac,
                lucLuong: $scope.LucLuongId,
                loaiHam: $scope.loaiHam,
                gioiTinh: $scope.gioiTinh
            },
            success: function (res) {
                $scope.ListCBCongTac = res.error.CongTac;
                $scope.ListCBBietPhai = res.error.BietPhai;
                $scope.ListCBChoHuu = res.error.ChoHuu;
                $scope.ListCBDiHoc = res.error.diHoc;
                $scope.ListCBTuyenMoi = res.error.tuyenMoi;
                $scope.ListCSNgiaVu = res.error.nghiaVu;

                $scope.modelSearch.totalItemCT = res.error.ToTalCT;
                $scope.modelSearch.totalItemCH = res.error.ToTalCH;
                $scope.modelSearch.totalItemBP = res.error.ToTalBP;
                $scope.modelSearch.totalItemDH = res.error.ToTalDH;
                $scope.modelSearch.totalItemNV = res.error.ToTalTM;
                $scope.modelSearch.totalItemTM = res.error.ToTalNV;

                //$scope.$apply();
            }
        })
    }
    $scope.tree_data_Tab1 = [];
    $scope.tree_data_Tab2 = [];
    $scope.tree_data_Tab3 = [];
    $scope.LoadDataTree = function (donVi, nam) {
        $.ajax({
            type: 'post',
            url: '/ChotQuanSoDauKy/GetDanhSachChiTiet',
            data: {
                donVi: data.donVi,
                nam: data.nam,
                pageSize: $scope.modelSearch.pageSize,
                pageNumber: $scope.modelSearch.currentPage,
                ky: data.ky,
                isDauKy: $scope.checkKy,
                soHieu: $scope.soHieu,
                capBac: $scope.capBac,
                lucLuong: $scope.LucLuongId,
                loaiHam: $scope.loaiHam,
                gioiTinh: $scope.gioiTinh
            },
            success: function (res) {
                $scope.tree_data_Tab1 = res.choHuu;
                $scope.tree_data_Tab2 = res.diHocBietPhai;
                $scope.tree_data_Tab3 = res.tuyenMoi;
                if (!$scope.$$phase)
                    $scope.$apply();
            }
        })
    }
    $scope.LoadDataTree();

    //#region Chờ Hưu
    // Có hiển thị cột checkbox ở đầu không
    $scope.tree_Table_Checkbox_Tab1 = false;
    // cấu hình hiển thị cùng với icon
    $scope.expanding_property_Tab1 = {
        field: "ngayTao",
        displayName: "Ngày Tạo",
        width: "5%",
        rowspan: 2,
        colspan: 1
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.col_defs_Tab1 = [[
        { field: "soHieu", displayName: "Số hiệu", width: "10%", rowspan: 2, colspan: 1 },
        { field: "gioiTinh", displayName: "Giới tính", width: "7%", rowspan: 2, colspan: 1 },
        { field: "loaiHam", displayName: "Cấp bậc - Loai làm", width: "20%", rowspan: 1, colspan: 2 },
        { field: "chucVu", displayName: "Chức vụ - Đơn vị - Lực lượng", width: "30%", rowspan: 1, colspan: 3 },
        { field: "coGiay", displayName: "Cỡ giày", width: "5%", rowspan: 2, colspan: 1 },
        { field: "coMu", displayName: "Cỡ mũ", width: "5%", rowspan: 2, colspan: 1 },
        { field: "coQA", displayName: "Cõ QA", width: "5%", rowspan: 2, colspan: 1 },
        { field: "namChoHuu", displayName: "Năm chờ hưu", width: "7%", rowspan: 2, colspan: 1 },
        { field: "nhanHienVat", displayName: "Nhận hiện vật", width: "10%", rowspan: 2, colspan: 1 }
    ], [
        { field: "capBac", displayName: "cấp bậc", width: "10%", rowspan: 1, colspan: 1 },
        { field: "loaiHam", displayName: "Loại hàm", width: "11", rowspan: 1, colspan: 1 },
        { field: "chucVu", displayName: "Chức vụ", width: "10%", rowspan: 1, colspan: 1 },
        { field: "viTriCongTac", displayName: "Ví trí công tác", width: "5%", rowspan: 1, colspan: 1 },
        { field: "lucLuong", displayName: "Lực lượng", width: "5%", rowspan: 1, colspan: 1 }
    ]];

    $scope.body_defs_Tab1 = [{ field: "soHieu", rowspan: 1, colspan: 2 }, { field: "gioiTinh" }, { field: "capBac" }, { field: "loaiHam" }
        , { field: "chucVu" }, { field: "viTriCongTac" }, { field: "lucLuong" }, { field: "coGiay" }, { field: "coMu" }
        , { field: "coQA" }, { field: "namChoHuu" }, { field: "nhanHienVat" }];
    // event when change value checkbox
    $scope.ChangeTreeTableCheckbox_Tab1 = function (item) {
        console.log(item);
    };

    // event when click choose row
    $scope.ClickTreeTableRow_Tab1 = function (item) {
        console.log(item);
    };
    //#endregion
    //#region Biệt phái đi học
    // Có hiển thị cột checkbox ở đầu không
    $scope.tree_Table_Checkbox_Tab2 = false;
    // cấu hình hiển thị cùng với icon
    $scope.expanding_property_Tab2 = {
        field: "ngayTao",
        displayName: "Ngày Tạo",
        width: "5%",
        rowspan: 2,
        colspan: 1
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.col_defs_Tab2 = [[
        { field: "soHieu", displayName: "Số hiệu", width: "10%", rowspan: 2, colspan: 1 },
        { field: "gioiTinh", displayName: "Giới tính", width: "7%", rowspan: 2, colspan: 1 },
        { field: "loaiHam", displayName: "Cấp bậc - Loai làm", width: "20%", rowspan: 1, colspan: 2 },
        { field: "chucVu", displayName: "Chức vụ - Đơn vị - Lực lượng", width: "30%", rowspan: 1, colspan: 3 },
        { field: "coGiay", displayName: "Cỡ giày", width: "5%", rowspan: 2, colspan: 1 },
        { field: "coMu", displayName: "Cỡ mũ", width: "5%", rowspan: 2, colspan: 1 },
        { field: "coQA", displayName: "Cõ QA", width: "5%", rowspan: 2, colspan: 1 },
        { field: "namChoHuu", displayName: "Năm chờ hưu", width: "7%", rowspan: 2, colspan: 1 },
        { field: "nhanHienVat", displayName: "Nhận hiện vật", width: "10%", rowspan: 2, colspan: 1 }
    ], [
        { field: "capBac", displayName: "cấp bậc", width: "10%", rowspan: 1, colspan: 1 },
        { field: "loaiHam", displayName: "Loại hàm", width: "11", rowspan: 1, colspan: 1 },
        { field: "chucVu", displayName: "Chức vụ", width: "10%", rowspan: 1, colspan: 1 },
        { field: "viTriCongTac", displayName: "Ví trí công tác", width: "5%", rowspan: 1, colspan: 1 },
        { field: "lucLuong", displayName: "Lực lượng", width: "5%", rowspan: 1, colspan: 1 }
    ]];

    $scope.body_defs_Tab2 = [{ field: "soHieu", rowspan: 1, colspan: 2 }, { field: "gioiTinh" }, { field: "capBac" }, { field: "loaiHam" }
        , { field: "chucVu" }, { field: "viTriCongTac" }, { field: "lucLuong" }, { field: "coGiay" }, { field: "coMu" }
        , { field: "coQA" }, { field: "namChoHuu" }, { field: "nhanHienVat" }];
    // event when change value checkbox
    $scope.ChangeTreeTableCheckbox_Tab2 = function (item) {
        console.log(item);
    };

    // event when click choose row
    $scope.ClickTreeTableRow_Tab2 = function (item) {
        console.log(item);
    };
    //#endregion
    //#region Tuyển mới
    // Có hiển thị cột checkbox ở đầu không
    $scope.tree_Table_Checkbox_Tab3 = false;
    // cấu hình hiển thị cùng với icon
    $scope.expanding_property_Tab3 = {
        field: "ngayTao",
        displayName: "Ngày Tạo",
        width: "5%",
        rowspan: 2,
        colspan: 1
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.col_defs_Tab3 = [[
        { field: "soHieu", displayName: "Số hiệu", width: "10%", rowspan: 2, colspan: 1 },
        { field: "gioiTinh", displayName: "Giới tính", width: "7%", rowspan: 2, colspan: 1 },
        { field: "loaiHam", displayName: "Cấp bậc - Loai làm", width: "20%", rowspan: 1, colspan: 2 },
        { field: "chucVu", displayName: "Chức vụ - Đơn vị - Lực lượng", width: "30%", rowspan: 1, colspan: 3 },
        { field: "coGiay", displayName: "Cỡ giày", width: "5%", rowspan: 2, colspan: 1 },
        { field: "coMu", displayName: "Cỡ mũ", width: "5%", rowspan: 2, colspan: 1 },
        { field: "coQA", displayName: "Cõ QA", width: "5%", rowspan: 2, colspan: 1 },
        { field: "namChoHuu", displayName: "Năm chờ hưu", width: "7%", rowspan: 2, colspan: 1 },
        { field: "nhanHienVat", displayName: "Nhận hiện vật", width: "10%", rowspan: 2, colspan: 1 }
    ], [
        { field: "capBac", displayName: "cấp bậc", width: "10%", rowspan: 1, colspan: 1 },
        { field: "loaiHam", displayName: "Loại hàm", width: "11", rowspan: 1, colspan: 1 },
        { field: "chucVu", displayName: "Chức vụ", width: "10%", rowspan: 1, colspan: 1 },
        { field: "viTriCongTac", displayName: "Ví trí công tác", width: "5%", rowspan: 1, colspan: 1 },
        { field: "lucLuong", displayName: "Lực lượng", width: "5%", rowspan: 1, colspan: 1 }
    ]];

    $scope.body_defs_Tab3 = [{ field: "soHieu", rowspan: 1, colspan: 2 }, { field: "gioiTinh" }, { field: "capBac" }, { field: "loaiHam" }
        , { field: "chucVu" }, { field: "viTriCongTac" }, { field: "lucLuong" }, { field: "coGiay" }, { field: "coMu" }
        , { field: "coQA" }, { field: "namChoHuu" }, { field: "nhanHienVat" }];
    // event when change value checkbox
    $scope.ChangeTreeTableCheckbox_Tab3 = function (item) {
        console.log(item);
    };

    // event when click choose row
    $scope.ClickTreeTableRow_Tab3 = function (item) {
        console.log(item);
    };
    //#endregion

    $scope.LoadPage();
    hideLoading();
    $scope.TabVer = function (item) {
        $scope.checkKy = item;
        $scope.LoadPage();
        $scope.LoadDataTree();
        $('.tab-ver').removeClass('active');
        $(this).addClass('active');
    }
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller('CapNhatChiTietLoi', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading, data) {
    $scope.model = data.item;
    //$scope.model.CAP_BAC_ID = $scope.model.CAP_BAC_ID.toString();
    //$scope.model.LOAI_HAM_ID = $scope.model.LOAI_HAM_ID.toString();
    //$scope.model.LUC_LUONG_ID = $scope.model.LUC_LUONG_ID.toString();
    var treeLucLuong;

    var treeLucLuongSource = [];
    angular.element(document).ready(function () {
        $.ajax({
            url: '/ChotQuanSoDauKy/GetDanhMucCapNhatLoi',
            type: "get",
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.ListCapBac = res.capBac;
                    $scope.ListLoaiHam = res.loaiHam;
                    $scope.ListLucLuong = res.lucLuong;
                    //#region Lực lượng

                    //treeLucLuongSource = [];
                    //if (res.lucLuong != null && res.lucLuong.length > 0) {
                    //    var lucLuong0s = res.lucLuong.filter(function (x) {
                    //        return (x.ParentId == 0);
                    //    });
                    //    if (lucLuong0s != null && lucLuong0s.length > 0) {
                    //        for (var i = 0; i < lucLuong0s.length; i++) {

                    //            var comboTree = { id: 0, title: lucLuong0s[i].Name /*'Lực lượng'*/ };
                    //            // kiểm tra có con không
                    //            var childs = res.lucLuong.filter(function (x) {
                    //                return (x.ParentId == lucLuong0s[i].Id);
                    //            });

                    //            if (childs != null && childs.length > 0) {
                    //                ConvertTreeLucLuong(comboTree, res.lucLuong, lucLuong0s[i].Id);
                    //            }
                    //            treeLucLuongSource.push(comboTree);

                    //        }
                    //    }
                    //}

                    //treeLucLuong = $('#justAnotherInputBox').comboTree({
                    //    source: treeLucLuongSource,
                    //    isMultiple: false
                    //});

                    //#endregion
                    $scope.$apply();
                }
            },
            error: function (err) {
                toastr.error("Lỗi nhập dữ liệu.")
            }
        });
    })

    function ConvertTreeLucLuong(lstTreeModel, lucLuongs, Id) {
        var lstPageMenu = lucLuongs.filter(function (x) {
            return (x.ParentId == Id);
        });

        if (lstPageMenu != null && lstPageMenu.length > 0) {
            lstTreeModel.subs = [];
            for (var i = 0; i < lstPageMenu.length; i++) {
                var tree =
                {
                    id: lstPageMenu[i].Id,
                    title: lstPageMenu[i].Name
                };
                // Kiểm tra xem có con không
                var lucLuongChilds = lucLuongs.filter(function (x) {
                    return (x.ParentId == lstPageMenu[i].Id);
                });
                if (lucLuongChilds != null && lucLuongChilds.length > 0) {
                    ConvertTreeLucLuong(tree, lucLuongs, lstPageMenu[i].Id);
                }
                lstTreeModel.subs.push(tree);
            }

        }
    }

    $scope.ChangeLucLuong = function () {
        var id = treeLucLuong.getSelectedIds();
        if (id > 0) {
            $scope.model.LUC_LUONG_ID = id;
        } else {
            $scope.model.LUC_LUONG_ID = 0;
        }
    }

    $scope.CapNhat = function () {
        $("#formUpdateCB").validate({
            rules: {
                SO_HIEU: {
                    required: true
                },
                GIOI_TINH: {
                    required: true
                },
                CAP_BAC_ID: {
                    required: true
                },
                LOAI_HAM_ID: {
                    required: true
                },
                LUC_LUONG_ID: {
                    required: true
                }
            },
            messages: {
                SO_HIEU: {
                    required: "Vui lòng nhập Số hiệu"
                },
                GIOI_TINH: {
                    required: "Vui lòng chọn Giới tính"
                },
                CAP_BAC_ID: {
                    required: "Vui lòng chọn Cấp bậc"
                },
                LOAI_HAM_ID: {
                    required: "Vui lòng chọn Loại hàm"
                },
                LUC_LUONG_ID: {
                    required: "Vui lòng chọn Lực lượng"
                }
            }
        });
        if ($('#formUpdateCB').valid()) {
            $.ajax({
                url: '/ChotQuanSoDauKy/CapNhatLoi',
                type: "post",
                data: { model: $scope.model, type: data.type, ky: data.ky },
                success: function (res) {
                    if (res.Error) {
                        toastr.error(res.Title);
                    } else {
                        toastr.success(res.Title);
                        $scope.cancel();
                    }
                },
                error: function (err) {
                    toastr.error("Lỗi nhập dữ liệu.")
                }
            });
        }
    }

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller('thongBaoLoi', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading, data) {
    $scope.model = data.danhSachLoi;
    hideLoading();
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});