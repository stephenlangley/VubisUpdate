using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;

namespace VubisUpdate
{
    class Program
    {

      static string Reverse(string strValue)
    {
        // this is the Termination of the Recursion when the string
        // length is 1 means on char
        if (strValue.Length == 1)
        {
            return strValue;
        }
        else
        {
            //print the or store the 1st char every time,
            //so that when 1st Char stored will be the end of the string
            //this way the string will be reversed
            return Reverse(strValue.Substring(1)) + strValue.Substring(0, 1);
        }
    }

    static string MapLocation(string location)
    {
    switch (location.Trim().ToLower())
    {
        case "rls" : return "LEA"; break;
        case "mm" : return "MOR"; break;
        case "tp" : return "TRI"; break;
        case "hia" : return "LEA"; break;
        case "rug" : return "RUG"; break;
        case "per" : return "PER"; break;
        case "mal": return "MAL"; break;
        case "eve": return "EVE"; break;
        case "leamington spa": return "LEA"; break;
        case "moreton morrell": return "MOR"; break;
        case "trident": return "TRI"; break;
        case "henley in arden": return "LEA"; break;
        case "rugby": return "RUG"; break;
        case "malvern": return "MAL"; break;
        case "evesham": return "EVE"; break;
        case "pershore": return "PER"; break;
        default: return "LEA"; break;
    }

    
    }

    static string MapStaffBarcodeChar(string staffID)
    {
        int chrIndex = 0;
        string chkChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string sid = staffID.ToString();
        int i = sid.Length;
        for (int x = 0; x < i; x++)
        {
            chrIndex += Convert.ToInt16(sid.Substring(x,1));
        }

        return chkChars.Substring(chrIndex, 1);
        //int c = chrIndex % 43;// mod the result by 43

    }

    static string getVubisDetails_SoapMessage(DataRowView drv)
        //create the soap message for the "details" messagetypeID which will create/update a Vubis account
        // this function will format the student data as specified in the Vubis documentation for this type fo soap message
    {
        string smartcardID, title, forename, surname, dob, ethnicity, gender, housenumber, flat, street, locality, town, postcode, district;
        string hometel, worktel, mobtel, email, homeemail, password, editdate, libraryid, homebranchid, category;
        string housenumber2, street2, locality2, town2, postcode2, district2, hometel2;
        string department, contact, membershipexpiry, optional1, optional3, optional5, preference4;
        string citizenID, issuer, authorisingID, messageTypeID, servApp, photo;
        DateTime dBirthDate;
        string Borrower;
        string sBirthDate = "";
        DateTime dExpirydate;
        string sExpirydate = "";
        char quote;
        quote = '"';
        // map GENDER 0 = unknown, 2 = female, 1 = male
        string sSex = "0";
        if (drv["gender"].ToString().Trim().ToUpper() == "M" || drv["gender"].ToString().Trim().ToUpper() == "MALE") { sSex = "1"; }
        if (drv["gender"].ToString().Trim().ToUpper() == "F" || drv["gender"].ToString().Trim().ToUpper() == "FEMALE") { sSex = "2"; } 

        // map DOB YYYY-MM-DD HH:MM:SS
        sBirthDate = "1900/01/01 00:00:00";

        if (DateTime.TryParse(drv["birth_dt"].ToString().Trim(), out dBirthDate))
        {
            sBirthDate = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Parse(drv["birth_dt"].ToString()));
            //sBirthDate = Convert.ToDateTime(drv["birth_dt"].ToString().Trim()).ToString("u").Replace("Z", "");
        }

        if (DateTime.TryParse(drv["VubisEndDate"].ToString().Trim(), out dExpirydate))
        {
            sExpirydate = String.Format("{0:yyyy-MM-dd}", DateTime.Parse(drv["VubisEndDate"].ToString()));
        }
    
        //note for staff the employeenumber is set to student_id in the stored procedure
        
