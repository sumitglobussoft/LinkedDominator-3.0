using Scraper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for UserControl_SalesNaviagtor_TitleAndKeyword.xaml
    /// </summary>
    public partial class UserControl_SalesNaviagtor_TitleAndKeyword : UserControl
    {
        public UserControl_SalesNaviagtor_TitleAndKeyword()
        {
            InitializeComponent();
            bindMethod();
        }
        public void bindMethod()
        {
            cmb_SalesNavigator_Current_Past.Items.Add("Current");
            cmb_SalesNavigator_Current_Past.Items.Add("Past");
            cmb_SalesNavigator_Current_Past.Items.Add("Current or Past");
            cmb_SalesNavigator_Current_Past.Items.Add("Past not current");
        }

        private void btn_IndustryRelationship_Save_Click(object sender, RoutedEventArgs e)
        {
            #region Ankit sir
            //try
            //{
            //    if (!string.IsNullOrEmpty(txtKeywordforLIScraper.Text))
            //    {
            //        GlobalsScraper.keyword_SalesNav = txtKeywordforLIScraper.Text;
            //    }
            //    if (!string.IsNullOrEmpty(txt_Title_SalesNav.Text))
            //    {
            //        GlobalsScraper.title_SalesNav = txt_Title_SalesNav.Text;
            //    }
            //    if (!string.IsNullOrEmpty(cmb_SalesNavigator_Current_Past.SelectedItem.ToString()))
            //    {
            //        GlobalsScraper.selectedCurrentPast_SalesNav = cmb_SalesNavigator_Current_Past.SelectedItem.ToString();
            //    }
            //}
            //catch (Exception ex)
            //{
            //}

            #endregion

            try
            {
                SalesNavigator.keyword = txtKeywordforLIScraper.Text;
                SalesNavigator.title = txt_Title_SalesNav.Text;
                if (cmb_SalesNavigator_Current_Past.SelectedItem != null)
                {
                    SalesNavigator.titleScope = cmb_SalesNavigator_Current_Past.SelectedItem.ToString();
                }
            }
            catch(Exception ex)
            { }

        }
    }
}
