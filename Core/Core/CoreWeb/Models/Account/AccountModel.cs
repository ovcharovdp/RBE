using System.ComponentModel.DataAnnotations;

namespace CoreWeb.Models.Account
{
    /// <summary>
    /// Пространство имен для реализации моделей пользовательского интерфейса проверки подлинности пользователя
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Класс для реализации модели пользовательского интеферфеса проверки подлинности пользователя
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        /// <summary>
        /// Запомнить пользователя в системе
        /// </summary>
        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}