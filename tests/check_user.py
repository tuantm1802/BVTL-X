import sys
try:
    sys.stdout.reconfigure(encoding='utf-8')
except Exception:
    pass
import pyodbc

conn = pyodbc.connect('DRIVER={ODBC Driver 17 for SQL Server};SERVER=103.77.167.206;DATABASE=BVTL_REPORTING_DEV;UID=sa;PWD=GLy74MwZ;')
cursor = conn.cursor()
cursor.execute("SELECT ID, UserName, Password, Name, GroupID, IsAdmin, CityCodes, MaDuAn FROM BVTL_QT_NGUOI_DUNG WHERE UserName IN ('tuantm', 'tuantmhcm')")
for row in cursor.fetchall():
    print(dict(zip([d[0] for d in cursor.description], row)))

cursor.execute("SELECT u.UserName, c.CityCode, c.IsActive FROM BVTL_QT_NGUOI_DUNG_CITY c JOIN BVTL_QT_NGUOI_DUNG u ON c.NguoiDungId = u.ID WHERE u.UserName IN ('tuantm', 'tuantmhcm')")
print('\nBVTL_QT_NGUOI_DUNG_CITY:')
for row in cursor.fetchall():
    print(row)

cursor.execute("SELECT u.UserName, t.NhomTBHMa, t.IsActive FROM BVTL_QT_NGUOI_DUNG_NHOM_TBH t JOIN BVTL_QT_NGUOI_DUNG u ON t.NguoiDungId = u.ID WHERE u.UserName IN ('tuantm', 'tuantmhcm')")
print('\nBVTL_QT_NGUOI_DUNG_NHOM_TBH:')
for row in cursor.fetchall():
    print(row)

conn.close()
