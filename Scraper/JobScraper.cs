using Accounts;
using BaseLib;
using Globussoft;
using LinkDominator;
using linkedDominator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper
{
    public class JobScraper
    {
        List<string> RecordURL = new List<string>();
        Queue<string> queRecordUrl = new Queue<string>();
        GlobusHttpHelper _HttpHelper = new GlobusHttpHelper();
        public bool JobScraperLimitCrossed = false;
        int LimitCount = 0;

        readonly object lockrThreadControllerJobScraper = new object();
        public void ThreadStartJobScraper()
        {
            try
            {
                int numberOfAccountPatch = 25;

                if (GlobalsScraper.NoOfThreadsJobScraper > 0)
                {
                    numberOfAccountPatch = GlobalsScraper.NoOfThreadsJobScraper;
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
                                lock (lockrThreadControllerJobScraper)
                                {
                                    try
                                    {
                                        if (GlobalsScraper.countThreadControllerJobScraper >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerJobScraper);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        LinkedinUser item = null;
                                        LDGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {
                                            Thread profilerThread = new Thread(StartMultiThreadsJobScraper);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;

                                            profilerThread.Start(new object[] { item });

                                            GlobalsScraper.countThreadControllerJobScraper++;
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
        public void StartMultiThreadsJobScraper(object parameters)
        {
            try
            {
                if (!GlobalsScraper.isStopJobScraper)
                {
                    try
                    {
                        GlobalsScraper.lstThreadsJobScraper.Add(Thread.CurrentThread);
                        GlobalsScraper.lstThreadsJobScraper.Distinct();
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
                            foreach (var itemJobUrl in GlobalsScraper.lstUrlJobScraper)
                            {
                                StartJobScraper(ref objLinkedinUser, itemJobUrl, GlobalsScraper.limitToScrape);
                            }
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
                        lock (lockrThreadControllerJobScraper)
                        {
                            GlobalsScraper.countThreadControllerJobScraper = GlobalsScraper.countThreadControllerJobScraper--;
                            Monitor.Pulse(lockrThreadControllerJobScraper);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        public void StartJobScraper(ref LinkedinUser objLinkedinUser, string JobUrl, int MaxLimitCount)
        {
            try
            {
                if (!GlobalsScraper.isStopJobScraper)
                {
                    GlobalsScraper.lstThreadsJobScraper.Add(Thread.CurrentThread);
                    GlobalsScraper.lstThreadsJobScraper = GlobalsScraper.lstThreadsJobScraper.Distinct().ToList();
                    Thread.CurrentThread.IsBackground = true;
                }
            }
            catch
            {
            }

            try
            {
                GlobusHttpHelper httpHelper = objLinkedinUser.globusHttpHelper;
                int current_Page_Num = 1;
                int total_no_of_pages = 0;
                int i = 1;
                bool IsCheckCount = true; ;

                List<string> lstJobUrl = new List<string>();

                if (JobUrl.Contains("<:>"))
                {
                    string[] arrJobUrl = Regex.Split(JobUrl, "<:>");
                    lstJobUrl.AddRange(arrJobUrl);
                }
                else
                {
                    lstJobUrl.Add(JobUrl);
                }

                foreach (string item in lstJobUrl)
                {
                    string tempItem = string.Empty;
                    string pagination_url = string.Empty;
                    string tempCount = string.Empty;
                    string starting_page = string.Empty;
                    string final_url = string.Empty;
                    string PageSource_first_page = string.Empty;

                    try
                    {
                        PageSource_first_page = httpHelper.getHtmlfromUrl(new Uri(item));
                        if (string.IsNullOrEmpty(PageSource_first_page))
                        {
                            PageSource_first_page = httpHelper.getHtmlfromUrl(new Uri(item));
                        }
                        try
                        {
                            tempCount = Utils.getBetween(PageSource_first_page, "resultCount\":", ",");
                            total_no_of_pages = int.Parse(tempCount);
                            GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Total Results :  " + total_no_of_pages + " ]");
                            total_no_of_pages = (total_no_of_pages / 25) + 1;
                        }
                        catch
                        { }
                        if (total_no_of_pages == -1)
                        {
                            total_no_of_pages = 2;
                        }
                        if (total_no_of_pages == 1)
                        {
                            total_no_of_pages = 2;
                        }
                        if (total_no_of_pages > 40)
                        {
                            total_no_of_pages = 40;
                        }
                        if (!string.IsNullOrEmpty(PageSource_first_page))
                        {
                            if (PageSource_first_page.Contains("page_number_i18n"))
                            {
                                try
                                {
                                    string pg_url = Utils.getBetween(PageSource_first_page, "page_number_i18n", "pageNum");
                                    pg_url = Utils.getBetween(pg_url, "pageURL\":\"", "\",");
                                    pagination_url = pg_url;
                                }
                                catch
                                { }
                            }
                        }
                        if (item.Contains("page_num="))
                        {
                            try
                            {
                                starting_page = Utils.getBetween(item, "page_num=", "&");
                                current_Page_Num = Convert.ToInt32(starting_page);
                                string current_page_url = Utils.getBetween("###" + pagination_url, "###", "page_num=");
                                current_page_url = current_page_url + "page_num=";
                                current_Page_Num += 1;
                                final_url = "https://www.linkedin.com" + current_page_url + current_Page_Num;
                            }
                            catch
                            { }
                        }
                        else
                        {
                            try
                            {
                                string pg_num = Utils.getBetween(pagination_url + "####", "page_num=", "####");
                                current_Page_Num = Convert.ToInt32(pg_num);
                                string current_page_url = Utils.getBetween("###" + pagination_url, "###", "page_num=");
                                current_page_url = current_page_url + "page_num=";

                                final_url = "https://www.linkedin.com" + current_page_url + current_Page_Num;
                            }
                            catch
                            { }
                        }
                    }
                    catch
                    { }

                    if (PageSource_first_page.Contains("lix_show_incommon_counts_jobs"))
                    {
                        try
                        {
                            List<string> PageSerchUrl = GettingAllUrl(PageSource_first_page);
                            PageSerchUrl.Distinct();

                            if (PageSerchUrl.Count == 0)
                            {
                                GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ On the basis of your Account you can able to see " + RecordURL.Count + " Results ]");
                                break;
                            }
                            foreach (string tempitem in PageSerchUrl)
                            {
                                if (true)
                                {
                                    if (tempitem.Contains("jobs2/view/"))
                                    {
                                        try
                                        {
                                            string urlSerch = tempitem;
                                            if (urlSerch.Contains("jobs2/view/"))
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

                            if (i == total_no_of_pages)
                            {
                                IsCheckCount = true;
                                break;
                            }

                            // countPageNum++;
                            i++;
                            Thread.Sleep(4000);
                        }
                        catch { }
                    }
                    _HttpHelper = httpHelper;
                    string Account = objLinkedinUser.username;
                    new Thread(() =>
                    {
                        if (IsCheckCount)
                        {
                            finalUrlCollectionForRecruter(Account, MaxLimitCount);
                        }
                    }).Start();

                StartAgain:
                    if (!string.IsNullOrEmpty(final_url))
                    {
                        while (true)
                        {
                            final_url = Utils.getBetween("###" + final_url, "###", "page_num=");
                            final_url = final_url + "page_num=" + current_Page_Num;

                            string PageSource = httpHelper.getHtmlfromUrl(new Uri(final_url));

                            if (IsCheckCount)
                            {

                            }

                            if (total_no_of_pages >= 1)
                            {
                                _HttpHelper = httpHelper;

                            }
                            {
                                if (true)
                                {
                                    if (PageSource.Contains("lix_show_incommon_counts_jobs"))
                                    {
                                        try
                                        {
                                            List<string> PageSerchUrl = GettingAllUrl(PageSource);
                                            PageSerchUrl.Distinct();

                                            if (PageSerchUrl.Count == 0)
                                            {
                                                GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ On the basis of your Account you can able to see " + RecordURL.Count + " Results ]");
                                                break;
                                            }

                                            foreach (string tempitem in PageSerchUrl)
                                            {
                                                if (true)
                                                {
                                                    if (tempitem.Contains("jobs2/view/"))
                                                    {
                                                        try
                                                        {
                                                            string urlSerch = tempitem;
                                                            if (urlSerch.Contains("jobs2/view/"))
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

                                            if (current_Page_Num == total_no_of_pages)
                                            {
                                                IsCheckCount = true;
                                                break;
                                            }
                                            current_Page_Num++;
                                            i++;
                                            Thread.Sleep(4000);
                                            if (!GlobalsScraper.isStopJobScraper)
                                            {
                                                // goto StartAgain;
                                            }
                                        }
                                        catch { }
                                    }
                                    else
                                    {
                                        if ((current_Page_Num == total_no_of_pages) && (!PageSource.Contains("&jobId=")))
                                        {
                                            break;
                                        }
                                        Thread.Sleep(5000);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch { }

            GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED ]");
            GlobusLogHelper.log.Info("-----------------------------------------------------------------------------------------------------------------------------------");
        }

        public List<string> GettingAllUrl(string PageSource)
        {
            List<string> lstGettingUrl = new List<string>();
            try
            {
                if (PageSource.Contains("lix_show_incommon_counts_jobs"))
                {
                    string[] trkArr = Regex.Split(PageSource, "lix_show_incommon_counts_jobs");

                    foreach (var item in trkArr)
                    {
                        if (!item.Contains("<!DOCTYPE html>"))
                        {
                            string data = Utils.getBetween(item, "&pid=", ",").Replace("&trk", "?trk").Replace("\"", "");
                            string url = "https://www.linkedin.com/jobs2/view/" + data;
                            if (!lstGettingUrl.Contains(url))
                            {
                                lstGettingUrl.Add(url);
                            }
                        }
                    }
                }
            }

            catch { }
            return lstGettingUrl;
        }

        private void finalUrlCollectionForRecruter(string User_Account, int MaxLimitCount)
        {
            string Account = string.Empty;
            if (JobScraperLimitCrossed)
            {
                return;
            }

            bool check = false;

            try
            {
                try
                {
                    GlobalsScraper.lstThreadsJobScraper.Add(Thread.CurrentThread);
                    GlobalsScraper.lstThreadsJobScraper = GlobalsScraper.lstThreadsJobScraper.Distinct().ToList();
                    Thread.CurrentThread.IsBackground = true;
                }
                catch { }

                List<string> numburlpp = new List<string>();
                GlobusHttpHelper HttpHelper = _HttpHelper;
                if (true)
                {
                    RecordURL = RecordURL.Distinct().ToList();

                    Thread.Sleep(1 * 10 * 1000);
                    while (true)
                    {
                        if (queRecordUrl.Count > 0)
                        {
                            string item = queRecordUrl.Dequeue();

                            try
                            {

                                if (item.Contains("/view/"))
                                {

                                    string urltemp = item;
                                    numburlpp.Add(urltemp);


                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ " + urltemp + " ]");

                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Fetching Data From URL ]");

                                    if (LimitCount >= MaxLimitCount)
                                    {
                                        JobScraperLimitCrossed = true;
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Maximum limit reached]");
                                        break;
                                    }

                                    check = CrawlingLinkedInPageRecruiter(urltemp, ref HttpHelper, User_Account);

                                    if (check)
                                    {
                                        LimitCount++;
                                    }
                                    if (!JobScraperLimitCrossed)
                                    {
                                        int delay = RandomNumberGenerator.GenerateRandom(SearchCriteria.JobscraperMinDelay, SearchCriteria.JobscraperMaxDelay);
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Delay for : " + delay + " Seconds ]");
                                        Thread.Sleep(delay * 1000);
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
                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Process Completed ]");
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public bool CrawlingLinkedInPageRecruiter(string Url, ref GlobusHttpHelper HttpHelper1, string Account)
        {
            string personalPhoneNumber = string.Empty;
            string CompanyName_withAddress = string.Empty;
            string currentCompanyWebsite = string.Empty;
            string CompanyPhNumber = string.Empty;

            GlobusHttpHelper HttpHelper = HttpHelper1;
            bool isscraped = false;
            string Jobtitle = string.Empty;
            string Location = string.Empty;
            string PersonUrlLink = string.Empty;
            string FirstName = string.Empty;
            string MiidleName = string.Empty;
            string LastName = string.Empty;
            string specialites = string.Empty;
            string Website = string.Empty;
            string Industry = string.Empty;
            string ProfileUrl = string.Empty;
            string strFamilyName = string.Empty;
            string careerDetails = string.Empty;
            string no_of_applicants = string.Empty;
            string email_id = string.Empty;
            string email_single = string.Empty;
            string scraped_time = string.Empty;
            string pagesourceProfildetails = string.Empty;

            try
            {
                //Url = "https://www.linkedin.com/jobs2/view/38612041?trk=vsrp_jobs_res_name&trkInfo=VSRPsearchId%3A82134271427382117065%2CVSRPtargetId%3A38612041%2CVSRPcmpt%3Aprimary";
                string pagesource = HttpHelper.getHtmlfromUrl(new Uri(Url));

                if (!pagesource.Contains("Contact the job poster"))
                {

                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ No Data Found For Url " + Url + " ] ");
                    return false; ;
                }
                try
                {
                    Jobtitle = Utils.getBetween(pagesource, "itemprop=\"title\">", "</h1>").Replace(",", ";").Replace("&amp;", "&");
                    Location = Utils.getBetween(pagesource, "itemprop=\"description\">", "</span>").Replace(",", ";").Replace("&amp;", "&");
                }
                catch
                { }

                if (string.IsNullOrEmpty(Location))
                {
                    try
                    {
                        Location = Utils.getBetween(pagesource, "location\":", ",").Replace(",", ";").Replace("&amp;", "&");
                    }
                    catch
                    { }
                }

                if (pagesource.Contains("<div class=\"applicant"))
                {
                    no_of_applicants = Utils.getBetween(pagesource, "<div class=\"applicant", "</div>");
                    no_of_applicants = Utils.getBetween(no_of_applicants, "-number\">", "</");
                }

                if (string.IsNullOrEmpty(no_of_applicants) && pagesource.Contains("<h2 class=\"applicant-analytics-header\""))
                {
                    string appicants = Utils.getBetween(pagesource, "<h2 class=\"applicant-analytics-header\"", "</h2>");
                    string[] applicantArr = Regex.Split(appicants, "<span class=");
                    if (applicantArr.Length == 2)
                    {
                        no_of_applicants = Utils.getBetween(applicantArr[1], "callout-text\">", "<");
                    }
                    else if (applicantArr.Length == 3)
                    {
                        no_of_applicants = Utils.getBetween(applicantArr[2], "callout-text\">", "<");
                    }
                }

                careerDetails = Utils.getBetween(pagesource, "companyPageNameLink\":", ",").Replace("\"", "").Replace(",", ";").Replace("&amp;", "&").Replace("careers?", "home?");
                string subPagedetails = HttpHelper.getHtmlfromUrl(new Uri(careerDetails));

                if (!string.IsNullOrEmpty(careerDetails) && string.IsNullOrEmpty(subPagedetails))
                {
                    try
                    {
                        careerDetails = careerDetails.Replace("http", "https");
                        subPagedetails = HttpHelper.getHtmlfromUrl(new Uri(careerDetails));
                    }
                    catch
                    { }
                }

                List<string> websiteAddress = HttpHelper.GetTextDataByTagAndAttributeName(subPagedetails, "li", "website");
                if (websiteAddress.Count > 0)
                {
                    Website = websiteAddress[0].Replace("Website", "").Replace(",", ";").Replace("&amp;", "&");
                }

                List<string> specialtiesAddress = HttpHelper.GetTextDataByTagAndAttributeName(subPagedetails, "div", "specialties");
                if (specialtiesAddress.Count > 0)
                {
                    specialites = specialtiesAddress[0].Replace("specialties", "").Replace(",", ";").Replace("&amp;", "&");
                }
                List<string> lstIndustry = HttpHelper.GetTextDataByTagAndAttributeName(subPagedetails, "li", "industry");
                if (lstIndustry.Count > 0)
                {
                    Industry = lstIndustry[0].Replace("Industry", "").Replace(",", ";").Replace("&amp;", "&");
                }

                string tempPagesource = Utils.getBetween(pagesource, "<div class=\"poster\"", "</div>");
                ProfileUrl = Utils.getBetween(tempPagesource, "<a href=", ">").Replace("\"", "").Trim();
                if (!string.IsNullOrEmpty(ProfileUrl))
                {
                    try
                    {
                        if (ProfileUrl.Contains("amp;"))
                        {
                            ProfileUrl = ProfileUrl.Replace("amp;", "");
                        }
                        if (ProfileUrl.Contains("picture"))
                        {
                            ProfileUrl = ProfileUrl.Replace("picture", "name");
                        }

                        pagesourceProfildetails = HttpHelper.getHtmlfromUrl(new Uri(ProfileUrl));

                        if (string.IsNullOrEmpty(pagesourceProfildetails))
                        {
                            string tempProfil_url = Utils.getBetween("####" + ProfileUrl, "####", "&");
                            if (!tempProfil_url.Contains("https"))
                            {
                                tempProfil_url = tempProfil_url.Replace("http", "https");
                            }
                            pagesourceProfildetails = HttpHelper.getHtmlfromUrl(new Uri(tempProfil_url));
                            if (string.IsNullOrEmpty(pagesourceProfildetails))
                            {
                                pagesourceProfildetails = HttpHelper.getHtmlfromUrl(new Uri(tempProfil_url));
                            }
                        }
                    }
                    catch
                    { }

                    #region Name
                    try
                    {
                        try
                        {
                            try
                            {
                                int StartIndex = pagesourceProfildetails.IndexOf("<title>");
                                string Start = pagesourceProfildetails.Substring(StartIndex).Replace("<title>", string.Empty);
                                int EndIndex = Start.IndexOf("| LinkedIn</title>");
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
                                strFamilyName = pagesourceProfildetails.Substring(pagesourceProfildetails.IndexOf("fmt__full_name\":"), (pagesourceProfildetails.IndexOf(",", pagesourceProfildetails.IndexOf("fmt__full_name\":")) - pagesourceProfildetails.IndexOf("fmt__full_name\":"))).Replace("fmt__full_name\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Trim();

                            }
                            catch { }
                        }

                        if (string.IsNullOrEmpty(strFamilyName))
                        {
                            try
                            {
                                strFamilyName = pagesourceProfildetails.Substring(pagesourceProfildetails.IndexOf("<span class=\"full-name\">"), (pagesourceProfildetails.IndexOf("</span><span></span></span></h1></div></div><div id=\"headline-container\" data-li-template=\"headline\">", pagesourceProfildetails.IndexOf("</span><span></span></span></h1></div></div><div id=\"headline-container\" data-li-template=\"headline\">")) - pagesourceProfildetails.IndexOf("<span class=\"full-name\">"))).Replace("<span class=\"full-name\">", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Trim();
                            }
                            catch
                            { }
                        }

                        if (string.IsNullOrEmpty(strFamilyName))
                        {
                            try
                            {
                                int StartIndex = pagesourceProfildetails.IndexOf("<span class=\"full-name\">");
                                string Start = pagesourceProfildetails.Substring(StartIndex).Replace("<span class=\"full-name\">", string.Empty);
                                int EndIndex = Start.IndexOf("</span>");
                                string End = Start.Substring(0, EndIndex).Replace("</span>", string.Empty);
                                strFamilyName = End.Trim();
                            }
                            catch
                            { }
                        }

                        if (string.IsNullOrEmpty(strFamilyName) && pagesourceProfildetails.Contains("<span class=\"full-name\""))
                        {
                            try
                            {
                                int StartIndex = pagesourceProfildetails.IndexOf("<span class=\"full-name\"");
                                string Start = pagesourceProfildetails.Substring(StartIndex).Replace("<span class=\"full-name\"", string.Empty);
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
                                int StartIndex = pagesourceProfildetails.IndexOf("<title>");
                                string Start = pagesourceProfildetails.Substring(StartIndex).Replace("</title>", string.Empty);
                                int EndIndex = Start.IndexOf("| LinkedIn Recruiter</title>");
                                string End = Start.Substring(0, EndIndex).Replace(":", string.Empty).Replace("'", string.Empty).Replace(",", string.Empty).Trim();
                                strFamilyName = End.Trim();
                            }
                            catch
                            { }
                        }

                        if (string.IsNullOrEmpty(strFamilyName) && pagesourceProfildetails.Contains("<span class=\"full-name"))
                        {
                            try
                            {
                                strFamilyName = Utils.getBetween(pagesourceProfildetails, "<span class=\"full-name", "</span>");
                                strFamilyName = Utils.getBetween(strFamilyName + "###", ">", "###");
                            }
                            catch
                            { }

                        }
                    }
                    catch { }
                    #endregion

                    #region EmailId

                    if (pagesourceProfildetails.Contains("@"))
                    {
                        List<string> s1 = GetEmails(pagesourceProfildetails);
                        foreach (string item in s1)
                        {
                            email_id = email_id + item + ";";
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
                        FirstName = NameArr[0];
                    }
                    catch { }
                    #endregion

                    #region LastName

                    try
                    {
                        LastName = NameArr[1];
                    }
                    catch { }

                    try
                    {
                        if (NameArr.Count() == 3)
                        {
                            try
                            {
                                MiidleName = NameArr[1];
                                LastName = NameArr[2];
                            }
                            catch { }
                        }


                    }
                    catch { }
                    #endregion

                    #region ScrapedTime

                    DateTime now = DateTime.Now;
                    scraped_time = now.ToString();


                    #endregion

                    #region Ashley's Task

                    #region current Company Address
                    try
                    {
                        string[] companylist = Regex.Split(pagesourceProfildetails, "companyName\"");
                        if (companylist.Count() == 1)
                        {
                            companylist = Regex.Split(pagesourceProfildetails, "company-name");
                        }
                        CompanyName_withAddress = Utils.getBetween(companylist[2], "\"auto\">", "</a>");
                        string location1 = Utils.getBetween(companylist[2], "class=\"locality\">", "</span>");
                        if (string.IsNullOrEmpty(location1))
                        {
                            try
                            {
                                location1 = Utils.getBetween(companylist[1], "\"auto\">", "</a>");
                            }
                            catch { }
                        }

                        //if (string.IsNullOrEmpty(location1) && !string.IsNullOrEmpty(companycurrent))
                        //{
                        //    location1 = companycurrent;
                        //}

                        CompanyName_withAddress = CompanyName_withAddress + " " + location1;
                    }
                    catch { }

                    #endregion

                    #region current  Company website
                    try
                    {
                        string curentCompanyUrl = Utils.getBetween(pagesourceProfildetails, "Current<", "</a></strong>");
                        curentCompanyUrl = Utils.getBetween(curentCompanyUrl, "<a href=\"", "\" dir=\"auto\">");

                        if (string.IsNullOrEmpty(curentCompanyUrl))
                        {
                            curentCompanyUrl = Utils.getBetween(pagesourceProfildetails + "###", "<h3>Experience</h3>", "###");
                            curentCompanyUrl = Utils.getBetween(curentCompanyUrl, "<a href=\"", "\">");
                        }

                        if (!string.IsNullOrEmpty(curentCompanyUrl))
                        {
                            curentCompanyUrl = "https://www.linkedin.com" + curentCompanyUrl;
                            // currentCompanyUrl = curentCompanyUrl;
                            string currentComapnySource = HttpHelper.getHtmlfromUrl(new Uri(curentCompanyUrl));

                            currentCompanyWebsite = Utils.getBetween(currentComapnySource, "<h4>Website</h4>", "</a>");
                            currentCompanyWebsite = Utils.getBetween(currentCompanyWebsite + "###", "rel=\"nofollow\">", "###");
                        }

                        if (string.IsNullOrEmpty(currentCompanyWebsite))
                        {
                            currentCompanyWebsite = Website;
                        }
                    }
                    catch
                    { }

                    #endregion

                    try
                    {
                        string PhNo = string.Empty;
                        PhNo = Utils.getBetween(pagesource, "profile-overview", "Contact Info</span>");
                        PhNo = Utils.getBetween(PhNo, "summary=\"Contact Info\"", "</li></ul></div>");
                        PhNo = Utils.getBetween(PhNo + "###", "id=\"phone-view\"><ul><li>", "###");
                        personalPhoneNumber = PhNo;
                    }
                    catch
                    { }

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
                    }

                    #endregion

                    personalPhoneNumber = personalPhoneNumber.Replace(",", "").Replace("(", "").Replace(")", "").Replace("&nbsp;", "").Replace("-", "");
                    CompanyName_withAddress = CompanyName_withAddress.Replace(",", "").Replace("(", "").Replace(")", "").Replace("&nbsp;", "").Replace("-", "");
                    CompanyPhNumber = CompanyPhNumber.Replace(",", "").Replace("(", "").Replace(")", "").Replace("&nbsp;", "").Replace("-", "");

                    if (string.IsNullOrEmpty(FirstName)) FirstName = "N/A";
                    if (string.IsNullOrEmpty(MiidleName)) MiidleName = "N/A";
                    if (string.IsNullOrEmpty(LastName)) LastName = "N/A";
                    if (string.IsNullOrEmpty(ProfileUrl)) ProfileUrl = "N/A";
                    if (string.IsNullOrEmpty(Jobtitle)) Jobtitle = "N/A";
                    if (string.IsNullOrEmpty(Location)) Location = "N/A";
                    if (string.IsNullOrEmpty(Website)) Website = "N/A";
                    if (string.IsNullOrEmpty(specialites)) specialites = "N/A";
                    if (string.IsNullOrEmpty(Industry)) Industry = "N/A";
                    if (string.IsNullOrEmpty(Website)) Website = "N/A";
                    if (string.IsNullOrEmpty(no_of_applicants)) no_of_applicants = "N/A";

                    if (string.IsNullOrEmpty(personalPhoneNumber)) personalPhoneNumber = "N/A";
                    if (string.IsNullOrEmpty(CompanyName_withAddress)) CompanyName_withAddress = "N/A";
                    if (string.IsNullOrEmpty(CompanyPhNumber)) CompanyPhNumber = "N/A";

                    GlobusLogHelper.log.Info("----------------------------------------------------------------------");
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Scraping Data from : " + Url + " ] ");

                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ First Name : " + FirstName + " ] ");
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Last Name " + LastName + " ] ");
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Profile Url " + ProfileUrl + " ] ");
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Job Title " + Jobtitle + " ] ");
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Location " + Location + " ] ");
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Website " + Website + " ] ");
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Specialities " + specialites + " ] ");
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Industry " + Industry + " ] ");
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Website " + Website + " ] ");

                    if (!JobScraperLimitCrossed)
                    {
                        // string LDS_FinalData = Account + "," + Url.Replace(",", ";") + "," + Jobtitle.Replace(",", ";") + "," + Location.Replace(",", ";") + "," + FirstName.Replace(",", ";") + "," + MiidleName.Replace(",", ";") + "," + LastName.Replace(",", ";") + "," + ProfileUrl.Replace(",", ";") + "," + Website.Replace(",", ";") + "," + specialites.Replace(",", ";") + "," + Industry.Replace(",", ";") + "," + no_of_applicants.Replace(",", ";") + "," + email_id.Replace(",", ";") + "," + scraped_time;
                        string LDS_FinalData = Account + "," + Url.Replace(",", ";") + "," + Jobtitle.Replace(",", ";") + "," + Location.Replace(",", ";") + "," + FirstName.Replace(",", ";") + "," + MiidleName.Replace(",", ";") + "," + LastName.Replace(",", ";") + "," + ProfileUrl.Replace(",", ";") + "," + Website.Replace(",", ";") + "," + specialites.Replace(",", ";") + "," + Industry.Replace(",", ";") + "," + no_of_applicants.Replace(",", ";") + "," + email_id.Replace(",", ";") + "," + scraped_time + "," + CompanyName_withAddress + "," + personalPhoneNumber + "," + CompanyPhNumber;
                        AddingLinkedInDataToCSVFileCompanyEmployeeScraper(LDS_FinalData);
                    }
                    else
                    {
                        return false;
                    }
                    isscraped = true;
                }
            }
            catch (Exception ex)
            {

            }
            return isscraped;
        }

        public static void AddingLinkedInDataToCSVFileCompanyEmployeeScraper(string Data)
        {
            try
            {
                //string LinkedInAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LinkedInJobScraper.csv");
                string LinkedInDeskTop = Globals.DesktopFolder + "\\LinkedInJobScraper.csv";


                if (!File.Exists(LinkedInDeskTop))
                {
                    //string Header = "Account" + "," + "JobUrl" + "," + "JobTitle" + "," + "Location" + "," + "FirstName" + "," + "MiddleName" + "," + "LastName" + "," + "ProfileUrl" + "," + "Website" + "," + "specialites" + "," + "Industry" + "," + "Applicants" + "," + "EmailId" + "," + "ScrapedTime";
                    string Header = "Account" + "," + "JobUrl" + "," + "JobTitle" + "," + "Location" + "," + "FirstName" + "," + "MiddleName" + "," + "LastName" + "," + "ProfileUrl" + "," + "Website" + "," + "specialites" + "," + "Industry" + "," + "Applicants" + "," + "EmailId" + "," + "ScrapedTime" + "," + "CompanyName" + "," + "Profile_PhoneNumber" + "," + "CompanyPhoneNo";
                    GlobusFileHelper.AppendStringToTextfileNewLine(Header, LinkedInDeskTop);
                }


                if (!string.IsNullOrEmpty(Data))
                {
                    GlobusFileHelper.AppendStringToTextfileNewLine(Data, LinkedInDeskTop);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static List<string> GetEmails(string Pagesource)
        {
            List<string> Email = new List<string>();
            const string MatchEmailPattern =
           @"(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
           + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
             + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
           + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})";
            Regex rx = new Regex(MatchEmailPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            // Find matches.
            MatchCollection matches = rx.Matches(Pagesource);
            // Report the number of matches found.
            int noOfMatches = matches.Count;
            // Report on each match.
            foreach (Match match in matches)
            {
                string ss = match.Value.ToString();

                Email.Add(ss);
            }
            Email = Email.Distinct().ToList();
            return Email;
        }
    }
}
