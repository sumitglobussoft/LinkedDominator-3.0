using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Add_Connection
{
    public class GlobalsAddConn
    {
        #region Settings
        public static int minDelay { get; set; }
        public static int maxDelay { get; set; }
        public static bool selectedUniqueConnection { get; set; }
        public static int requestPerKeyword { get; set; }
        public static bool selectedOnlyVisit { get; set; }
        public static bool selectedClearDB { get; set; }
        public static bool selectedDailyLimit { get; set; }
        public static int dailyLimit { get; set; }
        public static bool selectedDivideData { get; set; }
        public static bool selectedRdbDivideEqually { get; set; }
        public static bool selectedRdbDivideGivenByUser { get; set; }
        public static int noOfUsers { get; set; }
        public static bool selectedManageConnEmail { get; set; }
        public static bool selectedManageConnKeyword { get; set; }
        #endregion

        #region Others
        public static int NoOfThreadsAddConn { get; set; }        
        public static int countThreadControllerAddConn { get; set; }
        public static bool isStopAddConn { get; set; }
        public static List<Thread> lstThreadsAddConn { get; set; }
        //public static List<string> lstKeywords { get; set; }

        public static List<string> lst_Emails_for_AddConnection = new List<string>();
        public static List<string> lst_keyWords_for_AddConnection=new List<string>();
       // public static List<string> lstLoadEmails { get; set; }
        public static int no_of_profiles_can_be_visited { get; set; }
        public static bool CheckTheDayLimit { get; set; }
        public static int no_of_profiles_visited { get; set; }
        #endregion
    }
}
