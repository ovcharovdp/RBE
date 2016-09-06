using CoreAPI.Helpers;
using CoreAPI.Operations;
using CoreAPI.Types;
using CoreDM;
using CoreWeb.Attributes.MVC;
using CoreWeb.Controllers.Base;
using CoreWeb.Models.BaseEditModels;
using CoreWeb.Models.EditModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreWeb.Controllers
{
    /// <summary>
    /// Контроллер управления журналом пользователей
    /// </summary>
    [ExtAuthorize(AdminOnly = true)]
    public class UserController : BaseDocFlowEntityController
    {
        /// <inheritdoc />
        public UserController(ICoreDBContext db) : base(db) { }
        /// <inheritdoc />
        protected override string CatalogGID { get { return "6042C665-9912-4877-B8EE-6A301E80956C"; } }
        /// <inheritdoc />
        protected override object IntNew(IEnumerable<CoreAPI.Types.ObjParam.ClientValue> data)
        {
            SysUser user = new SysUser();

            InitObject(user, data);

            string alias = user.Alias.ToUpper();
            if (UserExists(alias))
                throw new Exception(string.Format("Пользователь уже существует (NAME={0}).", alias));

            user.Name = user.Alias;
            user.Alias = alias;
            user.IsAD = false;

            if (!string.IsNullOrEmpty(user.Password))
            {
                user.Password = Crypto.GetMd5Hash(user.Password);
            }
            SysUserOperations.New(CoreDB, user);
            return user;
        }
        /// <inheritdoc />
        protected override string ODataEntity { get { return "SysUsers"; } }
        /// <inheritdoc />
        protected override void IntDel(long id)
        {
            SysUserOperations.Del(CoreDB, id);
        }
        /// <inheritdoc />
        protected override object IntEdit(IEnumerable<CoreAPI.Types.ObjParam.ClientValue> data, long id)
        {
            SysUser user = CoreDB.SysUsers.Find(id);
            if (user == null)
                throw new Exception(string.Format("Не найден редактируемый пользователь (ID={0}).", id));

            string pass = user.Password;
            string alias = user.Alias;

            InitObject(user, data);

            if (!alias.Equals(user.Alias))
            {
                user.Alias = user.Alias.ToUpper();
                if (UserExists(user.Alias))
                    throw new Exception(string.Format("Пользователь уже существует (NAME={0}).", alias));
            }
            user.Name = user.Alias;

            if (!user.IsAD && !pass.Equals(user.Password))
            {
                user.Password = Crypto.GetMd5Hash(user.Password);
            }
            SysUserOperations.Update(CoreDB, user);
            return user;
        }
        /// <inheritdoc />
        protected override Models.BaseEditModels.IEditObjectModel GetEditObjectModel(long id)
        {
            return new UserModel(_db, id, ParamGroupID) as IEditObjectModel;
        }
        /// <inheritdoc />
        private bool UserExists(string name)
        {
            if (CoreDB.SysUsers.Count(p => p.Alias.ToUpper().Equals(name)) > 0)
                return true;
            return false;
        }
    }
}