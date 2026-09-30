"""
Upgrade SP_CD45_GetBaoCao, SP_CD45_GetDrillDown, SP_CD45_GetListTCV
To support multi-city comma-separated codes
"""
import re
import pyodbc

# Old block pattern
old_block = """    -- Resolve @CityCode mapping (hỗ trợ cả 34 tỉnh mới và 63 tỉnh cũ)
    DECLARE @MappedCityCodes TABLE (Code VARCHAR(10));
    IF @CityCode IS NOT NULL AND @CityCode <> ''
    BEGIN
        IF EXISTS (SELECT 1 FROM BVTL_DM_TINH_MOI WHERE Code = @CityCode)
        BEGIN
            INSERT INTO @MappedCityCodes(Code)
            SELECT OldCityCode FROM BVTL_MAP_TINH_CU_MOI WHERE NewCityCode = @CityCode;
        END
        ELSE
        BEGIN
            INSERT INTO @MappedCityCodes(Code) VALUES (@CityCode);
        END
    END"""

# New block supporting comma-separated codes, 34 new and 63 old
new_block = """    -- Resolve @CityCode mapping (hỗ trợ cả 34 tỉnh mới, 63 tỉnh cũ và chuỗi phân cách dấu phẩy)
    DECLARE @MappedCityCodes TABLE (Code VARCHAR(10));
    IF @CityCode IS NOT NULL AND @CityCode <> ''
    BEGIN
        INSERT INTO @MappedCityCodes(Code)
        SELECT DISTINCT m.OldCityCode 
        FROM STRING_SPLIT(@CityCode, ',') s
        JOIN BVTL_MAP_TINH_CU_MOI m ON LTRIM(RTRIM(s.value)) = m.NewCityCode
        UNION
        SELECT DISTINCT LTRIM(RTRIM(s.value))
        FROM STRING_SPLIT(@CityCode, ',') s
        WHERE NOT EXISTS (SELECT 1 FROM BVTL_MAP_TINH_CU_MOI m WHERE m.NewCityCode = LTRIM(RTRIM(s.value)))
          AND LTRIM(RTRIM(s.value)) <> '';
    END"""

# 1. Update SQL_CD45_SP.sql
with open(r'D:\Projects\BVTL-X\SQL_CD45_SP.sql', 'r', encoding='utf-8-sig') as f:
    sql1 = f.read()

count1 = sql1.count(old_block)
print(f"Found {count1} blocks to replace in SQL_CD45_SP.sql")
sql1_updated = sql1.replace(old_block, new_block)

with open(r'D:\Projects\BVTL-X\SQL_CD45_SP.sql', 'w', encoding='utf-8-sig') as f:
    f.write(sql1_updated)
print("Updated SQL_CD45_SP.sql successfully")

# 2. Update SQL_CD45_SP_DrillDown.sql
with open(r'D:\Projects\BVTL-X\SQL_CD45_SP_DrillDown.sql', 'r', encoding='utf-8-sig') as f:
    sql2 = f.read()

count2 = sql2.count(old_block)
print(f"Found {count2} blocks to replace in SQL_CD45_SP_DrillDown.sql")
sql2_updated = sql2.replace(old_block, new_block)

with open(r'D:\Projects\BVTL-X\SQL_CD45_SP_DrillDown.sql', 'w', encoding='utf-8-sig') as f:
    f.write(sql2_updated)
print("Updated SQL_CD45_SP_DrillDown.sql successfully")

# 3. Execute both scripts on SQL Server
conn = pyodbc.connect('DRIVER={ODBC Driver 17 for SQL Server};SERVER=103.77.167.206;DATABASE=BVTL_REPORTING_DEV;UID=sa;PWD=GLy74MwZ;', autocommit=True)
cursor = conn.cursor()

def execute_batches(sql_text, name):
    print(f"\nExecuting {name} into SQL Server...")
    batches = re.split(r'^\s*GO\s*$', sql_text, flags=re.MULTILINE | re.IGNORECASE)
    for i, b in enumerate(batches):
        b = b.lstrip('\ufeff').strip()
        if b:
            try:
                cursor.execute(b)
            except Exception as e:
                print(f"Error in batch {i}: {e}")
                raise
    print(f"Executed {name} successfully.")

execute_batches(sql1_updated, "SQL_CD45_SP.sql")
execute_batches(sql2_updated, "SQL_CD45_SP_DrillDown.sql")

conn.close()
print("\nAll SPs updated on SQL Server!")
