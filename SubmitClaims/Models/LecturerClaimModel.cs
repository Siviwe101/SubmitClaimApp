using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Castle.Components.DictionaryAdapter;
using Microsoft.Build.Framework;

namespace SubmitClaims.Models;

public class LecturerClaim
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LecturerId { get; set; }
    [Required]
    public double HoursWorked { get; set; }
    [Required]
    public double HourlyRate { get; set; }
    public string AdditionalNotes { get; set; }
    public DateTime SubmissionDate { get; set; }
    [DefaultValue("Pending")]  // Set default value to "Pending"
    public string Status { get; set; } = "Pending"; // Default status value --- "Pending", "Approved", "Rejected"
    public string? FilePath { get; set; } // For storing file path
}