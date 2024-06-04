using E_Diary.WEB.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace E_Diary.WEB.Data
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "admin@gmail.com";
            string password = "_Aa123456";
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("teacher") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("teacher"));
            }
            if (await roleManager.FindByNameAsync("schoolboy") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("schoolboy"));
            }
            if (await roleManager.FindByNameAsync("parent") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("parent"));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User {
                    Email = adminEmail, UserName = adminEmail, EmailConfirmed = true,
                    Name="Админ", Patronymic="Админович", Surname="Админов", Gender = Enums.Gender.Male
                };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }
    }
}
