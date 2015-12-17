using BaseLib;
using linkedDominator;
using Scraper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace LinkeddinDominator.CustomUserControls
{
    /// <summary>
    /// Interaction logic for UserControl_SalesNavigator_Input.xaml
    /// </summary>
    public partial class UserControl_SalesNavigator_Input : UserControl
    {
        public string country = "us:United States,af:Afghanistan,ax:Aland Islands,al:Albania,dz:Algeria,as:American Samoa,ad:Andorra,ao:Angola,ai:Anguilla,aq:Antarctica,ag:Antigua and Barbuda,ar:Argentina,am:Armenia,aw:Aruba,au:Australia,at:Austria,az:Azerbaijan,bs:Bahamas,bh:Bahrain,bd:Bangladesh,bb:Barbados,by:Belarus,be:Belgium,bz:Belize,bj:Benin,bm:Bermuda,bt:Bhutan,bo:Bolivia,ba:Bosnia and Herzegovina,bw:Botswana,br:Brazil,io:British Indian Ocean Territory,bn:Brunei Darussalam,bg:Bulgaria,bf:Burkina Faso,bi:Burundi,kh:Cambodia,cm:Cameroon,ca:Canada,cv:Cape Verde,cb:Caribbean Nations,ky:Cayman Islands,cf:Central African Republic,td:Chad,cl:Chile,cn:China,cx:Christmas Island,cc:Cocos (Keeling) Islands,co:Colombia,km:Comoros,cg:Congo,ck:Cook Islands,cr:Costa Rica,ci:Cote D'Ivoire (Ivory Coast),hr:Croatia,cu:Cuba,cy:Cyprus,cz:Czech Republic,cd:Democratic Republic of the Congo,dk:Denmark,dj:Djibouti,dm:Dominica,do:Dominican Republic,tp:East Timor,ec:Ecuador,eg:Egypt,sv:El Salvador,gq:Equatorial Guinea,er:Eritrea,ee:Estonia,et:Ethiopia,fk:Falkland Islands (Malvinas),fo:Faroe Islands,fm:Federated States of Micronesia,fj:Fiji,fi:Finland,fr:France,gf:French Guiana,pf:French Polynesia,tf:French Southern Territories,ga:Gabon,gm:Gambia,ge:Georgia,de:Germany,gh:Ghana,gi:Gibraltar,gr:Greece,gl:Greenland,gd:Grenada,gp:Guadeloupe,gu:Guam,gt:Guatemala,gg:Guernsey,gn:Guinea,gw:Guinea-Bissau,gy:Guyana,ht:Haiti,hn:Honduras,hk:Hong Kong,hu:Hungary,is:Iceland,in:India ,id:Indonesia,ir:Iran,iq:Iraq,ie:Ireland,im:Isle of Man,il:Israel,it:Italy,jm:Jamaica,jp:Japan,je:Jersey,jo:Jordan,kz:Kazakhstan,ke:Kenya,ki:Kiribati,kr:Korea,kp:Korea (North),kw:Kuwait,kg:Kyrgyzstan,la:Laos,lv:Latvia,lb:Lebanon,ls:Lesotho,lr:Liberia,ly:Libya,li:Liechtenstein,lt:Lithuania,lu:Luxembourg,mo:Macao,mk:Macedonia,mg:Madagascar,mw:Malawi,my:Malaysia,mv:Maldives,ml:Mali,mt:Malta,mh:Marshall Islands,mq:Martinique,mr:Mauritania,mu:Mauritius,yt:Mayotte,mx:Mexico,md:Moldova,mc:Monaco,mn:Mongolia,me:Montenegro,ms:Montserrat,ma:Morocco,mz:Mozambique,mm:Myanmar,na:Namibia,nr:Nauru,np:Nepal,nl:Netherlands,an:Netherlands Antilles,nc:New Caledonia,nz:New Zealand,ni:Nicaragua,ne:Niger,ng:Nigeria,nu:Niue,nf:Norfolk Island,mp:Northern Mariana Islands,no:Norway,om:Oman,pk:Pakistan,pw:Palau,ps:Palestinian Territory,pa:Panama,pg:Papua New Guinea,py:Paraguay,pe:Peru,ph:Philippines,pn:Pitcairn,pl:Poland,pt:Portugal,pr:Puerto Rico,qa:Qatar,re:Reunion,ro:Romania,ru:Russian Federation,rw:Rwanda,sh:Saint Helena,kn:Saint Kitts and Nevis,lc:Saint Lucia,pm:Saint Pierre and Miquelon,vc:Saint Vincent and the Grenadines,ws:Samoa,sm:San Marino,st:Sao Tome and Principe,sa:Saudi Arabia,sn:Senegal,rs:Serbia,sc:Seychelles,sl:Sierra Leone,sg:Singapore,sk:Slovak Republic,si:Slovenia,sb:Solomon Islands,so:Somalia,za:South Africa,es:Spain,lk:Sri Lanka,sd:Sudan,sr:Suriname,sj:Svalbard and Jan Mayen,sz:Swaziland,se:Sweden,ch:Switzerland,,sy:Syria,tw:Taiwan,tj:Tajikistan,tz:Tanzania,th:Thailand,tl:Timor-Leste,tg:Togo,tk:Tokelau,to:Tonga,tt:Trinidad and Tobago,tn:Tunisia,tr:Turkey,tm:Turkmenistan,tc:Turks and Caicos Islands,tv:Tuvalu,ug:Uganda,ua:Ukraine,ae:United Arab Emirates,gb:United Kingdom,uy:Uruguay,uz:Uzbekistan,vu:Vanuatu,va:Vatican City State (Holy See),ve:Venezuela,vn:Vietnam,vg:Virgin Islands (British),vi:Virgin Islands (U.S.),wf:Wallis and Futuna,eh:Western Sahara,ye:Yemen,zm:Zambia,zw:Zimbabwe,oo:Other";
        public UserControl_SalesNavigator_Input()
        {
            InitializeComponent();
            bindMethod();
        }
        
        Dictionary<string, string> CountryCode = new Dictionary<string, string>();

        public void bindMethod()
        {
              MainWindow objMainWindow = new MainWindow();
            string WithingList = objMainWindow.WithingList;

            foreach (string item in LDGlobals.listAccounts)
            {
                string userName = item.Split(':')[0];
                cmb_LIScraperInput_selectedAcc.Items.Add(userName);

            }


            Dictionary<string, string> CountryCode = new Dictionary<string, string>();
            ClsSelect ObjSelectMethod = new ClsSelect();
            CountryCode = ObjSelectMethod.getCountry();
            foreach (KeyValuePair<string, string> pair in CountryCode)
            {
                try
                {
                    cmb_SaleNavigtor_Country_Code.Items.Add(pair.Value);
                   // CombScraperCountry.Items.Add(pair.Value);


                  //  lstCountry.Add(pair.Value);
                   // CampainGroupCreate.Campaign_lstCountry.Add(pair.Value);
                    
                }
                catch
                {
                }
            }
            
            // location
            try
            {
                //cmb_SaleNavigtor_Location.Items.Add("Located in or near");
                //cmb_SaleNavigtor_Location.Items.Add("Anywhere");

            }
            catch
            { }

            // within
            string[] arraywithin = Regex.Split(WithingList, ",");
            foreach (string item in arraywithin)
            {
                string[] arrayPostalwithin = Regex.Split(item, ":");
                if (arrayPostalwithin.Length == 2)
                {
                    cmb_SaleNavigtor_Within.Items.Add(arrayPostalwithin[1]);
                }
            }

            // title curernt or past
            string TitleValue = objMainWindow.TitleValue;
            string[] arrayTitleValue = Regex.Split(TitleValue, ",");
           // if (!Globals.ComboBoxCurrent_Past_Task_Done)
           // {
                foreach (string item in arrayTitleValue)
                {
                    string[] arraytitleValue = Regex.Split(item, ":");
                    if (arraytitleValue.Length == 2)
                    {
                       // cmb_SaleNavigtor_Company_CrrentOrPast.Items.Add(arraytitleValue[1]);
                       // cmbboxCompanyValue.Items.Add(arraytitleValue[1]);
                    }
                }
          //  }

        }

        private void btn_IndustryRelationship_Save_Click(object sender, RoutedEventArgs e)
        {
            #region ankit sir
            //try
            //{
            //    if(!string.IsNullOrEmpty(cmb_LIScraperInput_selectedAcc.SelectedItem.ToString()))
            //    {
            //        GlobalsScraper.selectedEmail_Input_SalesNav = cmb_LIScraperInput_selectedAcc.SelectedItem.ToString();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Please Select Account.");
            //    }

            //    if(!string.IsNullOrEmpty(txt_SalesNavigator_FirstName.Text))
            //    {
            //        GlobalsScraper.firstName_Input_SalesNav = txt_SalesNavigator_FirstName.Text;
            //    }
            //    if(!string.IsNullOrEmpty(txt_SaleNavigtor_LastName.Text))
            //    {
            //        GlobalsScraper.lastName_Input_SalesNav = txt_SaleNavigtor_LastName.Text;
            //    }

            //    if(!string.IsNullOrEmpty(cmb_SaleNavigtor_Location.SelectedItem.ToString()))
            //    {
            //        GlobalsScraper.selectedLocation_Input_SalesNav = cmb_SaleNavigtor_Location.SelectedItem.ToString();
            //    }
            //    if(!string.IsNullOrEmpty(cmb_SaleNavigtor_Country_Code.SelectedItem.ToString()))
            //    {
            //        GlobalsScraper.selectedCountry_Input_SalesNav = cmb_SaleNavigtor_Country_Code.SelectedItem.ToString();
            //    }

            //    if(!string.IsNullOrEmpty(txt_SaleNavigtor_PostalCode.Text))
            //    {
            //        GlobalsScraper.postalCode_Input_SalesNav = txt_SaleNavigtor_PostalCode.Text;
            //    }
            //    if(cmb_SaleNavigtor_Within.SelectedItem !=null)
            //    {
            //        GlobalsScraper.selectedWithin_Input_SalesNav = cmb_SaleNavigtor_Within.SelectedItem.ToString();
            //    }

            //    if(!string.IsNullOrEmpty(txt_SaleNavigtor_Company.Text))
            //    {
            //        GlobalsScraper.company_Input_SalesNav = txt_SaleNavigtor_Company.Text;
            //    }
            //    if(!string.IsNullOrEmpty(cmb_SaleNavigtor_Company_CrrentOrPast.SelectedItem.ToString()))
            //    {
            //        GlobalsScraper.selectedCurrentPast_Input_SalesNav = cmb_SaleNavigtor_Company_CrrentOrPast.SelectedItem.ToString();
            //    }
            //}
            //catch (Exception ex)
            //{
            //}
            #endregion

           // Dictionary<string, string> CountryCode = new Dictionary<string, string>();
            try
            {
                #region CountrySelected
                CountryCode = getCountry();
                foreach (KeyValuePair<string, string> item in CountryCode)
                {
                    try
                    {
                        if (item.Value == cmb_SaleNavigtor_Country_Code.SelectedItem.ToString())
                        {
                            SalesNavigator.country = item.Key;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                #endregion

                SalesNavigator.firstName = txt_SalesNavigator_FirstName.Text;
                SalesNavigator.lastName = txt_SaleNavigtor_LastName.Text;
                SalesNavigator.selectedEmailId = cmb_LIScraperInput_selectedAcc.SelectedItem.ToString();
                SalesNavigator.location = txt_SaleNavigtor_Location.Text;
                SalesNavigator.postalCode = txt_SaleNavigtor_PostalCode.Text;
                SalesNavigator.within = cmb_SaleNavigtor_Within.SelectedItem.ToString();
                SalesNavigator.currentCompany = txt_SaleNavigtor_Company.Text;

                
            }
            catch(Exception ex)
            {
            }

        }

        #region getCountry()
        public Dictionary<string, string> getCountry()
        {
            Dictionary<string, string> CountryCode = new Dictionary<string, string>();
            List<string> lststate = new List<string>();
            lststate = spliter(country);

            foreach (string Country in lststate)
            {
                try
                {
                    string[] array = Regex.Split(Country, ":");
                    CountryCode.Add(array[0], array[1]);
                }
                catch (Exception ex)
                {

                }
            }

            return CountryCode;
        }
        #endregion

        #region List<string> spliter(string data)
        public List<string> spliter(string data)
        {
            List<string> list = new List<string>();
            string[] statedata = data.Split(',');
            foreach (string item in statedata)
            {
                list.Add(item);
            }
            return list;
        }
        #endregion

       
           
        
    }
}
