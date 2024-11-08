using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Castle.Components.DictionaryAdapter;

namespace SubmitClaims.Models;

public class LecturerClaim
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LecturerId { get; set; }
    [Microsoft.Build.Framework.Required]
    [Range(1, 100, ErrorMessage = "Hours worked must be between 1 and 100.")]
    public double HoursWorked { get; set; }
    [Microsoft.Build.Framework.Required]
    [Range(10, 500, ErrorMessage = "Hourly rate must be between 10 and 500.")]
    public double HourlyRate { get; set; }
    public string AdditionalNotes { get; set; }
    public DateTime SubmissionDate { get; set; }
    [DefaultValue("Pending")]  // Set default value to "Pending"
    public string Status { get; set; } = "Pending"; // Default status value --- "Pending", "Approved", "Rejected"
    public string? FilePath { get; set; } // For storing file path
    public decimal FinalPayment => (decimal)(HoursWorked * HourlyRate);

}