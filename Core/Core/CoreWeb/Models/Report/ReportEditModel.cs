using CoreAPI.Types;
using CoreWeb.Models.BaseEditModels;

namespace CoreWeb.Models.Report
{
    /// <summary>
    /// Пространство имен для реализации моделей пользовательского интерфейса отчетов
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Класс реализации модели пользовательского интерфейса редактирования отчетов
    /// </summary>
    public class ReportEditModel : BaseEditExistsModel
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="id">Идентификатор отчета</param>
        /// <param name="fieldGroupID">Идентификатор группы отчетов</param>
        public ReportEditModel(ICoreDBContext db, long id, long fieldGroupID)
            : base(db, id, fieldGroupID)
        {
            _element = CoreDB.SysReports.Find(id);
        }
    }
}