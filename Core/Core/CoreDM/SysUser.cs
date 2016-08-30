
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using BaseEntities;
    
    /// <summary>
    /// Пользователь системы
    /// </summary>
    public partial class SysUser: BaseEntity
    {
    	/// <summary>
    	/// Конструктор
    	/// </summary>
        public SysUser()
        {
            this.IsAD = false;
            this.IsAdmin = false;
            this.UserRoles = new HashSet<SysUserRole>();
        }
    
    	/// <summary>
    	/// Наименование
    	/// </summary>
        public string Name { get; set; }
    	/// <summary>
    	/// Дата последнего входа
    	/// </summary>
        public Nullable<System.DateTime> LastLogon { get; set; }
    	/// <summary>
    	/// Полное имя
    	/// </summary>
        public string FullName { get; set; }
    	/// <summary>
    	/// Псевдоним
    	/// </summary>
        public string Alias { get; set; }
    	/// <summary>
    	/// Признак учетной записи Active Directory
    	/// </summary>
        public bool IsAD { get; set; }
    	/// <summary>
    	/// Пароль
    	/// </summary>
        public string Password { get; set; }
    	/// <summary>
    	/// Электронная почта пользователя
    	/// </summary>
        public string eMail { get; set; }
    	/// <summary>
    	/// Признак того, что пользователь является администратором системы
    	/// </summary>
        public bool IsAdmin { get; set; }
    
    	/// <summary>
    	/// Список связок пользователя с ролью
    	/// </summary>
    	[JsonIgnore]
        public virtual ICollection<SysUserRole> UserRoles { get; set; }
    	/// <summary>
    	/// Состояние
    	/// </summary>
        public virtual ObjCatalogState State { get; set; }
    }
}
