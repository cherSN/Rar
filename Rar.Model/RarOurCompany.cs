using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rar.Model
{
    public class RarOurCompany : RarCompany
    {
        #region - Public Properties -
        public string Phone { set; get; }
        public string Email { set; get; }
        public string UnLisenseActivity { set; get; }
        public RarFIO Director { set; get; }
        public RarFIO Accountant { set; get; }
        public List<RarSubdevision> SubdevisionList { set; get; }
        #endregion
        #region - Constructor - 
        public RarOurCompany() : base()
        {
            Director = new RarFIO();
            Accountant = new RarFIO();
            SubdevisionList = new List<RarSubdevision>();
        } 
        #endregion
    }
}
