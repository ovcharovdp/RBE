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

namespace CoreWeb.Areas.Fuel.Controllers.OData
{
    /*
    Для класса WebApiConfig может понадобиться внесение дополнительных изменений, чтобы добавить маршрут в этот контроллер. Объедините эти инструкции в методе Register класса WebApiConfig соответствующим образом. Обратите внимание, что в URL-адресах OData учитывается регистр символов.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using CoreDM;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<FlOrder>("FlOrders");
    builder.EntitySet<TRNAuto>("TRNAutos"); 
    builder.EntitySet<FlOrderItem>("FlOrderItems"); 
    builder.EntitySet<OrgDepartment>("OrgDepartments"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class FlOrdersController : ODataController
    {
        private CoreEntities db = new CoreEntities();

        // GET: odata/FlOrders
        [EnableQuery]
        public IQueryable<FlOrder> GetFlOrders()
        {
            return db.FlOrders;
        }

        // GET: odata/FlOrders(5)
        [EnableQuery]
        public SingleResult<FlOrder> GetFlOrder([FromODataUri] long key)
        {
            return SingleResult.Create(db.FlOrders.Where(flOrder => flOrder.ID == key));
        }

        // PUT: odata/FlOrders(5)
        public IHttpActionResult Put([FromODataUri] long key, Delta<FlOrder> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            FlOrder flOrder = db.FlOrders.Find(key);
            if (flOrder == null)
            {
                return NotFound();
            }

            patch.Put(flOrder);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlOrderExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(flOrder);
        }

        // POST: odata/FlOrders
        //public IHttpActionResult Post(FlOrder flOrder)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.FlOrders.Add(flOrder);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (FlOrderExists(flOrder.ID))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Created(flOrder);
        //}

        //// PATCH: odata/FlOrders(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public IHttpActionResult Patch([FromODataUri] long key, Delta<FlOrder> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    FlOrder flOrder = db.FlOrders.Find(key);
        //    if (flOrder == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(flOrder);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FlOrderExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(flOrder);
        //}

        //// DELETE: odata/FlOrders(5)
        //public IHttpActionResult Delete([FromODataUri] long key)
        //{
        //    FlOrder flOrder = db.FlOrders.Find(key);
        //    if (flOrder == null)
        //    {
        //        return NotFound();
        //    }

        //    db.FlOrders.Remove(flOrder);
        //    db.SaveChanges();

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // GET: odata/FlOrders(5)/Auto
        [EnableQuery]
        public SingleResult<TRNAuto> GetAuto([FromODataUri] long key)
        {
            return SingleResult.Create(db.FlOrders.Where(m => m.ID == key).Select(m => m.Auto));
        }

        // GET: odata/FlOrders(5)/Items
        [EnableQuery]
        public IQueryable<FlOrderItem> GetItems([FromODataUri] long key)
        {
            return db.FlOrders.Where(m => m.ID == key).SelectMany(m => m.Items);
        }

        // GET: odata/FlOrders(5)/TankFarm
        [EnableQuery]
        public SingleResult<OrgDepartment> GetTankFarm([FromODataUri] long key)
        {
            return SingleResult.Create(db.FlOrders.Where(m => m.ID == key).Select(m => m.TankFarm));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FlOrderExists(long key)
        {
            return db.FlOrders.Count(e => e.ID == key) > 0;
        }
    }
}
