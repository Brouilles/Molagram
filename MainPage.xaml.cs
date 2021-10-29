using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Molagram
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

        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size(860, 350);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            this.ViewModel = new DataModel();
        }

        // Events
        private async void AboutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = "About",
                Content = "Molagram - Copyright © 2021 Dezeiraud Gaëtan (gaetan.dezeiraud.com), Chloé Vinour, Honorine Claudot. All rights reserved.",
                CloseButtonText = "Ok"
            };

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

        private void WikipediaButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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
