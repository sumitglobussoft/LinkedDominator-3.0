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

namespace Groups
{
    public class GroupStatus
    {
        public static Dictionary<string, string> GroupSpecMem = new Dictionary<string, string>();
        public static Events CampaignStopLogevents = new Events();

        public static void ComboBoxDataBind(string log)
        {
            EventsArgs eArgs = new EventsArgs(log);
            CampaignStopLogevents.LogText(eArgs);

        }

        #region Get Groups
        readonly object lockrThreadControllerGroupStatus = new object();
        public void ThreadStartGroupStatus()
        {
            try
            {
                int numberOfAccountPatch = 25;

                if (GlobalsGroups.NoOfThreadsGroupStatus > 0)
                {
                    numberOfAccountPatch = GlobalsGroups.NoOfThreadsGroupStatus;
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
                                lock (lockrThreadControllerGroupStatus)
                                {
                                    try
                                    {
                                        if (GlobalsGroups.countThreadControllerGroupStatus >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerGroupStatus);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        LinkedinUser item = null;
                                        LDGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {
                                            Thread profilerThread = new Thread(StartMultiThreadsGroupStatus);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;

                                            profilerThread.Start(new object[] { item });

                                            GlobalsGroups.countThreadControllerGroupStatus++;
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
        public void StartMultiThreadsGroupStatus(object parameters)
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
                            StartGroupStatus(ref objLinkedinUser);
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
                        lock (lockrThreadControllerGroupStatus)
                        {
                            GlobalsGroups.countThreadControllerGroupStatus = GlobalsGroups.countThreadControllerGroupStatus--;
                            Monitor.Pulse(lockrThreadControllerGroupStatus);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }
        public void StartGroupStatus(ref LinkedinUser objLinkedinUser)
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
            objEvents.BindGroupMembersToCheckedListBox(objEventsArgs);
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

        #region Send Update
        public static readonly object Locked_GrpKey_Post = new object();
        public static readonly object Locked_Que_GrpAttachLink_Post = new object();
        public static readonly object Locked_GrpMoreDtl_Post = new object();
        public static readonly object Locked_GrpPostTitle_Post = new object();
        readonly object lockrThreadControllerSendUpdate = new object();
        public void ThreadStartSendUpdate()
        {
            try
            {
                int numberOfAccountPatch = 25;

                if (GlobalsGroups.NoOfThreadsSendUpdate > 0)
                {
                    numberOfAccountPatch = GlobalsGroups.NoOfThreadsSendUpdate;
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
                                lock (lockrThreadControllerSendUpdate)
                                {
                                    try
                                    {
                                        if (GlobalsGroups.countThreadControllerSendUpdate >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerSendUpdate);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        LinkedinUser item = null;
                                        LDGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {
                                            Thread profilerThread = new Thread(StartMultiThreadsSendUpdate);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;

                                            profilerThread.Start(new object[] { item });

                                            GlobalsGroups.countThreadControllerSendUpdate++;
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
        public void StartMultiThreadsSendUpdate(object parameters)
        {
            try
            {
                if (!GlobalsGroups.isStopSendUpdate)
                {
                    try
                    {
                        GlobalsGroups.lstThreadsSendUpdate.Add(Thread.CurrentThread);
                        GlobalsGroups.lstThreadsSendUpdate.Distinct();
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
                            StartSendUpdate(ref objLinkedinUser);
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
                        lock (lockrThreadControllerSendUpdate)
                        {
                            GlobalsGroups.countThreadControllerSendUpdate = GlobalsGroups.countThreadControllerSendUpdate--;
                            Monitor.Pulse(lockrThreadControllerSendUpdate);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        public void StartSendUpdate(ref LinkedinUser objLinkedinUser)
        {
            try
            {
                int numberOfThreads = 0;

                if (GlobalsGroups.ListGrpDiscussion.Count > 0)
                {
                    foreach (string Message in GlobalsGroups.ListGrpDiscussion)
                    {
                        GlobalsGroups.Que_GrpPostTitle_Post.Enqueue(Message);
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Please Add Group Title Message Text ]");
                    return;
                }

                if (GlobalsGroups.GroupUrl.Count > 0)
                {
                    foreach (string grpKey in GlobalsGroups.GroupUrl)
                    {
                        GlobalsGroups.Que_GrpKey_Post.Enqueue(grpKey);
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Group User Key Invalid ]");
                    return;
                }

                if (GlobalsGroups.ListGrpMoreDetails.Count > 0)
                {
                    foreach (string grpmoredtl in GlobalsGroups.ListGrpMoreDetails)
                    {
                        GlobalsGroups.Que_GrpMoreDtl_Post.Enqueue(grpmoredtl);
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Please Add Group MoreDetails Message Text ]");
                    return;
                }

                //if (chkSameMessageForAllGroup.Checked)
                {
                    GlobalsGroups.Que_GrpPostTitle_Post.Clear();
                    GlobalsGroups.Que_GrpMoreDtl_Post.Clear();
                    if (GlobalsGroups.ListGrpDiscussion.Count > 0)
                    {
                        foreach (string Message in GlobalsGroups.ListGrpDiscussion)
                        {
                            GlobalsGroups.Que_GrpPostTitle_Post.Enqueue(Message);
                            break;
                        }
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Please Add Group Title Message Text ]");
                        return;
                    }



                    if (GlobalsGroups.ListGrpMoreDetails.Count > 0)
                    {
                        foreach (string grpmoredtl in GlobalsGroups.ListGrpMoreDetails)
                        {
                            GlobalsGroups.Que_GrpMoreDtl_Post.Enqueue(grpmoredtl);
                            break;
                        }
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Please Add Group MoreDetails Message Text ]");
                        return;
                    }
                }

                PostGroupMsgUpdate(ref objLinkedinUser);
            }
            catch (Exception ex)
            {
                GlobusFileHelper.AppendStringToTextfileNewLine("DateTime :- " + DateTime.Now + " :: Error --> Group Update --> LinkedInGroupMessage() >>>> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace, FilePath.Path_LinkedinErrorLogs);
                GlobusFileHelper.AppendStringToTextfileNewLine("DateTime :- " + DateTime.Now + " :: Error --> Group Update --> LinkedInGroupMessage() >>>> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace, FilePath.Path_LinkedinGetGroupMemberErrorLogs);
            }
        }

        public void PostGroupMsgUpdate(ref LinkedinUser objLnkedinUser)
        {
            try
            {
                if (objLnkedinUser.isloggedin)
                {
                    GlobusHttpHelper HttpHelper = objLnkedinUser.globusHttpHelper;
                    string postdata = string.Empty;
                    string postUrl = string.Empty;
                    string ResLogin = string.Empty;
                    string csrfToken = string.Empty;
                    string sourceAlias = string.Empty;
                    string referal = string.Empty;

                    string ReturnString = string.Empty;
                    string PostGrpDiscussion = string.Empty;
                    string PostGrpMoreDetails = string.Empty;
                    string PostGrpAttachLink = string.Empty;
                    string PostGrpKey = string.Empty;

                    try
                    {
                        string MessageText = string.Empty;
                        string PostedMessage = string.Empty;
                        string pageSource = string.Empty;
                        
                        pageSource = HttpHelper.getHtmlfromUrl1(new Uri("http://www.linkedin.com/home?trk=hb_tab_home_top"));

                        if (pageSource.Contains("csrfToken"))
                        {
                            string pattern = @"\";
                            csrfToken = pageSource.Substring(pageSource.IndexOf("csrfToken"), 50);
                            string[] Arr = csrfToken.Split('&');
                            csrfToken = Arr[0];
                            csrfToken = csrfToken.Replace("csrfToken", "").Replace("\"", string.Empty).Replace("value", string.Empty).Replace("cs", string.Empty).Replace("id", string.Empty).Replace("=", string.Empty);
                            csrfToken = csrfToken.Replace(pattern, string.Empty.Trim());
                        }

                        if (pageSource.Contains("sourceAlias"))
                        {
                            string pattern1 = @"\";
                            sourceAlias = pageSource.Substring(pageSource.IndexOf("sourceAlias"), 100);
                            string[] Arr = sourceAlias.Split('"');
                            sourceAlias = Arr[2];
                            sourceAlias = sourceAlias.Replace(pattern1, string.Empty.Trim());
                        }

                        try
                        {

                            foreach (var Itegid in GlobalsGroups.SelectedGroups)
                            {
                                string[] grpNameWithDetails = Itegid.Split('^');

                                try
                                {
                                    lock (Locked_GrpKey_Post)
                                    {
                                        if (GlobalsGroups.Que_GrpKey_Post.Count > 0)
                                        {
                                            try
                                            {
                                                PostGrpKey = GlobalsGroups.Que_GrpKey_Post.Dequeue();
                                            }
                                            catch { }
                                        }
                                    }

                                    lock (Locked_GrpPostTitle_Post)
                                    {
                                        if (GlobalsGroups.Que_GrpPostTitle_Post.Count > 0)
                                        {
                                            try
                                            {
                                                PostGrpDiscussion = GlobalsGroups.Que_GrpPostTitle_Post.Dequeue();
                                            }
                                            catch { }
                                        }
                                    }

                                    lock (Locked_GrpMoreDtl_Post)
                                    {
                                        if (GlobalsGroups.Que_GrpMoreDtl_Post.Count > 0)
                                        {
                                            try
                                            {
                                                PostGrpMoreDetails = GlobalsGroups.Que_GrpMoreDtl_Post.Dequeue();
                                                //Que_GrpMoreDtl_Post.Clear();
                                            }
                                            catch { }
                                        }


                                    }

                                    lock (Locked_Que_GrpAttachLink_Post)
                                    {
                                        if (GlobalsGroups.Que_GrpAttachLink_Post.Count > 0)
                                        {
                                            try
                                            {
                                                PostGrpAttachLink = GlobalsGroups.Que_GrpAttachLink_Post.Dequeue();
                                            }
                                            catch { }
                                        }

                                    }

                                    string[] grpDisplay = Itegid.Split('^');
                                    string GrpName = grpDisplay[0].ToString().Replace("[", string.Empty).Trim();
                                    string[] PostGid = grpDisplay[1].Replace("]", string.Empty).Split(',');
                                    string Gid = string.Empty;

                                    //        HJKHKJH

                                    try
                                    {
                                        if (NumberHelper.ValidateNumber(PostGid[1].Trim()))
                                        {
                                            Gid = PostGid[1].Trim();
                                        }
                                        else if (NumberHelper.ValidateNumber(PostGid[2].Trim()))
                                        {
                                            Gid = PostGid[2].Trim();
                                        }
                                        else if (NumberHelper.ValidateNumber(PostGid[3].Trim()))
                                        {
                                            Gid = PostGid[3].Trim();
                                        }
                                        else if (NumberHelper.ValidateNumber(PostGid[4].Trim()))
                                        {
                                            Gid = PostGid[4].Trim();
                                        }
                                        else if (NumberHelper.ValidateNumber(PostGid[5].Trim()))
                                        {
                                            Gid = PostGid[5].Trim();
                                        }
                                        else if (NumberHelper.ValidateNumber(PostGid[6].Trim()))
                                        {
                                            Gid = PostGid[6].Trim();
                                        }
                                        else if (NumberHelper.ValidateNumber(PostGid[7].Trim()))
                                        {
                                            Gid = PostGid[7].Trim();
                                        }
                                        else if (NumberHelper.ValidateNumber(PostGid[8].Trim()))
                                        {
                                            Gid = PostGid[8].Trim();
                                        }
                                        else if (NumberHelper.ValidateNumber(PostGid[9].Trim()))
                                        {
                                            Gid = PostGid[9].Trim();
                                        }
                                        else if (NumberHelper.ValidateNumber(PostGid[10].Trim()))
                                        {
                                            Gid = PostGid[10].Trim();
                                        }
                                        else if (NumberHelper.ValidateNumber(PostGid[11].Trim()))
                                        {
                                            Gid = PostGid[11].Trim();
                                        }
                                        else if (NumberHelper.ValidateNumber(PostGid[12].Trim()))
                                        {
                                            Gid = PostGid[12].Trim();
                                        }
                                    }
                                    catch { }

                                    //string ReqUrl = PostGrpAttachLink;
                                    string ReqUrl = PostGrpMoreDetails;
                                    ReqUrl = ReqUrl.Replace(":", "%3A").Replace("//", "%2F%2F");
                                    referal = "http://www.linkedin.com/groups/" + grpDisplay[2].Replace(" ", "-") + "-" + Gid + "?goback=%2Egmr_" + Gid;
                                    string GetStatus = HttpHelper.getHtmlfromUrl(new Uri("http://www.linkedin.com/share?getPreview=&url=" + ReqUrl), referal);

                                    string ImgCount = string.Empty;
                                    try
                                    {
                                        int StartinImgCnt = GetStatus.IndexOf("current");
                                        string startImgCnt = GetStatus.Substring(StartinImgCnt);
                                        int EndIndexImgCnt = startImgCnt.IndexOf("</span>");
                                        string EndImgCnt = startImgCnt.Substring(0, EndIndexImgCnt).Replace("value\":", "").Replace("\"", "");
                                        ImgCount = EndImgCnt.Replace("current", string.Empty).Replace(">", string.Empty);
                                    }
                                    catch
                                    {
                                        ImgCount = "0";
                                    }

                                    string LogoUrl = string.Empty;
                                    try
                                    {
                                        int StartinImgUrl = GetStatus.IndexOf("url");
                                        string startImgUrl = GetStatus.Substring(StartinImgUrl);
                                        int EndIndexImgUrl = startImgUrl.IndexOf("border=");
                                        string EndImgUrl = startImgUrl.Substring(0, EndIndexImgUrl).Replace("value\":", "").Replace("\"", "");
                                        LogoUrl = EndImgUrl.Replace("url=", string.Empty).Trim();
                                    }
                                    catch
                                    {
                                        LogoUrl = "false";
                                    }

                                    string EntityId = string.Empty;
                                    try
                                    {
                                        int StartinEntityId = GetStatus.IndexOf("data-entity-id");
                                        string startEntityId = GetStatus.Substring(StartinEntityId);
                                        int EndIndexEntityId = startEntityId.IndexOf("data-entity-url");
                                        string EndEntityId = startEntityId.Substring(0, EndIndexEntityId).Replace("value\":", "").Replace("\"", "");
                                        EntityId = EndEntityId.Replace("\"", string.Empty).Replace("data-entity-id", string.Empty).Replace("=", string.Empty).Trim();
                                    }
                                    catch { }

                                    string contentTitle = string.Empty;
                                    try
                                    {
                                        int StartinContent = GetStatus.IndexOf("share-view-title");
                                        string startContent = GetStatus.Substring(StartinContent);
                                        int EndIndexContent = startContent.IndexOf("</h4>");
                                        string EndContent = startContent.Substring(0, EndIndexContent).Replace("value\":", "").Replace("\"", "");
                                        contentTitle = EndContent.Replace("\"", string.Empty).Replace("\n", string.Empty).Replace("share-view-title", string.Empty).Replace("id=", string.Empty).Replace(">", string.Empty).Replace("&", "and").Replace("amp;", string.Empty).Trim();

                                        if (contentTitle.Contains("#"))
                                        {
                                            contentTitle = contentTitle.Replace("and", "&");
                                            contentTitle = Uri.EscapeDataString(contentTitle);
                                        }

                                    }
                                    catch { }

                                    string contentSummary = string.Empty;
                                    try
                                    {
                                        int StartinConSumm = GetStatus.IndexOf("share-view-summary\">");
                                        string startConSumm = GetStatus.Substring(StartinConSumm);
                                        int EndIndexConSumm = startConSumm.IndexOf("</span>");
                                        string EndConSumm = startConSumm.Substring(0, EndIndexConSumm).Replace("value\":", "").Replace("\"", "");
                                        contentSummary = EndConSumm.Replace("\"", string.Empty).Replace("\n", string.Empty).Replace("share-view-summary", string.Empty).Replace("id=", string.Empty).Replace(">", string.Empty).Replace("</span<a href=#", string.Empty).Trim();
                                        contentSummary = contentSummary.Replace(",", "%2C").Replace(" ", "%20");

                                        if (contentSummary.Contains("#"))
                                        {
                                            contentSummary = contentSummary.Replace("and", "&");
                                            contentSummary = Uri.EscapeDataString(contentSummary);
                                        }
                                    }
                                    catch { }

                                    string PostGroupstatus = string.Empty;
                                    string ResponseStatusMsg = string.Empty;
                                    csrfToken = csrfToken.Replace("<meta http-", "").Replace(">", "").Trim();
                                    try
                                    {
                                        //PostGroupstatus = "csrfToken=" + csrfToken + "&postTitle=" + PostGrpDiscussion + "&postText=" + PostGrpMoreDetails + "&pollChoice1-ANetPostForm=&pollChoice2-ANetPostForm=&pollChoice3-ANetPostForm=&pollChoice4-ANetPostForm=&pollChoice5-ANetPostForm=&pollEndDate-ANetPostForm=0&contentImageCount=0&contentImageIndex=-1&contentImage=&contentEntityID=&contentUrl=&contentTitle=&contentSummary=&contentImageIncluded=true&%23=&gid=" + Gid.Trim() + "&postItem=&ajax=true&tetherAccountID=&facebookTetherID=";
                                        PostGroupstatus = "csrfToken=" + csrfToken + "&postTitle=" + PostGrpDiscussion + "&postText=" + PostGrpMoreDetails + "&pollChoice1-ANetPostForm=&pollChoice2-ANetPostForm=&pollChoice3-ANetPostForm=&pollChoice4-ANetPostForm=&pollChoice5-ANetPostForm=&pollEndDate-ANetPostForm=0&contentImageCount=" + ImgCount + "&contentImageIndex=-1&contentImage=" + LogoUrl + "&contentEntityID=" + EntityId + "&contentUrl=" + ReqUrl + "&contentTitle=" + contentTitle + "&contentSummary=" + contentSummary + "&contentImageIncluded=true&%23=&gid=" + Gid + "&postItem=&ajax=true&tetherAccountID=&facebookTetherID=";
                                        ResponseStatusMsg = HttpHelper.postFormData(new Uri("http://www.linkedin.com/groups"), PostGroupstatus);
                                    }
                                    catch { }
                                    #region written by sharan
                                    try
                                    {
                                        string PagesourceProfile = HttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com/profile/view?id=394473043&trk=nav_responsive_tab_profile"));
                                        string GoBackValue = string.Empty;
                                        string Url = string.Empty;
                                        if (PagesourceProfile.Contains("&goback="))
                                        {
                                            GoBackValue = Utils.getBetween(PagesourceProfile, "&goback=", "&").Trim();
                                            Url = "https://www.linkedin.com/grp/home?gid=" + Gid + "&goback=" + GoBackValue;
                                        }

                                        PostGroupstatus = "csrfToken=" + csrfToken + "&title=" + PostGrpDiscussion + "&details=" + PostGrpMoreDetails + "&groupId=" + Gid + "&displayCategory=DISCUSSION";
                                        ResponseStatusMsg = HttpHelper.postFormDataRef(new Uri("https://www.linkedin.com/grp/postForm/submit"), PostGroupstatus, Url, "", "");
                                    }
                                    catch (Exception ex)
                                    {
                                    }

                                    #endregion

                                    string CSVHeader = "UserName" + "," + "HeaderPost" + "," + "Details Post" + "," + "ToGroup";

                                    if (ResponseStatusMsg.Contains("SUCCESS") || ResponseStatusMsg.Contains("Accept  the description According to you"))
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Message Header Posted : " + PostGrpDiscussion + " Successfully on Group : " + grpDisplay[2] + " ]");
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Message More Details Posted : " + PostGrpMoreDetails + " Successfully on Group : " + grpDisplay[2] + " ]");

                                        string CSV_Content = objLnkedinUser.username + "," + PostGrpDiscussion.Replace(",", ";") + "," + PostGrpMoreDetails.Replace(",", ";") + "," + grpDisplay[2].Replace(",", string.Empty);
                                        CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, FilePath.path_GroupUpdates);

                                    }
                                    else if (ResponseStatusMsg.Contains("Your request to join is still pending"))
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Your membership is pending approval on a Group:" + grpDisplay[2] + " ]");
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Message Header: " + PostGrpDiscussion + " Not Posted on Group:" + grpDisplay[2] + " Because Your membership is pending for approval. ]");
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Message More Details: " + PostGrpMoreDetails + " Not Posted on Group:" + grpDisplay[2] + " Because Your membership is pending for approval. ]");

                                        GlobusFileHelper.AppendStringToTextfileNewLine("Your membership is pending approval on a Group:" + grpDisplay[2], FilePath.path_GroupUpdate);
                                        GlobusFileHelper.AppendStringToTextfileNewLine("Message Header: " + PostGrpDiscussion + " Not Posted on Group:" + grpDisplay[2] + " Because Your membership is pending for approval. ", FilePath.path_GroupUpdate);
                                        GlobusFileHelper.AppendStringToTextfileNewLine("Message More Details: " + PostGrpMoreDetails + " Not Posted on Group:" + grpDisplay[2] + " Because Your membership is pending for approval. ", FilePath.path_GroupUpdate);
                                    }
                                    else if (ResponseStatusMsg.Contains("Your post has been submitted for review"))
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Message Header Posted : " + PostGrpDiscussion + " Successfully on Group : " + grpDisplay[2] + " ]");
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Message More Details Posted : " + PostGrpMoreDetails + " Successfully on Group : " + grpDisplay[2] + " ]");
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Your post has been submitted for review ]");
                                        string CSV_Content = objLnkedinUser.username + "," + PostGrpDiscussion.Replace(",", ";") + "," + PostGrpMoreDetails.Replace(",", ";") + "," + grpDisplay[2];

                                    }
                                    else if (ResponseStatusMsg.Contains("Error"))
                                    {
                                        //Log("[ " + DateTime.Now + " ] => [ Error in Post ]");
                                        GlobusFileHelper.AppendStringToTextfileNewLine("Error in Post", FilePath.path_GroupUpdate);

                                    }
                                    else
                                    {
                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Message Not Posted ]");
                                        GlobusFileHelper.AppendStringToTextfileNewLine("Message Not Posted", FilePath.path_GroupUpdate);
                                    }

                                    int delay = RandomNumberGenerator.GenerateRandom(GlobalsGroups.minDelay, GlobalsGroups.maxDelay);
                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Delay for : " + delay + " Seconds ]");
                                    Thread.Sleep(delay * 1000);

                                }
                                catch (Exception ex)
                                {
                                    GlobusFileHelper.AppendStringToTextfileNewLine("DateTime :- " + DateTime.Now + " :: Error --> Group Update --> cmbGroupUser_SelectedIndexChanged() ---1--->>>> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace + "    Stack Trace >>> " + ex.StackTrace, FilePath.Path_LinkedinGetGroupMemberErrorLogs);
                                    //Log("[ " + DateTime.Now + " ] => [ Error:" + ex.Message + "StackTrace --> >>>" + ex.StackTrace + " ]");
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            GlobusFileHelper.AppendStringToTextfileNewLine("DateTime :- " + DateTime.Now + " :: Error --> Group Update --> cmbGroupUser_SelectedIndexChanged() ---1--->>>> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace, FilePath.Path_LinkedinErrorLogs);
                            GlobusFileHelper.AppendStringToTextfileNewLine("DateTime :- " + DateTime.Now + " :: Error --> Group Update --> cmbGroupUser_SelectedIndexChanged() ---1--->>>> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace, FilePath.Path_LinkedinGetGroupMemberErrorLogs);
                            // Log("[ " + DateTime.Now + " ] => [ Error:" + ex.Message + "StackTrace --> >>>" + ex.StackTrace + " ]");
                        }

                    }
                    catch (Exception ex)
                    {
                        GlobusFileHelper.AppendStringToTextfileNewLine("DateTime :- " + DateTime.Now + " :: Error --> Group Update --> cmbGroupUser_SelectedIndexChanged() ---2--->>>> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace, FilePath.Path_LinkedinErrorLogs);
                        GlobusFileHelper.AppendStringToTextfileNewLine("DateTime :- " + DateTime.Now + " :: Error --> Group Update --> cmbGroupUser_SelectedIndexChanged() ---2--->>>> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace, FilePath.Path_LinkedinGetGroupMemberErrorLogs);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }
}

