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
                    ChuyenDoi_BCTheoDoiTuong_GioiTinh(bcDoiTuong, ref DoiTuongKHs);

                    ////////////////////Báo cáo theo giới tính////
                    var bcGioiTinh = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[1]);
                    ChuyenDoi_BCTheoDoiTuong_GioiTinh(bcGioiTinh, ref GioiTinhs);

                    ////////////////////Báo cáo theo tuổi ////
                    var bcTuoi = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[2]);
                    ChuyenDoi_BCTheoTuoi(bcTuoi, ref Tuois);


                    ////////////////////Báo cáo theo kết quả HIV ////
                    var bcKetQuaHIV = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[3]);
                    ChuyenDoi_BCTheoKetQuaHIV(bcKetQuaHIV, ref KetQuaHIVs);

                    ////////////////////Báo cáo theo Chất gây nghiện sử dụng trong 3 tháng gần đây////
                    var bcCGNSuDung3T = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[4]);
                    ChuyenDoi_BCTheoDoiTuong_GioiTinh(bcCGNSuDung3T, ref ChatGayNghien3Thangs);


                    ////////////////////Báo cáo theo Số chất gây nghiện////
                    var bcSoCGN = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[5]);
                    ChuyenDoi_BCTheoSoChatGayNghien(bcSoCGN, ref SoChatGayNghiens);

                    ////////////////////Báo cáo theo Loại chất gây nghiện sử dụng thường xuyên nhất trong 3 tháng gần đây////
                    var bcChatGayNghienSDThuongXuyen = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[6]);
                    ChuyenDoi_BCTheoDoiTuong_GioiTinh(bcChatGayNghienSDThuongXuyen, ref ChatGayNghienSDThuongXuyens);

                    ////////////////////Báo cáo theo Đường sử dụng ma túy đá////
                    var bcDuongSDMaTuyDas = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[7]);
                    ChuyenDoi_BCTheoDuongSDMaTuyDa(bcDuongSDMaTuyDas, ref DuongSDMaTuyDas);

                    ////////////////////Báo cáo theo Tần suất sử dụng ma túy đá trong 3 tháng gần đây////
                    var bcTanSuatSDMaTuyDa = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[8]);
                    ChuyenDoi_BCTheoTanSuatSDMaTuyDa(bcTanSuatSDMaTuyDa, ref TanSuatSDMaTuyDas);

                    ////////////////////Báo cáo theo Lần đầu SD ma tuý ////
                    var bcLanDauSDMaTuyDa = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[9]);
                    ChuyenDoi_BCTheoLanDauSDMaTuyDa(bcLanDauSDMaTuyDa, ref LanDauSDMaTuyDas);

                    ////////////////////Báo cáo theo Loại ma túy SD đầu tiên////
                    var bcLoaiMaTuyDaSDDauTien = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[10]);
                    ChuyenDoi_BCTheoLoaiMaTuyDaSDDauTien(bcLoaiMaTuyDaSDDauTien, ref LoaiMaTuyDaSDDauTiens);

                    ////////////////////Báo cáo theo Nguy cơ khi SD ma túy////
                    var bcNguyCoKhiSDMTD = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[11]);
                    ChuyenDoi_BCTheoNguyCoKhiSDMTD(bcNguyCoKhiSDMTD, ref NguyCoSDMaTuyDas);

                    ////////////////////Báo cáo theo Chung BKT////
                    var bcChungBKT = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[12]);
                    ChuyenDoi_BCTheoChungBKT(bcChungBKT, ref ChungBKTs);

                    ////////////////////Báo cáo theo Nguy cơ tình dục////
                    var bcNguyCoTinhDuc = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[13]);
                    ChuyenDoi_BCTheoNguyCoTinhDuc(bcNguyCoTinhDuc, ref NguyCoTinhDucs);

                    ////////////////////Báo cáo theo dùng BCS////
                    var bcDungBCS = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[14]);
                    ChuyenDoi_BCTheoDungBCS(bcDungBCS, ref DungBCSs);

                    ////////////////////Báo cáo theo SD ma túy khi QHTD ////
                    var bcSDMaTuyDaKhiQHTD = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[15]);
                    ChuyenDoi_BCTheoSDMaTuyDaKhiQHTD(bcSDMaTuyDaKhiQHTD, ref SDMaTuyDaKhiQHTDs);

                    ////////////////////Báo cáo theo QHTD tập thể ////
                    var bcQHTDTapThes = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[16]);
                    ChuyenDoi_BCTheoSDMaTuyDaKhiQHTD(bcQHTDTapThes, ref QHTDTapThes);

                    ////////////////////Báo cáo theo Bán dâm////
                    var bcBanDams = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[17]);
                    ChuyenDoi_BCTheoSDMaTuyDaKhiQHTD(bcBanDams, ref BanDams);

                    ////////////////////Báo cáo theo Nhiều nguy cơ tình dục(Qhđồng giới, ko thường xuyên sử dụng BCS, QH tập thể, QH khi sd ma túy, bán dâm) ////
                    var bcNhieuNguycoTinhDuc = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[18]);
                    ChuyenDoi_BCTheoNhieuNguycoTinhDuc(bcNhieuNguycoTinhDuc, ref NhieuNguycoTinhDucs);

                    ////////////////////Báo cáo theo Bệnh tật STI////
                    var bcBenhSTI = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[19]);
                    ChuyenDoi_BCTheoBenhSTI(bcBenhSTI, ref BenhSTIs);

                    ////////////////////Báo cáo theo Lao ////
                    var bcBenhLao = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[20]);
                    ChuyenDoi_BCTheoBenhLao_VGC(bcBenhLao, ref BenhLaos);

                    ////////////////////Báo cáo theo VGC ////
                    var bcBenhVGC = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[21]);
                    ChuyenDoi_BCTheoBenhLao_VGC(bcBenhVGC, ref BenhVGCs);


                    ////////////////////Báo cáo theo Sốc thuốc Heroin ////
                    var bcSocHeroin = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[22]);
                    ChuyenDoi_BCTheoSocHeroins(bcSocHeroin, ref SocHeroins);


                    ////////////////////Báo cáo theo Sốc thuốc Meth ////
                    var bcSocMeth = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[23]);
                    ChuyenDoi_BCTheoSocMeth(bcSocMeth, ref SocMeths);


                    ////////////////////Báo cáo theo CÁC LOẠI CHẤT GÂY NGHIỆN\KẾT QUẢ ASSIST////
                    var bcCacLoaiChatGayNghien = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[24]);
                    ChuyenDoi_BCTheoCacLoaiChatGayNghien(bcCacLoaiChatGayNghien, ref CacLoaiChatGayNghiens);

                    ////////////////////Báo cáo theo KẾT QUẢ QST(SỨC KHỎE TÂM THẦN) ////
                    var bcKetQuaQST = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[25]);
                    ChuyenDoi_BCTheoKetQuaQST(bcKetQuaQST, ref KetQuaQSTs);

                    ////////////////////Báo cáo theo Sàng lọc SKTT Mức độ gặp các vấn đề SKTT trong 2 tuần qua(câu 1)////
                    var bcMucDoGapVanDeSKTT = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[26]);
                    ChuyenDoi_BCTheoMucDoGapVanDeSKTT(bcMucDoGapVanDeSKTT, ref MucDoGapVanDeSKTTs);

                    ////////////////////Báo cáo theo Sàng lọc SKTT Ý nghĩ tự làm hại bản thân trong 2 tuần qua(câu 2) ////
                    var bcTuLamHaiBanThan = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[27]);
                    ChuyenDoi_BCTheoSDMaTuyDaKhiQHTD(bcTuLamHaiBanThan, ref TuLamHaiBanThans);

                    ////////////////////Báo cáo theo Sàng lọc SKTT Cố tự sát từ trước đến nay(câu 3)////
                    var bcCoTuSat = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[28]);
                    ChuyenDoi_BCTheoSDMaTuyDaKhiQHTD(bcCoTuSat, ref CoTuSats);

                    ////////////////////Báo cáo theo Loạn thần////
                    var bcLoanThan = _DatabaseSql.ConvertDataTableToList<KetQuaSangLocProModel>(ds.Tables[29]);
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
            decimal phanTram = 0;
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
            phanTram = soLuongTheoDoiRinhRap / tongSo;
            co.TheoDoiRinhRap_PhanTram = Math.Round(phanTram, 2);

            soLuongTheoDoiRinhRap = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_4a) && x.c_4a.Contains("Không")).Sum(x => x.SoLuong);
            khong.TheoDoiRinhRap_SoLuong = soLuongTheoDoiRinhRap;
            phanTram = soLuongTheoDoiRinhRap / tongSo;
            khong.TheoDoiRinhRap_PhanTram = Math.Round(phanTram, 2);


            // Cau c_4b
            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_4b)).Sum(x => x.SoLuong);
            soLuongTheoDoiRinhRap = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_4b) && x.c_4b.Contains("Có")).Sum(x => x.SoLuong);
            co.TheoDoiRinhRap_SoLuong = soLuongTheoDoiRinhRap;
            phanTram = soLuongTheoDoiRinhRap / tongSo;
            co.TheoDoiRinhRap_PhanTram = Math.Round(phanTram, 2);

            soLuongTheoDoiRinhRap = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_4b) && x.c_4b.Contains("Không")).Sum(x => x.SoLuong);
            khong.TheoDoiRinhRap_SoLuong = soLuongTheoDoiRinhRap;
            phanTram = soLuongTheoDoiRinhRap / tongSo;
            khong.TheoDoiRinhRap_PhanTram = Math.Round(phanTram, 2);

            // Cau c_4c
            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_4c)).Sum(x => x.SoLuong);
            soLuongTheoDoiRinhRap = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_4c) && x.c_4c.Contains("Có")).Sum(x => x.SoLuong);
            co.TheoDoiRinhRap_SoLuong = soLuongTheoDoiRinhRap;
            phanTram = soLuongTheoDoiRinhRap / tongSo;
            co.TheoDoiRinhRap_PhanTram = Math.Round(phanTram, 2);

            soLuongTheoDoiRinhRap = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_4c) && x.c_4c.Contains("Không")).Sum(x => x.SoLuong);
            khong.TheoDoiRinhRap_SoLuong = soLuongTheoDoiRinhRap;
            phanTram = soLuongTheoDoiRinhRap / tongSo;
            khong.TheoDoiRinhRap_PhanTram = Math.Round(phanTram, 2);

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
            decimal phanTram = 0;
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
            phanTram = soLuongKhongChutNao / tongSo;
            loLangCangThang.KhongChutNao_PhanTram = Math.Round(phanTram, 2);

            soLuong1_7Ngay = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1a) && x.c_1a.Contains("Từ 1")).Sum(x => x.SoLuong);
            loLangCangThang.Tu1Den7Ngay_SoLuong = soLuong1_7Ngay;
            phanTram = soLuong1_7Ngay / tongSo;
            loLangCangThang.Tu1Den7Ngay_PhanTram = Math.Round(phanTram, 2);

            soLuong8NgayTroLen = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1a) && x.c_1a.Contains("Từ 8")).Sum(x => x.SoLuong);
            loLangCangThang.Tu8NgayTroLen_SoLuong = soLuong8NgayTroLen;
            phanTram = soLuong8NgayTroLen / tongSo;
            loLangCangThang.Tu8NgayTroLen_PhanTram = Math.Round(phanTram, 2);

            soLuongGanNhuHangNgay = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1a) && x.c_1a.Contains("gần")).Sum(x => x.SoLuong);
            loLangCangThang.GanNhuHangNgay_SoLuong = soLuongGanNhuHangNgay;
            phanTram = soLuongGanNhuHangNgay / tongSo;
            loLangCangThang.GanNhuHangNgay_PhanTram = Math.Round(phanTram, 2);

            outDatas.Add(loLangCangThang);

            // Lo âu tới mức ko kiểm soát được
            var loAu = new MucDoGapVanDeSKTTModel
            {
                NoiDung = "Lo âu tới mức ko kiểm soát được",
            };
            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1b)).Sum(x => x.SoLuong);
            soLuongKhongChutNao = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1b) && x.c_1b.Contains("Không")).Sum(x => x.SoLuong);
            loAu.KhongChutNao_SoLuong = soLuongKhongChutNao;
            phanTram = soLuongKhongChutNao / tongSo;
            loAu.KhongChutNao_PhanTram = Math.Round(phanTram, 2);

            soLuong1_7Ngay = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1b) && x.c_1b.Contains("Từ 1")).Sum(x => x.SoLuong);
            loAu.Tu1Den7Ngay_SoLuong = soLuong1_7Ngay;
            phanTram = soLuong1_7Ngay / tongSo;
            loAu.Tu1Den7Ngay_PhanTram = Math.Round(phanTram, 2);

            soLuong8NgayTroLen = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1b) && x.c_1b.Contains("Từ 8")).Sum(x => x.SoLuong);
            loAu.Tu8NgayTroLen_SoLuong = soLuong8NgayTroLen;
            phanTram = soLuong8NgayTroLen / tongSo;
            loAu.Tu8NgayTroLen_PhanTram = Math.Round(phanTram, 2);

            soLuongGanNhuHangNgay = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1b) && x.c_1b.Contains("gần")).Sum(x => x.SoLuong);
            loAu.GanNhuHangNgay_SoLuong = soLuongGanNhuHangNgay;
            phanTram = soLuongGanNhuHangNgay / tongSo;
            loAu.GanNhuHangNgay_PhanTram = Math.Round(phanTram, 2);
            outDatas.Add(loAu);
            // Cảm thấy buồn chán, mất hết hy vọng
            var buonChan = new MucDoGapVanDeSKTTModel
            {
                NoiDung = "Cảm thấy buồn chán, mất hết hy vọng",
            };
            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1c)).Sum(x => x.SoLuong);
            soLuongKhongChutNao = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1c) && x.c_1c.Contains("Không")).Sum(x => x.SoLuong);
            buonChan.KhongChutNao_SoLuong = soLuongKhongChutNao;
            phanTram = soLuongKhongChutNao / tongSo;
            buonChan.KhongChutNao_PhanTram = Math.Round(phanTram, 2);

            soLuong1_7Ngay = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1c) && x.c_1c.Contains("Từ 1")).Sum(x => x.SoLuong);
            buonChan.Tu1Den7Ngay_SoLuong = soLuong1_7Ngay;
            phanTram = soLuong1_7Ngay / tongSo;
            buonChan.Tu1Den7Ngay_PhanTram = Math.Round(phanTram, 2);

            soLuong8NgayTroLen = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1c) && x.c_1c.Contains("Từ 8")).Sum(x => x.SoLuong);
            buonChan.Tu8NgayTroLen_SoLuong = soLuong8NgayTroLen;
            phanTram = soLuong8NgayTroLen / tongSo;
            buonChan.Tu8NgayTroLen_PhanTram = Math.Round(phanTram, 2);

            soLuongGanNhuHangNgay = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1c) && x.c_1c.Contains("gần")).Sum(x => x.SoLuong);
            buonChan.GanNhuHangNgay_SoLuong = soLuongGanNhuHangNgay;
            phanTram = soLuongGanNhuHangNgay / tongSo;
            buonChan.GanNhuHangNgay_PhanTram = Math.Round(phanTram, 2);
            outDatas.Add(buonChan);

            // Ít quan tâm hoặc ít hứng thú với mọi thứ
            var itQuanTam = new MucDoGapVanDeSKTTModel
            {
                NoiDung = "Ít quan tâm hoặc ít hứng thú với mọi thứ",
            };
            tongSo = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1d)).Sum(x => x.SoLuong);
            soLuongKhongChutNao = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1d) && x.c_1d.Contains("Không")).Sum(x => x.SoLuong);
            itQuanTam.KhongChutNao_SoLuong = soLuongKhongChutNao;
            phanTram = soLuongKhongChutNao / tongSo;
            itQuanTam.KhongChutNao_PhanTram = Math.Round(phanTram, 2);

            soLuong1_7Ngay = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1d) && x.c_1d.Contains("Từ 1")).Sum(x => x.SoLuong);
            itQuanTam.Tu1Den7Ngay_SoLuong = soLuong1_7Ngay;
            phanTram = soLuong1_7Ngay / tongSo;
            itQuanTam.Tu1Den7Ngay_PhanTram = Math.Round(phanTram, 2);

            soLuong8NgayTroLen = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1d) && x.c_1d.Contains("Từ 8")).Sum(x => x.SoLuong);
            itQuanTam.Tu8NgayTroLen_SoLuong = soLuong8NgayTroLen;
            phanTram = soLuong8NgayTroLen / tongSo;
            itQuanTam.Tu8NgayTroLen_PhanTram = Math.Round(phanTram, 2);

            soLuongGanNhuHangNgay = modelInputs.Where(x => !string.IsNullOrEmpty(x.c_1d) && x.c_1d.Contains("gần")).Sum(x => x.SoLuong);
            itQuanTam.GanNhuHangNgay_SoLuong = soLuongGanNhuHangNgay;
            phanTram = soLuongGanNhuHangNgay / tongSo;
            itQuanTam.GanNhuHangNgay_PhanTram = Math.Round(phanTram, 2);
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
            decimal phanTram = 0;

            foreach (var item in modelInputs)
            {
                phanTram = item.SoLuong / tongSo;
                outDatas.Add(new KetQuaSangLocModel
                {
                    NoiDung = item.tongdiem+ " điểm",
                    SoNguoi = item.SoLuong,
                    PhanTram = Math.Round(phanTram, 2)
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
            decimal phanTram = 0;
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
            phanTram = soLuongNguyCoThap / tongSo;
            thuocLa.NguyCoThap_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemthuocla) && Convert.ToInt32(x.diemthuocla) >= 4 && Convert.ToInt32(x.diemthuocla) <= 27).Sum(x => x.SoLuong);
            thuocLa.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;
            phanTram = soLuongNguyCoTrungBinh / tongSo;
            thuocLa.NguyCoTrungBinh_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemthuocla) && Convert.ToInt32(x.diemthuocla) > 27).Sum(x => x.SoLuong);
            thuocLa.NguyCoCao_SoLuong = soLuongNguyCoCao;
            phanTram = soLuongNguyCoCao / tongSo;
            thuocLa.NguyCoCao_PhanTram = Math.Round(phanTram, 2);

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
            phanTram = soLuongNguyCoThap / tongSo;
            conRuou.NguyCoThap_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemthucuong) && Convert.ToInt32(x.diemthucuong) >= 4 && Convert.ToInt32(x.diemthucuong) <= 27).Sum(x => x.SoLuong);
            conRuou.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;
            phanTram = soLuongNguyCoTrungBinh / tongSo;
            conRuou.NguyCoTrungBinh_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemthucuong) && Convert.ToInt32(x.diemthucuong) > 27).Sum(x => x.SoLuong);
            conRuou.NguyCoCao_SoLuong = soLuongNguyCoCao;
            phanTram = soLuongNguyCoCao / tongSo;
            conRuou.NguyCoCao_PhanTram = Math.Round(phanTram, 2);

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
            phanTram = soLuongNguyCoThap / tongSo;
            canSa.NguyCoThap_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemcansa) && Convert.ToInt32(x.diemcansa) >= 4 && Convert.ToInt32(x.diemcansa) <= 27).Sum(x => x.SoLuong);
            canSa.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;
            phanTram = soLuongNguyCoTrungBinh / tongSo;
            canSa.NguyCoTrungBinh_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemcansa) && Convert.ToInt32(x.diemcansa) > 27).Sum(x => x.SoLuong);
            canSa.NguyCoCao_SoLuong = soLuongNguyCoCao;
            phanTram = soLuongNguyCoCao / tongSo;
            canSa.NguyCoCao_PhanTram = Math.Round(phanTram, 2);

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
            phanTram = soLuongNguyCoThap / tongSo;
            cocaine.NguyCoThap_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemcoca) && Convert.ToInt32(x.diemcoca) >= 4 && Convert.ToInt32(x.diemcoca) <= 27).Sum(x => x.SoLuong);
            cocaine.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;
            phanTram = soLuongNguyCoTrungBinh / tongSo;
            cocaine.NguyCoTrungBinh_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemcoca) && Convert.ToInt32(x.diemcoca) > 27).Sum(x => x.SoLuong);
            cocaine.NguyCoCao_SoLuong = soLuongNguyCoCao;
            phanTram = soLuongNguyCoCao / tongSo;
            cocaine.NguyCoCao_PhanTram = Math.Round(phanTram, 2);

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
            phanTram = soLuongNguyCoThap / tongSo;
            maTuyDa.NguyCoThap_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatkichthich) && Convert.ToInt32(x.diemchatkichthich) >= 4 && Convert.ToInt32(x.diemchatkichthich) <= 27).Sum(x => x.SoLuong);
            maTuyDa.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;
            phanTram = soLuongNguyCoTrungBinh / tongSo;
            maTuyDa.NguyCoTrungBinh_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatkichthich) && Convert.ToInt32(x.diemchatkichthich) > 27).Sum(x => x.SoLuong);
            maTuyDa.NguyCoCao_SoLuong = soLuongNguyCoCao;
            phanTram = soLuongNguyCoCao / tongSo;
            maTuyDa.NguyCoCao_PhanTram = Math.Round(phanTram, 2);

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
            phanTram = soLuongNguyCoThap / tongSo;
            khiXongHit.NguyCoThap_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemkhixong) && Convert.ToInt32(x.diemkhixong) >= 4 && Convert.ToInt32(x.diemkhixong) <= 27).Sum(x => x.SoLuong);
            khiXongHit.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;
            phanTram = soLuongNguyCoTrungBinh / tongSo;
            khiXongHit.NguyCoTrungBinh_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemkhixong) && Convert.ToInt32(x.diemkhixong) > 27).Sum(x => x.SoLuong);
            khiXongHit.NguyCoCao_SoLuong = soLuongNguyCoCao;
            phanTram = soLuongNguyCoCao / tongSo;
            khiXongHit.NguyCoCao_PhanTram = Math.Round(phanTram, 2);

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
            phanTram = soLuongNguyCoThap / tongSo;
            thuocAnThan.NguyCoThap_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatanthan) && Convert.ToInt32(x.diemchatanthan) >= 4 && Convert.ToInt32(x.diemchatanthan) <= 27).Sum(x => x.SoLuong);
            thuocAnThan.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;
            phanTram = soLuongNguyCoTrungBinh / tongSo;
            thuocAnThan.NguyCoTrungBinh_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatanthan) && Convert.ToInt32(x.diemchatanthan) > 27).Sum(x => x.SoLuong);
            thuocAnThan.NguyCoCao_SoLuong = soLuongNguyCoCao;
            phanTram = soLuongNguyCoCao / tongSo;
            thuocAnThan.NguyCoCao_PhanTram = Math.Round(phanTram, 2);

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
            phanTram = soLuongNguyCoThap / tongSo;
            chatGayAoGiac.NguyCoThap_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatgayaogiac) && Convert.ToInt32(x.diemchatgayaogiac) >= 4 && Convert.ToInt32(x.diemchatgayaogiac) <= 27).Sum(x => x.SoLuong);
            chatGayAoGiac.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;
            phanTram = soLuongNguyCoTrungBinh / tongSo;
            chatGayAoGiac.NguyCoTrungBinh_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatgayaogiac) && Convert.ToInt32(x.diemchatgayaogiac) > 27).Sum(x => x.SoLuong);
            chatGayAoGiac.NguyCoCao_SoLuong = soLuongNguyCoCao;
            phanTram = soLuongNguyCoCao / tongSo;
            chatGayAoGiac.NguyCoCao_PhanTram = Math.Round(phanTram, 2);

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
            phanTram = soLuongNguyCoThap / tongSo;
            chatThuocPhien.NguyCoThap_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatthuocphien) && Convert.ToInt32(x.diemchatthuocphien) >= 4 && Convert.ToInt32(x.diemchatthuocphien) <= 27).Sum(x => x.SoLuong);
            chatThuocPhien.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;
            phanTram = soLuongNguyCoTrungBinh / tongSo;
            chatThuocPhien.NguyCoTrungBinh_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatthuocphien) && Convert.ToInt32(x.diemchatthuocphien) > 27).Sum(x => x.SoLuong);
            chatThuocPhien.NguyCoCao_SoLuong = soLuongNguyCoCao;
            phanTram = soLuongNguyCoCao / tongSo;
            chatThuocPhien.NguyCoCao_PhanTram = Math.Round(phanTram, 2);

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
            phanTram = soLuongNguyCoThap / tongSo;
            cacThuocKhac.NguyCoThap_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoTrungBinh = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatkhac) && Convert.ToInt32(x.diemchatkhac) >= 4 && Convert.ToInt32(x.diemchatkhac) <= 27).Sum(x => x.SoLuong);
            cacThuocKhac.NguyCoTrungBinh_SoLuong = soLuongNguyCoTrungBinh;
            phanTram = soLuongNguyCoTrungBinh / tongSo;
            cacThuocKhac.NguyCoTrungBinh_PhanTram = Math.Round(phanTram, 2);

            soLuongNguyCoCao = modelInputs.Where(x => !string.IsNullOrEmpty(x.diemchatkhac) && Convert.ToInt32(x.diemchatkhac) > 27).Sum(x => x.SoLuong);
            cacThuocKhac.NguyCoCao_SoLuong = soLuongNguyCoCao;
            phanTram = soLuongNguyCoCao / tongSo;
            cacThuocKhac.NguyCoCao_PhanTram = Math.Round(phanTram, 2);

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
            decimal phanTram = 0;

            // Hoang tưởng/ ảo giác
            var soLuongHoangTuong = modelInputs.Where(x => x.trieuchung_2.Contains("Hoang")).Sum(x => x.SoLuong);
            phanTram = soLuongHoangTuong / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Hoang tưởng/ ảo giác",
                SoNguoi = soLuongHoangTuong,
                PhanTram = Math.Round(phanTram, 2)
            });

            // Buồn nôn/ nôn mửa      
            var soLuongBuonNon = modelInputs.Where(x => x.trieuchung_2.Contains("Buồn")).Sum(x => x.SoLuong);
            phanTram = soLuongBuonNon / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Buồn nôn/ nôn mửa",
                SoNguoi = soLuongBuonNon,
                PhanTram = Math.Round(phanTram, 2)
            });

            // Run rẩy/ co giật
            var soLuongRunRay = modelInputs.Where(x => x.trieuchung_2.Contains("Run")).Sum(x => x.SoLuong);
            phanTram = soLuongRunRay / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Run rẩy/ co giật",
                SoNguoi = soLuongRunRay,
                PhanTram = Math.Round(phanTram, 2)
            });

            // Cảm thấy tê liệt nhưng vẫn tỉnh
            var soLuongCam = modelInputs.Where(x => x.trieuchung_2.Contains("Cảm")).Sum(x => x.SoLuong);
            phanTram = soLuongCam / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Cảm thấy tê liệt nhưng vẫn tỉnh",
                SoNguoi = soLuongCam,
                PhanTram = Math.Round(phanTram, 2)
            });

            // Lo âu/ cơn hoảng loạn
            var soLuongLoAu = modelInputs.Where(x => x.trieuchung_2.Contains("Lo")).Sum(x => x.SoLuong);
            phanTram = soLuongLoAu / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Lo âu/ cơn hoảng loạn",
                SoNguoi = soLuongLoAu,
                PhanTram = Math.Round(phanTram, 2)
            });

            // Đau ngực hoặc căng tức ngực
            var soLuongDau = modelInputs.Where(x => x.trieuchung_2.Contains("Đau")).Sum(x => x.SoLuong);
            phanTram = soLuongDau / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Đau ngực hoặc căng tức ngực",
                SoNguoi = soLuongDau,
                PhanTram = Math.Round(phanTram, 2)
            });

            //Đột quỵ 
            var soLuongDột = modelInputs.Where(x => x.trieuchung_2.Contains("Đột")).Sum(x => x.SoLuong);
            phanTram = soLuongDột / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Đột quỵ",
                SoNguoi = soLuongDột,
                PhanTram = Math.Round(phanTram, 2)
            });

            // Tim đập rất nhanh
            var soLuongTim = modelInputs.Where(x => x.trieuchung_2.Contains("Tim")).Sum(x => x.SoLuong);
            phanTram = soLuongTim / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Tim đập rất nhanh",
                SoNguoi = soLuongTim,
                PhanTram = Math.Round(phanTram, 2)
            });

            // Chưa từng gặp triệu chứng nào
            var soLuongChua = modelInputs.Where(x => x.trieuchung_2.Contains("Chưa từng")).Sum(x => x.SoLuong);
            phanTram = soLuongChua / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Chưa từng gặp triệu chứng nào",
                SoNguoi = soLuongChua,
                PhanTram = Math.Round(phanTram, 2)
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
            decimal phanTram = 0;
            var soLuongKhongGap = modelInputs.Where(x => x.trieuchung.Contains("không gặp")).Sum(x => x.SoLuong);
            // Từng có một trong các triệu chứng

            phanTram = (tongSo - soLuongKhongGap) / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Từng có một trong các triệu chứng",
                SoNguoi = (tongSo - soLuongKhongGap),
                PhanTram = Math.Round(phanTram, 2)
            });

            // Chưa từng gặp triệu chứng nào
            phanTram = soLuongKhongGap / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Chưa từng gặp triệu chứng nào",
                SoNguoi = soLuongKhongGap,
                PhanTram = Math.Round(phanTram, 2)
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
            decimal phanTram = 0;

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

            phanTram = soLuongHTCo / tongSo;
            co.HienTai_SoLuong = soLuongHTCo;
            co.HienTai_PhanTram = Math.Round(phanTram, 2);
            co.QuaKhu_SoLuong = soLuongQKCo;
            phanTram = soLuongQKCo / tongSo;
            co.QuaKhu_PhanTram = Math.Round(phanTram, 2);

            var soLuongHTKhong = modelInputs.Where(x => x.NoiDung_HT.Contains("Không")).Sum(x => x.SoLuong);
            var soLuongQKKhong = modelInputs.Where(x => x.NoiDung_QK.Contains("Không")).Sum(x => x.SoLuong);
            phanTram = soLuongHTKhong / tongSo;
            khong.HienTai_SoLuong = soLuongHTKhong;
            khong.HienTai_PhanTram = Math.Round(phanTram, 2);
            khong.QuaKhu_SoLuong = soLuongQKKhong;
            phanTram = soLuongQKKhong / tongSo;
            khong.QuaKhu_PhanTram = Math.Round(phanTram, 2);

            var soLuongHTKB = modelInputs.Where(x => x.NoiDung_HT.Contains("KB")).Sum(x => x.SoLuong);
            var soLuongQKKB = modelInputs.Where(x => x.NoiDung_QK.Contains("KB")).Sum(x => x.SoLuong);
            phanTram = soLuongHTKB / tongSo;
            kbktl.HienTai_SoLuong = soLuongHTKB;
            kbktl.HienTai_PhanTram = Math.Round(phanTram, 2);
            kbktl.QuaKhu_SoLuong = soLuongQKKB;
            phanTram = soLuongQKKB / tongSo;
            kbktl.QuaKhu_PhanTram = Math.Round(phanTram, 2);

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
            decimal phanTram = 0;

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
            phanTram = soLuongLau / tongSo;
            soNguoi.Lau = soLuongLau;
            tyLe.Lau = Math.Round(phanTram, 2);

            var soLuongSuiMaoGa = modelInputs.Where(x => x.sti1.Contains("SÙI")).Sum(x => x.SoLuong);
            phanTram = soLuongSuiMaoGa / tongSo;
            soNguoi.SuiMaoGa = soLuongSuiMaoGa;
            tyLe.SuiMaoGa = Math.Round(phanTram, 2);

            var soLuongKhongMac = modelInputs.Where(x => x.sti1.Contains("KHÔNG")).Sum(x => x.SoLuong);
            phanTram = soLuongKhongMac / tongSo;
            soNguoi.KhongMac = soLuongKhongMac;
            tyLe.KhongMac = Math.Round(phanTram, 2);

            var soLuongKhac = modelInputs.Where(x => x.sti1.Contains("KHÁC")).Sum(x => x.SoLuong);
            phanTram = soLuongKhac / tongSo;
            soNguoi.Khac = soLuongKhac;
            tyLe.Khac = Math.Round(phanTram, 2);

            var soLuongGiangMai = modelInputs.Where(x => x.sti1.Contains("GIANG")).Sum(x => x.SoLuong);
            phanTram = soLuongGiangMai / tongSo;
            soNguoi.GiangMai = soLuongGiangMai;
            tyLe.GiangMai = Math.Round(phanTram, 2);

            var soLuongKBKTL = modelInputs.Where(x => x.sti1.Contains("KB")).Sum(x => x.SoLuong);
            phanTram = soLuongKBKTL / tongSo;
            soNguoi.KBKTL = soLuongKBKTL;
            tyLe.KBKTL = Math.Round(phanTram, 2);

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
            decimal phanTram = 0;

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
            phanTram = soLuong1NC / tongSo;
            soNguoi.Mot = soLuong1NC;
            tyLe.Mot = Math.Round(phanTram, 2);

            var soLuongHai = modelInputs.Where(x => x.NoiDung.Contains("2")).Sum(x => x.SoLuong);
            phanTram = soLuongHai / tongSo;
            soNguoi.Hai = soLuongHai;
            tyLe.Hai = Math.Round(phanTram, 2);

            var soLuongBa = modelInputs.Where(x => x.NoiDung.Contains("3")).Sum(x => x.SoLuong);
            phanTram = soLuongBa / tongSo;
            soNguoi.Ba = soLuongBa;
            tyLe.Ba = Math.Round(phanTram, 2);

            var soLuongBonNam = modelInputs.Where(x => Convert.ToInt32(x.NoiDung) > 3).Sum(x => x.SoLuong);
            phanTram = soLuongBonNam / tongSo;
            soNguoi.BonNam = soLuongBonNam;
            tyLe.BonNam = Math.Round(phanTram, 2);

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
            decimal phanTram = 0;

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
            phanTram = soLuongCo / tongSo;
            soNguoi.Co = soLuongCo;
            tyLe.Co = Math.Round(phanTram, 2);

            var soLuongKhong = modelInputs.Where(x => x.NoiDung.Contains("không") || x.NoiDung.Contains("Không")).Sum(x => x.SoLuong);
            phanTram = soLuongKhong / tongSo;
            soNguoi.Khong = soLuongKhong;
            tyLe.Khong = Math.Round(phanTram, 2);

            var soLuongKBKTL = modelInputs.Where(x => x.NoiDung.Contains("kB") || x.NoiDung.Contains("KB")).Sum(x => x.SoLuong);
            phanTram = soLuongKBKTL / tongSo;
            soNguoi.KBKTL = soLuongKBKTL;
            tyLe.KBKTL = Math.Round(phanTram, 2);

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
            decimal phanTram = 0;

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
            phanTram = soLuongLuonLuon / tongSo;
            soNguoi.LuonLuon = soLuongLuonLuon;
            tyLe.LuonLuon = Math.Round(phanTram, 2);

            var soLuongThuongXuyen = modelInputs.Where(x => x.qhtd_2.Contains("thường") || x.qhtd_2.Contains("Thường")).Sum(x => x.SoLuong);
            phanTram = soLuongThuongXuyen / tongSo;
            soNguoi.ThuongXuyen = soLuongThuongXuyen;
            tyLe.ThuongXuyen = Math.Round(phanTram, 2);

            var soLuongThiThoang = modelInputs.Where(x => x.qhtd_2.Contains("thỉnh") || x.qhtd_2.Contains("Thỉnh")).Sum(x => x.SoLuong);
            phanTram = soLuongThiThoang / tongSo;
            soNguoi.ThiThoang = soLuongThiThoang;
            tyLe.ThiThoang = Math.Round(phanTram, 2);

            var soLuongHiemKhi = modelInputs.Where(x => x.qhtd_2.Contains("hiếm") || x.qhtd_2.Contains("Hiếm")).Sum(x => x.SoLuong);
            phanTram = soLuongHiemKhi / tongSo;
            soNguoi.HiemKhi = soLuongHiemKhi;
            tyLe.HiemKhi = Math.Round(phanTram, 2);

            var soLuongKhongBaoGio = modelInputs.Where(x => x.qhtd_2.Contains("không bao") || x.qhtd_2.Contains("Không bao")).Sum(x => x.SoLuong);
            phanTram = soLuongKhongBaoGio / tongSo;
            soNguoi.KhongBaoGio = soLuongKhongBaoGio;
            tyLe.KhongBaoGio = Math.Round(phanTram, 2);

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
            decimal phanTram = 0;

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
            phanTram = soLuongChuaBaoGio / tongSo;
            soNguoi.ChuaBaoGio = soLuongChuaBaoGio;
            tyLe.ChuaBaoGio = Math.Round(phanTram, 2);

            var soLuongDongGioi = modelInputs.Where(x => x.qhtd.Contains("đồng") || x.qhtd.Contains("Đồng")).Sum(x => x.SoLuong);
            phanTram = soLuongDongGioi / tongSo;
            soNguoi.DongGioi = soLuongDongGioi;
            tyLe.DongGioi = Math.Round(phanTram, 2);

            var soLuongKhacGioi = modelInputs.Where(x => x.qhtd.Contains("khác") || x.qhtd.Contains("Khác")).Sum(x => x.SoLuong);
            phanTram = soLuongKhacGioi / tongSo;
            soNguoi.KhacGioi = soLuongKhacGioi;
            tyLe.KhacGioi = Math.Round(phanTram, 2);

            var soLuongCaHai = modelInputs.Where(x => x.qhtd.Contains(",")).Sum(x => x.SoLuong);
            phanTram = soLuongCaHai / tongSo;
            soNguoi.CaHai = soLuongCaHai;
            tyLe.CaHai = Math.Round(phanTram, 2);

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
            decimal phanTram = 0;

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
            phanTram = soLuongDaTungDungChung / tongSo;
            soNguoi.DaTungDungChung = soLuongDaTungDungChung;
            tyLe.DaTungDungChung = Math.Round(phanTram, 2);


            var soLuongChuaBaoGio = modelInputs.Where(x => x.dungchung.Contains("chưa bao") || x.dungchung.Contains("Chưa bao")).Sum(x => x.SoLuong);
            phanTram = soLuongChuaBaoGio / tongSo;
            soNguoi.ChuaBaoGio = soLuongChuaBaoGio;
            tyLe.ChuaBaoGio = Math.Round(phanTram, 2);

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
            decimal phanTram = 0;

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
            phanTram = soLuongChuaBaoGio / tongSo;
            soNguoi.ChuaBaoGio = soLuongChuaBaoGio;
            tyLe.ChuaBaoGio = Math.Round(phanTram, 2);

            var soLuongDaTungTiemChich = modelInputs.Where(x => x.tiemchich.Contains("đã") || x.tiemchich.Contains("Đã")).Sum(x => x.SoLuong);
            phanTram = soLuongDaTungTiemChich / tongSo;
            soNguoi.DaTungTiemChich = soLuongDaTungTiemChich;
            tyLe.DaTungTiemChich = Math.Round(phanTram, 2);

            var soLuongVanDangTiemChich = modelInputs.Where(x => x.tiemchich.Contains("vẫn") || x.tiemchich.Contains("Vẫn")).Sum(x => x.SoLuong);
            phanTram = soLuongVanDangTiemChich / tongSo;
            soNguoi.VanDangTiemChich = soLuongVanDangTiemChich;
            tyLe.VanDangTiemChich = Math.Round(phanTram, 2);

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
            decimal phanTram = 0;

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
            phanTram = soLuongDa / tongSo;
            soNguoi.Da = soLuongDa;
            tyLe.Da = Math.Round(phanTram, 2);

            var soLuongKeo = modelInputs.Where(x => x.matuydautien.Contains("keo") || x.matuydautien.Contains("Keo")).Sum(x => x.SoLuong);
            phanTram = soLuongKeo / tongSo;
            soNguoi.Keo = soLuongKeo;
            tyLe.Keo = Math.Round(phanTram, 2);

            var soLuongCanCo = modelInputs.Where(x => x.matuydautien.Contains("cần") || x.matuydautien.Contains("Cần") || x.matuydautien.Contains("cỏ") || x.matuydautien.Contains("Cỏ")).Sum(x => x.SoLuong);
            phanTram = soLuongCanCo / tongSo;
            soNguoi.CanCo = soLuongCanCo;
            tyLe.CanCo = Math.Round(phanTram, 2);

            var soLuongKetamin = modelInputs.Where(x => x.matuydautien.Contains("ketamin") || x.matuydautien.Contains("Ketamin")).Sum(x => x.SoLuong);
            phanTram = soLuongKetamin / tongSo;
            soNguoi.Ketamin = soLuongKetamin;
            tyLe.Ketamin = Math.Round(phanTram, 2);

            var soLuongBongCuoi = modelInputs.Where(x => x.matuydautien.Contains("bóng") || x.matuydautien.Contains("Bóng")).Sum(x => x.SoLuong);
            phanTram = soLuongBongCuoi / tongSo;
            soNguoi.BongCuoi = soLuongBongCuoi;
            tyLe.BongCuoi = Math.Round(phanTram, 2);

            var soLuongHeroin = modelInputs.Where(x => x.matuydautien.Contains("heroin") || x.matuydautien.Contains("Heroin")).Sum(x => x.SoLuong);
            phanTram = soLuongHeroin / tongSo;
            soNguoi.Heroin = soLuongHeroin;
            tyLe.Heroin = Math.Round(phanTram, 2);

            var soLuongCacChatHit = modelInputs.Where(x => x.matuydautien.Contains("hít") || x.matuydautien.Contains("Hít")).Sum(x => x.SoLuong);
            phanTram = soLuongCacChatHit / tongSo;
            soNguoi.CacChatHit = soLuongCacChatHit;
            tyLe.CacChatHit = Math.Round(phanTram, 2);

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
            decimal phanTram = 0;

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
            phanTram = soLuong13Tuoi / tongSo;
            soNguoi._13 = soLuong13Tuoi;
            tyLe._13 = Math.Round(phanTram, 2);

            var soLuong14Tuoi = modelInputs.Where(x => x.tuoi == 14).Sum(x => x.SoLuong);
            phanTram = soLuong14Tuoi / tongSo;
            soNguoi._14 = soLuong14Tuoi;
            tyLe._14 = Math.Round(phanTram, 2);

            var soLuong15Tuoi = modelInputs.Where(x => x.tuoi == 15).Sum(x => x.SoLuong);
            phanTram = soLuong15Tuoi / tongSo;
            soNguoi._15 = soLuong15Tuoi;
            tyLe._15 = Math.Round(phanTram, 2);

            var soLuong16Tuoi = modelInputs.Where(x => x.tuoi == 16).Sum(x => x.SoLuong);
            phanTram = soLuong16Tuoi / tongSo;
            soNguoi._16 = soLuong16Tuoi;
            tyLe._16 = Math.Round(phanTram, 2);

            var soLuong17Tuoi = modelInputs.Where(x => x.tuoi == 17).Sum(x => x.SoLuong);
            phanTram = soLuong17Tuoi / tongSo;
            soNguoi._17 = soLuong17Tuoi;
            tyLe._17 = Math.Round(phanTram, 2);

            var soLuong18Tuoi = modelInputs.Where(x => x.tuoi == 18).Sum(x => x.SoLuong);
            phanTram = soLuong18Tuoi / tongSo;
            soNguoi._18 = soLuong18Tuoi;
            tyLe._18 = Math.Round(phanTram, 2);

            var soLuong19Tuoi = modelInputs.Where(x => x.tuoi == 19).Sum(x => x.SoLuong);
            phanTram = soLuong19Tuoi / tongSo;
            soNguoi._19 = soLuong19Tuoi;
            tyLe._19 = Math.Round(phanTram, 2);

            var soLuong20Tuoi = modelInputs.Where(x => x.tuoi == 20).Sum(x => x.SoLuong);
            phanTram = soLuong20Tuoi / tongSo;
            soNguoi._20 = soLuong20Tuoi;
            tyLe._20 = Math.Round(phanTram, 2);

            var soLuong21Tuoi = modelInputs.Where(x => x.tuoi == 21).Sum(x => x.SoLuong);
            phanTram = soLuong21Tuoi / tongSo;
            soNguoi._21 = soLuong21Tuoi;
            tyLe._21 = Math.Round(phanTram, 2);

            var soLuong22Tuoi = modelInputs.Where(x => x.tuoi == 22).Sum(x => x.SoLuong);
            phanTram = soLuong22Tuoi / tongSo;
            soNguoi._22 = soLuong22Tuoi;
            tyLe._22 = Math.Round(phanTram, 2);

            var soLuong23Tuoi = modelInputs.Where(x => x.tuoi == 23).Sum(x => x.SoLuong);
            phanTram = soLuong23Tuoi / tongSo;
            soNguoi._23 = soLuong23Tuoi;
            tyLe._23 = Math.Round(phanTram, 2);

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
            decimal phanTram = 0;

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
            phanTram = soLuong1Ngay / tongSo;
            soNguoi.VaiLan1Ngay = soLuong1Ngay;
            tyLe.VaiLan1Ngay = Math.Round(phanTram, 2);

            var soLuongHangNgay = modelInputs.Where(x => x.tansuatda.Contains("Hàng")).Sum(x => x.SoLuong);
            phanTram = soLuongHangNgay / tongSo;
            soNguoi.HangNgay = soLuongHangNgay;
            tyLe.HangNgay = Math.Round(phanTram, 2);

            var soLuongVaiLan1Tuan = modelInputs.Where(x => x.tansuatda.Contains("1 tuần")).Sum(x => x.SoLuong);
            phanTram = soLuongVaiLan1Tuan / tongSo;
            soNguoi.VaiLan1Tuan = soLuongVaiLan1Tuan;
            tyLe.VaiLan1Tuan = Math.Round(phanTram, 2);

            var soLuongVaiNgayRoiTamNghi = modelInputs.Where(x => x.tansuatda.Contains("nghỉ")).Sum(x => x.SoLuong);
            phanTram = soLuongVaiNgayRoiTamNghi / tongSo;
            soNguoi.VaiNgayRoiTamNghi = soLuongVaiNgayRoiTamNghi;
            tyLe.VaiNgayRoiTamNghi = Math.Round(phanTram, 2);

            var soLuongDungCuoiTuan = modelInputs.Where(x => x.tansuatda.Contains("cuối")).Sum(x => x.SoLuong);
            phanTram = soLuongDungCuoiTuan / tongSo;
            soNguoi.DungCuoiTuan = soLuongDungCuoiTuan;
            tyLe.DungCuoiTuan = Math.Round(phanTram, 2);

            var soLuongVaiLan1Thang = modelInputs.Where(x => x.tansuatda.Contains("1 tháng")).Sum(x => x.SoLuong);
            phanTram = soLuongVaiLan1Thang / tongSo;
            soNguoi.VaiLan1Thang = soLuongVaiLan1Thang;
            tyLe.VaiLan1Thang = Math.Round(phanTram, 2);

            var soLuongItHon1Lan1Thang = modelInputs.Where(x => x.tansuatda.Contains("1 lần")).Sum(x => x.SoLuong);
            phanTram = soLuongItHon1Lan1Thang / tongSo;
            soNguoi.ItHon1Lan1Thang = soLuongItHon1Lan1Thang;
            tyLe.ItHon1Lan1Thang = Math.Round(phanTram, 2);

            var soLuongKBKTL = modelInputs.Where(x => x.tansuatda.Contains("KB")).Sum(x => x.SoLuong);
            phanTram = soLuongKBKTL / tongSo;
            soNguoi.KBKTL = soLuongKBKTL;
            tyLe.KBKTL = Math.Round(phanTram, 2);

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
            decimal phanTram = 0;

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
            phanTram = soLuongHutHit / tongSo;
            soNguoiTungLoai.HutHit = soLuongHutHit;
            tyLeNguoiTungLoai.HutHit = Math.Round(phanTram, 2);

            var soLuongDangBot = modelInputs.Where(x => x.NoiDung.Contains("bột")).Sum(x => x.SoLuong);
            phanTram = soLuongDangBot / tongSo;
            soNguoiTungLoai.DangBot = soLuongDangBot;
            tyLeNguoiTungLoai.DangBot = Math.Round(phanTram, 2);

            var soLuongUongNuot = modelInputs.Where(x => x.NoiDung.Contains("nuốt")).Sum(x => x.SoLuong);
            phanTram = soLuongUongNuot / tongSo;
            soNguoiTungLoai.UongNuot = soLuongUongNuot;
            tyLeNguoiTungLoai.UongNuot = Math.Round(phanTram, 2);

            var soLuongTiemChich = modelInputs.Where(x => x.NoiDung.Contains("Tiêm")).Sum(x => x.SoLuong);
            phanTram = soLuongTiemChich / tongSo;
            soNguoiTungLoai.TiemChich = soLuongTiemChich;
            tyLeNguoiTungLoai.TiemChich = Math.Round(phanTram, 2);

            var soLuongKBKTL = modelInputs.Where(x => x.NoiDung.Contains("KB")).Sum(x => x.SoLuong);
            phanTram = soLuongKBKTL / tongSo;
            soNguoiTungLoai.KBKTL = soLuongKBKTL;
            tyLeNguoiTungLoai.KBKTL = Math.Round(phanTram, 2);

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
            decimal phanTram = 0;

            // Chỉ 1 chất
            var soLuong1Chat = modelInputs.Where(x => Convert.ToInt32(x.NoiDung) == 1).Sum(x => x.SoLuong);
            phanTram = soLuong1Chat / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Chỉ 1 chất",
                SoNguoi = soLuong1Chat,
                PhanTram = Math.Round(phanTram, 2)
            });

            // 2 chất
            var soLuong2Chat = modelInputs.Where(x => Convert.ToInt32(x.NoiDung) == 2).Sum(x => x.SoLuong);
            phanTram = soLuong2Chat / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "2 chất",
                SoNguoi = soLuong2Chat,
                PhanTram = Math.Round(phanTram, 2)
            });

            // 3 chất
            var soLuong3Chat = modelInputs.Where(x => Convert.ToInt32(x.NoiDung) == 3).Sum(x => x.SoLuong);
            phanTram = soLuong3Chat / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "3 chất",
                SoNguoi = soLuong3Chat,
                PhanTram = Math.Round(phanTram, 2)
            });

            // 4 chất trở lên
            var soLuong4Chat = modelInputs.Where(x => Convert.ToInt32(x.NoiDung) >= 4).Sum(x => x.SoLuong);
            phanTram = soLuong4Chat / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "4 chất trở lên",
                SoNguoi = soLuong4Chat,
                PhanTram = Math.Round(phanTram, 2)
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
            decimal phanTram = 0;

            // Âm tính
            var soLuongAmTinh = modelInputs.Where(x => x.ketqua == -1).Sum(x => x.SoLuong);
            phanTram = soLuongAmTinh / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Âm tính",
                SoNguoi = soLuongAmTinh,
                PhanTram = Math.Round(phanTram, 2)
            });

            // Dương tính mới
            var soLuongDuongTinhMoi = modelInputs.Where(x => x.ketqua == 1).Sum(x => x.SoLuong);
            phanTram = soLuongDuongTinhMoi / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Dương tính mới",
                SoNguoi = soLuongDuongTinhMoi,
                PhanTram = Math.Round(phanTram, 2)
            });

            // Dương tính - bỏ trị/chưa điều trị
            var soLuongDTBoChuaDT = 0;
            phanTram = soLuongDTBoChuaDT / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Dương tính - bỏ trị/chưa điều trị",
                SoNguoi = soLuongDTBoChuaDT,
                PhanTram = Math.Round(phanTram, 2)
            });

            // Dương tính - đang điều trị
            var soLuongDTDangDT = 0;
            phanTram = soLuongDTDangDT / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Dương tính - đang điều trị",
                SoNguoi = soLuongDTDangDT,
                PhanTram = Math.Round(phanTram, 2)
            });

            // Không xác định
            var soLuongKXD = modelInputs.Where(x => x.ketqua == 0).Sum(x => x.SoLuong);
            phanTram = soLuongKXD / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Không xác định",
                SoNguoi = soLuongKXD,
                PhanTram = Math.Round(phanTram, 2)
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
            decimal phanTram = 0;

            // Nhóm tuổi tù 16-18
            var soLuong1618 = modelInputs.Where(x => x.tuoi >= 16 && x.tuoi <= 18).Sum(x => x.SoLuong);
            phanTram = soLuong1618 / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "16-18",
                SoNguoi = soLuong1618,
                PhanTram = Math.Round(phanTram, 2)
            });

            // Nhóm tuổi tù 19-22
            var soLuong1922 = modelInputs.Where(x => x.tuoi >= 19 && x.tuoi <= 22).Sum(x => x.SoLuong);
            phanTram = soLuong1922 / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "19-22",
                SoNguoi = soLuong1922,
                PhanTram = Math.Round(phanTram, 2)
            });

            // Nhóm tuổi tù 23-24
            var soLuong2324 = modelInputs.Where(x => x.tuoi >= 23 && x.tuoi <= 24).Sum(x => x.SoLuong);
            phanTram = soLuong2324 / tongSo;
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "23-24",
                SoNguoi = soLuong2324,
                PhanTram = Math.Round(phanTram, 2)
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
            decimal phanTram = 0;
            foreach (var item in modelInputs)
            {
                phanTram = item.SoLuong / tongSo;
                outDatas.Add(new KetQuaSangLocModel
                {
                    NoiDung = item.NoiDung,
                    SoNguoi = item.SoLuong,
                    PhanTram = Math.Round(phanTram, 2)
                });
            }
            // Thêm dòng tổng số
            outDatas.Add(new KetQuaSangLocModel
            {
                NoiDung = "Tổng",
                SoNguoi = tongSo,
                PhanTram = 100
            });
        }

    }
}
