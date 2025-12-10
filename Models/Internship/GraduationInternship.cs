using SIS.Models.Period;

namespace SIS.Models.Internship
{
    public class GraduationInternship : Internship
    {
        public override Assignment.AssignmentType AssignmentType => Assignment.AssignmentType.ENGINEERING;

        public GraduationInternship(Organization.Organization organization, string city,
                                  long dateOfSubmission, string projectTitle,
                                  string shortDescription, string longDescription, Period.Period period)
            : base(organization, city, dateOfSubmission, projectTitle, shortDescription, longDescription, period)
        {
        }
    }
}