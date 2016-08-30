using System.Linq;
using System.Web.Http;
using System.Web.Http.OData;
using CoreDM;
using System;
using BaseEntities;
using CoreWeb.Attributes.OData;

namespace CoreWeb.Controllers.OData
{
    /// <summary>
    /// Пространство имен для описания контроллеров OData
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Контроллер для управления сущностью "Пользователь"
    /// </summary>
    /// <remarks>
    /// Для использования возможности работы с журналом пользователей необходимо зарегистрировать сущности модели OData в файле WebApiConfig
    /// <code language="C#">
    /// public static void Register(HttpConfiguration config)
    /// {
    ///     ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    ///     ...
    ///     CoreWeb.WebApiConfig.RegisterUserEntity(builder);
    ///     ...
    ///     config.Routes.MapODataServiceRoute("ODataRoute", "odata", builder.GetEdmModel());
    /// }
    /// </code>
    /// </remarks>
    [ExtAuthorize(AdminOnly = true)]
    public class SysUsersController : ODataController
    {
        private CoreEntities db = new CoreEntities();

        /// <summary>
        /// Возвращает пользователей
        /// </summary>
        /// <example>
        /// GET: odata/SysUsers
        /// </example>
        /// <returns>Пользователи</returns>
        [EnableQuery]
        public IQueryable<SysUser> GetSysUsers()
        {
            return db.SysUsers;
        }

        /// <summary>
        /// Возвращает сущность "Пользователь"
        /// </summary>
        /// <example>
        ///  GET: odata/SysUsers(5)
        /// </example>
        /// <param name="key">Идентификатор пользователя</param>
        /// <returns>Сущность "Пользователь"</returns>
        [EnableQuery]
        public SingleResult<SysUser> GetSysUser([FromODataUri] long key)
        {
            return SingleResult.Create(db.SysUsers.Where(sysUser => sysUser.ID == key));
        }

        // PUT: odata/SysUsers(5)
        //public IHttpActionResult Put([FromODataUri] long key, Delta<SysUser> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    SysUser sysUser = db.SysUsers.Find(key);
        //    if (sysUser == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Put(sysUser);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!SysUserExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(sysUser);
        //}

        //// POST: odata/SysUsers
        //public IHttpActionResult Post(SysUser sysUser)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.SysUsers.Add(sysUser);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (SysUserExists(sysUser.ID))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Created(sysUser);
        //}

        //// PATCH: odata/SysUsers(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public IHttpActionResult Patch([FromODataUri] long key, Delta<SysUser> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    SysUser sysUser = db.SysUsers.Find(key);
        //    if (sysUser == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(sysUser);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!SysUserExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(sysUser);
        //}

        /// <summary>
        /// Удаляет пользователя системы
        /// </summary>
        /// <example>
        /// DELETE: odata/SysUsers(5)
        /// </example>
        /// <param name="key">Идентификатор пользователя</param>
        /// <returns>Результат вызова</returns>
        public IHttpActionResult Delete([FromODataUri] long key)
        {
            //SysUser sysUser = db.SysUsers.Find(key);
            //if (sysUser == null)
            //{
            return BadRequest("Test");
          //  return NotFound();
            //}

            //db.SysUsers.Remove(sysUser);
            //db.SaveChanges();

           // return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Получить роли
        /// </summary>
        /// <example>
        /// GET: odata/SysUsers(5)/UserRoles
        /// </example>
        /// <param name="key">Идентификатор пользователя</param>
        /// <returns>Роли</returns>
        [EnableQuery]
        public IQueryable<SysUserRole> GetUserRoles([FromODataUri] long key)
        {
            return db.SysUsers.Where(m => m.ID == key).SelectMany(m => m.UserRoles);
        }
        /// <summary>
        /// Выполняет правило документооборота
        /// </summary>
        /// <param name="key">Идентификатор пользователя</param>
        /// <param name="ruleID">Идентификатор правила</param>
        /// <returns>Пользователь</returns>
        [HttpPost]
        [EnableQuery]
        public IHttpActionResult RunRule([FromODataUri] long key, [FromODataUri] long ruleID)
        {
            SysUser user = db.SysUsers.Include("State.Rules.FinishState").FirstOrDefault(p => p.ID == key);
            if (user == null)
                throw new Exception(string.Format("Пользователь не найден (ID={0}).", key));
            var rule = user.State.Rules.FirstOrDefault(p => p.ID == ruleID);
            if (rule == null)
                throw new Exception(string.Format("Невозможно применить данное правило (ID={0}).", ruleID));
            user.State = rule.FinishState;
            db.SaveChanges();
            return Ok(user);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// GET: odata/SysUsers(5)/State
        /// </example>
        /// <param name="key"></param>
        /// <returns></returns>
        [EnableQuery]
        public SingleResult<ObjCatalogState> GetState([FromODataUri] long key)
        {
            return SingleResult.Create(db.SysUsers.Where(m => m.ID == key).Select(m => m.State));
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SysUserExists(long key)
        {
            return db.SysUsers.Count(e => e.ID == key) > 0;
        }
    }
}
