app.controller("XoaQuanSoTrungController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location, constant) {
    //#region giá trị mặc định
    $scope.IsShowRight = false;
    $scope.HocVien = false;
    $scope.ListNam = [];
    $scope.ListQSTrungLog = [];
    
    var currentYear = new Date().getFullYear();
    var currentMonth = new Date().getMonth();
    for (var i = currentYear; i > currentYear - 7; i--) {
        $scope.ListNam.push({ Id: i, Name: i });
    }
    $scope.NAM = currentYear.toString();
    $scope.KY = currentMonth <7 ?'XH':'TD';

    $scope.pageSize = 10;
    $scope.currentPage = 1;
    $scope.SoLuongKetQua = 0;

    $scope.pageSizeQSTrung = 10;
    $scope.currentPageQSTrung = 1;
    $scope.SoLuongKetQuaQSTrung = 0;

    angular.element(document).ready(function () {
        $scope.GetDanhMuc();
        $scope.LoadData();
    });

    $scope.pageChanged = function () {
        $scope.LoadData();
    }

    $scope.GetDanhMuc = function () {
        $.ajax({
            type: 'get',
            async: false,
            url: '/ChotQuanSoDauKy/GetDanhMuc',
            success: function (res) {
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
                $scope.NhomCBCallback = function (data) {
                    $scope.NhomCBComboTree = data;
                };
                //#endregion
                $scope.ListCapBacs = res.capBac;
                //$scope.$apply();
            }
        })
    }
    
    $scope.treeData = {};
    $scope.treeData.data = {};
    $scope.ListDonVi;
    
    //#endregion tạo treeview
    $scope.model = {};
    $scope.LoadData = function () {
        showToast();
        $.ajax({
            type: 'post',
            data: { nam: $scope.NAM, ky: $scope.KY, page: $scope.currentPage, pageSize: $scope.pageSize},
            url: '/XoaQuanSoTrung/LayQSTrungLog',
            success: function (res) {
                $scope.ListQSTrungLog = res.data;
                $scope.SoLuongKetQua = res.totalRow;
                $scope.$apply();
                hideLoading();
            }
        })
    }

    // Lấy quân số trùng
    $scope.QuetQSTrung = function () {
        var currentYear = new Date().getFullYear();
        var currentMonth = new Date().getMonth();
        $scope.NAM = currentYear.toString();
        var ky = currentMonth < 7 ? 'XH' : 'TD';
        if (ky != $scope.KY || $scope.NAM != currentYear) {
            toastr.error("Hệ thống không cho quét dữ liệu quá khứ, đề nghị bạn chọn kỳ, năm mới nhất theo dữ liệu toàn hệ thống!");
        } else {
            $scope.CheckAllQSTrung = false;
            QuetQSTrung(0);
        }
    }

    $scope.pageChangedQSTrung = function (data) {
        QuetQSTrung(1);
    }
    $scope.CheckAllQSTrung = false;
    QuetQSTrung = (isNextPage) => {
        $scope.ListQSTrung = [];
        $scope.SoLuongKetQuaQSTrung = 0;
        showToast();
        $.ajax({
            type: 'POST',
            //dataType: 'json',
            cache: false,
            async: true,
            url: '/XoaQuanSoTrung/QuetQSTrung',
            data: {
                nam: $scope.NAM, ky: $scope.KY, page: $scope.currentPageQSTrung, pageSize: $scope.pageSizeQSTrung
            },
            success: function (res) {
                if (res.data != null && res.data.length > 0) {
                    $scope.ListQSTrung = res.data;
                    $scope.currentPageQSTrung = res.totalRow;
                    $scope.$apply();
                    if (isNextPage == 0) {
                        $('#qsTrungModal').modal('show');
                    }
                    $scope.ListQSTrung.forEach(x => x.selected = $scope.CheckAllQSTrung);
                } else {
                    if (isNextPage == 0) {
                        toastr.error('Không có dữ liệu!');
                    }
                }
                hideLoading();
            }, error: (e) => {
                hideLoading();
                toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
            },
            complete: () => {
            }
        });
    }

    $scope.ChangeCheckAllQSTrung = function () {
        $scope.ListQSTrung.forEach(x => x.selected = $scope.CheckAllQSTrung);
    }

    $scope.ChangeCheckQSTrung = function () {
        $scope.CheckAllQSTrung = false;
        var dataSelected = $scope.ListQSTrung.filter(x => x.selected == true);
        if (dataSelected != null && dataSelected.length == $scope.ListQSTrung.length)
            $scope.CheckAllQSTrung = true;
    }
    
    $scope.LuuLog = function () {
       
        // Lấy các dòng được chọn
        var dataSelected = $scope.ListQSTrung.filter(x => x.selected == true);
        if (dataSelected != null && dataSelected.length > 0) {
            $('#qsTrungModal').modal('hide');
            $.ajax({
                type: 'post',
                url: '/XoaQuanSoTrung/LuuQuanSo',
                data: {
                    data: dataSelected
                },
                success: function (res) {
                    if (res.Error) {
                        toastr.error('Xóa dữ liệu không thành công');
                    } else {
                        toastr.success(res.Title);
                        $scope.currentPage = 1;
                        $scope.LoadData();
                    }
                }
            });
        }
        else {
            toastr.error('Vui lòng chọn các bản ghi cần thay đổi!');
        }
    }

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});