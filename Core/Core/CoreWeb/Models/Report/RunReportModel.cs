using CoreDM;
using System.Linq;

namespace CoreWeb.Models.Report
{
    /// <summary>
    /// Класс реализации модели пользовательского интерфейса запуска на отображение отчета
    /// </summary>
    public class RunReportModel
    {
        private string _reportPath;
        private string _reportServerUri;
        private string _reportName;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="reportPath">Путь до отчета</param>
        /// <param name="reportServer">Путь до сервера</param>
        /// <param name="reportName">Имя отчета</param>
        public RunReportModel(string reportPath, string reportServer, string reportName)
        {
            _reportPath = reportPath;
            _reportServerUri = reportServer;
            _reportName = reportName;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="id">Идентификатор отчета</param>
        public RunReportModel(CoreEntities db, long id)
        {
            var q = db.SysReports.AsNoTracking().FirstOrDefault(p => p.ID == id);
            _reportPath = q.Path;
            _reportServerUri = q.Url;
            _reportName = q.Name;
        }
        /// <summary>
        /// Путь до отчета
        /// </summary>
        public string ReportPath { get { return _reportPath; } }
        /// <summary>
        /// Путь до сервера
        /// </summary>
        public string ReportServerUri { get { return _reportServerUri; } }
        /// <summary>
        /// Имя отчета
        /// </summary>
        public string ReportName { get { return _reportName; } }
    }
}