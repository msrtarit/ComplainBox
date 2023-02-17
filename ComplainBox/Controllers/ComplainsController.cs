using System;
using System.Linq;
using System.Threading.Tasks;
using ComplainBox.Data;
using ComplainBox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ComplainBox.Controllers
{
    [Authorize]
    public class ComplainsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ComplainsController(ApplicationDbContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Complains
        
        public async Task<IActionResult> Index(int? pageNo)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Count == 0)
            {
                return View(await PaginatedList<Complain>.ApplyPagingAsync(_context.Complains.Where(p=>p.ComplainedBy.Equals(user.UserName))
                    .OrderByDescending(p => p.ComplainTime), pageNo ?? 1, 20));
            }
            return View(await PaginatedList<Complain>.ApplyPagingAsync(_context.Complains.Where(p=> userRoles.Any(q=>q.Equals(p.ComplainTo)))
                .OrderByDescending(p => p.ComplainTime), pageNo ?? 1, 20));
        }

        // GET: Complains/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var complain = await _context.Complains
                .FirstOrDefaultAsync(m => m.Id == id);
            if (complain == null)
            {
                return NotFound();
            }

            return View(complain);
        }

        // GET: Complains/Create
        [AllowAnonymous]
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await _roleManager.Roles.Select(p=> new SelectListItem{ Text = p.Name, Value = p.Name}).ToListAsync();
            return View();
        }

        // POST: Complains/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create(Complain complain)
        {
            if (ModelState.IsValid)
            {
                complain.Id = Guid.NewGuid();
                complain.ComplainTime = DateTime.UtcNow;
                _context.Add(complain);
                await _context.SaveChangesAsync();

                ViewBag.ErrorTitle = "Successfully Submitted!";
                ViewBag.ErrorMessage = "We will reach to your contact as soon as possible. Thank you.";
                return View("MessagePop");
            }
            return View(complain);
        }

        //// GET: Complains/Edit/5
        //public async Task<IActionResult> Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var complain = await _context.Complains.FindAsync(id);
        //    if (complain == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(complain);
        //}

        //// POST: Complains/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, Complain complain)
        //{
        //    if (id != complain.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(complain);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ComplainExists(complain.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(complain);
        //}

        //// GET: Complains/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var complain = await _context.Complains
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (complain == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(complain);
        //}

        //// POST: Complains/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var complain = await _context.Complains.FindAsync(id);
        //    _context.Complains.Remove(complain);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        
        public async Task<IActionResult> MarkAsResolved(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var complain = await _context.Complains.FindAsync(id);

            if (complain == null)
            {
                return NotFound();
            }
            return await UpdateStatus(complain, true);
        }
        
        public async Task<IActionResult> MarkAsReturn(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var complain = await _context.Complains.FindAsync(id);

            if (complain == null)
            {
                return NotFound();
            }
            return await UpdateStatus(complain, false);
        }

        private async Task<IActionResult> UpdateStatus(Complain complain, bool status)
        {
            complain.IsResolved = status;
            try
            {
                _context.Update(complain);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComplainExists(complain.Id))
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

        private bool ComplainExists(Guid id)
        {
            return _context.Complains.Any(e => e.Id == id);
        }
    }
}
