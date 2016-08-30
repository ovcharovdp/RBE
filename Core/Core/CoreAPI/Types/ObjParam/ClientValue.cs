namespace CoreAPI.Types.ObjParam
{
    /// <summary>
    /// Пространство имен для описания типов, необходимых при работе с параметрами
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Класс для десериализации значений параметров, передаваемых с клиента
    /// </summary>
    public class ClientValue
    {
        /// <summary>
        /// Родительский объект
        /// </summary>
        public long p { get; set; }
        /// <summary>
        /// Идентификатор параметра
        /// </summary>
        public long i { get; set; }
        /// <summary>
        /// Код параметра
        /// </summary>
        public string c { get; set; }
        /// <summary>
        /// Порядок значения
        /// </summary>
        public byte o { get; set; }
        /// <summary>
        /// Состояние значения
        /// (n - новое; d - удаление; c - изменение)
        /// </summary>
        public string s { get; set; }
        /// <summary>
        /// Значение
        /// </summary>
        public object v { get; set; }
    }
}