using BaseLib;
using Globussoft;
using Scraper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LinkeddinDominator.CustomUserControls
{
    /// <summary>
    /// Interaction logic for UserControlLISSearchByUrl.xaml
    /// </summary>
    public partial class UserControlLISSearchByUrl : UserControl
    {
        public UserControlLISSearchByUrl()
        {
            InitializeComponent();
        }

        private void btn_LinkedInScraper_BrowseUrl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread uploadAccountThread = new Thread(LoadUrls);
                uploadAccountThread.SetApartmentState(System.Threading.ApartmentState.STA);
                uploadAccountThread.IsBackground = true;

                uploadAccountThread.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void LoadUrls()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                GlobalsScraper.lstUrlSearchByUrl = GlobusFileHelper.ReadFiletoStringList(dlg.FileName);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
