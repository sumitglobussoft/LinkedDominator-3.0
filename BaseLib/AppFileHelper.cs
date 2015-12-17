using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Globussoft;


namespace BaseLib
{
    public class AppFileHelper
    {
        #region Global declaration
        static string LoggerFileAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LinkedInDominator\\Logger.txt");
        static string LoggerFileDesktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "LinkedInDominator\\Logger.txt");
        static string EmailFileDeskTopDone = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "LinkedInDominator\\ListOfEmails.txt");
        static string EmailFileDeskTopFailed = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "LinkedInDominator\\LoginFailed.txt");
        #endregion

        #region AddLoggerFile
        public static void AddLoggerFile(string Item)
        {
            try
            {
                GlobusFileHelper.AppendStringToTextfileNewLine(Item, FilePath.Path_LinkedinErrorLogs);
                GlobusFileHelper.AppendStringToTextfileNewLine(Item, Globals.DesktopFolder);
            }
            catch (Exception ex)
            {

            }
        } 
        #endregion

        #region AddingLinkedInDataToCSVFile
        public static void AddingLinkedInDataToCSVFile(string Data, string FileName)
        {
            try
            {
                string LinkedInAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LinkedInScraper\\" + FileName + ".csv");
                string LinkedInDeskTop = Globals.DesktopFolder + "\\LinkedInScraper" + FileName + ".csv";

                #region LinkedIn Writer
                if (!File.Exists(LinkedInAppData))
                {
                    string Header = "ProfileType" + "," + "UserProfileLink" + "," + "ProfileID" + "," + "FirstName" + "," + "LastName" + "," + "HeadLineTitle" + "," + "CurrentTitle" + "," + "Current Company" + "," + "Current Company Url" + "," + "Description of all Company" + "," + "Background - Summary" + "," + "Connection" + "," + "Recommendations " + "," + "SkillAndExpertise " + "," + "Experience " + "," + " Education" + "," + "Groups" + "," + "UserEmail" + "," + "UserContactNumber" + "," + "PastTitles" + "," + "PastCompany" + "," + "Location" + "," + "Country" + "," + "Industry" + "," + "WebSites" + "," + "LinkedInLoginID" + "," + "AccountType" + "," + "Email" + "," + "ProfilePhoneNumber" + "," + "CurrentComapnyName" + "," + "CompanyPhoneNumber" + ",";
                    GlobusFileHelper.AppendStringToTextfileNewLine(Header, LinkedInAppData);
                }

                //Checking File Exixtance
                if (!File.Exists(LinkedInDeskTop))
                {
                    string Header = "ProfileType" + "," + "UserProfileLink" + "," + "ProfileID" + "," + "FirstName" + "," + "LastName" + "," + "HeadLineTitle" + "," + "CurrentTitle" + "," + "Company" + "," + "Current Company Url" + "," + "Background - Summary" + "," + "Connection" + "," + "Recommendations " + "," + "SkillAndExpertise " + "," + "Experience " + "," + " Education" + "," + "Groups" + "," + "UserEmail" + "," + "UserContactNumber" + "," + "PastTitles" + "," + "PastCompany" + "," + "Location" + "," + "Country" + "," + "Industry" + "," + "WebSites" + "," + "LinkedInLoginID" + "," + "AccountType" + "," + "Email" + "," + "ProfilePhoneNumber" + "," + "CurrentComapnyName" + "," + "CompanyPhoneNumber" + ",";
                    GlobusFileHelper.AppendStringToTextfileNewLine(Header, LinkedInDeskTop);
                }

                if (!string.IsNullOrEmpty(Data))
                {
                    GlobusFileHelper.AppendStringToTextfileNewLine(Data, LinkedInDeskTop);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        } 
        #endregion

        #region 
        public static void AddingLinkedInDataToCSVFileWithoutGoingToMainProfile(string Data, string FileName)
        {
            try
            {
                string LinkedInAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LinkedInScraper\\" + FileName + ".csv");
                string LinkedInDeskTop = Globals.DesktopFolder + "\\LinkedInScraper" + FileName + ".csv";

                #region LinkedIn Writer
                if (!File.Exists(LinkedInAppData))
                {
                    string Header = "ProfileUrl" + "," + "FirstName" + "," + "LastName" + "," + "HeadlineTitle" + "," + "Location" + "," + "Industry" + "," + "CurrentTitle" + "," + "PastTitle" + "," + "degreeOfConnection"+",";
                    GlobusFileHelper.AppendStringToTextfileNewLine(Header, LinkedInAppData);
                }

                //Checking File Exixtance
                if (!File.Exists(LinkedInDeskTop))
                {
                    string Header = "ProfileUrl" + "," + "FirstName" + "," + "LastName" + "," + "HeadlineTitle" + "," + "Location" + "," + "Industry" + "," + "CurrentTitle" + "," + "PastTitle" + "," + "degreeOfConnection"+",";
                    GlobusFileHelper.AppendStringToTextfileNewLine(Header, LinkedInDeskTop);
                }

                if (!string.IsNullOrEmpty(Data))
                {
                    GlobusFileHelper.AppendStringToTextfileNewLine(Data, LinkedInDeskTop);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        } 
        #endregion

        #region AddingLinkedInDataToCSVFile-CompanyEmployeeScraper
        public static void AddingLinkedInDataToCSVFileCompanyEmployeeScraper(string Data, string FileName)
        {
            try
            {
                string LinkedInAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LinkedInScraper\\" + FileName + ".csv");
                string LinkedInDeskTop = Globals.DesktopFolder + "\\" + FileName + ".csv";

                #region LinkedIn Writer
                if (!File.Exists(LinkedInAppData))
                {
                    string Header = "ProfileType" + "," + "Id" + "," + "UserProfileLink" + "," + "FirstName" + "," + "LastName" + "," + "HeadLineTitle" + "," + "CurrentTitle" + "," + "Company" + "," + "Current Company Url" + "," + "Description of all Company" + "," + "Background - Summary" + "," + "Connection" + "," + "Recommendations " + "," + "SkillAndExpertise " + "," + "Experience " + "," + " Education" + "," + "Groups" + "," + "UserEmail" + "," + "UserContactNumber" + "," + "PastTitles" + "," + "PastCompany" + "," + "Location" + "," + "Country" + "," + "Industry" + "," + "WebSites" + "," + "LinkedInLoginID" + "," + "AccountType" + ",";
                    GlobusFileHelper.AppendStringToTextfileNewLine(Header, LinkedInAppData);
                }

                //Checking File Exixtance
                if (!File.Exists(LinkedInDeskTop))
                {
                    string Header = "ProfileType" + "," + "UserProfileLink" + "," + "Id" + "," + "FirstName" + "," + "LastName" + "," + "HeadLineTitle" + "," + "CurrentTitle" + "," + "Company" + "," + "Current Company Url" + "," + "Background - Summary" + "," + "Connection" + "," + "Recommendations " + "," + "SkillAndExpertise " + "," + "Experience " + "," + " Education" + "," + "Groups" + "," + "UserEmail" + "," + "UserContactNumber" + "," + "PastTitles" + "," + "PastCompany" + "," + "Location" + "," + "Country" + "," + "Industry" + "," + "WebSites" + "," + "LinkedInLoginID" + "," + "AccountType" + "," + "EmailId" + ",";
                    GlobusFileHelper.AppendStringToTextfileNewLine(Header, LinkedInDeskTop);
                }

                if (!string.IsNullOrEmpty(Data))
                {
                    GlobusFileHelper.AppendStringToTextfileNewLine(Data, LinkedInDeskTop);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        

        #region SalesNavigatorScraperWriteToCSV
        public static void SalesNavigatorScraperWriteToCSV(string Data, string Header, string FileName)
        {
            try
            {
                string LinkedInAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LinkedInScraper\\" + FileName + ".csv");
                string LinkedInDeskTop = Globals.DesktopFolder + "\\" + FileName + ".csv";

                #region LinkedIn Writer
                if (!File.Exists(LinkedInAppData))
                {
                    
                    GlobusFileHelper.AppendStringToTextfileNewLine(Header, LinkedInAppData);
                }

                //Checking File Exixtance
                if (!File.Exists(LinkedInDeskTop))
                {
                    
                    GlobusFileHelper.AppendStringToTextfileNewLine(Header, LinkedInDeskTop);
                }

                if (!string.IsNullOrEmpty(Data))
                {
                    GlobusFileHelper.AppendStringToTextfileNewLine(Data, LinkedInDeskTop);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region AddingLinkedInGroupMemberDataToCSVFile
        public static void AddingLinkedInGroupMemberDataToCSVFile(string Data, string FileName)
        {
            try
            {
                string LinkedInAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LinkedInScraperGroupMember.csv");
                string LinkedInDeskTop = Globals.DesktopFolder + "\\LinkedInScraperGroupMember.csv";

                #region LinkedIn Writer
                if (!File.Exists(LinkedInAppData))
                {
                    string Header = "ProfileType" + "," + "UserProfileLink" + "," + "FirstName" + "," + "LastName" + "," + "HeadLineTitle" + "," + "CurrentTitle " + "," + "Company" + "," + "Description of all Company" + "," + "Background - Summary" + "," + "Connection" + "," + "Recommendations " + "," + "SkillAndExpertise " + "," + "Experience " + "," + " Education" + "," + "Groups" + "," + "UserEmail" + "," + "UserContactNumber" + "," + "PastTitles" + "," + "PastCompany" + "," + "Location" + "," + "Country" + "," + "Industry" + "," + "WebSites" + "," + "LinkedInLoginID" + ",";
                    GlobusFileHelper.AppendStringToTextfileNewLine(Header, LinkedInAppData);
                }

                //Checking File Exixtance
                if (!File.Exists(LinkedInDeskTop))
                {
                    string Header = "ProfileType" + "," + "UserProfileLink" + "," + "FirstName" + "," + "LastName" + "," + "HeadLineTitle" + "," + "CurrentTitle " + "," + "Company" + "," + "Description of all Company" + "," + "Background - Summary" + "," + "Connection" + "," + "Recommendations " + "," + "SkillAndExpertise " + "," + "Experience " + "," + " Education" + "," + "Groups" + "," + "UserEmail" + "," + "UserContactNumber" + "," + "PastTitles" + "," + "PastCompany" + "," + "Location" + "," + "Country" + "," + "Industry" + "," + "WebSites" + "," + "LinkedInLoginID" + ",";
                    GlobusFileHelper.AppendStringToTextfileNewLine(Header, LinkedInDeskTop);
                }

                if (!string.IsNullOrEmpty(Data))
                {
                    GlobusFileHelper.AppendStringToTextfileNewLine(Data, LinkedInDeskTop);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        } 
        #endregion

        #region AppendStringToTextFile
        public static void AppendStringToTextFile(string FilePath, string content)
        {
            using (StreamWriter sw = new StreamWriter(EmailFileDeskTopDone, true))
            {
                sw.WriteLine(content);
            }
        } 
        #endregion

        public static void WriteInTextFile(string Username, string Password, bool Done)
        {
            try
            {
                if (Done)
                {
                    if (File.Exists(EmailFileDeskTopDone))
                    {
                        TextWriter tw = new StreamWriter(EmailFileDeskTopDone, true);
                        tw.WriteLine("Login Done with : " + Username + ":" + Password);
                        tw.Close();
                    }
                }
                else
                {
                    if (File.Exists(EmailFileDeskTopFailed))
                    {
                        TextWriter tw = new StreamWriter(EmailFileDeskTopFailed, true);
                        tw.WriteLine("Login Failed with : " + Username + ":" + Password);
                        tw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
