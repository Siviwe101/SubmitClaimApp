namespace SubmitClaims.Models;

public class ClaimModel
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