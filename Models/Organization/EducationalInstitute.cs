namespace SIS.Models.Organization
{
    public class EducationalInstitute : Organization
    {
        public Period.InstituteType InstituteType { get; set; }

        public EducationalInstitute(string name, string address, string phone, 
                                  string email, string url, ContactPerson contactPerson, 
                                  Period.InstituteType instituteType)
            : base(name, address, phone, email, url, contactPerson)
        {
            InstituteType = instituteType;
        }
    }
}