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
        private ListCollectionView turnoverDataListCollectionView;
        private RarCompany selectedBuyer;
        private ObservableCollection<RarCompany> savingCompaniesList;
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
      
        public ListCollectionView TurnoverDataListCollectionView
        {
            get
            {
                return turnoverDataListCollectionView;
            }

            set
            {
                turnoverDataListCollectionView = value;
                OnPropertyChanged("TurnoverDataListCollectionView");

            }
        }

        public ObservableCollection<RarCompany> SavingCompaniesList
        {
            get
            {
                return savingCompaniesList;
            }

            set
            {
                savingCompaniesList = value;
                OnPropertyChanged("SavingCompaniesList");

            }
        }

        public RarCompany SelectedBuyer
        {
            get
            {
                return selectedBuyer;
            }

            set
            {
                selectedBuyer = value;
                OnPropertyChanged("SelectedBuyer");

            }
        }
        #endregion

        #region - Constructor -
        public RarViewModel()
        {
            _RarFile = new RarFormF6();
            AlcoCodesList = new ObservableCollection<string>(ParserF6.GetAlcoCodesListFromXSD());
            TurnoverDataList = new ObservableCollection<RarTurnoverData>(_RarFile.TurnoverDataList);
            BuyersList = new ObservableCollection<RarCompany>(_RarFile.BuyersList);
            ManufacturersList = new ObservableCollection<RarCompany>(_RarFile.ManufacturersList);
            SavingCompaniesList = new ObservableCollection<RarCompany>();
        }
        #endregion

        private bool IsInnValid(string inn)
        {
            return true;
        }
        private bool IsKppValid(string inn)
        {
            return true;
        }

        public void InitializeCompaniesList()
        {
            List<RarCompany> companiesList = TurnoverDataList.Where(s => s.Buyer == SelectedBuyer).Select(p => p.Manufacturer).Distinct().ToList();
            foreach (RarCompany company in companiesList)
            {
                string Inn = company.INN.Trim();
                string Kpp = company.KPP.Trim();
                if ((Inn.Length != 10) || (Kpp.Length != 9)) continue;
                if (IsInnValid(Inn)&&IsKppValid(Kpp))
                {
                    company.CounryID = "643";
                    company.Adress.RegionId = "77";
                }


            }
            SavingCompaniesList = new ObservableCollection<RarCompany>(companiesList);

        }




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


        private int SortStringsAsNumbers(string s1, string s2)
        {
            double num1;
            double num2;
            if (Double.TryParse(s1, out num1))
            {
                if (Double.TryParse(s2, out num2))
                {
                    return num1.CompareTo(num2);
                }

            }
            return String.Compare(s1, s2);
        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {

                _RarFile.LoadF6(openFileDialog.FileName);
                TurnoverDataList = new ObservableCollection<RarTurnoverData>(_RarFile.TurnoverDataList);
                //_RarFile.BuyersList.Sort( (s1, s2) => String.Compare(s1.Name, s2.Name) );
                _RarFile.BuyersList.Sort((s1, s2)=>SortStringsAsNumbers(s1.ID, s2.ID));

                BuyersList = new ObservableCollection<RarCompany>(_RarFile.BuyersList);

                ManufacturersList = new ObservableCollection<RarCompany>(_RarFile.ManufacturersList);

                TurnoverDataListCollectionView = new ListCollectionView(TurnoverDataList);
                //TurnoverDataListCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Subdevision"));
                TurnoverDataListCollectionView.SortDescriptions.Add(new SortDescription("DocumentNumber", ListSortDirection.Ascending));
                TurnoverDataListCollectionView.Filter = Buyer_Filter;

                UpdateAll();
            }
        }

        private RelayCommand _saveCompaniesFileCommand;

        public ICommand SaveCompaniesFileCommand
        {
            get
            {
                if (_saveCompaniesFileCommand == null)
                {
                    _saveCompaniesFileCommand = new RelayCommand(param => SaveCompaniesFile(), param => CanSaveCompaniesFile());
                }
                return _saveCompaniesFileCommand;
            }
        }



        private void SaveCompaniesFile()
        {

            //List<RarCompany> CompaniesList = TurnoverDataList.Where(s => s.Buyer == SelectedBuyer).Select(p => p.Manufacturer).Distinct().ToList();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "Companies"; // Default file name
            saveFileDialog.DefaultExt = ".xml"; // Default file extension
            saveFileDialog.Filter = "Xml documents (.xml)|*.xml"; // Filter files by extension
            if (saveFileDialog.ShowDialog() == true)
                ParserF6.SaveCompanies(SavingCompaniesList.ToList(), saveFileDialog.FileName);
            
        }

        public bool CanSaveCompaniesFile()
        {
            return true;
        }

        private RelayCommand _saveTurnoverFileCommand;

        public ICommand SaveTurnoverFileCommand
        {
            get
            {
                if (_saveTurnoverFileCommand == null)
                {
                    _saveTurnoverFileCommand = new RelayCommand(param => SaveTurnoverFile(), param => CanSaveTurnoverFile());
                }
                return _saveTurnoverFileCommand;
            }
        }



        private void SaveTurnoverFile()
        {

            List<RarTurnoverData> TurnoverList = TurnoverDataList.Where(s => s.Buyer == SelectedBuyer).ToList();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "Form11"; // Default file name
            saveFileDialog.DefaultExt = ".xml"; // Default file extension
            saveFileDialog.Filter = "Xml documents (.xml)|*.xml"; // Filter files by extension
            if (saveFileDialog.ShowDialog() == true)
                ParserF6.SaveTurnoverData(TurnoverList, saveFileDialog.FileName);

        }

        public bool CanSaveTurnoverFile()
        {
            return true;
        }

#endregion


        private bool Buyer_Filter(object item)
        {
            if (SelectedBuyer == null) return true;
            RarTurnoverData dt = (RarTurnoverData)item;
            if (dt.Buyer == SelectedBuyer)
                return true;
            else return false;
        }
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