        string UniqueID = drv["unique_id"].ToString().Trim();
        if (UniqueID.Length < 6)
        {
            if (UniqueID.Length < 5)
            {
                UniqueID = "0" + UniqueID + MapStaffBarcodeChar(UniqueID);
            }
            else
            {
                UniqueID = UniqueID + MapStaffBarcodeChar(UniqueID);
            }
        }
        string sCitizenID = drv["citizenID"].ToString().Trim();
        string sSmartCardID = drv["smartcardID"].ToString().Trim();
        string sLibraryID = drv["libraryID"].ToString().Trim();
        string sTitle = drv["title"].ToString().Trim();
        string sForename = drv["forename"].ToString().Trim();
        string sSurname = drv["surname"].ToString().Trim();
        string sDOB = sBirthDate;
        string sEthnicity = "";
        string sGender = sSex;
        string sHouseNumber = "";
        string sFlat = "";
        string sStreet = drv["add_1"].ToString().Trim().Replace("&"," &amp; "); ///main address line 1
        string sLocality = drv["add_2"].ToString().Trim().Replace("&", " &amp; "); // line 2
        string sTown = drv["add_3"].ToString().Trim().Replace("&", " &amp; "); // postal town
        string sPostCode = drv["post_code"].ToString().Trim(); // post code
        string sDistrict = drv["add_4"].ToString().Trim().Replace("&", " &amp; "); // county
        string sHomeTel = drv["tele_no"].ToString().Trim().Replace("&", " &amp; "); 
        string sWorkTel = "";
        string sMobTel = drv["mobile_phone_no"].ToString().Trim().Replace("&", " &amp; "); 
        string s2HouseNumber = "";
        string s2Flat = "";
        string s2Street = drv["_add_1"].ToString().Trim().Replace("&"," &amp; "); //Second address line 1
        string s2Locality = drv["_add_2"].ToString().Trim().Replace("&"," &amp; "); // line 2
        string s2Town = drv["_add_3"].ToString().Trim().Replace("&"," &amp; "); // postal town
        string s2PostCode = drv["_post_code"].ToString().Trim(); // post code
        string s2District = drv["_add_4"].ToString().Trim().Replace("&"," &amp; "); // county
        string s2HomeTel = drv["_tele_no"].ToString().Trim().Replace("&", " &amp; ");
        string s2WorkTel = "";
        string s2MobTel = drv["mobile_phone_no"].ToString().Trim().Replace("&", " &amp; ");
        string sEmail = drv["email_id"].ToString().Trim() + "@student.warwickshire.ac.uk";
        string sPassword = "";
        string sEditDate = DateTime.Now.ToString("u").Replace("Z", "");
//        string sLibraryID = drv["student_id"].ToString().Trim() + drv["seqchar"].ToString().Trim();
        string sHomeBranchID = MapLocation(drv["location"].ToString().Trim());
        string sCategory = drv["sCategory"].ToString().Trim();
        string sAcYear = drv["AcYear"].ToString().Trim();
        string sCourse = drv["course_title"].ToString().Trim().Replace("&"," &amp; ");
        string sDept = drv["dept"].ToString().Trim().Replace("&", " &amp; ").Replace("+", " and ");
        string photo_id = drv["photo_id"].ToString().Trim();
        string sPhoto = "http://idphoto.warkscol.ac.uk/qlreport/photographs/" + photo_id + ".jpg";
        // test environment       string sPhoto = "http://tango/vstest/images/student/" + stuID + ".bmp";
        //string sExpirydate = drv["aos_end_dt"].ToString().Trim();
        string sTutor = drv["tutor_name"].ToString().Trim();
        string sHomeEmail = drv["ext_email"].ToString().Trim().Replace("&", " &amp; ");
        string sStuCat = "HE";
        int sPreference = 0;

        citizenID = "<Citizen id=\"" + sCitizenID + "\"";
        issuer = " issuer=" + quote + "Warwickshire College" + quote;
        authorisingID = " authorisingId=" + quote + "infor" + quote;
//original        messageTypeID = " messageTypeId=" + quote + "details" + quote + " smartcardId=" + quote + sLibraryID + quote + ">";
        messageTypeID = " messageTypeId=" + quote + "details" + quote + " smartcardId=" + quote + sSmartCardID + quote + ">";

