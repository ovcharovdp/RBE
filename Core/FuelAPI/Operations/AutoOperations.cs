using CoreAPI.Operations;
using CoreDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FuelAPI.Operations
{
    public class AutoOperations: BaseDBOperations
    {
        public static long New(CoreEntities DB, TRNAuto element)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (element.ID == 0)
                        element.ID = GetNextID(DB);
                    DB.TRNAutos.Add(element);
                    DB.SaveChanges();
                    scope.Complete();
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка при создании автотранспорта.", e);
                }
            }
            return element.ID;
        }
    }
}
