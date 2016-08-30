using System.Linq;
using System.Web.Http.OData;
using CoreDM;
using BaseEntities;
using CoreWeb.Attributes.OData;

namespace CoreWeb.Controllers.OData
{
    /// <summary>
    /// Управляет состояниями объекта
    /// </summary>
    [ExtAuthorize]
    public class ObjStatesController : ODataController
    {
        private CoreEntities db = new CoreEntities();

        /// <summary>
        /// Возвращает состояния объектов
        /// </summary>
        /// <example>
        /// GET: odata/ObjStates
        /// </example>
        /// <returns>Состояния объектов</returns>
        [EnableQuery]
        public IQueryable<ObjCatalogState> GetObjStates()
        {
            return db.ObjCatalogStates;
        }

        //// GET: odata/ObjCatalogStates(5)
        //[EnableQuery]
        //public SingleResult<ObjCatalogState> GetObjCatalogState([FromODataUri] long key)
        //{
        //    return SingleResult.Create(db.ObjCatalogStates.Where(objCatalogState => objCatalogState.ID == key));
        //}

        //// PUT: odata/ObjCatalogStates(5)
        //public IHttpActionResult Put([FromODataUri] long key, Delta<ObjCatalogState> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    ObjCatalogState objCatalogState = db.ObjCatalogStates.Find(key);
        //    if (objCatalogState == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Put(objCatalogState);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ObjCatalogStateExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(objCatalogState);
        //}

        //// POST: odata/ObjCatalogStates
        //public IHttpActionResult Post(ObjCatalogState objCatalogState)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.ObjCatalogStates.Add(objCatalogState);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (ObjCatalogStateExists(objCatalogState.ID))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Created(objCatalogState);
        //}

        //// PATCH: odata/ObjCatalogStates(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public IHttpActionResult Patch([FromODataUri] long key, Delta<ObjCatalogState> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    ObjCatalogState objCatalogState = db.ObjCatalogStates.Find(key);
        //    if (objCatalogState == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(objCatalogState);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ObjCatalogStateExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(objCatalogState);
        //}

        //// DELETE: odata/ObjCatalogStates(5)
        //public IHttpActionResult Delete([FromODataUri] long key)
        //{
        //    ObjCatalogState objCatalogState = db.ObjCatalogStates.Find(key);
        //    if (objCatalogState == null)
        //    {
        //        return NotFound();
        //    }

        //    db.ObjCatalogStates.Remove(objCatalogState);
        //    db.SaveChanges();

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //// GET: odata/ObjCatalogStates(5)/Catalog
        //[EnableQuery]
        //public SingleResult<ObjCatalog> GetCatalog([FromODataUri] long key)
        //{
        //    return SingleResult.Create(db.ObjCatalogStates.Where(m => m.ID == key).Select(m => m.Catalog));
        //}

        /// <summary>
        /// Получает доступные правила
        /// </summary>
        /// <example>
        /// GET: odata/ObjStates(5)/Rules
        /// </example>
        /// <param name="key">Идентификатор состояния</param>
        /// <returns>Возможные правила перехода из переданного состояния</returns>
        [EnableQuery]
        public IQueryable<ObjCatalogRule> GetRules([FromODataUri] long key)
        {
            return db.ObjCatalogStates.Where(m => m.ID == key).SelectMany(m => m.Rules);
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
    }
}
