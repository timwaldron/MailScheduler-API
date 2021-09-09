using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Helpers
{
    public class Templates
    {
        /*
         * List of replaceable values:
         * 
         * {FIRSTNAME}      - Patients first name
         * {LASTNAME}       - Patients last name
         * {TIMEPOINT}      - Time point for follow (e.g. 6 weeks, 12 months, etc)
         * {SURGERYTYPE}    - Type of surgery (injury type)
         * {SURVEYURL}      - Survey URL (in appsettings.json)
         */
        public static string EmailHTML = "<body>Dear {FIRSTNAME} {LASTNAME}, it's been {TIMEPOINT} since you had your {SURGERYTYPE} surgery. Please complete this survery: {SURVEYURL}</body>";
    }
}
