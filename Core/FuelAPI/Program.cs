using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            Logistica.UploadWaybills();
            Console.ReadKey();
        }
    }
}
