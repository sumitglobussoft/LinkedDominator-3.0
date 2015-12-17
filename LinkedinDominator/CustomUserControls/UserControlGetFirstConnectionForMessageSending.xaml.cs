using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using BaseLib;
using System.Data;
using System.Threading;
using Globussoft;
using System.Text.RegularExpressions;
using linkedDominator;
using Groups;
using Messages;
using Microsoft.Windows.Controls;
//using Microsoft.Office.Interop.Excel;

using Excel = Microsoft.Office.Interop.Excel;

namespace LinkeddinDominator.CustomUserControls
{
    
    /// <summary>
    /// Interaction logic for UserControlGetFirstConnectionForMessageSending.xaml
    /// </summary>
    public partial class UserControlGetFirstConnectionForMessageSending : UserControl
    {
        
        public string selectedAccount_ForComposeMessage = string.Empty;
        static int counter_compose_msg = 0;
        GlobusHttpHelper HttpHelper = new GlobusHttpHelper();

     

        //Dictionary<string, Dictionary<string, string>> MessageContacts = new Dictionary<string, Dictionary<string, string>>();
        //Dictionary<string, Dictionary<string, string>> GrpMess = new Dictionary<string, Dictionary<string, string>>();
        //Dictionary<string, Dictionary<string, string>> LinkdInContacts = new Dictionary<string, Dictionary<string, string>>();



        public static Events CampaignStopLogevents = new Events();
        public UserControlGetFirstConnectionForMessageSending()
        {
            GroupStatus.CampaignStopLogevents.addToLogger += new EventHandler(CampaignnameLog);
            InitializeComponent();
            bindMethod();
        }
        public void bindMethod()
        {
            foreach(string item in LDGlobals.listAccounts)
            {
                string userName=item.Split(':')[0];
                cmb_Message_ComposeMsg_SelectAcc.Items.Add(userName);

            }

            #region disable buttons

            //chk_ExcelInput_For_FirstConnection.IsEnabled = false;
            txt_ComposeMsg_Excel_FilePath.IsEnabled = false;
            btn_Browse_ExcelInput.IsEnabled = false;
            //chk_search.IsEnabled = false;
            txt_SearchKeyword.IsEnabled = false;

            #endregion

        }

        public void BindConnectionMethod(string name)
        {

        }
        void CampaignnameLog(object sender, EventArgs e)
        {
            if (e is EventsArgs)
            {
                EventsArgs eArgs = e as EventsArgs;
                BindDataInComboBox(eArgs.log);
            }
        }


