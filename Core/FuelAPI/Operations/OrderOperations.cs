using BaseEntities;
using CoreAPI.Operations;
using CoreDM;
using System;
using System.Linq;

namespace FuelAPI.Operations
{
    public class OrderOperations : BaseDBOperations
    {
        public static long New(CoreEntities DB, FlOrder element)
        {
            try
            {
                if (element.ID == 0)
                    element.ID = GetNextID(DB);
                DB.FlOrders.Add(element);
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при создании заказа.", e);
            }
            return element.ID;
        }
        public static IQueryable<SysDictionary> GetStates(CoreEntities db)
        {
            return from g in db.ObjGroups.Where(p => p.Code.Equals("A991679A-9FD5-49CC-B69D-83E0DF8610CA"))
                   join og in db.ObjGroupObjects on g.ID equals og.GroupID
                   join d in db.SysDictionaries on og.ObjectID equals d.ID
                   select d;
        }
        public static long AddItem(CoreEntities DB, FlOrderItem element)
        {
            try
            {
                if (element.ID == 0)
                    element.ID = GetNextID(DB);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при создании заказа.", e);
            }
            return element.ID;
        }
    }
}
