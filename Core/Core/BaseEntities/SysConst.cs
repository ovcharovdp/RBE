namespace BaseEntities
{
    /// <summary>
    /// Константа
    /// </summary>
    public class SysConst : BaseEntity
    {
        /// <summary>
        /// GUID константы
        /// </summary>
        public string GID { get; set; }
        /// <summary>
        /// Описание константы
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Значение константы
        /// </summary>
        public long Value { get; set; }
    }
}