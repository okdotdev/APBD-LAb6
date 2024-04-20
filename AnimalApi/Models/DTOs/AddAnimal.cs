using System.ComponentModel.DataAnnotations;

namespace AnimalApi.Models.DTOs;

public class AddAnimal
{
    [MaxLength(20)]
    [MinLength(3)]
    [Required]
    public string Name { get; set; }
    public string? Description{ get; set; }
    public string Category { get; set; }
    public string Area { get; set; }

}