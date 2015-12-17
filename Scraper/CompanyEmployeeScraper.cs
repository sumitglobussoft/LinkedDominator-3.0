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
    public class CompanyEmployeeScraper
    {
        readonly object lockrThreadControllerCompanyEmployeeScraper = new object();
        public void ThreadStartCompanyEmployeeScraper()
        {
            try
            {
                int numberOfAccountPatch = 25;

                if (GlobalsScraper.NoOfThreadsCompanyEmployeeScraper > 0)
                {
                    numberOfAccountPatch = GlobalsScraper.NoOfThreadsCompanyEmployeeScraper;
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
                                lock (lockrThreadControllerCompanyEmployeeScraper)
                                {
                                    try
                                    {
                                        if (GlobalsScraper.countThreadControllerCompanyEmployeeScraper >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerCompanyEmployeeScraper);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        LinkedinUser item = null;
                                        LDGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {
                                            Thread profilerThread = new Thread(StartMultiThreadsCompanyEmployeeScraper);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;

                                            profilerThread.Start(new object[] { item });

                                            GlobalsScraper.countThreadControllerCompanyEmployeeScraper++;
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
        public void StartMultiThreadsCompanyEmployeeScraper(object parameters)
        {
            try
            {
                if (!GlobalsScraper.isStopCompanyEmployeeScraper)
                {
                    try
                    {
                        GlobalsScraper.lstThreadsCompanyEmployeeScraper.Add(Thread.CurrentThread);
                        GlobalsScraper.lstThreadsCompanyEmployeeScraper.Distinct();
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
                            StartCompanyEmployeeScraper(ref objLinkedinUser);
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
                        lock (lockrThreadControllerCompanyEmployeeScraper)
                        {
                            GlobalsScraper.countThreadControllerCompanyEmployeeScraper = GlobalsScraper.countThreadControllerCompanyEmployeeScraper--;
                            Monitor.Pulse(lockrThreadControllerCompanyEmployeeScraper);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }
        public void StartCompanyEmployeeScraper(ref LinkedinUser objLinkedinUser)
        {
            try
            {
                if(objLinkedinUser.isloggedin)
                {
                    GetEmployeeDataFromCompanyURL(ref objLinkedinUser.globusHttpHelper, GlobalsScraper.lstUrlCompanyEmpScraper);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void GetEmployeeDataFromCompanyURL(ref GlobusHttpHelper HttpHelper, List<string> lstCompanyUrls)
        {
            try
            {
                if (!GlobalsScraper.isSearchByCompanyNamesToGetEmailId)
                {
                    foreach (string item in lstCompanyUrls)
                    {
                        try
                        {
                            GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Starting Parsing With UserName : " + SearchCriteria.LoginID + " ]");
                            //Log("Url >>> " + item);
                            string compCode = Regex.Split(item, "company/")[1];
                            compCode = compCode.Split('?')[0];
                            string EmployeeUrl = "https://www.linkedin.com/vsearch/p?keywords=" + SearchCriteria.Keyword + "&openAdvancedForm=true&locationType=" + SearchCriteria.Country + "&countryCode=" + SearchCriteria.Country + "&f_CC=" + compCode + "&rsid=&orig=ADVS";

                            string LinkPagesourceCompEmp = HttpHelper.getHtmlfromUrl1(new Uri(EmployeeUrl));

                            try
                            {
                                int pagenumber = 0;
                                string strPageNumber = string.Empty;
                                string[] Arr12 = Regex.Split(LinkPagesourceCompEmp, "<p class=\"summary\">");

                                if (Arr12.Count() == 1)
                                {
                                    Arr12 = Regex.Split(LinkPagesourceCompEmp, "formattedResultCount");
                                }

                                foreach (string item1 in Arr12)
                                {
                                    try
                                    {
                                        if (!item1.Contains("<!DOCTYPE"))
                                        {
                                            if (item1.Contains("<strong>"))
                                            {
                                                try
                                                {
                                                    //":"15,439","i18n_survey_feedback_thanks":
                                                    string pageNO = Regex.Split(item1, "i18n_survey")[0].Replace(":", string.Empty).Replace(",", string.Empty).Replace("\"", string.Empty);

                                                    string[] arrPageNO = Regex.Split(pageNO, "[^0-9]");

                                                    foreach (string item2 in arrPageNO)
                                                    {
                                                        try
                                                        {
                                                            if (!string.IsNullOrEmpty(item2))
                                                            {
                                                                strPageNumber = item2;
                                                            }
                                                        }
                                                        catch
                                                        {
                                                        }
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
                                }



                                try
                                {
                                    strPageNumber = strPageNumber.Replace(".", string.Empty);
                                    if (strPageNumber != string.Empty || strPageNumber == "0")
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Total Results found: " + strPageNumber + " ]");
                                    }
                                    pagenumber = int.Parse(strPageNumber);
                                }
                                catch (Exception)
                                {

                                }

                                pagenumber = (pagenumber / 10) + 1;

                                LinkedInScraper obj_LinkedInScraper = new LinkedInScraper();

                                EmployeeUrl = EmployeeUrl + "##CompanyEmployeeScraper";

                                obj_LinkedInScraper.StartCompanyEmployeeScraperWithPagination(ref HttpHelper, EmployeeUrl, pagenumber);

                            }
                            catch
                            { }
                        }
                        catch
                        { }
                    }
                }

                #region for kishore's requirement
                if (GlobalsScraper.isSearchByCompanyNamesToGetEmailId)
                {
                    foreach (string item in lstCompanyUrls)
                    {
                        try
                        {
                            string compCode = "";
                            GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Starting Parsing With UserName : " + SearchCriteria.LoginID + " ]");
                            // string compCode = Regex.Split(item, "company/")[1];
                            // compCode = compCode.Split('?')[0];

                            string compName = item.Replace(" ", "+");
                            string CompUrl = "https://www.linkedin.com/vsearch/c?type=companies&keywords=" + compName;
                            string source = HttpHelper.getHtmlfromUrl1(new Uri(CompUrl));
                            string[] arr = Regex.Split(source, "fmt_industry");
                            string url = "";
                            if (arr.Length > 1)
                            {
                                url = Utils.getBetween(arr[1], "link_biz_overview_", "\",\"");
                                url = Utils.getBetween(url + "###", "\":\"/", "###");
                                compCode = Utils.getBetween(url, "company/", "?");


                            }
                            url = "https://www.linkedin.com/" + url;
                            source = HttpHelper.getHtmlfromUrl1(new Uri(url));

                            string EmployeeUrl = "https://www.linkedin.com/vsearch/p?keywords=" + SearchCriteria.Keyword + "&openAdvancedForm=true&locationType=" + SearchCriteria.Country + "&countryCode=" + SearchCriteria.Country + "&f_CC=" + compCode + "&rsid=&orig=ADVS";
                            string LinkPagesourceCompEmp = HttpHelper.getHtmlfromUrl1(new Uri(EmployeeUrl));

                            try
                            {
                                int pagenumber = 0;
                                string strPageNumber = string.Empty;
                                string[] Arr12 = Regex.Split(LinkPagesourceCompEmp, "<p class=\"summary\">");

                                if (Arr12.Count() == 1)
                                {
                                    Arr12 = Regex.Split(LinkPagesourceCompEmp, "formattedResultCount");
                                }

                                foreach (string item1 in Arr12)
                                {
                                    try
                                    {
                                        if (!item1.Contains("<!DOCTYPE"))
                                        {
                                            if (item1.Contains("<strong>"))
                                            {
                                                try
                                                {
                                                    //":"15,439","i18n_survey_feedback_thanks":
                                                    string pageNO = Regex.Split(item1, "i18n_survey")[0].Replace(":", string.Empty).Replace(",", string.Empty).Replace("\"", string.Empty);

                                                    string[] arrPageNO = Regex.Split(pageNO, "[^0-9]");

                                                    foreach (string item2 in arrPageNO)
                                                    {
                                                        try
                                                        {
                                                            if (!string.IsNullOrEmpty(item2))
                                                            {
                                                                strPageNumber = item2;
                                                            }
                                                        }
                                                        catch
                                                        {
                                                        }
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
                                }



                                try
                                {
                                    strPageNumber = strPageNumber.Replace(".", string.Empty);
                                    if (strPageNumber != string.Empty || strPageNumber == "0")
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Total Results found: " + strPageNumber + " ]");
                                    }
                                    pagenumber = int.Parse(strPageNumber);
                                }
                                catch (Exception)
                                {

                                }

                                pagenumber = (pagenumber / 10) + 1;

                                LinkedInScraper obj_LinkedInScraper = new LinkedInScraper();

                                EmployeeUrl = EmployeeUrl + "##CompanyEmployeeScraper";

                                obj_LinkedInScraper.StartCompanyEmployeeScraperWithPagination(ref HttpHelper, EmployeeUrl, pagenumber);

                            }
                            catch
                            { }
                        }
                        catch
                        { }
                    }
                }

                #endregion
            }
            catch { }
        }
    }
}
