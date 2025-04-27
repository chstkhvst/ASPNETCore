using ASPNETCore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
                var adminUser = new User { UserName = "admin", Email = "admin@example.com", Passport = "2112855774",  FullName = "azaza" };
                var userCreationResult = await userManager.CreateAsync(adminUser, "Admin@123");
                if (userCreationResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "admin");
                }

                var normalUser = new User { UserName = "user", Email = "user@example.com", Passport = "2112811774",  FullName = "brazaza" };
                var normalUserCreationResult = await userManager.CreateAsync(normalUser, "User@123");
                if (normalUserCreationResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(normalUser, "user");
                }
            }
            if (!context.ResStatuses.Any())
            {
                var obj1 = new ResStatus
                {
                    StatusType = "Оставлена",
                };

                var obj2 = new ResStatus
                {
                    StatusType = "Одобрена",
                };
                var obj3 = new ResStatus
                {
                    StatusType = "Отменена",
                };

                context.ResStatuses.Add(obj1);
                context.ResStatuses.Add(obj2);
                context.ResStatuses.Add(obj3);

                await context.SaveChangesAsync();
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
                    Roomnum = 34,
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
                    Roomnum = 34,
                    Price = 10000,
                    StatusId = 1
                };
                
                context.Objects.Add(obj1);
                context.Objects.Add(obj2);

                await context.SaveChangesAsync();
            }
            var existingObjectsCount = await context.Objects.CountAsync();
            if (existingObjectsCount < 5)
            {
                var newObjects = new List<REObject>
                {
                    new REObject
                    {
                        Rooms = 2,
                        Floors = 5,
                        Square = 45,
                        TypeId = 1,
                        DealTypeId = 2,
                        Street = "Ленина",
                        Building = 10,
                        Roomnum = 25,
                        Price = 2500000,
                        StatusId = 1
                    },
                    new REObject
                    {
                        Rooms = 4,
                        Floors = 2,
                        Square = 80,
                        TypeId = 2,
                        DealTypeId = 2,
                        Street = "Гагарина",
                        Building = 15,
                        Roomnum = 3,
                        Price = 5000000,
                        StatusId = 1
                    },
                    new REObject
                    {
                        Rooms = 1,
                        Floors = 9,
                        Square = 35,
                        TypeId = 1,
                        DealTypeId = 1,
                        Street = "Советская",
                        Building = 22,
                        Roomnum = 12,
                        Price = 15000,
                        StatusId = 1
                     }
                };

                await context.Objects.AddRangeAsync(newObjects);
                await context.SaveChangesAsync();
            }
            if (!await context.Reservations.AnyAsync())
            {
                var user = await userManager.FindByNameAsync("user");
                var objects = await context.Objects.OrderBy(o => o.Id).ToListAsync();

                // Бронируем 3-й и 4-й объекты (новые добавленные)
                var reservations = new List<Reservation>
                    {
                        new Reservation
                        {
                            ObjectId = objects[2].Id,
                            UserId = user.Id,
                            StartDate = DateTime.Now.AddDays(-5),
                            EndDate = DateTime.Now.AddDays(1000),
                            ResStatusId = 1
                        },
                        new Reservation
                        {
                            ObjectId = objects[3].Id,
                            UserId = user.Id,
                            StartDate = DateTime.Now.AddDays(-2),
                            EndDate = DateTime.Now.AddDays(1000),
                            ResStatusId = 1
                        }
                    };
                objects[2].StatusId = 2;
                objects[3].StatusId = 2;
                await context.Reservations.AddRangeAsync(reservations);
                await context.SaveChangesAsync();
            }
            if (!await context.Contracts.AnyAsync())
            {
                var user = await userManager.FindByNameAsync("user");
                var reservation = await context.Reservations.FirstOrDefaultAsync(); // Изменено на FirstOrDefault

                if (reservation != null) // Добавлена проверка на null
                {
                    var contract = new Contract
                    {
                        SignDate = DateTime.Now,
                        ReservationId = reservation.Id,
                        UserId = user.Id,
                        Total = 1500000
                    };

                    await context.Contracts.AddAsync(contract);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
