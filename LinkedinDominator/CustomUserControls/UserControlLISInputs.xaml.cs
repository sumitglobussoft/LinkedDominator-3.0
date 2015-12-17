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
using BaseLib;
using System.Text.RegularExpressions;
using linkedDominator;
using Scraper;
namespace LinkeddinDominator.CustomUserControls
{
    /// <summary>
    /// Interaction logic for UserControlLISInputs.xaml
    /// </summary>
    public partial class UserControlLISInputs : UserControl
    {
        public UserControlLISInputs()
        {
            InitializeComponent();
            bindMethod();
        }
        string TitleValue = string.Empty;
        string WithingList = string.Empty;
        public void bindMethod()
        {
            MainWindow objMainWindow = new MainWindow();
             WithingList = objMainWindow.WithingList;

            foreach(string item in LDGlobals.listAccounts)
            {
                string userName=item.Split(':')[0];
                cmb_LIScraper_SelectedAcc.Items.Add(userName);

            }
            

            Dictionary<string, string> CountryCode = new Dictionary<string, string>();
            ClsSelect ObjSelectMethod = new ClsSelect();
            CountryCode = ObjSelectMethod.getCountry();
            foreach (KeyValuePair<string, string> pair in CountryCode)
            {
                try
                {
                    cmb_LIS_Country_Code.Items.Add(pair.Value);
                   // CombScraperCountry.Items.Add(pair.Value);


                  //  lstCountry.Add(pair.Value);
                   // CampainGroupCreate.Campaign_lstCountry.Add(pair.Value);
                    
                }
                catch
                {
                }
            }
            
            // location
            try
            {
                cmb_LIS_Location.Items.Add("Located in or near");
                cmb_LIS_Location.Items.Add("Anywhere");
                cmb_LIS_Location.Text = "Anywhere";


            }
            catch
            { }

            // within
            string[] arraywithin = Regex.Split(WithingList, ",");
            foreach (string item in arraywithin)
            {
                string[] arrayPostalwithin = Regex.Split(item, ":");
                if (arrayPostalwithin.Length == 2)
                {
                    cmb_LIS_Within.Items.Add(arrayPostalwithin[1]);
                }
            }

            // title curernt or past
            TitleValue = objMainWindow.TitleValue;
            string[] arrayTitleValue = Regex.Split(TitleValue, ",");
           // if (!Globals.ComboBoxCurrent_Past_Task_Done)
           // {
                foreach (string item in arrayTitleValue)
                {
                    string[] arraytitleValue = Regex.Split(item, ":");
                    if (arraytitleValue.Length == 2)
                    {
                        cmb_LIS_Company_CrrentOrPast.Items.Add(arraytitleValue[1]);
                       // cmbboxCompanyValue.Items.Add(arraytitleValue[1]);
                    }
                }
          //  }
                cmb_LIS_Country_Code.IsEnabled = false;
                LinkedInScraper.LocationType = "Y";
        }

        private void btn_LIScraper_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
              LinkedInScraper.selectedEmailId = cmb_LIScraper_SelectedAcc.SelectedItem.ToString();
              LinkedInScraper.FirstName = txt_LIScraper_FirstName.Text;
              LinkedInScraper.LastName = txt_LIScraper_LastName.Text;
              LinkedInScraper.selectedLocation = cmb_LIS_Location.SelectedItem.ToString();
              LinkedInScraper.selectedCountry = cmb_LIS_Country_Code.SelectedItem.ToString();
              LinkedInScraper.postalCode = txt_LIScraper_PostalCode.Text;
              LinkedInScraper.Within = cmb_LIS_Within.SelectedItem.ToString();
              LinkedInScraper.CompanyValue = txt_LIScraper_Comapny.Text;
              LinkedInScraper.CompanyScope = cmb_LIS_Company_CrrentOrPast.SelectedItem.ToString();

              string countryName = cmb_LIS_Country_Code.SelectedItem.ToString();

              #region within CompanyScope
              try
              {
                 // if (cmb_LIS_Company_CrrentOrPast.Enabled == true)
                  {
                      string[] arrayTitleList = Regex.Split(TitleValue, ",");
                      foreach (string item in arrayTitleList)
                      {
                          string[] arrayTitleValue = Regex.Split(item, ":");

                          if (arrayTitleValue[1] == cmb_LIS_Company_CrrentOrPast.SelectedItem.ToString())
                          {
                              LinkedInScraper.CompanyScope = arrayTitleValue[0];
                          }
                      }
                  }
              }
              catch (Exception ex)
              {

              }
              #endregion


              #region Within the PostalCode
              try
              {
                  string[] arraywithinList = Regex.Split(WithingList, ",");
                  foreach (string item in arraywithinList)
                  {
                      if (item.Contains(cmb_LIS_Within.SelectedItem.ToString()))
                      {
                          string[] arrayWithin = Regex.Split(item, ":");
                          LinkedInScraper.Within = arrayWithin[0];
                      }
                  }
              }
              catch (Exception ex)
              {

              }
              #endregion

                #region country Code

              try
              {
                  ClsSelect CountryList = new ClsSelect();
                  string[] arrayCountry = Regex.Split(CountryList.country, ",");

                  foreach (string CountryValue in arrayCountry)
                  {
                      if (CountryValue.Contains(countryName))
                      {
                          if (countryName == "India")
                          {
                              string[] value = Regex.Split(CountryValue, ":");
                              countryName = value[1].ToString();
                              LinkedInScraper.Country = value[0].ToString();
                          }
                          else
                          {
                              string[] value = Regex.Split(CountryValue, ":");
                              countryName = value[1].ToString();
                              LinkedInScraper.Country = value[0].ToString();
                              break;
                          }
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

        private void cmb_LIS_Location_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmb_LIS_Location.SelectedItem.Equals("Anywhere"))
                {
                    cmb_LIS_Country_Code.IsEnabled = false;
                    cmb_LIS_Country_Code.Text = "";
                    LinkedInScraper.LocationType = "Y";

                }
                if(cmb_LIS_Location.SelectedItem.Equals("Located in or near"))
                {
                    cmb_LIS_Country_Code.IsEnabled = true;
                    cmb_LIS_Country_Code.Text = "United States";
                    LinkedInScraper.LocationType = "I";

                }
            }
            catch(Exception ex)
            { }
        }
    }
}
