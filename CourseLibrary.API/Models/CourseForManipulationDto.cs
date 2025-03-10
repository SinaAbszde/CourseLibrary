using System.ComponentModel.DataAnnotations;
using CourseLibrary.API.Entities;

namespace CourseLibrary.API.Models;

public abstract class CourseForManipulationDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(100, ErrorMessage = "The title cannot have more than 100 characters")]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(1500, ErrorMessage = "The description cannot have more than 1500 characters")]
    public virtual string Description { get; set; } = string.Empty;
}