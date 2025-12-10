using SIS.Models.Period;

namespace SIS.Models.Internship
{
    public class MinorInternship : Internship
    {
        public Organization.EducationalInstitute OrganizationInstitute => Organization as Organization.EducationalInstitute;
        public int MinorId { get; set; }
        public override Assignment.AssignmentType AssignmentType => Assignment.AssignmentType.MINOR;

        public MinorInternship(Organization.EducationalInstitute organization, string city,
                             long dateOfSubmission, string projectTitle,
                             string shortDescription, string longDescription, 
                             Period.Period period, int minorId)
            : base(organization, city, dateOfSubmission, projectTitle, shortDescription, longDescription, period)
        {
            MinorId = minorId;
        }
    }
}