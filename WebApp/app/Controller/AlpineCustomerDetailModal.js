window.openCustomerDetailModal = function (id, recordid) {
    if (!id) return;
    
    // Ensure modal container exists
    let container = document.getElementById('customerDetailModalContainer');
    if (!container) {
        container = document.createElement('div');
        container.id = 'customerDetailModalContainer';
        document.body.appendChild(container);
    }

    const modalHtml = `
    <div class="modal fade" id="modalCustomerDetail" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content border-0 shadow-lg rounded-lg" x-data="customerDetailModal(${id}, '${recordid}')" x-init="initData()">
                
                <!-- Modal Header -->
                <div class="modal-header bg-gradient-primary text-white py-3 px-4 align-items-center">
                    <div class="d-flex align-items-center">
                        <div class="p-2 bg-white-20 rounded-circle mr-3 text-white">
                            <i class="fas fa-id-card fa-lg"></i>
                        </div>
                        <div>
                            <h5 class="modal-title font-weight-bold text-white mb-0">Hồ sơ Chi tiết Khách hàng</h5>
                            <small class="text-white-50" x-text="'Mã Record ID: ' + (customer.RecordId || recordid)"></small>
                        </div>
                    </div>
                    <button type="button" class="close text-white opacity-80" data-dismiss="modal" aria-label="Close" style="outline: none;">
                        <span aria-hidden="true" style="font-size: 1.8rem;">&times;</span>
                    </button>
                </div>

                <!-- Modal Body -->
                <div class="modal-body p-4 bg-light">
                    
                    <!-- Loading State -->
                    <div class="text-center py-5" x-show="loading">
                        <div class="spinner-border text-primary mb-3" style="width: 3rem; height: 3rem;" role="status">
                            <span class="sr-only">Đang tải...</span>
                        </div>
                        <p class="text-muted font-weight-bold">Đang tải toàn bộ dữ liệu hồ sơ khách hàng...</p>
                    </div>

                    <div x-show="!loading" style="display: none;">
                        <!-- Top Profile Summary Card -->
                        <div class="card shadow-sm border-0 rounded-lg mb-4 bg-white">
                            <div class="card-body p-3">
                                <div class="row align-items-center">
                                    <div class="col-auto">
                                        <div class="avatar-circle-lg bg-primary text-white font-weight-bold d-flex align-items-center justify-content-center rounded-circle"
                                             style="width: 64px; height: 64px; font-size: 24px;"
                                             x-text="getInitials(customer.RecordId || recordid)">
                                        </div>
                                    </div>
                                    <div class="col">
                                        <h5 class="font-weight-bold text-gray-800 mb-1" x-text="'Khách hàng #' + (customer.RecordId || recordid)"></h5>
                                        <div class="d-flex flex-wrap align-items-center gap-2 text-muted small">
                                            <span class="badge badge-pill badge-primary px-3 py-1 mr-2" x-text="'Giới tính: ' + customer.gioiTinhText"></span>
                                            <span class="badge badge-pill badge-info px-3 py-1 mr-2" x-text="'Năm sinh: ' + (customer.NgayThangNamSinh || 'Chưa rõ')"></span>
                                            <span class="badge badge-pill badge-secondary px-3 py-1" x-text="'Nghề nghiệp: ' + customer.ngheNghiepText"></span>
                                        </div>
                                    </div>
                                    <div class="col-auto text-right">
                                        <a :href="'/KhachHang/Details/' + customerId + '?recordid=' + recordId" class="btn btn-outline-primary btn-sm font-weight-bold">
                                            <i class="fas fa-external-link-alt mr-1"></i>Mở trang toàn màn hình
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Tab Pill Navigation -->
                        <ul class="nav nav-pills nav-justified mb-4 bg-white p-2 rounded-lg shadow-sm border">
                            <li class="nav-item">
                                <a class="nav-link font-weight-bold py-2" :class="{'active bg-primary text-white': activeTab === 'basic-info'}" href="javascript:void(0)" x-on:click="activeTab = 'basic-info'">
                                    <i class="fas fa-user-circle mr-1"></i>I. Thông tin cơ bản
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link font-weight-bold py-2" :class="{'active bg-primary text-white': activeTab === 'hiv-arv-results'}" href="javascript:void(0)" x-on:click="activeTab = 'hiv-arv-results'">
                                    <i class="fas fa-vial mr-1"></i>II. Kết quả XN HIV & ARV
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link font-weight-bold py-2" :class="{'active bg-primary text-white': activeTab === 'screening-results'}" href="javascript:void(0)" x-on:click="activeTab = 'screening-results'">
                                    <i class="fas fa-clipboard-check mr-1"></i>III. Kết quả sàng lọc
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link font-weight-bold py-2" :class="{'active bg-primary text-white': activeTab === 'intervention'}" href="javascript:void(0)" x-on:click="activeTab = 'intervention'">
                                    <i class="fas fa-hands-helping mr-1"></i>IV. Can thiệp & Tư vấn
                                </a>
                            </li>
                        </ul>

                        <!-- Tab 1: Basic Info -->
                        <div x-show="activeTab === 'basic-info'" class="tab-pane fade show active">
                            <div class="row">
                                <div class="col-md-4 mb-3">
                                    <div class="card border-0 shadow-sm rounded-lg text-center p-3 h-100 bg-white">
                                        <img class="img-thumbnail rounded-circle mx-auto mb-3" style="width: 120px; height: 120px; object-fit: cover;" src="/Assest/img/undraw_profile.svg" alt="Avatar">
                                        <h6 class="font-weight-bold text-primary mb-1" x-text="customer.RecordId"></h6>
                                        <p class="text-muted small mb-0">Ảnh hồ sơ đăng ký</p>
                                    </div>
                                </div>
                                <div class="col-md-8 mb-3">
                                    <div class="card border-0 shadow-sm rounded-lg p-3 h-100 bg-white">
                                        <h6 class="font-weight-bold text-primary border-bottom pb-2 mb-3">Thông tin chi tiết đối tượng</h6>
                                        <div class="row">
                                            <div class="col-sm-6 mb-3">
                                                <label class="text-muted small mb-1">Mã khách hàng / Record ID</label>
                                                <div class="font-weight-bold text-dark" x-text="customer.RecordId || '—'"></div>
                                            </div>
                                            <div class="col-sm-6 mb-3">
                                                <label class="text-muted small mb-1">Ngày sinh</label>
                                                <div class="font-weight-bold text-dark" x-text="customer.NgayThangNamSinh || '—'"></div>
                                            </div>
                                            <div class="col-sm-6 mb-3">
                                                <label class="text-muted small mb-1">Giới tính</label>
                                                <div class="font-weight-bold text-dark" x-text="customer.gioiTinhText || '—'"></div>
                                            </div>
                                            <div class="col-sm-6 mb-3">
                                                <label class="text-muted small mb-1">Bậc học cao nhất</label>
                                                <div class="font-weight-bold text-dark" x-text="customer.capBacHocVanText || '—'"></div>
                                            </div>
                                            <div class="col-sm-12">
                                                <label class="text-muted small mb-1">Nghề nghiệp</label>
                                                <div class="font-weight-bold text-dark" x-text="customer.ngheNghiepText || '—'"></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Tab 2: HIV & ARV Results -->
                        <div x-show="activeTab === 'hiv-arv-results'" class="tab-pane fade show active">
                            <div class="card border-0 shadow-sm rounded-lg p-4 bg-white mb-3">
                                <h6 class="font-weight-bold text-primary border-bottom pb-2 mb-3"><i class="fas fa-heartbeat mr-2"></i>Kết quả Xét nghiệm HIV và Điều trị ARV</h6>
                                <div class="row">
                                    <div class="col-md-6 mb-3">
                                        <div class="p-3 bg-light rounded border">
                                            <label class="text-muted small mb-1">Kết quả XN nhanh HIV</label>
                                            <div class="h6 font-weight-bold text-danger mb-0" x-text="customer.KetQuaXNHiv || 'Chưa cập nhật'"></div>
                                        </div>
                                    </div>
                                    <div class="col-md-6 mb-3">
                                        <div class="p-3 bg-light rounded border">
                                            <label class="text-muted small mb-1">XN Khẳng định & Điều trị ARV</label>
                                            <div class="h6 font-weight-bold text-primary mb-0" x-text="customer.KetQuaChuyenGuiDieuTriARV || 'Chưa cập nhật'"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Tab 3: Screening Results -->
                        <div x-show="activeTab === 'screening-results'" class="tab-pane fade show active">
                            <div class="row">
                                <div class="col-md-6 mb-3">
                                    <div class="card border-0 shadow-sm rounded-lg p-3 bg-white h-100">
                                        <h6 class="font-weight-bold text-primary border-bottom pb-2 mb-2">1. Sử dụng chất</h6>
                                        <table class="table table-sm table-borderless">
                                            <tr><td class="text-muted">Các chất Chemsex (3 tháng):</td><td class="font-weight-bold" x-text="customerSuDungChat.ChatSuDungChemsex3Thang || '—'"></td></tr>
                                            <tr><td class="text-muted">Chất dùng thường xuyên nhất:</td><td class="font-weight-bold" x-text="customerSuDungChat.ChatSuDungThuongXuyenNhat || '—'"></td></tr>
                                            <tr><td class="text-muted">Tần suất sử dụng:</td><td class="font-weight-bold" x-text="customerSuDungChat.TanSuatSuDungMTDChemsex3Thang || '—'"></td></tr>
                                            <tr><td class="text-muted">Sử dụng đa chất:</td><td class="font-weight-bold" x-text="customerSuDungChat.SuDungDaChat || '—'"></td></tr>
                                        </table>
                                    </div>
                                </div>
                                <div class="col-md-6 mb-3">
                                    <div class="card border-0 shadow-sm rounded-lg p-3 bg-white h-100">
                                        <h6 class="font-weight-bold text-primary border-bottom pb-2 mb-2">2. Quan hệ tình dục (QHTD)</h6>
                                        <table class="table table-sm table-borderless">
                                            <tr><td class="text-muted">Đối tượng QHTD:</td><td class="font-weight-bold" x-text="customerQHTD.DoiTuongQuanHe || '—'"></td></tr>
                                            <tr><td class="text-muted">Tình trạng mối quan hệ:</td><td class="font-weight-bold" x-text="customerQHTD.TinhTrangMoiQHHT || '—'"></td></tr>
                                            <tr><td class="text-muted">Tần suất dùng BCS (3 tháng):</td><td class="font-weight-bold" x-text="customerQHTD.TanSuatSuDungBCSChemsex3Thang || '—'"></td></tr>
                                            <tr><td class="text-muted">QHTD tập thể (3 tháng):</td><td class="font-weight-bold" x-text="customerQHTD.QHTDTT3Thang || '—'"></td></tr>
                                        </table>
                                    </div>
                                </div>
                                <div class="col-md-6 mb-3">
                                    <div class="card border-0 shadow-sm rounded-lg p-3 bg-white h-100">
                                        <h6 class="font-weight-bold text-primary border-bottom pb-2 mb-2">3. STIs & Viêm gan C</h6>
                                        <table class="table table-sm table-borderless">
                                            <tr><td class="text-muted">Tình trạng mắc STIs:</td><td class="font-weight-bold" x-text="customerQHTD.STIs || '—'"></td></tr>
                                            <tr><td class="text-muted">Tình trạng mắc VGC:</td><td class="font-weight-bold" x-text="customerQHTD.TinhTrangViemGanC || '—'"></td></tr>
                                        </table>
                                    </div>
                                </div>
                                <div class="col-md-6 mb-3">
                                    <div class="card border-0 shadow-sm rounded-lg p-3 bg-white h-100">
                                        <h6 class="font-weight-bold text-primary border-bottom pb-2 mb-2">4. Đánh giá QST & ACE</h6>
                                        <table class="table table-sm table-borderless">
                                            <tr><td class="text-muted">QST Đã từng cố tự sát:</td><td class="font-weight-bold" x-text="customerQHTD.QSTCoTuSat || '—'"></td></tr>
                                            <tr><td class="text-muted">Điểm QST:</td><td class="font-weight-bold" x-text="customerQHTD.QSTTongDiem || '—'"></td></tr>
                                            <tr><td class="text-muted">Số lượng chỉ số ACE:</td><td class="font-weight-bold" x-text="customerQHTD.ACESoLuong || '—'"></td></tr>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Tab 4: Intervention & Counseling -->
                        <div x-show="activeTab === 'intervention'" class="tab-pane fade show active">
                            <!-- Transfer Services -->
                            <div class="card border-0 shadow-sm rounded-lg p-3 bg-white mb-3">
                                <h6 class="font-weight-bold text-primary border-bottom pb-2 mb-2"><i class="fas fa-exchange-alt mr-2"></i>Chuyển gửi Dịch vụ</h6>
                                <div class="table-responsive">
                                    <table class="table table-sm table-striped mb-0">
                                        <thead>
                                            <tr><th>#</th><th>Dịch vụ</th><th>Ngày khám</th><th>Lần khám</th><th>TLVR</th></tr>
                                        </thead>
                                        <tbody>
                                            <template x-for="(dv, idx) in customerChuyenGuiDichVu" :key="idx">
                                                <tr>
                                                    <td x-text="idx + 1"></td>
                                                    <td x-text="dv.DichVu || '—'"></td>
                                                    <td x-text="formatDate(dv.NgayKham)"></td>
                                                    <td x-text="dv.LanKham || '—'"></td>
                                                    <td x-text="dv.TaiLuongVR || '—'"></td>
                                                </tr>
                                            </template>
                                            <template x-if="!customerChuyenGuiDichVu || customerChuyenGuiDichVu.length === 0">
                                                <tr><td colspan="5" class="text-center text-muted font-italic">Không có dữ liệu chuyển gửi dịch vụ</td></tr>
                                            </template>
                                        </tbody>
                                    </table>
                                </div>
                            </div>

                            <!-- Phieu Tu Van -->
                            <div class="card border-0 shadow-sm rounded-lg p-3 bg-white mb-3">
                                <h6 class="font-weight-bold text-primary border-bottom pb-2 mb-2"><i class="fas fa-comments mr-2"></i>Chi tiết các lần Tư vấn</h6>
                                <div class="table-responsive">
                                    <table class="table table-sm table-striped mb-0">
                                        <thead>
                                            <tr><th>#</th><th>Ngày tư vấn</th><th>Hành vi nguy cơ</th><th>Sức khỏe tâm thần</th><th>Nội dung giảm hại</th></tr>
                                        </thead>
                                        <tbody>
                                            <template x-for="(tv, idx) in (customerListPhieuTuVan || [])" :key="idx">
                                                <tr>
                                                    <td x-text="'Lần ' + (idx + 1)"></td>
                                                    <td x-text="formatDate(tv.NgayTuVan)"></td>
                                                    <td x-text="tv.HanhViNguyCo || '—'"></td>
                                                    <td x-text="tv.SucKhoeTamThan || '—'"></td>
                                                    <td x-text="tv.TuVanGiamHai || '—'"></td>
                                                </tr>
                                            </template>
                                            <template x-if="!customerListPhieuTuVan || customerListPhieuTuVan.length === 0">
                                                <tr><td colspan="5" class="text-center text-muted font-italic">Không có dữ liệu tư vấn</td></tr>
                                            </template>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>

                <!-- Modal Footer -->
                <div class="modal-footer bg-white py-2 px-4 border-top">
                    <button type="button" class="btn btn-secondary px-4 font-weight-bold" data-dismiss="modal">Đóng</button>
                </div>
            </div>
        </div>
    </div>
    `;

    container.innerHTML = modalHtml;
    const $modal = $('#modalCustomerDetail');
    $modal.modal('show');
};

