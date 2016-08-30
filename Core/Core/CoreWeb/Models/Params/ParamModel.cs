using CoreAPI.Types;
using CoreDM;
using System.Collections.Generic;
using System.Linq;

namespace CoreWeb.Models.Params
{
    /// <summary>
    /// Пространство имен для реализации моделей пользовательского параметров
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Модель интерфейса редактирования параметров
    /// </summary>
    public class EditControlModel
    {
        private int _order;
        /// <summary>
        /// Настройки
        /// </summary>
        public ParamSettings Settings { get; set; }
        /// <summary>
        /// Значение параметра
        /// </summary>
        public ObjParamValue Value { get; set; }
        /// <summary>
        /// Порядок
        /// </summary>
        public int Order
        {
            get
            {
                if (Value != null)
                    return Value.Order;
                return _order;
            }
            set { _order = value; }
        }
    }

    /// <summary>
    /// Модель интерфейса списка параметров
    /// </summary>
    public class ParamListModel
    {
        /// <summary>
        /// Провайдер доступа к модели данных
        /// </summary>
        protected ICoreDBContext _db;
        /// <summary>
        /// Контекст модели данных ядра
        /// </summary>
        protected CoreEntities CoreDB { get { return _db.CoreEntities; } }
        /// <summary>
        /// Идентификатор родителя
        /// </summary>
        private long _parentID;
        /// <summary>
        /// Получение идентификатора родителя
        /// </summary>
        public long ObjectID { get { return _parentID; } }
        /// <summary>
        /// Идентификатор группы параметров
        /// </summary>
        private long _fieldGroupID;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="parentID">Идентификатор родителя</param>
        /// <param name="groupID">Идентификатор группы</param>
        public ParamListModel(ICoreDBContext db, long parentID, long groupID)
        {
            _db = db;
            _parentID = parentID;
            _fieldGroupID = groupID;
        }
        /// <summary>
        /// Список параметров
        /// </summary>
        protected List<ParamSettings> _params;

        /// <summary>
        /// Инициализация полей параметров для формы редактирования
        /// </summary>
        protected virtual void InitFields()
        {
            _params = (from g in CoreDB.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == _fieldGroupID)
                       join p in CoreDB.ObjParams.AsNoTracking() on g.ObjectID equals p.ID
                       orderby p.Order
                       select new ParamSettings { Param = p, ParentID = _parentID }).ToList();
        }
        /// <summary>
        /// Действие после инициализации полей
        /// </summary>
        protected virtual void PostInitFields() { }
        /// <summary>
        /// Инициализация модели
        /// </summary>
        public void Init()
        {
            InitFields();
            PostInitFields();
        }
        /// <summary>
        /// Список настроек параметров
        /// </summary>
        public IEnumerable<ParamSettings> Params
        {
            get
            {
                if (_params == null)
                {
                    Init();
                }
                return _params;
            }
        }
    }
}