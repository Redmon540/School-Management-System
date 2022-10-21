using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace School_Manager
{
    [Serializable]
    public class VoucherTemplate
    {
        public BitmapImage FrontImage { get; set; }
        public BitmapImage BackImage { get; set; }
        public ObservableCollection<DesignerControls> FrontItems { get; set; }
        public ObservableCollection<DesignerControls> BackItems { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public bool IsBackAdded { get; set; }
    }

    public class XmlBitmapImage : IXmlSerializable
    {
        public XmlBitmapImage()
        {

        }
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}

