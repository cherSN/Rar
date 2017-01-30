using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Rar
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public RarViewModel ViewModel { set; get; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new RarViewModel();

            DataContext = ViewModel;
            //CompanyList = new List<RarCompany>();
            //File = new RarFile();
 //           dataGridCompanies.ItemsSource = CompanyList;
        }
        private void SetupFormData(XElement data)
        {

        }
        private void SetupOrganization(XElement organization)
        {
            ViewModel.OurCompany.Director.Name =            (string) organization.Element("ОтветЛицо").Element("Руководитель").Element("Фамилия");
            ViewModel.OurCompany.Director.Surname =         (string)organization.Element("ОтветЛицо").Element("Руководитель").Element("Имя");
            ViewModel.OurCompany.Director.Middlename =      (string)organization.Element("ОтветЛицо").Element("Руководитель").Element("Отчество");

            ViewModel.OurCompany.Accountant.Name =          (string)organization.Element("ОтветЛицо").Element("Главбух").Element("Фамилия");
            ViewModel.OurCompany.Accountant.Surname =       (string)organization.Element("ОтветЛицо").Element("Главбух").Element("Имя");
            ViewModel.OurCompany.Accountant.Middlename =    (string)organization.Element("ОтветЛицо").Element("Главбух").Element("Отчество");

            ViewModel.OurCompany.Name = (string)organization.Element("Реквизиты").Attribute("Наим");
            ViewModel.OurCompany.Phone = (string)organization.Element("Реквизиты").Attribute("ТелОрг");
            ViewModel.OurCompany.Email = (string)organization.Element("Реквизиты").Attribute("EmailОтпр");
            SetupAdress(ViewModel.OurCompany, organization.Element("Реквизиты").Element("АдрОрг"));
            XElement company = organization.Element("Реквизиты").Element("ЮЛ");
            if (company != null)
            {
                ViewModel.OurCompany.INN = (string)company.Attribute("ИННЮЛ");
                ViewModel.OurCompany.KPP = (string)company.Attribute("КППЮЛ");
            }
            else
            {
                XElement individual = organization.Element("Реквизиты").Element("ФЛ");
                if (individual != null)
                    ViewModel.OurCompany.INN = (string)individual.Attribute("ИННФЛ");
            }

            XElement lactivity = organization.Element("Деятельность").Element("Лицензируемая");
            if (lactivity!=null)
            {
                SetupLisences(ViewModel.OurCompany, lactivity);
            }
            else
            {
                ViewModel.OurCompany.UnLisenseActivity = (string)organization.Element("Деятельность").Element("Нелицензируемая").Attribute("ВидДеят");
            }


        }
        private void SetupPartners(XElement references)
        {
            foreach (XNode node in references.Elements("Контрагенты"))
            {
                XElement el = (XElement)node;
                RarCompany rc = new RarCompany();
                rc.Producter = false;
                rc.Name = el.Attribute("П000000000007").Value;
                rc.ID = el.Attribute("ИдКонтр").Value;

                XElement resident = el.Element("Резидент");
                if (resident != null)
                {
                    rc.CounryID = "643";
                    SetupLisences(rc, resident.Element("Лицензии"));
                    XElement adress = resident.Element("П000000000008");
                    SetupAdress(rc, adress);

                    XElement company = resident.Element("ЮЛ");
                    if (company != null)
                    {
                        rc.INN = (string) company.Attribute("П000000000009");
                        rc.KPP = (string) company.Attribute("П000000000010");
                    }
                    else
                    {
                        XElement individual = resident.Element("ФЛ");
                        if (individual != null)
                            rc.INN = (string) individual.Attribute("П000000000009");
                    }
                }
                else
                {
                    XElement foreigner = el.Element("Иностр");
                    if (foreigner != null)
                    {
                        rc.CounryID = (string) foreigner.Attribute("П000000000081");
                        rc.INN = (string) foreigner.Attribute("Номер");
                        rc.Adress = new RarAdress((string) foreigner.Attribute("П000000000082"));
                    }
                }

                ViewModel.CompanyList.Add(rc);
            }

        }
        private void SetupProducters(XElement references)
        {
            foreach (XNode node in references.Elements("ПроизводителиИмпортеры"))
            {
                XElement el = (XElement)node;
                RarCompany rc = new RarCompany();
                rc.Producter = true;
                rc.ID =     (string)el.Attribute("ИдПроизвИмп");
                rc.Name =   (string)el.Attribute("П000000000004");
                rc.INN =    (string)el.Attribute("П000000000005");
                rc.KPP =    (string)el.Attribute("П000000000006");

                ViewModel.CompanyList.Add(rc);
            }


        }
        private void SetupLisences(RarCompany rc, XElement lisenses)
        {
            foreach (XNode node in lisenses.Elements("Лицензия"))
            {
                RarLicense license = new RarLicense();
                XElement el = (XElement)node;
                license.ID = el.Attribute("ИдЛицензии").Value;
                license.SeriesNumber= el.Attribute("П000000000011").Value;
                license.DateFrom = DateTime.Parse(el.Attribute("П000000000012").Value);
                license.DateTo= DateTime.Parse(el.Attribute("П000000000013").Value);
                license.Issuer= el.Attribute("П000000000014").Value;
                rc.LicensesList.Add(license);

            }
        }
        private void SetupAdress(RarCompany rc, XElement adress)
        {
            RarAdress adr = new RarAdress();
            adr.StrictAdress = true;
            adr.CountryId = "643";
            adr.PostCode =  (string) adress.Element("Индекс");
            adr.RegionId=   (string) adress.Element("КодРегион");
            adr.District=   (string) adress.Element("Район");
            adr.City=       (string) adress.Element("Город");
            adr.Locality=   (string) adress.Element("НаселПункт");
            adr.Street=     (string) adress.Element("Улица");
            adr.Building=   (string) adress.Element("Дом");
            adr.Block=      (string) adress.Element("Корпус");
            adr.Litera =    (string) adress.Element("Литера");
            adr.Apartment = (string) adress.Element("Кварт");
            adr.AdressString =
                adr.CountryId + "," +
                adr.PostCode + "," +
                adr.RegionId + "," +
                adr.District + "," +
                adr.City + "," +
                adr.Locality + "," +
                adr.Street + "," +
                adr.Building + "," +
                adr.Block + "," +
                adr.Litera + "," +
                adr.Apartment + ",";
            rc.Adress = adr;
        }
        private bool InDocumentIsValid(XDocument xdoc)
        {
            string xsdMarkup = Rar.Properties.Resources.strXSD;
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", XmlReader.Create(new StringReader(xsdMarkup)));
            bool errors = false;
            List<string> errNodes = new List<string>();
            xdoc.Validate(schemas, (o, ee) =>
            {
                string errNode = ee.Message; //  ((XElement)o).Name.ToString();
                errNodes.Add(errNode);
                errors = true;
            });
            if (errors)
            {
                string mess = "Не соответствует схеме: " + "\n";
                foreach (string item in errNodes) mess = mess + item + "\n";
                MessageBox.Show(mess);
                return false;
            }
            else return true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                XDocument xdoc = XDocument.Load(openFileDialog.FileName);
                if (!InDocumentIsValid(xdoc)) return;

                XElement root = xdoc.Root;
                ViewModel.ProgramName =     (string) root.Attribute("НаимПрог");
                ViewModel.Version=          (string) root.Attribute("ВерсФорм");
                ViewModel.DocumentDate  =  DateTime.Parse(root.Attribute("ДатаДок").Value);

                ViewModel.FormNumber =      (string) root.Element("ФормаОтч").Attribute("НомФорм");
                ViewModel.ReportPeriod =    (string) root.Element("ФормаОтч").Attribute("ПризПериодОтч");
                ViewModel.YearReport =      (string) root.Element("ФормаОтч").Attribute("ГодПериодОтч");

                XElement corrections = root.Element("ФормаОтч").Element("Корректирующая");
                if (corrections==null)
                    ViewModel.CorrectionNumber = 0;
                else
                    ViewModel.CorrectionNumber = (int)corrections.Attribute("НомерКорр");


                SetupOrganization(root.Element("Документ").Element("Организация"));

                XElement references = root.Element("Справочники");
                SetupPartners(references);
                SetupProducters(references);

            }

            IList<RarCompany> rac = ViewModel.CompanyList.Select(p => p).OrderBy(s=>s.Producter).ThenBy(s=> 
            {
                int r;
                try
                {
                    r = Int32.Parse(s.ID);
                }
                catch (Exception)
                {
                    return 0;
                }
                return r;
                }).ToList();
            //dataGridCompanies.ItemsSource = rac;
            comboBoxPartner.ItemsSource = ViewModel.CompanyList;

        }

    }
}
