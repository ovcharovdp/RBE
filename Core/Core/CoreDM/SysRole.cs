
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using BaseEntities;
    
    /// <summary>
    /// Роли доступа
    /// </summary>
    public partial class SysRole: BaseEntity
    {
    	/// <summary>
    	/// Конструктор
    	/// </summary>
        public SysRole()
        {
            this.UserRoles = new HashSet<SysUserRole>();
        }
    
    	/// <summary>
    	/// Наименование
    	/// </summary>
        public string Name { get; set; }
    	/// <summary>
    	/// Код
    	/// </summary>
        public string Code { get; set; }
    	/// <summary>
    	/// Описание
    	/// </summary>
        public string Description { get; set; }
    
    	/// <summary>
    	/// Список связок пользователя с ролью
    	/// </summary>
    	[JsonIgnore]
        public virtual ICollection<SysUserRole> UserRoles { get; set; }
    }
}
