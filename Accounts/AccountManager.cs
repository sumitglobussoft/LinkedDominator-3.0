using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Globussoft;
using OpenPOP.POP3;
using BaseLib;
using System.IO;
using System.Data;

using linkedDominator;
using LinkDominator;





namespace Accounts
{
    public class AccountManager
    {
        public AccountManager()
        {

        }

        public void LoginHttpHelper(ref LinkedinUser linkedUser)
        {
            try
            {
                GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();
                linkedUser.globusHttpHelper = objGlobusHttpHelper;
                GlobusHttpHelper HttpHelper = linkedUser.globusHttpHelper;


               GlobusLogHelper.log.Info("Logging In With Account : " + linkedUser.username );
               GlobusLogHelper.log.Info("Login Process is Running... ");
               string Url = LDGlobals.Instance.LDhomeurl;
                string pageSrcLogin = string.Empty;
                int ProxyPort = 0;
                if (!string.IsNullOrEmpty(linkedUser.proxyport) && NumberHelper.ValidateNumber(linkedUser.proxyport))
                {
                    ProxyPort = int.Parse(linkedUser.proxyport);
                }
                pageSrcLogin = HttpHelper.getHtmlfromUrlProxy(new Uri(Url),"", linkedUser.proxyip, linkedUser.proxyport, linkedUser.proxyusername, linkedUser.proxypassword);
                if (pageSrcLogin.Contains("Sign Out"))
                {                  
                    return;
                }
                string postdata = string.Empty;
                string postUrl = string.Empty;
                string ResLogin = string.Empty;
                string csrfToken = string.Empty;
                string regCsrfParam = string.Empty;
                string sourceAlias = string.Empty;


                if (pageSrcLogin.Contains("csrfToken"))
                {
                    try
                    {
                        int startIndex = pageSrcLogin.IndexOf("name=\"csrfToken\"");
                        string start = pageSrcLogin.Substring(startIndex).Replace("name=\"csrfToken\"", "");
                        int endIndex = start.IndexOf("\" ");
                        string end = start.Substring(0, endIndex).Replace("value=\"", "").Trim();
                        csrfToken = end;
                        if (csrfToken.Contains(LDGlobals.Instance.LDhomeurl))
                        {
                            csrfToken = Utils.getBetween("@@@@@@@" + csrfToken, "@@@@@@@", "\"/></form>");
                        }
                        //csrfToken = csrfToken;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " +ex.StackTrace);
                    }

                }

                try
                {
                    if (csrfToken.Contains("&"))
                    {
                        string[] Arr = csrfToken.Split('&');
                        csrfToken = Arr[0].Replace("\"", string.Empty);

                    }

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                if (pageSrcLogin.Contains("sourceAlias"))
                {
                    sourceAlias = pageSrcLogin.Substring(pageSrcLogin.IndexOf("sourceAlias"), 100);
                    string[] Arr = sourceAlias.Split('"');
                    sourceAlias = Arr[2].Replace(@"\", string.Empty).Replace("//", string.Empty).Trim();
                }


                try
                {
                    int SourceAliasStart = pageSrcLogin.IndexOf("regCsrfParam");
                    if (SourceAliasStart > 0)
                    {
                        try
                        {

                            regCsrfParam = pageSrcLogin.Substring(pageSrcLogin.IndexOf("regCsrfParam"), 100);
                            string[] Arr = regCsrfParam.Split('"');
                            regCsrfParam = Arr[2].Replace(@"\", string.Empty).Replace("//", string.Empty);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (string.IsNullOrEmpty(regCsrfParam))
                    {
                        regCsrfParam = Utils.getBetween(pageSrcLogin, "loginCsrfParam", "/>");
                        regCsrfParam = Utils.getBetween(regCsrfParam, "value=\"", "\"");
                    }

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                postUrl = "https://www.linkedin.com/uas/login-submit";
                postdata = "isJsEnabled=true&source_app=&tryCount=&session_key=" + Uri.EscapeDataString(linkedUser.username) + "&session_password=" + Uri.EscapeDataString(linkedUser.password) + "&signin=Sign%20In&session_redirect=&loginCsrfParam=" + regCsrfParam + "&csrfToken=" + csrfToken + "&sourceAlias=" + sourceAlias;
                 
                int port=80;
                try
                {
                  port  = Convert.ToInt32(linkedUser.proxyport);
                    ResLogin = HttpHelper.postFormDataProxy(new Uri(postUrl), postdata, "", linkedUser.proxyip, port, linkedUser.proxyusername, linkedUser.proxypassword);//HttpHelper.postFormDataRef(new Uri(postUrl), postdata, "https://www.linkedin.com/uas/login?goback=&trk=hb_signin", "", "");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                if (ResLogin.Contains("Request Error") || ResLogin.Contains("Join Today"))
                {
                    Url = "https://www.linkedin.com/";
                    postdata = "isJsEnabled=true&source_app=&tryCount=&clickedSuggestion=false&session_key=" + Uri.EscapeDataString(linkedUser.username) + "&session_password=" + Uri.EscapeDataString(linkedUser.password) + "&signin=Sign%20In&session_redirect=&trk=hb_signin&loginCsrfParam=" + regCsrfParam + "&fromEmail=&csrfToken=" + csrfToken + "&sourceAlias=" + sourceAlias + "&client_ts=1434028850511&client_r=rairaaz4%40gmail.com%3A910202384%3A297133466%3A594693585&client_output=-480305440&client_n=910202384%3A297133466%3A594693585&client_v=1.0.1";
                    ResLogin = HttpHelper.postFormDataProxy(new Uri(postUrl), postdata,"", linkedUser.proxyip, port,  linkedUser.proxyusername,  linkedUser.proxypassword);
                }


                string ImageUrl = string.Empty;
                string captchaText = string.Empty;
                string captchachallengeid = string.Empty;
                string dts = string.Empty;
                string origActionAlias = string.Empty;
                string origSourceAlias = string.Empty;
                string irhf = string.Empty;
                string submissionID = string.Empty;
                string CAPTCHAfwdcsrftoken = string.Empty;
                string CAPTCHAfwdsignin = string.Empty;
                string CAPTCHAfwdsession_password = string.Empty;
                string CAPTCHAfwdsession_key = string.Empty;
                string CAPTCHAfwdisJsEnabled = string.Empty;
                string CAPTCHAfwdloginCsrfParam = string.Empty;


                #region coommented seurity verification
                //if (ResLogin.Contains("Security Verification"))
                //{
                //    string dataforcapctha = HttpHelper.getHtmlfromUrl1(new Uri("https://www.google.com/recaptcha/api/noscript?k=6LcnacMSAAAAADoIuYvLUHSNLXdgUcq-jjqjBo5n"));
                //    if (!string.IsNullOrEmpty(dataforcapctha))
                //    {
                //        int startindex = dataforcapctha.IndexOf("id=\"recaptcha_challenge_field\"");
                //        if (startindex > 0)
                //        {
                //            string start = dataforcapctha.Substring(startindex).Replace("id=\"recaptcha_challenge_field\"", "");
                //            int endindex = start.IndexOf("\">");
                //            string end = start.Substring(0, endindex).Replace("value=", string.Empty).Replace("\"", string.Empty).Trim();
                //            ImageUrl = "https://www.google.com/recaptcha/api/image?c=" + end;
                //            System.Net.WebClient webclient = new System.Net.WebClient();
                //            byte[] args = webclient.DownloadData(ImageUrl);
                //            string[] arr1 = new string[] { Globals.CapchaLoginID, Globals.CapchaLoginPassword, "" };
                //            captchaText = DecodeDBC(arr1, args);
                //        }

                //        if (ResLogin.Contains("name=\"security-challenge-id\""))
                //        {
                //            int startindexnew = ResLogin.IndexOf("name=\"security-challenge-id\"");
                //            if (startindexnew > 0)
                //            {
                //                string start = ResLogin.Substring(startindexnew).Replace("name=\"security-challenge-id\"", "").Replace("value=\"", "");
                //                int endindex = start.IndexOf("\"");
                //                string end = start.Substring(0, endindex);
                //                captchachallengeid = end.Replace("\"", string.Empty).Trim();
                //            }
                //        }

                //        if (ResLogin.Contains("name=\"dts\""))
                //        {
                //            int startindexnew = ResLogin.IndexOf("name=\"dts\"");
                //            if (startindexnew > 0)
                //            {
                //                string start = ResLogin.Substring(startindexnew).Replace("name=\"dts\"", "").Replace("value=\"", "");
                //                int endindex = start.IndexOf("\"");
                //                string end = start.Substring(0, endindex);
                //                dts = end.Replace("\"", string.Empty).Trim();
                //            }
                //        }

                //        if (ResLogin.Contains("name=\"origActionAlias\""))
                //        {
                //            int startindexnew = ResLogin.IndexOf("name=\"origActionAlias\"");
                //            if (startindexnew > 0)
                //            {
                //                string start = ResLogin.Substring(startindexnew).Replace("name=\"origActionAlias\"", "").Replace("value=\"", "");
                //                int endindex = start.IndexOf("\"");
                //                string end = start.Substring(0, endindex);
                //                origActionAlias = end.Replace("\"", string.Empty).Trim();
                //            }
                //        }

                //        if (ResLogin.Contains("name=\"submissionId\""))
                //        {
                //            int startindexnew = ResLogin.IndexOf("name=\"submissionId\"");
                //            if (startindexnew > 0)
                //            {
                //                string start = ResLogin.Substring(startindexnew).Replace("name=\"submissionId\"", string.Empty).Replace("value=\"", string.Empty);
                //                int endindex = start.IndexOf("\"");
                //                string end = start.Substring(0, endindex);
                //                submissionID = end.Replace("\"", string.Empty).Trim();
                //            }
                //        }
                //        if (ResLogin.Contains("name=\"CAPTCHA-fwd-csrfToken\""))
                //        {
                //            int startindexnew = ResLogin.IndexOf("name=\"CAPTCHA-fwd-csrfToken\"");
                //            if (startindexnew > 0)
                //            {
                //                string start = ResLogin.Substring(startindexnew).Replace("name=\"CAPTCHA-fwd-csrfToken\"", string.Empty).Replace("value=\"", string.Empty);
                //                int endindex = start.IndexOf("\"");
                //                string end = start.Substring(0, endindex);
                //                CAPTCHAfwdcsrftoken = end.Replace("\"", string.Empty).Trim();
                //            }
                //        }
                //        if (ResLogin.Contains("name=\"CAPTCHA-fwd-signin\""))
                //        {
                //            int startindexnew = ResLogin.IndexOf("name=\"CAPTCHA-fwd-signin\"");
                //            if (startindexnew > 0)
                //            {
                //                string start = ResLogin.Substring(startindexnew).Replace("name=\"CAPTCHA-fwd-signin\"", string.Empty).Replace("value=\"", string.Empty);
                //                int endindex = start.IndexOf("\"");
                //                string end = start.Substring(0, endindex);
                //                CAPTCHAfwdsignin = end.Replace("\"", string.Empty).Trim();
                //            }
                //        }
                //        if (ResLogin.Contains("name=\"CAPTCHA-fwd-session_password\""))
                //        {
                //            int startindexnew = ResLogin.IndexOf("name=\"CAPTCHA-fwd-session_password\"");
                //            if (startindexnew > 0)
                //            {
                //                string start = ResLogin.Substring(startindexnew).Replace("name=\"CAPTCHA-fwd-session_password\"", string.Empty).Replace("value=\"", string.Empty);
                //                int endindex = start.IndexOf("\"");
                //                string end = start.Substring(0, endindex);
                //                CAPTCHAfwdsession_password = end.Replace("\"", string.Empty).Trim();
                //            }
                //        }
                //        if (ResLogin.Contains("name=\"CAPTCHA-fwd-session_key\""))
                //        {
                //            int startindexnew = ResLogin.IndexOf("name=\"CAPTCHA-fwd-session_key\"");
                //            if (startindexnew > 0)
                //            {
                //                string start = ResLogin.Substring(startindexnew).Replace("name=\"CAPTCHA-fwd-session_key\"", string.Empty).Replace("value=\"", string.Empty);
                //                int endindex = start.IndexOf("\"");
                //                string end = start.Substring(0, endindex);
                //                CAPTCHAfwdsession_key = end.Replace("\"", string.Empty).Trim();
                //            }
                //        }
                //        if (ResLogin.Contains("name=\"CAPTCHA-fwd-isJsEnabled\""))
                //        {
                //            int startindexnew = ResLogin.IndexOf("name=\"CAPTCHA-fwd-isJsEnabled\"");
                //            if (startindexnew > 0)
                //            {
                //                string start = ResLogin.Substring(startindexnew).Replace("name=\"CAPTCHA-fwd-isJsEnabled\"", string.Empty).Replace("value=\"", string.Empty);
                //                int endindex = start.IndexOf("\"");
                //                string end = start.Substring(0, endindex);
                //                CAPTCHAfwdisJsEnabled = end.Replace("\"", string.Empty).Trim();
                //            }
                //        }
                //        if (ResLogin.Contains("name=\"CAPTCHA-fwd-loginCsrfParam\""))
                //        {
                //            int startindexnew = ResLogin.IndexOf("name=\"CAPTCHA-fwd-loginCsrfParam\"");
                //            if (startindexnew > 0) ;
                //            {
                //                string start = ResLogin.Substring(startindexnew).Replace("name=\"CAPTCHA-fwd-loginCsrfParam\"", string.Empty).Replace("value=\"", string.Empty);
                //                int endindex = start.IndexOf("\"");
                //                string end = start.Substring(0, endindex);
                //                CAPTCHAfwdloginCsrfParam = end.Replace("\"", string.Empty).Trim();
                //            }
                //        }


                //        if (ResLogin.Contains("name=\"origSourceAlias\""))
                //        {
                //            int startindexnew = ResLogin.IndexOf("name=\"origSourceAlias\"");
                //            if (startindexnew > 0)
                //            {
                //                string start = ResLogin.Substring(startindexnew).Replace("name=\"origSourceAlias\"", "").Replace("value=\"", "");
                //                int endindex = start.IndexOf("\"");
                //                string end = start.Substring(0, endindex);
                //                origSourceAlias = end.Replace("\"", string.Empty).Trim();
                //            }
                //        }

                //        if (ResLogin.Contains("name=\"irhf\""))
                //        {
                //            int startindexnew = ResLogin.IndexOf("name=\"irhf\"");
                //            if (startindexnew > 0)
                //            {
                //                string start = ResLogin.Substring(startindexnew).Replace("name=\"irhf\"", "").Replace("value=\"", "");
                //                int endindex = start.IndexOf("\"");
                //                string end = start.Substring(0, endindex);
                //                irhf = end.Replace("\"", string.Empty).Trim();
                //            }
                //        }

                //        if (!string.IsNullOrEmpty(ImageUrl) && !string.IsNullOrEmpty(captchaText))
                //        {
                //            //postdata = "recaptcha_challenge_field=" + ImageUrl.Replace("https://www.google.com/recaptcha/api/image?c=", string.Empty) + "&recaptcha_response_field=" + captchaText.Replace(" ", "+") + "&dts=" + dts + "&security-challenge-id=" + captchachallengeid + "&hr=&source_app=&csrfToken=" + csrfToken + "&isJsEnabled=true&session_redirect=&session_password=" + accountPass + "&session_key=" + Uri.EscapeDataString(accountUser) + "&origSourceAlias=" + origSourceAlias + "&origActionAlias=" + origActionAlias + "&irhf=" + irhf + "&sourceAlias=" + sourceAlias + "&submissionId=" + submissionID + ;
                //            postdata = "recaptcha_challenge_field=" + ImageUrl.Replace("https://www.google.com/recaptcha/api/image?c=", string.Empty) + "&recaptcha_response_field=" + captchaText.Replace(" ", "+") + "&irhf=" + irhf + "&dts=" + dts + "&security-challenge-id=" + captchachallengeid + "&submissionId=" + submissionID + "&CAPTCHA-fwd-csrfToken=" + CAPTCHAfwdcsrftoken + "&CAPTCHA-fwd-isJsEnabled=" + CAPTCHAfwdisJsEnabled + "&CAPTCHA-fwd-signin=" + CAPTCHAfwdsignin + "&CAPTCHA-fwd-loginCsrfParam=" + CAPTCHAfwdloginCsrfParam + "&CAPTCHA-fwd-session_password=" + CAPTCHAfwdsession_password + "&CAPTCHAfwd-session_key=" + CAPTCHAfwdsession_key + "&session_password=" + accountPass + "&session_key=" + Uri.EscapeDataString(accountUser) + "&origSourceAlias=" + origSourceAlias + "&origActionAlias=" + origActionAlias + "&csrfToken=" + csrfToken + "&sourceAlias=" + sourceAlias;
                //            postdata = postdata.Replace(" ", string.Empty);
                //            ResLogin = HttpHelper.postFormDataRef(new Uri("https://www.linkedin.com/uas/captcha-submit"), postdata, "https://www.linkedin.com/uas/login-submit", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                //        }
                //        else
                //        {
                //            ResLogin = string.Empty;
                //        }

                //        if (ResLogin.Contains("The text you entered does not match the characters in the security image. Please try again with this new image") || string.IsNullOrEmpty(ResLogin))
                //        {
                //            Log("[ " + DateTime.Now + " ] => [ " + accountUser + "  Cannot Login because of Captcha ]");
                //            GlobusFileHelper.WriteStringToTextfile(accountUser + ":" + accountPass + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUserName + ":" + proxyPassword, Globals.pathCapcthaLogin);
                //            SearchCriteria.loginREsponce = string.Empty;
                //            //FrmScrapGroupMember.ChangeToNextAccount = true;
                //        }
                //    }
                //}
                #endregion 

                if (ResLogin.Contains("Sign Out") || ResLogin.Contains("class=\"signout\"") || ResLogin.Contains("Cerrar sesión"))
                {
                    linkedUser.isloggedin = true;

                    GlobusLogHelper.log.Info("Logged In With Account : " + linkedUser.username);
                    try
                    {
                       // string Search = HttpHelper.getHtmlfromUrlProxy(new Uri("https://www.linkedin.com/search?trk=advsrch"),"", linkedUser.proxyip, linkedUser.proxyport, linkedUser.proxyusername, linkedUser.proxypassword);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                else if (ResLogin.Contains("logout?session_full_logout"))
                {
                    string Search = HttpHelper.getHtmlfromUrlProxy(new Uri("https://www.linkedin.com/search?trk=advsrch"), "", linkedUser.proxyip, linkedUser.proxyport, linkedUser.proxyusername, linkedUser.proxypassword);
                }
                else if (ResLogin.Contains("Sign-In Verification"))
                {
                    GlobusLogHelper.log.Info(" Verification required : " + linkedUser.username );
                }
                else if (ResLogin.Contains("Your LinkedIn account has been temporarily restricted"))
                {
                    GlobusLogHelper.log.Info("Your LinkedIn account : " + linkedUser.username + " has been temporarily restricted ");
                }
                else
                {
                    GlobusLogHelper.log.Info( "NotLogged In With Account : " + linkedUser.username );
                }

              
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("NotLogged In With Account : " + linkedUser.username );
            }
        }
    }
     
}
       

    

    
    

