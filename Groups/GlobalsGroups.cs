using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Groups
{
    public class GlobalsGroups
    {
        public static List<string> lstGroups = new List<string>();
        public static List<string> SelectedGroups = new List<string>();

        #region Settings
        public static string selectedAccount { get; set; }
        public static int minDelay { get; set; }
        public static int maxDelay { get; set; }
        public static bool chkGroup { get; set; }
        #endregion

        #region Others
        public static int countThreadControllerGroupStatus { get; set; }
        public static List<Thread> lstThreadsGroupStatus =new List<Thread>();
        public static bool isStopGroupStatus { get; set; }
        public static int NoOfThreadsGroupStatus { get; set; }
        public static List<string> lstStatusHeader = new List<string>();
        public static List<string> ListGrpDiscussion = new List<string>();
        public static List<string> ListGrpMoreDetails = new List<string>();

        public static int NoOfThreadsSendUpdate { get; set; }
        public static int countThreadControllerSendUpdate { get; set; }
        public static bool isStopSendUpdate = false;
        public static List<Thread> lstThreadsSendUpdate = new List<Thread>();
        public static Queue<string> Que_GrpPostTitle_Post = new Queue<string>();
        public static List<string> GroupUrl = new List<string>();
        public static Queue<string> Que_GrpKey_Post = new Queue<string>();
        public static Queue<string> Que_GrpMoreDtl_Post = new Queue<string>();
        public static Queue<string> Que_GrpAttachLink_Post = new Queue<string>();
        #endregion

        #region Get Groups
        public static int NoOfThreadsGetGroups { get; set; }
        public static int countThreadControllerGetGroups { get; set; }
        #endregion
    }
}
