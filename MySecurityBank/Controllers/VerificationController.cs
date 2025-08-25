using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySecurityBank.Models;
using MySecurityBank.Services;

namespace MySecurityBank.Controllers
{
    [Authorize]
 
    public class VerificationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ISwiftValidationService _swiftService;
        private readonly IAuditingService _auditService;

        public VerificationController(ApplicationDbContext db, ISwiftValidationService swiftService, IAuditingService auditService)
        {
            _db = db;
            _swiftService = swiftService;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IActionResult> Queue()
        {
            var pending = await _db.Transactions.Where(t => t.Status == "Pending").ToListAsync();
            return View(pending);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var tx = await _db.Transactions.FindAsync(id);
            if (tx == null) return NotFound();

            if (!await _swiftService.IsValidAsync(tx.SwiftCode)) return BadRequest();

            tx.Status = "Approved";
            await _db.SaveChangesAsync();

            await _auditService.LogAsync($"Transaction {id} approved by staff {User.Identity!.Name}");

            return RedirectToAction("Dashboard", "Home");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var tx = await _db.Transactions.FindAsync(id);
            if (tx == null) return NotFound();

            tx.Status = "Rejected";
            await _db.SaveChangesAsync();

            await _auditService.LogAsync($"Transaction {id} rejected by staff {User.Identity!.Name}");

            return RedirectToAction("Dashboard", "Home");
        }
    }
}