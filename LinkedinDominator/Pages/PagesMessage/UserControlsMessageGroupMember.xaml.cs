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

namespace LinkeddinDominator.Pages.PagesMessage
{
    /// <summary>
    /// Interaction logic for UserControlsMessageGroupMember.xaml
    /// </summary>
    public partial class UserControlsMessageGroupMember : UserControl
    {
        public UserControlsMessageGroupMember()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new ModernDialog
                {
                    Content = new UserControlGetGroupAndMembers()
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
                var window = new ModernDialog
                {
                    Content = new UserControlGetGroupAndMembers()
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

        private void CheckBox_Checked_2(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new ModernDialog
                {
                    Content = new UserControlMessageInputForGroups()
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
    }
}
