using System.Collections.Generic;
using System.Linq;
using CoreDM;
using CoreAPI;
using System;

namespace CoreWeb.Models.Roles
{
    /// <summary>
    /// Пространство имен для реализации моделей пользовательского интерфейса настройки доступа к объекту
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }
    /// <summary>
    /// Класс реализации модель роли объекта
    /// </summary>
    public class ObjectRoleItemModel
    {
        /// <summary>
        /// Идентификатор роли
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// Имя 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Состояние активности роли 
        /// </summary>
        public bool Checked { get; set; }
    }
    /// <summary>
    /// Класс реализации модель пользовательского интерфейса редактирования ролей объекта
    /// </summary>
    public class ObjectRolesModel
    {
        /// <summary>
        /// Идентификатор Объекта
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// Список ролей объекта
        /// </summary>
        public List<ObjectRoleItemModel> Roles { get; set; }
        private CoreEntities _db;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="id">Идентификатор группы ролей</param>
        /// <param name="group_preffix">префикс родительских имен групп</param>
        public ObjectRolesModel(CoreEntities db, long id, string group_preffix)
        {
            if (id <= 0)
                throw new ArgumentNullException("Не указан идентификатор группы ролей.");
            _db = db;
            ID = id;
            Roles = GetRolesFromGroup(ID, group_preffix);
        }
        /// <summary>
        /// Получение списка ролей объекта
        /// </summary>
        /// <param name="id">>Идентификатор группы ролей</param>
        /// <param name="prefixGroup">префикс родительских имен групп</param>
        /// <returns>Список ролей объекта</returns>
        private List<ObjectRoleItemModel> GetRolesFromGroup(long id, string prefixGroup)
        {
            List<ObjectRoleItemModel> listOG = new List<ObjectRoleItemModel>();
            var _roles = from og in _db.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == id)
                         join g in _db.ObjGroups.AsNoTracking() on og.GroupID equals g.ID
                         join r in _db.SysRoles.AsNoTracking().Where(p => p.ID != CoreConstant.EVERYONE_ROLE_ID) on og.ObjectID equals r.ID into roles
                         from r in roles.DefaultIfEmpty()
                         select new { ID = r == null ? 0 : r.ID, ObjectID = og.ObjectID, GName = g.Name, RName = r == null ? null : r.Name };
            foreach (var item in _roles)
            {
                if (item.ID == 0)
                {
                    listOG.AddRange(GetRolesFromGroup(item.ObjectID, string.IsNullOrEmpty(prefixGroup) ? "/" : string.Format("{0}{1}/", prefixGroup, item.GName)));
                }
                else
                {
                    listOG.Add(new ObjectRoleItemModel { ID = item.ID, Name = string.IsNullOrEmpty(prefixGroup) ? item.RName : string.Format("{0}{1}/{2}", prefixGroup.Remove(0, 1), item.GName, item.RName), Checked = false });
                }
            }
            return listOG;
        }
    }
}