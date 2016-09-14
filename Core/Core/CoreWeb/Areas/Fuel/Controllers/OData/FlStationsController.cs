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
using BaseEntities;
using CoreWeb.Attributes.OData;

namespace CoreWeb.Areas.Fuel.Controllers.OData
{
    /*
    Для класса WebApiConfig может понадобиться внесение дополнительных изменений, чтобы добавить маршрут в этот контроллер. Объедините эти инструкции в методе Register класса WebApiConfig соответствующим образом. Обратите внимание, что в URL-адресах OData учитывается регистр символов.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using CoreDM;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<FlStation>("FlStations");
    builder.EntitySet<OrgDepartment>("OrgDepartments"); 
    builder.EntitySet<SysDictionary>("SysDictionaries"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    [ExtAuthorize]
    public class FlStationsController : ODataController
    {
        private CoreEntities db = new CoreEntities();

        // GET: odata/FlStations
        [EnableQuery]
        public IQueryable<FlStation> GetFlStations()
        {
            return db.FlStations;
        }

        // GET: odata/FlStations(5)
        [EnableQuery]
        public SingleResult<FlStation> GetFlStation([FromODataUri] long key)
        {
            return SingleResult.Create(db.FlStations.Where(flStation => flStation.ID == key));
        }

        // PUT: odata/FlStations(5)
        //public IHttpActionResult Put([FromODataUri] long key, Delta<FlStation> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    FlStation flStation = db.FlStations.Find(key);
        //    if (flStation == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Put(flStation);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FlStationExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(flStation);
        //}

        //// POST: odata/FlStations
        //public IHttpActionResult Post(FlStation flStation)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.FlStations.Add(flStation);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (FlStationExists(flStation.ID))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Created(flStation);
        //}

        //// PATCH: odata/FlStations(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public IHttpActionResult Patch([FromODataUri] long key, Delta<FlStation> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    FlStation flStation = db.FlStations.Find(key);
        //    if (flStation == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(flStation);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FlStationExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(flStation);
        //}

        //// DELETE: odata/FlStations(5)
        //public IHttpActionResult Delete([FromODataUri] long key)
        //{
        //    FlStation flStation = db.FlStations.Find(key);
        //    if (flStation == null)
        //    {
        //        return NotFound();
        //    }

        //    db.FlStations.Remove(flStation);
        //    db.SaveChanges();

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // GET: odata/FlStations(5)/Organization
        [EnableQuery]
        public SingleResult<OrgDepartment> GetOrganization([FromODataUri] long key)
        {
            return SingleResult.Create(db.FlStations.Where(m => m.ID == key).Select(m => m.Organization));
        }

        // GET: odata/FlStations(5)/Type
        [EnableQuery]
        public SingleResult<SysDictionary> GetType([FromODataUri] long key)
        {
            return SingleResult.Create(db.FlStations.Where(m => m.ID == key).Select(m => m.Type));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FlStationExists(long key)
        {
            return db.FlStations.Count(e => e.ID == key) > 0;
        }
    }
}
