using System.Collections.Generic;
using System.Linq;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using CoreDM;
using System.Text.RegularExpressions;

namespace CoreWeb.Models.Users
{
    /// <summary>
    /// Пространство имен для реализации моделей работы с пользователями системы
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }
    /// <summary>
    /// Модель получения пользователей из Active Directory
    /// </summary>
    public class ADUtils
    {
        /// <summary>
        /// Обновление ФИО пользователей
        /// </summary>
        /// <param name="db">Контекст базы данных</param>
        /// <param name="domains">Список доменов</param>
        /// <returns>Пользователей, у которых обновилась данные по ФИО</returns>
        public static IEnumerable<SysUser> UpdateUsersDisplayName(CoreEntities db, string[] domains)
        {
            var emptyFioUsers = db.SysUsers.Where(p => p.IsAD && string.IsNullOrEmpty(p.FullName)).ToList();

            List<ADUser> lUsers = GetUsers(domains, null, string.Format("(&(objectClass=user)(objectCategory=person)(|(samaccountname={0}*)))", string.Join("*)(samaccountname=", emptyFioUsers.Select(p => Regex.Replace(p.Alias, ".*\\\\(.*)", "$1", RegexOptions.None))))).ToList();
            foreach (var _u in lUsers.Where(p => !string.IsNullOrEmpty(p.FullName)))
            {
                if (emptyFioUsers.Exists(p => p.Name.ToUpper().Equals(_u.Name.ToUpper())))
                    emptyFioUsers.First(p => p.Name.ToUpper().Equals(_u.Name.ToUpper())).FullName = _u.FullName;
            }
            db.SaveChanges();
            return emptyFioUsers.Where(p => !string.IsNullOrEmpty(p.FullName));
        }

        /// <summary>
        /// Поиск пользователей в Active Directory
        /// </summary>
        /// <param name="domains">Список доменов, в которых происходит поиск пользователя</param>
        /// <param name="alias">Фильтр на псевдоним пользователя</param>
        /// <param name="customFilter">Фильтр на запрос в ActiveDirectory</param>
        /// <returns>Список пользователей</returns>
        public static IEnumerable<ADUser> GetUsers(string[] domains, string alias, string customFilter = "")
        {
            List<ADUser> lstADUsers = new List<ADUser>();
            foreach (string domain in domains)
            {
                try
                {
                    string DomainPath = "LDAP://" + domain;
                    DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
                    DirectorySearcher search = new DirectorySearcher(searchRoot);
                    if (string.IsNullOrEmpty(customFilter))
                        search.Filter = "(&(objectClass=user)(objectCategory=person)(samaccountname=" + alias + "*))";
                    else
                        search.Filter = customFilter;
                    search.PropertiesToLoad.Add("samaccountname");
                    search.PropertiesToLoad.Add("displayname");
                    SearchResultCollection resultCol = search.FindAll();
                    if (resultCol != null)
                    {
                        foreach (SearchResult counter in resultCol)
                        {
                            string UserNameEmailString = string.Empty;
                            if (counter.Properties.Contains("samaccountname") &&
                                counter.Properties.Contains("displayname"))
                            {
                                ADUser _user = new ADUser()
                                {
                                    Alias = counter.Properties["samaccountname"][0].ToString().ToUpper(),
                                    Name = searchRoot.Properties["name"].Value.ToString().ToUpper() + "\\" + counter.Properties["samaccountname"][0].ToString(),
                                    Domain = domain,
                                    FullName = counter.Properties["displayname"][0].ToString()
                                };
                                lstADUsers.Add(_user);
                            }
                        }
                    }
                }
                catch { }
            }
            return lstADUsers.OrderBy(p => p.FullName);
        }
        /// <summary>
        /// Возвращает учетку из Active Directory
        /// </summary>
        /// <param name="domain">Домен, в котором осуществляется поиск учетной записи</param>
        /// <param name="alias">Псевдоним пользователя</param>
        /// <returns>Описание учетной записи</returns>
        public static ADUser GetUser(string domain, string alias)
        {
            string DomainPath = "LDAP://" + domain;
            DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
            DirectorySearcher search = new DirectorySearcher(searchRoot);
            search.Filter = "(&(objectClass=user)(objectCategory=person)(samaccountname=" + alias + "))";
            search.PropertiesToLoad.Add("samaccountname");
            search.PropertiesToLoad.Add("displayname");
            PropertyCollection pc = searchRoot.Properties;
            SearchResult r = search.FindOne();

            if (r == null) return null;
            return new ADUser()
            {
                Name = searchRoot.Properties["name"].Value.ToString().ToUpper() + "\\" + r.Properties["samaccountname"][0].ToString(),
                FullName = r.Properties["displayname"][0].ToString()
            };

        }
        /// <summary>
        /// Возвращает домены Active Directory
        /// </summary>
        /// <returns>Список доменов</returns>
        public static string[] GetDomains()
        {
            List<string> d = new List<string>();

            foreach (Domain dom in Forest.GetCurrentForest().Domains)
            {
                try
                {
                    d.Add(dom.Name);
                }
                catch { }
            }
            return d.OrderBy(p => p).ToArray();
        }
    }
}