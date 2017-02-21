using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Windows;

namespace Rar.Model
{
    public static class ParserF6
    {

        private static bool IsDocumentValid(XDocument xdoc)
        {
            Type type = Type.GetType("Rar.Model.ParserF6", false);
            using (Stream str = type.Assembly.GetManifestResourceStream("Rar.Model.Resources.f6_010117.xsd"))
            {
                using (XmlReader reader = new XmlTextReader(str))
                {
                    XmlSchemaSet schemas = new XmlSchemaSet();
                    schemas.Add("", reader);

                    bool errors = false;
                    List<string> errNodes = new List<string>();
                    xdoc.Validate(schemas, (o, ee) =>
                    {
                        string errNode = ee.Message;
                        errNodes.Add(errNode);
                        errors = true;
                    });
                    if (errors)
                    {
                        string mess = "Не соответствует схеме: " + "\n";
                        foreach (string item in errNodes) mess = mess + item + "\n";
                        return false;
                    }
                    else return true;
                }
            }
        }


        public static List<string> GetAlcoCodesListFromXSD()
        {
            List<string> listString = new List<string>();
            Type type = Type.GetType("Rar.Model.ParserF6", false);
            XDocument xdoc;
            using (Stream str = type.Assembly.GetManifestResourceStream("Rar.Model.Resources.f6_010117.xsd"))
            {
                using (XmlReader reader = new XmlTextReader(str))
                {
                    //string xsdForm6 = Rar.Model.Properties.Resources.xsd_F6_010117;
                    xdoc = XDocument.Load(reader);
                }
            }

            XElement el = xdoc.Descendants().Where(item => (item.Attribute("name") != null) && (item.Attribute("name").Value == "П000000000003")).FirstOrDefault();
            XElement restriction = el.Element("{http://www.w3.org/2001/XMLSchema}simpleType").Element("{http://www.w3.org/2001/XMLSchema}restriction");
            foreach (XNode node in restriction.Elements("{http://www.w3.org/2001/XMLSchema}enumeration"))
            {
                XElement elAlcoCode = node as XElement;
                string val = (elAlcoCode.Attribute("value")).Value;
                listString.Add(val);
            }
            return listString;

        }

        public static void Parse(string fileName, RarFormF6 formF6)
        {
            XDocument xdoc = XDocument.Load(fileName);
            if (!IsDocumentValid(xdoc)) throw new Exception("Не соответствует схеме");

            XElement root = xdoc.Root;
            formF6.ProgramName = (string)root.Attribute("НаимПрог");
            formF6.Version = (string)root.Attribute("ВерсФорм");
            formF6.DocumentDate = DateTime.Parse(root.Attribute("ДатаДок").Value);

            formF6.FormNumber = (string)root.Element("ФормаОтч").Attribute("НомФорм");
            formF6.ReportPeriod = (string)root.Element("ФормаОтч").Attribute("ПризПериодОтч");
            formF6.YearReport = (string)root.Element("ФормаОтч").Attribute("ГодПериодОтч");

            XElement corrections = root.Element("ФормаОтч").Element("Корректирующая");
            if (corrections == null) formF6.CorrectionNumber = "";
            else formF6.CorrectionNumber = (string)corrections.Attribute("НомерКорр");

            SetupOrganization(root.Element("Документ").Element("Организация"), formF6.OurCompany);

            XElement references = root.Element("Справочники");
            SetupBuyers(references, formF6);
            SetupManufacturers(references, formF6);
            SetupTurnoverData(root.Element("Документ").Element("ОбъемОборота"), formF6);
        }


