using CoreAPI.Operations;
using CoreAPI.Types;
using CoreDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FuelAPI.Operations
{
    public class StationOperations : BaseDBOperations
    {
        public static long New(CoreEntities DB, FlStation element)
        {
            try
            {
                if (element.ID == 0)
                    element.ID = GetNextID(DB);
                DB.FlStations.Add(element);
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при создании АЗС.", e);
            }
            return element.ID;
        }
        public static void Del(ICoreDBContext DB, long id)
        {
            try
            {
                var db = DB.CoreEntities;
                FlStation element = db.FlStations.Find(id);
                if (element != null)
                {
                    db.FlStations.Remove(element);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления АЗС.", e);
            }
        }
    }
}
