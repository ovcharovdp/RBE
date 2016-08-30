namespace CoreDM
{
    /// <summary>
    /// Пространство имен для реализации классов контекста базы данных ядра системы
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Связь объекта с ролью
    /// </summary>
    public partial class SysObjectRole: BaseEntities.SysObjectRole
    {
        /// <summary>
        /// Роль
        /// </summary>
        public virtual SysRole Role { get; set; }
    }
}