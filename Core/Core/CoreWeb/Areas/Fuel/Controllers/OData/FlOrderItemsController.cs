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
    builder.EntitySet<FlOrderItem>("FlOrderItems");
    builder.EntitySet<FlOrder>("FlOrders"); 
    builder.EntitySet<SysDictionary>("SysDictionaries"); 
    builder.EntitySet<FlStation>("FlStations"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class FlOrderItemsController : ODataController
    {
        private CoreEntities db = new CoreEntities();

        // GET: odata/FlOrderItems
        [EnableQuery(MaxExpansionDepth = 3)]
        public IQueryable<FlOrderItem> GetFlOrderItems()
        {
            return db.FlOrderItems;
        }

        // GET: odata/FlOrderItems(5)
        [EnableQuery]
        public SingleResult<FlOrderItem> GetFlOrderItem([FromODataUri] long key)
        {
            return SingleResult.Create(db.FlOrderItems.Where(flOrderItem => flOrderItem.Order.ID == key));
        }

        // PUT: odata/FlOrderItems(5)
        //public IHttpActionResult Put([FromODataUri] long key, Delta<FlOrderItem> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    FlOrderItem flOrderItem = db.FlOrderItems.Find(key);
        //    if (flOrderItem == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Put(flOrderItem);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FlOrderItemExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(flOrderItem);
        //}

        //// POST: odata/FlOrderItems
        //public IHttpActionResult Post(FlOrderItem flOrderItem)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.FlOrderItems.Add(flOrderItem);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (FlOrderItemExists(flOrderItem.OrderID))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Created(flOrderItem);
        //}

        //// PATCH: odata/FlOrderItems(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public IHttpActionResult Patch([FromODataUri] long key, Delta<FlOrderItem> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    FlOrderItem flOrderItem = db.FlOrderItems.Find(key);
        //    if (flOrderItem == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(flOrderItem);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FlOrderItemExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(flOrderItem);
        //}

        //// DELETE: odata/FlOrderItems(5)
        //public IHttpActionResult Delete([FromODataUri] long key)
        //{
        //    FlOrderItem flOrderItem = db.FlOrderItems.Find(key);
        //    if (flOrderItem == null)
        //    {
        //        return NotFound();
        //    }

        //    db.FlOrderItems.Remove(flOrderItem);
        //    db.SaveChanges();

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //// GET: odata/FlOrderItems(5)/Order
        //[EnableQuery]
        //public SingleResult<FlOrder> GetOrder([FromODataUri] long key)
        //{
        //    return SingleResult.Create(db.FlOrderItems.Where(m => m.OrderID == key).Select(m => m.Order));
        //}

        // GET: odata/FlOrderItems(5)/Product
        [EnableQuery]
        public SingleResult<SysDictionary> GetProduct([FromODataUri] long key)
        {
            return SingleResult.Create(db.FlOrderItems.Where(m => m.Order.ID == key).Select(m => m.Product));
        }

        // GET: odata/FlOrderItems(5)/Station
        [EnableQuery]
        public SingleResult<FlStation> GetStation([FromODataUri] long key)
        {
            return SingleResult.Create(db.FlOrderItems.Where(m => m.Order.ID == key).Select(m => m.Station));
        }

        [HttpPost]
        [EnableQuery]
        public IHttpActionResult SetStation([FromODataUri] long key, [FromODataUri] long stationID)
        {
            var item = db.FlOrderItems.Find(key);
            return Ok(item);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FlOrderItemExists(long key)
        {
            return db.FlOrderItems.Count(e => e.Order.ID == key) > 0;
        }
    }
}
