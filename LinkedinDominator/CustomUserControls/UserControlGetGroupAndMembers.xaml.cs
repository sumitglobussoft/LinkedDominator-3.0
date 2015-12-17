using linkedDominator;
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
using log4net;
using BaseLib;
using System.Threading;
using Messages;

namespace LinkeddinDominator.CustomUserControls
{
    /// <summary>
    /// Interaction logic for UserControlGetGroupAndMembers.xaml
    /// </summary>
    public partial class UserControlGetGroupAndMembers : UserControl
    {
        

        public UserControlGetGroupAndMembers()
        {
            //GroupStatus.CampaignStopLogevents.addToLogger += new EventHandler(CampaignnameLog);
            MessageGroupMember.objEvents.addToLogger_sharan += new EventHandler(CampaignnameLog);
            MessageGroupMember.objEvents.addGroupNamesForStatusUpdate += new EventHandler(bindGroupMembers);
            InitializeComponent();
            bindMethod();
        }
        void CampaignnameLog(object sender, EventArgs e)
        {
            if (e is EventsArgs)
            {
                EventsArgs eArgs = e as EventsArgs;
                BindGroupNamesToComboBox(eArgs.log);
            }
        }

        void bindGroupMembers(object sender,EventArgs e )
        {
            if (e is EventsArgs)
            {
                EventsArgs eArgs = e as EventsArgs;
                BindGroupMembersToCheckedListBox(eArgs.log);
            }

        }


       
        private void BindGroupNamesToComboBox(string p)
        {
           // if (GroupStatus.ManageTabGroupStatus == true)
            {
                cmb_MsgGroupMemer_selectedAcc.Dispatcher.Invoke(new Action(delegate

                                {

                                    new Thread(() =>
                                    {
                                        cmb_MsgGroupMemer_selectedAcc.Dispatcher.Invoke(new Action(delegate
                                        {

                try
                {
                    cmb_members_of_selected_group.Items.Clear();
                    MessageGroupMember.GroupMemUrl.Clear();
                }
                catch(Exception ex)
                {

                    GlobusLogHelper.log.Error("Exception : " + ex);
                }
                try
                {
                    string GetUserID = cmb_MsgGroupMemer_selectedAcc.SelectedItem.ToString();
                    //label47.Text = cmbAllUser.SelectedItem.ToString();

                    foreach (KeyValuePair<string, Dictionary<string, string>> item in MessageGroupMember.AllGroupNames)
                    {
                        if (GetUserID.Contains(item.Key))
                        {
                            List<string> GmUserIDs = new List<string>();
                            foreach (KeyValuePair<string, string> item1 in item.Value)
                            {
                                string group = item1.Key;
                                if (!string.IsNullOrEmpty(group))
                                {
                                    string[] group1 = group.Split('^');

                                    if (GetUserID == group1[1].ToString())
                                    {
                                        //cmbSelectG roup
                                        cmb_members_of_selected_group.Items.Add(group1[1] + ':' + group1[0].ToString()); //Items.Add(group1[1] + ':' + group1[0].ToString());
                                        MessageGroupMember.GroupMemUrl.Add(item1.Key + ":" + item1.Value);
                                    }
                                }

                            }

                            GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Finished Adding Groups of Usernames  Please Select User Groups..]");
                            GlobusLogHelper.log.Info("----------------------------------------------------------------------------------------------------------------------------------------");
                        }
                    }

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Exception : " + ex);
                }
                                        }));
                                          }).Start();
                                }));

               // GroupStatus.ManageTabGroupStatus = true;
            }

        }

        private void BindGroupMembersToCheckedListBox(string p)
        {
            cmb_MsgGroupMemer_selectedAcc.Dispatcher.Invoke(new Action(delegate

                                {

                                    new Thread(() =>
                                    {
                                        cmb_MsgGroupMemer_selectedAcc.Dispatcher.Invoke(new Action(delegate
                                        {

                                            try
                                            {
                                              //  chklstBoxGroupMembers.
                                                foreach (var item in MessageGroupMember.GroupMemData)
                                                {

                                                    chklstBoxGroupMembers.Items.Add(item.Value);

                                                    //if (count < Globals.no_of_accounts_to_be_checked)
                                                    //{

                                                    //    chklstBoxGroupMembers.Items.Add(itemM.Value, true);
                                                    //    count++;
                                                    //}
                                                    //else
                                                    //{
                                                    //    chkListGroupMembers.Items.Add(itemM.Value, false);

                                                    //}
                                                }

                                            }
                                            catch(Exception ex)
                                            { }
                                        }));
                                    }).Start();
                                }));

        }

