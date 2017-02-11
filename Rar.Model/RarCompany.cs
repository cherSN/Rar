using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rar.Model
{
    public class RarCompany
    {
        public string ID { set; get; }
        public bool Producter { set; get; }
        public string Name { set; get; }
        public string INN { set; get; }
        public string KPP { set; get; }
        public string CounryID { set; get; }
        public RarAdress Adress { set; get; }
        public List<RarLicense> LicensesList { set; get; }

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
        public override string ToString() { return Name+" ИНН: "+INN+"; КПП: "+KPP; }
    }
}
