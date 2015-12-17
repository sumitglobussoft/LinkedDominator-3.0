using Accounts;
using BaseLib;
using Globussoft;
using Groups;
using LinkDominator;
using linkedDominator;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;



namespace Messages
{
    public class ComposeMessage
    {
        public static Events CampaignStopLogevents = new Events();
        public static void ComboBoxDataBind(string log)
        {
            EventsArgs eArgs = new EventsArgs(log);
            CampaignStopLogevents.LogText(eArgs);
        }

        public static bool isExcelInput = false;

        public static Dictionary<string, string> MemberNameAndID = new Dictionary<string, string>();

        public static List<string[]> Cmpmsg_excelData = new List<string[]>();

        public static bool IsStop=false;

        public static bool isSpintax = false;

        public static bool MesageWithTag = false;
        
        public static int ComposeMessageMinDelay = 0;
        public static int ComposeMessageMaxDelay = 0;

        public static string ComposeMessagesubject = string.Empty;
        public static string ComposeMessagebody = string.Empty;

        public static Dictionary<string, string> SlectedContacts1 = new Dictionary<string, string>();
        public static Dictionary<string, string> SlectedContacts2 = new Dictionary<string, string>();

        public static string SelectedAcc = string.Empty;
        public static List<string> SelectedMembers;
        List<string> subjectlistCompose = new List<string>();
        string msgBodycomposePass = string.Empty;
        bool msg_spintaxt = false;
        public bool preventMsgSameUser = false;
        public bool preventMsgGlobalUser = false;

        public string UserSlecetedDetails = string.Empty;

        public int NoOfThreadsGetConnections;
        readonly object lockrThreadControllerPostPicOnWall = new object();
        int countThreadControllerPostPicOnWall = 0;
        public bool isStopPostPicOnWall = false;
        public List<Thread> lstThreadsPostPicOnWall = new List<Thread>();


        public int countThreadControllerGetConnections
        {
            get;
            set;
        }

