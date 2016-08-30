using CoreAPI.Types;
using CoreDM;

namespace CoreWeb.Controllers.Base
{
    /// <summary>
    /// Базовый контроллер для доступа к БД
    /// </summary>
    public class BaseDBController : BaseController
    {
        /// <summary>
        /// Провайдер доступа к моделям данных
        /// </summary>
        protected ICoreDBContext _db;
        /// <summary>
        /// Контекст модели данных планформы
        /// </summary>
        protected CoreEntities CoreDB { get { return _db.CoreEntities; } }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к моделям данных</param>
        public BaseDBController(ICoreDBContext db) { _db = db; }
    }
}