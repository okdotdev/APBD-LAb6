using AnimalApi.Models;
using AnimalApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AnimalApi.DataBase;

public interface IAnimalDataBase
{
    IEnumerable<Animal> GetAllAnimalsOrderBy(string sortCategory);
    void AddAnimal(AddAnimal? animal);
    bool DeleteAnimal(int id);
    Animal? EditAnimal(int id,[FromBody]Animal? animal);
}