        public void BindDataInComboBox(string Log)
        {

            cmb_Message_ComposeMsg_SelectAcc.Dispatcher.Invoke(new Action(delegate
                                {
                                    new Thread(() =>
                                    {
                                        cmb_Message_ComposeMsg_SelectAcc.Dispatcher.Invoke(new Action(delegate
                                        {

                                            try
                                            {
                                                selectedAccount_ForComposeMessage = cmb_Message_ComposeMsg_SelectAcc.SelectedItem.ToString();
                                            }
                                            catch(Exception ex )
                                            {
                                                Console.Write(ex);
                                            }
                                            int count = 0;
                                            try
                                            {
                                                chklstBox_Messages_ComposeMessage_FirstConnections.Items.Clear();


                                                string GetUserID = cmb_Message_ComposeMsg_SelectAcc.SelectedItem.ToString();
                                                FromSendMsg = cmb_Message_ComposeMsg_SelectAcc.SelectedItem.ToString();
                                                List<string> GmUserID = new List<string>();
                                                Globals.selected_account_for_compose_message = cmb_Message_ComposeMsg_SelectAcc.SelectedItem.ToString();
                                                
                                                try
                                                {
                                                    foreach (KeyValuePair<string, Dictionary<string, string>> item in Globals.MessageContacts)
                                                    {
                                                        if (GetUserID.Contains(item.Key))
                                                        {
                                                           
                                                            foreach (KeyValuePair<string, string> item1 in item.Value)
                                                            {
                                                                string group = item1.Key;
                                                                string[] group1 = group.Split(':');

                                                                if (group1[0] == GetUserID)
                                                                {
                                                                    if (count < Globals.no_of_accounts_to_be_selected_for_compose_message)
                                                                    {
                                                                        chklstBox_Messages_ComposeMessage_FirstConnections.Items.Add(item1.Value.Replace(",", ""));
                                                                        GmUserID.Add(item1.Value.Replace(",", ""));
                                                                        count++;
                                                                    }
                                                                    else
                                                                    {
                                                                        chklstBox_Messages_ComposeMessage_FirstConnections.Items.Add(item1.Value.Replace(",", ""));
                                                                        GmUserID.Add(item1.Value.Replace(",", ""));
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        GlobusLogHelper.log.Info(" Finished Adding Friend List of :" + item.Key);
                                                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ " + GmUserID.Count() + " Friend List of :" + " " + item.Key + " ]");
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


                                            //  LinkeddinDominator.CustomUserControls.UserControlGetFirstConnectionForMessageSending
                                            //cmb_Message_ComposeMsg_SelectAcc.Items.Add(objLinkedinUser.username);
                                            // cmb_Message_ComposeMsg_SelectAcc.SelectedIndex = 0;
                                        }));
                                    }).Start();
                                }));
            }

      


        private void chk_ExcelInput_For_FirstConnection_Checked(object sender, RoutedEventArgs e)
        {
            if(chk_ExcelInput_For_FirstConnection.IsChecked==true)
            {
                txt_ComposeMsg_Excel_FilePath.IsEnabled = true;
                btn_Browse_ExcelInput.IsEnabled = true;
                ComposeMessage.isExcelInput = true;
            }
            else
            {
                txt_ComposeMsg_Excel_FilePath.IsEnabled = false;
                btn_Browse_ExcelInput.IsEnabled = false;
                ComposeMessage.isExcelInput = false;
            }
        }

        string FromSendMsg = string.Empty;
        private void cmb_Message_ComposeMsg_SelectAcc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedAccount_ForComposeMessage = cmb_Message_ComposeMsg_SelectAcc.SelectedItem.ToString();
            ComposeMessage.SelectedAcc = selectedAccount_ForComposeMessage;

            int count = 0;
            try
            {
                chklstBox_Messages_ComposeMessage_FirstConnections.Items.Clear();
                // GroupStatus.GroupUrl.Clear();

                string GetUserID = cmb_Message_ComposeMsg_SelectAcc.SelectedItem.ToString();
                FromSendMsg = cmb_Message_ComposeMsg_SelectAcc.SelectedItem.ToString();
                //lblTotFriendList.Text = "(" + "0" + ")";
                Globals.selected_account_for_compose_message = cmb_Message_ComposeMsg_SelectAcc.SelectedItem.ToString();
                List<string> GmUserID = new List<string>();
                try
                {
                    foreach (KeyValuePair<string, Dictionary<string, string>> item in Globals.MessageContacts)
                    {
                        if (GetUserID.Contains(item.Key))
                        {
                            
                            foreach (KeyValuePair<string, string> item1 in item.Value)
                            {
                                string group = item1.Key;
                                string[] group1 = group.Split(':');
                                if (group1[0] == GetUserID)
                                {
                                    #region commented
                                    //if (GetUserID == group1[0])
                                    //{
                                    //    string checkTempstr = item1.Value.ToString();

                                    //    if (GroupStatus.checkKeywordSearch)
                                    //    {
                                    //        if (checkTempstr.ToLower().Contains(GroupStatus.searchKeyword.ToLower()))
                                    //        {

                                    //            if (count < Globals.no_of_accounts_to_be_selected_for_compose_message)
                                    //            {
                                    //                chkMessageTo.Items.Add(item1.Value.Replace(",", ""), true);
                                    //                GmUserID.Add(item1.Value.Replace(",", ""));
                                    //                count++;
                                    //            }
                                    //            else
                                    //            {
                                    //                chkMessageTo.Items.Add(item1.Value.Replace(",", ""));
                                    //                GmUserID.Add(item1.Value.Replace(",", ""));
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            if (count < Globals.no_of_accounts_to_be_selected_for_compose_message)
                                    //            {
                                    //                chkMessageTo.Items.Add(item1.Value.Replace(",", ""), true);
                                    //                GmUserID.Add(item1.Value.Replace(",", ""));
                                    //                count++;
                                    //            }
                                    //            else
                                    //            {
                                    //                chkMessageTo.Items.Add(item1.Value.Replace(",", ""));
                                    //                GmUserID.Add(item1.Value.Replace(",", ""));
                                    //            }
                                    //        }

                                    //    }
                                    //else
                                    // {
                                    #endregion
                                    if (count < Globals.no_of_accounts_to_be_selected_for_compose_message)
                                    {
                                        chklstBox_Messages_ComposeMessage_FirstConnections.Items.Add(item1.Value.Replace(",", ""));
                                        GmUserID.Add(item1.Value.Replace(",", ""));
                                        count++;
                                    }
                                    else
                                    {
                                        chklstBox_Messages_ComposeMessage_FirstConnections.Items.Add(item1.Value.Replace(",", ""));
                                        GmUserID.Add(item1.Value.Replace(",", ""));
                                    }
                                }
                                //  }
                            }
                        }
                        GlobusLogHelper.log.Info( GmUserID.Count() + " Friend List of :" + " " + item.Key );
                        GlobusLogHelper.log.Info(" Finished Adding Friend List of :" + item.Key);
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



        
        Utils objUtils = new Utils();

      
        private void btn_ComposeMessage_GetFirstConnection_Click(object sender, RoutedEventArgs e)
        {
            #region MyRegion
            //if(LDUsers.LDGlobals.listAccounts.Count==0)
            //{
            //    GlobusLogHelper.log.Info("Please Upload Accout");
            //    return;
            //}
            //if(string.IsNullOrEmpty(selectedAccount))
            //{
            //    GlobusLogHelper.log.Info("Please select an account from drop down menu");
            //    return;
            //}

            //try
            //{
            //    MessageContacts.Clear();
            //    GrpMess.Clear();
            //    chklstBox_Messages_ComposeMessage_FirstConnections.Items.Clear();
            //    LinkdInContacts.Clear();
            //    //LstComposeMsg.Items.Clear();
            //    chklstBox_Messages_ComposeMessage_FirstConnections.Items.Clear();

            //    new Thread(() =>
            //    {
            //        LinkdinAddFromID();

            //    }).Start();

            //}
            //catch
            //{
            //} 
            #endregion


            try
            {

                if (LDGlobals.listAccounts.Count > 0)
                {
                   

                    Regex checkNo = new Regex("^[0-9]*$");
                    int processorCount = objUtils.GetProcessor();
                    int threads = 25;
                    int maxThread = 25 * processorCount;

                    ComposeMessage objComposeMessage = new ComposeMessage();

                    Thread GroupStatusThread = new Thread(objComposeMessage.StarGetFirstConnection);
                    GroupStatusThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string selectedAcc = cmb_Message_ComposeMsg_SelectAcc.SelectedItem.ToString();
            Dictionary<string, string> SelectedContacts = new Dictionary<string, string>();


            foreach (KeyValuePair<string, Dictionary<string, string>> contacts in Globals.MessageContacts)
            {
               // if (contacts.Key == item.Key)
                if (contacts.Key == selectedAcc)
                {
                    foreach (KeyValuePair<string, string> Details in contacts.Value)
                    {
                        foreach (string itemChecked in chklstBox_Messages_ComposeMessage_FirstConnections.CheckedItems)
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
            Globals.SelectedContacts.Clear();
            Globals.SelectedContacts = SelectedContacts;



        }

        private void btn_saveSelectedConnections_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(LDGlobals.loadedAccountsDictionary.Count==0)
                {
                    GlobusLogHelper.log.Info("Please Load the accounts");
                    return;
                }
                try
                {

                    if (string.IsNullOrEmpty(cmb_Message_ComposeMsg_SelectAcc.SelectedItem.ToString()))//.Items.Count==0)
                    {
                        GlobusLogHelper.log.Info("Plese select an account from drop down menu");
                    }
                }
                catch(Exception ex )
                {
                        GlobusLogHelper.log.Info("Please select an account from drop down menu");
                        return;
                }
                    
                if (chklstBox_Messages_ComposeMessage_FirstConnections.Items.Count == 0)
                {
                    GlobusLogHelper.log.Info("Please Click on Get Members button");
                    return;
                }
                if (chklstBox_Messages_ComposeMessage_FirstConnections.CheckedItems.Count == 0)
                {
                    GlobusLogHelper.log.Info("Please select atleast one member");
                    return;
                }

                List<string> selectedConnections = new List<string>();
                foreach (string item in chklstBox_Messages_ComposeMessage_FirstConnections.CheckedItems)
                {

                    selectedConnections.Add(item);
                }
                ComposeMessage.SelectedMembers = selectedConnections;
                GlobusLogHelper.log.Info("Data saved succesfully !!!");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Exception : " + ex);
            }
                

        }

        private void chk_search_Checked(object sender, RoutedEventArgs e)
        {
            if(chk_search.IsChecked==true)
            {
                txt_SearchKeyword.IsEnabled = true;
            }
            else
            {
                txt_SearchKeyword.IsEnabled = false;
            }
        }

        private void btn_Browse_ExcelInput_Click(object sender, RoutedEventArgs e)
        {
              Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
               // dlg.Filter = "Text documents (.txt)|*.txt";

               dlg.Filter = "Text Files (*.xlsx)|*.xlsx";
            
                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    try
                    {
                        string query = "insert into tb_ExcelFilePathForComposeMessage(FilePath)values('" + dlg.FileName + "')";
                        DataBaseHandler.InsertQuery(query, "tb_ExcelFilePathForComposeMessage");
                    }
                    catch(Exception ex)
                    {
                        GlobusLogHelper.log.Info("Exception : " + ex);
                    }

                    ComposeMessage.Cmpmsg_excelData = parseExcel(dlg.FileName);

                    foreach (string[] item in ComposeMessage.Cmpmsg_excelData)
                    {
                        ThreadPool.SetMaxThreads(100, 100);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(store_data_to_DB_from_Excel), new object[] { item });

                    }
                    GlobusLogHelper.log.Info("Adding Excel file Input completed");
                }
        }

        public void store_data_to_DB_from_Excel(object Cmpmsg_excelData_item1)
        {
            {
                try
                {
                    string Cmpmsg_excelData_item0 = string.Empty;
                    object[] array = Cmpmsg_excelData_item1 as object[];
                    string Cmpmsg_excelData_item11 = string.Empty;
                    string Cmpmsg_excelData_RecipintName = string.Empty;
                    try
                    {
                        string[] Cmpmsg_excelData = (string[])array[0];
                        Cmpmsg_excelData_item0 = Cmpmsg_excelData[0];
                        Cmpmsg_excelData_item11 = Cmpmsg_excelData[1];
                        Cmpmsg_excelData_RecipintName = Cmpmsg_excelData[2];
                    }
                    catch(Exception ex)
                    {
                        GlobusLogHelper.log.Info("Exception : " + ex);
                    };
                    
                    string id = Utils.getBetween(Cmpmsg_excelData_item11 + "###", "id=", "###");
                    string query = "insert into tb_ComposeMessageExcelData(UserName,RecipientProfileUrl,Status,RecipientProfileId,RecipientsName) values('" + Cmpmsg_excelData_item0 + "','" + Cmpmsg_excelData_item11 + "',0,'" + id + "','" + Cmpmsg_excelData_RecipintName + "');";
                    DataBaseHandler.InsertQuery(query, "tb_ComposeMessageExcelData");

                    GlobusLogHelper.log.Info("[ Url Inserted in database " + DateTime.Now + " ] => [" + Cmpmsg_excelData_item11 + "]");

                    //    string query = "Insert into tb_CampaignScraper (CampaignName, Account, FirstName, LastName, Location, Country, AreaWiseLocation, PostalCode, Company, Keyword, Title, Industry, Relationship, Language, Groups, ExportedFileName, TitleValue, CompanyValue, within, YearsOfExperience, Function, SeniorLevel, IntrestedIn, CompanySize, Fortune1000, RecentlyJoined) Values ('" + CampaignName + "','" + Account + ":" + Password + ":" + ProxyAddress + ":" + ProxyPort + ":" + ":" + ProxyUserName + ":" + ProxyPassword + "','" + FirstName + "','" + LastName + "','" + Location + "','" + Country + "','" + LocationArea + "','" + PostalCode + "','" + Company + "','" + Keyword + "','" + Title + "','" + IndustryType + "','" + Relationship + "','" + language + "','" + Group + "','" + FileName + "','" + TitleValue + "','" + CompanyValue + "','" + within + "','" + YearsOfExperience + "','" + Function + "','" + SeniorLevel + "','" + IntrestedIn + "','" + CompanySize + "','" + Fortune1000 + "','" + RecentlyJoined + "');";
                }
                catch { };

            }

            //AddLoggerComposeMessage("[ " + DateTime.Now + " ] => [ Add Excel Input file completed... ]");

        }









        public List<string[]> parseExcel(string path)
        {
            List<string[]> parsedData = new List<string[]>();
            try
            {
                // string[] row =new string[5];
                string singlerow = string.Empty;
                //Excel.FillFormat xlfile = new Excel.FillFormat();
                Microsoft.Office.Interop.Excel.Application xlApp;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                Microsoft.Office.Interop.Excel.Range range;

                string str;
                int rCnt = 0;
                int cCnt = 0;

               // xlApp = new Excel.ApplicationClass();
                 xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(path, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                //xlWorkBook = xlApp.Workbooks.Open(path, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                range = xlWorkSheet.UsedRange;

                for (rCnt = 1; rCnt <= range.Rows.Count; rCnt++)
                {
                    //string[] row = new string[9];
                    string[] row = new string[range.Columns.Count];
                    for (cCnt = 1; cCnt <= range.Columns.Count; cCnt++)
                    {
                        try
                        {
                            str = (string)(range.Cells[rCnt, cCnt] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
                            row[cCnt - 1] = str;
                        }
                        catch { }
                    }
                    parsedData.Add(row);
                }

                xlWorkBook.Close(true, null, null);
                xlApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
                return parsedData;
            }
            catch
            {
                return parsedData;
            }

        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                //MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }





      


       

     

       


      
    }
}
