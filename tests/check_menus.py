import sys
try:
    sys.stdout.reconfigure(encoding='utf-8')
except Exception:
    pass
import pyodbc

conn = pyodbc.connect('DRIVER={ODBC Driver 17 for SQL Server};SERVER=103.77.167.206;DATABASE=BVTL_REPORTING_DEV;UID=sa;PWD=GLy74MwZ;')
cursor = conn.cursor()

# Get menus for user 257
cursor.execute("""
    SELECT pm.ID, pm.NAME, pm.HREF_URL, pm.CONTROLLER_NAME, pm.PARENT_PAGE_ID 
    FROM BVTL_QT_QUYEN_PAGE rp
    JOIN BVTL_QT_PAGE_MENU pm ON rp.PageID = pm.ID
    JOIN BVTL_QT_NGUOI_DUNG u ON rp.RoleID = u.GroupID
    WHERE u.ID = 257 AND pm.IS_ACTIVE = 1 AND rp.IS_ACTIVE = 1
    ORDER BY pm.PARENT_PAGE_ID, pm.ORDER_BY
""")
print('Menus for tuantmhcm:')
for r in cursor.fetchall():
    print(r)

conn.close()
