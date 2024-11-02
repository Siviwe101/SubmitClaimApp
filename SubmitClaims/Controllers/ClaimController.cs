using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SubmitClaims.Data;
using SubmitClaims.Models;

namespace SubmitClaims.Controllers;

public class ClaimController(ApplicationDbContext dbContext, IWebHostEnvironment env) : Controller
{
    [HttpPost]
    public async Task<IActionResult> SubmitClaim(LecturerClaim claim, IFormFile? uploadedFile)
    {
        if (ModelState.IsValid)
        {
            // Handle file upload
            if (uploadedFile is { Length: > 0 })
            {
                var filePath = Path.Combine(env.WebRootPath, "uploads", uploadedFile.FileName);
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(stream);
                }
                claim.FilePath = filePath; // Assign file path to Claim's FilePath property
            }

            dbContext.LecturerClaims.Add(claim);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("SubmitClaim");
        }
        return View(claim);
    }
}