namespace CoreAPI.Const
{
    /// <summary>
    /// Интерфейс загрузчика значения константы
    /// </summary>
    public interface IConstLoader
    {
        /// <summary>
        /// Загружает значение константы
        /// </summary>
        /// <param name="gid">Глобальный идентификатор константы</param>
        /// <returns>Идентификатор константы</returns>
        long Load(string gid);
    }
}
