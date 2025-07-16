app.controller("KhoVatPhamController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "kqslace_id";

    $scope.ListDuAn = [];
    $scope.ListMaNhomTBH = [];
    $scope.ListNhomTBH = [];

    var ChiTietPhieuXuatNhapTable = null;
    $scope.ParamIdSeleted = 0;
    angular.element(document).ready(function () {
        $scope.ListDuAn = [];
        GetBottomAction();
        $scope.LoadPage(1);
    });

    $scope.RoleBtnExportExcel = false;
    $scope.RoleBtnSearch = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/KhoVatPham/GetBottomAction',
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnExportExcel') {
                            $scope.RoleBtnExportExcel = true;
                        }
                        if (item == 'btnSearch') {
                            $scope.RoleBtnSearch = true;
                        }
                    });
                }
                //if (data.DuAns != null && data.DuAns.length > 0) {
                //    $scope.ListDuAn = data.DuAns;
                //}
                
                $scope.$apply();
            }
        });
    }

    $('#ChiTietPhieuXuatNhapTable').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $scope.LoadPage = function (genTable) {
        showToast();

        $scope.ListKhoVatPham = [];
        if (genTable == 1 || genTable == 2) {
            if (genTable == 2)
                ChiTietPhieuXuatNhapTable.destroy();

            //$.extend($.fn.dataTable.ext.classes, {
            //    sPageButton: 'dt-paging-button page-item'
            //});

            ChiTietPhieuXuatNhapTable = $('#ChiTietPhieuXuatNhapTable').DataTable({
                processing: true,
                serverSide: true,
                
                ajax: {
                    url: '/KhoVatPham/GetAllChiTietPhieuXuatNhaps',
                    type: 'GET',
                    dataType: 'json',
                    data: function (d) {                        
                        d.draw = d.draw;
                        d.start = d.start;
                        d.length = d.length;
                        d.search.value = $('#ChiTietPhieuXuatNhapTable_filter input').val();
                    }
                },
                columns: [
                    //{ data: 'Id' },
                    { data: 'MaSanPham' },
                    { data: 'TenSanPham' },
                    { data: 'MaPhieu' },
                    {
                        data: 'NgayLap',
                        render: function (data) {
                            return moment(data).isValid() ? moment(data).format('DD/MM/YYYY') : '';
                        }
                    },
                    {
                        data: 'SoLuong'
                    },                          
                    {
                        data: 'LoaiPhieu',
                        render: function (data) {
                            switch (data) {
                                case 1: return 'Nhập'
                                case 2: return 'Xuất'                                
                                default: return '';
                            }
                        }
                    },        
                    {
                        data: 'GhiChu'
                    },
                    //{
                    //    data: null,
                    //    className: 'text-center',
                    //    render: function (d, type, row) {
                    //        return `
                    //                <a href="/KhoVatPham/Details/${row.Id}?recordid=${row.RecordId}" class="btn btn-sm btn-success"><i class="fa fa-eye"></i> Chi tiết</a>
                    //                `;
                    //    }
                    //}
                ],

                language: {
                    processing: "Đang xử lý...",
                    paginate: {
                        first: "Đầu",
                        last: "Cuối",
                        next: "Sau",
                        previous: "Trước"
                    },
                    emptyTable: "Không có dữ liệu",
                    lengthMenu: "Hiển thị _MENU_ bản ghi",
                    zeroRecords: "Không tìm thấy bản ghi nào phù hợp"
                }
            });
        } else {
            ChiTietPhieuXuatNhapTable.ajax.reload();
        }
        hideLoading();
    };

    // Lắng nghe sự kiện keyup trên ô tìm kiếm nhanh
    $('#ChiTietPhieuXuatNhapTable_filter input').unbind() // Hủy sự kiện tìm kiếm mặc định của DataTable
        .bind('keyup', function (e) {
            if (e.keyCode === 13 || this.value.length > 2) { // Chỉ tìm kiếm khi Enter hoặc từ 3 ký tự trở lên
                table.search(this.value).draw();
            }
            if (this.value.length === 0) { // Xóa tìm kiếm nếu không có ký tự nào
                table.search('').draw();
            }
        });

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };
    
    $scope.CapNhatPhieuXuatNhap = function (loaiphieu) {
        // Lấy dữ liệu cần thiết từ giao diện
        var dataToSend = {
            loaiphieu: loaiphieu, // 1: Nhập, 2: Xuất            
        };

        // Gửi AJAX POST tới server
        $.ajax({
            url: '/KhoVatPham/CapNhatPhieuXuatNhap',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(dataToSend), // Chuyển dữ liệu sang JSON
            success: function (response) {
                if (response.success) {
                    // Thông báo thành công
                    showToast('Cập nhật phiếu thành công!', 'success');
                    $scope.Refesh(); // Refresh lại danh sách
                } else {
                    // Thông báo lỗi từ server
                    showToast(response.message || 'Đã có lỗi xảy ra.', 'error');
                }
            },
            error: function (error) {
                // Thông báo lỗi hệ thống
                showToast('Không thể cập nhật phiếu. Vui lòng thử lại sau.', 'error');
                console.error(error);
            }
        });
    };
    

    $scope.ExportExcel = function () {
        window.location.href = '/KhoVatPham/ExportData?keyword=' + $scope.modelSearch.KeyWord;
    }

});

