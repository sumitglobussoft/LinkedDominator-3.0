using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseLib;
using System.Text.RegularExpressions;

namespace ManageConnections
{
    public class WithdrawConnectionInvitation
    {
        public Dictionary<string, string> getAllMembers(ref GlobusHttpHelper HttpHelper, string userName)
        {
            Dictionary<string, string> details = new Dictionary<string, string>();
            try
            {
                string url = "https://www.linkedin.com/people/invites?trk=connect_hub_manage_invitations_sent";
                string src = HttpHelper.getHtmlfromUrl(new Uri(url));
                if (src.Contains("{\"lastName\""))
                {
                    string[] arr = Regex.Split(src, "{\"lastName\"");
                    foreach (string item in arr)
                    {
                        try
                        {
                            if (!item.Contains("<!DOCTYPE"))
                            {
                                string invitationId=string.Empty;
                                string Id=string.Empty;
                                string fullName = Utils.getBetween(item, "i18n_check_to_remove\":\"","\",");
                                fullName = Utils.getBetween(fullName + "###", "to remove", "###");
                                fullName= fullName.Trim();
                                if (item.Contains(""))
                                {
                                    invitationId = Utils.getBetween(item, "\"invitationId\":\"", "\",\"");
                                }
                                if (item.Contains("memberId\":"))
                                {
                                    Id = Utils.getBetween(item, "memberId\":", "}");
                                }
                                Id = userName + ":" + Id;
                                details.Add(Id, fullName + ":" + invitationId);

                            }
                        }
                        catch
                        { }
                    }

                }
            }
            catch
            { }
            return details;
        }

        public void withDrawConnection(ref GlobusHttpHelper HttpHelper,Dictionary<string, string> SlectedContacts, string UserName)
        {
            try
            {
                string csrfToken = string.Empty;
                string sourceAlias = string.Empty;

                string referer = "https://www.linkedin.com/people/invites?trk=connect_hub_manage_invitations_sent";
                string actionUrl = "https://www.linkedin.com/people/invites/withdraw?isInvite=true";
                string postData = string.Empty;

                string url="https://www.linkedin.com/people/invites?trk=connect_hub_manage_invitations_sent";
                string src = HttpHelper.getHtmlfromUrl(new Uri( url));
                if (src.Contains("csrfToken="))
                {
                    try
                    {
                        csrfToken = Utils.getBetween(src, "csrfToken=", "\"");
                        if (csrfToken.Contains("%3A"))
                        {
                            csrfToken = csrfToken.Replace("%3A", ":");
                        }
                    }
                    catch
                    { }
                }
                if (src.Contains("sourceAlias"))
                {
                    try
                    {
                        sourceAlias = Utils.getBetween(src, "sourceAlias", "}");
                        sourceAlias = Utils.getBetween(sourceAlias, "value\":\"", "\"");
                        if (sourceAlias.Contains("\\u002d"))
                        {
                            sourceAlias = sourceAlias.Replace("\u002d", "-");
                        }
                    }
                    catch
                    { }
                }

                foreach (KeyValuePair<string, string> item in SlectedContacts)
                {
                    string invitationId = item.Value.Split(':')[1];
                    postData = "csrfToken=" + csrfToken + "&sourceAlias=" + sourceAlias + "&Ids=" + invitationId;
                    string responce = HttpHelper.postDataFormessagePosting(new Uri( actionUrl), postData, referer);
                    if (responce.Contains("status\":\"ok\""))
                    {
                        //AddLoggerManageConnection("");

                    }


                }




            }
            catch
            { }

        }
    }
}
