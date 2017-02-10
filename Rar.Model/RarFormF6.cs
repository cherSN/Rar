using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
namespace Rar.Model
{
    public class RarFormF6
    {
        #region - Public Properties -
        public DateTime DocumentDate { set; get; }
        public string Version { set; get; }
        public string ProgramName { set; get; }
        public string FormNumber { set; get; }
        public string ReportPeriod { set; get; }
        public string YearReport { set; get; }
        public int CorrectionNumber { set; get; }
        public RarOurCompany OurCompany { set; get; }
        public ObservableCollection<RarCompany> CompanyList { set; get; }
        public ObservableCollection<RarTurnoverData> TurnoverDataList { set; get; }
        #endregion

        #region - Constructor -
        public RarFormF6()
        {
            OurCompany = new RarOurCompany();
            CompanyList = new ObservableCollection<RarCompany>();
            TurnoverDataList = new ObservableCollection<RarTurnoverData>();

            DocumentDate = DateTime.Now;
            Version = "NoVersion";
            ProgramName = "NoProgramName";
            FormNumber = "NoFormNumber";
            ReportPeriod = "NoReportPeriod";
            YearReport = "NoYear";
            CorrectionNumber = 999;
        } 
        #endregion

        public void LoadF6(string filename) {
            //Очистить все
            //CompanyList.RemoveAll(s => true);
            //TurnoverDataList.RemoveAll(s => true);
            ParserF6.Parse(filename, this);
            }

    }
}
