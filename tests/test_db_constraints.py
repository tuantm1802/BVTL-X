"""
Test Database Constraints & Foreign Keys for BVTL-X Phase 2
Powered by pyodbc
"""
import sys
import pyodbc

try:
    sys.stdout.reconfigure(encoding='utf-8')
    sys.stderr.reconfigure(encoding='utf-8')
except Exception:
    pass

conn_str = (
    "DRIVER={ODBC Driver 17 for SQL Server};"
    "SERVER=103.77.167.206;"
    "DATABASE=BVTL_REPORTING_DEV;"
    "UID=sa;"
    "PWD=GLy74MwZ;"
)

conn = pyodbc.connect(conn_str, autocommit=False)
cursor = conn.cursor()

print("==========================================================================")
print("     KIỂM TRA TỰ ĐỘNG CÁC RÀNG BUỘC CƠ SỞ DỮ LIỆU (DATABASE CONSTRAINTS)   ")
print("==========================================================================")

# TC-DB-01: Chèn Nhóm CBO rác
print("\n--- TC-DB-01: Kiểm tra Foreign Key FK_NDNhomTBH_NhomTBH (Chặn CBO rác) ---")
try:
    cursor.execute("INSERT INTO BVTL_QT_NGUOI_DUNG_NHOM_TBH (NguoiDungId, NhomTBHMa, IsActive) VALUES (243, 'MA_RAC', 1)")
    conn.commit()
    print("  [FAIL] TC-DB-01: Không chặn được foreign key rác")
except Exception as e:
    err_str = str(e)
    if 'FK_NDNhomTBH_NhomTBH' in err_str or 'FOREIGN KEY' in err_str:
        print("  [PASS] TC-DB-01: FK_NDNhomTBH_NhomTBH hoạt động chính xác! SQL Server đã chặn.")
        print(f"         Lỗi SQL Server: {err_str.splitlines()[0]}")
    else:
        print(f"  [ERROR] Lỗi khác: {err_str}")
finally:
    conn.rollback()

# TC-DB-02: Chèn Tỉnh rác
print("\n--- TC-DB-02: Kiểm tra Foreign Key FK_NDCity_City (Chặn Tỉnh rác) ---")
try:
    cursor.execute("INSERT INTO BVTL_QT_NGUOI_DUNG_CITY (NguoiDungId, CityCode, IsActive) VALUES (243, 'TINH_RAC', 1)")
    conn.commit()
    print("  [FAIL] TC-DB-02: Không chặn được foreign key tỉnh rác")
except Exception as e:
    err_str = str(e)
    if 'FK_NDCity_City' in err_str or 'FOREIGN KEY' in err_str:
        print("  [PASS] TC-DB-02: FK_NDCity_City hoạt động chính xác! SQL Server đã chặn.")
        print(f"         Lỗi SQL Server: {err_str.splitlines()[0]}")
    else:
        print(f"  [ERROR] Lỗi khác: {err_str}")
finally:
    conn.rollback()

# TC-DB-03: Cascade Delete khi xóa User
print("\n--- TC-DB-03: Kiểm tra ON DELETE CASCADE khi xóa Người dùng ---")
try:
    # 1. Tạo user tạm
    cursor.execute("INSERT INTO BVTL_QT_NGUOI_DUNG (UserName, Password, Name, MaDuAn, Status, IsAdmin, GroupID, IsActive) VALUES ('temp_cascade_test', '123', N'Test Cascade User', 'BVTL', 1, 0, 'QUYENBC_DM', 1)")
    cursor.execute("SELECT ID FROM BVTL_QT_NGUOI_DUNG WHERE UserName = 'temp_cascade_test'")
    user_id = cursor.fetchone()[0]
    
    # 2. Gán 1 tỉnh và 1 nhóm CBO cho user này
    cursor.execute("INSERT INTO BVTL_QT_NGUOI_DUNG_CITY (NguoiDungId, CityCode, IsActive) VALUES (?, 'HNO', 1)", user_id)
    cursor.execute("INSERT INTO BVTL_QT_NGUOI_DUNG_NHOM_TBH (NguoiDungId, NhomTBHMa, IsActive) VALUES (?, 'HC_ALO', 1)", user_id)
    conn.commit()
    print(f"  Đã tạo User nháp ID = {user_id}, gán vào BVTL_QT_NGUOI_DUNG_CITY và BVTL_QT_NGUOI_DUNG_NHOM_TBH.")

    # 3. Xóa user cha
    cursor.execute("DELETE FROM BVTL_QT_NGUOI_DUNG WHERE Id = ?", user_id)
    conn.commit()

    # 4. Kiểm tra bảng con
    cursor.execute("SELECT COUNT(*) FROM BVTL_QT_NGUOI_DUNG_CITY WHERE NguoiDungId = ?", user_id)
    count_city = cursor.fetchone()[0]
    cursor.execute("SELECT COUNT(*) FROM BVTL_QT_NGUOI_DUNG_NHOM_TBH WHERE NguoiDungId = ?", user_id)
    count_cbo = cursor.fetchone()[0]

    if count_city == 0 and count_cbo == 0:
        print(f"  [PASS] TC-DB-03: Cascade Delete thành công! Số bản ghi mồ côi còn lại: City={count_city}, CBO={count_cbo}.")
    else:
        print(f"  [FAIL] TC-DB-03: Dữ liệu mồ côi chưa được dọn sạch: City={count_city}, CBO={count_cbo}.")
except Exception as e:
    print(f"  [ERROR] {e}")
    conn.rollback()

conn.close()
print("\nKiểm tra toàn bộ DB Constraints hoàn tất!")
