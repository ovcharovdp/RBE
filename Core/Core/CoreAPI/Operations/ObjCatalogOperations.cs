using BaseEntities;
using CoreDM;
using System;
using System.Transactions;

namespace CoreAPI.Operations
{
    /// <summary>
    /// Класс, реализующий действия над сущностью "Поле журнала"
    /// </summary>
    public class ObjCatalogOperations : BaseDBOperations
    {
        /// <summary>
        /// Проверяет корректность заполнения данных
        /// </summary>
        /// <param name="element">Поле журнала</param>
        public static void Check(ObjCatalog element)
        {
            if (element == null)
                throw new Exception("Элемент не определен.");
            if (string.IsNullOrEmpty(element.Name))
                throw new Exception("Не задано название элемента.");
        }
        /// <summary>
        /// Создает журнал объектов
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="parentID">Группа</param>
        /// <param name="catalog">Журнал</param>
        /// <returns>Сущьность "Журнал"</returns>
        public static ObjCatalog New(CoreEntities db, long parentID, ObjCatalog catalog)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (catalog.ID == 0)
                        catalog.ID = GetNextID(db);
                    if (catalog.RootID == 0)
                        catalog.RootID = null;
                    if (string.IsNullOrEmpty(catalog.GID))
                        catalog.GID = Guid.NewGuid().ToString().ToUpper();

                    db.ObjCataloges.Add(catalog);
                    db.ObjGroupObjects.Add(new ObjGroupObject() { GroupID = parentID, ObjectID = catalog.ID });
                    db.SaveChanges();
                    scope.Complete();
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка при создании журнала.", e);
                }
            }
            return catalog;
        }
        /// <summary>
        /// Добавляет поле в журнал
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="catalogID">Идентификатор журнала</param>
        /// <param name="field">Поле журнала</param>
        /// <returns>Идентификатор поля журнала</returns>
        public static long AddField(CoreEntities DB, long catalogID, ObjCatalogField field)
        {
            try
            {
                ObjCatalog c = DB.ObjCataloges.Find(catalogID);

                if (field.ID == 0)
                    field.ID = GetNextID(DB);
                c.Fields.Add(field);
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при создании поля журнала.", e);
            }
            return field.ID;
        }
        /// <summary>
        /// Удаляет журнал
        /// </summary>
        /// <param name="id">Идентификатор журнала</param>
        /// <param name="db">Контекст БД</param>
        public static void Del(CoreEntities db, long id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    ObjCatalog element = db.ObjCataloges.Find(id);
                    if (element != null)
                    {
                        BaseObjectOperations.DelFromGroups(db, id);
                        BaseObjectOperations.DelFromRoles(db, id);
                        db.ObjCataloges.Remove(element);
                        db.SaveChanges();
                        scope.Complete();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка удаления журнала.", e);
                }
            }
        }
        /// <summary>
        /// Удаляет поле из журнала
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="id">Идентификатор поля</param>
        public static void DelField(CoreEntities db, long id)
        {
            try
            {
                ObjCatalogField c = db.ObjCatalogFields.Find(id);
                db.ObjCatalogFields.Remove(c);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления поля журнала.", e);
            }
        }
    }
}
