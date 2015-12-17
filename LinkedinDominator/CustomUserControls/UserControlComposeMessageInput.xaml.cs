using BaseLib;
using Messages;
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

namespace LinkeddinDominator.CustomUserControls
{
    /// <summary>
    /// Interaction logic for UserControlComposeMessageInput.xaml
    /// </summary>
    public partial class UserControlComposeMessageInput : UserControl
    {
        public UserControlComposeMessageInput()
        {
            InitializeComponent();
        }

        private void btnSave_ComposeMessageInput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(txt_ComposeMessageBody.Text))
                {
                    GlobusLogHelper.log.Info("Please add message body");
                    txt_ComposeMessageBody.Focus();
                    return;
                }
                if(string.IsNullOrEmpty(txt_ComposeMessageSubject.Text))
                {
                    GlobusLogHelper.log.Info("Please add mesasge subject");
                    txt_ComposeMessageSubject.Focus();
                    return;
                }

                if (!string.IsNullOrEmpty(txt_ComposeMessageBody.Text))
                {
                    ComposeMessage.ComposeMessagebody = txt_ComposeMessageBody.Text;
                }
                if (!string.IsNullOrEmpty(txt_ComposeMessageSubject.Text))
                {
                    ComposeMessage.ComposeMessagesubject = txt_ComposeMessageSubject.Text;
                }
                if(rdbtn_messageWithTag.IsChecked==true)
                {
                    ComposeMessage.MesageWithTag = true;
                }
                else if(rdbtn_messageWithoutTag.IsChecked==true)
                {
                    ComposeMessage.MesageWithTag = false;
                }
                if(chkSpintax.IsChecked==true)
                {
                    ComposeMessage.isSpintax = true;

                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Info("Exception : " + ex);
            }
        }
    }
}
