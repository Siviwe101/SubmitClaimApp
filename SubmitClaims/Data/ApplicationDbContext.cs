using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SubmitClaims.Models;

namespace SubmitClaims.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<LecturerClaim> LecturerClaims { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        
        public DbSet<Invoice> Invoices { get; set; }
        
    }
}