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

namespace LinkeddinDominator.Pages.PagesScraper
{
    /// <summary>
    /// Interaction logic for UserControlsJobScraper.xaml
    /// </summary>
    public partial class UserControlsJobScraper : UserControl
    {
        public UserControlsJobScraper()
        {
            InitializeComponent();
        }

        private void btn_JobScraper_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread uploadAccountThread = new Thread(LoadJobUrls);
                uploadAccountThread.SetApartmentState(System.Threading.ApartmentState.STA);
                uploadAccountThread.IsBackground = true;

                uploadAccountThread.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void LoadJobUrls()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                GlobalsScraper.lstUrlJobScraper = GlobusFileHelper.ReadFiletoStringList(dlg.FileName);
            }
            catch (Exception ex)
            {
            }
        }

        private void btn_JobScraper_Start_Click(object sender, RoutedEventArgs e)
        {
            #region Settings
            try
            {
                if(!string.IsNullOrEmpty(txt_JobScraper_url_flePath.Text))
                {
                    GlobalsScraper.txtUrlJobScraper = txt_JobScraper_url_flePath.Text;
                }
                else
                {
                    GlobusLogHelper.log.Info("Please upload the Job_Url first.");
                    MessageBox.Show("Please upload the Job_Url first.");
                }

                if(!string.IsNullOrEmpty(txt_JobScraper_MinDelay.Text)&&!string.IsNullOrEmpty(txt_JobScraper_MaxDelay.Text))
                {
                    GlobalsScraper.txtMinDelay = Convert.ToInt32(txt_JobScraper_MinDelay.Text);
                    GlobalsScraper.txtMaxDelay = Convert.ToInt32(txt_JobScraper_MaxDelay.Text);
                }
                else
                {
                    GlobusLogHelper.log.Info("Delay field can't be empty.");
                    MessageBox.Show("Delay field can't be empty.");
                    return;
                }

                if(!string.IsNullOrEmpty(txt_Limit_for_job_scraper.Text))
                {
                    GlobalsScraper.txtLimitToScrapeJobs = Convert.ToInt32(txt_Limit_for_job_scraper.Text);
                }
                else
                {
                    GlobusLogHelper.log.Info("Limit field can't be empty.");
                    MessageBox.Show("Limit field can't be empty.");
                }
            }
            catch (Exception ex)
            {
            }
            #endregion

            try
            {
                JobScraper objJobScraper = new JobScraper();
                Thread thrThreadStartJobScraper = new Thread(objJobScraper.ThreadStartJobScraper);
                thrThreadStartJobScraper.Start();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
