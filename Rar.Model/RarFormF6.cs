using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Windows;

namespace Rar.Model
{
    public class RarFormF6
    {
        #region - Public Properties -
        public DateTime DocumentDate { set; get; }
        public string Version { set; get; }
        public string ProgramName { set; get; }
        public string FormNumber { set; get; }
        public int ReportPeriod { set; get; }
        public int YearReport { set; get; }
        public int CorrectionNumber { set; get; }
        public RarOurCompany OurCompany { set; get; }
        public List<RarCompany> BuyersList { set; get; }
        public List<RarCompany> ManufacturersList { set; get; }
        public List<RarTurnoverData> TurnoverDataList { set; get; }
        #endregion
        #region - Constructor -
        public RarFormF6()
        {
            OurCompany = new RarOurCompany();
            BuyersList = new List<RarCompany>();
            ManufacturersList = new List<RarCompany>();
            TurnoverDataList = new List<RarTurnoverData>();

            DocumentDate = DateTime.Now;
            Version = "NoVersion";
            ProgramName = "NoProgramName";
            FormNumber = "NoFormNumber";
            ReportPeriod = 0;
            YearReport = 0;
            CorrectionNumber = 999;
        }
        #endregion

        public void LoadF6(string filename)
        {
            BuyersList.Clear();
            ManufacturersList.Clear();
            TurnoverDataList.Clear();
            try
            {
                ParserF6.Parse(filename, this);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

    }
}
