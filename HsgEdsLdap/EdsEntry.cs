using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.DirectoryServices.Protocols;

namespace HsgEdsLdap
{
    /// <summary>
    /// Abstract Class representing an entry from Enterprise Directory.
    /// </summary>
    [Serializable]
    public sealed class EdsEntry : IComparable<EdsEntry>, IEquatable<EdsEntry>
    {
        // Members
        #region Members

        /// <summary>
        /// The value of the Attrbutes property.
        /// </summary>
        private SortedList<string, string[]> _attributes = new SortedList<string,string[]>();

        #endregion

        // Constructors
        #region Constructors

        /// <summary>
        /// Default Constructor for EdsEntry
        /// </summary>
        public EdsEntry() 
        {

        }

        /// <summary>
        /// Creates a new EdsEntry using the information in the searchResultEntry, searchResult.
        /// </summary>
        /// <param name="searchEntry">The searchResultEntry to use when creating the EdsEntry.</param>
        public EdsEntry(SearchResultEntry searchEntry)
        {
            const char _splitCharacter = '|';

            if (searchEntry == null)
            {
                throw new ArgumentNullException("searchEntry", "searchEntry cannot be null.  You cannot use a null object to create an EdsEntry.");
            }
            else
            {
                // Process the Search Response result by pulling the list of attributes
                //  and their values.
                foreach (DirectoryAttribute _attribute in searchEntry.Attributes.Values)
                {
                    string _attributeName = _attribute.Name.ToString();
                    string[] _attributeValue = null;
                    StringBuilder _attributeValueBuilder = new StringBuilder();

                    // Grab each value for the current attribute.
                    for (int _loopCounter = 0; _loopCounter < searchEntry.Attributes[_attributeName].Count; _loopCounter++)
                    {
                        if (searchEntry.Attributes[_attributeName][_loopCounter] is string)
                        {
                            _attributeValueBuilder.Append(_splitCharacter);
                            _attributeValueBuilder.Append(searchEntry.Attributes[_attributeName][_loopCounter].ToString());
                        }
                    }

                    // Make sure we have at least one value and store the attribute in our Eds Entry
                    if (_attributeValueBuilder.Length > 1)
                    {
                        _attributeValueBuilder.Remove(0, 1);
                        string temp = _attributeValueBuilder.ToString();
                        _attributeValue = temp.Split(_splitCharacter);

                        AddAttribute(_attributeName, _attributeValue);
                    }
                }
            }
        }

        #endregion

        // Properties
        #region Properties

        /// <summary>
        /// Returns the number of Attributes this entry has
        /// </summary>
        private int AttributeCount
        {
            get { return _attributes.Count; }
        }

        /// <summary>
        /// Returns the list of attribute Names in a string[]
        /// </summary>
        private string[] AttributeNames
        {
            get
            {
                string[] result = { };
                if (_attributes.Count > 0)
                    result = _attributes.Keys.ToArray();

                return result;
            }
        }

        #region Property Attributes

