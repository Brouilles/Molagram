using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.Windows.ApplicationModel.Resources;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MolagramWindowsAppSDK
{
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString().Replace(".", ",");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value.ToString().Replace(".", ",");
        }
    }

    public class StringNullOrEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.IsNullOrEmpty(value as string)
                ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return string.IsNullOrEmpty(value as string)
                ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public DataModel ViewModel { get; set; }

        private ResourceLoader m_resourceLoader;

        public MainPage()
        {
            this.InitializeComponent();

            this.ViewModel = new DataModel();
            m_resourceLoader = new ResourceLoader();
        }

        // Events
        private async void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = m_resourceLoader.GetString("About/Label"),
                Content = "Molagram - Copyright © 2021 Dezeiraud Gaëtan (gaetan.dezeiraud.com), Chloé Vinour, Honorine Claudot. All rights reserved.",
                CloseButtonText = m_resourceLoader.GetString("Ok"),
            };

            dialog.XamlRoot = this.Content.XamlRoot;
            await dialog.ShowAsync();
        }

        private void ChemicalSpecies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null) return;

            var content = comboBox.SelectedValue as Species?;
            if (content != null)
            {
                this.ViewModel.CurrentChemicalSpecies = (Species)content;
                this.ViewModel.Mole = 1;
                this.ViewModel.Weight = this.ViewModel.CurrentChemicalSpecies.MolarMass;
            }
        }

        private void Units_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null) return;

            var content = comboBox.SelectedValue as string;
            if (content != null)
            {
                this.ViewModel.CurrentUnit = content;
            }
        }

        private void WikipediaButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.CurrentChemicalSpecies.URL != null)
            {
                string uriToLaunch = this.ViewModel.CurrentChemicalSpecies.URL;
                var uri = new Uri(uriToLaunch);

                _ = Windows.System.Launcher.LaunchUriAsync(uri);
            }
        }
    }
}
