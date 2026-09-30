import sys
try:
    sys.stdout.reconfigure(encoding='utf-8')
except Exception:
    pass
import pyodbc

conn = pyodbc.connect('DRIVER={ODBC Driver 17 for SQL Server};SERVER=103.77.167.206;DATABASE=BVTL_REPORTING_DEV;UID=sa;PWD=GLy74MwZ;')
cursor = conn.cursor()
cursor.execute("SELECT Code, Name, IsActive, Code_Map FROM BVTL_CITES WHERE Code IN ('NBI', 'HCM', 'HNO')")
for r in cursor.fetchall():
    print(r)

print('\nTotal cities count:')
cursor.execute("SELECT COUNT(*) FROM BVTL_CITES")
print(cursor.fetchone()[0])

cursor.execute("SELECT CityCode FROM BVTL_QT_NGUOI_DUNG_CITY WHERE NguoiDungId = 257 AND IsActive = 1")
print('\nUser 257 cities in BVTL_QT_NGUOI_DUNG_CITY:')
for r in cursor.fetchall():
    print(r)

conn.close()
