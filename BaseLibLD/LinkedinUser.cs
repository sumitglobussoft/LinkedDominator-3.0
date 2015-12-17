using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseLib;
using Globussoft;

namespace LinkDominator
{
    public class LinkedinUser
    {
        public GlobusHttpHelper globusHttpHelper = new GlobusHttpHelper();
        public bool isloggedin;

        public Guid Id { get; set; }
        public string username;
        public string password;
        public string proxyip;
        public string proxyport;
        public string proxyusername;
        public string proxypassword;
        public string proxytype; //http or socks

      

        public enum AccountStatus { 
        AccountCreated,AccountEmailVerified,AccountPhoneVerified,AccountIncorrectEmail,AccountPhoneEmailVerified,
            AccountTempLocked,AccountUndefinedError,Account30DaysBlock,AccountUnverified,AccountPVARequired,AccountDisabled
        };

       
        public string birthdaymonth;
        public string birthdayday;
    
        
        public string profilepic;
        public string quotations;
        public string dateOfBirth;
        public string securityAnswer;
        public string dbcUsername;
        public string dbcPassword;
        

        public LinkedinUser()
        {

        }


    }
}

