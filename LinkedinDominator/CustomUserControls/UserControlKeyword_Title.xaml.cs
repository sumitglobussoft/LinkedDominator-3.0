using Scraper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for UserControlKeyword_Title.xaml
    /// </summary>
    public partial class UserControlKeyword_Title : UserControl
    {

        string TitleValue = string.Empty;

        public UserControlKeyword_Title()
        {
            InitializeComponent();
            bindMethod();
        }



        public void bindMethod()
        {
            MainWindow objMainWindow = new MainWindow();
            TitleValue = objMainWindow.TitleValue;


            cmb_Current_Past.Items.Add("Current");
            cmb_Current_Past.Items.Add("Past");
            cmb_Current_Past.Items.Add("Current or past");
            cmb_Current_Past.Items.Add("Past not current");
            
        }

        private void btn_Keyword_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                LinkedInScraper.Keyword = txtKeywordforLIScraper.Text;
                LinkedInScraper.TitleValue = txtTitleforLIScraper.Text;
                if (cmb_Current_Past.SelectedItem !=null)
                {
                    LinkedInScraper.TitleScope = cmb_Current_Past.SelectedItem.ToString();
                }
                #region within Title value
                try
                {
                    string[] arrayTitleList = Regex.Split(TitleValue, ",");
                    foreach (string item in arrayTitleList)
                    {
                        string[] arrayTitleValue = Regex.Split(item, ":");
                        if (arrayTitleValue[1] == cmb_Current_Past.SelectedItem.ToString())
                        {
                            LinkedInScraper.TitleScope = arrayTitleValue[0];
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                #endregion


            }
            catch (Exception ex)
            {
            }
        }
    }
}
