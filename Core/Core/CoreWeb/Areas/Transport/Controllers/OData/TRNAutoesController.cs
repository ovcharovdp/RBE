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

namespace CoreWeb.Areas.Transport.Controllers.OData
{
    /*
    Для класса WebApiConfig может понадобиться внесение дополнительных изменений, чтобы добавить маршрут в этот контроллер. Объедините эти инструкции в методе Register класса WebApiConfig соответствующим образом. Обратите внимание, что в URL-адресах OData учитывается регистр символов.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using CoreDM;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<TRNAuto>("TRNAutoes");
    builder.EntitySet<OrgDepartment>("OrgDepartments"); 
    builder.EntitySet<SysDictionary>("SysDictionaries"); 
    builder.EntitySet<TRNAutoSection>("TRNAutoSections"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class TRNAutoesController : ODataController
    {
        private CoreEntities db = new CoreEntities();

        // GET: odata/TRNAutoes
        [EnableQuery]
        public IQueryable<TRNAuto> GetTRNAutoes()
        {
            return db.TRNAutos;
        }

        // GET: odata/TRNAutoes(5)
        [EnableQuery]
        public SingleResult<TRNAuto> GetTRNAuto([FromODataUri] long key)
        {
            return SingleResult.Create(db.TRNAutos.Where(tRNAuto => tRNAuto.ID == key));
        }

        // PUT: odata/TRNAutoes(5)
        //public IHttpActionResult Put([FromODataUri] long key, Delta<TRNAuto> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    TRNAuto tRNAuto = db.TRNAutos.Find(key);
        //    if (tRNAuto == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Put(tRNAuto);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TRNAutoExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(tRNAuto);
        //}

        //// POST: odata/TRNAutoes
        //public IHttpActionResult Post(TRNAuto tRNAuto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.TRNAutos.Add(tRNAuto);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (TRNAutoExists(tRNAuto.ID))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Created(tRNAuto);
        //}

        //// PATCH: odata/TRNAutoes(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public IHttpActionResult Patch([FromODataUri] long key, Delta<TRNAuto> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    TRNAuto tRNAuto = db.TRNAutos.Find(key);
        //    if (tRNAuto == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(tRNAuto);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TRNAutoExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(tRNAuto);
        //}

        //// DELETE: odata/TRNAutoes(5)
        //public IHttpActionResult Delete([FromODataUri] long key)
        //{
        //    TRNAuto tRNAuto = db.TRNAutos.Find(key);
        //    if (tRNAuto == null)
        //    {
        //        return NotFound();
        //    }

        //    db.TRNAutos.Remove(tRNAuto);
        //    db.SaveChanges();

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // GET: odata/TRNAutoes(5)/Organization
        [EnableQuery]
        public SingleResult<OrgDepartment> GetOrganization([FromODataUri] long key)
        {
            return SingleResult.Create(db.TRNAutos.Where(m => m.ID == key).Select(m => m.Organization));
        }

        // GET: odata/TRNAutoes(5)/Model
        [EnableQuery]
        public SingleResult<SysDictionary> GetModel([FromODataUri] long key)
        {
            return SingleResult.Create(db.TRNAutos.Where(m => m.ID == key).Select(m => m.Model));
        }

        // GET: odata/TRNAutoes(5)/Sections
        [EnableQuery]
        public IQueryable<TRNAutoSection> GetSections([FromODataUri] long key)
        {
            return db.TRNAutos.Where(m => m.ID == key).SelectMany(m => m.Sections);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TRNAutoExists(long key)
        {
            return db.TRNAutos.Count(e => e.ID == key) > 0;
        }
    }
}
