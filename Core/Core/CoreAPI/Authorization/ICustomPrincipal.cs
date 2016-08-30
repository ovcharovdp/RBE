using System;

namespace CoreAPI.Authorization
{
    /// <summary>
    /// Пространство имен для описания средств авторизации пользователя в системе.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Интерфейс для описания сведений о пользователе
    /// </summary>
    public interface ICustomPrincipal : System.Security.Principal.IPrincipal
    {
        /// <summary>
        /// Имя
        /// </summary>
        string FirstName { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        string LastName { get; set; }
        /// <summary>
        /// Полное имя пользователя
        /// </summary>
        string FullName { get; }
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        long ID { get; }
        /// <summary>
        /// Признак администратора
        /// </summary>
        bool isAdmin { get; set; }
        /// <summary>
        /// Дата последнего входа
        /// </summary>
        DateTime LastLogon { get; set; }
        /// <summary>
        /// Идентификаторы ролей, доступных пользователю
        /// </summary>
        long[] RoleIDs { get; }
    }
}
