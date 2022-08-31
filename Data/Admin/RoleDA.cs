using Data.InterfaceDA.Admin;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Data.Admin
{
    public class RoleDA: IRoleDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public BVTL_QT_QUYEN GetItemByBVTL_QT_QUYENName(string name)
        {
            return db.BVTL_QT_QUYEN.FirstOrDefault(x => x.Name == name);
        }

        public List<BVTL_QT_QUYEN> GetAll(int page)
        {
            return db.BVTL_QT_QUYEN.ToList();
        }

        public List<RolePageModel> GetAllByPage(ModelSearch modelSearch, ref int pageSize)
        {
            var result = new List<RolePageModel>();
            pageSize = 10;

            try
            {
                var param = db.BVTL_QT_THAM_SO.FirstOrDefault(x => x.ParamCode == "PageSize");
                if (param != null)
                    pageSize = Convert.ToInt32(param.ParamValue);

                var sqlString = "SELECT *, count(ID) over() as TotalRow FROM [BVTL_QT_QUYEN] WHERE IsActive =1";
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
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "BVTL_QT_QUYENDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách quyền theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<RolePageModel>();
            }

            return result;
        }

        public ObjectMessage Add(BVTL_QT_QUYEN model, List<TreeModel> pageMenus)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                model.IsActive = true;
                db.BVTL_QT_QUYEN.Add(model);
                var RoleId = model.ID;
                // Thêm quyền sử dụng page
                if (!string.IsNullOrEmpty(RoleId))
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
                            db.BVTL_QT_QUYEN_PAGE.Add(new BVTL_QT_QUYEN_PAGE
                            {
                                RoleID = RoleId,
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
        public ObjectMessage Edit(BVTL_QT_QUYEN model, List<TreeModel> pageMenus)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.BVTL_QT_QUYEN.FirstOrDefault(x => x.ID == model.ID);
                data.Name = model.Name;
                data.Descripttion = model.Descripttion;

                var RoleId = model.ID;
                // Thêm quyền sử dụng page
                if (!string.IsNullOrEmpty(RoleId))
                {
                    // Xóa BVTL_QT_QUYENpage cũ
                    var itemDelete = db.BVTL_QT_QUYEN_PAGE.Where(x => x.RoleID == RoleId).ToList();
                    if (itemDelete != null && itemDelete.Count > 0)
                    {
                        for (int i = 0; i < itemDelete.Count; i++)
                        {
                            db.BVTL_QT_QUYEN_PAGE.Remove(itemDelete[i]);
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
                            db.BVTL_QT_QUYEN_PAGE.Add(new BVTL_QT_QUYEN_PAGE
                            {
                                RoleID = RoleId,
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
                var BVTL_QT_QUYENPage = db.BVTL_QT_QUYEN_PAGE.Where(x => x.RoleID == Id).ToList();
                if (BVTL_QT_QUYENPage != null && BVTL_QT_QUYENPage.Count > 0)
                {
                    for (int i = 0; i < BVTL_QT_QUYENPage.Count; i++)
                    {
                        db.BVTL_QT_QUYEN_PAGE.Remove(BVTL_QT_QUYENPage[i]);
                    }

                }
                var itemDelete = db.BVTL_QT_QUYEN.Find(Id);
                db.BVTL_QT_QUYEN.Remove(itemDelete);
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
