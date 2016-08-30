namespace BaseEntities
{
    /// <summary>
    /// Группы
    /// </summary>
    public partial class ObjGroup : BaseEntity
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }
    }
}