        public void bindMethod()
        {
            foreach(string item in LDGlobals.listAccounts)
            {
                string userName=item.Split(':')[0];
                cmb_MsgGroupMemer_selectedAcc.Items.Add(userName);
               
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LDGlobals.loadedAccountsDictionary.Count <= 0)
                {
                    GlobusLogHelper.log.Info("Please upload the accounts");
                    return;
                }
                if (string.IsNullOrEmpty(cmb_MsgGroupMemer_selectedAcc.SelectedItem.ToString()))
                {
                    GlobusLogHelper.log.Info("Please select an acount from drop down menu");
                    return;
                }

                try
                {
                   // cmb_MsgGroupMemer_selectedAcc.Items.Remove()
                }
                catch
                { }

                MessageGroupMember objMessageGroupMember = new MessageGroupMember();

                new Thread(() =>
                {
                    objMessageGroupMember.LinkdinGroupMemberUpdate();

                }).Start();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Info("Exception : " + ex);
            }
        }

        private void cmb_MsgGroupMemer_selectedAcc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (MessageGroupMember.ManageTabGroupStatus == true)
            {
                cmb_members_of_selected_group.Items.Clear();
                MessageGroupMember.GroupMemUrl.Clear();

                try
                {
                    string GetUserID = cmb_MsgGroupMemer_selectedAcc.SelectedItem.ToString();
                    //label47.Text = cmbAllUser.SelectedItem.ToString();

                    foreach (KeyValuePair<string, Dictionary<string, string>> item in Globals.AllGroupNames)
                    {
                        if (GetUserID.Contains(item.Key))
                        {
                            List<string> GmUserIDs = new List<string>();
                            foreach (KeyValuePair<string, string> item1 in item.Value)
                            {
                                string group = item1.Key;
                                if (!string.IsNullOrEmpty(group))
                                {
                                    string[] group1 = group.Split('^');

                                    if (GetUserID == group1[1].ToString())
                                    {
                                        cmb_members_of_selected_group.Items.Add(group1[1] + ':' + group1[0].ToString()); //Items.Add(group1[1] + ':' + group1[0].ToString());
                                        MessageGroupMember.GroupMemUrl.Add(item1.Key + ":" + item1.Value);
                                    }
                                }

                            }

                         GlobusLogHelper.log.Info(" Finished Adding Groups of Usernames  Please Select User Groups..]");
                         GlobusLogHelper.log.Info("----------------------------------------------------------------------------------------------------------------------------------------");
                        }
                    }

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info("Exception : " + ex);
                }

               MessageGroupMember.ManageTabGroupStatus = true;
            }
            MessageGroupMember.ManageTabGroupStatus = true;
        }

        private void cmb_members_of_selected_group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void aaa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btn_GetMembers_of_SelectedGroup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(cmb_MsgGroupMemer_selectedAcc.ToString()))
                {
                    MessageGroupMember.SelectedAcc = cmb_MsgGroupMemer_selectedAcc.SelectedItem.ToString();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please select an account from drop down menu");
                    return;

                }
                if (!string.IsNullOrEmpty(cmb_members_of_selected_group.ToString()))
                {
                    MessageGroupMember.selected_group = cmb_members_of_selected_group.SelectedItem.ToString();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please select group from drop down menu");
                    return;
                }
                if(!string.IsNullOrEmpty(txt_MsgGroupMember_SearchKeyword.ToString()))
                {
                    MessageGroupMember.SearchKeyword = txt_MsgGroupMember_SearchKeyword.ToString();
                }
                chklstBoxGroupMembers.Items.Clear();
                MessageGroupMember objMessageGroupMember = new MessageGroupMember();
                Thread th = new Thread(objMessageGroupMember.startGettingMembers);
                th.Start();

            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Info("Exception : " + ex);
            }
            
            
        }

        List<string> selectedMembers = new List<string>();

        private void btn_Messages_MessageGroupMember_SaveSelectedMembers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach(string item in chklstBoxGroupMembers.CheckedItems)
                {
                    selectedMembers.Add(item);

                }
                MessageGroupMember.selectedMembers = selectedMembers;

            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("ExcEption : " + ex);
            }

        }

       
       // private void cmb_MsgGroupMemer_selectedAcc_
        

       
    }
}
