using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
namespace Rar.Model
{
    public class RarFormF6
    {
        public DateTime DocumentDate { set; get; }
        public string Version { set; get; }
        public string ProgramName { set; get; }
        public string FormNumber { set; get; }
        public string ReportPeriod { set; get; }
        public string YearReport { set; get; }
        public int CorrectionNumber { set; get; }

        public RarOurCompany OurCompany { set; get; }
        public List<RarCompany> CompanyList { set; get; }
        public List<RarTurnoverData> TurnoverDataList { set; get; }

        private void SetupFormData(XElement turnoverdata)
        {
            RarSubdevision subdevision = new RarSubdevision();
            subdevision.Name = (string)turnoverdata.Attribute("Наим");
            subdevision.KPP = (string)turnoverdata.Attribute("КППЮЛ");
            subdevision.SalePresented = (bool)turnoverdata.Attribute("НаличиеПоставки");
            subdevision.ReturnPresented = (bool)turnoverdata.Attribute("НаличиеВозврата");
            subdevision.Adress = SetupAdress(turnoverdata.Element("АдрОрг"));
            OurCompany.SubdevisionList.Add(subdevision);
            foreach (XNode node in turnoverdata.Elements("Оборот"))
            {
                XElement el = (XElement)node;
                RarTurnoverData data = new RarTurnoverData();
                data.Subdevision = subdevision;
                data.ProductionSortID = (string)el.Attribute("П000000000003");
                string producterID = (string)el.Element("СведПроизвИмпорт").Attribute("ИдПроизвИмп");
                data.Producter = CompanyList.Where(p => p.Producter && p.ID == producterID).First();
                string buyerID = (string)el.Element("СведПроизвИмпорт").Element("Получатель").Attribute("ИдПолучателя");
                data.Buyer = CompanyList.Where(p => !p.Producter && p.ID == producterID).First();
                string licenseID = (string)el.Element("СведПроизвИмпорт").Element("Получатель").Attribute("ИдЛицензии");

                List<RarCompany> buyers = CompanyList.Where(p => !p.Producter).ToList();
                RarLicense l = null;
                foreach (RarCompany item in buyers)
                {
                    l = item.LicensesList.Where(s => s.ID == licenseID).FirstOrDefault();
                    if (l != null) break;
                }

                if (l != null)
                    data.License = l;
                //List<RarLicense> lss = buyers.Select(p => p.LicensesList.Where(s => s.ID == licenseID);

                //data.License = buyers.Select(p => p.LicensesList.Where(s => s.ID == licenseID).First();

                //if (el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000015") != null)
                //    data.NotificationDate = DateTime.Parse(el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000015").Value);

                //data.NotificationDate=      DateTime.Parse(el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000015").Value);
                //data.NotificationNumber =   (string)el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000016");
                //data.NotificationTurnover = (double)el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000017");
                data.DocumentDate = DateTime.Parse(el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000018").Value);
                data.DocumentNumber = (string)el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000019");
                //data.CustomsDeclarationNumber = (string)el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000020");
                data.Turnover = (double)el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000021");
                TurnoverDataList.Add(data);
            }

        }
        private void SetupOrganization(XElement organization)
        {
            OurCompany.Director.Name = (string)organization.Element("ОтветЛицо").Element("Руководитель").Element("Фамилия");
            OurCompany.Director.Surname = (string)organization.Element("ОтветЛицо").Element("Руководитель").Element("Имя");
            OurCompany.Director.Middlename = (string)organization.Element("ОтветЛицо").Element("Руководитель").Element("Отчество");

            OurCompany.Accountant.Name = (string)organization.Element("ОтветЛицо").Element("Главбух").Element("Фамилия");
            OurCompany.Accountant.Surname = (string)organization.Element("ОтветЛицо").Element("Главбух").Element("Имя");
            OurCompany.Accountant.Middlename = (string)organization.Element("ОтветЛицо").Element("Главбух").Element("Отчество");

            OurCompany.Name = (string)organization.Element("Реквизиты").Attribute("Наим");
            OurCompany.Phone = (string)organization.Element("Реквизиты").Attribute("ТелОрг");
            OurCompany.Email = (string)organization.Element("Реквизиты").Attribute("EmailОтпр");
            OurCompany.Adress = SetupAdress(organization.Element("Реквизиты").Element("АдрОрг"));
            XElement company = organization.Element("Реквизиты").Element("ЮЛ");
            if (company != null)
            {
                OurCompany.INN = (string)company.Attribute("ИННЮЛ");
                OurCompany.KPP = (string)company.Attribute("КППЮЛ");
            }
            else
            {
                XElement individual = organization.Element("Реквизиты").Element("ФЛ");
                if (individual != null)
                    OurCompany.INN = (string)individual.Attribute("ИННФЛ");
            }

            XElement lactivity = organization.Element("Деятельность").Element("Лицензируемая");
            if (lactivity != null)
            {
                foreach (XNode node in lactivity.Elements("Лицензия"))
                {
                    RarLicense license = new RarLicense();
                    XElement el = (XElement)node;
                    //license.ID = "";
                    license.SeriesNumber = (string)el.Attribute("СерНомЛиц");
                    license.DateFrom = DateTime.Parse(el.Attribute("ДатаНачЛиц").Value);
                    license.DateTo = DateTime.Parse(el.Attribute("ДатаОконЛиц").Value);
                    license.BusinesType = (string)el.Attribute("ВидДеят");
                    OurCompany.LicensesList.Add(license);

                }
            }
            else
            {
                OurCompany.UnLisenseActivity = (string)organization.Element("Деятельность").Element("Нелицензируемая").Attribute("ВидДеят");
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
                    rc.Adress = SetupAdress(adress);

                    XElement company = resident.Element("ЮЛ");
                    if (company != null)
                    {
                        rc.INN = (string)company.Attribute("П000000000009");
                        rc.KPP = (string)company.Attribute("П000000000010");
                    }
                    else
                    {
                        XElement individual = resident.Element("ФЛ");
                        if (individual != null)
                            rc.INN = (string)individual.Attribute("П000000000009");
                    }
                }
                else
                {
                    XElement foreigner = el.Element("Иностр");
                    if (foreigner != null)
                    {
                        rc.CounryID = (string)foreigner.Attribute("П000000000081");
                        rc.INN = (string)foreigner.Attribute("Номер");
                        rc.Adress = new RarAdress((string)foreigner.Attribute("П000000000082"));
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
                rc.ID = (string)el.Attribute("ИдПроизвИмп");
                rc.Name = (string)el.Attribute("П000000000004");
                rc.INN = (string)el.Attribute("П000000000005");
                rc.KPP = (string)el.Attribute("П000000000006");

                CompanyList.Add(rc);
            }


        }
        private void SetupLisences(RarCompany rc, XElement lisenses)
        {
            foreach (XNode node in lisenses.Elements("Лицензия"))
            {
                RarLicense license = new RarLicense();
                XElement el = (XElement)node;
                license.ID = (string)el.Attribute("ИдЛицензии");
                license.SeriesNumber = (string)el.Attribute("П000000000011");
                license.DateFrom = DateTime.Parse(el.Attribute("П000000000012").Value);
                license.DateTo = DateTime.Parse(el.Attribute("П000000000013").Value);
                license.Issuer = (string)el.Attribute("П000000000014");
                rc.LicensesList.Add(license);

            }
        }
        private RarAdress SetupAdress(XElement adress)
        {
            RarAdress adr = new RarAdress();
            adr.StrictAdress = true;
            adr.CountryId = "643";
            adr.PostCode = (string)adress.Element("Индекс");
            adr.RegionId = (string)adress.Element("КодРегион");
            adr.District = (string)adress.Element("Район");
            adr.City = (string)adress.Element("Город");
            adr.Locality = (string)adress.Element("НаселПункт");
            adr.Street = (string)adress.Element("Улица");
            adr.Building = (string)adress.Element("Дом");
            adr.Block = (string)adress.Element("Корпус");
            adr.Litera = (string)adress.Element("Литера");
            adr.Apartment = (string)adress.Element("Кварт");
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
            return adr;
        }
        //private bool InDocumentIsValid(XDocument xdoc)
        //{
        //    string xsdMarkup = Rar.Properties.Resources.strXSD;
        //    XmlSchemaSet schemas = new XmlSchemaSet();
        //    schemas.Add("", XmlReader.Create(new StringReader(xsdMarkup)));
        //    bool errors = false;
        //    List<string> errNodes = new List<string>();
        //    xdoc.Validate(schemas, (o, ee) =>
        //    {
        //        string errNode = ee.Message; //  ((XElement)o).Name.ToString();
        //        errNodes.Add(errNode);
        //        errors = true;
        //    });
        //    if (errors)
        //    {
        //        string mess = "Не соответствует схеме: " + "\n";
        //        foreach (string item in errNodes) mess = mess + item + "\n";
        //        MessageBox.Show(mess);
        //        return false;
        //    }
        //    else return true;
        //}

        public void LoadF6(string filename) { 
                XDocument xdoc = XDocument.Load(filename);
                //if (!InDocumentIsValid(xdoc)) return;

                XElement root = xdoc.Root;
                ProgramName = (string)root.Attribute("НаимПрог");
                Version = (string)root.Attribute("ВерсФорм");
                DocumentDate = DateTime.Parse(root.Attribute("ДатаДок").Value);

                FormNumber = (string)root.Element("ФормаОтч").Attribute("НомФорм");
                ReportPeriod = (string)root.Element("ФормаОтч").Attribute("ПризПериодОтч");
                YearReport = (string)root.Element("ФормаОтч").Attribute("ГодПериодОтч");

                XElement corrections = root.Element("ФормаОтч").Element("Корректирующая");
                if (corrections == null)
                    CorrectionNumber = 0;
                else
                    CorrectionNumber = (int)corrections.Attribute("НомерКорр");


                SetupOrganization(root.Element("Документ").Element("Организация"));

                XElement references = root.Element("Справочники");
                SetupPartners(references);
                SetupProducters(references);
                SetupFormData(root.Element("Документ").Element("ОбъемОборота"));

            }

    }

}
