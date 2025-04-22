using LibraryManagement.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Tạo các roles nếu chưa tồn tại
            string[] roleNames = { "Admin", "Librarian", "Member" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Tạo tài khoản Admin nếu chưa tồn tại
            var adminEmail = "admin@library.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    Address = "Library HQ",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    MemberSince = DateTime.Now,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Tạo tài khoản Librarian nếu chưa tồn tại
            var librarianEmail = "librarian@library.com";
            var librarianUser = await userManager.FindByEmailAsync(librarianEmail);
            if (librarianUser == null)
            {
                var librarian = new ApplicationUser
                {
                    UserName = librarianEmail,
                    Email = librarianEmail,
                    FirstName = "Librarian",
                    LastName = "User",
                    Address = "Library HQ",
                    DateOfBirth = new DateTime(1995, 5, 5),
                    MemberSince = DateTime.Now,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(librarian, "Librarian@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(librarian, "Librarian");
                }
            }
        }
    }
}