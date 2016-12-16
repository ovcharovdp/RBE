using CoreDM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Linq;

namespace FuelAPI.TTN
{
    public class SectionData
    {
        public SectionData()
        {
            ProductClass = 5;
            AllowExport = false;
            SectionNum = 0;
        }
        public bool AllowExport { get; set; }
        public SectionData(XmlNode s) : this()
        {
            XmlNode n = s["КодТоплива"];
            if (!n.HasChildNodes) { throw new Exception("Нет кода топлива"); }
            this.ProductCode = Convert.ToInt16(n.InnerText);

            n = s["Объем"];
            if (!n.HasChildNodes) { throw new Exception("Нет объема"); }
            this.Volume = (int)Convert.ToDouble(n.InnerText, CultureInfo.InvariantCulture);

            n = s["Плотность"];
            if (!n.HasChildNodes) { throw new Exception("Нет плотности"); }
            this.Density = Convert.ToDecimal(n.InnerText, CultureInfo.InvariantCulture);

            n = s["Масса"];
            if (!n.HasChildNodes) { throw new Exception("Нет массы"); }
            this.Weight = (int)Convert.ToDouble(n.InnerText, CultureInfo.InvariantCulture);

            n = s["Температура"];
            if (!n.HasChildNodes) { throw new Exception("Нет температуры"); }
            this.Temperature = Convert.ToDecimal(n.InnerText, CultureInfo.InvariantCulture);

            n = s["ПаспортКачестваНомер"];
            if (!n.HasChildNodes) { throw new Exception("Нет номера паспорта качества"); }
            this.PassNumber = n.InnerText;

            n = s["ПаспортКачестваДата"];
            if (!n.HasChildNodes) { throw new Exception("Нет даты паспорта качества"); }
            this.PassDate = Convert.ToDateTime(n.InnerText);

            n = s["НомерСекции"];
            if (n != null) this.SectionNum = Convert.ToByte(n.InnerText);

            n = s["ПлотностьПаспорта"];
            if (n != null) this.PassDensity = Convert.ToDecimal(n.InnerText, CultureInfo.InvariantCulture);
        }
        public byte SectionNum { get; set; }
        public Int16 ProductCode { get; set; }
        public byte ProductClass { get; set; }
        public Int32 Volume { get; set; }
        public Decimal Density { get; set; }
        public int Weight { get; set; }
        public Decimal Temperature { get; set; }
        public string PassNumber { get; set; }
        public DateTime PassDate { get; set; }
        public Decimal PassDensity { get; set; }
    }
    public class Document
    {
        public Document()
        { }
        public Document(XmlNode data)
        {
            XmlNode n = data["ДатаТТН"];
            if (!n.HasChildNodes) { throw new Exception("Нет даты ТТН"); }
            this.DocDate = Convert.ToDateTime(n.InnerText);

            n = data["НомерТТН"];
            if (!n.HasChildNodes) { throw new Exception("Нет номера ТТН"); }
            this.DocNumber = Convert.ToInt32(n.InnerText);

            n = data["Номер"];
            if (!n.HasChildNodes) { throw new Exception("Нет номера внешней системы"); }
            this.DocNumberExt = n.InnerText;

            n = data["НомерАЦ"];
            if (!n.HasChildNodes) { throw new Exception("Нет номера АЦ"); }
            this.RegNum = n.InnerText;

            n = data["ФИОВодителя"];
            if (!n.HasChildNodes) { throw new Exception("Нет ФИО водителя"); }
            this.Driver = n.InnerText;

            n = data["КодМестаХраненияГрузоотправителя"];
            if (!n.HasChildNodes) { throw new Exception("Нет кода места хранения"); }
            this.PlaceCode = n.InnerText;

            n = data["НаименованиеМестаХраненияГрузоотправителя"];
            if (!n.HasChildNodes) { throw new Exception("Нет кода места хранения"); }
            this.PlaceName = n.InnerText.ToUpper();

            // Console.WriteLine(String.Format("{0} от {1}, {2}({3}), {4}", num, d, regNum, fio, place));
            foreach (XmlNode s in data.SelectNodes("Секция"))
            {
                Sections.Add(new SectionData(s));
            }
        }
        // public FlStation Station { get; set; }
        public string StationID { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get { return !string.IsNullOrEmpty(ErrorMessage); } }
        public int DocNumber { get; set; }
        public string DocNumberExt { get; set; }
        public DateTime DocDate { get; set; }
        private string _regNum;
        public string RegNum
        {
            get { return _regNum; }
            set
            {
                _regNum = value.Replace(" ", "");
                // определяем основную часть номера
                Regex rgx = new Regex(@"[А-Я]\d{3}[А-Я]{2}");
                Match m = rgx.Match(_regNum);
                if (m.Success)
                {
                    // определяем регион
                    Regex rgxRegion = new Regex(@"\d{2,3}$");
                    Match mReg = rgxRegion.Match(_regNum);
                    _regNum = m.Value + ((mReg.Success) ? "-" + mReg.Value : "");
                }
                else
                {
                    _regNum = value;
                }
            }
        }
        public string Driver { get; set; }
        // Грузоотправитель
        public string PlaceCode { get; set; }
        public string PlaceName { get; set; }
        // Поставщик
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        List<SectionData> _sections = new List<SectionData>();
        public List<SectionData> Sections { get { return _sections; } }

