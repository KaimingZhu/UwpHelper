using ContactManager.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWPHelper.Models;

namespace UWPHelper.Areas.Identity.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new IdentityContext(
                serviceProvider.GetRequiredService<DbContextOptions<IdentityContext>>()))
            {
                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@neu.edu.com");
                await EnsureRole(serviceProvider, adminID, Constants.ContactAdministratorsRole);

            }
        }

        //添加SeedUser
        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                            string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<UWPHelperUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new UWPHelperUser { UserName = UserName };
                await userManager.CreateAsync(user, testUserPw);
            }

            return user.Id;
        }

        //对指定User添加Role
        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                              string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<UWPHelperUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }
    }
}