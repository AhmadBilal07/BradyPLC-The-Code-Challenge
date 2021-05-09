using System;
using System.Collections;

namespace BradyPLC_Test_Assignment
{
    // The class Contains the Business Logic of the Software
    class Calculator
    {
        // Calculates Total Energy Value For The Given Energy
        public decimal GetTotalEnergy(ArrayList energies, ArrayList prices, decimal factor)
        {
            decimal totalEnergy = 0;
            for (int i = 0; i < energies.Count; i++)
            {
                totalEnergy = totalEnergy + Convert.ToDecimal(energies[i]) * Convert.ToDecimal(prices[i]) * factor;
            }
            return totalEnergy;
        }
        
        // Calculates Highest Daily Emission Value For The Given Energy
        public ArrayList GetHighestDailyEmission(FileParser myObject, ArrayList energyGas, ArrayList energyCoal)
        {
            ArrayList HighestDailyEmission = new ArrayList();
            string energy, date;
            decimal emission;
            decimal ERG = Convert.ToDecimal(myObject.GetSingleElement("Gas", "EmissionsRating"));
            decimal emissionFactorGas = myObject.GetFactor("EmissionsFactor", "Medium");
            ArrayList GasDailyEmission = GetDailyEmission(energyGas, ERG, emissionFactorGas);
            decimal ERC = Convert.ToDecimal(myObject.GetSingleElement("Coal", "EmissionsRating"));
            decimal emissionFactorCoal = myObject.GetFactor("EmissionsFactor", "High");
            ArrayList CoalDailyEmission = GetDailyEmission(energyCoal, ERG, emissionFactorGas);
            for (int i = 0; i < energyGas.Count; i++)
            {
                if ((decimal)GasDailyEmission[i] > (decimal)CoalDailyEmission[i])
                {
                    energy = myObject.GetSingleElement("Gas", "Name");
                    date = (string)myObject.GetElement("Gas", "Date")[i];
                    emission = (decimal)GasDailyEmission[i];
                }
                else
                {
                    energy = myObject.GetSingleElement("Coal", "Name");
                    date = (string)myObject.GetElement("Coal", "Date")[i];
                    emission = (decimal)CoalDailyEmission[i];
                }
                HighestDailyEmission.Add(energy);
                HighestDailyEmission.Add(date);
                HighestDailyEmission.Add(emission);
            }
            return HighestDailyEmission;
        }

        // Calculates Daily Emission Value For The Given Energy
        public ArrayList GetDailyEmission(ArrayList energies, decimal emissionRating, decimal factor)
        {
            ArrayList dailyEmission = new ArrayList();
            for (int i = 0; i < energies.Count; i++)
            {
                var temp = Convert.ToDecimal(energies[i]) * emissionRating * factor;
                dailyEmission.Add(temp);
            }
            return dailyEmission;
        }

        // Calculates Actual Heat Input
        public Decimal getActualHeatInput(decimal TotalHeatInput, decimal ActualNetGeneration)
        {
            decimal actualHeatInput;
            actualHeatInput = TotalHeatInput * ActualNetGeneration;
            return actualHeatInput;
        }
    }

}