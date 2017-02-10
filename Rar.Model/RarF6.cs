using System;
using System.Collections.Generic;
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
        public DateTime DocumentDate { set; get; }
        public string Version { set; get; }
        public string ProgramName { set; get; }
        public string FormNumber { set; get; }
        public string ReportPeriod { set; get; }
        public string YearReport { set; get; }
        public int CorrectionNumber { set; get; }

        public RarFormF6()
        {
            OurCompany = new RarOurCompany();
            CompanyList = new List<RarCompany>();
            TurnoverDataList = new List<RarTurnoverData>();

            DocumentDate = DateTime.Now;
            Version = "NoVersion";
            ProgramName = "NoProgramName";
            FormNumber = "NoFormNumber";
            ReportPeriod = "NoReportPeriod";
            YearReport = "NoYear";
            CorrectionNumber = 999;
        }

        public RarOurCompany OurCompany { set; get; }
        public List<RarCompany> CompanyList { set; get; }
        public List<RarTurnoverData> TurnoverDataList { set; get; }


        public void LoadF6(string filename) {
            //Очистить все
            CompanyList.RemoveAll(s => true);
            TurnoverDataList.RemoveAll(s => true);


            ParserF6.Parse(filename, this);
            }



    }

}
