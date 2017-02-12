using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rar.Model
{
    public class RarLicense
    {
        #region - Public Properties -
        public string ID { set; get; }
        public string SeriesNumber { set; get; }
        public DateTime DateFrom { set; get; }
        public DateTime DateTo { set; get; }
        public string Issuer { set; get; }
        public string BusinesType { set; get; } 
        #endregion
    }
}
