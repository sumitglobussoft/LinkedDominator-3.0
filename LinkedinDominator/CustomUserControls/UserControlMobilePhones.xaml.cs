using BaseLib;
using FirstFloor.ModernUI.Windows.Controls;
using LinkedinDominator.Pages.PageAccount;
using System;
using System.Windows;
using System.Windows.Controls;


namespace TwtDominator.CustomUserControls
{
    /// <summary>
    /// Interaction logic for UserControlMobilePhones.xaml
    /// </summary>
    public partial class UserControlMobilePhones : UserControl
    {
        public UserControlMobilePhones()
        {
            InitializeComponent();
        }
    

        public static BaseLib.Events LoadAccounts = new BaseLib.Events();
        QueryManager Qm = new QueryManager();

       // public static BaseLib.Events LoadAccounts = new BaseLib.Events();
        private void btnUserControl_SaveSingleUserDetails_Submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxButton btnC = MessageBoxButton.OK;

                Manage_Accounts obj_Manage_Accounts = new Manage_Accounts();
                string singleUsername = string.Empty;
                string siglePassword = string.Empty;
                string singleproxy = string.Empty;

                string proxyAddress = string.Empty;
                string proxyPort = string.Empty;
                string proxyUserName = string.Empty;
                string proxyPassword = string.Empty;

                singleUsername = txt_AddSingleAccount_Account.Text;
                siglePassword = txt_AddSingleAccount_Password.Password;

                if (string.IsNullOrEmpty(singleUsername))
                {
                    ModernDialog.ShowMessage("Please Enter Account !", "Message Box ", btnC);

                    txt_AddSingleAccount_Account.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(siglePassword))
                {
                    ModernDialog.ShowMessage("Please Enter Password !", "Message Box ", btnC);

                    txt_AddSingleAccount_Password.Focus();
                    return;

                }

                try
                {

                    proxyAddress = txt_AddSingleAccount_ProxyAddress.Text;

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {

                    proxyPort = txt_AddSingleAccount_ProxyPort.Text;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                try
                {

                    proxyUserName = txt_AddSingleAccount_ProxyUsername.Text;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {

                    proxyPassword = txt_AddSingleAccount_ProxyPassword.Password;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                Qm.AddAccountInDataBase(singleUsername, siglePassword, proxyAddress, proxyPort, proxyUserName, proxyPassword);

                obj_Manage_Accounts.LoadAccountsFromDataBase();

                Window parentWindow = (Window)this.Parent;
                parentWindow.Close();

               // Manage_Accounts objManage_Accounts = new Manage_Accounts();
               //objManage_Accounts.LoadAccountsFromDataBase();

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnUserControl_AddSingleAccount_Clear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxButton btnC = MessageBoxButton.YesNoCancel;
                var result = ModernDialog.ShowMessage("Are you want to Clear all text box ?", " Message Box ", btnC);

                if (result == MessageBoxResult.Yes)
                {

                    this.Dispatcher.Invoke(new System.Action(delegate
                    {
                        txt_AddSingleAccount_Account.Clear();
                        txt_AddSingleAccount_Password.Clear();
                        txt_AddSingleAccount_ProxyAddress.Clear();
                        txt_AddSingleAccount_ProxyPort.Clear();
                        txt_AddSingleAccount_ProxyUsername.Clear();
                        txt_AddSingleAccount_ProxyPassword.Clear();

                    }));

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

       
       

        

       
    }
}
