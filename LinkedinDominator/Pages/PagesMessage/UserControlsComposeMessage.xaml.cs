using FirstFloor.ModernUI.Windows.Controls;
using LinkeddinDominator.CustomUserControls;
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
using Messages;
using BaseLib;
using System.Threading;

namespace LinkeddinDominator.Pages.PagesMessage
{
    /// <summary>
    /// Interaction logic for UserControlsComposeMessage.xaml
    /// </summary>
    public partial class UserControlsComposeMessage : UserControl
    {
        public UserControlsComposeMessage()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

         try
            {
                var window = new ModernDialog
                {
                    Content = new UserControlGetFirstConnectionForMessageSending()
                };
                window.MinWidth = 550;
                window.MinHeight = 350;
                window.Title = "Get The First Connection";               
                window.ShowDialog();
            }
            catch (Exception ex)
            {
              //  GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            } 
          //  new  UserControlGetFirstConnectionForMessageSending()
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new ModernDialog
                {
                    Content = new UserControlComposeMessageInput()
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

        private void btn_StartMessageSending_Click(object sender, RoutedEventArgs e)
        {
           try
           {
               if (!string.IsNullOrEmpty(txt_ComposeMessageMinDelay.Text))
               {
                   ComposeMessage.ComposeMessageMinDelay = Convert.ToInt32(txt_ComposeMessageMinDelay.Text);
               }
               if(!string.IsNullOrEmpty(txt_ComposeMesageMaxDelay.Text))
               {
                   ComposeMessage.ComposeMessageMaxDelay = Convert.ToInt32(txt_ComposeMesageMaxDelay.Text);
               }
           }
            catch(Exception ex)
           {
               GlobusLogHelper.log.Info("Exception : " + ex); 

           }
           //btn_StartMessageSending = Cursors.AppStarting;

            ComposeMessage objComposeMessage = new ComposeMessage();
            Thread thrSendingMessage = new Thread(objComposeMessage.startSendingMessage);
            thrSendingMessage.Start();
        }

      
    }
}