document.addEventListener('alpine:init', () => {
    Alpine.data('customerDetailModal', (id, recordid) => ({
        customerId: id,
        recordId: recordid,
        loading: true,
        activeTab: 'basic-info',
        customer: {},
        customerChuyenGui: {},
        customerSuDungChat: {},
        customerQHTD: {},
        customerChuyenGuiDichVu: [],
        customerSinhHoatNhom: {},
        customerPhieuTuVan: {},
        customerListPhieuTuVan: [],
        customerListKhamVaDieuTriSKTT: [],
        customerKhamVaDieuTri: {},

        async initData() {
            this.loading = true;
            try {
                const [
                    resDetails, resChuyenGui, resChat, resQhtd,
                    resDichVu, resShn, resTv, resListTv, resListSktt, resSktt
                ] = await Promise.all([
                    fetch('/KhachHang/GetCustomerDetails/' + this.customerId).then(r => r.json()).catch(() => ({})),
                    fetch('/KhachHang/GetCustomerChuyenGuiById/' + this.customerId).then(r => r.json()).catch(() => ({})),
                    fetch('/KhachHang/GetCustomerSuDungChatById/' + this.customerId).then(r => r.json()).catch(() => ({})),
                    fetch('/KhachHang/GetCustomerQHTDById/' + this.customerId).then(r => r.json()).catch(() => ({})),
                    fetch('/KhachHang/GetDichVuChuyenGuiByCustomerId/' + this.customerId + '?recordid=' + this.recordId).then(r => r.json()).catch(() => ({})),
                    fetch('/KhachHang/GetSinhHoatNhomByCustomerId/' + this.customerId + '?recordid=' + this.recordId).then(r => r.json()).catch(() => ({})),
                    fetch('/KhachHang/GetPhieuTuVanCustomerId/' + this.customerId + '?recordid=' + this.recordId).then(r => r.json()).catch(() => ({})),
                    fetch('/KhachHang/GetListPhieuTuVanCustomerId/' + this.customerId + '?recordid=' + this.recordId).then(r => r.json()).catch(() => ([])),
                    fetch('/KhachHang/GetListKhamVaDieuTriSKTTCustomerId/' + this.customerId + '?recordid=' + this.recordId).then(r => r.json()).catch(() => ([])),
                    fetch('/KhachHang/GetKhamVaDieuTriCustomerId/' + this.customerId + '?recordid=' + this.recordId).then(r => r.json()).catch(() => ({}))
                ]);

                this.customer = resDetails || {};
                if (this.customer.NgayThangNamSinh && window.moment) {
                    this.customer.NgayThangNamSinh = moment(this.customer.NgayThangNamSinh).format('DD/MM/YYYY');
                }
                this.customer.gioiTinhText = this.customer.GioiTinh === '1' ? 'Nam' : (this.customer.GioiTinh === '2' ? 'Nữ' : 'Khác');
                this.customer.capBacHocVanText = this.customer.CapBacHocVan === '1' ? 'Không đi học' :
                    (this.customer.CapBacHocVan === '2' ? 'Cấp I' : (this.customer.CapBacHocVan === '3' ? 'Cấp II' : (this.customer.CapBacHocVan === '4' ? 'Cấp III' : 'Trung cấp/Cao đẳng/Đại học')));
                this.customer.ngheNghiepText = this.customer.NgheNghiep === '1' ? 'Khu vực tư nhân' :
                    (this.customer.NgheNghiep === '2' ? 'Khu vực Nhà nước' : (this.customer.NgheNghiep === '3' ? 'Kinh doanh' : (this.customer.NgheNghiep === '4' ? 'Lao động tình dục' : 'Khác')));

                this.customerChuyenGui = resChuyenGui || {};
                this.customerSuDungChat = resChat || {};
                this.customerQHTD = resQhtd || {};
                this.customerChuyenGuiDichVu = (resDichVu && resDichVu.DichVuChuyenGuiList) ? resDichVu.DichVuChuyenGuiList : [];
                this.customerSinhHoatNhom = resShn || {};
                this.customerPhieuTuVan = resTv || {};
                this.customerListPhieuTuVan = Array.isArray(resListTv) ? resListTv : [];
                this.customerListKhamVaDieuTriSKTT = Array.isArray(resListSktt) ? resListSktt : [];
                this.customerKhamVaDieuTri = resSktt || {};

            } catch (err) {
                console.error("Load customer details failed:", err);
            } finally {
                this.loading = false;
            }
        },

        getInitials(name) {
            if (!name) return 'KH';
            return name.substring(0, 2).toUpperCase();
        },

        formatDate(dateStr) {
            if (!dateStr) return '—';
            if (dateStr.includes('/Date(')) {
                const ts = parseInt(dateStr.replace('/Date(', '').replace(')/', ''), 10);
                if (!isNaN(ts)) {
                    const d = new Date(ts);
                    return d.toLocaleDateString('vi-VN');
                }
            }
            if (window.moment && moment(dateStr).isValid()) {
                return moment(dateStr).format('DD/MM/YYYY');
            }
            return dateStr;
        }
    }));
});
