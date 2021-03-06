﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Store.PartnerCenter.PowerShell.Commands
{
    using System.Linq;
    using System.Management.Automation;
    using System.Text.RegularExpressions;
    using Models.Authentication;
    using Models.Relationships;
    using PartnerCenter.Models;
    using PartnerCenter.Models.Relationships;

    /// <summary>
    /// Gets a list of indirect resellers from Partner Center.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "PartnerIndirectReseller")]
    [OutputType(typeof(PSPartnerRelationship))]
    public class GetPartnerIndirectReseller : PartnerAsyncCmdlet
    {
        /// <summary>
        /// Gets or sets the required customer identifier.
        /// </summary>
        [Parameter(HelpMessage = "The identifier for the customer.", Mandatory = false)]
        [ValidatePattern(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", Options = RegexOptions.Compiled | RegexOptions.IgnoreCase)]
        public string CustomerId { get; set; }

        /// <summary>
        /// Executes the operations associated with the cmdlet.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            Scheduler.RunTask(async () =>
            {
                IPartner partner = await PartnerSession.Instance.ClientFactory.CreatePartnerOperationsAsync(CorrelationId, CancellationToken).ConfigureAwait(false);
                ResourceCollection<PartnerRelationship> resellers;

                if (string.IsNullOrEmpty(CustomerId))
                {
                    resellers = await partner.Relationships.GetAsync(PartnerRelationshipType.IsIndirectCloudSolutionProviderOf, CancellationToken).ConfigureAwait(false);
                }
                else
                {
                    resellers = await partner.Customers[CustomerId].Relationships.GetAsync(CancellationToken).ConfigureAwait(false);
                }

                WriteObject(resellers.Items.Select(r => new PSPartnerRelationship(r)), true);
            }, true);
        }
    }
}