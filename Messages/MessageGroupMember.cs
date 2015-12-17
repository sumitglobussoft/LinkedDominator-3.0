using Accounts;
using BaseLib;
using Globussoft;
using LinkDominator;
using linkedDominator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace Messages
{
    public class MessageGroupMember
    {
        public static Dictionary<string, Dictionary<string, string>> AllGroupNames = new Dictionary<string, Dictionary<string, string>>();
        public static Events objEvents = new Events();
        public void bindGroupNamesToComboBox(string args)
        {
            EventsArgs objEventsArgs = new EventsArgs(args);
            objEvents.LogText_sharan(objEventsArgs);
            //CampaignStopLogevents.LogText(eArgs);    

        }


        public void BindGroupMembersToCheckedListBox(string args)
        {
            EventsArgs objEventsArgs = new EventsArgs(args);
            objEvents.BindGroupMembersToCheckedListBox(objEventsArgs);

        }

        #region Variable declaration

        public bool preventMsgSameGroup = false;
        public bool preventMsgWithoutGroup = false;
        public bool preventMsgGlobal = false;

        public static string FromNam = string.Empty;
        public static string category_to_send_inmail = string.Empty;
        public static bool IssendInMail = false;
        public static int minDelay = 20;
        public static int maxDelay = 25;
        List<string> GrpMemMessagelist = new List<string>();
        List<string> GrpMemSubjectlist = new List<string>();
        public static bool IsSpintaxUsed = false;
        string FromemailId = string.Empty;
         string FromEmailNam = string.Empty;
        public static bool selectAllGroup = false;
        public static string messageSubject = string.Empty;
        public static string messageBody = string.Empty;
        public static bool IsMessagingWithTag = false;
        public static bool IschkAllAcounts = false;
      //  public static bool IswithExcelInput = false;
        public static List<string> selectedMembers = new List<string>();
        public static string selectedSpeUser = string.Empty;
        public static bool WithCsvinAddFriend = false;
        public static bool WithGroupSearch = false;
        public static Dictionary<string, string> GroupSpecMem = new Dictionary<string, string>();
        public static string SelectedAcc = string.Empty;
        public static string selected_group = string.Empty;
        public static Dictionary<string, string>  GroupMemData = new Dictionary<string, string>();
        Dictionary<string, Dictionary<string, string>> GrpMemNameWithGid = new Dictionary<string, Dictionary<string, string>>();
        public static string SearchKeyword = string.Empty;
        string Specgroupdetails = string.Empty;
        string SpeGroupId = string.Empty;
        public static bool withExcelInput = false;


        public static bool IsStop = false;
        List<Thread> lstGroupMessageThread = new List<Thread>();
        Dictionary<string, Dictionary<string, string>> GrpMemMess = new Dictionary<string, Dictionary<string, string>>();
        public static Dictionary<string, string> GroupName = new Dictionary<string, string>();
        public static List<string> GroupMemUrl = new List<string>();
        public static bool ManageTabGroupStatus = false;

        #endregion

        #region LinkdinGroupUpdate()
        public void LinkdinGroupMemberUpdate()
        {
            try
            {
                int numberofThreds = 5;
                //  counter_GroupMemberSearch = LinkedInManager.linkedInDictionary.Count;

                if (LDGlobals.loadedAccountsDictionary.Count() > 0)
                {
                    ThreadPool.SetMaxThreads(numberofThreds, 5);
                    foreach (KeyValuePair<string, LinkedinUser> item in LDGlobals.loadedAccountsDictionary)
                    {
                        try
                        {

                            ThreadPool.SetMaxThreads(numberofThreds, 5);

                             ThreadPool.QueueUserWorkItem(new WaitCallback(StartDMMultiThreadedGroupMemberUser), new object[] { item });

                            Thread.Sleep(1000);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Exception : " + ex);
                        }
                    }
                }
                else
                {
                    // MessageBox.Show("Please Load LinkedIn Accounts From MyAccounts Menu");
                    // MessageBoxResult result = MessageBox.Show("Hello MessageBox"); 
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Please Load LinkedIn Accounts From MyAccounts Menu ]");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Exception : " + ex);
            }
        }

        #endregion   
     

        #region StartDMMultiThreadedGroupMemberUser

        public void StartDMMultiThreadedGroupMemberUser(object parameter)
        {
            try
            {
                try
                {
                    if (IsStop)
                    {
                        return;
                    }

                    if (!IsStop)
                    {
                        lstGroupMessageThread.Add(Thread.CurrentThread);
                        lstGroupMessageThread.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error >>> " + ex.StackTrace);
                }
                 Array paramsArray = new object[1];
                paramsArray = (Array)parameter;

                KeyValuePair<string, LinkedinUser> item = (KeyValuePair<string, LinkedinUser>)paramsArray.GetValue(0);
                 LinkedinUser objLinkedinUser = item.Value;

                GlobusHttpHelper objGlobusHttpHelper = item.Value.globusHttpHelper;
               // string src = objGlobusHttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com"));

                if(!objLinkedinUser.isloggedin)
                {

                    GlobusHttpHelper objGlobusHttpHelper_new = new GlobusHttpHelper();
                    objLinkedinUser.globusHttpHelper = objGlobusHttpHelper_new;
                    AccountManager objAccountManager = new AccountManager();
                    //GlobusLogHelper.log.Info(objLinkedinUser);
                    objAccountManager.LoginHttpHelper(ref objLinkedinUser);

                }
                
                try
                {
                    if (objLinkedinUser.isloggedin)
                    {

                        Dictionary<string, string> Data = GetGroupNames(ref objLinkedinUser.globusHttpHelper, objLinkedinUser.username);

                        AllGroupNames.Add(objLinkedinUser.username, Data);

                        //for Event Generation

                        bindGroupNamesToComboBox("Argument");

                        //GroupStatus.ComboBoxDataBind("jhkj");






                        //if (cmbAllUser.InvokeRequired)
                        //{
                        //    new Thread(() =>
                        //    {
                        //        cmbAllUser.Invoke(new MethodInvoker(delegate
                        //        {
                        //            if (!cmbAllUser.Items.Contains(Login.accountUser))
                        //            {
                        //                cmbAllUser.Items.Add(Login.accountUser);
                        //            }
                        //        }));
                        //    }).Start();
                        //}
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Couldn't login with LinkedIn account : " + objLinkedinUser.username );
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info("Exception : " + ex);
                }

                finally
                {
                    //counter_GroupMemberSearch--;

                    //if (counter_GroupMemberSearch == 0)
                    //{
                    //    if (cmbAllUser.InvokeRequired)
                    //    {
                    //        cmbAllUser.Invoke(new MethodInvoker(delegate
                    //        {
                    //            cmbAllUser.Enabled = true;
                    //            AddLoggerLinkedingrp("[ " + DateTime.Now + " ] => [ PROCESS COMPLETE..Please select Account ]");
                    //            AddLoggerLinkedingrp("----------------------------------------------------------------------------------------------------------------------------------------");
                    //            btnGetGroup.Cursor = Cursors.Default;
                    //        }));
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Exception : " + ex);
            }
        }

        #endregion

        #region PostCreateGroupNames

        public Dictionary<string, string> GetGroupNames(ref GlobusHttpHelper HttpHelper, string user)
        {
            try
            {
              //  string url = "https://www.linkedin.com/profile/mappers?x-a=my_profile_follow%2Cmy_profile_groups&x-p=my_profile_summary_upsell%2EsummaryUpsell%3Atrue%2Cmy_profile_top_card%2Etc%3Atrue%2Cmy_profile_top_card%2EprofileContactsIntegrationStatus%3A0&x-oa=bottomAliases&id=327385798&locale=&snapshotID=&authToken=&authType=&invAcpt=&promoId=&notContactable=&primaryAction=&isPublic=false&sfd=false&_=1448346384101";
                string url = "https://www.linkedin.com";

                string pageSource = HttpHelper.getHtmlfromUrl(new Uri(url));

                string[] arr = Regex.Split(pageSource, "<h3>");
                string profId = Utils.getBetween(arr[2], "<a href=\"", "\"");
                pageSource = HttpHelper.getHtmlfromUrl(new Uri(profId));


                if (pageSource == "" || pageSource.Contains("Make sure you have cookies and Javascript enabled in your browser before signing in") || pageSource.Contains("manual_redirect_link"))
                {
                    Thread.Sleep(2000);
                    pageSource = HttpHelper.getHtmlfromUrl(new Uri("http://www.linkedin.com/myGroups?trk=nav_responsive_sub_nav_groups"));
                }

                if (pageSource == "" || pageSource.Contains("Make sure you have cookies and Javascript enabled in your browser before signing in") || pageSource.Contains("manual_redirect_link"))
                {
                    Thread.Sleep(2000);
                    pageSource = HttpHelper.getHtmlfromUrl(new Uri("http://www.linkedin.com/grp/"));
                }

                if (pageSource == "" || pageSource.Contains("Make sure you have cookies and Javascript enabled in your browser before signing in") || pageSource.Contains("manual_redirect_link"))
                {
                    Thread.Sleep(2000);
                    pageSource = HttpHelper.getHtmlfromUrl(new Uri("http://www.linkedin.com/myGroups?trk=nav_responsive_sub_nav_groups"));
                }

                if (pageSource.Contains("Your Groups") && pageSource.Contains("You may be interested in"))
                {
                    string[] RgxGroupData = Regex.Split(pageSource, "h3 class=\"title\"");
                    foreach (var GrpName in RgxGroupData)
                    {
                        string groupName = Utils.getBetween(GrpName, ">", "</h3>");
                        groupName = groupName + ":" + user;
                    }
                }
                if (pageSource.Contains("media-block has-activity"))
                {
                    string[] RgxGroupData = Regex.Split(pageSource, "media-content");

                    foreach (var GrpName in RgxGroupData)
                    {
                        string endName = string.Empty;
                        string endKey = string.Empty;

                        try
                        {
                            if (GrpName.Contains("<!DOCTYPE html>") || GrpName.Contains("Membership Pending"))
                            {
                                continue;
                            }

                            if (GrpName.Contains("<a href=\"/groups/") || (GrpName.Contains("<a href=\"/groups?")))
                            {
                                if ((GrpName.Contains("public")))
                                {
                                    try
                                    {
                                        int startindex = GrpName.IndexOf("class=\"public\"");
                                        string start = GrpName.Substring(startindex);
                                        int endIndex = start.IndexOf("</a>");
                                        endName = start.Substring(0, endIndex).Replace("title", string.Empty).Replace("=", string.Empty).Replace(">", string.Empty).Replace("groups", string.Empty).Replace("/", string.Empty).Replace("\"", string.Empty).Replace("&amp;", "&").Replace("classpublic", string.Empty).Replace("&quot;", "'").Replace(":", ";").Replace("This is an open group", string.Empty);
                                        endName = endName.Replace("&amp;", "&") + "^" + user;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Info("Exception: " + ex);
                                    }

                                    try
                                    {
                                        endKey = "";
                                        int startindex1 = GrpName.IndexOf("group");
                                        string start1 = GrpName.Substring(startindex1);
                                        int endIndex1 = start1.IndexOf("?");
                                        endKey = start1.Substring(0, endIndex1).Replace("groups", string.Empty).Replace("/", string.Empty).Replace("<a href=", string.Empty).Replace("\"", string.Empty);

                                        if (endKey == string.Empty)
                                        {
                                            startindex1 = GrpName.IndexOf("gid=");
                                            start1 = GrpName.Substring(startindex1);
                                            endIndex1 = start1.IndexOf("&");
                                            endKey = start1.Substring(0, endIndex1).Replace("gid=", string.Empty).Trim();

                                            if (!NumberHelper.ValidateNumber(endKey))
                                            {
                                                try
                                                {
                                                    endKey = endKey.Split('\"')[0];
                                                }
                                                catch { }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Info("Exception : " + ex);
                                    }

                                    if (endKey.Contains("analyticsURL"))
                                    {
                                        continue;
                                    }

                                    if (endKey == string.Empty)
                                    {
                                        try
                                        {
                                            int startindex2 = GrpName.IndexOf("gid=");
                                            string start2 = GrpName.Substring(startindex2);
                                            int endIndex2 = start2.IndexOf("&");
                                            endKey = start2.Substring(0, endIndex2).Replace("gid", string.Empty).Replace("=", string.Empty);
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Info("Exc");
                                        }
                                    }
                                    else
                                    {
                                        string[] endKeyLast = endKey.Split('-');
                                        try
                                        {
                                            if (NumberHelper.ValidateNumber(endKeyLast[1]))
                                            {
                                                endKey = endKeyLast[1];
                                            }
                                            else if (NumberHelper.ValidateNumber(endKeyLast[2]))
                                            {
                                                endKey = endKeyLast[2];
                                            }
                                            else if (NumberHelper.ValidateNumber(endKeyLast[3]))
                                            {
                                                endKey = endKeyLast[3];
                                            }
                                            else if (NumberHelper.ValidateNumber(endKeyLast[4]))
                                            {
                                                endKey = endKeyLast[4];
                                            }
                                            else if (NumberHelper.ValidateNumber(endKeyLast[5]))
                                            {
                                                endKey = endKeyLast[5];
                                            }
                                            else if (NumberHelper.ValidateNumber(endKeyLast[6]))
                                            {
                                                endKey = endKeyLast[6];
                                            }
                                        }
                                        catch { }
                                    }

                                    try
                                    {
                                        if (NumberHelper.ValidateNumber(endKey))
                                        {
                                            GroupName.Add(endName, endKey);
                                            GlobusLogHelper.log.Info("Group Name: " + endName);
                                        }
                                    }
                                    catch { }
                                }

                            }

                            if (GrpName.Contains("<a href=\"/groups/") || (GrpName.Contains("<a href=\"/groups?")))
                            {
                                if ((GrpName.Contains("private")))
                                {
                                    try
                                    {
                                        int startindex = GrpName.IndexOf("class=\"private\"");
                                        string start = GrpName.Substring(startindex);
                                        int endIndex = start.IndexOf("</a>");
                                        endName = start.Substring(0, endIndex).Replace("=", string.Empty).Replace(">", string.Empty).Replace("groups", string.Empty).Replace("/", string.Empty).Replace("\"", string.Empty).Replace("&amp;", "&").Replace("&quot;", "'").Replace(":", ";").Replace("classprivate", string.Empty);
                                        endName = endName.Replace("&amp;", "&") + "^" + user;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Info("Exception : "+ex);
                                    }

                                    try
                                    {
                                        int startindex1 = GrpName.IndexOf("gid=");
                                        string start1 = GrpName.Substring(startindex1);
                                        int endIndex1 = start1.IndexOf("&");
                                        endKey = start1.Substring(0, endIndex1).Replace("gid=", string.Empty).Replace("/", string.Empty).Replace("<a href=", string.Empty).Replace("\"", string.Empty).Replace("class=blanket-target><a>groups?home=", string.Empty).Trim();

                                        if (endKey == string.Empty)
                                        {
                                            try
                                            {
                                                endKey = endKey.Split(' ')[0].Trim();
                                            }
                                            catch { }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                       GlobusLogHelper.log.Info("Exception : "+ex);
                                    }

                                    if (endKey.Contains("analyticsURL"))
                                    {
                                        continue;
                                    }

                                    if (endKey == string.Empty)
                                    {
                                        try
                                        {
                                            int startindex2 = GrpName.IndexOf("gid=");
                                            string start2 = GrpName.Substring(startindex2);
                                            int endIndex2 = start2.IndexOf("&");
                                            endKey = start2.Substring(0, endIndex2).Replace("gid", string.Empty).Replace("=", string.Empty);

                                            if (!NumberHelper.ValidateNumber(endKey))
                                            {
                                                try
                                                {
                                                    endKey = endKey.Split('\"')[0];
                                                }
                                                catch { }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Info("Exception : "+ex);
                                        }
                                    }
                                    else
                                    {
                                        string[] endKeyLast = endKey.Split('-');
                                        try
                                        {
                                            if (NumberHelper.ValidateNumber(endKeyLast[1]))
                                            {
                                                endKey = endKeyLast[1];
                                            }
                                            else if (NumberHelper.ValidateNumber(endKeyLast[2]))
                                            {
                                                endKey = endKeyLast[2];
                                            }
                                            else if (NumberHelper.ValidateNumber(endKeyLast[3]))
                                            {
                                                endKey = endKeyLast[3];
                                            }
                                            else if (NumberHelper.ValidateNumber(endKeyLast[4]))
                                            {
                                                endKey = endKeyLast[4];
                                            }
                                            else if (NumberHelper.ValidateNumber(endKeyLast[5]))
                                            {
                                                endKey = endKeyLast[5];
                                            }
                                            else if (NumberHelper.ValidateNumber(endKeyLast[6]))
                                            {
                                                endKey = endKeyLast[6];
                                            }
                                        }
                                        catch { }
                                    }

                                    try
                                    {
                                        if (NumberHelper.ValidateNumber(endKey))
                                        {
                                            GroupName.Add(endName, endKey);
                                        }
                                    }
                                    catch { }
                                }

                            }
                        }
                        catch { }
                    }

                }
                else if(pageSource.Contains("{\"content\":{\"Following\":"))
                {
                    string[] RgxGroupData = Regex.Split(pageSource, "groupID");
                    RgxGroupData = RgxGroupData.Skip(1).ToArray();
                    foreach(string item in RgxGroupData)
                    {
                        try
                        {
                            string grpName = string.Empty;
                            string grpId = string.Empty;

                            grpName = Utils.getBetween(item, "name\":\"", "\",\"");
                            grpId = Utils.getBetween(item, "\":", ",\"");

                            grpName = grpName + "^" + user;
                            if (NumberHelper.ValidateNumber(grpId))
                            {
                                GroupName.Add(grpName, grpId);
                                GlobusLogHelper.log.Info("Group Name : " + grpName);
                            }
                        }
                        catch (Exception ex)
                        { 

                        }

                    }

                }
                else if (pageSource.Contains("link_groups_settings") || GroupName.Count==0)
                {
                    try
                    {
                        //string[] arr1 = Regex.Split(pageSource, "<h3>");
                        //string profileId = Utils.getBetween(arr1[2], "<a href=\"", "\"");
                        //pageSource = HttpHelper.getHtmlfromUrl(new Uri(profileId));

                        string[] arr1_groups = Regex.Split(pageSource, "link_groups_settings");
                        arr1_groups = arr1_groups.Skip(1).ToArray();

                        foreach (string item in arr1_groups)
                        {
                            try
                            {
                                string grpName = string.Empty;
                                string grpId = string.Empty;

                                grpName = Utils.getBetween(item, "name\":\"", "\",\"");
                                grpId = Utils.getBetween(item, "&gid=", "&");
                                grpName = grpName + "^" + user;
                                if (NumberHelper.ValidateNumber(grpId))
                                {
                                    GroupName.Add(grpName, grpId);
                                    GlobusLogHelper.log.Info("Group Name : " + grpName);
                                }
                            }
                            catch (Exception ex)
                            { }

                        }


                    }
                    catch
                    { }


                    if (GroupName.Count == 0)
                    {
                        try
                        {
                            string url_to_get_Groups = string.Empty;
                            url_to_get_Groups = Utils.getBetween(pageSource, "<div id=\"groups-container", "</div>");
                            url_to_get_Groups = Utils.getBetween(url_to_get_Groups, "url\":\"", "\"})");

                            pageSource = HttpHelper.getHtmlfromUrl(new Uri(url_to_get_Groups));

                            string[] arr1_groups = Regex.Split(pageSource, "link_groups_settings");
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

                                    grpName = grpName + "^" + user;
                                    if (NumberHelper.ValidateNumber(grpId))
                                    {
                                        GroupName.Add(grpName, grpId);
                                        GlobusLogHelper.log.Info("Group Name : " + grpName.Split('^')[0]);
                                    }
                                }
                                catch
                                { }
                            }


                        }
                        catch
                        { }
                    }
                }

                else
                {
                    if (pageSource.Contains("View More</span>"))
                    {
                        pageSource = Utils.getBetween(pageSource, "<ul class=\"group-list\">", "View More</span>");
                    }
                    else
                    {
                        string pageSource1 = Utils.getBetween(pageSource, "<ul class=\"group-list\">", "<li class=\"find-more\">");
                    }
                    string[] RgxGroupData = Regex.Split(pageSource, "h3 class=\"title\"");//"<li class=");
                    RgxGroupData = RgxGroupData.Skip(1).ToArray();
                    foreach (var GrpName in RgxGroupData)
                    {
                        string endName = string.Empty;
                        string endKey = string.Empty;

                        try
                        {
                            if (GrpName.Contains("<!DOCTYPE html>") || GrpName.Contains("Pending"))
                            {
                                continue;
                            }

                            //if (GrpName.Contains("<a href=\"/groups/") || (GrpName.Contains("<a href=\"/groups?")))
                            //{
                            //if ((GrpName.Contains("title")))
                            //{
                            try
                            {
                                int startindex = GrpName.IndexOf("class=\"title\"");
                                string start = GrpName.Substring(startindex).Replace("class=\"title\"", string.Empty);
                                int endIndex = start.IndexOf("<span");
                                endName = start.Substring(0, endIndex).Replace("title", string.Empty).Replace("=", string.Empty).Replace(">", string.Empty).Replace("groups", string.Empty).Replace("/", string.Empty).Replace("\"", string.Empty).Replace("&amp;", "&").Replace("classpublic", string.Empty).Replace("&quot;", "'").Replace(":", ";").Replace("This is an open group", string.Empty).Replace("class", "").Replace("&#39;", "'");
                                endName = Utils.getBetween(GrpName, ">", "</h3>");

                                if (endName.Contains("<span"))
                                {
                                    string[] RegexSpan = Regex.Split(endName, "<span");
                                    endName = RegexSpan[0];
                                    endName = endName.Replace(":", "").Replace("&amp;", "");
                                }
                                if (endName.Contains("<div group-activity"))
                                {
                                    endIndex = endName.IndexOf("<h3");
                                    endName = endName.Substring(0, endIndex).Replace("<h3", "").Replace("<div group-activity", "");
                                }
                                endName = endName.Replace("&amp;", "&") + "^" + user;
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Exception : " + ex);
                            }

                            try
                            {
                                endKey = "";
                                //int startindex1 = GrpName.IndexOf("group");
                                //string start1 = GrpName.Substring(startindex1);
                                //int endIndex1 = start1.IndexOf("?");
                                //endKey = start1.Substring(0, endIndex1).Replace("groups", string.Empty).Replace("/", string.Empty).Replace("<a href=", string.Empty).Replace("\"", string.Empty);

                                if (endKey == string.Empty)
                                {
                                    int startindex1 = GrpName.IndexOf("gid=");
                                    string start1 = GrpName.Substring(startindex1);
                                    int endIndex1 = start1.IndexOf("&");
                                    endKey = start1.Substring(0, endIndex1).Replace("gid=", string.Empty).Trim();

                                    if (!NumberHelper.ValidateNumber(endKey))
                                    {
                                        try
                                        {
                                            endKey = endKey.Split('\"')[0];
                                        }
                                        catch { }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Exception : " + ex);
                            }

                            if (endKey.Contains("analyticsURL"))
                            {
                                continue;
                            }

                            if (endKey == string.Empty)
                            {
                                try
                                {
                                    int startindex2 = GrpName.IndexOf("gid=");
                                    string start2 = GrpName.Substring(startindex2);
                                    int endIndex2 = start2.IndexOf("&");
                                    endKey = start2.Substring(0, endIndex2).Replace("gid", string.Empty).Replace("=", string.Empty);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Info("Exception : " + ex);
                                }
                            }
                            else
                            {
                                string[] endKeyLast = endKey.Split('-');
                                try
                                {
                                    if (NumberHelper.ValidateNumber(endKeyLast[1]))
                                    {
                                        endKey = endKeyLast[1];
                                    }
                                    else if (NumberHelper.ValidateNumber(endKeyLast[2]))
                                    {
                                        endKey = endKeyLast[2];
                                    }
                                    else if (NumberHelper.ValidateNumber(endKeyLast[3]))
                                    {
                                        endKey = endKeyLast[3];
                                    }
                                    else if (NumberHelper.ValidateNumber(endKeyLast[4]))
                                    {
                                        endKey = endKeyLast[4];
                                    }
                                    else if (NumberHelper.ValidateNumber(endKeyLast[5]))
                                    {
                                        endKey = endKeyLast[5];
                                    }
                                    else if (NumberHelper.ValidateNumber(endKeyLast[6]))
                                    {
                                        endKey = endKeyLast[6];
                                    }
                                }
                                catch { }
                            }

                            try
                            {
                                if (NumberHelper.ValidateNumber(endKey))
                                {
                                    GroupName.Add(endName, endKey);
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Exception : " + ex);
                            }
                            //}

                            //}
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Exception : " + ex);
                        }
                    }
                }

                return GroupName;
            }
            catch (Exception ex)
            {
                return GroupName;
            }

        }
        #endregion

        #region getGrop Members

        public void startGettingMembers()
        {
            bool CheckNetConn = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

            if (CheckNetConn)
            {
                try
                {
                    lstGroupMessageThread.Clear();
                    if (IsStop)
                    {
                        IsStop = false;
                    }

                    if (LDGlobals.loadedAccountsDictionary.Count() == 0)
                    {
                        GlobusLogHelper.log.Info("Please Upload Account");
                        return;
                    }
                   
                   
                    GroupMemData.Clear();
                    GrpMemNameWithGid.Clear();
                    try
                    {
                        new Thread(() =>
                        {
                            LinkedAddSpecifiedGroupMem();

                        }).Start();
                    }
                    catch
                    {
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info("Exception  : " + ex.StackTrace);
                }
            }
            else
            {
                GlobusLogHelper.log.Info("Internet connection is not available !!!");
            }

        }

        #endregion

        #region LinkedAddSpecifiedGroupMem
        private void LinkedAddSpecifiedGroupMem()
        {
            GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Please wait...Group Members have being added ]");

            try
            {
                if (LDGlobals.loadedAccountsDictionary.Count() > 0)
                {
                    string[] SelUser = new string[] { };

                    
                    try
                    {
                        SelUser = selected_group.Split(':');
                    }
                    catch { }

                    foreach (var item1 in GroupMemUrl)
                    {
                        try
                        {
                            string[] grp = item1.Split('^');
                            if (SelUser[1] == grp[0])
                            {
                                Specgroupdetails = item1;
                                string[] arrSpeGroupId = grp[1].Split(':');
                                SpeGroupId = arrSpeGroupId[1];
                                break;
                            }
                        }
                        catch { }
                    }

                    foreach (KeyValuePair<string, LinkedinUser> item in LDGlobals.loadedAccountsDictionary)
                    {
                        try
                        {
                            string selUser = string.Empty;


                            selUser = SelectedAcc;
                           

                            //if (chkAllAcounts.Checked)
                            //{
                            //    if (selUser == item.Key)
                            //    {
                            //        StartCrawlSpecificGroupUser(new object[] { item });
                            //        break;
                            //    }
                            //}
                            //else
                            //{

                            if (SelUser[0] == item.Key)
                                {
                                    StartCrawlSpecificGroupUser(new object[] { item });
                                    break;
                                }
                            //}
                        }
                        catch { }
                    }
                }
                else
                {
                    //MessageBox.Show("[ " + DateTime.Now + " ] => [ Please Load LinkedIn Accounts From MyAccounts Menu ]");
                    GlobusLogHelper.log.Info(" Please Load LinkedIn Accounts From MyAccounts Menu ");
                }
            }
            catch
            {
            }
            finally
            {
            //    if (chkListGroupMembers.InvokeRequired)
            //    {
            //        chkListGroupMembers.Invoke(new MethodInvoker(delegate
            //        {
            //            AddLoggerLinkedingrp("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED ]");
            //            AddLoggerLinkedingrp("------------------------------------------------------------------------------------------------------------------------------------");
            //            cmbAllUser.Enabled = true;
            //            btnAddMember.Cursor = Cursors.Default;
            //        }));
            //    }
            }
        }

        #endregion


        public void StartCrawlSpecificGroupUser(object parameter)
        {
            try
            {
                try
                {
                    if (IsStop)
                    {
                        return;
                    }

                    if (!IsStop)
                    {
                        lstGroupMessageThread.Add(Thread.CurrentThread);
                        lstGroupMessageThread.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error >>> " + ex.StackTrace);
                }

                Array paramsArray = new object[1];
                paramsArray = (Array)parameter;
                KeyValuePair<string, LinkedinUser> item = (KeyValuePair<string, LinkedinUser>)paramsArray.GetValue(0);

                GlobusHttpHelper HttpHelper = new GlobusHttpHelper();

                LinkedinUser objLinkedinUser = item.Value;
                if (!objLinkedinUser.isloggedin)
                {
                    AccountManager objAccountManager = new AccountManager();
                    objAccountManager.LoginHttpHelper(ref objLinkedinUser);

                }

                try
                {
                    if (objLinkedinUser.isloggedin)
                    {
                        string selecteduser = string.Empty;
                       // GroupStatus dataScrape = new GroupStatus();
                        //dataScrape.loggerGroupMem.addToLogger += new EventHandler(GroupMemMessage_addToLogger);


                        //cmbAllUser.Invoke(new MethodInvoker(delegate
                        //{
                        //    selecteduser = cmbAllUser.SelectedItem.ToString();

                        //}));

                        if (objLinkedinUser.username == SelectedAcc)
                        {
                            if (withExcelInput == true)
                            {
                              //  GroupMemData = dataScrape.AddSpecificGroupUserWithExcelInput_without_hitting_any_url(ref HttpHelper, Login.accountUser, SpeGroupId);

                            }
                            else
                            {
                                //GroupStatus.GroupMemUrl
                                if (IschkAllAcounts)
                                {
                                    foreach (var SpeGroupId1 in GroupMemUrl)
                                    {
                                        Dictionary<string, string> GroupMem = AddSpecificGroupUser(ref HttpHelper, objLinkedinUser, SpeGroupId1);
                                        foreach (var GroupMem_item in GroupMem)
                                        {
                                            try
                                            {
                                                string GrpMemberKey = GroupMem_item.Key;
                                                string GrpMemberValue = GroupMem_item.Value;

                                                GroupMemData.Add(GrpMemberKey, GrpMemberValue);

                                            }
                                            catch { };

                                        }

                                    }
                                }
                                else
                                {
                                    GroupMemData = AddSpecificGroupUser(ref HttpHelper, objLinkedinUser, Specgroupdetails);
                                }
                            }

                            if (GroupMemData.Count == 0)
                            {
                                GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Group : " + objLinkedinUser.username + " Has No Other Member than You ]");
                                return;
                            }
                            GrpMemNameWithGid.Add(objLinkedinUser.username, GroupMemData);
                        }


                        //bindGroupNamesToComboBox("GroupMemName");
                        BindGroupMembersToCheckedListBox("GroupMemNames");
                        //if (chkListGroupMembers.InvokeRequired)
                        //{
                        //    new Thread(() =>
                        //    {
                        //       // chkListGroupMembers.Invoke(new MethodInvoker(delegate
                        //        {
                        //            int count = 0;
                        //            // Globals.no_of_accounts_to_be_checked;

                        //            foreach (var itemM in GroupMemData)
                        //            {

                        //                if (count < Globals.no_of_accounts_to_be_checked)
                        //                {
                        //                   // chkListGroupMembers.Items.Add(itemM.Value, true);
                        //                    count++;
                        //                }
                        //                else
                        //                {
                        //                    //chkListGroupMembers.Items.Add(itemM.Value, false);

                        //                }
                        //            }
                        //        }));
                        //    }).Start();
                        //}
                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Added Group Members of : " + objLinkedinUser.username + " ]");
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ LinkedIn account : " + objLinkedinUser.username + " has been temporarily restricted ]");
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info("Exception : " + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error >>> " + ex.StackTrace);
            }
        }


        #region AddSpecificGroupUser
        public Dictionary<string, string> AddSpecificGroupUser(ref GlobusHttpHelper HttpHelper, LinkedinUser objLinkedinUser, string gid)
        {
            string endName = string.Empty;
            string DeegreeConn = string.Empty;
            string endKey = string.Empty;
            string Locality = string.Empty;
            string Val_sourceAlias = string.Empty;
            string Val_key = string.Empty;
            string Val_defaultText = string.Empty;
            string Name = string.Empty;
            string Val_CsrToken = string.Empty;
            string Val_Subject = string.Empty;
            string Val_greeting = string.Empty;
            string Val_AuthToken = string.Empty;
            string Val_AuthType = string.Empty;
            string val_trk = string.Empty;
            string Val_lastName = string.Empty;
            string html = string.Empty;
            string Title = string.Empty;

            #region Data Initialization
            string GroupMemId = string.Empty;
            string GroupName = string.Empty;
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
            string titlepast1 = string.Empty;
            string companypast1 = string.Empty;
            string titlepast2 = string.Empty;
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
            string Company = string.Empty;
            List<string> lstpasttitle = new List<string>();
            List<string> checkpasttitle = new List<string>();
            string csrfToken = string.Empty;
            string pageSource = string.Empty;
            string[] RgxSikValue = new string[] { };
            string[] RgxPageNo = new string[] { };
            string sikvalue = string.Empty;
            int pageno = 25;
            int counter = 0;

            string group_Url = string.Empty;
            string pagination_url = string.Empty;

            string groupId = string.Empty;
            string UserID = string.Empty;

            #endregion


            try
            {
                HttpHelper = objLinkedinUser.globusHttpHelper;
                UserID = objLinkedinUser.username;
                GroupSpecMem.Clear();
                GroupName = gid.Split('^')[0];
                groupId = gid.Split(':')[1];
                string pageSource1 = HttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com/home?trk=hb_tab_home_top"));

                if (pageSource1.Contains("csrfToken"))
                {
                    csrfToken = pageSource1.Substring(pageSource1.IndexOf("csrfToken"), 50);
                    string[] Arr = csrfToken.Split('>');
                    csrfToken = Arr[0];
                    csrfToken = csrfToken.Replace(":", "%3A").Replace("csrfToken", "").Replace("\"", string.Empty).Replace("value", string.Empty).Replace("cs", string.Empty).Replace("id", string.Empty).Replace("=", string.Empty).Replace("\n", string.Empty).Replace(">", string.Empty).Replace("<script src", string.Empty);
                    csrfToken = csrfToken.Trim();
                }



                for (int i = 1; i <= pageno; i++)
                {
                    counter++;

                    string[] RgxGroupData = new string[] { };

                    #region with group search
                    if (WithGroupSearch == true)
                    {

                        string txid = (UnixTimestampFromDateTime(System.DateTime.Now) * 1000).ToString();

                        if (counter == 1)
                        {

                            //  pageSource = HttpHelper.getHtmlfromUrl(new Uri("http://www.linkedin.com/groups?viewMembers=&gid=" + gid.Split(':')[2]));

                            pageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com/groups?viewMembers=&gid=" + gid.Split(':')[1]));

                            string Gid = string.Empty;
                            try
                            {
                                Gid = gid.Split(':')[1];
                            }
                            catch (Exception ex)
                            {
                            }
                            string URL = "https://www.linkedin.com/groups?viewMembers=&gid=" + Gid;
                            pageSource = HttpHelper.getHtmlfromUrl(new Uri(URL));

                            RgxSikValue = System.Text.RegularExpressions.Regex.Split(pageSource, "sik");
                            try
                            {
                                sikvalue = RgxSikValue[1].Split('&')[0].Replace("=", string.Empty);
                            }
                            catch { }

                            try
                            {
                                if (NumberHelper.ValidateNumber(sikvalue))
                                {
                                    sikvalue = sikvalue.Split('\"')[0];
                                }
                                else
                                {
                                    sikvalue = sikvalue.Split('\"')[0];
                                }
                            }
                            catch
                            {
                                sikvalue = sikvalue.Split('\"')[0];
                            }

                            //if (pageSource.Contains("<p class=\"paginate\">"))
                            //{
                            //    pagination_url = Utils.getBetween(pageSource, "<p class=\"paginate\">", "</a>");
                            //    pagination_url = Utils.getBetween(pagination_url, "href=\"", "\">").Replace("amp;", "");
                            //    pagination_url = "https://www.linkedin.com" + pagination_url;
                            //}

                            if (!string.IsNullOrEmpty(sikvalue))
                            {

                                //string postdata = "csrfToken=" + csrfToken + "&searchField=" + SearchKeyword + "&searchMembers=submit&searchMembers=Search&gid=" + gid.Split(':')[2] + "&goback=.gna_" + gid.Split(':')[2] + "";
                                //pageSource = HttpHelper.postFormDataRef(new Uri("https://www.linkedin.com/groups"), postdata, "http://www.linkedin.com/groups?viewMembers=&gid=" + gid.Split(':')[2] + "&sik=" + txid + "&split_page=" + i + "&goback=%2Egna_" + gid.Split(':')[2] + "", "", "");
                                #region Commented by sharan
                                string postdata = "csrfToken=" + csrfToken + "&searchField=" + SearchKeyword + "&searchMembers=submit&searchMembers=Search&gid=" + gid.Split(':')[1] + "&goback=.gna_" + gid.Split(':')[1] + "";
                                pageSource = HttpHelper.postFormDataRef(new Uri("https://www.linkedin.com/groups"), postdata, "http://www.linkedin.com/groups?viewMembers=&gid=" + gid.Split(':')[1] + "&sik=" + txid + "&split_page=" + i + "&goback=%2Egna_" + gid.Split(':')[1] + "", "", "");
                                #endregion

                                #region written by sharan
                                pageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com/grp/members?csrfToken=" + csrfToken + "&search=" + SearchKeyword.Replace(" ", "+") + "&gid=" + gid.Split(':')[1]));
                                #endregion
                                //string postdata = "csrfToken=" + csrfToken + "&searchField=" + SearchKeyword + "&searchMembers=submit&searchMembers=Search&gid=" + gid.Split(':')[1] + "&goback=.gna_" + gid.Split(':')[1] + "";
                                //pageSource = HttpHelper.postFormDataRef(new Uri("http://www.linkedin.com/groups"), postdata, "http://www.linkedin.com/groups?viewMembers=&gid=" + gid.Split(':')[1] + "&sik=" + txid + "&split_page=" + i + "&goback=%2Egna_" + gid.Split(':')[1] + "", "", "");

                            }
                            else
                            {

                                pageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com/grp/members?csrfToken=" + csrfToken + "&search=" + SearchKeyword.Replace(" ", "+") + "&gid=" + gid.Split(':')[1]));

                                // pageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com/grp/members?csrfToken=" + csrfToken + "&search=" + SearchKeyword.Replace(" ", "+") + "&gid=" + gid.Split(':')[1]));

                            }
                        }

                        if (!string.IsNullOrEmpty(sikvalue))
                        {
                            if (counter > 1)
                            {


                                // string getdata = "http://www.linkedin.com/groups?viewMembers=&gid=" + gid.Split(':')[2] + "&sik=" + txid + "&split_page=" + i + "&goback=%2Egna_" + gid.Split(':')[2] + "";
                                string getdata = "http://www.linkedin.com/groups?viewMembers=&gid=" + gid.Split(':')[1] + "&sik=" + txid + "&split_page=" + i + "&goback=%2Egna_" + gid.Split(':')[1] + "";
                                string new_pagination_url = pagination_url;
                                new_pagination_url = Utils.getBetween("###" + new_pagination_url, "###", "page=");
                                new_pagination_url = new_pagination_url + "page=" + counter;
                                pageSource = HttpHelper.getHtmlfromUrl(new Uri(new_pagination_url));
                                //   pageSource = HttpHelper.getHtmlfromUrl(new Uri(getdata));


                                // string getdata = "http://www.linkedin.com/groups?viewMembers=&gid=" + gid.Split(':')[1] + "&sik=" + txid + "&split_page=" + i + "&goback=%2Egna_" + gid.Split(':')[1] + "";
                                //  pageSource = HttpHelper.getHtmlfromUrl(new Uri(getdata));

                            }



                            RgxGroupData = System.Text.RegularExpressions.Regex.Split(pageSource, "<li class=\"member\">");


                            if (counter == 1)
                            {
                                try
                                {
                                    RgxPageNo = System.Text.RegularExpressions.Regex.Split(pageSource, "<h3 class=\"page-title\">Search Results: <span>");
                                    pageno = Convert.ToInt32(RgxPageNo[1].Split('<')[0].Replace("(", string.Empty).Replace(")", string.Empty).Replace("+", string.Empty).Trim());
                                    pageno = pageno / 20 + 1;
                                }
                                catch { }

                                if (pageno > 25)
                                {
                                    pageno = 25;
                                }

                                try
                                {
                                    if (pageSource.Contains("<p class=\"paginate\">"))
                                    {
                                        pagination_url = Utils.getBetween(pageSource, "<p class=\"paginate\">", "</a>");
                                        pagination_url = Utils.getBetween(pagination_url, "href=\"", "\">").Replace("amp;", "");
                                        pagination_url = "https://www.linkedin.com" + pagination_url;
                                        pagination_url = pagination_url.Replace("amp;", "");
                                    }
                                }
                                catch
                                { }
                            }
                        }
                        else
                        {
                            // RgxGroupData = System.Text.RegularExpressions.Regex.Split(pageSource, "<li class=\"member\">");
                            if (counter > 1)
                            {


                                //string getdata = "https://www.linkedin.com/grp/members?csrfToken=" + csrfToken + "&search=" + SearchKeyword.Replace(" ", "+") + "&gid=" + gid.Split(':')[2] + "&page="+i;

                                //string getdata = "https://www.linkedin.com/grp/members?csrfToken=" + csrfToken + "&search=" + SearchKeyword.Replace(" ", "+") + "&gid=" + gid.Split(':')[2] + "&page=" + i;

                                string new_pagination_url = pagination_url;
                                new_pagination_url = Utils.getBetween("###" + new_pagination_url, "###", "page=");
                                new_pagination_url = new_pagination_url + "page=" + counter;
                                pageSource = HttpHelper.getHtmlfromUrl(new Uri(new_pagination_url));

                                // pageSource = HttpHelper.getHtmlfromUrl(new Uri(getdata));
                            }


                            if (string.IsNullOrEmpty(sikvalue) && counter == 1)
                            {
                                try
                                {
                                    if (pageSource.Contains("<p class=\"paginate\">"))
                                    {
                                        pagination_url = Utils.getBetween(pageSource, "<p class=\"paginate\">", "</a>");
                                        pagination_url = Utils.getBetween(pagination_url, "href=\"", "\">").Replace("amp;", "");
                                        pagination_url = "https://www.linkedin.com" + pagination_url;
                                        pagination_url = pagination_url.Replace("amp;", "");
                                    }
                                }
                                catch
                                { }
                            }



                            RgxGroupData = System.Text.RegularExpressions.Regex.Split(pageSource, "<li class=\"member\">");

                            if (counter == 1)
                            {
                                try
                                {
                                    RgxPageNo = System.Text.RegularExpressions.Regex.Split(pageSource, "<h3 class=\"page-title\">Search Results <span>");
                                    pageno = Convert.ToInt32(RgxPageNo[1].Split('<')[0].Replace("(", string.Empty).Replace(")", string.Empty).Replace("+", string.Empty).Trim());
                                    pageno = pageno / 20 + 1;
                                }
                                catch { }

                                if (pageno > 25)
                                {
                                    pageno = 25;
                                }
                            }
                        }
                    }

                    #endregion

                    else
                    {
                        //pageSource = HttpHelper.getHtmlfromUrl(new Uri("http://www.linkedin.com/groups?viewMembers=&gid=" + gid.Split(':')[2] + "&split_page=" + i + ""));
                        pageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com/grp/members?gid=" + gid.Split(':')[1] + "&page=" + i));

                        group_Url = "https://www.linkedin.com/grp/members?gid=" + gid.Split(':')[1];


                        RgxGroupData = System.Text.RegularExpressions.Regex.Split(pageSource, "<li class=\"member\" id=\"");
                        if (RgxGroupData.Length == 1)
                        {
                            RgxGroupData = Regex.Split(pageSource, "<span class=\"new-miniprofile-container\"");
                            if (RgxGroupData.Length == 1)
                            {
                                break;
                            }
                        }
                    }

                    #region for csv
                    if (WithCsvinAddFriend == true)
                    {
                        List<string> PageSerchUrl = ChilkatBasedRegex.GettingAllUrls(pageSource, "profile/view?id");
                        string FrnAcceptUrL = string.Empty;
                        foreach (string item in PageSerchUrl)
                        {
                            try
                            {
                                if (item.Contains("/profile/view?id"))
                                {
                                    FrnAcceptUrL = "http://www.linkedin.com" + item;
                                    string[] urll = Regex.Split(FrnAcceptUrL, "&authType");
                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ " + FrnAcceptUrL + " ]");

                                    string stringSource = HttpHelper.getHtmlfromUrl(new Uri(FrnAcceptUrL));

                                    #region GroupMemId
                                    try
                                    {
                                        string[] gid1 = FrnAcceptUrL.Split('&');
                                        GroupMemId = gid1[0].Replace("http://www.linkedin.com/profile/view?id=", string.Empty);
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
                                                strFamilyName = stringSource.Substring(stringSource.IndexOf("i18n__Overview_for_X"), (stringSource.IndexOf(",", stringSource.IndexOf("i18n__Overview_for_X")) - stringSource.IndexOf("i18n__Overview_for_X"))).Replace("i18n__Overview_for_X", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":Overview for", "").Trim();

                                            }
                                            catch { }
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

                                        if (NameArr.Count() == 3)
                                        {
                                            lastname = NameArr[1] + " " + NameArr[2];
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

                                    #region Company
                                    Company = string.Empty;
                                    try
                                    {
                                        try
                                        {
                                            try
                                            {
                                                //Company = stringSource.Substring(stringSource.IndexOf("visible\":true,\"memberHeadline"), (stringSource.IndexOf("memberID", stringSource.IndexOf("visible\":true,\"memberHeadline")) - stringSource.IndexOf("visible\":true,\"memberHeadline"))).Replace("visible\":true,\"memberHeadline", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Replace("   ", string.Empty).Trim();
                                                Company = stringSource.Substring(stringSource.IndexOf("\"memberHeadline"), (stringSource.IndexOf("memberID", stringSource.IndexOf("\"memberHeadline")) - stringSource.IndexOf("\"memberHeadline"))).Replace("\"memberHeadline", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Replace("i18n__LocationLocationcompletenessLevel4", string.Empty).Replace("visibletrue", "").Replace("isPortfoliofalse", string.Empty).Replace("isLNLedtrue", string.Empty).Trim();
                                            }
                                            catch
                                            {
                                            }

                                            if (string.IsNullOrEmpty(Company))
                                            {
                                                try
                                                {
                                                    //memberHeadline
                                                    Company = stringSource.Substring(stringSource.IndexOf("memberHeadline\":"), (stringSource.IndexOf(",", stringSource.IndexOf("memberHeadline\":")) - stringSource.IndexOf("memberHeadline\":"))).Replace("memberHeadline\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Replace("   ", string.Empty).Replace(":", "").Replace("&dsh;", "").Replace("&amp", "").Replace(";", "").Replace("isPortfoliofalse", string.Empty).Replace("isLNLedtrue", string.Empty).Trim();
                                                }
                                                catch
                                                {
                                                }

                                            }

                                            titlecurrent = string.Empty;
                                            companycurrent = string.Empty;
                                            string[] strdesigandcompany = new string[4];
                                            if (Company.Contains(" at ") || Company.Contains(" of "))
                                            {
                                                try
                                                {
                                                    strdesigandcompany = Regex.Split(Company, " at ");

                                                    if (strdesigandcompany.Count() == 1)
                                                    {
                                                        strdesigandcompany = Regex.Split(Company, " of ");
                                                    }
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
                                                    companycurrent = strdesigandcompany[1];
                                                }
                                                catch { }
                                                #endregion
                                            }

                                            if (titlecurrent == string.Empty)
                                            {
                                                titlecurrent = Company;
                                            }
                                        }
                                        catch { }

                                        #region PastCompany
                                        string[] companylist = Regex.Split(stringSource, "companyName\"");
                                        string AllComapny = string.Empty;

                                        string Companyname = string.Empty;
                                        checkerlst.Clear();
                                        foreach (string item1 in companylist)
                                        {
                                            try
                                            {
                                                if (!item1.Contains("<!DOCTYPE html>"))
                                                {
                                                    Companyname = item1.Substring(item1.IndexOf(":"), (item1.IndexOf(",", item1.IndexOf(":")) - item1.IndexOf(":"))).Replace(":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Replace("]", string.Empty).Replace("}", string.Empty).Trim();
                                                    //Checklist.Add(item);
                                                    string items = item1;
                                                    checkerlst.Add(Companyname);
                                                    checkerlst = checkerlst.Distinct().ToList();
                                                }
                                            }
                                            catch { }
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

                                        if (companycurrent == string.Empty)
                                        {
                                            companycurrent = checkerlst[0].ToString();
                                        }
                                        #endregion

                                    #endregion Company

                                        #region Company Descripription

                                        try
                                        {
                                            string[] str_CompanyDesc = Regex.Split(stringSource, "showSummarySection");

                                            foreach (string item2 in str_CompanyDesc)
                                            {
                                                try
                                                {
                                                    string Current_Company = string.Empty;
                                                    if (!item2.Contains("<!DOCTYPE html>"))
                                                    {
                                                        int startindex = item2.IndexOf("specialties\":\"");

                                                        if (startindex > 0)
                                                        {
                                                            try
                                                            {
                                                                string start = item2.Substring(startindex).Replace("specialties\":", "");
                                                                int endindex = start.IndexOf("\",\"associatedWith\"");
                                                                string end = start.Substring(0, endindex);
                                                                Current_Company = end.Replace(",\"specialties_lb\":", string.Empty).Replace("\"", string.Empty).Replace("summary_lb", "Summary").Replace(",", ";").Replace("\"u002", "-");
                                                                LDS_BackGround_Summary = Current_Company;
                                                            }
                                                            catch { }
                                                        }

                                                    }

                                                    if (!item2.Contains("<!DOCTYPE html>"))
                                                    {
                                                        int startindex = item2.IndexOf("\"summary_lb\"");

                                                        if (startindex > 0)
                                                        {
                                                            try
                                                            {
                                                                string start = item2.Substring(startindex).Replace("\"summary_lb\"", "");
                                                                int endindex = start.IndexOf("\",\"associatedWith\"");
                                                                string end = start.Substring(0, endindex);
                                                                Current_Company = end.Replace(",\"specialties_lb\":", string.Empty).Replace("<br>", string.Empty).Replace("\n\"", string.Empty).Replace("\"", string.Empty).Replace("summary_lb", "Summary").Replace(",", ";").Replace("u002", "-").Replace(":", string.Empty);
                                                                LDS_BackGround_Summary = Current_Company;
                                                            }
                                                            catch { }
                                                        }

                                                    }

                                                }
                                                catch { }
                                            }
                                        }
                                        catch { }

                                        #endregion

                                        #region Education
                                        EducationCollection = string.Empty;
                                        try
                                        {
                                            try
                                            {
                                                EducationCollection = stringSource.Substring(stringSource.IndexOf("\"schoolName\":"), (stringSource.IndexOf(",", stringSource.IndexOf("\"schoolName\":")) - stringSource.IndexOf("\"schoolName\":"))).Replace("\"schoolName\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Trim();
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    //  education1 = stringSource.Substring(stringSource.IndexOf("i18n__Overview_for_X"), (stringSource.IndexOf(",", stringSource.IndexOf("i18n__Overview_for_X")) - stringSource.IndexOf("i18n__Overview_for_X"))).Replace("i18n__Overview_for_X", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":Overview for", "").Trim();

                                                }
                                                catch { }
                                            }
                                        }
                                        catch { }

                                        #endregion

                                        #region Email
                                        try
                                        {
                                            string[] str_Email = Regex.Split(stringSource, "email\"");
                                            USERemail = stringSource.Substring(stringSource.IndexOf("[{\"email\":"), (stringSource.IndexOf("}]", stringSource.IndexOf("[{\"email\":")) - stringSource.IndexOf("[{\"email\":"))).Replace("[{\"email\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        #endregion Email

                                        #region Website
                                        Website = string.Empty;
                                        try
                                        {
                                            Website = stringSource.Substring(stringSource.IndexOf("[{\"URL\":"), (stringSource.IndexOf(",", stringSource.IndexOf("[{\"URL\":")) - stringSource.IndexOf("[{\"URL\":"))).Replace("[{\"URL\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace("}]", string.Empty).Trim();
                                        }
                                        catch { }
                                        #endregion Website

                                        #region location
                                        location = string.Empty;
                                        try
                                        {
                                            location = stringSource.Substring(stringSource.IndexOf("Country\",\"fmt__location\":"), (stringSource.IndexOf("i18n_no_location_matches", stringSource.IndexOf("Country\",\"fmt__location\":")) - stringSource.IndexOf("Country\",\"fmt__location\":"))).Replace("Country\",\"fmt__location\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(":", string.Empty).Replace(",", string.Empty).Trim();
                                        }
                                        catch (Exception ex)
                                        {
                                            try
                                            {
                                                if (string.IsNullOrEmpty(location))
                                                {
                                                    int startindex = stringSource.IndexOf("\"fmt_location\":\"");
                                                    if (startindex > 0)
                                                    {
                                                        string start = stringSource.Substring(startindex).Replace("\"fmt_location\":\"", "");
                                                        int endindex = start.IndexOf("\",");
                                                        string end = start.Substring(0, endindex);
                                                        country = end;
                                                    }
                                                }
                                            }
                                            catch (Exception ex1)
                                            {

                                            }
                                        }

                                        #endregion location

                                        #region Country
                                        try
                                        {
                                            int startindex = stringSource.IndexOf("\"locationName\":\"");
                                            if (startindex > 0)
                                            {
                                                string start = stringSource.Substring(startindex).Replace("\"locationName\":\"", "");
                                                int endindex = start.IndexOf("\",");
                                                string end = start.Substring(0, endindex);
                                                location = end;
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        #endregion

                                        #region Industry
                                        Industry = string.Empty;
                                        try
                                        {
                                            //Industry = stringSource.Substring(stringSource.IndexOf("fmt__industry_highlight\":"), (stringSource.IndexOf(",", stringSource.IndexOf("fmt__industry_highlight\":")) - stringSource.IndexOf("fmt__industry_highlight\":"))).Replace("fmt__industry_highlight\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                                            int startindex = stringSource.IndexOf("\"industry_highlight\":\"");
                                            if (startindex > 0)
                                            {
                                                string start = stringSource.Substring(startindex).Replace("\"industry_highlight\":\"", "");
                                                int endindex = start.IndexOf("\",");
                                                string end = start.Substring(0, endindex).Replace("&amp;", "&");
                                                Industry = end;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                        #endregion Industry

                                        #region Connection
                                        Connection = string.Empty;
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
                                        #endregion Connection

                                        #region Recommendation
                                        try
                                        {
                                            //recomandation = stringSource.Substring(stringSource.IndexOf("i18n__Recommend_Query\":"), (stringSource.IndexOf(",", stringSource.IndexOf("i18n__Recommend_Query\":")) - stringSource.IndexOf("i18n__Recommend_Query\":"))).Replace("i18n__Recommend_Query\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();         
                                            string PageSource = HttpHelper.getHtmlfromUrl(new Uri("http://www.linkedin.com/profile/profile-v2-endorsements?id=" + GroupMemId + "&authType=OUT_OF_NETWORK&authToken=rXRG&goback=%2Efps_PBCK_*1_*1_*1_*1_*1_*1_tcs_*2_CP_I_us_*1_*1_false_1_R_*1_*51_*1_*51_true_*1_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2_*2"));
                                            string[] arrayRecommendedName = Regex.Split(PageSource, "headline");
                                            List<string> ListRecommendationName = new List<string>();
                                            recomandation = string.Empty;
                                            foreach (var itemRecomName in arrayRecommendedName)
                                            {
                                                try
                                                {
                                                    if (!itemRecomName.Contains("Endorsements"))
                                                    {
                                                        try
                                                        {

                                                            int startindex = itemRecomName.IndexOf(":");
                                                            string start = itemRecomName.Substring(startindex);
                                                            int endIndex = start.IndexOf("\",");
                                                            string Heading = (start.Substring(0, endIndex).Replace("\"", string.Empty).Replace(":", string.Empty));

                                                            int startindex1 = itemRecomName.IndexOf("fmt__referrerfullName");
                                                            string start1 = itemRecomName.Substring(startindex1);
                                                            int endIndex1 = start1.IndexOf("\",");
                                                            Name = (start1.Substring(0, endIndex1).Replace("\"", string.Empty).Replace("fmt__referrerfullName", string.Empty).Replace(":", string.Empty));

                                                            ListRecommendationName.Add(Name + " : " + Heading);
                                                        }
                                                        catch { }
                                                    }
                                                }
                                                catch { }

                                            }

                                            foreach (var item5 in ListRecommendationName)
                                            {
                                                if (recomandation == string.Empty)
                                                {
                                                    recomandation = item5;
                                                }
                                                else
                                                {
                                                    recomandation += "  -  " + item5;
                                                }
                                            }

                                        }
                                        catch { }
                                        #endregion

                                        #region Experience
                                        LDS_Experience = string.Empty;
                                        if (LDS_Experience == string.Empty)
                                        {
                                            try
                                            {
                                                string[] array = Regex.Split(stringSource, "title_highlight");
                                                string exp = string.Empty;
                                                string comp = string.Empty;
                                                List<string> ListExperince = new List<string>();
                                                string SelItem = string.Empty;

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

                                                foreach (var itemExp in ListExperince)
                                                {
                                                    if (LDS_Experience == string.Empty)
                                                    {
                                                        LDS_Experience = itemExp;
                                                    }
                                                    else
                                                    {
                                                        LDS_Experience += "  -  " + itemExp;
                                                    }
                                                }

                                            }
                                            catch { }
                                        }

                                        #endregion

                                        #region Group
                                        try
                                        {
                                            string PageSource = HttpHelper.getHtmlfromUrl(new Uri("http://www.linkedin.com/profile/mappers?x-a=profile_v2_groups%2Cprofile_v2_follow%2Cprofile_v2_connections&x-p=profile_v2_discovery%2Erecords%3A4%2Ctop_card%2EprofileContactsIntegrationStatus%3A0%2Cprofile_v2_comparison_insight%2Edistance%3A1%2Cprofile_v2_right_fixed_discovery%2Eoffset%3A0%2Cprofile_v2_connections%2Edistance%3A1%2Cprofile_v2_right_fixed_discovery%2Erecords%3A4%2Cprofile_v2_network_overview_insight%2Edistance%3A1%2Cprofile_v2_right_top_discovery_teamlinkv2%2Eoffset%3A0%2Cprofile_v2_right_top_discovery_teamlinkv2%2Erecords%3A4%2Cprofile_v2_discovery%2Eoffset%3A0%2Cprofile_v2_summary_upsell%2EsummaryUpsell%3Atrue%2Cprofile_v2_network_overview_insight%2EnumConn%3A1668%2Ctop_card%2Etc%3Atrue&x-oa=bottomAliases&id=" + GroupMemId + "&locale=&snapshotID=&authToken=&authType=name&invAcpt=&notContactable=&primaryAction=&isPublic=false&sfd=true&_=1366115853014"));

                                            string[] array = Regex.Split(PageSource, "href=\"/groupRegistration?");
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

                                            foreach (var item6 in ListGroupName)
                                            {
                                                if (groupscollectin == string.Empty)
                                                {
                                                    groupscollectin = item6;
                                                }
                                                else
                                                {
                                                    groupscollectin += "  -  " + item6;

                                                }
                                            }

                                        }
                                        catch { }

                                        #endregion

                                        #region skill and Expertise
                                        try
                                        {
                                            string[] strarr_skill = Regex.Split(stringSource, "endorse-item-name-text\"");
                                            string[] strarr_skill1 = Regex.Split(stringSource, "fmt__skill_name\"");
                                            if (strarr_skill.Count() >= 2)
                                            {
                                                foreach (string item7 in strarr_skill)
                                                {
                                                    try
                                                    {
                                                        if (!item7.Contains("!DOCTYPE html"))
                                                        {
                                                            try
                                                            {
                                                                string Grp = item7.Substring(item7.IndexOf("<"), (item7.IndexOf(">", item7.IndexOf("<")) - item7.IndexOf("<"))).Replace("<", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                                                                checkgrplist.Add(Grp);
                                                                checkgrplist.Distinct().ToList();
                                                            }
                                                            catch { }
                                                        }

                                                    }
                                                    catch { }
                                                }

                                                foreach (string item8 in checkgrplist)
                                                {
                                                    if (string.IsNullOrEmpty(Skill))
                                                    {
                                                        Skill = item8;
                                                    }
                                                    else
                                                    {
                                                        Skill = Skill + "  -  " + item8;
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

                                                        foreach (string item9 in checkgrplist)
                                                        {
                                                            if (string.IsNullOrEmpty(Skill))
                                                            {
                                                                Skill = item9;
                                                            }
                                                            else
                                                            {
                                                                Skill = Skill + "  -  " + item9;
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

                                        #endregion

                                        #region Pasttitle and All Company Summary
                                        string[] pasttitles = Regex.Split(stringSource, "company_name");
                                        string pstTitlesitem = string.Empty;
                                        string pstDescCompitem = string.Empty;
                                        LDS_PastTitles = string.Empty;
                                        pasttitles = pasttitles.Skip(1).ToArray();
                                        foreach (string item10 in pasttitles)
                                        {
                                            if (item10.Contains("positionId"))
                                            {
                                                try
                                                {
                                                    int startindex = item10.IndexOf(":");
                                                    if (startindex > 0)
                                                    {
                                                        string start = item10.Substring(startindex).Replace(":\"", "");
                                                        int endindex = start.IndexOf("\",");
                                                        string end = start.Substring(0, endindex);
                                                        pstTitlesitem = end.Replace(",", ";");
                                                    }

                                                    if (string.IsNullOrEmpty(LDS_PastTitles))
                                                    {
                                                        LDS_PastTitles = pstTitlesitem;
                                                    }
                                                    else
                                                    {
                                                        LDS_PastTitles = LDS_PastTitles + "  :  " + pstTitlesitem;
                                                    }


                                                    int startindex1 = item10.IndexOf("summary_lb\":\"");
                                                    if (startindex > 0)
                                                    {
                                                        string start1 = item10.Substring(startindex1).Replace("summary_lb\":\"", "");
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
                                                        pstDescCompitem = end1.Replace(",", ";").Replace("u002d", "-").Replace("<br>", string.Empty).Replace("\n\"", string.Empty);

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

                                        #region FullUrl
                                        try
                                        {
                                            string[] UrlFull = System.Text.RegularExpressions.Regex.Split(FrnAcceptUrL, "&authType");
                                            LDS_UserProfileLink = UrlFull[0];

                                            LDS_UserProfileLink = UrlFull[0];
                                            //  LDS_UserProfileLink = stringSource.Substring(stringSource.IndexOf("canonicalUrl\":"), (stringSource.IndexOf(",", stringSource.IndexOf("canonicalUrl\":")) - stringSource.IndexOf("canonicalUrl\":"))).Replace("canonicalUrl\":", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty).Trim();
                                        }
                                        catch { }
                                        #endregion

                                        LDS_LoginID = objLinkedinUser.username;

                                        if (string.IsNullOrEmpty(firstname))
                                        {
                                            firstname = "Linkedin Member";
                                        }

                                        LDS_BackGround_Summary = LDS_BackGround_Summary.Replace("\n", "").Replace("-", "").Replace("d", "").Replace("&#x2022", "").Replace(";", "").Replace("\n", "").Replace(",", "").Replace("&#x201", "").Trim();

                                        LDS_Desc_AllComp = "NA";
                                        Skill = "NA";

                                        string LDS_FinalData = TypeOfProfile + "," + LDS_UserProfileLink + "," + firstname + "," + lastname + "," + Company.Replace(",", ";") + "," + titlecurrent.Replace(",", ";") + "," + companycurrent.Replace(",", ";") + "," + LDS_Desc_AllComp + "," + LDS_BackGround_Summary.Replace(",", ";") + "," + Connection.Replace(",", ";") + "," + recomandation.Replace(",", string.Empty) + "," + Skill.Replace(",", ";") + "," + LDS_Experience.Replace(",", string.Empty) + "," + EducationCollection.Replace(",", ";") + "," + groupscollectin.Replace(",", ";") + "," + USERemail.Replace(",", ";") + "," + LDS_UserContact.Replace(",", ";") + "," + LDS_PastTitles + "," + AllComapny.Replace(",", ";") + "," + country.Replace(",", ";") + "," + location.Replace(",", ";") + "," + Industry.Replace(",", ";") + "," + Website.Replace(",", ";") + "," + LDS_LoginID.Replace(",", ";") + ",";
                                      //  AppFileHelper.AddingLinkedInGroupMemberDataToCSVFile(LDS_FinalData, SearchCriteria.FileName);

                                    }
                                    catch { }
                                }

                            }
                            catch { }
                        }
                    }
                    #endregion

                    foreach (var GrpUser in RgxGroupData)
                    {
                        try
                        {
                            if (GrpUser.Contains("member"))
                            {
                                if (GrpUser.Contains("title=\"YOU") || GrpUser.Contains("<!DOCTYPE html>"))
                                {
                                    if (GrpUser.Contains("title=\"YOU"))
                                    {

                                    }
                                    continue;
                                }
                                string profileUrl = string.Empty;
                                try
                                {
                                    profileUrl = Utils.getBetween(GrpUser, "<a href=\"", "\">");
                                }
                                catch
                                {
                                }
                                try
                                {
                                    //data-li-fullName="Kashish Arora">Send message</a>

                                    int startindex = GrpUser.IndexOf("fullName=");
                                    if (startindex > 0)
                                    {
                                        endName = string.Empty;
                                        string start = GrpUser.Substring(startindex);
                                        int endIndex = start.IndexOf(">Send message<");
                                        if (endIndex == -1)
                                        {
                                            endIndex = start.IndexOf(">");
                                        }
                                        endName = start.Substring(0, endIndex).Replace("fullName=", string.Empty).Replace("'", string.Empty).Replace(",", string.Empty).Replace("/", string.Empty).Replace("\"", string.Empty).Replace("&amp;", "&").Replace("&quot;", "'").Replace("tracking=anetppl_sendmsg", string.Empty).Replace("tracking=anetppl_invite", string.Empty).Replace("\\u00e9", "é").Trim();
                                    }
                                    else
                                    {
                                        endName = string.Empty;
                                        int startindex1 = GrpUser.IndexOf("alt=");
                                        string start = GrpUser.Substring(startindex1).Replace("alt=\"", "");
                                        int endIndex = start.IndexOf("\"");
                                        try
                                        {
                                            endName = start.Substring(0, endIndex).Replace("alt=", string.Empty).Replace("'", string.Empty).Replace(",", string.Empty).Replace(">", string.Empty).Replace("\"", string.Empty).Replace("&amp;", "&").Replace("&quot;", "'").Replace("tracking=anetppl_sendmsg", string.Empty).Replace("tracking=anetppl_invite", string.Empty).Replace("\\u00e9", "é").Trim();
                                        }
                                        catch { }
                                        try
                                        {
                                            if (string.IsNullOrEmpty(endName))
                                            {
                                                endName = start.Substring(start.IndexOf("alt="), start.IndexOf("class=", start.IndexOf("alt=")) - start.IndexOf("alt=")).Replace("alt=", string.Empty).Replace("alt=", string.Empty).Replace("\"", string.Empty).Replace("&quot;", "'").Replace("tracking=anetppl_sendmsg", string.Empty).Replace("tracking=anetppl_invite", string.Empty).Replace("\\u00e9", "é").Trim();
                                            }

                                        }
                                        catch { }
                                    }
                                }
                                catch
                                {

                                }

                                //Deegree connection
                                try
                                {
                                    int startindex = GrpUser.IndexOf("<span class=\"degree-icon\">");
                                    if (startindex > 0)
                                    {
                                        DeegreeConn = string.Empty;
                                        string start = GrpUser.Substring(startindex);
                                        int endIndex = start.IndexOf("<sup>");
                                        DeegreeConn = start.Substring(0, endIndex).Replace("<span class=\"degree-icon\">", string.Empty);

                                        if (DeegreeConn == "1")
                                        {
                                            DeegreeConn = DeegreeConn + "st";
                                        }
                                        else if (DeegreeConn == "2")
                                        {
                                            DeegreeConn = DeegreeConn + "nd";
                                        }
                                        else if (DeegreeConn == "3")
                                        {
                                            DeegreeConn = DeegreeConn + "rd";
                                        }
                                    }
                                    else
                                    {
                                        startindex = GrpUser.IndexOf("span class=\"degree-icon group\">");
                                        DeegreeConn = string.Empty;

                                        if (startindex > 0)
                                        {
                                            DeegreeConn = string.Empty;
                                            string start = GrpUser.Substring(startindex);
                                            int endIndex = start.IndexOf("</span>");
                                            DeegreeConn = start.Substring(0, endIndex).Replace("span class=\"degree-icon group\">", string.Empty);

                                        }

                                        if (DeegreeConn == string.Empty)
                                        {

                                            DeegreeConn = "3rd";
                                        }
                                    }

                                }

                                catch { }

                                try
                                {
                                    int startindex2 = GrpUser.IndexOf("memberId=");
                                    if (startindex2 > 0)
                                    {
                                        endKey = string.Empty;
                                        string start1 = GrpUser.Substring(startindex2);
                                        int endIndex1 = start1.IndexOf("data-li-fullName=");
                                        endKey = start1.Substring(0, endIndex1).Replace("memberId=", string.Empty).Replace("'", string.Empty).Replace(",", string.Empty).Replace("/", string.Empty).Replace("\"", string.Empty).Trim();
                                    }
                                    else
                                    {
                                        endKey = string.Empty;
                                        int startindex3 = GrpUser.IndexOf("member-");
                                        string start1 = GrpUser.Substring(startindex3);
                                        int endIndex1 = start1.IndexOf(">");
                                        endKey = start1.Substring(0, endIndex1).Replace("member-", string.Empty).Replace("'", string.Empty).Replace(",", string.Empty).Replace(">", string.Empty).Replace("\"", string.Empty).Trim();
                                    }
                                }
                                catch
                                {

                                }

                                try
                                {
                                    GroupSpecMem.Add(endKey, " [" + GroupName.Replace(",", string.Empty) + " ] " + endName + " (" + DeegreeConn.Replace(",", string.Empty) + ")");
                                    string item = UserID + "," + GroupName.Replace(",", string.Empty) + "," + group_Url.Replace("'", "").Replace("\r", "").Replace(" ", "").Replace("\n", " ") + "," + endName + "," + profileUrl.Replace("'", "").Replace("\r", "").Replace(" ", "").Replace("\n", " ") + "," + DeegreeConn.Replace(",", string.Empty);
                                   // AddingLinkedInDataToCSVFile1(item);
                                    if (WithGroupSearch == true)
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Added Group Member : " + endName + " (" + DeegreeConn + ") with Search keyword : " + SearchKeyword + " ]");
                                        //  objfrmMain.AddLoggerLoggerForGroupScraper("[ " + DateTime.Now + " ] => [ Added Group Member : " + endName + " (" + DeegreeConn + ") with Search keyword : " + SearchKeyword + " ]");
                                    }
                                    else
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Added Group Member : " + endName + " ]");
                                    }
                                }
                                catch { }
                            }
                            else
                            {

                            }
                        }
                        catch { }
                    }



                    return GroupSpecMem;
                }

                #region new code_11_26

                if (GroupSpecMem.Count <= 0)
                {
                    List<string> lstNewMembers = new List<string>();
                    bool gotScrapeUrl = false;
                    int startPage = 10;

                  string  url = "https://www.linkedin.com/communities-api/v1/memberships/community/" + groupId + "?projection=MINI&count=20&start=0";
                GetItemToScrapeUrls:
                    List<string> lstitemForScrap = new List<string>();
             //   pageSource = HttpHelper.getHtmlfromUrl(new Uri(url), "https://www.linkedin.com/groups/" + groupId + "", "", "");


                #region Cookie settings
              //  string CookieDaata = "bcookie=\"v=2&e456608f-798a-4a30-8111-4abf66a2a2d2\"; bscookie=\"v=1&201504281150112c2cb966-888b-44d4-8fe3-2bed850fc88eAQH5LeTYIub6nyWgnwTdMWl9XSSvnwsJ\"; visit=\"v=1&M\"; _chartbeat2=vnNg6xd-hkBwUPD5.1439389439450.1439994463832.10000001; VID=V_2015_08_19_07_4913; ELOQUA=GUID=28AF04F863BF45A78F3AA16B2C88EFBC; BKUT=1439994600; sessionid=\"eyJkamFuZ29fdGltZXpvbmUiOiJBc2lhL0tvbGthdGEifQ:1a1DwL:Z1TtuNNPyqGqvT1Pix66crqU9RA\"; csrftoken=fGdgb242nAkaqdDI0T72TptnHK9Fq9MM; _ga=GA1.2.1307723840.1448360766; L1c=5eba1285; oz_props_fetch_size1_125011638=9; wutan=YjukjFsDQIHEoN/VG0Nwwa8w7dzs308V6oAfSeVHCY8=; L1e=4a2dae59; oz_props_fetch_size1_394473043=5; oz_props_fetch_size1_394482147=1; sl=\"v=1&y7Ik2\"; li_at=AQEDAReDUeMB8eJnAAABUXFNV2wAAAFRcbs0bEsAw1uzWq20X7lGOtdsUgxs31vuFr1q74cnHL82GBdRUIVjbPbKccr0gJVqHj9mqczMKjWVA417H7gRrQeFQARUNssZqNTx0aZZR1hyEmdxvf4iYbF1; liap=true; JSESSIONID=\"ajax:7383772031323678847\"; share_setting=PUBLIC; sdsc=1%3A1SZM1shxDNbLt36wZwCgPgvN58iw%3D; _lipt=0_0MPCZKVCxoBpoHR17D_hDUfM2NiX0FG-oN1B8U9sDtgK_Ydwh1Gw2TGDG8LSwwC-BWGgocnwaixcQM_gvLFHZRCDaMOR9vUNoFFU7Pv4sekUOXFI9ys8jl6QfmKapvABKOlY89bVv5mqx58JvuF7NyP6lyWIckctWO6k6UG0iu1y6ABeq4BLdi4Hx_r4fhtEsbUgeBtzZHE00OAoBcpWzyBorypcrsvu10vjpfQ4JrlkZcgesHUL0zqxRmg6dwbDlfWnQT0C4hwhjEdaTEVNTp; lidc=\"b=LB47:g=319:u=13:i=1449305064:t=1449390866:s=AQG1xLWNXv_mwQpQa_XpXXydwVB0xggq\"; lang=\"v=2&lang=en-us\"; RT=s=1449305300845&r=https%3A%2F%2Fwww.linkedin.com%2Fgroups%2F47307%2Fmembers";


              //  string[] CookieDaatalst = Regex.Split(CookieDaata, ";");
              //  HttpHelper.gCookies = new System.Net.CookieCollection();
              //  System.Net.CookieCollection CookieCollection =   new System.Net.CookieCollection();
                
              //  foreach (string itemlst in CookieDaatalst)
              //  {

              //      try
              //      {
                        
              //          string[] namevalue = Regex.Split(itemlst, "=");

              //          string name = namevalue[0].Replace(" ","");
              //          string value = namevalue[1].Replace(" ", "");

              //          string Domain = "linkedin.com";

              //          System.Net.Cookie cookie = new System.Net.Cookie();
              //          cookie.Name = name;
              //          cookie.Value = value;
              //          cookie.Domain = Domain;
              //          bool IsFound = false;
              //          try
              //          {
                         
              //              foreach (System.Net.Cookie cokieitem in HttpHelper.gCookies)
              //              {

              //                  if (cookie == cokieitem)
              //                  {
              //                      IsFound = true;
              //                      break;
              //                  }
              //              }
              //              if (!IsFound)
              //              {
              //                  HttpHelper.gCookies.Add(cookie);
              //              }
              //          }
              //          catch { };
              //      }
              //      catch { };
              //  }

                #endregion


                pageSource = HttpHelper.getHtmlfromUrl(new Uri(url), "https://www.linkedin.com/groups/" + groupId + "", "", "");
                    string[] arr = Regex.Split(pageSource, ".jpg\",\"name\":");
                    foreach (var itemArr in arr)
                    {
                        if (itemArr.Contains("{\"data\":[{\"profileUrl\""))
                        {
                            continue;
                        }

                        string memberName = string.Empty;
                        string memberId = string.Empty;

                        memberName = Utils.getBetween(itemArr, "\"", "\",\"");
                        memberId = Utils.getBetween(itemArr, "id\":\"", "\",\"");
                        GroupName = GroupName.Replace(",", string.Empty);
                        try
                        {
                            if (!memberName.Contains("data\":[{\""))
                            {
                                GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Added Group Member : " + memberName + " ]");
                                GroupSpecMem.Add(memberId, " [" + GroupName + " ] " + memberName);
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex);
                        }


                        //string name = Utils.getBetween(itemArr, "\"", ",").Replace("\"", "");
                        //string memberId = Utils.getBetween(itemArr, "nonIterableMemberId\":", ",").Replace("\"", "");
                        string itemToScrapUrl = "https://www.linkedin.com/profile/view?id=" + memberId + "&authType=name&authToken=2ZLd";
                        if (!lstitemForScrap.Contains(itemToScrapUrl))
                        {
                            gotScrapeUrl = true;
                            lstitemForScrap.Add(itemToScrapUrl);
                        }
                    }
                    if (gotScrapeUrl)
                    {
                        //foreach (var itemForScrap in lstitemForScrap)
                        //{
                        //    if (!lstNewMembers.Contains(itemForScrap))
                        //    {
                        //        if (!obj_ClsScrapGroupMember.CrawlingLinkedInPage(itemForScrap, ref HttpHelper))
                        //        {
                        //            obj_ClsScrapGroupMember.CrawlingPageDataSource(itemForScrap, ref HttpHelper);
                        //        }
                        //        lstNewMembers.Add(itemForScrap);
                        //    }
                        //}

                        startPage = startPage + 10;
                        url = "https://www.linkedin.com/communities-api/v1/memberships/community/" + groupId + "?membershipStatus=MEMBER&start=" + startPage + "&projection=FULL";
                        gotScrapeUrl = false;
                        if (startPage <= 500)
                        {
                            goto GetItemToScrapeUrls;
                        }

                    }



                }
                #endregion
            }
            catch (Exception ex)
            {
            }
            return GroupSpecMem;
        }

        #endregion

        public static long UnixTimestampFromDateTime(DateTime date)
        {
            long unixTimestamp = date.Ticks - new DateTime(1970, 1, 1).Ticks;
            unixTimestamp /= TimeSpan.TicksPerSecond;
            return unixTimestamp;
        }

        public void messageSending()
        {
            foreach (KeyValuePair<string, LinkedinUser> item in LDGlobals.loadedAccountsDictionary)
            {
                if (selectAllGroup == false)
                {
                    selected_group = selected_group.Split(':')[0];
                    if(selected_group==item.Key)
                    {
                        PostMessageGroupMembers(new object[] { item });
                    }

                }
                else
                {
                    PostMessageGroupMembers(new object[] { item });

                }
            }


        }

        #region For -- PostMessageGroupMembers --

        public void PostMessageGroupMembers(object parameter)
        {
            try
            {
                try
                {
                    if (IsStop)
                    {
                        return;
                    }

                    if (!IsStop)
                    {
                        lstGroupMessageThread.Add(Thread.CurrentThread);
                        lstGroupMessageThread.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error >>> " + ex.StackTrace);
                }

                Array paramsArray = new object[1];

                paramsArray = (Array)parameter;

                KeyValuePair<string, LinkedinUser> item = (KeyValuePair<string, LinkedinUser>)paramsArray.GetValue(0);
                LinkedinUser objLinkedinUser = item.Value;

                GlobusHttpHelper HttpHelper = objLinkedinUser.globusHttpHelper;

                GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Sending Message From Account : " + item.Key + " ]");
                string selectedusername = item.Key;
                try
                {
                    if (!objLinkedinUser.isloggedin)
                    {
                        AccountManager objAccountManager = new AccountManager();
                        objAccountManager.LoginHttpHelper(ref  objLinkedinUser);
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info("EXCeption ex : " + ex);
                }

                try
                {
                    if (objLinkedinUser.isloggedin)
                    {
                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Getting Contacts to Send Message ]");
                        List<string> SelectedItem = new List<string>();
                        string Userid = string.Empty;
                        if (selectAllGroup == false)
                        {
                            Userid = selected_group;
                        }
                        string FromEmailId = FromEmailCodeMsgGroupMem(ref HttpHelper, SpeGroupId);
                        string FromEmailName = FromName(ref HttpHelper);

                        FromemailId = FromEmailId;
                        FromEmailNam = FromEmailName;

                        Dictionary<string, string> SelectedGroupMem = new Dictionary<string, string>();
                        foreach (KeyValuePair<string, Dictionary<string, string>> contacts in GrpMemNameWithGid)
                        {
                            if (contacts.Key.Trim() == item.Key.Trim())
                            {
                                foreach (KeyValuePair<string, string> Details in contacts.Value)
                                {
                                    foreach (string itemChecked in selectedMembers)
                                    {
                                        if (itemChecked == Details.Value)
                                        {
                                            try
                                            {
                                                string id = Details.Key;
                                                SelectedGroupMem.Add(id, Details.Value);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        string msgSub = messageSubject;
                        string msgBody = messageBody;


                        if (IsSpintaxUsed)
                        {
                            try
                            {
                                msgSub = GrpMemSubjectlist[RandomNumberGenerator.GenerateRandom(0, GrpMemSubjectlist.Count - 1)];
                                msgBody = GrpMemMessagelist[RandomNumberGenerator.GenerateRandom(0, GrpMemMessagelist.Count - 1)];
                            }
                            catch
                            {
                            }
                        }


                      
                        try
                        {
                            //Groups.MessageGroupMember obj_MessageGroupMember = new Groups.MessageGroupMember();
                            //obj_MessageGroupMember.logger.addToLogger += new EventHandler(GroupMemMessage_addToLogger);
                            PostFinalMsgGroupMember_1By1(ref HttpHelper, SelectedGroupMem, GrpMemSubjectlist, GrpMemMessagelist, msgSub, msgBody, selectedusername, FromemailId, FromEmailNam, selected_group, SpeGroupId, IsMessagingWithTag, IsSpintaxUsed, minDelay, maxDelay, preventMsgSameGroup, preventMsgWithoutGroup, preventMsgGlobal);
                          //  obj_MessageGroupMember.logger.addToLogger -= new EventHandler(GroupMemMessage_addToLogger);
                        }
                        catch
                        {
                        }
                        finally
                        {
                            //if (btnGroupMessage.InvokeRequired)
                            //{
                               // btnGroupMessage.Invoke(new MethodInvoker(delegate
                                //{
                                   GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED ]");
                                    GlobusLogHelper.log.Info("----------------------------------------------------------------------------------------------------------------------------------------");
                                   // btnGroupMessage.Cursor = Cursors.Default;
                               // }));
                           // }
                        }
                    }
                    else if (!objLinkedinUser.isloggedin)
                    {
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info("Exception ex :" + ex);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Exceptin : " + ex);
            }
        }

        #endregion

        #region PostFinalMsgGroupMember_1By1
        public void PostFinalMsgGroupMember_1By1(ref GlobusHttpHelper HttpHelper, Dictionary<string, string> SlectedContacts, List<string> GrpMemSubjectlist, List<string> GrpMemMessagelist, string msg, string body, string UserName, string FromemailId, string FromEmailNam, string SelectedGrpName, string grpId, bool mesg_with_tag, bool msg_spintaxt, int mindelay, int maxdelay, bool preventMsgSameGroup, bool preventMsgWithoutGroup, bool preventMsgGlobal)
        {
            try
            {
               // MsgGroupMemDbManager objMsgGroupMemDbMgr = new MsgGroupMemDbManager();

                string postdata = string.Empty;
                string postUrl = string.Empty;
                string ResLogin = string.Empty;
                string csrfToken = string.Empty;
                string sourceAlias = string.Empty;
                string ReturnString = string.Empty;
                string PostMsgSubject = string.Empty;
                string PostMsgBody = string.Empty;
                string FString = string.Empty;

                try
                {


                    string MessageText = string.Empty;
                    string PostedMessage = string.Empty;
                    string senderEmail = string.Empty;
                    string getComposeData = HttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com/inbox/compose"));


                    if (!getComposeData.Contains("There was an error with the file upload. Please try again later."))
                    {
                        #region MyRegion
                        try
                        {
                            int startindex = getComposeData.IndexOf("\"senderEmail\" value=\"");
                            if (startindex < 0)
                            {
                                startindex = getComposeData.IndexOf("\"senderEmail\",\"value\":\"");
                            }
                            string start = getComposeData.Substring(startindex).Replace("\"senderEmail\" value=\"", string.Empty).Replace("\"senderEmail\",\"value\":\"", string.Empty);
                            int endindex = start.IndexOf("\"/>");
                            if (endindex < 0)
                            {
                                endindex = start.IndexOf("\",\"");
                            }
                            string end = start.Substring(0, endindex).Replace("\"/>", string.Empty).Replace("\",\"", string.Empty);
                            senderEmail = end.Trim();
                        }
                        catch (Exception ex)
                        {
                            senderEmail = Utils.getBetween(getComposeData, "<", ">");
                        }
                        string pageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com/home?trk=hb_tab_home_top"));
                        if (pageSource.Contains("csrfToken"))
                        {
                            try
                            {
                                csrfToken = pageSource.Substring(pageSource.IndexOf("csrfToken"), 50);
                                string[] Arr = csrfToken.Split('<');
                                csrfToken = Arr[0];
                                csrfToken = csrfToken.Replace("csrfToken", "").Replace("\"", string.Empty).Replace("value", string.Empty).Replace("cs", string.Empty).Replace("id", string.Empty).Replace("=", string.Empty).Replace("\n", string.Empty).Replace(">", string.Empty).Replace("<script typ", string.Empty);
                                csrfToken = csrfToken.Trim();
                            }
                            catch (Exception ex)
                            {

                            }

                        }

                        if (pageSource.Contains("sourceAlias"))
                        {
                            try
                            {
                                sourceAlias = pageSource.Substring(pageSource.IndexOf("sourceAlias"), 100);
                                string[] Arr = sourceAlias.Split('"');
                                sourceAlias = Arr[2];
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        if (pageSource.Contains("goback="))
                        {
                            try
                            {
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        foreach (KeyValuePair<string, string> itemChecked in SlectedContacts)
                        {
                            try
                            {
                                DataSet ds = new DataSet();
                                DataSet ds_bList = new DataSet();
                                string ContactName = string.Empty;
                                string Nstring = string.Empty;
                                string connId = string.Empty;
                                string FName = string.Empty;
                                string Lname = string.Empty;
                                string tempBody = string.Empty;
                                string tempsubject = string.Empty;
                                string n_ame1 = string.Empty;

                                //grpId = itemChecked.Key.ToString();

                                try
                                {
                                    // FName = itemChecked.Value.Split(' ')[0];
                                    // Lname = itemChecked.Value.Split(' ')[1];
                                    try
                                    {
                                        n_ame1 = itemChecked.Value.Split(']')[1].Trim(); ;
                                    }
                                    catch
                                    { }
                                    if (string.IsNullOrEmpty(n_ame1))
                                    {
                                        try
                                        {
                                            n_ame1 = itemChecked.Value;
                                        }
                                        catch
                                        { }
                                    }
                                    string[] n_ame = Regex.Split(n_ame1, " ");
                                    FName = " " + n_ame[0];
                                    Lname = n_ame[1];

                                    if (!string.IsNullOrEmpty(n_ame[2]))
                                    {
                                        Lname = Lname + n_ame[2];
                                    }
                                    if (!string.IsNullOrEmpty(n_ame[3]))
                                    {
                                        Lname = Lname + n_ame[3];
                                    }
                                    if (!string.IsNullOrEmpty(n_ame[4]))
                                    {
                                        Lname = Lname + n_ame[4];
                                    }
                                    if (!string.IsNullOrEmpty(n_ame[5]))
                                    {
                                        Lname = Lname + n_ame[5];
                                    }
                                }
                                catch (Exception ex)
                                {
                                }

                                try
                                {
                                    ContactName = FName + " " + Lname;
                                    ContactName = ContactName.Replace("%20", " ");
                                }
                                catch { }

                                
                                GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Adding Contact : " + ContactName + " ]");

                                string ToCd = itemChecked.Key;
                                List<string> AddAllString = new List<string>();

                                if (FString == string.Empty)
                                {
                                    string CompString = "{" + "\"" + "_" + ToCd.Trim() + "\"" + ":" + "{" + "\"" + "memberId" + "\"" + ":" + "\"" + ToCd.Trim() + "\"" + "," + "\"" + "firstName" + "\"" + ":" + "\"" + FName + "\"" + "," + "\"" + "lastName" + "\"" + ":" + "\"" + Lname + "\"" + "}";
                                    FString = CompString;
                                }
                                else
                                {
                                    string CompString = "\"" + "_" + ToCd.Trim() + "\"" + ":" + "{" + "\"" + "memberId" + "\"" + ":" + "\"" + ToCd.Trim() + "\"" + "," + "\"" + "firstName" + "\"" + ":" + "\"" + FName + "\"" + "," + "\"" + "lastName" + "\"" + ":" + "\"" + Lname + "\"" + "}";
                                    FString = CompString;
                                }

                                if (Nstring == string.Empty)
                                {
                                    Nstring = FString;
                                    connId = ToCd;
                                }
                                else
                                {
                                    Nstring += "," + FString;
                                    connId += " " + ToCd;
                                }

                                Nstring += "}";
                                tempsubject = msg;
                                try
                                {
                                    string PostMessage = string.Empty;
                                    string ResponseStatusMsg = string.Empty;

                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Message Sending Process Running.. ]");
                                    if (mesg_with_tag == true)
                                    {
                                        tempsubject = msg;
                                    }
                                    if (msg_spintaxt == true)
                                    {
                                        try
                                        {
                                            msg = GrpMemSubjectlist[RandomNumberGenerator.GenerateRandom(0, GrpMemSubjectlist.Count - 1)];
                                            body = GrpMemMessagelist[RandomNumberGenerator.GenerateRandom(0, GrpMemMessagelist.Count - 1)];
                                        }
                                        catch
                                        {
                                        }
                                    }

                                    if (mesg_with_tag == true)
                                    {
                                        if (string.IsNullOrEmpty(FName))
                                        {
                                            tempBody = body.Replace("<Insert Name>", ContactName);
                                        }
                                        else
                                        {
                                          //  tempBody = GlobusSpinHelper.spinLargeText(new Random(), body);

                                           // if (lstSubjectReuse.Count == GrpMemSubjectlist.Count)
                                           // {
                                          //      lstSubjectReuse.Clear();
                                           // }
                                            //foreach (var itemSubject in GrpMemSubjectlist)
                                            //{
                                            //    if (string.IsNullOrEmpty(TemporarySubject))
                                            //    {
                                            //        TemporarySubject = itemSubject;
                                            //        tempsubject = itemSubject;
                                            //        lstSubjectReuse.Add(itemSubject);
                                            //        break;
                                            //    }
                                            //    else if (!lstSubjectReuse.Contains(itemSubject))
                                            //    {
                                            //        TemporarySubject = itemSubject;
                                            //        tempsubject = itemSubject;
                                            //        lstSubjectReuse.Add(itemSubject);
                                            //        break;
                                            //    }
                                            //    else
                                            //    {
                                            //        continue;
                                            //    }
                                            //}

                                            tempBody = tempBody.Replace("<Insert Name>", FName);

                                            tempsubject = tempsubject.Replace("<Insert Name>", FName);
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(FName))
                                        {
                                            tempBody = body.Replace("<Insert Name>", ContactName);
                                        }
                                        else
                                        {
                                            tempBody = body.Replace("<Insert Name>", FName);
                                        }
                                    }

                                    if (SelectedGrpName.Contains(":"))
                                    {
                                        try
                                        {
                                            string[] arrSelectedGrpName = Regex.Split(SelectedGrpName, ":");
                                            if (arrSelectedGrpName.Length > 1)
                                            {
                                                SelectedGrpName = arrSelectedGrpName[1];
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }




                                    if (mesg_with_tag == true)
                                    {
                                        tempBody = tempBody.Replace("<Insert Group>", SelectedGrpName);
                                        tempBody = tempBody.Replace("<Insert From Email>", FromEmailNam);
                                        tempBody = tempBody.Replace("<Insert Name>", string.Empty).Replace("<Insert Group>", string.Empty).Replace("<Insert From Email>", string.Empty);
                                    }
                                    else
                                    {
                                        tempBody = tempBody.Replace("<Insert Group>", SelectedGrpName);
                                        tempBody = tempBody.Replace("<Insert From Email>", FromEmailNam);
                                        tempBody = tempBody.Replace("<Insert Name>", string.Empty).Replace("<Insert Group>", string.Empty).Replace("<Insert From Email>", string.Empty);
                                    }

                                    //Check BlackListed Accounts
                                    try
                                    {
                                        string Querystring = "Select ProfileID From tb_BlackListAccount Where ProfileID ='" + itemChecked.Key + "'";
                                        ds_bList = DataBaseHandler.SelectQuery(Querystring, "tb_BlackListAccount");
                                    }
                                    catch { }


                                    if (preventMsgSameGroup)
                                    {
                                        try
                                        {
                                            string Querystring = "Select MsgFrom,MsgToId,MsgTo,MsgGroupId,MsgGroupName,MsgSubject,MsgBody From tb_ManageMsgGroupMem Where MsgFrom ='" + UserName + "' and MsgGroupId = " + grpId + " and MsgToId = " + connId + "";
                                            ds = DataBaseHandler.SelectQuery(Querystring, "tb_ManageMsgGroupMem");
                                        }
                                        catch { }
                                    }

                                    if (preventMsgWithoutGroup)
                                    {
                                        try
                                        {
                                            string Querystring = "Select MsgFrom,MsgToId,MsgTo,MsgGroupId,MsgGroupName,MsgSubject,MsgBody From tb_ManageMsgGroupMem Where MsgFrom ='" + UserName + "' and MsgToId = " + connId + "";
                                            ds = DataBaseHandler.SelectQuery(Querystring, "tb_ManageMsgGroupMem");
                                        }
                                        catch { }
                                    }
                                    if (preventMsgGlobal)
                                    {
                                        try
                                        {
                                            string Querystring = "Select MsgFrom,MsgToId,MsgTo,MsgGroupId,MsgGroupName,MsgSubject,MsgBody From tb_ManageMsgGroupMem Where MsgToId = " + connId + "";
                                            ds = DataBaseHandler.SelectQuery(Querystring, "tb_ManageMsgGroupMem");
                                        }
                                        catch { }
                                    }

                                    try
                                    {

                                        if (ds.Tables.Count > 0)
                                        {
                                            if (ds.Tables[0].Rows.Count > 0)
                                            {

                                                PostMessage = "";
                                                ResponseStatusMsg = "Already Sent";

                                            }
                                            else
                                            {
                                                if (ds_bList.Tables.Count > 0 && ds_bList.Tables[0].Rows.Count > 0)
                                                {
                                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ User: " + ContactName + " is Added BlackListed List For Send Messages Pls Check ]");
                                                    ResponseStatusMsg = "BlackListed";
                                                }
                                                else
                                                {

                                                    try
                                                    {
                                                        string Url_compose = "https://www.linkedin.com/inbox/#compose?connId=" + ToCd + "&groupId=" + grpId;
                                                        string Responce = HttpHelper.getHtmlfromUrl(new Uri(Url_compose));
                                                        {

                                                            string PostUrlsssss = "https://www.linkedin.com/inbox/mailbox/message/send";


                                                            string PostDataFinal1 = "senderEmail=645883950&ccInput=&subject=" + Uri.EscapeDataString(tempsubject.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&persist=true&showRecipients=showRecipients&isReply=&isForward=&itemId=&recipients=" + ToCd + "&recipientNames=%5B%7B%22memberId%22%3A" + ToCd + "%2C%22fullName%22%3A%22" + ContactName + "%22%7D%5D&groupId=" + grpId + "&csrfToken=" + csrfToken + "&sourceAlias=0_3mbGc9okCQbybxvc2A5Vz5&submit=Envoyer";
                                                            ResponseStatusMsg = HttpHelper.postFormData(new Uri(PostUrlsssss), PostDataFinal1);
                                                        }


                                                    }
                                                    catch (Exception ex)
                                                    {
                                                    }
                                                    if (ResponseStatusMsg.Contains("upload_error") && ResponseStatusMsg.Contains("There was an error with the file upload. Please try again later"))
                                                    {
                                                        PostMessage = "csrfToken=" + csrfToken + "&subject=" + Uri.EscapeDataString(msg.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&submit=Send+Message&fromName=" + Uri.EscapeDataString(FromEmailNam) + "&showRecipeints=showRecipeints&fromEmail=" + FromemailId + "&connectionIds=" + connId + "&connectionNames=&allowEditRcpts=true&addMoreRcpts=false&openSocialAppBodySuffix=&st=&viewerDestinationUrl=&contentType=MEBC&groupID=" + grpId + "";
                                                        ResponseStatusMsg = HttpHelper.postFormData(new Uri("https://www.linkedin.com/groupMsg"), PostMessage);
                                                    }


                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (ds_bList.Tables.Count > 0)
                                            {
                                                if (ds_bList.Tables[0].Rows.Count > 0)
                                                {

                                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ User: " + ContactName + " is Added BlackListed List For Send Messages Pls Check ]");
                                                    ResponseStatusMsg = "BlackListed";
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        string Url_compose = "https://www.linkedin.com/inbox/#compose?connId=" + ToCd + "&groupId=" + grpId;
                                                        string Responce = HttpHelper.getHtmlfromUrl(new Uri(Url_compose));
                                                        {

                                                            string PostUrlsssss = "https://www.linkedin.com/inbox/mailbox/message/send";


                                                            string PostDataFinal1 = "senderEmail=645883950&ccInput=&subject=" + Uri.EscapeDataString(tempsubject.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&persist=true&showRecipients=showRecipients&isReply=&isForward=&itemId=&recipients=" + ToCd + "&recipientNames=%5B%7B%22memberId%22%3A" + ToCd + "%2C%22fullName%22%3A%22" + ContactName + "%22%7D%5D&groupId=" + grpId + "&csrfToken=" + csrfToken + "&sourceAlias=0_3mbGc9okCQbybxvc2A5Vz5&submit=Envoyer";
                                                            ResponseStatusMsg = HttpHelper.postFormData(new Uri(PostUrlsssss), PostDataFinal1);
                                                        }


                                                    }
                                                    catch (Exception ex)
                                                    {
                                                    }
                                                    if (ResponseStatusMsg.Contains("upload_error") && ResponseStatusMsg.Contains("There was an error with the file upload. Please try again later"))
                                                    {
                                                        PostMessage = "csrfToken=" + csrfToken + "&subject=" + Uri.EscapeDataString(msg.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&submit=Send+Message&fromName=" + Uri.EscapeDataString(FromEmailNam) + "&showRecipeints=showRecipeints&fromEmail=" + FromemailId + "&connectionIds=" + connId + "&connectionNames=&allowEditRcpts=true&addMoreRcpts=false&openSocialAppBodySuffix=&st=&viewerDestinationUrl=&contentType=MEBC&groupID=" + grpId + "";
                                                        ResponseStatusMsg = HttpHelper.postFormData(new Uri("https://www.linkedin.com/groupMsg"), PostMessage);
                                                    }


                                                    //  PostMessage = "csrfToken=" + csrfToken + "&subject=" + Uri.EscapeDataString(tempsubject.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&submit=Send+Message&fromName=" + Uri.EscapeDataString(FromEmailNam) + "&showRecipeints=showRecipeints&fromEmail=" + FromemailId + "&connectionIds=" + connId + "&connectionNames=&allowEditRcpts=true&addMoreRcpts=false&openSocialAppBodySuffix=&st=&viewerDestinationUrl=&contentType=MEBC&groupID=" + grpId + "";
                                                    // ResponseStatusMsg = HttpHelper.postFormData(new Uri("https://www.linkedin.com/groupMsg"), PostMessage);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        // if (ResponseStatusMsg.Contains("upload_error") && ResponseStatusMsg.Contains("There was an error with the file upload. Please try again later"))
                                        {
                                            PostMessage = "csrfToken=" + csrfToken + "&subject=" + Uri.EscapeDataString(msg.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&submit=Send+Message&fromName=" + Uri.EscapeDataString(FromEmailNam) + "&showRecipeints=showRecipeints&fromEmail=" + FromemailId + "&connectionIds=" + connId + "&connectionNames=&allowEditRcpts=true&addMoreRcpts=false&openSocialAppBodySuffix=&st=&viewerDestinationUrl=&contentType=MEBC&groupID=" + grpId + "";
                                            ResponseStatusMsg = HttpHelper.postFormData(new Uri("https://www.linkedin.com/groupMsg"), PostMessage);
                                        }

                                        //PostMessage = "csrfToken=" + csrfToken + "&subject=" + Uri.EscapeDataString(msg.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&submit=Send+Message&showRecipeints=showRecipeintsfromName=" + Uri.EscapeDataString(FromEmailNam) + "&fromEmail=" + FromemailId + "&connectionIds=" + connId + "&connectionNames=&allowEditRcpts=true&addMoreRcpts=false&openSocialAppBodySuffix=&st=&viewerDestinationUrl=&contentType=MEBC&groupID=" + grpId + "";
                                        //ResponseStatusMsg = HttpHelper.postFormData(new Uri("http://www.linkedin.com/groupMsg"), PostMessage);
                                    }

                                    if ((!ResponseStatusMsg.Contains("Your message was successfully sent.") && !ResponseStatusMsg.Contains("Already Sent")) && (!ResponseStatusMsg.Contains("Se ha enviado tu mensaje satisfactoriamente") && !ResponseStatusMsg.Contains("Ya ha sido enviada") && !ResponseStatusMsg.Contains("Uw bericht is verzonden")))
                                    {

                                        if (ResponseStatusMsg.Contains("Already Sent") || (ResponseStatusMsg.Contains("Ya ha sido enviada") || (ResponseStatusMsg.Contains("BlackListed"))))
                                        {
                                            continue;
                                        }

                                        try
                                        {
                                            pageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com/groups?viewMembers=&gid=" + grpId));

                                            if (pageSource.Contains("contentType="))
                                            {
                                                try
                                                {
                                                    string Url_compose = "https://www.linkedin.com/inbox/#compose?connId=" + ToCd + "&groupId=" + grpId;
                                                    string Responce = HttpHelper.getHtmlfromUrl(new Uri(Url_compose));
                                                    {

                                                        string PostUrlsssss = "https://www.linkedin.com/inbox/mailbox/message/send";


                                                        string PostDataFinal1 = "senderEmail=645883950&ccInput=&subject=" + Uri.EscapeDataString(tempsubject.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&persist=true&showRecipients=showRecipients&isReply=&isForward=&itemId=&recipients=" + ToCd + "&recipientNames=%5B%7B%22memberId%22%3A" + ToCd + "%2C%22fullName%22%3A%22" + ContactName + "%22%7D%5D&groupId=" + grpId + "&csrfToken=" + csrfToken + "&sourceAlias=0_3mbGc9okCQbybxvc2A5Vz5&submit=Envoyer";
                                                        ResponseStatusMsg = HttpHelper.postFormData(new Uri(PostUrlsssss), PostDataFinal1);
                                                    }
                                                    if (ResponseStatusMsg.Contains("upload_error") && ResponseStatusMsg.Contains("There was an error with the file upload. Please try again later"))
                                                    {
                                                        PostMessage = "csrfToken=" + csrfToken + "&subject=" + Uri.EscapeDataString(msg.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&submit=Send+Message&fromName=" + Uri.EscapeDataString(FromEmailNam) + "&showRecipeints=showRecipeints&fromEmail=" + FromemailId + "&connectionIds=" + connId + "&connectionNames=&allowEditRcpts=true&addMoreRcpts=false&openSocialAppBodySuffix=&st=&viewerDestinationUrl=&contentType=MEBC&groupID=" + grpId + "";
                                                        ResponseStatusMsg = HttpHelper.postFormData(new Uri("https://www.linkedin.com/groupMsg"), PostMessage);
                                                    }


                                                }
                                                catch (Exception ex)
                                                {
                                                }

                                                //try
                                                //{
                                                //    string contentType = pageSource.Substring(pageSource.IndexOf("contentType="), pageSource.IndexOf("&", pageSource.IndexOf("contentType=")) - pageSource.IndexOf("contentType=")).Replace("contentType=", string.Empty).Replace("contentType=", string.Empty).Trim();

                                                //    string pageSource2 = HttpHelper.getHtmlfromUrl1(new Uri("https://www.linkedin.com/groupMsg?displayCreate=&contentType=" + contentType + "&connId=" + connId + "&groupID=" + grpId + ""));

                                                //    //  PostMessage = "csrfToken=" + csrfToken + "&subject=" + Uri.EscapeDataString(msg.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&submit=Send+Message&showRecipeints=showRecipeints&fromName=" + FromEmailNam + "&fromEmail=" + FromemailId + "&connectionIds=" + connId + "&connectionNames=" + Uri.EscapeUriString(Nstring) + "&allowEditRcpts=true&addMoreRcpts=false&openSocialAppBodySuffix=&st=&viewerDestinationUrl=&contentType=" + contentType + "&groupID=" + grpId + "";

                                                //    string postData_Message_posting = "senderEmail=914640570&ccInput=&subject=hi&body=hello&persist=true&showRecipients=showRecipients&isReply=&isForward=&itemId=&recipients=313092875&recipientNames=%5B%7B%22memberId%22%3A313092875%2C%22fullName%22%3A%22SibghatUllah+K.+Khan%22%7D%5D&groupId=" + grpId + "&csrfToken=" + csrfToken + "&sourceAlias=0_3mbGc9okCQbybxvc2A5Vz5&submit=Send+Message";
                                                //    string postUrlll = "https://www.linkedin.com/inbox/mailbox/message/send";
                                                //    ResponseStatusMsg = HttpHelper.postFormData(new Uri(postUrlll), postData_Message_posting);
                                                //}
                                                //catch (Exception ex)
                                                //{
                                                //}
                                            }


                                            // ResponseStatusMsg = HttpHelper.postFormData(new Uri("https://www.linkedin.com/groupMsg"), PostMessage);
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }

                                    if ((ResponseStatusMsg.Contains("Your message was successfully sent.")) || (ResponseStatusMsg.Contains("Se ha enviado tu mensaje satisfactoriamente") || (ResponseStatusMsg.Contains("Uw bericht is verzonden"))))
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Subject Posted : " + tempsubject + " ]");
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Body Text Posted : " + tempBody.ToString() + " ]");
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Message Posted To Account: " + ContactName + " ]");
                                        ReturnString = "Your message was successfully sent.";

                                        #region CSV
                                        string bdy = string.Empty;
                                        try
                                        {
                                            bdy = body.ToString().Replace("\r", string.Empty).Replace("\n", " ").Replace(",", " ");
                                        }
                                        catch { }
                                        if (string.IsNullOrEmpty(bdy))
                                        {
                                            bdy = tempBody.ToString().Replace(",", ":");
                                        }
                                        string CSVHeader = "UserName" + "," + "Subject" + "," + "Body Text" + "," + "ContactName";
                                        string CSV_Content = UserName + "," + tempsubject + "," + bdy.ToString() + "," + ContactName;
                                        CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, FilePath.path_MessageSentGroupMember);

                                        try
                                        {
                                            InsertMsgGroupMemData(UserName, Convert.ToInt32(connId), ContactName, Convert.ToInt32(grpId), SelectedGrpName, msg, tempBody);
                                        }
                                        catch { }

                                        #endregion

                                    }
                                    else if (ResponseStatusMsg.Contains("There was an unexpected problem that prevented us from completing your request"))
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ There was an unexpected problem that prevented us from completing your request ! ]");
                                        GlobusFileHelper.AppendStringToTextfileNewLine("Error In Message Posting", FilePath.path_MessageGroupMember);
                                    }
                                    else if (ResponseStatusMsg.Contains("You are no longer authorized to message this"))
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ You are no longer authorized to message this ! ]");
                                        GlobusFileHelper.AppendStringToTextfileNewLine("Error In Message Posting", FilePath.path_MessageGroupMember);
                                    }
                                    else if ((ResponseStatusMsg.Contains("Already Sent")) || (ResponseStatusMsg.Contains("Ya ha sido enviada")))
                                    {
                                        string bdy = string.Empty;
                                        try
                                        {
                                            bdy = body.ToString().Replace("\r", string.Empty).Replace("\n", " ").Replace(",", " ");
                                        }
                                        catch { }
                                        if (string.IsNullOrEmpty(bdy))
                                        {
                                            bdy = tempBody.ToString().Replace(",", ":");
                                        }
                                        string CSVHeader = "UserName" + "," + "Subject" + "," + "Body Text" + "," + "ContactName";
                                        string CSV_Content = UserName + "," + msg + "," + bdy.ToString() + "," + ContactName;
                                        CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, FilePath.path_MessageAlreadySentGroupMember);

                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Message Not Posted To Account: " + ContactName + " because it has sent the same message]");
                                    }
                                    else if ((ResponseStatusMsg.Contains("Votre message a bien")) || ResponseStatusMsg.Contains("class=\"alert success") || (ResponseStatusMsg.Contains("Ya ha sido enviada")))
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Subject Posted : " + tempsubject + " ]");
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Body Text Posted : " + tempBody.ToString() + " ]");
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Message Posted To Account: " + ContactName + " ]");
                                        ReturnString = "Your message was successfully sent.";


                                        string bdy = string.Empty;
                                        try
                                        {
                                            bdy = body.ToString().Replace("\r", string.Empty).Replace("\n", " ").Replace(",", " ");
                                        }
                                        catch { }
                                        if (string.IsNullOrEmpty(bdy))
                                        {
                                            bdy = tempBody.ToString().Replace(",", ":");
                                        }
                                        string CSVHeader = "UserName" + "," + "Subject" + "," + "Body Text" + "," + "ContactName";
                                        string CSV_Content = UserName + "," + tempsubject + "," + bdy.ToString() + "," + ContactName;
                                        CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, FilePath.path_MessageSentGroupMember);

                                        try
                                        {
                                            InsertMsgGroupMemData(UserName, Convert.ToInt32(connId), ContactName, Convert.ToInt32(grpId), SelectedGrpName, msg, tempBody);
                                        }
                                        catch { }
                                    }
                                    else
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Error In Message Posting ]");
                                        GlobusFileHelper.AppendStringToTextfileNewLine("Error In Message Posting", FilePath.path_MessageGroupMember);
                                    }

                                    int delay = RandomNumberGenerator.GenerateRandom(mindelay, maxdelay);
                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Delay for : " + delay + " Seconds ]");
                                    Thread.Sleep(delay * 1000);

                                }
                                catch (Exception ex)
                                {
                                    //Log("[ " + DateTime.Now + " ] => [ Error:" + ex.Message + "StackTrace --> >>>" + ex.StackTrace + " ]");
                                    GlobusFileHelper.AppendStringToTextfileNewLine(" Error:" + ex.Message + "StackTrace --> >>>" + ex.StackTrace, FilePath.path_MessageGroupMember);
                                }
                            }
                            catch (Exception ex)
                            {
                                //Log("[ " + DateTime.Now + " ] => [ Error:" + ex.Message + "StackTrace --> >>>" + ex.StackTrace);
                                GlobusFileHelper.AppendStringToTextfileNewLine(" Error:" + ex.Message + "StackTrace --> >>>" + ex.StackTrace, FilePath.path_MessageGroupMember);
                            }
                        }
                        #endregion
                    }
                    else
                    {

                        try
                        {
                            int startindex = getComposeData.IndexOf("\"senderEmail\" value=\"");
                            if (startindex < 0)
                            {
                                startindex = getComposeData.IndexOf("\"senderEmail\",\"value\":\"");
                            }
                            string start = getComposeData.Substring(startindex).Replace("\"senderEmail\" value=\"", string.Empty).Replace("\"senderEmail\",\"value\":\"", string.Empty);
                            int endindex = start.IndexOf("\"/>");
                            if (endindex < 0)
                            {
                                endindex = start.IndexOf("\",\"");
                            }
                            string end = start.Substring(0, endindex).Replace("\"/>", string.Empty).Replace("\",\"", string.Empty);
                            senderEmail = end.Trim();
                        }
                        catch (Exception ex)
                        {
                            senderEmail = Utils.getBetween(getComposeData, "<", ">");
                        }





                        string pageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com/home?trk=hb_tab_home_top"));
                        if (pageSource.Contains("csrfToken"))
                        {
                            try
                            {
                                csrfToken = pageSource.Substring(pageSource.IndexOf("csrfToken"), 50);
                                string[] Arr = csrfToken.Split('<');
                                csrfToken = Arr[0];
                                csrfToken = csrfToken.Replace("csrfToken", "").Replace("\"", string.Empty).Replace("value", string.Empty).Replace("cs", string.Empty).Replace("id", string.Empty).Replace("=", string.Empty).Replace("\n", string.Empty).Replace(">", string.Empty).Replace("<script typ", string.Empty);
                                csrfToken = csrfToken.Trim();
                            }
                            catch (Exception ex)
                            {

                            }

                        }

                        if (pageSource.Contains("sourceAlias"))
                        {
                            try
                            {
                                sourceAlias = pageSource.Substring(pageSource.IndexOf("sourceAlias"), 100);
                                string[] Arr = sourceAlias.Split('"');
                                sourceAlias = Arr[2];
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        if (pageSource.Contains("goback="))
                        {
                            try
                            {
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        foreach (KeyValuePair<string, string> itemChecked in SlectedContacts)
                        {
                            try
                            {
                                DataSet ds = new DataSet();
                                DataSet ds_bList = new DataSet();
                                string ContactName = string.Empty;
                                string Nstring = string.Empty;
                                string connId = string.Empty;
                                string FName = string.Empty;
                                string Lname = string.Empty;
                                string tempBody = string.Empty;
                                string tempsubject = string.Empty;
                                string n_ame1 = string.Empty;

                                //grpId = itemChecked.Key.ToString();



                                try
                                {
                                    // FName = itemChecked.Value.Split(' ')[0];
                                    // Lname = itemChecked.Value.Split(' ')[1];
                                    try
                                    {
                                        n_ame1 = itemChecked.Value.Split(']')[1].Trim(); ;
                                    }
                                    catch
                                    { }
                                    if (string.IsNullOrEmpty(n_ame1))
                                    {
                                        try
                                        {
                                            n_ame1 = itemChecked.Value;
                                        }
                                        catch
                                        { }
                                    }
                                    string[] n_ame = Regex.Split(n_ame1, " ");
                                    FName = " " + n_ame[0];
                                    Lname = n_ame[1];

                                    if (!string.IsNullOrEmpty(n_ame[2]))
                                    {
                                        Lname = Lname + n_ame[2];
                                    }
                                    if (!string.IsNullOrEmpty(n_ame[3]))
                                    {
                                        Lname = Lname + n_ame[3];
                                    }
                                    if (!string.IsNullOrEmpty(n_ame[4]))
                                    {
                                        Lname = Lname + n_ame[4];
                                    }
                                    if (!string.IsNullOrEmpty(n_ame[5]))
                                    {
                                        Lname = Lname + n_ame[5];
                                    }
                                }
                                catch (Exception ex)
                                {
                                }

                                try
                                {
                                    ContactName = FName + " " + Lname;
                                    ContactName = ContactName.Replace("%20", " ");
                                }
                                catch { }

                                GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Adding Contact : " + ContactName + " ]");

                                string ToCd = itemChecked.Key;
                                List<string> AddAllString = new List<string>();

                                if (FString == string.Empty)
                                {
                                    string CompString = "{" + "\"" + "_" + ToCd.Trim() + "\"" + ":" + "{" + "\"" + "memberId" + "\"" + ":" + "\"" + ToCd.Trim() + "\"" + "," + "\"" + "firstName" + "\"" + ":" + "\"" + FName + "\"" + "," + "\"" + "lastName" + "\"" + ":" + "\"" + Lname + "\"" + "}";
                                    FString = CompString;
                                }
                                else
                                {
                                    string CompString = "\"" + "_" + ToCd.Trim() + "\"" + ":" + "{" + "\"" + "memberId" + "\"" + ":" + "\"" + ToCd.Trim() + "\"" + "," + "\"" + "firstName" + "\"" + ":" + "\"" + FName + "\"" + "," + "\"" + "lastName" + "\"" + ":" + "\"" + Lname + "\"" + "}";
                                    FString = CompString;
                                }

                                if (Nstring == string.Empty)
                                {
                                    Nstring = FString;
                                    connId = ToCd;
                                }
                                else
                                {
                                    Nstring += "," + FString;
                                    connId += " " + ToCd;
                                }

                                Nstring += "}";
                                tempsubject = msg;
                                string full_name = string.Empty;
                                try
                                {
                                    string PostMessage = string.Empty;
                                    string ResponseStatusMsg = string.Empty;

                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Message Sending Process Running.. ]");
                                    if (mesg_with_tag == true)
                                    {
                                        tempsubject = msg;
                                    }
                                    if (msg_spintaxt == true)
                                    {
                                        try
                                        {
                                            msg = GrpMemSubjectlist[RandomNumberGenerator.GenerateRandom(0, GrpMemSubjectlist.Count - 1)];
                                            body = GrpMemMessagelist[RandomNumberGenerator.GenerateRandom(0, GrpMemMessagelist.Count - 1)];
                                        }
                                        catch
                                        {
                                        }
                                    }

                                    if (mesg_with_tag == true)
                                    {
                                        if (string.IsNullOrEmpty(FName))
                                        {
                                            tempBody = body.Replace("<Insert Name>", ContactName);
                                        }
                                        else
                                        {
                                            //tempBody = GlobusSpinHelper.spinLargeText(new Random(), body);

                                            //if (lstSubjectReuse.Count == GrpMemSubjectlist.Count)
                                            //{
                                            //    lstSubjectReuse.Clear();
                                            //}
                                            //foreach (var itemSubject in GrpMemSubjectlist)
                                            //{
                                            //    if (string.IsNullOrEmpty(TemporarySubject))
                                            //    {
                                            //        TemporarySubject = itemSubject;
                                            //        tempsubject = itemSubject;
                                            //        lstSubjectReuse.Add(itemSubject);
                                            //        break;
                                            //    }
                                            //    else if (!lstSubjectReuse.Contains(itemSubject))
                                            //    {
                                            //        TemporarySubject = itemSubject;
                                            //        tempsubject = itemSubject;
                                            //        lstSubjectReuse.Add(itemSubject);
                                            //        break;
                                            //    }
                                            //    else
                                            //    {
                                            //        continue;
                                            //    }
                                            //}

                                            tempBody = tempBody.Replace("<Insert Name>", FName);

                                            tempsubject = tempsubject.Replace("<Insert Name>", FName);
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(FName))
                                        {
                                            tempBody = body.Replace("<Insert Name>", ContactName);
                                        }
                                        else
                                        {
                                            tempBody = body.Replace("<Insert Name>", FName);
                                        }
                                    }

                                    if (SelectedGrpName.Contains(":"))
                                    {
                                        try
                                        {
                                            string[] arrSelectedGrpName = Regex.Split(SelectedGrpName, ":");
                                            if (arrSelectedGrpName.Length > 1)
                                            {
                                                SelectedGrpName = arrSelectedGrpName[1];
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                    try
                                    {
                                        full_name = Utils.getBetween("####" + ContactName, "####", "(");
                                    }
                                    catch
                                    { }
                                    #region To insert Full Name

                                    if (mesg_with_tag == true)
                                    {
                                        try
                                        {
                                            if (tempsubject.Contains("<Insert Full Name>"))
                                            {
                                                tempsubject = tempsubject.Replace("<Insert Full Name>", full_name);
                                            }
                                            if (tempBody.Contains("<Insert Full Name>"))
                                            {
                                                tempBody = tempBody.Replace("<Insert Full Name>", full_name);
                                            }

                                        }
                                        catch { }

                                    }


                                    #endregion

                                    if (mesg_with_tag == true)
                                    {
                                        tempBody = tempBody.Replace("<Insert Group>", SelectedGrpName);
                                        tempBody = tempBody.Replace("<Insert From Email>", senderEmail);
                                        tempBody = tempBody.Replace("<Insert Name>", string.Empty).Replace("<Insert Group>", string.Empty).Replace("<Insert From Email>", string.Empty);

                                        tempsubject = tempsubject.Replace("<Insert Group>", SelectedGrpName);
                                        tempsubject = tempsubject.Replace("<Insert From Email>", senderEmail);
                                        tempsubject = tempsubject.Replace("<Insert Name>", string.Empty).Replace("<Insert Group>", string.Empty).Replace("<Insert From Email>", string.Empty);
                                    }
                                    else
                                    {
                                        tempBody = tempBody.Replace("<Insert Group>", SelectedGrpName);
                                        tempBody = tempBody.Replace("<Insert From Email>", FromEmailNam);
                                        tempBody = tempBody.Replace("<Insert Name>", string.Empty).Replace("<Insert Group>", string.Empty).Replace("<Insert From Email>", string.Empty);
                                    }

                                    //Check BlackListed Accounts
                                    try
                                    {
                                        string Querystring = "Select ProfileID From tb_BlackListAccount Where ProfileID ='" + itemChecked.Key + "'";
                                        ds_bList = DataBaseHandler.SelectQuery(Querystring, "tb_BlackListAccount");
                                    }
                                    catch { }


                                    if (preventMsgSameGroup)
                                    {
                                        try
                                        {
                                            string Querystring = "Select MsgFrom,MsgToId,MsgTo,MsgGroupId,MsgGroupName,MsgSubject,MsgBody From tb_ManageMsgGroupMem Where MsgFrom ='" + UserName + "' and MsgGroupId = " + grpId + " and MsgToId = " + connId + "";
                                            ds = DataBaseHandler.SelectQuery(Querystring, "tb_ManageMsgGroupMem");
                                        }
                                        catch { }
                                    }

                                    if (preventMsgWithoutGroup)
                                    {
                                        try
                                        {
                                            string Querystring = "Select MsgFrom,MsgToId,MsgTo,MsgGroupId,MsgGroupName,MsgSubject,MsgBody From tb_ManageMsgGroupMem Where MsgFrom ='" + UserName + "' and MsgToId = " + connId + "";
                                            ds = DataBaseHandler.SelectQuery(Querystring, "tb_ManageMsgGroupMem");
                                        }
                                        catch { }
                                    }
                                    if (preventMsgGlobal)
                                    {
                                        try
                                        {
                                            string Querystring = "Select MsgFrom,MsgToId,MsgTo,MsgGroupId,MsgGroupName,MsgSubject,MsgBody From tb_ManageMsgGroupMem Where MsgToId = " + connId + "";
                                            ds = DataBaseHandler.SelectQuery(Querystring, "tb_ManageMsgGroupMem");
                                        }
                                        catch { }
                                    }

                                    try
                                    {

                                        if (ds.Tables.Count > 0)
                                        {
                                            if (ds.Tables[0].Rows.Count > 0)
                                            {

                                                PostMessage = "";
                                                ResponseStatusMsg = "Already Sent";

                                            }
                                            else
                                            {
                                                if (ds_bList.Tables.Count > 0 && ds_bList.Tables[0].Rows.Count > 0)
                                                {
                                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ User: " + ContactName + " is Added BlackListed List For Send Messages Pls Check ]");
                                                    ResponseStatusMsg = "BlackListed";
                                                }
                                                else
                                                {
                                                    #region send InMail
                                                    if (IssendInMail)
                                                    {
                                                        try
                                                        {
                                                            string csrfToken_inmail = string.Empty;
                                                            string Referer_InMail = string.Empty;
                                                            string authToken = string.Empty;

                                                            string url = "https://www.linkedin.com/premium/inmail/compose?destID=" + ToCd;
                                                            string responce_Inmail = HttpHelper.getHtmlfromUrl(new Uri(url));
                                                            if (!string.IsNullOrEmpty(responce_Inmail))
                                                            {
                                                                try
                                                                {
                                                                    csrfToken_inmail = Utils.getBetween(responce_Inmail, "csrfToken=", "\">");
                                                                }
                                                                catch { }
                                                            }

                                                            string subject_InMail = tempsubject.Replace(" ", "+");
                                                            string body_InMail = tempBody.Replace(" ", "+");
                                                            Referer_InMail = "https://www.linkedin.com/premium/inmail/compose?destID=" + ToCd;
                                                            string Action_url_InMail = "https://www.linkedin.com/premium/inmail/send?csrfToken=" + csrfToken_inmail;
                                                            // Referer_InMail = "https://www.linkedin.com/premium/inmail/compose?destID=" + ToCd + "&creationType=DC&authToken=" + authToken + "&authType=name&utm_source=Profile_inmail&utm_medium=onsite&utm_campaign=Subs";
                                                            string postData_InMail = "title=" + subject_InMail + "&document=" + body_InMail + "&destID=" + ToCd + "&creationType=DC&proposalType=" + category_to_send_inmail + "&authType=&authToken=";
                                                            ResponseStatusMsg = HttpHelper.postDataFormessagePosting(new Uri(Action_url_InMail), postData_InMail, Referer_InMail);


                                                        }
                                                        catch
                                                        { }



                                                    }
                                                    #endregion

                                                    #region direct message
                                                    else
                                                    {
                                                        try
                                                        {
                                                            string Url_compose = "https://www.linkedin.com/inbox/#compose?connId=" + ToCd + "&groupId=" + grpId;
                                                            string Responce = HttpHelper.getHtmlfromUrl(new Uri(Url_compose));

                                                            string url = "https://www.linkedin.com/inbox/compose?connId=" + ToCd + "&groupId=" + grpId;
                                                            string emailSender = string.Empty;
                                                            string resp = HttpHelper.getHtmlfromUrl(new Uri(url));
                                                            try
                                                            {
                                                                emailSender = Utils.getBetween(resp, "senderEmail-composeForm", "recipientNames");
                                                                emailSender = Utils.getBetween(emailSender, "value\":\"", "\"}");
                                                                if (string.IsNullOrEmpty(emailSender))
                                                                {
                                                                    try
                                                                    {
                                                                        emailSender = Utils.getBetween(resp, "senderEmail", "selected\":true");
                                                                        emailSender = Utils.getBetween(emailSender, "\"value\":\"", "\",");
                                                                    }
                                                                    catch
                                                                    { }
                                                                }

                                                            }
                                                            catch
                                                            { }

                                                            try
                                                            {
                                                                ContactName = ContactName.Trim();
                                                                ContactName = Utils.getBetween("###" + ContactName, "###", "(");
                                                                ContactName = ContactName.Replace(" ", "+");
                                                            }
                                                            catch
                                                            { }

                                                            string postUrlFinal = "https://www.linkedin.com/inbox/mailbox/message/send";
                                                            string PostDataFinal = "senderEmail=" + emailSender + "&ccInput=&subject=" + tempsubject.Replace(" ", "+") + "&body=" + tempBody.Replace(" ", "+") + "&persist=true&showRecipients=showRecipients&isReply=&isForward=&itemId=&recipients=" + ToCd + "&recipientNames=%5B%7B%22memberId%22%3A" + ToCd + "%2C%22fullName%22%3A%22" + ContactName + "%22%7D%5D&groupId=" + grpId + "&csrfToken=" + csrfToken + "&sourceAlias=0_3mbGc9okCQbybxvc2A5Vz5&submit=Send+Message";


                                                            ResponseStatusMsg = HttpHelper.postFormData(new Uri(postUrlFinal), PostDataFinal);
                                                        }
                                                        catch
                                                        {
                                                        }
                                                    }
                                                    #endregion
                                                    if (!ResponseStatusMsg.Contains("Your message was successfully sent"))
                                                    {
                                                        //string Url_compose1 = "https://www.linkedin.com/inbox/#compose?connId=" + ToCd + "&groupId=" + grpId;
                                                        //string Responce1 = HttpHelper.getHtmlfromUrl(new Uri(Url_compose1));

                                                        //string postUrlFinal1 = "https://www.linkedin.com/inbox/mailbox/message/send";
                                                        //string PostDataFinal1 = "senderEmail=914640570&ccInput=&subject=" + Uri.EscapeDataString(tempsubject.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&persist=true&showRecipients=showRecipients&isReply=&isForward=&itemId=&recipients=" + ToCd + "&recipientNames=%5B%7B%22memberId%22%3A" + ToCd + "%2C%22fullName%22%3A%22" + ContactName + "%22%7D%5D&groupId=" + grpId + "&csrfToken=" + csrfToken + "&sourceAlias=0_3mbGc9okCQbybxvc2A5Vz5&submit=Send+Message";


                                                        //ResponseStatusMsg = HttpHelper.postFormData(new Uri(postUrlFinal1), PostDataFinal1);

                                                        //if (ResponseStatusMsg.Contains("upload_error") && ResponseStatusMsg.Contains("There was an error with the file upload. Please try again later"))
                                                        //{
                                                        //    PostMessage = "csrfToken=" + csrfToken + "&subject=" + Uri.EscapeDataString(msg.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&submit=Send+Message&fromName=" + Uri.EscapeDataString(FromEmailNam) + "&showRecipeints=showRecipeints&fromEmail=" + FromemailId + "&connectionIds=" + connId + "&connectionNames=&allowEditRcpts=true&addMoreRcpts=false&openSocialAppBodySuffix=&st=&viewerDestinationUrl=&contentType=MEBC&groupID=" + grpId + "";
                                                        //    ResponseStatusMsg = HttpHelper.postFormData(new Uri("https://www.linkedin.com/groupMsg"), PostMessage);
                                                        //}
                                                    }
                                                    //Comment By ajay 
                                                    //PostMessage = "csrfToken=" + csrfToken + "&subject=" + Uri.EscapeDataString(msg.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&submit=Send+Message&fromName=" + Uri.EscapeDataString(FromEmailNam) + "&showRecipeints=showRecipeints&fromEmail=" + FromemailId + "&connectionIds=" + connId + "&connectionNames=&allowEditRcpts=true&addMoreRcpts=false&openSocialAppBodySuffix=&st=&viewerDestinationUrl=&contentType=MEBC&groupID=" + grpId + "";
                                                    //ResponseStatusMsg = HttpHelper.postFormData(new Uri("https://www.linkedin.com/groupMsg"), PostMessage);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (ds_bList.Tables.Count > 0)
                                            {
                                                if (ds_bList.Tables[0].Rows.Count > 0)
                                                {

                                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ User: " + ContactName + " is Added BlackListed List For Send Messages Pls Check ]");
                                                    ResponseStatusMsg = "BlackListed";
                                                }
                                                else
                                                {

                                                    #region  wriiten by sharan

                                                    #region Send InMail
                                                    if (IssendInMail)
                                                    {
                                                        try
                                                        {
                                                            string csrfToken_inmail = string.Empty;
                                                            string Referer_InMail = string.Empty;
                                                            string authToken = string.Empty;

                                                            string url = "https://www.linkedin.com/premium/inmail/compose?destID=" + ToCd;
                                                            string responce_Inmail = HttpHelper.getHtmlfromUrl(new Uri(url));
                                                            if (!string.IsNullOrEmpty(responce_Inmail))
                                                            {
                                                                csrfToken_inmail = Utils.getBetween(responce_Inmail, "csrfToken=", "\">");
                                                                // Referer_InMail = Utils.getBetween(responce_Inmail, "X-FS-Origin-Request\":\"", "\"");



                                                            }

                                                            string subject_InMail = tempsubject.Replace(" ", "+");
                                                            string body_InMail = tempBody.Replace(" ", "+");
                                                            Referer_InMail = "https://www.linkedin.com/premium/inmail/compose?destID=" + ToCd;
                                                            string Action_url_InMail = "https://www.linkedin.com/premium/inmail/send?csrfToken=" + csrfToken_inmail;
                                                            // Referer_InMail = "https://www.linkedin.com/premium/inmail/compose?destID=" + ToCd + "&creationType=DC&authToken=" + authToken + "&authType=name&utm_source=Profile_inmail&utm_medium=onsite&utm_campaign=Subs";
                                                            string postData_InMail = "title=" + subject_InMail + "&document=" + body_InMail + "&destID=" + ToCd + "&creationType=DC&proposalType=" + category_to_send_inmail + "&authType=&authToken=";
                                                            ResponseStatusMsg = HttpHelper.postDataFormessagePosting(new Uri(Action_url_InMail), postData_InMail, Referer_InMail);


                                                        }
                                                        catch
                                                        { }


                                                    }
                                                    #endregion

                                                    #region directMessage
                                                    else
                                                    {
                                                        try
                                                        {
                                                            string Url_compose = "https://www.linkedin.com/inbox/#compose?connId=" + ToCd + "&groupId=" + grpId;
                                                            string Responce = HttpHelper.getHtmlfromUrl(new Uri(Url_compose));

                                                            string url = "https://www.linkedin.com/inbox/compose?connId=" + ToCd + "&groupId=" + grpId;
                                                            string emailSender = string.Empty;
                                                            string resp = HttpHelper.getHtmlfromUrl(new Uri(url));
                                                            try
                                                            {
                                                                emailSender = Utils.getBetween(resp, "senderEmail-composeForm", "recipientNames");
                                                                emailSender = Utils.getBetween(emailSender, "value\":\"", "\"}");
                                                                if (string.IsNullOrEmpty(emailSender))
                                                                {
                                                                    try
                                                                    {
                                                                        emailSender = Utils.getBetween(resp, "senderEmail", "selected\":true");
                                                                        emailSender = Utils.getBetween(emailSender, "\"value\":\"", "\",");
                                                                    }
                                                                    catch
                                                                    { }
                                                                }
                                                            }
                                                            catch
                                                            { }

                                                            try
                                                            {
                                                                ContactName = ContactName.Trim();
                                                                ContactName = Utils.getBetween("###" + ContactName, "###", "(");
                                                                ContactName = ContactName.Replace(" ", "+");
                                                            }
                                                            catch
                                                            { }

                                                            string postUrlFinal = "https://www.linkedin.com/inbox/mailbox/message/send";
                                                            string PostDataFinal = "senderEmail=" + emailSender + "&ccInput=&subject=" + tempsubject.Replace(" ", "+") + "&body=" + tempBody.Replace(" ", "+") + "&persist=true&showRecipients=showRecipients&isReply=&isForward=&itemId=&recipients=" + ToCd + "&recipientNames=%5B%7B%22memberId%22%3A" + ToCd + "%2C%22fullName%22%3A%22" + ContactName + "%22%7D%5D&groupId=" + grpId + "&csrfToken=" + csrfToken + "&sourceAlias=0_3mbGc9okCQbybxvc2A5Vz5&submit=Send+Message";


                                                            ResponseStatusMsg = HttpHelper.postFormData(new Uri(postUrlFinal), PostDataFinal);
                                                        }
                                                        catch
                                                        {
                                                        }
                                                    }

                                                    #endregion

                                                    if (ResponseStatusMsg.Contains("Sorry, you have reached a limit for directly messaging group members"))
                                                    {
                                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Sorry, you have reached a limit for directly messaging group members]");
                                                        return;
                                                    }

                                                    if (!ResponseStatusMsg.Contains("Your message was successfully sent"))
                                                    {
                                                        try
                                                        {
                                                            //string action_url = "https://www.linkedin.com/premium/inmail/send?csrfToken=" + csrfToken;
                                                            //string referer = "https://www.linkedin.com/premium/inmail/compose?destID=" + ToCd + "&creationType=OPEN_LINK&authToken=mdaI&authType=name&utm_source=Profile_inmail&utm_medium=onsite&utm_campaign=Subs";
                                                            //string pd = "title=" + tempsubject.Replace(" ", "+") + "&document=" + tempBody.Replace(" ", "+") + "&destID=" + ToCd + "&creationType=OPEN_LINK&proposalType=JOB_OFFER&senderEmail=&authType=name&authToken=llYo";
                                                            //ResponseStatusMsg = HttpHelper.postDataFormessagePosting(new Uri(action_url), pd, referer);
                                                        }
                                                        catch
                                                        { }
                                                    }
                                                    #endregion


                                                    //string Url_compose = "https://www.linkedin.com/inbox/#compose?connId=" + ToCd + "&groupId=" + grpId;
                                                    //string Responce = HttpHelper.getHtmlfromUrl(new Uri(Url_compose));

                                                    //string postUrlFinal = "https://www.linkedin.com/inbox/mailbox/message/send";
                                                    //string PostDataFinal = "senderEmail=913067065&ccInput=&subject=" + Uri.EscapeDataString(tempsubject) + "+&body=" + Uri.EscapeDataString(tempBody) + "&persist=true&showRecipients=showRecipients&isReply=&isForward=&itemId=&recipients=" + ToCd + "&recipientNames=%5B%7B%22memberId%22%3A" + ToCd + "%2C%22fullName%22%3A%22" + ContactName.Replace(" ", "+") + "%22%7D%5D&groupId=" + grpId + "&csrfToken=" + csrfToken + "&sourceAlias=0_3mbGc9okCQbybxvc2A5Vz5&submit=Send+Message";

                                                    //ResponseStatusMsg = HttpHelper.postFormData(new Uri(postUrlFinal), PostDataFinal);

                                                    //if (ResponseStatusMsg.Contains("upload_error") && ResponseStatusMsg.Contains("There was an error with the file upload. Please try again later"))
                                                    //{
                                                    //    PostMessage = "csrfToken=" + csrfToken + "&subject=" + Uri.EscapeDataString(tempsubject.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&submit=Send+Message&fromName=" + Uri.EscapeDataString(FromEmailNam) + "&showRecipeints=showRecipeints&fromEmail=" + FromemailId + "&connectionIds=" + connId + "&connectionNames=&allowEditRcpts=true&addMoreRcpts=false&openSocialAppBodySuffix=&st=&viewerDestinationUrl=&contentType=MEBC&groupID=" + grpId + "";
                                                    //    ResponseStatusMsg = HttpHelper.postFormData(new Uri("https://www.linkedin.com/groupMsg"), PostMessage);
                                                    //}






                                                }
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        //PostMessage = "csrfToken=" + csrfToken + "&subject=" + Uri.EscapeDataString(msg.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&submit=Send+Message&showRecipeints=showRecipeintsfromName=" + Uri.EscapeDataString(FromEmailNam) + "&fromEmail=" + FromemailId + "&connectionIds=" + connId + "&connectionNames=&allowEditRcpts=true&addMoreRcpts=false&openSocialAppBodySuffix=&st=&viewerDestinationUrl=&contentType=MEBC&groupID=" + grpId + "";
                                        //ResponseStatusMsg = HttpHelper.postFormData(new Uri("http://www.linkedin.com/groupMsg"), PostMessage);
                                    }
                                    #region commented for null response
                                    //if (string.IsNullOrEmpty(ResponseStatusMsg))
                                    //{
                                    //    try
                                    //    {
                                    //        string action_url = "https://www.linkedin.com/messaging/compose?connId="+ToCd+"&groupId="+grpId;
                                    //        string referer = "https://www.linkedin.com/grp/members?gid="+grpId;
                                    //        string get_response = HttpHelper.getHtmlfromUrl(new Uri(action_url));

                                    //        if (get_response.Contains("memberProfile"))
                                    //        {
                                    //            string all_details = Utils.getBetween(get_response, "<code id", "<script>");
                                    //            string[] arr = Regex.Split(all_details, "recipients");
                                    //            string recipient_details = string.Empty;
                                    //            string authToken = string.Empty;

                                    //            foreach (string item in arr)
                                    //            {
                                    //                if (item.Contains(ToCd))
                                    //                {
                                    //                    recipient_details = item;
                                    //                    break;
                                    //                }
                                    //            }

                                    //            authToken = Utils.getBetween(recipient_details, "authToken", "\"}");



                                    //            string s1 = Utils.getBetween("###" + authToken, "###", "profileId");
                                    //            string first_last_name = Utils.getBetween("###" + s1, "", "profileId");
                                    //            first_last_name = first_last_name + "profileId\":\"";


                                    //        }

                                    //    }
                                    //    catch
                                    //    { }
                                    //}
                                    #endregion

                                    #region commented by sharan after LI update

                                    //if ((!ResponseStatusMsg.Contains("Your message was successfully sent.") && !ResponseStatusMsg.Contains("Already Sent")) && (!ResponseStatusMsg.Contains("Se ha enviado tu mensaje satisfactoriamente") && !ResponseStatusMsg.Contains("Ya ha sido enviada") && !ResponseStatusMsg.Contains("Uw bericht is verzonden")))
                                    //{

                                    //    if (ResponseStatusMsg.Contains("Already Sent") || (ResponseStatusMsg.Contains("Ya ha sido enviada") || (ResponseStatusMsg.Contains("BlackListed"))))
                                    //    {
                                    //        continue;
                                    //    }

                                    //    try
                                    //    {
                                    //        pageSource = HttpHelper.getHtmlfromUrl1(new Uri("https://www.linkedin.com/groups?viewMembers=&gid=" + grpId));

                                    //        if (pageSource.Contains("contentType="))
                                    //        {
                                    //            try
                                    //            {
                                    //                string Url_compose = "https://www.linkedin.com/inbox/#compose?connId=" + ToCd + "&groupId=" + grpId;
                                    //                string Responce = HttpHelper.getHtmlfromUrl(new Uri(Url_compose));

                                    //                string postUrlFinal = "https://www.linkedin.com/inbox/mailbox/message/send";
                                    //                string PostDataFinal = "senderEmail=914640570&ccInput=&subject=" + Uri.EscapeDataString(tempsubject.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&persist=true&showRecipients=showRecipients&isReply=&isForward=&itemId=&recipients=" + ToCd + "&recipientNames=%5B%7B%22memberId%22%3A" + ToCd + "%2C%22fullName%22%3A%22" + ContactName + "%22%7D%5D&groupId=" + grpId + "&csrfToken=" + csrfToken + "&sourceAlias=0_3mbGc9okCQbybxvc2A5Vz5&submit=Send+Message";


                                    //                ResponseStatusMsg = HttpHelper.postFormData(new Uri(postUrlFinal), PostDataFinal);

                                    //                if (ResponseStatusMsg.Contains("upload_error") && ResponseStatusMsg.Contains("There was an error with the file upload. Please try again later"))
                                    //                {
                                    //                    PostMessage = "csrfToken=" + csrfToken + "&subject=" + Uri.EscapeDataString(tempsubject.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&submit=Send+Message&fromName=" + Uri.EscapeDataString(FromEmailNam) + "&showRecipeints=showRecipeints&fromEmail=" + FromemailId + "&connectionIds=" + connId + "&connectionNames=&allowEditRcpts=true&addMoreRcpts=false&openSocialAppBodySuffix=&st=&viewerDestinationUrl=&contentType=MEBC&groupID=" + grpId + "";
                                    //                    ResponseStatusMsg = HttpHelper.postFormData(new Uri("https://www.linkedin.com/groupMsg"), PostMessage);
                                    //                }

                                    //                #region MyRegion

                                    //                #endregion
                                    //            }
                                    //            catch (Exception ex)
                                    //            {
                                    //            }
                                    //        }


                                    //        ResponseStatusMsg = HttpHelper.postFormData(new Uri("https://www.linkedin.com/groupMsg"), PostMessage);
                                    //    }
                                    //    catch (Exception ex)
                                    //    {

                                    //    }
                                    //}

                                    #endregion

                                    if ((ResponseStatusMsg.Contains("Your message was successfully sent.")) || (ResponseStatusMsg.Contains("Se ha enviado tu mensaje satisfactoriamente") || (ResponseStatusMsg.Contains("Uw bericht is verzonden")) || ResponseStatusMsg.Contains("success")) || ResponseStatusMsg.Contains("inmCategory"))
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Subject Posted : " + tempsubject + " ]");
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Body Text Posted : " + tempBody.ToString() + " ]");
                                        if (IssendInMail)
                                        {
                                            GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ InMail  Posted To Account: " + ContactName + " ]");
                                        }
                                        else
                                        {
                                            GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Message Posted To Account: " + ContactName + " ]");
                                        }

                                        ReturnString = "Your message was successfully sent.";

                                        #region CSV
                                        string bdy = string.Empty;
                                        try
                                        {
                                            bdy = body.ToString().Replace("\r", string.Empty).Replace("\n", " ").Replace(",", " ");
                                        }
                                        catch { }
                                        if (string.IsNullOrEmpty(bdy))
                                        {
                                            bdy = tempBody.ToString().Replace(",", ":");
                                        }
                                        string CSVHeader = "UserName" + "," + "Subject" + "," + "Body Text" + "," + "ContactName";
                                        string CSV_Content = UserName + "," + tempsubject + "," + bdy.ToString() + "," + ContactName;
                                        CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, FilePath.path_MessageSentGroupMember);

                                        try
                                        {
                                            InsertMsgGroupMemData(UserName, Convert.ToInt32(connId), ContactName, Convert.ToInt32(grpId), SelectedGrpName, msg, tempBody);
                                        }
                                        catch { }

                                        #endregion

                                    }
                                    else if (ResponseStatusMsg.Contains("There was an unexpected problem that prevented us from completing your request"))
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ There was an unexpected problem that prevented us from completing your request ! ]");
                                        GlobusFileHelper.AppendStringToTextfileNewLine("Error In Message Posting", FilePath.path_MessageGroupMember);
                                    }
                                    else if (ResponseStatusMsg.Contains("You are no longer authorized to message this"))
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ You are no longer authorized to message this ! ]");
                                        GlobusFileHelper.AppendStringToTextfileNewLine("Error In Message Posting", FilePath.path_MessageGroupMember);
                                    }
                                    else if ((ResponseStatusMsg.Contains("Already Sent")) || (ResponseStatusMsg.Contains("Ya ha sido enviada")))
                                    {
                                        string bdy = string.Empty;
                                        try
                                        {
                                            bdy = body.ToString().Replace("\r", string.Empty).Replace("\n", " ").Replace(",", " ");
                                        }
                                        catch { }
                                        if (string.IsNullOrEmpty(bdy))
                                        {
                                            bdy = tempBody.ToString().Replace(",", ":");
                                        }
                                        string CSVHeader = "UserName" + "," + "Subject" + "," + "Body Text" + "," + "ContactName";
                                        string CSV_Content = UserName + "," + msg + "," + bdy.ToString() + "," + ContactName;
                                        CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, FilePath.path_MessageAlreadySentGroupMember);

                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Message Not Posted To Account: " + ContactName + " because it has sent the same message]");
                                    }
                                    else
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Error In Message Posting ]");
                                        GlobusFileHelper.AppendStringToTextfileNewLine("Error In Message Posting", FilePath.path_MessageGroupMember);
                                    }

                                    int delay = RandomNumberGenerator.GenerateRandom(mindelay, maxdelay);
                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Delay for : " + delay + " Seconds ]");
                                    Thread.Sleep(delay * 1000);

                                }
                                catch (Exception ex)
                                {
                                    //Log("[ " + DateTime.Now + " ] => [ Error:" + ex.Message + "StackTrace --> >>>" + ex.StackTrace + " ]");
                                    GlobusFileHelper.AppendStringToTextfileNewLine(" Error:" + ex.Message + "StackTrace --> >>>" + ex.StackTrace, FilePath.path_MessageGroupMember);
                                }
                            }
                            catch (Exception ex)
                            {
                                //Log("[ " + DateTime.Now + " ] => [ Error:" + ex.Message + "StackTrace --> >>>" + ex.StackTrace);
                                GlobusFileHelper.AppendStringToTextfileNewLine(" Error:" + ex.Message + "StackTrace --> >>>" + ex.StackTrace, FilePath.path_MessageGroupMember);
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info("Exception ex : " + ex);
                }



            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Exception : " + ex);
            }

        }
        #endregion


        #region InsertScarppRecordData
        public void InsertMsgGroupMemData(string msgFrom, int msgToID, string msgTo, int msgGroupID, string msgGroupName, string msgSub, string msgBody)
        {
            msgFrom = msgFrom.Replace("'", "''");
            msgTo = msgTo.Replace("'", "''");
            msgGroupName = msgGroupName.Replace("'", "''");
            msgSub = msgSub.Replace("'", "''");
            msgBody = msgBody.Replace("'", "''");

            try
            {
                string strQuery = "INSERT INTO tb_ManageMsgGroupMem (MsgFrom,MsgToId,MsgTo,MsgGroupId,MsgGroupName,MsgSubject,MsgBody,DateTime) VALUES('" + msgFrom + "'," + msgToID + ",'" + msgTo + "'," + msgGroupID + ",'" + msgGroupName + "','" + msgSub + "','" + msgBody + "','" + DateTime.Now + "')";
                DataBaseHandler.InsertQuery(strQuery, "tb_ManageMsgGroupMem");
            }
            catch (Exception)
            { }
        }
        #endregion

        #region FromEmailCodeMsgGroupMem
        public string FromEmailCodeMsgGroupMem(ref GlobusHttpHelper HttpHelper, string gid)
        {
            string FromId = string.Empty;
            string pageSource = string.Empty;
            string[] RgxGroupData = new string[] { };
            List<string> lstpasttitle = new List<string>();
            List<string> checkpasttitle = new List<string>();
            string GroupName = string.Empty;
            string csrfToken = string.Empty;
            string[] RgxSikValue = new string[] { };
            string[] RgxPageNo = new string[] { };
            string sikvalue = string.Empty;


            try
            {
                string pageSource1 = HttpHelper.getHtmlfromUrl(new Uri("http://www.linkedin.com/home?trk=hb_tab_home_top"));

                if (pageSource1.Contains("csrfToken"))
                {
                    csrfToken = pageSource1.Substring(pageSource1.IndexOf("csrfToken"), 50);
                    string[] Arr = csrfToken.Split('>');
                    csrfToken = Arr[0];
                    csrfToken = csrfToken.Replace(":", "%3A").Replace("csrfToken", "").Replace("\"", string.Empty).Replace("value", string.Empty).Replace("cs", string.Empty).Replace("id", string.Empty).Replace("=", string.Empty).Replace("\n", string.Empty).Replace(">", string.Empty).Replace("<script src", string.Empty);
                    csrfToken = csrfToken.Trim();
                }

                pageSource = HttpHelper.getHtmlfromUrl(new Uri("http://www.linkedin.com/groups?viewMembers=&gid=" + gid));
                RgxSikValue = System.Text.RegularExpressions.Regex.Split(pageSource, "sik");

                try
                {
                    sikvalue = RgxSikValue[1].Split('&')[0].Replace("=", string.Empty);
                }
                catch { }

                try
                {
                    if (NumberHelper.ValidateNumber(sikvalue))
                    {
                        sikvalue = sikvalue.Split('\"')[0];
                    }
                    else
                    {
                        sikvalue = sikvalue.Split('\"')[0];
                    }
                }
                catch
                {
                    sikvalue = sikvalue.Split('\"')[0];
                }


                string getdata = "http://www.linkedin.com/groups?viewMembers=&gid=" + gid + "&sik=" + sikvalue + "&split_page=1";
                pageSource = HttpHelper.getHtmlfromUrl(new Uri(getdata));
                RgxGroupData = System.Text.RegularExpressions.Regex.Split(pageSource, "name=\"fromEmail\"");

                try
                {
                    FromId = RgxGroupData[1].Split('=')[1];
                    FromId = FromId.Replace("id", string.Empty).Replace("\"", string.Empty).Trim().ToString();
                }
                catch { }
            }
            catch
            {
                return FromId;
            }


            return FromId;
        }
        #endregion

        #region FromName
        public string FromName(ref GlobusHttpHelper HttpHelper)
        {
            try
            {
                string FromNm = string.Empty;

                string pageSource = HttpHelper.getHtmlfromUrl(new Uri("http://www.linkedin.com/profile/edit?trk=nav_responsive_sub_nav_edit_profile"));

                string[] RgxGroupData = System.Text.RegularExpressions.Regex.Split(pageSource, "fmt_full_display_name");


                foreach (var fromname in RgxGroupData)
                {
                    if (fromname.Contains("\":\""))
                    {
                        try
                        {
                            if (!(fromname.Contains("<!DOCTYPE html>")))
                            {
                                try
                                {
                                    int StartIndex = fromname.IndexOf("\":\"");
                                    string start = fromname.Substring(StartIndex);
                                    int endIndex = start.IndexOf("i18n_optional_not_pinyin");
                                    FromNm = start.Substring(0, endIndex).Replace("\"", string.Empty).Replace("\":\"", string.Empty);
                                    FromNam = FromNm.Split(',')[0].Replace(":", string.Empty).Replace("\\u002d", "-");
                                }
                                catch
                                { }
                                try
                                {
                                    if (string.IsNullOrEmpty(FromNm) || FromNm.Contains("LastName"))
                                    {
                                        int StartIndex = fromname.IndexOf("\":\"");
                                        string start = fromname.Substring(StartIndex);
                                        int endIndex = start.IndexOf(",");
                                        FromNm = start.Substring(0, endIndex).Replace("\"", string.Empty).Replace("\":\"", string.Empty);
                                        FromNam = FromNm.Split(',')[0].Replace(":", string.Empty);
                                    }
                                }
                                catch
                                { }
                            }
                        }
                        catch { }
                    }
                }



                return FromNam;


            }
            catch (Exception ex)
            {
                return FromNam;
            }



        }
        #endregion



    }
}
