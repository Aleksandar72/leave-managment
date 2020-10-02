using LeaveManagment.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagment
{
    public static class ManageRoles
    {
        public static void CreateDefault(UserManager<Employee> userManager,
                                   RoleManager<IdentityRole> roleManager)
        {
            CreateRole(roleManager);
            CreateUser(userManager);

        }
        private static void CreateUser(UserManager<Employee> userManager)
        {
            if (userManager.FindByNameAsync("SuperAdmin").Result == null)
            {
                var user = new Employee
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com"
                };
                var res = userManager.CreateAsync(user, "Pasword-1").Result;
                if (res.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }

            }

        }
        private static void CreateRole(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Administrator").Result)
            {
                var createRole = new IdentityRole
                {
                    Name = "Administrator"
                };
               var newRole = roleManager.CreateAsync(createRole).Result;

            }
            if (!roleManager.RoleExistsAsync("Employee").Result)
            {
                var createRole = new IdentityRole
                {
                    Name = "Employee"
                };
                var newRole = roleManager.CreateAsync(createRole).Result;

            }

        }
    }
}
