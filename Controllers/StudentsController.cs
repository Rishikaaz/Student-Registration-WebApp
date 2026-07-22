using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentRegistrationWebApp.Data;
using StudentRegistrationWebApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StudentRegistrationWebApp.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly StudentCourseDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StudentsController(StudentCourseDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .Include(s => s.Course)
                .Include(s => s.User)
                .ToListAsync();
            return View(students);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Profile()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _context.Students
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return NotFound("Student profile not found.");
            }

            return View(student);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> EditProfile()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null)
            {
                return NotFound("Student profile not found.");
            }
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> EditProfile([Bind("StudentId,FullName,City")] Student studentModel)
        {
            var userId = _userManager.GetUserId(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null || student.StudentId != studentModel.StudentId)
            {
                return BadRequest("Invalid request.");
            }

            if (ModelState.IsValid)
            {
                student.FullName = studentModel.FullName;
                student.City = studentModel.City;

                _context.Update(student);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profile details updated successfully!";
                return RedirectToAction(nameof(Profile));
            }
            return View(studentModel);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> RegisterCourse()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _context.Students
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return NotFound("Student profile not found.");
            }

            if (student.CourseId != null)
            {
                TempData["ErrorMessage"] = $"You are already registered for the course '{student.Course?.CourseName}'. You can only register for one course.";
                return RedirectToAction(nameof(Profile));
            }

            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> RegisterCourse(int courseId)
        {
            var userId = _userManager.GetUserId(User);
            var student = await _context.Students
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return NotFound("Student profile not found.");
            }

            if (student.CourseId != null)
            {
                TempData["ErrorMessage"] = "You can only register for one course.";
                return RedirectToAction(nameof(Profile));
            }

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound("Course not found.");
            }

            student.CourseId = courseId;
            _context.Update(student);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully registered for course '{course.CourseName}'!";
            return RedirectToAction(nameof(Profile));
        }
    }
}
