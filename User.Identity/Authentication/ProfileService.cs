using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace User.Identity.Authentication
{
    /// <summary>
    /// GrantValidationResult 自定义Claim 方法延伸
    /// </summary>
    public class ProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentException(nameof(context.Subject));
            var subjectId = subject.Claims.FirstOrDefault(m => m.Type == "sub").Value;
            if (string.IsNullOrWhiteSpace(subjectId.ToString()))
            {
                throw new ArgumentException("无效的主键id");
            }
            context.IssuedClaims = context.Subject.Claims.ToList();
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentException(nameof(context.Subject));
            // var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;
            var subjectId = subject.Claims.FirstOrDefault(x => x.Type == "sub").Value;
            context.IsActive = !string.IsNullOrWhiteSpace(subjectId.ToString());
            return Task.CompletedTask;
        }
    }
}
