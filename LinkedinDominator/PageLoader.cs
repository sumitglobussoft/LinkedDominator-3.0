using FirstFloor.ModernUI.Windows;
using LinkeddinDominator.Pages;
using LinkeddinDominator.Pages.PagesAddConnection;
using LinkeddinDominator.Pages.PagesGroup;
using LinkeddinDominator.Pages.PagesMessage;
using LinkeddinDominator.Pages.PagesScraper;
using LinkeddinDominator.Pages.PagesSearch;
using LinkeddinDominator.Pages.PagesSettings;
using LinkedinDominator.Pages.PageAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace LinkeddinDominator
{
    /// <summary>
    /// Loads lorem ipsum content regardless the given uri.
    /// </summary>
    public class PageLoader : DefaultContentLoader
    {
        /// <summary>
        /// Loads the content from specified uri.
        /// </summary>
        /// <param name="uri">The content uri</param>
        /// <returns>The loaded content.</returns>
        protected override object LoadContent(Uri uri)
        {
            // return a new LoremIpsum user control instance no matter the uri

            //Accounts Module 

            if (uri.ToString() == "/Manage Accounts")
            {
                return new Manage_Accounts();
            }
            if (uri.ToString() == "/UserControlsAddConnection.xaml")
            {
                return new UserControlsAddConnection();
            }
            if (uri.ToString() == "/UserControlsAddConnection.xaml")
            {
                return new UserControlsAddConnection();
            }
            if (uri.ToString() == "UserControlsGroupStatusUpdate.xaml")
            {
                return new UserControlsGroupStatusUpdate();
            }
            if (uri.ToString() == "UserControlsJoinGroupUsingURL.xaml")
            {
                return new UserControlsGroupStatusUpdate();
            }
            if (uri.ToString() == "UserControlsInviteMembersToGroup.xaml")
             {
                 return new UserControlsInviteMembersToGroup();
             }
            if (uri.ToString() == "UserControlsRemoveGroups.xaml")
             {
                 return new UserControlsRemoveGroups();
             }

            if (uri.ToString() == "UserControlsComposeMessage.xaml")
            {
                return new UserControlsComposeMessage();
            }
            if (uri.ToString() == "UserControlsMessageGroupMember.xaml")
            {
                return new UserControlsMessageGroupMember();
            }

            if (uri.ToString() == "UserControlsLinkedinScraper.xaml")
            {
                return new UserControlsLinkedinScraper();
            }
            if (uri.ToString() == "UserControlsCompanyEmployeeScraper.xaml")
            {
                return new UserControlsCompanyEmployeeScraper();
            }
            if (uri.ToString() == "UserControlsJobScraper.xaml")
            {
                return new UserControlsJobScraper();
            }
            if (uri.ToString() == "UserControlsSalsesNavigator.xaml")
            {
                return new UserControlsSalsesNavigator();
            }

            if (uri.ToString() == "UserControlsLinkedinSearch.xaml")
            {
                return new UserControlsLinkedinSearch();
            }

            if (uri.ToString() == "ProxyManager.xaml")
            {
                return new ProxyManager();
            }

            if (uri.ToString() == "UserControlsDBCSettings.xaml")
            {
                return new UserControlsDBCSettings();
            }
            if (uri.ToString() == "Appearance.xaml")
            {
                return new Appearance();
            }

         

            return "";
        }
        
    }
}
