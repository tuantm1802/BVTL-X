using System;
using System.Collections.Generic;
using Model.ModelExtend;

namespace Common.Common
{
    public static class ReportValidatorHelper
    {
        /// <summary>
        /// VR-01 [BLOCKING]: Kiểm tra cấu trúc tổng bằng các cột thành phần:
        /// - Mode 1 (Quần thể): Tổng = PUD + PLHIV + TG + SW + MSM
        /// - Mode 2 (Giới tính): Tổng = Nam + Nữ
        /// - Mode 3 (Nhóm tuổi): Tổng = 18-25 + 26-35 + >=36
        /// Chặn xuất báo cáo nếu có bất kỳ dòng nào vi phạm.
        /// </summary>
        public static bool ValidateReportArithmetic(IEnumerable<BaoCaoCD45Model> rows, out string errorMessage, int displayMode = 1)
        {
            errorMessage = null;
            if (rows == null) return true;

            foreach (var r in rows)
            {
                // Bỏ qua dòng tiêu đề nhóm Section (Ví dụ: 'I', 'II', 'SEC_I'...)
                if (r.IsBold && (string.IsNullOrEmpty(r.Code) || r.Code.StartsWith("SEC_")))
                {
                    continue;
                }

                // Nếu dòng có STT nhưng không phải tiêu đề nhóm, hoặc có dữ liệu số
                if (!string.IsNullOrEmpty(r.STT) && !r.STT.StartsWith("SEC_"))
                {
                    int tong = r.Tong ?? 0;

                    if (displayMode == 2)
                    {
                        int nam = r.Nam ?? 0;
                        int nu = r.Nu ?? 0;
                        int khac = r.Khac ?? 0;
                        int sumSex = nam + nu + khac;
                        if (tong != sumSex)
                        {
                            errorMessage = $"Vi phạm VR-01 [BLOCKING]: Chỉ tiêu [{r.STT}] '{r.ChiTieu}' có cột Tổng ({tong:N0}) không bằng tổng giới tính ({sumSex:N0} = Nam:{nam} + Nữ:{nu} + Khác:{khac}). Tự động chặn xuất báo cáo.";
                            return false;
                        }
                    }
                    else if (displayMode == 3)
                    {
                        int t1 = r.Tuoi_18_25 ?? 0;
                        int t2 = r.Tuoi_26_35 ?? 0;
                        int t3 = r.Tuoi_Tren35 ?? 0;
                        int sumAge = t1 + t2 + t3;
                        if (tong != sumAge)
                        {
                            errorMessage = $"Vi phạm VR-01 [BLOCKING]: Chỉ tiêu [{r.STT}] '{r.ChiTieu}' có cột Tổng ({tong:N0}) không bằng tổng nhóm tuổi ({sumAge:N0} = 18-25:{t1} + 26-35:{t2} + >=36:{t3}). Tự động chặn xuất báo cáo.";
                            return false;
                        }
                    }
                    else
                    {
                        int p = r.PUD ?? 0;
                        int pl = r.PLHIV ?? 0;
                        int tg = r.TG ?? 0;
                        int sw = r.SW ?? 0;
                        int msm = r.MSM ?? 0;
                        int sum5 = p + pl + tg + sw + msm;

                        if (tong != sum5)
                        {
                            errorMessage = $"Vi phạm VR-01 [BLOCKING]: Chỉ tiêu [{r.STT}] '{r.ChiTieu}' có cột Tổng ({tong:N0}) không bằng tổng 5 cột phân nhóm ({sum5:N0} = PUD:{p} + PLHIV:{pl} + TG:{tg} + SW:{sw} + MSM:{msm}). Tự động chặn xuất báo cáo.";
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
