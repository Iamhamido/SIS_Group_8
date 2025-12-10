using SIS.Models.Period;

namespace SIS.Models.Internship
{
    public class IntermediateInternship : Internship
    {
        public override Assignment.AssignmentType AssignmentType => Assignment.AssignmentType.ENGINEERING;

        public IntermediateInternship(Organization.Organization organization, string city,
                                    long dateOfSubmission, string projectTitle,
                                    string shortDescription, string longDescription, Period.Period period)
            : base(organization, city, dateOfSubmission, projectTitle, shortDescription, longDescription, period)
        {
        }
    }
}