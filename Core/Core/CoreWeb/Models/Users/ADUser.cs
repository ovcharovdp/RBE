namespace CoreWeb.Models.Users
{
    /// <summary>
    /// Структура данных о пользователе из сервиса Active Directory
    /// </summary>
    public class ADUser
    {
        /// <summary>
        /// Псевдоним пользователя
        /// </summary>
        public string Alias;
        /// <summary>
        /// Псевдоним пользователя с префиксом названия домена
        /// </summary>
        public string Name;
        /// <summary>
        /// ФИО пользователя
        /// </summary>
        public string FullName;
        /// <summary>
        /// Домен
        /// </summary>
        public string Domain;
    }
}