        public void startSendingMessage()
        {
            bool CheckNetConn = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

            if (CheckNetConn)
            {

                #region StoreIntoDB
                //try
                //{
                //    string query_delete = "delete from tb_composeMessagePreviousData";


                //    SQLiteConnection m_dbConnection;

                //    m_dbConnection = new SQLiteConnection(DataBaseHandler.CONstr);
                //    m_dbConnection.Open();

                //    //  string sql = "create table tb_checkDailyLimit (id integer primary key , DaillyLimit int)";

                //    SQLiteCommand command = new SQLiteCommand(query_delete, m_dbConnection);
                //    command.ExecuteNonQuery();


                //    string subject = txtMsgSubject.Text;
                //    string body = txtMsgBody.Text;
                //    string InsertQuery = "Insert into tb_composeMessagePreviousData(subject,body) values('" + subject + "','" + body + "')";
                //    DataBaseHandler.InsertQuery(InsertQuery, "tb_composeMessagePreviousData");
                //}
                //catch
                //{

                //}

                #endregion StoreIntoDB

                try
                {
                    UserSlecetedDetails = SelectedAcc;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info("Exception : " + ex.StackTrace);
                }

                try
                {
                   // MessagelistCompose.Clear();
                   // lstComposeMessageThread.Clear();
                    if (IsStop)
                    {
                        IsStop = false;
                    }

                    if (LDGlobals.loadedAccountsDictionary.Count() == 0)
                    {
                        GlobusLogHelper.log.Info("Please upload the accounts");
                        return;
                    }
                    if(string.IsNullOrEmpty(SelectedAcc))
                    {
                        GlobusLogHelper.log.Info("Please select an account from drop down menu");
                        return;
                    }
                    if (Globals.MessageContacts.Count==0)
                    {
                        GlobusLogHelper.log.Info("Please click on Get Members button");
                        return;
                    }
                    if(SelectedMembers.Count==0)
                    {
                        GlobusLogHelper.log.Info("Please select atleast one member");
                        return;
                    }
                    if(string.IsNullOrEmpty( ComposeMessagebody))
                    {
                        GlobusLogHelper.log.Info("Please add message body");
                        return;
                    }
                    if(string.IsNullOrEmpty( ComposeMessagesubject))
                    {
                        GlobusLogHelper.log.Info("Please add message subject");
                        return;
                    }

                   //if (!ChkAllAccounts_ComposeMessage.Checked)
                   // {

                   // }


                    if (ComposeMessagebody.Contains("|"))
                    {
                        if (!isSpintax)
                        {
                            GlobusLogHelper.log.Info(" Please Check SpinTax CheckBox.. ");
                            return;
                        }

                        if (ComposeMessagebody.Contains("{") || ComposeMessagebody.Contains("}"))
                        {
                            GlobusLogHelper.log.Info(" Its a wrong SpinTax Format.. ");
                            return;
                        }
                    }

                    if (isSpintax)
                    {
                        subjectlistCompose = SpinnedListGenerator.GetSpinnedList(new List<string> { ComposeMessagesubject });
                        //messageSpin = GlobusSpinHelper.spinLargeText(new Random(), txtMsgBody.Text);
                    }
                   
                    //if (ChkAllAccounts_ComposeMessage.Checked)
                    //{
                    //    try
                    //    {
                    //        ComposeMessage.ComposeMessage.IsAllAccounts = true;

                    //        try
                    //        {
                    //            ComposeMessage.ComposeMessage.NoOfFriends = Convert.ToInt32(TxtNoOfFriends_ComposeMessage.Text);
                    //        }
                    //        catch
                    //        {
                    //            AddLoggerComposeMessage("[ " + DateTime.Now + " ] => [ Please Enter Numeric Value In No. Of Friends Field ! ]");
                    //            ComposeMessage.ComposeMessage.NoOfFriends = 5;
                    //            TxtNoOfFriends_ComposeMessage.Text = "5";
                    //        }
                    //    }
                    //    catch
                    //    {
                    //    }
                    //}


                    new Thread(() =>
                    {
                        LinkedInComposeMessage();

                    }).Start();

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info("Exception : " + ex);
                }
            }
            else
            {
               
            }

        }

        
        private void LinkedInComposeMessage()
        {
            GlobusLogHelper.log.Info("Starting Sending Message To Selected Contacts ");
            int numberOfThreads = 0;

            try
            {
                try
                {
                    if (!IsStop)
                    {
                        Globals.lstComposeMessageThread.Add(Thread.CurrentThread);
                        Globals.lstComposeMessageThread.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                }
                catch(Exception ex)
                {
                    GlobusLogHelper.log.Info("Exception : " + ex);
                }
                

                if ( LDGlobals.loadedAccountsDictionary.Count() > 0)
                {
                    ThreadPool.SetMaxThreads(numberOfThreads, 5);
                    string value = string.Empty;
                    


                    foreach (KeyValuePair<string, LinkedinUser> item in LDGlobals.loadedAccountsDictionary)
                    {
                        //if (!ChkAllAccounts_ComposeMessage.Checked)
                        //{
                        //    if (item.Key == value)
                        //    {
                        //        PostMessageBulk(new object[] { item });
                        //    }

                        //}
                        //else
                        //{
                        if (item.Key == SelectedAcc)
                        {
                            try
                            {
                                ThreadPool.SetMaxThreads(numberOfThreads, 5);
                                ThreadPool.QueueUserWorkItem(new WaitCallback(PostMessageBulk), new object[] { item });
                                Thread.Sleep(1000);

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Exception : " + ex.StackTrace);
                            }
                        }
                       // }

                        #region old code

                        //if (value.Contains(item.Key))
                        //{
                        //    PostMessageBulk(new object[] { item });
                        //    //ThreadPool.SetMaxThreads(numberOfThreads, 5);
                        //    //ThreadPool.QueueUserWorkItem(new WaitCallback(PostMessageBulk), new object[] { item });
                        //    //Thread.Sleep(1000);
                        //} 

                        #endregion

                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Exception : " + ex);
            }
        }

        public void PostMessageBulk(object parameter)
        {
            try
            {
                if (!IsStop)
                {
                    Globals.lstComposeMessageThread.Add(Thread.CurrentThread);
                    Globals.lstComposeMessageThread.Distinct().ToList();
                    Thread.CurrentThread.IsBackground = true;
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Info("Exception ex : " + ex);
            }

            try
            {
                Array paramsArray = new object[1];
                paramsArray = (Array)parameter;
                KeyValuePair<string, LinkedinUser> item = (KeyValuePair<string, LinkedinUser>)paramsArray.GetValue(0);
                string source = string.Empty;
                LinkedinUser objLinkedinUser = item.Value;
                GlobusHttpHelper objGlobusHttpHelper = objLinkedinUser.globusHttpHelper;
                source = objGlobusHttpHelper.getHtmlfromUrl(new Uri( "https://www.linkedin.com"));
               
                if (!objLinkedinUser.isloggedin)
                {
                    objLinkedinUser.globusHttpHelper = objGlobusHttpHelper;
                    AccountManager objAccountManager = new AccountManager();
                    objAccountManager.LoginHttpHelper(ref objLinkedinUser);
                }

                GlobusLogHelper.log.Info(" Sending Message From Account : " + item.Key);
                string UserEmail = string.Empty;
                UserEmail = item.Key;

                try
                {
                    if (objLinkedinUser.isloggedin)
                    {
                        List<string> SelectedItem = new List<string>();
                        string Userid = SelectedAcc;

                        //GroupStatus MemberScrape = new GroupStatus();

                        string FromEmailId = FromEmailCodeComposeMsg(ref objGlobusHttpHelper, Userid);
                        string FromEmailName = FromName(ref objGlobusHttpHelper);
                        Dictionary<string, string> SelectedContacts = new Dictionary<string, string>();

                        foreach (KeyValuePair<string, Dictionary<string, string>> contacts in Globals.MessageContacts)
                        {
                            if (contacts.Key == item.Key)
                            {
                                foreach (KeyValuePair<string, string> Details in contacts.Value)
                                {
                                  //  foreach (string itemChecked in chkMessageTo.CheckedItems)
                                    foreach (string itemChecked in SelectedMembers)
                                    {

                                        if (itemChecked == Details.Value)
                                        {
                                            try
                                            {
                                                string id = Regex.Split(Details.Key, ":")[1];
                                                SelectedContacts.Add(id, Details.Value);
                                            }
                                            catch
                                            {

                                                SelectedContacts.Add(Details.Key, Details.Value);
                                            }
                                        }
                                        if (!(itemChecked == Details.Value))
                                        {
                                            try
                                            {
                                                string Value = Details.Value.Replace(",", string.Empty);
                                                if (itemChecked == Value)
                                                {
                                                    try
                                                    {
                                                        string id = Regex.Split(Details.Key, ":")[1];
                                                        SelectedContacts.Add(id, Details.Value);
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                }
                            }
                        }

                        string msgBodyCompose = ComposeMessagebody;
                        string msgSubCompose = ComposeMessagesubject;
                        if (isSpintax)
                        {
                            try
                            {
                               // msgBodyCompose = GlobusSpinHelper.spinLargeText(new Random(), msgBodyCompose);
                                msgBodycomposePass = msgBodyCompose;
                                msgSubCompose = subjectlistCompose[RandomNumberGenerator.GenerateRandom(0, subjectlistCompose.Count - 1)];
                            }
                            catch
                            {
                            }
                        }

                        if(ComposeMessageMinDelay==0 && ComposeMessageMaxDelay==0)
                        {
                            ComposeMessageMinDelay = 20;
                            ComposeMessageMaxDelay = 25;
                        }

                        if (MesageWithTag)
                        {
                            PostFinalMsg_WithTag(ref objGlobusHttpHelper, SelectedContacts, subjectlistCompose, msgBodycomposePass, msgSubCompose.ToString(), msgBodyCompose.ToString(), UserEmail, FromEmailId, FromEmailName, msg_spintaxt, ComposeMessageMinDelay, ComposeMessageMaxDelay, preventMsgSameUser, preventMsgGlobalUser);
                        }
                        else
                        {
                            PostFinalMsg(ref objGlobusHttpHelper, SelectedContacts, msgBodycomposePass, subjectlistCompose, msgSubCompose.ToString(), msgBodyCompose.ToString(), UserEmail, FromEmailId, FromEmailName, msg_spintaxt, ComposeMessageMinDelay, ComposeMessageMaxDelay, preventMsgSameUser, preventMsgGlobalUser);


                            int counter = ComposeMessage.SlectedContacts1.Count();

                            if (counter > 0)
                            {
                                do
                                {
                                    PostFinalMsg(ref objGlobusHttpHelper, SelectedContacts, msgBodycomposePass, subjectlistCompose, msgSubCompose.ToString(), msgBodyCompose.ToString(), UserEmail, FromEmailId, FromEmailName, msg_spintaxt, ComposeMessageMinDelay, ComposeMessageMaxDelay, preventMsgSameUser, preventMsgGlobalUser);
                                    counter = ComposeMessage.SlectedContacts1.Count();

                                } while (counter > 0);
                            }
                        }

                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Couldn't Login With Email : " + objLinkedinUser.username);
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info("Exeption : " + ex);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Exception : " + ex);
            }
            finally
            {
                //if (chkSelectAll.Checked)
                //{
                //    NoOfAccountsLoggedin--;
                //    if (NoOfAccountsLoggedin == 0)
                //    {
                //        if (btnSendMsg.InvokeRequired)
                //        {
                //            btnSendMsg.Invoke(new MethodInvoker(delegate
                //            {
                //                AddLoggerComposeMessage("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED ]");
                //                AddLoggerComposeMessage("-------------------------------------------------------------------------------------------------------------------------------");
                //                btnSendMsg.Cursor = Cursors.Default;
                //            }));
                //        }
                //    }
                //}
                //else
                //{
                //    if (btnSendMsg.InvokeRequired)
                //    {
                //        btnSendMsg.Invoke(new MethodInvoker(delegate
                //        {
                //            AddLoggerComposeMessage("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED ]");
                //            AddLoggerComposeMessage("-----------------------------------------------------------------------------------------------------------------------------------");
                //            btnSendMsg.Cursor = Cursors.Default;
                //        }));
                //    }
                //}
            }
        }

        public string FromEmailCodeComposeMsg(ref GlobusHttpHelper HttpHelper, string email)
        {
            string FromId = string.Empty;
            //string FromNm = string.Empty;
            string namewithid = string.Empty;
            string pageSource = string.Empty;
            string[] RgxGroupData = new string[] { };

            if (string.IsNullOrEmpty(namewithid))
            {
                try
                {
                    pageSource = HttpHelper.getHtmlfromUrl1(new Uri("http://www.linkedin.com/inbox/compose?trk=inbox_messages-comm-left_nav-compose"));
                    RgxGroupData = Regex.Split(pageSource, "\"name\":\"senderEmail\"");

                    try
                    {
                        if (RgxGroupData[1].Contains("\"value\":"))
                        {
                            try
                            {
                                int StartIndex = RgxGroupData[1].IndexOf("\"value\":");
                                string start = RgxGroupData[1].Substring(StartIndex);
                                int endIndex = start.IndexOf(",");
                                FromId = start.Substring(0, endIndex).Replace("\"", string.Empty).Replace("value:", string.Empty).Replace("}", string.Empty).Trim();
                            }
                            catch
                            {
                            }
                        }
                        //namewithid = FromId + ":" + FromNm;
                        namewithid = FromId;
                    }
                    catch { }
                }
                catch (Exception ex)
                {
                    return namewithid;
                }
            }
            return namewithid;
        }
        public string FromName(ref GlobusHttpHelper HttpHelper)
        {
            string FromNm = string.Empty;
            try
            {
                string pageSource = HttpHelper.getHtmlfromUrl1(new Uri("http://www.linkedin.com/profile/edit?trk=nav_responsive_sub_nav_edit_profile"));

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
                                    FromNm = FromNm.Split(',')[0].Replace(":", string.Empty).Replace("\\u002d", "-");
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
                                        FromNm = FromNm.Split(',')[0].Replace(":", string.Empty);
                                    }
                                }
                                catch
                                { }
                            }
                        }
                        catch { }
                    }
                }
                return FromNm;
            }
            catch (Exception ex)
            {
                //return FromNm;
            }
            return FromNm;
        }

        #region PostFinalMsg
        public void PostFinalMsg(ref GlobusHttpHelper HttpHelper, Dictionary<string, string> SlectedContacts, string userText, List<string> GrpMemSubjectlist, string msg, string body, string UserEmail, string FromemailId, string FromEmailNam, bool msg_spintaxt, int mindelay, int maxdelay, bool preventMsgSameUser, bool preventMsgGlobalUser)
        {
           // ComposeMsgDbManager objComposeMsgDbMgr = new ComposeMsgDbManager();

            try
            {
                string companyName = string.Empty;
                string postdata = string.Empty;
                string postUrl = string.Empty;

                string ResLogin = string.Empty;
                string csrfToken = string.Empty;
                string sourceAlias = string.Empty;

                string ReturnString = string.Empty;
                string PostMsgSubject = string.Empty;
                string PostMsgBody = string.Empty;
                string FString = string.Empty;
                string Nstring = string.Empty;
                string connId = string.Empty;
                string FullName = string.Empty;
                string ToMsg = string.Empty;
                string ToCd = string.Empty;


                try
                {
                    DataSet ds_bList = new DataSet();
                    DataSet ds = new DataSet();
                    string MessageText = string.Empty;
                    string PostedMessage = string.Empty;

                    string pageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com/home?trk=hb_tab_home_top"));
                    if (pageSource.Contains("csrfToken"))
                    {
                        csrfToken = pageSource.Substring(pageSource.IndexOf("csrfToken"), 50);
                        string[] Arr = csrfToken.Split('<');
                        csrfToken = Arr[0];
                        csrfToken = csrfToken.Replace("csrfToken", "").Replace("\"", string.Empty).Replace("value", string.Empty).Replace("cs", string.Empty).Replace("id", string.Empty).Replace("=", string.Empty).Replace("\n", string.Empty).Replace(">", string.Empty).Replace("<script typ", string.Empty);
                        csrfToken = csrfToken.Trim();
                    }

                    if (pageSource.Contains("sourceAlias"))
                    {
                        sourceAlias = pageSource.Substring(pageSource.IndexOf("sourceAlias"), 100);
                        string[] Arr = sourceAlias.Split('"');
                        sourceAlias = Arr[2];
                    }

                    //if (IsAllAccounts)
                    //{
                    //    try
                    //    {
                    //        ClsLinkedinMain obj_ClsLinkedinMain = new ClsLinkedinMain();

                    //        Dictionary<string, string> dTotalFriends = obj_ClsLinkedinMain.PostAddMembers(ref HttpHelper, UserEmail);

                    //        Log("[ " + DateTime.Now + " ] => [ No. Of Friends = " + dTotalFriends.Count + " With Username >>> " + UserEmail + " ]");

                    //        if (dTotalFriends.Count > 0)
                    //        {


                    //            PostMessageToAllAccounts(ref HttpHelper, SlectedContacts, dTotalFriends, msg, body, UserEmail, FromemailId, FromEmailNam, mindelay, mindelay);

                    //            int count = SlectedContacts2.Count();

                    //            if (count > 0)
                    //            {
                    //                do
                    //                {

                    //                    PostMessageToAllAccounts(ref HttpHelper, SlectedContacts2, dTotalFriends, msg, body, UserEmail, FromemailId, FromEmailNam, mindelay, mindelay);
                    //                    count = SlectedContacts2.Count();

                    //                } while (count > 0);
                    //            }
                    //        }
                    //        Log("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED With Username >>> " + UserEmail + " ]");
                    //        Log("-----------------------------------------------------------------------------------------------------------------------------------");

                    //        return;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //    }
                    //}


                    string ContactName = string.Empty;
                    int counter = 1;
                    Dictionary<string, string> SlectedSentContacts = new Dictionary<string, string>();
                    Nstring = string.Empty;
                    ContactName = string.Empty;
                    string ProfileUrl = string.Empty;
                    string ProfileID = string.Empty;
                    foreach (KeyValuePair<string, string> itemChecked in SlectedContacts)
                    {
                        if (Globals.groupStatusString == "API")
                        {
                            ProfileUrl = "https://www.linkedin.com/contacts/view?id=" + itemChecked.Key + "";
                            string profilePageSource = HttpHelper.getHtmlfromUrl(new Uri(ProfileUrl));
                            ProfileID = Utils.getBetween(profilePageSource, "id=", ",").Replace("\"", "");
                        }
                        else
                        {
                            //ProfileUrl = "https://www.linkedin.com/profile/view?id=" + itemChecked.Key + ""; // commented for rodney account
                            //ProfileID = itemChecked.Key;

                            ProfileUrl = "https://www.linkedin.com/contacts/view?id=" + itemChecked.Key + "";
                            string profilePageSource = HttpHelper.getHtmlfromUrl(new Uri(ProfileUrl));
                            ProfileID = Utils.getBetween(profilePageSource, "id=", ",").Replace("\"", "");

                        }

                        if (counter < 50)
                        {
                            SlectedSentContacts.Add(itemChecked.Key, itemChecked.Value);

                            try
                            {
                                string Querystring = "Select ProfileID From tb_BlackListAccount Where ProfileID ='" + ProfileID + "'";
                                //string Querystring = "Select ProfileID From tb_BlackListAccount Where ProfileID ='" + itemChecked.Key +"'";
                                ds_bList = DataBaseHandler.SelectQuery(Querystring, "tb_BlackListAccount");
                            }
                            catch { }

                            if (ds_bList.Tables.Count > 0 && ds_bList.Tables[0].Rows.Count > 0)
                            {
                               // Log("[ " + DateTime.Now + " ] => [ User: " + itemChecked.Value.Replace(":", string.Empty).Trim() + " is Added BlackListed List For Send Messages Pls Check ]");

                            }
                            else
                            {

                                try
                                {

                                    string FName = string.Empty;
                                    string Lname = string.Empty;

                                    try
                                    {
                                        FName = itemChecked.Value.Split(' ')[0];
                                        Lname = itemChecked.Value.Split(' ')[1];
                                    }
                                    catch { }

                                    //FullName = FName + " " + Lname;
                                    FullName = itemChecked.Value.Split(':')[0];
                                    try
                                    {
                                        //ContactName = ContactName + "  :  " + FullName;
                                        ContactName = "  :  " + FullName;
                                    }
                                    catch { }

                                    if (ToMsg == string.Empty)
                                    {
                                        //ToMsg += FullName;
                                        ToMsg = FullName;
                                    }
                                    else
                                    {
                                        //ToMsg += ";" + FullName;
                                        ToMsg = ";" + FullName;
                                    }

                                    //Log("[ " + DateTime.Now + " ] => [ Adding Contact " + FullName + " ]");

                                    //ToCd = itemChecked.Key;  //for client sudi
                                    ToCd = ProfileID;
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
                                        //Nstring += "," + FString;
                                        Nstring = FString;
                                        connId = ToCd;
                                        //connId += " " + ToCd;
                                    }


                                }
                                catch (Exception ex)
                                {
                                    Console.Write("error : " + ex.StackTrace);
                                }
                                //Nstring += "}";
                            }

                        }
                        else
                        {
                            try
                            {
                                SlectedContacts1.Add(itemChecked.Key, itemChecked.Value);
                            }
                            catch (Exception ex)
                            {
                                Console.Write("error : " + ex.StackTrace);
                            }
                        }
                        counter++;


                        //}

                        if (SlectedContacts1.Count != 0)
                        {
                            foreach (KeyValuePair<string, string> itemremove in SlectedSentContacts)
                            {
                                SlectedContacts1.Remove(itemremove.Key);
                            }
                        }

                        if (msg_spintaxt == true)
                        {
                            try
                            {
                               // body = GlobusSpinHelper.spinLargeText(new Random(), userText);
                                msg = GrpMemSubjectlist[RandomNumberGenerator.GenerateRandom(0, GrpMemSubjectlist.Count - 1)];
                            }
                            catch (Exception ex)
                            {
                                Console.Write("error : " + ex.StackTrace);
                            }
                            try
                            {
                                body = body.Replace("<Insert Name>", FullName);
                                body = body.Replace("<Insert From Email>", FromEmailNam);
                                body = body.Replace("<Company Name>", "");
                                PostMsgSubject = PostMsgSubject.Replace("<Company Name>", "");
                            }
                            catch
                            { }
                        }


                        if (preventMsgSameUser)
                        {
                            try
                            {
                                string Querystring = "Select MsgFrom,MsgToId,MsgTo,MsgSubject,MsgBody From tb_ManageComposeMsg Where MsgFrom ='" + UserEmail + "' and MsgBody = '" + body + "' and MsgToId = " + connId + "";
                                ds = DataBaseHandler.SelectQuery(Querystring, "tb_ManageComposeMsg");
                            }
                            catch (Exception ex)
                            {
                                Console.Write("error : " + ex.StackTrace);
                            }
                        }


                        if (preventMsgGlobalUser)
                        {
                            try
                            {
                                string Querystring = "Select MsgFrom,MsgToId,MsgTo,MsgSubject,MsgBody From tb_ManageComposeMsg Where MsgToId = " + connId + "";
                                ds = DataBaseHandler.SelectQuery(Querystring, "tb_ManageComposeMsg");
                            }
                            catch (Exception ex)
                            {
                                Console.Write("error : " + ex.StackTrace);
                            }
                        }

                        try
                        {
                            string PostMessage = string.Empty;
                            string ResponseStatusMsg = string.Empty;

                           // Log("[ " + DateTime.Now + " ] => [ Message Sending Process Running.. ]");

                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {

                                    PostMessage = "";
                                    ResponseStatusMsg = "Already Sent";

                                }
                                else
                                {

                                    PostMessage = "senderEmail=" + FromemailId.Trim() + "&ccInput=&subject=" + Uri.EscapeDataString(msg.ToString()) + "&body=" + Uri.EscapeDataString(body.ToString()) + "&isReply=&isForward=&itemId=&recipients=" + Uri.EscapeUriString(connId) + "&recipientNames=" + Uri.EscapeUriString(Nstring) + "&groupId=&csrfToken=" + csrfToken + "&sourceAlias=" + sourceAlias + "&submit=Send+Message";
                                    ResponseStatusMsg = HttpHelper.postFormDataRef(new Uri("https://www.linkedin.com/inbox/mailbox/message/send"), PostMessage, "https://www.linkedin.com/inbox/", "", "", "XMLHttpRequest", "https://www.linkedin.com", "1");   //ahmed sudi client changes
                                }
                            }
                            else
                            {
                                PostMessage = "senderEmail=" + FromemailId.Trim() + "&ccInput=&subject=" + Uri.EscapeDataString(msg.ToString()) + "&body=" + Uri.EscapeDataString(body.ToString()) + "&isReply=&isForward=&itemId=&recipients=" + Uri.EscapeUriString(connId) + "&recipientNames=" + Uri.EscapeUriString(Nstring) + "&groupId=&csrfToken=" + csrfToken + "&sourceAlias=" + sourceAlias + "&submit=Send+Message";
                                ResponseStatusMsg = HttpHelper.postFormDataRef(new Uri("https://www.linkedin.com/inbox/mailbox/message/send"), PostMessage, "https://www.linkedin.com/inbox/", "", "", "XMLHttpRequest", "https://www.linkedin.com", "1");   //ahmed sudi client changes
                            }

                            if (ResponseStatusMsg.Contains("Your message was successfully sent.") || ResponseStatusMsg.Contains("Tu mensaje ha sido enviado con éxito."))
                            {
                                #region for newConnections only

                                try
                                {
                                    if (Globals.isnew_Connections_only)
                                    {
                                        string query_update = "update tb_New_Connections set status=1 where UserId=" + connId;
                                        DataBaseHandler.UpdateQuery(query_update, "tb_ComposeMessageExcelData");
                                    }
                                }
                                catch (Exception ex)
                                {
                                   // Log("[ " + DateTime.Now + " ] => [ Exception : " + ex + " ]");
                                }




                                #endregion


                                try
                                {
                                    string query_update = "update tb_ComposeMessageExcelData set status=1 where RecipientProfileId=" + connId;
                                    DataBaseHandler.UpdateQuery(query_update, "tb_ComposeMessageExcelData");
                                }
                                catch (Exception ex)
                                {
                                    Console.Write("error : " + ex.StackTrace);
                                }

                                // foreach (var item in SlectedSentContacts)
                                // {
                                try
                                {
                                    string Querystring = "Select ProfileID From tb_BlackListAccount Where ProfileID ='" + itemChecked.Key + "'";
                                    ds_bList = DataBaseHandler.SelectQuery(Querystring, "tb_BlackListAccount");
                                }
                                catch (Exception ex)
                                {
                                    Console.Write("error : " + ex.StackTrace);
                                }

                                if (ds_bList.Tables.Count > 0 && ds_bList.Tables[0].Rows.Count > 0)
                                {
                                   // Log("[ " + DateTime.Now + " ] => [ User: " + itemChecked.Value.Split(':')[0].Replace(":", string.Empty).Trim() + " is Added BlackListed List For Send Messages Pls Check ]");
                                }
                                else
                                {
                                   // Log("[ " + DateTime.Now + " ] => [ Subject Posted : " + msg + " ]");
                                   // Log("[ " + DateTime.Now + " ] => [ Body Text Posted : " + body.ToString() + " ]");
                                   // Log("[ " + DateTime.Now + " ] => [ Message Posted To Account: " + itemChecked.Value.Split(':')[0] + " With Username >>> " + UserEmail + " ]");

                                    ReturnString = "Your message was successfully sent.";
                                    string bdy = string.Empty;

                                    try
                                    {
                                    }
                                    catch { }
                                    if (string.IsNullOrEmpty(bdy))
                                    {
                                        bdy = body.ToString().Replace(",", ":");
                                    }


                                    string CSVHeader = "UserName" + "," + "Subject" + "," + "Body Text" + "," + "ContactName" + "," + "ProfileUrl";
                                    string CSV_Content = UserEmail + "," + msg + "," + bdy + "," + ContactName.Replace(":", string.Empty).Trim() + "," + ProfileUrl;
                                   // CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_ComposeMessageSent);

                                    try
                                    {
                                       // objComposeMsgDbMgr.InsertComposeMsgData(UserEmail, Convert.ToInt32(connId), ContactName, msg, bdy);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.Write("error : " + ex.StackTrace);
                                    }
                                }
                                //}

                            }

                            else if (ResponseStatusMsg.Contains("There was an unexpected problem that prevented us from completing your request."))
                            {
                                //Log("[ " + DateTime.Now + " ] => [ Error In Message Posting ]");
                               // GlobusFileHelper.AppendStringToTextfileNewLine("Error In Message Posting", Globals.path_ComposeMessage);
                            }
                            else if ((ResponseStatusMsg.Contains("Already Sent")) || (ResponseStatusMsg.Contains("Ya ha sido enviada")))
                            {
                                string bdy = string.Empty;
                                try
                                {
                                    bdy = body.ToString().Replace("\r", string.Empty).Replace("\n", " ").Replace(",", " ");
                                }
                                catch (Exception ex)
                                {
                                    Console.Write("error : " + ex.StackTrace);
                                }

                                if (string.IsNullOrEmpty(bdy))
                                {
                                    bdy = bdy.ToString().Replace(",", ":");
                                }
                                string CSVHeader = "UserName" + "," + "Subject" + "," + "Body Text" + "," + "ContactName";
                                string CSV_Content = UserEmail + "," + msg + "," + bdy.ToString() + "," + ContactName;
                               // CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_MessageAlreadySentComposeMgs);

                               // Log("[ " + DateTime.Now + " ] => [ Message Not Posted To Account: " + ContactName.Replace(":", string.Empty) + " because it has sent the same message already]");
                            }
                            else
                            {
                               // Log("[ " + DateTime.Now + " ] => [ Failed In Message Posting ]");
                               // GlobusFileHelper.AppendStringToTextfileNewLine("Failed In Message Posting", Globals.path_ComposeMessage);
                            }

                            {
                                int delay = RandomNumberGenerator.GenerateRandom(mindelay, maxdelay);
                               GlobusLogHelper.log.Info( "Delay for : " + delay + " Seconds ]");
                                Thread.Sleep(delay * 1000);
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
        #endregion

        #region PostFinalMsg_with tag
        public void PostFinalMsg_WithTag(ref GlobusHttpHelper HttpHelper, Dictionary<string, string> SlectedContacts, List<string> messagesubject, string userText, string msg, string body, string UserEmail, string FromemailId, string FromEmailNam, bool msg_spintaxt, int mindelay, int maxdelay, bool preventMsgSameUser, bool preventMsgGlobalUser)
        {
            try
            {
                string postdata = string.Empty;
                string postUrl = string.Empty;

                string ResLogin = string.Empty;
                string csrfToken = string.Empty;
                string sourceAlias = string.Empty;

                string ReturnString = string.Empty;
                string PostMsgSubject = string.Empty;
                string PostMsgBody = string.Empty;
                string FString = string.Empty;
                string FullName = string.Empty;
                string ToMsg = string.Empty;

                try
                {
                    string MessageText = string.Empty;
                    string PostedMessage = string.Empty;

                    string pageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com/home?trk=hb_tab_home_top"));
                    if (pageSource.Contains("csrfToken"))
                    {
                        csrfToken = pageSource.Substring(pageSource.IndexOf("csrfToken"), 50);
                        string[] Arr = csrfToken.Split('>');
                        csrfToken = Arr[0];
                        csrfToken = csrfToken.Replace("csrfToken", "").Replace("\"", string.Empty).Replace("value", string.Empty).Replace("cs", string.Empty).Replace("id", string.Empty).Replace("=", string.Empty);
                        csrfToken = csrfToken.Trim();
                    }

                    if (pageSource.Contains("sourceAlias"))
                    {
                        sourceAlias = pageSource.Substring(pageSource.IndexOf("sourceAlias"), 100);
                        string[] Arr = sourceAlias.Split('"');
                        sourceAlias = Arr[2];
                    }
                    #region For all
                    //if (IsAllAccounts)
                    //{
                    //    try
                    //    {
                    //        ClsLinkedinMain obj_ClsLinkedinMain = new ClsLinkedinMain();

                    //        Dictionary<string, string> dTotalFriends = obj_ClsLinkedinMain.PostAddMembers(ref HttpHelper, UserEmail);

                    //        Log("[ " + DateTime.Now + " ] => [ No. Of Friends = " + dTotalFriends.Count + " With Username >>> " + UserEmail + " ]");

                    //        if (dTotalFriends.Count > 0)
                    //        {
                    //            // PostMessageToAllAccounts_1By1(ref HttpHelper, dTotalFriends, msg, body, UserEmail, FromemailId, FromEmailNam, mindelay, maxdelay);
                    //            PostMessageToAllAccounts_1By1(ref HttpHelper, dTotalFriends, messagesubject, userText, UserEmail, FromemailId, FromEmailNam, mindelay, maxdelay);
                    //        }
                    //        Log("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED With Username >>> " + UserEmail + " ]");
                    //        Log("-----------------------------------------------------------------------------------------------------------------------------------");

                    //        return;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        GlobusFileHelper.AppendStringToTextfileNewLine("DateTime :- " + DateTime.Now + " :: Error -->  PostFinalMsg_1By1() --> 1 --> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace, Globals.Path_LinkedinErrorLogs);
                    //        GlobusFileHelper.AppendStringToTextfileNewLine("DateTime :- " + DateTime.Now + " :: Error --> PostFinalMsg_1By1() --> 1 --> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace, Globals.Path_LinkedinComposeMessageErrorLogs);
                    //    }
                    //}
                    #endregion

                    string ProfileUrl = string.Empty;
                    foreach (KeyValuePair<string, string> itemChecked in SlectedContacts)
                    {
                        //ComposeMsgDbManager objComposeMsgDbMgr = new ComposeMsgDbManager();

                        //ProfileUrl = "https://www.linkedin.com/profile/view?id=" + itemChecked.Key + ""; for client ahmad

                        //ProfileUrl = "https://www.linkedin.com/contacts/view?id=" + itemChecked.Key + "";
                        //string profilePageSource = HttpHelper.getHtmlfromUrl1(new Uri(ProfileUrl));
                        //string  ProfileID = Utils.getBetween(profilePageSource, "id=", ",").Replace("\"", "");

                        string ProfileID = string.Empty;
                        if (Globals.groupStatusString == "API")
                        {
                            ProfileUrl = "https://www.linkedin.com/contacts/view?id=" + itemChecked.Key + "";
                            string profilePageSource = HttpHelper.getHtmlfromUrl(new Uri(ProfileUrl));
                            ProfileID = Utils.getBetween(profilePageSource, "id=", ",").Replace("\"", "");
                        }
                        else
                        {
                            ProfileUrl = "https://www.linkedin.com/profile/view?id=" + itemChecked.Key + "";

                            ProfileID = itemChecked.Key;
                        }
                        try
                        {
                            DataSet ds = new DataSet();
                            DataSet ds_bList = new DataSet();
                            string companyName = string.Empty;
                            string ContactName = string.Empty;
                            string Nstring = string.Empty;
                            string connId = string.Empty;
                            string FName = string.Empty;
                            string Lname = string.Empty;
                            string tempBody = string.Empty;
                            string tempsubject = string.Empty;

                            try
                            {
                                FName = itemChecked.Value.Split(' ')[0];
                                FullName = itemChecked.Value.Split(':')[0];
                                companyName = itemChecked.Value.Split(':')[2];
                                ContactName = ContactName + "  :  " + FullName;
                            }
                            catch
                            {
                            }
                           
                            if (ToMsg == string.Empty)
                            {
                                ToMsg += FullName;
                            }
                            else
                            {
                                ToMsg += ";" + FullName;
                            }

                            GlobusLogHelper.log.Info (" Adding Contact " + FullName );
                            string ToCd = ProfileID;//itemChecked.Key; for client ahmed sudi
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

                            try
                            {
                                string PostMessage = string.Empty;
                                string ResponseStatusMsg = string.Empty;

                                if (msg_spintaxt)
                                {

                                    //if (lstSubjectReuse.Count == messagesubject.Count)
                                    //{
                                    //    lstSubjectReuse.Clear();
                                    //}
                                    //foreach (var itemSubject in messagesubject)
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
                                }
                                else
                                {
                                    tempBody = body;
                                    tempsubject = msg;
                                }


                                GlobusLogHelper.log.Info("Sending Message ");
                                if (!FName.Contains("."))
                                {
                                    //tempBody = tempBody.Replace("<Insert Name>", FullName);
                                    tempBody = tempBody.Replace("Insert Name", FName);
                                    tempBody = tempBody.Replace("<Insert From Email>", FromEmailNam);
                                    tempsubject = tempsubject.Replace("Insert Name", FName);
                                    tempBody = tempBody.Replace("Company Name", companyName);
                                    tempsubject = tempsubject.Replace("Company Name", companyName);
                                }
                                else
                                {
                                    tempBody = tempBody.Replace("Insert Name", FullName);
                                    tempBody = tempBody.Replace("<Insert From Email>", FromEmailNam);
                                    tempBody = tempBody.Replace("<Company Name>", companyName);
                                    tempsubject = tempsubject.Replace("<Company Name>", companyName);
                                }

                                try
                                {
                                    string Querystring = "Select ProfileID From tb_BlackListAccount Where ProfileID ='" + ProfileID + "'";
                                    ds_bList = DataBaseHandler.SelectQuery(Querystring, "tb_BlackListAccount");
                                }
                                catch { }

                                if (preventMsgSameUser)
                                {
                                    try
                                    {
                                        string Querystring = "Select MsgFrom,MsgToId,MsgTo,MsgSubject,MsgBody From tb_ManageComposeMsg Where MsgFrom ='" + UserEmail + "' and MsgBody = '" + tempBody + "' and MsgToId = " + connId + "";
                                        ds = DataBaseHandler.SelectQuery(Querystring, "tb_ManageComposeMsg");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.Write("error : " + ex.StackTrace);
                                    }
                                }


                                if (preventMsgGlobalUser)
                                {
                                    try
                                    {
                                        string Querystring = "Select MsgFrom,MsgToId,MsgTo,MsgSubject,MsgBody From tb_ManageComposeMsg Where MsgToId = " + connId + "";
                                        ds = DataBaseHandler.SelectQuery(Querystring, "tb_ManageComposeMsg");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.Write("error : " + ex.StackTrace);
                                    }
                                }

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

                                            GlobusLogHelper.log.Info(" User: " + ContactName.Replace(":", string.Empty).Trim() + " is Added BlackListed List For Send Messages Pls Check ");
                                            ResponseStatusMsg = "BlackListed";
                                        }
                                        else
                                        {
                                            tempBody = tempBody.Replace("<Insert Name>", string.Empty).Replace("<Insert Group>", string.Empty).Replace("<Insert From Email>", string.Empty);

                                            PostMessage = "senderEmail=" + FromemailId.Trim() + "&ccInput=&subject=" + Uri.EscapeDataString(tempsubject.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&isReply=&isForward=&itemId=&recipients=" + Uri.EscapeUriString(connId) + "&recipientNames=" + Uri.EscapeUriString(Nstring) + "&groupId=&csrfToken=" + csrfToken + "&sourceAlias=" + sourceAlias + "&submit=Send+Message";
                                            ResponseStatusMsg = HttpHelper.postFormDataRef(new Uri("https://www.linkedin.com/inbox/mailbox/message/send"), PostMessage, "https://www.linkedin.com/inbox/", "", "", "XMLHttpRequest", "https://www.linkedin.com", "1");   //ahmed sudi client changes
                                            //PostMessage = "csrfToken=" + csrfToken + "&subject=" + Uri.EscapeDataString(tempsubject.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&submit=Send+Message&fromName=" + FromEmailNam + "&fromEmail=" + FromemailId + "&connectionIds=" + connId + "&connectionNames=" + Uri.EscapeUriString(Nstring) + "&allowEditRcpts=true&addMoreRcpts=false&openSocialAppBodySuffix=&st=&viewerDestinationUrl=&goback=.smg_*1_*1_*1_*1_*1_*1_*1_*1_*1";   //commented for client ahmad sudi
                                            //ResponseStatusMsg = HttpHelper.postFormData(new Uri("http://www.linkedin.com/msgToConns"), PostMessage);
                                        }
                                    }
                                }
                                else
                                {
                                    if (ds_bList.Tables.Count > 0 && ds_bList.Tables[0].Rows.Count > 0)
                                    {

                                        GlobusLogHelper.log.Info(" User: " + ContactName.Replace(":", string.Empty).Trim() + " is Added BlackListed List For Send Messages Pls Check ]");
                                        ResponseStatusMsg = "BlackListed";
                                    }
                                    else
                                    {
                                        tempBody = tempBody.Replace("<Insert Name>", string.Empty).Replace("<Insert Group>", string.Empty).Replace("<Insert From Email>", string.Empty);
                                        PostMessage = "senderEmail=" + FromemailId.Trim() + "&ccInput=&subject=" + Uri.EscapeDataString(tempsubject.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&isReply=&isForward=&itemId=&recipients=" + Uri.EscapeUriString(connId) + "&recipientNames=" + Uri.EscapeUriString(Nstring) + "&groupId=&csrfToken=" + csrfToken + "&sourceAlias=" + sourceAlias + "&submit=Send+Message";
                                        ResponseStatusMsg = HttpHelper.postFormDataRef(new Uri("https://www.linkedin.com/inbox/mailbox/message/send"), PostMessage, "https://www.linkedin.com/inbox/", "", "", "XMLHttpRequest", "https://www.linkedin.com", "1");   //ahmed sudi client changes


                                        //PostMessage = "csrfToken=" + csrfToken + "&subject=" + Uri.EscapeDataString(tempsubject.ToString()) + "&body=" + Uri.EscapeDataString(tempBody.ToString()) + "&submit=Send+Message&fromName=" + FromEmailNam + "&fromEmail=" + FromemailId + "&connectionIds=" + connId + "&connectionNames=" + Uri.EscapeUriString(Nstring) + "&allowEditRcpts=true&addMoreRcpts=false&openSocialAppBodySuffix=&st=&viewerDestinationUrl=&goback=.smg_*1_*1_*1_*1_*1_*1_*1_*1_*1";
                                        //ResponseStatusMsg = HttpHelper.postFormData(new Uri("http://www.linkedin.com/msgToConns"), PostMessage);
                                    }
                                }

                                if ((ResponseStatusMsg.Contains("Your message was successfully sent.")) || (ResponseStatusMsg.Contains("Se ha enviado tu mensaje satisfactoriamente")) || (ResponseStatusMsg.Contains("Sua mensagem foi enviada")))
                                {
                                    GlobusLogHelper.log.Info(" Subject Posted : " + tempsubject.ToString());
                                    GlobusLogHelper.log.Info(" Body Text Posted : " + tempBody.ToString());
                                    GlobusLogHelper.log.Info(" Message posted to " + FullName );

                                    ReturnString = "Your message was successfully sent.";
                                    string bdy = string.Empty;
                                    try
                                    {
                                        bdy = tempBody.ToString().Replace("\r", string.Empty).Replace("\n", " ").Replace(",", " ");
                                    }
                                    catch { }
                                    if (string.IsNullOrEmpty(bdy))
                                    {
                                        bdy = tempBody.ToString().Replace(",", ":");
                                    }

                                    string CSVHeader = "UserName" + "," + "Subject" + "," + "Body Text" + "," + "ContactName" + "," + "ProfileUrl";
                                    string CSV_Content = UserEmail + "," + tempsubject + "," + bdy + "," + ContactName.Replace(":", string.Empty).Trim() + "," + ProfileUrl;
                                   // CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_ComposeMessageSent);

                                    try
                                    {
                                     //   objComposeMsgDbMgr.InsertComposeMsgData(UserEmail, Convert.ToInt32(connId), ContactName, msg, tempBody);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.Write("error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        string query_update = "update tb_ComposeMessageExcelData set status=1 where RecipientProfileId=" + connId;
                                        DataBaseHandler.UpdateQuery(query_update, "tb_ComposeMessageExcelData");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.Write("error : " + ex.StackTrace);
                                    }

                                }

                                else if (ResponseStatusMsg.Contains("There was an unexpected problem that prevented us from completing your request."))
                                {
                                     GlobusLogHelper.log.Info("Error In Message Posting ");
                                   // GlobusFileHelper.AppendStringToTextfileNewLine("Error In Message Posting", Globals.path_ComposeMessage);
                                }
                                else if ((ResponseStatusMsg.Contains("Already Sent")) || (ResponseStatusMsg.Contains("Ya ha sido enviada")))
                                {
                                    string bdy = string.Empty;
                                    try
                                    {
                                        bdy = body.ToString().Replace("\r", string.Empty).Replace("\n", " ").Replace(",", " ");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.Write("error : " + ex.StackTrace);
                                    }
                                    if (string.IsNullOrEmpty(bdy))
                                    {
                                        bdy = tempBody.ToString().Replace(",", ":");
                                    }
                                    string CSVHeader = "UserName" + "," + "Subject" + "," + "Body Text" + "," + "ContactName";
                                    string CSV_Content = UserEmail + "," + tempsubject + "," + bdy.ToString() + "," + ContactName.Replace(":", string.Empty);
                                    //CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_MessageAlreadySentComposeMgs);

                                    GlobusLogHelper.log.Info("Message Not Posted To Account: " + ContactName.Replace(":", string.Empty) + " because it has sent the same message already");
                                }
                                else if (ResponseStatusMsg.Contains("BlackListed"))
                                {

                                }
                                else
                                {
                                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Failed In Message Posting ]");
                                    //GlobusFileHelper.AppendStringToTextfileNewLine("Failed In Message Posting", Globals.path_ComposeMessage);
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Exception : " + ex);
                            }

                            int delay = RandomNumberGenerator.GenerateRandom(mindelay, maxdelay);
                            GlobusLogHelper.log.Info("Delay for : " + delay + " Seconds ");
                            Thread.Sleep(delay * 1000);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Exception : " + ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info("Exception : " + ex);
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Exception : " + ex);
            }

        }
        #endregion

        public void StarGetFirstConnection()
        {


            try
            {

                if (LDGlobals.listAccounts.Count >= 1)
                {
                    countThreadControllerGetConnections = 0;
                    int numberOfAccountPatch = 25;

                    if (NoOfThreadsGetConnections > 0)
                    {
                        numberOfAccountPatch = NoOfThreadsGetConnections;
                    }

                    List<List<string>> list_listAccounts = new List<List<string>>();



                    list_listAccounts = Utils.Split(LDGlobals.listAccounts, numberOfAccountPatch);

                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        //int tempCounterAccounts = 0; 

                        foreach (string account in listAccounts)
                        {
                            string selectedAcc = Globals.selected_account_for_compose_message;
                            string accFromList = account.Split(':')[0];
                            // if (selectedAcc == accFromList)
                            {
                                try
                                {
                                    lock (lockrThreadControllerPostPicOnWall)
                                    {
                                        try
                                        {
                                            if (countThreadControllerGetConnections >= listAccounts.Count)
                                            {
                                                Monitor.Wait(lockrThreadControllerPostPicOnWall);
                                            }

                                            string acc = account.Remove(account.IndexOf(':'));

                                            //Run a separate thread for each account
                                            LinkedinUser item = null;
                                            LDGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                            if (item != null)
                                            {

                                                Thread profilerThread = new Thread(StartMultiThreadGetFirstConnection);
                                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                                profilerThread.IsBackground = true;

                                                profilerThread.Start(new object[] { item });

                                                countThreadControllerPostPicOnWall++;
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
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void StartMultiThreadGetFirstConnection(object parameters)
        {
            try
            {
                if (!isStopPostPicOnWall)
                {
                    try
                    {
                        lstThreadsPostPicOnWall.Add(Thread.CurrentThread);
                        lstThreadsPostPicOnWall.Distinct();
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
                            //Login Process
                            AccountManager objAccountManager = new AccountManager();
                            objAccountManager.LoginHttpHelper(ref objLinkedinUser);
                        }


                        if (objLinkedinUser.isloggedin)
                        {
                            // Call StartActionMessageReply
                            StartActionGetFirstConnection(ref objLinkedinUser);

                        }
                        else
                        {
                            
                            GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();
                            objLinkedinUser.globusHttpHelper = objGlobusHttpHelper;GlobusLogHelper.log.Info("Couldn't Login With Username : " + objLinkedinUser.username);
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
                    // if (!isStopPostPicOnWall)
                    {
                        lock (lockrThreadControllerPostPicOnWall)
                        {
                            countThreadControllerPostPicOnWall--;
                            Monitor.Pulse(lockrThreadControllerPostPicOnWall);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        public void StartActionGetFirstConnection(ref LinkedinUser LdUser)
        {

            if(isExcelInput)
            {

                Dictionary<string, string> Result = PostaddMembersWithExcelInput_WithoutHittingUrl(ref LdUser.globusHttpHelper, LdUser.username);

                if (Result.Count == 0)
                {
                    if (Globals.MessageContacts.Count == 0)
                    {
                        string query = "select * from  tb_ComposeMessageExcelData where UserName='" + LdUser.username + "'";
                        DataSet DS = DataBaseHandler.SelectQuery(query, "tb_ComposeMessageExcelData");
                        foreach (DataRow itemAccountData in DS.Tables[0].Rows)
                        {

                            string Account1 = itemAccountData["UserName"].ToString();
                            string ProfileUrl = itemAccountData["RecipientProfileUrl"].ToString();
                            Result.Add(Account1, ProfileUrl);

                        }
                    }


                }

               Globals.MessageContacts.Add(LdUser.username, Result);
            }
            else
            {

                Dictionary<string, string> Result = PostAddMembers(ref LdUser);
                Globals.MessageContacts.Add(LdUser.username, Result);

            }

            GlobusLogHelper.log.Info("[ Member Added Successfully : " + LdUser.username);
            GroupStatus.ComboBoxDataBind("jhkj");
            //ComboBoxDataBind("aaaa");



        }

        public Dictionary<string, string> PostAddMembers(ref LinkedinUser LDUser)
        {


            try
            {
                // MemberNameAndID.Add(MemId, MemFullName + ":" + Title);
                MemberNameAndID.Clear();
                string MemId = string.Empty;
                string MemFName = string.Empty;
                string MemLName = string.Empty;
                string MemFullName = string.Empty;
                string Memfullname1 = string.Empty;
                string memID1 = string.Empty;
                string MemId1 = string.Empty;
                string user = LDUser.username;
                GlobusHttpHelper HttpHelper = LDUser.globusHttpHelper;
                string pageSource = string.Empty;

                #region without_search
                if (!Globals.is_Searched_Compose_msg)
                {
                    pageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com/contacts/api/contacts/?&sortOrder=recent&source=LinkedIn"));   // if we url is getting redirected then comment this line


                    // if (!string.IsNullOrEmpty(pageSource)&&!pageSource.Contains("success")) // if we url is getting redirected then remove this comment
                    if (!pageSource.Contains("success"))
                    {
                        for (int i = 1; i <= 2; i++)
                        {
                            Thread.Sleep(4000);
                            pageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.linkedin.com/contacts/api/contacts/?&sortOrder=recent&source=LinkedIn"));
                            if (pageSource.Contains("success"))
                            {
                                break;
                            }

                        }
                    }
                    if (!string.IsNullOrEmpty(pageSource))
                    {

                        string[] RgxGroupData = System.Text.RegularExpressions.Regex.Split(pageSource, "{\"name\"");
                        foreach (var Members in RgxGroupData)
                        {
                            string Fname = string.Empty;
                            string Title = string.Empty;
                            string companyName = string.Empty;
                            if (Members.Contains("title"))
                            {
                                try
                                {
                                    try
                                    {
                                        companyName = Utils.getBetween(Members, "company\":", "name\":");
                                        companyName = Utils.getBetween(companyName, "\"li_", "\",");
                                    }
                                    catch
                                    { }
                                    try
                                    {
                                        string[] arr = Regex.Split(Members, "\"li_");
                                        MemId1 = Utils.getBetween("###" + arr[2], "###", "\"}");
                                        MemId = user + ':' + MemId1;
                                    }
                                    catch
                                    {

                                        string[] arr = Regex.Split(Members, "\"li_");
                                        MemId1 = Utils.getBetween("###" + arr[1], "###", "\"}");
                                        MemId = user + ':' + MemId1;
                                    }

                                    Fname = Utils.getBetween(Members, "\"", "\"");
                                    Title = Utils.getBetween(Members, "\"title\": \"", "\",");
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Info("Exception : " + ex);
                                }

                                try
                                {
                                    if (!string.IsNullOrEmpty(MemId1))
                                    {
                                        string MemUrl = "https://www.linkedin.com/contacts/view?id=" + MemId1; //"https://www.linkedin.com/profile/view?id=" + MemId1;

                                        MemberNameAndID.Add(MemId, Fname + ":" + Title + ":" + companyName);

                                        //GlobusFileHelper.AppendStringToTextfileNewLine(user + " : " + MemFullName, Globals.path_ComposeMessage_FriendList);
                                        GlobusLogHelper.log.Info(" Added member : " + Fname + " ");

                                        string tempFinalData = user + "," + Fname + "," + MemId1 + "," + MemUrl + "," + Title;
                                        AddingLinkedInDataToCSVFile(tempFinalData);

                                        string Query = "INSERT INTO tb_endorsement (FriendName, FriendId,Username,Status) VALUES ('" + MemFullName + "', '" + MemId1 + "','" + user + "','0')";
                                        DataBaseHandler.InsertQuery(Query, "tb_endorsement");

                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Info("Exception : " + ex);
                                }
                            }
                        }

                    }
                }
                #endregion without_search


                #region with_search

                //if (Globals.is_Searched_Compose_msg)
                //{
                //    string pageSourceaAdvanceSearch = HttpHelper.getHtmlfromUrl1(new Uri("http://www.linkedin.com/search"));
                //    string rsid = Utils.getBetween(pageSourceaAdvanceSearch, "vsearch/g?rsid=", "&trk");

                //    string url_search = "https://www.linkedin.com/vsearch/p?orig=FCTD&rsid=" + rsid + "&keywords=" + GroupStatus.searchKeyword + "&trk=vsrp_people_sel&trkInfo=VSRPsearchId%3A4198823611434003376235,VSRPcmpt%3Atrans_nav&openFacets=N,G,CC&f_N=F";
                //    pageSource = HttpHelper.getHtmlfromUrl(new Uri(url_search));   // if we url is getting redirected then comment this line

                //    // if (!string.IsNullOrEmpty(pageSource)&&!pageSource.Contains("success")) // if we url is getting redirected then remove this comment

                //    string no_of_profiles = string.Empty;
                //    string pagination_url = string.Empty;
                //    int no_of_pages = 0;
                //    if (!string.IsNullOrEmpty(pageSource))
                //    {
                //        no_of_profiles = Utils.getBetween(pageSource, "i18n_results_count_capped_without_keywords", "/strong");
                //        no_of_profiles = Utils.getBetween(no_of_profiles, "003e", "+");

                //    }
                //    if (pageSource.Contains("isCurrentPage\":true,\"pageURL"))
                //    {
                //        pagination_url = Utils.getBetween(pageSource, "isCurrentPage", "pageNum");
                //        pagination_url = Utils.getBetween(pagination_url, "pageURL\":\"", "\",");
                //        pagination_url = "https://www.linkedin.com" + pagination_url;

                //        string s1 = "";




                //        //isCurrentPage":true,
                //    }
                //    if (string.IsNullOrEmpty(pageSource))
                //    {
                //        for (int i = 1; i <= 2; i++)
                //        {
                //            Thread.Sleep(4000);
                //            pageSource = HttpHelper.getHtmlfromUrl(new Uri(url_search));
                //            if (pageSource.Contains("<!DOCTYPE html>"))
                //            {
                //                break;
                //            }

                //        }
                //    }
                //    if (!string.IsNullOrEmpty(no_of_profiles))
                //    {
                //        no_of_pages = Convert.ToInt32(no_of_profiles);
                //        no_of_pages = (no_of_pages / 10) + 1;
                //    }

                //    for (int i = 1; i <= no_of_pages; i++)
                //    {
                //        if (i == 1)
                //        {

                //        }
                //        else
                //        {
                //            pagination_url = Utils.getBetween(pagination_url, "", "num=");
                //            pagination_url = pagination_url + "num=" + i;
                //            pageSource = HttpHelper.getHtmlfromUrl(new Uri(pagination_url));

                //        }


                //        if (!string.IsNullOrEmpty(pageSource))
                //        {
                //            string[] RgxGroupData = System.Text.RegularExpressions.Regex.Split(pageSource, "fmt_headline");

                //            foreach (var Members in RgxGroupData)
                //            {
                //                string Fname = string.Empty;
                //                string Title = string.Empty;

                //                if (!Members.Contains("<!DOCTYPE html>"))
                //                {
                //                    try
                //                    {
                //                        try
                //                        {

                //                            MemId1 = Utils.getBetween(Members, "&pid=", "&");




                //                            // int startindex = Members.IndexOf(", \"id\": \"li_");
                //                            // string start = Members.Substring(startindex).Replace(", \"id\": \"li_", string.Empty);
                //                            // int endIndex = start.IndexOf("\"}");
                //                            // MemId1 = start.Substring(0, endIndex).Replace("{[", string.Empty).Replace("}]", string.Empty).Trim();
                //                            MemId = user + ':' + MemId1;
                //                            Globals.groupStatusString = "withoutAPI because of li_";
                //                        }
                //                        catch
                //                        {
                //                            int startindex = Members.IndexOf(", \"id\":");
                //                            string start = Members.Substring(startindex).Replace(", \"id\":", string.Empty);
                //                            int endIndex = start.IndexOf("},");
                //                            MemId1 = start.Substring(0, endIndex).Replace("{[", string.Empty).Replace("},", string.Empty).Trim();
                //                            MemId = user + ':' + MemId1;

                //                            Globals.groupStatusString = "API";
                //                        }

                //                        try
                //                        {
                //                            Fname = Utils.getBetween(Members, "fmt_name\":\"", "\",\"");
                //                            //  if(Fname.Contains(""))


                //                            // string Start = Members.Substring(StartIndex1).Replace(": \"", string.Empty);
                //                            // int EndIndex = Start.IndexOf("\",");
                //                            //string End = Start.Substring(0, EndIndex);
                //                            //Fname = End.Replace(",", ";").Replace("&#xe3;", "ã").Replace("&#xe7;", "ç").Replace("&#xf4;", "ô").Replace("&#xe9;", "é").Replace("&#xba;", "º").Replace("&#xc1;", "Á").Replace("&#xb4;", "'").Replace("&#xed;", "í").Replace("&#xf5;", "õ").Replace("&#xf3;", "ó").Replace("&#xe1;", "á").Replace("&#xea;", "ê").Replace("&#xe0;", "à").Replace("&#xfc;", "ü").Replace("&#xe4;", "ä").Replace("&#xf6;", "ö").Replace("&#xfa;", "ú").Replace("&#xf4;", "ô").Replace("&#xc9;", "É").Replace("&#xe2;", "â").Replace("&#x113;", "ē").Replace("&#xd3;", "Ó").Replace("&#xf1;", "ñ").Replace("&#x20ac;", "€").Replace("&#xd1;", "Ñ").Replace("&#xe8;", "è").Replace("&#xd3;", "Ó").Replace("&#xaa;", "ª").Replace("&#x2605;", "★").Replace("&#x2606;", "☆").Replace("&#xf1;", "ñ").Replace("&#xc0;", "À").Replace("&#x263a;", "☺").Replace("&#xbf;", "¿").Replace("\\u00ae", "®").Replace("{[", string.Empty).Replace("}]", string.Empty);
                //                        }
                //                        catch
                //                        { }

                //                        try
                //                        {
                //                            Title = Utils.getBetween(Members, "\":\"", "\",\"");
                //                            // string Start = Members.Substring(StartIndextemp).Replace("title\":", string.Empty);
                //                            //int EndIndex = Start.IndexOf(",");
                //                            // string End = Start.Substring(0, EndIndex).Replace("\"", "").Trim();
                //                            // Title = End.Replace(",", ";").Replace("&#xe3;", "ã").Replace("&#xe7;", "ç").Replace("&#xf4;", "ô").Replace("&#xe9;", "é").Replace("&#xba;", "º").Replace("&#xc1;", "Á").Replace("&#xb4;", "'").Replace("&#xed;", "í").Replace("&#xf5;", "õ").Replace("&#xf3;", "ó").Replace("&#xe1;", "á").Replace("&#xea;", "ê").Replace("&#xe0;", "à").Replace("&#xfc;", "ü").Replace("&#xe4;", "ä").Replace("&#xf6;", "ö").Replace("&#xfa;", "ú").Replace("&#xf4;", "ô").Replace("&#xc9;", "É").Replace("&#xe2;", "â").Replace("&#x113;", "ē").Replace("&#xd3;", "Ó").Replace("&#xf1;", "ñ").Replace("&#x20ac;", "€").Replace("&#xd1;", "Ñ").Replace("&#xe8;", "è").Replace("&#xd3;", "Ó").Replace("&#xaa;", "ª").Replace("&#x2605;", "★").Replace("&#x2606;", "☆").Replace("&#xf1;", "ñ").Replace("&#xc0;", "À").Replace("&#x263a;", "☺").Replace("&#xbf;", "¿").Replace("\\u00ae", "®").Replace("{[", string.Empty).Replace("}]", string.Empty);
                //                            if (Title.Contains("u003e"))
                //                            {
                //                                Title = Utils.getBetween(Title + "##", "u003e", "#");
                //                            }
                //                            if (Title.Contains("\\u003c/strong\\u003e"))
                //                            {
                //                                Title = Title.Replace("\\u003c/strong\\u003e", "");
                //                            }


                //                            if (Title == "null")
                //                            {
                //                                Title = "N/A";
                //                            }
                //                        }
                //                        catch
                //                        { }
                //                    }
                //                    catch (Exception ex)
                //                    {
                //                        GlobusFileHelper.AppendStringToTextfileNewLine("DateTime :- " + DateTime.Now + " :: Error --> Add Friends Groups --> PostAddMembers() >> 1 >>>> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace, Globals.Path_LinkedinErrorLogs);
                //                        GlobusFileHelper.AppendStringToTextfileNewLine("DateTime :- " + DateTime.Now + " :: Error --> Add Friends Groups --> PostAddMembers() >> 1 >>>> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace, Globals.Path_LinkedinAddFriendsGroupErrorLogs);
                //                    }
                //                    //   #endregion

                //                    MemFullName = Fname;

                //                    try
                //                    {

                //                        if (!string.IsNullOrEmpty(MemId1))
                //                        {
                //                            #region WriteenByMe

                //                            // string MemUrl = "https://www.linkedin.com/profile/view?id=" + MemId1;

                //                            #endregion




                //                            string MemUrl = "https://www.linkedin.com/contacts/view?id=" + MemId1; //"https://www.linkedin.com/profile/view?id=" + MemId1;





                //                            //string memResponse = HttpHelper.getHtmlfromUrl(new Uri(MemUrl));
                //                            //if (memResponse.Contains("is your connection"))
                //                            //{

                //                            //if (!string.IsNullOrEmpty(Title))
                //                            {
                //                                MemberNameAndID.Add(MemId, MemFullName + ":" + Title);
                //                            }

                //                            GlobusFileHelper.AppendStringToTextfileNewLine(user + " : " + MemFullName, Globals.path_ComposeMessage_FriendList);
                //                            Logger("[ " + DateTime.Now + " ] => [ Added member : " + MemFullName + " ]");

                //                            string tempFinalData = user + "," + MemFullName + "," + MemId1 + "," + MemUrl + "," + Title;
                //                            AddingLinkedInDataToCSVFile(tempFinalData);

                //                            string Query = "INSERT INTO tb_endorsement (FriendName, FriendId,Username,Status) VALUES ('" + MemFullName + "', '" + MemId1 + "','" + user + "','0')";
                //                            DataBaseHandler.InsertQuery(Query, "tb_endorsement");

                //                            if (moduleLog == "endorsecamp")
                //                            {
                //                                Log_Endorse("[ " + DateTime.Now + " ] => [ Insert Member ID " + MemId1 + " of " + (MemFullName) + " ]");
                //                            }
                //                            // }
                //                        }
                //                    }
                //                    catch (Exception ex)
                //                    {
                //                        GlobusFileHelper.AppendStringToTextfileNewLine("DateTime :- " + DateTime.Now + " :: Error --> Add Friends Groups --> PostAddMembers() >> 2 >>>> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace, Globals.Path_LinkedinErrorLogs);
                //                        GlobusFileHelper.AppendStringToTextfileNewLine("DateTime :- " + DateTime.Now + " :: Error --> Add Friends Groups --> PostAddMembers() >> 2 >>>> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace, Globals.Path_LinkedinAddFriendsGroupErrorLogs);
                //                    }
                //                }
                //            }

                //        }
                //    }
                //}
                #endregion with_search


              

                return MemberNameAndID;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Exception : " + ex);
                return MemberNameAndID;
            }
        }

        public Dictionary<string, string> PostaddMembersWithExcelInput_WithoutHittingUrl(ref GlobusHttpHelper Httphelper, string user)
        {

            try
            {
                if (!IsStop)
                {
                    Globals.lstComposeMessageThread.Add(Thread.CurrentThread);
                    Globals.lstComposeMessageThread.Distinct().ToList();
                    Thread.CurrentThread.IsBackground = true;
                }
            }
            catch
            { }
            try
            {
                MemberNameAndID.Clear();


               // this.HttpHelper = Httphelper;
                int count = 0;

                if (Globals.no_of_accounts_to_be_selected_for_compose_message == 0)
                {
                    Globals.no_of_accounts_to_be_selected_for_compose_message = Cmpmsg_excelData.Count();
                }


                foreach (string[] itemArr in Cmpmsg_excelData)
                {

                    count++;
                    if (count > Globals.no_of_accounts_to_be_selected_for_compose_message)
                    {
                        break;
                    }


                    string MemId = string.Empty;
                    string MemFName = string.Empty;
                    string MemLName = string.Empty;
                    string MemFullName = string.Empty;
                    string Memfullname1 = string.Empty;
                    string memID1 = string.Empty;
                    string Name = string.Empty;

                    if (user == itemArr.GetValue(0).ToString())
                    {
                        string URL = itemArr.GetValue(1).ToString();
                        //if (!URL.Contains("https://www.linkedin.com/profile/") && !URL.Contains("www.linkedin.com"))
                        //if (!URL.Contains("https://www.linkedin.com/profile/"))
                        //{
                        //    string id = getBetween(URL + "#", "id=", "#");
                        //    // URL = "https://www.linkedin.com/profile/view?id=" + URL;
                        //    URL = "https://www.linkedin.com/profile/view?id=" + id;
                        //}
                        //string PagesrcProfileExcel = Httphelper.getHtmlfromUrl(new Uri(URL));

                        try
                        {
                            //int startindex = URL.IndexOf("?id=");
                            //string start = URL.Substring(startindex).Replace("?id=", string.Empty);
                            //int endindex = start.IndexOf("&");
                            //string end = start.Substring(0, endindex).Replace("&", string.Empty);
                            //MemId = end.Trim();

                            MemId =Utils.getBetween(URL + "#", "id=", "#");

                            MemId = user + ":" + MemId;
                        }
                        catch
                        { }

                        if (string.IsNullOrEmpty(MemId))
                        {
                            if (URL.Contains("&id"))
                            {
                                try
                                {
                                    URL = URL + "&&*@";
                                    int startindex1 = URL.IndexOf("&id=");
                                    string start1 = URL.Substring(startindex1).Replace("&id=", string.Empty);
                                    int endindex1 = start1.IndexOf("&&*@");
                                    string end1 = start1.Substring(0, endindex1).Replace("&&*@", string.Empty);
                                    MemId = end1.Trim();
                                    MemId = user + ":" + MemId;
                                }
                                catch
                                { }
                            }
                        }

                        if (string.IsNullOrEmpty(MemId))
                        {
                            if (URL.Contains("?id="))
                            {
                                try
                                {

                                    MemId = URL.Split('=')[1].ToString();
                                    MemId = user + ":" + MemId;
                                }
                                catch
                                { }
                            }
                        }
                        Name = itemArr.GetValue(2).ToString();

                        Memfullname1 = Name;

                        try
                        {
                            if (!string.IsNullOrEmpty(Name))
                            {
                                MemberNameAndID.Add(MemId, Memfullname1);
                            }
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                }
                return MemberNameAndID;

            }
            catch { return MemberNameAndID; }


        }

        #region AddingLinkedInDataToCSVFile
        public static void AddingLinkedInDataToCSVFile(string Data)
        {
            try
            {
                //string LinkedInAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LinkedInScraper\\" + SearchCriteria.FileName + ".csv");
                string LinkedInDeskTop = Globals.DesktopFolder + "\\GroupMemberList.csv";

                #region LinkedIn Writer

                //Checking File Exixtance
                if (!File.Exists(LinkedInDeskTop))
                {
                    string Header = "Account" + "," + "GroupName" + "," + "GroupUrl" + "," + "ContactPerson" + "," + "ProfileUrl" + "," + "Degree of Connection";
                    GlobusFileHelper.AppendStringToTextfileNewLine(Header, LinkedInDeskTop);
                }

                if (!string.IsNullOrEmpty(Data))
                {
                    GlobusFileHelper.AppendStringToTextfileNewLine(Data, LinkedInDeskTop);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion


    }
}