        private void AddValue(XmlWriter stream, string nodeName, string nodeValue)
        {
            stream.WriteStartElement(nodeName);
            stream.WriteString(nodeValue);
            stream.WriteEndElement();
        }
        public void CreateDocument(string fileName)
        {
            if (!this._sections.Any(p => p.AllowExport))
                return;

            XmlWriter w = XmlWriter.Create(fileName);
            w.WriteStartDocument();
            w.WriteStartElement("TTN");
            w.WriteStartElement("Документы");
            w.WriteStartElement("Документ");
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                AddValue(w, "ErrorMessage", this.ErrorMessage);
            }
            AddValue(w, "ДатаТТН", this.DocDate.ToString("yyyy-MM-dd"));
            AddValue(w, "НомерТТН", this.DocNumber.ToString());
            AddValue(w, "КодФилиалаПолучателя", CustomerCode);
            AddValue(w, "НаименованиеФилиалаПолучателя", CustomerName);
            AddValue(w, "КодМестаХраненияГрузоотправителя", this.PlaceCode);
            AddValue(w, "НаименованиеМестаХраненияГрузоотправителя", this.PlaceName);
            AddValue(w, "Номер", this.DocNumberExt);
            AddValue(w, "НомерАЦ", this.RegNum);
            AddValue(w, "ФИОВодителя", this.Driver);
            AddValue(w, "АЗС", this.StationID);

            //    AddValue(w, "ДопИнф", "Не проводить!");

            foreach (var s in this._sections.Where(p => p.AllowExport))
            {
                w.WriteStartElement("Секция");
                w.WriteStartElement("КодТоплива");
                w.WriteValue(s.ProductCode);
                w.WriteEndElement();
                w.WriteStartElement("КлассТоплива");
                w.WriteValue(s.ProductClass);
                w.WriteEndElement();

                w.WriteStartElement("Объем");
                w.WriteValue(s.Volume);
                w.WriteEndElement();

                w.WriteStartElement("Плотность");
                w.WriteValue(s.Density);
                w.WriteEndElement();

                w.WriteStartElement("Масса");
                w.WriteValue(s.Weight);
                w.WriteEndElement();

                w.WriteStartElement("Температура");
                w.WriteValue(s.Temperature);
                w.WriteEndElement();

                w.WriteStartElement("ПаспортКачестваНомер");
                w.WriteValue(s.PassNumber);
                w.WriteEndElement();

                w.WriteStartElement("ПаспортКачестваДата");
                w.WriteValue(s.PassDate.ToString("yyyy-MM-dd"));
                w.WriteEndElement();

                if (s.PassDensity > 0)
                {
                    w.WriteStartElement("ТемператураПриведения");
                    w.WriteValue(15);
                    w.WriteEndElement();

                    w.WriteStartElement("ПлотностьПаспорта");
                    w.WriteValue(s.PassDensity);
                    w.WriteEndElement();
                }

                w.WriteEndElement();
            }

            w.WriteEndDocument();
            w.Close();
        }
    }
}
