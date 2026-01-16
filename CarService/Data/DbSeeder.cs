using CarService.Models;
using CarService.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarService.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var context = services.GetRequiredService<ApplicationDbContext>();

            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
            await SeedServicesAsync(context);
            await SeedPartsAsync(context);
            await SeedDemoDataAsync(context, userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Mechanic", "Client" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            // Admin user
            await CreateUserIfNotExists(
                userManager,
                "admin@carservice.com",
                "Admin123!",
                "System",
                "Administrator",
                "Admin");

            // Mechanic 1
            await CreateUserIfNotExists(
                userManager,
                "jan.kowalski@carservice.com",
                "Mechanic123!",
                "Jan",
                "Kowalski",
                "Mechanic");

            // Mechanic 2
            await CreateUserIfNotExists(
                userManager,
                "anna.nowak@carservice.com",
                "Mechanic123!",
                "Anna",
                "Nowak",
                "Mechanic");

            // Client
            await CreateUserIfNotExists(
                userManager,
                "client@example.com",
                "Client123!",
                "Tomasz",
                "Klient",
                "Client");
        }

        private static async Task CreateUserIfNotExists(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string firstName,
            string lastName,
            string role)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        private static async Task SeedServicesAsync(ApplicationDbContext context)
        {
            if (await context.Services.AnyAsync())
            {
                return; // Already seeded
            }

            var services = new List<Service>
            {
                // Diagnostyka (2)
                new Service { Name = "Diagnostyka komputerowa", Description = "Pełna diagnostyka komputerowa wszystkich systemów pojazdu", Price = 150.00m, IsActive = true },
                new Service { Name = "Przegląd przedsprzedażowy", Description = "Kompleksowa inspekcja pojazdu przed zakupem", Price = 250.00m, IsActive = true },

                // Usługi silnikowe (4)
                new Service { Name = "Wymiana oleju", Description = "Wymiana oleju silnikowego i filtra", Price = 120.00m, IsActive = true },
                new Service { Name = "Wymiana paska rozrządu", Description = "Wymiana paska rozrządu i napinacza", Price = 800.00m, IsActive = true },
                new Service { Name = "Wymiana świec zapłonowych", Description = "Wymiana wszystkich świec zapłonowych", Price = 180.00m, IsActive = true },
                new Service { Name = "Regulacja silnika", Description = "Kompleksowa regulacja i strojenie silnika", Price = 350.00m, IsActive = true },

                // Hamulce (4)
                new Service { Name = "Wymiana klocków hamulcowych - przód", Description = "Wymiana przednich klocków hamulcowych", Price = 280.00m, IsActive = true },
                new Service { Name = "Wymiana klocków hamulcowych - tył", Description = "Wymiana tylnych klocków hamulcowych", Price = 250.00m, IsActive = true },
                new Service { Name = "Wymiana tarcz hamulcowych", Description = "Wymiana tarcz hamulcowych na jednej osi", Price = 450.00m, IsActive = true },
                new Service { Name = "Wymiana płynu hamulcowego", Description = "Całkowita wymiana płynu hamulcowego", Price = 120.00m, IsActive = true },

                // Zawieszenie (3)
                new Service { Name = "Wymiana amortyzatorów", Description = "Wymiana amortyzatorów (para)", Price = 600.00m, IsActive = true },
                new Service { Name = "Geometria kół", Description = "Regulacja geometrii wszystkich kół", Price = 180.00m, IsActive = true },
                new Service { Name = "Przegląd zawieszenia", Description = "Kompleksowy przegląd układu zawieszenia", Price = 100.00m, IsActive = true },

                // Opony (3)
                new Service { Name = "Rotacja opon", Description = "Rotacja wszystkich czterech opon", Price = 60.00m, IsActive = true },
                new Service { Name = "Wyważanie kół", Description = "Wyważanie wszystkich czterech kół", Price = 80.00m, IsActive = true },
                new Service { Name = "Wymiana opon sezonowa", Description = "Sezonowa wymiana opon (4 sztuki)", Price = 100.00m, IsActive = true },

                // Elektryka (3)
                new Service { Name = "Wymiana akumulatora", Description = "Wymiana i testowanie akumulatora", Price = 200.00m, IsActive = true },
                new Service { Name = "Wymiana alternatora", Description = "Wymiana alternatora", Price = 450.00m, IsActive = true },
                new Service { Name = "Wymiana rozrusznika", Description = "Wymiana rozrusznika", Price = 380.00m, IsActive = true },

                // Klimatyzacja i chłodzenie (3)
                new Service { Name = "Nabijanie klimatyzacji", Description = "Uzupełnienie czynnika chłodniczego klimatyzacji", Price = 180.00m, IsActive = true },
                new Service { Name = "Płukanie chłodnicy", Description = "Płukanie i wymiana płynu chłodzącego", Price = 150.00m, IsActive = true },
                new Service { Name = "Wymiana termostatu", Description = "Wymiana termostatu silnika", Price = 200.00m, IsActive = true },

                // Inne (3)
                new Service { Name = "Wymiana wycieraczek", Description = "Wymiana przednich i tylnych wycieraczek", Price = 60.00m, IsActive = true },
                new Service { Name = "Wymiana żarówek", Description = "Wymiana żarówek reflektorów lub świateł przeciwmgielnych", Price = 80.00m, IsActive = true },
                new Service { Name = "Przegląd ogólny", Description = "Ogólny przegląd pojazdu i kontrola bezpieczeństwa", Price = 100.00m, IsActive = true }
            };

            context.Services.AddRange(services);
            await context.SaveChangesAsync();
        }

        private static async Task SeedPartsAsync(ApplicationDbContext context)
        {
            if (await context.Parts.AnyAsync())
            {
                return; // Already seeded
            }

            var parts = new List<Part>
            {
                // Oleje i płyny (6)
                new Part { Name = "Olej silnikowy 5W-30 (5L)", StockQuantity = 50, UnitPrice = 120.00m },
                new Part { Name = "Olej silnikowy 5W-40 (5L)", StockQuantity = 40, UnitPrice = 130.00m },
                new Part { Name = "Płyn hamulcowy DOT4 (1L)", StockQuantity = 30, UnitPrice = 45.00m },
                new Part { Name = "Koncentrat płynu chłodniczego (5L)", StockQuantity = 25, UnitPrice = 80.00m },
                new Part { Name = "Płyn do wspomagania (1L)", StockQuantity = 20, UnitPrice = 35.00m },
                new Part { Name = "Olej przekładniowy ATF (4L)", StockQuantity = 15, UnitPrice = 150.00m },

                // Filtry (4)
                new Part { Name = "Filtr oleju - uniwersalny", StockQuantity = 100, UnitPrice = 25.00m },
                new Part { Name = "Filtr powietrza - uniwersalny", StockQuantity = 80, UnitPrice = 35.00m },
                new Part { Name = "Filtr kabinowy", StockQuantity = 60, UnitPrice = 40.00m },
                new Part { Name = "Filtr paliwa", StockQuantity = 40, UnitPrice = 55.00m },

                // Hamulce (4)
                new Part { Name = "Klocki hamulcowe - przód (komplet)", StockQuantity = 30, UnitPrice = 180.00m },
                new Part { Name = "Klocki hamulcowe - tył (komplet)", StockQuantity = 30, UnitPrice = 150.00m },
                new Part { Name = "Tarcza hamulcowa - przód", StockQuantity = 20, UnitPrice = 250.00m },
                new Part { Name = "Tarcza hamulcowa - tył", StockQuantity = 20, UnitPrice = 220.00m },

                // Elektryka (6)
                new Part { Name = "Akumulator 12V 60Ah", StockQuantity = 15, UnitPrice = 350.00m },
                new Part { Name = "Akumulator 12V 72Ah", StockQuantity = 10, UnitPrice = 420.00m },
                new Part { Name = "Świeca zapłonowa - irydowa", StockQuantity = 100, UnitPrice = 45.00m },
                new Part { Name = "Świeca zapłonowa - standardowa", StockQuantity = 80, UnitPrice = 25.00m },
                new Part { Name = "Żarówka H7", StockQuantity = 50, UnitPrice = 30.00m },
                new Part { Name = "Żarówka H4", StockQuantity = 50, UnitPrice = 35.00m },

                // Paski i węże (4)
                new Part { Name = "Zestaw paska rozrządu", StockQuantity = 10, UnitPrice = 280.00m },
                new Part { Name = "Pasek klinowy wielorowkowy", StockQuantity = 25, UnitPrice = 60.00m },
                new Part { Name = "Wąż chłodnicy - górny", StockQuantity = 20, UnitPrice = 45.00m },
                new Part { Name = "Wąż chłodnicy - dolny", StockQuantity = 20, UnitPrice = 50.00m },

                // Zawieszenie (4)
                new Part { Name = "Amortyzator - przód", StockQuantity = 16, UnitPrice = 220.00m },
                new Part { Name = "Amortyzator - tył", StockQuantity = 16, UnitPrice = 180.00m },
                new Part { Name = "Sprężyna zawieszenia - przód", StockQuantity = 12, UnitPrice = 150.00m },
                new Part { Name = "Sprężyna zawieszenia - tył", StockQuantity = 12, UnitPrice = 130.00m },

                // Klimatyzacja (2)
                new Part { Name = "Czynnik chłodniczy R134a (500g)", StockQuantity = 30, UnitPrice = 65.00m },
                new Part { Name = "Termostat", StockQuantity = 20, UnitPrice = 55.00m },

                // Wycieraczki (3)
                new Part { Name = "Wycieraczka 600mm (24\")", StockQuantity = 40, UnitPrice = 35.00m },
                new Part { Name = "Wycieraczka 500mm (20\")", StockQuantity = 40, UnitPrice = 30.00m },
                new Part { Name = "Wycieraczka tylna 400mm (16\")", StockQuantity = 30, UnitPrice = 25.00m }
            };

            context.Parts.AddRange(parts);
            await context.SaveChangesAsync();
        }

        private static async Task SeedDemoDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            var client = await userManager.FindByEmailAsync("client@example.com");
            var mechanic = await userManager.FindByEmailAsync("jan.kowalski@carservice.com");

            if (client == null || mechanic == null)
            {
                return;
            }

            // Sprawdź czy demo pojazdy już istnieją
            if (await context.Vehicles.AnyAsync(v => v.OwnerId == client.Id))
            {
                return; // Już zaseedowane
            }

            // Utwórz 2 demo pojazdy dla klienta
            var vehicle1 = new Vehicle
            {
                Brand = "Toyota",
                Model = "Corolla",
                VIN = "JTDKN3DU5A0123456",
                RegistrationNumber = "WA 12345",
                OwnerId = client.Id
            };

            var vehicle2 = new Vehicle
            {
                Brand = "Volkswagen",
                Model = "Golf",
                VIN = "WVWZZZ3CZWE123456",
                RegistrationNumber = "WA 67890",
                OwnerId = client.Id
            };

            context.Vehicles.AddRange(vehicle1, vehicle2);
            await context.SaveChangesAsync();

            // Pobierz usługi i części do demo zlecenia
            var oilChangeService = await context.Services.FirstOrDefaultAsync(s => s.Name == "Wymiana oleju");
            var diagnosticsService = await context.Services.FirstOrDefaultAsync(s => s.Name == "Diagnostyka komputerowa");
            var oilPart = await context.Parts.FirstOrDefaultAsync(p => p.Name == "Olej silnikowy 5W-30 (5L)");
            var oilFilterPart = await context.Parts.FirstOrDefaultAsync(p => p.Name == "Filtr oleju - uniwersalny");

            if (oilChangeService == null || diagnosticsService == null || oilPart == null || oilFilterPart == null)
            {
                return;
            }

            // Utwórz ukończone zlecenie serwisowe
            var serviceOrder = new ServiceOrder
            {
                VehicleId = vehicle1.Id,
                ClientId = client.Id,
                MechanicId = mechanic.Id,
                Status = ServiceOrderStatus.Completed,
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                CompletedAt = DateTime.UtcNow.AddDays(-5),
                DiagnosticNotes = "Pojazd w dobrym stanie. Wykonano rutynową wymianę oleju i diagnostykę komputerową. Nie wykryto żadnych problemów.",
                LaborHours = 1.5m,
                TotalCost = 0 // Zostanie obliczone
            };

            context.ServiceOrders.Add(serviceOrder);
            await context.SaveChangesAsync();

            // Dodaj pozycje zlecenia
            var items = new List<ServiceOrderItem>
            {
                new ServiceOrderItem
                {
                    ServiceOrderId = serviceOrder.Id,
                    ServiceId = oilChangeService.Id,
                    Quantity = 1,
                    UnitPrice = oilChangeService.Price
                },
                new ServiceOrderItem
                {
                    ServiceOrderId = serviceOrder.Id,
                    ServiceId = diagnosticsService.Id,
                    Quantity = 1,
                    UnitPrice = diagnosticsService.Price
                },
                new ServiceOrderItem
                {
                    ServiceOrderId = serviceOrder.Id,
                    PartId = oilPart.Id,
                    Quantity = 1,
                    UnitPrice = oilPart.UnitPrice
                },
                new ServiceOrderItem
                {
                    ServiceOrderId = serviceOrder.Id,
                    PartId = oilFilterPart.Id,
                    Quantity = 1,
                    UnitPrice = oilFilterPart.UnitPrice
                }
            };

            context.ServiceOrderItems.AddRange(items);

            // Oblicz całkowity koszt
            serviceOrder.TotalCost = items.Sum(i => i.UnitPrice * i.Quantity);
            await context.SaveChangesAsync();

            // Dodaj opinię dla ukończonego zlecenia
            var review = new Review
            {
                ServiceOrderId = serviceOrder.Id,
                Rating = 5,
                Comment = "Świetna obsługa! Zespół był profesjonalny i dokładny. Mój samochód działa płynnie po wymianie oleju. Gorąco polecam!",
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();
        }
    }
}
