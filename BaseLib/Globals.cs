﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace BaseLib
{
    public class Globals
    {
        #region Message Group Member

        public static Dictionary<string, Dictionary<string, string>> GrpMemMess = new Dictionary<string, Dictionary<string, string>>();
        public static Dictionary<string, Dictionary<string, string>> AllGroupNames = new Dictionary<string, Dictionary<string, string>>();

        #endregion



        public static List<Thread> lstComposeMessageThread = new List<Thread>();
        public static string groupStatusString = string.Empty;
        public static bool isnew_Connections_only = false;
        public static bool is_Searched_Compose_msg = false;
        public static string DesktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\LinkedInDominator";
        public static int no_of_accounts_to_be_selected_for_compose_message = 0;
        public static Dictionary<string, Dictionary<string, string>> MessageContacts = new Dictionary<string, Dictionary<string, string>>();
        public static string selected_account_for_compose_message = string.Empty;
        public static Dictionary<string, string> SelectedContacts = new Dictionary<string, string>();


        public static string CapchaLoginID = string.Empty;
        public static string CapchaLoginPassword = string.Empty;


        public static bool IsFreeVersion = false;

        public static string checkcheduleSetting = string.Empty;

        public static List<string> listAccounts = new List<string>();

        public static AccountMode accountMode;
        public static string CheckLicenseManager = string.Empty;
        public static string LicenseCheckUserName = string.Empty;
        public static string LicenseCheckDate = string.Empty;
        public static string Licence_Details = string.Empty;

        public static string DeCaptcherHost = string.Empty;
        public static string DeCaptcherPort = string.Empty;
        public static string DeCaptcherUsername = string.Empty;
        public static string DeCaptcherPassword = string.Empty;
        public static string DBCUsername = string.Empty;
        public static string DBCPassword = string.Empty;
        public static string EmailsFilePath = string.Empty;
        public static bool IsMobileVersion = false;
        public static bool IsCopyLoggerData = false;
        public static bool IsGlobalDelay = false;
        public static int MinGlobalDelay = 20;
        public static int MaxGlobalDelay = 25;
        public static bool IsCheckValueOfDelay = true;
        public static bool proxyNotWorking = false;
        public static bool IsRefreshAccountExecute = false;
        //for global delay settings: if two module is working then it sholuld activate.
        public static string FollowerRunningText = string.Empty;
        public static string TweetRunningText = string.Empty;
        //public static bool IsMobileVersion = false;

        public static bool isQuoteModuleStop = false;
        public static List<Thread> lstOfRunningQuoteThread = new List<Thread>();

        //public static string FbAccountDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\FaceDominatorFbAccount";

        //public static List<string> lstDesktopFilePaths = new List<string>() { Path.Combine(FbAccountDesktopPath, "DisableFbAccount.txt"), Path.Combine(FbAccountDesktopPath, "IncorrectFbAccount.txt"), Path.Combine(FbAccountDesktopPath, "PhoneVerifyFbAccount.txt"), Path.Combine(FbAccountDesktopPath, "CorrectFbAccount.txt"), Path.Combine(FbAccountDesktopPath, "TemporarilyFbAccount.txt"), Path.Combine(FbAccountDesktopPath, "AccountNotConfirmed.txt"), Path.Combine(FbAccountDesktopPath, "CorrectFbAccount.txt") };

        public static string path_AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\TwtDominator";

        public static string path_ScrapedImageFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ScrapedImageFolder";

        public static string path_DesktopUsedTweetedImage = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\UsedTweetedImage";

        public static string path_DesktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator";

        public static string path_FailedLoginAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\FailedLogins.txt";

        public static string path_FailedToFollowAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\FailedToFollow.txt";

        public static string path_AskingCaptchaAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\AskingCaptcha.txt";

        public static string path_SuccessfullyFollowAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\SuccessfullyFollow.txt";

        public static string path_SuccessfullyFollowedArabAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\SuccessfullyAuthorizedArabFollowAccounts.txt";

        public static string path_AlreadtAuthorizedArabFollowAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\AlreadyAuthorizedArabFollowAccounts.txt";

        public static string path_SuccessfullyNotFollowedArabAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\FailedArabFollowAccounts.txt";

        public static string path_SuccessfullyDirectMessageSend = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\SuccessfullyMessageSend.txt";

        public static string path_FailedToUnfollowAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\FailedToUnfollow.txt";

        public static string path_CaptchaImages = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\CaptchaImages";

        public static string path_SuccessfullyUnfollowAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\SuccessfullyUnfollowed.txt";

        public static string path_FailedToTweetAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\FailedToTweet.txt";

        public static string path_SuccessfullyTweetAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\SuccessfullyTweeted.txt";

        public static string path_SuccessfullyRepliedAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\SuccessfullyReplied.txt";

        public static string path_FailedRepliedAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\SuccessfullyFailed.txt";

        public static string path_FailedToProfileAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\FailedToProfile.txt";

        public static string path_SuccessfullyProfiledAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\SuccessfullyProfiled.txt";

        public static string path_ExistingIDs = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ExistingUserIDsByUserName.txt";

        public static string path_NonExistingIDs = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\NonExistingUserIDsByUsername.txt";

        public static string path_StoreKeywordTweetExtractor = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\TweetExtractor";

        public static string path_StoreKeywordTweetExtractorForQuoteModule = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\TweetExtractor";

        //  public static string Path_SuccessfullyTweetedUrls = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\SuccessfullyTweetedUrls";

        //*************Check Account By Email required file *********

        public static string Path_SuccessfullyTweetedUrls = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\SuccessfullyTweetedUrls";

        public static string path_ActiveEmailAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ActiveEmailAccounts.txt";

        public static string path_EmailWithLoginErrorAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\AccountsWithLoginError.txt";

        public static string path_RequiredEmailVerificationAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\RequiredEmailVerificationAccounts.txt";

        public static string path_SuspendedEmailAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\SuspendedEmailAccounts.txt";

        public static string path_EmailisNotCurrectOrIncorrectPass = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\EmailIsNotCorrectOrIncorrectPass.txt";

        public static string path_ReplyInformation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ReplyInformation.txt";

        public static string path_RetweetInformation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ReTweetInformation.txt";

        public static string path_FailedCreatedAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\FailedCreatedAccounts.txt";
        public static string path_FailedCreatedAccountsOnlyEmailPass = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\FailedCreatedAccountsOnlyEmailPass.txt";

        public static string path_SuccessfulCreatedAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\SuccessfulCreatedAccounts.txt";

        public static string path_EmailVerifiedAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\EmailVerifiedAccounts.txt";

        public static string path_NonEmailVerifiedAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\NonEmailVerifiedAccounts.txt";

        public static string path_EmailAlreadyTaken = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\EmailAlreadyTaken.txt";

        public static string Path_Non_ExistingProxies = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\NonExistingProxies.txt";

        public static string Path_ExsistingProxies = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ExistingProxies.txt";

        public static string Path_ExsistingPvtProxies = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ExistingPrivateProxies.txt";

        public static string Path_ScrappedIDs = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ScrappedIDs.txt";

        public static string Path_ScrapedUserID = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ScrappedUserIds.csv";

        public static string Path_TweetExtractor = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\TweetExtractor.txt";
        public static string Path_TweetExtractorCSV = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\TweetExtractor.csv";

        public static string Path_TweetExtractorUpload = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\UploadTweetExtractor.txt";

        public static string Path_RETweetExtractor = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ReTweetExtractor.txt";

        public static string Path_RETweetExtractorCSV = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ReTweetExtractor.csv";

        public static string Path_RETweetExtractorUpload = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\UploadReTweetExtractor.txt";

        public static string Path_keywordFollowerScrapedData = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\UserScrapedData.txt";

        public static string Path_SpinnedTweets = path_DesktopFolder + "\\SpinnedTweets.txt";

        public static List<string> lstScrapedUserIDs = new List<string>();

        public static string Path_FakeEmailIds = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\FakeEmailAccounts.txt";

        public static string Path_ScrapedFollowersList = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ScrapedFollowersList.csv";

        public static string Path_ScrapedTweetedInSpecificeDays = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ScrapedTweetedInSpecificeDays.csv";

        public static string Path_ScrapedFollowersListtxt = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ScrapedFollowersList.txt";

        public static string Path_ScrapedMembersList = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ScrapedMembersList.csv";

        public static string Path_ScrapedFollowingsList = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ScrapedFollowingsList.csv";

        public static string Path_ScrapedHashTagList = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ScrapedHashTagList.csv";

        public static string Path_ScrapedFollowingsListtxt = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ScrapedFollowingsList.txt";

        public static string Path_KeywordScrapedList = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\KeywordScrapedList.csv";

        public static string Path_KeywordScrapedListData = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\KeywordScrapedListData";

        public static string Path_UserListInfoData = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\UserBioInfoData.csv";

        public static string Path_KeywordScrapedListWithoutDuplicates = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\KeywordScrapedList(Without_Duplicates).csv";

        public static string Path_TwtErrorLogs = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\ErrorLog.txt";

        public static string Path_HashtagsStore = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\HashTags.txt";

        public static string Path_BrowserCreatedAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\BrowserCreatedAccounts.txt";

        public static string path_Group_Name = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\GroupName.txt";
        public static string path_SuspendedIDs = path_DesktopFolder + "\\SuspendedIDs.txt";

        public static bool TwtFakeAccounts = false;

        public static Regex IdCheck = new Regex("^[0-9]*$");

        public static List<string> FakeEmailList = new List<string>();

        public static bool IsUseFakeEmailAccounts = false;

        public static string Path_tweetMessagePath = string.Empty;

        public static bool IsDirectedFromFollower = false;

        public static string Path_FollowSettings = path_DesktopFolder + "\\FollowSettings.txt";

        public static string Path_waitandreplySetting = path_DesktopFolder + "\\WaitAndReplySetting.txt";

        public static string Path_TweetSettings = path_DesktopFolder + "\\TweetSettings.txt";

        public static List<string> HashTags = new List<string>();

        public static string[] Array = new string[4];

        public static bool IsCampaign = false;

        public static string Campaign_Name = string.Empty;
        /// <summary>
        /// Log Files Declartion
        /// </summary>

        public static string Path_AccountCreatorErrorLog = path_AppDataFolder + "\\ErrorAccountCreator.txt";

        public static string Path_ProfileManagerErroLog = path_AppDataFolder + "\\ErrorProfileManager.txt";

        public static string Path_FollowerErroLog = path_AppDataFolder + "\\ErrorFollower.txt";

        public static string Path_TweetingErroLog = path_AppDataFolder + "\\ErrorTweet.txt";

        public static string Path_AccountCheckerErroLog = path_AppDataFolder + "\\ErrorAccountChecker.txt";

        public static string Path_UnfollowerErroLog = path_AppDataFolder + "\\ErrorUnFollower.txt";

        public static string Path_WaitNreplyErroLog = path_AppDataFolder + "\\ErrorWaitnReply.txt";

        public static string Path_ProxySettingErroLog = path_AppDataFolder + "\\ErrorProxySetting.txt";

        public static string Path_ScrapeUsersErroLog = path_AppDataFolder + "\\ErrorScrapeUsers.txt";

        public static string Path_FilterUsersLog = path_AppDataFolder + "\\ErrorFilterUsers.txt";

        public static string Path_TweetCreatorErroLog = path_AppDataFolder + "\\ErrorTweetCreator.txt";

        public static string Path_FakeEmailCraetorErroLog = path_AppDataFolder + "\\ErrorFakeEmailCreator.txt";

        public static string Path_DMErroLog = path_AppDataFolder + "\\ErrorDM.txt";

        public static string Path_TwitterDataScrapper = path_AppDataFolder + "\\ErrorDataScraper.txt";

        public static string Path_TweetAccountManager = path_AppDataFolder + "\\ErrorTweetAccountManager.txt";

        public static string Path_CampaignManager = path_AppDataFolder + "\\ErrorCampaign.txt";

        public static string Path_ErrortweetCreator = path_AppDataFolder + "\\ErrortweetCreator.txt";

        public static string Path_MobEmailCreator = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\MobEmailCreator.txt";

        public static string Path_MobTweets = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\MobTweet.txt";

        public static string Path_NotMobEmailCreator = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\AccountsNotCreatedByMob.txt";

        public static string Path_MobEmailProfiled = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\AccountsProfilesByMob.txt";

        public static string Path_NotMobEmailProfiled = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\AccountsNotProfiledByMob.txt";

        public static string Path_NotMobEmailTweeted = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\AccountsNotTweetedByMob.txt";

        public static string Path_MobTweeted = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\AccountsTweetedByMob.txt";

        public static string Path_CheckAccountByEmail = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\getusernamefromaccount.csv";

        public static string Path_BlackListedUser = path_DesktopFolder + "\\BlackListedUser.txt";

        public static string Path_UserNotExistListedUser = path_DesktopFolder + "\\UserNotExist.txt";

        public static string Path_WhiteListedUser = path_DesktopFolder + "\\WhiteListedUser.txt";

        public static string Path_CaptchaRequired = path_DesktopFolder + "\\CaptchaRequired.txt";

        public static string Path_AccountUploadingErrorLog = path_AppDataFolder + "\\AccountUploadingError.txt";

        public static string path_App_FollowAccountInfo = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\App_Follow_Info.txt";



        public static string path_SuccessfullyFollowedApplicationAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\SuccessfullyAuthorizedApplicationFollowAccounts.txt";

        public static string path_AlreadtAuthorizedApplicationFollowAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\AlreadyAuthorizedApplicationFollowAccounts.txt";

        public static string path_SuccessfullyNotFollowedApplicationAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\FailedApplicationFollowAccounts.txt";


        public static void InitializeDelegate()
        {

        }



        #region << Header image log file >>


        public static string Path_ErrorLogForHeaderImage = path_DesktopFolder + "\\HeaderImageChangeErrorLog.txt";

        public static string Path_SuccessfullyChangedHeaderImage = path_DesktopFolder + "\\SuccessfullyChangedHeaderImage.txt";

        public static string Path_FailedHeaderImageChanging = path_DesktopFolder + "\\FailedToChangeHeaderImage.txt";


        #endregion



        #region << Account Manager log file >>


        public static string Path_ErrorLogForAccountManager = path_DesktopFolder + "\\AccountManagerModuleErrorLog.txt";

        //public static string Path_SuccessfullyChangedAccountDetails = path_DesktopFolder + "\\SuccessfullyChangedAccountDetails.txt";

        //public static string Path_FailedAccountDetailsChanging = path_DesktopFolder + "\\FailedToChangeAccountDetails.txt";

        public static string Path_SuccessFullyPasswordChange = path_DesktopFolder + "\\PasswordChanged.txt";

        public static string Path_FailedChangingPass = path_DesktopFolder + "\\FailedPasswordChange.txt";


        //public static string Path_SuccessFillyChangeUserAndEmail = path_DesktopFolder + "\\SuccessFillyChangeUserAndEmail.txt";

        public static string Path_SuccessFullyChangeEmail = path_DesktopFolder + "\\SuccessFullyChangeEmail.txt";
        public static string Path_SuccessFullyChangeUserName = path_DesktopFolder + "\\SuccessFullyChangeUserName.txt";

        public static string Path_FailedChangingUserAndEmail = path_DesktopFolder + "\\UserAndEmailChangingFailed.txt";



        #endregion


        public static string path_SuccessfullyVerifiedAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\SuccessfullyVerifiedAccounts.txt";
        public static string path_VerificationFailedAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\VerificationFailedAccounts.txt";

        //***************************************************

        public static string path_SucessfullyRetweetToUrlFromId = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\SucessfullyRetweetedAccounts.txt";
        public static string path_FailureRetweetToUrlFromId = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\FailureRetweetToUrlFromId.txt";
        public static string path_SucessfullyFavoriteToUrlFromId = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\SucessfullyFavouriteAccountInfo.txt";
        public static string path_FailureFavoriteToUrlFromId = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\FailureFavoriteToUrlFromId.txt";

        public static string path_LogErrorFromRetweetAndFavorit = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\LogErrorFromRetweetAndFavorit.txt";

        public static string path_LogCampaignCompleted = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\CampaignCompleted.txt";
        public static string path_CountNoOfProcessDone = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TwtDominator\\CountNoOfProcessDone.txt";
        public static int totalcountFollower = 0;
        public static int totalcountTweet = 0;
        //////////***********End error Log Declartion*************/////////////////////////////////////

        //public static string DisableFbAccountPath = string.Empty;
        //public static string DisableFbAccountPath = string.Empty;
        //public static string DisableFbAccountPath = string.Empty;
        //public static string DisableFbAccountPath = string.Empty;
        //public static string DisableFbAccountPath = string.Empty;

        #region Stopping Variables

        public static string threadNaming_WaitAndReply_ = "WaitAndReply_";
        public static string threadNaming_Tweet_ = "Tweet_";
        public static string threadNaming_Retweet_ = "Retweet_";
        public static string threadNaming_Reply_ = "Reply_";
        public static string threadNaming_Follow_ = "Follow_";
        public static string threadNaming_Unfollow_ = "Unfollow_";
        public static string threadNaming_ProfileManager_ = "ProfileManager_";
        public static string threadNaming_AccountCreation_ = "AccountCreator_";
        public static string threadNaming_WhoToFollow_ = "WhoToFollow_";
        #endregion
        public static string strModule(Module module)
        {
            switch (module)
            {
                case Module.WaitAndReply:
                    return threadNaming_WaitAndReply_;

                case Module.Tweet:
                    return threadNaming_Tweet_;

                case Module.Retweet:
                    return threadNaming_Retweet_;

                case Module.Reply:
                    return threadNaming_Reply_;

                case Module.Follow:
                    return threadNaming_Follow_;

                case Module.Unfollow:
                    return threadNaming_Unfollow_;

                case Module.ProfileManager:
                    return threadNaming_ProfileManager_;

                case Module.AccountCreation:
                    return threadNaming_AccountCreation_;

                case Module.WhoToScrap:
                    return threadNaming_WhoToFollow_;
                default:
                    return "";
            }
        }

        public string DataFormat(string format, params string[] dataArr)
        {
            string dataFormat = string.Empty;
            try
            {
                foreach (string item in dataArr)
                {
                    try
                    {
                        dataFormat = dataFormat + item + format;
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (dataFormat.EndsWith(format))
                {
                    dataFormat = dataFormat.Remove(dataFormat.LastIndexOf(format));
                }
            }
            catch (Exception ex)
            {
            }
            return dataFormat;
        }


        private static object locker_queDataForSignUp = new object();

        public static Queue<object[]> queDataForSignUp = new Queue<object[]>();

        public static object[] DequequeDataForSignUp()
        {
            object[] data = null;
            try
            {
                lock (locker_queDataForSignUp)
                {
                    if (queDataForSignUp.Count > 0)
                    {
                        data = queDataForSignUp.Dequeue();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return data;
        }

        public static void EnquequeDataForSignUp(object[] data)
        {
            try
            {
                lock (locker_queDataForSignUp)
                {
                    queDataForSignUp.Enqueue(data);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static int GetCountqueDataAfterEmailVerification()
        {
            int count = 0;
            try
            {
                lock (locker_queDataForSignUp)
                {
                    count = queDataForSignUp.Count;
                }
            }
            catch (Exception ex)
            {
            }
            return count;
        }

        private static object locker_queWorkingProxiesForSignUp = new object();
        public static Queue<string> queWorkingProxiesForSignUp = new Queue<string>();

        public static void EnquequeWorkingProxiesForSignUp(string proxy)
        {
            try
            {
                lock (locker_queWorkingProxiesForSignUp)
                {
                    queWorkingProxiesForSignUp.Enqueue(proxy);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public static string DequequeWorkingProxiesForSignUp()
        {
            string proxy = "";
            try
            {
                lock (locker_queWorkingProxiesForSignUp)
                {
                    if (queWorkingProxiesForSignUp.Count > 0)
                    {
                        proxy = queWorkingProxiesForSignUp.Dequeue();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return proxy;
        }

        public static int GetCountqueWorkingProxiesForSignUp()
        {
            int count = 0;
            try
            {
                lock (locker_queWorkingProxiesForSignUp)
                {
                    count = queWorkingProxiesForSignUp.Count;
                }
            }
            catch (Exception ex)
            {
            }
            return count;
        }

    }

    public static class CampaignDetails
    {
        #region Globals variable for Follow campaign module

        public static string followCampaignName = string.Empty;
        public static string followCampaignAccount = string.Empty;
        public static string followCampaignFollowUserPath = string.Empty;
        public static bool followCampaignDividEql = false;
        public static int followCampaignDivideByUser = 5;
        public static int followCampaignNoOfUser = 10;
        public static int followCampaignFastFollow = 0;
        public static int followCampaignNoOfFollowPerAccount = 10;
        public static int followCampaignScheduledDaily = 0;
        public static string followCampaignStartTime = string.Empty;
        public static string followCampaignEndTime = string.Empty;
        public static int followCampaignDelayMax = 25;
        public static int followCampaignDelayMin = 10;
        public static int followCampaignNoOfThread = 25;
        public static int followCampaignScraperUserCount = 50;
        public static bool followCampaignIsScrapeUserToFollow = false;
        public static string followCampaignScrapeUserKeywordPath = string.Empty;

        #endregion

        #region Globals variable for retweet campaign module

        public static string retweetCampaignName = string.Empty;
        public static string retweetCampaignAccount = string.Empty;
        public static string retweetCampaignKeyword = string.Empty;
        public static int retweetCampaignIsUserName = 0;
        public static int retweetCampaignUniqueMessage = 5;
        public static int retweetCampaignRetweetParDay = 10;
        public static int retweetCampaignNoOfRetweetParDay = 10;
        public static int retweetCampaignNoOfRetweetParAccount = 10;
        public static int retweetCampaignScheduledDaily = 0;
        public static string retweetCampaignStartTime = string.Empty;
        public static string retweetCampaignStopTime = string.Empty;
        public static int retweetCampaignDelayMax = 30;
        public static int retweetCampaignDelayMin = 15;
        public static int retweetCampaignThread = 25;
        public static string retweetCampaignModule = "Retweet";

        #endregion

        #region Globals variable for reply campaign module

        public static string replyCampaignCampaignName = string.Empty;
        public static string replyCampaignAcFilePath = string.Empty;
        public static string replyCampaignReplyFilePath = string.Empty;
        public static string replyCampaignKeyword = string.Empty;
        public static int replyCampaignIsUsername = 0;
        public static int replyCampaignUniqueMessage = 0;
        public static int replyCampaignReplyParDay = 0;
        public static int replyCampaignNoofReplyParDay = 10;
        public static int replyCampaignNoofReplyParAc = 5;
        public static int replyCampaignScheduledDaily = 0;
        public static int replyCampaignIsDuplicateMessage = 0;
        public static string replyCampaignStartTime = string.Empty;
        public static string replyCampaignEndTime = string.Empty;
        public static int replyCampaignDelayMin = 15;
        public static int replyCampaignDelayMax = 30;
        public static int replyCampaignThreads = 25;
        public static string replyCampaignModule = "Reply";

        #endregion

        #region Globals variable for Tweet campaign module

        public static string tweetCampaignCampaignName = string.Empty;
        public static string tweetCampaignAcFilePath = string.Empty;
        public static string tweetCampaignTweetMessageFilePath = string.Empty;
        public static string tweetCampaignTweetImageFolderPath = string.Empty;
        public static string tweetCampaignTweetUploadUserFilePath = string.Empty;

        public static int tweetCampaignIsUploadUserFilePath = 0;
        public static int tweetCampaignIsUniqueMessage = 0;
        public static int tweetCampaignIsAllTweetParAcc = 0;
        public static int tweetCampaignIsTweetParDay = 0;
        public static int tweetCampaignIsTweetWithImage = 0;
        public static int tweetCampaignIsScheduledDaily = 0;
        public static int tweetCampaignIsDuplicateMessage = 0;
        public static int tweetCampaignIsHashTag = 0;
        public static int tweetCampaignIsTweetedInLastSpecificDay = 0;
        public static int tweetCampaignIsTweetMentionUserScrapedList = 0;
        public static int tweetCampaignIsTweetFollowingScrapedList = 0;
        public static int tweetCampaignIsTweetFollowersScrapedList = 0;

        public static int tweetCampaignNoofTweetParDay = 10;
        public static int tweetCampaignNoofTweetParAc = 5;
        public static int tweetCampaignNoOfTweetMentionUserScrapedList = 5;
        public static int tweetCampaignNoOfScrapedUserScrapedList = 50;
        public static int tweetCampaignNoOfDayTweetedByUser = 5;
        public static int tweetCampaignNoOfRecuring = 5;

        public static string tweetCampaignIsRecureProcess = string.Empty;
        public static string tweetCampaignTweetMentionUserNameScrapedList = string.Empty;
        public static string tweetCampaignStartTime = string.Empty;
        public static string tweetCampaignEndTime = string.Empty;
        public static int tweetCampaignDelayMin = 15;
        public static int tweetCampaignDelayMax = 30;
        public static int tweetCampaignThreads = 25;
        public static string tweetCampaignModule = "Tweet";

        #endregion
    }



    public enum Module
    {
        Tweet, Retweet, Reply, Follow, Unfollow, ProfileManager, Proxies, AccountCreation, WaitAndReply, WhoToScrap
    }

    public enum AccountMode
    {
        NoProxy, PublicProxy, PrivateProxy
    }

}
