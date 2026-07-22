using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentRegistrationWebApp.Models;

namespace StudentRegistrationWebApp.Data
{
    public class StudentCourseDbContext : IdentityDbContext
    {
        public StudentCourseDbContext(DbContextOptions<StudentCourseDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Student>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Course>().HasData(
                new Course { CourseId = 1, CourseName = "ASP.NET Core MVC", CourseDuration = 3 },
                new Course { CourseId = 2, CourseName = "Angular", CourseDuration = 2 }
            );
        }
    }
}
