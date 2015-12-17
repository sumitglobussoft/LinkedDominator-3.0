using BaseLib;
using FirstFloor.ModernUI.Windows.Controls;
using LinkeddinDominator.CustomUserControls;
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
    /// Interaction logic for UserControlsSalsesNavigator.xaml
    /// </summary>
    public partial class UserControlsSalsesNavigator : UserControl
    {
        public UserControlsSalsesNavigator()
        {
            InitializeComponent();
        }

        private void chk_SalesNavigator_Inputs_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chk_SalesNavigator_Inputs.IsChecked==true)
                {
                    GlobalsScraper.chkLinkedInScraperInputSalesNav = true;
                    var window = new ModernDialog
                    {
                        Content = new UserControl_SalesNavigator_Input()
                    };
                    window.MinWidth = 550;
                    window.MinHeight = 350;
                    // window.Title = "Upload Follow Details";               
                    window.ShowDialog(); 
                }
                else
                {
                    GlobalsScraper.chkLinkedInScraperInputSalesNav = false;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void chk_SalesNavigator_PremiumOption_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chk_SalesNavigator_PremiumOption.IsChecked==true)
                {
                    GlobalsScraper.chkPremiumOptionsSalesNav = true;
                    var window = new ModernDialog
                            {
                                Content = new UserControl_SalesNaviagtor_PremiumOption()
                            };
                    window.MinWidth = 550;
                    window.MinHeight = 350;
                    // window.Title = "Upload Follow Details";               
                    window.ShowDialog(); 
                }
                else
                {
                    GlobalsScraper.chkPremiumOptionsSalesNav = false;
                }
            }
            catch (Exception ex)
            {
                //  GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            } 
        }

        private void chk_SalesNavigator_Industry_Relationship_Language_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new ModernDialog
                {
                    Content = new UserControl_SalesNaviagtor_IndustryLangRelationship()
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

        private void chk_SalesNavigator_Keyword_Title_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chk_SalesNavigator_Keyword_Title.IsChecked==true)
                {
                    GlobalsScraper.chkKeywordTitleSalesNav = true;
                    var window = new ModernDialog
                            {
                                Content = new UserControl_SalesNaviagtor_TitleAndKeyword()
                            };
                    window.MinWidth = 550;
                    window.MinHeight = 350;
                    // window.Title = "Upload Follow Details";               
                    window.ShowDialog(); 
                }
                else
                {
                    GlobalsScraper.chkKeywordTitleSalesNav = false;
                }
            }
            catch (Exception ex)
            {
                //  GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            } 
        }

        private void chk_SalesNavigator_Industry_Relationship_Language_Checked_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chk_SalesNavigator_Industry_Relationship_Language.IsChecked == true)
                {
                    GlobalsScraper.chkIndustry_Relationship_Language = true;
                    var window = new ModernDialog
                        {
                            Content = new UserControl_SalesNaviagtor_IndustryLangRelationship()
                        };
                    window.MinWidth = 550;
                    window.MinHeight = 350;
                    // window.Title = "Upload Follow Details";
                    window.ShowDialog();
                }
                else
                {
                    GlobalsScraper.chkIndustry_Relationship_Language = false;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void btn_SalesNavigator_Start_Click(object sender, RoutedEventArgs e)
        {            
            try
            {
                if(chk_SalesNavigator_Inputs.IsChecked==true||chk_SalesNavigator_PremiumOption.IsChecked==true||chk_SalesNavigator_Keyword_Title.IsChecked==true||chk_SalesNavigator_Industry_Relationship_Language.IsChecked==true)
                {
                    SalesNavigator objSalesNavigator=new SalesNavigator();
                    Thread thrThreadStartSalesNavigator = new Thread(objSalesNavigator.ThreadStartSalesNavigator);
                    thrThreadStartSalesNavigator.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please select one of the check boxes.");
                    MessageBox.Show("Please select one of the check boxes.");
                }
            }
            catch (Exception ex)
            {
            }            
        }

       
     
    }
}
