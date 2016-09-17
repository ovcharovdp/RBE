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
    public class OrderOperations: BaseDBOperations
    {
        public static long New(CoreEntities DB, FlOrder element)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (element.ID == 0)
                        element.ID = GetNextID(DB);
                    DB.FlOrders.Add(element);
                    DB.SaveChanges();
                    scope.Complete();
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка при создании заказа.", e);
                }
            }
            return element.ID;
        }
    }
}
