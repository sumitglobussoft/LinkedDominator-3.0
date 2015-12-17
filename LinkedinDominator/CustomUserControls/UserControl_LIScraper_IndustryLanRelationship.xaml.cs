using BaseLib;
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
    /// Interaction logic for UserControl_LIScraper_IndustryLanRelationship.xaml
    /// </summary>
    public partial class UserControl_LIScraper_IndustryLanRelationship : UserControl
    {
        public UserControl_LIScraper_IndustryLanRelationship()
        {
            InitializeComponent();
            bindMethod();
        }

        string IndustryList = string.Empty;
        string Language = string.Empty;
        string RelationshipList = string.Empty;

        public void bindMethod()
        {

            MainWindow objMainWindow = new MainWindow();
             IndustryList = objMainWindow.IndustryList;
             Language = objMainWindow.Language;
             RelationshipList = objMainWindow.RelationshipList;


            string[] arrayIndustry = Regex.Split(IndustryList, ",");
            foreach (string item in arrayIndustry)
            {
                string[] arrayInds = Regex.Split(item, ";");
                if (arrayInds.Length == 2)
                {
                    chk_LIS_Industry.Items.Add(arrayInds[0]);
                }
            }

            // Language
           
            string[] arrayLanguage = Regex.Split(Language, ",");
            foreach (string item in arrayLanguage)
            {
                string[] arraylang = Regex.Split(item, ";");
                if (arraylang.Length == 2)
                {
                    chk_LIS_Language.Items.Add(arraylang[0]);
                }
            }

            // RElationship
            
            string[] arrayRelationship = Regex.Split(RelationshipList, ",");
            foreach (string item in arrayRelationship)
            {
                string[] arrayrelat = Regex.Split(item, ":");
                if (arrayrelat.Length == 2)
                {
                    chk_LIS_Relationship.Items.Add(arrayrelat[1]);
                }
            }

        }

        private void btn_IndustryRelationship_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach(var items in chk_LIS_Industry.CheckedItems)
                {
                    GlobalsScraper.lstIndustry.Add(items.ToString());
                }
                foreach(var items in chk_LIS_Language.CheckedItems)
                {
                    GlobalsScraper.lstLanguage.Add(items.ToString());
                }
                foreach(var items in chk_LIS_Relationship.CheckedItems)
                {
                    GlobalsScraper.lstRelationship.Add(items.ToString());
                }
            }
            catch (Exception ex)
            {

            }
            #region Industry
            string Industryvalue = string.Empty;
            string[] arrIndustry = Regex.Split(IndustryList, ",");
            try
            {
                foreach (string Industry in chk_LIS_Industry.CheckedItems)
                {
                    foreach (string itemIndustry in arrIndustry)
                    {
                        try
                        {
                            if (itemIndustry.Contains(Industry))
                            {
                                string[] arryIndustry = Regex.Split(itemIndustry, ";");
                                if (arryIndustry.Length == 2)
                                {
                                    if (!string.IsNullOrEmpty(arryIndustry[1]))
                                    {
                                        if (string.IsNullOrEmpty(Industryvalue))
                                        {
                                            Industryvalue = arryIndustry[1];
                                        }
                                        else
                                        {
                                            Industryvalue = Industryvalue + "," + arryIndustry[1];
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Exception : " + ex);
                        }
                    }
                }

                LinkedInScraper.IndustryType = Industryvalue;
            }
            catch(Exception ex)
            { }
            #endregion

            #region Relationship

            string Relationship = string.Empty;
            string[] arrRelation = Regex.Split(RelationshipList, ",");
            try
            {
                foreach (string RelationL in chk_LIS_Relationship.CheckedItems)
                {
                    foreach (string item in arrRelation)
                    {
                        try
                        {
                            if (item.Contains(RelationL))
                            {
                                string[] arryRelat = Regex.Split(item, ":");
                                if (arryRelat.Length == 2)
                                {
                                    if (!string.IsNullOrEmpty(arryRelat[0]))
                                    {
                                        if (string.IsNullOrEmpty(Relationship))
                                        {
                                            Relationship = arryRelat[0];
                                        }
                                        else
                                        {
                                            Relationship = arryRelat[0] + "," + Relationship;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                if (Relationship == "N")
                {
                    SearchCriteria.Relationship = string.Empty;
                     LinkedInScraper.Relationship=string.Empty;
                }
                else
                {
                    SearchCriteria.Relationship = Relationship;
                    LinkedInScraper.Relationship=Relationship;
                }
            }

            catch(Exception ex)
            { }

            #endregion

            #region Language List
            string language = string.Empty;
            string[] arrLang = Regex.Split(Language, ",");
            foreach (string LanguageL in chk_LIS_Language.CheckedItems)
            {
                foreach (string item in arrLang)
                {
                    try
                    {
                        if (item.Contains(LanguageL))
                        {
                            string[] arryLang = Regex.Split(item, ";");
                            if (arryLang.Length == 2)
                            {
                                if (!string.IsNullOrEmpty(arryLang[1]))
                                {
                                    if (string.IsNullOrEmpty(language))
                                    {
                                        language = arryLang[1];
                                    }
                                    else
                                    {
                                        language = language + "," + arryLang[1];
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            if (string.IsNullOrEmpty(language))
            {
                //language = "select-all";
                language = "";
            }
            SearchCriteria.language = language;
            LinkedInScraper.language = language;
            #endregion



        }

    }
}
