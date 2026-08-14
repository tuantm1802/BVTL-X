app.controller("KhachHangController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "kqslace_id";

    $scope.ListDuAn = [];
    var customerTable = null;
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
            url: '/KhachHang/GetBottomAction',
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

    $('#customerTable').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $scope.LoadPage = function (genTable) {
        showToast();

        $scope.ListKhachHang = [];
        if (genTable == 1 || genTable == 2) {
            if (genTable == 2)
                customerTable.destroy();

            //$.extend($.fn.dataTable.ext.classes, {
            //    sPageButton: 'dt-paging-button page-item'
            //});

            $('#customerTable').DataTable({
                processing: true,
                serverSide: true,
                
                ajax: {
                    url: '/KhachHang/GetCustomers',
                    type: 'POST',
                    dataType: 'json',
                    data: function (d) {                        
                        d.draw = d.draw;
                        d.start = d.start;
                        d.length = d.length;
                        d.search.value = $('#customerTable_filter input').val();
                    }
                },
                columns: [
                    //{ data: 'Id' },
                    { data: 'RecordId' },
                    { data: 'MaNhomTbh' },
                    {
                        data: 'NgayThangNamSinh',
                        render: function (data) {
                            return moment(data).isValid() ? moment(data).format('DD/MM/YYYY') : '';
                        }
                    },
                    {
                        data: 'GioiTinh',
                        render: function (data) {
                            switch (data) {
                                case '1': return 'Nam'
                                case '2': return 'Nữ'
                                case '3': return 'Khác'                         
                                default: return '';
                            }
                        }
                    },
                    {
                        data: 'CapBacHocVan',
                        render: function (data) {
                            switch (data) {
                                case '1': return 'Không đi học'
                                case '2': return 'Cấp I (Lớp 1 - lớp 5)'
                                case '3': return 'Cấp II (Lớp 6 - lớp 9)'
                                case '4': return 'Cấp III (Lớp 10 - lớp 12)'
                                case '5': return 'Trung cấp, cao đẳng, đại học'
                                case '6': return 'Sau đại học (thạc sĩ, tiến sĩ...)'
                                default: return '';
                            }
                        }
                    },
                    {
                        data: 'NgheNghiep',
                        render: function (data) {
                            switch (data) {
                                case '1': return 'Khu vực tư nhân';
                                case '2': return 'Khu vực Nhà nước';
                                case '3': return 'Kinh doanh';
                                case '4': return 'Lao động tình dục (mại dâm)';
                                case '5': return 'Học sinh/sinh viên';
                                case '6': return 'Lao động tự do';
                                case '7': return 'Không có việc làm/ nội trợ';
                                case '8': return 'Khác';
                                default: return '';
                            }
                        }
                    },
                    {
                        data: 'NgayHoi',
                        render: function (data) {
                            return moment(data).isValid() ? moment(data).format('DD/MM/YYYY') : '';
                        }
                    },                          
                    {
                        data: null,
                        className: 'text-center',
                        render: function (d, type, row) {
                            return `
                                    <button type="button" onclick="openCustomerDetailModal(${row.Id}, '${row.RecordId}')" class="btn btn-sm btn-primary mr-1" title="Xem nhanh Modal"><i class="fa fa-eye"></i> Xem nhanh</button>
                                    <a href="/KhachHang/Details/${row.Id}?recordid=${row.RecordId}" class="btn btn-sm btn-outline-success" title="Mở trang chi tiết"><i class="fa fa-external-link-alt"></i> Chi tiết</a>
                                    `;
                        }
                    }
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
            customerTable.ajax.reload();
        }
        hideLoading();
    };

    // Lắng nghe sự kiện keyup trên ô tìm kiếm nhanh
    $('#customerTable_filter input').unbind() // Hủy sự kiện tìm kiếm mặc định của DataTable
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
    

    $scope.ExportExcel = function () {
        window.location.href = '/KhachHang/ExportData?keyword=' + $scope.modelSearch.KeyWord;
    }

});
app.controller('CustomerDetailsController', ['$scope', '$http', '$location', function ($scope, $http, $location) {
    $scope.activeTab = 'basic-info';

    $scope.setTab = function (tab) {
        $scope.activeTab = tab;
    };
    $scope.customer = {};
    $scope.customerChuyenGui = {};
    $scope.customerSuDungChat = {};
    $scope.customerQHTD = {};
    $scope.customerChuyenGuiDichVu = {};
    $scope.customerSinhHoatNhom = {};
    $scope.customerPhieuTuVan = {};
    $scope.customerListPhieuTuVan = {};
    $scope.customerListKhamVaDieuTriSKTT = {};
    $scope.customerKhamVaDieuTri = {};
    

    $scope.init = function(id, recordid) {
        console.log("Init with ID:", id, "RecordID:", recordid);
        
        // Call API to get customer details
        $http.get('/KhachHang/GetCustomerDetails/' + id)
            .then(function (response) {
            console.log(response);

            if (response.data) {
                $scope.customer = response.data;
                // Đảm bảo rằng moment.js được thêm vào dự án trước khi sử dụng
                if ($scope.customer.NgayThangNamSinh) {
                    $scope.customer.NgayThangNamSinh = moment($scope.customer.NgayThangNamSinh).format('DD/MM/YYYY');
                }

                $scope.customer.gioiTinhText = $scope.customer.GioiTinh === '1' ? 'Nam' :
                    $scope.customer.GioiTinh === '2' ? 'Nữ' : 'Khác';

                $scope.customer.capBacHocVanText = $scope.customer.CapBacHocVan === '1' ? 'Không đi học' :
                    $scope.customer.CapBacHocVan === '2' ? 'Cấp I (Lớp 1 - lớp 5)' :
                        $scope.customer.CapBacHocVan === '3' ? 'Cấp II (Lớp 6 - lớp 9)' :
                            $scope.customer.CapBacHocVan === '4' ? 'Cấp III (Lớp 10 - lớp 12)' :
                                $scope.customer.CapBacHocVan === '5' ? 'Trung cấp, cao đẳng, đại học' :
                                    'Sau đại học (thạc sĩ, tiến sĩ...)';

                $scope.customer.ngheNghiepText = $scope.customer.NgheNghiep === '1' ? 'Khu vực tư nhân' :
                    $scope.customer.NgheNghiep === '2' ? 'Khu vực Nhà nước' :
                        $scope.customer.NgheNghiep === '3' ? 'Kinh doanh' :
                            $scope.customer.NgheNghiep === '4' ? 'Lao động tình dục' :
                                $scope.customer.NgheNghiep === '5' ? 'Học sinh/sinh viên' :
                                    $scope.customer.NgheNghiep === '6' ? 'Lao động tự do' :
                                        $scope.customer.NgheNghiep === '7' ? 'Không có việc làm/ nội trợ' : 'Khác';
            }

        }, function (error) {
            console.error('Error fetching customer details:', error);
        });

    $http.get('/KhachHang/GetCustomerChuyenGuiById/' + id)
        .then(function (response) {
            console.log(response);

            if (response.data) {
                $scope.customerChuyenGui = response.data;
                // Đảm bảo rằng moment.js được thêm vào dự án trước khi sử dụng
                if ($scope.customerChuyenGui.NgayXNTLVR) {
                    $scope.customerChuyenGui.NgayXNTLVR = moment($scope.customerChuyenGui.NgayXNTLVR).format('DD/MM/YYYY');
                }
            }
            

        }, function (error) {
            console.error('Error fetching customer details:', error);
        });

    $http.get('/KhachHang/GetCustomerSuDungChatById/' + id)
        .then(function (response) {
            console.log(response);
            $scope.customerSuDungChat = response.data;        
            
        }, function (error) {
            console.error('Error fetching customer details:', error);
        });

    $http.get('/KhachHang/GetCustomerQHTDById/' + id)
        .then(function (response) {
            console.log(response);
            $scope.customerQHTD = response.data;        
            
        }, function (error) {
            console.error('Error fetching customer details:', error);
        });

    
    $http.get('/KhachHang/GetDichVuChuyenGuiByCustomerId/' + id + '?recordid=' + recordid)
        .then(function (response) {
            console.log(response);
            $scope.customerChuyenGuiDichVu = response.data.DichVuChuyenGuiList;  
            
        }, function (error) {
            console.error('Error fetching customer details:', error);
        });

    $http.get('/KhachHang/GetSinhHoatNhomByCustomerId/' + id + '?recordid=' + recordid)
        .then(function (response) {
            console.log(response);
            $scope.customerSinhHoatNhom = response.data;

        }, function (error) {
            console.error('Error fetching customer details:', error);
        });

    $http.get('/KhachHang/GetPhieuTuVanCustomerId/' + id + '?recordid=' + recordid)
        .then(function (response) {
            console.log(response);
            $scope.customerPhieuTuVan = response.data;

        }, function (error) {
            console.error('Error fetching customer details:', error);
        });
        
    $http.get('/KhachHang/GetListPhieuTuVanCustomerId/' + id + '?recordid=' + recordid)
        .then(function (response) {
            console.log(response);
            $scope.customerListPhieuTuVan = response.data;

        }, function (error) {
            console.error('Error fetching customer details:', error);
        });

    $http.get('/KhachHang/GetKhamVaDieuTriCustomerId/' + id + '?recordid=' + recordid)
        .then(function (response) {
            console.log(response);
            $scope.customerKhamVaDieuTri = response.data;

        }, function (error) {
            console.error('Error fetching customer details:', error);
        });


        $http.get('/KhachHang/GetListKhamVaDieuTriSKTTCustomerId/' + id + '?recordid=' + recordid)
            .then(function (response) {
                console.log(response);
                $scope.customerListKhamVaDieuTriSKTT = response.data;

            }, function (error) {
                console.error('Error fetching customer details:', error);
            });
    };

    $scope.parseDate = function (dateString) {
        // Kiểm tra nếu dateString là null, undefined hoặc không phải chuỗi hợp lệ
        if (!dateString || typeof dateString !== "string" || !dateString.includes("/Date(")) {
            console.warn("Invalid dateString:", dateString); // Ghi log cảnh báo nếu giá trị không hợp lệ
            return null; // Trả về null khi giá trị không hợp lệ
        }

        try {
            // Chuyển đổi từ định dạng /Date(1718902800000)/ sang Date object
            var timestamp = parseInt(dateString.replace("/Date(", "").replace(")/", ""), 10);
            return new Date(timestamp); // Trả về đối tượng Date
        } catch (error) {
            console.error("Error parsing dateString:", dateString, error); // Ghi log lỗi nếu xảy ra lỗi
            return null; // Trả về null nếu lỗi xảy ra
        }
    };


}]);

//app.controller('CustomerDetailsController', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {

//    $scope.activeTab = 'basic-info';

//    $scope.setTab = function (tab) {
//        $scope.activeTab = tab;
//    };

//    $scope.customer = {};

//    $scope.getCustomerDetails = function (id) {
//        $http.get(`/KhachHang/GetCustomerDetails?id=${id}`)
//            .then(function (response) {
//                $scope.customer = response.data;
//                $scope.customer.gioiTinhText = $scope.customer.GioiTinh === 1 ? 'Nam' :
//                    $scope.customer.GioiTinh === 2 ? 'Nữ' : 'Khác';
//                $scope.customer.capBacHocVanText = $scope.customer.CapBacHocVan === 1 ? 'Không đi học' :
//                    $scope.customer.CapBacHocVan === 2 ? 'Cấp I (Lớp 1 - lớp 5)' :
//                        $scope.customer.CapBacHocVan === 3 ? 'Cấp II (Lớp 6 - lớp 9)' :
//                            $scope.customer.CapBacHocVan === 4 ? 'Cấp III (Lớp 10 - lớp 12)' :
//                                $scope.customer.CapBacHocVan === 5 ? 'Trung cấp, cao đẳng, đại học' :
//                                    'Sau đại học (thạc sĩ, tiến sĩ...)';
//                $scope.customer.ngheNghiepText = $scope.customer.NgheNghiep === 1 ? 'Khu vực tư nhân' :
//                    $scope.customer.NgheNghiep === 2 ? 'Khu vực Nhà nước' :
//                        $scope.customer.NgheNghiep === 3 ? 'Kinh doanh' :
//                            $scope.customer.NgheNghiep === 4 ? 'Lao động tình dục' :
//                                $scope.customer.NgheNghiep === 5 ? 'Học sinh/sinh viên' :
//                                    $scope.customer.NgheNghiep === 6 ? 'Lao động tự do' :
//                                        $scope.customer.NgheNghiep === 7 ? 'Không có việc làm/ nội trợ' : 'Khác';
//            });
//    };

//    // Lấy ID khách hàng từ URL
//    var customerId = itemId;
//    $scope.getCustomerDetails(customerId);
//});