        servApp = "<Service application=\"" + "ISO File Handler" + "\" refinement=" + quote + "ISO File Handler" + quote + "/>";
        servApp = servApp + "<Service application=" + quote + "CCDA" + quote + " refinement=" + quote + "CCDA" + quote + ">";

//original        smartcardID = "<Item name=" + quote + "SMARTCARDID" + quote + ">" + sLibraryID + "</Item>";
        smartcardID = "<Item name=" + quote + "SMARTCARDID" + quote + ">" + sSmartCardID + "</Item>";
        title = "<Item name=" + quote + "TITLE" + quote + ">" + sTitle + "</Item>";
        forename = "<Item name=" + quote + "FORENAME" + quote + ">" + sForename + "</Item>";
        surname = "<Item name=" + quote + "SURNAME" + quote + ">" + sSurname + "</Item>";
        dob = "<Item name=" + quote + "DOB" + quote + ">" + sDOB + "</Item>";
        ethnicity = "<Item name=" + quote + "ETHNICITY" + quote + ">" + sEthnicity + "</Item>";
        gender = "<Item name=" + quote + "GENDER" + quote + ">" + sGender + "</Item>";
        housenumber = "<Item name=" + quote + "HOUSE NUMBER/NAME" + quote + ">" + sHouseNumber + "</Item>";
        flat = "<Item name=" + quote + "FLAT" + quote + ">" + sFlat + "</Item>";
        street = "<Item name=" + quote + "STREET" + quote + ">" + sStreet + "</Item>";
        locality = "<Item name=" + quote + "LOCALITY" + quote + ">" + sLocality + "</Item>";
        town = "<Item name=" + quote + "TOWN" + quote + ">" + sTown + "</Item>";
        postcode = "<Item name=" + quote + "POSTCODE" + quote + ">" + sPostCode + "</Item>";
        district = "<Item name=" + quote + "DISTRICT" + quote + ">" + sDistrict + "</Item>";
        hometel = "<Item name=" + quote + "HOME TEL" + quote + ">" + sHomeTel + "</Item>";
        worktel = "<Item name=" + quote + "WORK TEL" + quote + ">" + sWorkTel + "</Item>";
        mobtel = "<Item name=" + quote + "MOB TEL" + quote + ">" + sMobTel + "</Item>";
        housenumber2 = "<Item name=" + quote + "HOUSE NUMBER/NAME2" + quote + ">" + s2HouseNumber + "</Item>";
        street2 = "<Item name=" + quote + "STREET2" + quote + ">" + s2Street + "</Item>";
        locality2 = "<Item name=" + quote + "LOCALITY2" + quote + ">" + s2Locality + "</Item>";
        town2 = "<Item name=" + quote + "TOWN2" + quote + ">" + s2Town + "</Item>";
        postcode2 = "<Item name=" + quote + "POSTCODE2" + quote + ">" + s2PostCode + "</Item>";
        district2 = "<Item name=" + quote + "DISTRICT2" + quote + ">" + s2District + "</Item>";
        hometel2 = "<Item name=" + quote + "HOME TEL2" + quote + ">" + s2HomeTel + "</Item>";

        homeemail = "<Item name=" + quote + "EMAIL2" + quote + ">" + sHomeEmail + "</Item>";
        email = "<Item name=" + quote + "EMAIL" + quote + ">" + sEmail + "</Item>";
        photo = "<Item name=" + quote + "PHOTO" + quote + ">" + sPhoto + "</Item>";
        department = "<Item name=" + quote + "DEPARTMENT" + quote + ">" + sDept + "</Item>";
        contact = "<Item name=" + quote + "CONTACT" + quote + ">" + sTutor + "</Item>";
        optional1 = "<Item name=" + quote + "OPTIONAL1" + quote + ">" + sCourse + "</Item>";
       // test before making live // optional3 = "<Item name=" + quote + "OPTIONAL3" + quote + ">" + sStuCat + "</Item>";
        optional5 = "<Item name=" + quote + "OPTIONAL5" + quote + ">" + sAcYear + "</Item>";
        membershipexpiry = "<Item name=" + quote + "MEMBERSHIPEXPIRY" + quote + ">" + sExpirydate + "</Item>";
        password = "<Item name=" + quote + "PASSWORD" + quote + ">" + sPassword + "</Item>";
        editdate = "<Item name=" + quote + "EDIT DATE" + quote + ">" + sEditDate + "</Item>";
        libraryid = "<Item name=" + quote + "LIBRARYID" + quote + ">" + "WCOL/" + sLibraryID + "</Item>";
        homebranchid = "<Item name=" + quote + "HOMEBRANCHID" + quote + ">" + sHomeBranchID + "</Item>";
        category = "<Item name=" + quote + "CATEGORY" + quote + ">" + sCategory + "</Item>";
        // test before making live //preference4 = "<Item name=" + quote + "LOAN HISTORY" + quote + ">" + sPreference + "</Item>";// PREFERENCE4 ?

