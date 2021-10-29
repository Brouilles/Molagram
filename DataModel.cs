using Molagram.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Molagram
{
    public struct Species
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public double MolarMass { get; set; }
        public string URL { get; set; }
    };

    public class DataModel : ViewModelBase
    {
        private readonly string m_filePath = "ChemicalSpecies.ini";

        // CurrentChemicalSpecies
        private Species m_currentChemicalSpecies;
        public Species CurrentChemicalSpecies
        {
            get => m_currentChemicalSpecies;
            set
            {
                if (m_currentChemicalSpecies.Symbol != value.Symbol)
                {
                    m_currentChemicalSpecies = value;
                    OnPropertyChanged(nameof(CurrentChemicalSpecies));
                }
            }
        }

        // ChemicalSpecies
        private ObservableCollection<Species> m_chemicalSpecies = new ObservableCollection<Species>();
        public ReadOnlyObservableCollection<Species> ChemicalSpecies
        {
            get;
        }

        // Units
        private ObservableCollection<string> m_units = new ObservableCollection<string>();
        public ReadOnlyObservableCollection<string> Units
        {
            get;
        }
        
        // CurrentUnit
        private string m_currentUnit;
        public string CurrentUnit
        {
            get => m_currentUnit;
            set
            {
                if (m_currentUnit != value)
                {
                    m_currentUnit = value;
                    m_weight = m_currentChemicalSpecies.MolarMass * m_mole; // Gram
                    m_weight = ConvertGramsToUnits(m_weight);
                    OnPropertyChanged(nameof(CurrentUnit));
                    OnPropertyChanged(nameof(Weight));
                }
            }
        }

        // Mole
        private double m_mole;
        public double Mole
        {
            get => m_mole;
            set
            {
                if (m_mole != value)
                {
                    m_mole = value;
                    m_weight = m_currentChemicalSpecies.MolarMass * m_mole; // Gram
                    m_weight = ConvertGramsToUnits(m_weight);
                    OnPropertyChanged(nameof(Mole));
                    OnPropertyChanged(nameof(Weight));
                }
            }
        }

        // Weight
        private double m_weight;
        public double Weight
        {
            get => m_weight;
            set
            {
                if (m_weight != value)
                {
                    m_weight = value;
                    var tempWeight = ConvertToGrams(m_weight);
                    m_mole = tempWeight / m_currentChemicalSpecies.MolarMass; // Gram
                    OnPropertyChanged(nameof(Weight));
                    OnPropertyChanged(nameof(Mole));
                }
            }
        }

        // Constructor
        public DataModel()
        {
            ChemicalSpecies = new ReadOnlyObservableCollection<Species>(m_chemicalSpecies);
            Units = new ReadOnlyObservableCollection<string>(m_units);
            ReadChemicalSpecies();
        }

        private async void ReadChemicalSpecies()
        {
            m_mole = 0.0;
            m_weight = 0.0;

            // Units
            m_units.Add("g");
            m_units.Add("kg");
            m_units.Add("oz");
            m_units.Add("lb");

            m_currentUnit = m_units[0];

            // Load species from file
            try
            {
                var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var file = await folder.GetFileAsync(m_filePath);
                var readFile = await Windows.Storage.FileIO.ReadLinesAsync(file);
                foreach (var line in readFile)
                {
                    string[] lineParams = line.Split(' ');
                    m_chemicalSpecies.Add(new Species
                    {
                        Name = lineParams[0],
                        Symbol = lineParams[1],
                        MolarMass = double.Parse(lineParams[2]),
                        URL = lineParams[3]
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Error",
                    Content = "Can't load the 'ChemicalSpecies.ini' file.",
                    CloseButtonText = "Ok"
                };
                await dialog.ShowAsync();
            }
        }

        // Methods
        private double ConvertGramsToUnits(double gram)
        {
            switch (m_currentUnit)
            {
                case "kg":
                    return gram / 1000;
                case "oz":
                    return gram / 28.3495231;
                case "lb":
                    return gram / 453.59237;
                default: // g
                    return gram;
            }
        }

        private double ConvertToGrams(double value)
        {
            switch (m_currentUnit)
            {
                case "kg":
                    return value * 1000;
                case "oz":
                    return value * 28.3495231;
                case "lb":
                    return value * 453.59237;
                default: // g
                    return value;
            }
        }
    }
}
