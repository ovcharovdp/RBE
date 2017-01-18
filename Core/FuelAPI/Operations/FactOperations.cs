using CoreAPI.Operations;
using CoreDM;
using System;
using System.Linq;

namespace FuelAPI.Operations
{
    public class FactOperations : BaseDBOperations
    {
        public static long Resolve(CoreEntities DB, FlFact element)
        {
            try
            {
                var q = DB.FlFacts.FirstOrDefault(p => p.FactDate == element.FactDate && p.Station.ID == element.Station.ID);
                if (q != null)
                {
                    element = q;
                }
                else
                {
                    if (element.ID == 0)
                        element.ID = GetNextID(DB);
                    DB.FlFacts.Add(element);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при создании факта слива.", e);
            }
            return element.ID;
        }
    }
}
