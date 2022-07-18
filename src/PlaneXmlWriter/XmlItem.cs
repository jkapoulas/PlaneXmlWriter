using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneXmlWriter
{
    public class XmlItem : ObservableObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string XPath { get; set; }
        public string ScienSym { get; set; }
        public string Unit { get; set; }
        public Type DataType { get; set; }
        public object DefaultValue
        {
            get
            {
                return XmlFileAccess.getAttr(XPath, true, DataType);
            }
        }

        public object Value
        {
            get
            {
                return XmlFileAccess.getAttr(XPath, false, DataType);
            }
            set
            {
                XmlFileAccess.setAttr(XPath, value, false, DataType);
            }
        }
    }
}
