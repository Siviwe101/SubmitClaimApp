using Microsoft.AspNetCore.Mvc;
using SubmitClaims.Data;
using SubmitClaims.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace SubmitClaims.Controllers
{
    public class ClaimController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment env;

        public ClaimController(ApplicationDbContext dbContext, IWebHostEnvironment env)
        {
            this.dbContext = dbContext;
            this.env = env;
        }

        [HttpGet]
        public IActionResult SubmitClaim()
        {
            ViewBag.Claims = dbContext.LecturerClaims.ToList(); // Fetch all claims to display in the view
            return View(new LecturerClaim()); // Return a new LecturerClaim for the form
        }

        [HttpPost]
        public async Task<IActionResult> SubmitClaim(LecturerClaim claim, IFormFile? uploadedFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Claims = dbContext.LecturerClaims.ToList(); // Refresh the claims list on validation error
                return View(claim);
            }

            // Ensure the file is uploaded and saved
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder); // Ensure the uploads directory exists

                var filePath = Path.Combine(uploadsFolder, uploadedFile.FileName);
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(stream);
                }

                // Set the file path in the claim object
                claim.FilePath = Path.Combine("uploads", uploadedFile.FileName); // Store relative path for web access
            }

            // Save the claim to the database
            dbContext.LecturerClaims.Add(claim);
            await dbContext.SaveChangesAsync();

            // Use TempData to store a success message for redirect
            TempData["Message"] = "Claim submitted successfully!";
            return RedirectToAction("SubmitClaim"); // Redirect to clear the form and display the updated claims list
        }

    }
}
