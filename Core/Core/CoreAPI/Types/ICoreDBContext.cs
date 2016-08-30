using CoreDM;

namespace CoreAPI.Types
{
    /// <summary>
    /// Интерфейс доступа к контексту БД ядра
    /// </summary>
    public interface ICoreDBContext
    {
        /// <summary>
        /// Получение контекста БД ядра
        /// </summary>
        CoreEntities CoreEntities { get; }
    }
}
