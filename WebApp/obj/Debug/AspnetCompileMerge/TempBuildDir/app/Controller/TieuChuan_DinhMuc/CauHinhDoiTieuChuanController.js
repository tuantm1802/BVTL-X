app.controller("CauHinhDoiTieuChuanController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "ROLE_TYPE DESC";

    $scope.TC_MaDoiTieuChuan = "";
    $scope.DSTieuChuan = [];
    $scope.AddSPDoi_SanPhamChons = [];
    $scope.selectedRowTieuChuan = -1;
    $scope.selectedRowSPD = -1;
    $scope.MaTieuChuanChon = "";
    $scope.SPD_TenSanPham = "";
    $scope.SPD_CheckAllSPDoi = false;
    $scope.DSSanPhamDoi = [];
    $scope.TenSanPhamDoi = "";
    $scope.SPN_TenSanPham = "";
    $scope.SPN_CheckAll = false;
    $scope.SPN_SanPhamChonId = 0;
    $scope.SPN_KhuVucChonId = 0;
    $scope.ShowBoxRight = false;
    $scope.DSSanPhamNhan = [];
    $scope.selectedRowSPN = -1;
    $scope.AddSPDoi_KhuVuc = 0;
    $scope.AddSPNhan_SanPhamChons = [];
    angular.element(document).ready(function () {
        GetBottomAction();
        GetDanhMuc();
        GetTieuChuan();
    });

    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnView = false;
    $scope.RoleBtnRollback = false;
    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/TieuChuanCBCSPham/GetBottomAction',
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'btnSave') {
                            $scope.RoleBtnSave = true;
                        }
                        if (item == 'btnDelete') {
                            $scope.RoleBtnDelete = true;
                        }
                        if (item == 'btnView') {
                            $scope.RoleBtnView = true;
                        }
                        if (item == 'btnRollback') {
                            $scope.RoleBtnRollback = true;
                        }
                    });
                }
                $scope.$apply();
            }
        });
    }

    function GetDanhMuc() {
        $scope.ListSanPham = [];
        $scope.ListKhuVuc = [];
        $.ajax({
            type: 'post',
            url: '/CauHinhDoiTieuChuan/GetDanhMuc',
            data: {},
            success: function (data) {
                $scope.ListSanPham = data.sanPhams;
                $scope.ListSanPhamALL = data.sanPhams;
                
                $scope.ListKhuVuc = data.khuVucs;
                $scope.$apply();
            }
        });
    }


    //// Danh sách function

    // Tìm kiếm tiêu chuẩn
    $scope.TC_TimKiem = function () {
        //if ($scope.TC_MaDoiTieuChuan == null || $scope.TC_MaDoiTieuChuan == '') {
        //    toastr.error('Bạn chưa nhập điều kiện tìm kiếm!');
        //} else {
        GetTieuChuan();
        //}
    };

    function GetTieuChuan() {
        $scope.selectedRowTieuChuan = -1;
        //showToast();
        $scope.DSTieuChuan = [];
        $.ajax({
            type: 'post',
            url: '/CauHinhDoiTieuChuan/GetQuyetDinh',
            data: {
                maTieuChuan: $scope.TC_MaDoiTieuChuan
            },
            success: function (data) {
                //hideLoading();
                $scope.DSTieuChuan = data.data;
                $scope.$apply();
            }
        });
    }

    $scope.TC_TieuChuanSelect = {};
    // Chọn tiêu chuẩn
    $scope.TC_SelectTieuChuan = function (item, index) {
        $scope.selectedRowTieuChuan = index;
        $scope.TC_TieuChuanSelect = item;
        $scope.ShowBoxRight = true;
        $scope.SPDoiSTT = 0;
        $scope.DSSanPhamNhan = [];
        $scope.AddSPDoi_SanPhams = $scope.ListSanPhamALL;
		//$scope.AddSPDoi_SanPhamsForAddNew = [];
		
        GetSPDoi();
		
    };

    function GetSPDoi() {
        // Lấy danh sách sản phẩm đổi
        $scope.DSSanPhamDoi = [];
        $.ajax({
            type: 'post',
            url: '/CauHinhDoiTieuChuan/GetTrangPhucByQD',
            data: {
                id: $scope.TC_TieuChuanSelect.ID,
                keyword: $scope.SPD_TenSanPham
            },
            success: function (data) {
                hideLoading();
                if (data.Error == true) {
                    toastr.error(data.Title);
                } else {
                    //toastr.success(data.Title);
                    $scope.DSSanPhamDoi = data.data;
                    $scope.$apply();
                }
				console.log($scope.DSSanPhamDoi);
            }
        });
    }

    $scope.AddSPDoi_SanPhams = [];
    // Form thêm sản phẩm đổi
    $scope.AddSPDoi_SearchSanPham = function () {
        $scope.AddSPDoi_SanPhams = [];
        var sanPhams = $scope.ListSanPham.filter(function (x) {
            return (x.TEN_SP != null && x.TEN_SP.indexOf($scope.AddSPDoi_Keyword) != -1);
        });

        if (sanPhams !== undefined && sanPhams != null && sanPhams.length > 0) {
            for (var i = 0; i < sanPhams.length; i++) {
                sanPhams[i].SELECT = false;
                if ($scope.AddSPDoi_KhuVuc != null && $scope.AddSPDoi_KhuVuc != '') {
                    // Xong danh sách sản phẩm có khu vực chọn chưa
                    var spKVs = $scope.DSSanPhamDoi.filter(function (x) {
                        return (x.id == $scope.AddSPDoi_KhuVuc);
                    });
                    //console.log(spKVs);

                    if (spKVs == null || spKVs.length == 0 || spKVs == undefined) {
                        $scope.AddSPDoi_SanPhams.push(sanPhams[i]);
                    } else {
                        //console.log($scope.DSSanPhamDoi);
                        //console.log($scope.AddSPDoi_KhuVuc);

                        for (var j = 0; j < $scope.DSSanPhamDoi.length; j++) {
                            if ($scope.DSSanPhamDoi[j].id == $scope.AddSPDoi_KhuVuc) {
                                //console.log($scope.DSSanPhamDoi[j].id);

                                if ($scope.DSSanPhamDoi[j].children != null && $scope.DSSanPhamDoi[j].children.length > 0) {
                                    var checkChild = $scope.DSSanPhamDoi[j].children.filter(function (x) {
                                        return (x.id == sanPhams[i].ID);
                                    });
                                    if (checkChild == null || checkChild.length == 0) {
                                        $scope.AddSPDoi_SanPhams.push(sanPhams[i]);
                                    }
                                } else {
                                    $scope.AddSPDoi_SanPhams.push(sanPhams[i]);
                                }
                            }
                        }
                    }

                } else {
                    $scope.AddSPDoi_SanPhams.push(sanPhams[i]);
                }
            }

            //console.log($scope.AddSPDoi_SanPhams.length);

        }
    };

    // Tìm kiếm sản phẩm
    $scope.SPD_TimKiem = function () {
        GetSPDoi();
    };

    // Làm mới danh sách tiêu chuẩn
    $scope.TC_LamMoi = function () {
        GetTieuChuan();
    };

    // thêm mới tiêu chuẩn
    $scope.TC_ThemMoi = function () {
        $scope.TC_SO_QUYET_DINH = '';
        $('#AddTieuChuan').modal('show');
    };

    // Lưu tiêu chuẩn
    $scope.TC_LuuTieuChuan = function () {
        if ($scope.TC_SO_QUYET_DINH == null || $scope.TC_SO_QUYET_DINH == '') {
            toastr.error('Bạn chưa nhập mô tả!');
        } else {
            $.ajax({
                type: 'post',
                url: '/CauHinhDoiTieuChuan/AddTieuChuan',
                data: {
                    soQuyetDinh: $scope.TC_SO_QUYET_DINH
                },
                success: function (data) {
                    hideLoading();
                    if (data.Error == true) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $('#AddTieuChuan').modal('hide');
                        GetTieuChuan();

                    }
                }
            });
        }
    };

    // thêm mới sản phẩm đổi
    $scope.SPD_ThemMoi = function () {
		
        $scope.AddSPDoi_SanPhams = $scope.ListSanPhamALL;

        $scope.AddSPDoi_Keyword = "";
		$scope.AddSPDoi_SanPhamChons = [];
        //$scope.$apply();
        $('#AddSPDoi').modal('show');
    };

    // thêm mới sản phẩm đổi
    //$scope.AddSPDoi_SanPhamsForAddNew = [];

    $scope.AddSPDoi_ThemSP = function () {
        if ($scope.AddSPDoi_KhuVuc != null && $scope.AddSPDoi_KhuVuc != '') {
            var spChons = $scope.AddSPDoi_SanPhamChons;

            if (spChons != null && spChons.length > 0) {
                // Xong danh sách sản phẩm có khu vực chọn chưa
                var spKVs = $scope.DSSanPhamDoi.filter(function (x) {
                    return (x.id == $scope.AddSPDoi_KhuVuc);
                });
                if (spKVs == null || spKVs.length == 0) {
                    var KVs = $scope.ListKhuVuc.filter(function (x) {
                        return (x.ID == $scope.AddSPDoi_KhuVuc);
                    });
                    for (var k = 0; k < KVs.length; k++) {
                        var kv = {
                            name: KVs[k].TEN_KHU_VUC,
                            id: KVs[k].ID,							
                            opened: true,
                            parent: true,
                            children: []
                        };
                        for (var sp = 0; sp < spChons.length; sp++) {
                            var spd = {
                                id: spChons[sp].ID,
                                name: spChons[sp].TEN_SP,
								ch_id: 0,
                                parent: false,
                                title: spChons[sp].TEN_SP
                            }
                            kv.children.push(spd);
                        }
                        $scope.DSSanPhamDoi.push(kv);
                    }
                } else {
                    for (var i = 0; i < $scope.DSSanPhamDoi.length; i++) {
                        if ($scope.DSSanPhamDoi[i].id == $scope.AddSPDoi_KhuVuc) {
                            if ($scope.DSSanPhamDoi[i].children != null && $scope.DSSanPhamDoi[i].children.length > 0) {
                                for (var sp = 0; sp < spChons.length; sp++) {
                                    var spd = {
                                        id: spChons[sp].ID,
                                        name: spChons[sp].TEN_SP,
										ch_id: 0,
                                        parent: false,
                                        title: spChons[sp].TEN_SP
                                    }
                                    $scope.DSSanPhamDoi[i].children.push(spd);
                                }
                            } else {
                                $scope.DSSanPhamDoi[i].children = [];
                                for (var sp = 0; sp < spChons.length; sp++) {
                                    var spd = {
                                        id: spChons[sp].ID,
                                        name: spChons[sp].TEN_SP,
										ch_id: 0,
                                        parent: false,
                                        title: spChons[sp].TEN_SP
                                    }
                                    $scope.DSSanPhamDoi[i].children.push(spd);
                                }
                            }
                        }

                    }
                }
                //$scope.$apply();
                $('#AddSPDoi').modal('hide');
            }
            else {
                toastr.error("Vui lòng chọn sản phẩm!");
            }
        } else {
            toastr.error("Vui lòng chọn khu vực!");
        }
		
		$scope.AddSPDoi_SanPhamChons.forEach(x=>{
			x.ch_id = 0
		});
        //$scope.AddSPDoi_SanPhamsForAddNew = $scope.AddSPDoi_SanPhamsForAddNew.concat($scope.AddSPDoi_SanPhamChons);
		console.log($scope.DSSanPhamDoi);

    };

    $scope.CancelDoiSP = function () {
        
        if ($scope.AddSPDoi_SanPhamChons !== null && $scope.AddSPDoi_SanPhamChons.length > 0) {
            console.log($scope.AddSPDoi_SanPhams.length);
            $scope.AddSPDoi_SanPhams = $scope.AddSPDoi_SanPhams.concat($scope.AddSPDoi_SanPhamChons);
            $scope.AddSPDoi_SanPhamChons = [];
            console.log($scope.AddSPDoi_SanPhams);

        }
    }
    // Xóa sản phẩm đổi
    $scope.SPD_Xoa = function () {
        // Kiểm tra xem đã chọn sản phẩm cần xóa chưa
        var countDelete = 0;
        for (var i = 0; i < $scope.DSSanPhamDoi.length; i++) {
            if ($scope.DSSanPhamDoi[i].selected == true) {
                countDelete++;
            }
            if ($scope.DSSanPhamDoi[i].children != null && $scope.DSSanPhamDoi[i].children.length > 0) {
                for (var j = 0; j < $scope.DSSanPhamDoi[i].children.length; j++) {
                    if ($scope.DSSanPhamDoi[i].children[j].selected == true) {
                        countDelete++;
                    }
                }
            }
        }

        if (countDelete != null && countDelete > 0) {
            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn xóa sản phẩm đã chọn?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Có',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            $scope.SPN_SanPhamChonId = 0;
                            $scope.TenSanPhamDoi = "";
                            $scope.SPN_KhuVucChonId = 0;
                            for (var i = 0; i < $scope.DSSanPhamDoi.length; i++) {
                                if ($scope.DSSanPhamDoi[i].selected == true) {
                                    $scope.DSSanPhamDoi.splice(i, 1);
                                }
                                if ($scope.DSSanPhamDoi[i].children != null && $scope.DSSanPhamDoi[i].children.length > 0) {
                                    var spKhongCho = $scope.DSSanPhamDoi[i].children.filter(function (x) {
                                        return (x.selected == false);
                                    });
                                    if (spKhongCho != null && spKhongCho.length > 0) {
                                        $scope.DSSanPhamDoi[i].children = spKhongCho;

                                    } else {
                                        $scope.DSSanPhamDoi[i].children = [];
                                    }

                                }
                            }
                            $scope.$apply();
                        }
                    },
                    close: {
                        text: 'Không',
                        action: function (scope, button) {

                        }
                    }
                }
            });
        }
        else {
            toastr.error("Chưa có sản phẩm nào được chọn!");
        }

    };

    // Chọn trang phục nhận
    $scope.AddSPNhan_CheckedTrangPhuc = function (index) {
        if ($scope.AddSPNhan_SanPhams[index].SELECT == true) {
            $scope.AddSPNhan_SanPhams[index].SELECT = false;
            // thêm vào danh sách chọn
            $scope.AddSPNhan_SanPhamChons.push($scope.AddSPNhan_SanPhams[index]);

            // xóa khỏi danh sách sản phẩm
            $scope.AddSPNhan_SanPhams.splice(index, 1);
        }
    };

    // Bỏ Chọn trang phục nhận
    $scope.AddSPNhan_CheckedTrangPhucChon = function (index) {
        if ($scope.AddSPNhan_SanPhamChons[index].SELECT == true) {
            $scope.AddSPNhan_SanPhamChons[index].SELECT = false;
            // thêm vào danh sách chọn
            $scope.AddSPNhan_SanPhams.push($scope.AddSPNhan_SanPhamChons[index]);

            // xóa khỏi danh sách sản phẩm
            $scope.AddSPNhan_SanPhamChons.splice(index, 1);
        }
    };


    // Chọn, bỏ chọn tất cả sản phẩm đổi
    $scope.SPD_ChangeSPDoi = function () {
    };

    // Chọn sản phẩm đổi
    $scope.SPD_ChooseSPD = function () {
    };

    // Hủy thay đổi sản phẩm đổi
    $scope.SPD_HuyThayDoi = function () {
        //$scope.AddSPDoi_SanPhamsForAddNew = [];
        GetSPDoi();
    };

    // Lưu sản phẩm đổi
    $scope.SPD_Luu = function () {
        console.log($scope.DSSanPhamDoi);
        $.ajax({
            type: 'post',
            url: '/CauHinhDoiTieuChuan/LuuCauHinhSPDoi',
            data: {
                qdId: $scope.TC_TieuChuanSelect.ID,
                sanPhamDois: $scope.DSSanPhamDoi
            },
            success: function (data) {
                hideLoading();
                if (data.Error) {
                    toastr.error(data.Title);
                }
                else {
                    toastr.success(data.Title);
                }
            }
        });
    };

    // thêm mới sản phẩm nhận
    $scope.SPN_ThemMoi = function () {
        if ($scope.SPN_SanPhamChonId != null && $scope.SPN_SanPhamChonId > 0) {
            var spn = {
                SoLuong: 1,
                TEN_KHU_VUC: '',
                TEN_SP: '',
                ID: 0,
                SP_ID: 0,
                SO_QUYET_DINH_ID: 0,
                MA_KHU_VUC: 0,
                NGAY_TAO: null,
                NGAY_CAP_NHAT: null,
                NGUOI_CAP_NHAT: null,
                SELECT: false
            };
            $scope.DSSanPhamNhan.push(spn);
            $scope.disabledSoLuongNhan = true;
        }
        else {
            toastr.error("Vui lòng chọn trang phục đổi!");
        }

    };

    // xóa sản phẩm nhận
    $scope.SPN_Xoa = function () {

        var spKhongCho = $scope.DSSanPhamNhan.filter(function (x) {
            return (x.SELECT != true);
        });
        if (spKhongCho != null && spKhongCho.length > 0) {
            $scope.DSSanPhamNhan = spKhongCho;

        } else {
            $scope.DSSanPhamNhan = [];
        }
    };

    // Chọn tất cả sản phẩm để add vào sp đổi
    $scope.AddSPDoi_ChangeCheckAllTP = function () {
        if ($scope.AddSPDoi_CheckAllTP == true) {
            $scope.AddSPDoi_CheckAllTP = false;
            for (var i = 0; i < $scope.AddSPDoi_SanPhams.length; i++) {
                $scope.AddSPDoi_SanPhams[i].SELECT = false;
                // thêm vào danh sách sản phẩm
                $scope.AddSPDoi_SanPhamChons.push($scope.AddSPDoi_SanPhams[i]);
            }
            $scope.AddSPDoi_SanPhams = [];
        }

    };

    // Bỏ Chọn tất cả sản phẩm để add vào sp đổi
    $scope.AddSPDoi_ChangeCheckAllTPChon = function () {
        if ($scope.AddSPDoi_CheckAllTPChon == true) {
            $scope.AddSPDoi_CheckAllTPChon = false;
            for (var i = 0; i < $scope.AddSPDoi_SanPhamChons.length; i++) {
                $scope.AddSPDoi_SanPhamChons[i].SELECT = false;
                // thêm vào danh sách sản phẩm
                $scope.AddSPDoi_SanPhams.push($scope.AddSPDoi_SanPhamChons[i]);
            }
            $scope.AddSPDoi_SanPhamChons = [];
        }

    };
	
	$scope.checkDuplicateSPInArray = function(arr, idSPCheck){
		var counter = 0;
		arr.forEach( x => {
			if(x.SP_ID === idSPCheck){
				counter++;				
			}
		});
		return counter;
	}
	
	$scope.checkHasDuplicate = function(arr){
		const uniqueValues = new Set(arr.map(v => v.SP_ID));

		if (uniqueValues.size < arr.length) {
		  return true;
		}
		return false;
	}

    $scope.SPN_ChangeSanPhamNhan = function (index, tenSP, idSP) {
		 console.log($scope.DSSanPhamNhan);
		//Kiểm tra trùng SP
		
		console.log($scope.checkDuplicateSPInArray($scope.DSSanPhamNhan, idSP ));
		
		if($scope.checkDuplicateSPInArray($scope.DSSanPhamNhan, idSP ) > 1){
			toastr.error("Sản phẩm nhận không được trùng nhau");
		}
		
        $scope.DSSanPhamNhan[index].TEN_SP = tenSP;
        		
    };

    // chọn thêm sp vào sp đổi
    $scope.AddSPDoi_CheckedTrangPhuc = function (Id) {
        $scope.AddSPDoi_CheckAllTP = false;
		
		//console.log(Id);
		//console.log($scope.AddSPDoi_SanPhams.find(x=>x.ID === Id));
        /* if ($scope.AddSPDoi_SanPhams[index].SELECT == true) {
            $scope.AddSPDoi_SanPhams[index].SELECT = false;
            // thêm vào danh sách sản phẩm
            $scope.AddSPDoi_SanPhamChons.push($scope.AddSPDoi_SanPhams[index]);

            // xóa khỏi danh sách sản phẩm chọn
            $scope.AddSPDoi_SanPhams.splice(index, 1);
        } */
		
		if ($scope.AddSPDoi_SanPhams.find(x=>x.ID === Id).SELECT == true) {
            $scope.AddSPDoi_SanPhams.find(x=>x.ID === Id).SELECT = false;
            // thêm vào danh sách sản phẩm
            $scope.AddSPDoi_SanPhamChons.push($scope.AddSPDoi_SanPhams.find(x=>x.ID === Id));

            // xóa khỏi danh sách sản phẩm chọn
			//var indexXoa = $scope.AddSPDoi_SanPhams.findIndex(x=>x.ID === Id);
            //$scope.AddSPDoi_SanPhams.splice(indexXoa, 1);
        }


        //var sanPhams = $scope.AddSPDoi_SanPhams.filter(function (x) {
        //    return (x.SELECT == true);
        //});
        //if (sanPhams != null && sanPhams.length > 0 && sanPhams.length == $scope.AddSPDoi_SanPhams.length) {
        //    $scope.AddSPDoi_CheckAllTP = true;
        //}
    };

    // bỏ chọn sp vào sp đổi
    $scope.AddSPDoi_CheckedTrangPhucChon = function (index) {
        $scope.AddSPDoi_CheckAllTPChon = false;
        if ($scope.AddSPDoi_SanPhamChons[index].SELECT == true) {
            $scope.AddSPDoi_SanPhamChons[index].SELECT = false;
            // thêm vào danh sách sản phẩm
            $scope.AddSPDoi_SanPhams.push($scope.AddSPDoi_SanPhamChons[index]);

            // xóa khỏi danh sách sản phẩm chọn
            $scope.AddSPDoi_SanPhamChons.splice(index, 1);
        }


        //var sanPhams = $scope.AddSPDoi_SanPhams.filter(function (x) {
        //    return (x.SELECT == true);
        //});
        //if (sanPhams != null && sanPhams.length > 0 && sanPhams.length == $scope.AddSPDoi_SanPhams.length) {
        //    $scope.AddSPDoi_CheckAllTP = true;
        //}
    };

    // Tìm kiếm SPN
    $scope.SPN_TimKiem = function () {
        GetSanPhamNhan();
    };

    // Chọn, bỏ chọn tất cả SPN
    $scope.SPN_ChangeCheckAll = function () {
        for (var i = 0; i < $scope.DSSanPhamNhan.length; i++) {
            $scope.DSSanPhamNhan[i].SELECT = $scope.SPN_CheckAll;
        }
    };

    //Chọn SPN
    $scope.SPN_ChooseSanPhamNhan = function () {
        $scope.SPN_CheckAll = false;
        var sanPhams = $scope.DSSanPhamNhan.filter(function (x) {
            return (x.SELECT == true);
        });
        if (sanPhams != null && sanPhams.length > 0 && sanPhams.length == $scope.DSSanPhamNhan.length) {
            $scope.SPN_CheckAll = true;
        }
    };

    //Danh sách SPN
    $scope.SPN_DSSanPham = function () {
        if ($scope.SPN_SanPhamChonId != null && $scope.SPN_SanPhamChonId > 0) {
            $scope.AddSPNhan_SanPhamChons = [];
            for (var i = 0; i < $scope.ListSanPham.length; i++) {
                $scope.ListSanPham[i].SELECT = false;
                $scope.AddSPNhan_SanPhams.push($scope.ListSanPham[i]);
            }
           
            $scope.AddSPNhan_Keyword = "";
            $('#AddSPNhan').modal('show');
        }
        else {
            toastr.error("Vui lòng chọn trang phục đổi!");
        }
    };

    $scope.AddSPNhan_SanPhams = [];
    // Form thêm sản phẩm đổi
    $scope.AddSPNhan_SearchSanPham = function () {
        $scope.AddSPNhan_SanPhams = [];
        var sanPhams = $scope.ListSanPham.filter(function (x) {
            return (x.TEN_SP != null && x.TEN_SP.indexOf($scope.AddSPNhan_Keyword) != -1);
        });
        if (sanPhams != null && sanPhams.length > 0) {
            $scope.AddSPNhan_SanPhams = sanPhams;
        }
    };

    // Chọn tất cả sản phẩm để add vào sp đổi
    $scope.AddSPNhan_ChangeCheckAllTP = function () {
        if ($scope.AddSPNhan_CheckAllTP == true) {
            $scope.AddSPNhan_CheckAllTP = false;
            for (var i = 0; i < $scope.AddSPNhan_SanPhams.length; i++) {
                $scope.AddSPNhan_SanPhams[i].SELECT = false;
                // thêm vào danh sách sản phẩm
                $scope.AddSPNhan_SanPhamChons.push($scope.AddSPNhan_SanPhams[i]);
            }
            $scope.AddSPNhan_SanPhams = [];
        }

    };

    // Bỏ Chọn tất cả sản phẩm để add vào sp nhận
    $scope.AddSPNhan_ChangeCheckAllTPChon = function () {
        if ($scope.AddSPNhan_CheckAllTPChon == true) {
            $scope.AddSPNhan_CheckAllTPChon = false;
            for (var i = 0; i < $scope.AddSPNhan_SanPhamChons.length; i++) {
                $scope.AddSPNhan_SanPhamChons[i].SELECT = false;
                // thêm vào danh sách sản phẩm
                $scope.AddSPNhan_SanPhams.push($scope.AddSPNhan_SanPhamChons[i]);
            }
            $scope.AddSPNhan_SanPhamChons = [];
        }

    };

    //$scope.AddSPDoi_CheckedTrangPhuc = function () {
    //    $scope.AddSPNhan_CheckAllTP = false;
    //    var sanPhams = $scope.AddSPNhan_SanPhams.filter(function (x) {
    //        return (x.SELECT == true);
    //    });
    //    if (sanPhams != null && sanPhams.length > 0 && sanPhams.length == $scope.AddSPNhan_SanPhams.length) {
    //        $scope.AddSPNhan_CheckAllTP = true;
    //    }
    //};

    // Thêm sản phẩm nhận
    $scope.AddSPNhan_Luu = function () {
        for (var i = 0; i < $scope.AddSPNhan_SanPhamChons.length; i++) {
            var spn = {
                SoLuong: 1,
                TEN_KHU_VUC: '',
                TEN_SP: $scope.AddSPNhan_SanPhamChons[i].TEN_SP,
                ID: 0,
                SP_ID: $scope.AddSPNhan_SanPhamChons[i].ID,
                SO_QUYET_DINH_ID: 0,
                MA_KHU_VUC: 0,
                NGAY_TAO: null,
                NGAY_CAP_NHAT: null,
                NGUOI_CAP_NHAT: null,
                SELECT: false
            };
            $scope.DSSanPhamNhan.push(spn);

        }
        $('#AddSPNhan').modal('hide');
    };

    //hủy thay đổi SPN
    $scope.SPN_HuyThayDoi = function () {
        GetSanPhamNhan();
    };

    //Lưu SPN
    $scope.SPN_Luu = function () {
        if ($scope.SPN_SanPhamChonId != null || $scope.SPN_SanPhamChonId > 0 && $scope.SPN_KhuVucChonId != null || $scope.SPN_KhuVucChonId > 0) {

            //Kiểm tra SP_ID có trùng hay không
            if ($scope.checkHasDuplicate($scope.DSSanPhamNhan)) {
                toastr.error("Không thể lưu do Sản phẩm nhận bị trùng!");
                return;
            }
            // kiểm tra xem số lượng có nhập nhỏ hơn 0 không
            var checkSP = $scope.DSSanPhamNhan.filter(function (x) {
                return (x.SoLuong <= 0);
            });
            if (checkSP == null || checkSP.length == 0) {
                showToast();
                $scope.ListData = [];
                $.ajax({
                    type: 'post',
                    url: '/CauHinhDoiTieuChuan/LuuCauHinh',
                    data: {
                        qdId: $scope.TC_TieuChuanSelect.ID,
                        spDoiId: $scope.SPN_SanPhamChonId,
                        khuVucId: $scope.SPN_KhuVucChonId,
                        sanPhamNhans: $scope.DSSanPhamNhan
                    },
                    success: function (data) {
                        hideLoading();
                        if (data.Error) {
                            toastr.error(data.Title);
                        }
                        else {
                            toastr.success(data.Title);
                        }
                    }
                });
            }
            else {
                toastr.error("Vui lòng nhập số lượng lớn hơn 0.");
            }
        }
    };

    //// Kết thúc

    $scope.pageChanged = function () {
        $scope.LoadPage();
    };

    $scope.ChangeSoLuong = function () {
        $scope.disabledRollbackData = false;
        $scope.disabledSaveChange = false;
    };

    $scope.ViewDetail = function (ID, rowIndex) {
        $scope.selectedRow = rowIndex;
    };

    $scope.LoadPage = function () {
        $.ajax({
            type: 'post',
            url: '/CauHinhDoiTieuChuan/GetAllByPage',
            data: $scope.modelSearch,
            success: function (data) {
                $scope.modelSearch.totalItems = data.totalItems;
                $scope.ListRole = data.data;
                $scope.modelSearch.pageSize = data.pageSize;
                $scope.$apply();
                hideLoading();
            }
        });
    };

    $scope.AddTrangPhuc = function () {
        var trangPhucSelected = $scope.ListSanPhamSearch.filter(function (x) {
            return (x.SELECT == true);
        });
        if (trangPhucSelected != null && trangPhucSelected.length > 0) {
            $.ajax({
                type: 'post',
                url: '/CauHinhDoiTieuChuan/Add',
                data: {
                    loaiTPs: trangPhucSelected,
                    lucLuongId: $scope.LucLuongId,
                    nhomTCId: $scope.NhomTCId,
                    nhomCBId: $scope.NhomCBId,
                    mua: $scope.Mua,
                    nam: $scope.NamBanHanh
                    //, ngayCapNhat: moment($scope.dateUpdate).format()
                },
                success: function (data) {
                    if (data.Error == false) {
                        toastr.success("Thêm mới thành công!");
                        GetTieuChuanCBCS();
                    } else {
                        toastr.error(data.Title);
                    }
                }
            });
        } else {
            toastr.error('Bạn chưa chọn Loại trang phục!');
        }
    };

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
        $scope.NhomCBId = 0;
        $scope.NhomTCId = 0;
        $scope.NhomTCOrNhomCB = "";
        $scope.Mua = "";
        // Lấy danh sách tiêu chuẩn của nhóm tiêu chuẩn đã chọn
        if (treeNode != null) {
            $scope.Mua = treeNode.mua;
            $scope.NhomTCOrNhomCB = treeNode.name_control;
            var arrayId = treeNode.id.split('_');
            if (arrayId != null) {

                if (arrayId.length == 2) {
                    $scope.NhomCBId = 0;
                    $scope.NhomTCId = 0;
                }

                if (arrayId.length == 3) {
                    $scope.NhomCBId = 0;
                    $scope.NhomTCId = parseInt(arrayId[1]);
                }

                if (arrayId.length > 3) {
                    $scope.NhomCBId = parseInt(arrayId[arrayId.length - 2]);;
                    $scope.NhomTCId = parseInt(arrayId[1]);
                }
            } else {
                $scope.NhomCBId = 0;
                $scope.NhomTCId = 0;
            }
        }
        GetTieuChuanCBCS();
    }

    var treeLucLuong;

    var treeLucLuongSource = [];



    function GetTieuChuanCBCS() {
        if ($scope.LucLuongName == 'Lực lượng' || $scope.LucLuongId == 0) {
            toastr.error('Lực lượng chọn không được là lực lượng cha');
        } else {
            showToast();
            $scope.ListData = [];
            $.ajax({
                type: 'post',
                url: '/CauHinhDoiTieuChuan/GetTieuChuanCBCS',
                data: {
                    nhomCBId: $scope.NhomCBId,
                    nhomTCId: $scope.NhomTCId,
                    lucluongId: $scope.LucLuongId,
                    mua: $scope.Mua,
                    nam: $scope.NamBanHanh
                },
                success: function (data) {
                    hideLoading();

                    $scope.ListData = data.data;
                    $scope.$apply();
                }
            });
        }
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


    $scope.ChangeLucLuong = function () {
        $scope.LucLuongName = treeLucLuong.getSelectedNames();
        var id = treeLucLuong.getSelectedIds();
        if (id > 0) {
            $scope.LucLuongId = id;
        } else {
            $scope.LucLuongId = 0;
        }
        GetTieuChuanCBCS();
    };

    // tìm kiếm nhóm tc
    $scope.ChangeNhomTieuChuan = function () {
        $scope.ListPageMenu = [];
        $.ajax({
            type: 'post',
            url: '/CauHinhDoiTieuChuan/SearchNhomTieuChuan',
            data: { keyword: $scope.NhomTieuChuanText },
            success: function (data) {

                $scope.NhomCBId = 0;
                $scope.NhomTCId = 0;
                $scope.NhomTCOrNhomCB = "";
                $scope.Mua = "";

                $scope.ListPageMenu = data.TreeTCData;
                $.fn.zTree.init($("#treeRole"), setting, $scope.ListPageMenu);
                var zTree = $.fn.zTree.getZTreeObj("treeRole");
                var type = { "Y": "ps", "N": "ps" };
                zTree.setting.check.chkboxType = type;
            }
        });
    };


    $scope.AddNew = function () {
        if ($scope.LucLuongName == 'Lực lượng' || $scope.LucLuongId == 0) {
            toastr.error('Lực lượng chọn không được là lực lượng cha');
        } else {
            $scope.ListSanPhamSearch = $scope.ListSanPham;
            $('#AddSanPham').modal('show');
        }
    };

    // Thêm trang phục
    $scope.LoaiSanPham = "NEW";
    $scope.AddNew = function () {
        if ($scope.LucLuongName == 'Lực lượng' || $scope.LucLuongId == 0) {
            toastr.error('Lực lượng chọn không được là lực lượng cha');
        } else {
            $scope.ChangeLoaiTrangPhuc();
            $('#AddSanPham').modal('show');
        }
    };

    // thay đổi loại trang phục
    $scope.ChangeLoaiTrangPhuc = function () {
        $.ajax({
            type: 'post',
            url: '/CauHinhDoiTieuChuan/SearchSanPham',
            data: {
                keyword: $scope.TrangPhucText,
                nhomTCId: $scope.NhomTCId,
                loaiTrangPhuc: $scope.LoaiSanPham,
                nhomCBId: $scope.NhomCBId,
                lucLuongId: $scope.LucLuongId,
                mua: $scope.Mua,
                nam: $scope.NamBanHanh
            },
            success: function (data) {
                $scope.ListSanPhamSearch = data.SanPhamS;
                $scope.$apply()
            }
        });
    };

    // Chọn trang ohuc
    $scope.CheckedTrangPhuc = function () {
        var lstPageMenu = $scope.ListSanPhamSearch.filter(function (x) {
            return (x.SELECT == true);
        });
        if (lstPageMenu != null && lstPageMenu.length == $scope.ListSanPhamSearch.length) {
            $scope.CheckAllTP = true;
        } else {
            $scope.CheckAllTP = false;
        }

    };

    // Chọn bỏ chọn all trang phục
    $scope.ChangeCheckAllTP = function () {
        for (var i = 0; i < $scope.ListSanPhamSearch.length; i++) {
            $scope.ListSanPhamSearch[i].SELECT = $scope.CheckAllTP;
        }
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
            templateUrl: '/CauHinhDoiTieuChuan/_Add',
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
            templateUrl: '/CauHinhDoiTieuChuan/_Edit',
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


    // Lấy lại dữ liệu cũ
    $scope.RollbackData = function () {
        GetTieuChuanCBCS();
        $scope.disabledRollbackData = true;
        $scope.disabledSaveChange = true;
    }

    $scope.delete = function (itemId, name) {

        // lấy danh sách id tieu chuan cbcs can xoa
        var tccbcsIds = $scope.ListData.filter(function (x) {
            return (x.SELECT == true);
        });

        if (tccbcsIds != null && tccbcsIds.length > 0) {
            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn xóa trang phục này?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Xóa',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            $.ajax({
                                type: 'post',
                                url: '/CauHinhDoiTieuChuan/Delete',
                                data: { cbcss: tccbcsIds },
                                success: function (data) {
                                    if (data.Error) {
                                        toastr.error(data.Title);
                                    } else {
                                        toastr.success(data.Title);
                                        //$scope.cancel();
                                        GetTieuChuanCBCS();
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
        }
        else {
            toastr.error("Chưa có hồ sơ nào được chọn!");
        }
    };

    // Lưu thay đổi
    $scope.SaveChange = function () {
        $.ajax({
            type: 'post',
            url: '/CauHinhDoiTieuChuan/Edit',
            data: { loaiTPs: $scope.ListData },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    toastr.success(data.Title);
                    GetTieuChuanCBCS();
                    $scope.disabledRollbackData = true;
                    $scope.disabledSaveChange = true;
                }
            }
        });
    };

    /// Lấy danh sách sản phẩm nhận
    function GetSanPhamNhan() {
        if ($scope.SPN_SanPhamChonId != null || $scope.SPN_SanPhamChonId > 0 && $scope.SPN_KhuVucChonId != null || $scope.SPN_KhuVucChonId > 0) {
            showToast();
            $scope.ListData = [];
            $.ajax({
                type: 'post',
                url: '/CauHinhDoiTieuChuan/GetSanPhamNhan',
                data: {
                    qdId: $scope.TC_TieuChuanSelect.ID,
                    spDoiId: $scope.SPN_SanPhamChonId,
                    khuVucId: $scope.SPN_KhuVucChonId,
                    keyword: $scope.SPN_TenSanPham
                },
                success: function (data) {
                    hideLoading();

                    $scope.DSSanPhamNhan = data.data;
                    $scope.$apply();
                }
            });
        }
    }

    ///////////////////////////////////
    $scope.toggleAllCheckboxes = function ($event) {
        var i,
            item,
            len,
            ref,
            results,
            selected;
        selected = $event.target.checked;
        ref = $scope.DSSanPhamDoi;
        results = [];
        for (i = 0, len = ref.length; i < len; i++) {
            item = ref[i];
            item.selected = selected;
            if (item.children != null) {
                results.push($scope.$broadcast('changeChildren',
                    item));
            } else {
                results.push(void 0);
            }
        }
        return results;
    };
    $scope.initCheckbox = function (item, parentItem) {
        return item.selected = parentItem && parentItem.selected || item.selected || false;
    };
    $scope.toggleCheckbox = function (item, parentScope) {
        $scope.disabledSoLuongNhan = true;

        if (item.children != null) {
            $scope.$broadcast('changeChildren', item);
        }
        if (parentScope.item != null) {

            $scope.SPN_SanPhamChonId = 0;
            $scope.TenSanPhamDoi = "";
            $scope.SPN_KhuVucChonId = 0;

            if (item.selected == true) {
                $scope.SPN_SanPhamChonId = item.id;
                $scope.TenSanPhamDoi = item.name;
                // Lấy khu vực id
                for (var i = 0; i < $scope.DSSanPhamDoi.length; i++) {
                    if ($scope.DSSanPhamDoi[i].children != null && $scope.DSSanPhamDoi[i].children.length > 0) {
                        for (var j = 0; j < $scope.DSSanPhamDoi[i].children.length; j++) {
                            if ($scope.DSSanPhamDoi[i].children[j].id == item.id) {
                                $scope.SPN_KhuVucChonId = $scope.DSSanPhamDoi[i].id;
                            }
                        }
                    }
                }
                GetSanPhamNhan();
            } else {
                for (var i = 0; i < $scope.DSSanPhamDoi.length; i++) {
                    if ($scope.DSSanPhamDoi[i].children != null && $scope.DSSanPhamDoi[i].children.length > 0) {
                        for (var j = 0; j < $scope.DSSanPhamDoi[i].children.length; j++) {
                            if ($scope.DSSanPhamDoi[i].children[j].selected == true) {
                                $scope.SPN_KhuVucChonId = $scope.DSSanPhamDoi[i].id;
                                $scope.SPN_SanPhamChonId = $scope.DSSanPhamDoi[i].children[j].id;
                                $scope.TenSanPhamDoi = $scope.DSSanPhamDoi[i].children[j].name;

                            }
                        }
                    }
                }
                GetSanPhamNhan();
            }

            return $scope.$emit('changeParent', parentScope);
        }
    };
    $scope.$on('changeChildren',
        function (event,
            parentItem) {
            var child,
                i,
                len,
                ref,
                results;
            ref = parentItem.children;
            results = [];
            for (i = 0, len = ref.length; i < len; i++) {
                child = ref[i];
                child.selected = parentItem.selected;
                if (child.children != null) {
                    results.push($scope.$broadcast('changeChildren',
                        child));
                } else {
                    results.push(void 0);
                }
            }

            return results;
        });
    return $scope.$on('changeParent',
        function (event,
            parentScope) {
            var children;
            children = parentScope.item.children;
            parentScope.item.selected = $filter('selected')(children).length === children.length;
            parentScope = parentScope.$parent.$parent;
            if (parentScope.item != null) {
                return $scope.$broadcast('changeParent',
                    parentScope);
            }

            //for (var i = 0; i < $scope.DSSanPhamDoi.length; i++) {
            //    if ($scope.DSSanPhamDoi[i].id != item.id) {
            //        $scope.DSSanPhamDoi[i].selected = false;
            //    }
            //}
        });
});
