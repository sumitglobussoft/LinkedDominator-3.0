using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseLib;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data;
using Globussoft;

namespace ManageConnections
{
    public class ConnectUsing_Search
    {
        //** Variable and Object Declaration *****************************************************************************


        #region Variable and Object Declaration

        string _UserName = string.Empty;
        string _Password = string.Empty;
        string _ProxyAddress = string.Empty;
        string _ProxyPort = string.Empty;
        string _ProxyUserName = string.Empty;
        string _ProxyPassword = string.Empty;
        int SendInvitationCount = 0;
        string _ConnectSearchKeyword = string.Empty;
        public static readonly object locker_InvitataionKeyword = new object();
        int PageNumber = 0;
        bool valid = true;
        Queue<string> UsingKeyWordQueue = new Queue<string>();
        public static int SearchMinDelay = 20;
        public static int SeacrhMaxDelay = 25;


        public AccountInfo.LinkedinLoginAndLogout linkedinLoginAndLogout = new AccountInfo.LinkedinLoginAndLogout();

        GlobusHttpHelper HttpHelper = new GlobusHttpHelper();

        public static Events ConnectSearchLogEvents = new Events();
        public static readonly object LockerConnection = new object();

        public static Queue<string> lstQueuKeywords = new Queue<string>();
        public static Queue<string> lstQueuEmail = new Queue<string>();
        public static int countofKeywords = 0;
        public static bool UseuniqueConn = false;
        public static bool UseUniqueEmailConn = false;
        public static bool OnlyVisitProfile = false;
        #endregion

        #region ConnectUsingSearch
        public ConnectUsing_Search(string username, string password, string proxyaddress, string proxyport, string proxyusername, string proxypassword, Queue<string> QUEsearchName)
        {
            this._UserName = username;
            this._Password = password;
            this._ProxyAddress = proxyaddress;
            this._ProxyPort = proxyport;
            this._ProxyUserName = proxyusername;
            this._ProxyPassword = proxypassword;
            this.UsingKeyWordQueue = QUEsearchName;
        }
        #endregion

        #region ConnectUsingSearch
        public ConnectUsing_Search(string username, string password, string proxyaddress, string proxyport, string proxyusername, string proxypassword)
        {
            this._UserName = username;
            this._Password = password;
            this._ProxyAddress = proxyaddress;
            this._ProxyPort = proxyport;
            this._ProxyUserName = proxyusername;
            this._ProxyPassword = proxypassword;
            //this.UsingKeyWordQueue = QUEsearchName;
        }
        #endregion

        
        public int  checkDaily(string _UserName)
        {
            int count = 0;
            try
            {
                string selectQuery = "SELECT * FROM tb_checkDailyLimit";
                DataSet ds = DataBaseHandler.SelectQuery(selectQuery, "tb_checkDailyLimit");
              
                count = Convert.ToInt32( ds.Tables[0].Rows[0]["DaillyLimit"].ToString());
              


            }
            catch (Exception)
            {
                return count; 
               
            }
            return count; ; 
        }

        public bool checkDailyLimitWithDB(string _UserName)
        {

          

            bool CheckStatus = false;
            int Count = checkDaily(_UserName);
            try
            {
                var dateAndTime = DateTime.Now;
                var date = dateAndTime.Date;

                string Date = dateAndTime.ToString("dd/MM/yyyy");

                  string selectQuery = "SELECT * FROM tb_AccountsDetails WHERE Usename ='" + _UserName + "'and Date='" + Date +"'" ;
             //   string selectQuery = "SELECT * FROM tb_AccountsDetails WHERE Usename ='" + _UserName+"'";
                DataSet ds = DataBaseHandler.SelectQuery(selectQuery, "tb_AccountsDetails");
                int checkDailyLimitCount= ds.Tables[0].Rows.Count;
                if (checkDailyLimitCount < Count)
                {
                    Globals.no_of_profiles_can_be_visited = Count - checkDailyLimitCount;
                    CheckStatus = true;
                }
                else
                {
                    CheckStatus = false;
                }


            }
            catch (Exception)
            {
                return CheckStatus; 
               
            }
            return CheckStatus; ; 
        }

