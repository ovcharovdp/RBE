namespace BaseEntities
{
    /// <summary>
    /// Правило изменения состояния объекта
    /// </summary>
    public partial class ObjCatalogRule : BaseEntity
    {
        /// <summary>
        /// Наименование (отображается в журнале)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Пиктограмма правила в журнале
        /// </summary>
        public string ImageName { get; set; }
        /// <summary>
        /// Порядок
        /// </summary>
        public byte Order { get; set; }
        /// <summary>
        /// Текст подтверждения выполнения
        /// </summary>
        public string ConfirmText { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Конечное состояние объекта
        /// </summary>
        public virtual ObjCatalogState FinishState { get; set; }
    }
}