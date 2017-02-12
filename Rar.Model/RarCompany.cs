using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rar.Model
{
    public class RarCompany
    {
        #region - Public Properties -
        public string ID { set; get; }
        public string Name { set; get; }
        public int INN { set; get; }
        public int KPP { set; get; }
        public int CounryID { set; get; }
        public RarAdress Adress { set; get; }
        public List<RarLicense> LicensesList { set; get; }
        #endregion
        #region - Constructors -
        public RarCompany()
        {
            Adress = new RarAdress();
            LicensesList = new List<RarLicense>();
        }
        public RarCompany(string name)
        {
            Adress = new RarAdress();
            LicensesList = new List<RarLicense>();
            Name = name;

        }
        #endregion

        public override string ToString() { return Name+" ИНН: "+INN+"; КПП: "+KPP; }
    }
}
