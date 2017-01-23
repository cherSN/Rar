﻿using Microsoft.Win32;
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
                XElement file = xdoc.Root;

                File.ProgramName = file.Attribute("НаимПрог").Value;
                File.Version= file.Attribute("ВерсФорм").Value;
                File.DocumentDate  =  DateTime.Parse(file.Attribute("ДатаДок").Value);

                XElement references = file.Element("Справочники");
                XElement partners = references.Element("Контрагенты");

                XNode partner = references.FirstNode;
                while (partner != null)
                {
                    //проверяем, что текущий узел - это элемент
                    if (partner.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        XElement el = (XElement)partner;
                        RarCompany rc = new RarCompany();

                        if (el.Name == "Контрагенты")
                        {
                            rc.Name = el.Attribute("П000000000007").Value;
                        }
                        else
                        {
                            rc.Name = el.Attribute("П000000000004").Value;
                            rc.Producter = true;
                        }

                        CompanyList.Add(rc);
                        //dataGridCompanies.Items.Add(rc);
                    }
                    partner = partner.NextNode;
                }
            }
            IList<RarCompany> rac = CompanyList.Select(p => p).ToList();
            // dataGridCompanies.ItemsSource = rac;
            foreach (RarCompany item in rac)
            {
                dataGridCompanies.Items.Add(item);

            }
        }
    }
}