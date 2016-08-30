using CoreDM;
using System.Collections.Generic;
using System.Linq;
using CoreAPI.Extensions;
using CoreAPI.Types;
using System.Web.Configuration;
using System;
using CoreAPI.Const;

namespace CoreWeb.Models.StartPage
{
    /// <summary>
    /// Класс реализации модели пользовательского интерфейса главного меню
    /// </summary>
    public class MainMenuLoader : IMainMenuLoader
    {
        private CoreEntities _db;
        /// <summary>
        /// Идентификатор родителя
        /// </summary>
        private long _parentID;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Контекст БД</param>
        public MainMenuLoader(ICoreDBContext db)
        {
            string gid = WebConfigurationManager.AppSettings.Get("MenuRootGID");
            if (string.IsNullOrEmpty(gid))
                throw new Exception("Ошибка определения корневого элемента меню.");
            _parentID = ConstManager.Get(gid, db);
            _db = db.CoreEntities;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="gid">Глобальный идентификатор меню</param>
        public MainMenuLoader(ICoreDBContext db, string gid)
        {
            _parentID = ConstManager.Get(gid, db);
            _db = db.CoreEntities;
        }
        /// <summary>
        /// Получение списка элементов главного меню
        /// </summary>
        /// <returns>Список элементов меню</returns>
        public List<MainMenuItem> LoadItems()
        {
            var q = from g in _db.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == _parentID)
                    join m in _db.GetAllowObjects<SysMenu, SysObjectRole>() on g.ObjectID equals m.ID
                    select m;
            return LoadItems(q);
        }

        private List<MainMenuItem> LoadItems(IQueryable<SysMenu> query)
        {
            List<MainMenuItem> res;
            var q = query.OrderBy(p => p.Order).Select(p => new MainMenuItem()
                    {
                        ID = p.ID,
                        Name = p.Name,
                        ImageName = p.ImageName,
                        IsVisible = p.IsVisible,
                        Type = p.Type,
                        Url = p.Url,
                        Params = p.Params,
                        Description = p.Description
                    });
            res = q.ToList();
            foreach (var i in res)
            {
                if (i.Type.Equals("GROUP"))
                    i.Children = LoadItems(_db.GetAllowObjects<SysMenu, SysObjectRole>().Where(p => p.ParentID == i.ID));
            }
            return res.ToList();
        }
    }
}