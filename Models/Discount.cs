using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Discount
{
    public int DiscountId { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    public Product Product { get; set; }
    [Required]
    [Range(0, 100, ErrorMessage = "Discount Percent must be between 0 and 100.")]
    public double DiscountPercent { get; set; }
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime EndTime { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
}