app.controller('TonKhoVatPhamController', function ($scope, $http) {
    // Tính toán ngày hiện tại và ngày cách đây 3 tháng
    const today = new Date();
    const threeMonthsAgo = new Date();
    threeMonthsAgo.setMonth(today.getMonth() - 3);

    // Gán giá trị mặc định cho Từ Ngày và Đến Ngày
    $scope.fromDate = threeMonthsAgo; // Đối tượng Date
    $scope.toDate = today; // Đối tượng Date
    $scope.MaNhomTBH = '';

    let dataTable = null; // Khai báo biến để quản lý DataTable
    

    // Phương thức tìm kiếm tồn kho
    $scope.searchTonKho = function () {
        // Kiểm tra xem người dùng đã chọn ngày chưa
        if ($scope.fromDate && $scope.toDate) {
            // Định dạng lại ngày thành chuỗi "yyyy-MM-dd" khi gửi API
            const fromDateFormatted = $scope.fromDate.toISOString().split('T')[0];
            const toDateFormatted = $scope.toDate.toISOString().split('T')[0];

            $scope.MaNhomTBH = '';
            if ($scope.ListMaNhomTBH != null && $scope.ListMaNhomTBH.length > 0) {
                for (var i = 0; i < $scope.ListMaNhomTBH.length; i++) {
                    if ($scope.MaNhomTBH == null || $scope.MaNhomTBH == '') {
                        $scope.MaNhomTBH = $scope.ListMaNhomTBH[i];
                    } else {
                        $scope.MaNhomTBH += ',' + $scope.ListMaNhomTBH[i];
                    }
                }
            }
            console.log($scope.MaNhomTBH);

            // Gọi API trực tiếp trong hàm searchTonKho
            $http.get('/KhoVatPham/GetTonKhoData', {
                params: {
                    fromDate: fromDateFormatted,
                    toDate: toDateFormatted,
                    maNhomTBH: $scope.MaNhomTBH
                }
            })
                .then(function (response) {
                    // Cập nhật dữ liệu vào bảng sau khi nhận kết quả từ API
                    console.log(response.data.data);
                    if (dataTable) {
                        // Xóa DataTable cũ nếu đã tồn tại
                        dataTable.clear().destroy();
                    }

                    //$('#BaoCaoTonKhoTable').DataTable().clear().rows.add(response.data.data).draw();
                    dataTable = $('#BaoCaoTonKhoTable').DataTable({
                        pageLength: 50, // Hiển thị mặc định 20 dòng
                        lengthMenu: [10, 20, 50, 100], // Các tùy chọn hiển thị số dòng
                        destroy: true, // Cho phép khởi tạo lại
                        responsive: true, // Đảm bảo giao diện đẹp trên mọi màn hình
                        columns: [
                            {
                                data: null, // Không sử dụng dữ liệu từ API
                                title: 'STT',
                                render: function (data, type, row, meta) {
                                    return meta.row + 1; // meta.row là chỉ số hàng, bắt đầu từ 0
                                },
                                className: 'text-center' // Căn giữa nếu cần
                            },
                            { data: 'TenSanPham', title: 'Tên vật phẩm' },
                            { data: 'DVT', title: 'ĐVT' },
                            { data: 'TonDauKy', title: 'Tồn kho đầu kỳ' },
                            { data: 'NhapKho', title: 'Nhập kho' },
                            { data: 'XuatKho', title: 'Xuất kho' },
                            { data: 'TonCuoiKy', title: 'Tồn kho cuối kỳ' }
                        ]
                    });

                    dataTable.clear().rows.add(response.data.data).draw();

                })
                .catch(function (error) {
                    console.error('Error fetching data:', error);
                });
        } else {
            alert('Vui lòng chọn Từ Ngày và Đến Ngày!');
        }
    };

    // Gọi tự động hàm searchTonKho khi load form
    $scope.searchTonKho();

    $scope.ExportExcel = function () {
       
        if ($scope.fromDate == null || $scope.fromDate == '') {
            toastr.error("Vui lòng chọn Từ ngày!");
            return;
        }

        if ($scope.toDate == null || $scope.toDate == '') {
            toastr.error("Vui lòng chọn Đến ngày!");
            return;
        }     
        $scope.MaNhomTBH = '';
        if ($scope.ListMaNhomTBH != null && $scope.ListMaNhomTBH.length > 0) {
            for (var i = 0; i < $scope.ListMaNhomTBH.length; i++) {
                if ($scope.MaNhomTBH == null || $scope.MaNhomTBH == '') {
                    $scope.MaNhomTBH = $scope.ListMaNhomTBH[i];
                } else {
                    $scope.MaNhomTBH += ',' + $scope.ListMaNhomTBH[i];
                }
            }
        }
        const fromDateFormatted = $scope.fromDate.toISOString().split('T')[0];
        const toDateFormatted = $scope.toDate.toISOString().split('T')[0];
        console.log('MaNhomTBH:');
        console.log($scope.MaNhomTBH);
        window.location.href = '/KhoVatPham/ExportData?fromDate=' + fromDateFormatted + '&toDate=' + toDateFormatted
                            + '&maNhomTBHs=' + ($scope.MaNhomTBH == undefined ? '' : $scope.MaNhomTBH);
    }

    $scope.Changecity = function () {

        var CityCodes = '';
        if ($scope.ListCityCode != null && $scope.ListCityCode.length > 0) {
            for (var i = 0; i < $scope.ListCityCode.length; i++) {
                if (CityCodes == null || CityCodes == '') {
                    CityCodes = $scope.ListCityCode[i];
                } else {
                    CityCodes += ',' + $scope.ListCityCode[i];
                }
            }
        }
        $scope.ListNhomTBH = [];
        $scope.ListMaNhomTBH = [];
        $.ajax({
            type: 'post',
            url: '/BaoCaoCD43/GetNhomTBHByMaNhomMap',
            cache: false,
            async: false,
            data: {
                CityCodes: CityCodes
            },
            success: function (respone) {
                $scope.ListNhomTBH = respone.NhomTBHs;
            }
        });

    };

    $scope.Changecity();

});
