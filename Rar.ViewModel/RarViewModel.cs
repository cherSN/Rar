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
        private ObservableCollection<string> alcoCodesList;
        private ObservableCollection<RarCompany> buyersList;
        private ObservableCollection<RarCompany> manufacturersList;
        private ObservableCollection<RarTurnoverData> turnoverDataList;
        private CollectionViewSource turnoverDataCollectionViewSource;
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
        public string CorrectionNumber
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

        public ObservableCollection<string> AlcoCodesList
        {
            get
            {
                return alcoCodesList;
            }

            set
            {
                alcoCodesList = value;
            }
        }
        public ObservableCollection<RarCompany> BuyersList
        {
            get
            {
                return buyersList;
            }

            set
            {
                buyersList = value;
                OnPropertyChanged("BuyersList");
            }
        }
        public ObservableCollection<RarCompany> ManufacturersList
        {
            get
            {
                return manufacturersList;
            }

            set
            {
                manufacturersList = value;
                OnPropertyChanged("ManufacturersList");

            }
        }
        public ObservableCollection<RarTurnoverData> TurnoverDataList
        {
            get
            {
                return turnoverDataList;
            }

            set
            {
                turnoverDataList = value;
                OnPropertyChanged("TurnoverDataList");

            }
        }
        public CollectionViewSource TurnoverDataCollectionViewSource
        {
            get
            {
                return turnoverDataCollectionViewSource;

                //CollectionViewSource cvs= new CollectionViewSource();
            }

            set
            {
                turnoverDataCollectionViewSource = value;
                //OnPropertyChanged("TurnoverDataListView");
            }
        }
        #endregion

        void viewSource_Filter(object sender, FilterEventArgs e)
        {
            //e.Accepted = ((RarTurnoverData)e.Item).IndexOf(filter.Text) >= 0;
            RarTurnoverData dt= (RarTurnoverData)e.Item;
            if (dt.AlcoCode == "200") e.Accepted = true;
            else e.Accepted = false;
        }
        #region - Constructor -
        public RarViewModel()
        {
            _RarFile = new RarFormF6();
            AlcoCodesList = new ObservableCollection<string>(ParserF6.GetAlcoCodesListFromXSD());
            TurnoverDataCollectionViewSource = new CollectionViewSource();
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
                //ParserF6.GetAlcoCodesListFromXSD();

                _RarFile.LoadF6(openFileDialog.FileName);
                TurnoverDataList = new ObservableCollection<RarTurnoverData>(_RarFile.TurnoverDataList);
                BuyersList = new ObservableCollection<RarCompany>(_RarFile.BuyersList);
                ManufacturersList = new ObservableCollection<RarCompany>(_RarFile.ManufacturersList);

                TurnoverDataCollectionViewSource.Source = _RarFile.TurnoverDataList;
                TurnoverDataCollectionViewSource.GroupDescriptions.Add(new PropertyGroupDescription("Subdevision"));
                TurnoverDataCollectionViewSource.SortDescriptions.Add(new SortDescription("AlcoCode", ListSortDirection.Ascending));
                //TurnoverDataCollectionViewSource.Filter += viewSource_Filter;

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
            //OnPropertyChanged("TurnoverDataList");
            //OnPropertyChanged("BuyersList");

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
