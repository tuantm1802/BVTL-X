using System;
using System.Collections.Generic;
using Model.ModelExtend;

namespace Common.Common
{
    public static class ReportValidatorHelper
    {
        /// <summary>
        /// VR-01 [BLOCKING]: Kiểm tra cấu trúc tổng bằng các cột thành phần (Tổng = PUD + PLHIV + TG + SW + MSM).
        /// Với MỌI dòng số liệu, cột "Tổng" phải bằng đúng tổng 5 cột phân nhóm.
        /// Chặn xuất báo cáo nếu có bất kỳ dòng nào vi phạm.
        /// </summary>
        public static bool ValidateReportArithmetic(IEnumerable<BaoCaoCD45Model> rows, out string errorMessage)
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
                    int p = r.PUD ?? 0;
                    int pl = r.PLHIV ?? 0;
                    int tg = r.TG ?? 0;
                    int sw = r.SW ?? 0;
                    int msm = r.MSM ?? 0;
                    int sum5 = p + pl + tg + sw + msm;
                    int tong = r.Tong ?? 0;

                    if (tong != sum5)
                    {
                        errorMessage = $"Vi phạm VR-01 [BLOCKING]: Chỉ tiêu [{r.STT}] '{r.ChiTieu}' có cột Tổng ({tong:N0}) không bằng tổng 5 cột phân nhóm ({sum5:N0} = PUD:{p} + PLHIV:{pl} + TG:{tg} + SW:{sw} + MSM:{msm}). Tự động chặn xuất báo cáo.";
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
