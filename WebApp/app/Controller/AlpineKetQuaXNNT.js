document.addEventListener('alpine:init', function () {
    Alpine.data('alpineKetQuaXNNT', function () {
        return {
            dataTable: null,
            keyword: '',
            init: function () {
                var self = this;
                this.$nextTick(function () {
                    if ($('#dataTableKetQuaXNNT').length) {
                        self.dataTable = $('#dataTableKetQuaXNNT').DataTable({
                            lengthMenu: [15, 25, 50, 100],
                            pageLength: 25,
                            ordering: true,
                            searching: true,
                            processing: true,
                            language: {
                                emptyTable: "Không có dữ liệu trong bảng",
                                info: "Hiển thị _START_ đến _END_ của _TOTAL_ bản ghi",
                                infoEmpty: "Hiển thị 0 đến 0 của 0 bản ghi",
                                infoFiltered: "(lọc từ _MAX_ tổng bản ghi)",
                                lengthMenu: "Hiển thị _MENU_ bản ghi",
                                loadingRecords: "Đang tải...",
                                search: "Tìm kiếm nhanh:",
                                zeroRecords: "Không tìm thấy kết quả phù hợp",
                                paginate: {
                                    first: "«",
                                    last: "»",
                                    next: "›",
                                    previous: "‹"
                                }
                            }
                        });
                    }
                });
            },
            searchTable: function () {
                if (this.dataTable) {
                    this.dataTable.search(this.keyword).draw();
                }
            },
            exportExcel: function () {
                var searchVal = this.keyword ? encodeURIComponent(this.keyword) : '';
                window.location.href = '/KetQuaXNNT/ExportData?keyword=' + searchVal;
            }
        };
    });
});
