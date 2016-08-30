using CoreDM;

namespace CoreAPI.Types
{
    /// <summary>
    /// Контекст БД
    /// </summary>
    public class CoreDBContext : ICoreDBContext
    {
        const string CoreDMContextKey = "CoreDMEntities";
        CoreEntities _coreEntities;
        /// <summary>
        /// Источник данных сущностей системы
        /// </summary>
        public CoreEntities CoreEntities
        {
            get
            {
                if (_coreEntities == null)
                    _coreEntities = new CoreEntities();
                return _coreEntities;
            }
        }
    }
}
