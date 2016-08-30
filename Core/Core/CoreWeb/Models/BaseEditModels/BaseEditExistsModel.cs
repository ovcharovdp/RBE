using CoreAPI.Types;
using CoreDM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreWeb.Models.BaseEditModels
{
    /// <summary>
    /// Пространство имен для реализации моделей пользовательского интерфейса объекта
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Базовый класс для реализации моделей пользовательского интерфейса редактирования объекта
    /// </summary>
    public class BaseEditExistsModel : BaseEditModel, IEditObjectModel
    {
        /// <summary>
        /// Редактируемый объект
        /// </summary>
        protected object _element;
        private long _id;
        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        public long ID { get { return _id; } }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="id">Идентификатор редактируемого объекта</param>
        /// <param name="fieldGroupID">Идентификатор группы, в которой храняться параметры для описания правил заполнения полей класса</param>
        public BaseEditExistsModel(ICoreDBContext db, long id, long fieldGroupID) : base(db, fieldGroupID) { _id = id; }
        /// <summary>
        /// Инициализация полей параметров для формы редактирования на основе полей объекта
        /// </summary>
        protected override void InitFields()
        {
            if (_element == null)
            {
                throw new Exception(string.Format("Не найден объект (ID={0})", _id));
            }
            base.InitFields();

            foreach (var r in _params)
            {
                var prop = _element.GetType().GetProperties().SingleOrDefault(p => p.Name.ToUpper() == r.Code.ToUpper());
                if (prop != null && prop.GetValue(_element) != null)
                {
                    r.Values = new List<ObjParamValue>();
                    ObjParamValue val = new ObjParamValue() { Order = 0 };
                    switch (r.Type)
                    {
                        case "DATE":
                            val.DateValue = (DateTime)prop.GetValue(_element);
                            break;
                        case "NUMBER":
                            val.NumberValue = Convert.ToDecimal(prop.GetValue(_element));
                            break;
                        case "LOGIC":
                            val.NumberValue = Convert.ToInt16(prop.GetValue(_element));
                            break;
                        case "OBJECT":
                            val.ObjectValue = Convert.ToInt64(prop.GetValue(_element));
                            break;
                        default:
                            val.VarcharValue = (string)prop.GetValue(_element);
                            break;
                    }
                    r.Values.Add(val);
                }
            }
        }
        /// <summary>
        /// Действие при нажатии на кнопку "Сохранить"
        /// </summary>
        public virtual string SaveEvent { get { return string.Format("editElement(\"{0}\")", _id); } }
        /// <summary>
        /// Действие при нажатии на кнопку "Отмена"
        /// </summary>
        public virtual string CancelEvent { get { throw new NotImplementedException(); } }
    }
}