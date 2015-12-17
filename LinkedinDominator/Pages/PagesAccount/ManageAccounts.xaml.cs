using BaseLib;
using FirstFloor.ModernUI.Windows.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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
using TwtDominator.CustomUserControls;
using System.Threading;
using Globussoft;
using LinkDominator;
using linkedDominator;


//using BaseLibLD;

namespace LinkedinDominator.Pages.PageAccount
{
    /// <summary>
    /// Interaction logic for Manage_Accounts.xaml
    /// </summary>
    public partial class Manage_Accounts : UserControl
    {
      
        BaseLib.Events AddSingleAccountInDataGrid = new BaseLib.Events();
        public Manage_Accounts()
        {
            InitializeComponent();
            LoadAccountsFromDataBase();
        }

        public static string singleUsername = string.Empty;
        public static string siglePassword = string.Empty;
        public static string singleproxy = string.Empty;



        void CampaignnameLog(object sender, EventArgs e)
        {
            if (e is EventsArgs)
            {
                EventsArgs eArgs = e as EventsArgs;
                Addtologger();
            }
        }
        public void Addtologger()
        {
           
        }
        private void DeleteSingleAccount_Click(object sender, RoutedEventArgs e)
        {
           // Thread th = new Thread(DeleteSingleAccount);
            //th.Start();
            DeleteSingleAccount();
            //LoadAccountsFromDataBase();

        }

        private void DeleteSingleAccount()
        {
            try
            {
                int i = grvAccounts_AccountCreator_AccountDetails.SelectedIndex;

                if (i < 0)
                {
                    GlobusLogHelper.log.Info("Please select account for deletion");
                    return;
                }
                QueryManager qm = new QueryManager();
                MessageBoxButton btn = MessageBoxButton.OK;
                MessageBoxButton btnC = MessageBoxButton.YesNoCancel;

                var result = ModernDialog.ShowMessage("Are you want to delete this Accounts permanently?", " Delete Account ", btnC);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (var selection in grvAccounts_AccountCreator_AccountDetails.SelectedItems)
                    {
                        try
                        {
                            DataRowView row = (DataRowView)selection;

                            string Username = row[0].ToString();
                            string Password = row[1].ToString();
                            qm.DeleteAccounts(Username);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : 55" + ex.Message);
                        }
                    }
                    LoadAccountsFromDataBase();
                }
                try
                {
                    DataSet ds = new DataSet();
                    ds = Qm.SelectAccounts();
                    DataTable dt = new DataTable();
                    dt = ds.Tables["tb_LinkedInAccount"];
                    Application.Current.Dispatcher.Invoke(new Action(() => { lblaccounts_ManageAccounts_LoadsAccountsCount.Content = dt.Rows.Count.ToString(); }));
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : 55" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : 55" + ex.Message);
            }
        }
        private void addSingleAccount_Click(object sender, RoutedEventArgs e)
        {
            
            var window = new ModernDialog
            {
                Content = new UserControlMobilePhones()
            };
            window.MinHeight = 250;
            window.MinWidth = 450;
            window.Title = "Add Single Account";
            window.ShowDialog();
            LoadAccountsFromDataBase();
           
        }




        private void btnAccounts_ManageAccounts_LoadAccounts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadAccountProgressBar.IsIndeterminate = true;
                Thread uploadAccountThread = new Thread(LoadAccounts);
                uploadAccountThread.SetApartmentState(System.Threading.ApartmentState.STA);
                uploadAccountThread.IsBackground = true;

                uploadAccountThread.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnAccounts_ManageAccounts_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int i = grvAccounts_AccountCreator_AccountDetails.SelectedIndex;

                if (i < 0)
                {
                    GlobusLogHelper.log.Info("Please select account for deletion");
                    return;
                }
                QueryManager qm = new QueryManager();
                MessageBoxButton btn = MessageBoxButton.OK;
                MessageBoxButton btnC = MessageBoxButton.YesNoCancel;

