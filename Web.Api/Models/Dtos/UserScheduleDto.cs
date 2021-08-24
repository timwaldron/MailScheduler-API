using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Models.Dtos
{
    public class UserScheduleDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string SurveyId { get; set; }
        public string RecalcFollowupDates { get; set; }

        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }

        public string RecruitmentDate { get; set; }
        public string SurgeryDate { get; set; }

        /**
         * Current followup dates
         * Survey sent on Recruitment Date
         * Surgery Date
         * 
         * Surveys After Surgery:
         *  - 6 Weeks
         *  - 3 Months
         *  - 6 Months
         *  - 12 Months
         *  - 2 Years
         *  - 5 Years
         *  - 10 Years
         */
        public IEnumerable<string> FollowupDates { get; set; }
    }
}
