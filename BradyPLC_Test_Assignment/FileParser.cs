using System;
using System.Collections;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

namespace BradyPLC_Test_Assignment
{
    class FileParser
    {
        private XElement xelement;
        
        // Constructor
        public FileParser(string aPath)
        {
            xelement = XElement.Load(aPath);
        }
        
        // Method to Get Element having Multiple Nodes from XML
        public ArrayList GetElement(string energyType, string key)
        {
            var response = from res in xelement.Elements(energyType).Descendants(key)
                           select res;
            ArrayList result = new ArrayList();
            foreach (XElement xEle in response)
            {
                result.Add(xEle.Value);
            }
            return result;
        }

        // Method to Get Element having Single Node from XML
        public string GetSingleElement(string energyType, string key)
        {
            string response = xelement.Elements(energyType).Descendants(key).First().Value;
            return response;
        }

        // Method to Get Wind Element from XML
        public ArrayList GetWindElement(string shoreMode, string key)
        {
            var windGenerator = from energySource in xelement.Descendants("WindGenerator")
                                where energySource.Element("Name").Value == shoreMode
                                select energySource;
            ArrayList result = new ArrayList();
            foreach (XElement xEle in windGenerator.Descendants("Energy"))
            {
                result.Add(xEle.Value);
            }
            return result;
        }

        // Method to Get Emission or Value Factor from XML
        public decimal GetFactor(string factorType, string factorLevel)
        {
            string referenceXMLPath = ConfigurationManager.AppSettings["ReferenceData"];
            XElement referenceData = XElement.Load(referenceXMLPath);
            decimal valueTemp;
            var result = referenceData.Descendants(factorType).Elements(factorLevel).First().Value;
            valueTemp = Convert.ToDecimal(result);
            return valueTemp;
        }
    }
}
