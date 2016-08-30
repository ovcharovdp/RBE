
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Унаследованные роли
    /// </summary>
    public partial class SysInheritRole
    {
    	/// <summary>
    	/// Идентификатор роли-наследника
    	/// </summary>
        public long HeirRoleID { get; set; }
    	/// <summary>
    	/// Идентификатор наследуемой роли
    	/// </summary>
        public long RoleID { get; private set; }
    	/// <summary>
    	/// Дата связки
    	/// </summary>
        public Nullable<System.DateTime> LinkDate { get; set; }
    }
}
