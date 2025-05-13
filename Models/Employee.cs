using System.ComponentModel.DataAnnotations;

public class Employee
{
    public int EmployeeId { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Email { get; set; }
    public string Phone { get; set; }
}
