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
        public string FormNumber { set; get; }
        public string ReportPeriod { set; get; }
        public string YearReport { set; get; }
        public int CorrectionNumber { set; get; }
    }

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

        public bool StrictAdress { set; get; }
        public string AdressString { set; get; }

        public RarAdress() { }
        public RarAdress(string adress)
        {
            StrictAdress = false;
            AdressString = adress;
        }

    }

    public class RarLicense
    {
//        public RarCompany Owner { set; get; }
        public string ID { set; get; }
        public string SeriesNumber { set; get; }
        public DateTime DateFrom { set; get; }
        public DateTime DateTo { set; get; }
        public string Issuer { set; get; }
    }
}