        #region ConnectionSearch
        bool Isaccountvalid = true;
        public void ConnectionSearch(int SearchMinDelay, int SearchMaxDelay)
        {
            bool checkStatus = true;
           

            try
            {
                if (true)
                {
                    string strPageNumber = string.Empty;
                    string Textmessage = string.Empty;

                    //Login with User name and password ***********************

                    if (Globals.IsCheckDailyLimit)
                    {
                       checkStatus = checkDailyLimitWithDB(_UserName);
                    }
                    if (!checkStatus)
                    {
                        LoggerManageConnection("[ " + DateTime.Now + " ] => [ You have reached the daily limit with : " + _UserName + " ]");
                        return;
                    }

                    if (linkedinLoginAndLogout.LoginHttpHelper(_UserName, _Password, _ProxyAddress, _ProxyPort, _ProxyUserName, _ProxyPassword, ref HttpHelper, ref Textmessage))
                    {
                        LoggerManageConnection("[ " + DateTime.Now + " ] => [ Logged in with " + _UserName + " ]");

                        int proxyport = 888;
                        string SummaryLink = string.Empty;

                        Regex PortCheck = new Regex("^[0-9]*$");

                        if (PortCheck.IsMatch(_ProxyPort) && !string.IsNullOrEmpty(_ProxyPort))
                        {
                            proxyport = int.Parse(_ProxyPort);
                        }

                        while (UsingKeyWordQueue.Count > 0)
                        {
                            lock (locker_InvitataionKeyword)
                            {
                                if (UsingKeyWordQueue.Count > 0)
                                {
                                    _ConnectSearchKeyword = UsingKeyWordQueue.Dequeue();
                                }
                                else
                                {
                                    LoggerManageConnection("[ " + DateTime.Now + " ] => [ All Loaded Keyword are used ]");
                                }
                            }


                            string Url = "http://www.linkedin.com/search/fpsearch?keywords=" + _ConnectSearchKeyword + "&keepFacets=keepFacets&page_num=1&pplSearchOrigin=ADVS&viewCriteria=2&sortCriteria=R&redir=redir";

                            string PgSrcMain = HttpHelper.getHtmlfromUrlProxy(new Uri(Url), _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);
                            string[] Arr = Regex.Split(PgSrcMain, "results_count_with_keywords_i18n");
                            foreach (string item in Arr)
                            {
                                try
                                {
                                    if (!item.Contains("<!DOCTYPE"))
                                    {
                                        if (item.Contains("results for"))
                                        {
                                            string data = RemoveAllHtmlTag.StripHTML(item);
                                            data = data.Replace("\n", "");
                                            if (data.Contains(">"))
                                            {
                                                string[] ArrTemp = Regex.Split(data, "results");
                                                data = ArrTemp[0];
                                                data = data.Replace("\":\"", "").Replace(",", string.Empty);
                                                data = data.Trim();
                                                //string[] ArrTemp1 = data.Split(' ');
                                                //ArrTemp1 = data.Split(':');
                                                //data = ArrTemp1[7].Trim();
                                                //data = ArrTemp1.(item.IndexOf("\">"), item.IndexOf("</b>") - item.IndexOf("\">"));
                                                //strPageNumber = data.Substring(data.IndexOf("\""), data.IndexOf("for") - data.IndexOf("\"")).Replace("\"", "").Replace(" ", string.Empty).Replace(",", string.Empty).Trim();
                                                strPageNumber = data.ToString().Replace("&quot;", string.Empty).Replace(",", string.Empty).Replace("&", string.Empty).Replace(":", string.Empty).Replace("strong", string.Empty).Replace("\\u003e", "").Replace("\\u003c", "").Replace("/", "").Trim();
                                                break;
                                            }
                                        }
                                    }
                                }
                                catch { }
                            }


                            if (string.IsNullOrEmpty(strPageNumber))
                            {

                                string urlGetdata = "http://www.linkedin.com/vsearch/f?keywords=" + _ConnectSearchKeyword + "&orig=GLHD&pageKey=voltron_federated_search_internal_jsp&search=Search";
                                urlGetdata = "http://www.linkedin.com/vsearch/f?keywords=Jobs&orig=GLHD&pageKey=voltron_federated_search_internal_jsp&search=Search";
                                PgSrcMain = HttpHelper.getHtmlfromUrlProxy(new Uri(Url), _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);
                                Arr = Regex.Split(PgSrcMain, "div");
                                foreach (string item in Arr)
                                {
                                    try
                                    {
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

                                                    strPageNumber = data.Replace(" ", string.Empty).Replace(",", string.Empty).Trim();
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    catch { }
                                }
                            }

                            try
                            {
                                PageNumber = int.Parse(strPageNumber);//find the page number for each key word
                            }
                            catch (Exception)
                            {
                                PageNumber = 0;
                                //throw;
                            }
                            if ((PageNumber == 0))
                            {
                                LoggerManageConnection("[ " + DateTime.Now + " ] => [ Sorry No Search Results For  Keyword : " + _ConnectSearchKeyword + " From Account: " + _UserName + " ]");
                                break;
                                //TestConnectPageSearch("1");
                            }

                            PageNumber = (PageNumber / 10) - 1;//each page have 10 data so number of record divide by 10....

                            if (PageNumber == -1)
                            {
                                PageNumber = 2;
                            }

                            for (int i = 1; i <= PageNumber; i++)
                            {
                                //loop for send request
                                if (!Isaccountvalid)
                                {
                                    return;
                                }
                                if (Globals.CheckTheDayLimit)
                                {
                                    if (Globals.no_of_profiles_can_be_visited == Globals.no_of_profiles_visited)
                                    {
                                        return;
                                    }
                                    if (Globals.no_of_profiles_to_be_visited <= Globals.no_of_profiles_visited)
                                    {
                                        return;
                                    }
                                }
                                if (!UseuniqueConn)
                                {
                                    //LoggerManageConnection("Adding Connections From Page : " + i + " From Account " + _UserName);
                                    TestConnectPageSearch(i.ToString(), SearchMinDelay, SearchMaxDelay);
                                    
                                }
                                else
                                {
                                    //LoggerManageConnection("Adding Connections From Page : " + i + " From Account " + _UserName);
                                    TestConnectPageSearchForUniqueUrl(i.ToString(), SearchMinDelay, SearchMaxDelay);
                                  //  break;
                                }

                            }

                            //insert
                          


                            if (valid)
                            {
                                string CSVHeader = "UserName" + "," + "SearchKeyword";
                                string CSV_Content = _UserName + "," + _ConnectSearchKeyword;
                                // CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_AddConnectionFail);
                                //LoggerManageConnection("*************************************************************************************************");
                                //LoggerManageConnection("*************************************************************************************************");

                                //LoggerManageConnection("*************************************************************************************************");
                                //LoggerManageConnection("*************************************************************************************************");
                                if (SendInvitationCount > 0)
                                {

                                    LoggerManageConnection("[ " + DateTime.Now + " ] => [ " + SendInvitationCount + "  Send Request Completed With " + _UserName + " and For Keyword " + _ConnectSearchKeyword + " ]");
                                    //GlobusFileHelper.AppendStringToTextfileNewLine(_ConnectSearchKeyword + "  " +_UserName , Globals.path_AddConnection);
                                    //CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_AddConnectionSuccess);


                                }
                                else
                                {
                                    CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_AddConnectionFail);
                                    LoggerManageConnection("[ " + DateTime.Now + " ] => [ Not Able To send Any Connection Request For Keyword " + _ConnectSearchKeyword + " ]");
                                    // GlobusFileHelper.AppendStringToTextfileNewLine(_ConnectSearchKeyword + "  " + _UserName, Globals.path_AddConnection);

                                }
                                //LoggerManageConnection("*************************************************************************************************");
                                //LoggerManageConnection("*************************************************************************************************");
                                SendInvitationCount = 0;
                                //LoggerManageConnection("Send Request Completed");
                            }
                        }

                    }
                    else
                    {

                        LoggerManageConnection("[ " + DateTime.Now + " ] => [ " + _UserName + " Your LinkedIn account has been temporarily restricted ]");
                        return;
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region ConnectSearchUsingkeyword
        public static void ConnectSearchUsingkeyword(ref ManageConnections.ConnectUsing_Search ConnectUsing_Search, int SearchMinDelay, int SearchMaxDelay)
        {
            try
            {
                // ManageConnections.ConnectUsingSearch ConnectUsing_Search = new ConnectUsingSearch(_Username, _Password, _ProxyAddress, _ProxyPort, _ProxyUsername, _ProxyPassword,_ConnectionSearchKeyword);
                ConnectUsing_Search.ConnectionSearch(SearchMinDelay, SearchMaxDelay);
            }
            catch
            {
            }
        } 
        #endregion

        #region SearchUsingkeywordForInvite
        public static void SearchUsingkeywordForInvite(ref ManageConnections.ConnectUsingSearchKeywod ConnectUsingSearchKeywod, int DelayStart, int DelayEnd)
        {
            try
            {
                ConnectUsingSearchKeywod.ConnectUsing_SearchKeywod(DelayStart, DelayEnd);
            }
            catch
            {
            }
        }
        #endregion


        public bool CheckAccountDetailDB(string Username,string profileUrl)
        {
            bool checkStatus = true;
            try
            {
                var dateAndTime = DateTime.Now;
                var date = dateAndTime.Date;

                string Date = dateAndTime.ToString("dd/MM/yyyy");

                string selectQuery = "SELECT * FROM tb_AccountsDetails WHERE Usename ='" + _UserName + "'and ProfileId='" + profileUrl + "'and Date='" + Date+"' " ;
                DataSet ds = DataBaseHandler.SelectQuery(selectQuery, "tb_AccountsDetails");
                if (ds.Tables[0].Rows.Count != 0)
                {
                    return checkStatus = false;
                }
                else
                {
                    return checkStatus = true;
                }
               
            }
            catch
            {
                return checkStatus=false;
            };
            return checkStatus;
        }







        #region TestConnectPageSearch
        public void TestConnectPageSearch(string pageNumber, int SearchMinDelay, int SearchMaxDelay)
        {
            int proxyport = 888;
            string SummaryLink = string.Empty;

            Regex PortCheck = new Regex("^[0-9]*$");

           if (PortCheck.IsMatch(_ProxyPort) && !string.IsNullOrEmpty(_ProxyPort))
            {
                proxyport = int.Parse(_ProxyPort);
            }

            //recive the page number and then hit page wise and find the url in which we need to send the request.....

            string UrlConnectpage = "http://www.linkedin.com/search/fpsearch?keywords=" + _ConnectSearchKeyword + "&keepFacets=keepFacets&page_num=" + pageNumber + "&pplSearchOrigin=ADVS&viewCriteria=2&sortCriteria=R&redir=redir";
            string PgSrcMain1 = HttpHelper.getHtmlfromUrlProxy(new Uri(UrlConnectpage), _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);

            string PostData = string.Empty;
            string page_num = string.Empty;
            string postUrl = string.Empty;
            string facetsOrder = string.Empty;

            if (PgSrcMain1.Contains("facetsOrder"))
            {
                facetsOrder = PgSrcMain1.Substring(PgSrcMain1.IndexOf("facetsOrder"), 200);
                string[] Arr = facetsOrder.Split('"');
                facetsOrder = Arr[2];
                string DecodedCharTest = Uri.UnescapeDataString(facetsOrder);
                string DecodedEmail = Uri.EscapeDataString(facetsOrder);
                facetsOrder = DecodedEmail;
            }

            page_num = pageNumber;

            PostData = "searchLocationType=Y&inNetworkSearch=inNetworkSearch&search=&viewCriteria=2&sortCriteria=C&facetsOrder=" + facetsOrder + "&page_num=" + page_num + "&openFacets=N%2CCC%2CG";
            postUrl = "http://www.linkedin.com/vsearch/p?page_num=" + page_num + "&orig=ADVS&keywords=" + _ConnectSearchKeyword + "";

            //Diclaration For Post Add Connection For Keyword
            string Val_goback = string.Empty;
            string Val_sourceAlias = string.Empty;
            string Val_key = string.Empty;
            string Val_defaultText = string.Empty;
            string Val_firstName = string.Empty;
            string Val_CsrToken = string.Empty;
            string Val_Subject = string.Empty;
            string Val_greeting = string.Empty;
            string Val_AuthToken = string.Empty;
            string Val_AuthType = string.Empty;
            string val_trk = string.Empty;
            string Val_lastName = string.Empty;

            string postResponse = HttpHelper.postFormDataProxy(new Uri(postUrl), PostData, _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);
       
            #region commented by sharan
            //  List<string> PageSerchUrl = ChilkatBasedRegex.GettingAllUrls1(postResponse, "profile/view?id"); // commented because it scraping the urls in string format
            #endregion
            
          //  List<string> PageSerchUrl = ChilkatBasedRegex.GettingAllUrls1_writtenBysharan(postResponse, "profile/view?id");

              List<string> PageSerchUrl = ChilkatBasedRegex.GettingAllUrls1_writtenBysharan_fullUrl(postResponse);
            //fkglkfgl


            if (PageSerchUrl.Count == 0)
            {

            }
            string FrnAcceptUrL = string.Empty;
            foreach (string item in PageSerchUrl)
            {
                string FRNURLresponce = string.Empty;

                if (Globals.CheckTheDayLimit)
                {
                    if (Globals.no_of_profiles_can_be_visited == Globals.no_of_profiles_visited)
                    {
                        break;
                    }

                    if (Globals.no_of_profiles_to_be_visited == Globals.no_of_profiles_visited)
                    {
                        return;
                    }
                }
              

                try
                {
                            FrnAcceptUrL = item;
                            string[] urll = Regex.Split(FrnAcceptUrL, "&authType");



                   /// Check Url[0] exit in Database or not 

                        bool check=CheckAccountDetailDB(_UserName,urll[0]);
                        if (!check)
                        {
                            if (OnlyVisitProfile)
                            {
                                LoggerManageConnection("[ " + DateTime.Now + " ] this Profile :" + urll[0] + "  has already been visited today with username : " + _UserName);
                                continue;
                            }
                            else
                            {
                                continue;
                            }
                        }

                            DataSet ds = new DataSet();
                            try
                            {
                                string Querystring = "Select * From tb_ManageAddConnection Where Keyword='" + _ConnectSearchKeyword + "' and LinkedinUrl='" + urll[0] + "'";
                                ds = DataBaseHandler.SelectQuery(Querystring, "tb_ManageAddConnection");
                            }
                            catch { }
                          
                            if (OnlyVisitProfile)
                            {
                                string[] url2 = Regex.Split(FrnAcceptUrL, "&authType");
                                DataSet ds1 = new DataSet();
                                try
                                {
                                    string Querystring = "Select * From tb_OnlyVisitProfile Where Email='" + _UserName + "' and Url='" + url2[0] + "'";
                                    ds1 = DataBaseHandler.SelectQuery(Querystring, "tb_OnlyVisitProfile");
                                }
                                catch { }

                                if (ds.Tables[0].Rows.Count < 1)
                                {
                                    FRNURLresponce = HttpHelper.getHtmlfromUrl1(new Uri(FrnAcceptUrL));
                                }
                            }

                            if (!OnlyVisitProfile)
                            {
                                FRNURLresponce = HttpHelper.getHtmlfromUrl1(new Uri(FrnAcceptUrL));

                                if (FRNURLresponce.Contains("1<sup>st</sup>"))
                                {
                                    string connectionType=Utils.getBetween(FRNURLresponce, "<div class=\"profile-overview\">","profile-aux");
                                    //string check1stCon = Utils.getBetween(FRNURLresponce, "class=\"profile-pic\"", "<a href=\"");
                                   // if (check1stCon.Contains("1<sup>st</sup>"))
                                    if (connectionType.Contains("1<sup>st</sup>"))
                                    {
                                        try
                                        {
                                            string fullname=Utils.getBetween(connectionType,"span class=\"full-name\" dir=\"auto\">","</span>");
                                            if(!string.IsNullOrEmpty(fullname))
                                            {
                                                LoggerManageConnection("[ " + DateTime.Now + " ] =>"+fullname+" is your first connection");
                                            }
                                        }
                                        catch
                                        {}
                                        continue;
                                    }
                                }
                                if (FRNURLresponce.Contains("Upgrade for full name"))
                                {
                                    continue;
                                }
                                //if (FRNURLresponce.Contains("/profile/view?id="))
                                //{
                                //    try
                                //    {
                                //        Val_key = FRNURLresponce.Substring(FRNURLresponce.IndexOf("/profile/view?id="), 30);
                                //        string[] Arr = Val_key.Split('&');
                                //        Val_key = Arr[0].Replace("/profile/view?id=", "");
                                //    }
                                //    catch (Exception ex)
                                //    {

                                //    }

                                //}
                                //if (FrnAcceptUrL.Contains("/profile/view?id="))
                                //{
                                //    try
                                //    {
                                //       // Val_key = FrnAcceptUrL.Substring(FrnAcceptUrL.IndexOf("id="), (FrnAcceptUrL.IndexOf("&", FrnAcceptUrL.IndexOf("id=")) - FrnAcceptUrL.IndexOf("id="))).Replace("id=", string.Empty).Trim();
                                //        Val_key = Utils.getBetween(FrnAcceptUrL+"####", "id=", "####");
                                //    }
                                //    catch { }

                                //}

                                if (FRNURLresponce.Contains("memberId"))
                                {
                                    Val_key = Utils.getBetween(FRNURLresponce, "memberId:\"", "\"}");
                                }
                                if (FRNURLresponce.Contains("csrfToken"))
                                {
                                    try
                                    {
                                        Val_CsrToken = FRNURLresponce.Substring(FRNURLresponce.IndexOf("csrfToken"), 100);
                                        string[] Arr = Val_CsrToken.Split('>'); //Val_CsrToken.Split('&');
                                        Val_CsrToken = Arr[0].Replace("csrfToken=", "").Replace("\"", "").Trim();
                                    }
                                    catch { }

                                }

                                if (FRNURLresponce.Contains("authToken"))
                                {
                                    try
                                    {
                                        int startindex = FRNURLresponce.IndexOf("authToken");
                                        string start = FRNURLresponce.Substring(startindex);
                                        int endindex = start.IndexOf(",");
                                        string end = start.Substring(0, endindex);
                                        Val_AuthToken = end.Replace(",", string.Empty).Replace("authToken=", string.Empty).Replace("\"", string.Empty).Trim();
                                        if (Val_AuthToken.Contains("&"))
                                        {
                                            string[] Arr = Val_AuthToken.Split('&');
                                            Val_AuthToken = Arr[0].Replace("authToken=", string.Empty);
                                        }
                                        if (Val_AuthToken.Contains("</noscript>"))
                                        {
                                            try
                                            {
                                                string[] Val_Auth = Regex.Split(Val_AuthToken, ">");
                                                Val_AuthToken = Val_Auth[0].Replace("\"", string.Empty);
                                            }
                                            catch { }
                                        }
                                    }
                                    catch { }
                                }
                                if (FRNURLresponce.Contains("authType"))
                                {
                                    try
                                    {
                                        Val_AuthType = FRNURLresponce.Substring(FRNURLresponce.IndexOf("authType"), 50);
                                        string[] Arr = Val_AuthType.Split('&');
                                        Val_AuthType = Arr[0].Replace("authType=", "");
                                        if (Val_AuthType.Contains("</noscript>"))
                                        {
                                            Val_AuthType = Val_AuthType.Replace("</noscript>", string.Empty).Replace("\n", string.Empty).Replace(">", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
                                        }
                                        try
                                        {
                                            if (string.IsNullOrEmpty(Val_AuthType))
                                            {
                                                Val_AuthType = "OUT_OF_NETWORK";
                                            }
                                        }
                                        catch { }

                                    }
                                    catch { }
                                }


                                if (FRNURLresponce.Contains("goback"))
                                {
                                    try
                                    {
                                        Val_goback = FRNURLresponce.Substring(FRNURLresponce.IndexOf("goback"), 300);
                                        string[] Arr = Val_goback.Split('"');
                                        Val_goback = Arr[0].Replace("goback=", "");
                                    }
                                    catch { }
                                }
                                if (FRNURLresponce.Contains("trk"))
                                {
                                    try
                                    {
                                        val_trk = FRNURLresponce.Substring(FRNURLresponce.IndexOf("trk"), 100);
                                        string[] Arr = val_trk.Split(',');
                                        val_trk = Arr[0].Replace("trk=", "").Replace("\"", string.Empty);
                                    }
                                    catch { }
                                }

                                if (FRNURLresponce.Contains("\"full-name\""))
                                {
                                    try
                                    {
                                        string[] name = Regex.Split(Utils.getBetween(FRNURLresponce, "\"full-name\" dir=\"auto\">", "</span>"), " ");
                                        Val_firstName = name[0];
                                        Val_lastName = name[1];
                                    }
                                    catch
                                    { }
                                }

                                else
                                {
                                    if (FRNURLresponce.Contains("lastName"))
                                    {
                                        try
                                        {
                                            Val_lastName = FRNURLresponce.Substring(FRNURLresponce.IndexOf("lastName="), 300);
                                            try
                                            {
                                                string[] Arr = Val_lastName.Split('&');
                                                Val_lastName = Arr[0].Replace("lastName=", string.Empty).Replace(",", "").Replace("\"", "").Replace("&#225;", string.Empty).Trim();
                                            }
                                            catch { }
                                        }
                                        catch { }
                                    }

                                    if (FRNURLresponce.Contains("firstName") || FRNURLresponce.Contains("ShortTitle"))
                                    {
                                        try
                                        {
                                            if (FRNURLresponce.Contains("firstName"))
                                            {
                                                try
                                                {
                                                    Val_firstName = FRNURLresponce.Substring(FRNURLresponce.IndexOf("firstName="), 300);
                                                    string[] Arr = Val_firstName.Split('&');
                                                    Val_firstName = Arr[0].Replace("firstName=", "").Replace("i18n_HEADLINE", string.Empty).Replace("\"", "").Replace(",", string.Empty).Replace("}", string.Empty).Replace("link__endpoint", "").Trim();
                                                }
                                                catch { }
                                            }
                                            else if (FRNURLresponce.Contains("ShortTitle"))
                                            {
                                                Val_firstName = FRNURLresponce.Substring(FRNURLresponce.IndexOf("ShortTitle"), 30);
                                                string[] Arr = Val_firstName.Split('"');
                                                Val_firstName = Arr[2].Replace("ShortTitle=", "");
                                            }

                                        }
                                        catch { }
                                    }
                                }

                             
                                try
                                {
                                    string[] Valuesss = Regex.Split(FRNURLresponce, "markAsReadOnClick");
                                    try
                                    {
                                        Val_goback = Valuesss[1].Substring(Valuesss[1].IndexOf("goback="), (Valuesss[1].IndexOf(",", Valuesss[1].IndexOf("goback=")) - Valuesss[1].IndexOf("goback="))).Replace("goback=", string.Empty).Trim();
                                    }
                                    catch { }

                                
                                }
                                catch { }
                               
                                string thiredResponce = "http://www.linkedin.com/people/invite?from=profile&key=" + Val_key + "&firstName=" + Val_firstName + "&lastName=" + Val_lastName + "&authToken=" + Val_AuthToken + "&authType=" + Val_AuthType + "&csrfToken=" + Val_CsrToken + "&goback=" + Val_goback;
                                string pageResponce2 = HttpHelper.getHtmlfromUrl1(new Uri(thiredResponce));
                               
                                if (pageResponce2.Contains("You and this LinkedIn user don’t know anyone in common"))
                                {
                                    string CSVHeader = "UserName" + "," + "SearchKeyword" + "," + "Invitation Sent To" + "," + "InvitationUrl";
                                    string[] url = Regex.Split(FrnAcceptUrL, "&authType");
                                    string CSV_Content = _UserName + "," + _ConnectSearchKeyword + ", ," + url[0];
                                    CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_ConnectionThroughUnkonwPeopleLink);
                                    LoggerManageConnection("[ " + DateTime.Now + " ] => [ Failed To Send Invitation From " + _UserName + " ]");
                                    valid = false;
                                    Globals.no_of_profiles_visited++;
                                    Isaccountvalid = false;
                                    continue;
                                }


                                if (pageResponce2.Contains("sourceAlias"))
                                {
                                    try
                                    {
                                        Val_sourceAlias = pageResponce2.Substring(pageResponce2.IndexOf("sourceAlias"), 100);
                                        string[] Arr = Val_sourceAlias.Split('"');
                                        Val_sourceAlias = Arr[2];
                                    }
                                    catch (Exception ex)
                                    {


                                    }
                                }
                                string[] strNameValue = Regex.Split(pageResponce2, "name=");

                                #region DecareVariable By vicky
                                string iweReconnectSubmit = string.Empty;
                                string iweLimitReached = string.Empty;
                                string companyID0 = string.Empty;
                                string companyName0 = string.Empty;
                                string companyID1 = string.Empty;
                                string schoolID = string.Empty;
                                string schoolcountryCode = string.Empty;
                                string schoolprovinceCode = string.Empty;
                                string subject = string.Empty;
                                string defaultText = string.Empty;
                                string csrfToken = string.Empty;
                                string sourceAlias = string.Empty;
                                string goback = string.Empty;
                                string titleIC0 = string.Empty;
                                string greeting = string.Empty;
                                string startYearIC0 = string.Empty;
                                string endYearIC0 = string.Empty;
                                string schoolText = string.Empty;
                                string titleIB0 = string.Empty;
                                string startYearIB0 = string.Empty;
                                string trk = string.Empty;
                                string authType = string.Empty;
                                string otheremail = string.Empty;
                                string firstName = string.Empty;
                                string lastName = string.Empty;

                                string existingEducation = string.Empty;
                                string activity_210152675 = string.Empty;
                                string activity_210152497 = string.Empty;
                                string existingPositionIB = string.Empty;
                                string reason = string.Empty;

                                #endregion

                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"existingEducation");
                                    int tempstartindex = ArrForValue[1].IndexOf("id=\"");
                                    string strValue = ArrForValue[1].Substring(tempstartindex).Replace("id=\"", string.Empty);
                                    strValue = strValue.Substring(0, strValue.IndexOf("\""));
                                    existingEducation = strValue;
                                }
                                catch { }
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"activity_210152675");
                                    int tempstartindex = ArrForValue[1].IndexOf("id=\"");
                                    string strValue = ArrForValue[1].Substring(tempstartindex).Replace("id=\"", string.Empty);
                                    strValue = strValue.Substring(0, strValue.IndexOf("\""));
                                    activity_210152675 = strValue;
                                }
                                catch { }
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"activity_210152497");
                                    int tempstartindex = ArrForValue[1].IndexOf("id=\"");
                                    string strValue = ArrForValue[1].Substring(tempstartindex).Replace("id=\"", string.Empty);
                                    strValue = strValue.Substring(0, strValue.IndexOf("\""));
                                    activity_210152497 = strValue;
                                }
                                catch { }
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"existingPositionIB");
                                    int tempstartindex = ArrForValue[1].IndexOf("id=\"");
                                    string strValue = ArrForValue[1].Substring(tempstartindex).Replace("id=\"", string.Empty);
                                    strValue = strValue.Substring(0, strValue.IndexOf("\""));
                                    existingPositionIB = strValue;
                                }
                                catch { }
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"reason");
                                    int tempstartindex = ArrForValue[1].IndexOf("value=\"");
                                    string strValue = ArrForValue[1].Substring(tempstartindex).Replace("value=\"", string.Empty);
                                    strValue = strValue.Substring(0, strValue.IndexOf("\""));
                                    reason = strValue;
                                }
                                catch { }
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"startYearIC.0");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    startYearIC0 = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"authType");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    authType = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"authType");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    authType = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"trk");
                                    int tempstartIndex = ArrForValue[1].IndexOf("id=\"");
                                    string strValue = ArrForValue[1].Substring(tempstartIndex).Replace("id=\"", "");
                                    strValue = strValue.Substring(0, strValue.IndexOf("\""));
                                    // string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("type="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("type=")) - ArrForValue[1].IndexOf("type=")).Replace("type=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    trk = (strValue).Trim();

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"endYearIC.0");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    endYearIC0 = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"schoolText");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    schoolText = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"startYearIB.0");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    startYearIB0 = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"titleIB.0");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    titleIB0 = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"companyID.0");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    companyID0 = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"companyID.1");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    companyID1 = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"schoolID");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    schoolID = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"schoolcountryCode");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    schoolcountryCode = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"schoolprovinceCode");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    schoolprovinceCode = (strValue);
                                    if (!string.IsNullOrEmpty(schoolprovinceCode))
                                    {

                                        schoolprovinceCode = "";
                                    }
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"subject");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    subject = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"defaultText");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    defaultText = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"csrfToken");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    csrfToken = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"sourceAlias");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    sourceAlias = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"goback");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    goback = (strValue).Replace("&quot;", "%22").Replace(":", "%3A").Replace(",", "%2C").Replace("/", "%2F").Replace("{", "%7B").Replace(" ", "+");
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"titleIC.0");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    titleIC0 = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"otherEmail");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    otheremail = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"iweLimitReached");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    iweLimitReached = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    try
                                    {
                                        string[] ArrForValue = Regex.Split(pageResponce2, "name=\"iweReconnectSubmit");
                                        string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("class=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                        iweReconnectSubmit = (strValue);
                                    }
                                    catch { }
                                    try
                                    {
                                        if (iweReconnectSubmit.Contains(">") || iweReconnectSubmit.Contains(">"))
                                        {
                                            iweReconnectSubmit = "Send Invitation";
                                        }
                                    }
                                    catch { }
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    try
                                    {
                                        string[] ArrForValue = Regex.Split(pageResponce2, "name=\"greeting");
                                        string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("class=\"message\">"), ArrForValue[1].IndexOf("<", ArrForValue[1].IndexOf("class=\"message\">")) - ArrForValue[1].IndexOf("class=\"message\">")).Replace("class=\"message\">", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                        greeting = (strValue);
                                        greeting = greeting.Replace("&#39;", "'");
                                        greeting = greeting.Replace(" ", "+");
                                        greeting = Uri.EscapeUriString(greeting);
                                        greeting = greeting.Remove(greeting.IndexOf("@"));  //added new
                                    }
                                    catch { }
                                    try
                                    {
                                        if (iweReconnectSubmit.Contains(">") || iweReconnectSubmit.Contains(">"))
                                        {
                                            //iweReconnectSubmit = "Send Invitation";
                                        }
                                    }
                                    catch { }
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"companyName.0");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    companyName0 = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"firstName");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    firstName = (strValue).Replace("&#225;", string.Empty);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"lastName");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace(",", ";").Replace("\"", string.Empty).Trim());
                                    lastName = (strValue);
                                }
                                catch { }
                                #endregion

                                string lastPostUrl = "http://www.linkedin.com/people/iweReconnectAction";
                                //
                                //*********post request for last process of send request 

                                //existingPositionIC=&companyName.0=&titleIC.0=&startYearIC.0=&endYearIC.0=&schoolText=&schoolID=&existingPositionIB=&companyName.1=&titleIB.0=&startYearIB.0=&endYearIB.0=&reason=IF&otherEmail=&greeting=I%27d+like+to+add+you+to+my+professional+network+on+LinkedIn.%0D%0A%0D%0A-+gaurav+agrawal&iweReconnectSubmit=Send+Invitation&key=141450&firstName=Ron&lastName=Bates&authToken=d7Ir&authType=OPENLINK&trk=prof-0-sb-connect-button&iweLimitReached=false&companyID.0=&companyID.1=&schoolID=&schoolcountryCode=&schoolprovinceCode=&javascriptEnabled=false&subject=gaurav+agrawal+wants+to+connect+on+LinkedIn+&defaultText=a908b3ba18701677a5879c1955035d05&csrfToken=ajax%3A8096156173388039261&                                                            sourceAlias=0_0ZD1VB-f52hFxNsCjPZdLV&goback=.fps_PBCK_*1_*1_*1_*1_*1_*1_*1_*2_*1_Y_*1_*1_*1_false_1_C_*1_*51_*1_*51_true_CC%2CN%2CG%2CI%2CPC%2CED%2CL%2CFG%2CTE%2CFA%2CSE%2CP%2CCS%2CF%2CDR_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2.npv_141450_*1_*1_OPENLINK_d7Ir_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1;
                                string NameOfSender = _UserName.Remove(_UserName.IndexOf("@"));
                                // tab-name username">
                                string SenderName = string.Empty;
                                try
                                {
                                    SenderName = FRNURLresponce.Substring(FRNURLresponce.IndexOf("tab-name username\">"), (FRNURLresponce.IndexOf("<", FRNURLresponce.IndexOf("tab-name username\">")) - FRNURLresponce.IndexOf("tab-name username\">"))).Replace("tab-name username\">", string.Empty).Trim().Replace("tab-name username\">", string.Empty);
                                    NameOfSender = Uri.EscapeUriString(SenderName);
                                }
                                catch { }

                                string LastPostData = "companyName.0=" + companyName0 + "titleIC.0=" + titleIC0 + "&startYearIC.0=" + startYearIC0 + "&endYearIC.0=" + endYearIC0 + "&schoolText=" + schoolText + "&schoolID=" + schoolID + "&companyName.1=" + companyName0 + "&titleIB.0=" + titleIB0 + "&startYearIB.0=" + startYearIB0 + "&endYearIB.0=" + endYearIC0 + "&reason=IF&otherEmail=" + otheremail + "&greeting=" + greeting + "&iweReconnectSubmit=" + iweReconnectSubmit.Replace(" ", "+") + "&key=" + Val_key + "&firstName=" + firstName + "&lastName=" + lastName + "&authToken=" + Val_AuthToken + "&authType=" + Val_AuthType + "&trk=" + trk + "&iweLimitReached=false&companyID.0=" + companyID0 + "&companyID.1=" + companyID1 + "&schoolID=" + schoolID + "&schoolcountryCode=" + schoolcountryCode + "&schoolprovinceCode=" + schoolprovinceCode + "&javascriptEnabled=false&subject=" + subject.Replace(" ", "+") + "&defaultText=" + defaultText + "&csrfToken=" + Val_CsrToken + "&sourceAlias=" + sourceAlias + "&goback=" + Val_goback;
                                string postResponse1 = HttpHelper.postFormDataProxy(new Uri(lastPostUrl), LastPostData, _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);
                            
                                //Esuccess
                                string FinalValue = string.Empty;
                                try
                                {
                                    FinalValue = postResponse1.Substring(postResponse1.IndexOf("Esuccess"), (postResponse1.IndexOf(">", postResponse1.IndexOf("Esuccess")) - postResponse1.IndexOf("Esuccess"))).Replace("Esuccess", string.Empty).Trim();
                                    FinalValue = Uri.UnescapeDataString(FinalValue);
                                    string[] valuess = Regex.Split(FinalValue, ",");
                                    FinalValue = valuess[0].Replace("=", string.Empty).Replace("\"", string.Empty);
                                }
                                catch { }
                                try
                                {
                                    string lastGetResponse = HttpHelper.getHtmlfromUrl1(new Uri("http://www.linkedin.com/people/pymk?goback=" + Val_goback + "&trk=" + val_trk + "&report%2Esuccess=" + FinalValue));
                                }
                                catch { }
                                if (postResponse1.Contains("You must confirm your primary email address before sending an invitation."))
                                {
                                    LoggerManageConnection("[ " + DateTime.Now + " ] => [ " + _UserName + "You must confirm your primary email address before sending an invitation. ]");
                                    valid = false;
                                    PageNumber = 0;
                                    Isaccountvalid = false;
                                    break;
                                }
                              

                                    if (postResponse1.Contains("<strong>Invitations") || postResponse1.Contains("Send Invitation") || postResponse1.Contains("Send invitation"))
                                    {
                                        if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
                                        {
                                            firstName = "linkedin";
                                            lastName = "Member";
                                        }
                                        LoggerManageConnection("[ " + DateTime.Now + " ] => [ Invitation to " + firstName + " " + lastName + " sent From Account :" + _UserName + " ]");
                                        string CSVHeader = "UserName" + "," + "SearchKeyword" + "," + "Invitation Sent To" + "," + "InvitationUrl";
                                        try
                                        {
                                            string[] url = Regex.Split(FrnAcceptUrL, "&authType");
                                            string CSV_Content = _UserName + "," + _ConnectSearchKeyword + "," + firstName + " " + lastName + "," + url[0];
                                            CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_AddConnectionSuccessWith2ndDegree);
                                            valid = true;
                                            insertAccountData(_UserName, url[0], SendInvitationCount);
                                            
                                   
                                        }
                                        catch { }
                                    }
                                    else if (pageResponce2.Contains("You sent an invitation to") || pageResponce2.Contains("Invitation to") || pageResponce2.Contains("to connect on LinkedIn"))
                                    {
                                        LoggerManageConnection("[ " + DateTime.Now + " ] => [ Invitation to " + Val_firstName + " " + Val_lastName + " sent From Account :" + _UserName + " ]");
                                        string CSVHeader = "UserName" + "," + "SearchKeyword" + "," + "Invitation Sent To" + "," + "InvitationUrl";
                                        try
                                        {
                                            string[] url = Regex.Split(FrnAcceptUrL, "&authType");
                                            string CSV_Content = _UserName + "," + _ConnectSearchKeyword + "," + firstName + " " + lastName + "," + url[0];
                                            CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_AddConnectionSuccessWith2ndDegree);
                                            valid = true;
                                            insertAccountData(_UserName, url[0], SendInvitationCount);
                                        }
                                        catch { }
                                    }
                                    else if (postResponse1.Contains("You and this LinkedIn user don’t know anyone in common"))
                                    {
                                        string CSVHeader = "UserName" + "," + "SearchKeyword" + "," + "Invitation Sent To" + "," + "InvitationUrl";
                                        string[] url = Regex.Split(FrnAcceptUrL, "&authType");
                                        string CSV_Content = _UserName + "," + _ConnectSearchKeyword + ", ," + url[0];
                                        CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_ConnectionThroughUnkonwPeopleLink);
                                        LoggerManageConnection("[ " + DateTime.Now + " ] => [ Failed To Send Invitation From " + _UserName + " ]");
                                        valid = false;
                                        Isaccountvalid = false;

                                      
                                    }
                                    else
                                    {
                                        string CSVHeader = "UserName" + "," + "SearchKeyword" + "," + "Invitation Sent To" + "," + "InvitationUrl";
                                        string[] url = Regex.Split(FrnAcceptUrL, "&authType");
                                        string CSV_Content = _UserName + "," + _ConnectSearchKeyword + "," + firstName + " " + lastName + "," + url[0];
                                        CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_ConnectionthroughKeywordSearchnotadded);
                                        LoggerManageConnection("[ " + DateTime.Now + " ] => [ Failed To Send Invitation From " + _UserName + " ]");
                                        valid = false;
                                        SendInvitationCount--;

                                    }

                              
                                int Delay = RandomNumberGenerator.GenerateRandom(SearchMinDelay, SearchMaxDelay);
                                Thread.Sleep(Delay * 1000);
                                LoggerManageConnection("[ " + DateTime.Now + " ] => [ Invitation Delayed For : " + Delay + " Seconds ]");

                                SendInvitationCount++;
                                if (SendInvitationCount >= SearchCriteria.NumberOfRequestPerKeyword)  //Here we check  that number of request send if counter is equal to the number of request put by user then it return to mathod and made pagenumber = 0; 
                                {  
                                    PageNumber = 0;
                                    return;
                                }
                            }

                            else
                            {
                                LoggerManageConnection("[ " + DateTime.Now + " ] => [ Only visit the site " + FrnAcceptUrL + " ]");
                                string CSVHeader = "UserName" + "," + "Url";
                               

                                string[] url = Regex.Split(FrnAcceptUrL, "&authType");
                                string CSV_Content = _UserName + "," + url[0];

                                insertAccountData(_UserName, url[0], SendInvitationCount);

                                CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_OnlyVisitProfileUsingKeyword);

                                DataSet ds2 = new DataSet();
                                try
                                {
                                    string Querystring = "INSERT INTO tb_OnlyVisitProfile (Email,Url,DateTime) Values ('" + _UserName + "','" + url[0] + "','" + DateTime.Now + "')";
                                    ds2 = DataBaseHandler.SelectQuery(Querystring, "tb_OnlyVisitProfile");
                                }
                                catch { }


                                int Delay = RandomNumberGenerator.GenerateRandom(SearchMinDelay, SearchMaxDelay);
                                Thread.Sleep(Delay * 1000);
                                LoggerManageConnection("[ " + DateTime.Now + " ] => [ Delayed For : " + Delay + " Seconds ]");

                                Globals.no_of_profiles_visited++;
                            }
                     
                }
                catch (Exception ex)
                {
                    string CSVHeader = "UserName" + "," + "SearchKeyword" + "," + "Invitation Sent To" + "," + "InvitationUrl";
                    string[] url = Regex.Split(FrnAcceptUrL, "&authType");
                    string CSV_Content = _UserName + "," + _ConnectSearchKeyword + ",," + url[0];
                    CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_ConnectionthroughKeywordSearchnotadded);
                    LoggerManageConnection("[ " + DateTime.Now + " ] => [ Failed To Send Invitation From " + _UserName + " ]");
                    valid = false;
                    Isaccountvalid = false;
                }
            }
        }
        #endregion


        public void insertAccountData(string _UserName, string profileID, int SendInvitationCount)
        {

            clsDBQueryManager Qm = new clsDBQueryManager();
            var dateAndTime = DateTime.Now;
            var date = dateAndTime.Date;

           string  Date = dateAndTime.ToString("dd/MM/yyyy");
           Qm.InsertAccountsDetailsSetting(_UserName, profileID, Date, SendInvitationCount);
        }

        #region ConnectionSearchPymknow()
        public void ConnectionSearchPymKnow()
        {
            try
            {
                //int PageNumber = 0;
                string strPageNumber = string.Empty;
                string Textmessage = string.Empty;

                if (linkedinLoginAndLogout.LoginHttpHelper(_UserName, _Password, _ProxyAddress, _ProxyPort, _ProxyUserName, _ProxyPassword, ref HttpHelper, ref Textmessage))
                {
                    // LoggerManageConnection("Logged in with " + _UserName);

                    int proxyport = 888;
                    string SummaryLink = string.Empty;

                    Regex PortCheck = new Regex("^[0-9]*$");

                    if (PortCheck.IsMatch(_ProxyPort) && !string.IsNullOrEmpty(_ProxyPort))
                    {
                        proxyport = int.Parse(_ProxyPort);
                    }


                    string Url = "http://www.linkedin.com/people/pymk?trk=frontier-tabs_connections-new_pymk";// "http://www.linkedin.com/search?search="+_ConnectSearchKeyword+"&sortCriteria=Distance_Relevance&viewCriteria=2&excoon=true&newnessType=M&searchLocationType=Y&proposalType=Y&trk=nmp_profile_network_stats_new_people#facets=searchLocationType%3DY%26inNetworkSearch%3DinNetworkSearch%26page_num%3D1%26search%3D%26viewCriteria%3D2%26facetsOrder%3DCC%252CN%252CG%252CI%252CPC%252CED%252CL%252CFG%252CTE%252CFA%252CSE%252CP%252CCS%252CF%252CDR%26sortCriteria%3DC%26clickAction%3Dsort%26openFacets%3DN%252CCC%252CG";
                    string PgSrcMain = HttpHelper.getHtmlfromUrlProxy(new Uri(Url), _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);
                    ConnectPageSearchPymKnow();

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region ConnectPageSearchPymknow
        public void ConnectPageSearchPymKnow()
        {
            int proxyport = 888;
            string SummaryLink = string.Empty;

            Regex PortCheck = new Regex("^[0-9]*$");

            if (PortCheck.IsMatch(_ProxyPort) && !string.IsNullOrEmpty(_ProxyPort))
            {
                proxyport = int.Parse(_ProxyPort);
            }
            string UrlConnectpage = "http://www.linkedin.com/people/pymk?trk=frontier-tabs_connections-new_pymk";

            string PgSrcMain1 = HttpHelper.getHtmlfromUrlProxy(new Uri(UrlConnectpage), _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);
            //find peopleCount:12
            //find rnd



            string test = "http://www.linkedin.com/people/pymk-people-cards?locale=en_US&records=12&offset=133&facetID=&facetType=&decorate=false&trk=new_pymk-show_more&rnd=1343042316579";
            string PgSrcMain2 = HttpHelper.getHtmlfromUrlProxy(new Uri(test), _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);
            string IDdata = string.Empty;
            List<string> lstIDForAddconnection = new List<string>();

            if (PgSrcMain2.Contains("peopleCount\":12"))
            {
                List<string> URLID = Regex.Split(PgSrcMain2, "/profile/connections?").ToList();
                foreach (string item in URLID)
                {
                    if (item.Contains("profileURL"))
                    {
                        //item.Substring(PgSrcMain1.IndexOf("profileURL"), 120);

                        string[] Arr = item.Split('&');
                        IDdata = Arr[0].Replace("?id=", "").Trim();
                        lstIDForAddconnection.Add("http://www.linkedin.com/profile/view?id=" + IDdata);
                    }

                }



                //List<string> PageSerchUrl1 = ChilkatBasedRegex.GettingAllUrls(PgSrcMain2, "/profile/connections");
                //List<string> URLID = Regex.Split(PgSrcMain2, "/profile/connections?").ToList();
                #region Declaration
                string PostData = string.Empty;
                string page_num = string.Empty;
                string postUrl = string.Empty;
                string facetsOrder = string.Empty;



                //Diclaration For Post Add Connection For Keyword
                string Val_goback = string.Empty;
                string Val_sourceAlias = string.Empty;
                string Val_key = string.Empty;
                string Val_defaultText = string.Empty;
                string Val_firstName = string.Empty;
                string Val_CsrToken = string.Empty;
                string Val_Subject = string.Empty;
                string Val_greeting = string.Empty;
                string Val_AuthToken = string.Empty;
                string Val_AuthType = string.Empty;
                string val_trk = string.Empty;
                string Val_lastName = string.Empty;

                string Rnd = string.Empty;
                #endregion

                //Post Response
                // string postResponse = HttpHelper.postFormDataProxy(new Uri(postUrl), PostData, _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);
                List<string> PageSerchUrl = ChilkatBasedRegex.GettingAllUrls(PgSrcMain1, "profile/view?id");
                string FrnAcceptUrL = string.Empty;
                #region parsing
                foreach (string item in lstIDForAddconnection)
                {
                    if (item.Contains("http://www.linkedin.com/profile/view?id="))
                    {
                        FrnAcceptUrL = item;


                        string FRNURLresponce = HttpHelper.getHtmlfromUrl1(new Uri(FrnAcceptUrL));

                        if (FRNURLresponce.Contains("rnd"))
                        {
                            try
                            {
                                Rnd = FRNURLresponce.Substring(FRNURLresponce.IndexOf("rnd"), 100);
                                string[] Arr = Rnd.Split('&');
                                Rnd = Arr[0].Replace("rnd=", "");
                            }
                            catch (Exception ex)
                            {


                            }

                        }


                        if (FRNURLresponce.Contains("http://www.linkedin.com/profile/view?id="))
                        {
                            try
                            {
                                Val_key = FRNURLresponce.Substring(FRNURLresponce.IndexOf("/profile/view?id="), 30);
                                string[] Arr = Val_key.Split('&');
                                Val_key = Arr[0].Replace("/profile/view?id=", "");
                            }
                            catch (Exception ex)
                            {


                            }

                        }

                        if (FRNURLresponce.Contains("csrfToken"))
                        {
                            try
                            {
                                Val_CsrToken = FRNURLresponce.Substring(FRNURLresponce.IndexOf("csrfToken"), 100);
                                string[] Arr = Val_CsrToken.Split('&');
                                Val_CsrToken = Arr[0].Replace("csrfToken=", "");
                            }
                            catch (Exception ex)
                            {


                            }

                        }




                        if (FRNURLresponce.Contains("authToken"))
                        {
                            try
                            {
                                Val_AuthToken = FRNURLresponce.Substring(FRNURLresponce.IndexOf("authToken"), 50);
                                string[] Arr = Val_AuthToken.Split('"');
                                Val_AuthToken = Arr[0].Replace("authToken=", "");
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        if (FRNURLresponce.Contains("authType"))
                        {
                            try
                            {
                                Val_AuthType = FRNURLresponce.Substring(FRNURLresponce.IndexOf("authType"), 50);
                                string[] Arr = Val_AuthType.Split('&');
                                Val_AuthType = Arr[0].Replace("authType=", "");
                            }
                            catch (Exception ex)
                            {

                            }
                        }


                        if (FRNURLresponce.Contains("goback"))
                        {
                            try
                            {
                                Val_goback = FRNURLresponce.Substring(FRNURLresponce.IndexOf("goback"), 300);
                                string[] Arr = Val_goback.Split('"');
                                Val_goback = Arr[0].Replace("goback=", "");
                            }
                            catch (Exception ex)
                            {


                            }
                        }
                        if (FRNURLresponce.Contains("trk"))
                        {
                            try
                            {
                                val_trk = FRNURLresponce.Substring(FRNURLresponce.IndexOf("trk"), 100);
                                string[] Arr = val_trk.Split(';');
                                val_trk = Arr[0].Replace("trk=", "");
                            }
                            catch (Exception ex)
                            {


                            }
                        }

                        if (FRNURLresponce.Contains("lastName"))
                        {
                            try
                            {
                                Val_lastName = FRNURLresponce.Substring(FRNURLresponce.IndexOf("lastName"), 30);
                                string[] Arr = Val_lastName.Split('&');
                                Val_lastName = Arr[0].Replace("lastName=", "");
                            }
                            catch (Exception ex)
                            {


                            }
                        }

                        if (FRNURLresponce.Contains("firstName"))
                        {
                            try
                            {
                                Val_firstName = FRNURLresponce.Substring(FRNURLresponce.IndexOf("firstName"), 30);
                                string[] Arr = Val_firstName.Split('&');
                                Val_firstName = Arr[0].Replace("firstName=", "");
                            }
                            catch (Exception ex)
                            {


                            }



                        }


                #endregion                    //Frnd Requset Send Option URl
                        //http://www.linkedin.com/people/pymk-people-cards?locale=en_US&records=12&offset=13&facetID=&facetType=&decorate=false&trk=new_pymk-show_more&rnd=1343134697855
                        //



                        string thiredResponce = "http://www.linkedin.com/people/invite?from=profile&key=" + IDdata + "&firstName=" + Val_firstName + "&lastName=" + Val_lastName + "&authToken=" + Val_AuthToken + "&authType=" + Val_AuthType + "&goback=" + Val_goback + "%2Enpv_141450_*1_*1_OPENLINK_d7Ir_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1&trk=prof-0-sb-connect-button";
                        string SecondUrl = "http://www.linkedin.com/people/pymk-people-cards?locale=en_US&records=12&offset=37&facetID=&facetType=&decorate=false&trk=new_pymk-show_more&rnd=1343134804144";
                        string pageResponce2 = HttpHelper.getHtmlfromUrl1(new Uri(SecondUrl));

                        if (pageResponce2.Contains("sourceAlias"))
                        {
                            try
                            {
                                Val_sourceAlias = pageResponce2.Substring(pageResponce2.IndexOf("sourceAlias"), 100);
                                string[] Arr = Val_sourceAlias.Split('"');
                                Val_sourceAlias = Arr[2];
                            }
                            catch (Exception ex)
                            {


                            }
                        }


                        string lastPostUrl = "http://www.linkedin.com/people/iweReconnectAction";
                        //
                        //existingPositionIC=&companyName.0=&titleIC.0=&startYearIC.0=&endYearIC.0=&schoolText=&schoolID=&existingPositionIB=&companyName.1=&titleIB.0=&startYearIB.0=&endYearIB.0=&reason=IF&otherEmail=&greeting=I%27d+like+to+add+you+to+my+professional+network+on+LinkedIn.%0D%0A%0D%0A-+gaurav+agrawal&iweReconnectSubmit=Send+Invitation&key=141450&firstName=Ron&lastName=Bates&authToken=d7Ir&authType=OPENLINK&trk=prof-0-sb-connect-button&iweLimitReached=false&companyID.0=&companyID.1=&schoolID=&schoolcountryCode=&schoolprovinceCode=&javascriptEnabled=false&subject=gaurav+agrawal+wants+to+connect+on+LinkedIn+&defaultText=a908b3ba18701677a5879c1955035d05&csrfToken=ajax%3A8096156173388039261&                                                            sourceAlias=0_0ZD1VB-f52hFxNsCjPZdLV&goback=.fps_PBCK_*1_*1_*1_*1_*1_*1_*1_*2_*1_Y_*1_*1_*1_false_1_C_*1_*51_*1_*51_true_CC%2CN%2CG%2CI%2CPC%2CED%2CL%2CFG%2CTE%2CFA%2CSE%2CP%2CCS%2CF%2CDR_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2.npv_141450_*1_*1_OPENLINK_d7Ir_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1;
                        string LastPostData = "existingPositionIC=&companyName.0=&titleIC.0=&startYearIC.0=&endYearIC.0=&schoolText=&schoolID=&existingPositionIB=&companyName.1=&titleIB.0=&startYearIB.0=&endYearIB.0=&reason=IF&otherEmail=&greeting=I%27d+like+to+add+you+to+my+professional+network+on+LinkedIn.%0D%0A%0D%0A-+gaurav+agrawal&iweReconnectSubmit=Send+Invitation&key=" + Val_key + "&firstName=" + Val_firstName + "&lastName=" + Val_lastName + "&authToken=" + Val_AuthToken + "&authType=" + Val_AuthType + "&trk=prof-0-sb-connect-button&iweLimitReached=false&companyID.0=&companyID.1=&schoolID=&schoolcountryCode=&schoolprovinceCode=&javascriptEnabled=false&subject=gaurav+agrawal+wants+to+connect+on+LinkedIn+&defaultText=a908b3ba18701677a5879c1955035d05&csrfToken=" + Val_CsrToken + "&sourceAlias=" + Val_sourceAlias + "&goback=" + Val_goback.Replace("%2E", ".") + ".npv_" + Val_key + "_*1_*1_OPENLINK_d7Ir_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1";
                        string postResponse1 = HttpHelper.postFormDataProxy(new Uri(lastPostUrl), LastPostData, _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);

                        if (postResponse1.Contains("Accept Invitations"))
                        {
                            LoggerManageConnection("[ " + DateTime.Now + " ] => [ Invitation to " + Val_firstName + " sent. ]");
                        }

                        else
                        {
                            LoggerManageConnection("[ " + DateTime.Now + " ] => [ Failed To Send Invitation To" + Val_firstName + " ]");
                        }


                    }
                }

            }
            else
            {
                LoggerManageConnection("[ " + DateTime.Now + " ] => [ Connection End ]");
            }

        }
        #endregion

        #region LoggerManageConnection
        private void LoggerManageConnection(string message)
        {
            EventsArgs eventArgs = new EventsArgs(message);
            ConnectSearchLogEvents.LogText(eventArgs);
        }
        #endregion

        #region TestConnectionUniqueSearch
        static List<string> AllPageSerchUrl = new List<string>();
        private List<string> TestConnectionUniqueSearch(string pageNumber)
        {
            List<string> PageSerchUrl = new List<string>();
            List<string> lstSearchResult = new List<string>();
            try
            {

                int proxyport = 888;
                string SummaryLink = string.Empty;

                Regex PortCheck = new Regex("^[0-9]*$");

                if (PortCheck.IsMatch(_ProxyPort) && !string.IsNullOrEmpty(_ProxyPort))
                {
                    proxyport = int.Parse(_ProxyPort);
                }

                //recive the page number and then hit page wise and find the url in which we need to send the request.....

                string UrlConnectpage = "http://www.linkedin.com/search/fpsearch?keywords=" + _ConnectSearchKeyword + "&searchLocationType=I&countryCode=in&keepFacets=keepFacets&page_num=" + pageNumber + "&pplSearchOrigin=ADVS&viewCriteria=2&sortCriteria=R&redir=redir";
                //  string UrlConnectpage = "http://www.linkedin.com/search?search=&sortCriteria=Distance_Relevance&viewCriteria=2&excoon=true&newnessType=M&searchLocationType=Y&proposalType=Y&trk=nmp_profile_network_stats_new_people#facets=searchLocationType%3DY%26inNetworkSearch%3DinNetworkSearch%26search%3D%26viewCriteria%3D2%26sortCriteria%3DC%26facetsOrder%3DCC%252CN%252CG%252CI%252CPC%252CED%252CL%252CFG%252CTE%252CFA%252CSE%252CP%252CCS%252CF%252CDR%26page_num%" + pageNumber + "D2%26openFacets%3DN%252CCC%252CG";
                string PgSrcMain1 = HttpHelper.getHtmlfromUrlProxy(new Uri(UrlConnectpage), _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);


                string PostData = string.Empty;
                string page_num = string.Empty;
                string postUrl = string.Empty;
                string facetsOrder = string.Empty;

                if (PgSrcMain1.Contains("facetsOrder"))
                {
                    facetsOrder = PgSrcMain1.Substring(PgSrcMain1.IndexOf("facetsOrder"), 200);
                    string[] Arr = facetsOrder.Split('"');
                    facetsOrder = Arr[2];
                    string DecodedCharTest = Uri.UnescapeDataString(facetsOrder);
                    string DecodedEmail = Uri.EscapeDataString(facetsOrder);
                    facetsOrder = DecodedEmail;
                }

                page_num = pageNumber;

                PostData = "searchLocationType=Y&inNetworkSearch=inNetworkSearch&search=&viewCriteria=2&sortCriteria=C&facetsOrder=" + facetsOrder + "&page_num=" + page_num + "&openFacets=N%2CCC%2CG";

                //Post Url for Invite Message 
                postUrl = "http://www.linkedin.com/search/hits";

                //Diclaration For Post Add Connection For Keyword


                //Post Response
                string postResponse = HttpHelper.postFormDataProxy(new Uri(postUrl), PostData, _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);
                PageSerchUrl = ChilkatBasedRegex.GettingAllUrls(PgSrcMain1, "profile/view?id");
                foreach (string item in PageSerchUrl)
                {
                    try
                    {
                        if (item.Contains("pp_profile_name_link"))  //this key word can only seprate the Url with all common urls....
                        {
                            try
                            {
                                string FrnAcceptUrL = "http://www.linkedin.com" + item;
                                lstSearchResult.Add(FrnAcceptUrL);
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
                return lstSearchResult;
            }
            catch { }
            return lstSearchResult;


        }
        #endregion

        #region TestConnectPageSearchForUniqueUrl
        Queue<string> lstqueueUrlsForKeywords = new Queue<string>();

        //private void AddConnectionUnique()
        //{
        //    string strPageNumber = string.Empty;
        //    string Textmessage = string.Empty;


        //    //Login with User name and password ***********************

        //    if (linkedinLoginAndLogout.LoginHttpHelper(_UserName, _Password, _ProxyAddress, _ProxyPort, _ProxyUserName, _ProxyPassword, ref HttpHelper, ref Textmessage))
        //    {
        //        LoggerManageConnection("Logged in with " + _UserName);

        //        int proxyport = 888;
        //        string SummaryLink = string.Empty;

        //        Regex PortCheck = new Regex("^[0-9]*$");

        //        if (PortCheck.IsMatch(_ProxyPort) && !string.IsNullOrEmpty(_ProxyPort))
        //        {
        //            proxyport = int.Parse(_ProxyPort);
        //        }

        //        while (UsingKeyWordQueue.Count > 0)
        //        {

        //            lock (locker_InvitataionKeyword)
        //            {
        //                if (UsingKeyWordQueue.Count > 0)
        //                {
        //                    _ConnectSearchKeyword = UsingKeyWordQueue.Dequeue();//pass the keyword to the string
        //                }
        //                else
        //                {
        //                    LoggerManageConnection("All Loaded Keyword are used");
        //                }
        //            }

        //            //for search make a URL with our input and then used with get request********************* 
        //            //if(lstqueueUrlsForKeywords.Count>1)
        //            //{
        //            string Url = "http://www.linkedin.com/search/fpsearch?keywords=" + _ConnectSearchKeyword + "&searchLocationType=I&countryCode=in&keepFacets=keepFacets&page_num=1&pplSearchOrigin=ADVS&viewCriteria=2&sortCriteria=R&redir=redir";
        //            //string Url = "http://www.linkedin.com/search?search="+_ConnectSearchKeyword+"&sortCriteria=Distance_Relevance&viewCriteria=2&excoon=true&newnessType=M&searchLocationType=Y&proposalType=Y&trk=nmp_profile_network_stats_new_people#facets=searchLocationType%3DY%26inNetworkSearch%3DinNetworkSearch%26page_num%3D1%26search%3D%26viewCriteria%3D2%26facetsOrder%3DCC%252CN%252CG%252CI%252CPC%252CED%252CL%252CFG%252CTE%252CFA%252CSE%252CP%252CCS%252CF%252CDR%26sortCriteria%3DC%26clickAction%3Dsort%26openFacets%3DN%252CCC%252CG";
        //            string PgSrcMain = HttpHelper.getHtmlfromUrlProxy(new Uri(Url), _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);

        //            string[] Arr = Regex.Split(PgSrcMain, "<li");
        //            foreach (string item in Arr)
        //            {
        //                if (!item.Contains("<!DOCTYPE"))
        //                {
        //                    if (item.Contains("results-summary"))
        //                    {
        //                        string data = RemoveAllHtmlTag.StripHTML(item);
        //                        data = data.Replace("\n", "");
        //                        if (data.Contains(">"))
        //                        {
        //                            string[] ArrTemp = data.Split('>');
        //                            data = ArrTemp[1];
        //                            data = data.Replace("results", "");
        //                            data = data.Trim();
        //                            string[] ArrTemp1 = data.Split(' ');
        //                            data = ArrTemp1[0].Replace(',', ' ').Trim();

        //                            strPageNumber = data.Replace(" ", string.Empty);
        //                            break;
        //                        }

        //                    }
        //                }
        //            }

        //            try
        //            {
        //                PageNumber = int.Parse(strPageNumber);//find the page number for each key word
        //            }
        //            catch (Exception)
        //            {

        //                throw;
        //            }
        //            if (!(PageNumber > 0))
        //            {

        //                TestConnectPageSearch("1");
        //            }

        //            PageNumber = (PageNumber / 10) - 1;//each page have 10 data so number of record divide by 10....

        //            for (int i = 1; i <= PageNumber; i++)
        //            {
        //                //loop for send request
        //                if (!Isaccountvalid)
        //                {
        //                    return;
        //                }

        //                if (true)
        //                {
        //                    List<string> lstUrls = TestConnectionUniqueSearch(i.ToString());
        //                    AllPageSerchUrl.AddRange(lstUrls);
        //                    int Account=Globals.listAccounts.Count;
        //                    int NeedLinkedinUrl = Account * countofKeywords;
        //                    if (AllPageSerchUrl.Count >= NeedLinkedinUrl)
        //                    {
        //                        foreach (string item in AllPageSerchUrl)
        //                        {
        //                            try
        //                            {
        //                                lstqueueUrlsForKeywords.Enqueue(item);
        //                            }
        //                            catch { }
        //                        }
        //                        break;
        //                    }
        //                }
        //                // TestConnectPageSearch(i.ToString());
        //            }



        //           // for (int i = 0; i <= AllPageSerchUrl.Count;i++)
        //            {
        //                try
        //                {
        //                    string item=lstqueueUrlsForKeywords.Dequeue();
        //                    SentAddConnection(item);
        //                }
        //                catch { }
        //            }

        //            if (valid)
        //            {
        //                string CSVHeader = "UserName" + "," + "SearchKeyword";
        //                string CSV_Content = _UserName + "," + _ConnectSearchKeyword;
        //                // CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_AddConnectionFail);
        //                //LoggerManageConnection("*************************************************************************************************");
        //                //LoggerManageConnection("*************************************************************************************************");

        //                //LoggerManageConnection("*************************************************************************************************");
        //                //LoggerManageConnection("*************************************************************************************************");
        //                if (SendInvitationCount > 0)
        //                {

        //                    LoggerManageConnection(SendInvitationCount + "  Send Request Completed With " + _UserName + " and For Keyword " + _ConnectSearchKeyword);
        //                    //GlobusFileHelper.AppendStringToTextfileNewLine(_ConnectSearchKeyword + "  " +_UserName , Globals.path_AddConnection);
        //                    //CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_AddConnectionSuccess);

        //                }
        //                else
        //                {
        //                    CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_AddConnectionFail);
        //                    LoggerManageConnection("Not Able To send Any Connection Request For Keyword " + _ConnectSearchKeyword);
        //                    // GlobusFileHelper.AppendStringToTextfileNewLine(_ConnectSearchKeyword + "  " + _UserName, Globals.path_AddConnection);

        //                }
        //                //LoggerManageConnection("*************************************************************************************************");
        //                //LoggerManageConnection("*************************************************************************************************");
        //                SendInvitationCount = 0;
        //                //LoggerManageConnection("Send Request Completed");
        //            }
        //        }

        //    }
        //    else
        //    {
        //        LoggerManageConnection(_UserName + "Your LinkedIn account has been temporarily restricted");
        //        return;
        //    }

        //}
        //private void SentAddConnection(string FrnAcceptUrL)
        //{

        //    string Val_goback = string.Empty;
        //    string Val_sourceAlias = string.Empty;
        //    string Val_key = string.Empty;
        //    string Val_defaultText = string.Empty;
        //    string Val_firstName = string.Empty;
        //    string Val_CsrToken = string.Empty;
        //    string Val_Subject = string.Empty;
        //    string Val_greeting = string.Empty;
        //    string Val_AuthToken = string.Empty;
        //    string Val_AuthType = string.Empty;
        //    string val_trk = string.Empty;
        //    string Val_lastName = string.Empty;
        //    string FRNURLresponce = HttpHelper.getHtmlfromUrl(new Uri(FrnAcceptUrL));

        //    if (FRNURLresponce.Contains("/profile/view?id="))
        //    {
        //        try
        //        {
        //            Val_key = FRNURLresponce.Substring(FRNURLresponce.IndexOf("/profile/view?id="), 30);
        //            string[] Arr = Val_key.Split('&');
        //            Val_key = Arr[0].Replace("/profile/view?id=", "");
        //        }
        //        catch (Exception ex)
        //        {


        //        }

        //    }

        //    if (FRNURLresponce.Contains("csrfToken"))
        //    {
        //        try
        //        {
        //            Val_CsrToken = FRNURLresponce.Substring(FRNURLresponce.IndexOf("csrfToken"), 100);
        //            string[] Arr = Val_CsrToken.Split('&');
        //            Val_CsrToken = Arr[0].Replace("csrfToken=", "");
        //        }
        //        catch (Exception ex)
        //        {


        //        }

        //    }

        //    if (FRNURLresponce.Contains("authToken"))
        //    {
        //        try
        //        {
        //            Val_AuthToken = FRNURLresponce.Substring(FRNURLresponce.IndexOf("authToken"), 50);
        //            string[] Arr = Val_AuthToken.Split('&');
        //            Val_AuthToken = Arr[0].Replace("authToken=", "");
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }
        //    if (FRNURLresponce.Contains("authType"))
        //    {
        //        try
        //        {
        //            //Val_AuthType = FRNURLresponce.Substring(FRNURLresponce.IndexOf("authType"), 50);
        //            //string[] Arr = Val_AuthType.Split('&');
        //            //Val_AuthType = Arr[0].Replace("authType=", "");
        //            //if (Val_AuthType.Contains("</noscript>"))
        //            //{
        //            //    Val_AuthType = Val_AuthType.Replace("</noscript>", string.Empty).Replace("\n", string.Empty).Replace(">", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
        //            //}
        //            try
        //            {
        //                Val_AuthType = "OUT_OF_NETWORK";
        //                // Val_AuthType = FRNURLresponce.Substring(FRNURLresponce.IndexOf("authType="), (FRNURLresponce.IndexOf(">", FRNURLresponce.IndexOf("authType=")) - FRNURLresponce.IndexOf("authType="))).Replace("authType=", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
        //            }
        //            catch { }
        //            //authType=
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }


        //    if (FRNURLresponce.Contains("goback"))
        //    {
        //        try
        //        {
        //            Val_goback = FRNURLresponce.Substring(FRNURLresponce.IndexOf("goback"), 300);
        //            string[] Arr = Val_goback.Split('"');
        //            Val_goback = Arr[0].Replace("goback=", "");
        //        }
        //        catch (Exception ex)
        //        {


        //        }
        //    }
        //    if (FRNURLresponce.Contains("trk"))
        //    {
        //        try
        //        {
        //            val_trk = FRNURLresponce.Substring(FRNURLresponce.IndexOf("trk"), 100);
        //            string[] Arr = val_trk.Split(';');
        //            val_trk = Arr[0].Replace("trk=", "");
        //        }
        //        catch (Exception ex)
        //        {


        //        }
        //    }

        //    if (FRNURLresponce.Contains("lastName"))
        //    {
        //        try
        //        {
        //            Val_lastName = FRNURLresponce.Substring(FRNURLresponce.IndexOf("lastName"), 30);
        //            string[] Arr = Val_lastName.Split('&');
        //            Val_lastName = Arr[0].Replace("lastName=", "");
        //        }
        //        catch (Exception ex)
        //        {


        //        }
        //    }

        //    if (FRNURLresponce.Contains("firstName") || FRNURLresponce.Contains("ShortTitle"))
        //    {
        //        try
        //        {
        //            if (FRNURLresponce.Contains("firstName"))
        //            {
        //                Val_firstName = FRNURLresponce.Substring(FRNURLresponce.IndexOf("firstName"), 30);
        //                string[] Arr = Val_firstName.Split('&');
        //                Val_firstName = Arr[0].Replace("firstName=", "");
        //            }
        //            else if (FRNURLresponce.Contains("ShortTitle"))
        //            {
        //                Val_firstName = FRNURLresponce.Substring(FRNURLresponce.IndexOf("ShortTitle"), 30);
        //                string[] Arr = Val_firstName.Split('"');
        //                Val_firstName = Arr[2].Replace("ShortTitle=", "");
        //            }

        //        }
        //        catch (Exception ex)
        //        {


        //        }



        //    }

        //    //Frnd Requset Send Option URl
        //    string thiredResponce = "http://www.linkedin.com/people/invite?from=profile&key=" + Val_key + "&firstName=" + Val_firstName + "&lastName=" + Val_lastName + "&authToken=" + Val_AuthToken + "&authType=" + Val_AuthType + "&goback=" + Val_goback + "%2Enpv_141450_*1_*1_OPENLINK_d7Ir_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1&trk=prof-0-sb-connect-button";
        //    //string SecondUrl = "http://www.linkedin.com/profile/view?id=" + Val_key + "&goback=" + Val_goback + "&authType=" + Val_AuthType + "&authToken=" + Val_AuthToken + "&trk=" + val_trk;
        //    string pageResponce2 = HttpHelper.getHtmlfromUrl(new Uri(thiredResponce));
        //    string ss = HttpHelper.getHtmlfromUrl(new Uri("http://www.linkedin.com/people/invite?from=profile&key=212815606&firstName=Dr%2E+Sanjeev&lastName=Puri&authToken=KIFZ&authType=OUT_OF_NETWORK&goback=%2Efps_PBCK_sanjeev_*1_*1_*1_*1_*1_*1_*2_*1_I_in_*1_*1_false_1_R_*1_*51_*1_*51_true_*1_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2%2Enpv_141450_*1_*1_OPENLINK_d7Ir_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1&trk=prof-0-sb-connect-button"));
        //    if (pageResponce2.Contains("sourceAlias"))
        //    {
        //        try
        //        {
        //            Val_sourceAlias = pageResponce2.Substring(pageResponce2.IndexOf("sourceAlias"), 100);
        //            string[] Arr = Val_sourceAlias.Split('"');
        //            Val_sourceAlias = Arr[2];
        //        }
        //        catch (Exception ex)
        //        {


        //        }
        //    }

        //    string lastPostUrl = "http://www.linkedin.com/people/iweReconnectAction";

        //    //
        //    //*********post request for last process of send request 

        //    //existingPositionIC=&companyName.0=&titleIC.0=&startYearIC.0=&endYearIC.0=&schoolText=&schoolID=&existingPositionIB=&companyName.1=&titleIB.0=&startYearIB.0=&endYearIB.0=&reason=IF&otherEmail=&greeting=I%27d+like+to+add+you+to+my+professional+network+on+LinkedIn.%0D%0A%0D%0A-+gaurav+agrawal&iweReconnectSubmit=Send+Invitation&key=141450&firstName=Ron&lastName=Bates&authToken=d7Ir&authType=OPENLINK&trk=prof-0-sb-connect-button&iweLimitReached=false&companyID.0=&companyID.1=&schoolID=&schoolcountryCode=&schoolprovinceCode=&javascriptEnabled=false&subject=gaurav+agrawal+wants+to+connect+on+LinkedIn+&defaultText=a908b3ba18701677a5879c1955035d05&csrfToken=ajax%3A8096156173388039261&                                                            sourceAlias=0_0ZD1VB-f52hFxNsCjPZdLV&goback=.fps_PBCK_*1_*1_*1_*1_*1_*1_*1_*2_*1_Y_*1_*1_*1_false_1_C_*1_*51_*1_*51_true_CC%2CN%2CG%2CI%2CPC%2CED%2CL%2CFG%2CTE%2CFA%2CSE%2CP%2CCS%2CF%2CDR_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2.npv_141450_*1_*1_OPENLINK_d7Ir_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1;
        //    string NameOfSender = _UserName.Remove(_UserName.IndexOf("@"));
        //    // tab-name username">
        //    string SenderName = string.Empty;
        //    try
        //    {
        //        SenderName = FRNURLresponce.Substring(FRNURLresponce.IndexOf("tab-name username\">"), (FRNURLresponce.IndexOf("<", FRNURLresponce.IndexOf("tab-name username\">")) - FRNURLresponce.IndexOf("tab-name username\">"))).Replace("tab-name username\">", string.Empty).Trim().Replace("tab-name username\">", string.Empty);
        //        NameOfSender = Uri.EscapeUriString(SenderName);
        //    }
        //    catch { }
        //    //if(string.IsNullOrEmpty(SenderName))
        //    //{
        //    //SenderName=NameOfSender;
        //    //}

        //    // catch { }
        //    //if (Val_AuthType.Contains("</noscript>"))
        //    //{
        //    //    Val_AuthType = Val_AuthType.Replace("</noscript>", string.Empty).Replace("\n",string.Empty).Replace(">",string.Empty).Replace("\\",string.Empty).Replace("\"",string.Empty).Trim();
        //    //}
        //    string LastPostData = "existingPositionIC=&companyName.0=&titleIC.0=&startYearIC.0=&endYearIC.0=&schoolText=&schoolID=&existingPositionIB=&companyName.1=&titleIB.0=&startYearIB.0=&endYearIB.0=&reason=IF&otherEmail=&greeting=I%27d+like+to+add+you+to+my+professional+network+on+LinkedIn.%0D%0A%0D%0A-" + NameOfSender + "&iweReconnectSubmit=Send+Invitation&key=" + Val_key + "&firstName=" + Val_firstName + "&lastName=" + Val_lastName + "&authToken=" + Val_AuthToken + "&authType=" + Val_AuthType + "&trk=prof-0-sb-connect-button&iweLimitReached=false&companyID.0=&companyID.1=&schoolID=&schoolcountryCode=&schoolprovinceCode=&javascriptEnabled=false&subject=" + NameOfSender + "+wants+to+connect+on+LinkedIn+&defaultText=a908b3ba18701677a5879c1955035d05&csrfToken=" + Val_CsrToken + "&sourceAlias=" + Val_sourceAlias + "&goback=" + Val_goback.Replace("%2E", ".") + ".npv_" + Val_key + "_*1_*1_OPENLINK_d7Ir_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1";
        //    string postResponse1 = HttpHelper.postFormDataProxy(new Uri(lastPostUrl), LastPostData, _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);

        //    if (postResponse1.Contains("You must confirm your primary email address before sending an invitation."))
        //    {
        //        LoggerManageConnection(_UserName + "You must confirm your primary email address before sending an invitation.");
        //        valid = false;
        //        PageNumber = 0;
        //        Isaccountvalid = false;
        //        return; 
        //    }

        ////  else  if (postResponse1.Contains("Accept Invitations"))
        //    else if (postResponse1.Contains("<strong>Invitation to"))
        //    {
        //        LoggerManageConnection("Invitation to " + Val_firstName + " " + Val_lastName + " sent.");
        //        string CSVHeader = "UserName" + "," + "SearchKeyword" + "," + "Invitation Sent To" + "," + "InvitationUrl";
        //        try
        //        {
        //            string[] url = Regex.Split(FrnAcceptUrL, "&authType");
        //            string CSV_Content = _UserName + "," + _ConnectSearchKeyword + "," + Val_firstName + " " + Val_lastName + "," + url[0];
        //            CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_AddConnectionSuccess);
        //            valid = true;
        //        }
        //        catch { }
        //    }

        //    else
        //    {
        //        LoggerManageConnection("Failed To Send Invitation To" + Val_firstName + " " + Val_lastName);
        //        valid = false;
        //        Isaccountvalid = false;
        //    }

        //    int Delay = RandomNumberGenerator.GenerateRandom(SearchMinDelay, SeacrhMaxDelay);
        //    Thread.Sleep(Delay * 1000);
        //    LoggerManageConnection("Invitation Delayed For " + Delay + " Seconds");

        //    SendInvitationCount++;
        //    if (SendInvitationCount >= SearchCriteria.NumberOfRequest)  //Here we check  that number of request send if counter is equal to the number of request put by user then it return to mathod and made pagenumber = 0; 
        //    {
        //        //SendInvitationCount--;

        //        PageNumber = 0;

        //        return;
        //    }


        //}

        public void TestConnectPageSearchForUniqueUrl(string pageNumber, int SearchMinDelay, int SearchMaxDelay)
        {
            int proxyport = 888;
            string SummaryLink = string.Empty;

            Regex PortCheck = new Regex("^[0-9]*$");

            if (PortCheck.IsMatch(_ProxyPort) && !string.IsNullOrEmpty(_ProxyPort))
            {
                proxyport = int.Parse(_ProxyPort);
            }

            //recive the page number and then hit page wise and find the url in which we need to send the request.....

            string UrlConnectpage = "http://www.linkedin.com/search/fpsearch?keywords=" + _ConnectSearchKeyword + "&keepFacets=keepFacets&page_num=" + pageNumber + "&pplSearchOrigin=ADVS&viewCriteria=2&sortCriteria=R&redir=redir";
            string PgSrcMain1 = HttpHelper.getHtmlfromUrlProxy(new Uri(UrlConnectpage), _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);

            string PostData = string.Empty;
            string page_num = string.Empty;
            string postUrl = string.Empty;
            string facetsOrder = string.Empty;

            if (PgSrcMain1.Contains("facetsOrder"))
            {
                facetsOrder = PgSrcMain1.Substring(PgSrcMain1.IndexOf("facetsOrder"), 200);
                string[] Arr = facetsOrder.Split('"');
                facetsOrder = Arr[2];
                string DecodedCharTest = Uri.UnescapeDataString(facetsOrder);
                string DecodedEmail = Uri.EscapeDataString(facetsOrder);
                facetsOrder = DecodedEmail;
            }

            page_num = pageNumber;

            PostData = "search=&viewCriteria=2&sortCriteria=C&facetsOrder=" + facetsOrder + "&page_num=" + page_num + "&openFacets=N%2CCC%2CG";

            //Post Url for Invite Message 
            postUrl = "http://www.linkedin.com/search/hits";

            //Diclaration For Post Add Connection For Keyword
                        
            //Post Response
            string postResponse = HttpHelper.postFormDataProxy(new Uri(postUrl), PostData, _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);
          //  List<string> PageSerchUrl = ChilkatBasedRegex.GettingAllUrls1(PgSrcMain1, "/profile/view?id"); 
           // List<string> PageSerchUrl = ChilkatBasedRegex.GettingAllUrls1_writtenBysharan(PgSrcMain1, "/profile/view?id");
            List<string> PageSerchUrl = ChilkatBasedRegex.GettingAllUrls1_writtenBysharan_fullUrl(PgSrcMain1);
            

            ///profile/view?id=22828991&authType=OUT_OF_NETWORK&authToken=pYz-&locale=en_US&srchid=2361660011371725230773&srchindex=1&srchtotal=3010&trk=vsrp_people_res_name&trkInfo=VSRPsearchId%3A2361660011371725230773%2CVSRPtargetId%3A22828991%2CVSRPcmpt%3Aprimary

            string FrnAcceptUrL = string.Empty;
            foreach (string item in PageSerchUrl)
            {
                string Val_firstName = string.Empty;
                string Val_lastName = string.Empty;
                string Val_key = string.Empty;
                string Val_goback = string.Empty;
                string Val_sourceAlias = string.Empty;
                string Val_defaultText = string.Empty;
                string Val_CsrToken = string.Empty;
                string Val_Subject = string.Empty;
                string Val_greeting = string.Empty;
                string Val_AuthToken = string.Empty;
                string Val_AuthType = string.Empty;
                string val_trk = string.Empty;



                bool check = CheckAccountDetailDB(_UserName, item);
                if (!check)
                {
                    LoggerManageConnection("this Profile :" + item + "  has already been visited today with username : " + _UserName);
                    continue;

                }
                if (Globals.no_of_profiles_can_be_visited ==SendInvitationCount && SendInvitationCount!=0)
                {
                    break;
                }
                try
                {
                    if (item.Contains("/profile/view?id"))  //this key word can only seprate the Url with all common urls....
                    {
                        lock (LockerConnection)
                        {
                            //if (!item.Contains("http://www.linkedin.com"))
                            //{
                            //    FrnAcceptUrL = "http://www.linkedin.com" + item;
                            //}
                           // else
                            {
                                FrnAcceptUrL = item;
                            }
                            string[] urll = Regex.Split(FrnAcceptUrL, "&authType");
                            DataSet ds = new DataSet();
                            try
                            {
                               string Querystring = "Select * From tb_ManageAddConnection Where Keyword='" + _ConnectSearchKeyword + "' and LinkedinUrl='" + urll[0] + "'";//"Insert into tb_ManageAddConnection (Keyword,LinkedinUrl,Username,DateTime) Values('" + _ConnectSearchKeyword + "','" + url[0] + "','" + _UserName + "','" + DateTime.Now.ToString("dd-MM-yyyy") + "')";
                                ds = DataBaseHandler.SelectQuery(Querystring, "tb_ManageAddConnection");
                            }
                            catch { }
                            if (ds.Tables[0].Rows.Count < 1)
                            {
                               string FRNURLresponce = HttpHelper.getHtmlfromUrl1(new Uri(FrnAcceptUrL));

                                //if (FRNURLresponce.Contains("/profile/view?id="))
                                //{
                                //    try
                                //    {
                                //        Val_key = FRNURLresponce.Substring(FRNURLresponce.IndexOf("/profile/view?id="), 30);
                                //        string[] Arr = Val_key.Split('&');
                                //        Val_key = Arr[0].Replace("/profile/view?id=", "");
                                //    }
                                //    catch { }

                                //}
                                //if (FrnAcceptUrL.Contains("/profile/view?id="))
                                //{
                                //    try
                                //    {
                                //        //Val_key = FrnAcceptUrL.Substring(FrnAcceptUrL.IndexOf("id="), (FrnAcceptUrL.IndexOf("&", FrnAcceptUrL.IndexOf("id=")) - FrnAcceptUrL.IndexOf("id="))).Replace("id=", string.Empty).Trim();

                                //        Val_key = Utils.getBetween(FrnAcceptUrL + "#####", "id=", "#####");

                                //    }
                                //    catch { }

                                //}

                               if (FRNURLresponce.Contains("memberId"))
                               {
                                   try
                                   {
                                       //Val_key = FrnAcceptUrL.Substring(FrnAcceptUrL.IndexOf("id="), (FrnAcceptUrL.IndexOf("&", FrnAcceptUrL.IndexOf("id=")) - FrnAcceptUrL.IndexOf("id="))).Replace("id=", string.Empty).Trim();

                                       Val_key = Utils.getBetween(FRNURLresponce , "memberId:\"", "\"}");

                                   }
                                   catch { }

                               }

                                if (FRNURLresponce.Contains("csrfToken"))
                                {
                                    try
                                    {
                                        Val_CsrToken = FRNURLresponce.Substring(FRNURLresponce.IndexOf("csrfToken"), 100);
                                        string[] Arr = Val_CsrToken.Split('>');
                                        Val_CsrToken = Arr[0].Replace("csrfToken=", "").Replace("\"", string.Empty).Trim();
                                    }
                                    catch { }

                                }

                                if (FRNURLresponce.Contains("authToken"))
                                {
                                    try
                                    {
                                        int startindex = FRNURLresponce.IndexOf("authToken");
                                        string start = FRNURLresponce.Substring(startindex);
                                        int endindex = start.IndexOf(",");
                                        string end = start.Substring(0, endindex);
                                        Val_AuthToken = end.Replace(",", string.Empty).Replace("authToken=", string.Empty).Replace("\"", string.Empty).Trim();
                                        if (Val_AuthToken.Contains("&"))
                                        {
                                            string[] Arr = Val_AuthToken.Split('&');
                                            Val_AuthToken = Arr[0].Replace("authToken=", string.Empty);
                                        }
                                        if (Val_AuthToken.Contains("</noscript>"))
                                        {
                                            try
                                            {
                                                string[] Val_Auth = Regex.Split(Val_AuthToken, ">");
                                                Val_AuthToken = Val_Auth[0].Replace("\"", string.Empty);
                                            }
                                            catch { }
                                        }
                                    }
                                    catch { }
                                }
                                if (FRNURLresponce.Contains("authType"))
                                {
                                    try
                                    {
                                        Val_AuthType = FRNURLresponce.Substring(FRNURLresponce.IndexOf("authType"), 50);
                                        string[] Arr = Val_AuthType.Split('&');
                                        Val_AuthType = Arr[0].Replace("authType=", "");
                                        if (Val_AuthType.Contains("</noscript>"))
                                        {
                                            Val_AuthType = Val_AuthType.Replace("</noscript>", string.Empty).Replace("\n", string.Empty).Replace(">", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
                                        }
                                        try
                                        {
                                            if (string.IsNullOrEmpty(Val_AuthType))
                                            {
                                                Val_AuthType = "OUT_OF_NETWORK";
                                            }
                                        }
                                        catch { }
                                    }
                                    catch { }
                                }


                                if (FRNURLresponce.Contains("goback"))
                                {
                                    try
                                    {
                                        Val_goback = FRNURLresponce.Substring(FRNURLresponce.IndexOf("goback"), 300);
                                        string[] Arr = Val_goback.Split('"');
                                        Val_goback = Arr[0].Replace("goback=", "");
                                    }
                                    catch { }
                                }
                                if (FRNURLresponce.Contains("trk"))
                                {
                                    try
                                    {
                                        val_trk = FRNURLresponce.Substring(FRNURLresponce.IndexOf("trk"), 100);
                                        string[] Arr = val_trk.Split(';');
                                        val_trk = Arr[0].Replace("trk=", "");
                                    }
                                    catch { }
                                }

                                if (FRNURLresponce.Contains("lastName"))
                                {
                                    try
                                    {
                                        Val_lastName = FRNURLresponce.Substring(FRNURLresponce.IndexOf("View Profile"), 300);
                                        try
                                        {
                                            string[] Arr = Val_lastName.Split(':');
                                            Val_lastName = Arr[2].Replace("firstName", "").Replace(",", "").Replace("\"", "").Replace("&#225;", string.Empty).Trim();
                                        }
                                        catch { }
                                    }
                                    catch { }
                                }

                                if (FRNURLresponce.Contains("firstName") || FRNURLresponce.Contains("ShortTitle"))
                                {
                                    try
                                    {
                                        if (FRNURLresponce.Contains("firstName"))
                                        {
                                            try
                                            {
                                                Val_firstName = FRNURLresponce.Substring(FRNURLresponce.IndexOf("View Profile"), 300);
                                                string[] Arrr = Val_firstName.Split(':');
                                                Val_firstName = Arrr[3].Replace("lastName=", "").Replace("i18n_HEADLINE", string.Empty).Replace("\"", "").Replace(",", string.Empty).Replace("}", string.Empty).Replace("link__endpoint", "").Trim();
                                            }
                                            catch { }
                                        }
                                        else if (FRNURLresponce.Contains("ShortTitle"))
                                        {
                                            Val_firstName = FRNURLresponce.Substring(FRNURLresponce.IndexOf("ShortTitle"), 30);
                                            string[] Arr = Val_firstName.Split('"');
                                            Val_firstName = Arr[2].Replace("ShortTitle=", "");
                                        }
                                    }
                                    catch { }

                                }

                                if (string.IsNullOrEmpty(Val_firstName))
                                {
                                    int startindex = FRNURLresponce.IndexOf("<title>");
                                    string start = FRNURLresponce.Substring(startindex).Replace("<title>", string.Empty);
                                    int endindex = start.IndexOf("</title>");
                                    string end = start.Substring(0, endindex).Replace("</title>", string.Empty);
                                    Val_firstName = end.Replace("| LinkedIn", "").Trim();
                                    string[] Arr = Regex.Split(Val_firstName, " ");
                                    try
                                    {
                                        Val_firstName = Arr[0];
                                        Val_lastName = Arr[1];
                                    }
                                    catch
                                    { }
                                    if (Arr.Count() == 2)
                                    {
                                        try
                                        {
                                            Val_lastName = Arr[1];
                                        }
                                        catch
                                        { }
                                    }
                                    if (Arr.Count() == 3)
                                    {
                                        try
                                        {
                                            Val_lastName = Arr[1] + " " + Arr[2];
                                        }
                                        catch
                                        { }
                                    }
                                    if (Arr.Count() == 4)
                                    {
                                        try
                                        {
                                            Val_lastName = Arr[1] + " " + Arr[2] + " " + Arr[3];
                                        }
                                        catch
                                        { }
                                    }
                                    if (Arr.Count() == 5)
                                    {
                                        try
                                        {
                                            Val_lastName = Arr[1] + " " + Arr[2] + " " + Arr[3] + " " + Arr[4];
                                        }
                                        catch
                                        { }
                                    }
                                    if (Arr.Count() == 6)
                                    {
                                        try
                                        {
                                            Val_lastName = Arr[1] + " " + Arr[2] + " " + Arr[3] + " " + Arr[4] + " " + Arr[5];
                                        }
                                        catch
                                        { }
                                    }
                                    if (Arr.Count() == 7)
                                    {
                                        try
                                        {
                                            Val_lastName = Arr[1] + " " + Arr[2] + " " + Arr[3] + " " + Arr[4] + " " + Arr[5] + " " + Arr[6];
                                        }
                                        catch
                                        { }
                                    }
                                    }

                            
                                try
                                {
                                    string[] Valuesss = Regex.Split(FRNURLresponce, "markAsReadOnClick");
                                    try
                                    {
                                        Val_goback = Valuesss[1].Substring(Valuesss[1].IndexOf("goback="), (Valuesss[1].IndexOf(",", Valuesss[1].IndexOf("goback=")) - Valuesss[1].IndexOf("goback="))).Replace("goback=", string.Empty).Trim();

                                    }
                                    catch { }
                                }
                                catch { }


                                //Check BlackListed Accounts
                                DataSet ds_bList = new DataSet();
                                try
                                {
                                    string Querystring = "Select ProfileID From tb_BlackListAccount Where ProfileID ='" + Val_key + "'";
                                    ds_bList = DataBaseHandler.SelectQuery(Querystring, "tb_BlackListAccount");
                                }
                                catch { }

                                string pageResponce2 = string.Empty;

                                if (ds_bList.Tables.Count > 0)
                                {
                                    if (ds_bList.Tables[0].Rows.Count > 0)
                                    {
                                        LoggerManageConnection("[ " + DateTime.Now + " ] => [ User: " + _UserName.Replace(":", string.Empty).Trim() + " is Added BlackListed List For Send Invitation Pls Check ]");
                                        return;
                                    }
                                }
                                else
                                {
                                    string thiredResponce = "http://www.linkedin.com/people/invite?from=profile&key=" + Val_key + "&firstName=" + Val_firstName + "&lastName=" + Val_lastName + "&authToken=" + Val_AuthToken + "&authType=" + Val_AuthType + "&csrfToken=" + Val_CsrToken + "&goback=" + Val_goback;
                                    pageResponce2 = HttpHelper.getHtmlfromUrl1(new Uri(thiredResponce));
                                }
                               
                                if (pageResponce2.Contains("You and this LinkedIn user don’t know anyone in common"))
                                {
                                    string CSVHeader = "UserName" + "," + "SearchKeyword" + "," + "Invitation Sent To" + "," + "InvitationUrl";
                                    string[] url = Regex.Split(FrnAcceptUrL, "&authType");
                                    string CSV_Content = _UserName + "," + _ConnectSearchKeyword + ", ," + url[0];
                                    CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_ConnectionThroughUnkonwPeopleLink);
                                    LoggerManageConnection("[ " + DateTime.Now + " ] => [ Failed To Send Invitation From " + _UserName + " ]");
                                    valid = false;
                                    Isaccountvalid = false;
                                    continue;
                                }
                                if (pageResponce2.Contains("sourceAlias"))
                                {
                                    try
                                    {
                                        Val_sourceAlias = pageResponce2.Substring(pageResponce2.IndexOf("sourceAlias"), 100);
                                        string[] Arr = Val_sourceAlias.Split('"');
                                        Val_sourceAlias = Arr[2];
                                    }
                                    catch { }
                                }
                                string[] strNameValue = Regex.Split(pageResponce2, "name=");

                                #region DecareVariable By Sanjeev
                                string iweReconnectSubmit = string.Empty;
                                string iweLimitReached = string.Empty;
                                string companyID0 = string.Empty;
                                string companyName0 = string.Empty;
                                //companyName.0
                                string companyID1 = string.Empty;
                                string schoolID = string.Empty;
                                string schoolcountryCode = string.Empty;
                                string schoolprovinceCode = string.Empty;
                                string subject = string.Empty;
                                string defaultText = string.Empty;
                                string csrfToken = string.Empty;
                                string sourceAlias = string.Empty;
                                string goback = string.Empty;
                                string titleIC0 = string.Empty;
                                string greeting = string.Empty;
                                string startYearIC0 = string.Empty;
                                string endYearIC0 = string.Empty;
                                string schoolText = string.Empty;
                                string titleIB0 = string.Empty;
                                string startYearIB0 = string.Empty;
                                string trk = string.Empty;
                                string authType = string.Empty;
                                string otheremail = string.Empty;
                                string firstName = string.Empty;
                                string lastName = string.Empty;
                                #endregion



                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"startYearIC.0");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    startYearIC0 = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"authType");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    authType = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"authType");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    authType = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    //string[] ArrForValue = Regex.Split(pageResponce2, "name=\"trk");
                                    //string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    //trk = (strValue);

                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"trk");
                                    int tempstartIndex = ArrForValue[1].IndexOf("id=\"");
                                    string strValue = ArrForValue[1].Substring(tempstartIndex).Replace("id=\"", "");
                                    strValue = strValue.Substring(0, strValue.IndexOf("\""));
                                    trk = (strValue).Trim();

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"endYearIC.0");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    endYearIC0 = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"schoolText");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    schoolText = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"startYearIB.0");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    startYearIB0 = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"titleIB.0");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    titleIB0 = (strValue);

                                }
                                catch { }
                                #endregion
                                //schoolText titleIB.0=
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"companyID.0");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    companyID0 = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"companyID.1");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    companyID1 = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"schoolID");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    schoolID = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"schoolcountryCode");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    schoolcountryCode = (strValue);

                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"schoolprovinceCode");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf(" ", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    schoolprovinceCode = (strValue);
                                    if (!string.IsNullOrEmpty(schoolprovinceCode))
                                    {

                                        schoolprovinceCode = "";
                                    }
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"subject");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    subject = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"defaultText");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    defaultText = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"csrfToken");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    csrfToken = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"sourceAlias");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    sourceAlias = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    //string[] ArrForValue = Regex.Split(pageResponce2, "name=\"goback");
                                    //string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    //goback = (strValue);

                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"goback");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    goback = (strValue).Replace("&quot;", "%22").Replace(":", "%3A").Replace(",", "%2C").Replace("/", "%2F").Replace("{", "%7B").Replace(" ", "+");
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"titleIC.0");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    titleIC0 = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"otherEmail");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    otheremail = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"iweLimitReached");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    iweLimitReached = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    try
                                    {
                                        string[] ArrForValue = Regex.Split(pageResponce2, "name=\"iweReconnectSubmit");
                                        string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("class=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                        iweReconnectSubmit = (strValue);
                                    }
                                    catch { }
                                    try
                                    {
                                        if (iweReconnectSubmit.Contains(">") || iweReconnectSubmit.Contains(">"))
                                        {
                                            iweReconnectSubmit = "Send Invitation";
                                        }
                                    }
                                    catch { }
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    try
                                    {
                                        string[] ArrForValue = Regex.Split(pageResponce2, "name=\"greeting");
                                        string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("class=\"message\">"), ArrForValue[1].IndexOf("<", ArrForValue[1].IndexOf("class=\"message\">")) - ArrForValue[1].IndexOf("class=\"message\">")).Replace("class=\"message\">", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                        greeting = (strValue);
                                        greeting = greeting.Replace("&#39;", "'");
                                        greeting = greeting.Replace(" ", "+");
                                        greeting = Uri.EscapeUriString(greeting);
                                        greeting = greeting.Remove(greeting.IndexOf("@"));
                                    }
                                    catch { }
                                    try
                                    {
                                        if (iweReconnectSubmit.Contains(">") || iweReconnectSubmit.Contains(">"))
                                        {
                                            //iweReconnectSubmit = "Send Invitation";
                                        }
                                    }
                                    catch { }
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"companyName.0");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    companyName0 = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"firstName");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    firstName = (strValue);
                                }
                                catch { }
                                #endregion
                                #region
                                try
                                {
                                    string[] ArrForValue = Regex.Split(pageResponce2, "name=\"lastName");
                                    string strValue = (ArrForValue[1].Substring(ArrForValue[1].IndexOf("value="), ArrForValue[1].IndexOf("id=", ArrForValue[1].IndexOf("value=")) - ArrForValue[1].IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\"", string.Empty).Trim());
                                    lastName = (strValue);

                                    if (lastName.Contains("&quot"))
                                    {
                                        lastName = string.Empty;

                                    }
                                }
                                catch { }
                                #endregion


                                string lastPostUrl = "http://www.linkedin.com/people/iweReconnectAction";

                                //
                                //*********post request for last process of send request 

                                //existingPositionIC=&companyName.0=&titleIC.0=&startYearIC.0=&endYearIC.0=&schoolText=&schoolID=&existingPositionIB=&companyName.1=&titleIB.0=&startYearIB.0=&endYearIB.0=&reason=IF&otherEmail=&greeting=I%27d+like+to+add+you+to+my+professional+network+on+LinkedIn.%0D%0A%0D%0A-+gaurav+agrawal&iweReconnectSubmit=Send+Invitation&key=141450&firstName=Ron&lastName=Bates&authToken=d7Ir&authType=OPENLINK&trk=prof-0-sb-connect-button&iweLimitReached=false&companyID.0=&companyID.1=&schoolID=&schoolcountryCode=&schoolprovinceCode=&javascriptEnabled=false&subject=gaurav+agrawal+wants+to+connect+on+LinkedIn+&defaultText=a908b3ba18701677a5879c1955035d05&csrfToken=ajax%3A8096156173388039261&                                                            sourceAlias=0_0ZD1VB-f52hFxNsCjPZdLV&goback=.fps_PBCK_*1_*1_*1_*1_*1_*1_*1_*2_*1_Y_*1_*1_*1_false_1_C_*1_*51_*1_*51_true_CC%2CN%2CG%2CI%2CPC%2CED%2CL%2CFG%2CTE%2CFA%2CSE%2CP%2CCS%2CF%2CDR_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2.npv_141450_*1_*1_OPENLINK_d7Ir_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1;
                                string NameOfSender = _UserName.Remove(_UserName.IndexOf("@"));
                                // tab-name username">
                                string SenderName = string.Empty;
                                try
                                {
                                    SenderName = FRNURLresponce.Substring(FRNURLresponce.IndexOf("tab-name username\">"), (FRNURLresponce.IndexOf("<", FRNURLresponce.IndexOf("tab-name username\">")) - FRNURLresponce.IndexOf("tab-name username\">"))).Replace("tab-name username\">", string.Empty).Trim().Replace("tab-name username\">", string.Empty);
                                    NameOfSender = Uri.EscapeUriString(SenderName);
                                }
                                catch { }
                                //if(string.IsNullOrEmpty(SenderName))
                                //{
                                //SenderName=NameOfSender;
                                //}

                                // catch { }
                                //if (Val_AuthType.Contains("</noscript>"))
                                //{
                                //    Val_AuthType = Val_AuthType.Replace("</noscript>", string.Empty).Replace("\n",string.Empty).Replace(">",string.Empty).Replace("\\",string.Empty).Replace("\"",string.Empty).Trim();
                                //}
                             //   string LastPostData = "companyName.0=" + companyName0 + "titleIC.0=" + titleIC0 + "&startYearIC.0=" + startYearIC0 + "&endYearIC.0=" + endYearIC0 + "&schoolText=" + schoolText + "&schoolID=" + schoolID + "&companyName.1=" + companyName0 + "&titleIB.0=" + titleIB0 + "&startYearIB.0=" + startYearIB0 + "&endYearIB.0=" + endYearIC0 + "&reason=IF&otherEmail=" + otheremail + "&greeting=" + greeting + "&iweReconnectSubmit=" + iweReconnectSubmit.Replace(" ", "+") + "&key=" + Val_key + "&firstName=" + firstName + "&lastName=" + lastName + "&authToken=" + Val_AuthToken + "&authType=" + Val_AuthType + "&trk=" + trk + "&iweLimitReached=false&companyID.0=" + companyID0 + "&companyID.1=" + companyID1 + "&schoolID=" + schoolID + "&schoolcountryCode=" + schoolcountryCode + "&schoolprovinceCode=" + schoolprovinceCode + "&javascriptEnabled=false&subject=" + subject.Replace(" ", "+") + "&defaultText=" + defaultText + "&csrfToken=" + Val_CsrToken + "&sourceAlias=" + sourceAlias + "&goback=" + goback;
                                string LastPostData = "companyName.0=" + companyName0 + "titleIC.0=" + titleIC0 + "&startYearIC.0=" + startYearIC0 + "&endYearIC.0=" + endYearIC0 + "&schoolText=" + schoolText + "&schoolID=" + schoolID + "&companyName.1=" + companyName0 + "&titleIB.0=" + titleIB0 + "&startYearIB.0=" + startYearIB0 + "&endYearIB.0=" + endYearIC0 + "&reason=IF&otherEmail=" + otheremail + "&greeting=" + greeting + "&iweReconnectSubmit=" + iweReconnectSubmit.Replace(" ", "+") + "&key=" + Val_key + "&firstName=" + firstName + "&lastName=" + lastName + "&authToken=" + Val_AuthToken + "&authType=" + Val_AuthType + "&trk=" + trk + "&iweLimitReached=false&companyID.0=" + companyID0 + "&companyID.1=" + companyID1 + "&schoolID=" + schoolID + "&schoolcountryCode=" + schoolcountryCode + "&schoolprovinceCode=" + schoolprovinceCode + "&javascriptEnabled=false&subject=" + subject.Replace(" ", "+") + "&defaultText=" + defaultText + "&csrfToken=" + Val_CsrToken + "&sourceAlias=" + sourceAlias + "&goback=" + Val_goback;
                                //string LastPostData = "companyName.0=" + companyName0 + "titleIC.0=" + titleIC0 + "&startYearIC.0=" + startYearIC0 + "&endYearIC.0=" + endYearIC0 + "&schoolText=" + schoolText + "&schoolID=" + schoolID + "&companyName.1=" + companyName0 + "&titleIB.0=" + titleIB0 + "&startYearIB.0=" + startYearIB0 + "&endYearIB.0=" + endYearIC0 + "&reason=IF&otherEmail=" + otheremail + "&greeting=" + greeting + "&iweReconnectSubmit=" + iweReconnectSubmit.Replace(" ", "+") + "&key=" + Val_key + "&firstName=" + firstName + "&lastName=" + lastName + "&authToken=" + Val_AuthToken + "&authType=" + Val_AuthType + "&trk=" + trk + "&iweLimitReached=false&companyID.0=" + companyID0 + "&companyID.1=" + companyID1 + "&schoolID=" + schoolID + "&schoolcountryCode=" + schoolcountryCode + "&schoolprovinceCode=" + schoolprovinceCode + "&javascriptEnabled=false&subject=" + subject.Replace(" ", "+") + "&defaultText=" + defaultText + "&csrfToken=" + Val_CsrToken + "&sourceAlias=" + sourceAlias + "&goback=" + goback;
                                //string LastPostData = "existingPositionIC=&companyName.0=&titleIC.0=&startYearIC.0=&endYearIC.0=&schoolText=&schoolID=&existingPositionIB=&companyName.1=&titleIB.0=&startYearIB.0=&endYearIB.0=&reason=IF&otherEmail=&greeting=I%27d+like+to+add+you+to+my+professional+network+on+LinkedIn.%0D%0A%0D%0A-" + NameOfSender + "&iweReconnectSubmit=Send+Invitation&key=" + Val_key + "&firstName=" + Val_firstName + "&lastName=" + Val_lastName + "&authToken=" + Val_AuthToken + "&authType=" + Val_AuthType + "&trk=prof-0-sb-connect-button&iweLimitReached=false&companyID.0=&companyID.1=&schoolID=&schoolcountryCode=&schoolprovinceCode=&javascriptEnabled=false&subject=" + NameOfSender + "+wants+to+connect+on+LinkedIn+&defaultText=a908b3ba18701677a5879c1955035d05&csrfToken=" + Val_CsrToken + "&sourceAlias=" + Val_sourceAlias + "&goback=" + Val_goback.Replace("%2E", ".") + ".npv_" + Val_key + "_*1_*1_OPENLINK_d7Ir_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1_*1";
                                string postResponse1 = HttpHelper.postFormDataProxy(new Uri(lastPostUrl), LastPostData, _ProxyAddress, proxyport, _ProxyUserName, _ProxyPassword);

                                // Here we getting captcha sometimes

                                #region captcha solving code

                                string ImageUrl = string.Empty;
                                string captchaText = string.Empty;
                                string captchachallengeid = string.Empty;
                                string dts = string.Empty;
                                string origActionAlias = string.Empty;
                                string origSourceAlias = string.Empty;
                                string irhf = string.Empty;
                                string submissionID = string.Empty;
                                string CAPTCHAfwdcsrftoken = string.Empty;
                                string CAPTCHAfwdsignin = string.Empty;
                                string CAPTCHAfwdsession_password = string.Empty;
                                string CAPTCHAfwdsession_key = string.Empty;
                                string CAPTCHAfwdisJsEnabled = string.Empty;
                                string CAPTCHAfwdloginCsrfParam = string.Empty;

                                string security_challenge_id=string.Empty;
                                string cyrpto_CAPTCHA_fwd_reason=string.Empty;
                                string cyrpto_CAPTCHA_fwd_lastName=string.Empty;
                                string cyrpto_CAPTCHA_fwd_goback=string.Empty;
                                string cyrpto_CAPTCHA_fwd_subject=string.Empty;
                                string cyrpto_CAPTCHA_fwd_greeting = string.Empty;
                                string cyrpto_CAPTCHA_fwd_authToken=string.Empty;
                                string cyrpto_CAPTCHA_fwd_connectionParam = string.Empty;
                                string cyrpto_CAPTCHA_fwd_iweReconnectSubmit = string.Empty;
                                string cyrpto_CAPTCHA_fwd_javascriptEnabled = string.Empty;
                                string cyrpto_CAPTCHA_fwd_authType = string.Empty;
                                string cyrpto_CAPTCHA_fwd_key = string.Empty;
                                string cyrpto_CAPTCHA_fwd_trk = string.Empty;
                                string cyrpto_CAPTCHA_fwd_firstName = string.Empty;
                                string cyrpto_CAPTCHA_fwd_csrfToken = string.Empty;
                                string cyrpto_CAPTCHA_fwd_defaultText = string.Empty;
                                string cyrpto_CAPTCHA_fwd_iweLimitReached = string.Empty;
                               
                               // string origSourceAlias = string.Empty;
                                //string origActionAlias = string.Empty;
                                string csrfToken_forCaptch = string.Empty;
                                string sourceAlias_forCaptcha = string.Empty;
                                string goback_forCaptcha  = string.Empty;
                                
                                if (postResponse1.Contains("Security Verification"))
                                {
                                    string dataforcapctha = HttpHelper.getHtmlfromUrl1(new Uri("https://www.google.com/recaptcha/api/noscript?k=6LcnacMSAAAAADoIuYvLUHSNLXdgUcq-jjqjBo5n"));
                                    if (!string.IsNullOrEmpty(dataforcapctha))
                                    {
                                        int startindex = dataforcapctha.IndexOf("id=\"recaptcha_challenge_field\"");
                                        if (startindex > 0)
                                        {
                                            string start = dataforcapctha.Substring(startindex).Replace("id=\"recaptcha_challenge_field\"", "");
                                            int endindex = start.IndexOf("\">");
                                            string end = start.Substring(0, endindex).Replace("value=", string.Empty).Replace("\"", string.Empty).Trim();
                                            ImageUrl = "https://www.google.com/recaptcha/api/image?c=" + end;
                                            System.Net.WebClient webclient = new System.Net.WebClient();
                                            byte[] args = webclient.DownloadData(ImageUrl);
                                            string[] arr1 = new string[] { Globals.CapchaLoginID, Globals.CapchaLoginPassword, "" };
                                            captchaText = DecodeDBC(arr1, args);
                                        }

                                        if (postResponse1.Contains("name=\"security-challenge-id\""))
                                        {
                                            int startindexnew = postResponse1.IndexOf("name=\"security-challenge-id\"");
                                            if (startindexnew > 0)
                                            {
                                                string start = postResponse1.Substring(startindexnew).Replace("name=\"security-challenge-id\"", "").Replace("value=\"", "");
                                                int endindex = start.IndexOf("\"");
                                                string end = start.Substring(0, endindex);
                                                captchachallengeid = end.Replace("\"", string.Empty).Trim();
                                            }
                                        }

                                        if (postResponse1.Contains("name=\"dts\""))
                                        {
                                            int startindexnew = postResponse1.IndexOf("name=\"dts\"");
                                            if (startindexnew > 0)
                                            {
                                                string start = postResponse1.Substring(startindexnew).Replace("name=\"dts\"", "").Replace("value=\"", "");
                                                int endindex = start.IndexOf("\"");
                                                string end = start.Substring(0, endindex);
                                                dts = end.Replace("\"", string.Empty).Trim();
                                            }
                                        }

                                        if (postResponse1.Contains("name=\"origActionAlias\""))
                                        {
                                            int startindexnew = postResponse1.IndexOf("name=\"origActionAlias\"");
                                            if (startindexnew > 0)
                                            {
                                                string start = postResponse1.Substring(startindexnew).Replace("name=\"origActionAlias\"", "").Replace("value=\"", "");
                                                int endindex = start.IndexOf("\"");
                                                string end = start.Substring(0, endindex);
                                                origActionAlias = end.Replace("\"", string.Empty).Trim();
                                            }
                                        }

                                        if (postResponse1.Contains("name=\"submissionId\""))
                                        {
                                            int startindexnew = postResponse1.IndexOf("name=\"submissionId\"");
                                            if (startindexnew > 0)
                                            {
                                                string start = postResponse1.Substring(startindexnew).Replace("name=\"submissionId\"", string.Empty).Replace("value=\"", string.Empty);
                                                int endindex = start.IndexOf("\"");
                                                string end = start.Substring(0, endindex);
                                                submissionID = end.Replace("\"", string.Empty).Trim();
                                            }
                                        }
                                        if (postResponse1.Contains("name=\"CAPTCHA-fwd-csrfToken\""))
                                        {
                                            int startindexnew = postResponse1.IndexOf("name=\"CAPTCHA-fwd-csrfToken\"");
                                            if (startindexnew > 0)
                                            {
                                                string start = postResponse1.Substring(startindexnew).Replace("name=\"CAPTCHA-fwd-csrfToken\"", string.Empty).Replace("value=\"", string.Empty);
                                                int endindex = start.IndexOf("\"");
                                                string end = start.Substring(0, endindex);
                                                CAPTCHAfwdcsrftoken = end.Replace("\"", string.Empty).Trim();
                                            }
                                        }
                                        if (postResponse1.Contains("name=\"CAPTCHA-fwd-signin\""))
                                        {
                                            int startindexnew = postResponse1.IndexOf("name=\"CAPTCHA-fwd-signin\"");
                                            if (startindexnew > 0)
                                            {
                                                string start = postResponse1.Substring(startindexnew).Replace("name=\"CAPTCHA-fwd-signin\"", string.Empty).Replace("value=\"", string.Empty);
                                                int endindex = start.IndexOf("\"");
                                                string end = start.Substring(0, endindex);
                                                CAPTCHAfwdsignin = end.Replace("\"", string.Empty).Trim();
                                            }
                                        }
                                        if (postResponse1.Contains("name=\"CAPTCHA-fwd-session_password\""))
                                        {
                                            int startindexnew = postResponse1.IndexOf("name=\"CAPTCHA-fwd-session_password\"");
                                            if (startindexnew > 0)
                                            {
                                                string start = postResponse1.Substring(startindexnew).Replace("name=\"CAPTCHA-fwd-session_password\"", string.Empty).Replace("value=\"", string.Empty);
                                                int endindex = start.IndexOf("\"");
                                                string end = start.Substring(0, endindex);
                                                CAPTCHAfwdsession_password = end.Replace("\"", string.Empty).Trim();
                                            }
                                        }
                                        if (postResponse1.Contains("name=\"CAPTCHA-fwd-session_key\""))
                                        {
                                            int startindexnew = postResponse1.IndexOf("name=\"CAPTCHA-fwd-session_key\"");
                                            if (startindexnew > 0)
                                            {
                                                string start = postResponse1.Substring(startindexnew).Replace("name=\"CAPTCHA-fwd-session_key\"", string.Empty).Replace("value=\"", string.Empty);
                                                int endindex = start.IndexOf("\"");
                                                string end = start.Substring(0, endindex);
                                                CAPTCHAfwdsession_key = end.Replace("\"", string.Empty).Trim();
                                            }
                                        }
                                        if (postResponse1.Contains("name=\"CAPTCHA-fwd-isJsEnabled\""))
                                        {
                                            int startindexnew = postResponse1.IndexOf("name=\"CAPTCHA-fwd-isJsEnabled\"");
                                            if (startindexnew > 0)
                                            {
                                                string start = postResponse1.Substring(startindexnew).Replace("name=\"CAPTCHA-fwd-isJsEnabled\"", string.Empty).Replace("value=\"", string.Empty);
                                                int endindex = start.IndexOf("\"");
                                                string end = start.Substring(0, endindex);
                                                CAPTCHAfwdisJsEnabled = end.Replace("\"", string.Empty).Trim();
                                            }
                                        }
                                        if (postResponse1.Contains("name=\"CAPTCHA-fwd-loginCsrfParam\""))
                                        {
                                            int startindexnew = postResponse1.IndexOf("name=\"CAPTCHA-fwd-loginCsrfParam\"");
                                            if (startindexnew > 0) ;
                                            {
                                                string start = postResponse1.Substring(startindexnew).Replace("name=\"CAPTCHA-fwd-loginCsrfParam\"", string.Empty).Replace("value=\"", string.Empty);
                                                int endindex = start.IndexOf("\"");
                                                string end = start.Substring(0, endindex);
                                                CAPTCHAfwdloginCsrfParam = end.Replace("\"", string.Empty).Trim();
                                            }
                                        }


                                        if (postResponse1.Contains("name=\"origSourceAlias\""))
                                        {
                                            int startindexnew = postResponse1.IndexOf("name=\"origSourceAlias\"");
                                            if (startindexnew > 0)
                                            {
                                                string start = postResponse1.Substring(startindexnew).Replace("name=\"origSourceAlias\"", "").Replace("value=\"", "");
                                                int endindex = start.IndexOf("\"");
                                                string end = start.Substring(0, endindex);
                                                origSourceAlias = end.Replace("\"", string.Empty).Trim();
                                            }
                                        }

                                        if (postResponse1.Contains("name=\"irhf\""))
                                        {
                                            int startindexnew = postResponse1.IndexOf("name=\"irhf\"");
                                            if (startindexnew > 0)
                                            {
                                                string start = postResponse1.Substring(startindexnew).Replace("name=\"irhf\"", "").Replace("value=\"", "");
                                                int endindex = start.IndexOf("\"");
                                                string end = start.Substring(0, endindex);
                                                irhf = end.Replace("\"", string.Empty).Trim();
                                            }
                                        }

                                        #region captcha value scraper region written by sharan

                                        if (string.IsNullOrEmpty(irhf))
                                        {
                                            string[] arr = Regex.Split(postResponse1, "irhf");
                                            irhf = Utils.getBetween(arr[1], "value=\"", "\"");
                                        }

                                        if (string.IsNullOrEmpty(dts))
                                        {
                                            dts = Utils.getBetween(postResponse1, "dts", "id=");
                                            dts = Utils.getBetween(dts , "value=\"", "\"");
                                        }
                                        if (string.IsNullOrEmpty(security_challenge_id))
                                        {
                                            security_challenge_id = Utils.getBetween(postResponse1, "security-challenge-id", "id=");
                                            security_challenge_id = Utils.getBetween(security_challenge_id , "value=\"", "\"");
                                        }

                                        if (string.IsNullOrEmpty(cyrpto_CAPTCHA_fwd_reason))
                                        {
                                            cyrpto_CAPTCHA_fwd_reason = Utils.getBetween(postResponse1, "cyrpto-CAPTCHA-fwd-reason", "id=");
                                            cyrpto_CAPTCHA_fwd_reason = Utils.getBetween(cyrpto_CAPTCHA_fwd_reason  , "value=\"", "\"");
                                        }

                                        if(string.IsNullOrEmpty(cyrpto_CAPTCHA_fwd_lastName))
                                        {
                                            cyrpto_CAPTCHA_fwd_lastName=Utils.getBetween(postResponse1,"cyrpto-CAPTCHA-fwd-lastName\" value=\"","\"");
                                        }
                                        if(string.IsNullOrEmpty(cyrpto_CAPTCHA_fwd_goback))
                                        {
                                            cyrpto_CAPTCHA_fwd_goback=Utils.getBetween(postResponse1,"cyrpto-CAPTCHA-fwd-goback\" value=\"","\"");
                                        }

                                        if(string.IsNullOrEmpty(cyrpto_CAPTCHA_fwd_subject))
                                        {
                                            cyrpto_CAPTCHA_fwd_subject=Utils.getBetween(postResponse1,"cyrpto-CAPTCHA-fwd-subject\" value=\"","\"");
                                               
                                        }

                                        if (string.IsNullOrEmpty(cyrpto_CAPTCHA_fwd_greeting))
                                        {
                                            cyrpto_CAPTCHA_fwd_greeting = Utils.getBetween(postResponse1, "cyrpto-CAPTCHA-fwd-greeting\" value=\"", "\"");
                                        }

                                        if (string.IsNullOrEmpty(cyrpto_CAPTCHA_fwd_authToken))
                                        {
                                            cyrpto_CAPTCHA_fwd_authToken = Utils.getBetween(postResponse1, "cyrpto-CAPTCHA-fwd-authToken\" value=\"", "\"");
                                        }
                                        if(string.IsNullOrEmpty(cyrpto_CAPTCHA_fwd_connectionParam))
                                        {
                                            cyrpto_CAPTCHA_fwd_connectionParam=Utils.getBetween(postResponse1,"cyrpto-CAPTCHA-fwd-connectionParam\" value=\"","\"");

                                        }
                                        if (string.IsNullOrEmpty(cyrpto_CAPTCHA_fwd_iweReconnectSubmit))
                                        {
                                            cyrpto_CAPTCHA_fwd_iweReconnectSubmit = Utils.getBetween(postResponse1, "cyrpto-CAPTCHA-fwd-iweReconnectSubmit\" value=\"", "\"");
                                        }
                                        if (string.IsNullOrEmpty(cyrpto_CAPTCHA_fwd_javascriptEnabled))
                                        {
                                            cyrpto_CAPTCHA_fwd_javascriptEnabled = Utils.getBetween(postResponse1, "cyrpto-CAPTCHA-fwd-javascriptEnabled\" value=\"", "\"");
                                        }
                                        if (string.IsNullOrEmpty(cyrpto_CAPTCHA_fwd_authType))
                                        {
                                            cyrpto_CAPTCHA_fwd_authType = Utils.getBetween(postResponse1, "cyrpto-CAPTCHA-fwd-authType\" value=\"", "\"");
                                        }
                                        if (string.IsNullOrEmpty(cyrpto_CAPTCHA_fwd_key))
                                        {
                                            cyrpto_CAPTCHA_fwd_key = Utils.getBetween(postResponse1, "cyrpto-CAPTCHA-fwd-key\" value=\"", "\"");
                                        }
                                        if (string.IsNullOrEmpty(cyrpto_CAPTCHA_fwd_trk))
                                        {
                                            cyrpto_CAPTCHA_fwd_trk = Utils.getBetween(postResponse1, "cyrpto-CAPTCHA-fwd-trk\" value=\"", "\"");
                                        }
                                        if (string.IsNullOrEmpty(cyrpto_CAPTCHA_fwd_firstName))
                                        {
                                            cyrpto_CAPTCHA_fwd_firstName = Utils.getBetween(postResponse1, "cyrpto-CAPTCHA-fwd-firstName\" value=\"", "\"");

                                        }
                                        if (string.IsNullOrEmpty(cyrpto_CAPTCHA_fwd_csrfToken))
                                        {
                                            cyrpto_CAPTCHA_fwd_csrfToken = Utils.getBetween(postResponse1, "cyrpto-CAPTCHA-fwd-csrfToken\" value=\"", "\"");

                                        }
                                        if (string.IsNullOrEmpty(cyrpto_CAPTCHA_fwd_defaultText))
                                        {
                                            cyrpto_CAPTCHA_fwd_defaultText = Utils.getBetween(postResponse1, "cyrpto-CAPTCHA-fwd-defaultText\" value=\"", "\"");
                                        }
                                        if (string.IsNullOrEmpty(cyrpto_CAPTCHA_fwd_iweLimitReached))
                                        {
                                            cyrpto_CAPTCHA_fwd_iweLimitReached = Utils.getBetween(postResponse1, "cyrpto-CAPTCHA-fwd-iweLimitReached\" value=\"", "\"");

                                        }
                                        if (string.IsNullOrEmpty(origSourceAlias))
                                        {
                                            origSourceAlias = Utils.getBetween(postResponse1, "origSourceAlias\" value=\"", "\"");

                                        }
                                        if (string.IsNullOrEmpty(origActionAlias))
                                        {
                                            origActionAlias = Utils.getBetween(postResponse1, "origActionAlias\" value=\"", "\"");
                                        }
                                        if (string.IsNullOrEmpty(csrfToken_forCaptch))
                                        {
                                            csrfToken_forCaptch = Utils.getBetween(postResponse1, "csrfToken=", "&");
                                            //csrfToken_forCaptch = Uri.EscapeDataString(csrfToken_forCaptch);
                                            // here it should be in ajax:8743438493 format
                                        }
                                        if (string.IsNullOrEmpty(sourceAlias_forCaptcha))
                                        {
                                            sourceAlias_forCaptcha = Utils.getBetween(postResponse1, "sourceAlias\" value=\"", "\"");

                                        }
                                        if (string.IsNullOrEmpty(goback_forCaptcha))
                                        {
                                            goback_forCaptcha = Utils.getBetween(postResponse1, "name=\"goback\" value=\"", "\"");
                                        }

                                        #endregion 




                                        if (!string.IsNullOrEmpty(ImageUrl) && !string.IsNullOrEmpty(captchaText))
                                        {
                                            string accountUser = string.Empty;
                                            string accountPass = string.Empty;

                                        // string   postdata = "recaptcha_challenge_field=" + ImageUrl.Replace("https://www.google.com/recaptcha/api/image?c=", string.Empty) + "&recaptcha_response_field=" + captchaText.Replace(" ", "+") + "&irhf=" + irhf + "&dts=" + dts + "&security-challenge-id=" + captchachallengeid + "&submissionId=" + submissionID + "&CAPTCHA-fwd-csrfToken=" + CAPTCHAfwdcsrftoken + "&CAPTCHA-fwd-isJsEnabled=" + CAPTCHAfwdisJsEnabled + "&CAPTCHA-fwd-signin=" + CAPTCHAfwdsignin + "&CAPTCHA-fwd-loginCsrfParam=" + CAPTCHAfwdloginCsrfParam + "&CAPTCHA-fwd-session_password=" + CAPTCHAfwdsession_password + "&CAPTCHAfwd-session_key=" + CAPTCHAfwdsession_key + "&session_password=" + accountPass + "&session_key=" + Uri.EscapeDataString(accountUser) + "&origSourceAlias=" + origSourceAlias + "&origActionAlias=" + origActionAlias + "&csrfToken=" + csrfToken + "&sourceAlias=" + sourceAlias;

                                            string postData_new = "recaptcha_challenge_field=" + ImageUrl.Replace("https://www.google.com/recaptcha/api/image?c=", string.Empty) + "&recaptcha_response_field=" + captchaText.Replace(" ", "+") + "&irhf=" + irhf + "&dts=" + dts + "&submissionId=&security-challenge-id=" + security_challenge_id + "&cyrpto-CAPTCHA-fwd-reason=" + cyrpto_CAPTCHA_fwd_reason + "&cyrpto-CAPTCHA-fwd-lastName=" + cyrpto_CAPTCHA_fwd_lastName + "&cyrpto-CAPTCHA-fwd-goback=" + cyrpto_CAPTCHA_fwd_goback + "&cyrpto-CAPTCHA-fwd-subject=" + cyrpto_CAPTCHA_fwd_subject + "&cyrpto-CAPTCHA-fwd-greeting=" + cyrpto_CAPTCHA_fwd_greeting + "&cyrpto-CAPTCHA-fwd-authToken=" + cyrpto_CAPTCHA_fwd_authToken + "&cyrpto-CAPTCHA-fwd-connectionParam=" + cyrpto_CAPTCHA_fwd_connectionParam + "&cyrpto-CAPTCHA-fwd-iweReconnectSubmit=" + cyrpto_CAPTCHA_fwd_iweReconnectSubmit + "&cyrpto-CAPTCHA-fwd-javascriptEnabled=" + cyrpto_CAPTCHA_fwd_javascriptEnabled + "&cyrpto-CAPTCHA-fwd-authType=" + cyrpto_CAPTCHA_fwd_authType + "&cyrpto-CAPTCHA-fwd-key=" + cyrpto_CAPTCHA_fwd_key + "&cyrpto-CAPTCHA-fwd-trk=" + cyrpto_CAPTCHA_fwd_trk + "&cyrpto-CAPTCHA-fwd-firstName=" + cyrpto_CAPTCHA_fwd_firstName + "&cyrpto-CAPTCHA-fwd-csrfToken=" + cyrpto_CAPTCHA_fwd_csrfToken + "&cyrpto-CAPTCHA-fwd-defaultText=" + cyrpto_CAPTCHA_fwd_defaultText + "&cyrpto-CAPTCHA-fwd-iweLimitReached=" + cyrpto_CAPTCHA_fwd_iweLimitReached + "&origSourceAlias=" + origSourceAlias + "&origActionAlias=" + origActionAlias + "&csrfToken=" + csrfToken_forCaptch + "&sourceAlias=" + sourceAlias_forCaptcha + "&goback=" + goback_forCaptcha;
                                           
                                            postData_new = postData_new.Replace(" ", string.Empty);
                                            postResponse1 = HttpHelper.postFormDataRef(new Uri("https://www.linkedin.com/people/captcha-submit"), postData_new, "https://www.linkedin.com/uas/login-submit", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);


                                        }
                                    }
                                }

                                #endregion


                                string FinalValue = string.Empty;
                                try
                                {
                                    FinalValue = postResponse1.Substring(postResponse1.IndexOf("Esuccess"), (postResponse1.IndexOf(">", postResponse1.IndexOf("Esuccess")) - postResponse1.IndexOf("Esuccess"))).Replace("Esuccess", string.Empty).Trim();
                                    FinalValue = Uri.UnescapeDataString(FinalValue);
                                    string[] valuess = Regex.Split(FinalValue, "&");
                                    FinalValue = valuess[0].Replace("=", string.Empty);
                                    // FinalValue = postResponse1.Substring(postResponse1.IndexOf("Esuccess"), (postResponse.IndexOf(">", postResponse1.IndexOf("Esuccess")) - postResponse1.IndexOf("Esuccess"))).Replace("Esuccess", string.Empty);
                                    // Val_AuthType = FRNURLresponce.Substring(FRNURLresponce.IndexOf("authType="), (FRNURLresponce.IndexOf(">", FRNURLresponce.IndexOf("authType=")) - FRNURLresponce.IndexOf("authType="))).Replace("authType=", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
                                }
                                catch { }
                                try
                                {
                                    string lastGetResponse = HttpHelper.getHtmlfromUrl1(new Uri("http://www.linkedin.com/people/pymk?goback=" + Val_goback + "&trk=" + val_trk + "&report%2Esuccess=" + FinalValue));
                                }
                                catch { }
                                if (postResponse1.Contains("You must confirm your primary email address before sending an invitation."))
                                {
                                    LoggerManageConnection("[ " + DateTime.Now + " ] => [ " + _UserName + "You must confirm your primary email address before sending an invitation.]");
                                    valid = false;
                                    PageNumber = 0;
                                    Isaccountvalid = false;
                                    break;
                                }
                                if (!pageResponce2.Contains("emailAddress-invitee-invitation"))
                                {
                                    //else if (postResponse1.Contains("<strong>Invitation to"))
                                    if (postResponse1.Contains("<strong>Invitations") || postResponse1.Contains("Send Invitation") || postResponse1.Contains("Send invitation") || postResponse1.Contains("<strong>Invitation"))
                                    {

                                        if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
                                        {
                                            //firstName = "linkedin";
                                            //lastName = "Member";
                                            firstName = Val_firstName;
                                            lastName = Val_lastName;
                                        }
                                        if (!(postResponse1.Contains("Your invitation was not sent")))
                                        {
                                            LoggerManageConnection("[ " + DateTime.Now + " ] => [ Invitation to " + firstName + " " + lastName + " sent For Keyword : " + _ConnectSearchKeyword + " From Account : " + _UserName + " ]");
                                            string CSVHeader = "UserName" + "," + "SearchKeyword" + "," + "Invitation Sent To" + "," + "InvitationUrl";
                                            try
                                            {
                                                string[] url = Regex.Split(FrnAcceptUrL, "&authType");
                                                string CSV_Content = _UserName + "," + _ConnectSearchKeyword + "," + firstName + " " + lastName + "," + url[0];
                                                CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_AddConnectionSuccessWith2ndDegree);

                                                try
                                                {
                                                    string Querystring = "Insert into tb_ManageAddConnection (Keyword,LinkedinUrl,Username,DateTime) Values('" + _ConnectSearchKeyword + "','" + url[0] + "','" + _UserName + "','" + DateTime.Now.ToString("dd-MM-yyyy") + "')";
                                                    DataBaseHandler.InsertQuery(Querystring, "tb_ManageAddConnection");
                                                }
                                                catch { }
                                                valid = true;

                                                insertAccountData(_UserName, url[0], SendInvitationCount);




                                            }
                                            catch { }
                                        }
                                    }
                                    else if (postResponse1.Contains("You and this LinkedIn user don’t know anyone in common"))
                                    {
                                        string CSVHeader = "UserName" + "," + "SearchKeyword" + "," + "Invitation Sent To" + "," + "InvitationUrl";
                                        string[] url = Regex.Split(FrnAcceptUrL, "&authType");
                                        string CSV_Content = _UserName + "," + _ConnectSearchKeyword + ", ," + url[0];
                                        CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_ConnectionThroughUnkonwPeopleLink);
                                        LoggerManageConnection("[ " + DateTime.Now + " ] => [ Failed To Send Invitation From " + _UserName + " ]");
                                        valid = false;
                                        Isaccountvalid = false;
                                        continue;
                                    }
                                    else
                                    {
                                        string CSVHeader = "UserName" + "," + "SearchKeyword" + "," + "Invitation Sent To" + "," + "InvitationUrl";
                                        string[] url = Regex.Split(FrnAcceptUrL, "&authType");
                                        string CSV_Content = _UserName + "," + _ConnectSearchKeyword + "," + firstName + " " + lastName + "," + url[0];
                                        CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\LinkedInDominator\\ConnnectionNotAddedbyKeyword.csv");
                                        LoggerManageConnection("[ " + DateTime.Now + " ] => [ Failed To Send Invitation To" + firstName + " " + lastName + " ]");
                                        valid = false;
                                       // Isaccountvalid = false;
                                        SendInvitationCount--;
                                    }
                                }
                                if (pageResponce2.Contains("emailAddress-invitee-invitation"))
                                {
                                    LoggerManageConnection("[ " + DateTime.Now + " ] => [ Invitation to " + firstName + " " + lastName + " has already been sent from " + _UserName + " ]");
                                    SendInvitationCount--;
                                }
                               
                                if (postResponse1.Contains("Your invitation was not sent"))
                                {
                                    string CSVHeader1 = "UserName" + "," + "SearchKeyword" + "," + "Invitation Sent To" + "," + "InvitationUrl";
                                    string[] url = Regex.Split(FrnAcceptUrL, "&authType");
                                    string CSV_Content = _UserName + "," + _ConnectSearchKeyword + "," + firstName + " " + lastName + "," + url[0];
                                    CSVUtilities.ExportDataCSVFile(CSVHeader1, CSV_Content, Globals.path_ConnectionthroughKeywordSearchnotadded);
                                    LoggerManageConnection("[ " + DateTime.Now + " ] => [ Failed To Send Invitation From " + _UserName + " ]");
                                    valid = false;
                                    //   Isaccountvalid = false;
                                    SendInvitationCount--;

                                }

                                int Delay = RandomNumberGenerator.GenerateRandom(SearchMinDelay, SeacrhMaxDelay);
                                LoggerManageConnection("[ " + DateTime.Now + " ] => [ Invitation Delayed For : " + Delay + " Seconds ]");
                                Thread.Sleep(Delay * 1000);

                                SendInvitationCount++;
                                if (SendInvitationCount >= SearchCriteria.NumberOfRequestPerKeyword)  //Here we check  that number of request send if counter is equal to the number of request put by user then it return to mathod and made pagenumber = 0; 
                                {
                                    //SendInvitationCount--;
                                    PageNumber = 0;
                                    return;
                                }
                            }
                            else
                            {
                                LoggerManageConnection("[ " + DateTime.Now + " ] => [ Only visit the site " + FrnAcceptUrL + " ]");
                                string CSVHeader = "UserName" + "," + "Url";
                                string[] url = Regex.Split(FrnAcceptUrL, "&authType");
                                string CSV_Content = _UserName + "," + url[0];
                                CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_OnlyVisitProfileUsingKeyword);

                                DataSet Ds = new DataSet();
                                try
                                {
                                    string Querystring = "INSERT INTO tb_OnlyVisitProfile (Email,Url,DateTime) Values ('" + _UserName + "','" + url[0] + "','" + DateTime.Now + "')";
                                    Ds = DataBaseHandler.SelectQuery(Querystring, "tb_ManageAddConnection");
                                }
                                catch { }

                                insertAccountData(_UserName, url[0], SendInvitationCount);





                                int Delay = RandomNumberGenerator.GenerateRandom(SearchMinDelay, SeacrhMaxDelay);
                                Thread.Sleep(Delay * 1000);
                                LoggerManageConnection("[ " + DateTime.Now + " ] => [ Delayed For : " + Delay + " Seconds ]");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Stream was not readable")
                    {

                    }
                    string CSVHeader = "UserName" + "," + "SearchKeyword" + "," + "Invitation Sent To" + "," + "InvitationUrl";
                    string[] url = Regex.Split(FrnAcceptUrL, "&authType");
                    string CSV_Content = _UserName + "," + _ConnectSearchKeyword + ",," + url[0];
                    CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_ConnectionthroughKeywordSearchnotadded);
                    LoggerManageConnection("[ " + DateTime.Now + " ] => [ Failed To Send Invitation From " + _UserName + " ]");
                    valid = false;
                    Isaccountvalid = false;
                    SendInvitationCount--;
                }
            }
        }
        #endregion

        #region DecodeDBC
        public string DecodeDBC(string[] args, byte[] imageBytes)
        {
            try
            {
                // Put your DBC username & password here:
                DeathByCaptcha.Client client = (DeathByCaptcha.Client)new DeathByCaptcha.SocketClient(args[0], args[1]);
                client.Verbose = true;

                Console.WriteLine("Your balance is {0:F2} US cents", client.Balance);//Log("Your balance is " + client.Balance + " US cents ");

                if (!client.User.LoggedIn)
                {
                   // Log("[ " + DateTime.Now + " ] => [ Please check your DBC Account Details ]");
                    return null;
                }
                if (client.Balance == 0.0)
                {
                  //  Log("[ " + DateTime.Now + " ] => [ You have 0 Balance in your DBC Account ]");
                    return null;
                }

                for (int i = 2, l = args.Length; i < l; i++)
                {
                    Console.WriteLine("Solving CAPTCHA {0}", args[i]);

                    // Upload a CAPTCHA and poll for its status.  Put the CAPTCHA image
                    // file name, file object, stream, or a vector of bytes, and desired
                    // solving timeout (in seconds) here:
                    DeathByCaptcha.Captcha captcha = client.Decode(imageBytes, 2 * DeathByCaptcha.Client.DefaultTimeout);
                    if (null != captcha)
                    {
                        Console.WriteLine("CAPTCHA {0:D} solved: {1}", captcha.Id, captcha.Text);

                        //// Report an incorrectly solved CAPTCHA.
                        //// Make sure the CAPTCHA was in fact incorrectly solved, do not
                        //// just report it at random, or you might be banned as abuser.
                        //if (client.Report(captcha))
                        //{
                        //    Console.WriteLine("Reported as incorrectly solved");
                        //}
                        //else
                        //{
                        //    Console.WriteLine("Failed reporting as incorrectly solved");
                        //}

                        return captcha.Text;
                    }
                    else
                    {
                       // Log("[ " + DateTime.Now + " ] => [ CAPTCHA was not solved ]");
                        Console.WriteLine("CAPTCHA was not solved");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusFileHelper.AppendStringToTextfileNewLine(DateTime.Now + " --> Error --> TwitterSignup -  SignupMultiThreaded() -- DecodeDBC()  --> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace, Globals.Path_LinkedinErrorLogs);
                GlobusFileHelper.AppendStringToTextfileNewLine("Error --> TwitterSignup -  DecodeDBC() >>>> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace + " || DateTime :- " + DateTime.Now, Globals.Path_LinkedinAccountCraetionErrorLogs);
            }
            return null;
        }
        #endregion



    }
}
