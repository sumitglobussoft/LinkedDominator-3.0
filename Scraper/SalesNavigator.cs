using Accounts;
using BaseLib;
using Globussoft;
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
    public class SalesNavigator
    {
     
        #region Global variable declaration


        public static string whenJoined = string.Empty;

        public static string seniorityLevel = string.Empty;
        public static string companySize = string.Empty;
        public static string function = string.Empty;
        public static string yearOfExperience = string.Empty;


        public static string keyword = string.Empty;
        public static string title = string.Empty;
        public static string titleScope = string.Empty;


        public static string firstName = string.Empty;
        public static string lastName = string.Empty;
        public static string selectedEmailId = string.Empty;
        public static string country = string.Empty;
        public static string location = string.Empty;
        public static string postalCode = string.Empty;
        public static string within = string.Empty;
        public static string currentCompany = string.Empty;


        public static string language = string.Empty;
        public static string relationship = string.Empty;
        public static string industry = string.Empty;   


        #endregion

        List<string> lstProfileUrls = new List<string>();
        readonly object lockrThreadControllerSalesNavigator = new object();
        public void ThreadStartSalesNavigator()
        {
            try
            {
                int numberOfAccountPatch = 25;

                if (GlobalsScraper.NoOfThreadsSalesNavigator > 0)
                {
                    numberOfAccountPatch = GlobalsScraper.NoOfThreadsSalesNavigator;
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
                                lock (lockrThreadControllerSalesNavigator)
                                {
                                    try
                                    {
                                        if (GlobalsScraper.countThreadControllerSalesNavigator >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerSalesNavigator);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        LinkedinUser item = null;
                                        LDGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {
                                            Thread profilerThread = new Thread(StartMultiThreadsSalesNavigator);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;

                                            profilerThread.Start(new object[] { item });

                                            GlobalsScraper.countThreadControllerSalesNavigator++;
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
        public void StartMultiThreadsSalesNavigator(object parameters)
        {
            try
            {
                if (!GlobalsScraper.isStopSalesNavigator)
                {
                    try
                    {
                        GlobalsScraper.lstThreadsSalesNavigator.Add(Thread.CurrentThread);
                        GlobalsScraper.lstThreadsSalesNavigator.Distinct();
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
                            StartSalesNavigator(ref objLinkedinUser);
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
                        lock (lockrThreadControllerSalesNavigator)
                        {
                            GlobalsScraper.countThreadControllerSalesNavigator = GlobalsScraper.countThreadControllerSalesNavigator--;
                            Monitor.Pulse(lockrThreadControllerSalesNavigator);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        public void StartSalesNavigator(ref LinkedinUser objLinkedinUser)
        {
            try
            {
                GlobusHttpHelper objHttpHelper = objLinkedinUser.globusHttpHelper;
                string mainUrl = string.Empty;
                mainUrl = FormURL(ref objLinkedinUser);
               
                Pagination(ref objHttpHelper, mainUrl);
                //ScrapeProfileDetails(ref objHttpHelper);
                try
                {
                    string[] profileUrls = lstProfileUrls.ToArray();
                    System.IO.File.WriteAllLines(FilePath.profileUrlsSalesNavigatorScraper, profileUrls);
                }
                catch
                { }
            }
            catch (Exception ex)
            {
            }
        }

        public void Pagination(ref GlobusHttpHelper objHttpHelper, string mainUrl)
        {
            int PageCunt = 1;
            try
            {
                string totalResults = string.Empty;
                bool dispTotalResults = true;
                string mainPageResponse = string.Empty;
                int paginationCounter = 0;
                do
                {
                    //mainUrl = mainUrl.Replace("replaceVariableCounter", paginationCounter.ToString());

                    //if (SalesNavigatorGlobals.isStop)
                    //{
                    //    return;
                    //}

                    ///string hoMEuRL = "https://www.linkedin.com/sales/search/?facet=N&facet.N=O&facet=G&facet.G=in:7350&facet=I&facet.I=96&facet=FA&facet.FA=12&defaultSelection=false&start=0&count=10&searchHistoryId=1540160093&keywords=ITIL&trk=lss-search-tab";
                    // string Url = mainUrl.Replace("replaceVariableCounter", paginationCounter.ToString());
                    mainPageResponse = objHttpHelper.getHtmlfromUrl(new Uri(mainUrl));
                    if (string.IsNullOrEmpty(mainPageResponse))
                    {
                        mainPageResponse = objHttpHelper.getHtmlfromUrl1(new Uri("https://www.linkedin.com/sales/?trk=nav_responsive_sub_nav_upgrade"));
                        mainPageResponse = objHttpHelper.getHtmlfromUrl(new Uri(mainUrl));
                    }

                    //if (mainPageResponse.Contains("We'll be back soon.")&&(mainPageResponse.Contains("We're getting things cleaned up.")))
                    //{
                    //    string unwatedStr = "https://www.linkedin.com/sales/search/?facet=N&facet.N=O&facet=G&facet.G=in:7350&facet=I&facet.I=96&facet=FA&facet.FA=12&defaultSelection=false&start=0&count=10&searchHistoryId=1540160093&keywords="+""+"&trk=lss-search-tab";// "&countryCode=" + Utils.getBetween(mainUrl, "&countryCode=", "&");
                    //    mainPageResponse = objHttpHelper.getHtmlfromUrl(new Uri(mainUrl.Replace(unwatedStr, paginationCounter.ToString())));
                    //}


                    if (string.IsNullOrEmpty(mainPageResponse))
                    {
                        if (string.IsNullOrEmpty(mainPageResponse))
                        {
                            //MessageBox.Show("Null response from internet. Please check your internet connection and restart the software.");
                            GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ No response from internet. Please check your internet connection. ] ");
                        }
                        Thread.Sleep(2000);
                        mainPageResponse = objHttpHelper.getHtmlfromUrl(new Uri(mainUrl.Replace("replaceVariableCounter", paginationCounter.ToString())));

                    }
                    try
                    {
                        if (dispTotalResults)
                        {
                            totalResults = Utils.getBetween(mainPageResponse, "\"total\":", ",\"").Trim();
                            GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Total results found : " + totalResults + " ]");
                            GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Scraping profile Url ]");
                            dispTotalResults = false;
                        }
                    }
                    catch
                    { }
                    int checkCountUrls = 0;
                    string[] profileUrl_Split = Regex.Split(mainPageResponse, "\"profileUrl\"");
                    List<string> ProfileList = new List<string>();
                    foreach (string profileUrlItem in profileUrl_Split)
                    {
                        if (!profileUrlItem.Contains("<!DOCTYPE"))
                        {
                            checkCountUrls++;
                            string profileUrl = Utils.getBetween(profileUrlItem, ":\"", "\",\"");
                            lstProfileUrls.Add(profileUrl);
                            GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Scraped Url : " + profileUrl + " ] ");
                            ProfileList.Add(profileUrl);

                        }
                    }
                    Thread.Sleep(1000);
                    ScrapeProfileDetails(ref objHttpHelper, ProfileList);
                    int startindex = paginationCounter;
                    paginationCounter = paginationCounter + 100;
                    mainUrl = CreatePaginationUrl(mainUrl, startindex, paginationCounter);
                } while (mainPageResponse.Contains("\"profileUrl\":\""));
            }
            catch (Exception ex)
            {
            }
        }

        public string FormURL(ref LinkedinUser objLinkedinUser)
        {
            GlobusHttpHelper objHttpHelper = objLinkedinUser.globusHttpHelper;
            string url = "https://www.linkedin.com/sales/search/?";
            try
            {
                if (!string.IsNullOrEmpty(currentCompany))
                {
                    url = url + "&company=" + currentCompany;
                    // url = url + "&companyScope=" + GlobalsScraper.selectedCurrentPast_Input_SalesNav;
                }


                if (!string.IsNullOrEmpty(relationship))
                {
                    int i = 0;
                    string[] rawRelationship = Regex.Split(relationship, ",");
                    string addRelationshipValue = "facet=N";
                    for (i = 0; i < rawRelationship.Count(); i++)
                    {
                        addRelationshipValue = addRelationshipValue + "&facet.N=" + rawRelationship[i];
                    }
                    url = url + addRelationshipValue;
                }
                if (!string.IsNullOrEmpty(location))
                {
                    string locationResponse = objHttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com/ta/region?query=" + Uri.EscapeDataString(location)));
                    string rawLocationValue = Utils.getBetween(locationResponse, "\"id\":\"", "\",\"");
                    url = url + "&facet=G&facet.G=" + rawLocationValue;
                }
                if (!string.IsNullOrEmpty(title))
                {
                    url = url + "&jobTitle=" + title;
                    
                }
                if (!string.IsNullOrEmpty(titleScope))
                {
                    url = url + "&titleScope=" + titleScope;
                }
                    
                if (!string.IsNullOrEmpty(industry))
                {
                    int i = 0;
                    string[] rawIndustry = Regex.Split(industry, ",");
                    string addIndustryValue = "&facet=I";
                    for (i = 0; i < rawIndustry.Count(); i++)
                    {
                        addIndustryValue = addIndustryValue + "&facet.I=" + rawIndustry[i];
                    }
                    url = url + addIndustryValue;
                }
                if (!string.IsNullOrEmpty(country))
                {
                    //  url = url + "&countryCode=" + SalesNavigatorGlobals.country;
                }
                if (!string.IsNullOrEmpty(within))
                {
                    url = url + "&radiusMiles=" + within;
                }
                if (!string.IsNullOrEmpty(postalCode))
                {
                    url = url + "&postalCode=" + postalCode;
                }
                if (!string.IsNullOrEmpty(firstName))
                {
                    url = url + "&firstName=" + firstName;
                }
                if (!string.IsNullOrEmpty(lastName))
                {
                    url = url + "&lastName=" + lastName;
                }
                if (!string.IsNullOrEmpty(seniorityLevel))
                {
                    int i = 0;
                    string[] rawSeniorityLevel = Regex.Split(seniorityLevel, ",");
                    string addSeniorityValue = "&facet=SE";
                    for (i = 0; i < rawSeniorityLevel.Count(); i++)
                    {
                        addSeniorityValue = addSeniorityValue + "&facet.SE=" + rawSeniorityLevel[i];
                    }
                    url = url + addSeniorityValue;
                }
                if (!string.IsNullOrEmpty(function))
                {
                    int i = 0;
                    string[] rawFunction = Regex.Split(function, ",");
                    string addFunctionValue = "&facet=FA";
                    for (i = 0; i < rawFunction.Count(); i++)
                    {
                        addFunctionValue = addFunctionValue + "&facet.FA=" + rawFunction[i];
                    }
                    url = url + addFunctionValue;
                }
                if (!string.IsNullOrEmpty(companySize))
                {
                    int i = 0;
                    string[] rawCompanySize = Regex.Split(companySize, ",");
                    string addCompanySizeValue = "&facet=CS";
                    for (i = 0; i < rawCompanySize.Count(); i++)
                    {
                        addCompanySizeValue = addCompanySizeValue + "&facet.CS=" + rawCompanySize[i];
                    }
                    url = url + addCompanySizeValue;
                }
                if (!string.IsNullOrEmpty(yearOfExperience))
                {
                    int i = 0;
                    string[] rawYearsOfExperience = Regex.Split(yearOfExperience, ",");
                    string addYearsOfExperienceValue = "&facet=TE";
                    for (i = 0; i < rawYearsOfExperience.Count(); i++)
                    {
                        addYearsOfExperienceValue = addYearsOfExperienceValue + "&facet.TE=" + rawYearsOfExperience[i];
                    }
                    url = url + addYearsOfExperienceValue;
                }
                if (!string.IsNullOrEmpty(language))
                {
                    int i = 0;
                    string[] rawlanguage = Regex.Split(language, ",");
                    string addLanguageValue = "&facet=L";
                    for (i = 0; i < rawlanguage.Count(); i++)
                    {
                        addLanguageValue = addLanguageValue + "&facet.L=" + rawlanguage[i];
                    }
                    url = url + addLanguageValue;
                }
                if (!string.IsNullOrEmpty(whenJoined))
                {
                    int i = 0;
                    string[] rawWhenJoined = Regex.Split(whenJoined, ",");
                    string addWhenJoinedValue = "&facet=DR";
                    for (i = 0; i < rawWhenJoined.Count(); i++)
                    {
                        addWhenJoinedValue = addWhenJoinedValue + "&facet.DR=" + rawWhenJoined[i];
                    }
                    url = url + addWhenJoinedValue;
                }
                //url = url + "&defaultSelection=false&start=replaceVariableCounter&count=100";
                url = url + "&defaultSelection=false&start=0&count=10&searchHistoryId=1540160093";


                if (!string.IsNullOrEmpty(keyword))
                {
                    url = url + "&keywords=" + Uri.EscapeDataString(keyword) + "&trk=lss-search-tab";
                }
                else
                {
                    url = url + "&trk=lss-search-tab";
                }
            }
            catch (Exception ex)
            {
            }
            return url;
        }

        public Dictionary<string, string> CheckDuplicate = new Dictionary<string, string>();
        public void ScrapeProfileDetails(ref GlobusHttpHelper objHttpHelper, List<string> ProfileUrls)
        {
            foreach (string profileURL in ProfileUrls)
            {
                try
                {
                    CheckDuplicate.Add(profileURL, profileURL);
                }
                catch (Exception)
                {
                    continue;
                }

                string name = string.Empty;
                string memberID = string.Empty;
                string imageUrl = string.Empty;
                string connection = string.Empty;
                string location = string.Empty;
                string industry = string.Empty;
                string headlineTitle = string.Empty;
                string currentTitle = string.Empty;
                string pastTitles = string.Empty;
                string currentCompany = string.Empty;
                string pastCompanies = string.Empty;
                string skills = string.Empty;
                string numberOfConnections = string.Empty;
                string education = string.Empty;
                string email = string.Empty;
                string phoneNumber = string.Empty;
                //if (SalesNavigatorGlobals.isStop)
                //{
                //    return;
                //}
                try
                {
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Scraping profile details of profile url : " + profileURL + " ]");

                    string pgSource = objHttpHelper.getHtmlfromUrl(new Uri(profileURL));
                    if (string.IsNullOrEmpty(pgSource))
                    {
                        pgSource = objHttpHelper.getHtmlfromUrl(new Uri(profileURL));
                    }
                    if (!pgSource.Contains("\"profile\":"))
                    {
                        Thread.Sleep(2000);
                        pgSource = objHttpHelper.getHtmlfromUrl(new Uri(profileURL));
                    }

                    name = GetName(pgSource);

                    memberID = Utils.getBetween(profileURL, "profile/", ",").Trim();

                    imageUrl = GetImageUrl(pgSource);

                    email = GetEmail(pgSource);

                    phoneNumber = GetPhoneNumber(pgSource);

                    connection = GetConnection(pgSource);

                    location = GetLocation(pgSource);

                    industry = GetIndustry(pgSource);

                    headlineTitle = GetHeadlineTitle(pgSource);

                    headlineTitle = headlineTitle.Replace("\\u002d", string.Empty);

                    string allTitles = GetAllTitle(pgSource).Replace("d/b/a", string.Empty).Replace("&amp;", string.Empty); //title at company : title at company : title at company

                    try
                    {
                        string[] titles = Regex.Split(allTitles, " : ");

                        currentTitle = Utils.getBetween(titles[0], "", " at ");

                        foreach (string item in titles)
                        {
                            if (string.IsNullOrEmpty(pastTitles))
                            {
                                pastTitles = Utils.getBetween(item, "", " at ");
                            }
                            else
                            {
                                pastTitles = pastTitles + ":" + Utils.getBetween(item, "", " at ");
                            }
                        }


                        currentCompany = Utils.getBetween(titles[0] + "@", " at ", "@").Replace("d/b/a", string.Empty);

                        foreach (string item in titles)
                        {
                            if (string.IsNullOrEmpty(pastCompanies))
                            {
                                pastCompanies = Utils.getBetween(item + "@", " at ", "@").Replace("d/b/a", string.Empty);
                            }
                            else
                            {
                                if (!pastCompanies.Contains(Utils.getBetween(item + "@", " at ", "@")))
                                {
                                    pastCompanies = pastCompanies + ":" + Utils.getBetween(item + "@", " at ", "@").Replace("d/b/a", string.Empty);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    skills = GetSkills(pgSource);

                    numberOfConnections = GetNumberOfConnections(pgSource);

                    education = GetEducation(pgSource);
                }
                catch (Exception ex)
                {
                }
                lstProfileUrls.Remove(profileURL);

                WriteDataToCSV(name, profileURL, memberID, connection, location, industry, headlineTitle, currentTitle, pastTitles, currentCompany, pastCompanies, skills, numberOfConnections, education, email, phoneNumber);
            }
        }
        public string GetName(string source)
        {
            string name = string.Empty;
            try
            {
                string rawName = Utils.getBetween(source, "\"profile\":", "\"numConnections\"");
                name = Utils.getBetween(rawName, "\"fullName\":\"", "\",");

                if (string.IsNullOrEmpty(name))
                {
                    string fName = Utils.getBetween(source, "\"firstName\":\"", "\",\"");
                    string lName = Utils.getBetween(source, "\"lastName\":\"", "\",\"");
                    name = fName + " " + lName;
                }
            }
            catch (Exception ex)
            {
            }
            return name;
        }
        public string CreatePaginationUrl(string Url, int startindex, int counter)
        {
            string PaginationUrl = string.Empty;
            try
            {
                string Oldstartindex = Utils.getBetween(Url, "start=", "&");
                string Oldcounter = Utils.getBetween(Url, "count=", "&");

                string Urlllll = Url.Replace("start=" + Oldstartindex + "&", "start=" + startindex.ToString() + "&").Replace("count=" + Oldcounter + "&", "count=" + counter.ToString() + "&");
                PaginationUrl = Urlllll;
            }
            catch { }
            return PaginationUrl;
        }
        public string GetImageUrl(string source)
        {
            string imageUrl = string.Empty;
            try
            {
                imageUrl = Utils.getBetween(source, "\"imageUri\":\"", "\",\"");
            }
            catch (Exception ex)
            {
            }
            return imageUrl;
        }

        public string GetEmail(string source)
        {
            string email = string.Empty;
            try
            {
                string rawEmail = Utils.getBetween(source, "\"profile\":", "\"industry\":\"");
                email = Utils.getBetween(rawEmail, "emails\":[\"", "\"],");
            }
            catch (Exception ex)
            {
            }
            return email;
        }

        public string GetPhoneNumber(string source)
        {
            string phoneNumber = string.Empty;
            try
            {
                string rawPhoneNumber = Utils.getBetween(source, "\"profile\":", "\"industry\":\"");
                phoneNumber = Utils.getBetween(rawPhoneNumber, "\"phones\":[\"", "\"],");
            }
            catch (Exception ex)
            {
            }
            return phoneNumber;
        }

        public string GetIndustry(string source)
        {
            string industry = string.Empty;
            try
            {
                industry = Utils.getBetween(source, "\"industry\":\"", "\",\"");
            }
            catch (Exception ex)
            {
            }
            return industry;
        }

        public string GetHeadlineTitle(string source)
        {
            string headlineTitle = string.Empty;
            try
            {
                string rawHeadLineTitle = Utils.getBetween(source, "\"profile\":", "\"numConnections\"");
                headlineTitle = Utils.getBetween(rawHeadLineTitle, "\"headline\":\"", "\",\"");
            }
            catch (Exception ex)
            {
            }
            return headlineTitle;
        }

        public string GetAllTitle(string source)
        {
            string allTitles = string.Empty;
            try
            {
                List<string> lstAllTitles = new List<string>();
                string[] title_split = Regex.Split(source, "position\":");
                foreach (string item in title_split)
                {
                    if (!item.Contains("<!DOCTYPE"))
                    {
                        string companies = Utils.getBetween(item, "\"companyName\":\"", "\",\"").Replace("&amp;", "&");
                        string titles = Utils.getBetween(item, "\"title\":\"", "\",\"").Replace("&amp;", "&");
                        if (!string.IsNullOrEmpty(companies))
                        {
                            lstAllTitles.Add(titles + " at " + companies);
                        }
                    }
                }

                lstAllTitles = lstAllTitles.Distinct().ToList();
                foreach (string item in lstAllTitles)
                {
                    if (string.IsNullOrEmpty(allTitles))
                    {
                        allTitles = item;
                    }
                    else
                    {
                        allTitles = allTitles + " : " + item;
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return allTitles;
        }

        public string GetPastTitles(string source)
        {
            string pastTitles = string.Empty;
            try
            {

            }
            catch (Exception ex)
            {
            }
            return pastTitles;
        }

        public string GetCurrentCompany(string source)
        {
            string currentCompany = string.Empty;
            try
            {

            }
            catch (Exception ex)
            {
            }
            return currentCompany;
        }

        public string GetPastCompanies(string source)
        {
            string pastCompanies = string.Empty;
            try
            {

            }
            catch (Exception ex)
            {
            }
            return pastCompanies;
        }

        public string GetSkills(string source)
        {
            string skills = string.Empty;
            try
            {
                skills = Utils.getBetween(source, "skills\":[\"", "\"],").Replace("\",\"", ":");
            }
            catch (Exception ex)
            {
            }
            return skills;
        }

        public string GetAdditionalInfo(string source)
        {
            string additionalInfo = string.Empty;
            try
            {

            }
            catch (Exception ex)
            {
            }
            return additionalInfo;
        }

        public string GetNumberOfConnections(string source)
        {
            string numberOfConnection = string.Empty;
            try
            {
                numberOfConnection = Utils.getBetween(source, "\"numConnections\":", ",\"");
            }
            catch (Exception ex)
            {
            }
            return numberOfConnection;
        }

        public string GetEducation(string source)
        {
            string education = string.Empty;
            try
            {
                List<string> lstEducation = new List<string>();
                string rawEducation = Utils.getBetween(source, "[{\"educationId\"", "]},\"");
                string[] educationSplit = Regex.Split(rawEducation, "\"endDateMonth\"");
                foreach (string item in educationSplit)
                {
                    if (item.Contains("degree"))
                    {
                        string degree = Utils.getBetween(item, "\"degree\":\"", "\",\"");
                        string school = Utils.getBetween(item, "\"schoolName\":\"", "\",\"");
                        string field = Utils.getBetween(item, "\"fieldOfStudy\":\"", "\",\"");
                        string startYear = Utils.getBetween(item, "startDateYear\":", ",\"");
                        string endYear = Utils.getBetween(item, "endDateYear\":", ",\"");
                        string edu = "[Degree:" + degree + "," + field + "  School:" + school + " {" + startYear + "-" + endYear + "}]";
                        lstEducation.Add(edu);
                    }
                }
                foreach (string item in lstEducation)
                {
                    if (string.IsNullOrEmpty(education))
                    {
                        education = item;
                    }
                    else
                    {
                        education = education + "::" + item;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return education;
        }

        public string GetConnection(string source)
        {
            string connection = string.Empty;
            try
            {
                string rawConnection = Utils.getBetween(source, "\"profile\":", "\"numConnections\"");
                connection = Utils.getBetween(rawConnection, "\",\"degree\":", ",\"");
            }
            catch (Exception ex)
            {
            }
            return connection;
        }

        public string GetLocation(string source)
        {
            string location = string.Empty;
            try
            {
                string rawLocation = Utils.getBetween(source, "\"profile\":", "authToken\":\"");
                location = Utils.getBetween(rawLocation, "\"location\":\"", "\",\"");
            }
            catch (Exception ex)
            {
            }
            return location;
        }

        public void WriteDataToCSV(string name, string profileUrl, string memberID, string connection, string location, string industry, string headlineTitle, string currentTitle, string pastTitles, string currentCompany, string pastCompany, string skills, string numberOfConnections, string education, string email, string phoneNumber)
        {
            try
            {
                string loginId = string.Empty;
                GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Profile details saved in CSV file of profile URL : " + profileUrl + " ]");

                if (name.Trim() == string.Empty) name = "LinkedIn member";
                if (profileUrl.Trim() == string.Empty) profileUrl = "--";
                if (connection.Trim() == string.Empty) connection = "--";
                if (location.Trim() == string.Empty) location = "--";
                if (industry.Trim() == string.Empty) industry = "--";
                if (headlineTitle.Trim() == string.Empty) headlineTitle = "--";
                if (currentTitle.Trim() == string.Empty) currentTitle = "--";
                if (pastTitles.Trim() == string.Empty) pastTitles = "--";
                if (currentCompany.Trim() == string.Empty) currentCompany = "--";
                if (pastCompany.Trim() == string.Empty) pastCompany = "--";
                if (skills.Trim() == string.Empty) skills = "--";
                if (numberOfConnections.Trim() == string.Empty) numberOfConnections = "--";
                if (education.Trim() == string.Empty) education = "--";
                if (email.Trim() == string.Empty) email = "--";
                if (phoneNumber.Trim() == string.Empty) phoneNumber = "--";


                string Header = "Profile name" + "," + "Profile URL" + "," + "Member ID" + "," + "Degree of connection" + "," + "Location" + "," + "Industry" + "," + "Headline title" + "," + "Current title" + "," + "Past titles" + "," + "Current company" + "," + "Past company" + "," + "Skills" + "," + "Number of connections" + "," + "Education" + "," + "Email" + "," + "Phone number" + "," + "Account Used" + ",";
                string LDS_FinalData = name.Replace(",", ";") + "," + profileUrl.Replace(",", ";") + "," + memberID.Replace(",", ";") + "," + connection.Replace(",", ";") + "," + location.Replace(",", ";") + "," + industry.Replace(",", ";") + "," + headlineTitle.Replace(",", ";") + "," + currentTitle.Replace(",", ";") + "," + pastTitles.Replace(",", ";") + "," + currentCompany.Replace(",", ";") + "," + pastCompany.Replace(",", ";") + "," + skills.Replace(",", ";") + "," + numberOfConnections.Replace(",", ";") + "," + education.Replace(",", ";") + "," + email.Replace(",", ";") + "," + phoneNumber.Replace(",", ";") + "," + loginId.Replace(",", ";");
                string FileName = "SalesNavigatorScraper";
                AppFileHelper.SalesNavigatorScraperWriteToCSV(LDS_FinalData, Header, FileName);
            }
            catch (Exception ex)
            {
            }
        }





    }
}
