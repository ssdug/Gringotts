using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Models.Users;
using MediatR;
using System;
using System.Linq;

namespace Wiz.Gringotts.UIWeb.Models.Organizations
{
    public class OrganizationDetailsQuery : IAsyncRequest<OrganizationDetails>
    {
        public int OrganizationId { get; set; }

        public OrganizationDetailsQuery(int organizationId)
        {
            OrganizationId = organizationId;
        }
    }

    public class OrganizationDetailsQueryHandler : IAsyncRequestHandler<OrganizationDetailsQuery, OrganizationDetails>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Organization> organizations;

        public OrganizationDetailsQueryHandler(ISearch<Organization> organizations)
        {
            this.organizations = organizations;
        }

        public async Task<OrganizationDetails> Handle(OrganizationDetailsQuery query)
        {
            Logger.Trace("Handle::{0}", query.OrganizationId);

            var organization = await organizations.GetById(query.OrganizationId)
                .Include(o => o.Features)
                .Include(o => o.Children)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (organization != null)
            {
                return new OrganizationDetails
                {
                    Organization = organization
                };
            }
            return null;
        }
    }

    public class OrganizationDetailsViewModelPostProccesor : IAsyncPostRequestHandler<OrganizationDetailsQuery, OrganizationDetails>
    {
        public ILogger Logger { get; set; }

        private readonly IUserRepository userRepository;

        public OrganizationDetailsViewModelPostProccesor(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task Handle(OrganizationDetailsQuery command, OrganizationDetails response)
        {
            Logger.Trace("Handle");

            if (response == null)
                return;

            await Task.Run(() =>
            {
                if (!string.IsNullOrWhiteSpace(response.Organization.FiscalContactSamAccountName))
                    response.FiscalContact = GetActiveDirectoryUser(response.Organization.FiscalContactSamAccountName);

                if (!string.IsNullOrWhiteSpace(response.Organization.ITConactSamAccountName))
                    response.ITContact = GetActiveDirectoryUser(response.Organization.ITConactSamAccountName);

                if (!string.IsNullOrWhiteSpace(response.Organization.GroupName))
                    response.Users = GetActiveDirectoryUsers(response.Organization.GroupName);
            });

        }

        private IEnumerable<User> GetActiveDirectoryUsers(string distinguishedName)
        {
            Logger.Trace("GetActiveDirectoryUsers::{0}", distinguishedName);
            try
            {
                return userRepository.FindByOrganization(distinguishedName);
            }
            catch(Exception)
            {
                return Enumerable.Empty<User>();
            }
        }

        private User GetActiveDirectoryUser(string samAccountName)
        {
            Logger.Trace("GetActiveDirectoryUser::{0}", samAccountName);
            try
            {
                return userRepository.FindByUser(samAccountName);

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}