using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BradyPLC_Test_Assignment
{
    class ReportGenerator
    {
        private XDocument xDoc;
        private StringWriter sw;

        //  Constructor: Initializes Report
        public ReportGenerator()
        {
            xDoc = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement("GenerationOutput",
                new XElement("Totals"), 
                new XElement("MaxEmissionGenerators"),
                new XElement("ActualHeatRates")
                ));
            sw = new StringWriter();
            xDoc.Save(sw);
            Console.WriteLine("     Report Initialized \u2713");
            Console.WriteLine("------------------------------");
        }

        // This Method Saves the Report
        public void SaveReport(string path)
        {
            xDoc.Save(path);
            Console.WriteLine("       Report Saved \u2713");
            Console.WriteLine("------------------------------");
        }

        // This Method Adds a New Generator Node In The Report
        public void AddGenerator(string eName, decimal eTotal)
        {
            XElement node = new XElement("Generator",
            new XElement("Name", eName),
            new XElement("Total", eTotal));
            xDoc.Descendants("Totals").FirstOrDefault().Add(node);
            xDoc.Save(sw);
        }

        // This Method Adds a New Max Emission Generators Node In The Report
        public void AddMaxEmissionGenerator(string eName, string eDate,decimal eEmission)
        {
            XElement node = new XElement("Day",
              new XElement("Name", eName),
              new XElement("Date", eDate),
              new XElement("Emission", eEmission));
            xDoc.Descendants("MaxEmissionGenerators").FirstOrDefault().Add(node);
            xDoc.Save(sw);
        }
        // This Method Adds a Heat Rate Node In The Report
        public void AddHeatRate(string eName, decimal eHeatRate)
        {
            XElement node1 = new XElement("Name", eName);
            XElement node2 = new XElement("Emission", eHeatRate);
            xDoc.Descendants("ActualHeatRates").FirstOrDefault().Add(node1);
            xDoc.Descendants("ActualHeatRates").FirstOrDefault().Add(node2);
            xDoc.Save(sw);
        }
    }
}
