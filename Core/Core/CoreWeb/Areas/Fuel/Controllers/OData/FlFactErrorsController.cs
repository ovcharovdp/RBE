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
    builder.EntitySet<FlFactError>("FlFactErrors");
    builder.EntitySet<SysDictionary>("SysDictionaries"); 
    builder.EntitySet<FlStation>("FlStations"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class FlFactErrorsController : ODataController
    {
        private CoreEntities db = new CoreEntities();

        // GET: odata/FlFactErrors
        [EnableQuery]
        public IQueryable<FlFact> GetFlFactErrors()
        {
            return db.FlFacts;
        }

        // GET: odata/FlFactErrors(5)
        [EnableQuery]
        public SingleResult<FlFact> GetFlFactError([FromODataUri] long key)
        {
            return SingleResult.Create(db.FlFacts.Where(flFactError => flFactError.ID == key));
        }

        // PUT: odata/FlFactErrors(5)
        //public IHttpActionResult Put([FromODataUri] long key, Delta<FlFactError> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    FlFactError flFactError = db.FlFactErrors.Find(key);
        //    if (flFactError == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Put(flFactError);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FlFactErrorExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(flFactError);
        //}

        //// POST: odata/FlFactErrors
        //public IHttpActionResult Post(FlFactError flFactError)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.FlFactErrors.Add(flFactError);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (FlFactErrorExists(flFactError.ID))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Created(flFactError);
        //}

        //// PATCH: odata/FlFactErrors(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public IHttpActionResult Patch([FromODataUri] long key, Delta<FlFactError> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    FlFactError flFactError = db.FlFactErrors.Find(key);
        //    if (flFactError == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(flFactError);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FlFactErrorExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(flFactError);
        //}

        //// DELETE: odata/FlFactErrors(5)
        //public IHttpActionResult Delete([FromODataUri] long key)
        //{
        //    FlFactError flFactError = db.FlFactErrors.Find(key);
        //    if (flFactError == null)
        //    {
        //        return NotFound();
        //    }

        //    db.FlFactErrors.Remove(flFactError);
        //    db.SaveChanges();

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // GET: odata/FlFactErrors(5)/State
        [EnableQuery]
        public SingleResult<SysDictionary> GetState([FromODataUri] long key)
        {
            return SingleResult.Create(db.FlFacts.Where(m => m.ID == key).Select(m => m.State));
        }

        // GET: odata/FlFactErrors(5)/Station
        [EnableQuery]
        public SingleResult<FlStation> GetStation([FromODataUri] long key)
        {
            return SingleResult.Create(db.FlFacts.Where(m => m.ID == key).Select(m => m.Station));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FlFactErrorExists(long key)
        {
            return db.FlFacts.Count(e => e.ID == key) > 0;
        }
    }
}
