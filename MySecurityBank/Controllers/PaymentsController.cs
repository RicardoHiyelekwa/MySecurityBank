using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySecurityBank.Models;
using MySecurityBank.Models.MySecureBank.Models;
using MySecurityBank.Services;
using System.Security.Claims;

namespace MySecurityBank.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IBlobStorageService _blobService;
        private readonly ISwiftValidationService _swiftService;
        private readonly IAuditingService _auditService;

        public PaymentsController(ApplicationDbContext db, IBlobStorageService blobService, ISwiftValidationService swiftService, IAuditingService auditService)
        {
            _db = db;
            _blobService = blobService;
            _swiftService = swiftService;
            _auditService = auditService;
        }

        [HttpGet]
        public IActionResult Create() => View();

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(PaymentViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (!await _swiftService.IsValidAsync(model.SwiftCode))
            {
                ModelState.AddModelError("", "Invalid SWIFT code.");
                return View(model);
            }

            string? attachmentUrl = null;
            if (model.Attachment != null)
            {
                attachmentUrl = await _blobService.UploadAsync(model.Attachment);
            }

            var transaction = new Transaction
            {
                CustomerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value),
                Amount = model.Amount,
                Currency = model.Currency,
                Provider = model.Provider,
                RecipientAccount = model.RecipientAccount,
                SwiftCode = model.SwiftCode,
                AttachmentUrl = attachmentUrl,
                Status = "Pending"
            };

            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync();

            await _auditService.LogAsync($"Payment created by {User.Identity!.Name}", transaction.Id);

            return RedirectToAction("Queue", "Verification");
        }
    }
}