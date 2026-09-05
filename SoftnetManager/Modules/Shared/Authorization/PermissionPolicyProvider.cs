
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SoftnetManager.Modules.Shared.Authorization
{
    public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private const string policyPrefix = "Permission.";
        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(policyPrefix,StringComparison.OrdinalIgnoreCase))
            {
                var permission = policyName.Substring(policyPrefix.Length);

                var policy  = new AuthorizationPolicyBuilder()
                                .AddRequirements(new PermissionRequirement(permission))
                                .Build();

                return policy;


            }

            return await base.GetPolicyAsync(policyName);
        }
    }
}
