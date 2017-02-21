//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Rar.Model;
using System.Windows.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Rar.Model
{
    public class RarAdress
    {
        #region - Public Properties -
        public string CountryId { set; get; }
        public string PostCode { set; get; }
        public string RegionId { set; get; }
        public string District { set; get; }
        public string City { set; get; }
        public string Locality { set; get; }
        public string Street { set; get; }
        public string Building { set; get; }
        public string Block { set; get; }
        public string Litera { set; get; }
        public string Apartment { set; get; }

        public bool StrictAdress { set; get; }
        public string AdressString { set; get; }
        #endregion
        #region - Constructors -
        public RarAdress() { }
        public RarAdress(string adress)
        {
            StrictAdress = false;
            AdressString = adress;
        } 
        #endregion
        public override string ToString()
        {
            if (StrictAdress)
            {
                return CountryId + "," + PostCode + "," + RegionId + "," + District + "," +
                City + "," + Locality + "," + Street + "," + Building + "," + Block + "," + Litera + "," + Apartment;
            }
            else
            {
                return AdressString;
            }
            
        }
    }
}