        Borrower = citizenID + issuer + authorisingID + messageTypeID;
        Borrower = Borrower + "<Services>";
        Borrower = Borrower + servApp + smartcardID + title + forename + surname + dob + ethnicity + gender + housenumber + flat + street;
        Borrower = Borrower + locality + town + postcode + district + hometel + worktel + mobtel + email + homeemail + password + editdate + libraryid;
        Borrower = Borrower + department + contact + membershipexpiry + optional1 + optional5 + district2;
        Borrower = Borrower + housenumber2 + street2 + locality2 + town2 + postcode2 + hometel2 + photo;
        Borrower = Borrower + "</Service>";
        Borrower = Borrower + "<Service application=" + quote + "Library Membership" + quote + " refinement=" + quote + "Standard" + quote + ">";
        Borrower = Borrower + homebranchid + category;
        Borrower = Borrower + "</Service>" + "</Services>" + "</Citizen>";
        return Borrower;
    
    }

    static string getVubisBlockAccount_SoapMessage(DataRowView drv, string sReason)
        //Create the soap message for a "block" messageTypeID which is 4
    // this function will format the student data as specified in the Vubis documentation for this type of soap message

    {
        string citizenID, issuer, authorisingID, messageTypeID, smartcardID;
        string statuschanged, libraryid, cardvalid, reason, Borrower;

        string UniqueID = drv["unique_id"].ToString().Trim();
        if (UniqueID.Length < 6)
        {
            if (UniqueID.Length < 5)
            {
                UniqueID = "0" + UniqueID + MapStaffBarcodeChar(UniqueID);
            }
            else
            {
                UniqueID = UniqueID + MapStaffBarcodeChar(UniqueID);
            }
        }
        //string sLibraryID = drv["exp_student_id"].ToString().Trim() + drv["seqchar"].ToString().Trim();

        string sCitizenID = drv["citizenID"].ToString().Trim();
        string sSmartCardID = drv["smartcardID"].ToString().Trim();
        string sLibraryID = drv["libraryID"].ToString().Trim();
        
        string sChangeddate = DateTime.Now.ToString("u").Replace("Z", "");
        DateTime dWithdrawndate;
        string sWithdrawndate = "null date";
        char quote;
        quote = '"';

        if (DateTime.TryParse(drv["VubisEndDate"].ToString().Trim(), out dWithdrawndate))
        {
            sWithdrawndate = String.Format("{0:dd-MM-yyyy}", DateTime.Parse(drv["VubisEndDate"].ToString()));
        }


        citizenID = "<Citizen id=\"" + sCitizenID + "\"";
        issuer = " issuer=" + quote + "Warwickshire College" + quote;
        authorisingID = " authorisingId=" + quote + "infor" + quote;
        messageTypeID = " messageTypeId=" + quote + "4" + quote + ">";
        smartcardID = "<Item name=" + quote + "SMARTCARDID" + quote + ">" + sSmartCardID + "</Item>";
        statuschanged = "<Item name=" + quote + "STATUS CHANGED" + quote + ">" + sChangeddate + "</Item>";
        libraryid = "<Item name=" + quote + "LIBRARYID" + quote + ">" + "WCOL/" + sLibraryID + "</Item>";
        cardvalid = "<Item name=" + quote + "CARD VALID" + quote + ">" + "FALSE" + "</Item>";
        reason = "<Item name=" + quote + "REASON" + quote + ">" + sReason + " ( " + sWithdrawndate + " )" + "</Item>";

        Borrower = citizenID + issuer + authorisingID + messageTypeID;
        Borrower = Borrower + "<Services>";
        Borrower = Borrower + "<Service application=" + quote + "Card Change" + quote + " refinement=" + quote + "Standard" + quote + ">";
        Borrower = Borrower + smartcardID + statuschanged + libraryid + cardvalid + reason;
        Borrower = Borrower + "</Service>" + "</Services>" + "</Citizen>";
        return Borrower;
    }
    static string getVubisRemoveBlockAccount_SoapMessage(DataRowView drv, string sReason)
        //Create the soap message for the "unblock" messageTypeID which is 5
    // this function will format the student data as specified in the Vubis documentation for this type of soap message

