using System.Data;
using AnimalApi.Models;
using AnimalApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace AnimalApi.DataBase;

 public class AnimalDataBase : IAnimalDataBase
    {
        private readonly IConfiguration _configuration;

        public AnimalDataBase(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetOpenConnection()
        {
            SqlConnection connection = new(_configuration.GetConnectionString("Default"));
            connection.Open();
            return connection;
        }

        public IEnumerable<Animal> GetAnimals()
        {
            using SqlConnection connection = GetOpenConnection();
            using SqlCommand? command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Animal;";

            using SqlDataReader? reader = command.ExecuteReader();
            List<Animal> animals = new(capacity: 10);

            while (reader.Read())
            {
                animals.Add(MapToAnimal(reader));
            }

            return animals;
        }

        public IEnumerable<Animal> GetAllAnimalsOrderBy(string orderBy = "name")
        {
            using SqlConnection connection = GetOpenConnection();
            using SqlCommand? command = connection.CreateCommand();

            string orderByColumn = orderBy.ToLower();
            if (orderByColumn != "name" && orderByColumn != "description" && orderByColumn != "category" &&
                orderByColumn != "area")
            {
                orderByColumn = "name";
            }

            command.CommandText = $"SELECT * FROM Animal ORDER BY {orderByColumn};";

            using SqlDataReader? reader = command.ExecuteReader();
            List<Animal> animals = new List<Animal>(capacity: 10);

            while (reader.Read())
            {
                animals.Add(MapToAnimal(reader));
            }

            return animals;
        }

        public void AddAnimal(AddAnimal animal)
        {
            using SqlConnection connection = GetOpenConnection();
            using SqlCommand? command = connection.CreateCommand();
            command.CommandText =
                "INSERT INTO Animal (Name, Description, Category, Area) VALUES (@Name, @Description, @Category, @Area)";
            command.Parameters.AddWithValue("@Name", animal.Name);
            command.Parameters.AddWithValue("@Description", (object)animal.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Category", (object)animal.Category);
            command.Parameters.AddWithValue("@Area", (object)animal.Area);
            command.ExecuteNonQuery();
        }

        public bool DeleteAnimal(int id)
        {
            using SqlConnection connection = GetOpenConnection();
            using SqlCommand? command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Animal WHERE IdAnimal = @Id";
            command.Parameters.AddWithValue("@Id", id);
            return command.ExecuteNonQuery() > 0;
        }

        public Animal? EditAnimal(int id, [FromBody] Animal animal)
        {
            using SqlConnection connection = GetOpenConnection();
            using SqlCommand? command = connection.CreateCommand();
            command.CommandText =
                "UPDATE Animal SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @Id";

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", animal.Name);
            command.Parameters.AddWithValue("@Description", (object)animal.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Category", (object)animal.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@Area", (object)animal.Area ?? DBNull.Value);

            command.ExecuteNonQuery();

            return GetAnimalById(id);
        }

        private Animal? GetAnimalById(int id)
        {
            using SqlConnection connection = GetOpenConnection();
            using SqlCommand? command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Animal WHERE IdAnimal = @Id";
            command.Parameters.AddWithValue("@Id", id);

            using SqlDataReader? reader = command.ExecuteReader();
            return reader.Read() ? MapToAnimal(reader) : null;
        }

        private Animal MapToAnimal(IDataRecord dataRecord)
        {
            if (dataRecord == null) throw new ArgumentNullException(nameof(dataRecord));
            return new Animal
            {
                IdAnimal = dataRecord.GetInt32(dataRecord.GetOrdinal("IdAnimal")),
                Name = dataRecord.GetString(dataRecord.GetOrdinal("Name")),
                Description = dataRecord.IsDBNull(dataRecord.GetOrdinal("Description")) ? null : dataRecord.GetString(dataRecord.GetOrdinal("Description")),
                Category = dataRecord.IsDBNull(dataRecord.GetOrdinal("Category")) ? null : dataRecord.GetString(dataRecord.GetOrdinal("Category")),
                Area = dataRecord.IsDBNull(dataRecord.GetOrdinal("Area")) ? null : dataRecord.GetString(dataRecord.GetOrdinal("Area"))
            };
        }
    }
