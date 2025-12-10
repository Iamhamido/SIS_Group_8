namespace SIS.Models.Organization
{
    public class Company : Organization
    {
        public Company(string name, string address, string phone, 
                      string email, string url, ContactPerson contactPerson)
            : base(name, address, phone, email, url, contactPerson)
        {
        }
    }
}