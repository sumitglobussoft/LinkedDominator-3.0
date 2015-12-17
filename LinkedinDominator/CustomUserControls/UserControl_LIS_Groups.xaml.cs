using BaseLib;
using linkedDominator;
using Scraper;
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
    /// Interaction logic for UserControl_LIS_Groups.xaml
    /// </summary>
    public partial class UserControl_LIS_Groups : UserControl
    {
        public UserControl_LIS_Groups()
        {
            GlobalsScraper.objEvents.addGroupNamesForScraper += new EventHandler(addGroupNamesForScraper);
            InitializeComponent();
            bindMethod();
        }

        private void addGroupNamesForScraper(object sender, EventArgs e)
        {
            if (e is EventsArgs)
            {
                EventsArgs eArgs = e as EventsArgs;
                BindGroupNamesForScraper(eArgs.log);
            }
        }

        private void BindGroupNamesForScraper(string p)
        {
            try
            {
                chklstBox_LIS_Groups.Dispatcher.Invoke(new Action(delegate
                {
                    new Thread(() =>
                    {
                        chklstBox_LIS_Groups.Dispatcher.Invoke(new Action(delegate
                        {
                            foreach (var item in GlobalsScraper.lstGroups)
                                chklstBox_LIS_Groups.Items.Add(item);
                        }));
                    }).Start();
                }));
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
                cmb_LIS_Groups_SelectEmail.Items.Add(userName);
            }
            chklstBox_LIS_Groups.Items.Add("Hi");
            chklstBox_LIS_Groups.Items.Add("Hello");

        }

        private void btn_ScraperGroup_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach(var items in chklstBox_LIS_Groups.CheckedItems)
                {
                    GlobalsScraper.lstGroups.Add(items.ToString());
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void btn_LIS_Groups_GetGroups_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LinkedInScraper objLinkedInScraper = new LinkedInScraper();
                Thread thrThreadStartGetGroups = new Thread(objLinkedInScraper.ThreadStartGetGroups);
                thrThreadStartGetGroups.Start();
            }
            catch (Exception ex)
            {
            }
        }
    }
}