    {
        string citizenID, issuer, authorisingID, messageTypeID, smartcardID;
        string statuschanged, libraryid, cardvalid, reason, Borrower;

        string UniqueID = drv["unique_id"].ToString().Trim();
        if (UniqueID.Length < 6)
        {
            if (UniqueID.Length < 5)
            {
                UniqueID = "0" + UniqueID + MapStaffBarcodeChar(UniqueID);
            }
            else
            {
                UniqueID = UniqueID + MapStaffBarcodeChar(UniqueID);
            }
        }
        //string sLibraryID = drv["exp_student_id"].ToString().Trim() + drv["seqchar"].ToString().Trim();
        string sCitizenID = drv["citizenID"].ToString().Trim();
        string sSmartCardID = drv["smartcardID"].ToString().Trim();
        string sLibraryID = drv["libraryID"].ToString().Trim();

        string sChangeddate = DateTime.Now.ToString("u").Replace("Z", "");
        DateTime dWithdrawndate;
        string sWithdrawndate = "null date";
        char quote;
        quote = '"';

        string sActivedate = String.Format("{0:dd-MM-yyyy HH:mm:ss}", DateTime.Parse(DateTime.Now.ToString()));


        citizenID = "<Citizen id=\"" + sCitizenID + "\"";
        issuer = " issuer=" + quote + "Warwickshire College" + quote;
        authorisingID = " authorisingId=" + quote + "infor" + quote;
        messageTypeID = " messageTypeId=" + quote + "5" + quote + ">";
        smartcardID = "<Item name=" + quote + "SMARTCARDID" + quote + ">" + sSmartCardID + "</Item>";
        statuschanged = "<Item name=" + quote + "STATUS CHANGED" + quote + ">" + sChangeddate + "</Item>";
        libraryid = "<Item name=" + quote + "LIBRARYID" + quote + ">" + "WCOL/" + sLibraryID + "</Item>";
        cardvalid = "<Item name=" + quote + "CARD VALID" + quote + ">" + "TRUE" + "</Item>";
        reason = "<Item name=" + quote + "REASON" + quote + ">" + sReason + " " + sActivedate + "</Item>";

        Borrower = citizenID + issuer + authorisingID + messageTypeID;
        Borrower = Borrower + "<Services>";
        Borrower = Borrower + "<Service application=" + quote + "Card Change" + quote + " refinement=" + quote + "Standard" + quote + ">";
        Borrower = Borrower + smartcardID + statuschanged + libraryid + cardvalid + reason;
        Borrower = Borrower + "</Service>" + "</Services>" + "</Citizen>";
        return Borrower;
    }

