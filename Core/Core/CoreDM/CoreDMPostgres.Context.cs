namespace CoreDM
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using BaseEntities;
    /// <summary>
    /// Модель данных ядра платформы
    /// </summary>
    public partial class CoreEntities : DbContext
    {
    	/// <inheritdoc />
        public CoreEntities()
            : base("name=CoreEntities")
        {
        }
    	/// <summary>
    	/// Настройка модели перед ее блокировкой
    	/// </summary>
    	/// <param name="modelBuilder">Построитель, который определяет модель для создаваемого контекста</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        /// <summary>
        /// Пользователь системы
        /// </summary>
        public DbSet<SysUser> SysUsers { get; set; }
        /// <summary>
        /// Роли доступа
        /// </summary>
        public DbSet<SysRole> SysRoles { get; set; }
        /// <summary>
        /// Связка пользователя с ролью
        /// </summary>
        public DbSet<SysUserRole> SysUserRoles { get; set; }
        /// <summary>
        /// Унаследованные роли
        /// </summary>
        public DbSet<SysInheritRole> SysInheritRoles { get; set; }
        /// <summary>
        /// Группы
        /// </summary>
        public DbSet<ObjGroup> ObjGroups { get; set; }
        /// <summary>
        /// Связка объекта с группой
        /// </summary>
        public DbSet<ObjGroupObject> ObjGroupObjects { get; set; }
        /// <summary>
        /// Пункт меню
        /// </summary>
        public DbSet<SysMenu> SysMenus { get; set; }
        /// <summary>
        /// Связь объекта с ролью
        /// </summary>
        public DbSet<SysObjectRole> SysObjectRoles { get; set; }
        /// <summary>
        /// Отчеты
        /// </summary>
        public DbSet<SysReport> SysReports { get; set; }
        /// <summary>
        /// Данные объекта (файлы картинок и т.п.)
        /// </summary>
        public DbSet<ObjBlob> ObjBlobs { get; set; }
        /// <summary>
        /// Словарь значений
        /// </summary>
        public DbSet<SysDictionary> SysDictionaries { get; set; }
        /// <summary>
        /// Параметр объекта
        /// </summary>
        public DbSet<ObjParam> ObjParams { get; set; }
        /// <summary>
        /// Допустимые значения параметра
        /// </summary>
        public DbSet<ObjParamItem> ObjParamItems { get; set; }
        /// <summary>
        /// Подчиненные параметры
        /// </summary>
        public DbSet<ObjParamSetItem> ObjParamSetItems { get; set; }
        /// <summary>
        /// Значения параметра объекта
        /// </summary>
        public DbSet<ObjParamValue> ObjParamValues { get; set; }
        /// <summary>
        /// Журнал системы
        /// </summary>
        public DbSet<ObjCatalog> ObjCataloges { get; set; }
        /// <summary>
        /// Константы системы
        /// </summary>
        public DbSet<SysConst> SysConsts { get; set; }
        /// <summary>
        /// Организационная единица
        /// </summary>
        public DbSet<OrgDepartment> OrgDepartments { get; set; }
        /// <summary>
        /// Поля журналов
        /// </summary>
        public DbSet<ObjCatalogField> ObjCatalogFields { get; set; }
        /// <summary>
        /// Состояние объекта
        /// </summary>
        public DbSet<ObjCatalogState> ObjCatalogStates { get; set; }
        /// <summary>
        /// Правило изменения состояния объекта
        /// </summary>
        public DbSet<ObjCatalogRule> ObjCatalogRules { get; set; }
        public DbSet<TRNAutoSection> TRNAutoSections { get; set; }
        public DbSet<TRNAuto> TRNAutos { get; set; }
        public DbSet<TRNDriver> TRNDrivers { get; set; }
    }
}
