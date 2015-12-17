using Add_Connection;
using BaseLib;
using Globussoft;
using Microsoft.Win32;
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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControlAddConnectionByKeyword : UserControl
    {
        public UserControlAddConnectionByKeyword()
        {
            InitializeComponent();
        }

        List<string> lst_Keywods = new List<string>();
        private void btn_AddConn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_AddConn_RequestPerKeyword.Text))
                {
                    GlobalsAddConn.requestPerKeyword = Convert.ToInt32(txt_AddConn_RequestPerKeyword.Text);
                }
                if(!string.IsNullOrEmpty(txt_AddConn_DailyLimit.Text))
                {
                    GlobalsAddConn.dailyLimit = Convert.ToInt32(txt_AddConn_DailyLimit.Text);
                }
                if (chk_AddConn_UniqueConnection.IsChecked == true)
                {
                    GlobalsAddConn.selectedUniqueConnection = true;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void btn_AddConn_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Thread uploadAccountThread = new Thread(LoadKeywords);
                //uploadAccountThread.SetApartmentState(System.Threading.ApartmentState.STA);
                //uploadAccountThread.IsBackground = true;

                //uploadAccountThread.Start();

                lst_Keywods.Clear();
                LoadKeywords();

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
        public void LoadKeywords()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                //txt_AddConn_Keyword.Text = dlg.FileName;
                GlobalsAddConn.lst_keyWords_for_AddConnection.Clear();
                List<string> templist = GlobusFileHelper.ReadFiletoStringList(dlg.FileName);
                foreach (string item in templist)
                {
                    if (!GlobalsAddConn.lst_keyWords_for_AddConnection.Contains(item))
                    {
                        if (!string.IsNullOrEmpty(item.Replace(" ", "").Replace("\t", "")))
                        {
                            GlobalsAddConn.lst_keyWords_for_AddConnection.Add(item);
                        }
                    }
                }
                Application.Current.Dispatcher.Invoke(new Action(() => { txt_AddConn_Keyword.Text = dlg.FileName; }));
            }
            catch (Exception ex)
            {
            }
        }

        private void chk_AddConn_UniqueConnection_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chk_AddConn_UniqueConnection.IsChecked == true)
                {
                   // txt_AddConn_RequestPerKeyword.IsEnabled = false;
                    GlobalsAddConn.selectedUniqueConnection = true;
                    //ConnectUsing_Search.UseuniqueConn = true;
                    if (GlobalsAddConn.lst_keyWords_for_AddConnection.Count>0)
                    {
                        foreach (string itemKeyword in GlobalsAddConn.lst_keyWords_for_AddConnection)
                        {
                            try
                            {
                                //ManageConnections.ConnectUsing_Search.lstQueuKeywords.Enqueue(itemKeyword);
                            }
                            catch { }
                        }
                    }
                }
                else
                {
                    GlobalsAddConn.selectedUniqueConnection = false;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void chk_AddConn_OnlyVisit_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (chk_AddConn_OnlyVisit.IsChecked==true)
                //{
                //    GlobalsAddConn.selectedOnlyVisit = true;
                //}
                //else
                {
                    GlobalsAddConn.selectedOnlyVisit = false;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void chk_AddConn_ClearDB_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chk_AddConn_ClearDB.IsChecked==true)
                {
                    GlobalsAddConn.selectedClearDB = true;
                }
                else
                {
                    GlobalsAddConn.selectedClearDB = false;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void chk_AddConn_CheckDailyLimit_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chk_AddConn_CheckDailyLimit.IsChecked==true)
                {
                    GlobalsAddConn.selectedDailyLimit = true;
                    clsDBQueryManager obj_DBQueryManager = new clsDBQueryManager();
                    if (!string.IsNullOrEmpty(txt_AddConn_DailyLimit.Text))
                    {
                        obj_DBQueryManager.InsertOrUpdateDailyLimitSetting(Convert.ToInt32(txt_AddConn_DailyLimit.Text));
                    }
                }
                else
                {
                    GlobalsAddConn.selectedDailyLimit = false;
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
