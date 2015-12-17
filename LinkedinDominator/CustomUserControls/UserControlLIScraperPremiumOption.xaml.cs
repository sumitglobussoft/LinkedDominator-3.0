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
    /// Interaction logic for UserControlLIScraperPremiumOption.xaml
    /// </summary>
    public partial class UserControlLIScraperPremiumOption : UserControl
    {
        public string Functionlist = MainWindow.Functionlist;
        public UserControlLIScraperPremiumOption()
        {
            InitializeComponent();
            bindFunctions();

        }

        private void lstBoxFunctions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        string senioritylevel = string.Empty;
        string IntrestedinList = string.Empty;
        string expList=string.Empty;
        string companysizelist = string.Empty;
        string RecentlyjoinedList=string.Empty;
        string Fortune_1000 = string.Empty;//fortuneList
        public void bindFunctions()
        {
            MainWindow objMainWindow = new MainWindow();
             Fortune_1000 = objMainWindow.fortuneList;
             companysizelist = objMainWindow.companysizelist;
             RecentlyjoinedList = objMainWindow.RecentlyjoinedList;
             expList = objMainWindow.expList;
             IntrestedinList = objMainWindow.IntrestedinList;
            senioritylevel = objMainWindow.senioritylevel;

            // For Functions
            string[] arrayFunction = Regex.Split(Functionlist, ",");
            foreach (string item in arrayFunction)
            {
                string[] arrayInds = Regex.Split(item, ":");
                if (arrayInds.Length == 2)
                {
                    chklstBoxFunctions.Items.Add(arrayInds[1]);
                }
            }

            // for Fortune 1000
            string[] arrayFortune = Regex.Split(Fortune_1000, ",");
            foreach (string item in arrayFortune)
            {
                string[] arrayInds = Regex.Split(item, ":");
                if (arrayInds.Length == 2)
                {
                    chk_Fortune_100.Items.Add(arrayInds[1]);
                }
            }

            // Company size
            string[] arrayCompanysize = Regex.Split(companysizelist, ",");
            foreach (string item in arrayCompanysize)
            {
                string[] arrayInds = Regex.Split(item, ":");
                if (arrayInds.Length == 2)
                {
                    chk_Company_size.Items.Add(arrayInds[1]);
                }
            }

            // Recently joined
            string[] arrayRecentlyjoined = Regex.Split(RecentlyjoinedList, ",");
            foreach (string item in arrayRecentlyjoined)
            {
                string[] arrayInds = Regex.Split(item, ":");
                if (arrayInds.Length == 2)
                {
                    chk_Recently_joined.Items.Add(arrayInds[1]);
                }
            }

            string[] arrayexpList = Regex.Split(expList, ",");
            foreach (string item in arrayexpList)
            {
                string[] arrayInds = Regex.Split(item, ":");
                if (arrayInds.Length == 2)
                {
                    chk_year_of_experiance.Items.Add(arrayInds[1]);
                }
            }

            string[] arrayIntrestedin = Regex.Split(IntrestedinList, ",");
            foreach (string item in arrayIntrestedin)
            {
                string[] arrayInds = Regex.Split(item, ":");
                if (arrayInds.Length == 2)
                {
                    chk_Interested_In.Items.Add(arrayInds[1]);
                }
            }

            // Seniority Lavel

            string[] arraySenoirtyLevel = Regex.Split(senioritylevel, ",");
            foreach (string item in arraySenoirtyLevel)
            {
                string[] arrayInds = Regex.Split(item, ":");
                if (arrayInds.Length == 2)
                {
                    chk_Seniority_Level.Items.Add(arrayInds[1]);
                }
            }

            //List<string> SSSS = new List<string>();
            //SSSS.Add("test");
            //SSSS.Add("test1");
            //SSSS.Add("test2");
            //foreach (var item in SSSS)
            //{
            //    chklstBoxFunctions.Items.Add(item);
                     
            //}
        }

        private void btn_LIS_Premium_Option_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //foreach (var items in chklstBoxFunctions.CheckedItems)
                //{
                //    GlobalsScraper.lstFunction.Add(items.ToString());
                //}
                //foreach (var items in chk_Fortune_100.CheckedItems)
                //{
                //    GlobalsScraper.lstFortune.Add(items.ToString());
                //}
                //foreach(var items in chk_Company_size.CheckedItems)
                //{
                //    GlobalsScraper.lstCompanySize.Add(items.ToString());
                //}
                //foreach(var items in chk_Recently_joined.CheckedItems)
                //{
                //    GlobalsScraper.lstRecentlyJoined.Add(items.ToString());
                //}
                //foreach(var items in chk_year_of_experiance.CheckedItems)
                //{
                //    GlobalsScraper.lstYearOfExperience.Add(items.ToString());
                //}
                //foreach(var items in chk_Interested_In.CheckedItems)
                //{
                //    GlobalsScraper.lstInterestedIn.Add(items.ToString());
                //}
                //foreach(var items in chk_Seniority_Level.CheckedItems)
                //{
                //    GlobalsScraper.lstSeniorityLevel.Add(items.ToString());
                //}

                #region Seniority
                try
                {
                    string Seniorlevelvalue = string.Empty;
                    string[] senoirLevelList = Regex.Split(senioritylevel, ",");
                    foreach (string Seniorlevel in chk_Seniority_Level.CheckedItems)
                    {
                        foreach (string itemSeniorLevel in senoirLevelList)
                        {
                            try
                            {
                                if (itemSeniorLevel.Contains(Seniorlevel))
                                {
                                    string[] arrysenoirLevel = Regex.Split(itemSeniorLevel, ":");
                                    if (arrysenoirLevel.Length == 2)
                                    {
                                        if (!string.IsNullOrEmpty(arrysenoirLevel[0]))
                                        {
                                            if (string.IsNullOrEmpty(Seniorlevelvalue))
                                            {
                                                Seniorlevelvalue = arrysenoirLevel[0];
                                            }
                                            else
                                            {
                                                Seniorlevelvalue = Seniorlevelvalue + "," + arrysenoirLevel[0];
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

                    if (Seniorlevelvalue == "all")
                    {
                        LinkedInScraper.SeniorLevel = string.Empty;
                    }
                    else
                    {
                        LinkedInScraper.SeniorLevel = Seniorlevelvalue;
                    }


                }
                catch(Exception ex)
                { }

                #endregion

                #region Company size List
                string companysizevalue = string.Empty;
                string[] arraycompnaysize = Regex.Split(companysizelist, ",");
                foreach (string companysize in chk_Company_size.CheckedItems)
                {
                    foreach (string item in arraycompnaysize)
                    {
                        try
                        {
                            if (item.Split(':')[1] == companysize)
                            {
                                string[] ArrayCompnay = Regex.Split(item, ":");
                                if (!string.IsNullOrEmpty(ArrayCompnay[0]))
                                {
                                    if (string.IsNullOrEmpty(companysizevalue))
                                    {
                                        companysizevalue = ArrayCompnay[0];
                                    }
                                    else
                                    {
                                        companysizevalue = companysizevalue + "," + ArrayCompnay[0];
                                    }
                                }
                                //break;
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    //break;
                }

                if (companysizevalue == "all")
                {
                    LinkedInScraper.CompanySize = string.Empty;
                }
                else
                {
                    LinkedInScraper.CompanySize = companysizevalue;
                }
                #endregion

                #region Function List
                string Function = string.Empty;
                //if (SearchCriteria.AccountType == "RecuiterType")
                //{
                //    Functionlist = Functionlistrecruiter;
                //}

                string[] FunctionList = Regex.Split(Functionlist, ",");
                foreach (string FunctionL in chklstBoxFunctions.CheckedItems)
                {
                    foreach (string itemFunction in FunctionList)
                    {
                        try
                        {
                            if (itemFunction.Contains(FunctionL))
                            {
                                string[] functionItem = Regex.Split(itemFunction, ":");
                                if (!string.IsNullOrEmpty(functionItem[0]))
                                {
                                    if (string.IsNullOrEmpty(Function))
                                    {
                                        Function = functionItem[0];
                                    }
                                    else
                                    {
                                        Function = Function + "," + functionItem[0];
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                if (Function == "all")
                {
                    LinkedInScraper.Function = string.Empty;
                }
                else
                {
                    LinkedInScraper.Function = Function;
                }
                #endregion

                #region Recently Joined
                string RecentlyJoined = string.Empty;
                string[] RecetlyJoinedArr = Regex.Split(RecentlyjoinedList, ",");
                foreach (string RecentlyJoinedL in chk_Recently_joined.CheckedItems)
                {
                    foreach (string item in RecetlyJoinedArr)
                    {
                        try
                        {
                            string[] arrayitem = Regex.Split(item, ":");
                            if (item.Contains(RecentlyJoinedL))
                            {
                                if (!string.IsNullOrEmpty(arrayitem[0]))
                                {
                                    if (string.IsNullOrEmpty(RecentlyJoined))
                                    {
                                        RecentlyJoined = arrayitem[0];
                                    }
                                    else
                                    {
                                        RecentlyJoined = RecentlyJoined + "," + arrayitem[0];
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                if (RecentlyJoined == "select-all")
                {
                    LinkedInScraper.RecentlyJoined = string.Empty;
                }
                else
                {
                    LinkedInScraper.RecentlyJoined = RecentlyJoined.Trim();
                }
                #endregion


                #region Fortune List
                string Fortune1000 = string.Empty;
                string[] FortuneArr = Regex.Split(Fortune_1000, ",");
                foreach (string Fortune1000L in chk_Fortune_100.CheckedItems)
                {
                    foreach (string item in FortuneArr)
                    {
                        try
                        {
                            string[] arrayItem = Regex.Split(item, ":");
                            if (item.Contains(Fortune1000L))
                            {
                                if (!string.IsNullOrEmpty(arrayItem[0]))
                                {
                                    if (string.IsNullOrEmpty(Fortune1000))
                                    {
                                        Fortune1000 = arrayItem[0];
                                    }
                                    else
                                    {
                                        Fortune1000 = Fortune1000 + "," + arrayItem[0];
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                if (RecentlyJoined == "select-all")
                {
                    LinkedInScraper.Fortune1000 = string.Empty;
                }
                else
                {
                    LinkedInScraper.Fortune1000 = Fortune1000;
                }
                #endregion

                #region Explerience list
                string TotalExperience = string.Empty;
                string[] arratExpericne = Regex.Split(expList, ",");
                foreach (string YearOfExperienceL in chk_year_of_experiance.CheckedItems)
                {
                    foreach (string itemExp in arratExpericne)
                    {
                        try
                        {
                            if (itemExp.Contains(YearOfExperienceL))
                            {
                                string[] arrayitem = Regex.Split(itemExp, ":");
                                if (!string.IsNullOrEmpty(arrayitem[1]))
                                {
                                    if (string.IsNullOrEmpty(TotalExperience))
                                    {
                                        TotalExperience = arrayitem[0];
                                    }
                                    else
                                    {
                                        TotalExperience = TotalExperience + "," + arrayitem[0];
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }

                if (TotalExperience == "select-all")
                {
                    LinkedInScraper.YearOfExperience = string.Empty;
                }
                else
                {
                    LinkedInScraper.YearOfExperience = TotalExperience;
                }
                #endregion

                #region IntrestedIn
                string InterestedIn = string.Empty;
                string[] IntrestesList = Regex.Split(IntrestedinList, ",");
                foreach (string InterestedL in chk_Interested_In.CheckedItems)
                {
                    foreach (string Intresteditem in IntrestesList)
                    {
                        try
                        {
                            if (Intresteditem.Contains(InterestedL))
                            {
                                string[] arrayIntrst = Regex.Split(Intresteditem, ":");
                                if (!string.IsNullOrEmpty(Intresteditem))
                                {
                                    if (string.IsNullOrEmpty(InterestedIn))
                                    {
                                        InterestedIn = arrayIntrst[0];
                                    }
                                    else
                                    {
                                        InterestedIn = InterestedIn + "," + arrayIntrst[0];
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                if (InterestedIn == "select-all")
                {
                    LinkedInScraper.InerestedIn = string.Empty;
                }
                else
                {
                    LinkedInScraper.InerestedIn = InterestedIn;
                }

                #endregion







            }
            catch (Exception ex)
            {
            }
        }

    }
}
