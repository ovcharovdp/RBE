using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using CoreDM;

namespace CoreWeb.Areas.Transport.Controllers.OData
{
    /*
    Для класса WebApiConfig может понадобиться внесение дополнительных изменений, чтобы добавить маршрут в этот контроллер. Объедините эти инструкции в методе Register класса WebApiConfig соответствующим образом. Обратите внимание, что в URL-адресах OData учитывается регистр символов.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using CoreDM;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<TNRDriver>("TNRDriver");
    builder.EntitySet<OrgDepartment>("OrgDepartments"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class TRNDriversController : ODataController
    {
        private CoreEntities db = new CoreEntities();

        // GET: odata/TNRDriver
        [EnableQuery]
        public IQueryable<TRNDriver> GetTRNDrivers()
        {
            return db.TRNDrivers;
        }

        // GET: odata/TNRDriver(5)
        [EnableQuery]
        public SingleResult<TRNDriver> GetTRNDrivers([FromODataUri] long key)
        {
            return SingleResult.Create(db.TRNDrivers.Where(tNRDriver => tNRDriver.ID == key));
        }

        //// PUT: odata/TNRDriver(5)
        //public IHttpActionResult Put([FromODataUri] long key, Delta<TNRDriver> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    TNRDriver tNRDriver = db.TNRDrivers.Find(key);
        //    if (tNRDriver == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Put(tNRDriver);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TNRDriverExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(tNRDriver);
        //}

        //// POST: odata/TNRDriver
        //public IHttpActionResult Post(TNRDriver tNRDriver)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.TNRDrivers.Add(tNRDriver);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (TNRDriverExists(tNRDriver.ID))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Created(tNRDriver);
        //}

        //// PATCH: odata/TNRDriver(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public IHttpActionResult Patch([FromODataUri] long key, Delta<TNRDriver> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    TNRDriver tNRDriver = db.TNRDrivers.Find(key);
        //    if (tNRDriver == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(tNRDriver);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TNRDriverExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(tNRDriver);
        //}

        //// DELETE: odata/TNRDriver(5)
        //public IHttpActionResult Delete([FromODataUri] long key)
        //{
        //    TNRDriver tNRDriver = db.TNRDrivers.Find(key);
        //    if (tNRDriver == null)
        //    {
        //        return NotFound();
        //    }

        //    db.TNRDrivers.Remove(tNRDriver);
        //    db.SaveChanges();

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // GET: odata/TNRDriver(5)/Organization
        [EnableQuery]
        public SingleResult<OrgDepartment> GetOrganization([FromODataUri] long key)
        {
            return SingleResult.Create(db.TRNDrivers.Where(m => m.ID == key).Select(m => m.Organization));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TRNDriversExists(long key)
        {
            return db.TRNDrivers.Count(e => e.ID == key) > 0;
        }
    }
}
