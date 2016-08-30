namespace BaseEntities
{
    /// <summary>
    /// Связь объекта с ролью
    /// </summary>
    public partial class SysObjectRole
    {
        /// <summary>
        /// Идентификатор роли
        /// </summary>
        public long RoleID { get; set; }
        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        public long ObjectID { get; set; }
        /// <summary>
        /// Признак доступа на чтение
        /// </summary>
        public string OnRead { get; set; }
        /// <summary>
        /// Признак доступа на изменение
        /// </summary>
        public string OnUpdate { get; set; }
    }
}
