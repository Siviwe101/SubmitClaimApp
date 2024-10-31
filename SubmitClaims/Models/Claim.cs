using Microsoft.EntityFrameworkCore;

namespace SubmitClaims.Models;

public class ClaimDbContext(DbContextOptions<ClaimDbContext> options) : DbContext(options)
{
    public DbSet<Claim> Claims { get; set; }
}
public class Claim
{
    public int Id { get; set; }
    public int LecturerId { get; set; }
    public double HoursWorked { get; set; }
    public double HourlyRate { get; set; }
    public string AdditionalNotes { get; set; }
    public DateTime SubmissionDate { get; set; }
    public string Status { get; set; } // "Pending", "Approved", "Rejected"
    public string FilePath { get; set; } // For storing file path
}
