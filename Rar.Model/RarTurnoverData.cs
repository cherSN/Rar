using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rar.Model
{
    public class RarTurnoverData
    {
        public RarSubdevision Subdevision { set; get; }
        public string ProductionSortID { set; get; }
        public RarCompany Producter { set; get; }
        public RarCompany Buyer { set; get; }
        public RarLicense License { set; get; }
        public DateTime NotificationDate { set; get; }
        public string NotificationNumber { set; get; }
        public double NotificationTurnover { set; get; }
        public DateTime DocumentDate { set; get; }
        public string DocumentNumber { set; get; }
        public string CustomsDeclarationNumber { set; get; }
        public double Turnover { set; get; }
    }
}
