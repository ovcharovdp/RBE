using CoreAPI.Operations;
using CoreDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;

namespace FuelAPI.Operations
{
    public class AutoOperations : BaseDBOperations
    {
        public static long New(CoreEntities DB, TRNAuto element)
        {
            try
            {
                if (element.ID == 0)
                    element.ID = GetNextID(DB);
                DB.TRNAutos.Add(element);
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при создании автотранспорта.", e);
            }
            return element.ID;
        }
        public static string GetFormatedRegNum(string value)
        {
            string regNum = value.ToUpper().Replace(" ", "");
            // определяем основную часть номера
            Regex rgx = new Regex(@"[А-Я]\d{3}[А-Я]{2}");
            Match m = rgx.Match(regNum);
            if (m.Success)
            {
                // определяем регион
                Regex rgxRegion = new Regex(@"\d{2,3}$");
                Match mReg = rgxRegion.Match(regNum);
                regNum = m.Value + ((mReg.Success) ? "-" + mReg.Value : "");
            }
            return regNum;
        }
    }
}