        private static void SetupOrganization(XElement organization, RarOurCompany OurCompany)
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
                OurCompany.INN = (string)company.Attribute("ИННЮЛ"); // GetIntAttribute(company, "ИННЮЛ");
                OurCompany.KPP = (string)company.Attribute("КППЮЛ"); //GetIntAttribute(company, "КППЮЛ");
            }
            else
            {
                XElement individual = organization.Element("Реквизиты").Element("ФЛ");
                if (individual != null)
                    OurCompany.INN = (string)individual.Attribute("ИННФЛ"); //GetIntAttribute(individual,"ИННФЛ");
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
        private static RarAdress SetupAdress(XElement adress)
        {
            RarAdress adr = new RarAdress();
            adr.StrictAdress = true;
            adr.CountryId = "643"; // ?????????????????????
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
        private static void SetupBuyers(XElement references, RarFormF6 formF6)
        {
            foreach (XNode node in references.Elements("Контрагенты"))
            {
                XElement el = (XElement)node;
                RarCompany rc = new RarCompany();
                rc.Name = el.Attribute("П000000000007").Value;
                rc.ID = el.Attribute("ИдКонтр").Value;

                XElement resident = el.Element("Резидент");
                if (resident != null)
                {
                    rc.CounryID = "643"; //  ?????????????????????
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

                formF6.BuyersList.Add(rc);
            }

        }
        private static void SetupManufacturers(XElement references, RarFormF6 formF6)
        {
            foreach (XNode node in references.Elements("ПроизводителиИмпортеры"))
            {
                XElement el = (XElement)node;
                RarCompany rc = new RarCompany();
                rc.ID = (string)el.Attribute("ИдПроизвИмп");
                rc.Name = (string)el.Attribute("П000000000004");
                rc.INN = (string)el.Attribute("П000000000005");
                rc.KPP = (string)el.Attribute("П000000000006");
                

                formF6.ManufacturersList.Add(rc);
            }


        }
        private static void SetupTurnoverData(XElement turnoverdata, RarFormF6 formF6)
        {
            RarSubdevision subdevision = new RarSubdevision();
            subdevision.Name = (string)turnoverdata.Attribute("Наим");
            subdevision.KPP = (string)turnoverdata.Attribute("КППЮЛ");
            //subdevision.SalePresented = (bool)turnoverdata.Attribute("НаличиеПоставки");
            //subdevision.ReturnPresented = (bool)turnoverdata.Attribute("НаличиеВозврата");
            subdevision.Adress = SetupAdress(turnoverdata.Element("АдрОрг"));
            formF6.OurCompany.SubdevisionList.Add(subdevision);
            foreach (XNode nodeAlcoCode in turnoverdata.Elements("Оборот"))
            {
                XElement elAlcoCode = (XElement)nodeAlcoCode;
                foreach (XNode nodeManufacturer in elAlcoCode.Elements("СведПроизвИмпорт"))
                {
                    XElement elManufacturer = (XElement)nodeManufacturer;
                    string manufacturID = (string)elManufacturer.Attribute("ИдПроизвИмп");
                    RarCompany manufacturer = formF6.ManufacturersList.Where(p => p.ID == manufacturID).First();
                    foreach (XNode nodeBuyer in elManufacturer.Elements("Получатель"))
                    {
                        XElement elBuyer = (XElement)nodeBuyer;
                        string buyerID = (string)elBuyer.Attribute("ИдПолучателя");
                        RarCompany buyer = formF6.BuyersList.Where(p => p.ID == buyerID).First();
                        buyer.IsUsed = true;
                        string licenseID = (string)elBuyer.Attribute("ИдЛицензии");
                        RarLicense license = buyer.LicensesList.Where(s => s.ID == licenseID).FirstOrDefault();

                        foreach (XNode nodeDocument in elBuyer.Elements("Поставка"))
                        {
                            XElement elDocument = (XElement)nodeDocument;

                            RarTurnoverData data = new RarTurnoverData();
                            data.Subdevision = subdevision;
                            data.AlcoCode = (string)elAlcoCode.Attribute("П000000000003");
                            data.Manufacturer = manufacturer;
                            data.Buyer = buyer;
                            data.License = license;

                            //data.NotificationDate=      DateTime.Parse(el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000015").Value);
                            //data.NotificationNumber =   (string)el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000016");
                            //data.NotificationTurnover = (double)el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000017");
                            data.DocumentDate = DateTime.Parse(elDocument.Attribute("П000000000018").Value);
                            data.DocumentNumber = (string)elDocument.Attribute("П000000000019");
                            //data.CustomsDeclarationNumber = (string)el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000020");
                            data.Turnover = (double)elDocument.Attribute("П000000000021");
                            formF6.TurnoverDataList.Add(data);
                        }
                    }
                }

            }
        }
        private static void SetupLisences(RarCompany rc, XElement lisenses)
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

        private static XElement GetAdressElement(RarAdress adress)
        {
            XElement el = new XElement("П000000000008",
                new XElement("КодСтраны","643"),
                new XElement("Индекс", adress.PostCode),
                new XElement("КодРегион", adress.RegionId),
                new XElement("Район", adress.District),
                new XElement("Город", adress.City),
                new XElement("НаселПункт", adress.Locality),
                new XElement("Улица", adress.Street),
                new XElement("Дом", adress.Building),
                new XElement("Корпус", adress.Block),
                new XElement("Литера", adress.Litera),
                new XElement("Кварт", adress.Apartment)
                );

            return el;
        }

        private static XElement GetCompanyElement(RarCompany company)
        {
            if ((company.CounryID == null)||(company.CounryID =="643"))
            {
                XElement domestic = new XElement("Резидент",
                    GetAdressElement(company.Adress),
                    new XElement("ЮЛ",
                        new XAttribute("П000000000009", company.INN),
                        new XAttribute("П000000000010", company.KPP)),
                    new XElement("Производитель", new XAttribute("value", "True")),
                    new XElement("Перевозчик", new XAttribute("value", "False"))
                    );
            return domestic;
            }

            XElement foreigner = new XElement("Иностр",
             new XAttribute("П000000000081", company.CounryID));

            return foreigner;


        }
        public static void SaveCompanies(List<RarCompany> companyList, string filename)
        {
            int i = 1;
            XDocument xdoc = new XDocument(
                 new XDeclaration("1.0", "windows-1251", "yes"),
                 new XElement("Файл",
                 new XAttribute(XNamespace.Xmlns + "xs", "http://www.w3.org/2001/XMLSchema"),
                 new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                     companyList.Select(p => new XElement("Контрагенты",
                     new XAttribute("ИдКонтр", i++),
                     new XAttribute("П000000000007", p.Name),
                     GetCompanyElement(p))
                     )
                 )
             );
            xdoc.Save(filename);
        }

        public static void SaveTurnoverData(List<RarTurnoverData> turnoverList, string filename)
        {
            XDocument xdoc = new XDocument(
                new XDeclaration("1.0", "windows-1251", "yes"),
                new XElement("Справочники",
                new XAttribute("ДатаДок", DateTime.Now.ToShortDateString()),
                new XAttribute("ВерсФорм", "4.20"),
                new XAttribute("Декларант", "1C"),
                    new XElement("Документ")
                )
            );
            xdoc.Save(filename);
        }

    }
}
