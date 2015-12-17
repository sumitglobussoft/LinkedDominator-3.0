using BaseLib;
using Globussoft;
using linkedDominator;
using Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace LinkeddinDominator.Pages.PagesSearch
{
    /// <summary>
    /// Interaction logic for UserControlsLinkedinSearch.xaml
    /// </summary>
    public partial class UserControlsLinkedinSearch : UserControl
    {
        public UserControlsLinkedinSearch()
        {
            InitializeComponent();
            bindMethod();
        }
        public void bindMethod()
        {

            foreach (string item in LDGlobals.listAccounts)
            {
                string userName = item.Split(':')[0];
                cmb_LISearch_Acc.Items.Add(userName);

            }
            try
            {
                cmb_LISearch_Keyword.Items.Add("People");
                cmb_LISearch_Keyword.Items.Add("Companies");
                rdbtn_Search_LISearch_SearchByKeyword.IsChecked = true;
            }
            catch { }
        }
        Utils objUtils = new Utils();
        SimpleScraper objSimpleScraper = new SimpleScraper();


        private void btn_Search_LISearch_Start_Click(object sender, RoutedEventArgs e)
        
        {
           try
           {
               if (!string.IsNullOrEmpty(cmb_LISearch_Keyword.SelectedItem.ToString()))
               {
                   SimpleScraper._Search = cmb_LISearch_Keyword.SelectedItem.ToString();
               }
               if(!string.IsNullOrEmpty(txt_Search_LISearch_KeyWord.ToString()))
               {
                   SimpleScraper._Keyword = txt_Search_LISearch_KeyWord.Text;
               }

           }
            catch(Exception ex)
           {

           }
            
            try
            {
                if (LDGlobals.listAccounts.Count > 0)
                {

                    objSimpleScraper.isStopSimpleScraper = false;
                    objSimpleScraper.lstThreadsSimpleScraper.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();//Environment.ProcessorCount;

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                  

                    //if (!string.IsNullOrEmpty(txtScrapers_FanPageScraper_Threads.Text) && checkNo.IsMatch(txtScrapers_FanPageScraper_Threads.Text))
                    //{
                    //    threads = Convert.ToInt32(txtScrapers_FanPageScraper_Threads.Text);
                    //}

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    //PageManager.NoOfThreads = threads;

             
                    Thread extractFanPageIdsThread = new Thread(objSimpleScraper.StartSimpleScraper);
                    extractFanPageIdsThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                   
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btn_LISearch_Search_SearchByUrl_Click(object sender, RoutedEventArgs e)
        {
             Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {

                    List<string> templist = GlobusFileHelper.ReadFile(dlg.FileName);


                    foreach (string item in templist)
                    {
                        try
                        {
                            objSimpleScraper.lstUrlsSimpleScraper.Add(item);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                    }
                }
                GlobusLogHelper.log.Info("Urls Loaded : " + objSimpleScraper.lstUrlsSimpleScraper.Count);
           
        }

        private void rdbtn_Search_LISearch_SearchByKeyword_Checked(object sender, RoutedEventArgs e)
        {
            rdbtn_Search_LISearch_searchByUrl.Dispatcher.Invoke(new Action(delegate
                                       {
                                           rdbtn_Search_LISearch_searchByUrl.IsChecked = false;
                                           txt_Search_LISearch_SearchByUrlFilePath.IsEnabled = false;
                                           btn_LISearch_Search_SearchByUrl.IsEnabled = false;
                                           SimpleScraper.isSearchByKeyword = true;

                                           cmb_LISearch_Keyword.IsEnabled = true;
                                           txt_Search_LISearch_KeyWord.IsEnabled = true;
                                         

                                       }));

        }

        private void rdbtn_Search_LISearch_searchByUrl_Checked(object sender, RoutedEventArgs e)
        {
            rdbtn_Search_LISearch_SearchByKeyword.Dispatcher.Invoke(new Action(delegate
            {
                rdbtn_Search_LISearch_SearchByKeyword.IsChecked = false;
                cmb_LISearch_Keyword.IsEnabled = false;
                txt_Search_LISearch_KeyWord.IsEnabled = false;
                SimpleScraper.isSearchByKeyword = false;

                txt_Search_LISearch_SearchByUrlFilePath.IsEnabled = true;
                btn_LISearch_Search_SearchByUrl.IsEnabled = true;

            }));
        
        }

      
    }
}
