using BaseLib;
using FirstFloor.ModernUI.Windows.Controls;
using Globussoft;
using Groups;
using LinkeddinDominator.CustomUserControls;
using linkedDominator;
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

namespace LinkeddinDominator.Pages.PagesGroup
{
    /// <summary>
    /// Interaction logic for UserControlsGroupStatusUpdate.xaml
    /// </summary>
    public partial class UserControlsGroupStatusUpdate : UserControl
    {
        public UserControlsGroupStatusUpdate()
        {
            InitializeComponent();
        }


        public void closeEvent()
        {


        }

        private void chk_GroupStatusUpdate_GetGroups_Checked(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    if (LDGlobals.listAccounts.Count > 0)
            //    {
            //        var window = new ModernDialog
            //        {
            //            Content = new UserControlGEtGroupsForStatusUpdate()
            //        };
            //        window.MinWidth = 550;
            //        window.MinHeight = 350;
            //        window.Title = "Upload Follow Details";                    
            //        window.ShowDialog();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Please upload accounts.");
            //        return;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //  GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            //}

            try
            {
                UserControlGEtGroupsForStatusUpdate obj = new UserControlGEtGroupsForStatusUpdate();
                
              
                var window = new ModernDialog
                {

                    Content = obj
                };
                window.ShowInTaskbar = true;
                Button customButton = new Button() { Content = "Save" };
                customButton.Click += (ss, ee) => { closeEvent(); window.Close(); };
                window.Buttons = new Button[] { customButton };

                window.ShowDialog();


            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : 55" + ex.Message);
            }



        }

        private void btn_GroupStatusUpdate_BrowseStatusHeader_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread uploadAccountThread = new Thread(LoadStatusHeader);
                uploadAccountThread.SetApartmentState(System.Threading.ApartmentState.STA);
                uploadAccountThread.IsBackground = true;

                uploadAccountThread.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void LoadStatusHeader()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                GlobalsGroups.lstStatusHeader = GlobusFileHelper.ReadFiletoStringList(dlg.FileName);

                Application.Current.Dispatcher.Invoke(new Action(() => { txt_GroupStatusUpdate_StatusHeader.Text = dlg.FileName; }));
                GlobusLogHelper.log.Info("Headers uploaded : " + GlobalsGroups.lstStatusHeader.Count);

                
                foreach (var item in GlobalsGroups.lstStatusHeader)
                {
                    if (item.Length < 200)
                    {
                        GlobalsGroups.ListGrpDiscussion.Add(item);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Item length exceeded 200");
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void btn_GroupStatusUpdate_BrowseMoreDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread uploadAccountThread = new Thread(LoadMoreDetails);
                uploadAccountThread.SetApartmentState(System.Threading.ApartmentState.STA);
                uploadAccountThread.IsBackground = true;

                uploadAccountThread.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void LoadMoreDetails()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                GlobalsGroups.ListGrpMoreDetails = GlobusFileHelper.ReadFiletoStringList(dlg.FileName);
                GlobusLogHelper.log.Info("Details uploaded : " + GlobalsGroups.ListGrpMoreDetails.Count);

                Application.Current.Dispatcher.Invoke(new Action(() => { txt_GroupStatusUpdate_MoreDetails.Text = dlg.FileName; }));

                txt_GroupStatusUpdate_MoreDetails.Text = dlg.FileName;
            }
            catch (Exception ex)
            {
            }
        }

        private void btn_GroupStatusUpdate_SendUpdate_Click(object sender, RoutedEventArgs e)
        {
            #region Settings
            try
            {
                if(!string.IsNullOrEmpty(txt_GroupStatusUpdate_MinDelay.Text)&&!string.IsNullOrEmpty(txt_GroupStatusUpdate_MaxDelay.Text))
                {
                    GlobalsGroups.minDelay = Convert.ToInt32(txt_GroupStatusUpdate_MinDelay.ToString());
                    GlobalsGroups.maxDelay=Convert.ToInt32(txt_GroupStatusUpdate_MaxDelay.ToString());
                }
                else
                {
                    GlobusLogHelper.log.Info("Delay field can't be empty.");
                }

                if(chk_GroupStatusUpdate_GetGroups.IsChecked==true)
                {
                    GlobalsGroups.chkGroup = true;
                }
                else
                {
                    GlobalsGroups.chkGroup = false;
                }
            }
            catch (Exception ex)
            {
            }
            #endregion

            try
            {
                GroupStatus objGroupStatus = new GroupStatus();
                Thread thrThreadStartGroupStatus = new Thread(objGroupStatus.ThreadStartGroupStatus);
                thrThreadStartGroupStatus.Start();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
