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
using System.Windows.Data;


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
        public int ReportPeriod
        {
            get { return _RarFile.ReportPeriod; }
            set
            {
                _RarFile.ReportPeriod = value;
                OnPropertyChanged("ReportPeriod");
            }
        }
        public int YearReport
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
        public ObservableCollection<RarCompany> BuyersList { get { return new ObservableCollection<RarCompany>(_RarFile.BuyersList); } }
        public ObservableCollection<RarCompany> ManufacturersList { get { return new ObservableCollection<RarCompany>(_RarFile.ManufacturersList); } }
        public ObservableCollection<RarTurnoverData> TurnoverDataList { get { return new ObservableCollection<RarTurnoverData>(_RarFile.TurnoverDataList); } }
        public ICollectionView TurnoverDataListView {
            get
            {
                CollectionViewSource turnoverDataListViewSource = new CollectionViewSource();
                turnoverDataListViewSource.Source = _RarFile.TurnoverDataList;
                turnoverDataListViewSource.Filter += viewSource_Filter;
                return turnoverDataListViewSource.View;
            }
        }


        #endregion
        void viewSource_Filter(object sender, FilterEventArgs e)
        {
            //e.Accepted = ((RarTurnoverData)e.Item).IndexOf(filter.Text) >= 0;
            //return true;
        }
        #region - Constructor -
        public RarViewModel()
        {
            _RarFile = new RarFormF6();
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
            OnCollectionChanged();
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public void OnCollectionChanged()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
