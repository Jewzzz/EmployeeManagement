using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[HttpGet]
        public IActionResult Index(string query)
        {

            // Use a single query to improve performance and maintainability
            var employees = _context.Employees.AsQueryable();

            // Apply the filter if the query is provided
            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.ToLower(); // Normalize the query for case-insensitive search

                employees = employees.Where(e => EF.Functions.Like(e.Name.ToLower(), $"%{query}%")
                                                 || EF.Functions.Like(e.Email.ToLower(), $"%{query}%"));

                var empData = employees.ToList();

                return Json( new { success = true, data = empData } );
            }

            //return View(_employees.ToList());
            return View(employees.ToList());

        }

        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.CreatedAt = DateTime.Now;
                _context.Employees.Add(employee);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message="Validation failed" });
        }

        [HttpGet]
        public IActionResult GetEmployeeById(int id)
        {
            var employee = _context.Employees.Find(id);

            if (employee == null) {
                return NotFound();
            }
            return Json(new { success = true, data = employee });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee == null) { 
                return NotFound();
            } 
            _context.Employees.Remove(employee);
            _context.SaveChanges();

            return Json(new { success = true });

        }
    }
}