                var result = ModernDialog.ShowMessage("Are you want to delete this Accounts permanently?", " Delete Account ", btnC);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (var selection in grvAccounts_AccountCreator_AccountDetails.SelectedItems)
                    {
                        try
                        {
                            DataRowView row = (DataRowView)selection;

                            string Username = row[0].ToString();
                            string Password = row[1].ToString();
                            qm.DeleteAccounts(Username);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : 55" + ex.Message);
                        }
                    }
                    LoadAccountsFromDataBase();
                }

                try
                {
                    DataSet ds = new DataSet();
                    ds = Qm.SelectAccounts();
                    DataTable dt = new DataTable();
                    dt = ds.Tables["tb_LinkedInAccount"];
                    Application.Current.Dispatcher.Invoke(new Action(() => { lblaccounts_ManageAccounts_LoadsAccountsCount.Content = dt.Rows.Count.ToString(); }));
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : 55" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : 55" + ex.Message);
            }
        }

        //private void DeleteSingleAccount()
        //{
         
        //    MessageBoxButton btn = MessageBoxButton.OK;
        //    MessageBoxButton btnC = MessageBoxButton.YesNoCancel;

        //    try
        //    {
        //        int i = grvAccounts_AccountCreator_AccountDetails.SelectedIndex;

        //        if (i < 0)
        //        {
        //            GlobusLogHelper.log.Info("Please Select Account For Deletion !");

        //            var ResultMessageBox = ModernDialog.ShowMessage("Please Select Account For Deletion !", " Delete Account ", btnC);

        //            return;
        //        }


        //        var result = ModernDialog.ShowMessage("Are You Want To Delete This Accounts Permanently?", " Delete Account ", btnC);

        //        if (result == MessageBoxResult.Yes)
        //        {
        //            foreach (var selection in grvAccounts_AccountCreator_AccountDetails.SelectedItems)
        //            {
        //                try
        //                {
        //                    DataRowView row = (DataRowView)selection;

        //                    string Username = row["UserName"].ToString();
        //                    string Password = row["Password"].ToString();
        //                    qm.DeleteAccounts(Username);
        //                }
        //                catch (Exception ex)
        //                {
        //                    GlobusLogHelper.log.Error("Error : 55" + ex.Message);
        //                }
        //            }
                    
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : 55" + ex.Message);
        //    }
        //}

        private void btnAccounts_ManageAccounts_AddSingleAccounts_Click(object sender, RoutedEventArgs e)
        {
            LoadAccountProgressBar.IsIndeterminate = true;
            try
            {
                var window = new ModernDialog
                  {
                      Title = " Add Single Account ",
                      Content = new UserControlMobilePhones()
                  };
                window.ShowInTaskbar = true;
              
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : 55" + ex.Message);
            }
         
            LoadAccountProgressBar.IsIndeterminate = false;
            LoadAccountsFromDataBase();
        }

        QueryManager Qm = new QueryManager();    
        private void LoadAccounts()
        {
            //Globals.IsFreeVersion = true;

          

            try
            {
                DataSet ds;

                DataTable dt = new DataTable();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {


                    DateTime sTime = DateTime.Now;

                    #region MyRegion
                    //if (lblAccounts_ManageAccounts_LoadsAccountsPath.InvokeRequired)
                    //{
                    //    lblAccounts_ManageAccounts_LoadsAccountsPath.Invoke(new MethodInvoker(delegate
                    //    {
                    //        lblAccounts_ManageAccounts_LoadsAccountsPath.Text = ofd.FileName;
                    //    }));
                    //}
                    //else
                    //{
                    //    lblAccounts_ManageAccounts_LoadsAccountsPath.Text = ofd.FileName;
                    //} 
                    #endregion

                    dt.Columns.Add("UserName");
                    dt.Columns.Add("Password");
                    
                    dt.Columns.Add("proxyAddress");
                    dt.Columns.Add("proxyPort");
                    dt.Columns.Add("ProxyUserName");
                    dt.Columns.Add("ProxyPassword");
                

                    ds = new DataSet();
                    ds.Tables.Add(dt);

                    #region MyRegion
                    //if (grvAccounts_ManageAccounts_ManageAccountsDetails.InvokeRequired)
                    //{
                    //    grvAccounts_ManageAccounts_ManageAccountsDetails.Invoke(new MethodInvoker(delegate
                    //    {
                    //        grvAccounts_ManageAccounts_ManageAccountsDetails.DataSource = null;
                    //    }));
                    //}
                    //else
                    //{
                    //    grvAccounts_ManageAccounts_ManageAccountsDetails.DataSource = null;
                    //} 
                    #endregion

                    List<string> templist = GlobusFileHelper.ReadFile(dlg.FileName);

                    //if (templist.Count > 0)
                    //{
                    //    LDGlobals.loadedAccountsDictionary.Clear();
                    //    LDGlobals.listAccounts.Clear();
                    //}
                    int counter = 0;
                    foreach (string item in templist)
                    {
                        if (Globals.CheckLicenseManager == "fdfreetrial" && counter == 5)
                        {
                            break;
                        }
                        counter = counter + 1;
                        try
                        {
                            string account = item;
                            string[] AccArr = account.Split(':');
                            if (AccArr.Count() > 1)
                            {
                                string accountUser = account.Split(':')[0];
                                string accountPass = account.Split(':')[1];
                               
                                string proxyAddress = string.Empty;
                                string proxyPort = string.Empty;
                                string proxyUserName = string.Empty;
                                string proxyPassword = string.Empty;
                               

                                int DataCount = account.Split(':').Length;
                                if (DataCount == 2)
                                {
                                    //Globals.accountMode = AccountMode.NoProxy;

                                }
                                else if (DataCount == 4)
                                {

                                    proxyAddress = account.Split(':')[2];
                                    proxyPort = account.Split(':')[3];
                                }
                                else if (DataCount > 5 && DataCount < 7)
                                {

                                    proxyAddress = account.Split(':')[2];
                                    proxyPort = account.Split(':')[3];
                                    proxyUserName = account.Split(':')[4];
                                    proxyPassword = account.Split(':')[5];

                                }
                                else if (DataCount == 7)
                                {

                                    
                                    proxyAddress = account.Split(':')[5];
                                    proxyPort = account.Split(':')[6];
                                    proxyUserName = account.Split(':')[7];
                                    proxyPassword = account.Split(':')[8];
                                  


                                }

                                //dt.Rows.Add(accountUser, accountPass, ScreenName, FollowerCount, FollwingCount, proxyAddress, proxyPort, proxyUserName, proxyPassword, GroupName, AccountStatus);
                                dt.Rows.Add(accountUser, accountPass, proxyAddress, proxyPort, proxyUserName, proxyPassword);


                                // dt.Rows.Add(accountUser, accountPass, proxyAddress, proxyPort, proxyUserName, proxyPassword);
                                Qm.AddAccountInDataBase(accountUser, accountPass, proxyAddress, proxyPort, proxyUserName, proxyPassword);



                                try
                                {
                                    // loadedAccountsDictionary.Clear();
                                    LinkedinUser objLinkedinUser = new LinkedinUser();
                                    objLinkedinUser.username = accountUser;
                                    objLinkedinUser.password = accountPass;
                                    objLinkedinUser.proxyip = proxyAddress;
                                    objLinkedinUser.proxyport = proxyPort;
                                    objLinkedinUser.proxyusername = proxyUserName;
                                    objLinkedinUser.proxypassword = proxyPassword;

                                    LDGlobals.loadedAccountsDictionary.Add(objLinkedinUser.username, objLinkedinUser);

                             

                                    LDGlobals.listAccounts.Add(objLinkedinUser.username + ":" + objLinkedinUser.password + ":" + objLinkedinUser.proxyip + ":" + objLinkedinUser.proxyport + ":" + objLinkedinUser.proxyusername + ":" + objLinkedinUser.proxypassword);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                ///Set this to "0" if loading unprofiled accounts
                                ///
                                string profileStatus = "0";


                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Account has some problem : " + item);
                                GlobusLogHelper.log.Debug("Account has some problem : " + item);
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                    }
                    DataSet objDataSet= Qm.SelectAccounts();
                    dt = objDataSet.Tables["tb_LinkedInAccount"];

                    DataView dv = dt.DefaultView;
                    dv.AllowNew = false;

                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        grvAccounts_AccountCreator_AccountDetails.ItemsSource = dv;

                    }));
                    try
                    {

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        Application.Current.Dispatcher.Invoke(new Action(() => { lblaccounts_ManageAccounts_LoadsAccountsCount.Content = dt.Rows.Count.ToString(); }));

                        GlobusLogHelper.log.Debug("Accounts Loaded : " + dt.Rows.Count.ToString() + " In " + timeSpan + " Seconds");

                        GlobusLogHelper.log.Info("Accounts Loaded : " + dt.Rows.Count.ToString() + " In " + timeSpan + " Seconds");
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
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                LoadAccountProgressBar.IsIndeterminate = false;
            }));

        }

        public void LoadAccountsFromDataBase()
        {
            try
            {
                LDGlobals.loadedAccountsDictionary.Clear();
                LDGlobals.listAccounts.Clear();

                DataTable dt = new DataTable();

                dt.Columns.Add("UserName");
                dt.Columns.Add("Password");
               // dt.Columns.Add("ScreenName");
               //// dt.Columns.Add("FollowerCount");
               // dt.Columns.Add("FollwingCount");
                dt.Columns.Add("proxyAddress");
                dt.Columns.Add("proxyPort");
                dt.Columns.Add("ProxyUserName");
                dt.Columns.Add("ProxyPassword");
             //   dt.Columns.Add("GroupName");
              //  dt.Columns.Add("AccountStatus");



                int counter = 0;
                DataSet ds = null;
                try
                {
                    ds = Qm.SelectAccounts();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }


                foreach (DataRow ds_item in ds.Tables[0].Rows)
                {
                    if (Globals.CheckLicenseManager == "fdfreetrial" && counter == 5)
                    {
                        break;
                    }
                    counter = counter + 1;
                    try
                    {
                        string item = ds_item[0].ToString() + ":" + ds_item[1].ToString() + ":" + ds_item[2].ToString() + ":" + ds_item[3].ToString() + ":" + ds_item[4].ToString() + ":" + ds_item[5].ToString();
                        string account = item;
                        string[] AccArr = account.Split(':');
                        if (AccArr.Count() > 1)
                        {
                            string accountUser = account.Split(':')[0];
                            string accountPass = account.Split(':')[1];

                            string ScreenName = string.Empty;
                            string FollowerCount = string.Empty;
                            string FollwingCount = string.Empty;
                            string proxyAddress = string.Empty;
                            string proxyPort = string.Empty;
                            string proxyUserName = string.Empty;
                            string proxyPassword = string.Empty;
                            string GroupName = string.Empty;
                            string AccountStatus = string.Empty;
                            string status = string.Empty;

                            DataGridColumn newcolumn = new DataGridHyperlinkColumn();


                            int DataCount = account.Split(':').Length;
                            if (DataCount == 2)
                            {
                                //Globals.accountMode = AccountMode.NoProxy;

                            }
                            else if (DataCount == 4)
                            {

                                proxyAddress = account.Split(':')[2];
                                proxyPort = account.Split(':')[3];
                            }
                            else if (DataCount ==6)
                            {

                                proxyAddress = account.Split(':')[2];
                                proxyPort = account.Split(':')[3];
                                proxyUserName = account.Split(':')[4];
                                proxyPassword = account.Split(':')[5];

                            }
                          

                            dt.Rows.Add(accountUser, accountPass,proxyAddress, proxyPort, proxyUserName, proxyPassword);


                            try
                            {
                                LinkedinUser objLD_Users = new LinkedinUser();
                               // TWTUsers objTwtUser = new TWTUsers();
                                objLD_Users.username = accountUser;
                                objLD_Users.password = accountPass;
                                objLD_Users.proxyip = proxyAddress;
                                objLD_Users.proxyport = proxyPort;
                                objLD_Users.proxyusername = proxyUserName;
                                objLD_Users.proxypassword = proxyPassword;

                                LDGlobals.loadedAccountsDictionary.Add(objLD_Users.username, objLD_Users);
//                                TWTGlobals.loadedAccountsDictionary.Add(objTwtUser.username, objTwtUser);

                                #region MyRegion
                                //try
                                //{
                                //    if (cmbGroups_GroupCampaignManager_Accounts.InvokeRequired)
                                //    {
                                //        cmbScraper__fanscraper_Accounts.Invoke(new MethodInvoker(delegate
                                //        {
                                //            cmbScraper__fanscraper_Accounts.Items.Add(accountUser);
                                //        }));
                                //    }
                                //}
                                //catch (Exception ex)
                                //{
                                //    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                //}

                                //try
                                //{
                                //    if (cmbScraper__CustomAudiencesScraper_Accounts.InvokeRequired)
                                //    {
                                //        cmbScraper__CustomAudiencesScraper_Accounts.Invoke(new MethodInvoker(delegate
                                //        {
                                //            cmbScraper__CustomAudiencesScraper_Accounts.Items.Add(accountUser);
                                //        }));
                                //    }
                                //}
                                //catch (Exception ex)
                                //{
                                //    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                //}
                                //try
                                //{
                                //    //cmbCommentsOnPostSelectAccount
                                //    if (cmbCommentsOnPostSelectAccount.InvokeRequired)
                                //    {
                                //        cmbCommentsOnPostSelectAccount.Invoke(new MethodInvoker(delegate
                                //        {
                                //            cmbCommentsOnPostSelectAccount.Items.Add(accountUser + ":" + accountPass);
                                //        }));
                                //    }
                                //}
                                //catch (Exception ex)
                                //{
                                //    GlobusLogHelper.log.Error(ex.Message);
                                //}
                                //try
                                //{
                                //    if (cmbGroups_GroupCampaignManager_Accounts.InvokeRequired)
                                //    {
                                //        cmbGroups_GroupCampaignManager_Accounts.Invoke(new MethodInvoker(delegate
                                //        {
                                //            cmbGroups_GroupCampaignManager_Accounts.Items.Add(accountUser);
                                //        }));
                                //    }
                                //}
                                //catch (Exception ex)
                                //{
                                //    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                //}

                                //try
                                //{
                                //    if (cmbScraper__GroupMemberScraper_Accounts.InvokeRequired)
                                //    {
                                //        cmbScraper__GroupMemberScraper_Accounts.Invoke(new MethodInvoker(delegate
                                //        {
                                //            cmbScraper__GroupMemberScraper_Accounts.Items.Add(accountUser);
                                //        }));
                                //    }
                                //}
                                //catch (Exception ex)
                                //{
                                //    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                //} 
                                #endregion

                                LDGlobals.listAccounts.Add(objLD_Users.username + ":" + objLD_Users.password + ":" + objLD_Users.proxyip + ":" + objLD_Users.proxyport + ":" + objLD_Users.proxyusername + ":" + objLD_Users.proxypassword);
                              //  TWTGlobals.listAccounts.Add(objTwtUser.username + ":" + objTwtUser.password + ":" + objTwtUser.proxyip + ":" + objTwtUser.proxyport + ":" + objTwtUser.proxyusername + ":" + objTwtUser.proxypassword);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                            ///Set this to "0" if loading unprofiled accounts
                            ///
                            string profileStatus = "0";


                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Account has some problem : " + item);
                            GlobusLogHelper.log.Debug("Account has some problem : " + item);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                }
                DataView dv = dt.DefaultView;
                dv.AllowNew = false;

           
                
                this.Dispatcher.Invoke(new Action(delegate
                {
                   // grvAccounts_AccountCreator_AccountDetails.ItemsSource = dv;

                }));

                try
                {
                    grvAccounts_AccountCreator_AccountDetails.ItemsSource = dt.DefaultView;
                }
                catch { }


                GlobusLogHelper.log.Debug("Accounts Loaded : " + dt.Rows.Count);
                GlobusLogHelper.log.Info("Accounts Loaded : " + dt.Rows.Count);

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
     
    }
}
