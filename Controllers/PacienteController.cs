using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
[Route("Paciente")]
[ApiController]
public class PacienteController : ControllerBase
{
    private readonly string _connectionString;

    public PacienteController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("MySqlConnection");
    }
    [HttpGet]
    public async Task<IActionResult> ListarPacientes()
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Paciente";
                List<Paciente> pacientes = new List<Paciente>();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        pacientes.Add(new Paciente
                        {
                            idPaciente = reader.GetInt32(0),
                            NombrePac = reader.GetString(1),
                            ApellidoPac = reader.GetString(2),
                            RunPac = reader.GetString(3),
                            Nacionalidad = reader.GetString(4),
                            Visa = reader.GetString(5),
                            Genero = reader.GetString(6),
                            SistomasPac = reader.GetString(7),
                            Medico_idMedico = reader.GetInt32(8)
                        });
                    }
                }

                return StatusCode(200, pacientes);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "No se pudo listar los registros por: " + ex);
        }
    }

    [HttpGet("{idPaciente}")]
    public async Task<IActionResult> ObtenerPaciente(int id)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Paciente WHERE idPaciente = @id";

                Paciente paciente = new Paciente();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            paciente.idPaciente = reader.GetInt32(0);
                            paciente.NombrePac = reader.GetString(1);
                            paciente.ApellidoPac = reader.GetString(2);
                            paciente.RunPac = reader.GetString(3);
                            paciente.Nacionalidad = reader.GetString(4);
                            paciente.Visa = reader.GetString(5);
                            paciente.Genero = reader.GetString(6);
                            paciente.SistomasPac = reader.GetString(7);
                            paciente.Medico_idMedico = reader.GetInt32(8);

                            return StatusCode(200, paciente);
                        }
                        else
                        {
                            return StatusCode(404, "No se encuentra el registro");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "No se puede realizar la petición por: " + ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CrearPaciente([FromBody] Paciente paciente)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO Paciente (NombrePac, ApellidoPac, RunPac, Nacionalidad, Visa, Genero, SistomasPac, Medico_idMedico) VALUES (@NombrePac, @ApellidoPac, @RunPac, @Nacionalidad, @Visa, @Genero, @SistomasPac, @Medico_idMedico)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NombrePac", paciente.NombrePac);
                    command.Parameters.AddWithValue("@ApellidoPac", paciente.ApellidoPac);
                    command.Parameters.AddWithValue("@RunPac", paciente.RunPac);
                    command.Parameters.AddWithValue("@Nacionalidad", paciente.Nacionalidad);
                    command.Parameters.AddWithValue("@Visa", paciente.Visa);
                    command.Parameters.AddWithValue("@Genero", paciente.Genero);
                    command.Parameters.AddWithValue("@SistomasPac", paciente.SistomasPac);
                    command.Parameters.AddWithValue("@Medico_idMedico", paciente.Medico_idMedico);

                    await command.ExecuteNonQueryAsync();
                    return StatusCode(201, $"Paciente creado correctamente: {paciente}");
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "No se pudo guardar el registro por: " + ex);
        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> EditarPaciente(int id, [FromBody] Paciente paciente)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE Paciente SET NombrePac = @NombrePac, ApellidoPac = @ApellidoPac, RunPac = @RunPac, Nacionalidad = @Nacionalidad, Visa = @Visa, Genero = @Genero, SistomasPac = @SistomasPac, Medico_idMedico = @Medico_idMedico WHERE idPaciente = @id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NombrePac", paciente.NombrePac);
                    command.Parameters.AddWithValue("@ApellidoPac", paciente.ApellidoPac);
                    command.Parameters.AddWithValue("@RunPac", paciente.RunPac);
                    command.Parameters.AddWithValue("@Nacionalidad", paciente.Nacionalidad);
                    command.Parameters.AddWithValue("@Visa", paciente.Visa);
                    command.Parameters.AddWithValue("@Genero", paciente.Genero);
                    command.Parameters.AddWithValue("@SistomasPac", paciente.SistomasPac);
                    command.Parameters.AddWithValue("@Medico_idMedico", paciente.Medico_idMedico);
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                    return StatusCode(200, "Registro editado correctamente");
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "No se pudo editar el paciente por: " + ex);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarPaciente(int id)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM Paciente WHERE idPaciente = @id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    var borrado = await command.ExecuteNonQueryAsync();
                    if (borrado == 0)
                    {
                        return StatusCode(404, "Registro no encontrado!!!");
                    }
                    else
                    {
                        return StatusCode(200, $"Paciente con el ID {id} eliminado correctamente");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "No se pudo eliminar el registro por: " + ex);
        }
    }
}
