using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SubmitClaims.Models;

namespace SubmitClaims.Controllers
{
    public class SubmitClaim(ClaimDbContext context) : Controller
    {
        // GET: SubmitClaim
        public async Task<IActionResult> Index()
        {
            return View(await context.Claims.ToListAsync());
        }

        // GET: SubmitClaim/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await context.Claims
                .FirstOrDefaultAsync(m => m.Id == id);
            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }

        // GET: SubmitClaim/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SubmitClaim/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LecturerId,HoursWorked,HourlyRate,AdditionalNotes,SubmissionDate,Status,FilePath")] Claim claim)
        {
            if (ModelState.IsValid)
            {
                context.Add(claim);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(claim);
        }

        // GET: SubmitClaim/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }
            return View(claim);
        }

        // POST: SubmitClaim/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LecturerId,HoursWorked,HourlyRate,AdditionalNotes,SubmissionDate,Status,FilePath")] Claim claim)
        {
            if (id != claim.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(claim);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClaimExists(claim.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(claim);
        }

        // GET: SubmitClaim/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await context.Claims
                .FirstOrDefaultAsync(m => m.Id == id);
            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }

        // POST: SubmitClaim/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var claim = await context.Claims.FindAsync(id);
            if (claim != null)
            {
                context.Claims.Remove(claim);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClaimExists(int id)
        {
            return context.Claims.Any(e => e.Id == id);
        }
    }
}
