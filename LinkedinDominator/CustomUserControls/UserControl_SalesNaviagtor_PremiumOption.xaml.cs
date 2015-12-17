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
    /// Interaction logic for UserControl_SalesNaviagtor_PremiumOption.xaml
    /// </summary>
    public partial class UserControl_SalesNaviagtor_PremiumOption : UserControl
    {
        string companysizelist = "all:All Company Sizes,1:1-10,2:11-50,3:51-200,4:201-500,5:501-1000,6:1001-5000,7:5001-10000,8:10000+";
        string Functionlist = "all:All Functions,1:Accounting,2:Administrative,3:Arts and Design,4:Business development,5:Community and Social Services,6:Consulting,7:Education,8:Engineering,9:Entrepreneurship,10:Finance,11:Healthcare Services,12:Human Resources,13:Information Technology,14:Legal,15:Marketing,16:Media and Communication,17:Military and Protective Services,18:Operations,19:Product Management,20:Program and Project Management,21:Purchasing,22:Quality Assurance,23:Real Estate,24:Research,25:Sales,26:Support";
        string senioritylevel = "all:All Seniority Levels,1:Unpaid,2:Training,3:Entry,4:Senior,5:Manager,6:Director,7:VP,8:CXO,9:Partner,10:Owner";

        string expList = "1:Less than 1 year,2:1 to 2 years,3:3 to 5 years,4:6 to 10 years,5:More than 10 years";
        string RecentlyjoinedList = "select-all:Any Time,1:1 day ago,2:2-7 days ago,3:8-14 days ago,4:15-30 days ago,5:1-3 months ago";

        public UserControl_SalesNaviagtor_PremiumOption()
        {
            InitializeComponent();
            bindMethod();
        }
        public void bindMethod()
        {
            MainWindow objMainWindow = new MainWindow();
            string Fortune_1000 = objMainWindow.fortuneList;
            string companysizelist = objMainWindow.companysizelist;
            string RecentlyjoinedList = objMainWindow.RecentlyjoinedList;
            string expList = objMainWindow.expList;
            string IntrestedinList = objMainWindow.IntrestedinList;
            string senioritylevel = objMainWindow.senioritylevelRecruiterType;
            string Functionlistrecruiter = objMainWindow.Functionlistrecruiter;
            // For Functions
            string[] arrayFunction = Regex.Split(Functionlistrecruiter, ",");
            foreach (string item in arrayFunction)
            {
                string[] arrayInds = Regex.Split(item, ":");
                if (arrayInds.Length == 2)
                {
                    chklstBox_SalesNavigator_Functions.Items.Add(arrayInds[1]);
                }
            }

            // for Fortune 1000
            //string[] arrayFortune = Regex.Split(Fortune_1000, ",");
            //foreach (string item in arrayFortune)
            //{
            //    string[] arrayInds = Regex.Split(item, ":");
            //    if (arrayInds.Length == 2)
            //    {
            //        chklstBox_SalesNavigator_Company_size.Items.Add(arrayInds[1]);
            //    }
            //}

            // Company size
            string[] arrayCompanysize = Regex.Split(companysizelist, ",");
            foreach (string item in arrayCompanysize)
            {
                string[] arrayInds = Regex.Split(item, ":");
                if (arrayInds.Length == 2)
                {
                    chklstBox_SalesNavigator_Company_size.Items.Add(arrayInds[1]);
                }
            }

            // Recently joined
            string[] arrayRecentlyjoined = Regex.Split(RecentlyjoinedList, ",");
            foreach (string item in arrayRecentlyjoined)
            {
                string[] arrayInds = Regex.Split(item, ":");
                if (arrayInds.Length == 2)
                {
                    chklstBox_SalesNavigator_WhenJoined.Items.Add(arrayInds[1]);
                }
            }

            string[] arrayexpList = Regex.Split(expList, ",");
            foreach (string item in arrayexpList)
            {
                string[] arrayInds = Regex.Split(item, ":");
                if (arrayInds.Length == 2)
                {
                    chklstBox_SalesNavigator_YearOfExperiance.Items.Add(arrayInds[1]);
                }
            }

            //string[] arrayIntrestedin = Regex.Split(IntrestedinList, ",");
            //foreach (string item in arrayIntrestedin)
            //{
            //    string[] arrayInds = Regex.Split(item, ":");
            //    if (arrayInds.Length == 2)
            //    {
            //        chk_Interested_In.Items.Add(arrayInds[1]);
            //    }
            //}

            // Seniority Lavel

            string[] arraySenoirtyLevel = Regex.Split(senioritylevel, ",");
            foreach (string item in arraySenoirtyLevel)
            {
                string[] arrayInds = Regex.Split(item, ":");
                if (arrayInds.Length == 2)
                {
                    chklstBox_SalesNavigator_SeniorityLevel.Items.Add(arrayInds[1]);
                }
            }

        }

        private void btn_LIS_Premium_Option_Save_Click(object sender, RoutedEventArgs e)
        {
            #region ankit sir
            //try
            //{
            //    foreach(var item in chklstBox_SalesNavigator_Functions.CheckedItems)
            //    {
            //        GlobalsScraper.chkLstFunction_Premium_SalesNav.Add(item.ToString());
            //    }
            //    foreach(var item in chklstBox_SalesNavigator_SeniorityLevel.CheckedItems)
            //    {
            //        GlobalsScraper.chkLstSeniorityLevel_Premium_SalesNav.Add(item.ToString());
            //    }
            //    foreach(var item in chklstBox_SalesNavigator_Company_size.CheckedItems)
            //    {
            //        GlobalsScraper.chkLstCompanySize_Premium_SalesNav.Add(item.ToString());
            //    }
            //    foreach(var item in chklstBox_SalesNavigator_YearOfExperiance.CheckedItems)
            //    {
            //        GlobalsScraper.chkLstYearOfExp_Premium_SalesNav.Add(item.ToString());
            //    }
            //    foreach(var item in chklstBox_SalesNavigator_WhenJoined.CheckedItems)
            //    {
            //        GlobalsScraper.chkLstWhenJoined_Premium_SalesNav.Add(item.ToString());
            //    }
            //}
            //catch (Exception ex)
            //{
            //}

            #endregion

            try
            {
                #region Company size List
                string companysizevalue = string.Empty;
                string[] arraycompnaysize = Regex.Split(companysizelist, ",");
                foreach (string companysize in chklstBox_SalesNavigator_Company_size.CheckedItems)
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
                    SalesNavigator.companySize = string.Empty;
                }
                else
                {
                    SalesNavigator.companySize = companysizevalue;
                }
                #endregion

                #region Function List
                string Function = string.Empty;
                string[] FunctionList = Regex.Split(Functionlist, ",");
                foreach (string FunctionL in chklstBox_SalesNavigator_Functions.CheckedItems)
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
                    SalesNavigator.function = string.Empty;
                }
                else
                {
                    SalesNavigator.function = Function;
                }
                #endregion

                #region Senority List
                string Seniorlevelvalue = string.Empty;
                string[] senoirLevelList = Regex.Split(senioritylevel, ",");
                foreach (string Seniorlevel in chklstBox_SalesNavigator_SeniorityLevel.CheckedItems)
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
                    SalesNavigator.seniorityLevel = string.Empty;
                }
                else
                {
                    SalesNavigator.seniorityLevel = Seniorlevelvalue;
                }
                #endregion

                #region Experience list
                string TotalExperience = string.Empty;
                string[] arratExpericne = Regex.Split(expList, ",");
                foreach (string YearOfExperienceL in chklstBox_SalesNavigator_YearOfExperiance.CheckedItems)
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
                    SalesNavigator.yearOfExperience = string.Empty;
                }
                else
                {
                    SalesNavigator.yearOfExperience = TotalExperience;
                }
                #endregion

                #region When Joined
                try
                {

                    string RecentlyJoined = string.Empty;
                    string[] RecetlyJoinedArr = Regex.Split(RecentlyjoinedList, ",");
                    foreach (string RecentlyJoinedL in chklstBox_SalesNavigator_WhenJoined.CheckedItems)
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
                        SalesNavigator.whenJoined = string.Empty;
                    }
                    else
                    {
                        SalesNavigator.whenJoined = RecentlyJoined.Trim();
                    }
                }
                catch (Exception ex)
                {
                }
                #endregion


            }
            catch(Exception ex)
            { }

        }
    }
}
