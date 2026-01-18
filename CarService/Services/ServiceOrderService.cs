using CarService.Data;
using CarService.Models;
using CarService.Models.Enums;
using CarService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarService.Services
{
    public class ServiceOrderService : IServiceOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ServiceOrderService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<ServiceOrder>> GetAllAsync()
        {
            return await _context.ServiceOrders
                .Include(o => o.Vehicle)
                .Include(o => o.Client)
                .Include(o => o.Mechanic)
                .Include(o => o.Items)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceOrder>> GetByClientAsync(string clientId)
        {
            return await _context.ServiceOrders
                .Include(o => o.Vehicle)
                .Include(o => o.Mechanic)
                .Include(o => o.Items)
                .Where(o => o.ClientId == clientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceOrder>> GetByMechanicAsync(string mechanicId)
        {
            return await _context.ServiceOrders
                .Include(o => o.Vehicle)
                .Include(o => o.Client)
                .Include(o => o.Items)
                .Where(o => o.MechanicId == mechanicId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceOrder>> GetByStatusAsync(ServiceOrderStatus status)
        {
            return await _context.ServiceOrders
                .Include(o => o.Vehicle)
                .Include(o => o.Client)
                .Include(o => o.Mechanic)
                .Where(o => o.Status == status)
                .ToListAsync();
        }

        public async Task<ServiceOrder?> GetByIdAsync(int id)
        {
            return await _context.ServiceOrders
                .Include(o => o.Vehicle)
                .Include(o => o.Client)
                .Include(o => o.Mechanic)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<ServiceOrder?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.ServiceOrders
                .Include(o => o.Vehicle)
                .Include(o => o.Client)
                .Include(o => o.Mechanic)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Service)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Part)
                .Include(o => o.Review)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<ServiceOrder> CreateAsync(ServiceOrder order)
        {
            order.Status = ServiceOrderStatus.Pending;
            order.CreatedAt = DateTime.UtcNow;
            _context.ServiceOrders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task AssignMechanicAsync(int orderId, string mechanicId)
        {
            var order = await _context.ServiceOrders.FindAsync(orderId);
            if (order == null) throw new InvalidOperationException("Nie znaleziono zlecenia");

            var mechanic = await _userManager.FindByIdAsync(mechanicId);
            if (mechanic == null) throw new InvalidOperationException("Nie znaleziono mechanika");

            var roles = await _userManager.GetRolesAsync(mechanic);
            if (!roles.Contains("Mechanic"))
                throw new InvalidOperationException("Użytkownik nie jest mechanikiem");

            order.MechanicId = mechanicId;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int orderId, ServiceOrderStatus status)
        {
            var order = await _context.ServiceOrders.FindAsync(orderId);
            if (order == null) throw new InvalidOperationException("Nie znaleziono zlecenia");

            if (!IsValidStatusTransition(order.Status, status))
                throw new InvalidOperationException($"Nie można zmienić statusu z {order.Status} na {status}");

            if (status == ServiceOrderStatus.Completed)
            {
                if (!await CanCompleteAsync(orderId))
                    throw new InvalidOperationException("Nie można zakończyć zlecenia bez co najmniej jednej usługi lub części");
                order.CompletedAt = DateTime.UtcNow;
            }

            order.Status = status;

            await _context.SaveChangesAsync();
        }

        private static bool IsValidStatusTransition(ServiceOrderStatus current, ServiceOrderStatus next)
        {
            return (current, next) switch
            {
                (ServiceOrderStatus.Pending, ServiceOrderStatus.Accepted) => true,
                (ServiceOrderStatus.Pending, ServiceOrderStatus.Cancelled) => true,
                (ServiceOrderStatus.Accepted, ServiceOrderStatus.InProgress) => true,
                (ServiceOrderStatus.Accepted, ServiceOrderStatus.Cancelled) => true,
                (ServiceOrderStatus.InProgress, ServiceOrderStatus.Completed) => true,
                (ServiceOrderStatus.InProgress, ServiceOrderStatus.Cancelled) => true,
                _ => false
            };
        }

        public async Task AddDiagnosticNotesAsync(int orderId, string notes)
        {
            var order = await _context.ServiceOrders.FindAsync(orderId);
            if (order == null) throw new InvalidOperationException("Nie znaleziono zlecenia");

            order.DiagnosticNotes = notes;
            await _context.SaveChangesAsync();
        }

        public async Task SetLaborHoursAsync(int orderId, decimal hours)
        {
            var order = await _context.ServiceOrders.FindAsync(orderId);
            if (order == null) throw new InvalidOperationException("Nie znaleziono zlecenia");

            order.LaborHours = hours;
            await _context.SaveChangesAsync();
        }

        public async Task AddServiceItemAsync(int orderId, int serviceId, int quantity)
        {
            var order = await _context.ServiceOrders.FindAsync(orderId);
            if (order == null) throw new InvalidOperationException("Nie znaleziono zlecenia");

            var service = await _context.Services.FindAsync(serviceId);
            if (service == null) throw new InvalidOperationException("Nie znaleziono usługi");

            var item = new ServiceOrderItem
            {
                ServiceOrderId = orderId,
                ServiceId = serviceId,
                Quantity = quantity,
                UnitPrice = service.Price
            };

            _context.ServiceOrderItems.Add(item);
            
            // Update total cost on the tracked order entity
            order.TotalCost += item.Quantity * item.UnitPrice;
            
            await _context.SaveChangesAsync();
        }

        public async Task AddPartItemAsync(int orderId, int partId, int quantity)
        {
            var order = await _context.ServiceOrders.FindAsync(orderId);
            if (order == null) throw new InvalidOperationException("Nie znaleziono zlecenia");

            var part = await _context.Parts.FindAsync(partId);
            if (part == null) throw new InvalidOperationException("Nie znaleziono części");

            if (part.StockQuantity < quantity)
                throw new InvalidOperationException($"Niewystarczająca ilość na stanie dla części '{part.Name}'. Dostępne: {part.StockQuantity}, wymagane: {quantity}");

            part.StockQuantity -= quantity;

            var item = new ServiceOrderItem
            {
                ServiceOrderId = orderId,
                PartId = partId,
                Quantity = quantity,
                UnitPrice = part.UnitPrice
            };

            _context.ServiceOrderItems.Add(item);
            
            // Update total cost on the tracked order entity
            order.TotalCost += item.Quantity * item.UnitPrice;
            
            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(int itemId)
        {
            var item = await _context.ServiceOrderItems
                .Include(i => i.Part)
                .Include(i => i.ServiceOrder)
                .FirstOrDefaultAsync(i => i.Id == itemId);
            
            if (item == null) return;

            if (item.PartId.HasValue && item.Part != null)
            {
                item.Part.StockQuantity += item.Quantity;
            }

            // Update total cost on the tracked order entity
            if (item.ServiceOrder != null)
            {
                item.ServiceOrder.TotalCost -= item.Quantity * item.UnitPrice;
                if (item.ServiceOrder.TotalCost < 0) item.ServiceOrder.TotalCost = 0;
            }

            _context.ServiceOrderItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> CalculateTotalCostAsync(int orderId)
        {
            var items = await _context.ServiceOrderItems
                .Where(i => i.ServiceOrderId == orderId)
                .Select(i => new { i.Quantity, i.UnitPrice })
                .ToListAsync();
            
            return items.Sum(i => i.Quantity * i.UnitPrice);
        }

        public async Task UpdateTotalCostAsync(int orderId)
        {
            var order = await _context.ServiceOrders.FindAsync(orderId);
            if (order == null) return;

            order.TotalCost = await CalculateTotalCostAsync(orderId);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CanCompleteAsync(int orderId)
        {
            return await _context.ServiceOrderItems
                .AnyAsync(i => i.ServiceOrderId == orderId);
        }

        public async Task<bool> BelongsToClientAsync(int orderId, string clientId)
        {
            return await _context.ServiceOrders
                .AnyAsync(o => o.Id == orderId && o.ClientId == clientId);
        }

        public async Task<bool> IsAssignedToMechanicAsync(int orderId, string mechanicId)
        {
            return await _context.ServiceOrders
                .AnyAsync(o => o.Id == orderId && o.MechanicId == mechanicId);
        }
    }
}
