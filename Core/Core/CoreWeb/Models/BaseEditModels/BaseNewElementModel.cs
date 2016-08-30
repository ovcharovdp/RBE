using CoreAPI.Types;
using System;

namespace CoreWeb.Models.BaseEditModels
{
    /// <summary>
    /// Базовый класс для реализации моделей пользовательского интерфейса создания объекта
    /// </summary>
    public class BaseNewElementModel : BaseEditModel, INewObjectModel
    {
        /// <inheritdoc />
        public BaseNewElementModel(ICoreDBContext db, long fieldGroupID)
            : base(db, fieldGroupID)
        { }
        /// <summary>
        /// Событие сохранение
        /// </summary>
        public virtual string SaveEvent
        {
            get { return "newElement()"; }
        }
        /// <summary>
        /// Событие отмены
        /// </summary>
        public virtual string CancelEvent { get { throw new NotImplementedException(); } }
    }
}