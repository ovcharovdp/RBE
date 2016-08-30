using BaseEntities;
using CoreDM;
using System;
using System.Linq;
using System.Transactions;

namespace CoreAPI.Operations
{
    /// <summary>
    /// Класс, реализующий действия над сущностью "Организаци"
    /// </summary>
    public class OrgOperations : BaseDBOperations
    {
        /// <summary>
        /// Проверяет корректность заполнения данных
        /// </summary>
        /// <param name="element">Организация</param>
        public static void Check(OrgDepartment element)
        {
            if (element == null)
                throw new Exception("Элемент не определен.");
            if (string.IsNullOrEmpty(element.Name))
                throw new Exception("Не задано название организации.");
            if (element.TypeID == 0)
                throw new Exception("Не задан тип организации.");
        }
        /// <summary>
        /// Формирует полное и структурное имя организации
        /// </summary>
        /// <param name="DB">Кондекст БД</param>
        /// <param name="element">Организация</param>
        public static void Refresh(CoreEntities DB, OrgDepartment element)
        {
            element.FullName = string.Format("{0} {1}", DB.SysDictionaries.AsNoTracking().FirstOrDefault(p => p.ID == element.TypeID).Name, element.Name);
            element.StructName = element.ParentID == null ? element.FullName :
                string.Format("{0}, {1}", element.FullName, DB.OrgDepartments.AsNoTracking().FirstOrDefault(p => p.ID == element.ParentID).StructName);
        }
        /// <summary>
        /// Создает новую организацию
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="parentID">Идентификатор группы/родительской организации</param>
        /// <param name="element">Организация</param>
        /// <returns>Идентификатор организации</returns>
        public static long New(CoreEntities DB, long parentID, OrgDepartment element)
        {
            try
            {
                Check(element);
                if (element.ID == 0) element.ID = GetNextID(DB);

                DB.OrgDepartments.Add(element);
                if (DB.OrgDepartments.Any(p => p.ID == parentID))
                {
                    element.ParentID = parentID;
                }
                else
                {
                    DB.ObjGroupObjects.Add(new ObjGroupObject() { GroupID = parentID, ObjectID = element.ID });
                }
                Refresh(DB, element);
                SysRoleOperations.LinkObjectEveryone(DB, element.ID);
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при создании организации.", e);
            }
            return element.ID;
        }
        /// <summary>
        /// Удаляет организацию
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="id">Идентификатор организации</param>
        public static void Del(CoreEntities DB, long id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (DB.OrgDepartments.AsNoTracking().Any(p => p.ParentID == id))
                        throw new Exception("Организация содержит дочерние подразделения.");

                    OrgDepartment element = DB.OrgDepartments.Find(id);
                    if (element != null)
                    {
                        /////////////////////////////////////////////////
                        // Нужно сделать проверку на использование елемента в других объектах
                        /////////////////////////////////////////////////
                        if (element.ParentID == null)
                            BaseObjectOperations.DelFromGroups(DB, id);

                        BaseObjectOperations.DelFromRoles(DB, id);
                        DB.OrgDepartments.Remove(element);
                        DB.SaveChanges();
                        scope.Complete();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка удаления организации.", e);
                }
            }
        }
    }
}
