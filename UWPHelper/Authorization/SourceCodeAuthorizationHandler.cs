using ContactManager.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWPHelper.Models;

namespace UWPHelper.Authorization
{
    public class SourceCodeAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, SourceCode>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, SourceCode resource)
        {
            //如果是空用户 : 返回
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            // 系统所有者可以访问，否则不通过
            if (context.User.IsInRole(Constants.ContactAdministratorsRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
