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
    /// Interaction logic for UserControl_SalesNaviagtor_IndustryLangRelationship.xaml
    /// </summary>
    public partial class UserControl_SalesNaviagtor_IndustryLangRelationship : UserControl
    {
        string Language = "English;en,Spanish;es,German;de,French;fr,Italian;it,Portuguese;pt,Dutch;nl,Bahasa Indonesia;in,Malay;ms,Romanian;ro,Russian;ru,Turkish;tr,Swedish;sv,Polish;pl,Others;_o";
        string IndustryList = "Accounting;47,Airlines/Aviation;94,Alternative Dispute Resolution;120,Alternative Medicine;125,Animation;127,Apparel & Fashion;19,Architecture & Planning;50,Arts and Crafts;111,Automotive;53,Aviation & Aerospace;52,Banking;41,Biotechnology;12,Broadcast Media;36,Building Materials;49,Business Supplies and Equipment;138,Capital Markets;129,Chemicals;54,Civic & Social Organization;90,Civil Engineering;51,Commercial Real Estate;128,Computer & Network Security;118,Computer Games;109,Computer Hardware;3,Computer Networking;5,Computer Software;4,Construction;48,Consumer Electronics;24,Consumer Goods;25,Consumer Services;91,Cosmetics;18,Dairy;65,Defense & Space;1,Design;99,Education Management;69,E-Learning;132,Electrical/Electronic Manufacturing;112,Entertainment;28,Environmental Services;86,Events Services;110,Executive Office;76,Facilities Services;122,Farming;63,Financial Services;43,Fine Art;38,Fishery;66,Food & Beverages;34,Food Production;23,Fund-Raising;101,Furniture;26,Gambling & casinos;29,Glass, Ceramics & Concrete;145,Government Administration;75,Government Relations;148,Graphic Design;140,Health, Wellness and Fitness;124,Higher Education;68,Hospital & Health Care;14,Hospitality;31,Human Resources;137,Import and Export;134,Individual & Family Services;88,Industrial Automation;147,Information Services;84,Information Technology and Services;96,Insurance;42,International Affairs;74,International Trade and Development;141,Internet;6,Investment Banking;45,Investment Management;46,Judiciary;73,Law Enforcement;77,Law Practice;9,Legal Services;10,Legislative Office;72,Leisure, Travel & Tourism;30,Libraries;85,Logistics and Supply Chain;116,Luxury Goods & Jewelry;143,Machinery;55,Management Consulting;11,Maritime;95,Marketing and Advertising;80,Market Research;97,Mechanical or Industrial Engineering;135,Media Production;126,Medical Devices;17,Medical Practice;13,Mental Health Care;139,Military;71,Mining & Metals;56,Motion Pictures and Film;35,Museums and Institution;37,Music;115,Nanotechnology;114,Newspapers;81,Nonprofit Organization Management;100,Oil & Energy;57,Online Media;113,Outsourcing/Offshoring;123,Package/Freight Delivery;87,Packaging and Containers;146,Paper & Forest Products;61,Performing Arts;39,Pharmaceuticals;15,Philanthropy;131,Photography;136,Plastics;117,Political Organization;107,Primary/Secondary Education;67,Printing;83,Professional Training & Coaching;105,Program Development;102,Public Policy;79,Public Relations and Communications;98,Public Safety;78,Publishing;82,Railroad Manufacture;62,Ranching;64,Real Estate;44,Recreational Facilities and Services;40,Religious Institutions;89,Renewables & Environment;144,Research;70,Restaurants;32,Retail;27,Security and Investigations;121,Semiconductors;7,Shipbuilding;58,Sporting Goods;20,Sports;33,Staffing and Recruiting;104,Supermarkets;22,Telecommunications;8,Textiles;60,Think Tanks;130,Tobacco;21,Translation and Localization;108,Transportation/Trucking/Railroad;92,Utilities;59,Venture Capital & Private Equity;106,Veterinary;16,Warehousing;93,Wholesale;133,Wine and Spirits;142,Wireless;119,Writing and Editing;103";
        string RelationshipList = "N:All LinkedIn Members,F:1st Connections,S:2nd Connections,A:Group Members,O:3rd + Everyone Else";

        public UserControl_SalesNaviagtor_IndustryLangRelationship()
        {
            InitializeComponent();
            bindMethod();
        }
        public void bindMethod()
        {
            MainWindow objMainWindow = new MainWindow();
            string IndustryList = objMainWindow.IndustryList;

            string[] arrayIndustry = Regex.Split(IndustryList, ",");
            foreach (string item in arrayIndustry)
            {
                string[] arrayInds = Regex.Split(item, ";");
                if (arrayInds.Length == 2)
                {
                    chklstBox_SalesNavigator_Industry.Items.Add(arrayInds[0]);
                }
            }

            // Language
            string Language = objMainWindow.Language;
            string[] arrayLanguage = Regex.Split(Language, ",");
            foreach (string item in arrayLanguage)
            {
                string[] arraylang = Regex.Split(item, ";");
                if (arraylang.Length == 2)
                {
                    chklstBox_SalesNavigator_Language.Items.Add(arraylang[0]);
                }
            }

            // RElationship
            string RelationshipList = objMainWindow.RelationshipList;
            string[] arrayRelationship = Regex.Split(RelationshipList, ",");
            foreach (string item in arrayRelationship)
            {
                string[] arrayrelat = Regex.Split(item, ":");
                if (arrayrelat.Length == 2)
                {
                    chklstBox_SalesNavigator_Relationship.Items.Add(arrayrelat[1]);
                }
            }

        }


        private void btn_IndustryRelationship_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach(var item in chklstBox_SalesNavigator_Industry.CheckedItems)
                {
                    GlobalsScraper.lstIndustry_salesNavigator.Add(item.ToString());
                }
                foreach (var item in chklstBox_SalesNavigator_Language.CheckedItems)
                {
                    GlobalsScraper.lstIndustry_salesNavigator.Add(item.ToString());
                }
                foreach(var item in chklstBox_SalesNavigator_Relationship.CheckedItems)
                {
                    GlobalsScraper.lstIndustry_salesNavigator.Add(item.ToString());
                }
            }
            catch (Exception ex)
            {
            }

            try
            {

                #region Language List
                string language = string.Empty;
                string[] arrLang = Regex.Split(Language, ",");
                foreach (string LanguageL in chklstBox_SalesNavigator_Language.CheckedItems)
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
                SalesNavigator.language = language;
                #endregion

                #region Relationship List
                string Relationship = string.Empty;
                string[] arrRelation = Regex.Split(RelationshipList, ",");
                foreach (string RelationL in chklstBox_SalesNavigator_Relationship.CheckedItems)
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
                    SalesNavigator.relationship = string.Empty;
                }
                else
                {
                    SalesNavigator.relationship = Relationship;
                }
                #endregion

                #region Industry List
                string Industryvalue = string.Empty;
                string[] arrIndustry = Regex.Split(IndustryList, ",");

                foreach (string Industry in chklstBox_SalesNavigator_Industry.CheckedItems)
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
                        }
                    }
                }

                SalesNavigator.industry = Industryvalue;
                #endregion


            }
            catch(Exception ex)
            {
            }







        }
    }
}
