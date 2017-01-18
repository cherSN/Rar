using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rar
{
    public class RarFile
    {
        public DateTime DocumentDate { set; get; }
        public string Version { set; get; }
        public string ProgramName { set; get; }
    }

    public class RarCompany
    {
        public string Name { set; get; }
        public string INN { set; get; }
        public string KPP { set; get; }
        public RarAdress Adress { set; get; }
        List<RarAdress> LicensesList;
    }

    public class RarAdress {
        public string CountryId { set; get; }
        public string PostCode { set; get; }
        public string RegionId { set; get; }
        public string District { set; get; }
        public string City { set; get; }
        public string Locality { set; get; }
        public string Street { set; get; }
        public string Building { set; get;}
        public string Block { set; get; }
        public string Litera { set; get; }
        public string Apartment { set; get; }

    }

    public class RarLicense
    {
        public string Series { set; get; }
        public string Number { set; get; }
        public DateTime DateFrom { set; get; }
        public DateTime DateTo { set; get; }
        public string Issuer { set; get; }
    }
}

