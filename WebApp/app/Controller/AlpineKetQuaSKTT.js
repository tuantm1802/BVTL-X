document.addEventListener('alpine:init', () => {
    Alpine.data('alpineKetQuaSKTT', () => ({
        dataTable: null,
        init() {
            this.$nextTick(() => {
                const target = $('#dataTableKetQuaSKTT').length ? $('#dataTableKetQuaSKTT') : $('table.table-bordered');
                if (target.length) {
                    this.dataTable = target.DataTable({
                        lengthMenu: [10, 20, 30, 50, 60, 100],
                        ordering: false,
                        searching: true,
                        processing: true,
                        language: {
                            emptyTable: "Không có dữ liệu trong bảng",
                            info: "Hiển thị _START_ đến _END_ của _TOTAL_ bản ghi",
                            infoEmpty: "Hiển thị 0 đến 0 của 0 bản ghi",
                            infoFiltered: "(lọc từ _MAX_ tổng bản ghi)",
                            lengthMenu: "Hiển thị _MENU_ bản ghi",
                            loadingRecords: "Đang tải...",
                            search: "Tìm kiếm:",
                            zeroRecords: "Không tìm thấy kết quả",
                            paginate: {
                                first: "<<",
                                last: ">>",
                                next: ">",
                                previous: "<"
                            }
                        }
                    });
                }
            });
        }
    }));
});
