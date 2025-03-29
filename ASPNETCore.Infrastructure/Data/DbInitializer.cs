using ASPNETCore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNETCore.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // Проверяем наличие ролей
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            // Проверяем наличие пользователей
            if (!userManager.Users.Any())
            {
                var adminUser = new User { UserName = "admin", Email = "admin@example.com", Passport = "2112855774", Phone = "88005553535", FullName = "azaza" };
                var userCreationResult = await userManager.CreateAsync(adminUser, "Admin@123");
                if (userCreationResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "admin");
                }

                var normalUser = new User { UserName = "user", Email = "user@example.com", Passport = "2112811774", Phone = "89005553535", FullName = "brazaza" };
                var normalUserCreationResult = await userManager.CreateAsync(normalUser, "User@123");
                if (normalUserCreationResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(normalUser, "user");
                }
            }
            if (!context.DealTypes.Any())
            {
                var obj1 = new DealType
                {
                    DealName = "Аренда",
                };

                var obj2 = new DealType
                {
                    DealName = "Продажа",
                };

                context.DealTypes.Add(obj1);
                context.DealTypes.Add(obj2);

                await context.SaveChangesAsync();
            }
            if (!context.ObjectTypes.Any())
            {
                var obj1 = new ObjectType
                {
                    TypeName = "Квартира",
                };

                var obj2 = new ObjectType
                {
                    TypeName = "Дом",
                };

                context.ObjectTypes.Add(obj1);
                context.ObjectTypes.Add(obj2);

                await context.SaveChangesAsync();
            }
            if (!context.Statuses.Any())
            {
                var obj1 = new Status
                {
                    StatusName = "Свободен",
                };

                var obj2 = new Status
                {
                    StatusName = "Занят",
                };

                context.Statuses.Add(obj1);
                context.Statuses.Add(obj2);

                await context.SaveChangesAsync();
            }

            if (!context.Objects.Any())
             {
                var obj1 = new REObject
                {
                    Rooms = 1,
                    Floors = 1,
                    Square = 20,
                    TypeId = 1,
                    DealTypeId = 2,
                    Street = "Рабфаковская",
                    Building = 34,
                    Number = 34,
                    Price = 1000000,
                    StatusId = 1
                };

                var obj2 = new REObject
                {
                    Rooms = 3,
                    Floors = 1,
                    Square = 40,
                    TypeId = 1,
                    DealTypeId = 1,
                    Street = "Владимировская",
                    Building = 34,
                    Number = 34,
                    Price = 10000,
                    StatusId = 1
                };
                
                context.Objects.Add(obj1);
                context.Objects.Add(obj2);

                await context.SaveChangesAsync();
            }
        }
    }
}
