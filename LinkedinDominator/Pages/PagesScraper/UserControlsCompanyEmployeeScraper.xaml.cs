using BaseLib;
using Globussoft;
using linkedDominator;
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

namespace LinkeddinDominator.Pages.PagesScraper
{
    /// <summary>
    /// Interaction logic for UserControlsCompanyEmployeeScraper.xaml
    /// </summary>
    public partial class UserControlsCompanyEmployeeScraper : UserControl
    {
        Dictionary<string, string> CountryCode = new Dictionary<string, string>();
        public UserControlsCompanyEmployeeScraper()
        {
            InitializeComponent();
            bindMethod();
        }
        public void  bindMethod()
        {

            
                foreach(string item in LDGlobals.listAccounts)
            {
                string userName=item.Split(':')[0];
                cmb_CompanyEmployeeScraper_SelectAcc.Items.Add(userName);

            }
            MainWindow bojMainWindow = new MainWindow();

            ClsSelect ObjSelectMethod = new ClsSelect();
            CountryCode = ObjSelectMethod.getCountry();

            foreach (KeyValuePair<string, string> pair in CountryCode)
            {
                try
                {
                    cmb_CompanyEmployeeScraper_Country.Items.Add(pair.Value);
                }
                catch
                {
                }
            }

        }

        private void btn_CompanyEmployeeScraper_Start_Click(object sender, RoutedEventArgs e)
        {
            #region Settings
            try
            {
                GlobalsScraper.selectedEmailIdCompanyEmp = cmb_CompanyEmployeeScraper_SelectAcc.SelectedItem.ToString();
                GlobalsScraper.keywordCompanyEmp = txt_CompanyEmployeeScraper_Keyaword.Text;
                GlobalsScraper.selectedCountryCompanyEmp = cmb_CompanyEmployeeScraper_Country.SelectedItem.ToString();
                if(string.IsNullOrEmpty(txt_CompanyEmployeeScraper_MinDelay.Text)&&string.IsNullOrEmpty(txt_CompanyEmployeeScraper_MaxDelay.Text))
                {
                    GlobalsScraper.minDelayCompanyEmp = Convert.ToInt32(txt_CompanyEmployeeScraper_MinDelay.Text);
                    GlobalsScraper.maxDelayCompanyEmp = Convert.ToInt32(txt_CompanyEmployeeScraper_MaxDelay.Text);
                }
                else
                {
                    GlobusLogHelper.log.Info("Delay field cann't be empty.");
                    return;
                }

                CompanyEmployeeScraper objCompanyEmployeeScraper = new CompanyEmployeeScraper();
                Thread thrStartCompanyEmployeeScraper = new Thread(objCompanyEmployeeScraper.ThreadStartCompanyEmployeeScraper);
                thrStartCompanyEmployeeScraper.Start();
            }
            catch (Exception ex)
            {
            }
            #endregion

            try
            {
                CompanyEmployeeScraper objCompanyEmployeeScraper=new CompanyEmployeeScraper();
                Thread thrThreadStartCompanyEmployeeScraper = new Thread(objCompanyEmployeeScraper.ThreadStartCompanyEmployeeScraper);
                thrThreadStartCompanyEmployeeScraper.Start();
            }
            catch (Exception ex)
            {
            }
        }

        private void btn_CompanyEmployeeScraper_Url_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread uploadAccountThread = new Thread(LoadComnpanyUrls);
                uploadAccountThread.SetApartmentState(System.Threading.ApartmentState.STA);
                uploadAccountThread.IsBackground = true;

                uploadAccountThread.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void LoadComnpanyUrls()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                GlobalsScraper.lstUrlCompanyEmpScraper = GlobusFileHelper.ReadFiletoStringList(dlg.FileName);
                GlobusLogHelper.log.Info("Number of urls uploaded : " + GlobalsScraper.lstUrlCompanyEmpScraper.Count);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
