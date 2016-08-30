using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Reflection;
using CoreAPI.Types.ObjParam;
using CoreAPI.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using CoreWeb.Models.BaseEditModels;
using BaseEntities;

namespace CoreWeb.Controllers.Base
{
    /// <summary>
    /// Базовый контроллер управления сущностями
    /// </summary>
    public abstract class BaseEntityController : BaseDBController
    {
        /// <inheritdoc />
        public BaseEntityController(ICoreDBContext db) : base(db) { }
        /// <summary>
        /// Глобальный идентификатор журнала
        /// </summary>
        protected abstract string CatalogGID { get; }
        private ObjCatalog _catalogParams;
        /// <summary>
        /// Параметры журнала
        /// </summary>
        protected ObjCatalog CatalogParams
        {
            get
            {
                if (_catalogParams == null)
                {
                    _catalogParams = CoreDB.ObjCataloges.FirstOrDefault(p => p.GID.Equals(CatalogGID));
                    if (_catalogParams == null) throw new Exception("Ошибка загрузки настроек журнала.");
                }
                return _catalogParams;
            }
        }
        /// <summary>
        /// Группа параметров сущности
        /// </summary>
        protected long ParamGroupID
        {
            get
            {
                ObjGroup q = CoreDB.ObjGroups.FirstOrDefault(p => p.Code.Equals(this.CatalogGID));
                if (q == null)
                    throw new Exception("Не найдена группа полей сущности.");
                return q.ID;
            }
        }
        /// <summary>
        /// Родительская группа, содержащая значения
        /// </summary>
        protected long? RootGroupID { get { return CatalogParams.RootID; } }
        /// <summary>
        /// Реализация логики обработки полей данных, которые не соответствуют полям объекта
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <param name="data">Данные объекта</param>
        protected virtual void CustomInitObject(object obj, ClientValue data)
        { }
        /// <summary>
        /// Инициализация полей объекта
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <param name="data">Данные объекта</param>
        /// <returns>Признак изменения объекта</returns>
        protected bool InitObject(object obj, IEnumerable<ClientValue> data)
        {
            bool _result = false;
            foreach (ClientValue cpv in data)
            {
                PropertyInfo propertyObj = obj.GetType().GetProperties().FirstOrDefault(s => s.Name.ToUpper() == cpv.c.ToUpper());
                if (propertyObj != null)
                {
                    if (propertyObj.GetValue(obj) == null || !propertyObj.GetValue(obj).Equals(cpv.v))
                    {
                        _result = true;
                    }
                    switch (propertyObj.PropertyType.IsGenericType ? propertyObj.PropertyType.GenericTypeArguments[0].Name : propertyObj.PropertyType.Name)
                    {
                        case "Int16":
                            propertyObj.SetValue(obj, Convert.ToInt16(cpv.v)); break;
                        case "Int32":
                            propertyObj.SetValue(obj, Convert.ToInt32(cpv.v)); break;
                        case "Int64":
                            if (cpv.v == null) propertyObj.SetValue(obj, null);
                            else propertyObj.SetValue(obj, Convert.ToInt64(cpv.v));
                            break;
                        case "Decimal":
                            propertyObj.SetValue(obj, Convert.ToDecimal(cpv.v)); break;
                        case "DateTime":
                            propertyObj.SetValue(obj, Convert.ToDateTime(cpv.v)); break;
                        case "Byte":
                            propertyObj.SetValue(obj, Convert.ToByte(cpv.v)); break;
                        case "Boolean":
                            propertyObj.SetValue(obj, (int)cpv.v != 0); break;
                        default: propertyObj.SetValue(obj, cpv.v); break;
                    }
                }
                else
                {
                    CustomInitObject(obj, cpv);
                }
            }
            return _result;
        }
        /// <summary>
        /// Реализация логики удаления объекта
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        protected abstract void IntDel(long id);
        /// <summary>
        /// Действие удаления объекта
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>Результат удаления объекта</returns>
        [HttpPost]
        public ActionResult Del(long id)
        {
            IntDel(id);
            return Json("Success");
        }
        /// <summary>
        /// Реализация логики редактирования объекта
        /// </summary>
        /// <param name="data">Измененные поля объекта</param>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>Отредактированный объект</returns>
        protected abstract object IntEdit(IEnumerable<ClientValue> data, long id);
        /// <summary>
        /// Действие редактирования объекта
        /// </summary>
        /// <param name="data">Измененные поля объекта</param>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>Результат редактирования объекта</returns>
        [HttpPost]
        public ActionResult Edit(IEnumerable<ClientValue> data, long id)
        {
            object o = null;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                o = IntEdit(data, id);
                scope.Complete();
            }
            return JavaScript(string.Format("{0}({1})", Request.Params["callback"], JsonConvert.SerializeObject(o, new JavaScriptDateTimeConverter())));
        }
        /// <summary>
        /// Получение модели представления для редактирования объекта
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>Модель редактирования объекта</returns>
        protected abstract IEditObjectModel GetEditObjectModel(long id);
        /// <summary>
        /// Возвращает представление окна редактирования объекта
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>Представление окна редактирования объекта</returns>
        [HttpPost]
        public virtual ActionResult PopupEdit(long id)
        {
            try
            {
                return PartialView("EditObjectPartial", GetEditObjectModel(id));
            }
            catch (Exception e)
            {
                return Content(ErrorManager.GetFullMessage(e));
            }
        }
    }
}
