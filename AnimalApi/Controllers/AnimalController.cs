using AnimalApi.DataBase;
using AnimalApi.Models;
using AnimalApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;


namespace AnimalApi.Controllers;

[ApiController]
[Route("/api/animals")]
public class AnimalController : Controller
{
    private readonly IAnimalDataBase _animalDataBase;

    public AnimalController(IAnimalDataBase animalDataBase)
    {
        _animalDataBase = animalDataBase;
    }

    [HttpGet]
    public IActionResult GetAllAnimals([FromQuery] string orderBy = "name")
    {
        IEnumerable<Animal> animals = _animalDataBase.GetAllAnimalsOrderBy(orderBy);
        return Ok(animals);
    }

    [HttpPost]
    public IActionResult AddAnimal([FromBody] AddAnimal? animal)
    {
        if (animal == null)
        {
            return BadRequest("Animal data is missing");
        }

        _animalDataBase.AddAnimal(animal);
        return Created("", null);
    }

    [HttpPut("{id}")]
    public IActionResult EditAnimal(int id, [FromBody] Animal? animal)
    {
        if (animal == null)
        {
            return BadRequest("Animal data is missing");
        }

        Animal? editedAnimal = _animalDataBase.EditAnimal(id, animal);
        if (editedAnimal == null)
        {
            return NotFound($"Animal with ID {id} not found");
        }

        return Ok(editedAnimal);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteAnimal(int id)
    {
        bool success = _animalDataBase.DeleteAnimal(id);
        if (!success)
        {
            return NotFound($"Animal with ID {id} not found");
        }

        return NoContent();
    }


}