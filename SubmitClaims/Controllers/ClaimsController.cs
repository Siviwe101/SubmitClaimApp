using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubmitClaims.Data;
using SubmitClaims.Models;

namespace SubmitClaims.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ClaimsController> _logger;

        public ClaimsController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            ILogger<ClaimsController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        // GET: Claims
        public async Task<IActionResult> Index()
        {
            var claims = await _context.LecturerClaims.ToListAsync();
            return View(claims);
        }

        // GET: Claims/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var lecturerClaim = await _context.LecturerClaims.FindAsync(id);
            if (lecturerClaim == null) return NotFound();

            return View(lecturerClaim);
        }

        // GET: Claims/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Claims/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("LecturerId,HoursWorked,HourlyRate,AdditionalNotes,SubmissionDate,Status")]
            LecturerClaim lecturerClaim, IFormFile uploadedFile)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogDebug("Model validation failed for creating claim.");
                return View(lecturerClaim);
            }

            try
            {
                // File handling
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    if (!IsValidFile(uploadedFile))
                    {
                        ModelState.AddModelError("", "Invalid file type. Allowed types: .pdf, .docx, .xlsx");
                        return View(lecturerClaim);
                    }

                    string uniqueFileName = await SaveUploadedFile(uploadedFile);
                    lecturerClaim.FilePath = uniqueFileName;
                }

                lecturerClaim.Status = "Pending";  // Default status for a new claim
                _context.Add(lecturerClaim);
                await _context.SaveChangesAsync();
                _logger.LogDebug("Claim created successfully with ID: {ClaimId}", lecturerClaim.Id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating claim");
                ModelState.AddModelError("", "An error occurred while creating the claim.");
                return View(lecturerClaim);
            }
        }

        // GET: Claims/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var lecturerClaim = await _context.LecturerClaims.FindAsync(id);
            if (lecturerClaim == null) return NotFound();

            return View(lecturerClaim);
        }

        // POST: Claims/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,LecturerId,HoursWorked,HourlyRate,AdditionalNotes,SubmissionDate,Status")]
            LecturerClaim lecturerClaim, IFormFile uploadedFile)
        {
            if (id != lecturerClaim.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                _logger.LogDebug("Model validation failed for editing claim with ID: {ClaimId}", id);
                return View(lecturerClaim);
            }

            try
            {
                var existingClaim = await _context.LecturerClaims.FindAsync(id);
                if (existingClaim == null) return NotFound();

                // File handling
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    if (!IsValidFile(uploadedFile))
                    {
                        ModelState.AddModelError("", "Invalid file type. Allowed types: .pdf, .docx, .xlsx");
                        return View(lecturerClaim);
                    }

                    if (!string.IsNullOrEmpty(existingClaim.FilePath))
                    {
                        DeleteFile(existingClaim.FilePath);
                    }

                    string uniqueFileName = await SaveUploadedFile(uploadedFile);
                    existingClaim.FilePath = uniqueFileName;
                }

                // Update other fields
                existingClaim.LecturerId = lecturerClaim.LecturerId;
                existingClaim.HoursWorked = lecturerClaim.HoursWorked;
                existingClaim.HourlyRate = lecturerClaim.HourlyRate;
                existingClaim.AdditionalNotes = lecturerClaim.AdditionalNotes;
                existingClaim.SubmissionDate = lecturerClaim.SubmissionDate;
                existingClaim.Status = lecturerClaim.Status;

                await _context.SaveChangesAsync();
                _logger.LogDebug("Claim updated successfully with ID: {ClaimId}", lecturerClaim.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LecturerClaimExists(lecturerClaim.Id)) return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while editing claim");
                ModelState.AddModelError("", "An error occurred while editing the claim.");
                return View(lecturerClaim);
            }
        }

        // GET: Claims/ManageClaims
        public async Task<IActionResult> ManageClaims()
        {
            var claims = await _context.LecturerClaims.ToListAsync();
            return View(claims);
        }

        // POST: Claims/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _context.LecturerClaims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = "Approved";
            _context.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageClaims));
        }

        // POST: Claims/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var claim = await _context.LecturerClaims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = "Rejected";
            _context.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageClaims));
        }

        // Helper method to save the uploaded file and return only the filename with its extension
        private async Task<string> SaveUploadedFile(IFormFile uploadedFile)
        {
            try
            {
               
                string uniqueFileName = uploadedFile.FileName;
                string filePath = Path.Combine(uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                return uniqueFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving file");
                throw;
            }
        }
        // GET: Claims/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecturerClaim = await _context.LecturerClaims.FindAsync(id);
            if (lecturerClaim == null)
            {
                return NotFound();
            }

            return View(lecturerClaim); // Shows the delete confirmation view
        }

        // POST: Claims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lecturerClaim = await _context.LecturerClaims.FindAsync(id);
            if (lecturerClaim != null)
            {
                // Delete the associated file if it exists
                if (!string.IsNullOrEmpty(lecturerClaim.FilePath))
                {
                    DeleteFile(lecturerClaim.FilePath);
                }

                _context.LecturerClaims.Remove(lecturerClaim); // Remove the claim
                await _context.SaveChangesAsync(); // Commit the deletion
                _logger.LogDebug("Claim deleted successfully with ID: {ClaimId}", lecturerClaim.Id);
            }
            return RedirectToAction(nameof(Index)); // Redirect to Index after deletion
        }

        // Helper method to delete a file
        private void DeleteFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    _logger.LogDebug("File deleted successfully: {FilePath}", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting file: {FilePath}", fileName);
            }
        }

        // Helper method to validate the uploaded file type
        private bool IsValidFile(IFormFile file)
        {
            var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(fileExtension);
        }

        private bool LecturerClaimExists(int id) => _context.LecturerClaims.Any(e => e.Id == id);
    }
}
