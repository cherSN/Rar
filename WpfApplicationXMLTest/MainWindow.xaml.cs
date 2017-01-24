using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Xml.Linq;

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


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                XDocument xdoc = XDocument.Load(openFileDialog.FileName);
                XElement root = xdoc.Root;

                File.ProgramName = root.Attribute("НаимПрог").Value;
                File.Version= root.Attribute("ВерсФорм").Value;
                File.DocumentDate  =  DateTime.Parse(root.Attribute("ДатаДок").Value);

                XElement references = root.Element("Справочники");
                foreach (XNode node in references.Elements("Контрагенты"))
                {
                    RarCompany rc = new RarCompany();
                    XElement el = (XElement)node;
                    rc.Name = el.Attribute("П000000000007").Value;
                    rc.ID = el.Attribute("ИдКонтр").Value;
                    XElement resident = el.Element("Резидент");
                    if (resident!=null)
                    {
                        XElement corporateBody = resident.Element("ЮЛ");
                        if (corporateBody != null)
                        {
                            rc.INN = corporateBody.Attribute("П000000000009").Value;
                            rc.KPP = corporateBody.Attribute("П000000000010").Value;
                        }
                    }
                }
            }
            IList<RarCompany> rac = CompanyList.Select(p => p).ToList();
            dataGridCompanies.ItemsSource = rac;

        }
    }
}
