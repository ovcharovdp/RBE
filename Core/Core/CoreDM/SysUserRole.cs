
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Связка пользователя с ролью
    /// </summary>
    public partial class SysUserRole
    {
    	/// <summary>
    	/// Идентификатор пользователя
    	/// </summary>
        public long UserID { get; set; }
    	/// <summary>
    	/// Идентификатор роли
    	/// </summary>
        public long RoleID { get; set; }
    	/// <summary>
    	/// Дата назначения
    	/// </summary>
        public System.DateTime StartDate { get; set; }
    
    	/// <summary>
    	/// Роль
    	/// </summary>
        public virtual SysRole Role { get; set; }
    	/// <summary>
    	/// Пользователь
    	/// </summary>
        public virtual SysUser User { get; set; }
    }
}
