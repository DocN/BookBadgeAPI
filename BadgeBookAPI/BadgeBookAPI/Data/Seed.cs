using BadgeBookAPI.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BadgeBookAPI.Data
{
    public class Seed
    {
        public static async Task Initialize(ApplicationDBContext context,
                               RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {

            string role1 = "Admin";
            //string desc1 = "This is the administrator role";

            string role2 = "User";
            //string desc2 = "This is the members role";

            context.Database.EnsureCreated();
            if (await roleManager.FindByNameAsync(role1) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(role1));
            }
            if (await roleManager.FindByNameAsync(role2) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(role2));
            }

            await createAppAuth(context, roleManager, userManager);

            if (await userManager.FindByEmailAsync("user1") == null)
            {
                IdentityUser newUser = new IdentityUser();
                newUser.Email = "user1@gmail.com";
                newUser.UserName = "user1@gmail.com";

                var result = await userManager.CreateAsync(newUser, "P@$$w0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "User");
                    UserData newUserData = new UserData();
                    newUserData.UID = newUser.Id;
                    newUserData.Country = "canada";
                    newUserData.FirstName = "batman1";
                    newUserData.LastName = "bruce1";
                    Profile ProfileData = new Profile();
                    ProfileData.UID = newUser.Id;
                    ProfileData.Description = @"<p>My name is Earl. I am fast and write PHP for BCIT schools. here is my loooong story ...  . Here is my project screenshot <img src=""https://cdn3.iconfinder.com/data/icons/street-food-and-food-trucker-1/64/hamburger-fast-food-patty-bread-512.png"" alt=""Earl's project ""> </p>";
                    ProfileData.Description = HttpUtility.HtmlEncode(ProfileData.Description);
                    newUserData.ProfileData = ProfileData;
                    DateTime newBirthday = new DateTime(1950, 10, 10);
                    newUserData.Birthday = newBirthday;
                    context.Profile.Add(ProfileData);
                    context.UserData.Add(newUserData);
                    context.SaveChanges();
                }
            }

            if (await userManager.FindByEmailAsync("user2") == null)
            {
                IdentityUser newUser = new IdentityUser();
                newUser.Email = "user2@gmail.com";
                newUser.UserName = "user2@gmail.com";
                var result = await userManager.CreateAsync(newUser, "P@$$w0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "User");
                    UserData newUserData = new UserData();
                    newUserData.UID = newUser.Id;
                    newUserData.Country = "canada";
                    newUserData.FirstName = "batman2";
                    newUserData.LastName = "bruce2";
                    Profile ProfileData = new Profile();
                    ProfileData.UID = newUser.Id;
                    ProfileData.Description = @"<p>My name is Earl. I am fast and write PHP for BCIT schools. here is my loooong story ...  . Here is my project screenshot <img src=""https://cdn3.iconfinder.com/data/icons/street-food-and-food-trucker-1/64/hamburger-fast-food-patty-bread-512.png"" alt=""Earl's project ""> </p>";
                    ProfileData.Description = HttpUtility.HtmlEncode(ProfileData.Description);
                    newUserData.ProfileData = ProfileData;
                    DateTime newBirthday = new DateTime(1950, 10, 10);
                    newUserData.Birthday = newBirthday;
                    context.Profile.Add(ProfileData);
                    context.UserData.Add(newUserData);
                    context.SaveChanges();
                }
            }
            if (await userManager.FindByEmailAsync("user3") == null)
            {
                IdentityUser newUser = new IdentityUser();
                newUser.Email = "user3@gmail.com";
                newUser.UserName = "user3@gmail.com";
                var result = await userManager.CreateAsync(newUser, "P@$$w0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "User");
                    UserData newUserData = new UserData();
                    newUserData.UID = newUser.Id;
                    newUserData.Country = "canada";
                    newUserData.FirstName = "batman3";
                    newUserData.LastName = "bruce3";
                    Profile ProfileData = new Profile();
                    ProfileData.UID = newUser.Id;
                    ProfileData.Description = @"<p>My name is Earl. I am fast and write PHP for BCIT schools. here is my loooong story ...  . Here is my project screenshot <img src=""https://cdn3.iconfinder.com/data/icons/street-food-and-food-trucker-1/64/hamburger-fast-food-patty-bread-512.png"" alt=""Earl's project ""> </p>";
                    ProfileData.Description = HttpUtility.HtmlEncode(ProfileData.Description);
                    newUserData.ProfileData = ProfileData;
                    DateTime newBirthday = new DateTime(1950, 10, 10);
                    newUserData.Birthday = newBirthday;
                    context.Profile.Add(ProfileData);
                    context.UserData.Add(newUserData);
                    context.SaveChanges();
                }
            }
        }

        private static async Task createAppAuth(ApplicationDBContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            string role3 = "App";
            string[] apps = { "hangman", "webcam", "tank" };
            if (await roleManager.FindByNameAsync(role3) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(role3));
            }
            foreach(var app in apps)
            {
                if (await userManager.FindByEmailAsync(app) == null)
                {
                    IdentityUser newUser = new IdentityUser();
                    newUser.Email = app + "@gmail.com";
                    newUser.UserName = app;
                    var result = await userManager.CreateAsync(newUser, "P@$$w0rd");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newUser, "App");
                    }
                }
            }
        }
    }
}
