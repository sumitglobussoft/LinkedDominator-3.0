
using BaseLib;
using FirstFloor.ModernUI.Windows.Controls;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
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
 

namespace LinkeddinDominator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        #region Global var declaration
        public string Language = "English;en,Spanish;es,German;de,French;fr,Italian;it,Portuguese;pt,Dutch;nl,Bahasa Indonesia;in,Malay;ms,Romanian;ro,Russian;ru,Turkish;tr,Swedish;sv,Polish;pl,Others;_o";
       public string RelationshipList = "N:All LinkedIn Members,F:1st Connections,S:2nd Connections,A:Group Members,O:3rd + Everyone Else";
       public string IndustryList = "Accounting;47,Airlines/Aviation;94,Alternative Dispute Resolution;120,Alternative Medicine;125,Animation;127,Apparel & Fashion;19,Architecture & Planning;50,Arts and Crafts;111,Automotive;53,Aviation & Aerospace;52,Banking;41,Biotechnology;12,Broadcast Media;36,Building Materials;49,Business Supplies and Equipment;138,Capital Markets;129,Chemicals;54,Civic & Social Organization;90,Civil Engineering;51,Commercial Real Estate;128,Computer & Network Security;118,Computer Games;109,Computer Hardware;3,Computer Networking;5,Computer Software;4,Construction;48,Consumer Electronics;24,Consumer Goods;25,Consumer Services;91,Cosmetics;18,Dairy;65,Defense & Space;1,Design;99,Education Management;69,E-Learning;132,Electrical/Electronic Manufacturing;112,Entertainment;28,Environmental Services;86,Events Services;110,Executive Office;76,Facilities Services;122,Farming;63,Financial Services;43,Fine Art;38,Fishery;66,Food & Beverages;34,Food Production;23,Fund-Raising;101,Furniture;26,Gambling & casinos;29,Glass, Ceramics & Concrete;145,Government Administration;75,Government Relations;148,Graphic Design;140,Health, Wellness and Fitness;124,Higher Education;68,Hospital & Health Care;14,Hospitality;31,Human Resources;137,Import and Export;134,Individual & Family Services;88,Industrial Automation;147,Information Services;84,Information Technology and Services;96,Insurance;42,International Affairs;74,International Trade and Development;141,Internet;6,Investment Banking;45,Investment Management;46,Judiciary;73,Law Enforcement;77,Law Practice;9,Legal Services;10,Legislative Office;72,Leisure, Travel & Tourism;30,Libraries;85,Logistics and Supply Chain;116,Luxury Goods & Jewelry;143,Machinery;55,Management Consulting;11,Maritime;95,Marketing and Advertising;80,Market Research;97,Mechanical or Industrial Engineering;135,Media Production;126,Medical Devices;17,Medical Practice;13,Mental Health Care;139,Military;71,Mining & Metals;56,Motion Pictures and Film;35,Museums and Institution;37,Music;115,Nanotechnology;114,Newspapers;81,Nonprofit Organization Management;100,Oil & Energy;57,Online Media;113,Outsourcing/Offshoring;123,Package/Freight Delivery;87,Packaging and Containers;146,Paper & Forest Products;61,Performing Arts;39,Pharmaceuticals;15,Philanthropy;131,Photography;136,Plastics;117,Political Organization;107,Primary/Secondary Education;67,Printing;83,Professional Training & Coaching;105,Program Development;102,Public Policy;79,Public Relations and Communications;98,Public Safety;78,Publishing;82,Railroad Manufacture;62,Ranching;64,Real Estate;44,Recreational Facilities and Services;40,Religious Institutions;89,Renewables & Environment;144,Research;70,Restaurants;32,Retail;27,Security and Investigations;121,Semiconductors;7,Shipbuilding;58,Sporting Goods;20,Sports;33,Staffing and Recruiting;104,Supermarkets;22,Telecommunications;8,Textiles;60,Think Tanks;130,Tobacco;21,Translation and Localization;108,Transportation/Trucking/Railroad;92,Utilities;59,Venture Capital & Private Equity;106,Veterinary;16,Warehousing;93,Wholesale;133,Wine and Spirits;142,Wireless;119,Writing and Editing;103";
       public static string Functionlist = "all:All Functions,1:Academics,2:Accounting,3:Administrative,4:Business development,5:Buyer,6:Consultant,7:Creative,8:Engineering,9:Entrepreneur,10:Finance,11:Human resources,12:Information technology,13:Legal,14:Marketing,15:Medical,16:Operations,17:Product,18:Public relations,19:Real estate,20:Sales,21:Support";
       public  string Functionlistrecruiter = "all:All Functions,1:Accounting,2:Administrative,3:Arts & Design,4:Business development,5:Community and Social service,6:Consultant,7:Education,8:Engineering,9:Entrepreneur,10:Finance,11:Healthcare Services,12:Human resources,13:Information technology,14:Legal,15:Marketing,16:Media and communication,17:Military and protective,18:Operation,19:product management ,20:Program and product management,21:Purchasing,22:Quality assurance,23:Real estate,24:Researh,25:Sales,26:Support";

      public string companysizelist = "all:All Company Sizes,1:1-10,2:11-50,3:51-200,4:201-500,5:501-1000,6:1001-5000,7:5001-10000,8:10000+";
       public string senioritylevel = "all:All Seniority Levels,0:Manager,1:Owner,2:Partner,3:CXO,4:VP,5:Director,6:Senior,7:Entry,8:Students & Interns,9:Volunteer";

       public string senioritylevelRecruiterType = "all:All Seniority Levels,10:Owner,9:Partner,8:CXO,7:VP,6:Director,5:Manager,4:Senior,3:Entry,2:Training ,1:Unpaid";


       public string IntrestedinList = "select-all:All LinkedIn Members,1:Potential employees,2:Consultants/contractors,4:Entrepreneurs,8:Hiring managers,16:Industry experts,32:Deal-making contacts,64:Reference check,128:Reconnect";
       public string expList = "1:Less than 1 year,2:1 to 2 years,3:3 to 5 years,4:6 to 10 years,5:More than 10 years";
       public string fortuneList = "select-all:All Companies,1:Fortune 50,2:Fortune 51-100,3:Fortune 101-250,4:Fortune 251-500,5:Fortune 501-1000";
       public string RecentlyjoinedList = "select-all:Any Time,1:1 day ago,2:2-7 days ago,3:8-14 days ago,4:15-30 days ago,5:1-3 months ago";
       public string WithingList = "10:10 mi (15km),25:25 mi (40 km),35:35 mi (55 km),50:50 mi (80 km),75:75 mi (120 km),100:100 mi (160 km)";
       public string TitleValue = "CP:Current or past,C:Current,P:Past,PNC:Past not current";

        #endregion
      
        public static MainWindow mainFormReference = null;
        public MainWindow()
        {
            XmlConfigurator.Configure();
            mainFormReference = this;
            InitializeComponent();
            GlobusLogHelper.log.Info("Welcome");
            CopyDataBase();
           
            // LoadAccount();

        }

        private void CopyDataBase()
        {
            try
            {
                Directory.CreateDirectory(FilePath.LD_PathDBFOlder);
                Directory.CreateDirectory(FilePath.LD_PathDataFolder);
                string baseDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string startUpDB = baseDir + "\\DB_LinkedInDominator.db";
                string localAppDataDB = "C:\\LinkedInDominator\\Data\\DB_LinkedInDominator.db";
                string startUpDB64 = Environment.GetEnvironmentVariable("ProgramFiles(x86)") + "\\DB_LinkedInDominator.db";

                if (!File.Exists(localAppDataDB))
                {
                    if (File.Exists(startUpDB))
                    {
                        try
                        {
                            File.Copy(startUpDB, localAppDataDB);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("Could not find a part of the path"))
                            {
                                Directory.CreateDirectory(FilePath.LD_PathDBFOlder);
                                Directory.CreateDirectory(FilePath.LD_PathDataFolder);
                                File.Copy(startUpDB, localAppDataDB);
                            }
                        }
                    }
                    else if (File.Exists(startUpDB64))   //for 64 Bit
                    {
                        try
                        {
                            File.Copy(startUpDB64, localAppDataDB);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("Could not find a part of the path"))
                            {
                                Directory.CreateDirectory(FilePath.LD_PathDBFOlder);
                                Directory.CreateDirectory(FilePath.LD_PathDataFolder);
                                File.Copy(startUpDB64, localAppDataDB);
                            }
                        }
                    }
                }
                // txtDBpath.Text = localAppDataDB;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
       

        private void ModernWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void ModernWindow_Closed(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

       
    }
    #region LogFornetclass
    public class GlobusLogAppender : log4net.Appender.AppenderSkeleton
    {

        private static readonly object lockerLog4Append = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            try
            {
                string loggerName = loggingEvent.Level.Name;

                MainWindow frmLinkedDominator = MainWindow.mainFormReference ;
              

                lock (lockerLog4Append)
                {
                    switch (loggingEvent.Level.Name)
                    {
                        case "DEBUG":
                            try
                            {

                                {
                                    if (!frmLinkedDominator.lstLogger.Dispatcher.CheckAccess())
                                    {
                                        frmLinkedDominator.lstLogger.Dispatcher.Invoke(new Action(delegate
                                        {
                                            try
                                            {
                                                if (frmLinkedDominator.lstLogger.Items.Count > 1000)
                                                {
                                                    frmLinkedDominator.lstLogger.Items.RemoveAt(frmLinkedDominator.lstLogger.Items.Count - 1);//.Add(frmDominator.listBoxLogs.Items.Add(loggingEvent.TimeStamp + "\t" + loggingEvent.LoggerName + "\r\t\t" + loggingEvent.RenderedMessage);
                                                }

                                                frmLinkedDominator.lstLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + "LinkedDominator 3.0 " + "\r\t" + loggingEvent.RenderedMessage);
                                            }
                                            catch (Exception ex)
                                            {
                                              GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                                            }

                                        }));

                                    }
                                    else
                                    {
                                        try
                                        {
                                            if (frmLinkedDominator.lstLogger.Items.Count > 1000)
                                            {
                                                frmLinkedDominator.lstLogger.Items.RemoveAt(frmLinkedDominator.lstLogger.Items.Count - 1);
                                            }

                                            frmLinkedDominator.lstLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + "LinkedDominator 3.0 " + "\r\t" + loggingEvent.RenderedMessage);
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : 74" + ex.Message);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error Case Debug : " + ex.StackTrace);
                                Console.WriteLine("Error Case Debug : " + ex.Message);
                                GlobusLogHelper.log.Error(" Error : " + ex.Message);
                            }
                            break;
                        case "INFO":
                            try
                            {


                                if (!frmLinkedDominator.lstLogger.Dispatcher.CheckAccess())
                                {
                                    frmLinkedDominator.lstLogger.Dispatcher.Invoke(new Action(delegate
                                    {
                                        try
                                        {
                                            if (frmLinkedDominator.lstLogger.Items.Count > 1000)
                                            {
                                                frmLinkedDominator.lstLogger.Items.RemoveAt(frmLinkedDominator.lstLogger.Items.Count - 1);
                                            }

                                            frmLinkedDominator.lstLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + "LinkedDominator 3.0 " + "\t\t" + loggingEvent.RenderedMessage);
                                        }
                                        catch (Exception ex)
                                        {
                                           GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                                        }

                                    }));

                                }
                                else
                                {
                                    try
                                    {
                                        if (frmLinkedDominator.lstLogger.Items.Count > 1000)
                                        {
                                            frmLinkedDominator.lstLogger.Items.RemoveAt(frmLinkedDominator.lstLogger.Items.Count - 1);
                                        }

                                        frmLinkedDominator.lstLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + "LinkedDominator 3.0 " + "\t\t" + loggingEvent.RenderedMessage);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : 75" + ex.Message);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error Case INFO : " + ex.StackTrace);
                                Console.WriteLine("Error Case INFO : " + ex.Message);
                                GlobusLogHelper.log.Error(" Error : " + ex.Message);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
               // GlobusLogHelper.log.Error("Error : 76" + ex.Message);
            }

        }


    }
    #endregion
}
