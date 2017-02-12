using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rar.Model
{
    public class RarSubdevision
    {
        #region - Public Properties -
        public string Name { set; get; }
        public int KPP { set; get; }
        public RarAdress Adress { set; get; }
        #endregion
        #region - Constructor -
        public RarSubdevision()
        {
            Adress = new RarAdress();
        } 
        #endregion
    }
}
