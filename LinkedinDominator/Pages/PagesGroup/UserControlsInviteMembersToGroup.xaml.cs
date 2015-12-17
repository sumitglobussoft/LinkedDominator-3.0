using linkedDominator;
using System;
using System.Collections.Generic;
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

namespace LinkeddinDominator.Pages.PagesGroup
{
    /// <summary>
    /// Interaction logic for UserControlsInviteMembersToGroup.xaml
    /// </summary>
    public partial class UserControlsInviteMembersToGroup : UserControl
    {
        public UserControlsInviteMembersToGroup()
        {
            InitializeComponent();
            bindMethod();
        }
        public void bindMethod()
        {
            foreach(string item in LDGlobals.listAccounts)
            {
                string userName=item.Split(':')[0];
                cmb_InviteMembersToGroup_selectedAcc.Items.Add(userName);

            }
            
        }
    }
}
