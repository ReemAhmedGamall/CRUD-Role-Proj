using CRUD_Role_Proj.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CRUD_Role_Proj.Controllers
{
    [Authorize] 
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers
                .Include(c => c.Governorate)
                .Include(c => c.District)
                .Include(c => c.Village)
                .Include(c => c.Gender)
                .ToListAsync();
            return View(customers);
        }

        // GET: Customers/Create
        [Authorize(Roles = "Admin,Employee")]
        public IActionResult Create()
        {
            // Fill ViewBags
            ViewBag.Governorates = new SelectList(_context.Governorates, "Id", "Name");
            ViewBag.Districts = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text"); // فارغ لغاية ما يختار محافظة
            ViewBag.Villages = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");
            ViewBag.Genders = new SelectList(_context.Genders, "Id", "Name");

            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
 
        public async Task<IActionResult> Create(Customer customer)
        {
            // Check for NationalId
            if (!string.IsNullOrEmpty(customer.NationalId))
            {
                // Must be 14 digits
                if (!System.Text.RegularExpressions.Regex.IsMatch(customer.NationalId, @"^\d{14}$"))
                {
                    ModelState.AddModelError("NationalId", "National ID must be 14 digits only.");
                }
                else
                {
                    // Get out BirthDate & Age from NationalId and check validity
                    try
                    {
                        string nationalId = customer.NationalId;

                        // century digit
                        var centuryDigit = nationalId[0];
                        string century = centuryDigit == '2' ? "19" :
                                         centuryDigit == '3' ? "20" : null;

                        if (century == null)
                        {
                            ModelState.AddModelError("NationalId", "Invalid National ID century digit.");
                        }
                        else
                        {
                            var year = century + nationalId.Substring(1, 2);
                            var month = nationalId.Substring(3, 2);
                            var day = nationalId.Substring(5, 2);

                            var birthDate = new DateTime(
                                int.Parse(year),
                                int.Parse(month),
                                int.Parse(day)
                            );

                            // check that not future date 
                            if (birthDate > DateTime.Today)
                            {
                                ModelState.AddModelError("NationalId", "Invalid National ID date (cannot be in the future).");
                            }
                            else
                            {
                                customer.BirthDate = birthDate;

                                var age = DateTime.Now.Year - customer.BirthDate.Value.Year;
                                if (DateTime.Now.DayOfYear < customer.BirthDate.Value.DayOfYear)
                                    age--;
                                customer.Age = age;
                            }
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("NationalId", "Invalid National ID date (month/day out of range).");
                    }
                }
            }

            // check uniqueness of NationalId
            if (ModelState.IsValid)
            {
                var existingCustomer = await _context.Customers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.NationalId == customer.NationalId);

                if (existingCustomer != null)
                {
                    ModelState.AddModelError("NationalId", "This National ID is already registered.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        // Ajax for Districts
        public JsonResult GetDistricts(int governorateId)
        {
            var districts = _context.Districts
                .Where(d => d.GovernorateId == governorateId)
                .Select(d => new
                {
                    value = d.Id,
                    text = d.Name
                }).ToList();

            return Json(districts);
        }

        // Ajax for Villages
        public JsonResult GetVillages(int districtId)
        {
            var villages = _context.Villages
                .AsNoTracking()
                .Where(v => v.DistrictId == districtId)
                .Select(v => new
                {
                    value = v.Id,
                    text = v.Name
                }).ToList();

            return Json(villages);
        }

        // GET: Customers/Edit/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Fill dropdowns
            ViewBag.Governorates = new SelectList(_context.Governorates, "Id", "Name", customer.GovernorateId);
            ViewBag.Districts = new SelectList(_context.Districts.Where(d => d.GovernorateId == customer.GovernorateId), "Id", "Name", customer.DistrictId);
            ViewBag.Villages = new SelectList(_context.Villages.Where(v => v.DistrictId == customer.DistrictId), "Id", "Name", customer.VillageId);
            ViewBag.Genders = new SelectList(_context.Genders, "Id", "Name", customer.GenderId);

            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Age & BirthDate not editable — keep the existing
                    var existingCustomer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                    if (existingCustomer == null)
                        return NotFound();

                    customer.NationalId = existingCustomer.NationalId; // Ensure NationalID not changed
                    customer.BirthDate = existingCustomer.BirthDate;   // Ensure BirthDate not changed
                    customer.Age = existingCustomer.Age;               // Ensure Age not changed

                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
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

            // Reload dropdowns if ModelState not valid
            ViewBag.Governorates = new SelectList(_context.Governorates, "Id", "Name", customer.GovernorateId);
            ViewBag.Districts = new SelectList(_context.Districts.Where(d => d.GovernorateId == customer.GovernorateId), "Id", "Name", customer.DistrictId);
            ViewBag.Villages = new SelectList(_context.Villages.Where(v => v.DistrictId == customer.DistrictId), "Id", "Name", customer.VillageId);
            ViewBag.Genders = new SelectList(_context.Genders, "Id", "Name", customer.GenderId);

            return View(customer);
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }


        // GET: Customers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers
                .Include(c => c.Governorate)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (customer == null) return NotFound();

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Print page (Manager + Admin)
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Print(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Governorate)
                .Include(c => c.District)
                .Include(c => c.Village)
                .Include(c => c.Gender)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return NotFound();

            return View(customer);
        }

    }
}