    static string getVubisDelete_SoapMessage(DataRowView drv)
    {
        string citizenID, issuer, authorisingID, messageTypeID, smartcardID;
        string surname, libraryid, forename, Borrower;

        string stuID = drv["student_id"].ToString().Trim();
        //string sLibraryID = drv["exp_student_id"].ToString().Trim() + drv["seqchar"].ToString().Trim();
        char quote;
        quote = '"';
        string sCitizenID = drv["citizenID"].ToString().Trim();
        string sSmartCardID = drv["smartcardID"].ToString().Trim();
        string sLibraryID = drv["libraryID"].ToString().Trim();
        citizenID = "<Citizen id=\"" + sCitizenID + "\"";
        issuer = " issuer=" + quote + "Warwickshire College" + quote;
        authorisingID = " authorisingId=" + quote + "infor" + quote;
        messageTypeID = " messageTypeId=" + quote + "3" + quote + " smartcardId=" + quote + sSmartCardID + quote + ">";
        smartcardID = "<Item name=" + quote + "SMARTCARDID" + quote + ">" + "WCOL/" + stuID + "</Item>";
        surname = "<Item name=" + quote + "SURNAME" + quote + ">" + "Deleted" + "</Item>";
        libraryid = "<Item name=" + quote + "LIBRARYID" + quote + ">" + "WCOL/" + sLibraryID + "</Item>";
        forename = "<Item name=" + quote + "FORENAME" + quote + ">" + "Deleted" + "</Item>";

        Borrower = citizenID + issuer + authorisingID + messageTypeID;
        Borrower = Borrower + "<Services>";
        Borrower = Borrower + "<Service application=" + quote + "CCDA" + quote + " refinement=" + quote + "CCDA" + quote + ">";
        Borrower = Borrower + smartcardID + surname + forename + libraryid;
        Borrower = Borrower + "</Service>" + "</Services>" + "</Citizen>";
        return Borrower;
    }


