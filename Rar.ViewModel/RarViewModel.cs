using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rar.ViewModel
{
    public class RarViewModel : INotifyPropertyChanged
    {
        //private RarFile _RarFile;
        //private RarOurCompany _OurCompany;

        //public DateTime DocumentDate
        //{
        //    get
        //    {
        //        return _RarFile.DocumentDate;
        //    }

        //    set
        //    {
        //        _RarFile.DocumentDate = value;
        //        OnPropertyChanged("DocumentDate");
        //    }
        //}
        //public string Version
        //{
        //    get
        //    {
        //        return _RarFile.Version;
        //    }

        //    set
        //    {
        //        _RarFile.Version = value;
        //        OnPropertyChanged("Version");

        //    }
        //}
        //public string ProgramName     {
        //    get  { return _RarFile.ProgramName; }
        //    set  {
        //        _RarFile.ProgramName = value;
        //        OnPropertyChanged("ProgramName");
        //    }
        //}
        //public string FormNumber  {
        //    get  { return _RarFile.FormNumber;  }
        //    set  {
        //        _RarFile.FormNumber = value;
        //        OnPropertyChanged("FormNumber");
        //    }
        //}
        //public string ReportPeriod   {
        //    get { return _RarFile.ReportPeriod; }
        //    set  {
        //        _RarFile.ReportPeriod = value;
        //        OnPropertyChanged("ReportPeriod");
        //    }
        //}
        //public string YearReport        {
        //    get { return _RarFile.YearReport; }
        //    set             {
        //        _RarFile.YearReport = value;
        //        OnPropertyChanged("YearReport");
        //    }
        //}
        //public int CorrectionNumber
        //{
        //    get { return _RarFile.CorrectionNumber; }
        //    set
        //    {
        //        _RarFile.CorrectionNumber = value;
        //        OnPropertyChanged("CorrectionNumber");
        //    }
        //}

        //public RarOurCompany OurCompany
        //{
        //    get
        //    {
        //        return _OurCompany;
        //    }

        //    set
        //    {
        //        _OurCompany = value;
        //        OnPropertyChanged("OurCompany");

        //    }
        //}


        //public List<RarCompany> CompanyList
        //{
        //    get
        //    {
        //        return companyList;
        //    }

        //    set
        //    {
        //        companyList = value;
        //        OnPropertyChanged("CompanyList");
        //    }
        //}

        //public List<RarTurnoverData> TurnoverDataList
        //{
        //    get
        //    {
        //        return turnoverDataList;
        //    }

        //    set
        //    {
        //        turnoverDataList = value;
        //        OnPropertyChanged("TurnoverDataList");

        //    }
        //}

        //private List<RarTurnoverData> turnoverDataList;

        //private List<RarCompany> companyList;

        public RarViewModel()
        {
            //_RarFile = new RarFile();
            //CompanyList = new List<RarCompany>();
            //OurCompany = new RarOurCompany();
            //TurnoverDataList = new List<RarTurnoverData>();

            //CompanyList.Add(new RarCompany("Первый"));
            //CompanyList.Add(new RarCompany("Второй"));
            //CompanyList.Add(new RarCompany("Третий"));

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
