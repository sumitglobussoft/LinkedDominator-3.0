using Add_Connection;
using BaseLib;
using Globussoft;
using System;
using System.Collections.Generic;
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

namespace LinkeddinDominator.CustomUserControls
{
    /// <summary>
    /// Interaction logic for UserControlAddConnectionByEmail.xaml
    /// </summary>
    public partial class UserControlAddConnectionByEmail : UserControl
    {
        public UserControlAddConnectionByEmail()
        {
            InitializeComponent();
        }
        
        List<string >  lstEmails=new List<string>();
        private void btn_AddConn_BrowseEmails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Thread uploadAccountThread = new Thread(LoadEmails);
                //uploadAccountThread.SetApartmentState(System.Threading.ApartmentState.STA);
                //uploadAccountThread.IsBackground = true;

                //uploadAccountThread.Start();
                lstEmails.Clear();
                LoadEmails();
                GlobalsAddConn.lst_Emails_for_AddConnection = lstEmails;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void LoadEmails()
        {
            try
            {
                try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
               // GlobalsAddConn.lstLoadEmails = GlobusFileHelper.ReadFiletoStringList(dlg.FileName);
                lstEmails = GlobusFileHelper.ReadFiletoStringList(dlg.FileName);
                Application.Current.Dispatcher.Invoke(new Action(() => { txt_AddConn_Email.Text =dlg.FileName; }));
            }
            catch (Exception ex)
            {
            }
            }
            catch (Exception ex)
            {
            }
        }

        private void chkDivideData_Checked(object sender, RoutedEventArgs e)
        {
            GlobalsAddConn.selectedDivideData = true;
            rdb_AddConn_DivideEqually.IsEnabled = true;
            rdb_AddConn_DivideGivenByUsers.IsEnabled = true;
        }

        private void rdb_AddConn_DivideEqually_Checked(object sender, RoutedEventArgs e)
        {
            if(rdb_AddConn_DivideEqually.IsChecked==true)
            {
                GlobalsAddConn.selectedRdbDivideEqually = true;
            }
            else
            {
                GlobalsAddConn.selectedRdbDivideEqually = false;
            }
        }

        private void rdb_AddConn_DivideGivenByUsers_Checked(object sender, RoutedEventArgs e)
        {
            GlobalsAddConn.selectedRdbDivideGivenByUser = true;
            lblNoOfUsers.IsEnabled = true;
            txt_AddConn_NoOfUsers.IsEnabled = true;
        }

        private void btn_AddConnection_Save_Click(object sender, RoutedEventArgs e)
        {
            #region Settings
            try
            {
                if (!string.IsNullOrEmpty(txt_AddConn_NoOfUsers.Text))
                {
                    GlobalsAddConn.noOfUsers = Convert.ToInt32(txt_AddConn_NoOfUsers.Text);
                }
            }
            catch (Exception ex)
            {
            } 
            #endregion
        }

        private void chk_AddConn_DivideData_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalsAddConn.selectedDivideData = false;
            rdb_AddConn_DivideEqually.IsEnabled = false;
            rdb_AddConn_DivideGivenByUsers.IsEnabled = false;
            lblNoOfUsers.IsEnabled = false;
            txt_AddConn_NoOfUsers.IsEnabled = false;
        }

        private void rdb_AddConn_DivideGivenByUsers_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalsAddConn.selectedRdbDivideGivenByUser = false;
            lblNoOfUsers.IsEnabled = false;
            txt_AddConn_NoOfUsers.IsEnabled = false;
        }
    }
}
