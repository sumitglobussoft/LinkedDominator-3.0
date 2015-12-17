using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkDominator;

namespace linkedDominator
{
    /// <summary>
    /// Globals is implemented as a singleton. It contains all the Facebook Urls, Parsed Data and Post Data
    /// Globals is Thread Safe.
    /// </summary>
    public sealed class LDGlobals
    {
        /// <summary>
        /// Contains all the accounts and related Information
        /// </summary>       
        // DBC Setting

        public static string dbcUserName = string.Empty;
        public static string dbcPassword = string.Empty;


        public static Dictionary<string, LinkedinUser> loadedAccountsDictionary = new Dictionary<string, LinkedinUser>();

        public static List<string> listAccounts = new List<string>();

        public readonly string LDhomeurl = "https://www.Linkedin.com/";
       
        
        


        //String Flag values

        public readonly string registrationSuccessString = "\"registration_succeeded\":true";
        public readonly string registrationErrorString = "\"error\":";

        public readonly string ldhomepath = @"C:\FaceDominator";
        public readonly string lddatapath = @"C:\FaceDominator\Data";
        public readonly string lddbfilename = @"\FaceDominator.db";

        public bool isfreeversion = false;


        /// <summary>
        /// Singleton object declaration.
        /// </summary>
        
        private static volatile LDGlobals globals = null;
        private static object syncRoot = new object(); 

        
        public static LDGlobals Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (globals == null)
                    {
                        globals = new LDGlobals();
                        
                    }
                }
            return globals;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private LDGlobals()
        {
        }
       
    }
}
