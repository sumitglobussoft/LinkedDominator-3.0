using Add_Connection;
using BaseLib;
using FirstFloor.ModernUI.Windows.Controls;
using LinkeddinDominator.CustomUserControls;
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

namespace LinkeddinDominator.Pages.PagesAddConnection
{
    /// <summary>
    /// Interaction logic for UserControlsAddConnection.xaml
    /// </summary>
    public partial class UserControlsAddConnection : UserControl
    {
        public UserControlsAddConnection()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalsAddConn.selectedManageConnEmail = true;
                var window = new ModernDialog
                {
                    Content = new UserControlAddConnectionByEmail()
                };
                window.MinWidth = 550;
                window.MinHeight = 350;
                // window.Title = "Upload Follow Details";               
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                //  GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            } 
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalsAddConn.selectedManageConnKeyword = true;
                var window = new ModernDialog
                {
                    Content = new UserControlAddConnectionByKeyword()
                };
                window.MinWidth = 550;
                window.MinHeight = 350;
                // window.Title = "Upload Follow Details";               
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                //  GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            } 
        }

        private void btn_AddConn_Start_Click(object sender, RoutedEventArgs e)
        {
            #region Settings
            try
            {
                if(!string.IsNullOrEmpty(txt_AddConn_MinDelay.Text)&&!string.IsNullOrEmpty(txt_AddConn_MaxDelay.Text))
                {
                    GlobalsAddConn.minDelay = Convert.ToInt32(txt_AddConn_MinDelay.Text);
                    GlobalsAddConn.maxDelay = Convert.ToInt32(txt_AddConn_MaxDelay.Text);
                }
                else
                {
                    MessageBox.Show("Delay field can't be empty.");
                    GlobusLogHelper.log.Info("Delay field can't be empty.");
                    return;
                }
            }
            catch (Exception ex)
            {
            }
            #endregion

            try
            {
                AddConnection objAddConnection=new AddConnection();
                Thread thrThreadStartAddConn = new Thread(objAddConnection.ThreadStartAddConn);
                thrThreadStartAddConn.Start();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
