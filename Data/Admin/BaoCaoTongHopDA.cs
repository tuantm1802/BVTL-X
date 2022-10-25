using log4net;
using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Data.InterfaceDA.Admin;
using Model.ModelExtend.Base;
using Common.Common;
using Common.ICommon;
using System.Data.SqlClient;
using Model.ModelExtend.Report;
using Model.ModelExtend;

namespace Data.Admin
{
    public class BaoCaoTongHopDA : IBaoCaoTongHopDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        IDatabaseSql _DatabaseSql = new DatabaseSql();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Lấy báo cáo
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<BaoCaoModel> GetDataReport(ReportSearchModel modelSearch)
        {
            var result = new List<BaoCaoModel>();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("Months", string.IsNullOrEmpty(modelSearch.Months) ? DBNull.Value : (object)modelSearch.Months),
                    new SqlParameter("Year", modelSearch.Year == null ? 0 : (object)modelSearch.Year),
                    new SqlParameter("CityCodes", string.IsNullOrEmpty(modelSearch.CityCodes) ? DBNull.Value : (object)modelSearch.CityCodes),
                    new SqlParameter("TypeReport", modelSearch.TypeReport)
                };
                result = _DatabaseSql.ExecuteProcToList<BaoCaoModel>(Constants.SP_Report_Get_All_Data, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "BaoCaoTongHopDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy tổng hợp báo cáo theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<BaoCaoModel>();
            }
            return result;
        }


        /// <summary>
        /// Lấy dữ liệu báo cáo tổng hợp theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<BaoCaoTongHopPageModel> GetBaoCaoTongHopByPage(ModelSearch modelSearch)
        {
            var result = new List<BaoCaoTongHopPageModel>();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("Keyword", string.IsNullOrEmpty(modelSearch.KeyWord) ? DBNull.Value : (object)modelSearch.KeyWord),//System.Data.SqlDbType.NVarChar,250,
                    new SqlParameter("OrderByName", modelSearch.SortColumn),
                    new SqlParameter("Months", string.IsNullOrEmpty(modelSearch.Months) ? DBNull.Value : (object)modelSearch.Months),
                    new SqlParameter("Year", modelSearch.Year == null ? 0 : (object)modelSearch.Year),
                    new SqlParameter("CityCodes", string.IsNullOrEmpty(modelSearch.CityCodes) ? DBNull.Value : (object)modelSearch.CityCodes),
                    new SqlParameter("Page", modelSearch.currentPage),
                    new SqlParameter("PageSize", modelSearch.pageSize)
                };
                result = _DatabaseSql.ExecuteProcToList<BaoCaoTongHopPageModel>(Constants.SP_BaoCaoTongHop_Get_By_Page, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "BaoCaoTongHopDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách dữ liệu báo cáo tổng hợp theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<BaoCaoTongHopPageModel>();
            }
            return result;
        }

        /// <summary>
        /// Lấy báo cáo tổng hợp theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaoCaoTongHopPageModel GetItemById(int id)
        {
            var result = new BaoCaoTongHopPageModel();

            var param = new List<SqlParameter>
                {
                    new SqlParameter("Id", id),
                };
            var resultPro = _DatabaseSql.ExecuteProcToList<BaoCaoTongHopPageModel>(Constants.SP_BaoCaoTongHop_Get_By_Id, param).ToList();
            if (resultPro != null && resultPro.Count > 0)
                result = resultPro.FirstOrDefault();
            return result;
        }


        /// <summary>
        /// Lấy dữ liệu kết quả sáng lọc
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public void KetQuaSangLoc(ReportSearchModel modelSearch, ref List<KetQuaSangLocModel> DoiTuongKHs, ref List<KetQuaSangLocModel> GioiTinhs, ref List<KetQuaSangLocModel> Tuois
            , ref List<KetQuaSangLocModel> KetQuaHIVs, ref List<KetQuaSangLocModel> ChatGayNghien3Thangs, ref List<KetQuaSangLocModel> SoChatGayNghiens, ref List<KetQuaSangLocModel> ChatGayNghienSDThuongXuyens
            , ref List<DuongSuDungMaTuyDaModel> DuongSDMaTuyDas, ref List<TanSuatSuDungMaTuyDaModel> TanSuatSDMaTuyDas, ref List<LanDauSuDungMaTuyDaModel> LanDauSDMaTuyDas, ref List<LoaiMaTuyDaSuDungDTModel> LoaiMaTuyDaSDDauTiens
            , ref List<NguyCoKhiSDMTDModel> NguyCoSDMaTuyDas, ref List<ChungBKTModel> ChungBKTs, ref List<NguyCoTinhDucModel> NguyCoTinhDucs, ref List<DungBCSModel> DungBCSs
            , ref List<SuDungMTDKhiQHTDModel> SDMaTuyDaKhiQHTDs, ref List<SuDungMTDKhiQHTDModel> QHTDTapThes, ref List<SuDungMTDKhiQHTDModel> BanDams, ref List<NhieuNguyCoTinhDucModel> NhieuNguycoTinhDucs
            , ref List<BenhSTIModel> BenhSTIs, ref List<BenhLao_VGCModel> BenhLaos, ref List<BenhLao_VGCModel> BenhVGCs, ref List<KetQuaSangLocModel> SocHeroins
            , ref List<KetQuaSangLocModel> SocMeths, ref List<CacChatGayNghienModel> CacLoaiChatGayNghiens, ref List<KetQuaSangLocModel> KetQuaQSTs, ref List<MucDoGapVanDeSKTTModel> MucDoGapVanDeSKTTs
            , ref List<SuDungMTDKhiQHTDModel> TuLamHaiBanThans, ref List<SuDungMTDKhiQHTDModel> CoTuSats, ref List<LoanThanModel> LoanThans)
        {
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("FromDate", string.IsNullOrEmpty(modelSearch.FromDate) ? DBNull.Value : (object)Convert.ToInt32(modelSearch.FromDate)),
                    new SqlParameter("ToDate",string.IsNullOrEmpty(modelSearch.ToDate) ? DBNull.Value : (object)Convert.ToInt32(modelSearch.ToDate)),
                    new SqlParameter("CityCodes", string.IsNullOrEmpty(modelSearch.CityCodes) ? DBNull.Value : (object)modelSearch.CityCodes),
                    new SqlParameter("MaNhomTBH",  string.IsNullOrEmpty(modelSearch.MaNhomTBH) ? DBNull.Value : (object)modelSearch.MaNhomTBH),
                     new SqlParameter("FromSttKhachHang", modelSearch.TuSoMaKH == null ? 0 : modelSearch.TuSoMaKH),
                    new SqlParameter("ToSttKhachHang", modelSearch.DenSoMaKH == null ? 0 : modelSearch.DenSoMaKH)
                };
                var ds = _DatabaseSql.ExecuteProcDataSet(Constants.SP_Report_Ket_Qua_Sang_Loc, param);
                if (ds != null && ds.Tables.Count > 0)
                {
                    ////////////////////Báo cáo theo đối tượng////
                    var bcDoiTuong = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[0]);
                    if (bcDoiTuong != null && bcDoiTuong.Count > 0)
                        ChuyenDoi_BCTheoDoiTuong_GioiTinh(bcDoiTuong, ref DoiTuongKHs);

                    ////////////////////Báo cáo theo giới tính////
                    var bcGioiTinh = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[1]);
                    if (bcGioiTinh != null && bcGioiTinh.Count > 0)
                        ChuyenDoi_BCTheoDoiTuong_GioiTinh(bcGioiTinh, ref GioiTinhs);

                    ////////////////////Báo cáo theo tuổi ////
                    var bcTuoi = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[2]);
                    if (bcTuoi != null && bcTuoi.Count > 0)
                        ChuyenDoi_BCTheoTuoi(bcTuoi, ref Tuois);


                    ////////////////////Báo cáo theo kết quả HIV ////
                    var bcKetQuaHIV = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[3]);
                    if (bcKetQuaHIV != null && bcKetQuaHIV.Count > 0)
                        ChuyenDoi_BCTheoKetQuaHIV(bcKetQuaHIV, ref KetQuaHIVs);

                    ////////////////////Báo cáo theo Chất gây nghiện sử dụng trong 3 tháng gần đây////
                    var bcCGNSuDung3T = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[4]);
                    if (bcCGNSuDung3T != null && bcCGNSuDung3T.Count > 0)
                        ChuyenDoi_BCTheoDoiTuong_GioiTinh(bcCGNSuDung3T, ref ChatGayNghien3Thangs);


                    ////////////////////Báo cáo theo Số chất gây nghiện////
                    var bcSoCGN = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[5]);
                    if (bcSoCGN != null && bcSoCGN.Count > 0)
                        ChuyenDoi_BCTheoSoChatGayNghien(bcSoCGN, ref SoChatGayNghiens);

                    ////////////////////Báo cáo theo Loại chất gây nghiện sử dụng thường xuyên nhất trong 3 tháng gần đây////
                    var bcChatGayNghienSDThuongXuyen = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[6]);
                    if (bcChatGayNghienSDThuongXuyen != null && bcChatGayNghienSDThuongXuyen.Count > 0)
                        ChuyenDoi_BCTheoDoiTuong_GioiTinh(bcChatGayNghienSDThuongXuyen, ref ChatGayNghienSDThuongXuyens);

                    ////////////////////Báo cáo theo Đường sử dụng ma túy đá////
                    var bcDuongSDMaTuyDas = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[7]);
                    if (bcDuongSDMaTuyDas != null && bcDuongSDMaTuyDas.Count > 0)
                        ChuyenDoi_BCTheoDuongSDMaTuyDa(bcDuongSDMaTuyDas, ref DuongSDMaTuyDas);

                    ////////////////////Báo cáo theo Tần suất sử dụng ma túy đá trong 3 tháng gần đây////
                    var bcTanSuatSDMaTuyDa = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[8]);
                    if (bcTanSuatSDMaTuyDa != null && bcTanSuatSDMaTuyDa.Count > 0)
                        ChuyenDoi_BCTheoTanSuatSDMaTuyDa(bcTanSuatSDMaTuyDa, ref TanSuatSDMaTuyDas);

                    ////////////////////Báo cáo theo Lần đầu SD ma tuý ////
                    var bcLanDauSDMaTuyDa = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[9]);
                    if (bcLanDauSDMaTuyDa != null && bcLanDauSDMaTuyDa.Count > 0)
                        ChuyenDoi_BCTheoLanDauSDMaTuyDa(bcLanDauSDMaTuyDa, ref LanDauSDMaTuyDas);

                    ////////////////////Báo cáo theo Loại ma túy SD đầu tiên////
                    var bcLoaiMaTuyDaSDDauTien = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[10]);
                    if (bcLoaiMaTuyDaSDDauTien != null && bcLoaiMaTuyDaSDDauTien.Count > 0)
                        ChuyenDoi_BCTheoLoaiMaTuyDaSDDauTien(bcLoaiMaTuyDaSDDauTien, ref LoaiMaTuyDaSDDauTiens);

                    ////////////////////Báo cáo theo Nguy cơ khi SD ma túy////
                    var bcNguyCoKhiSDMTD = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[11]);
                    if (bcNguyCoKhiSDMTD != null && bcNguyCoKhiSDMTD.Count > 0)
                        ChuyenDoi_BCTheoNguyCoKhiSDMTD(bcNguyCoKhiSDMTD, ref NguyCoSDMaTuyDas);

                    ////////////////////Báo cáo theo Chung BKT////
                    var bcChungBKT = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[12]);
                    if (bcChungBKT != null && bcChungBKT.Count > 0)
                        ChuyenDoi_BCTheoChungBKT(bcChungBKT, ref ChungBKTs);

                    ////////////////////Báo cáo theo Nguy cơ tình dục////
                    var bcNguyCoTinhDuc = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[13]);
                    if (bcNguyCoTinhDuc != null && bcNguyCoTinhDuc.Count > 0)
                        ChuyenDoi_BCTheoNguyCoTinhDuc(bcNguyCoTinhDuc, ref NguyCoTinhDucs);

                    ////////////////////Báo cáo theo dùng BCS////
                    var bcDungBCS = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[14]);
                    if (bcDungBCS != null && bcDungBCS.Count > 0)
                        ChuyenDoi_BCTheoDungBCS(bcDungBCS, ref DungBCSs);

                    ////////////////////Báo cáo theo SD ma túy khi QHTD ////
                    var bcSDMaTuyDaKhiQHTD = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[15]);
                    if (bcSDMaTuyDaKhiQHTD != null && bcSDMaTuyDaKhiQHTD.Count > 0)
                        ChuyenDoi_BCTheoSDMaTuyDaKhiQHTD(bcSDMaTuyDaKhiQHTD, ref SDMaTuyDaKhiQHTDs);

                    ////////////////////Báo cáo theo QHTD tập thể ////
                    var bcQHTDTapThes = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[16]);
                    if (bcQHTDTapThes != null && bcQHTDTapThes.Count > 0)
                        ChuyenDoi_BCTheoSDMaTuyDaKhiQHTD(bcQHTDTapThes, ref QHTDTapThes);

                    ////////////////////Báo cáo theo Bán dâm////
                    var bcBanDams = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[17]);
                    if (bcBanDams != null && bcBanDams.Count > 0)
                        ChuyenDoi_BCTheoSDMaTuyDaKhiQHTD(bcBanDams, ref BanDams);

                    ////////////////////Báo cáo theo Nhiều nguy cơ tình dục(Qhđồng giới, ko thường xuyên sử dụng BCS, QH tập thể, QH khi sd ma túy, bán dâm) ////
                    var bcNhieuNguycoTinhDuc = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[18]);
                    if (bcNhieuNguycoTinhDuc != null && bcNhieuNguycoTinhDuc.Count > 0)
                        ChuyenDoi_BCTheoNhieuNguycoTinhDuc(bcNhieuNguycoTinhDuc, ref NhieuNguycoTinhDucs);

                    ////////////////////Báo cáo theo Bệnh tật STI////
                    var bcBenhSTI = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[19]);
                    if (bcBenhSTI != null && bcBenhSTI.Count > 0)
                        ChuyenDoi_BCTheoBenhSTI(bcBenhSTI, ref BenhSTIs);

                    ////////////////////Báo cáo theo Lao ////
                    var bcBenhLao = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[20]);
                    if (bcBenhLao != null && bcBenhLao.Count > 0)
                        ChuyenDoi_BCTheoBenhLao_VGC(bcBenhLao, ref BenhLaos);

                    ////////////////////Báo cáo theo VGC ////
                    var bcBenhVGC = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[21]);
                    if (bcBenhVGC != null && bcBenhVGC.Count > 0)
                        ChuyenDoi_BCTheoBenhLao_VGC(bcBenhVGC, ref BenhVGCs);


                    ////////////////////Báo cáo theo Sốc thuốc Heroin ////
                    var bcSocHeroin = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[22]);
                    if (bcSocHeroin != null && bcSocHeroin.Count > 0)
                        ChuyenDoi_BCTheoSocHeroins(bcSocHeroin, ref SocHeroins);


                    ////////////////////Báo cáo theo Sốc thuốc Meth ////
                    var bcSocMeth = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[23]);
                    if (bcSocMeth != null && bcSocMeth.Count > 0)
                        ChuyenDoi_BCTheoSocMeth(bcSocMeth, ref SocMeths);


                    ////////////////////Báo cáo theo CÁC LOẠI CHẤT GÂY NGHIỆN\KẾT QUẢ ASSIST////
                    var bcCacLoaiChatGayNghien = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[24]);
                    if (bcCacLoaiChatGayNghien != null && bcCacLoaiChatGayNghien.Count > 0)
                        ChuyenDoi_BCTheoCacLoaiChatGayNghien(bcCacLoaiChatGayNghien, ref CacLoaiChatGayNghiens);

                    ////////////////////Báo cáo theo KẾT QUẢ QST(SỨC KHỎE TÂM THẦN) ////
                    var bcKetQuaQST = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[25]);
                    if (bcKetQuaQST != null && bcKetQuaQST.Count > 0)
                        ChuyenDoi_BCTheoKetQuaQST(bcKetQuaQST, ref KetQuaQSTs);

                    ////////////////////Báo cáo theo Sàng lọc SKTT Mức độ gặp các vấn đề SKTT trong 2 tuần qua(câu 1)////
                    var bcMucDoGapVanDeSKTT = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[26]);
                    if (bcMucDoGapVanDeSKTT != null && bcMucDoGapVanDeSKTT.Count > 0)
                        ChuyenDoi_BCTheoMucDoGapVanDeSKTT(bcMucDoGapVanDeSKTT, ref MucDoGapVanDeSKTTs);

                    ////////////////////Báo cáo theo Sàng lọc SKTT Ý nghĩ tự làm hại bản thân trong 2 tuần qua(câu 2) ////
                    var bcTuLamHaiBanThan = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[27]);
                    if (bcTuLamHaiBanThan != null && bcTuLamHaiBanThan.Count > 0)
                        ChuyenDoi_BCTheoSDMaTuyDaKhiQHTD(bcTuLamHaiBanThan, ref TuLamHaiBanThans);

                    ////////////////////Báo cáo theo Sàng lọc SKTT Cố tự sát từ trước đến nay(câu 3)////
                    var bcCoTuSat = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[28]);
                    if (bcCoTuSat != null && bcCoTuSat.Count > 0)
                        ChuyenDoi_BCTheoSDMaTuyDaKhiQHTD(bcCoTuSat, ref CoTuSats);

                    ////////////////////Báo cáo theo Loạn thần////
                    var bcLoanThan = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[29]);
                    if (bcLoanThan != null && bcLoanThan.Count > 0)
                        ChuyenDoi_BCTheoLoanThan(bcLoanThan, ref LoanThans);
                }
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "BaoCaoTongHopDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy tổng hợp báo cáo theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
            }
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo loạn thần
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoLoanThan(List<KetQuaSangLocProModel> modelInputs, ref List<LoanThanModel> outDatas)
        {
            var tongSo = 0;

            var soLuongTheoDoiRinhRap = 0;
            var soLuongYNghi = 0;
            var soLuongNgheThuMaNKKNT = 0;

            // Có
            var co = new LoanThanModel
            {
                NoiDung = "Có",
            };

            // Không
            var khong = new LoanThanModel
            {
                NoiDung = "Không",
            };
            // Cau c_4a
            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_4a)).Sum(x => x.SoLuong);
            soLuongTheoDoiRinhRap = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_4a) && x.c_4a.Contains("Có")).Sum(x => x.SoLuong);
            co.TheoDoiRinhRap_SoLuong = soLuongTheoDoiRinhRap;


            co.TheoDoiRinhRap_PhanTram = TinhPhanTram(soLuongTheoDoiRinhRap, tongSo);

            soLuongTheoDoiRinhRap = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_4a) && x.c_4a.Contains("Không")).Sum(x => x.SoLuong);
            khong.TheoDoiRinhRap_SoLuong = soLuongTheoDoiRinhRap;


            khong.TheoDoiRinhRap_PhanTram = TinhPhanTram(soLuongTheoDoiRinhRap, tongSo);


            // Cau c_4b
            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_4b)).Sum(x => x.SoLuong);
            soLuongTheoDoiRinhRap = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_4b) && x.c_4b.Contains("Có")).Sum(x => x.SoLuong);
            co.TheoDoiRinhRap_SoLuong = soLuongTheoDoiRinhRap;


            co.TheoDoiRinhRap_PhanTram = TinhPhanTram(soLuongTheoDoiRinhRap, tongSo);

            soLuongTheoDoiRinhRap = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_4b) && x.c_4b.Contains("Không")).Sum(x => x.SoLuong);
            khong.TheoDoiRinhRap_SoLuong = soLuongTheoDoiRinhRap;


            khong.TheoDoiRinhRap_PhanTram = TinhPhanTram(soLuongTheoDoiRinhRap, tongSo);

            // Cau c_4c
            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_4c)).Sum(x => x.SoLuong);
            soLuongTheoDoiRinhRap = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_4c) && x.c_4c.Contains("Có")).Sum(x => x.SoLuong);
            co.TheoDoiRinhRap_SoLuong = soLuongTheoDoiRinhRap;


            co.TheoDoiRinhRap_PhanTram = TinhPhanTram(soLuongTheoDoiRinhRap, tongSo);

            soLuongTheoDoiRinhRap = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_4c) && x.c_4c.Contains("Không")).Sum(x => x.SoLuong);
            khong.TheoDoiRinhRap_SoLuong = soLuongTheoDoiRinhRap;


            khong.TheoDoiRinhRap_PhanTram = TinhPhanTram(soLuongTheoDoiRinhRap, tongSo);

            outDatas.Add(co);
            outDatas.Add(khong);
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo các loại chất gây nghiện
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoMucDoGapVanDeSKTT(List<KetQuaSangLocProModel> modelInputs, ref List<MucDoGapVanDeSKTTModel> outDatas)
        {
            var tongSo = 0;

            var soLuongKhongChutNao = 0;
            var soLuong1_7Ngay = 0;
            var soLuong8NgayTroLen = 0;
            var soLuongGanNhuHangNgay = 0;

            // Lo lắng, căng thẳng
            var loLangCangThang = new MucDoGapVanDeSKTTModel
            {
                NoiDung = "Lo lắng, căng thẳng",
            };
            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1a)).Sum(x => x.SoLuong);

            soLuongKhongChutNao = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1a) && x.c_1a.Contains("Không")).Sum(x => x.SoLuong);
            loLangCangThang.KhongChutNao_SoLuong = soLuongKhongChutNao;


            loLangCangThang.KhongChutNao_PhanTram = TinhPhanTram(soLuongKhongChutNao, tongSo);

            soLuong1_7Ngay = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1a) && x.c_1a.Contains("Từ 1")).Sum(x => x.SoLuong);
            loLangCangThang.Tu1Den7Ngay_SoLuong = soLuong1_7Ngay;


            loLangCangThang.Tu1Den7Ngay_PhanTram = TinhPhanTram(soLuong1_7Ngay, tongSo);

            soLuong8NgayTroLen = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1a) && x.c_1a.Contains("Từ 8")).Sum(x => x.SoLuong);
            loLangCangThang.Tu8NgayTroLen_SoLuong = soLuong8NgayTroLen;


            loLangCangThang.Tu8NgayTroLen_PhanTram = TinhPhanTram(soLuong8NgayTroLen, tongSo);

            soLuongGanNhuHangNgay = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1a) && x.c_1a.Contains("gần")).Sum(x => x.SoLuong);
            loLangCangThang.GanNhuHangNgay_SoLuong = soLuongGanNhuHangNgay;


            loLangCangThang.GanNhuHangNgay_PhanTram = TinhPhanTram(soLuongGanNhuHangNgay, tongSo);

            outDatas.Add(loLangCangThang);

            // Lo âu tới mức ko kiểm soát được
            var loAu = new MucDoGapVanDeSKTTModel
            {
                NoiDung = "Lo âu tới mức ko kiểm soát được",
            };
            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1b)).Sum(x => x.SoLuong);
            soLuongKhongChutNao = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1b) && x.c_1b.Contains("Không")).Sum(x => x.SoLuong);
            loAu.KhongChutNao_SoLuong = soLuongKhongChutNao;


            loAu.KhongChutNao_PhanTram = TinhPhanTram(soLuongKhongChutNao, tongSo);

            soLuong1_7Ngay = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1b) && x.c_1b.Contains("Từ 1")).Sum(x => x.SoLuong);
            loAu.Tu1Den7Ngay_SoLuong = soLuong1_7Ngay;


            loAu.Tu1Den7Ngay_PhanTram = TinhPhanTram(soLuong1_7Ngay, tongSo);

            soLuong8NgayTroLen = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1b) && x.c_1b.Contains("Từ 8")).Sum(x => x.SoLuong);
            loAu.Tu8NgayTroLen_SoLuong = soLuong8NgayTroLen;


            loAu.Tu8NgayTroLen_PhanTram = TinhPhanTram(soLuong8NgayTroLen, tongSo);

            soLuongGanNhuHangNgay = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1b) && x.c_1b.Contains("gần")).Sum(x => x.SoLuong);
            loAu.GanNhuHangNgay_SoLuong = soLuongGanNhuHangNgay;


            loAu.GanNhuHangNgay_PhanTram = TinhPhanTram(soLuongGanNhuHangNgay, tongSo);
            outDatas.Add(loAu);
            // Cảm thấy buồn chán, mất hết hy vọng
            var buonChan = new MucDoGapVanDeSKTTModel
            {
                NoiDung = "Cảm thấy buồn chán, mất hết hy vọng",
            };
            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1c)).Sum(x => x.SoLuong);
            soLuongKhongChutNao = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1c) && x.c_1c.Contains("Không")).Sum(x => x.SoLuong);
            buonChan.KhongChutNao_SoLuong = soLuongKhongChutNao;


            buonChan.KhongChutNao_PhanTram = TinhPhanTram(soLuongKhongChutNao, tongSo);

            soLuong1_7Ngay = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1c) && x.c_1c.Contains("Từ 1")).Sum(x => x.SoLuong);
            buonChan.Tu1Den7Ngay_SoLuong = soLuong1_7Ngay;


            buonChan.Tu1Den7Ngay_PhanTram = TinhPhanTram(soLuong1_7Ngay, tongSo);

            soLuong8NgayTroLen = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1c) && x.c_1c.Contains("Từ 8")).Sum(x => x.SoLuong);
            buonChan.Tu8NgayTroLen_SoLuong = soLuong8NgayTroLen;


            buonChan.Tu8NgayTroLen_PhanTram = TinhPhanTram(soLuong8NgayTroLen, tongSo);

            soLuongGanNhuHangNgay = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1c) && x.c_1c.Contains("gần")).Sum(x => x.SoLuong);
            buonChan.GanNhuHangNgay_SoLuong = soLuongGanNhuHangNgay;


            buonChan.GanNhuHangNgay_PhanTram = TinhPhanTram(soLuongGanNhuHangNgay, tongSo);
            outDatas.Add(buonChan);

            // Ít quan tâm hoặc ít hứng thú với mọi thứ
            var itQuanTam = new MucDoGapVanDeSKTTModel
            {
                NoiDung = "Ít quan tâm hoặc ít hứng thú với mọi thứ",
            };
            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1d)).Sum(x => x.SoLuong);
            soLuongKhongChutNao = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1d) && x.c_1d.Contains("Không")).Sum(x => x.SoLuong);
            itQuanTam.KhongChutNao_SoLuong = soLuongKhongChutNao;


            itQuanTam.KhongChutNao_PhanTram = TinhPhanTram(soLuongKhongChutNao, tongSo);

            soLuong1_7Ngay = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1d) && x.c_1d.Contains("Từ 1")).Sum(x => x.SoLuong);
            itQuanTam.Tu1Den7Ngay_SoLuong = soLuong1_7Ngay;


            itQuanTam.Tu1Den7Ngay_PhanTram = TinhPhanTram(soLuong1_7Ngay, tongSo);

            soLuong8NgayTroLen = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1d) && x.c_1d.Contains("Từ 8")).Sum(x => x.SoLuong);
            itQuanTam.Tu8NgayTroLen_SoLuong = soLuong8NgayTroLen;


            itQuanTam.Tu8NgayTroLen_PhanTram = TinhPhanTram(soLuong8NgayTroLen, tongSo);

            soLuongGanNhuHangNgay = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1d) && x.c_1d.Contains("gần")).Sum(x => x.SoLuong);
            itQuanTam.GanNhuHangNgay_SoLuong = soLuongGanNhuHangNgay;


            itQuanTam.GanNhuHangNgay_PhanTram = TinhPhanTram(soLuongGanNhuHangNgay, tongSo);
            outDatas.Add(itQuanTam);
        }



        /// <summary>
        /// Tính toán dữ liệu báo cáo theo kết quả QST
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoKetQuaQST(List<KetQuaSangLocProModel> modelInputs, ref List<KetQuaSangLocModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);


            foreach (var item in modelInputs)
            {


                outDatas.Add(new KetQuaSangLocModel
                {
                    NoiDung = item.tongdiem + " điểm",
                    SoNguoi = item.SoLuong,
                    PhanTram = TinhPhanTram(item.SoLuong, tongSo)
                });
            }
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo các loại chất gây nghiện
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoCacLoaiChatGayNghien(List<KetQuaSangLocProModel> modelInputs, ref List<CacChatGayNghienModel> outDatas)
        {
            var tongSo = 0;

            var soLuongNguyCoThap = 0;
            var soLuongNguyCoTrungBinh = 0;
            var soLuongNguyCoCao = 0;

            // Thuốc lá
            var thuocLa = new CacChatGayNghienModel
            {
                NoiDung = "Thuốc lá",
            };
            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemthuocla)).Sum(x => x.SoLuong);

            soLuongNguyCoThap = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemthuocla) && Convert.ToInt32(x.diemthuocla) >= 0 && Convert.ToInt32(x.diemthuocla) <= 3).Sum(x => x.SoLuong);
            thuocLa.NguyCoThap_SoLuong = soLuongNguyCoThap;


            thuocLa.NguyCoThap_PhanTram = TinhPhanTram(soLuongNguyCoThap, tongSo);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemthuocla) && Convert.ToInt32(x.diemthuocla) >= 4 && Convert.ToInt32(x.diemthuocla) <= 27).Sum(x => x.SoLuong);
            thuocLa.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;


            thuocLa.NguyCoTrungBinh_PhanTram = TinhPhanTram(soLuongNguyCoTrungBinh, tongSo);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemthuocla) && Convert.ToInt32(x.diemthuocla) > 27).Sum(x => x.SoLuong);
            thuocLa.NguyCoCao_SoLuong = soLuongNguyCoCao;


            thuocLa.NguyCoCao_PhanTram = TinhPhanTram(soLuongNguyCoCao, tongSo);

            thuocLa.Tong_SoLuong = tongSo;
            outDatas.Add(thuocLa);
            // Cồn rượu
            var conRuou = new CacChatGayNghienModel
            {
                NoiDung = "Cồn rượu",
            };
            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemthucuong)).Sum(x => x.SoLuong);

            soLuongNguyCoThap = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemthucuong) && Convert.ToInt32(x.diemthucuong) >= 0 && Convert.ToInt32(x.diemthucuong) <= 3).Sum(x => x.SoLuong);
            conRuou.NguyCoThap_SoLuong = soLuongNguyCoThap;


            conRuou.NguyCoThap_PhanTram = TinhPhanTram(soLuongNguyCoThap, tongSo);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemthucuong) && Convert.ToInt32(x.diemthucuong) >= 4 && Convert.ToInt32(x.diemthucuong) <= 27).Sum(x => x.SoLuong);
            conRuou.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;


            conRuou.NguyCoTrungBinh_PhanTram = TinhPhanTram(soLuongNguyCoTrungBinh, tongSo);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemthucuong) && Convert.ToInt32(x.diemthucuong) > 27).Sum(x => x.SoLuong);
            conRuou.NguyCoCao_SoLuong = soLuongNguyCoCao;


            conRuou.NguyCoCao_PhanTram = TinhPhanTram(soLuongNguyCoCao, tongSo);

            conRuou.Tong_SoLuong = tongSo;
            outDatas.Add(conRuou);
            // canSa
            var canSa = new CacChatGayNghienModel
            {
                NoiDung = "Cần sa",
            };

            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemcansa)).Sum(x => x.SoLuong);

            soLuongNguyCoThap = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemcansa) && Convert.ToInt32(x.diemcansa) >= 0 && Convert.ToInt32(x.diemcansa) <= 3).Sum(x => x.SoLuong);
            canSa.NguyCoThap_SoLuong = soLuongNguyCoThap;


            canSa.NguyCoThap_PhanTram = TinhPhanTram(soLuongNguyCoThap, tongSo);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemcansa) && Convert.ToInt32(x.diemcansa) >= 4 && Convert.ToInt32(x.diemcansa) <= 27).Sum(x => x.SoLuong);
            canSa.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;


            canSa.NguyCoTrungBinh_PhanTram = TinhPhanTram(soLuongNguyCoTrungBinh, tongSo);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemcansa) && Convert.ToInt32(x.diemcansa) > 27).Sum(x => x.SoLuong);
            canSa.NguyCoCao_SoLuong = soLuongNguyCoCao;


            canSa.NguyCoCao_PhanTram = TinhPhanTram(soLuongNguyCoCao, tongSo);

            canSa.Tong_SoLuong = tongSo;
            outDatas.Add(canSa);
            // Cocaine
            var cocaine = new CacChatGayNghienModel
            {
                NoiDung = "Cocaine",
            };

            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemcoca)).Sum(x => x.SoLuong);

            soLuongNguyCoThap = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemcoca) && Convert.ToInt32(x.diemcoca) >= 0 && Convert.ToInt32(x.diemcoca) <= 3).Sum(x => x.SoLuong);
            cocaine.NguyCoThap_SoLuong = soLuongNguyCoThap;


            cocaine.NguyCoThap_PhanTram = TinhPhanTram(soLuongNguyCoThap, tongSo);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemcoca) && Convert.ToInt32(x.diemcoca) >= 4 && Convert.ToInt32(x.diemcoca) <= 27).Sum(x => x.SoLuong);
            cocaine.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;


            cocaine.NguyCoTrungBinh_PhanTram = TinhPhanTram(soLuongNguyCoTrungBinh, tongSo);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemcoca) && Convert.ToInt32(x.diemcoca) > 27).Sum(x => x.SoLuong);
            cocaine.NguyCoCao_SoLuong = soLuongNguyCoCao;


            cocaine.NguyCoCao_PhanTram = TinhPhanTram(soLuongNguyCoCao, tongSo);

            cocaine.Tong_SoLuong = tongSo;
            outDatas.Add(cocaine);
            // Ma túy đá
            var maTuyDa = new CacChatGayNghienModel
            {
                NoiDung = "Ma túy đá",
            };

            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatkichthich)).Sum(x => x.SoLuong);

            soLuongNguyCoThap = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatkichthich) && Convert.ToInt32(x.diemchatkichthich) >= 0 && Convert.ToInt32(x.diemchatkichthich) <= 3).Sum(x => x.SoLuong);
            maTuyDa.NguyCoThap_SoLuong = soLuongNguyCoThap;


            maTuyDa.NguyCoThap_PhanTram = TinhPhanTram(soLuongNguyCoThap, tongSo);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatkichthich) && Convert.ToInt32(x.diemchatkichthich) >= 4 && Convert.ToInt32(x.diemchatkichthich) <= 27).Sum(x => x.SoLuong);
            maTuyDa.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;


            maTuyDa.NguyCoTrungBinh_PhanTram = TinhPhanTram(soLuongNguyCoTrungBinh, tongSo);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatkichthich) && Convert.ToInt32(x.diemchatkichthich) > 27).Sum(x => x.SoLuong);
            maTuyDa.NguyCoCao_SoLuong = soLuongNguyCoCao;


            maTuyDa.NguyCoCao_PhanTram = TinhPhanTram(soLuongNguyCoCao, tongSo);

            maTuyDa.Tong_SoLuong = tongSo;
            outDatas.Add(maTuyDa);
            // Khí xông hít
            var khiXongHit = new CacChatGayNghienModel
            {
                NoiDung = "Khí xông hít",
            };

            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemkhixong)).Sum(x => x.SoLuong);

            soLuongNguyCoThap = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemkhixong) && Convert.ToInt32(x.diemkhixong) >= 0 && Convert.ToInt32(x.diemkhixong) <= 3).Sum(x => x.SoLuong);
            khiXongHit.NguyCoThap_SoLuong = soLuongNguyCoThap;


            khiXongHit.NguyCoThap_PhanTram = TinhPhanTram(soLuongNguyCoThap, tongSo);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemkhixong) && Convert.ToInt32(x.diemkhixong) >= 4 && Convert.ToInt32(x.diemkhixong) <= 27).Sum(x => x.SoLuong);
            khiXongHit.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;


            khiXongHit.NguyCoTrungBinh_PhanTram = TinhPhanTram(soLuongNguyCoTrungBinh, tongSo);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemkhixong) && Convert.ToInt32(x.diemkhixong) > 27).Sum(x => x.SoLuong);
            khiXongHit.NguyCoCao_SoLuong = soLuongNguyCoCao;


            khiXongHit.NguyCoCao_PhanTram = TinhPhanTram(soLuongNguyCoCao, tongSo);

            khiXongHit.Tong_SoLuong = tongSo;
            outDatas.Add(khiXongHit);
            // Thuốc an thần
            var thuocAnThan = new CacChatGayNghienModel
            {
                NoiDung = "Thuốc an thần",
            };

            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatanthan)).Sum(x => x.SoLuong);

            soLuongNguyCoThap = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatanthan) && Convert.ToInt32(x.diemchatanthan) >= 0 && Convert.ToInt32(x.diemchatanthan) <= 3).Sum(x => x.SoLuong);
            thuocAnThan.NguyCoThap_SoLuong = soLuongNguyCoThap;


            thuocAnThan.NguyCoThap_PhanTram = TinhPhanTram(soLuongNguyCoThap, tongSo);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatanthan) && Convert.ToInt32(x.diemchatanthan) >= 4 && Convert.ToInt32(x.diemchatanthan) <= 27).Sum(x => x.SoLuong);
            thuocAnThan.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;


            thuocAnThan.NguyCoTrungBinh_PhanTram = TinhPhanTram(soLuongNguyCoTrungBinh, tongSo);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatanthan) && Convert.ToInt32(x.diemchatanthan) > 27).Sum(x => x.SoLuong);
            thuocAnThan.NguyCoCao_SoLuong = soLuongNguyCoCao;


            thuocAnThan.NguyCoCao_PhanTram = TinhPhanTram(soLuongNguyCoCao, tongSo);

            thuocAnThan.Tong_SoLuong = tongSo;
            outDatas.Add(thuocAnThan);
            // Chất gây ảo giác
            var chatGayAoGiac = new CacChatGayNghienModel
            {
                NoiDung = "Chất gây ảo giác",
            };

            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatgayaogiac)).Sum(x => x.SoLuong);

            soLuongNguyCoThap = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatgayaogiac) && Convert.ToInt32(x.diemchatgayaogiac) >= 0 && Convert.ToInt32(x.diemchatgayaogiac) <= 3).Sum(x => x.SoLuong);
            chatGayAoGiac.NguyCoThap_SoLuong = soLuongNguyCoThap;


            chatGayAoGiac.NguyCoThap_PhanTram = TinhPhanTram(soLuongNguyCoThap, tongSo);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatgayaogiac) && Convert.ToInt32(x.diemchatgayaogiac) >= 4 && Convert.ToInt32(x.diemchatgayaogiac) <= 27).Sum(x => x.SoLuong);
            chatGayAoGiac.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;


            chatGayAoGiac.NguyCoTrungBinh_PhanTram = TinhPhanTram(soLuongNguyCoTrungBinh, tongSo);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatgayaogiac) && Convert.ToInt32(x.diemchatgayaogiac) > 27).Sum(x => x.SoLuong);
            chatGayAoGiac.NguyCoCao_SoLuong = soLuongNguyCoCao;


            chatGayAoGiac.NguyCoCao_PhanTram = TinhPhanTram(soLuongNguyCoCao, tongSo);

            chatGayAoGiac.Tong_SoLuong = tongSo;
            outDatas.Add(chatGayAoGiac);
            // Chất thuốc phiện
            var chatThuocPhien = new CacChatGayNghienModel
            {
                NoiDung = "Chất thuốc phiện",
            };
            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatthuocphien)).Sum(x => x.SoLuong);

            soLuongNguyCoThap = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatthuocphien) && Convert.ToInt32(x.diemchatthuocphien) >= 0 && Convert.ToInt32(x.diemchatthuocphien) <= 3).Sum(x => x.SoLuong);
            chatThuocPhien.NguyCoThap_SoLuong = soLuongNguyCoThap;


            chatThuocPhien.NguyCoThap_PhanTram = TinhPhanTram(soLuongNguyCoThap, tongSo);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatthuocphien) && Convert.ToInt32(x.diemchatthuocphien) >= 4 && Convert.ToInt32(x.diemchatthuocphien) <= 27).Sum(x => x.SoLuong);
            chatThuocPhien.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;


            chatThuocPhien.NguyCoTrungBinh_PhanTram = TinhPhanTram(soLuongNguyCoTrungBinh, tongSo);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatthuocphien) && Convert.ToInt32(x.diemchatthuocphien) > 27).Sum(x => x.SoLuong);
            chatThuocPhien.NguyCoCao_SoLuong = soLuongNguyCoCao;


            chatThuocPhien.NguyCoCao_PhanTram = TinhPhanTram(soLuongNguyCoCao, tongSo);

            chatThuocPhien.Tong_SoLuong = tongSo;
            outDatas.Add(chatThuocPhien);
            // Các thuốc khác
            var cacThuocKhac = new CacChatGayNghienModel
            {
                NoiDung = "Các thuốc khác",
            };
            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatkhac)).Sum(x => x.SoLuong);

            soLuongNguyCoThap = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatkhac) && Convert.ToInt32(x.diemchatkhac) >= 0 && Convert.ToInt32(x.diemchatkhac) <= 3).Sum(x => x.SoLuong);
            cacThuocKhac.NguyCoThap_SoLuong = soLuongNguyCoThap;


            cacThuocKhac.NguyCoThap_PhanTram = TinhPhanTram(soLuongNguyCoThap, tongSo);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatkhac) && Convert.ToInt32(x.diemchatkhac) >= 4 && Convert.ToInt32(x.diemchatkhac) <= 27).Sum(x => x.SoLuong);
            cacThuocKhac.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;


            cacThuocKhac.NguyCoTrungBinh_PhanTram = TinhPhanTram(soLuongNguyCoTrungBinh, tongSo);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatkhac) && Convert.ToInt32(x.diemchatkhac) > 27).Sum(x => x.SoLuong);
            cacThuocKhac.NguyCoCao_SoLuong = soLuongNguyCoCao;


            cacThuocKhac.NguyCoCao_PhanTram = TinhPhanTram(soLuongNguyCoCao, tongSo);

            cacThuocKhac.Tong_SoLuong = tongSo;
            outDatas.Add(cacThuocKhac);
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo sốc thuốc meth
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoSocMeth(List<KetQuaSangLocProModel> modelInputs, ref List<KetQuaSangLocModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);


            // Hoang tưởng/ ảo giác
            var soLuongHoangTuong = modelInputs.Where(x => x.trieuchung_2.Contains("Hoang")).Sum(x => x.SoLuong);


            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Hoang tưởng/ ảo giác",
                SoNguoi = soLuongHoangTuong,
                PhanTram = TinhPhanTram(soLuongHoangTuong, tongSo)
            });

            // Buồn nôn/ nôn mửa      
            var soLuongBuonNon = modelInputs.Where(x => x.trieuchung_2.Contains("Buồn")).Sum(x => x.SoLuong);


            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Buồn nôn/ nôn mửa",
                SoNguoi = soLuongBuonNon,
                PhanTram = TinhPhanTram(soLuongBuonNon, tongSo)
            });

            // Run rẩy/ co giật
            var soLuongRunRay = modelInputs.Where(x => x.trieuchung_2.Contains("Run")).Sum(x => x.SoLuong);


            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Run rẩy/ co giật",
                SoNguoi = soLuongRunRay,
                PhanTram = TinhPhanTram(soLuongRunRay, tongSo)
            });

            // Cảm thấy tê liệt nhưng vẫn tỉnh
            var soLuongCam = modelInputs.Where(x => x.trieuchung_2.Contains("Cảm")).Sum(x => x.SoLuong);


            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Cảm thấy tê liệt nhưng vẫn tỉnh",
                SoNguoi = soLuongCam,
                PhanTram = TinhPhanTram(soLuongCam, tongSo)
            });

            // Lo âu/ cơn hoảng loạn
            var soLuongLoAu = modelInputs.Where(x => x.trieuchung_2.Contains("Lo")).Sum(x => x.SoLuong);


            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Lo âu/ cơn hoảng loạn",
                SoNguoi = soLuongLoAu,
                PhanTram = TinhPhanTram(soLuongLoAu, tongSo)
            });

            // Đau ngực hoặc căng tức ngực
            var soLuongDau = modelInputs.Where(x => x.trieuchung_2.Contains("Đau")).Sum(x => x.SoLuong);


            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Đau ngực hoặc căng tức ngực",
                SoNguoi = soLuongDau,
                PhanTram = TinhPhanTram(soLuongDau, tongSo)
            });

            //Đột quỵ 
            var soLuongDot = modelInputs.Where(x => x.trieuchung_2.Contains("Đột")).Sum(x => x.SoLuong);


            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Đột quỵ",
                SoNguoi = soLuongDot,
                PhanTram = TinhPhanTram(soLuongDot, tongSo)
            });

            // Tim đập rất nhanh
            var soLuongTim = modelInputs.Where(x => x.trieuchung_2.Contains("Tim")).Sum(x => x.SoLuong);


            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Tim đập rất nhanh",
                SoNguoi = soLuongTim,
                PhanTram = TinhPhanTram(soLuongTim, tongSo)
            });

            // Chưa từng gặp triệu chứng nào
            var soLuongChua = modelInputs.Where(x => x.trieuchung_2.Contains("Chưa từng")).Sum(x => x.SoLuong);


            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Chưa từng gặp triệu chứng nào",
                SoNguoi = soLuongChua,
                PhanTram = TinhPhanTram(soLuongChua, tongSo)
            });

            // Thêm dòng tổng số
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Tổng",
                SoNguoi = tongSo,
                PhanTram = 100
            });
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo sốc thuốc heroin
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoSocHeroins(List<KetQuaSangLocProModel> modelInputs, ref List<KetQuaSangLocModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);

            var soLuongKhongGap = modelInputs.Where(x => x.trieuchung.ToLower().Contains("không gặp")).Sum(x => x.SoLuong);
            // Từng có một trong các triệu chứng



            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Từng có một trong các triệu chứng",
                SoNguoi = (tongSo - soLuongKhongGap),
                PhanTram = TinhPhanTram((tongSo - soLuongKhongGap), tongSo)
            });

            // Chưa từng gặp triệu chứng nào


            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Chưa từng gặp triệu chứng nào",
                SoNguoi = soLuongKhongGap,
                PhanTram = TinhPhanTram(soLuongKhongGap, tongSo)
            });

            // Thêm dòng tổng số
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Tổng",
                SoNguoi = tongSo,
                PhanTram = 100
            });
        }



        /// <summary>
        /// Tính toán dữ liệu báo cáo theo bệnh lao, vgc
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoBenhLao_VGC(List<KetQuaSangLocProModel> modelInputs, ref List<BenhLao_VGCModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);


            // có
            var co = new BenhLao_VGCModel
            {
                NoiDung = "Có",
            };
            // không
            var khong = new BenhLao_VGCModel
            {
                NoiDung = "Không",
            };

            // KBKTL
            var kbktl = new BenhLao_VGCModel
            {
                NoiDung = "KB/KTL",
            };

            var soLuongHTCo = modelInputs.Where(x => x.NoiDung_HT.Contains("Có")).Sum(x => x.SoLuong);
            var soLuongQKCo = modelInputs.Where(x => x.NoiDung_QK.Contains("Có")).Sum(x => x.SoLuong);



            co.HienTai_SoLuong = soLuongHTCo;
            co.HienTai_PhanTram = TinhPhanTram(soLuongHTCo, tongSo);
            co.QuaKhu_SoLuong = soLuongQKCo;


            co.QuaKhu_PhanTram = TinhPhanTram(soLuongQKCo, tongSo);

            var soLuongHTKhong = modelInputs.Where(x => x.NoiDung_HT.Contains("Không")).Sum(x => x.SoLuong);
            var soLuongQKKhong = modelInputs.Where(x => x.NoiDung_QK.Contains("Không")).Sum(x => x.SoLuong);

            khong.HienTai_SoLuong = soLuongHTKhong;
            khong.HienTai_PhanTram = TinhPhanTram(soLuongHTKhong, tongSo);
            khong.QuaKhu_SoLuong = soLuongQKKhong;


            khong.QuaKhu_PhanTram = TinhPhanTram(soLuongQKKhong, tongSo);

            var soLuongHTKB = modelInputs.Where(x => x.NoiDung_HT.Contains("KB")).Sum(x => x.SoLuong);
            var soLuongQKKB = modelInputs.Where(x => x.NoiDung_QK.Contains("KB")).Sum(x => x.SoLuong);


            kbktl.HienTai_SoLuong = soLuongHTKB;
            kbktl.HienTai_PhanTram = TinhPhanTram(soLuongHTKB, tongSo);
            kbktl.QuaKhu_SoLuong = soLuongQKKB;


            kbktl.QuaKhu_PhanTram = TinhPhanTram(soLuongQKKB, tongSo);

            outDatas.Add(co);
            outDatas.Add(khong);
            outDatas.Add(kbktl);
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo bệnh STI
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoBenhSTI(List<KetQuaSangLocProModel> modelInputs, ref List<BenhSTIModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);


            // số người dùng từng loại 
            var soNguoi = new BenhSTIModel
            {
                NoiDung = "Số người",
            };
            // tỉ lệ % người dùng từng loại
            var tyLe = new BenhSTIModel
            {
                NoiDung = "%",
            };

            var soLuongLau = modelInputs.Where(x => x.sti1.Contains("LẬU")).Sum(x => x.SoLuong);


            soNguoi.Lau = soLuongLau;
            tyLe.Lau = TinhPhanTram(soLuongLau, tongSo);

            var soLuongSuiMaoGa = modelInputs.Where(x => x.sti1.Contains("SÙI")).Sum(x => x.SoLuong);


            soNguoi.SuiMaoGa = soLuongSuiMaoGa;
            tyLe.SuiMaoGa = TinhPhanTram(soLuongSuiMaoGa, tongSo);

            var soLuongKhongMac = modelInputs.Where(x => x.sti1.Contains("KHÔNG")).Sum(x => x.SoLuong);


            soNguoi.KhongMac = soLuongKhongMac;
            tyLe.KhongMac = TinhPhanTram(soLuongKhongMac, tongSo);

            var soLuongKhac = modelInputs.Where(x => x.sti1.Contains("KHÁC")).Sum(x => x.SoLuong);


            soNguoi.Khac = soLuongKhac;
            tyLe.Khac = TinhPhanTram(soLuongKhac, tongSo);

            var soLuongGiangMai = modelInputs.Where(x => x.sti1.Contains("GIANG")).Sum(x => x.SoLuong);


            soNguoi.GiangMai = soLuongGiangMai;
            tyLe.GiangMai = TinhPhanTram(soLuongGiangMai, tongSo);

            var soLuongKBKTL = modelInputs.Where(x => x.sti1.Contains("KB")).Sum(x => x.SoLuong);


            soNguoi.KBKTL = soLuongKBKTL;
            tyLe.KBKTL = TinhPhanTram(soLuongKBKTL, tongSo);

            outDatas.Add(soNguoi);

            outDatas.Add(tyLe);
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo nhiều nguy cơ tính dục
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoNhieuNguycoTinhDuc(List<KetQuaSangLocProModel> modelInputs, ref List<NhieuNguyCoTinhDucModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);


            // số người dùng từng loại 
            var soNguoi = new NhieuNguyCoTinhDucModel
            {
                NoiDung = "Số người",
            };
            // tỉ lệ % người dùng từng loại
            var tyLe = new NhieuNguyCoTinhDucModel
            {
                NoiDung = "%",
            };

            var soLuong1NC = modelInputs.Where(x => x.NoiDung.Contains("1")).Sum(x => x.SoLuong);


            soNguoi.Mot = soLuong1NC;
            tyLe.Mot = TinhPhanTram(soLuong1NC, tongSo);

            var soLuongHai = modelInputs.Where(x => x.NoiDung.Contains("2")).Sum(x => x.SoLuong);


            soNguoi.Hai = soLuongHai;
            tyLe.Hai = TinhPhanTram(soLuongHai, tongSo);

            var soLuongBa = modelInputs.Where(x => x.NoiDung.Contains("3")).Sum(x => x.SoLuong);


            soNguoi.Ba = soLuongBa;
            tyLe.Ba = TinhPhanTram(soLuongBa, tongSo);

            var soLuongBonNam = modelInputs.Where(x => Convert.ToInt32(x.NoiDung) > 3).Sum(x => x.SoLuong);


            soNguoi.BonNam = soLuongBonNam;
            tyLe.BonNam = TinhPhanTram(soLuongBonNam, tongSo);

            outDatas.Add(soNguoi);

            outDatas.Add(tyLe);
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo sd ma túy đá khi QHTD, qhtd tập thể, bán dâm
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoSDMaTuyDaKhiQHTD(List<KetQuaSangLocProModel> modelInputs, ref List<SuDungMTDKhiQHTDModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);


            // số người dùng từng loại 
            var soNguoi = new SuDungMTDKhiQHTDModel
            {
                NoiDung = "Số người",
            };
            // tỉ lệ % người dùng từng loại
            var tyLe = new SuDungMTDKhiQHTDModel
            {
                NoiDung = "%",
            };

            var soLuongCo = modelInputs.Where(x => x.NoiDung.Contains("có") || x.NoiDung.Contains("Có")).Sum(x => x.SoLuong);


            soNguoi.Co = soLuongCo;
            tyLe.Co = TinhPhanTram(soLuongCo, tongSo);

            var soLuongKhong = modelInputs.Where(x => x.NoiDung.Contains("không") || x.NoiDung.Contains("Không")).Sum(x => x.SoLuong);


            soNguoi.Khong = soLuongKhong;
            tyLe.Khong = TinhPhanTram(soLuongKhong, tongSo);

            var soLuongKBKTL = modelInputs.Where(x => x.NoiDung.Contains("kB") || x.NoiDung.Contains("KB")).Sum(x => x.SoLuong);


            soNguoi.KBKTL = soLuongKBKTL;
            tyLe.KBKTL = TinhPhanTram(soLuongKBKTL, tongSo);

            outDatas.Add(soNguoi);

            outDatas.Add(tyLe);
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo dùng BCS
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoDungBCS(List<KetQuaSangLocProModel> modelInputs, ref List<DungBCSModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);


            // số người dùng từng loại 
            var soNguoi = new DungBCSModel
            {
                NoiDung = "Số người",
            };
            // tỉ lệ % người dùng từng loại
            var tyLe = new DungBCSModel
            {
                NoiDung = "%",
            };

            var soLuongLuonLuon = modelInputs.Where(x => x.qhtd_2.Contains("luôn")).Sum(x => x.SoLuong);


            soNguoi.LuonLuon = soLuongLuonLuon;
            tyLe.LuonLuon = TinhPhanTram(soLuongLuonLuon, tongSo);

            var soLuongThuongXuyen = modelInputs.Where(x => x.qhtd_2.Contains("thường") || x.qhtd_2.Contains("Thường")).Sum(x => x.SoLuong);


            soNguoi.ThuongXuyen = soLuongThuongXuyen;
            tyLe.ThuongXuyen = TinhPhanTram(soLuongThuongXuyen, tongSo);

            var soLuongThiThoang = modelInputs.Where(x => x.qhtd_2.Contains("thỉnh") || x.qhtd_2.Contains("Thỉnh")).Sum(x => x.SoLuong);


            soNguoi.ThiThoang = soLuongThiThoang;
            tyLe.ThiThoang = TinhPhanTram(soLuongThiThoang, tongSo);

            var soLuongHiemKhi = modelInputs.Where(x => x.qhtd_2.Contains("hiếm") || x.qhtd_2.Contains("Hiếm")).Sum(x => x.SoLuong);


            soNguoi.HiemKhi = soLuongHiemKhi;
            tyLe.HiemKhi = TinhPhanTram(soLuongHiemKhi, tongSo);

            var soLuongKhongBaoGio = modelInputs.Where(x => x.qhtd_2.Contains("không bao") || x.qhtd_2.Contains("Không bao")).Sum(x => x.SoLuong);


            soNguoi.KhongBaoGio = soLuongKhongBaoGio;
            tyLe.KhongBaoGio = TinhPhanTram(soLuongKhongBaoGio, tongSo);

            outDatas.Add(soNguoi);

            outDatas.Add(tyLe);
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo nguy cơ tình dục
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoNguyCoTinhDuc(List<KetQuaSangLocProModel> modelInputs, ref List<NguyCoTinhDucModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);


            // số người dùng từng loại 
            var soNguoi = new NguyCoTinhDucModel
            {
                NoiDung = "Số người",
            };
            // tỉ lệ % người dùng từng loại
            var tyLe = new NguyCoTinhDucModel
            {
                NoiDung = "%",
            };

            var soLuongChuaBaoGio = modelInputs.Where(x => x.qhtd.Contains("chưa") || x.qhtd.Contains("Chưa")).Sum(x => x.SoLuong);


            soNguoi.ChuaBaoGio = soLuongChuaBaoGio;
            tyLe.ChuaBaoGio = TinhPhanTram(soLuongChuaBaoGio, tongSo);

            var soLuongDongGioi = modelInputs.Where(x => x.qhtd.Contains("đồng") || x.qhtd.Contains("Đồng")).Sum(x => x.SoLuong);


            soNguoi.DongGioi = soLuongDongGioi;
            tyLe.DongGioi = TinhPhanTram(soLuongDongGioi, tongSo);

            var soLuongKhacGioi = modelInputs.Where(x => x.qhtd.Contains("khác") || x.qhtd.Contains("Khác")).Sum(x => x.SoLuong);


            soNguoi.KhacGioi = soLuongKhacGioi;
            tyLe.KhacGioi = TinhPhanTram(soLuongKhacGioi, tongSo);

            var soLuongCaHai = modelInputs.Where(x => x.qhtd.Contains(",")).Sum(x => x.SoLuong);


            soNguoi.CaHai = soLuongCaHai;
            tyLe.CaHai = TinhPhanTram(soLuongCaHai, tongSo);

            outDatas.Add(soNguoi);

            outDatas.Add(tyLe);
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo chung BKT
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoChungBKT(List<KetQuaSangLocProModel> modelInputs, ref List<ChungBKTModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);


            // số người dùng từng loại 
            var soNguoi = new ChungBKTModel
            {
                NoiDung = "Số người",
            };
            // tỉ lệ % người dùng từng loại
            var tyLe = new ChungBKTModel
            {
                NoiDung = "%",
            };

            var soLuongDaTungDungChung = modelInputs.Where(x => x.dungchung.Contains("đã") || x.dungchung.Contains("Đã")).Sum(x => x.SoLuong);


            soNguoi.DaTungDungChung = soLuongDaTungDungChung;
            tyLe.DaTungDungChung = TinhPhanTram(soLuongDaTungDungChung, tongSo);


            var soLuongChuaBaoGio = modelInputs.Where(x => x.dungchung.Contains("chưa bao") || x.dungchung.Contains("Chưa bao")).Sum(x => x.SoLuong);


            soNguoi.ChuaBaoGio = soLuongChuaBaoGio;
            tyLe.ChuaBaoGio = TinhPhanTram(soLuongChuaBaoGio, tongSo);

            outDatas.Add(soNguoi);

            outDatas.Add(tyLe);
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo nguy cơ sử dụng ma túy đá
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoNguyCoKhiSDMTD(List<KetQuaSangLocProModel> modelInputs, ref List<NguyCoKhiSDMTDModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);


            // số người dùng từng loại 
            var soNguoi = new NguyCoKhiSDMTDModel
            {
                NoiDung = "Số người",
            };
            // tỉ lệ % người dùng từng loại
            var tyLe = new NguyCoKhiSDMTDModel
            {
                NoiDung = "%",
            };

            var soLuongChuaBaoGio = modelInputs.Where(x => x.tiemchich.Contains("chưa") || x.tiemchich.Contains("Chưa")).Sum(x => x.SoLuong);


            soNguoi.ChuaBaoGio = soLuongChuaBaoGio;
            tyLe.ChuaBaoGio = TinhPhanTram(soLuongChuaBaoGio, tongSo);

            var soLuongDaTungTiemChich = modelInputs.Where(x => x.tiemchich.Contains("đã") || x.tiemchich.Contains("Đã")).Sum(x => x.SoLuong);


            soNguoi.DaTungTiemChich = soLuongDaTungTiemChich;
            tyLe.DaTungTiemChich = TinhPhanTram(soLuongDaTungTiemChich, tongSo);

            var soLuongVanDangTiemChich = modelInputs.Where(x => x.tiemchich.Contains("vẫn") || x.tiemchich.Contains("Vẫn")).Sum(x => x.SoLuong);


            soNguoi.VanDangTiemChich = soLuongVanDangTiemChich;
            tyLe.VanDangTiemChich = TinhPhanTram(soLuongVanDangTiemChich, tongSo);

            outDatas.Add(soNguoi);

            outDatas.Add(tyLe);
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo loại ma túy đá sử dụng đầu tiên
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoLoaiMaTuyDaSDDauTien(List<KetQuaSangLocProModel> modelInputs, ref List<LoaiMaTuyDaSuDungDTModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);


            // số người dùng từng loại 
            var soNguoi = new LoaiMaTuyDaSuDungDTModel
            {
                NoiDung = "Số người",
            };
            // tỉ lệ % người dùng từng loại
            var tyLe = new LoaiMaTuyDaSuDungDTModel
            {
                NoiDung = "%",
            };

            var soLuongDa = modelInputs.Where(x => x.matuydautien.Contains("đá") || x.matuydautien.Contains("Đá")).Sum(x => x.SoLuong);

            soNguoi.Da = soLuongDa;
            tyLe.Da = TinhPhanTram(soLuongDa, tongSo);

            var soLuongKeo = modelInputs.Where(x => x.matuydautien.Contains("keo") || x.matuydautien.Contains("Keo")).Sum(x => x.SoLuong);

            soNguoi.Keo = soLuongKeo;
            tyLe.Keo = TinhPhanTram(soLuongKeo, tongSo);

            var soLuongCanCo = modelInputs.Where(x => x.matuydautien.Contains("cần") || x.matuydautien.Contains("Cần") || x.matuydautien.Contains("cỏ") || x.matuydautien.Contains("Cỏ")).Sum(x => x.SoLuong);

            soNguoi.CanCo = soLuongCanCo;
            tyLe.CanCo = TinhPhanTram(soLuongCanCo, tongSo);

            var soLuongKetamin = modelInputs.Where(x => x.matuydautien.Contains("ketamin") || x.matuydautien.Contains("Ketamin")).Sum(x => x.SoLuong);

            soNguoi.Ketamin = soLuongKetamin;
            tyLe.Ketamin = TinhPhanTram(soLuongKetamin, tongSo);

            var soLuongBongCuoi = modelInputs.Where(x => x.matuydautien.Contains("bóng") || x.matuydautien.Contains("Bóng")).Sum(x => x.SoLuong);

            soNguoi.BongCuoi = soLuongBongCuoi;
            tyLe.BongCuoi = TinhPhanTram(soLuongBongCuoi, tongSo);

            var soLuongHeroin = modelInputs.Where(x => x.matuydautien.Contains("heroin") || x.matuydautien.Contains("Heroin")).Sum(x => x.SoLuong);

            soNguoi.Heroin = soLuongHeroin;
            tyLe.Heroin = TinhPhanTram(soLuongHeroin, tongSo);

            var soLuongCacChatHit = modelInputs.Where(x => x.matuydautien.Contains("hít") || x.matuydautien.Contains("Hít")).Sum(x => x.SoLuong);

            soNguoi.CacChatHit = soLuongCacChatHit;
            tyLe.CacChatHit = TinhPhanTram(soLuongCacChatHit, tongSo);

            outDatas.Add(soNguoi);

            outDatas.Add(tyLe);
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo lần đầu sử dụng ma túy đá
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoLanDauSDMaTuyDa(List<KetQuaSangLocProModel> modelInputs, ref List<LanDauSuDungMaTuyDaModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);


            // số người dùng từng loại 
            var soNguoi = new LanDauSuDungMaTuyDaModel
            {
                NoiDung = "Số người",
            };
            // tỉ lệ % người dùng từng loại
            var tyLe = new LanDauSuDungMaTuyDaModel
            {
                NoiDung = "%",
            };

            var soLuong13Tuoi = modelInputs.Where(x => x.tuoi == 13).Sum(x => x.SoLuong);
            soNguoi._13 = soLuong13Tuoi;
            tyLe._13 = TinhPhanTram(soLuong13Tuoi, tongSo);

            var soLuong14Tuoi = modelInputs.Where(x => x.tuoi == 14).Sum(x => x.SoLuong);
            soNguoi._14 = soLuong14Tuoi;
            tyLe._14 = TinhPhanTram(soLuong14Tuoi, tongSo);

            var soLuong15Tuoi = modelInputs.Where(x => x.tuoi == 15).Sum(x => x.SoLuong);
            soNguoi._15 = soLuong15Tuoi;
            tyLe._15 = TinhPhanTram(soLuong15Tuoi, tongSo);

            var soLuong16Tuoi = modelInputs.Where(x => x.tuoi == 16).Sum(x => x.SoLuong);
            soNguoi._16 = soLuong16Tuoi;
            tyLe._16 = TinhPhanTram(soLuong16Tuoi, tongSo);

            var soLuong17Tuoi = modelInputs.Where(x => x.tuoi == 17).Sum(x => x.SoLuong);
            soNguoi._17 = soLuong17Tuoi;
            tyLe._17 = TinhPhanTram(soLuong17Tuoi, tongSo);

            var soLuong18Tuoi = modelInputs.Where(x => x.tuoi == 18).Sum(x => x.SoLuong);
            soNguoi._18 = soLuong18Tuoi;
            tyLe._18 = TinhPhanTram(soLuong18Tuoi, tongSo);

            var soLuong19Tuoi = modelInputs.Where(x => x.tuoi == 19).Sum(x => x.SoLuong);
            soNguoi._19 = soLuong19Tuoi;
            tyLe._19 = TinhPhanTram(soLuong19Tuoi, tongSo);

            var soLuong20Tuoi = modelInputs.Where(x => x.tuoi == 20).Sum(x => x.SoLuong);
            soNguoi._20 = soLuong20Tuoi;
            tyLe._20 = TinhPhanTram(soLuong20Tuoi, tongSo);

            var soLuong21Tuoi = modelInputs.Where(x => x.tuoi == 21).Sum(x => x.SoLuong);
            soNguoi._21 = soLuong21Tuoi;
            tyLe._21 = TinhPhanTram(soLuong21Tuoi, tongSo);

            var soLuong22Tuoi = modelInputs.Where(x => x.tuoi == 22).Sum(x => x.SoLuong);
            soNguoi._22 = soLuong22Tuoi;
            tyLe._22 = TinhPhanTram(soLuong22Tuoi, tongSo);

            var soLuong23Tuoi = modelInputs.Where(x => x.tuoi == 23).Sum(x => x.SoLuong);
            soNguoi._23 = soLuong23Tuoi;
            tyLe._23 = TinhPhanTram(soLuong23Tuoi, tongSo);

            outDatas.Add(soNguoi);

            outDatas.Add(tyLe);
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo tấn suất sử dụng ma túy đá
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoTanSuatSDMaTuyDa(List<KetQuaSangLocProModel> modelInputs, ref List<TanSuatSuDungMaTuyDaModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);


            // số người dùng từng loại 

            var soNguoi = new TanSuatSuDungMaTuyDaModel
            {
                NoiDung = "Số người",
            };
            // tỉ lệ % người dùng từng loại
            var tyLe = new TanSuatSuDungMaTuyDaModel
            {
                NoiDung = "%",
            };

            var soLuong1Ngay = modelInputs.Where(x => x.tansuatda.Contains("1 ngày")).Sum(x => x.SoLuong);
            soNguoi.VaiLan1Ngay = soLuong1Ngay;
            tyLe.VaiLan1Ngay = TinhPhanTram(soLuong1Ngay, tongSo);

            var soLuongHangNgay = modelInputs.Where(x => x.tansuatda.Contains("Hàng")).Sum(x => x.SoLuong);
            soNguoi.HangNgay = soLuongHangNgay;
            tyLe.HangNgay = TinhPhanTram(soLuongHangNgay, tongSo);

            var soLuongVaiLan1Tuan = modelInputs.Where(x => x.tansuatda.Contains("1 tuần")).Sum(x => x.SoLuong);
            soNguoi.VaiLan1Tuan = soLuongVaiLan1Tuan;
            tyLe.VaiLan1Tuan = TinhPhanTram(soLuongVaiLan1Tuan, tongSo);

            var soLuongVaiNgayRoiTamNghi = modelInputs.Where(x => x.tansuatda.Contains("nghỉ")).Sum(x => x.SoLuong);
            soNguoi.VaiNgayRoiTamNghi = soLuongVaiNgayRoiTamNghi;
            tyLe.VaiNgayRoiTamNghi = TinhPhanTram(soLuongVaiNgayRoiTamNghi, tongSo);

            var soLuongDungCuoiTuan = modelInputs.Where(x => x.tansuatda.Contains("cuối")).Sum(x => x.SoLuong);
            soNguoi.DungCuoiTuan = soLuongDungCuoiTuan;
            tyLe.DungCuoiTuan = TinhPhanTram(soLuongDungCuoiTuan, tongSo);

            var soLuongVaiLan1Thang = modelInputs.Where(x => x.tansuatda.Contains("1 tháng")).Sum(x => x.SoLuong);
            soNguoi.VaiLan1Thang = soLuongVaiLan1Thang;
            tyLe.VaiLan1Thang = TinhPhanTram(soLuongVaiLan1Thang, tongSo);

            var soLuongItHon1Lan1Thang = modelInputs.Where(x => x.tansuatda.Contains("1 lần")).Sum(x => x.SoLuong);
            soNguoi.ItHon1Lan1Thang = soLuongItHon1Lan1Thang;
            tyLe.ItHon1Lan1Thang = TinhPhanTram(soLuongItHon1Lan1Thang, tongSo);

            var soLuongKBKTL = modelInputs.Where(x => x.tansuatda.Contains("KB")).Sum(x => x.SoLuong);
            soNguoi.KBKTL = soLuongKBKTL;
            tyLe.KBKTL = TinhPhanTram(soLuongKBKTL, tongSo);

            outDatas.Add(soNguoi);

            outDatas.Add(tyLe);
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo đường sử dụng ma túy đá
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoDuongSDMaTuyDa(List<KetQuaSangLocProModel> modelInputs, ref List<DuongSuDungMaTuyDaModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);


            // số người dùng từng loại 

            var soNguoiTungLoai = new DuongSuDungMaTuyDaModel
            {
                NoiDung = "Số người dùng từng loại",
            };
            // tỉ lệ % người dùng từng loại
            var tyLeNguoiTungLoai = new DuongSuDungMaTuyDaModel
            {
                NoiDung = "Tỉ lệ % người dùng từng loại",
            };

            var soLuongHutHit = modelInputs.Where(x => x.NoiDung.Contains("Hút")).Sum(x => x.SoLuong);
            soNguoiTungLoai.HutHit = soLuongHutHit;
            tyLeNguoiTungLoai.HutHit = TinhPhanTram(soLuongHutHit, tongSo);

            var soLuongDangBot = modelInputs.Where(x => x.NoiDung.Contains("bột")).Sum(x => x.SoLuong);
            soNguoiTungLoai.DangBot = soLuongDangBot;
            tyLeNguoiTungLoai.DangBot = TinhPhanTram(soLuongDangBot, tongSo);

            var soLuongUongNuot = modelInputs.Where(x => x.NoiDung.Contains("nuốt")).Sum(x => x.SoLuong);
            soNguoiTungLoai.UongNuot = soLuongUongNuot;
            tyLeNguoiTungLoai.UongNuot = TinhPhanTram(soLuongUongNuot, tongSo);

            var soLuongTiemChich = modelInputs.Where(x => x.NoiDung.Contains("Tiêm")).Sum(x => x.SoLuong);
            soNguoiTungLoai.TiemChich = soLuongTiemChich;
            tyLeNguoiTungLoai.TiemChich = TinhPhanTram(soLuongTiemChich, tongSo);

            var soLuongKBKTL = modelInputs.Where(x => x.NoiDung.Contains("KB")).Sum(x => x.SoLuong);
            soNguoiTungLoai.KBKTL = soLuongKBKTL;
            tyLeNguoiTungLoai.KBKTL = TinhPhanTram(soLuongKBKTL, tongSo);

            outDatas.Add(soNguoiTungLoai);

            outDatas.Add(tyLeNguoiTungLoai);
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo số chất gây nghiện
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoSoChatGayNghien(List<KetQuaSangLocProModel> modelInputs, ref List<KetQuaSangLocModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);


            // Chỉ 1 chất
            var soLuong1Chat = modelInputs.Where(x => Convert.ToInt32(x.NoiDung) == 1).Sum(x => x.SoLuong);
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Chỉ 1 chất",
                SoNguoi = soLuong1Chat,
                PhanTram = TinhPhanTram(soLuong1Chat, tongSo)
            });

            // 2 chất
            var soLuong2Chat = modelInputs.Where(x => Convert.ToInt32(x.NoiDung) == 2).Sum(x => x.SoLuong);
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "2 chất",
                SoNguoi = soLuong2Chat,
                PhanTram = TinhPhanTram(soLuong2Chat, tongSo)
            });

            // 3 chất
            var soLuong3Chat = modelInputs.Where(x => Convert.ToInt32(x.NoiDung) == 3).Sum(x => x.SoLuong);
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "3 chất",
                SoNguoi = soLuong3Chat,
                PhanTram = TinhPhanTram(soLuong3Chat, tongSo)
            });

            // 4 chất trở lên
            var soLuong4Chat = modelInputs.Where(x => Convert.ToInt32(x.NoiDung) >= 4).Sum(x => x.SoLuong);
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "4 chất trở lên",
                SoNguoi = soLuong4Chat,
                PhanTram = TinhPhanTram(soLuong4Chat, tongSo)
            });


            // Thêm dòng tổng số
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Tổng",
                SoNguoi = tongSo,
                PhanTram = 100
            });
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo kết quả HIV
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoKetQuaHIV(List<KetQuaSangLocProModel> modelInputs, ref List<KetQuaSangLocModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);

            // Âm tính
            var soLuongAmTinh = modelInputs.Where(x => x.ketqua == -1).Sum(x => x.SoLuong);
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Âm tính",
                SoNguoi = soLuongAmTinh,
                PhanTram = TinhPhanTram(soLuongAmTinh, tongSo)
            });

            // Dương tính mới
            var soLuongDuongTinhMoi = modelInputs.Where(x => x.ketqua == 1).Sum(x => x.SoLuong);
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Dương tính mới",
                SoNguoi = soLuongDuongTinhMoi,
                PhanTram = TinhPhanTram(soLuongDuongTinhMoi, tongSo)
            });

            // Dương tính - bỏ trị/chưa điều trị
            var soLuongDTBoChuaDT = 0;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Dương tính - bỏ trị/chưa điều trị",
                SoNguoi = soLuongDTBoChuaDT,
                PhanTram = TinhPhanTram(soLuongDTBoChuaDT, tongSo)
            });

            // Dương tính - đang điều trị
            var soLuongDTDangDT = 0;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Dương tính - đang điều trị",
                SoNguoi = soLuongDTDangDT,
                PhanTram = TinhPhanTram(soLuongDTDangDT, tongSo)
            });

            // Không xác định
            var soLuongKXD = modelInputs.Where(x => x.ketqua == 0).Sum(x => x.SoLuong);
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Không xác định",
                SoNguoi = soLuongKXD,
                PhanTram = TinhPhanTram(soLuongKXD, tongSo)
            });

            // Thêm dòng tổng số
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Tổng",
                SoNguoi = tongSo,
                PhanTram = 100
            });
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo tuổi
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoTuoi(List<KetQuaSangLocProModel> modelInputs, ref List<KetQuaSangLocModel> outDatas)
        {
            var tongSo = modelInputs.Where(x => x.tuoi >= 16 && x.tuoi <= 24).Sum(x => x.SoLuong);

            // Nhóm tuổi tù 16-18
            var soLuong1618 = modelInputs.Where(x => x.tuoi >= 16 && x.tuoi <= 18).Sum(x => x.SoLuong);
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "16-18",
                SoNguoi = soLuong1618,
                PhanTram = TinhPhanTram(soLuong1618, tongSo)
            });

            // Nhóm tuổi tù 19-22
            var soLuong1922 = modelInputs.Where(x => x.tuoi >= 19 && x.tuoi <= 22).Sum(x => x.SoLuong);
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "19-22",
                SoNguoi = soLuong1922,
                PhanTram = TinhPhanTram(soLuong1922, tongSo)
            });

            // Nhóm tuổi tù 23-24
            var soLuong2324 = modelInputs.Where(x => x.tuoi >= 23 && x.tuoi <= 24).Sum(x => x.SoLuong);
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "23-24",
                SoNguoi = soLuong2324,
                PhanTram = TinhPhanTram(soLuong2324, tongSo)
            });

            // Thêm dòng tổng số
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Tổng",
                SoNguoi = tongSo,
                PhanTram = 100
            });
        }


        /// <summary>
        /// Tính toán dữ liệu báo cáo theo đối tượng
        /// </summary>
        /// <param name="modelInputs"></param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public void ChuyenDoi_BCTheoDoiTuong_GioiTinh(List<KetQuaSangLocProModel> modelInputs, ref List<KetQuaSangLocModel> outDatas)
        {
            var tongSo = modelInputs.Sum(x => x.SoLuong);
            foreach (var item in modelInputs)
            {
                outDatas.Add(new KetQuaSangLocModel
                {
                    NoiDung = item.NoiDung,
                    SoNguoi = item.SoLuong,
                    PhanTram = TinhPhanTram(item.SoLuong, tongSo)
                });
                ;
            }
            // Thêm dòng tổng số
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Tổng",
                SoNguoi = tongSo,
                PhanTram = 100
            });
        }

        /// <summary>
        /// Tính phần trăm
        /// </summary>
        /// <param name="soLuong"></param>
        /// <param name="tongSo"></param>
        /// <returns></returns>

        public decimal TinhPhanTram(int soLuong, int tongSo)
        {
            try
            {
                return Math.Round(((100 * ((Convert.ToDecimal(soLuong * 100) / tongSo))) / 100), 2);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

    }
}
