using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SubmitClaims.Data;
using SubmitClaims.Models;

namespace SubmitClaims.Controllers
{
    public class ClaimsController(
        ApplicationDbContext context,
        IWebHostEnvironment env,
        ILogger<ClaimsController> logger)
        : Controller
    {
        // GET: Claims
        public async Task<IActionResult> Index()
        {
            var claims = await context.LecturerClaims.ToListAsync();
            return View(claims);
        }

        // GET: Claims/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecturerClaim = await context.LecturerClaims.FindAsync(id);
            if (lecturerClaim == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> Create([Bind("LecturerId,HoursWorked,HourlyRate,AdditionalNotes,SubmissionDate,Status")] LecturerClaim lecturerClaim, IFormFile uploadedFile)
        {
            if (!ModelState.IsValid)
            {
                logger.LogDebug("Model validation failed for creating claim.");
                return View(lecturerClaim);
            }

            try
            {
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    if (!IsValidFile(uploadedFile))
                    {
                        ModelState.AddModelError("", "Invalid file type. Allowed types: .pdf, .docx, .xlsx");
                        return View(lecturerClaim);
                    }

                    string uniqueFileName = await SaveUploadedFile(uploadedFile);
                    lecturerClaim.FilePath =  uniqueFileName;
                }

                context.Add(lecturerClaim);
                await context.SaveChangesAsync();
                logger.LogDebug("Claim created successfully with ID: {ClaimId}", lecturerClaim.Id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while creating claim");
                ModelState.AddModelError("", "An error occurred while creating the claim.");
                return View(lecturerClaim);
            }
        }

        // GET: Claims/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecturerClaim = await context.LecturerClaims.FindAsync(id);
            if (lecturerClaim == null)
            {
                return NotFound();
            }

            return View(lecturerClaim);
        }

        // POST: Claims/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LecturerId,HoursWorked,HourlyRate,AdditionalNotes,SubmissionDate,Status")] LecturerClaim lecturerClaim, IFormFile uploadedFile)
        {
            if (id != lecturerClaim.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                logger.LogDebug("Model validation failed for editing claim with ID: {ClaimId}", id);
                return View(lecturerClaim);
            }

            try
            {
                var existingClaim = await context.LecturerClaims.FindAsync(id);
                if (existingClaim == null)
                {
                    return NotFound();
                }

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
                    existingClaim.FilePath = uniqueFileName;  // Save only the filename
                }

                // Update other fields
                existingClaim.LecturerId = lecturerClaim.LecturerId;
                existingClaim.HoursWorked = lecturerClaim.HoursWorked;
                existingClaim.HourlyRate = lecturerClaim.HourlyRate;
                existingClaim.AdditionalNotes = lecturerClaim.AdditionalNotes;
                existingClaim.SubmissionDate = DateTime.Today;
                existingClaim.Status = lecturerClaim.Status;

                await context.SaveChangesAsync();
                logger.LogDebug("Claim updated successfully with ID: {ClaimId}", lecturerClaim.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LecturerClaimExists(lecturerClaim.Id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Error while editing claim");
                ModelState.AddModelError("", "An error occurred while editing the claim.");
                return View(lecturerClaim);
            }
        }

        // GET: Claims/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecturerClaim = await context.LecturerClaims.FindAsync(id);
            if (lecturerClaim == null)
            {
                return NotFound();
            }

            return View(lecturerClaim);
        }

        // POST: Claims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lecturerClaim = await context.LecturerClaims.FindAsync(id);
            if (lecturerClaim != null)
            {
                if (!string.IsNullOrEmpty(lecturerClaim.FilePath))
                {
                    DeleteFile(lecturerClaim.FilePath);
                }

                context.LecturerClaims.Remove(lecturerClaim);
                await context.SaveChangesAsync();
                logger.LogDebug("Claim deleted successfully with ID: {ClaimId}", lecturerClaim.Id);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LecturerClaimExists(int id)
        {
            return context.LecturerClaims.Any(e => e.Id == id);
        }

        // Helper method to save the uploaded file and return only the filename with its extension
        private async Task<string> SaveUploadedFile(IFormFile uploadedFile)
        {
            try
            {
                // string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                // Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = uploadedFile.FileName; // Keeps original name and extension
                string filePath =  uniqueFileName;

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                logger.LogDebug("File uploaded successfully: {FilePath}", uniqueFileName);
                return uniqueFileName;  // Return the filename including its extension
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Error while saving file");
                throw;
            }
        }
        
        // Helper method to delete a file
        private void DeleteFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(env.WebRootPath, "uploads", fileName);
                if (!System.IO.File.Exists(filePath)) return;
                System.IO.File.Delete(filePath);
                logger.LogDebug("File deleted successfully: {FilePath}", fileName);
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Error while deleting file: {FilePath}", fileName);
            }
        }

        // Helper method to validate the uploaded file type
        private bool IsValidFile(IFormFile file)
        {
            var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(fileExtension);
        }
    }
}
