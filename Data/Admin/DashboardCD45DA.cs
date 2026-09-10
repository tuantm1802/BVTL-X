using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Data.Admin
{
    public class DashboardCD45DA : IDashboardCD45DA
    {
        private BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        private static string GetTenDoiTuong(byte? dt)
        {
            switch (dt)
            {
                case 1: return "PUD (Sử dụng ma túy)";
                case 2: return "PLHIV (Sống với HIV)";
                case 3: return "TG (Người chuyển giới)";
                case 4: return "MSM (Nam QHTD đồng giới)";
                case 5: return "SW (Người bán dâm)";
                default: return "Khác";
            }
        }

        private static string GetTenDoiTuongNgan(byte? dt)
        {
            switch (dt)
            {
                case 1: return "PUD";
                case 2: return "PLHIV";
                case 3: return "TG";
                case 4: return "MSM";
                case 5: return "SW";
                default: return "Khác";
            }
        }

        private static string GetTenTinh(string code)
        {
            switch (code)
            {
                case "HNO": return "Hà Nội";
                case "HPG": return "Hải Phòng";
                case "NAN": return "Nghệ An";
                case "HCM": return "TP. Hồ Chí Minh";
                case "HYE": return "Hưng Yên";
                case "NBI": return "Ninh Bình";
                default: return code ?? "";
            }
        }

        private static string GetTenNhomTuoi(string nt)
        {
            if (string.IsNullOrEmpty(nt) || nt.Contains("?") || nt.Contains("Ch") || nt.Contains("á"))
                return "Chưa xác định";
            return nt;
        }

        public DashboardCD45FullDataModel GetDashboardData(string cityCode, string maNhom, string fromDate, string toDate, string nhomTuoiTable1 = null)
        {
            var model = new DashboardCD45FullDataModel
            {
                Overview = new DashboardCD45OverviewModel(),
                ByTargetGroup = new List<DashboardCD45ByTargetGroupModel>(),
                ByAgeGroup = new List<DashboardCD45ByAgeGroupModel>(),
                MentalHealth = new List<DashboardCD45MentalHealthModel>(),
                ByProvince = new List<DashboardCD45ByProvinceModel>(),
                CascadeFunnel = new DashboardCD45CascadeFunnelModel(),
                SocialSupport = new DashboardCD45SocialSupportModel()
            };

            var conn = (SqlConnection)db.Database.Connection;
            bool wasClosed = conn.State != ConnectionState.Open;

            try
            {
                if (wasClosed) conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SP_CD45_Dashboard";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 120;

                    cmd.Parameters.Add(new SqlParameter("@CityCode", string.IsNullOrEmpty(cityCode) ? (object)DBNull.Value : cityCode));
                    cmd.Parameters.Add(new SqlParameter("@MaNhom", string.IsNullOrEmpty(maNhom) ? (object)DBNull.Value : maNhom));

                    if (!string.IsNullOrEmpty(fromDate))
                    {
                        DateTime dtFrom;
                        if (DateTime.TryParseExact(fromDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtFrom) || DateTime.TryParse(fromDate, out dtFrom))
                        {
                            cmd.Parameters.Add(new SqlParameter("@FromDate", dtFrom));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter("@FromDate", DBNull.Value));
                        }
                    }
                    else
                    {
                        cmd.Parameters.Add(new SqlParameter("@FromDate", DBNull.Value));
                    }

                    if (!string.IsNullOrEmpty(toDate))
                    {
                        DateTime dtTo;
                        if (DateTime.TryParseExact(toDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtTo) || DateTime.TryParse(toDate, out dtTo))
                        {
                            cmd.Parameters.Add(new SqlParameter("@ToDate", dtTo));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter("@ToDate", DBNull.Value));
                        }
                    }
                    else
                    {
                        cmd.Parameters.Add(new SqlParameter("@ToDate", DBNull.Value));
                    }

                    cmd.Parameters.Add(new SqlParameter("@NhomTuoiTable1", string.IsNullOrEmpty(nhomTuoiTable1) ? (object)DBNull.Value : nhomTuoiTable1));

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet();
                        adapter.Fill(ds);

                        // Table 0: KPI Overview
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            var r = ds.Tables[0].Rows[0];
                            model.Overview.TongKhachHang = r["TongKhachHang"] != DBNull.Value ? Convert.ToInt32(r["TongKhachHang"]) : 0;
                            model.Overview.TongSangLocQST = r["TongSangLocQST"] != DBNull.Value ? Convert.ToInt32(r["TongSangLocQST"]) : 0;
                            model.Overview.QSTNguyCoCao = r["QSTNguyCoCao"] != DBNull.Value ? Convert.ToInt32(r["QSTNguyCoCao"]) : 0;
                            model.Overview.TyLeQSTNguyCoCao = model.Overview.TongSangLocQST > 0
                                ? Math.Round((double)model.Overview.QSTNguyCoCao / model.Overview.TongSangLocQST * 100.0, 1)
                                : 0.0;
                            model.Overview.TongKhamSKTT = r["TongKhamSKTT"] != DBNull.Value ? Convert.ToInt32(r["TongKhamSKTT"]) : 0;
                            model.Overview.TongLuotKhamSKTT = r["TongLuotKhamSKTT"] != DBNull.Value ? Convert.ToInt32(r["TongLuotKhamSKTT"]) : 0;
                            model.Overview.TongTuVanL1 = r["TongTuVanL1"] != DBNull.Value ? Convert.ToInt32(r["TongTuVanL1"]) : 0;
                            model.Overview.TongHoTroXH = r["TongHoTroXH"] != DBNull.Value ? Convert.ToInt32(r["TongHoTroXH"]) : 0;
                            model.Overview.TongTaiLieuPhat = r["TongTaiLieuPhat"] != DBNull.Value ? Convert.ToInt32(r["TongTaiLieuPhat"]) : 0;

                            if (r["LastSyncTime"] != DBNull.Value)
                            {
                                var syncDate = Convert.ToDateTime(r["LastSyncTime"]);
                                model.Overview.LastSyncTime = syncDate;
                                model.Overview.LastSyncTimeString = syncDate.ToString("HH:mm - dd/MM/yyyy");
                            }
                            else
                            {
                                model.Overview.LastSyncTimeString = "Mới cập nhật";
                            }
                        }

                        // Table 1: QST by Target Group
                        if (ds.Tables.Count > 1)
                        {
                            foreach (DataRow r in ds.Tables[1].Rows)
                            {
                                byte? doiTuongId = r["DoiTuongId"] != DBNull.Value ? Convert.ToByte(r["DoiTuongId"]) : (byte?)null;
                                var item = new DashboardCD45ByTargetGroupModel
                                {
                                    DoiTuongId = doiTuongId,
                                    TenDoiTuong = GetTenDoiTuong(doiTuongId),
                                    TongKH = r["TongKH"] != DBNull.Value ? Convert.ToInt32(r["TongKH"]) : 0,
                                    SoKHSangLoc = r["SoKHSangLoc"] != DBNull.Value ? Convert.ToInt32(r["SoKHSangLoc"]) : 0,
                                    Muc1_RatCao = r["Muc1_RatCao"] != DBNull.Value ? Convert.ToInt32(r["Muc1_RatCao"]) : 0,
                                    Muc2_Cao = r["Muc2_Cao"] != DBNull.Value ? Convert.ToInt32(r["Muc2_Cao"]) : 0,
                                    Muc3_TrungBinh = r["Muc3_TrungBinh"] != DBNull.Value ? Convert.ToInt32(r["Muc3_TrungBinh"]) : 0,
                                    Muc4_Thap = r["Muc4_Thap"] != DBNull.Value ? Convert.ToInt32(r["Muc4_Thap"]) : 0
                                };
                                item.TyLeNguyCoCao = item.SoKHSangLoc > 0
                                    ? Math.Round((double)(item.Muc1_RatCao + item.Muc2_Cao) / item.SoKHSangLoc * 100.0, 1)
                                    : 0.0;
                                model.ByTargetGroup.Add(item);
                            }
                        }

                        // Table 2: QST by Age Group
                        if (ds.Tables.Count > 2)
                        {
                            foreach (DataRow r in ds.Tables[2].Rows)
                            {
                                string rawAge = r["NhomTuoi"] != DBNull.Value ? r["NhomTuoi"].ToString() : "Chưa xác định";
                                var item = new DashboardCD45ByAgeGroupModel
                                {
                                    NhomTuoi = GetTenNhomTuoi(rawAge),
                                    TongKH = r["TongKH"] != DBNull.Value ? Convert.ToInt32(r["TongKH"]) : 0,
                                    SoKHSangLoc = r["SoKHSangLoc"] != DBNull.Value ? Convert.ToInt32(r["SoKHSangLoc"]) : 0,
                                    Muc1 = r["Muc1"] != DBNull.Value ? Convert.ToInt32(r["Muc1"]) : 0,
                                    Muc2 = r["Muc2"] != DBNull.Value ? Convert.ToInt32(r["Muc2"]) : 0,
                                    Muc3 = r["Muc3"] != DBNull.Value ? Convert.ToInt32(r["Muc3"]) : 0,
                                    Muc4 = r["Muc4"] != DBNull.Value ? Convert.ToInt32(r["Muc4"]) : 0
                                };
                                item.TyLeNguyCoCao = item.SoKHSangLoc > 0
                                    ? Math.Round((double)(item.Muc1 + item.Muc2) / item.SoKHSangLoc * 100.0, 1)
                                    : 0.0;
                                model.ByAgeGroup.Add(item);
                            }
                        }

                        // Table 3: Mental Health (PCL5, AUDIT-C, STIGMA)
                        if (ds.Tables.Count > 3)
                        {
                            foreach (DataRow r in ds.Tables[3].Rows)
                            {
                                byte? doiTuongId = r["DoiTuongId"] != DBNull.Value ? Convert.ToByte(r["DoiTuongId"]) : (byte?)null;
                                var item = new DashboardCD45MentalHealthModel
                                {
                                    DoiTuongId = doiTuongId,
                                    TenDoiTuong = GetTenDoiTuongNgan(doiTuongId),
                                    SoCaTuVan = r["SoCaTuVan"] != DBNull.Value ? Convert.ToInt32(r["SoCaTuVan"]) : 0,
                                    PCL5_DuongTinh = r["PCL5_DuongTinh"] != DBNull.Value ? Convert.ToInt32(r["PCL5_DuongTinh"]) : 0,
                                    PCL5_AmTinh = r["PCL5_AmTinh"] != DBNull.Value ? Convert.ToInt32(r["PCL5_AmTinh"]) : 0,
                                    DiemAuditCTB = r["DiemAuditCTB"] != DBNull.Value ? Convert.ToDouble(r["DiemAuditCTB"]) : 0.0,
                                    DiemKyThiTB = r["DiemKyThiTB"] != DBNull.Value ? Convert.ToDouble(r["DiemKyThiTB"]) : 0.0
                                };
                                item.TyLePCL5DuongTinh = item.SoCaTuVan > 0
                                    ? Math.Round((double)item.PCL5_DuongTinh / item.SoCaTuVan * 100.0, 1)
                                    : 0.0;
                                model.MentalHealth.Add(item);
                            }
                        }

                        // Table 4: By Province
                        if (ds.Tables.Count > 4)
                        {
                            foreach (DataRow r in ds.Tables[4].Rows)
                            {
                                string cCode = r["CityCode"] != DBNull.Value ? r["CityCode"].ToString() : "";
                                model.ByProvince.Add(new DashboardCD45ByProvinceModel
                                {
                                    CityCode = cCode,
                                    CityName = GetTenTinh(cCode),
                                    TongKH = r["TongKH"] != DBNull.Value ? Convert.ToInt32(r["TongKH"]) : 0,
                                    SangLocQST = r["SangLocQST"] != DBNull.Value ? Convert.ToInt32(r["SangLocQST"]) : 0,
                                    KhamSKTT = r["KhamSKTT"] != DBNull.Value ? Convert.ToInt32(r["KhamSKTT"]) : 0,
                                    TuVanL1 = r["TuVanL1"] != DBNull.Value ? Convert.ToInt32(r["TuVanL1"]) : 0
                                });
                            }
                        }

                        // Table 5: Cascade Funnel
                        if (ds.Tables.Count > 5 && ds.Tables[5].Rows.Count > 0)
                        {
                            var r = ds.Tables[5].Rows[0];
                            model.CascadeFunnel.Step1_TiepCanTruyenThong = r["Step1_TiepCanTruyenThong"] != DBNull.Value ? Convert.ToInt32(r["Step1_TiepCanTruyenThong"]) : 0;
                            model.CascadeFunnel.Step2_SangLocQST = r["Step2_SangLocQST"] != DBNull.Value ? Convert.ToInt32(r["Step2_SangLocQST"]) : 0;
                            model.CascadeFunnel.Step3_NguyCoCaoQST = r["Step3_NguyCoCaoQST"] != DBNull.Value ? Convert.ToInt32(r["Step3_NguyCoCaoQST"]) : 0;
                            model.CascadeFunnel.Step4_TuVanTamLy = r["Step4_TuVanTamLy"] != DBNull.Value ? Convert.ToInt32(r["Step4_TuVanTamLy"]) : 0;
                            model.CascadeFunnel.Step5_KhamChuyenKhoa = r["Step5_KhamChuyenKhoa"] != DBNull.Value ? Convert.ToInt32(r["Step5_KhamChuyenKhoa"]) : 0;
                            model.CascadeFunnel.Step6_TaiKhamSKTT = r["Step6_TaiKhamSKTT"] != DBNull.Value ? Convert.ToInt32(r["Step6_TaiKhamSKTT"]) : 0;
                        }

                        // Table 6: Social Support
                        if (ds.Tables.Count > 6 && ds.Tables[6].Rows.Count > 0)
                        {
                            var r = ds.Tables[6].Rows[0];
                            model.SocialSupport.TongNhanHoTro = r["TongNhanHoTro"] != DBNull.Value ? Convert.ToInt32(r["TongNhanHoTro"]) : 0;
                            model.SocialSupport.HoTroBHYT = r["HoTroBHYT"] != DBNull.Value ? Convert.ToInt32(r["HoTroBHYT"]) : 0;
                            model.SocialSupport.HoTroMethadone = r["HoTroMethadone"] != DBNull.Value ? Convert.ToInt32(r["HoTroMethadone"]) : 0;
                            model.SocialSupport.XetNghiemHIV = r["XetNghiemHIV"] != DBNull.Value ? Convert.ToInt32(r["XetNghiemHIV"]) : 0;
                            model.SocialSupport.STIs = r["STIs"] != DBNull.Value ? Convert.ToInt32(r["STIs"]) : 0;
                            model.SocialSupport.ViemGan = r["ViemGan"] != DBNull.Value ? Convert.ToInt32(r["ViemGan"]) : 0;
                        }
                    }
                }
            }
            finally
            {
                if (wasClosed && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return model;
        }
    }
}
