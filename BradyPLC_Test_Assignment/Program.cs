using System;
using System.Collections;
using System.Configuration;
using System.IO;

namespace BradyPLC_Test_Assignment
{
    class Program
    {
        // Name of The Output File
        static string outputFileName = "GenerationOutput.xml";

        static void Main(string[] args)
        {
            string path = ConfigurationManager.AppSettings["InputPath"];
            Program program = new Program();
            program.MonitorDirectory(path);
            Console.ReadKey();
        }

        // This Method Monitors The Directory To See If There's Any File is Added
        private  void MonitorDirectory(string path)
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine("  == Watching Directory ==");
            Console.WriteLine("------------------------------");
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.Created += FileAdded;
            watcher.EnableRaisingEvents = true;
        }

        // This Method is Triggered When a New File is Added
        private  void FileAdded(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("        New File Added");
            Console.WriteLine("     " + e.Name);
            Console.WriteLine("==============================");
            ProcessXML(e.FullPath);
        }



        private  void ProcessXML(string path)
        {
            Console.WriteLine("     == Processing XML ==");
            Console.WriteLine("------------------------------");
            FileParser fileParser = new FileParser(path);
            Calculator calculator = new Calculator();
            ReportGenerator reportGenerator = new ReportGenerator();
            string outputPath = ConfigurationManager.AppSettings["OutputPath"];


            // Total Generation Value 
            TotalGenerationValue(fileParser, calculator, reportGenerator);
            Console.WriteLine("   Total Generation Value \u2713");
            Console.WriteLine("------------------------------");


            // Highest Daily Emission Calculator        
            HighestDailyEmission(fileParser, calculator, reportGenerator);
            Console.WriteLine("   Highest Daily Emission \u2713");
            Console.WriteLine("------------------------------");


            // Actual Heat Rate
            HeatRate(fileParser, calculator, reportGenerator);
            Console.WriteLine("     Actual Heat Rate \u2713");
            Console.WriteLine("------------------------------");

            // Finally Saving the File
            reportGenerator.SaveReport(outputPath + outputFileName);
        }

        private void HighestDailyEmission(FileParser fileParser, Calculator calculator, ReportGenerator reportGenerator)
        {
            // Highest Daily Emission
            ArrayList energyCoal = fileParser.GetElement("Coal", "Energy");
            ArrayList energyGas = fileParser.GetElement("Gas", "Energy");
            ArrayList HDE = calculator.GetHighestDailyEmission(fileParser, energyGas, energyCoal);
            for (int i = 0; i < HDE.Count; i+=3) {
                string energy = (string)HDE[i];
                string date = (string)HDE[i+1];
                decimal emission = (decimal)HDE[i + 2];
                reportGenerator.AddMaxEmissionGenerator(energy, date, emission);
            }
        }

        // This Method Calls GetTotalEnergy() Method for each energy from Calculator & Stores the result it in the Report
        private void TotalGenerationValue(FileParser fileParser, Calculator calculator, ReportGenerator reportGenerator)
        {
            // Coal Total Generation Value
            string energyName = fileParser.GetSingleElement("Coal", "Name");
            ArrayList energyCoal = fileParser.GetElement("Coal", "Energy");
            ArrayList priceCoal = fileParser.GetElement("Coal", "Energy");
            decimal valueFactor = fileParser.GetFactor("ValueFactor", "Medium");
            decimal totalEnergy = calculator.GetTotalEnergy(energyCoal, priceCoal, valueFactor);
            reportGenerator.AddGenerator(energyName, totalEnergy);

            // Gas Total Generation Value
            energyName = fileParser.GetSingleElement("Gas", "Name");
            ArrayList energyGas = fileParser.GetElement("Gas", "Energy");
            ArrayList priceGas = fileParser.GetElement("Gas", "Energy");
            valueFactor = fileParser.GetFactor("ValueFactor", "Medium");
            totalEnergy = calculator.GetTotalEnergy(energyGas, priceGas, valueFactor);
            reportGenerator.AddGenerator(energyName, totalEnergy);

            // Wind[OffShore] Total Generation Value
            energyName = fileParser.GetSingleElement("Wind", "Name");
            ArrayList energyWindOff = fileParser.GetWindElement("Wind[Offshore]", "Energy");
            ArrayList priceWindOff = fileParser.GetWindElement("Wind[Offshore]", "Price");
            valueFactor = fileParser.GetFactor("ValueFactor", "Low");
            totalEnergy = calculator.GetTotalEnergy(energyWindOff, priceWindOff, valueFactor);
            reportGenerator.AddGenerator(energyName, totalEnergy);

            // Wind[OnShore] Total Generation Value
            energyName = (string)fileParser.GetElement("Wind", "Name")[1];
            ArrayList energyWindOn = fileParser.GetWindElement("Wind[Onshore]", "Energy");
            ArrayList priceWindOn = fileParser.GetWindElement("Wind[Onshore]", "Price");
            valueFactor = fileParser.GetFactor("ValueFactor", "High");
            totalEnergy = calculator.GetTotalEnergy(energyWindOn, priceWindOn, valueFactor);
            reportGenerator.AddGenerator(energyName, totalEnergy);
        }

        // This Method Calls getActualHeatInput() Method from Calculator and Stores the result it in the Report
        private void HeatRate(FileParser fileParser, Calculator calculator, ReportGenerator reportGenerator)
        {
            // Actual Heat Rate
            decimal heatInput = Convert.ToDecimal(fileParser.GetSingleElement("Coal", "TotalHeatInput"));
            decimal netGeneration = Convert.ToDecimal(fileParser.GetSingleElement("Coal", "ActualNetGeneration"));
            string energy = fileParser.GetSingleElement("Coal", "Name");
            decimal actualHeatInput = calculator.getActualHeatInput(heatInput, netGeneration);
            reportGenerator.AddHeatRate(energy, actualHeatInput);          
        }


    }
}
