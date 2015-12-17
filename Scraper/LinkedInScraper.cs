using Accounts;
using BaseLib;
using Globussoft;
using Groups;
using LinkDominator;
using linkedDominator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper
{
    public class LinkedInScraper
    {


        public static string Fortune1000 = string.Empty;
        public static string SeniorLevel = string.Empty;
        public static string InerestedIn = string.Empty;
        public static string CompanySize = string.Empty;
        public static string Function = string.Empty;
        public static string RecentlyJoined = string.Empty;
        public static string YearOfExperience = string.Empty;


        public static string FirstName = string.Empty;
        public static string LastName = string.Empty;
        public static string selectedEmailId = string.Empty;
        public static string selectedLocation = string.Empty;
        public static string selectedCountry = string.Empty;
        public static string postalCode = string.Empty;
        public static string Within = string.Empty;
        public static string CompanyValue = string.Empty;
        public static string CompanyScope = string.Empty;
        public static string Country = string.Empty;
        public static string LocationType = string.Empty;


        


        public static string Relationship = string.Empty;
        public static string IndustryType = string.Empty;
        public static string language = string.Empty;
        public static string TitleValue = string.Empty;
        public static string Keyword = string.Empty;
        public static string TitleScope = string.Empty;

        string PostRequestURL = string.Empty;
        string csrfToken = string.Empty;
        string PostdataForPagination = string.Empty;

        string NewSearchPage = string.Empty;
        string ResponseWallPostForPremiumAcc = string.Empty;
        GlobusHttpHelper _HttpHelper = new GlobusHttpHelper();
        List<string> RecordURL = new List<string>();
        Queue<string> queRecordUrl = new Queue<string>();
        ChilkatHttpHelpr objChilkat = new ChilkatHttpHelpr();
        string PostResponce = string.Empty;

        readonly object lockrThreadControllerLinkedInScraper = new object();
        public void ThreadStartLinkedInScraper()
        {
            try
            {
                int numberOfAccountPatch = 25;

                if (GlobalsScraper.NoOfThreadsLinkedInScraper > 0)
                {
                    numberOfAccountPatch = GlobalsScraper.NoOfThreadsLinkedInScraper;
                }

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (LDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Split(LDGlobals.listAccounts, numberOfAccountPatch);

                    foreach (List<string> listAccounts in list_listAccounts)
                    {

                        foreach (string account in listAccounts)
                        {
                            try
                            {
                                lock (lockrThreadControllerLinkedInScraper)
                                {
                                    try
                                    {
                                        if (GlobalsScraper.countThreadControllerLinkedInScraper >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerLinkedInScraper);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        LinkedinUser item = null;
                                        LDGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {
                                            Thread profilerThread = new Thread(StartMultiThreadsLinkedInScraper);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;

                                            profilerThread.Start(new object[] { item });

                                            GlobalsScraper.countThreadControllerLinkedInScraper++;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        // GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
        public void StartMultiThreadsLinkedInScraper(object parameters)
        {
            try
            {
                if (!GlobalsScraper.isStopLinkedInScraper)
                {
                    try
                    {
                        GlobalsScraper.lstThreadsLinkedInScraper.Add(Thread.CurrentThread);
                        GlobalsScraper.lstThreadsLinkedInScraper.Distinct();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {
                        Array paramsArray = new object[1];
                        paramsArray = (Array)parameters;

                        LinkedinUser objLinkedinUser = (LinkedinUser)paramsArray.GetValue(0);

                        if (!objLinkedinUser.isloggedin)
                        {
                            GlobusHttpHelper objGlobusHttpHelper = objLinkedinUser.globusHttpHelper;

                            //Login Process
                            Accounts.AccountManager objAccountManager = new AccountManager();
                            objAccountManager.LoginHttpHelper(ref objLinkedinUser);
                        }

                        if (objLinkedinUser.isloggedin)
                        {
                            // Call StartActionMessageReply
                            //StartLinkedInScraper(ref objLinkedinUser);
                            SearchCriteria.starter = true;
                            StartLinkedinScraperWithPagination(ref objLinkedinUser);

                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Couldn't Login With Username : " + objLinkedinUser.username);
                            GlobusLogHelper.log.Debug("Couldn't Login With Username : " + objLinkedinUser.username);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            finally
            {
                try
                {
                    // if (!isStopWallPoster)
                    {
                        lock (lockrThreadControllerLinkedInScraper)
                        {
                            GlobalsScraper.countThreadControllerLinkedInScraper = GlobalsScraper.countThreadControllerLinkedInScraper--;
                            Monitor.Pulse(lockrThreadControllerLinkedInScraper);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        public void StartLinkedinScraperWithPagination(ref LinkedinUser objLinkedinUser)
         {
            #region Login
            try
            {
                GlobusHttpHelper HttpHelper = objLinkedinUser.globusHttpHelper;
                //Temprary class
                //======================================================
                //string tempurl = "http://www.linkedin.com/profile/view?id=224916256&authType=OUT_OF_NETWORK&authToken=SWNz&locale=en_US&srchid=3387141351401255871148&srchindex=1&srchtotal=2017&trk=vsrp_people_res_name&trkInfo=VSRPsearchId%3A3387141351401255871148%2CVSRPtargetId%3A224916256%2CVSRPcmpt%3Aprimary";
                //CrawlingLinkedInPage(tempurl, ref HttpHelper);
                //======================================================
                string nextPageUrl = string.Empty;
                if (SearchCriteria.AccountType == "RecuiterType")
                {
                    string pageSourceaAdvanceSearch = HttpHelper.getHtmlfromUrl1(new Uri("https://www.linkedin.com/recruiter/search"));
                    string referralUrl = string.Empty;
                    if (pageSourceaAdvanceSearch.Contains("csrfToken"))
                    {
                        try
                        {
                            int pagenumberrecruiter = 0;
                            bool IsShowLoggerPagecount = true;
                            int i = 1;
                            int startCount = 0;
                       

                            #region seniorLevel
                            string tempSeniorlevel = string.Empty;
                            if (SearchCriteria.SeniorLevel.Contains(","))
                            {
                                string[] arrseniorLevel = Regex.Split(SearchCriteria.SeniorLevel, ",");
                                if (arrseniorLevel.Count() > 1)
                                {
                                    foreach (string item in arrseniorLevel)
                                    {
                                        tempSeniorlevel += "&facet.SE=" + item;
                                    }
                                }
                                else
                                {
                                    tempSeniorlevel = "&facet.SE=";
                                }
                            }
                            else if (string.IsNullOrEmpty(SearchCriteria.SeniorLevel))
                            {
                                tempSeniorlevel = "&facet.SE=";
                            }
                            else
                            {
                                tempSeniorlevel = "&facet.SE=" + SearchCriteria.SeniorLevel;
                            } 
                            #endregion

                            #region Function
                            string tempFunction = string.Empty;
                            if (SearchCriteria.Function.Contains(","))
                            {
                                string[] arrFunction = Regex.Split(SearchCriteria.Function, ",");
                                if (arrFunction.Count() > 1)
                                {
                                    foreach (string item in arrFunction)
                                    {
                                        tempFunction += "&facet.FA=" + item;
                                    }
                                }
                                else
                                {
                                    tempFunction = "&facet.FA=";
                                }
                            }
                            else if (string.IsNullOrEmpty(SearchCriteria.Function))
                            {
                                tempFunction = "&facet.FA=";
                            }
                            else
                            {
                                tempFunction = "&facet.FA=" + SearchCriteria.Function;
                            }
                            #endregion

                            #region RelationShip
                            string tempRelationShip = string.Empty;
                            if (SearchCriteria.Relationship.Contains(","))
                            {
                                string[] arrRelationShip = Regex.Split(SearchCriteria.Relationship, ",");
                                if (arrRelationShip.Count() > 1)
                                {
                                    foreach (string item in arrRelationShip)
                                    {
                                        tempRelationShip += "&facet.N=" + item;
                                    }
                                }
                                else
                                {
                                    tempRelationShip = "&facet.N=";
                                }
                            }
                            else if (string.IsNullOrEmpty(SearchCriteria.Relationship))
                            {
                                tempRelationShip = "&facet.N=";
                            }
                            else
                            {
                                tempRelationShip = "&facet.N=" + SearchCriteria.Relationship;
                            }
                            #endregion

                            #region Language
                            string tempLanguage = string.Empty;
                            if (SearchCriteria.language.Contains(","))
                            {
                                string[] arrLanguage = Regex.Split(SearchCriteria.language, ",");
                                if (arrLanguage.Count() > 1)
                                {
                                    foreach (string item in arrLanguage)
                                    {
                                        tempLanguage += "&facet.L=" + item;
                                    }
                                }
                                else
                                {
                                    tempLanguage = "&facet.L=";
                                }
                            }
                            else if (string.IsNullOrEmpty(SearchCriteria.language))
                            {
                                tempLanguage = "&facet.L=";
                            }
                            else
                            {
                                tempLanguage = "&facet.L=" + SearchCriteria.language;
                            }
                            #endregion

                            #region Industry
                            string tempIndustry = string.Empty;
                            if (SearchCriteria.IndustryType.Contains(","))
                            {
                                string[] arrIndustry = Regex.Split(SearchCriteria.IndustryType, ",");
                                if (arrIndustry.Count() > 1)
                                {
                                    foreach (string item in arrIndustry)
                                    {
                                        tempIndustry += "&facet.I=" + item;
                                    }
                                }
                                else
                                {
                                    tempIndustry = "&facet.I=";
                                }
                            }
                            else if (string.IsNullOrEmpty(SearchCriteria.IndustryType))
                            {
                                tempIndustry = "&facet.I=";
                            }
                            else
                            {
                                tempIndustry = "&facet.I=" + SearchCriteria.IndustryType;
                            }
                            #endregion

                            #region Year of Experience
                            string tempExperience = string.Empty;
                            if (SearchCriteria.YearOfExperience.Contains(","))
                            {
                                string[] arrYearOfExperience = Regex.Split(SearchCriteria.YearOfExperience, ",");
                                if (arrYearOfExperience.Count() > 1)
                                {
                                    foreach (string item in arrYearOfExperience)
                                    {
                                        tempExperience += "&facet.TE=" + item;
                                    }
                                }
                                else
                                {
                                    tempExperience = "&facet.TE=";
                                }
                            }
                            else if (string.IsNullOrEmpty(SearchCriteria.YearOfExperience))
                            {
                                tempExperience = "&facet.TE=";
                            }
                            else
                            {
                                tempExperience = "&facet.TE=" + SearchCriteria.YearOfExperience;
                            }
                            #endregion

                            #region InterestedIN
                            string tempInteresedIn = string.Empty;
                            if (SearchCriteria.InerestedIn.Contains(","))
                            {
                                string[] arrInterestedIn = Regex.Split(SearchCriteria.InerestedIn, ",");
                                if (arrInterestedIn.Count() > 1)
                                {
                                    foreach (string item in arrInterestedIn)
                                    {
                                        tempInteresedIn += "&facet.P=" + item;
                                    }
                                }
                                else
                                {
                                    tempInteresedIn = "&facet.P=";
                                }
                            }
                            else if (string.IsNullOrEmpty(SearchCriteria.InerestedIn))
                            {
                                tempInteresedIn = "&facet.P=";
                            }
                            else
                            {
                                tempInteresedIn = "&facet.P=" + SearchCriteria.InerestedIn;
                            }
                            #endregion

                            #region Company Size

                            string tempCompanySize = string.Empty;
                            if (SearchCriteria.CompanySize.Contains(","))
                            {
                                string[] arrIndustry = Regex.Split(SearchCriteria.CompanySize, ",");
                                if (arrIndustry.Count() > 1)
                                {
                                    foreach (string item in arrIndustry)
                                    {
                                        tempCompanySize += "&facet.CS=" + item;
                                    }
                                }
                                else
                                {
                                    tempCompanySize = "&facet.CS=";
                                }
                            }
                            else if (string.IsNullOrEmpty(SearchCriteria.CompanySize))
                            {
                                tempCompanySize = "&facet.CS=";
                            }
                            else
                            {
                                tempCompanySize = "&facet.CS=" + SearchCriteria.CompanySize;
                            }

                            #endregion

                        StartAgain:

                            string recruiterUrl = string.Empty; //
                            recruiterUrl = "https://www.linkedin.com/recruiter/api/search?keywords=" + Uri.EscapeDataString(SearchCriteria.Keyword) + "&page=" + i + "&start=" + startCount + "&count=25&countryCode=" + SearchCriteria.Country + "&postalCode=" + SearchCriteria.PostalCode + "&radiusMiles=" + SearchCriteria.within + "&jobTitle=" + SearchCriteria.Title + "&jobTitleTimeScope=" + SearchCriteria.TitleValue + "&company=" + SearchCriteria.Company + "&companyTimeScope=" + SearchCriteria.CompanyValue + "&firstName=" + SearchCriteria.FirstName + "&lastName=" + SearchCriteria.LastName + "&facet.TE=" + SearchCriteria.YearOfExperience + "&facet.CS=" + SearchCriteria.CompanySize + "&facet.L=" + SearchCriteria.language + "&facet.I=" + SearchCriteria.IndustryType + tempIndustry + "&facet.FG=" + SearchCriteria.Group + "&facet.N=" + SearchCriteria.Relationship + "&facet.FA=" + SearchCriteria.Function + "&facet.SE=" + SearchCriteria.SeniorLevel + "&facet.P=" + SearchCriteria.InerestedIn + "&facet.F=" + SearchCriteria.Fortune1000 + "&facet.DR=" + SearchCriteria.RecentlyJoined + "&origin=ASAS"; //&facet.I=" + SearchCriteria.IndustryType + "&facet.FG=" + SearchCriteria.Group + "&facet.N=" + SearchCriteria.Relationship + "&facet.FA=" + SearchCriteria.Function + "&facet.SE=" + SearchCriteria.SeniorLevel + "&facet.P=" + SearchCriteria.InerestedIn + "&facet.F=" + SearchCriteria.Fortune1000 + "&facet.DR=" + SearchCriteria.RecentlyJoined + "
                            //recruiterUrl = "https://www.linkedin.com/recruiter/api/search?keywords=" + Uri.EscapeDataString(SearchCriteria.Keyword) + "&page=" + i + "&start=" + startCount + "&count=25&countryCode=" + SearchCriteria.Country + "&postalCode=" + SearchCriteria.PostalCode + "&radiusMiles=" + SearchCriteria.within + "&jobTitle=" + SearchCriteria.Title + "&jobTitleTimeScope=" + SearchCriteria.TitleValue + "&company=" + SearchCriteria.Company + "&companyTimeScope=" + SearchCriteria.CompanyValue + "&firstName=" + SearchCriteria.FirstName + "&lastName=" + SearchCriteria.LastName + "" + tempExperience + "&facet.CS=" + SearchCriteria.CompanySize + "" + tempLanguage+ "" + tempIndustry + "&facet.FG=" + SearchCriteria.Group + "" + tempRelationShip + "" + tempFunction + "" + tempSeniorlevel + "" + tempInteresedIn + "&facet.F=" + SearchCriteria.Fortune1000 + "&facet.DR=" + SearchCriteria.RecentlyJoined + "&origin=ASAS"; //&facet.I=" + SearchCriteria.IndustryType + "&facet.FG=" + SearchCriteria.Group + "&facet.N=" + SearchCriteria.Relationship + "&facet.FA=" + SearchCriteria.Function + "&facet.SE=" + SearchCriteria.SeniorLevel + "&facet.P=" + SearchCriteria.InerestedIn + "&facet.F=" + SearchCriteria.Fortune1000 + "&facet.DR=" + SearchCriteria.RecentlyJoined + "

                        recruiterUrl = "https://www.linkedin.com/recruiter/api/search?&page=1&start=0&count=25&jobTitle=" + SearchCriteria.Title + "&jobTitleTimeScope=CP&companyTimeScope=CP" + tempIndustry + tempFunction + tempCompanySize + tempSeniorlevel + "&origin=ASAS";

                        string recruiterUrlForCount = "https://www.linkedin.com/recruiter/api/search?reset=countryCode&reset=jobTitle&reset=jobTitleTimeScope&reset=facet.I&reset=facet.FA&reset=facet.SE&reset=facet.ED&reset=facet.CS&reset=facet.L&reset=facet.P&reset=facet.DR&reset=facet.N&reset=keywords&count=0&decorateHits=false&decorateFacets=false&updateSearchHistory=false&resetFacets=true&searchHistoryId=1673044163&countryCode=" + SearchCriteria.Country + "&jobTitle=" + SearchCriteria.Title + "&jobTitleTimeScope=C" + tempIndustry + tempFunction + tempSeniorlevel + tempCompanySize + "&facet.L=" + SearchCriteria.language + "&facet.P=" + SearchCriteria.InerestedIn + "&facet.DR=" + SearchCriteria.RecentlyJoined + "&facet.N=" + SearchCriteria.Relationship + "&origin=ASAS";
                        //recruiterUrl = "https://www.linkedin.com/recruiter/api/search?keywords=" + Uri.EscapeDataString(SearchCriteria.Keyword) + "&page=" + i + "&start=" + startCount + "&count=25&countryCode=" + SearchCriteria.Country + "&postalCode=" + SearchCriteria.PostalCode + "&radiusMiles=" + SearchCriteria.within + "&jobTitle=" + SearchCriteria.Title + "&jobTitleTimeScope=" + SearchCriteria.TitleValue + "&company=" + SearchCriteria.Company + "&companyTimeScope=" + SearchCriteria.CompanyValue + "&firstName=" + SearchCriteria.FirstName + "&lastName=" + SearchCriteria.LastName + "&facet.TE=" + SearchCriteria.YearOfExperience + "&facet.CS=" + tempCompanySize + "&facet.L=" + SearchCriteria.language + "&facet.I=" + tempIndustry + "&facet.FG=" + SearchCriteria.Group + "&facet.N=" + SearchCriteria.Relationship + "&facet.FA=" + tempFunction + "&facet.SE=" + tempSeniorlevel + "&facet.P=" + SearchCriteria.InerestedIn + "&facet.F=" + SearchCriteria.Fortune1000 + "&facet.DR=" + SearchCriteria.RecentlyJoined + "&origin=ASAS";

                            //string PostDatarecruiterUrl = "{\"facetParams\":{\"facets\":[\"I\",\"SE\",\"CS\"],\"facetSelections\":[{\"facet\":\"I\",\"selections\":[\"4\",\"3\",\"109\",\"5\",\"118\"]},{\"facet\":\"SE\",\"selections\":[\"5\"]},{\"facet\":\"CS\",\"selections\":[\"4\",\"5\"]}]},\"locationParams\":{\"countryCode\":\"us\",\"postalCode\":[null]},\"jobAndCompanyParams\":{\"jobTitle\":\"latin america\"},\"metaParams\":{\"resetFacets\":true,\"reset\":[\"keywords\",\"countryCode\",\"postalCode\",\"jobTitle\",\"jobTitleTimeScope\",\"I\",\"SE\",\"CS\",\"company\",\"companyTimeScope\",\"notes\",\"projects\",\"reminders\"],\"origin\":\"ASDS\",\"doFacetDecoration\":false},\"pagingParams\":{\"count\":-1}}";
                            //recruiterUrl = "https://www.linkedin.com/recruiter/search";
                            //string GetResponse = HttpHelper.postFormDataRef(new Uri(recruiterUrl), PostDatarecruiterUrl, "https://www.linkedin.com/cap/dashboard/home?trk=nav_responsive_sub_nav_upgrade", "", "");

                            if (string.IsNullOrEmpty(SearchCriteria.Keyword))
                            {
                                recruiterUrl = recruiterUrl.Replace("keywords=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("keywords=", "");
                            }

                            if (string.IsNullOrEmpty(SearchCriteria.Country))
                            {
                                recruiterUrl = recruiterUrl.Replace("&countryCode=", "&countryCode=us");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&countryCode=", "").Replace("reset=countryCode&", "");
                            }

                            if (string.IsNullOrEmpty(SearchCriteria.PostalCode))
                            {
                                recruiterUrl = recruiterUrl.Replace("&postalCode=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&postalCode=", "");
                            }
                            if (string.IsNullOrEmpty(SearchCriteria.within))
                            {
                                recruiterUrl = recruiterUrl.Replace("&radiusMiles=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&radiusMiles=", "");
                            }

                            if (string.IsNullOrEmpty(SearchCriteria.Title))
                            {
                                recruiterUrl = recruiterUrl.Replace("&jobTitle=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&jobTitle=", "");
                            }
                            if (string.IsNullOrEmpty(SearchCriteria.TitleValue))
                            {
                                recruiterUrl = recruiterUrl.Replace("&jobTitleTimeScope=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&jobTitleTimeScope=", "");
                            }
                            if (string.IsNullOrEmpty(SearchCriteria.Company))
                            {
                                recruiterUrl = recruiterUrl.Replace("&company=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&company=", "");
                            }
                            if (string.IsNullOrEmpty(SearchCriteria.CompanyValue))
                            {
                                recruiterUrl = recruiterUrl.Replace("&companyTimeScope=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&companyTimeScope=", "");
                            }

                            if (string.IsNullOrEmpty(SearchCriteria.FirstName))
                            {
                                recruiterUrl = recruiterUrl.Replace("&firstName=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&firstName=", "");
                            }
                            if (string.IsNullOrEmpty(SearchCriteria.LastName))
                            {
                                recruiterUrl = recruiterUrl.Replace("&lastName=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&lastName=", "");
                            }
                            if (string.IsNullOrEmpty(SearchCriteria.YearOfExperience))
                            {
                                recruiterUrl = recruiterUrl.Replace("&facet.TE=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&facet.TE=", "");
                            }
                            if (string.IsNullOrEmpty(SearchCriteria.CompanySize))
                            {
                                recruiterUrl = recruiterUrl.Replace("&facet.CS=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&facet.CS=", "");
                            }
                            if (string.IsNullOrEmpty(SearchCriteria.language))
                            {
                                recruiterUrl = recruiterUrl.Replace("&facet.L=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&facet.L=", "");
                            }
                            if (string.IsNullOrEmpty(SearchCriteria.IndustryType))
                            {
                                recruiterUrl = recruiterUrl.Replace("&facet.I=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&facet.I=", "");
                            }
                            if (string.IsNullOrEmpty(SearchCriteria.SeniorLevel))
                            {
                                recruiterUrl = recruiterUrl.Replace("&facet.SE=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&facet.SE=", "");
                            }
                            if (string.IsNullOrEmpty(SearchCriteria.Function))
                            {
                                recruiterUrl = recruiterUrl.Replace("&facet.FA=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&facet.FA=", "");
                            }
                            if (string.IsNullOrEmpty(SearchCriteria.Relationship))
                            {
                                recruiterUrl = recruiterUrl.Replace("&facet.N=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&facet.N=", "");
                            }
                            if (string.IsNullOrEmpty(SearchCriteria.InerestedIn))
                            {
                                recruiterUrl = recruiterUrl.Replace("&facet.P=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&facet.P=", "");
                            }
                            if (string.IsNullOrEmpty(SearchCriteria.Fortune1000))
                            {
                                recruiterUrl = recruiterUrl.Replace("&facet.F=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&facet.F=", "");
                            }
                            if (string.IsNullOrEmpty(SearchCriteria.Group))
                            {
                                recruiterUrl = recruiterUrl.Replace("&facet.FG=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&facet.FG=", "");
                            }
                            if (string.IsNullOrEmpty(SearchCriteria.RecentlyJoined))
                            {
                                recruiterUrl = recruiterUrl.Replace("&facet.DR=", "");
                                recruiterUrlForCount = recruiterUrlForCount.Replace("&facet.DR=", "");
                            }

                            recruiterUrl = recruiterUrl.Trim();

                            referralUrl = "https://www.linkedin.com/recruiter/search?searchHistoryId=1256614203&searchCacheKey=226602fa-2763-48a7-a2ff-dfddc87c4840%2CIwQE&linkContext=Controller%3ApeopleSearch%2CAction%3AresultsWithFacets%2CID%3A1256614203&page=1&start=0&count=25";
                            string pageSourceaAdvanceSearch11 = string.Empty;

                            try
                            {
                                pageSourceaAdvanceSearch11 = HttpHelper.getHtmlfromUrlNewRefre(new Uri(recruiterUrl), referralUrl);
                            }
                            catch { }
                            string strTotalPageNO = string.Empty;
                            
                            if (IsShowLoggerPagecount)
                            {
                                strTotalPageNO = Utils.getBetween(pageSourceaAdvanceSearch11, "total\":", ",").Trim();                                

                                string pageSourceaAdvanceSearchForCount = HttpHelper.getHtmlfromUrlNewRefre(new Uri(recruiterUrlForCount), referralUrl);

                                strTotalPageNO = Utils.getBetween(pageSourceaAdvanceSearchForCount, "total\":", ",").Trim();

                                
                                try
                                {
                                    pagenumberrecruiter = int.Parse(strTotalPageNO);


                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Total Results :  " + pagenumberrecruiter + " ]");

                                }
                                catch (Exception)
                                {

                                }

                                pagenumberrecruiter = (pagenumberrecruiter / 25) + 1;

                                if (pagenumberrecruiter == -1)
                                {
                                    pagenumberrecruiter = 2;
                                }

                                if (pagenumberrecruiter == 1)
                                {
                                    pagenumberrecruiter = 2;
                                }

                                //if (IsShowLoggerPagecount)
                                {
                                    if (pagenumberrecruiter >= 1)
                                    {
                                        _HttpHelper = HttpHelper;

                                        if (!GlobalsScraper.scrapeWithoutGoingToMainProfile)
                                        {
                                            new Thread(() =>
                                            {
                                                if (SearchCriteria.starter)
                                                {
                                                    string CheckString = string.Empty;
                                                    finalUrlCollectionForRecruter(CheckString);
                                                }

                                            }).Start();
                                        }
                                    }
                                }
                            }
                            IsShowLoggerPagecount = false;
                            while (i <= pagenumberrecruiter)
                            {
                                if (SearchCriteria.starter)
                                {


                                    if (pageSourceaAdvanceSearch11.Contains("memberId"))
                                    {
                                        try
                                        {
                                            List<string> PageSerchUrl = GettingAllUrlRecruiterType(pageSourceaAdvanceSearch11);
                                            PageSerchUrl.Distinct();


                                            if (PageSerchUrl.Count == 0)
                                            {
                                                GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ On the basis of your Account you can able to see " + RecordURL.Count + " Results ]");
                                                break;
                                            }

                                            foreach (string item in PageSerchUrl)
                                            {
                                                if (SearchCriteria.starter)
                                                {
                                                    if (item.Contains("recruiter/profile"))
                                                    {
                                                        try
                                                        {
                                                            string urlSerch = item;
                                                            if (urlSerch.Contains("recruiter/profile"))
                                                            {
                                                                if (GlobalsScraper.limitToScrape == 123456789)
                                                                {
                                                                    RecordURL.Add(urlSerch);
                                                                    if (!queRecordUrl.Contains(urlSerch))
                                                                    {
                                                                        queRecordUrl.Enqueue(urlSerch);
                                                                    }
                                                                    RecordURL = RecordURL.Distinct().ToList();
                                                                }
                                                                else
                                                                {
                                                                    if (GlobalsScraper.limitToScrape > Convert.ToInt32(strTotalPageNO))
                                                                    {
                                                                        GlobalsScraper.limitToScrape = Convert.ToInt32(strTotalPageNO);
                                                                    }
                                                                    if (GlobalsScraper.limitToScrape == RecordURL.Count)
                                                                    {
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        RecordURL.Add(urlSerch);
                                                                        if (!queRecordUrl.Contains(urlSerch))
                                                                        {
                                                                            queRecordUrl.Enqueue(urlSerch);
                                                                        }
                                                                        RecordURL = RecordURL.Distinct().ToList();

                                                                    }
                                                                }                                                                
                                                            }

                                                            try
                                                            {
                                                                GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ " + urlSerch + " ]");
                                                            }
                                                            catch { }
                                                        }
                                                        catch { }
                                                    }
                                                }
                                            }

                                            if (i == pagenumberrecruiter)
                                            {
                                                break;
                                            }

                                            startCount += 25; ;
                                            i++;
                                            Thread.Sleep(4000);
                                            goto StartAgain;
                                        }
                                        catch { }
                                    }

                                }
                            }


                        }
                        catch { }

                    }
                    return;
                   
                }

                
                if (SearchCriteria.starter)
                {
                    #region Serch
                  
                    string pageSourceaAdvanceSearch = HttpHelper.getHtmlfromUrl1(new Uri("https://www.linkedin.com/search"));
                    NewSearchPage = string.Empty;

                    if (pageSourceaAdvanceSearch.Contains("csrfToken"))
                    {
                        try
                        {
                            int startindex = pageSourceaAdvanceSearch.IndexOf("csrfToken");
                            if (startindex > 0)
                            {
                                string start = pageSourceaAdvanceSearch.Substring(startindex);
                                int endindex = start.IndexOf(">");
                                string end = start.Substring(0, endindex);
                                csrfToken = end.Replace("csrfToken=", "").Replace("\\", "").Replace("\"", string.Empty); ;
                            }
                        }
                        catch { }

                    }

                 
                                   
                    try
                    {
                        if (SearchCriteria.Location == "Y")
                        {
                            SearchCriteria.Country = string.Empty;
                        }
                       
                        {

                            string GetDataForPrimiumAccount = string.Empty;
                           
                            string pageSourceaAdvanceSearch_New = HttpHelper.getHtmlfromUrl1(new Uri("https://www.linkedin.com/search"));
                            string rsid = Utils.getBetween(pageSourceaAdvanceSearch_New, "/vsearch/g?rsid=", "&trk=");
                           



                            #endregion
                           
                            #region final Post data written by sharan
                            
                           // GetDataForPrimiumAccount = "https://www.linkedin.com/vsearch/p?f_N=" + SearchCriteria.Relationship +"&f_G="+ SearchCriteria.LocationArea+ "&f_CC=&f_I="+SearchCriteria.IndustryType+"&f_L=" + SearchCriteria.language + "&f_LF=&f_SE="+SearchCriteria.SeniorLevel+"&f_P="+SearchCriteria.InerestedIn+"&f_CS=" + SearchCriteria.CompanySize + "&f_F="+SearchCriteria.Fortune1000+"&trk=federated_advs&openFacets=N,G,CC,I,PC,ED,L,LF&rsid="+rsid+"&titleScope="+SearchCriteria.TitleValue+"&companyScope="+SearchCriteria.CompanyValue+"&locationType="+SearchCriteria.Location+"&orig=FCTD&keywords="+SearchCriteria.Keyword+"&firstName="+SearchCriteria.FirstName+"&lastName="+SearchCriteria.LastName+"&title="+Uri.EscapeDataString(SearchCriteria.Title)+"&company="+SearchCriteria.Company+"&school=&openAdvancedForm=true&countryCode="+SearchCriteria.Country+"&distance="+SearchCriteria.within+"&postalCode="+SearchCriteria.PostalCode;
                            //  GetDataForPrimiumAccount = "https://www.linkedin.com/vsearch/p?f_N=" + SearchCriteria.Relationship + "&f_G=" + SearchCriteria.LocationArea + "&f_CC=&f_I=" + SearchCriteria.IndustryType + "&f_L=" + SearchCriteria.language + "&f_LF=&f_SE=" + SearchCriteria.SeniorLevel + "&f_P=" + SearchCriteria.InerestedIn + "&f_CS=" + SearchCriteria.CompanySize + "&f_F=" + SearchCriteria.Fortune1000 + "&trk=federated_advs&openFacets=N,G,CC,I,PC,ED,L,LF&rsid=" + rsid + "&titleScope=" + SearchCriteria.TitleValue + "&companyScope=" + SearchCriteria.CompanyValue + "&locationType=" + SearchCriteria.Location + "&orig=FCTD&keywords=" + SearchCriteria.Keyword + "&firstName=" + SearchCriteria.FirstName + "&lastName=" + SearchCriteria.LastName + "&title=" + Uri.EscapeDataString(SearchCriteria.Title) + "&company=" + SearchCriteria.Company + "&school=&openAdvancedForm=true&countryCode=" + SearchCriteria.Country + "&distance=" + SearchCriteria.within + "&postalCode=" + SearchCriteria.PostalCode;  //  works fine without selecting the groups

                            if (GlobalsScraper.isSearchByUrl)
                            {
                                try
                                {
                                    if (GlobalsScraper.lst_urls_for_linkedin_scraper.Count > 0)
                                    {
                                        GetDataForPrimiumAccount = GlobalsScraper.lst_urls_for_linkedin_scraper[0];
                                    }
                                }
                                catch
                                { }
                            }

                        
                            else
                            {

                                GetDataForPrimiumAccount = "https://www.linkedin.com/vsearch/p?f_N=" + Relationship + "&f_G=" + SearchCriteria.LocationArea + "&f_CC=&f_I=" + IndustryType + "&f_L=" + language + "&f_LF=&f_SE=" + SeniorLevel + "&f_P=" + InerestedIn + "&f_CS=" + CompanySize + "&f_F=" + Fortune1000 + "&trk=federated_advs&openFacets=N,G,CC,I,PC,ED,L,LF&rsid=" + rsid + "&titleScope=" + TitleValue + "&companyScope=" + CompanyScope + "&locationType=" + LocationType + "&orig=FCTD&keywords=" + Keyword + "&firstName=" + FirstName + "&lastName=" + LastName + "&title=" + Uri.EscapeDataString(TitleValue) + "&company=" + Uri.EscapeDataString(CompanyValue) + "&school=&openAdvancedForm=true&countryCode=" + Country + "&distance=" + Within + "&postalCode=" + postalCode + "&f_FG=" + SearchCriteria.Group_id;

                                //GetDataForPrimiumAccount = "https://www.linkedin.com/vsearch/p?f_N=" + SearchCriteria.Relationship + "&f_G=" + SearchCriteria.LocationArea + "&f_CC=&f_I=" + SearchCriteria.IndustryType + "&f_L=" + SearchCriteria.language + "&f_LF=&f_SE=" + SearchCriteria.SeniorLevel + "&f_P=" + SearchCriteria.InerestedIn + "&f_CS=" + SearchCriteria.CompanySize + "&f_F=" + SearchCriteria.Fortune1000 + "&trk=federated_advs&openFacets=N,G,CC,I,PC,ED,L,LF&rsid=" + rsid + "&titleScope=" + SearchCriteria.TitleValue + "&companyScope=" + SearchCriteria.CompanyValue + "&locationType=" + SearchCriteria.Location + "&orig=FCTD&keywords=" + SearchCriteria.Keyword + "&firstName=" + SearchCriteria.FirstName + "&lastName=" + SearchCriteria.LastName + "&title=" + Uri.EscapeDataString(SearchCriteria.Title) + "&company=" + Uri.EscapeDataString(SearchCriteria.Company) + "&school=&openAdvancedForm=true&countryCode=" + SearchCriteria.Country + "&distance=" + SearchCriteria.within + "&postalCode=" + SearchCriteria.PostalCode + "&f_FG=" + SearchCriteria.Group_id;
                               
                            }
                            #endregion


                            if (true)
                            {
                            }

                            
                            //   GetDataForPrimiumAccount = "https://www.linkedin.com/vsearch/p?title=%20finance%20OR%20financial%20OR%20fiscal%20OR%20%22human%20resources%22%20OR%20%22human%20resource%22%20OR%20HR%20OR%20benefits%20OR%20rewards%20OR%20reward%20OR%20comp%20OR%20compensation%20OR%20controller%20OR%20CEO%20OR%20CFO%20OR%20COO%20OR%20treasurer%20OR%20treasury%20OR%20Bookeeper%20OR%20401k%20OR%20401(kiss)%20OR%20%22retirement%20plan%22%20OR%20%22Practice%22%20OR%20%22Firm%22%20OR%20%22Business%20Manager%22%20OR%20accounting%20OR%20accountant%20OR%20officer%20OR%20chief%20OR%20head%20OR%20VP%20OR%20vice%20president%20OR%20president%20OR%20owner%20OR%20partner%20OR%20principal%20OR%20executive%20OR%20director%20OR%20EVP%20OR%20AVP%20OR%20counsel%20OR%20administration&company=%20%22American%20Eagle%20Outfitters%22%20OR%20%22Iron%20Workers%20Of%20Western%22%20OR%20%22Bd%20Of%20Trust%20Steamfitters%20Local%20449%22%20OR%20%22Boilermakers%20Lodge%20154%20Retirement%22%20OR%20%22Pitt%20Ohio%20Express%22%20OR%20%22Eckert%20Seamans%20Cherin%20%26%20Mellott%22%20OR%20%22Three%20Rivers%22%20OR%20%22Ansaldo%20Sts%22%20OR%20%22Trumbull%22%20OR%20%22General%20Nutrition%20Centers%22%20OR%20%22H%20J%20Heinz%22%20OR%20%22Local%20354%20Plumbers%20%26%20Pipe%20Fitters%22%20OR%20%22Matthews%20International%22%20OR%20%22Cohen%20%26%20Grigsby%22%20OR%20%22Dollar%20Bank%20Federal%20Savings%20Bank%22%20OR%20%22Duquesne%20Light%22%20OR%20%22Sms%20Siemag%22%20OR%20%22Plumbers%20Local%2027%22%20OR%20%22Duquesne%20University%20Of%20Holy%20Spirit%22%20OR%20%22Dck%20Worldwide%22%20OR%20%22Carnegie%20Institute%22%20OR%20%22Plumbers%20%26%20Steamfitters%20Local%2047%20Of%20Northwest%22%20OR%20%22Point%20Park%20University%22%20OR%20%22Dickie%20Mccamey%20%26%20Chilcote%22%20OR%20%22Calgon%20Carbon%22%20OR%20%22Carmeuse%20Lime%22%20OR%20%22K%26L%20Gates%22%20OR%20%22Med3000%20Group%22%20OR%20%22Carnegie%20Library%20Of%20Pittsburgh%22%20OR%20%22Arcelormittal%20Tubular%20Products%22%20OR%20%22Ibew%20Local%20712%22%20OR%20%22Hudson%20Highland%20Group%22%20OR%20%22Schneider%20Downs%20%26%22%20OR%20%22Russell%2FMellon%20Analytical%20Services%22%20OR%20%22Png%20Companies%22%20OR%20%22Limbach%20Facility%20Services%22%20OR%20%22Civil%20%26%20Environmental%20Consultants%22%20OR%20%22Hefren%20Tillotson%22%20OR%20%22Iu%20T%20Of%20Western%22%20OR%20%22Pg%20Publishing%22%20OR%20%22Horsehead%22%20OR%20%22Tangent%20Rail%20%26%20Subsidiaries%22%20OR%20%22Fenner%20Dunlop%22%20OR%20%22System%20One%22%20OR%20%22Institute%20For%20Transfusion%20Medicine%22%20OR%20%22Carlow%20University%22%20OR%20%22Ampco%20Pittsburgh%22%20OR%20%22Sauer%22%20OR%20%22Meyer%20Unkovic%20%26%20Scott%22%20OR%20%22Preferred%20Primary%20Care%20Physicians%22&postalCode=15222&openAdvancedForm=true&titleScope=C&companyScope=C&locationType=I&countryCode=us&distance=35&rsid=4204379461431621350077&orig=MDYS";

                            ResponseWallPostForPremiumAcc = HttpHelper.getHtmlfromUrl1(new Uri(GetDataForPrimiumAccount));
                            string new_url=string.Empty;
                            if (ResponseWallPostForPremiumAcc.Contains("i18n_show_all_results"))
                            {
                                new_url = Utils.getBetween(ResponseWallPostForPremiumAcc, "i18n_show_all_results", "{");
                                new_url = Utils.getBetween(new_url, "escapeHatchUrl\":\"", "\"}");
                                new_url = "https://www.linkedin.com" + new_url;
                                if (new_url.Contains("\\u002d"))
                                {
                                    new_url = new_url.Replace("\\u002d", "-");
                                }
                                if (new_url.Contains("\u002d"))
                                {
                                    new_url = new_url.Replace("\u002d", "-");
                                }
                            }
                            if (!string.IsNullOrEmpty(new_url))
                            {
                                ResponseWallPostForPremiumAcc = HttpHelper.getHtmlfromUrl1(new Uri(new_url));
                            }
                            nextPageUrl =Utils.getBetween(ResponseWallPostForPremiumAcc, "page_number_i18n", "\",\"pageNum");
                            List<string> PageSerchUrl = GettingAllUrl(ResponseWallPostForPremiumAcc);
                            PageSerchUrl.Distinct();

                            if (PageSerchUrl.Count == 0)
                            {
                                //Log("[ " + DateTime.Now + " ] => [ On the basis of your Account you can able to see " + RecordURL.Count + " Results ]");
                                //break;
                            }

                            foreach (string item in PageSerchUrl)
                            {
                                if (SearchCriteria.starter)
                                {
                                    if (item.Contains("pp_profile_photo_link") || item.Contains("vsrp_people_res_name") || item.Contains("profile/view?"))
                                    {
                                        try
                                        {
                                            string urlSerch = item;
                                            if (urlSerch.Contains("vsrp_people_res_name"))
                                            {
                                                RecordURL.Add(urlSerch);
                                                if (!queRecordUrl.Contains(urlSerch))
                                                {
                                                    queRecordUrl.Enqueue(urlSerch);
                                                }
                                                RecordURL = RecordURL.Distinct().ToList();
                                            }

                                            try
                                            {
                                                GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ " + urlSerch + " ]");
                                            }
                                            catch { }
                                        }
                                        catch { }
                                    }
                                }
                            }

                        }

                    }
                    catch { }
                    
                    string facetsOrder = string.Empty;
                    if (PostResponce.Contains("facetsOrder"))
                    {
                        facetsOrder = ResponseWallPostForPremiumAcc.Substring(PostResponce.IndexOf("facetsOrder"), 200);
                        string[] Arr3 = facetsOrder.Split('"');
                        facetsOrder = Arr3[2];
                        string DecodedCharTest = Uri.UnescapeDataString(facetsOrder);
                        string DecodedEmail = Uri.EscapeDataString(facetsOrder);
                        facetsOrder = DecodedEmail;
                    }
                   #endregion
                }
                int pagenumber = 0;
                string strPageNumber = string.Empty;
                string[] Arr12 = Regex.Split(ResponseWallPostForPremiumAcc, "<li");
                foreach (string item in Arr12)
                {
                    if (SearchCriteria.starter)
                    {
                        #region Loop
                        if (!item.Contains("<!DOCTYPE"))
                        {
                            if (item.Contains("results-summary"))
                            {
                                string data = RemoveAllHtmlTag.StripHTML(item);
                                data = data.Replace("\n", "");
                                if (data.Contains(">"))
                                {
                                    string[] ArrTemp = data.Split('>');
                                    data = ArrTemp[1];
                                    data = data.Replace("results", "");
                                    data = data.Trim();
                                    string[] ArrTemp1 = data.Split(' ');
                                    data = ArrTemp1[0].Replace(',', ' ').Trim();
                                    strPageNumber = data.Replace(" ", string.Empty);
                                    break;
                                }

                            }
                        }
                        #endregion
                    }
                }

                if (string.IsNullOrEmpty(strPageNumber))
                {
                    try
                    {
                        if (ResponseWallPostForPremiumAcc.Contains("resultCount"))
                        {
                            string[] countResultArr = Regex.Split(ResponseWallPostForPremiumAcc, "resultCount");

                            if (countResultArr.Length > 1)
                            {
                                string tempResult = countResultArr[1].Substring(0, countResultArr[1].IndexOf(","));

                                #region Commented
                                //Regex IdCheck = new Regex("^[0-9]*$");

                                //string[] tempResultArr = Regex.Split(tempResult, "[^0-9]");

                                //foreach (string item in tempResultArr)
                                //{
                                //    try
                                //    {
                                //        if(IdCheck.IsMatch(item))
                                //        {
                                //            strPageNumber = item;

                                //            break;
                                //        }
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //    }
                                //} 
                                #endregion

                                if (tempResult.Contains("<strong>"))
                                {
                                    strPageNumber = tempResult.Substring(tempResult.IndexOf("<strong>"), tempResult.IndexOf("</strong>", tempResult.IndexOf("<strong>")) - tempResult.IndexOf("<strong>")).Replace("<strong>", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Trim();
                                }
                                else if (tempResult.Contains(":"))
                                {
                                    strPageNumber = tempResult.Replace(":", string.Empty).Replace("\"", string.Empty);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }

                if (string.IsNullOrEmpty(strPageNumber))
                {
                    try
                    {
                        if (ResponseWallPostForPremiumAcc.Contains("results_count_without_keywords_i18n"))
                        {
                            string[] countResultArr = Regex.Split(ResponseWallPostForPremiumAcc, "results_count_without_keywords_i18n");

                            if (countResultArr.Length > 1)
                            {
                                string tempResult = countResultArr[1].Substring(0, countResultArr[1].IndexOf(","));
                             
                                if (tempResult.Contains("<strong>"))
                                {
                                    strPageNumber = tempResult.Substring(tempResult.IndexOf("<strong>"), tempResult.IndexOf("</strong>", tempResult.IndexOf("<strong>")) - tempResult.IndexOf("<strong>")).Replace("<strong>", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Trim();
                                }
                                else if (tempResult.Contains(":"))
                                {
                                    strPageNumber = tempResult.Replace(":", string.Empty).Replace("\"", string.Empty);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }

                string logtag = string.Empty;

                try
                {
                    pagenumber = int.Parse(strPageNumber);

                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Total Results :  " + pagenumber + " ]");
                }
                catch (Exception)
                {

                }

                pagenumber = (pagenumber / 10) + 1;

                if (pagenumber == -1)
                {
                    pagenumber = 2;
                }

                if (pagenumber == 1)
                {
                    pagenumber = 2;
                }


                //if (GlobalsScraper.limitToScrape != 123456789 )
                //{
                //    int total = (GlobalsScraper.limitToScrape / 10) + 1;
                //    pagenumber = total;

                //}

                string GetResponce = string.Empty;
                string GetRequestURL = string.Empty;

                if (pagenumber >= 1)
                {
                    _HttpHelper = HttpHelper;

                    #region comment it for ashley

                    if (!GlobalsScraper.scrapeWithoutGoingToMainProfile)
                    {
                        new Thread(() =>
                        {
                            if (SearchCriteria.starter)
                            {
                                string CheckString = string.Empty;
                                finalUrlCollection(CheckString);
                            }

                        }).Start();
                    }

                    #endregion
                    if (GlobalsScraper.scrapeWithoutGoingToMainProfile)
                    {
                        CrawlingProfileDataFromSearchPage(ResponseWallPostForPremiumAcc, ref HttpHelper);
 
                    }


                    PostdataForPagination = "https://www.linkedin.com" + Utils.getBetween(nextPageUrl + "&&&&#######", "pageURL\":\"", "&&&&#######");
                    if (PostdataForPagination.Contains("\\u002d"))
                    {
                        PostdataForPagination = PostdataForPagination.Replace("\\u002d", "-");
                    }
                    if (PostdataForPagination.Contains("\u002d"))
                    {
                        PostdataForPagination = PostdataForPagination.Replace("\u002d", "-");
                    }




                    for (int i = 2; i <= pagenumber; i++)
                    {
                        string new_PostdataForPagination = string.Empty;
                        //if (queRecordUrl.Count() >= 300)
                        //{
                        //    break;
                        //}
                        if (SearchCriteria.starter)
                        {
                            #region loop

                            if (i != 1)
                            {
                                int countPage = i;

                               // nextPageUrl = getBetween(PostResponce, "page_number_i18n", "\",\"pageNum");
                                PostdataForPagination = (Utils.getBetween("@*@*@*@**@" + PostdataForPagination, "@*@*@*@**@", "&page_num=") + "&page_num=" + countPage);
                                new_PostdataForPagination = PostdataForPagination;
                                PostResponce = HttpHelper.getHtmlfromUrl(new Uri(new_PostdataForPagination));
                            }
                            else
                            {
                                new_PostdataForPagination = PostdataForPagination;
                                PostResponce = HttpHelper.getHtmlfromUrl(new Uri(new_PostdataForPagination));
                            }

                            if (ResponseWallPostForPremiumAcc.Contains("Account Type:</span> Basic"))
                            {
                                try
                                {
                                    PostRequestURL = "https://www.linkedin.com/search/hits";
                                    //PostdataForPagination = "keywords=" + Uri.EscapeDataString(SearchCriteria.Keyword) + "&title=" + Uri.EscapeDataString(SearchCriteria.Title) + "&fname=" + SearchCriteria.FirstName + "&lname=" + SearchCriteria.LastName + "&searchLocationType=" + SearchCriteria.Location + "&f_FG=" + SearchCriteria.Group + "&companyScope=" + SearchCriteria.CompanyValue + "&countryCode=" + SearchCriteria.Country + "&company=" + SearchCriteria.Company + "&viewCriteria=1&sortCriteria=R&facetsOrder=CC%2CN%2CG%2CI%2CPC%2CED%2CL%2CFG%2CTE%2CFA%2CSE%2CP%2CCS%2CF%2CDR&page_num=" + i + "&openFacets=N%2CCC%2CG";
                                   
                                    PostResponce = HttpHelper.postFormData(new Uri(PostRequestURL), PostdataForPagination);
                                    string  my_Response=HttpHelper.getHtmlfromUrl(new Uri(PostdataForPagination));
                                    
                                }
                                catch { }
                            }
                            else if (ResponseWallPostForPremiumAcc.Contains("Account Type:</span> Executive"))
                            {
                                try
                                {
                                    PostRequestURL = "https://www.linkedin.com/search/hits";
                                   // PostdataForPagination = "keywords=" + Uri.EscapeDataString(SearchCriteria.Keyword) + "&title=" + Uri.EscapeDataString(SearchCriteria.Title) + "&fname=" + SearchCriteria.FirstName + "&lname=" + SearchCriteria.LastName + "&searchLocationType=" + SearchCriteria.Location + "&f_FG=" + SearchCriteria.Group + "&companyScope=" + SearchCriteria.CompanyValue + "&countryCode=" + SearchCriteria.Country + "&keepFacets=keepFacets&I=" + SearchCriteria.IndustryType + "&SE=" + SearchCriteria.SeniorLevel + "&pplSearchOrigin=ADVS&viewCriteria=2&sortCriteria=R&facetsOrder=N%2CCC%2CI%2CPC%2CED%2CL%2CFG%2CTE%2CFA%2CSE%2CP%2CCS%2CF%2CDR%2CG&page_num=" + i + "&openFacets=N%2CCC%2CI";

                                    PostResponce = HttpHelper.postFormData(new Uri(PostRequestURL), PostdataForPagination);
                                    //Temporosy code for client
                                    //GlobusFileHelper.AppendStringToTextfileNewLine("DateTime :- " + DateTime.Now + " :: Pagesource 3 >>>> " + PostResponce, Globals.Path_LinkedinScrapperPagesource);
                                }
                                catch { }
                            }
                            else if (ResponseWallPostForPremiumAcc.Contains("openAdvancedForm=true"))
                            {
                                PostRequestURL = "https://www.linkedin.com/vsearch/p?";
                                if (string.IsNullOrEmpty(PostResponce))
                                {
                                    string my_Response = HttpHelper.getHtmlfromUrl(new Uri(new_PostdataForPagination));
                                    PostResponce = my_Response;
                                }

                                //Temporosy code for client
                                
                                GlobusFileHelper.AppendStringToTextfileNewLine("DateTime :- " + DateTime.Now + " :: Pagesource 4 >>>> " + PostResponce, FilePath.Path_LinkedinScrapperPagesource);
                            }
                            else
                            {
                               
                            }

                            if (PostResponce.Contains("/profile/view?id"))
                            {
                                if (GlobalsScraper.scrapeWithoutGoingToMainProfile)
                                {
                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Crawling search result page number : " + i + " ]");
                                        CrawlingProfileDataFromSearchPage(PostResponce, ref HttpHelper);
                                }
                                else
                                {
                                    List<string> PageSerchUrl = GettingAllUrl(PostResponce);
                                    PageSerchUrl.Distinct();

                                    if (PageSerchUrl.Count == 0)
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ On the basis of your Account you can able to see " + RecordURL.Count + " Results ]");
                                        break;
                                    }

                                    foreach (string item in PageSerchUrl)
                                    {
                                        if (SearchCriteria.starter)
                                        {
                                            if (item.Contains("pp_profile_photo_link") || item.Contains("vsrp_people_res_name") || item.Contains("profile/view?"))
                                            {
                                                try
                                                {
                                                    string urlSerch = item;
                                                    if (urlSerch.Contains("vsrp_people_res_name"))
                                                    {
                                                        RecordURL.Add(urlSerch);
                                                        if (!queRecordUrl.Contains(urlSerch))
                                                        {
                                                            queRecordUrl.Enqueue(urlSerch);
                                                        }
                                                        RecordURL = RecordURL.Distinct().ToList();
                                                    }

                                                    try
                                                    {
                                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ " + urlSerch + " ]");
                                                    }
                                                    catch { }
                                                }
                                                catch { }
                                            }
                                        }
                                    }
                                }
                            }

                            else if (!PostResponce.Contains("pp_profile_name_link") && PostResponce.Contains("Custom views are no longer supported. Please select either Basic or Expanded view"))
                            {
                                break;
                            }

                            #endregion
                        }
                    }
                    #region remove comment for ashley
                    //if (!Globals.scrapeWithoutGoingToMainProfile)
                    //{
                    //    new Thread(() =>
                    //    {
                    //        if (SearchCriteria.starter)
                    //        {
                    //            string CheckString = string.Empty;
                    //            finalUrlCollection(CheckString);
                    //        }

                    //    }).Start();
                    //}
                    #endregion
                }
                #region For Else
                else
                {
                    if (!GlobalsScraper.scrapeWithoutGoingToMainProfile)
                    {
                        if (SearchCriteria.starter)
                        {

                            #region loop
                            if (ResponseWallPostForPremiumAcc.Contains("/profile/view?id"))
                            {

                                List<string> PageSerchUrl = ChilkatBasedRegex.GettingAllUrls(ResponseWallPostForPremiumAcc, "profile/view?id");
                                if (PageSerchUrl.Count == 0)
                                {

                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ On the basis of your Account or Your Input you can able to see " + RecordURL.Count + "  Results ]");

                                }

                                foreach (string item in PageSerchUrl)
                                {
                                    if (SearchCriteria.starter)
                                    {
                                        if (item.Contains("pp_profile_name_link"))
                                        {
                                            string urlSerch = "http://www.linkedin.com" + item;
                                            GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ " + urlSerch + " ]");
                                            RecordURL.Add(urlSerch);
                                            queRecordUrl.Enqueue(urlSerch);

                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }

                }

                if (strPageNumber != string.Empty)
                {
                    if (strPageNumber != "0")
                    {
                        GlobusLogHelper.log.Info("-------------------------------------------------------------------------------------------------------------------------------");
                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ No Of Results Found >>> " + strPageNumber + " ]");
                    }
                }

                RecordURL.Distinct();
                //if (SearchCriteria.starter)
                //{
                //    finalUrlCollection(ref HttpHelper);
                //    //new Thread(() =>
                //    //{
                //    //    test();
                //    //}).Start();
                //}

            }

           #endregion

            catch(Exception ex)
            {
                Console.Write("Exc : " + ex);
            }
            //#endregion
        }


        #region CrawlingProfileDataFromSearchPage
        public void CrawlingProfileDataFromSearchPage(string searchResultPageSource, ref GlobusHttpHelper HttpHelper)
        {


            try
            {
                string[] person_split = Regex.Split(searchResultPageSource, "{\"person\":");
                foreach (string personData in person_split)
                {
                    try
                    {
                        string firstName = string.Empty;
                        string lastName = string.Empty;
                        string headlineTitle = string.Empty;
                        string industry = string.Empty;
                        string location = string.Empty;
                        string currentTitle = string.Empty;
                        string pastTitle = string.Empty;
                        string profileUrl = string.Empty;
                        string degreeOfConnection = string.Empty;

                        if (!personData.Contains("<!DOCTYPE html>"))
                        {
                            try
                            {
                                firstName = Utils.getBetween(personData, "\"firstName\":\"", "\",\"").Replace("&amp;", "&").Replace("\\u002d", "-");
                                lastName = Utils.getBetween(personData, "\"lastName\":\"", "\",\"");
                                headlineTitle = Utils.getBetween(personData, "\"fmt_headline\":\"", "\",\"").Replace("\\u002d", "-").Replace("\\u003cstrong class=\\\"highlight\\\"\\u003e", string.Empty).Replace("\\u003c/strong\\u003e", string.Empty).Replace("\",\"", string.Empty).Replace("&amp;", "&").Replace("\\u002d", "-").Trim();
                                location = Utils.getBetween(personData, "\"fmt_location\":\"", "\",\"").Replace("&amp;", "&").Replace("\\u002d", "-");
                                industry = Utils.getBetween(personData, "\"fmt_industry\":\"", "\",\"").Replace("&amp;", "&").Replace("\\u002d", "-");
                                profileUrl = Utils.getBetween(personData, "link_nprofile_view_3\":", "&srchindex=");
                                if (string.IsNullOrEmpty(profileUrl))
                                {
                                    try
                                    {
                                        profileUrl = Utils.getBetween(personData, "link_nprofile_view_headless\":\"", "%2");
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            catch { };

                            if (personData.Contains("[{\"fieldName\":\"Current\",\""))
                            {
                                string currentTitleScrape = Utils.getBetween(personData, "[{\"fieldName\":\"Current\",\"", "],");
                                currentTitle = Utils.getBetween(currentTitleScrape, "\"fmt_heading\":\"", "\"}").Replace("\\u003cstrong class=\\\"highlight\\\"\\u003e", string.Empty).Replace("\\u003c/strong\\u003e", string.Empty).Replace("\\u003cB", string.Empty).Replace("\\u003e", string.Empty).Replace("\\u003c", string.Empty).Replace("/B", string.Empty).Replace("\",\"fmt_body\":\"", ".").Replace("\",\"", ", ").Replace("&amp;", "&").Replace("\\u002d", "-").Trim();
                                if (string.IsNullOrEmpty(currentTitle))
                                {
                                    try
                                    {
                                        currentTitle = Utils.getBetween(currentTitleScrape, "bodyList\":[\"", "\"]}").Replace("\\u003cstrong class=\\\"highlight\\\"\\u003e", string.Empty).Replace("\\u003c/strong\\u003e", string.Empty).Replace("\\u003cB", string.Empty).Replace("\\u003e", string.Empty).Replace("\\u003c", string.Empty).Replace("/B", string.Empty).Replace("\",\"fmt_body\":\"", ".").Replace("\",\"", ", ").Replace("&amp;", "&").Replace("\\u002d", "-").Trim();
                                    }
                                    catch { };
                                }
                            }
                            if (personData.Contains("\"fieldName\":\"Past\",\"bodyList\":"))
                            {
                                try
                                {
                                    string pastTitleScrape = Utils.getBetween(personData, "\"fieldName\":\"Past\",\"bodyList\":", "}]");
                                    pastTitle = Utils.getBetween(pastTitleScrape, "[\"", "\"]").Replace("\\u003cstrong class=\\\"highlight\\\"\\u003e", string.Empty).Replace("\\u003c/strong\\u003e", string.Empty).Replace("\",\"", ", ").Replace("&amp;", "&").Replace("\\u002d", "-").Trim();
                                }
                                catch { };
                            }
                            try
                            {
                                string degreeConnectionScrape =Utils.getBetween(personData, "degree_contact_key\":\"", "\",\"");
                                if (degreeConnectionScrape.Contains(" 1st "))
                                {
                                    degreeOfConnection = "1st";
                                }
                                else if (degreeConnectionScrape.Contains(" 2nd "))
                                {
                                    degreeOfConnection = "2nd";
                                }
                                else if (degreeConnectionScrape.Contains(" 3rd "))
                                {
                                    degreeOfConnection = "3rd";
                                }
                            }
                            catch
                            { }

                            try
                            {
                                try
                                {
                                    if (string.IsNullOrEmpty(firstName)) firstName = "LinkedIn";
                                    if (string.IsNullOrEmpty(lastName)) lastName = "Member";
                                    if (string.IsNullOrEmpty(headlineTitle)) headlineTitle = "--";
                                    if (string.IsNullOrEmpty(location)) location = "--";
                                    if (string.IsNullOrEmpty(industry)) industry = "--";
                                    if (string.IsNullOrEmpty(currentTitle)) currentTitle = "--";
                                    if (string.IsNullOrEmpty(pastTitle)) pastTitle = "--";
                                    if (string.IsNullOrEmpty(degreeOfConnection)) degreeOfConnection = "--";
                                }
                                catch { };

                                GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Writing data in the CSV for the profile of " + firstName + " " + lastName + " ]");
                                if (string.IsNullOrEmpty(profileUrl))
                                {

                                }
                                string LDS_FinalData = profileUrl.Replace("'", "").Replace("\"", "").Replace("\r", "").Replace("\n", "") + "," + firstName.Replace(",", ";") + "," + lastName.Replace(",", ";").Replace("-", "") + "," + headlineTitle.Replace(",", ";") + "," + location.Replace(",", ";") + "," + industry.Replace(",", ";") + "," + currentTitle.Replace(",", ";") + "," + pastTitle.Replace(",", ";") + "," + degreeOfConnection.Replace(",", ";");
                                string FileName = "ScrapeWithoutGoingToMainProfile-" + SearchCriteria.FileName;
                                AppFileHelper.AddingLinkedInDataToCSVFileWithoutGoingToMainProfile(LDS_FinalData, FileName);
                                //return true;
                            }
                            catch
                            { }
                        }
                    }
                    catch { };
                }
            }
            catch
            { }
        }
        #endregion

        #region finalUrlCollection
        private void finalUrlCollectionForRecruter(string CheckString)
        {
            string Account = string.Empty;
            if (GlobalsScraper.IsStop_CompanyEmployeeScraperThread)
            {
                return;
            }



            GlobalsScraper.lstCompanyEmployeeScraperThread.Add(Thread.CurrentThread);
            GlobalsScraper.lstCompanyEmployeeScraperThread = GlobalsScraper.lstCompanyEmployeeScraperThread.Distinct().ToList();
            Thread.CurrentThread.IsBackground = true;


            try
            {
                List<string> numburlpp = new List<string>();
                GlobusHttpHelper HttpHelper = _HttpHelper;
                if (SearchCriteria.starter)
                {
                    RecordURL = RecordURL.Distinct().ToList();

                    //Log("[ " + DateTime.Now + " ] => [ Total Find URL >>> " + RecordURL.Count.ToString() + " ]");
                    //Log("-------------------------------------------------------------------------------------------------------------------------------");
                    Thread.Sleep(1 * 10 * 1000);
                    //foreach (string item in RecordURL)
                    while (true)
                    {
                        if (queRecordUrl.Count > 0)
                        {
                            string item = queRecordUrl.Dequeue();

                            try
                            {

                                if (item.Contains("recruiter/profile"))
                                {

                                    string urltemp = item;
                                    numburlpp.Add(urltemp);


                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ " + urltemp + " ]");

                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Fetching Data From URL ]");
                                    urltemp = urltemp + CheckString;
                                    bool check = CrawlingLinkedInPageRecruiter(urltemp, ref HttpHelper);

                                    int delay = RandomNumberGenerator.GenerateRandom(SearchCriteria.scraperMinDelay, SearchCriteria.scraperMaxDelay);
                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Delay for : " + delay + " Seconds ]");
                                    Thread.Sleep(delay * 1000);

                                }
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            Thread.Sleep(1 * 30 * 1000);

                            if (queRecordUrl.Count == 0)
                            {
                                Thread.Sleep(1 * 60 * 1000);

                                if (queRecordUrl.Count == 0)
                                {
                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Find All the Data ]");
                                    break;
                                }
                            }
                        }
                    }


                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED ]");
                    GlobusLogHelper.log.Info("-----------------------------------------------------------------------------------------------------------------------------------");
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region GettingAllUrlRecruiteryType
        public List<string> GettingAllUrlRecruiterType(string PageSource)
        {
            List<string> suburllist = new List<string>();
            List<string> subtitlelist = new List<string>();
            try
            {

                if (PageSource.Contains("memberId\":"))
                {
                    string[] trkArr = Regex.Split(PageSource, "memberId\":");
                    if (trkArr[0].Contains("authToken"))
                    {
                        trkArr = Regex.Split(PageSource, "authToken\":");
                    }
                    trkArr = trkArr.Skip(1).ToArray();
                    foreach (string item in trkArr)
                    {
                        try
                        {
                            if (item.Contains("authToken") || item.Contains("memberId\":"))
                            {
                                string authToken = Utils.getBetween(item, "authToken\":", ",").Replace("\"", "").Replace("}", "").Trim();
                                string memberId = Utils.getBetween(item, "memberId\":", ",").Replace("\"", "").Replace("}", "").Trim();
                                //string headline = Utils.getBetween(item, "headline\":", ",\"").Replace("\"", "").Replace("}", "").Replace("&amp;","&").Trim();
                                if (string.IsNullOrEmpty(memberId))
                                {
                                    memberId = Utils.getBetween(item, "", ",").Replace("\"", "").Replace("}", "").Trim();
                                }
                                if (string.IsNullOrEmpty(authToken))
                                {
                                    authToken = Utils.getBetween(item, "", ",").Replace("\"", "").Replace("}", "").Trim();
                                }
                                //string finalurl = "https://www.linkedin.com/recruiter/profile/" + memberId + "," + authToken + ",CAP" + "<:>" + headline;
                                string finalurl = "https://www.linkedin.com/recruiter/profile/" + memberId + "," + authToken + ",CAP";
                                suburllist.Add(finalurl);
                            }
                        }
                        catch
                        {
                        }
                    }

                }

                if (PageSource.Contains("headline\":"))
                {
                    string[] trkArrtitle = Regex.Split(PageSource, "headline\":");
                    trkArrtitle = trkArrtitle.Skip(1).ToArray();
                    foreach (string titleItem in trkArrtitle)
                    {
                        string title = Utils.getBetween(titleItem, "", ",\"").Replace("\"", "").Replace("}", "").Trim();
                        subtitlelist.Add(title);
                    }
                }

                if (suburllist.Count == subtitlelist.Count)
                {
                    for (int i = 0; i < suburllist.Count; i++)
                    {
                        suburllist[i] = suburllist[i] + "<:>" + subtitlelist[i];
                    }
                }


            }
            catch
            {
            }
            suburllist = suburllist.Distinct().ToList();
            return suburllist.Distinct().ToList();
        }
        #endregion


        #region CrawlingLinkedInPage
        public bool CrawlingLinkedInPageRecruiter(string Url, ref GlobusHttpHelper HttpHelper)
        {

            #region Data Initialization
            string GroupMemId = string.Empty;
            string Industry = string.Empty;
            string URLprofile = string.Empty;
            string firstname = string.Empty;
            string lastname = string.Empty;
            string location = string.Empty;
            string country = string.Empty;
            string postal = string.Empty;
            string phone = string.Empty;
            string USERemail = string.Empty;
            string code = string.Empty;
            string education1 = string.Empty;
            string education2 = string.Empty;
            string titlecurrent = string.Empty;
            string companycurrent = string.Empty;
            string CurrentCompUrl = string.Empty;
            string CurrentCompSite = string.Empty;
            string titlepast1 = string.Empty;
            string companypast1 = string.Empty;
            string titlepast2 = string.Empty;
            string html = string.Empty;
            string companypast2 = string.Empty;
            string titlepast3 = string.Empty;
            string companypast3 = string.Empty;
            string titlepast4 = string.Empty;
            string companypast4 = string.Empty;
            string Recommendations = string.Empty;
            string Connection = string.Empty;
            string Designation = string.Empty;
            string Website = string.Empty;
            string Contactsettings = string.Empty;
            string recomandation = string.Empty;

            string titleCurrenttitle = string.Empty;
            string titleCurrenttitle2 = string.Empty;
            string titleCurrenttitle3 = string.Empty;
            string titleCurrenttitle4 = string.Empty;
            string Skill = string.Empty;
            string TypeOfProfile = "Public";
            List<string> EducationList = new List<string>();
            string Finaldata = string.Empty;
            string EducationCollection = string.Empty;
            List<string> checkerlst = new List<string>();
            List<string> checkgrplist = new List<string>();
            string groupscollectin = string.Empty;
            string strFamilyName = string.Empty;
            string LDS_LoginID = string.Empty;
            string LDS_Websites = string.Empty;
            string LDS_UserProfileLink = string.Empty;
            string LDS_CurrentTitle = string.Empty;
            string LDS_Experience = string.Empty;
            string LDS_UserContact = string.Empty;
            string LDS_PastTitles = string.Empty;
            string LDS_BackGround_Summary = string.Empty;
            string LDS_Desc_AllComp = string.Empty;
            string HeadlineTitle = string.Empty;
            List<string> lstpasttitle = new List<string>();
            List<string> checkpasttitle = new List<string>();
            string DeegreeConn = string.Empty;
            string AccountType = string.Empty;
            bool CheckEmployeeScraper = false;
            string fileName = string.Empty;
            bool CampaignScraper = false;
            #endregion









            #region GetRequest
            if (Url.Contains("CompanyEmployeeScraper"))
            {
                try
                {
                    Url = Url.Replace("CompanyEmployeeScraper", string.Empty);
                    CheckEmployeeScraper = true;
                }
                catch
                { }
            }

            if (Url.Contains("CampaignScraper"))
            {
                try
                {
                    string[] Url_Split = Regex.Split(Url, "CampaignScraper");
                    Url = Url_Split[0];
                    fileName = Url_Split[1];
                    CampaignScraper = true;
                }
                catch
                { }
            }
            string GetDataPageSource = string.Empty;
            string[] arrtemp = Regex.Split(Url, "<:>");
            Url = arrtemp[0];
            string stringSource = HttpHelper.getHtmlfromUrl(new Uri(Url));
            if (string.IsNullOrEmpty(stringSource))
            {
                stringSource = HttpHelper.getHtmlfromUrl(new Uri(Url));
            }
            #endregion

            #region GroupMemId
            try
            {
                string[] gid = Url.Split(',');
                GroupMemId = gid[0].Replace("https://www.linkedin.com/recruiter/profile", string.Empty).Replace("\"", "");
            }
            catch { }
            #endregion

            #region Name
            try
            {
                try
                {
                    try
                    {
                        int StartIndex = stringSource.IndexOf("<title>");
                        string Start = stringSource.Substring(StartIndex).Replace("<title>", string.Empty);
                        int EndIndex = Start.IndexOf("| LinkedIn Recruiter</title>");
                        string End = Start.Substring(0, EndIndex).Replace(":", string.Empty).Replace("'", string.Empty).Replace(",", string.Empty).Trim();
                        strFamilyName = End.Trim();
                    }
                    catch
                    { }
                }
                catch
                {
                    try
                    {
                        strFamilyName = stringSource.Substring(stringSource.IndexOf("fmt__full_name\":"), (stringSource.IndexOf(",", stringSource.IndexOf("fmt__full_name\":")) - stringSource.IndexOf("fmt__full_name\":"))).Replace("fmt__full_name\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Trim();

                    }
                    catch { }
                }

                if (string.IsNullOrEmpty(strFamilyName))
                {
                    try
                    {
                        strFamilyName = stringSource.Substring(stringSource.IndexOf("<span class=\"full-name\">"), (stringSource.IndexOf("</span><span></span></span></h1></div></div><div id=\"headline-container\" data-li-template=\"headline\">", stringSource.IndexOf("</span><span></span></span></h1></div></div><div id=\"headline-container\" data-li-template=\"headline\">")) - stringSource.IndexOf("<span class=\"full-name\">"))).Replace("<span class=\"full-name\">", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Trim();
                    }
                    catch
                    { }
                }

                if (string.IsNullOrEmpty(strFamilyName))
                {
                    try
                    {
                        int StartIndex = stringSource.IndexOf("<span class=\"full-name\">");
                        string Start = stringSource.Substring(StartIndex).Replace("<span class=\"full-name\">", string.Empty);
                        int EndIndex = Start.IndexOf("</span>");
                        string End = Start.Substring(0, EndIndex).Replace("</span>", string.Empty);
                        strFamilyName = End.Trim();
                    }
                    catch
                    { }
                }

                if (string.IsNullOrEmpty(strFamilyName) && stringSource.Contains("<span class=\"full-name\""))
                {
                    try
                    {
                        int StartIndex = stringSource.IndexOf("<span class=\"full-name\"");
                        string Start = stringSource.Substring(StartIndex).Replace("<span class=\"full-name\"", string.Empty);
                        int EndIndex = Start.IndexOf("</span>");
                        string End = Start.Substring(0, EndIndex).Replace("</span>", string.Empty);
                        strFamilyName = End.Replace("dir=\"auto\">", "").Replace("\"", "").Trim();
                    }
                    catch
                    { }
                }


                if (string.IsNullOrEmpty(strFamilyName))
                {
                    try
                    {
                        int StartIndex = stringSource.IndexOf("<title>");
                        string Start = stringSource.Substring(StartIndex).Replace("</title>", string.Empty);
                        int EndIndex = Start.IndexOf("| LinkedIn Recruiter</title>");
                        string End = Start.Substring(0, EndIndex).Replace(":", string.Empty).Replace("'", string.Empty).Replace(",", string.Empty).Trim();
                        strFamilyName = End.Trim();
                    }
                    catch
                    { }
                }
            }
            catch { }

            #endregion

            #region Namesplitation
            string[] NameArr = new string[5];
            if (strFamilyName.Contains(" "))
            {
                try
                {
                    NameArr = Regex.Split(strFamilyName, " ");
                }
                catch { }
            }
            #endregion

            #region FirstName
            try
            {
                firstname = NameArr[0];
            }
            catch { }
            #endregion

            #region LastName

            try
            {
                lastname = NameArr[1];
            }
            catch { }

            try
            {
                if (NameArr.Count() == 3)
                {
                    try
                    {
                        lastname = NameArr[1] + " " + NameArr[2];
                    }
                    catch { }
                }

                if (lastname.Contains("}]"))
                {

                    #region Name
                    try
                    {
                        try
                        {
                            strFamilyName = stringSource.Substring(stringSource.IndexOf("<span class=\"n fn\">")).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Trim();
                        }
                        catch
                        {
                            try
                            {
                                strFamilyName = stringSource.Substring(stringSource.IndexOf("fmt__full_name\":"), (stringSource.IndexOf(",", stringSource.IndexOf("fmt__full_name\":")) - stringSource.IndexOf("fmt__full_name\":"))).Replace("fmt__full_name\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Trim();

                            }
                            catch { }
                        }
                    }
                    catch { }
                    #endregion
                }
            }
            catch { }
            #endregion

            #region Current Company
            try
            {
                int startindex = stringSource.IndexOf("<tr id=\"overview-summary-current\">");
                string start = stringSource.Substring(startindex).Replace("<tr id=\"overview-summary-current\">", string.Empty);
                int endindex = start.IndexOf("<tr id=\"overview-summary-past\">");
                string end = start.Substring(0, endindex).Replace("\u002d", string.Empty);
                string[] finalresult = Regex.Split(end, "\"auto\">");
                finalresult = finalresult.Skip(1).ToArray();

                foreach (var item in finalresult)
                {
                    if (string.IsNullOrEmpty(companycurrent))
                    {
                        try
                        {
                            companycurrent = Regex.Split(item, "</a>")[0].Replace("&amp;", "&").Replace("<strong class=\"highlight\">", string.Empty).Replace("</strong>", string.Empty);
                        }
                        catch { }

                    }
                    else
                    {
                        try
                        {
                            companycurrent = companycurrent + " : " + Regex.Split(item, "</a>")[0].Replace("&amp;", "&").Replace("<strong class=\"highlight\">", string.Empty).Replace("</strong>", string.Empty);
                        }
                        catch { }
                    }
                }

            }
            catch { }
            #endregion


            #region HeadlineTitle
            try
            {
                try
                {
                    try
                    {
                        HeadlineTitle = stringSource.Substring(stringSource.IndexOf("\"memberHeadline"), (stringSource.IndexOf("memberID", stringSource.IndexOf("\"memberHeadline")) - stringSource.IndexOf("\"memberHeadline"))).Replace("\"memberHeadline", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Replace("   ", string.Empty).Replace("visibletrue", string.Empty).Replace("isLNLedtrue", string.Empty).Replace("isPortfoliofalse", string.Empty).Replace("i18n__Location", string.Empty).Replace("Locationi18n__Linkedin_member", string.Empty).Replace("u002d", "-").Replace("LinkedIn Member", string.Empty).Replace("--Location", "--").ToString().Trim();
                        if (HeadlineTitle.Contains("#Name?"))
                        {
                            HeadlineTitle = "--";
                        }
                        if (HeadlineTitle.Contains("i18n"))
                        {
                            HeadlineTitle = Regex.Split(HeadlineTitle, "i18n")[0];
                        }

                    }
                    catch
                    {
                    }

                    if (string.IsNullOrEmpty(HeadlineTitle))
                    {
                        try
                        {
                            HeadlineTitle = stringSource.Substring(stringSource.IndexOf("memberHeadline\":"), (stringSource.IndexOf(",", stringSource.IndexOf("memberHeadline\":")) - stringSource.IndexOf("memberHeadline\":"))).Replace("memberHeadline\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Replace("   ", string.Empty).Replace(":", "").Replace("visibletrue", string.Empty).Replace("&dsh;", "").Replace("&amp", "").Replace(";", "").Replace("isLNLedtrue", string.Empty).Replace("isPortfoliofalse", string.Empty).Trim();
                        }
                        catch { }

                    }

                    if (string.IsNullOrEmpty(HeadlineTitle))
                    {
                        try
                        {
                            HeadlineTitle = stringSource.Substring(stringSource.IndexOf("<p class=\"title\">"), (stringSource.IndexOf("</p></div></div><div class=\"demographic-info adr editable-item\" id=\"demographics\">", stringSource.IndexOf("</p></div></div><div class=\"demographic-info adr editable-item\" id=\"demographics\">")) - stringSource.IndexOf("<p class=\"title\">"))).Replace("<p class=\"title\">", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace("<strong class=highlight>", string.Empty).Replace("</strong>", string.Empty).Trim();
                        }
                        catch { }
                    }

                    if (string.IsNullOrEmpty(HeadlineTitle))
                    {
                        HeadlineTitle = arrtemp[1];
                    }

                    string[] strdesigandcompany = new string[4];
                    if (HeadlineTitle.Contains(" at "))
                    {
                        try
                        {
                            strdesigandcompany = Regex.Split(HeadlineTitle, " at ");
                        }
                        catch { }

                        #region Title
                        try
                        {
                            titlecurrent = strdesigandcompany[0];
                        }
                        catch { }
                        #endregion

                        #region Current Company
                        try
                        {
                            if (string.IsNullOrEmpty(companycurrent))
                            {
                                companycurrent = strdesigandcompany[1];
                            }
                        }
                        catch { }
                        #endregion
                    }
                }
                catch { }

                #region Current Company Site
                try
                {
                    try
                    {
                        CurrentCompUrl = stringSource.Substring(stringSource.IndexOf("<strong><a href=\"/company"), (stringSource.IndexOf("<strong><a href=\"/company", stringSource.IndexOf("<strong><a href=\"/company")) - stringSource.IndexOf("dir=\"auto\">"))).Replace("<a href=\"", string.Empty).ToString().Trim();
                        CurrentCompUrl = "https://www.linkedin.com" + CurrentCompUrl;
                        CurrentCompUrl = CurrentCompUrl.Split('?')[0].Replace("<strong>", string.Empty).Trim();
                    }
                    catch { }

                    string CompanyUrl = HttpHelper.getHtmlfromUrl1(new Uri(CurrentCompUrl));


                    try
                    {
                        CurrentCompSite = CompanyUrl.Substring(CompanyUrl.IndexOf("<dt>Website</dt>"), (CompanyUrl.IndexOf("</dd>", CompanyUrl.IndexOf("<dt>Website</dt>")) - CompanyUrl.IndexOf("<dt>Website</dt>"))).Replace("<dt>Website</dt>", string.Empty).Replace("<dd>", string.Empty).Trim();
                    }
                    catch { }

                    try
                    {
                        CurrentCompSite = CompanyUrl.Substring(CompanyUrl.IndexOf("<h4>Website</h4>"), (CompanyUrl.IndexOf("</p>", CompanyUrl.IndexOf("<h4>Website</h4>")) - CompanyUrl.IndexOf("<h4>Website</h4>"))).Replace("<h4>Website</h4>", string.Empty).Replace("<p>", string.Empty).Trim();

                        if (CurrentCompSite.Contains("a href="))
                        {
                            try
                            {
                                string[] websArr = Regex.Split(CurrentCompSite, ">");
                                CurrentCompSite = websArr[1].Replace("</a", string.Empty).Replace("\n", string.Empty).Trim(); ;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
                catch { }

                #endregion

                #region PastCompany
                string[] companylist = Regex.Split(stringSource, "companyName\":");
                if (companylist.Count() == 1)
                {
                    companylist = Regex.Split(stringSource, "company-name");
                }
                if (companylist.Count() == 1)
                {
                    //companylist = Regex.Split(stringSource, "Companies");
                }

                string AllComapny = string.Empty;

                string Companyname = string.Empty;
                if (!stringSource.Contains("company-name") && companylist.Count() > 1)
                {
                    foreach (string item in companylist)
                    {
                        try
                        {
                            if (!item.Contains("<!DOCTYPE html>"))
                            {
                                Companyname = item.Substring(item.IndexOf(":"), (item.IndexOf(",", item.IndexOf(":")) - item.IndexOf(":"))).Replace(":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Replace("}", string.Empty).Replace("]", string.Empty).Trim();
                                string items = item;
                                checkerlst.Add(Companyname);
                                checkerlst = checkerlst.Distinct().ToList();
                            }
                        }
                        catch { }
                    }
                }
                else
                {
                    foreach (string item in companylist)
                    {
                        try
                        {
                            if (!item.Contains("<!DOCTYPE html>"))
                            {
                                Companyname = Utils.getBetween(item, "", ",").Replace(">", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Replace("}", string.Empty).Replace("]", string.Empty).Trim();
                                string items = item;
                                if (!string.IsNullOrEmpty(Companyname))
                                {
                                    checkerlst.Add(Companyname);
                                    checkerlst = checkerlst.Distinct().ToList();
                                }
                            }
                        }
                        catch { }
                    }
                }


                AllComapny = string.Empty;
                foreach (string item1 in checkerlst)
                {
                    if (string.IsNullOrEmpty(AllComapny))
                    {
                        AllComapny = item1.Replace("}", "").Replace("]", "").Replace("&amp;", "&");
                    }
                    else
                    {
                        AllComapny = AllComapny + " : " + item1.Replace("}", "").Replace("]", "").Replace("&amp;", "&");
                    }
                }
                #endregion

            #endregion Company

                #region Company Descripription

                try
                {
                    string[] str_CompanyDesc = Regex.Split(stringSource, "showSummarySection");

                    foreach (string item in str_CompanyDesc)
                    {
                        try
                        {
                            string Current_Company = string.Empty;
                            if (!item.Contains("<!DOCTYPE html>"))
                            {
                                int startindex = item.IndexOf("specialties\":\"");

                                if (startindex > 0)
                                {
                                    try
                                    {
                                        string start = item.Substring(startindex).Replace("specialties\":", "");
                                        int endindex = start.IndexOf("\",\"associatedWith\"");
                                        string end = start.Substring(0, endindex);
                                        Current_Company = end.Replace(",\"specialties_lb\":", string.Empty).Replace("\"", string.Empty).Replace("summary_lb", "Summary").Replace("&#x2022;", ";").Replace("<br>", string.Empty).Replace("\\n", string.Empty).Replace("\"u002", "-");
                                        LDS_BackGround_Summary = Current_Company;
                                    }
                                    catch { }
                                }

                            }

                            if (!item.Contains("<!DOCTYPE html>"))
                            {
                                int startindex = item.IndexOf("\"summary_lb\"");

                                if (startindex > 0)
                                {
                                    try
                                    {
                                        string start = item.Substring(startindex).Replace("\"summary_lb\"", "");
                                        int endindex = start.IndexOf("\",\"associatedWith\"");
                                        string end = start.Substring(0, endindex);
                                        Current_Company = end.Replace(",\"specialties_lb\":", string.Empty).Replace("<br>", string.Empty).Replace("\n", string.Empty).Replace("\"", string.Empty).Replace("summary_lb", "Summary").Replace(",", ";").Replace("u002", "-").Replace("&#x2022;", string.Empty).Replace(":", string.Empty);
                                        LDS_BackGround_Summary = Current_Company;
                                    }
                                    catch { }
                                }

                            }

                        }
                        catch { }
                    }

                    if (string.IsNullOrEmpty(LDS_BackGround_Summary))
                    {
                        try
                        {
                            LDS_BackGround_Summary = HttpHelper.GetDataWithTagValueByTagAndAttributeNameWithId(stringSource, "div", "summary-item-view");
                            LDS_BackGround_Summary = Regex.Replace(LDS_BackGround_Summary, "<.*?>", string.Empty).Replace(",", "").Replace("\n", "").Replace("<![CDATA[", "").Trim();
                        }
                        catch { }
                    }
                }
                catch { }

                #endregion

                #region Education
                try
                {
                    string[] str_UniversityName = Regex.Split(stringSource, "link__school_name");
                    foreach (string item in str_UniversityName)
                    {
                        try
                        {
                            string School = string.Empty;
                            string Degree = string.Empty;
                            string SessionEnd = string.Empty;
                            string SessionStart = string.Empty;
                            string Education = string.Empty;
                            if (stringSource.Contains("link__school_name"))
                            {
                                if (!item.Contains("<!DOCTYPE html>"))
                                {
                                    try
                                    {
                                        try
                                        {
                                            int startindex = item.IndexOf("fmt__school_highlight");
                                            string start = item.Substring(startindex).Replace("fmt__school_highlight", "");
                                            int endindex = start.IndexOf(",");
                                            School = start.Substring(0, endindex).Replace("\\u002d", string.Empty).Replace(":", string.Empty).Replace("\"", string.Empty).Replace("_highlight", string.Empty);
                                        }
                                        catch { }

                                        try
                                        {
                                            int startindex1 = item.IndexOf("degree");
                                            string start1 = item.Substring(startindex1).Replace("degree", "");
                                            int endindex1 = start1.IndexOf(",");
                                            Degree = start1.Substring(0, endindex1).Replace("\\u002d", string.Empty).Replace(":", string.Empty).Replace("\"", string.Empty).Replace("_highlight", string.Empty);
                                        }
                                        catch { }

                                        try
                                        {
                                            int startindex2 = item.IndexOf("enddate_my");
                                            string start2 = item.Substring(startindex2).Replace("enddate_my", "");
                                            int endindex2 = start2.IndexOf(",");
                                            SessionEnd = start2.Substring(0, endindex2).Replace("\\u002d", string.Empty).Replace(":", string.Empty).Replace("\"", string.Empty).Replace("_highlight", string.Empty);
                                        }
                                        catch { }

                                        try
                                        {
                                            int startindex3 = item.IndexOf("startdate_my");
                                            string start3 = item.Substring(startindex3).Replace("startdate_my", "");
                                            int endindex3 = start3.IndexOf(",");
                                            SessionStart = start3.Substring(0, endindex3).Replace("\\u002d", string.Empty).Replace(":", string.Empty).Replace("\"", string.Empty);
                                        }
                                        catch { }

                                        if (SessionStart == string.Empty && SessionEnd == string.Empty)
                                        {
                                            Education = " [" + School + "] Degree: " + Degree;
                                        }
                                        else
                                        {
                                            Education = " [" + School + "] Degree: " + Degree + " Session: " + SessionStart + "-" + SessionEnd;
                                        }
                                        //University = item.Substring(item.IndexOf(":"), (item.IndexOf(",", item.IndexOf(":")) - item.IndexOf(":"))).Replace(":", string.Empty).Replace("\\u002d", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                                    }
                                    catch { }
                                    EducationList.Add(Education);

                                }
                            }
                            else
                            {

                                str_UniversityName = Regex.Split(stringSource, "<div class=\"education");
                                foreach (string tempItem in str_UniversityName)
                                {
                                    try
                                    {
                                        if (!tempItem.Contains("<!DOCTYPE html>"))
                                        {
                                            List<string> lstSchool = HttpHelper.GetTextDataByTagAndAttributeName(tempItem, "h4", "summary fn org");
                                            List<string> lstDegree = HttpHelper.GetTextDataByTagAndAttributeName(tempItem, "span", "degree");
                                            List<string> lstSession = HttpHelper.GetTextDataByTagAndAttributeName(tempItem, "span", "education-date");

                                            if (lstSession.Count == 0)
                                            {
                                                Education = " [" + lstSchool[0] + "] Degree: " + lstDegree[0];
                                            }
                                            else
                                            {
                                                Education = " [" + lstSchool[0] + "] Degree: " + lstDegree[0] + " Session: " + lstSession[0].Replace("&#8211;", "-").Replace(",", "").Trim();
                                            }

                                            EducationList.Add(Education);
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch { }

                    }

                    EducationList = EducationList.Distinct().ToList();

                    foreach (string item in EducationList)
                    {
                        if (string.IsNullOrEmpty(EducationCollection))
                        {
                            EducationCollection = item.Replace("}", "").Replace("]", "").Replace("&amp;", "&");
                        }
                        else
                        {
                            EducationCollection = EducationCollection + "  -  " + item.Replace("}", "").Replace("]", "").Replace("&amp;", "&");
                        }
                    }
                    // string University1 = stringSource.Substring(stringSource.IndexOf("schoolName\":"), (stringSource.IndexOf(",", stringSource.IndexOf("schoolName\":")) - stringSource.IndexOf("schoolName\":"))).Replace("schoolName\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();

                }
                catch { }

                // Vamshi
                try
                {
                    string[] arrEducation = Regex.Split(stringSource, "educations");
                    EducationCollection = Utils.getBetween(stringSource, "schoolName\":\"", "\"");

                }
                catch
                {
                }


                #endregion Education







                #region location
                try
                {
                    //location = stringSource.Substring(stringSource.IndexOf("Country\",\"fmt__location\":"), (stringSource.IndexOf("i18n_no_location_matches", stringSource.IndexOf("Country\",\"fmt__location\":")) - stringSource.IndexOf("Country\",\"fmt__location\":"))).Replace("Country\",\"fmt__location\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                    int startindex = stringSource.IndexOf("location\":");
                    string start = stringSource.Substring(startindex).Replace("location\":", "");
                    int endindex = start.IndexOf(",");
                    string end = start.Substring(0, endindex).Replace("\u002d", string.Empty).Replace("\"", "").Replace("", "").Trim();
                    location = end;
                }
                catch (Exception ex)
                {

                }
                if (string.IsNullOrEmpty(location))
                {
                    try
                    {
                        List<string> lstLocation = HttpHelper.GetTextDataByTagAndAttributeName(stringSource, "span", "locality");
                        if (lstLocation.Count > 0)
                        {
                            location = lstLocation[lstLocation.Count - 1].Trim();
                        }
                    }
                    catch { }
                }
                #endregion location

                #region Country
                try
                {
                    int startindex = stringSource.IndexOf("\"geo_region\":");
                    if (startindex > 0)
                    {
                        string start = stringSource.Substring(startindex).Replace("\"geo_region\":", "");
                        int endindex = start.IndexOf("\"i18n_geo_region\":\"Location\"");
                        string end = start.Substring(0, endindex);
                        country = end;

                        string[] array = Regex.Split(end, "\"name\":\"");
                        array = array.Skip(1).ToArray();
                        foreach (string item in array)
                        {
                            try
                            {
                                int startindex1 = item.IndexOf("\",\"");
                                string strat1 = item.Substring(0, startindex1);
                                country = strat1;
                                break;
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                    }
                }
                catch (Exception ex)
                {

                }
                if (country == string.Empty)
                {
                    try
                    {
                        string[] countLocation = location.Split(',');

                        if (countLocation.Count() == 2)
                        {
                            country = location.Split(',')[1];
                        }
                        else if (countLocation.Count() == 3)
                        {
                            country = location.Split(',')[2];
                        }


                    }
                    catch { }

                }
                if (!string.IsNullOrEmpty(location))
                {
                    country = location;
                }
                #endregion

                #region Industry
                try
                {
                    //Industry = stringSource.Substring(stringSource.IndexOf("fmt__industry_highlight\":"), (stringSource.IndexOf(",", stringSource.IndexOf("fmt__industry_highlight\":")) - stringSource.IndexOf("fmt__industry_highlight\":"))).Replace("fmt__industry_highlight\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                    int startindex = stringSource.IndexOf("\"industry_highlight\":\"");
                    if (startindex > 0)
                    {
                        string start = stringSource.Substring(startindex).Replace("\"industry_highlight\":\"", "");
                        int endindex = start.IndexOf("\",");
                        string end = start.Substring(0, endindex).Replace("\"", string.Empty).Replace("</strong>", string.Empty).Replace("&amp;", "&");
                        if (end.Contains("strong class"))
                        {
                            Industry = end.Split('>')[1];
                        }
                        else
                        {
                            Industry = end;
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                if (string.IsNullOrEmpty(Industry))
                {
                    try
                    {
                        List<string> lstIndustry = HttpHelper.GetTextDataByTagAndAttributeName(stringSource, "dd", "industry");
                        if (lstIndustry.Count > 0)
                        {
                            Industry = lstIndustry[0].Replace(",", ":").Trim();
                        }
                    }
                    catch { }
                }
                #endregion Industry

                #region Connection
                try
                {
                    //Connection = stringSource.Substring(stringSource.IndexOf("_count_string\":"), (stringSource.IndexOf(",", stringSource.IndexOf("_count_string\":")) - stringSource.IndexOf("_count_string\":"))).Replace("_count_string\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                    int startindex = stringSource.IndexOf("\"numberOfConnections\":");
                    if (startindex > 0)
                    {
                        string start = stringSource.Substring(startindex).Replace("\"numberOfConnections\":", "");
                        int endindex = start.IndexOf(",");
                        string end = start.Substring(0, endindex).Replace("&amp;", "&").Replace("}", string.Empty);
                        Connection = end;
                    }
                }
                catch (Exception ex)
                {
                }

                if (string.IsNullOrEmpty(Connection))
                {
                    try
                    {
                        List<string> lstConnection = HttpHelper.GetTextDataByTagAndAttributeName(stringSource, "div", "member-connections");
                        if (lstConnection.Count > 0)
                        {
                            Connection = lstConnection[0].Replace(",", ":").Trim();
                        }
                    }
                    catch { }

                    // Vamshi
                    if (string.IsNullOrEmpty(Connection))
                        try
                        {
                            Connection = Utils.getBetween(stringSource, "numConnections\":", ",");
                        }
                        catch
                        { }


                }
                #endregion Connection

                #region Recommendation


                try
                {
                    string RecomnedUrl = string.Empty;
                    try
                    {
                        int startindex = stringSource.IndexOf("endorsements?id=");
                        string start = stringSource.Substring(startindex);
                        int endIndex = start.IndexOf("\"mem_pic\":");
                        if (endIndex < 0)
                        {
                            endIndex = start.IndexOf(">");
                        }
                        RecomnedUrl = (start.Substring(0, endIndex).Replace(",", string.Empty).Replace("\"", string.Empty).Replace(":", string.Empty));

                    }
                    catch { }

                    string PageSource = Utils.getBetween(stringSource, "<h2 class=\"title\">Recommendations</h2>", "/ul></li></ul></div>");
                    string[] arrayRecommendedName = Regex.Split(PageSource, "target=\"_blank\">");

                    arrayRecommendedName = arrayRecommendedName.Skip(1).ToArray();
                    List<string> ListRecommendationName = new List<string>();

                    foreach (var itemRecomName in arrayRecommendedName)
                    {
                        try
                        {
                            if (!itemRecomName.Contains("Endorsements"))
                            {
                                string Heading = string.Empty;
                                string Name = string.Empty;

                                try
                                {

                                    Name = Utils.getBetween(itemRecomName, "", "</a>");
                                    Name = Name.Replace("\"", string.Empty).Replace(":", string.Empty).Replace(",", ";").Trim();
                                }
                                catch { }

                                try
                                {
                                    Heading = Utils.getBetween(itemRecomName, "<h5 class=\"searchable\">", "</h5>");
                                    Heading = Heading.Replace(":", string.Empty).Replace("headline", string.Empty).Replace(",", string.Empty).Trim();
                                }
                                catch { }


                                ListRecommendationName.Add(Name + " : " + Heading);

                            }
                        }
                        catch { }

                    }

                    foreach (var item in ListRecommendationName)
                    {
                        if (recomandation == string.Empty)
                        {
                            recomandation = item;
                        }
                        else
                        {
                            recomandation += "  -  " + item;
                        }
                    }

                }
                catch { }

                #endregion

                #region Following


                #endregion

                #region Experience
                if (LDS_Experience == string.Empty)
                {
                    try
                    {
                        string[] array = Regex.Split(stringSource, "title_highlight");
                        string exp = string.Empty;
                        string comp = string.Empty;
                        List<string> ListExperince = new List<string>();
                        string SelItem = string.Empty;
                        if (stringSource.Contains("title_highlight"))
                        {
                            foreach (var itemGrps in array)
                            {
                                try
                                {
                                    if (itemGrps.Contains("title_pivot") && !itemGrps.Contains("<!DOCTYPE html")) //">Join
                                    {
                                        try
                                        {

                                            int startindex = itemGrps.IndexOf("\":\"");
                                            string start = itemGrps.Substring(startindex);
                                            int endIndex = start.IndexOf(",");
                                            exp = (start.Substring(0, endIndex).Replace("\"", string.Empty).Replace(":", string.Empty).Replace("&amp", "&").Replace(";", string.Empty).Replace("\\u002d", "-").Replace("name:", string.Empty));

                                        }
                                        catch { }
                                        if ((exp.Contains("strong class")) || (exp.Contains("highlight")) || (string.IsNullOrEmpty(exp)))
                                        {
                                            try
                                            {
                                                int startindex1 = itemGrps.IndexOf("\"title\":");
                                                string start1 = itemGrps.Substring(startindex1).Replace("\"title\":", string.Empty);
                                                int endIndex1 = start1.IndexOf(",");
                                                exp = (start1.Substring(0, endIndex1).Replace("\"", string.Empty).Replace(":", string.Empty).Replace("&amp", "&").Replace(";", string.Empty).Replace("\\u002d", "-").Replace("name:", string.Empty));
                                            }
                                            catch
                                            { }
                                        }

                                        try
                                        {

                                            int startindex1 = itemGrps.IndexOf("companyName");
                                            string start1 = itemGrps.Substring(startindex1);
                                            int endIndex1 = start1.IndexOf(",");
                                            comp = (start1.Substring(0, endIndex1).Replace("\"", string.Empty).Replace("companyName", string.Empty).Replace(":", string.Empty).Replace(";", string.Empty).Replace("\\u002d", "-").Replace("name:", string.Empty));

                                        }
                                        catch { }

                                        if (titlecurrent == string.Empty)
                                        {
                                            titlecurrent = exp;
                                        }

                                        if (companycurrent == string.Empty)
                                        {
                                            companycurrent = comp;
                                        }

                                        ListExperince.Add(exp + ":" + comp);


                                    }
                                }
                                catch { }
                            }

                        }
                        else
                        {
                            array = Regex.Split(stringSource, "<header>");
                            foreach (string tempItem in array)
                            {
                                try
                                {
                                    if (!tempItem.Contains("<!DOCTYPE html>"))
                                    {

                                        List<string> lstExp = objChilkat.GetDataTag(tempItem, "h4");
                                        List<string> lstComp = objChilkat.GetDataTag(tempItem, "h5");
                                        if (lstExp.Count > 0)
                                        {
                                            exp = lstExp[0];
                                        }
                                        if (lstComp.Count > 0)
                                        {
                                            comp = lstComp[0];
                                            if (string.IsNullOrEmpty(comp))
                                            {
                                                comp = lstComp[1];
                                            }
                                        }
                                        if (titlecurrent == string.Empty)
                                        {
                                            titlecurrent = lstExp[0];
                                        }

                                        if (companycurrent == string.Empty)
                                        {
                                            companycurrent = lstComp[0];
                                        }

                                        ListExperince.Add(exp + ":" + comp);

                                    }
                                }
                                catch { }
                            }

                        }
                        foreach (var item in ListExperince)
                        {
                            if (LDS_Experience == string.Empty)
                            {
                                LDS_Experience = item;
                            }
                            else
                            {
                                LDS_Experience += "  -  " + item;
                            }
                        }

                    }

                    catch { }

                    try
                    {
                        if (string.IsNullOrEmpty(titlecurrent))
                        {
                            int StartIndex = stringSource.IndexOf("trk=prof-0-ovw-curr_pos\">");
                            string Start = stringSource.Substring(StartIndex).Replace("trk=prof-0-ovw-curr_pos\">", string.Empty);
                            int EndIndex = Start.IndexOf("</a>");
                            string End = Start.Substring(0, EndIndex).Replace("</a>", string.Empty);
                            titlecurrent = End.Trim();
                        }
                    }
                    catch
                    { }


                }

                #endregion

                #region Group

                try
                {
                    string GroupUrl = string.Empty;
                    try
                    {
                        int startindex = stringSource.IndexOf("templateId\":\"profile_v2_connections");
                        string start = stringSource.Substring(startindex);
                        //int endIndex = start.IndexOf("vsrp_people_res_name");
                        int endIndex = start.IndexOf("}");
                        GroupUrl = (start.Substring(0, endIndex).Replace(",", string.Empty).Replace("\"", string.Empty).Replace("templateId:profile_v2_connectionsurl:", string.Empty));

                    }
                    catch { }

                    string PageSource = Utils.getBetween(stringSource, "<h2 class=\"title\">Groups</h2>", "</li></ul></div>");

                    string[] array1 = Regex.Split(PageSource, "target=\"_blank\">");
                    List<string> ListGroupName = new List<string>();
                    string SelItem = string.Empty;

                    foreach (var itemGrps in array1)
                    {
                        try
                        {
                            if (itemGrps.Contains("</a>") && !itemGrps.Contains("<!DOCTYPE html") && !itemGrps.StartsWith("<img")) //">Join
                            {
                                //if (itemGrps.IndexOf("?gid=") == 0)
                                {
                                    try
                                    {
                                        string _grpname = Utils.getBetween(itemGrps, "", "</a>");
                                        _grpname = _grpname.Replace("\"", string.Empty).Replace(":", string.Empty).Replace(",", ";").Replace("\"", string.Empty).Replace("amp", string.Empty).Replace("&", string.Empty).Replace(";", string.Empty).Replace("csrfToken", string.Empty).Replace("name:", string.Empty).Trim();
                                        ListGroupName.Add(_grpname);
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch { }
                    }

                    foreach (var item in ListGroupName)
                    {
                        if (groupscollectin == string.Empty)
                        {
                            groupscollectin = item;
                        }
                        else
                        {
                            groupscollectin += "  -  " + item;
                        }
                    }

                }
                catch { }

                if (string.IsNullOrEmpty(groupscollectin))
                {
                    List<string> lstGroupData = new List<string>();
                    string[] array1 = Regex.Split(stringSource, "link_groups_settings\":?");
                    array1 = array1.Skip(1).ToArray();
                    foreach (string item in array1)
                    {
                        string _item = Utils.getBetween(item, "name\":", ",").Replace("name\":", "").Replace(":", "").Replace("\"", "");
                        lstGroupData.Add(_item);
                    }

                    foreach (var item in lstGroupData)
                    {
                        if (groupscollectin == string.Empty)
                        {
                            groupscollectin = item;
                        }
                        else
                        {
                            groupscollectin += "  -  " + item;
                        }
                    }
                }

                if (string.IsNullOrEmpty(groupscollectin))
                {
                    List<string> lstGroupData = new List<string>();
                    string tempResponse = Utils.getBetween(stringSource, "<div id=\"groups\"", "<div>");
                    lstGroupData = HttpHelper.GetDataTag(tempResponse, "strong");

                    foreach (var item in lstGroupData)
                    {
                        if (groupscollectin == string.Empty)
                        {
                            groupscollectin = item;
                        }
                        else
                        {
                            groupscollectin += "  -  " + item;
                        }
                    }
                }

                #endregion

                #region skill and Expertise
                try
                {
                    string skillSource = Utils.getBetween(stringSource, "skills\":[", "],");
                    string[] strarr_skill = Regex.Split(skillSource, ",");
                    string[] strarr_skill1 = Regex.Split(stringSource, "fmt__skill_name\"");
                    if (strarr_skill.Count() >= 1)
                    {
                        foreach (string item in strarr_skill)
                        {
                            try
                            {
                                if (!item.Contains("!DOCTYPE html"))
                                {
                                    try
                                    {
                                        //string Grp = item.Substring(item.IndexOf("<"), (item.IndexOf(">", item.IndexOf("<")) - item.IndexOf("<"))).Replace("<", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Replace("\"u002", "-").Trim();
                                        string Grp = item.Replace(">", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Replace("\"u002", "-").Trim();
                                        checkgrplist.Add(Grp);
                                        checkgrplist.Distinct().ToList();
                                    }
                                    catch { }
                                }

                            }
                            catch { }
                        }

                        foreach (string item in checkgrplist)
                        {
                            if (string.IsNullOrEmpty(Skill))
                            {
                                Skill = item.Replace("\"u002", "-").Trim();
                            }
                            else
                            {
                                Skill = Skill + "  -  " + item;
                            }
                        }
                    }
                    else
                    {
                        if (strarr_skill1.Count() >= 2)
                        {
                            try
                            {
                                foreach (string skillitem in strarr_skill1)
                                {
                                    if (!skillitem.Contains("!DOCTYPE html"))
                                    {
                                        try
                                        {
                                            string Grp = skillitem.Substring(skillitem.IndexOf(":"), (skillitem.IndexOf("}", skillitem.IndexOf(":")) - skillitem.IndexOf(":"))).Replace(":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                                            checkgrplist.Add(Grp);
                                            checkgrplist.Distinct().ToList();
                                        }
                                        catch { }
                                    }
                                }

                                foreach (string item in checkgrplist)
                                {
                                    if (string.IsNullOrEmpty(Skill))
                                    {
                                        Skill = item;
                                    }
                                    else
                                    {
                                        Skill = Skill + "  -  " + item;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                Skill = Skill.Replace("a href=\";edu", "").Trim();
                #endregion


                #region AccountType
                try
                {
                    if (stringSource.Contains("has a Premium Account") || stringSource.Contains("Account Holder"))
                    {
                        AccountType = "Premium Account";
                    }
                    else
                    {
                        AccountType = "Basic Account";
                    }
                }
                catch (Exception ex)
                {

                }
                #endregion Email

                #region FullUrl
                try
                {
                    string[] UrlFull = System.Text.RegularExpressions.Regex.Split(Url, "&authType");
                    LDS_UserProfileLink = UrlFull[0];

                    LDS_UserProfileLink = Url;
                    //  LDS_UserProfileLink = stringSource.Substring(stringSource.IndexOf("canonicalUrl\":"), (stringSource.IndexOf(",", stringSource.IndexOf("canonicalUrl\":")) - stringSource.IndexOf("canonicalUrl\":"))).Replace("canonicalUrl\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                }
                catch { }
                #endregion

                LDS_LoginID = SearchCriteria.LoginID;
                if (string.IsNullOrEmpty(firstname))
                {
                    firstname = "Linkedin Member";
                }

                if (!string.IsNullOrEmpty(location))
                {
                    country = location;
                }

                if (string.IsNullOrEmpty(companycurrent))
                {
                    string[] ArrCompany = Regex.Split(stringSource, "href=\"/recruiter/company");
                    companycurrent = Utils.getBetween(ArrCompany[1], "title", "target").Replace(" ", "").Replace("\"", "").Replace("=", "").Trim();
                }

                string endName = firstname + " " + lastname;
                GroupStatus.GroupSpecMem.Add(GroupMemId, endName);
                if (firstname == string.Empty) firstname = "LinkedIn";
                if (lastname == string.Empty || lastname == null) lastname = "Member";
                if (HeadlineTitle == string.Empty) HeadlineTitle = "--";
                if (titlecurrent == string.Empty) titlecurrent = "--";
                if (companycurrent == string.Empty) companycurrent = "--";
                if (LDS_Desc_AllComp == string.Empty) LDS_Desc_AllComp = "--";
                if (LDS_BackGround_Summary == string.Empty) LDS_BackGround_Summary = "--";
                if (Connection == string.Empty) Connection = "--";
                if (recomandation == string.Empty) recomandation = "--";
                if (Skill == string.Empty) Skill = "--";
                if (LDS_Experience == string.Empty) LDS_Experience = "--";
                if (EducationCollection == string.Empty) EducationCollection = "--";
                if (groupscollectin == string.Empty) groupscollectin = "--";
                if (USERemail == string.Empty) USERemail = "--";
                if (LDS_UserContact == string.Empty) LDS_UserContact = "--";
                if (LDS_PastTitles == string.Empty) LDS_PastTitles = "--";
                if (AllComapny == string.Empty) AllComapny = "--";
                if (location == string.Empty) location = "--";
                if (country == string.Empty) country = "--";
                if (Industry == string.Empty) Industry = "--";
                if (Website == string.Empty || Website == null) Website = "--";

                if (!string.IsNullOrEmpty(location))
                {
                    country = location;
                }

                string LDS_FinalData = TypeOfProfile.Replace(",", ";") + "," + LDS_UserProfileLink.Replace(",", ";") + "," + GroupMemId.Replace(",", ";") + "," + firstname.Replace(",", ";") + "," + lastname.Replace(",", ";") + "," + HeadlineTitle.Replace(",", ";") + "," + titlecurrent.Replace(",", ";") + "," + companycurrent.Replace(",", ";") + "," + CurrentCompSite.Replace(",", ";") + "," + LDS_BackGround_Summary.Replace(",", ";") + "," + Connection.Replace(",", ";") + "," + recomandation.Replace(",", string.Empty) + "," + Skill.Replace(",", ";") + "," + LDS_Experience.Replace(",", string.Empty) + "," + EducationCollection.Replace(",", ";") + "," + groupscollectin.Replace(",", ";") + "," + USERemail.Replace(",", ";") + "," + LDS_UserContact.Replace(",", ";") + "," + LDS_PastTitles + "," + AllComapny.Replace(",", ";") + "," + location.Replace(",", ";") + "," + country.Replace(",", ";") + "," + Industry.Replace(",", ";") + "," + Website.Replace(",", ";") + "," + LDS_LoginID + "," + AccountType;

                if (!string.IsNullOrEmpty(firstname))
                {
                    //Log("[ " + DateTime.Now + " ] => [ Data : " + LDS_FinalData + " ]");
                }
                else
                {
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ No Data For URL : " + Url + " ]");
                    GlobusFileHelper.AppendStringToTextfileNewLineWithCarat(Url, Globals.DesktopFolder + "\\UnScrapedList.txt");
                }

                if (SearchCriteria.starter)
                {
                    string tempFinalData = LDS_FinalData.Replace(";", "").Replace(LDS_UserProfileLink, "").Replace(TypeOfProfile, "").Replace(",", "").Replace(LDS_LoginID, "").Trim();

                    if (!string.IsNullOrEmpty(tempFinalData))
                    {
                        if (CheckEmployeeScraper)
                        {
                            string FileName = "CompanyEmployeeScraper";
                            AppFileHelper.AddingLinkedInDataToCSVFileCompanyEmployeeScraper(LDS_FinalData, FileName);
                            return true;
                        }
                        else if (CampaignScraper)
                        {
                            AppFileHelper.AddingLinkedInDataToCSVFile(LDS_FinalData, fileName);
                            return true;
                        }
                        else
                        {
                            AppFileHelper.AddingLinkedInDataToCSVFile(LDS_FinalData, SearchCriteria.FileName);
                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }
        #endregion

































        public void StartLinkedInScraper(ref LinkedinUser objLinkedinUser)
        {
            try
            {
                GlobusHttpHelper HttpHelper = objLinkedinUser.globusHttpHelper;
                string nextPageUrl = string.Empty;
                foreach(var itemUrl in GlobalsScraper.lstUrlCompanyEmpScraper)
                {
                    string GetDataForPrimiumAccount = itemUrl;
                    ResponseWallPostForPremiumAcc = HttpHelper.getHtmlfromUrl1(new Uri(GetDataForPrimiumAccount));
                    string new_url = string.Empty;
                    if (ResponseWallPostForPremiumAcc.Contains("i18n_show_all_results"))
                    {
                        new_url = Utils.getBetween(ResponseWallPostForPremiumAcc, "i18n_show_all_results", "{");
                        new_url = Utils.getBetween(new_url, "escapeHatchUrl\":\"", "\"}");
                        new_url = "https://www.linkedin.com" + new_url;
                        if (new_url.Contains("\\u002d"))
                        {
                            new_url = new_url.Replace("\\u002d", "-");
                        }
                        if (new_url.Contains("\u002d"))
                        {
                            new_url = new_url.Replace("\u002d", "-");
                        }
                    }
                    if (!string.IsNullOrEmpty(new_url))
                    {
                        ResponseWallPostForPremiumAcc = HttpHelper.getHtmlfromUrl1(new Uri(new_url));
                    }
                    nextPageUrl = Utils.getBetween(ResponseWallPostForPremiumAcc, "page_number_i18n", "\",\"pageNum");
                    List<string> PageSerchUrl = GettingAllUrl(ResponseWallPostForPremiumAcc);
                    PageSerchUrl.Distinct();

                    if (PageSerchUrl.Count == 0)
                    {
                        //Log("[ " + DateTime.Now + " ] => [ On the basis of your Account you can able to see " + RecordURL.Count + " Results ]");
                        //break;
                    }

                    foreach (string item in PageSerchUrl)
                    {
                        if (SearchCriteria.starter)
                        {
                            if (item.Contains("pp_profile_photo_link") || item.Contains("vsrp_people_res_name") || item.Contains("profile/view?"))
                            {
                                try
                                {
                                    string urlSerch = item;
                                    if (urlSerch.Contains("vsrp_people_res_name"))
                                    {
                                        RecordURL.Add(urlSerch);
                                        if (!queRecordUrl.Contains(urlSerch))
                                        {
                                            queRecordUrl.Enqueue(urlSerch);
                                        }
                                        RecordURL = RecordURL.Distinct().ToList();
                                    }

                                    try
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ " + urlSerch + " ]");
                                    }
                                    catch { }
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void StartCompanyEmployeeScraperWithPagination(ref GlobusHttpHelper HttpHelper, string Url, int pagenumber)
        {
            string CheckString = "CampaignScraper";
            try
            {
                string[] Url_split = Regex.Split(Url, "##");
                if (Url_split.Count() > 1)
                {
                    Url = Url_split[0];
                    CheckString = Url_split[1];
                }
            }
            catch
            { }
            #region Login
            try
            {

                if (SearchCriteria.starter)
                {
                    #region Serch

                    string pageSourceaAdvanceSearch = HttpHelper.getHtmlfromUrl1(new Uri("http://www.linkedin.com/search"));
                    NewSearchPage = string.Empty;

                    #endregion
                }
                ResponseWallPostForPremiumAcc = HttpHelper.getHtmlfromUrl1(new Uri(Url));

                string GetResponce = string.Empty;
                string GetRequestURL = string.Empty;

                if (pagenumber >= 1)
                {
                    _HttpHelper = HttpHelper;

                    new Thread(() =>
                    {
                        if (SearchCriteria.starter)
                        {
                            finalUrlCollection(CheckString);

                        }

                    }).Start();

                    int count = 0;
                    Url = Url + "&page_num=";

                    for (int i = 1; i <= pagenumber; i++)
                    {
                        if (SearchCriteria.starter)
                        {
                            #region loop


                            ResponseWallPostForPremiumAcc = HttpHelper.getHtmlfromUrl(new Uri(Url + i));
                            if (ResponseWallPostForPremiumAcc.Contains("/profile/view?id"))
                            {

                                List<string> PageSerchUrl = GettingAllUrl(ResponseWallPostForPremiumAcc);
                                PageSerchUrl.Distinct();

                                if (PageSerchUrl.Count == 0)
                                {
                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ On the basis of your Account you can able to see " + RecordURL.Count + " Results ]");
                                    break;
                                }

                                foreach (string item in PageSerchUrl)
                                {
                                    if (SearchCriteria.starter)
                                    {
                                        if (item.Contains("pp_profile_photo_link") || item.Contains("vsrp_people_res_name") || item.Contains("profile/view?"))
                                        {
                                            try
                                            {
                                                string urlSerch = item;
                                                if (urlSerch.Contains("vsrp_people_res_name"))
                                                {
                                                    RecordURL.Add(urlSerch);
                                                    //if (!queRecordUrl.Contains(urlSerch))
                                                    //{
                                                    queRecordUrl.Enqueue(urlSerch);
                                                    //}
                                                    RecordURL = RecordURL.Distinct().ToList();
                                                }

                                                try
                                                {
                                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ " + urlSerch + " ]");
                                                }
                                                catch { }
                                            }
                                            catch { }
                                        }
                                    }
                                }
                            }

                            else if (!PostResponce.Contains("pp_profile_name_link") && PostResponce.Contains("Custom views are no longer supported. Please select either Basic or Expanded view"))
                            {
                                break;
                            }

                            #endregion
                        }
                    }
                }
                #region For Else
                else
                {
                    if (SearchCriteria.starter)
                    {

                        #region loop
                        if (ResponseWallPostForPremiumAcc.Contains("/profile/view?id"))
                        {

                            List<string> PageSerchUrl = ChilkatBasedRegex.GettingAllUrls(ResponseWallPostForPremiumAcc, "profile/view?id");
                            if (PageSerchUrl.Count == 0)
                            {

                                GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ On the basis of your Account or Your Input you can able to see " + RecordURL.Count + "  Results ]");

                            }

                            foreach (string item in PageSerchUrl)
                            {
                                if (SearchCriteria.starter)
                                {
                                    if (item.Contains("pp_profile_name_link"))
                                    {
                                        string urlSerch = "http://www.linkedin.com" + item;
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ " + urlSerch + " ]");
                                        RecordURL.Add(urlSerch);
                                        queRecordUrl.Enqueue(urlSerch);

                                    }
                                }
                            }
                        }
                        #endregion
                    }

                }

                RecordURL.Distinct();

            }

                #endregion

            catch { }
            #endregion
        }
        private void finalUrlCollection(string CheckString)
        {
            string Account = string.Empty;
            if (GlobalsScraper.isStopCompanyEmployeeScraper)
            {
                return;
            }

            if (CheckString.Contains("CampaignScraper"))
            {
                int startIndex = CheckString.IndexOf("#");
                string Start = CheckString.Substring(startIndex);
                int endIndex = Start.IndexOf("*");
                string end = Start.Substring(0, endIndex).Replace("*", string.Empty);
                Account = end;
            }

            //GlobalsScraper.lstThreadsCompanyEmployeeScraper.Add(Thread.CurrentThread);
            //GlobalsScraper.lstThreadsCompanyEmployeeScraper = GlobalsScraper.lstThreadsCompanyEmployeeScraper.Distinct().ToList();
            //Thread.CurrentThread.IsBackground = true;

            try
            {
                List<string> numburlpp = new List<string>();
                GlobusHttpHelper HttpHelper = _HttpHelper;
                if (SearchCriteria.starter)
                {
                    RecordURL = RecordURL.Distinct().ToList();
                    Thread.Sleep(1 * 10 * 1000);
                    while (true)
                    {
                        if (queRecordUrl.Count > 0)
                        {
                            if ((GlobalsScraper.no_of_profiles_scraped == GlobalsScraper.limitToScrape) && (GlobalsScraper.no_of_profiles_scraped != 0 && GlobalsScraper.limitToScrape != 0))
                            {
                                GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Scraping limit has been reached  ]");
                                return;
                            }
                            string item = queRecordUrl.Dequeue();

                            try
                            {
                                if (item.Contains("pp_profile_name_link"))
                                {
                                    if (SearchCriteria.starter)
                                    {

                                        string urltemp = item;
                                        numburlpp.Add(urltemp);

                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ " + urltemp + " ]");

                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Fetching Data From URL ]");
                                        urltemp = urltemp + CheckString;
                                        bool check = CrawlingLinkedInPage(urltemp, ref HttpHelper);

                                        GlobalsScraper.no_of_profiles_scraped++;

                                        int delay = RandomNumberGenerator.GenerateRandom(SearchCriteria.scraperMinDelay, SearchCriteria.scraperMaxDelay);
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Delay for : " + delay + " Seconds ]");
                                        Thread.Sleep(delay * 1000);

                                        if (!check)
                                        {
                                            string stop = string.Empty;
                                        }
                                    }

                                }
                                else if (item.Contains("/profile/view?"))
                                {

                                    string urltemp = item;
                                    numburlpp.Add(urltemp);

                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ " + urltemp + " ]");

                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Fetching Data From URL ]");
                                    urltemp = urltemp + CheckString;
                                    bool check = CrawlingLinkedInPage(urltemp, ref HttpHelper);

                                    GlobalsScraper.no_of_profiles_scraped++;

                                    int delay = RandomNumberGenerator.GenerateRandom(SearchCriteria.scraperMinDelay, SearchCriteria.scraperMaxDelay);
                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Delay for : " + delay + " Seconds ]");
                                    Thread.Sleep(delay * 1000);

                                    if (!check)
                                    {
                                        string stop = string.Empty;
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            Thread.Sleep(1 * 30 * 1000);

                            if (queRecordUrl.Count == 0)
                            {
                                Thread.Sleep(1 * 60 * 1000);

                                if (queRecordUrl.Count == 0)
                                {
                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Find All the Data ]");
                                    break;
                                }
                            }
                        }
                    }

                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED ]");
                    GlobusLogHelper.log.Info("-----------------------------------------------------------------------------------------------------------------------------------");
                }
            }
            catch (Exception ex)
            {

            }
        }
        public bool CrawlingLinkedInPage(string Url, ref GlobusHttpHelper HttpHelper)
        {
            //Url = "http://www.linkedin.com/profile/view?id=156004&authType=OUT_OF_NETWORK&authToken=6dZc&locale=en_US&srchid=3817933251417760999809&srchindex=22&srchtotal=1367893&trk=vsrp_people_res_name&trkInfo=VSRPsearchId%3A3817933251417760999809%2CVSRPtargetId%3A156004%2CVSRPcmpt%3Aprimary";

            #region Data Initialization
            string currentCompanyUrl = string.Empty;
            string personalPhoneNumber = string.Empty;
            string currentCompanyWebsite = string.Empty;
            string CompanyName_withAddress = string.Empty;
            string CompanyPhNumber = string.Empty;
            string pastCompany = string.Empty;
            string GroupMemId = string.Empty;
            string Industry = string.Empty;
            string URLprofile = string.Empty;
            string firstname = string.Empty;
            string lastname = string.Empty;
            string location = string.Empty;
            string country = string.Empty;
            string postal = string.Empty;
            string phone = string.Empty;
            string USERemail = string.Empty;
            string code = string.Empty;
            string education1 = string.Empty;
            string education2 = string.Empty;
            string titlecurrent = string.Empty;
            string companycurrent = string.Empty;
            string CurrentCompUrl = string.Empty;
            string CurrentCompSite = string.Empty;
            string titlepast1 = string.Empty;
            string companypast1 = string.Empty;
            string titlepast2 = string.Empty;
            string html = string.Empty;
            string companypast2 = string.Empty;
            string titlepast3 = string.Empty;
            string companypast3 = string.Empty;
            string titlepast4 = string.Empty;
            string companypast4 = string.Empty;
            string Recommendations = string.Empty;
            string Connection = string.Empty;
            string Designation = string.Empty;
            string Website = string.Empty;
            string Contactsettings = string.Empty;
            string recomandation = string.Empty;

            string titleCurrenttitle = string.Empty;
            string titleCurrenttitle2 = string.Empty;
            string titleCurrenttitle3 = string.Empty;
            string titleCurrenttitle4 = string.Empty;
            string Skill = string.Empty;
            string TypeOfProfile = "Public";
            List<string> EducationList = new List<string>();
            string Finaldata = string.Empty;
            string EducationCollection = string.Empty;
            List<string> checkerlst = new List<string>();
            List<string> checkgrplist = new List<string>();
            string groupscollectin = string.Empty;
            string strFamilyName = string.Empty;
            string LDS_LoginID = string.Empty;
            string LDS_Websites = string.Empty;
            string LDS_UserProfileLink = string.Empty;
            string LDS_CurrentTitle = string.Empty;
            string LDS_Experience = string.Empty;
            string LDS_UserContact = string.Empty;
            string LDS_PastTitles = string.Empty;
            string LDS_BackGround_Summary = string.Empty;
            string LDS_Desc_AllComp = string.Empty;
            string HeadlineTitle = string.Empty;
            List<string> lstpasttitle = new List<string>();
            List<string> checkpasttitle = new List<string>();
            string DeegreeConn = string.Empty;
            string AccountType = string.Empty;
            bool CheckEmployeeScraper = false;
            string fileName = string.Empty;
            bool CampaignScraper = false;

            string email_id = string.Empty;

            #endregion

            #region GetRequest
            if (Url.Contains("CompanyEmployeeScraper"))
            {
                try
                {
                    Url = Url.Replace("CompanyEmployeeScraper", string.Empty);
                    CheckEmployeeScraper = true;
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }

            if (Url.Contains("CampaignScraper"))
            {
                try
                {
                    string[] Url_Split = Regex.Split(Url, "CampaignScraper");
                    Url = Url_Split[0];
                    fileName = Url_Split[1];
                    CampaignScraper = true;
                }
                catch
                { }
            }


            string stringSource = HttpHelper.getHtmlfromUrl(new Uri(Url));
            if (string.IsNullOrEmpty(stringSource))
            {
                stringSource = HttpHelper.getHtmlfromUrl(new Uri(Url));
            }

            #endregion

            #region LoginId

            try
            {
                LDS_LoginID = HttpHelper.userName;

            }
            catch
            { }

            #endregion

            #region GroupMemId
            try
            {
                string[] gid = Url.Split('&');
                GroupMemId = gid[0].Replace("http://www.linkedin.com/profile/view?id=", string.Empty);
            }
            catch { }
            #endregion

            #region Name
            try
            {
                try
                {
                    strFamilyName = stringSource.Substring(stringSource.IndexOf("\"See Full Name\",\"i18n_Edit\":\"Edit\",\"fmt__full_name\":\""), (stringSource.IndexOf("i18n__expand_your_network_to_see_more", stringSource.IndexOf("\"See Full Name\",\"i18n_Edit\":\"Edit\",\"fmt__full_name\":\"")) - stringSource.IndexOf("\"See Full Name\",\"i18n_Edit\":\"Edit\",\"fmt__full_name\":\""))).Replace("\"See Full Name\",\"i18n_Edit\":\"Edit\",\"fmt__full_name\":\"", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Trim();
                }
                catch
                {
                    try
                    {
                        strFamilyName = stringSource.Substring(stringSource.IndexOf("fmt__full_name\":"), (stringSource.IndexOf(",", stringSource.IndexOf("fmt__full_name\":")) - stringSource.IndexOf("fmt__full_name\":"))).Replace("fmt__full_name\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Trim();

                    }
                    catch { }
                }

                if (string.IsNullOrEmpty(strFamilyName))
                {
                    try
                    {
                        strFamilyName = stringSource.Substring(stringSource.IndexOf("<span class=\"full-name\">"), (stringSource.IndexOf("</span><span></span></span></h1></div></div><div id=\"headline-container\" data-li-template=\"headline\">", stringSource.IndexOf("</span><span></span></span></h1></div></div><div id=\"headline-container\" data-li-template=\"headline\">")) - stringSource.IndexOf("<span class=\"full-name\">"))).Replace("<span class=\"full-name\">", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Trim();
                    }
                    catch
                    { }
                }

                if (string.IsNullOrEmpty(strFamilyName))
                {
                    try
                    {
                        int StartIndex = stringSource.IndexOf("<span class=\"full-name\">");
                        string Start = stringSource.Substring(StartIndex).Replace("<span class=\"full-name\">", string.Empty);
                        int EndIndex = Start.IndexOf("</span>");
                        string End = Start.Substring(0, EndIndex).Replace("</span>", string.Empty);
                        strFamilyName = End.Trim();
                    }
                    catch
                    { }
                }

                if (string.IsNullOrEmpty(strFamilyName) && stringSource.Contains("<span class=\"full-name\""))
                {
                    try
                    {
                        int StartIndex = stringSource.IndexOf("<span class=\"full-name\"");
                        string Start = stringSource.Substring(StartIndex).Replace("<span class=\"full-name\"", string.Empty);
                        int EndIndex = Start.IndexOf("</span>");
                        string End = Start.Substring(0, EndIndex).Replace("</span>", string.Empty);
                        strFamilyName = End.Replace("dir=\"auto\">", "").Replace("\"", "").Trim();
                    }
                    catch
                    { }
                }


                if (string.IsNullOrEmpty(strFamilyName))
                {
                    try
                    {
                        int StartIndex = stringSource.IndexOf("</script><title>");
                        string Start = stringSource.Substring(StartIndex).Replace("</script><title>", string.Empty);
                        int EndIndex = Start.IndexOf("| LinkedIn</title>");
                        string End = Start.Substring(0, EndIndex).Replace(":", string.Empty).Replace("'", string.Empty).Replace(",", string.Empty).Trim();
                        strFamilyName = End.Trim();
                    }
                    catch
                    { }
                }
            }
            catch { }

            #endregion

            #region AllEmailId

            //if (stringSource.Contains("@"))
            //{
            //    List<string> s1 = ScrapingJobUrls.GetEmails(stringSource);
            //    foreach (string item in s1)
            //    {
            //        email_id = email_id + item + ";";
            //    }
            //}
            #endregion

            #region FirstConnecrtion Email Id

            if (stringSource.Contains("@"))
            {
                try
                {
                    email_id = Utils.getBetween(stringSource, "<div class=\"profile-actions edit-actions", "<tr class=\"no-contact-info-data\"");
                    email_id = Utils.getBetween(email_id, "Email", "</li>");
                    email_id = Utils.getBetween(email_id, "<a href=", "</a>");
                    email_id = Utils.getBetween(email_id + "###", ">", "###");
                    if (!email_id.Contains("@"))
                    {
                        email_id = "";
                    }

                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }


            #endregion

            #region Namesplitation
            string[] NameArr = new string[5];
            if (strFamilyName.Contains(" "))
            {
                try
                {
                    NameArr = Regex.Split(strFamilyName, " ");
                }
                catch { }
            }
            #endregion

            #region FirstName
            try
            {
                firstname = NameArr[0];
            }
            catch { }
            #endregion

            #region LastName

            try
            {
                lastname = NameArr[1];
            }
            catch { }

            try
            {
                if (NameArr.Count() == 3)
                {
                    try
                    {
                        lastname = NameArr[1] + " " + NameArr[2];
                    }
                    catch { }
                }

                if (lastname.Contains("}]"))
                {

                    #region Name
                    try
                    {
                        try
                        {
                            strFamilyName = stringSource.Substring(stringSource.IndexOf("<span class=\"n fn\">")).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Trim();
                        }
                        catch
                        {
                            try
                            {
                                strFamilyName = stringSource.Substring(stringSource.IndexOf("fmt__full_name\":"), (stringSource.IndexOf(",", stringSource.IndexOf("fmt__full_name\":")) - stringSource.IndexOf("fmt__full_name\":"))).Replace("fmt__full_name\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Trim();

                            }
                            catch { }
                        }
                    }
                    catch { }
                    #endregion
                }
            }
            catch { }
            #endregion

            #region Current Company
            try
            {
                int startindex = stringSource.IndexOf("<tr id=\"overview-summary-current\">");
                string start = stringSource.Substring(startindex).Replace("<tr id=\"overview-summary-current\">", string.Empty);
                int endindex = start.IndexOf("<tr id=\"overview-summary-past\">");
                string end = start.Substring(0, endindex).Replace("\u002d", string.Empty);
                string[] finalresult = Regex.Split(end, "\"auto\">");
                finalresult = finalresult.Skip(1).ToArray();

                foreach (var item in finalresult)
                {
                    if (string.IsNullOrEmpty(companycurrent))
                    {
                        try
                        {
                            companycurrent = Regex.Split(item, "</a>")[0].Replace("&amp;", "&").Replace("<strong class=\"highlight\">", string.Empty).Replace("</strong>", string.Empty);
                        }
                        catch { }

                    }
                    else
                    {
                        try
                        {
                            companycurrent = companycurrent + " : " + Regex.Split(item, "</a>")[0].Replace("&amp;", "&").Replace("<strong class=\"highlight\">", string.Empty).Replace("</strong>", string.Empty);
                        }
                        catch { }
                    }
                }

            }
            catch { }
            #endregion

            #region Past Company
            try
            {

                string pastCompanies = Utils.getBetween(stringSource, "<div class=\"profile-overview-content", "</tr></table></div>");
                pastCompanies = Utils.getBetween(pastCompanies, "Previous", "Edit experience");
                string[] arr = Regex.Split(pastCompanies, "dir=\"auto\"");
                foreach (string item in arr)
                {
                    try
                    {
                        string company = Utils.getBetween(item, ">", "</a>");
                        company = company + ":";
                        pastCompany = pastCompany + company;
                        //pastCompany = pastCompany.Tri;
                    }
                    catch
                    { }
                }
            }
            catch
            { }
            #endregion


            #region HeadlineTitle
            try
            {
                try
                {
                    try
                    {

                        HeadlineTitle = Utils.getBetween(stringSource, "<p class=\"title\" dir=\"ltr\">", "</p>");
                        string HT = string.Empty;
                        string[] arr = null;
                        if (HeadlineTitle.Contains("<strong class=\"highlight\">"))
                        {
                            arr = Regex.Split(HeadlineTitle, "HeadlineTitle");
                        }
                        foreach (string item in arr)
                        {
                            HT += item + " ";
                        }
                        if (HT.Contains("</strong>"))
                        {
                            HT = HT.Replace("</strong>", "");
                        }
                        if (HT.Contains("<strong class=\"highlight\">"))
                        {
                            HT = HT.Replace("<strong class=\"highlight\">", "");
                        }
                        HeadlineTitle = HT;
                        if (string.IsNullOrEmpty(HeadlineTitle))
                        {
                            HeadlineTitle = stringSource.Substring(stringSource.IndexOf("\"memberHeadline"), (stringSource.IndexOf("memberID", stringSource.IndexOf("\"memberHeadline")) - stringSource.IndexOf("\"memberHeadline"))).Replace("\"memberHeadline", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Replace("   ", string.Empty).Replace("visibletrue", string.Empty).Replace("isLNLedtrue", string.Empty).Replace("isPortfoliofalse", string.Empty).Replace("i18n__Location", string.Empty).Replace("Locationi18n__Linkedin_member", string.Empty).Replace("u002d", "-").Replace("LinkedIn Member", string.Empty).Replace("--Location", "--").ToString().Trim();
                            if (HeadlineTitle.Contains("#Name?"))
                            {
                                HeadlineTitle = "--";
                            }
                            if (HeadlineTitle.Contains("i18n"))
                            {
                                HeadlineTitle = Regex.Split(HeadlineTitle, "i18n")[0];
                            }
                        }
                    }
                    catch
                    {
                    }

                    if (string.IsNullOrEmpty(HeadlineTitle))
                    {
                        try
                        {
                            HeadlineTitle = stringSource.Substring(stringSource.IndexOf("memberHeadline\":"), (stringSource.IndexOf(",", stringSource.IndexOf("memberHeadline\":")) - stringSource.IndexOf("memberHeadline\":"))).Replace("memberHeadline\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Replace("   ", string.Empty).Replace(":", "").Replace("visibletrue", string.Empty).Replace("&dsh;", "").Replace("&amp", "").Replace(";", "").Replace("isLNLedtrue", string.Empty).Replace("isPortfoliofalse", string.Empty).Trim();
                        }
                        catch { }

                    }

                    if (string.IsNullOrEmpty(HeadlineTitle))
                    {
                        try
                        {
                            HeadlineTitle = stringSource.Substring(stringSource.IndexOf("<p class=\"title\">"), (stringSource.IndexOf("</p></div></div><div class=\"demographic-info adr editable-item\" id=\"demographics\">", stringSource.IndexOf("</p></div></div><div class=\"demographic-info adr editable-item\" id=\"demographics\">")) - stringSource.IndexOf("<p class=\"title\">"))).Replace("<p class=\"title\">", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace("<strong class=highlight>", string.Empty).Replace("</strong>", string.Empty).Trim();
                        }
                        catch { }
                    }

                    string[] strdesigandcompany = new string[4];
                    if (HeadlineTitle.Contains(" at "))
                    {
                        try
                        {
                            strdesigandcompany = Regex.Split(HeadlineTitle, " at ");
                        }
                        catch { }

                        #region Title
                        try
                        {
                            titlecurrent = strdesigandcompany[0];
                        }
                        catch { }
                        #endregion

                        #region Current Company
                        try
                        {
                            if (string.IsNullOrEmpty(companycurrent))
                            {
                                companycurrent = strdesigandcompany[1];
                            }
                        }
                        catch { }
                        #endregion
                    }
                }
                catch { }

                #region Current Company Site
                try
                {

                    try
                    {
                        string[] currComp = Regex.Split(stringSource, "<div id=\"background-experience\"");




                    }
                    catch
                    { }

                    try
                    {
                        CurrentCompUrl = stringSource.Substring(stringSource.IndexOf("<strong><a href=\"/company"), (stringSource.IndexOf("<strong><a href=\"/company", stringSource.IndexOf("<strong><a href=\"/company")) - stringSource.IndexOf("dir=\"auto\">"))).Replace("<a href=\"", string.Empty).ToString().Trim();
                        CurrentCompUrl = "https://www.linkedin.com" + CurrentCompUrl;
                        CurrentCompUrl = CurrentCompUrl.Split('?')[0].Replace("<strong>", string.Empty).Trim();
                    }
                    catch { }

                    string CompanyUrl = HttpHelper.getHtmlfromUrl1(new Uri(CurrentCompUrl));


                    try
                    {
                        CurrentCompSite = CompanyUrl.Substring(CompanyUrl.IndexOf("<dt>Website</dt>"), (CompanyUrl.IndexOf("</dd>", CompanyUrl.IndexOf("<dt>Website</dt>")) - CompanyUrl.IndexOf("<dt>Website</dt>"))).Replace("<dt>Website</dt>", string.Empty).Replace("<dd>", string.Empty).Trim();
                    }
                    catch { }

                    try
                    {
                        CurrentCompSite = CompanyUrl.Substring(CompanyUrl.IndexOf("<h4>Website</h4>"), (CompanyUrl.IndexOf("</p>", CompanyUrl.IndexOf("<h4>Website</h4>")) - CompanyUrl.IndexOf("<h4>Website</h4>"))).Replace("<h4>Website</h4>", string.Empty).Replace("<p>", string.Empty).Trim();

                        if (CurrentCompSite.Contains("a href="))
                        {
                            try
                            {
                                string[] websArr = Regex.Split(CurrentCompSite, ">");
                                CurrentCompSite = websArr[1].Replace("</a", string.Empty).Replace("\n", string.Empty).Trim(); ;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
                catch { }

                #endregion

                #region PastCompany
                string[] companylist = Regex.Split(stringSource, "companyName\"");
                if (companylist.Count() == 1)
                {
                    companylist = Regex.Split(stringSource, "company-name");
                }
                if (companylist.Count() == 1)
                {
                    //companylist = Regex.Split(stringSource, "Companies");
                }

                string AllComapny = string.Empty;

                string Companyname = string.Empty;
                if (!stringSource.Contains("company-name") && companylist.Count() > 1)
                {
                    foreach (string item in companylist)
                    {
                        try
                        {
                            if (!item.Contains("<!DOCTYPE html>"))
                            {
                                Companyname = item.Substring(item.IndexOf(":"), (item.IndexOf(",", item.IndexOf(":")) - item.IndexOf(":"))).Replace(":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Replace("}", string.Empty).Replace("]", string.Empty).Trim();
                                string items = item;
                                checkerlst.Add(Companyname);
                                checkerlst = checkerlst.Distinct().ToList();
                            }
                        }
                        catch { }
                    }
                }
                else
                {
                    foreach (string item in companylist)
                    {
                        try
                        {
                            if (!item.Contains("<!DOCTYPE html>"))
                            {
                                Companyname = item.Substring(item.IndexOf(">"), (item.IndexOf("<", item.IndexOf(">")) - item.IndexOf(">"))).Replace(">", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Replace("}", string.Empty).Replace("]", string.Empty).Trim();
                                string items = item;
                                if (!string.IsNullOrEmpty(Companyname))
                                {
                                    checkerlst.Add(Companyname);
                                    checkerlst = checkerlst.Distinct().ToList();
                                }
                            }
                        }
                        catch { }
                    }
                    try
                    {
                        CompanyName_withAddress = Utils.getBetween(companylist[2], "\"auto\">", "</a>");
                    }
                    catch { };
                    string location1 = string.Empty;
                    if (companylist.Count() > 1)
                    {
                        location1 = Utils.getBetween(companylist[2], "class=\"locality\">", "</span>");
                    }
                    else
                    {
                        try
                        {
                            location1 = Utils.getBetween(stringSource, "class=\"locality\">", "</span>");
                            if (location1.Contains("<strong"))
                            {
                                try
                                {
                                    string[] getLocation = Regex.Split(location1, "strong");
                                    location1 = Utils.getBetween(getLocation[1], ">", "<");
                                }
                                catch { };
                            }
                        }
                        catch { };
                    }
                    if (string.IsNullOrEmpty(location1))
                    {
                        try
                        {
                            location1 = Utils.getBetween(companylist[1], "\"auto\">", "</a>");
                        }
                        catch { }
                    }

                    if (string.IsNullOrEmpty(location1) && !string.IsNullOrEmpty(companycurrent))
                    {
                        location1 = companycurrent;
                    }

                    CompanyName_withAddress += location1;


                }


                AllComapny = string.Empty;
                foreach (string item1 in checkerlst)
                {
                    if (string.IsNullOrEmpty(AllComapny))
                    {
                        AllComapny = item1.Replace("}", "").Replace("]", "").Replace("&amp;", "&");
                    }
                    else
                    {
                        AllComapny = AllComapny + " : " + item1.Replace("}", "").Replace("]", "").Replace("&amp;", "&");
                    }
                }
                #endregion

            #endregion Company

                #region Company Descripription

                try
                {
                    string[] str_CompanyDesc = Regex.Split(stringSource, "showSummarySection");

                    foreach (string item in str_CompanyDesc)
                    {
                        try
                        {
                            string Current_Company = string.Empty;
                            if (!item.Contains("<!DOCTYPE html>"))
                            {
                                int startindex = item.IndexOf("specialties\":\"");

                                if (startindex > 0)
                                {
                                    try
                                    {
                                        string start = item.Substring(startindex).Replace("specialties\":", "");
                                        int endindex = start.IndexOf("\",\"associatedWith\"");
                                        string end = start.Substring(0, endindex);
                                        Current_Company = end.Replace(",\"specialties_lb\":", string.Empty).Replace("\"", string.Empty).Replace("summary_lb", "Summary").Replace("&#x2022;", ";").Replace("<br>", string.Empty).Replace("\\n", string.Empty).Replace("\"u002", "-");
                                        LDS_BackGround_Summary = Current_Company;
                                    }
                                    catch { }
                                }

                            }

                            if (!item.Contains("<!DOCTYPE html>"))
                            {
                                int startindex = item.IndexOf("\"summary_lb\"");

                                if (startindex > 0)
                                {
                                    try
                                    {
                                        string start = item.Substring(startindex).Replace("\"summary_lb\"", "");
                                        int endindex = start.IndexOf("\",\"associatedWith\"");
                                        string end = start.Substring(0, endindex);
                                        Current_Company = end.Replace(",\"specialties_lb\":", string.Empty).Replace("<br>", string.Empty).Replace("\n", string.Empty).Replace("\"", string.Empty).Replace("summary_lb", "Summary").Replace(",", ";").Replace("u002", "-").Replace("&#x2022;", string.Empty).Replace(":", string.Empty);
                                        LDS_BackGround_Summary = Current_Company;
                                    }
                                    catch { }
                                }

                            }

                        }
                        catch { }
                    }

                    if (string.IsNullOrEmpty(LDS_BackGround_Summary))
                    {
                        try
                        {
                            LDS_BackGround_Summary = HttpHelper.GetDataWithTagValueByTagAndAttributeNameWithId(stringSource, "div", "summary-item-view");
                            LDS_BackGround_Summary = Regex.Replace(LDS_BackGround_Summary, "<.*?>", string.Empty).Replace(",", "").Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace("<![CDATA[", "").Trim();
                            string[] arr = Regex.Split(LDS_BackGround_Summary, "\n");
                            if (arr.Length > 1)
                            {
                                LDS_BackGround_Summary = arr[0];
                            }
                        }
                        catch { }
                    }
                }
                catch { }

                #endregion

                #region Education
                try
                {
                    string[] str_UniversityName = Regex.Split(stringSource, "link__school_name");
                    foreach (string item in str_UniversityName)
                    {
                        try
                        {
                            string School = string.Empty;
                            string Degree = string.Empty;
                            string SessionEnd = string.Empty;
                            string SessionStart = string.Empty;
                            string Education = string.Empty;
                            if (stringSource.Contains("link__school_name"))
                            {
                                if (!item.Contains("<!DOCTYPE html>"))
                                {
                                    try
                                    {
                                        try
                                        {
                                            int startindex = item.IndexOf("fmt__school_highlight");
                                            string start = item.Substring(startindex).Replace("fmt__school_highlight", "");
                                            int endindex = start.IndexOf(",");
                                            School = start.Substring(0, endindex).Replace("\\u002d", string.Empty).Replace(":", string.Empty).Replace("\"", string.Empty).Replace("_highlight", string.Empty);
                                        }
                                        catch { }

                                        try
                                        {
                                            int startindex1 = item.IndexOf("degree");
                                            string start1 = item.Substring(startindex1).Replace("degree", "");
                                            int endindex1 = start1.IndexOf(",");
                                            Degree = start1.Substring(0, endindex1).Replace("\\u002d", string.Empty).Replace(":", string.Empty).Replace("\"", string.Empty).Replace("_highlight", string.Empty);
                                        }
                                        catch { }

                                        try
                                        {
                                            int startindex2 = item.IndexOf("enddate_my");
                                            string start2 = item.Substring(startindex2).Replace("enddate_my", "");
                                            int endindex2 = start2.IndexOf(",");
                                            SessionEnd = start2.Substring(0, endindex2).Replace("\\u002d", string.Empty).Replace(":", string.Empty).Replace("\"", string.Empty).Replace("_highlight", string.Empty);
                                        }
                                        catch { }

                                        try
                                        {
                                            int startindex3 = item.IndexOf("startdate_my");
                                            string start3 = item.Substring(startindex3).Replace("startdate_my", "");
                                            int endindex3 = start3.IndexOf(",");
                                            SessionStart = start3.Substring(0, endindex3).Replace("\\u002d", string.Empty).Replace(":", string.Empty).Replace("\"", string.Empty);
                                        }
                                        catch { }

                                        if (SessionStart == string.Empty && SessionEnd == string.Empty)
                                        {
                                            Education = " [" + School + "] Degree: " + Degree;
                                        }
                                        else
                                        {
                                            Education = " [" + School + "] Degree: " + Degree + " Session: " + SessionStart + "-" + SessionEnd;
                                        }
                                        //University = item.Substring(item.IndexOf(":"), (item.IndexOf(",", item.IndexOf(":")) - item.IndexOf(":"))).Replace(":", string.Empty).Replace("\\u002d", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                                    }
                                    catch { }
                                    EducationList.Add(Education);

                                }
                            }
                            else
                            {

                                str_UniversityName = Regex.Split(stringSource, "<div class=\"education");
                                foreach (string tempItem in str_UniversityName)
                                {
                                    try
                                    {
                                        if (!tempItem.Contains("<!DOCTYPE html>"))
                                        {
                                            List<string> lstSchool = HttpHelper.GetTextDataByTagAndAttributeName(tempItem, "h4", "summary fn org");
                                            List<string> lstDegree = HttpHelper.GetTextDataByTagAndAttributeName(tempItem, "span", "degree");
                                            List<string> lstSession = HttpHelper.GetTextDataByTagAndAttributeName(tempItem, "span", "education-date");

                                            if (lstSession.Count == 0)
                                            {
                                                Education = " [" + lstSchool[0] + "] Degree: " + lstDegree[0];
                                            }
                                            else
                                            {
                                                Education = " [" + lstSchool[0] + "] Degree: " + lstDegree[0] + " Session: " + lstSession[0].Replace("&#8211;", "-").Replace(",", "").Trim();
                                            }
                                            Education = Education.Replace("&#39;", "'");

                                            EducationList.Add(Education);
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch { }

                    }

                    EducationList = EducationList.Distinct().ToList();

                    foreach (string item in EducationList)
                    {
                        if (string.IsNullOrEmpty(EducationCollection))
                        {
                            EducationCollection = item.Replace("}", "").Replace("]", "").Replace("&amp;", "&");
                        }
                        else
                        {
                            EducationCollection = EducationCollection + "  -  " + item.Replace("}", "").Replace("]", "").Replace("&amp;", "&");
                        }
                    }
                    // string University1 = stringSource.Substring(stringSource.IndexOf("schoolName\":"), (stringSource.IndexOf(",", stringSource.IndexOf("schoolName\":")) - stringSource.IndexOf("schoolName\":"))).Replace("schoolName\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();

                }

                catch { }

                #endregion Education

                #region Email
                try
                {
                    if (stringSource.Contains("mailto:"))
                    {
                        string[] str_Email = Regex.Split(stringSource, "mailto:");
                        USERemail = stringSource.Substring(stringSource.IndexOf("mailto:"), (stringSource.IndexOf(">", stringSource.IndexOf("mailto:")) - stringSource.IndexOf("mailto:"))).Replace("mailto:", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                    }
                    else
                    {
                        string[] str_Email = Regex.Split(stringSource, "email\"");
                        USERemail = stringSource.Substring(stringSource.IndexOf("[{\"email\":"), (stringSource.IndexOf("}]", stringSource.IndexOf("[{\"email\":")) - stringSource.IndexOf("[{\"email\":"))).Replace("[{\"email\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();

                    }
                }
                catch (Exception ex)
                {

                }
                #endregion Email

                #region UserContact
                try
                {
                    if (stringSource.Contains("<div id=\"phone-view\">"))
                    {
                        //string[] str_Contact = Regex.Split(stringSource, "<div id=\"phone-view\">");
                        LDS_UserContact = stringSource.Substring(stringSource.IndexOf("<div id=\"phone-view\">"), (stringSource.IndexOf("</li>", stringSource.IndexOf("<div id=\"phone-view\">")) - stringSource.IndexOf("<div id=\"phone-view\">"))).Replace("<div id=\"phone-view\">", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace("<ul><li>", string.Empty).Replace("&nbsp;", "").Trim();
                    }
                    else
                    {
                        //string[] str_Email = Regex.Split(stringSource, "<div id=\"phone-view\">");
                        LDS_UserContact = stringSource.Substring(stringSource.IndexOf("<div id=\"phone-view\">"), (stringSource.IndexOf("</li>", stringSource.IndexOf("<div id=\"phone-view\">")) - stringSource.IndexOf("<div id=\"phone-view\">"))).Replace("<div id=\"phone-view\">", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace("<ul><li>", string.Empty).Replace("&nbsp;", "").Trim();

                    }
                }
                catch (Exception ex)
                {

                }
                #endregion Email

                #region Website
                try
                {
                    Website = stringSource.Substring(stringSource.IndexOf("[{\"URL\":"), (stringSource.IndexOf(",", stringSource.IndexOf("[{\"URL\":")) - stringSource.IndexOf("[{\"URL\":"))).Replace("[{\"URL\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty).Replace("]", string.Empty).Trim();
                }
                catch { }
                if (string.IsNullOrEmpty(Website))
                {
                    try
                    {

                        Website = HttpHelper.GetDataWithTagValueByTagAndAttributeNameWithId(stringSource, "div", "website-view");
                        Website = Regex.Replace(Website, "<[^>]*>", String.Empty).Replace("\n", "").Replace("\r", "").Replace(" ", " ").Trim();
                        Website = Regex.Replace(Website, @"\s+", " ").Replace(",", " ").Trim();
                    }
                    catch { }
                }
                #endregion Website

                #region location
                try
                {
                    //location = stringSource.Substring(stringSource.IndexOf("Country\",\"fmt__location\":"), (stringSource.IndexOf("i18n_no_location_matches", stringSource.IndexOf("Country\",\"fmt__location\":")) - stringSource.IndexOf("Country\",\"fmt__location\":"))).Replace("Country\",\"fmt__location\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                    int startindex = stringSource.IndexOf("fmt_location");
                    string start = stringSource.Substring(startindex).Replace("fmt_location\":\"", "");
                    int endindex = start.IndexOf("\"");
                    string end = start.Substring(0, endindex).Replace("\u002d", string.Empty);
                    location = end;
                }
                catch (Exception ex)
                {

                }
                if (string.IsNullOrEmpty(location))
                {
                    try
                    {
                        List<string> lstLocation = HttpHelper.GetTextDataByTagAndAttributeName(stringSource, "span", "locality");
                        if (lstLocation.Count > 0)
                        {
                            location = lstLocation[lstLocation.Count - 1].Trim();
                        }
                    }
                    catch { }
                }
                #endregion location

                #region Country
                try
                {
                    int startindex = stringSource.IndexOf("\"geo_region\":");
                    if (startindex > 0)
                    {
                        string start = stringSource.Substring(startindex).Replace("\"geo_region\":", "");
                        int endindex = start.IndexOf("\"i18n_geo_region\":\"Location\"");
                        string end = start.Substring(0, endindex);
                        country = end;

                        string[] array = Regex.Split(end, "\"name\":\"");
                        array = array.Skip(1).ToArray();
                        foreach (string item in array)
                        {
                            try
                            {
                                int startindex1 = item.IndexOf("\",\"");
                                string strat1 = item.Substring(0, startindex1);
                                country = strat1;
                                break;
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                    }
                }
                catch (Exception ex)
                {

                }
                if (country == string.Empty)
                {
                    try
                    {
                        string[] countLocation = location.Split(',');

                        if (countLocation.Count() == 2)
                        {
                            country = location.Split(',')[1];
                        }
                        else if (countLocation.Count() == 3)
                        {
                            country = location.Split(',')[2];
                        }


                    }
                    catch { }

                }
                #endregion

                #region Industry
                try
                {
                    //Industry = stringSource.Substring(stringSource.IndexOf("fmt__industry_highlight\":"), (stringSource.IndexOf(",", stringSource.IndexOf("fmt__industry_highlight\":")) - stringSource.IndexOf("fmt__industry_highlight\":"))).Replace("fmt__industry_highlight\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                    int startindex = stringSource.IndexOf("\"industry_highlight\":\"");
                    if (startindex > 0)
                    {
                        string start = stringSource.Substring(startindex).Replace("\"industry_highlight\":\"", "");
                        int endindex = start.IndexOf("\",");
                        string end = start.Substring(0, endindex).Replace("\"", string.Empty).Replace("</strong>", string.Empty).Replace("&amp;", "&");
                        if (end.Contains("strong class"))
                        {
                            Industry = end.Split('>')[1];
                        }
                        else
                        {
                            Industry = end;
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                if (string.IsNullOrEmpty(Industry))
                {
                    try
                    {
                        List<string> lstIndustry = HttpHelper.GetTextDataByTagAndAttributeName(stringSource, "dd", "industry");
                        if (lstIndustry.Count > 0)
                        {
                            Industry = lstIndustry[0].Replace(",", ":").Trim();
                        }
                    }
                    catch { }
                }
                #endregion Industry

                #region Connection
                try
                {
                    //Connection = stringSource.Substring(stringSource.IndexOf("_count_string\":"), (stringSource.IndexOf(",", stringSource.IndexOf("_count_string\":")) - stringSource.IndexOf("_count_string\":"))).Replace("_count_string\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                    int startindex = stringSource.IndexOf("\"numberOfConnections\":");
                    if (startindex > 0)
                    {
                        string start = stringSource.Substring(startindex).Replace("\"numberOfConnections\":", "");
                        int endindex = start.IndexOf(",");
                        string end = start.Substring(0, endindex).Replace("&amp;", "&").Replace("}", string.Empty);
                        Connection = end;
                    }
                }
                catch (Exception ex)
                {
                }

                if (string.IsNullOrEmpty(Connection))
                {
                    try
                    {
                        List<string> lstConnection = HttpHelper.GetTextDataByTagAndAttributeName(stringSource, "div", "member-connections");
                        if (lstConnection.Count > 0)
                        {
                            Connection = lstConnection[0].Replace(",", ":").Trim();
                        }
                    }
                    catch { }
                }
                #endregion Connection

                #region Recommendation


                try
                {
                    string RecomnedUrl = string.Empty;
                    try
                    {
                        int startindex = stringSource.IndexOf("endorsements?id=");
                        string start = stringSource.Substring(startindex);
                        int endIndex = start.IndexOf("\"mem_pic\":");
                        if (endIndex < 0)
                        {
                            endIndex = start.IndexOf(">");
                        }
                        RecomnedUrl = (start.Substring(0, endIndex).Replace(",", string.Empty).Replace("\"", string.Empty).Replace(":", string.Empty));

                    }
                    catch { }

                    string PageSource = HttpHelper.getHtmlfromUrl1(new Uri("http://www.linkedin.com/profile/profile-v2-" + RecomnedUrl + ""));
                    string[] arrayRecommendedName = Regex.Split(PageSource, "fmt__referrerfullName");

                    if (arrayRecommendedName.Count() == 1)
                    {
                        arrayRecommendedName = Regex.Split(PageSource, "fmt__recommendeeFullName");
                    }


                    List<string> ListRecommendationName = new List<string>();

                    foreach (var itemRecomName in arrayRecommendedName)
                    {
                        try
                        {
                            if (!itemRecomName.Contains("Endorsements"))
                            {
                                string Heading = string.Empty;
                                string Name = string.Empty;

                                try
                                {

                                    int startindex = itemRecomName.IndexOf(":");
                                    string start = itemRecomName.Substring(startindex);
                                    int endIndex = start.IndexOf("\",");
                                    Name = (start.Substring(0, endIndex).Replace("\"", string.Empty).Replace(":", string.Empty).Replace(",", ";")).Trim();
                                }
                                catch { }

                                try
                                {
                                    int startindex1 = itemRecomName.IndexOf("headline");
                                    string start1 = itemRecomName.Substring(startindex1);
                                    int endIndex1 = start1.IndexOf("memberID");
                                    Heading = (start1.Substring(0, endIndex1).Replace("\"", string.Empty).Replace(":", string.Empty).Replace("headline", string.Empty).Replace(",", string.Empty)).Trim();
                                }
                                catch { }

                                if (Name == string.Empty)
                                {
                                    int startindex1 = itemRecomName.IndexOf("recommenderTitle\":");
                                    string start1 = itemRecomName.Substring(startindex1);
                                    int endIndex1 = start1.IndexOf("\",");
                                    Name = (start1.Substring(0, endIndex1).Replace("\"", string.Empty).Replace("recommenderTitle", string.Empty).Replace(":", string.Empty).Replace(",", string.Empty));
                                }

                                ListRecommendationName.Add(Name + " : " + Heading);

                            }
                        }
                        catch { }

                    }

                    foreach (var item in ListRecommendationName)
                    {
                        if (recomandation == string.Empty)
                        {
                            recomandation = item;
                        }
                        else
                        {
                            recomandation += "  -  " + item;
                        }
                    }

                }
                catch { }

                #endregion

                #region Following


                #endregion

                #region Experience
                if (LDS_Experience == string.Empty)
                {
                    try
                    {
                        string[] array = Regex.Split(stringSource, "title_highlight");
                        string exp = string.Empty;
                        string comp = string.Empty;
                        List<string> ListExperince = new List<string>();
                        string SelItem = string.Empty;
                        if (stringSource.Contains("title_highlight"))
                        {
                            foreach (var itemGrps in array)
                            {
                                try
                                {
                                    if (itemGrps.Contains("title_pivot") && !itemGrps.Contains("<!DOCTYPE html")) //">Join
                                    {
                                        try
                                        {

                                            int startindex = itemGrps.IndexOf("\":\"");
                                            string start = itemGrps.Substring(startindex);
                                            int endIndex = start.IndexOf(",");
                                            exp = (start.Substring(0, endIndex).Replace("\"", string.Empty).Replace(":", string.Empty).Replace("&amp", "&").Replace(";", string.Empty).Replace("\\u002d", "-").Replace("name:", string.Empty));

                                        }
                                        catch { }
                                        if ((exp.Contains("strong class")) || (exp.Contains("highlight")) || (string.IsNullOrEmpty(exp)))
                                        {
                                            try
                                            {
                                                int startindex1 = itemGrps.IndexOf("\"title\":");
                                                string start1 = itemGrps.Substring(startindex1).Replace("\"title\":", string.Empty);
                                                int endIndex1 = start1.IndexOf(",");
                                                exp = (start1.Substring(0, endIndex1).Replace("\"", string.Empty).Replace(":", string.Empty).Replace("&amp", "&").Replace(";", string.Empty).Replace("\\u002d", "-").Replace("name:", string.Empty));
                                            }
                                            catch
                                            { }
                                        }

                                        try
                                        {

                                            int startindex1 = itemGrps.IndexOf("companyName");
                                            string start1 = itemGrps.Substring(startindex1);
                                            int endIndex1 = start1.IndexOf(",");
                                            comp = (start1.Substring(0, endIndex1).Replace("\"", string.Empty).Replace("companyName", string.Empty).Replace(":", string.Empty).Replace(";", string.Empty).Replace("\\u002d", "-").Replace("name:", string.Empty));

                                        }
                                        catch { }

                                        if (titlecurrent == string.Empty)
                                        {
                                            titlecurrent = exp;
                                        }

                                        if (companycurrent == string.Empty)
                                        {
                                            companycurrent = comp;
                                        }

                                        ListExperince.Add(exp + ":" + comp);


                                    }
                                }
                                catch { }
                            }

                        }
                        else
                        {
                            array = Regex.Split(stringSource, "<header>");
                            foreach (string tempItem in array)
                            {
                                try
                                {
                                    if (!tempItem.Contains("<!DOCTYPE html>"))
                                    {

                                        List<string> lstExp = objChilkat.GetDataTag(tempItem, "h4");
                                        List<string> lstComp = objChilkat.GetDataTag(tempItem, "h5");
                                        if (lstExp.Count > 0)
                                        {
                                            exp = lstExp[0];
                                        }
                                        if (lstComp.Count > 0)
                                        {
                                            comp = lstComp[0];
                                            if (string.IsNullOrEmpty(comp))
                                            {
                                                comp = lstComp[1];
                                            }
                                        }
                                        if (titlecurrent == string.Empty)
                                        {
                                            titlecurrent = lstExp[0];
                                        }

                                        if (companycurrent == string.Empty)
                                        {
                                            companycurrent = lstComp[0];
                                        }

                                        ListExperince.Add(exp + ":" + comp);

                                    }
                                }
                                catch { }
                            }

                        }
                        foreach (var item in ListExperince)
                        {
                            if (LDS_Experience == string.Empty)
                            {
                                LDS_Experience = item;
                            }
                            else
                            {
                                LDS_Experience += "  -  " + item;
                            }
                        }

                    }

                    catch { }

                    try
                    {
                        if (string.IsNullOrEmpty(titlecurrent))
                        {
                            int StartIndex = stringSource.IndexOf("trk=prof-0-ovw-curr_pos\">");
                            string Start = stringSource.Substring(StartIndex).Replace("trk=prof-0-ovw-curr_pos\">", string.Empty);
                            int EndIndex = Start.IndexOf("</a>");
                            string End = Start.Substring(0, EndIndex).Replace("</a>", string.Empty);
                            titlecurrent = End.Trim();
                        }
                    }
                    catch
                    { }


                }

                #endregion

                #region Group

                try
                {
                    string GroupUrl = string.Empty;
                    try
                    {
                        int startindex = stringSource.IndexOf("templateId\":\"profile_v2_connections");
                        string start = stringSource.Substring(startindex);
                        //int endIndex = start.IndexOf("vsrp_people_res_name");
                        int endIndex = start.IndexOf("}");
                        GroupUrl = (start.Substring(0, endIndex).Replace(",", string.Empty).Replace("\"", string.Empty).Replace("templateId:profile_v2_connectionsurl:", string.Empty));

                    }
                    catch { }

                    string PageSource = HttpHelper.getHtmlfromUrl1(new Uri(GroupUrl));

                    string[] array1 = Regex.Split(PageSource, "groupRegistration?");
                    List<string> ListGroupName = new List<string>();
                    string SelItem = string.Empty;

                    foreach (var itemGrps in array1)
                    {
                        try
                        {
                            if (itemGrps.Contains("?gid=") && !itemGrps.Contains("<!DOCTYPE html")) //">Join
                            {
                                if (itemGrps.IndexOf("?gid=") == 0)
                                {
                                    try
                                    {
                                        int startindex = itemGrps.IndexOf("\"name\":");
                                        string start = itemGrps.Substring(startindex);
                                        int endIndex = start.IndexOf(",");
                                        ListGroupName.Add(start.Substring(0, endIndex).Replace("\"", string.Empty).Replace("amp", string.Empty).Replace("&", string.Empty).Replace(";", string.Empty).Replace("csrfToken", string.Empty).Replace("name:", string.Empty));
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch { }
                    }

                    foreach (var item in ListGroupName)
                    {
                        if (groupscollectin == string.Empty)
                        {
                            groupscollectin = item;
                        }
                        else
                        {
                            groupscollectin += "  -  " + item;
                        }
                    }

                }
                catch { }

                if (string.IsNullOrEmpty(groupscollectin))
                {
                    List<string> lstGroupData = new List<string>();
                    string[] array1 = Regex.Split(stringSource, "link_groups_settings\":?");
                    array1 = array1.Skip(1).ToArray();
                    foreach (string item in array1)
                    {
                        string _item = Utils.getBetween(item, "name\":", ",").Replace("name\":", "").Replace(":", "").Replace("\"", "");
                        lstGroupData.Add(_item);
                    }

                    foreach (var item in lstGroupData)
                    {
                        if (groupscollectin == string.Empty)
                        {
                            groupscollectin = item;
                        }
                        else
                        {
                            groupscollectin += "  -  " + item;
                        }
                    }
                }

                if (string.IsNullOrEmpty(groupscollectin))
                {
                    List<string> lstGroupData = new List<string>();
                    string tempResponse = Utils.getBetween(stringSource, "<div id=\"groups\"", "<div>");
                    lstGroupData = HttpHelper.GetDataTag(tempResponse, "strong");

                    foreach (var item in lstGroupData)
                    {
                        if (groupscollectin == string.Empty)
                        {
                            groupscollectin = item;
                        }
                        else
                        {
                            groupscollectin += "  -  " + item;
                        }
                    }
                }

                #endregion

                #region skill and Expertise
                try
                {
                    string[] strarr_skill = Regex.Split(stringSource, "endorse-item-name-text\"");
                    string[] strarr_skill1 = Regex.Split(stringSource, "fmt__skill_name\"");
                    if (strarr_skill.Count() >= 2)
                    {
                        foreach (string item in strarr_skill)
                        {
                            try
                            {
                                if (!item.Contains("!DOCTYPE html"))
                                {
                                    try
                                    {
                                        //string Grp = item.Substring(item.IndexOf("<"), (item.IndexOf(">", item.IndexOf("<")) - item.IndexOf("<"))).Replace("<", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Replace("\"u002", "-").Trim();
                                        string Grp = item.Substring(item.IndexOf(">"), (item.IndexOf("<", item.IndexOf(">")) - item.IndexOf(">"))).Replace(">", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Replace("\"u002", "-").Trim();
                                        checkgrplist.Add(Grp);
                                        checkgrplist.Distinct().ToList();
                                    }
                                    catch { }
                                }

                            }
                            catch { }
                        }

                        foreach (string item in checkgrplist)
                        {
                            if (string.IsNullOrEmpty(Skill))
                            {
                                Skill = item.Replace("\"u002", "-").Trim();
                            }
                            else
                            {
                                Skill = Skill + "  -  " + item;
                            }
                        }
                    }
                    else
                    {
                        if (strarr_skill1.Count() >= 2)
                        {
                            try
                            {
                                foreach (string skillitem in strarr_skill1)
                                {
                                    if (!skillitem.Contains("!DOCTYPE html"))
                                    {
                                        try
                                        {
                                            string Grp = skillitem.Substring(skillitem.IndexOf(":"), (skillitem.IndexOf("}", skillitem.IndexOf(":")) - skillitem.IndexOf(":"))).Replace(":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                                            checkgrplist.Add(Grp);
                                            checkgrplist.Distinct().ToList();
                                        }
                                        catch { }
                                    }
                                }

                                foreach (string item in checkgrplist)
                                {
                                    if (string.IsNullOrEmpty(Skill))
                                    {
                                        Skill = item;
                                    }
                                    else
                                    {
                                        Skill = Skill + "  -  " + item;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                Skill = Skill.Replace("a href=\";edu", "").Trim();
                #endregion

                #region Pasttitle
                string[] pasttitles = Regex.Split(stringSource, "title_highlight");
                string pstTitlesitem = string.Empty;
                pasttitles = pasttitles.Skip(1).ToArray();
                foreach (string item in pasttitles)
                {
                    try
                    {

                        if (!item.Contains("<!DOCTYPE html>") && !item.Contains("Tip: You can also search by keyword"))
                        {

                            try
                            {
                                string[] Past_Ttl = Regex.Split(item, ",");
                                pstTitlesitem = Past_Ttl[0].Replace(":", string.Empty).Replace("\"", string.Empty).Replace("\\u002d", "-").Replace("&amp;", "&");
                            }
                            catch { }
                            if ((pstTitlesitem.Contains("strong class")) || (pstTitlesitem.Contains("highlight")) || (string.IsNullOrEmpty(pstTitlesitem)))
                            {
                                try
                                {
                                    int startindex1 = item.IndexOf("\"title\":");
                                    string start1 = item.Substring(startindex1).Replace("\"title\":", string.Empty);
                                    int endIndex1 = start1.IndexOf(",");
                                    pstTitlesitem = (start1.Substring(0, endIndex1).Replace("\"", string.Empty).Replace(":", string.Empty).Replace("&amp", "&").Replace(";", string.Empty).Replace("\\u002d", "-").Replace("name:", string.Empty));
                                }
                                catch
                                { }
                            }

                            if (string.IsNullOrEmpty(LDS_PastTitles))
                            {
                                LDS_PastTitles = pstTitlesitem;
                            }
                            else if (LDS_PastTitles.Contains(pstTitlesitem))
                            {
                                continue;
                            }
                            else
                            {
                                LDS_PastTitles = LDS_PastTitles + "  :  " + pstTitlesitem;
                            }

                        }

                    }
                    catch
                    {
                    }
                }
                #endregion

                #region PastTitle written by sharan
                if (string.IsNullOrEmpty(LDS_PastTitles))
                {
                    string pastTitl = string.Empty;

                    try
                    {
                        if (stringSource.Contains("<div id=\"background-experience\""))
                        {
                            string source_for_website = Utils.getBetween(stringSource, "<div id=\"background-experience\"", "</script></div>");
                            string[] website_arr = Regex.Split(source_for_website, "<h4>");

                            //website_arr = website_arr.
                            for (int i = 1; i < website_arr.Length; i++)
                            {
                                string s1 = Utils.getBetween(website_arr[i], "title=", "&amp;");
                                if (s1.Contains("Learn more about this title"))
                                {
                                    s1 = Utils.getBetween(s1, "Learn more about this title\">", "</a>");
                                }
                                string at = Utils.getBetween(website_arr[i], "dir=\"auto\">", "</a>");
                                s1 = s1.Replace("+", " ").Replace("%2F", " ").Replace("%21", "").Replace("%26", "").Replace("%27", "").Replace("%28", "").Replace("%29", "");
                                s1 = "[" + s1 + "-" + at + "]";

                                LDS_PastTitles = LDS_PastTitles + s1 + ";";

                            }

                        }
                    }
                    catch
                    { }
                }

                #endregion

                #region All Company Summary
                //string[] pasttitles = Regex.Split(stringSource, "company_name");
                //string pstTitlesitem = string.Empty;
                string pstDescCompitem = string.Empty;
                pasttitles = pasttitles.Skip(1).ToArray();
                foreach (string item in pasttitles)
                {
                    if (item.Contains("positionId"))
                    {
                        try
                        {
                            int startindex = item.IndexOf(":");
                            if (startindex > 0)
                            {
                                string start = item.Substring(startindex).Replace(":\"", "");
                                int endindex = start.IndexOf("\",");
                                string end = start.Substring(0, endindex);
                                pstTitlesitem = end.Replace(",", ";").Replace("&amp;", "&").Replace("\\u002d", "-");
                            }




                            int startindex1 = item.IndexOf("summary_lb\":\"");
                            if (startindex > 0)
                            {
                                string start1 = item.Substring(startindex1).Replace("summary_lb\":\"", "");
                                int endindex1 = 0;

                                if (start1.Contains("associatedWith"))
                                {
                                    endindex1 = start1.IndexOf("\",\"associatedWith\"");

                                    if (start1.Contains("\"}"))
                                    {
                                        endindex1 = start1.IndexOf("\"}");
                                    }

                                }
                                else if (start1.Contains("\"}"))
                                {
                                    endindex1 = start1.IndexOf("\"}");
                                }


                                string end1 = start1.Substring(0, endindex1);
                                pstDescCompitem = end1.Replace(",", ";").Replace("u002d", "-").Replace("<br>", string.Empty).Replace("\\n", string.Empty).Replace("\\", string.Empty).Replace("&#xf0a7;", "@").Replace("&#x2019;", "'").Replace("&#x2022", "@").Replace("&#x25cf;", "@");

                                if (pstDescCompitem.Contains("\";\"associatedWith"))
                                {
                                    pstDescCompitem = Regex.Split(pstDescCompitem, "\";\"associatedWith")[0];
                                }
                            }

                            if (string.IsNullOrEmpty(LDS_Desc_AllComp))
                            {
                                LDS_Desc_AllComp = pstTitlesitem + "-" + pstDescCompitem + " ++ ";
                            }
                            else
                            {
                                LDS_Desc_AllComp = LDS_Desc_AllComp + pstTitlesitem + "-" + pstDescCompitem + " ++ ";
                            }
                        }
                        catch
                        {
                        }
                    }

                }
                #endregion

                #region AccountType
                try
                {
                    if (stringSource.Contains("has a Premium Account") || stringSource.Contains("Account Holder"))
                    {
                        AccountType = "Premium Account";
                    }
                    else
                    {
                        AccountType = "Basic Account";
                    }
                }
                catch (Exception ex)
                {

                }
                #endregion Email

                #region FullUrl
                try
                {
                    string[] UrlFull = System.Text.RegularExpressions.Regex.Split(Url, "&authType");
                    LDS_UserProfileLink = UrlFull[0];

                    LDS_UserProfileLink = Url;
                    //  LDS_UserProfileLink = stringSource.Substring(stringSource.IndexOf("canonicalUrl\":"), (stringSource.IndexOf(",", stringSource.IndexOf("canonicalUrl\":")) - stringSource.IndexOf("canonicalUrl\":"))).Replace("canonicalUrl\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                }
                catch { }
                #endregion

                #region current  Company website
                try
                {
                    string curentCompanyUrl = Utils.getBetween(stringSource, "Current<", "</a></strong>");
                    curentCompanyUrl = Utils.getBetween(curentCompanyUrl, "<a href=\"", "\" dir=\"auto\">");

                    if (string.IsNullOrEmpty(curentCompanyUrl))
                    {
                        curentCompanyUrl = Utils.getBetween(stringSource + "###", "<h3>Experience</h3>", "###");
                        curentCompanyUrl = Utils.getBetween(curentCompanyUrl, "<a href=\"", "\">");
                    }

                    if (!string.IsNullOrEmpty(curentCompanyUrl))
                    {
                        curentCompanyUrl = "https://www.linkedin.com" + curentCompanyUrl;
                        currentCompanyUrl = curentCompanyUrl;
                        string currentComapnySource = HttpHelper.getHtmlfromUrl(new Uri(curentCompanyUrl));

                        currentCompanyWebsite = Utils.getBetween(currentComapnySource, "<h4>Website</h4>", "</a>");
                        currentCompanyWebsite = Utils.getBetween(currentCompanyWebsite + "###", "rel=\"nofollow\">", "###");
                    }


                }
                catch
                { }

                #endregion

                #region AshleyTaask
                try
                {
                    string PhNo = string.Empty;
                    PhNo = Utils.getBetween(stringSource, "profile-overview", "Contact Info</span>");
                    PhNo = Utils.getBetween(PhNo, "summary=\"Contact Info\"", "</li></ul></div>");
                    PhNo = Utils.getBetween(PhNo + "###", "id=\"phone-view\"><ul><li>", "###");
                    personalPhoneNumber = PhNo;
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }

                if (string.IsNullOrEmpty(personalPhoneNumber))
                {

                    try
                    {
                        string CompanyPhNo = string.Empty;
                        string url = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=" + CompanyName_withAddress + "&key=AIzaSyCahT-qVNgfV-GPIhu715TTTzqegVv3cQ4";
                        string source = HttpHelper.getHtmlfromUrl(new Uri(url));

                        if (source.Contains("\"status\" : \"ZERO_RESULTS\"") && !string.IsNullOrEmpty(currentCompanyWebsite))
                        {
                            url = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=" + currentCompanyWebsite + "&key=AIzaSyCahT-qVNgfV-GPIhu715TTTzqegVv3cQ4";
                            source = HttpHelper.getHtmlfromUrl(new Uri(url));
                        }

                        string place_Id = Utils.getBetween(source, "place_id\"", "\",");
                        place_Id = Utils.getBetween(place_Id + "###", "\"", "###");
                        if (!string.IsNullOrEmpty(place_Id))
                        {
                            string finalUrl = "https://maps.googleapis.com/maps/api/place/details/json?placeid=" + place_Id + "&key=AIzaSyCahT-qVNgfV-GPIhu715TTTzqegVv3cQ4";
                            string finalSource = HttpHelper.getHtmlfromUrl(new Uri(finalUrl));
                            if (finalSource.Contains("formatted_phone_number"))
                            {
                                try
                                {
                                    CompanyPhNo = Utils.getBetween(finalSource, "formatted_phone_number", "\",");
                                    CompanyPhNo = Utils.getBetween(CompanyPhNo + "###", ": \"", "###");
                                    CompanyPhNumber = CompanyPhNo;
                                }
                                catch (Exception ex)
                                { }
                            }
                            if (string.IsNullOrEmpty(CompanyPhNumber))
                            {
                                if (finalSource.Contains("international_phone_number"))
                                {
                                    try
                                    {
                                        CompanyPhNo = Utils.getBetween(finalSource, "international_phone_number\" : \"", "\",");
                                        CompanyPhNumber = CompanyPhNo;
                                    }
                                    catch (Exception ex)
                                    { }

                                }
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex);
                    }

                    CompanyPhNumber = CompanyPhNumber.Replace(",", "").Replace("(", "").Replace(")", "").Replace(".", "").Replace("-", "");
                }

                #endregion

                //  LDS_LoginID = SearchCriteria.LoginID;
                if (string.IsNullOrEmpty(firstname))
                {
                    firstname = "Linkedin Member";
                }

                if (firstname.Contains("Linkedin") || firstname.Contains("LinkedIn"))
                {
                    try
                    {
                        if (stringSource.Contains("noticeMsg"))
                        {
                            titlecurrent = Utils.getBetween(stringSource, "noticeMsg", "</p>");

                            if (titlecurrent.Contains("<p>"))
                            {
                                titlecurrent = titlecurrent.Replace("<p>", "");
                            }
                            if (titlecurrent.Contains(">"))
                            {
                                titlecurrent = Utils.getBetween(titlecurrent + "###", ">", "###");
                            }
                        }
                    }
                    catch
                    { }
                }

                if ((stringSource.Contains("You and this LinkedIn user don&#8217;t know anyone in common")))
                {
                    try
                    {
                        titlecurrent = Utils.getBetween(stringSource, "<h1>", "</h1>");
                        if (titlecurrent.Contains("&#8217;"))
                        {
                            titlecurrent = titlecurrent.Replace("&#8217;", "");
                        }
                    }
                    catch
                    { }

                }

                #region Replace Comma with null

                firstname = firstname.Replace(",", "");
                lastname = lastname.Replace(",", "");
                HeadlineTitle = HeadlineTitle.Replace(",", "");
                titlecurrent = titlecurrent.Replace(",", "");
                companycurrent = companycurrent.Replace(",", "");
                LDS_Desc_AllComp = LDS_Desc_AllComp.Replace(",", "");
                LDS_BackGround_Summary = LDS_BackGround_Summary.Replace(",", "");
                Connection = Connection.Replace(",", "");
                recomandation = recomandation.Replace(",", "");
                Skill = Skill.Replace(",", "");
                LDS_Experience = LDS_Experience.Replace(",", "");
                EducationCollection = EducationCollection.Replace(",", "");
                groupscollectin = groupscollectin.Replace(",", "");
                USERemail = USERemail.Replace(",", "");
                LDS_UserContact = LDS_UserContact.Replace(",", "");
                LDS_PastTitles = LDS_PastTitles.Replace(",", "");
                AllComapny = AllComapny.Replace(",", "");
                location = location.Replace(",", "");
                country = country.Replace(",", "");
                Industry = Industry.Replace(",", "");
                Website = Website.Replace(",", "");
                email_id = email_id.Replace(",", "");

                personalPhoneNumber = personalPhoneNumber.Replace(",", "").Replace("&nbsp;", "");
                CompanyName_withAddress = CompanyName_withAddress.Replace(",", "").Replace("&nbsp;", "");
                CompanyPhNumber = CompanyPhNumber.Replace(",", "").Replace("&nbsp;", "");

                #endregion


                string endName = firstname + " " + lastname;
                //GroupStatus.GroupSpecMem.Add(GroupMemId, endName);
                if (firstname == string.Empty) firstname = "LinkedIn";
                if (lastname == string.Empty || lastname == null) lastname = "Member";
                if (HeadlineTitle == string.Empty) HeadlineTitle = "--";
                if (titlecurrent == string.Empty) titlecurrent = "--";
                if (companycurrent == string.Empty) companycurrent = "--";
                if (LDS_Desc_AllComp == string.Empty) LDS_Desc_AllComp = "--";
                if (LDS_BackGround_Summary == string.Empty) LDS_BackGround_Summary = "--";
                if (Connection == string.Empty) Connection = "--";
                if (recomandation == string.Empty) recomandation = "--";
                if (Skill == string.Empty) Skill = "--";
                if (LDS_Experience == string.Empty) LDS_Experience = "--";
                if (EducationCollection == string.Empty) EducationCollection = "--";
                if (groupscollectin == string.Empty) groupscollectin = "--";
                if (USERemail == string.Empty) USERemail = "--";
                if (LDS_UserContact == string.Empty) LDS_UserContact = "--";
                if (LDS_PastTitles == string.Empty) LDS_PastTitles = "--";
                if (AllComapny == string.Empty) AllComapny = "--";
                if (location == string.Empty) location = "--";
                if (country == string.Empty) country = "--";
                if (Industry == string.Empty) Industry = "--";
                if (Website == string.Empty) Website = "--";
                if (email_id == string.Empty) email_id = "--";


                if (personalPhoneNumber == string.Empty) personalPhoneNumber = "--";
                if (CompanyName_withAddress == string.Empty) CompanyName_withAddress = "--";
                if (CompanyPhNumber == string.Empty) CompanyPhNumber = "--";

                if (fileName.Contains("LinkedIn"))
                {
                    string s1 = string.Empty;
                }

                string LDS_FinalData = TypeOfProfile.Replace(",", ";") + "," + LDS_UserProfileLink.Replace(",", ";") + "," + GroupMemId.Replace(",", ";") + "," + firstname.Replace(",", ";") + "," + lastname.Replace(",", ";") + "," + HeadlineTitle.Replace(",", ";") + "," + titlecurrent.Replace(",", ";") + "," + companycurrent.Replace(",", ";") + "," + currentCompanyUrl.Replace(",", ";") + "," + LDS_BackGround_Summary.Replace(",", ";") + "," + Connection.Replace(",", ";") + "," + recomandation.Replace(",", string.Empty) + "," + Skill.Replace(",", ";") + "," + LDS_Experience.Replace(",", string.Empty) + "," + EducationCollection.Replace(",", ";") + "," + groupscollectin.Replace(",", ";") + "," + USERemail.Replace(",", ";") + "," + LDS_UserContact.Replace(",", ";") + "," + LDS_PastTitles + "," + pastCompany.Replace(",", ";") + "," + location.Replace(",", ";") + "," + country.Replace(",", ";") + "," + Industry.Replace(",", ";") + "," + currentCompanyWebsite.Replace(",", ";") + "," + LDS_LoginID + "," + AccountType + "," + email_id + "," + personalPhoneNumber + "," + CompanyName_withAddress + "," + CompanyPhNumber + ",";

                if (!string.IsNullOrEmpty(firstname))
                {
                    //Log("[ " + DateTime.Now + " ] => [ Data : " + LDS_FinalData + " ]");
                }
                else
                {
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ No Data For URL : " + Url + " ]");
                    GlobusFileHelper.AppendStringToTextfileNewLineWithCarat(Url, Globals.DesktopFolder + "\\UnScrapedList.txt");
                }

                if (SearchCriteria.starter)
                {
                    string tempFinalData = "";
                    try
                    {
                        //tempFinalData = LDS_FinalData.Replace(";", "").Replace(LDS_UserProfileLink, "").Replace(TypeOfProfile, "").Replace(",", "").Replace(LDS_LoginID, "").Trim();
                        tempFinalData = LDS_FinalData.Replace(";", "").Replace(LDS_UserProfileLink, "").Replace(TypeOfProfile, "").Replace(",", "").Trim();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Exception : " + ex.StackTrace + " ]");
                    }
                    if (!string.IsNullOrEmpty(tempFinalData))
                    {
                        if (CheckEmployeeScraper)
                        {
                            string FileName = "CompanyEmployeeScraper";
                            AppFileHelper.AddingLinkedInDataToCSVFileCompanyEmployeeScraper(LDS_FinalData, FileName);
                            return true;
                        }
                        else if (CampaignScraper)
                        {
                            AppFileHelper.AddingLinkedInDataToCSVFile(LDS_FinalData, fileName);
                            return true;
                        }
                        else
                        {
                            AppFileHelper.AddingLinkedInDataToCSVFile(LDS_FinalData, SearchCriteria.FileName);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
            return false;
        }
        public List<string> GettingAllUrl(string PageSource)
        {
            List<string> suburllist = new List<string>();

            try
            {

                if (PageSource.Contains("/profile/view?"))
                {
                    string[] trkArr = Regex.Split(PageSource, "/profile/view?");

                    foreach (string item in trkArr)
                    {
                        try
                        {
                            if (item.Contains("vsrp_people_res_name"))
                            {
                                string url = item.Substring(0, item.IndexOf("\"")).Replace("\"", string.Empty).Replace("vsrp_companies_res_sim", "vsrp_companies_res_name").Trim();
                                string finalurl = "http://www.linkedin.com/profile/view" + url;
                                if (finalurl.Contains("\\u002d"))
                                {
                                    finalurl = finalurl.Replace("\\u002d", "-");
                                }
                                if (finalurl.Contains("\u002d"))
                                {
                                    finalurl = finalurl.Replace("\u002d", "-");
                                }

                                suburllist.Add(finalurl);
                            }
                        }
                        catch
                        {
                        }
                    }

                }

            }
            catch
            {
            }
            suburllist = suburllist.Distinct().ToList();
            return suburllist.Distinct().ToList();
        }

        #region Get Groups
        readonly object lockrThreadControllerGetGroups = new object();
        public void ThreadStartGetGroups()
        {
            try
            {
                int numberOfAccountPatch = 25;

                if (GlobalsGroups.NoOfThreadsGetGroups > 0)
                {
                    numberOfAccountPatch = GlobalsGroups.NoOfThreadsGetGroups;
                }

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (LDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Split(LDGlobals.listAccounts, numberOfAccountPatch);

                    foreach (List<string> listAccounts in list_listAccounts)
                    {

                        foreach (string account in listAccounts)
                        {
                            try
                            {
                                lock (lockrThreadControllerGetGroups)
                                {
                                    try
                                    {
                                        if (GlobalsGroups.countThreadControllerGetGroups >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerGetGroups);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        LinkedinUser item = null;
                                        LDGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {
                                            Thread profilerThread = new Thread(StartMultiThreadsGetGroups);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;

                                            profilerThread.Start(new object[] { item });

                                            GlobalsGroups.countThreadControllerGetGroups++;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        // GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
        public void StartMultiThreadsGetGroups(object parameters)
        {
            try
            {
                if (!GlobalsGroups.isStopGroupStatus)
                {
                    try
                    {
                        GlobalsGroups.lstThreadsGroupStatus.Add(Thread.CurrentThread);
                        GlobalsGroups.lstThreadsGroupStatus.Distinct();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {
                        Array paramsArray = new object[1];
                        paramsArray = (Array)parameters;

                        LinkedinUser objLinkedinUser = (LinkedinUser)paramsArray.GetValue(0);

                        if (!objLinkedinUser.isloggedin)
                        {
                            GlobusHttpHelper objGlobusHttpHelper = objLinkedinUser.globusHttpHelper;

                            //Login Process
                            Accounts.AccountManager objAccountManager = new AccountManager();
                            objAccountManager.LoginHttpHelper(ref objLinkedinUser);
                        }

                        if (objLinkedinUser.isloggedin)
                        {
                            // Call StartActionMessageReply
                            StartGetGroups(ref objLinkedinUser);
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Couldn't Login With Username : " + objLinkedinUser.username);
                            GlobusLogHelper.log.Debug("Couldn't Login With Username : " + objLinkedinUser.username);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            finally
            {
                try
                {
                    // if (!isStopWallPoster)
                    {
                        lock (lockrThreadControllerGetGroups)
                        {
                            GlobalsGroups.countThreadControllerGetGroups = GlobalsGroups.countThreadControllerGetGroups--;
                            Monitor.Pulse(lockrThreadControllerGetGroups);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }
        public void StartGetGroups(ref LinkedinUser objLinkedinUser)
        {
            try
            {
                Dictionary<string, string> dicGroup = new Dictionary<string, string>();
                GlobusLogHelper.log.Info("Finding Groups...");
                dicGroup = GetGroups(ref objLinkedinUser);

                GlobalsGroups.lstGroups.Clear();

                foreach (var item in dicGroup)
                {
                    GlobalsGroups.lstGroups.Add(Regex.Split(item.Key, "<:><:>")[0]);
                }

                BindingGroupNames();
            }
            catch (Exception ex)
            {
            }
        }

        public static Events objEvents = new Events();
        public void BindingGroupNames()
        {
            EventsArgs objEventsArgs = new EventsArgs("how where");
            objEvents.BindGroupMembersScraperToCheckedListBox(objEventsArgs);
        }
        public Dictionary<string, string> GetGroups(ref LinkedinUser objLinkedInUser)
        {
            string pageSource = string.Empty;
            GlobusHttpHelper HttpHelper = objLinkedInUser.globusHttpHelper;
            Dictionary<string, string> GroupName = new Dictionary<string, string>();
            try
            {
                string url = "https://www.linkedin.com";
                pageSource = HttpHelper.getHtmlfromUrl(new Uri(url));

                string[] arr = Regex.Split(pageSource, "<h3>");
                string profId = Utils.getBetween(arr[2], "<a href=\"", "\"");
                pageSource = HttpHelper.getHtmlfromUrl(new Uri(profId));

                string[] arr1 = Regex.Split(pageSource, "<h3>");
                string profileId = Utils.getBetween(arr1[2], "<a href=\"", "\"");
                pageSource = HttpHelper.getHtmlfromUrl(new Uri(profileId));

                string[] arr1_groups = Regex.Split(pageSource, "link_groups_settings");
                arr1_groups = arr1_groups.Skip(1).ToArray();

                if (arr1_groups.Length > 0)
                {
                    GlobusLogHelper.log.Info(arr1_groups.Length + " Groups Found");
                }

                foreach (string item in arr1_groups)
                {
                    string grpName = string.Empty;
                    string grpId = string.Empty;

                    grpName = Utils.getBetween(item, "name\":\"", "\",\"");
                    grpName = grpName.Replace(":", "");
                    grpId = Utils.getBetween(item, "&gid=", "&");

                    Thread.Sleep(500);
                    GlobusLogHelper.log.Info(grpName);

                    grpName = grpName + "<:><:>" + objLinkedInUser.username;
                    if (NumberHelper.ValidateNumber(grpId))
                    {
                        GroupName.Add(grpName, grpId);
                    }
                }

                if (GroupName.Count == 0)
                {
                    try
                    {
                        string url_to_get_Groups = string.Empty;
                        url_to_get_Groups = Utils.getBetween(pageSource, "<div id=\"groups-container", "</div>");
                        url_to_get_Groups = Utils.getBetween(url_to_get_Groups, "url\":\"", "\"})");

                        pageSource = HttpHelper.getHtmlfromUrl(new Uri(url_to_get_Groups));

                        arr1_groups = Regex.Split(pageSource, "link_groups_settings");
                        arr1_groups = arr1_groups.Skip(1).ToArray();

                        foreach (string item in arr1_groups)
                        {
                            try
                            {
                                string grpName = string.Empty;
                                string grpId = string.Empty;

                                grpName = Utils.getBetween(item, "name\":\"", "\",\"");
                                grpName = grpName.Replace(":", "");
                                grpId = Utils.getBetween(item, "&gid=", "&");

                                grpName = grpName + "<:><:>" + objLinkedInUser.username;
                                if (NumberHelper.ValidateNumber(grpId))
                                {
                                    GroupName.Add(grpName, grpId);
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return GroupName;
        }
        #endregion
    }
}
