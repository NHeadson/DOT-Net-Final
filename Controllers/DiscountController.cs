using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "employee")]
public class DiscountController : Controller
{
    private readonly DataContext _dataContext;

    public DiscountController(DataContext db)
    {
        _dataContext = db;
    }

    public IActionResult Index()
    {
        var discounts = _dataContext.Discounts.Include(d => d.Product).ToList();
        return View(discounts);
    }

    public IActionResult Create()
    {
        ViewBag.Products = _dataContext.Products.ToList();
        return View(new Discount());
    }

    [HttpPost]
    public IActionResult Create(Discount discount)
    {
        if (ModelState.IsValid)
        {
            discount.DiscountPercent /= 100;

            discount.Code = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();

            _dataContext.Discounts.Add(discount);
            _dataContext.SaveChanges();
            return RedirectToAction("Index");
        }
        ViewBag.Products = _dataContext.Products.ToList();
        return View(discount);
    }

    public IActionResult Edit(int id)
    {
        var discount = _dataContext.Discounts.FirstOrDefault(d => d.DiscountId == id);
        if (discount == null) return NotFound();
        ViewBag.Products = _dataContext.Products.ToList();
        return View(discount);
    }

    [HttpPost]
    public IActionResult Edit(Discount discount)
    {
        if (ModelState.IsValid)
        {
            var existingDiscount = _dataContext.Discounts.FirstOrDefault(d => d.DiscountId == discount.DiscountId);
            if (existingDiscount != null)
            {
                existingDiscount.Title = discount.Title;
                existingDiscount.ProductId = discount.ProductId;
                existingDiscount.DiscountPercent = discount.DiscountPercent / 100;
                existingDiscount.StartTime = discount.StartTime;
                existingDiscount.EndTime = discount.EndTime;
                existingDiscount.Description = discount.Description;

                _dataContext.SaveChanges();
            }
            else
        {
            return NotFound();
        }

            return RedirectToAction("Index");
        }

        ViewBag.Products = _dataContext.Products.ToList();
        return View(discount);
    }

    public IActionResult Delete(int id)
    {
        var discount = _dataContext.Discounts.Include(d => d.Product).FirstOrDefault(d => d.DiscountId == id);
        if (discount == null)
        {
            return NotFound();
        }
        return View(discount);
    }

    [HttpPost]
    [ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        Console.WriteLine($"DeleteConfirmed called with id: {id}"); // Debugging log

        var discount = _dataContext.Discounts.FirstOrDefault(d => d.DiscountId == id);
        if (discount != null)
        {
            _dataContext.Discounts.Remove(discount);
            _dataContext.SaveChanges();
        }
        return RedirectToAction("Index");
    }
}
