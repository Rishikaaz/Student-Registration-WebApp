using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentRegistrationWebApp.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        [Range(1, 48)]
        [Display(Name = "Duration (Months)")]
        public int CourseDuration { get; set; }

        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
