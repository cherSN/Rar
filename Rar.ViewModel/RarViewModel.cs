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

namespace Rar.ViewModel
{
    public class RarViewModel : INotifyPropertyChanged
    {
        #region  - Private Fields -
        private RarFormF6 _RarFile;
        #endregion

        #region - Public Properties -
        public DateTime DocumentDate
        {
            get
            {
                return _RarFile.DocumentDate;
            }

            set
            {
                _RarFile.DocumentDate = value;
                OnPropertyChanged("DocumentDate");
            }
        }
        public string Version
        {
            get
            {
                return _RarFile.Version;
            }

            set
            {
                _RarFile.Version = value;
                OnPropertyChanged("Version");

            }
        }
        public string ProgramName
        {
            get { return _RarFile.ProgramName; }
            set
            {
                _RarFile.ProgramName = value;
                OnPropertyChanged("ProgramName");
            }
        }
        public string FormNumber
        {
            get { return _RarFile.FormNumber; }
            set
            {
                _RarFile.FormNumber = value;
                OnPropertyChanged("FormNumber");
            }
        }
        public string ReportPeriod
        {
            get { return _RarFile.ReportPeriod; }
            set
            {
                _RarFile.ReportPeriod = value;
                OnPropertyChanged("ReportPeriod");
            }
        }
        public string YearReport
        {
            get { return _RarFile.YearReport; }
            set
            {
                _RarFile.YearReport = value;
                OnPropertyChanged("YearReport");
            }
        }
        public int CorrectionNumber
        {
            get { return _RarFile.CorrectionNumber; }
            set
            {
                _RarFile.CorrectionNumber = value;
                OnPropertyChanged("CorrectionNumber");
            }
        }
        public RarOurCompany OurCompany
        {
            get
            {
                return _RarFile.OurCompany;
            }

            set
            {
                _RarFile.OurCompany = value;
                OnPropertyChanged("OurCompany");

            }
        }
        public ObservableCollection<RarCompany> CompanyList
        {
            get
            {
                return _RarFile.CompanyList;
            }

            set
            {
                _RarFile.CompanyList = value;
                OnPropertyChanged("CompanyList");
            }
        }
        public ObservableCollection<RarTurnoverData> TurnoverDataList
        {
            get
            {
                return _RarFile.TurnoverDataList;
            }

            set
            {
                _RarFile.TurnoverDataList = value;
                OnPropertyChanged("TurnoverDataList");

            }
        }
        #endregion

        #region - Constructor -
        public RarViewModel()
        {
            _RarFile = new RarFormF6();
            //CompanyList = new List<RarCompany>();
            //OurCompany = new RarOurCompany();
            //TurnoverDataList = new List<RarTurnoverData>();

            //CompanyList.Add(new RarCompany("Первый"));
            //CompanyList.Add(new RarCompany("Второй"));
            //CompanyList.Add(new RarCompany("Третий"));
        }
        #endregion

        #region - Commands -
        private RelayCommand _openFileCommand;

        public ICommand OpenFileCommand
        {
            get
            {
                if (_openFileCommand == null)
                {
                    _openFileCommand = new RelayCommand(param => OpenFile(), param => CanOpenFile());
                }
                return _openFileCommand;
            }
        }
        public bool CanOpenFile()
        {
            return true;
        }
        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _RarFile.LoadF6(openFileDialog.FileName);
                UpdateAll();
            }
        } 
        #endregion

        private void UpdateAll()
        {
            OnPropertyChanged("DocumentDate");
            OnPropertyChanged("Version");
            OnPropertyChanged("ProgramName");
            OnPropertyChanged("FormNumber");
            OnPropertyChanged("ReportPeriod");
            OnPropertyChanged("YearReport");
            OnPropertyChanged("CorrectionNumber");
            OnPropertyChanged("OurCompany");
            OnPropertyChanged("CompanyList");

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
