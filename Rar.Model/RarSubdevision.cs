using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rar.Model
{
    public class RarSubdevision
    {
        public string Name { set; get; }
        public string KPP { set; get; }
        public RarAdress Adress { set; get; }
        public bool SalePresented { set; get; }
        public bool ReturnPresented { set; get; }
        public RarSubdevision()
        {
            Adress = new RarAdress();
        }
    }
}
