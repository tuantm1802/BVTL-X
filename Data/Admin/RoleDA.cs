using log4net;
using Model.Model;
using Model.ModelExtend;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Data.Admin
{
    public class RoleDA
    {
        BaoCaoBVTLEntities db = new BaoCaoBVTLEntities();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public Role GetItemByRoleName(string name)
        {
            return db.Roles.FirstOrDefault(x => x.Name == name);
        }

        public List<Role> GetAll(int page)
        {
            return db.Roles.ToList();
        }

        public List<RolePageModel> GetAllByPage(ModelSearch modelSearch, ref int pageSize)
        {
            var result = new List<RolePageModel>();
            pageSize = 10;

            try
            {
                var param = db.SysParameters.FirstOrDefault(x => x.ParamCode == "PageSize");
                if (param != null)
                    pageSize = Convert.ToInt32(param.ParamValue);

                var sqlString = "SELECT *, count(ID) over() as TotalRow FROM [Role] WHERE IsActive =1";
                if (!string.IsNullOrEmpty(modelSearch.KeyWord))
                {
                    sqlString += " AND (Name LIKE N'%" + modelSearch.KeyWord + "%' OR Descripttion LIKE N'%" + modelSearch.KeyWord + "%')";
                }
                if (!string.IsNullOrEmpty(modelSearch.SortColumn))
                    sqlString += " ORDER BY " + modelSearch.SortColumn;
                sqlString += " OFFSET " + ((modelSearch.currentPage - 1) * modelSearch.pageSize) + " ROWS FETCH NEXT " + modelSearch.pageSize + " ROWS ONLY;";
                result = db.Database.SqlQuery<RolePageModel>(sqlString).ToList();
            }
            catch (Exception ex)
            {
                var log = new SysLog
                {
                    ControllerName = "RoleDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách quyền theo trang lỗi:" + ex.Message
                };
                db.SysLogs.Add(log);
                result = new List<RolePageModel>();
            }

            return result;
        }

        public ObjectMessage Add(Role role, List<TreeModel> pageMenus)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                role.IsActive = true;
                db.Roles.Add(role);
                var roleID = role.ID;
                // Thêm quyền sử dụng page
                if (!string.IsNullOrEmpty(roleID))
                {
                    var pageSelect = pageMenus.Where(x => x.@checked && x.id.IndexOf("/") <= 0).ToList();
                    if (pageSelect != null && pageSelect.Count > 0)
                    {
                        for (int i = 0; i < pageSelect.Count; i++)
                        {
                            var pageID = pageSelect[i].pace_id;
                            var actions = pageMenus.Where(x => x.@checked && x.pace_id == pageID && x.id.IndexOf("/") > 0).ToList();
                            string action = "";
                            if (actions != null && actions.Count > 0)
                            {
                                action = string.Join("|", actions.Select(x => x.name_control));
                            }
                            db.RolePages.Add(new RolePage
                            {
                                RoleID = roleID,
                                CONTROL_STRING = action,
                                PageID = Convert.ToInt32(pageID),
                                IS_ACTIVE = true
                            });
                        }
                    }
                    db.SaveChanges();
                    obj.Error = false;
                    obj.Title = "Thêm mới thành công!";
                }
                else
                {
                    obj.Error = true;
                    obj.Title = "Thêm mới lỗi!";
                }

                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Edit(Role role, List<TreeModel> pageMenus)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.Roles.FirstOrDefault(x => x.ID == role.ID);
                data.Name = role.Name;
                data.Descripttion = role.Descripttion;

                var roleID = role.ID;
                // Thêm quyền sử dụng page
                if (!string.IsNullOrEmpty(roleID))
                {
                    // Xóa rolepage cũ
                    var itemDelete = db.RolePages.Where(x => x.RoleID == roleID).ToList();
                    if (itemDelete != null && itemDelete.Count > 0)
                    {
                        for (int i = 0; i < itemDelete.Count; i++)
                        {
                            db.RolePages.Remove(itemDelete[i]);
                        }
                    }

                    var pageSelect = pageMenus.Where(x => x.@checked && x.id.IndexOf("/") <= 0).ToList();
                    if (pageSelect != null && pageSelect.Count > 0)
                    {
                        for (int i = 0; i < pageSelect.Count; i++)
                        {
                            var pageID = pageSelect[i].pace_id;
                            var actions = pageMenus.Where(x => x.@checked && x.pace_id == pageID && x.id.IndexOf("/") > 0).ToList();
                            string action = "";
                            if (actions != null && actions.Count > 0)
                            {
                                action = string.Join("|", actions.Select(x => x.name_control));
                            }
                            db.RolePages.Add(new RolePage
                            {
                                RoleID = roleID,
                                CONTROL_STRING = action,
                                PageID = Convert.ToInt32(pageID),
                                IS_ACTIVE = true
                            });
                        }
                    }
                    db.SaveChanges();
                    obj.Error = false;
                    obj.Title = "Cập nhật thành công!";
                }
                else
                {
                    obj.Error = true;
                    obj.Title = "Cập nhật lỗi!";
                }

                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Delete(string Id)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var RolePage = db.RolePages.Where(x => x.RoleID == Id).ToList();
                if (RolePage != null && RolePage.Count > 0)
                {
                    for (int i = 0; i < RolePage.Count; i++)
                    {
                        db.RolePages.Remove(RolePage[i]);
                    }

                }
                var itemDelete = db.Roles.Find(Id);
                db.Roles.Remove(itemDelete);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Xóa thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
    }
}
