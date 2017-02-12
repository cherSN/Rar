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
            string xsdMarkup = Rar.Model.Properties.Resources.xsd_F6_010117;
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
                //MessageBox.Show(mess);
                return false;
            }
            else return true;
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
            formF6.ReportPeriod = (int)root.Element("ФормаОтч").Attribute("ПризПериодОтч");
            formF6.YearReport = (int)root.Element("ФормаОтч").Attribute("ГодПериодОтч");

            XElement corrections = root.Element("ФормаОтч").Element("Корректирующая");
            if (corrections == null) formF6.CorrectionNumber = 0;
            else formF6.CorrectionNumber = (int)corrections.Attribute("НомерКорр");

            SetupOrganization(root.Element("Документ").Element("Организация"), formF6.OurCompany);

            XElement references = root.Element("Справочники");
            SetupBuyers(references, formF6);
            SetupManufacturers(references, formF6);
            SetupFormData(root.Element("Документ").Element("ОбъемОборота"), formF6);
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
                OurCompany.INN = (int)company.Attribute("ИННЮЛ");
                OurCompany.KPP = (int)company.Attribute("КППЮЛ");
            }
            else
            {
                XElement individual = organization.Element("Реквизиты").Element("ФЛ");
                if (individual != null)
                    OurCompany.INN = (int)individual.Attribute("ИННФЛ");
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
            adr.CountryId = 643; // ?????????????????????
            adr.PostCode = (int)adress.Element("Индекс");
            adr.RegionId = (int)adress.Element("КодРегион");
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
                    rc.CounryID = 643; //  ?????????????????????
                    SetupLisences(rc, resident.Element("Лицензии"));
                    XElement adress = resident.Element("П000000000008");
                    rc.Adress = SetupAdress(adress);

                    XElement company = resident.Element("ЮЛ");
                    if (company != null)
                    {
                        rc.INN = (int)company.Attribute("П000000000009");
                        rc.KPP = (int)company.Attribute("П000000000010");
                    }
                    else
                    {
                        XElement individual = resident.Element("ФЛ");
                        if (individual != null)
                            rc.INN = (int)individual.Attribute("П000000000009");
                    }
                }
                else
                {
                    XElement foreigner = el.Element("Иностр");
                    if (foreigner != null)
                    {
                        rc.CounryID = (int)foreigner.Attribute("П000000000081");
                        rc.INN = (int)foreigner.Attribute("Номер");
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
                rc.INN = (int)el.Attribute("П000000000005");
                rc.KPP = (int)el.Attribute("П000000000006");

                formF6.ManufacturersList.Add(rc);
            }


        }
        private static void SetupFormData(XElement turnoverdata, RarFormF6 formF6)
        {
            RarSubdevision subdevision = new RarSubdevision();
            subdevision.Name = (string)turnoverdata.Attribute("Наим");
            subdevision.KPP = (int)turnoverdata.Attribute("КППЮЛ");
            //subdevision.SalePresented = (bool)turnoverdata.Attribute("НаличиеПоставки");
            //subdevision.ReturnPresented = (bool)turnoverdata.Attribute("НаличиеВозврата");
            subdevision.Adress = SetupAdress(turnoverdata.Element("АдрОрг"));
            formF6.OurCompany.SubdevisionList.Add(subdevision);
            foreach (XNode node in turnoverdata.Elements("Оборот"))
            {
                XElement el = (XElement)node;
                RarTurnoverData data = new RarTurnoverData();
                data.Subdevision = subdevision;
                data.ProductionSortID = (int)el.Attribute("П000000000003");
                string producterID = (string)el.Element("СведПроизвИмпорт").Attribute("ИдПроизвИмп");
                data.Manufacturer = formF6.ManufacturersList.Where(p => p.ID == producterID).First();
                string buyerID = (string)el.Element("СведПроизвИмпорт").Element("Получатель").Attribute("ИдПолучателя");
                data.Buyer = formF6.BuyersList.Where(p => p.ID == producterID).First();
                string licenseID = (string)el.Element("СведПроизвИмпорт").Element("Получатель").Attribute("ИдЛицензии");

                RarLicense l = null;
                foreach (RarCompany item in formF6.BuyersList)
                {
                    l = item.LicensesList.Where(s => s.ID == licenseID).FirstOrDefault();
                    if (l != null) break;
                }

                if (l != null) data.License = l;

                //data.NotificationDate=      DateTime.Parse(el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000015").Value);
                //data.NotificationNumber =   (string)el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000016");
                //data.NotificationTurnover = (double)el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000017");
                data.DocumentDate = DateTime.Parse(el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000018").Value);
                data.DocumentNumber = (string)el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000019");
                //data.CustomsDeclarationNumber = (string)el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000020");
                data.Turnover = (double)el.Element("СведПроизвИмпорт").Element("Получатель").Element("Поставка").Attribute("П000000000021");
                formF6.TurnoverDataList.Add(data);
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
    }
}
