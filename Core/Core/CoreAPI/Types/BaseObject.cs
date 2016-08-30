namespace CoreAPI.Types
{
    /// <summary>
    /// Пространство имен описания основных типов ядра
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Базовый тип для описания объекта
    /// </summary>
    public class BaseObject
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
    }
}
