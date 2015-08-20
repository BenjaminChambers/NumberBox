using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestNB
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ClickText(object sender, RoutedEventArgs e)
        {
            nBox.Number = double.Parse(tBox.Text);
        }

        private void Click1(object sender, RoutedEventArgs e)
        {
            nBox.Number = 1.0f;
        }

        private void Click505(object sender, RoutedEventArgs e)
        {
            nBox.Number = 505;
        }

        private void ClickPi(object sender, RoutedEventArgs e)
        {
            nBox.Number = 3.1415926;
        }

        private void Click31(object sender, RoutedEventArgs e)
        {
            nBox.Number = 3.1;
        }

        private void ClickGet(object sender, RoutedEventArgs e)
        {
            sValue.Text = nBox.Text;
            nValue.Text = nBox.Number.ToString();
        }
    }
}
