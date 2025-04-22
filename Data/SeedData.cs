// Data/SeedData.cs
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
            // Đảm bảo các role tồn tại
            string[] roleNames = { "Admin", "Librarian", "Member" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Tạo Admin nếu chưa tồn tại
            var adminEmail = "admin@library.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            
            if (admin == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Administrator",
                    Address = "Library Address",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    MemberSince = DateTime.Now
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Tạo một Librarian mẫu
            var librarianEmail = "librarian@library.com";
            var librarian = await userManager.FindByEmailAsync(librarianEmail);
            
            if (librarian == null)
            {
                var librarianUser = new ApplicationUser
                {
                    UserName = librarianEmail,
                    Email = librarianEmail,
                    EmailConfirmed = true,
                    FirstName = "Library",
                    LastName = "Staff",
                    Address = "Library Address",
                    DateOfBirth = new DateTime(1995, 5, 15),
                    MemberSince = DateTime.Now
                };

                var result = await userManager.CreateAsync(librarianUser, "Librarian@123");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(librarianUser, "Librarian");
                }
            }
        }
    }
}