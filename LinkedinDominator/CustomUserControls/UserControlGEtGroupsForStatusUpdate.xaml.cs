using BaseLib;
using Groups;

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

namespace LinkeddinDominator.CustomUserControls
{
    /// <summary>
    /// Interaction logic for UserControlGEtGroupsForStatusUpdate.xaml
    /// </summary>
    public partial class UserControlGEtGroupsForStatusUpdate : UserControl
    {
        public UserControlGEtGroupsForStatusUpdate()
        {

            GroupStatus.objEvents.addGroupNamesForStatusUpdate += new EventHandler(addGroupNamesForStatusUpdate);

            InitializeComponent();
            //Thread thrBindMethod = new Thread(bindMethod);
            //thrBindMethod.Start();
            bindMethod();
        }

        private void addGroupNamesForStatusUpdate(object sender, EventArgs e)
        {
            if (e is EventsArgs)
            {
                EventsArgs eArgs = e as EventsArgs;
                BindGroupNamesForStatusUpdate(eArgs.log);
            }
        }

        private void BindGroupNamesForStatusUpdate(string p)
        {
            try
            {
                chklstBox_Groups_for_status_Upadte.Dispatcher.Invoke(new Action(delegate
                                {
                                    new Thread(() =>
                                                    {
                                                        chklstBox_Groups_for_status_Upadte.Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            foreach (var item in GlobalsGroups.lstGroups)
                                                                chklstBox_Groups_for_status_Upadte.Items.Add(item);
                                                        }));
                                                    }).Start();
                                }));
                    //chklstBox_Groups_for_status_Upadte.Items.Add(item);
            }
            catch (Exception ex)
            {
            }
        }
        public void bindMethod()
        {
            foreach (string item in LDGlobals.listAccounts)
            {
                string userName = item.Split(':')[0];
                cmb_group_status_selecedAcc.Items.Add(userName);
            }

          //  cmb_group_status_selecedAcc.Items.Add("jfd");
        }

        private void cmb_group_status_selecedAcc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                GlobalsGroups.selectedAccount = cmb_group_status_selecedAcc.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
            }
        }

        private void btn_GroupStatusUpdate_GetGroups_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(!string.IsNullOrEmpty(GlobalsGroups.selectedAccount))
                {
                    GroupStatus objGroupStatus=new GroupStatus();
                    Thread thrThreadStartGroupStatus = new Thread(objGroupStatus.ThreadStartGroupStatus);
                    thrThreadStartGroupStatus.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please select Account first.");
                    MessageBox.Show("Please select Account first.");
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void btn_GroupStatusUpdate_GetGroups_Click_1(object sender, RoutedEventArgs e)
        {
            GroupStatus objGroupStatus = new GroupStatus();
            Thread objThread = new Thread(objGroupStatus.ThreadStartGroupStatus);
            objThread.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GlobalsGroups.SelectedGroups.Clear();
            foreach (var item in chklstBox_Groups_for_status_Upadte.CheckedItems)
            {
                GlobalsGroups.SelectedGroups.Add(item.ToString());
            }
        }
    }
}