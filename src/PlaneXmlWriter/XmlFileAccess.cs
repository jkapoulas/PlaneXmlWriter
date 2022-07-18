using CommonLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace PlaneXmlWriter
{
    public class XmlFileAccess : ObservableObject
    {

        static NumberFormatInfo _nfi;
        static XmlFileAccess()
        {
            _nfi = new NumberFormatInfo();
            _nfi.NumberDecimalSeparator = ".";
            _nfi.NumberGroupSeparator = ",";

            DefaultXml = XDocument.Load("default.xml");
        }

        public static XDocument Xml { get; private set; }
        public static XDocument DefaultXml { get; private set; }

        public string FilePath { get => getter<string>(); private set => setter(value); }

        public ObservableCollection<XmlItem> Rows { get; set; } = new ObservableCollection<XmlItem>();

        public void Load(string path)
        {
            FilePath = path;
            Xml = XDocument.Load(path);

            var configXml = XDocument.Load("XmlDefinition.xml");

            var rows = from item in configXml.Root.Elements("Row")
                         select new XmlItem
                         {
                             Name = (string)item.Attribute("Name"),
                             ScienSym = (string)item.Attribute("ScienSym"),
                             Description = (string)item.Attribute("Description"),
                             Unit = (string)item.Attribute("Unit"),
                             XPath = (string)item.Attribute("XPath"),
                             DataType = getDataTypeFromString((string)item.Attribute("DataType")),
                         };

            Rows.Clear();
            foreach (var row in rows)
                Rows.Add(row);

            raisePropertyChanged();
        }

        private Type getDataTypeFromString(string str)
        {
            if(String.Equals(str, "Double", StringComparison.OrdinalIgnoreCase))
                return typeof(double);
            else if (String.Equals(str, "Decimal", StringComparison.OrdinalIgnoreCase))
                return typeof(double);
            else if (String.Equals(str, "Float", StringComparison.OrdinalIgnoreCase))
                return typeof(double);
            else if (String.Equals(str, "string", StringComparison.OrdinalIgnoreCase))
                return typeof(string);
            else if (String.Equals(str, "int", StringComparison.OrdinalIgnoreCase))
                return typeof(int);
            else
                return typeof(double);
        }

        public static object getAttr(string xPath, bool dflt, Type dataType)
        {
            if (Xml == null)
            {
                if (dataType == typeof(string))
                    return String.Empty;
                else if (dataType == typeof(Double) || dataType == typeof(Single) || dataType == typeof(Decimal))
                    return Convert.ChangeType(0, dataType, _nfi);
                return Convert.ChangeType(0, dataType);
            }
                

            XDocument xdoc = Xml;
            if (dflt)
                xdoc = DefaultXml;

            foreach (XAttribute attr in ((IEnumerable)
                xdoc.XPathEvaluate(xPath)).OfType<XAttribute>())
            {
                if (dataType == typeof(Double) || dataType == typeof(Single) || dataType == typeof(Decimal))
                    return Convert.ChangeType(attr.Value, dataType, _nfi);
                return Convert.ChangeType(attr.Value, dataType);
            }

            if (dataType == typeof(string))
                return String.Empty;
            else
                return 0;
        }
        public static void setAttr(string xPath, object value, bool dflt, Type dataType)
        {
            XDocument xdoc = Xml;
            if (dflt)
                xdoc = DefaultXml;

            if (xdoc == null)
                return;

            foreach (XAttribute attr in ((IEnumerable)
                xdoc.XPathEvaluate(xPath)).OfType<XAttribute>())
            {
                if (dataType == typeof(Double))
                    attr.Value = Convert.ToDouble(value).ToString(_nfi);
                else if (dataType == typeof(float))
                    attr.Value = Convert.ToSingle(value).ToString(_nfi);
                else if (dataType == typeof(Decimal))
                    attr.Value = Convert.ToDecimal(value).ToString(_nfi);
                else
                    attr.Value = value.ToString();
            }
        }

        internal async Task Save()
        {
            Xml.Save(FilePath);
        }

    }
}
