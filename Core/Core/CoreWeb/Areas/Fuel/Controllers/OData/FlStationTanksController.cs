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

namespace CoreWeb.Areas.Fuel.Controllers.OData
{
    /*
    Для класса WebApiConfig может понадобиться внесение дополнительных изменений, чтобы добавить маршрут в этот контроллер. Объедините эти инструкции в методе Register класса WebApiConfig соответствующим образом. Обратите внимание, что в URL-адресах OData учитывается регистр символов.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using CoreDM;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<FlStationTank>("FlStationTanks");
    builder.EntitySet<FlStation>("FlStations"); 
    builder.EntitySet<SysDictionary>("SysDictionaries"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class FlStationTanksController : ODataController
    {
        private CoreEntities db = new CoreEntities();

        // GET: odata/FlStationTanks
        [EnableQuery]
        public IQueryable<FlStationTank> GetFlStationTanks()
        {
            return db.FlStationTanks;
        }

        // GET: odata/FlStationTanks(5)
        [EnableQuery]
        public SingleResult<FlStationTank> GetFlStationTank([FromODataUri] long key)
        {
            return SingleResult.Create(db.FlStationTanks.Where(flStationTank => flStationTank.ID == key));
        }

        // PUT: odata/FlStationTanks(5)
        //public IHttpActionResult Put([FromODataUri] long key, Delta<FlStationTank> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    FlStationTank flStationTank = db.FlStationTanks.Find(key);
        //    if (flStationTank == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Put(flStationTank);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FlStationTankExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(flStationTank);
        //}

        //// POST: odata/FlStationTanks
        //public IHttpActionResult Post(FlStationTank flStationTank)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.FlStationTanks.Add(flStationTank);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (FlStationTankExists(flStationTank.ID))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Created(flStationTank);
        //}

        //// PATCH: odata/FlStationTanks(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public IHttpActionResult Patch([FromODataUri] long key, Delta<FlStationTank> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    FlStationTank flStationTank = db.FlStationTanks.Find(key);
        //    if (flStationTank == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(flStationTank);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FlStationTankExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(flStationTank);
        //}

        //// DELETE: odata/FlStationTanks(5)
        //public IHttpActionResult Delete([FromODataUri] long key)
        //{
        //    FlStationTank flStationTank = db.FlStationTanks.Find(key);
        //    if (flStationTank == null)
        //    {
        //        return NotFound();
        //    }

        //    db.FlStationTanks.Remove(flStationTank);
        //    db.SaveChanges();

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // GET: odata/FlStationTanks(5)/Station
        [EnableQuery]
        public SingleResult<FlStation> GetStation([FromODataUri] long key)
        {
            return SingleResult.Create(db.FlStationTanks.Where(m => m.ID == key).Select(m => m.Station));
        }

        // GET: odata/FlStationTanks(5)/State
        [EnableQuery]
        public SingleResult<SysDictionary> GetState([FromODataUri] long key)
        {
            return SingleResult.Create(db.FlStationTanks.Where(m => m.ID == key).Select(m => m.State));
        }

        // GET: odata/FlStationTanks(5)/Product
        [EnableQuery]
        public SingleResult<SysDictionary> GetProduct([FromODataUri] long key)
        {
            return SingleResult.Create(db.FlStationTanks.Where(m => m.ID == key).Select(m => m.Product));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FlStationTankExists(long key)
        {
            return db.FlStationTanks.Count(e => e.ID == key) > 0;
        }
    }
}
