using CarService.Models;
using CarService.Models.Enums;
using CarService.Models.ViewModels;
using CarService.Models.ViewModels.Mechanic;
using CarService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarService.Controllers
{
    [Authorize(Roles = "Mechanic")]
    public class WorkOrdersController : Controller
    {
        private readonly IServiceOrderService _orderService;
        private readonly IServiceCatalogService _serviceCatalogService;
        private readonly IPartService _partService;
        private readonly UserManager<ApplicationUser> _userManager;

        public WorkOrdersController(
            IServiceOrderService orderService,
            IServiceCatalogService serviceCatalogService,
            IPartService partService,
            UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _serviceCatalogService = serviceCatalogService;
            _partService = partService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var mechanicId = _userManager.GetUserId(User)!;
            var orders = await _orderService.GetByMechanicAsync(mechanicId);

            var viewModel = orders.Select(o => new ServiceOrderSummaryViewModel
            {
                Id = o.Id,
                VehicleInfo = $"{o.Vehicle?.Brand} {o.Vehicle?.Model} ({o.Vehicle?.RegistrationNumber})",
                Status = o.Status,
                StatusBadgeClass = GetStatusBadgeClass(o.Status),
                CreatedAt = o.CreatedAt,
                TotalCost = o.TotalCost
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var mechanicId = _userManager.GetUserId(User)!;
            
            if (!await _orderService.IsAssignedToMechanicAsync(id, mechanicId))
                return Forbid();

            var order = await _orderService.GetByIdWithDetailsAsync(id);
            if (order == null) return NotFound();

            var viewModel = MapToDetailsViewModel(order);
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var mechanicId = _userManager.GetUserId(User)!;
            
            if (!await _orderService.IsAssignedToMechanicAsync(id, mechanicId))
                return Forbid();

            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            var viewModel = new UpdateStatusViewModel
            {
                OrderId = id,
                CurrentStatus = order.Status,
                AvailableStatuses = GetAllowedStatusTransitions(order.Status)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(UpdateStatusViewModel model)
        {
            var mechanicId = _userManager.GetUserId(User)!;
            
            if (!await _orderService.IsAssignedToMechanicAsync(model.OrderId, mechanicId))
                return Forbid();

            var order = await _orderService.GetByIdAsync(model.OrderId);
            if (order == null) return NotFound();

            // Validate status transition
            var allowedTransitions = GetAllowedStatusTransitions(order.Status);
            if (!allowedTransitions.Any(s => s.Value == ((int)model.NewStatus).ToString()))
            {
                ModelState.AddModelError("NewStatus", "Nieprawidłowa zmiana statusu.");
            }

            // Validate completion requirements
            if (model.NewStatus == ServiceOrderStatus.Completed)
            {
                var canComplete = await _orderService.CanCompleteAsync(model.OrderId);
                if (!canComplete)
                {
                    ModelState.AddModelError("NewStatus", "Nie można zakończyć zlecenia bez co najmniej jednej pozycji.");
                }
            }

            if (!ModelState.IsValid)
            {
                model.CurrentStatus = order.Status;
                model.AvailableStatuses = GetAllowedStatusTransitions(order.Status);
                return View(model);
            }

            await _orderService.UpdateStatusAsync(model.OrderId, model.NewStatus);
            return RedirectToAction(nameof(Details), new { id = model.OrderId });
        }

        [HttpGet]
        public async Task<IActionResult> AddNotes(int id)
        {
            var mechanicId = _userManager.GetUserId(User)!;
            
            if (!await _orderService.IsAssignedToMechanicAsync(id, mechanicId))
                return Forbid();

            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            var viewModel = new AddNotesViewModel
            {
                OrderId = id,
                ExistingNotes = order.DiagnosticNotes,
                Notes = order.DiagnosticNotes ?? string.Empty
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNotes(AddNotesViewModel model)
        {
            var mechanicId = _userManager.GetUserId(User)!;
            
            if (!await _orderService.IsAssignedToMechanicAsync(model.OrderId, mechanicId))
                return Forbid();

            if (!ModelState.IsValid)
            {
                var order = await _orderService.GetByIdAsync(model.OrderId);
                model.ExistingNotes = order?.DiagnosticNotes;
                return View(model);
            }

            await _orderService.AddDiagnosticNotesAsync(model.OrderId, model.Notes);
            return RedirectToAction(nameof(Details), new { id = model.OrderId });
        }

        [HttpGet]
        public async Task<IActionResult> AddParts(int id)
        {
            var mechanicId = _userManager.GetUserId(User)!;
            
            if (!await _orderService.IsAssignedToMechanicAsync(id, mechanicId))
                return Forbid();

            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            if (order.Status == ServiceOrderStatus.Completed || order.Status == ServiceOrderStatus.Cancelled)
            {
                TempData["Error"] = "Nie można dodać części do zlecenia zakończonego lub anulowanego.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var parts = await _partService.GetInStockAsync();
            var viewModel = new AddPartItemViewModel
            {
                OrderId = id,
                AvailableParts = parts.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} - {p.UnitPrice:C} (Stan: {p.StockQuantity})"
                })
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddParts(AddPartItemViewModel model)
        {
            var mechanicId = _userManager.GetUserId(User)!;
            
            if (!await _orderService.IsAssignedToMechanicAsync(model.OrderId, mechanicId))
                return Forbid();

            var order = await _orderService.GetByIdAsync(model.OrderId);
            if (order == null) return NotFound();

            if (order.Status == ServiceOrderStatus.Completed || order.Status == ServiceOrderStatus.Cancelled)
            {
                TempData["Error"] = "Nie można dodać części do zlecenia zakończonego lub anulowanego.";
                return RedirectToAction(nameof(Details), new { id = model.OrderId });
            }

            // Validate stock
            var hasSufficientStock = await _partService.HasSufficientStockAsync(model.PartId, model.Quantity);
            if (!hasSufficientStock)
            {
                ModelState.AddModelError("Quantity", "Niewystarczająca ilość na stanie dla wybranej części.");
            }

            if (!ModelState.IsValid)
            {
                var parts = await _partService.GetInStockAsync();
                model.AvailableParts = parts.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} - {p.UnitPrice:C} (Stan: {p.StockQuantity})"
                });
                return View(model);
            }

            await _orderService.AddPartItemAsync(model.OrderId, model.PartId, model.Quantity);
            return RedirectToAction(nameof(Details), new { id = model.OrderId });
        }

        [HttpGet]
        public async Task<IActionResult> AddService(int id)
        {
            var mechanicId = _userManager.GetUserId(User)!;
            
            if (!await _orderService.IsAssignedToMechanicAsync(id, mechanicId))
                return Forbid();

            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            if (order.Status == ServiceOrderStatus.Completed || order.Status == ServiceOrderStatus.Cancelled)
            {
                TempData["Error"] = "Nie można dodać usług do zlecenia zakończonego lub anulowanego.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var services = await _serviceCatalogService.GetActiveAsync();
            var viewModel = new AddServiceItemViewModel
            {
                OrderId = id,
                AvailableServices = services.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.Name} - {s.Price:C}"
                })
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddService(AddServiceItemViewModel model)
        {
            var mechanicId = _userManager.GetUserId(User)!;
            
            if (!await _orderService.IsAssignedToMechanicAsync(model.OrderId, mechanicId))
                return Forbid();

            var order = await _orderService.GetByIdAsync(model.OrderId);
            if (order == null) return NotFound();

            if (order.Status == ServiceOrderStatus.Completed || order.Status == ServiceOrderStatus.Cancelled)
            {
                TempData["Error"] = "Nie można dodać usług do zlecenia zakończonego lub anulowanego.";
                return RedirectToAction(nameof(Details), new { id = model.OrderId });
            }

            if (!ModelState.IsValid)
            {
                var services = await _serviceCatalogService.GetActiveAsync();
                model.AvailableServices = services.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.Name} - {s.Price:C}"
                });
                return View(model);
            }

            await _orderService.AddServiceItemAsync(model.OrderId, model.ServiceId, model.Quantity);
            return RedirectToAction(nameof(Details), new { id = model.OrderId });
        }

        [HttpGet]
        public async Task<IActionResult> SetLaborHours(int id)
        {
            var mechanicId = _userManager.GetUserId(User)!;
            
            if (!await _orderService.IsAssignedToMechanicAsync(id, mechanicId))
                return Forbid();

            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            var viewModel = new SetLaborHoursViewModel
            {
                OrderId = id,
                CurrentHours = order.LaborHours,
                Hours = order.LaborHours > 0 ? order.LaborHours : 0.25m
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetLaborHours(SetLaborHoursViewModel model)
        {
            var mechanicId = _userManager.GetUserId(User)!;
            
            if (!await _orderService.IsAssignedToMechanicAsync(model.OrderId, mechanicId))
                return Forbid();

            if (!ModelState.IsValid)
            {
                var order = await _orderService.GetByIdAsync(model.OrderId);
                model.CurrentHours = order?.LaborHours ?? 0;
                return View(model);
            }

            await _orderService.SetLaborHoursAsync(model.OrderId, model.Hours);
            return RedirectToAction(nameof(Details), new { id = model.OrderId });
        }

        #region Helper Methods

        private static string GetStatusBadgeClass(ServiceOrderStatus status)
        {
            return status switch
            {
                ServiceOrderStatus.Pending => "bg-warning text-dark",
                ServiceOrderStatus.Accepted => "bg-primary",
                ServiceOrderStatus.InProgress => "bg-info",
                ServiceOrderStatus.Completed => "bg-success",
                ServiceOrderStatus.Cancelled => "bg-secondary",
                _ => "bg-secondary"
            };
        }

        private static IEnumerable<SelectListItem> GetAllowedStatusTransitions(ServiceOrderStatus currentStatus)
        {
            var transitions = new List<SelectListItem>();

            switch (currentStatus)
            {
                case ServiceOrderStatus.Pending:
                    transitions.Add(new SelectListItem { Value = ((int)ServiceOrderStatus.Accepted).ToString(), Text = "Przyjmij" });
                    transitions.Add(new SelectListItem { Value = ((int)ServiceOrderStatus.Cancelled).ToString(), Text = "Anuluj" });
                    break;
                case ServiceOrderStatus.Accepted:
                    transitions.Add(new SelectListItem { Value = ((int)ServiceOrderStatus.InProgress).ToString(), Text = "Rozpocznij pracę" });
                    transitions.Add(new SelectListItem { Value = ((int)ServiceOrderStatus.Cancelled).ToString(), Text = "Anuluj" });
                    break;
                case ServiceOrderStatus.InProgress:
                    transitions.Add(new SelectListItem { Value = ((int)ServiceOrderStatus.Completed).ToString(), Text = "Zakończ" });
                    break;
            }

            return transitions;
        }

        private WorkOrderDetailsViewModel MapToDetailsViewModel(ServiceOrder order)
        {
            return new WorkOrderDetailsViewModel
            {
                Id = order.Id,
                VehicleInfo = $"{order.Vehicle?.Brand} {order.Vehicle?.Model} ({order.Vehicle?.RegistrationNumber})",
                ClientName = $"{order.Client?.FirstName} {order.Client?.LastName}",
                ClientEmail = order.Client?.Email ?? string.Empty,
                Status = order.Status,
                StatusBadgeClass = GetStatusBadgeClass(order.Status),
                CreatedAt = order.CreatedAt,
                CompletedAt = order.CompletedAt,
                TotalCost = order.TotalCost,
                DiagnosticNotes = order.DiagnosticNotes,
                LaborHours = order.LaborHours,
                Items = order.Items.Select(i => new ServiceOrderItemViewModel
                {
                    Name = i.Service?.Name ?? i.Part?.Name ?? "Nieznane",
                    Type = i.ServiceId.HasValue ? "Usługa" : "Część",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.Quantity * i.UnitPrice
                }).ToList(),
                CanUpdateStatus = order.Status != ServiceOrderStatus.Completed && order.Status != ServiceOrderStatus.Cancelled,
                CanAddItems = order.Status != ServiceOrderStatus.Completed && order.Status != ServiceOrderStatus.Cancelled
            };
        }

        #endregion
    }
}
