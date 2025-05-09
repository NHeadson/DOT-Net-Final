using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "employee")]
public class DiscountController : Controller
{
    private readonly DataContext _dataContext;
    private readonly UserManager<AppUser> _userManager;

    public DiscountController(DataContext dataContext, UserManager<AppUser> userManager)
    {
        _dataContext = dataContext;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var discounts = _dataContext.Discounts.Include(d => d.Product).ToList();
        return View(discounts);
    }

    public IActionResult Create() => View(new Discount());

    [HttpPost, ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Discount discount)
{
    if (ModelState.IsValid)
    {
        discount.Code = new Random().Next(1000, 9999); // Generate random code
        _dataContext.Discounts.Add(discount);
        await _dataContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    return View(discount);
}

[HttpPost, ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(Discount discount)
{
    if (ModelState.IsValid)
    {
        _dataContext.Discounts.Update(discount);
        await _dataContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    return View(discount);
}

    public IActionResult Delete(int id) => View(_dataContext.Discounts.FirstOrDefault(d => d.DiscountId == id));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var discount = _dataContext.Discounts.FirstOrDefault(d => d.DiscountId == id);
        if (discount != null)
        {
            _dataContext.Discounts.Remove(discount);
            await _dataContext.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
