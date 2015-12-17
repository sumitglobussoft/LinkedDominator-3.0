using Accounts;
using BaseLib;
using FirstFloor.ModernUI.Windows.Controls;
using Globussoft;
using LinkDominator;
using LinkeddinDominator.CustomUserControls;
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
    /// Interaction logic for UserControlsLinkedinScraper.xaml
    /// </summary>
    public partial class UserControlsLinkedinScraper : UserControl
    {
        public UserControlsLinkedinScraper()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LDGlobals.listAccounts.Count>0)
                {
                    var window = new ModernDialog
                            {
                                Content = new UserControlLISInputs()
                            };
                    window.MinWidth = 550;
                    window.MinHeight = 350;
                    // window.Title = "Upload Follow Details";               
                    window.ShowDialog(); 
                }
                else
                {
                    MessageBox.Show("Please upload the Accounts.");
                }
            }
            catch (Exception ex)
            {
              //  GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            } 
        }

        private void chkSearchByUrl_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LDGlobals.listAccounts.Count>0)
                {
                    var window = new ModernDialog
                            {
                                Content = new UserControlLISSearchByUrl()
                            };
                    window.MinWidth = 550;
                    window.MinHeight = 350;
                    // window.Title = "Upload Follow Details";               
                    window.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Please upload the Accounts.");
                }
            }
            catch (Exception ex)
            {
                //  GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            } 
        }

        private void chk_Scraper_LIScraper_Keyword_Title_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                    var window = new ModernDialog
                            {
                                Content = new UserControlKeyword_Title()
                            };
                    window.MinWidth = 550;
                    window.MinHeight = 350;
                    // window.Title = "Upload Follow Details";               
                    window.ShowDialog();
            }
            catch (Exception ex)
            {
                //  GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            } 
        }

        private void chk_LIScraper_Industry_Relationship_Language_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                    var window = new ModernDialog
                            {
                                Content = new UserControl_LIScraper_IndustryLanRelationship()
                            };
                    window.MinWidth = 550;
                    window.MinHeight = 350;
                    // window.Title = "Upload Follow Details";               
                    window.ShowDialog();                 
            }
            catch (Exception ex)
            {
                //  GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            } 
        }

        private void chk_LIScraper_PremiumOption_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                    var window = new ModernDialog
                            {
                                Content = new UserControlLIScraperPremiumOption()
                            };
                    window.MinWidth = 550;
                    window.MinHeight = 350;
                    // window.Title = "Upload Follow Details";               
                    window.ShowDialog();
            }
            catch (Exception ex)
            {
                //  GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            } 

        }

        private void chk_LIS_Groups_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LDGlobals.listAccounts.Count>0)
                {
                    var window = new ModernDialog
                            {
                                Content = new UserControl_LIS_Groups()
                            };
                    window.MinWidth = 550;
                    window.MinHeight = 350;
                    // window.Title = "Upload Follow Details";               
                    window.ShowDialog(); 
                }
                else
                {
                    MessageBox.Show("Please upload the Accounts.");
                }
            }
            catch (Exception ex)
            {
                //  GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        

        private void btn_LinkedinScraper_Start_Click(object sender, RoutedEventArgs e)
        {
            #region Settings
            try            
            {
                if (chk_LinkedinScraper_ScraperInputs.IsChecked == true || chk_LIScraper_PremiumOption.IsChecked == true || chk_Scraper_LIScraper_Keyword_Title.IsChecked == true || chk_LinkedinScraper_Groups.IsChecked == true || chk_LIScraper_Industry_Relationship_Language.IsChecked == true || chk_LinkedinScraper_SearchByUrl.IsChecked == true)
                {
                    if (chk_LinkedinScraper_ScraperInputs.IsChecked == true)
                        GlobalsScraper.chkScraperInput = true;
                    if (chk_LIScraper_PremiumOption.IsChecked == true)
                        GlobalsScraper.chkPremiumOptions = true;
                     if (chk_Scraper_LIScraper_Keyword_Title.IsChecked == true)
                        GlobalsScraper.chkKeyword_Title = true;
                    if (chk_LinkedinScraper_Groups.IsChecked == true)
                        GlobalsScraper.chkGroups = true;
                    if (chk_LIScraper_Industry_Relationship_Language.IsChecked == true)
                        GlobalsScraper.chkIndustry_Relationship_Language = true;
                    if (chk_LinkedinScraper_SearchByUrl.IsChecked == true)
                        GlobalsScraper.chkSearchByUrl = true;
                }
                else
                {
                    GlobusLogHelper.log.Info("Please check atleast one of the checkboxes.");
                    MessageBox.Show("Please check atleast one of the checkboxes.");
                    return;
                }

                if (string.IsNullOrEmpty(txt_LinkedinScraper_MinDelay.Text) && string.IsNullOrEmpty(txt_LinkedinScraper_MaxDelay.Text))
                {
                    GlobusLogHelper.log.Info("Delay field cann't be empty.");
                    return;
                }
                else
                {
                    GlobalsScraper.txtMinDelay = Convert.ToInt32(txt_LinkedinScraper_MinDelay.Text.ToString());
                    GlobalsScraper.txtMaxDelay = Convert.ToInt32(txt_LinkedinScraper_MaxDelay.Text.ToString());
                }
            }
            catch (Exception ex)
            {
            } 
            #endregion

            try
            {
                Scraper.LinkedInScraper objLinkedInScraper = new LinkedInScraper();
                Thread thrStartLinkedInScraper = new Thread(objLinkedInScraper.ThreadStartLinkedInScraper);
                thrStartLinkedInScraper.Start();
            }
            catch (Exception ex)
            {
            }
        }

        

        // aBove code will be same  for all modules
        // Action

        
       
      
       

      

      

       
    }
}
