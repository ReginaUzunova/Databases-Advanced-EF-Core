using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.Dtos.Import
{
    [XmlType("Car")]
    public class ImportCarDto
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("TraveledDistance")]
        public long TraveledDistance { get; set; }

        [XmlElement("parts")]
        public ImportCarPartDto[] CarParts { get; set; }
    }

    [XmlType("partId")]
    public class ImportCarPartDto
    {
        [XmlElement("id")]
        public int PartId { get; set; }
    }
}
