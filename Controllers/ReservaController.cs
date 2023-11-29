using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
[Route("Reserva")]
[ApiController]
public class ReservaController : ControllerBase
{
    private readonly string _connectionString;
    public ReservaController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("MySqlConnection");
    }

    [HttpGet]
    public async Task<IActionResult> ListarReservas()
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM Reserva";

                List<Reserva> reservas = new List<Reserva>();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        reservas.Add(new Reserva
                        {
                            idReserva = reader.GetInt32(0),
                            Especialidad = reader.GetString(1),
                            DiaReserva = reader.GetString(2),
                            Paciente_idPaciente = reader.GetInt32(3)
                        });
                    }
                }
                return StatusCode(200, reservas);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "No se pudo listar los registros por: " + ex);
        }
    }
    [HttpGet("{idReserva}")]
    public async Task<IActionResult> ObtenerReserva(int id)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Reserva WHERE idReserva = @id";

                Reserva reserva = new Reserva();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            reserva.idReserva = reader.GetInt32(0);
                            reserva.Especialidad = reader.GetString(1);
                            reserva.DiaReserva = reader.GetString(2);
                            reserva.Paciente_idPaciente = reader.GetInt32(3);
                            return StatusCode(200, reserva);
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
    public async Task<IActionResult> CrearReserva([FromBody] Reserva reserva)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO Reserva (Especialidad, DiaReserva, Paciente_idPaciente) VALUES (@Especialidad, @DiaReserva, @Paciente_idPaciente)";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Especialidad", reserva.Especialidad);
                    command.Parameters.AddWithValue("@DiaReserva", reserva.DiaReserva);
                    command.Parameters.AddWithValue("@Paciente_idPaciente", reserva.Paciente_idPaciente);

                    await command.ExecuteNonQueryAsync();
                    return StatusCode(201, $"Reserva creada correctamente: {reserva}");
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "No se pudo guardar el registro por: " + ex);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditarReserva(int id, [FromBody] Reserva reserva)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE Reserva SET Especialidad = @Especialidad, DiaReserva = @DiaReserva, Paciente_idPaciente = @Paciente_idPaciente WHERE idReserva = @id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Especialidad", reserva.Especialidad);
                    command.Parameters.AddWithValue("@DiaReserva", reserva.DiaReserva);
                    command.Parameters.AddWithValue("@Paciente_idPaciente", reserva.Paciente_idPaciente);
                    command.Parameters.AddWithValue("@id", id);

                    await command.ExecuteNonQueryAsync();
                    return StatusCode(200, "Registro editado correctamente");
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "No se pudo editar la reserva por: " + ex);
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarReserva(int id)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM Reserva WHERE idReserva = @id";
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
                        return StatusCode(200, $"Reserva con el ID {id} eliminada correctamente");
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
