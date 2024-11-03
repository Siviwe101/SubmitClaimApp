using Microsoft.AspNetCore.Mvc;
using SubmitClaims.Data;
using SubmitClaims.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace SubmitClaims.Controllers
{
    public class ClaimController(ApplicationDbContext dbContext, IWebHostEnvironment env) : Controller
    {
        [HttpGet]
        public IActionResult SubmitClaim()
        {
            return View(new LecturerClaim());
        }

        [HttpPost]
        public async Task<IActionResult> SubmitClaim(LecturerClaim claim, IFormFile? uploadedFile)
        {
            if (!ModelState.IsValid) return View();
            // Handle file upload
            if (uploadedFile is { Length: > 0 })
            {
                var uploadsFolder = Path.Combine(env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists

                var filePath = Path.Combine(uploadsFolder, uploadedFile.FileName);
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(stream);
                }

                claim.FilePath = filePath;
            }

            dbContext.LecturerClaims.Add(claim);
            await dbContext.SaveChangesAsync();

            ViewBag.Message = "Claim submitted successfully!";
            return View();

        }
    }
}