        static void ProcessStudents()
    {
        string autolib = "1";
        string Borrower = "";
        string status = "PN";
        // Amended due to an upgrade by the VUBIS people  - cache 5 to cache 2008
        // AMENDED SJL 10/03/2011 vbsLive.Proces to vbsLive.SCProcess because cache 2008 uses Process as a reserved word.
        //vubis.BorrowerServices vbs = new vubis.BorrowerServices();
        
        //vubis.ProcessResponseProcessResult vbsRes = new vubis.ProcessResponseProcessResult();

        // create a borrower service variable vbsLive
        VubisLive.BorrowerServices vbsLive = new VubisLive.BorrowerServices();

        // Cretae a result variable vbsResLive
        VubisLive.SCProcessResponseSCProcessResult vbsResLive = new VubisLive.SCProcessResponseSCProcessResult();
     
        // Retrieve a list of student data from our Identity mangement system to be processed
        DataView dvStudent = readDataView("csWCIdentitiesDB", "pro_LibrarywcStudentPrimaryCourseForVubis '" + status + "'");
        //DataView dvStudent = readDataView("csWCIdentitiesDB", "pro_LibrarywcStudentPrimaryCourseForVubisTestSJL 'YUN94140639'");
        //DataView dvStaff = sdStaff.dataView("csWCIdentitiesDB", "pro_LibraryWcStaffForVubis ");
        
        if (dvStudent.Count > 0)
        {
            // Process each student in the list of students returned into dvStudent
            foreach (DataRowView drv in dvStudent)

            {
                status = drv["autolib"].ToString().Trim(); 
                string studentID = drv["citizenID"].ToString().Trim();
                // Create the Vubis soapmessage for a student
                Borrower = getVubisDetails_SoapMessage(drv); //pass in a borrowers data from QLS and return a vubis soap message
                try
                {
                    vbsResLive = vbsLive.SCProcess(Borrower); //Process the vubis soap message - create account if not exist
                }
                catch ( Exception  ue) 
                { 
                    String wM = ue.Message;
                    Console.WriteLine("vbsLive.Process Error: " + wM);
                }
                if (vbsResLive.Items != null)
                //if (false)
                {
                   
                    
                System.Xml.XmlElement[] res = vbsResLive.Items;
                string retMessage = res[0].InnerText.ToString();
                string outMessage = "DETAILS - " + retMessage.ToString();
                int outcome = Convert.ToInt16(retMessage.Substring(0, 1));
                //if (true)
                if (outcome == 0 )//|| retMessage == "1SMARTCARDID used as a previous card IDWCOL/")// Vubis account needs to be Re-instated by the library team before they will be processed. 
                                 // this means an existing account will be blocked or unblocked depending on  the field "aoutolib" status.
                {
                    // if student is not active then block the Vubis account
                    // by the withdrawn process
                    if (drv["autolib"].ToString().Trim() == "PW") // Student not active
                    {
                        Borrower = getVubisBlockAccount_SoapMessage(drv,"Withdrawn");
                        //vbsRes = vbs.Process(Borrower);
                        //res = vbsRes.Items;
                        vbsResLive = vbsLive.SCProcess(Borrower);
                        res = vbsResLive.Items;
                        retMessage = res[0].InnerText.ToString();
                        outcome = Convert.ToInt16(retMessage.Substring(0, 1));
                        outMessage += " - WITHDRAWN - " + retMessage.ToString();
                        if (outcome == 0) { status = "SW"; } 
                    }
                    // If student is active then unblock the Vubis account
                    if (drv["autolib"].ToString().Trim() == "PN") // this is an ACTIVE account
                    {
                        Borrower = getVubisRemoveBlockAccount_SoapMessage(drv,"Active");
                        vbsResLive = vbsLive.SCProcess(Borrower);
                        res = vbsResLive.Items;
                        retMessage = res[0].InnerText.ToString();
                        outcome = Convert.ToInt16(retMessage.Substring(0, 1));
                        outMessage += " - ACTIVE - " + retMessage.ToString();
                        if (outcome == 0) { status = "SN"; } 

                    }
                    if (drv["autolib"].ToString().Trim() == "PR") // PR is for replacement card has occurred
                    {
                        string issueNumber = drv["student_seq"].ToString();
                        Borrower = getVubisBlockAccount_SoapMessage(drv, "Replacement card - Issue No: " + issueNumber.ToString());
                        //vbsRes = vbs.Process(Borrower);
                        //res = vbsRes.Items;
                        vbsResLive = vbsLive.SCProcess(Borrower);
                        res = vbsResLive.Items;
                        retMessage = res[0].InnerText.ToString();
                        outcome = Convert.ToInt16(retMessage.Substring(0, 1));
                        outMessage += " - Replacement Card - " + retMessage.ToString();
                        if (outcome == 0) { status = "SR"; } // set autolib status = PP for success 
                        // update the student record with success or failure message and set the autolib status accordingly
                    }

                    if (drv["autolib"].ToString().Trim() == "SD") // need to change this to PD when LIVE
                    {
                        //Borrower = getVubisDelete_SoapMessage(drv);
                        //vbsResLive = vbsLive.Process(Borrower);
                        //res = vbsResLive.Items;
                        //vbsResLive = vbsLive.Process(Borrower);
                        //System.Xml.XmlElement[] res = vbsResLive.Items;
                        //retMessage = res[0].InnerText.ToString();
                        //outcome = Convert.ToInt16(retMessage.Substring(0, 1));
                        //outMessage += " - DELETE - " + retMessage.ToString();
                        //if (outcome == 0) { status = "PP"; } // set autolib status = PP for success 
                        // update the student record with success or failure message and set the autolib status accordingly
                    }

                }
                // Update our Student Identity management system with the new status
                // Remeber to change the stored procedure back to pro_UpdatewcStudentPrimaryCourseForVubis when going LIVE
                DataView dvUpd = readDataView("csWCIdentitiesDB", "pro_UpdatewcStudentPrimaryCourseForVubis '" + studentID + "', '" + status + "', '" + outMessage + "^" + DateTime.Now.ToString("u").Replace("Z", "") + "'");
                }
            }
        }        
    }
    

       static public DataView readDataView(String aCSName, String aSqlQuery)
        {
                ConnectionStringSettingsCollection connections =
                    ConfigurationManager.ConnectionStrings;
                DataTable dt = new DataTable();
                SqlConnection connUtility = new SqlConnection(
                    connections[aCSName].ConnectionString
                    );
                try
                {
                    SqlDataAdapter adp = new SqlDataAdapter();
                    SqlCommand cmd = new SqlCommand(aSqlQuery, connUtility);
                    adp.SelectCommand = cmd;
                    try { adp.Fill(dt); }
                    catch (Exception ue) { };
                }
                finally { connUtility.Close(); }
                return dt.DefaultView;
        }
   


        static void Main(string[] args)
        {

            //ProcessStaff();
            ProcessStudents();

        }
    }
}

