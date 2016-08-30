using CoreAPI.Types;
using System;

namespace CoreWeb.Models.BaseEditModels
{
    /// <summary>
    /// Класс, для получения интерфейса создания модели элемента группы
    /// </summary>
    public class BaseNewGroupedElementModel : BaseEditModel, INewObjectModel
    {
        /// <summary>
        /// Идентификатор родителя
        /// </summary>
        protected long _parentID;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="parentID">Идентификатор родителя</param>
        /// <param name="fieldGroupID">Идентификатор группы поля</param>
        public BaseNewGroupedElementModel(ICoreDBContext db, long parentID, long fieldGroupID)
            : base(db, fieldGroupID)
        {
            _parentID = parentID;
        }
        /// <summary>
        /// Событие сохранение
        /// </summary>
        public virtual string SaveEvent
        {
            get { return string.Format("newElement(\"{0}\")", _parentID); }
        }
        /// <summary>
        /// Событие отмены
        /// </summary>
        public virtual string CancelEvent { get { throw new NotImplementedException(); } }
    }
}