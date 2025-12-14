using SIS.Models;
using SIS.Models.Internship;
using SIS.Models.Organization;
using System.Collections.Generic;

namespace SIS.Repositories
{
    /// <summary>
    /// Repository interface for Organization entities
    /// </summary>
    public interface IOrganizationRepository : IRepository<Organization>
    {
        /// <summary>
        /// Validates organization data before adding/updating
        /// </summary>
        /// <param name="organization">Organization to validate</param>
        /// <returns>True if valid</returns>
        bool ValidateOrganizationData(Organization organization);

        /// <summary>
        /// Adds an internship to an organization
        /// </summary>
        /// <param name="organizationId">Organization ID</param>
        /// <param name="internship">Internship to add</param>
        /// <returns>True if successful</returns>
        bool AddInternshipToOrganization(int organizationId, Internship internship);

        /// <summary>
        /// Gets all internships for a specific organization
        /// </summary>
        /// <param name="organizationId">Organization ID</param>
        /// <returns>List of internships</returns>
        List<Internship> GetInternshipsByOrganizationId(int organizationId);

        /// <summary>
        /// Gets organization by name
        /// </summary>
        /// <param name="name">Organization name</param>
        /// <returns>Organization or null if not found</returns>
        Organization GetByName(string name);

        /// <summary>
        /// Searches organizations by name (partial match)
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching organizations</returns>
        List<Organization> SearchByName(string searchTerm);

        /// <summary>
        /// Gets organizations by type
        /// </summary>
        /// <param name="organizationType">Type of organization</param>
        /// <returns>List of organizations of the specified type</returns>
        List<Organization> GetByType(string organizationType);

        /// <summary>
        /// Gets all contact persons for an organization
        /// </summary>
        /// <param name="organizationId">Organization ID</param>
        /// <returns>List of contact persons</returns>
        List<ContactPerson> GetContactPersonsByOrganizationId(int organizationId);

        /// <summary>
        /// Checks if an organization with the given name exists
        /// </summary>
        /// <param name="name">Organization name</param>
        /// <returns>True if exists</returns>
        bool OrganizationExists(string name);
    }
}