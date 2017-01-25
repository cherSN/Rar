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
        public List<RarCompany> CompanyList { set; get; }
        public RarFile File { set; get; }

        public MainWindow()
        {
            InitializeComponent();
            CompanyList = new List<RarCompany>();
            File = new RarFile();
 //           dataGridCompanies.ItemsSource = CompanyList;
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
                    SetupLisences(rc, resident);
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

                CompanyList.Add(rc);
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

                CompanyList.Add(rc);
            }


        }
        private void SetupLisences(RarCompany rc, XElement resident)
        {
            foreach (XNode node in resident.Element("Лицензии").Elements("Лицензия"))
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
                File.ProgramName = root.Attribute("НаимПрог").Value;
                File.Version= root.Attribute("ВерсФорм").Value;
                File.DocumentDate  =  DateTime.Parse(root.Attribute("ДатаДок").Value);

                XElement references = root.Element("Справочники");
                SetupPartners(references);
                SetupProducters(references);

            }
            IList<RarCompany> rac = CompanyList.Select(p => p).ToList();
            dataGridCompanies.ItemsSource = rac;

        }
    }
}