        /// <summary>
        /// <para>Returns the "dateOfBirth" attribute of the EdsEntry.</para>
        /// <para>The "dateOfBirth" string in EDS is in the format yyyyMMdd (20110310).  This property will convert that into the DateTime value.</para>
        /// <para>If the "dateOfBirth" attribute is not found then returns default(DateTime);</para>
        /// </summary>
        /// <value>optional</value>
        public DateTime DateOfBirth
        {
            get
            {
                var name = "dateOfBirth";
                DateTime result = default(DateTime);

                var resultArray = GetAttribute(name);
                if (resultArray.Length > 0)
                {
                    CultureInfo cultureInfo = CultureInfo.CurrentCulture;
                    if (cultureInfo == null)
                        cultureInfo = new CultureInfo("en-US");

                    if (!DateTime.TryParseExact(resultArray[0], "yyyyMMdd", cultureInfo, DateTimeStyles.None, out result))
                        result = default(DateTime);
                }

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// returns "mail" attribute of EDS
        /// </summary>
        /// <see cref="Mail"/>
        public string Email { get; }

        /// <summary>
        /// <para>Returns the "emplId" attribute of the EdsEntry</para>
        /// <para>Prounounced "em-pull-eye-dee", this 8-digit number is the new primary identifier on campus with the advent of the Mosaic Human Resources and Student Administration PeopleSoft systems. The EMPLID is a common identifier that will be shared between the student and employee systems.</para>
        /// </summary>
        /// <value>optional</value>
        public string EmplId
        {
            get
            {
                var name = "emplId";
                var result = string.Empty;

                var resultArray = GetAttribute(name);
                if (resultArray.Length > 0)
                    result = resultArray[0];

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns the "employeeFTE" attribute of the EdsEntry.</para>        
        /// <para>FTE for the employee as a whole.  Note: this is more complex than just summing the FTE percentages from the incumbent positions.</para>
        /// </summary>
        /// <value>optional</value>
        public string EmployeeFTE
        {
            get
            {
                var name = "employeeFTE";
                var result = string.Empty;

                var resultArray = GetAttribute(name);
                if (resultArray.Length > 0)
                    result = resultArray[0];

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns the "employeeIncumbentPosition" attribute of the EdsEntry.</para>                
        /// <para>a colon (:) separated list with</para>
        /// <para>employee's title</para>
        /// <para>Position Control Number (PCN) from Mosaic HR (######)</para>
        /// <para>start date for the position (yyyyMMdd)</para>
        /// <para>Employee Type</para>
        /// <para>Employee Status</para>
        /// </summary>
        /// <value>optional</value>
        /// <seealso cref="EmployeeStatus"/>
        public string[] EmployeeIncumbentPosition
        {
            get
            {
                var name = "employeeIncumbentPosition";
                var result = GetAttribute(name);

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Checks "employeeIsFerpaTrained" to see if it has a value of "Y" in the list of values.</para>        
        /// <para>"Y" for employees who have had FERPA training and "N" for employees who have not.</para>
        /// </summary>
        /// <value>If no "Y" value is found, will always return false;  will only return true when "Y" value is present.</value>
        public bool EmployeeIsFerpaTrained
        {
            get
            {
                var result = false;

                var value = "Y";
                var name = "employeeIsFerpaTrained";
                var resultArray = GetAttribute(name);

                if ((resultArray.Length > 0) && (resultArray.Contains(value)))
                    result = true;

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns the "employeePrimaryDept" attribute of the EdsEntry.</para>        
        /// <para>Dept # of employee's primary department (also known as home department); refers to the department where an employee's paycheck is sent.</para>
        /// </summary>
        /// <value>optional</value>
        public string EmployeePrimaryDepartment
        {
            get
            {
                var name = "employeePrimaryDept";
                var result = string.Empty;

                var resultArray = GetAttribute(name);
                if (resultArray.Length > 0)
                    result = resultArray[0];

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns the "employeePrimaryDeptName" attribute of the EdsEntry.</para>
        /// <para>Textual description corresponding to employeePrimaryDept.</para>
        /// </summary>
        /// <value>optional</value>
        /// <seealso cref="EmployeePrimaryDepartment"/>
        public string EmployeePrimaryDepartmentName
        {
            get
            {
                var name = "employeePrimaryDeptName";
                var result = string.Empty;

                var resultArray = GetAttribute(name);
                if (resultArray.Length > 0)
                    result = resultArray[0];

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns "employeePositionFunding" attribute of the EdsEntry.</para>        
        /// <para>Colon (:) separated list containing</para>
        /// <para>Mosaic HR position control number (PCN)</para>
        /// <para>the funding department number</para>
        /// <para>For each position an employee occupies (see employeeIncumbentPosition) there will be at least one corresponding value in this attribute if the position is funded; non-funded positions will not appear in the value set of this attribute.  Positions funded by multiple departments will have multiple values in this attribute.</para>
        /// </summary>
        /// <value>optional</value>
        /// <seealso cref="EmployeeIncumbentPosition" />
        public string[] EmployeePositionFunding
        {
            get
            {
                var name = "employeePositionFunding";
                var result = GetAttribute(name);

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns "employeeRosterDept" attribute of the EdsEntry.</para>
        /// <para>Dept # of department to which an employee submits their timesheet. Each instance of this attribute will contain a PCN (position control number), and the Roster Department number for that position, separated by a colon (':').</para>
        /// </summary>
        /// <value>optional</value>
        public string[] EmployeeRosterDept
        {
            get
            {
                var name = "employeeRosterDept";
                var result = GetAttribute(name);

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns "employeeStatus" attributes of EdsEntry</para>
        /// <para>Mosaic HR status codes for employees:</para>
        /// <para>A = Active</para>
        /// <para>D = Deceased</para>
        /// <para>L = Leave of Absence</para>
        /// <para>P = Leave With Pay</para>
        /// <para>R = Retired</para>
        /// <para>S = Suspended</para>
        /// <para>T = Terminated</para>
        /// <para>W = Short Work Break (an employee with this status is still considered to be "active").</para>
        /// </summary>
        /// <value>optional</value>
        public string EmployeeStatus
        {
            get
            {
                var name = "employeeStatus";
                var result = string.Empty;

                var resultArray = GetAttribute(name);
                if (resultArray.Length > 0)
                    result = resultArray[0];

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns the "employeeType" attribute of the EdsEntry.</para>
        /// <para>One character code from Mosaic HR that identifies  the type of an employment.</para>
        /// <para>A = Classified Staff Wage</para>
        /// <para>D = Ancillary Staff Wage</para>
        /// <para>E = Regular Appointed Acad</para>
        /// <para>F = Regular Appointed Temp</para>
        /// <para>H = Ancillary Appointed Temp</para>
        /// <para>G = Ancillary Appointed Academic</para>
        /// <para>I = Supplemental Comp</para>
        /// <para>J = High School Students</para>
        /// <para>P = Contingent Worker</para>
        /// <para>U = Unknown</para>
        /// <para>X = Extra Help - Faculty</para>
        /// <para>1 = Student Employees</para>
        /// <para>2 = Classified Staff Salary</para>
        /// <para>3 = Ancillary Staff Salary</para>
        /// <para>4 = Regular Appointed Fiscal</para>
        /// <para>5 = Clinical Faculty</para>
        /// <para>6 = Federal Employees</para>
        /// <para>7 = Ancillary Appointed Fiscal</para>
        /// <para>8 = Graduate Assistants/Associates</para>
        /// <para>For a Person of Interest (POI), the type value in Mosaic HR is blank.  In EDS, this is indicated by the absence of an employeeType attrbiute.</para>
        /// </summary>
        /// <value>optional</value>
        public string EmployeeType
        {
            get
            {
                var name = "employeeType";
                var result = string.Empty;

                var resultArray = GetAttribute(name);
                if (resultArray.Length > 0)
                    result = resultArray[0];

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// Returns the "givenName" attribute of the EdsEntry
        /// </summary>
        /// <see cref="GivenName"/>
        public string FirstName
        {
            get
            {
                var name = "givenName";
                var result = string.Empty;

                var resultArray = GetAttribute(name);
                if (resultArray.Length > 0)
                    result = resultArray[0];

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// Returns the "cn" attribute of the EdsEntry
        /// </summary>
        /// <see cref="CommonName"/>
        public string FullName
        {
            get
            {
                var name = "cn";
                var result = string.Empty;                
                
                var resultArray = GetAttribute(name);
                if(resultArray.Length > 0)
                    result = resultArray[0];

                return result;
            }
        }

        /// <summary>
        /// <para>Returns the "givenName" attribute of the EdsEntry.</para>        
        /// <para>first name and middle initial, from Mosaic SA when the person's primary affiliation is student, Mosaic HR when primary affiliation is employee, or DSV (NetID) when primary affiliation is affiliate.</para>
        /// </summary>
        /// <value>optional</value>
        public string GivenName
        {
            get { return FirstName; }
        }

        /// <summary>
        /// <para>Returns "isMemberOf" attribute</para>        
        /// <para>Each value is the DN of a group the person is a member of (see course groups).</para>
        /// </summary>
        /// <value>optional</value>
        public string[] Groups
        {
            get
            {                
                var name = "isMemberOf";
                var result = GetAttribute(name);

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns the "studentInfoReleaseCode" attribute of the EdsEntry</para>        
        /// <para>Contains one or more values representing items of student information not to be released. Possible values are:</para>
        /// <para>CAMPUS EMAIL</para>
        /// <para>DATE OF BIRTH</para>
        /// <para>MAIN PHONE</para>
        /// <para>PERMANENT ADDRESS</para>
        /// <para>PRIMARY NAME</para>
        /// </summary>
        /// <value>optional</value>
        public string[] InfoReleaseCode
        {
            get
            {
                var name = "studentInfoReleaseCode";
                var result = GetAttribute(name);

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// Checks "eduPersonAffiliation" to see if it has a value of "admit" in the list of values.
        /// </summary>
        /// <see cref="PersonAffiliation"/>
        public bool IsAdmit
        {
            get
            {
                var result = false;

                string value = "admit";
                var name = "eduPersonAffiliation";
                var resultArray = GetAttribute(name);

                if ((resultArray.Length > 0) && (resultArray.Contains(value)))
                    result = true;

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// Checks "eduPersonAffiliation" to see if it has a value of "employee" in the list of values.
        /// </summary>
        /// <see cref="PersonAffiliation"/>        
        public bool IsEmployee
        {
            get
            {
                var result = false;

                string value = "employee";
                var name = "eduPersonAffiliation";
                var resultArray = GetAttribute(name);

                if ((resultArray.Length > 0) && (resultArray.Contains(value)))
                    result = true;

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns the "studentHonorsActive" attribute of the EdsEntry</para>
        /// <para>If student is, or has previously been, a member of the Honors College, this attribute will be present and reflect their current status in the Honors program. Y indicates "active" and N indicates "inactive".</para>
        /// </summary>
        /// <value>Only returns true if "Y" is present.  No value, or any value other then "Y" returns false.</value>
        public bool IsHonors
        {
            get
            {
                var result = false;

                string value = "Y";
                var name = "studentHonorsActive";
                var resultArray = GetAttribute(name);

                if ((resultArray.Length > 0) && (resultArray.Contains(value)))
                    result = true;

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// Checks "eduPersonAffiliation" to see if it has a value of "member" in the list of values.
        /// </summary>
        /// <see cref="PersonAffiliation"/>
        public bool IsMember
        {
            get
            {
                var result = false;

                string value = "member";
                var name = "eduPersonAffiliation";
                var resultArray = GetAttribute(name);

                if ((resultArray.Length > 0) && (resultArray.Contains(value)))
                    result = true;

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns the "isoNumber" attribute of the EdsEntry</para>        
        /// <para>16-digit ISO number from person's CatCard.</para>
        /// </summary>
        /// <value>optional</value>
        public string IsoNumber
        {
            get
            {
                var name = "isoNumber";
                var result = string.Empty;

                var resultArray = GetAttribute(name);
                if (resultArray.Length > 0)
                    result = resultArray[0];

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// Checks "eduPersonAffiliation" to see if it has a value of "staff" in the list of values.
        /// </summary>
        /// <see cref="PersonAffiliation"/>
        public bool IsStaff
        {
            get
            {
                var result = false;

                string value = "staff";
                var name = "eduPersonAffiliation";
                var resultArray = GetAttribute(name);

                if ((resultArray.Length > 0) && (resultArray.Contains(value)))
                    result = true;

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// Checks "eduPersonAffiliation" to see if it has a value of "student" in the list of values.
        /// </summary>
        /// <see cref="PersonAffiliation"/>
        public bool IsStudent
        {
            get
            {
                var result = false;

                string value = "student";
                var name = "eduPersonAffiliation";
                var resultArray = GetAttribute(name);

                if ((resultArray.Length > 0) && (resultArray.Contains(value)))
                    result = true;

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// Returns the Surname attribute of the EdsEntry
        /// </summary>
        /// <see cref="Surname"/>
        public string LastName
        {
            get
            {
                var name = "sn";
                var result = string.Empty;

                var resultArray = GetAttribute(name);
                if (resultArray.Length > 0)
                    result = resultArray[0];

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// Returns the "mail" attribute of the EdsEntry.  Typically UA email (@email.arizona.edu).
        /// </summary>        
        /// <value>optional</value>
        public string Mail
        {
            get
            {
                var name = "mail";
                var result = string.Empty;

                var resultArray = GetAttribute(name);
                if (resultArray.Length > 0)
                    result = resultArray[0];

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns the "studentCareerProgramPlan" attribute of the EdsEntry where the plan type is 'MAJ'</para>
        /// <para>Contains current, active career program plans with these colon (:) delimited items:</para>
        /// <para>academic career</para>
        /// <para>academic program</para>
        /// <para>academic plan</para>
        /// <para>option (comma separated, if multiple values)</para>
        /// <para>plan type (major, minor, etc.)</para>
        /// <para>program status (active, completed, etc.)</para>
        /// <para>admit term</para>
        /// <para>completion term</para>
        /// </summary>
        /// <value>optional</value>
        public string[] Major
        {
            get
            {
                string[] result = { };
                var plans = new List<string>();

                foreach (string item in StudentCareerProgramPlan)
                {
                    string[] itemArray = item.Split(':');
                    if ((itemArray.Length >= 5) && (!string.IsNullOrEmpty(itemArray[4]) && (itemArray[4].Trim().ToUpper().CompareTo("MAJ") == 0)))
                        plans.Add(item);
                }

                if (plans.Count > 0)
                {
                    result = plans.ToArray();
                    Array.Sort(result);
                }

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns the "studentCareerProgramPlan" attribute of the EdsEntry where the plan type is 'MIN'</para>
        /// <para>Contains current, active career program plans with these colon (:) delimited items:</para>
        /// <para>academic career</para>
        /// <para>academic program</para>
        /// <para>academic plan</para>
        /// <para>option (comma separated, if multiple values)</para>
        /// <para>plan type (major, minor, etc.)</para>
        /// <para>program status (active, completed, etc.)</para>
        /// <para>admit term</para>
        /// <para>completion term</para>
        /// </summary>
        /// <value>optional</value>
        public string[] Minor
        {
            get
            {
                string[] result = { };
                var plans = new List<string>();

                foreach (string item in StudentCareerProgramPlan)
                {
                    string[] itemArray = item.Split(':');
                    if ((itemArray.Length >= 5) && (!string.IsNullOrEmpty(itemArray[4]) && (itemArray[4].Trim().ToUpper().CompareTo("MIN") == 0)))
                        plans.Add(item);
                }

                if (plans.Count > 0)
                {
                    result = plans.ToArray();
                    Array.Sort(result);
                }

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }


        /// <summary>
        /// Returns the "eduPersonNickName" attribute.  Person's nickname (currently not populated).
        /// </summary>
        /// <value>optional</value>
        public string NickName
        {
            get
            {
                var name = "eduPersonNickName";
                var result = string.Empty;

                var resultArray = GetAttribute(name);
                if (resultArray.Length > 0)
                    result = resultArray[0];

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns the "eduPersonAffiliation" attribute.</para>
        /// <para>Possible values are student, admit, employee, faculty, staff, affiliate, poi, member, former-student, former-employee, former-faculty, former-staff, fomer-affiliate, former-poi and former-member.</para>
        /// <para>For students, the values are determined by the current set of rules/heuristics applied to student data, as represented in Mosaic Student Administration. For employees, the values are derived from the UA ABOR codes in the Mosaic HR job records.</para>
        /// <para>The "staff" affiliation includes employees with UA ABOR codes: DOC - Post Doctoral Scholar, PRO - Professional, ADM - Administrative, CLS - Classified Staff</para>
        /// <para>The "faculty" affiliation includes employees wirh UA ABOR code: FAC - Faculty</para>
        /// <para>The "student" affiliation includes employees with UA ABOR codes: GRA - Graduate Assistant/Associate, STU - Student Worker</para>
        /// <para>"Staff" and "faculty" affiliations are instances of "employee", thus "employee" will always be included along with these affiliations.</para>
        /// <para>Students will have the "student" affiliation value. Admitted students who have not yet matriculated will have the "admit" affiliation value.</para>
        /// <para>Departmental-sponsored visitors (DSVs) will have "affiliate" affiliation.</para>
        /// <para>The "former" affiliations are mutually exclusive with the corresponding current affiliations.  For example, a person cannot have both "employee" and "former-employee" affiliations.</para>
        /// <para>"Member" is added if one or more affiliation values (with the exception of "admit") exist. "Former-member" is added if a person has both former-employee and former-student affiliations.</para>
        /// </summary>
        /// <value>optional</value>
        public string[] PersonAffiliation
        {
            get
            {
                var name = "eduPersonAffiliation";
                var result = GetAttribute(name);

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// Returns the "eduPersonPrimaryAffiliation" attribute.
        /// </summary>
        /// <see cref="PersonAffiliation" />
        /// <value>optional</value>
        public string PrimaryPersonAffiliation
        {
            get
            {
                var name = "eduPersonPrimaryAffiliation";
                var result = string.Empty;

                var resultArray = GetAttribute(name);
                if (resultArray.Length > 0)
                    result = resultArray[0];

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns the "studentCareerProgramPlan" attribute of the EdsEntry</para>
        /// <para>Contains current, active career program plans with these colon (:) delimited items:</para>
        /// <para>academic career</para>
        /// <para>academic program</para>
        /// <para>academic plan</para>
        /// <para>option (comma separated, if multiple values)</para>
        /// <para>plan type (major, minor, etc.)</para>
        /// <para>program status (active, completed, etc.)</para>
        /// <para>admit term</para>
        /// <para>completion term</para>
        /// </summary>
        /// <value>optional</value>
        public string[] StudentCareerProgramPlan
        {
            get
            {
                var name = "studentCareerProgramPlan";
                var result = GetAttribute(name);

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns the "studentPrimaryCareerProgramPlan" attribute of the EdsEntry</para>        
        /// <para>Reflects the student's current primary career program plan and has the same format as studentCareerProgramPlan.</para>
        /// </summary>
        /// <value>optional</value>
        /// <seealso cref="StudentCareerProgramPlan"/>
        public string StudentPrimaryCareerProgramPlan
        {
            get
            {
                var name = "studentPrimaryCareerProgramPlan";
                var result = string.Empty;

                var resultArray = GetAttribute(name);
                if (resultArray.Length > 0)
                    result = resultArray[0];

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// <para>Returns the "studentId" attribute of the EdsEntry</para>                
        /// <para>"S" + 8 digits, or 9-digit number, that uniquely identifies a UA student. Note: This attribute will be deprecated when the Mosaic Student Administration initiative (PeopleSoft Campus Solutions) goes live with student bio/demo data in January, 2010.</para>
        /// <para>If no value is found, this attribute will return emplId.</para>
        /// </summary>
        /// <value>optional</value>
        /// <seealso cref="EmplId"/>
        public string StudentId
        {
            get
            {
                var name = "studentId";
                var result = string.Empty;

                var resultArray = GetAttribute(name);

                if (resultArray.Length > 0)
                {
                    result = resultArray[0];
                }
                else
                {
                    result = EmplId;
                }

                return result;
            }
        }

        /// <summary>
        /// <para>Returns the "studentStatus" attribute of the EdsEntry.</para>        
        /// <para>Contains these colon (:) delimited current student status items:</para>
        /// <para>academic career</para>
        /// <para>academic level</para>
        /// <para>academic load</para>
        /// <para>residency</para>
        /// <para>term</para>
        /// </summary>
        /// <value>optional</value>
        public string[] StudentStatus
        {
            get
            {
                var name = "studentStatus";
                var result = GetAttribute(name);

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// Returns the "studentStatusHistory" attribute of the EdsEntry.  Contains past values of the studentStatus attribute.
        /// </summary>
        /// <value>optional</value>
        /// <seealso cref="StudentStatus"/>
        public string[] StudentStatusHistory
        {
            get
            {
                var name = "studentStatusHistory";
                var result = GetAttribute(name);

                return result;
            }
            set { throw new NotImplementedException("EdsEntry is Read-Only.  Cannot Set Property."); }
        }

        /// <summary>
        /// Returns the "uaId" attribute of the EdsEntry.  Uniquely identifies each UA-affiliated person.
        /// </summary>
        /// <value>optional</value>
        public string UaID
        {
            get
            {
                var name = "uaId";
                var result = string.Empty;

                var resultArray = GetAttribute(name);
                if (resultArray.Length > 0)
                    result = resultArray[0];

                return result;
            }
        }

        /// <summary>
        /// Returns UID attribute of EDS
        /// </summary>
        public string UserName
        {
            get
            {
                var name = "uid";
                var result = string.Empty;

                var resultArray = GetAttribute(name);
                if (resultArray.Length > 0)
                    result = resultArray[0];

                return result;
            }
        }

        #endregion

        #endregion

        // Methods
        #region Methods

        /// <summary>
        /// Implicitly converts a SearchResult into an EdsEntry
        /// </summary>
        /// <param name="searchResult">The searchResult to convert.</param>
        /// <returns></returns>
        public static explicit operator EdsEntry(SearchResultEntry searchResult)
        {
            if (searchResult != null)
                return new EdsEntry(searchResult);                
            else
                return null;
        }

        /// <summary>
        /// Adds the specified attribute from the entry.
        /// </summary>
        /// <param name="name">The name of the attribute you are adding.</param>
        /// <param name="value">The array of values for the attribute.</param>
        private void AddAttribute(string name, string[] value)
        {
            if (HasAttribute(name))
                RemoveAttribute(name);                

            _attributes.Add(name, value);
        }

        /// <summary>
        /// Removes the specified attribute from the entry.
        /// </summary>
        /// <param name="name">The name of the attribute you are removing.</param>
        private void RemoveAttribute(string name)
        {
            if (HasAttribute(name))
                _attributes.Remove(name);
        }

        /// <summary>
        /// Check to see if the specified attribute exists in Attributes
        /// </summary>
        /// <param name="name">The name of the attribute you are looking for</param>
        /// <returns></returns>
        public bool HasAttribute(string name)
        {
            var result = false;

            if ((_attributes != null) && (_attributes.Count > 0) && (_attributes.Keys.Contains(name)))
                result = true;

            return result;
        }

        /// <summary>
        /// Gets the specified attribute
        /// </summary>
        /// <param name="name">The name of the attribute you are looking for</param>
        /// <returns></returns>
        public string[] GetAttribute(string name)
        {
            string[] result = { };
            if (HasAttribute(name))
                result = _attributes[name];

            if (result.Length > 0)
                Array.Sort(result);

            return result;
        }

        /// <summary>
        /// Returns ToString, but replaces "\n" with "br" tags
        /// </summary>
        /// <returns></returns>
        public string ToHtml()
        {
            var result = string.Empty;
            StringBuilder builder = new StringBuilder();

            if ((_attributes != null) && (_attributes.Count > 0))
            {
                foreach (string key in _attributes.Keys)
                {
                    builder.AppendLine("<br />");
                    builder.Append(string.Format("<span style=\"font-weight:bold\">{0}:</span>", key));

                    string[] attribute = GetAttribute(key);

                    if (attribute.Length == 1)
                    {
                        builder.AppendLine(string.Format(" [{0}]", attribute[0]));
                    }
                    else if (attribute.Length > 1)
                    {
                        builder.AppendLine("<br />");
                        foreach (string value in attribute)
                            builder.AppendLine(string.Format("&nbsp;&nbsp;[{0}]", value));
                    }                    
                }
            }

            result = builder.ToString();

            return result;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns the List of Attributes and their values.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var result = string.Empty;
            StringBuilder builder = new StringBuilder();

            if ((_attributes != null) && (_attributes.Count > 0))
            {
                foreach (string key in _attributes.Keys)
                {                    
                    builder.Append(string.Format("{0}:", key));

                    string[] attribute = GetAttribute(key);

                    if (attribute.Length == 1)
                    {
                        builder.AppendLine(string.Format(" [{0}]", attribute[0]));
                    }
                    else if (attribute.Length > 1)
                    {
                        builder.AppendLine();
                        foreach (string value in attribute)
                            builder.AppendLine(string.Format("  [{0}]", value));
                    }                    
                }
            }

            result = builder.ToString();

            return result;
        }

        // IComparable
        #region IComparable

        /// <summary>
        /// Implements the CompareTo(EdsEntry) interface.
        /// </summary>
        /// <param name="other">The other EdsEntry to compare to</param>
        /// <remarks>Order of comparison by Attribute is </remarks>
        /// <returns></returns>
        public int CompareTo(EdsEntry other)
        {
            int result = 0;

            string myString = ToString();
            string otherString = string.Empty;

            if (other != null)
                otherString = other.ToString();

            result = myString.CompareTo(otherString);

            return result;
        }

        #endregion

        // IEquatable
        #region IEquatable

        /// <summary>
        /// Implements IEquatable(EdsEntry)
        /// </summary>
        /// <param name="other">The other EdsEntry</param>
        /// <returns></returns>
        public bool Equals(EdsEntry other)
        {
            var result = false;

            if (CompareTo(other) == 0)
                result = true;

            return result;
        }

        #endregion

        #endregion

    }
}
