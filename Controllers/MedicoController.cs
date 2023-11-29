 using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

// Controlador Medico
[Route("Medico")]
[ApiController]
public class MedicoController : ControllerBase
{
    // Conexion
    private readonly string _connectionString;
    public MedicoController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("MySqlConnection");
    }

    // Para traer Resultados GET
    [HttpGet]
    public async Task<IActionResult> ListarMedicos()
    {
        try
        {
            // Traer la conexion
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                //Consulta a la base de datos
                string query = "SELECT * FROM Medico";

                // Traer lista de medico
                List<Medico> medicos = new List<Medico>();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (var Lector = await command.ExecuteReaderAsync())
                {
                    //Ciclo para poner los datos de la consulta a la base de datos
                    while (await Lector.ReadAsync())
                    {
                        medicos.Add(new Medico
                        {
                            idMedico = Lector.GetInt32(0),
                            NombreMed = Lector.GetString(1),
                            ApellidoMed = Lector.GetString(2),
                            RunMed = Lector.GetString(3),
                            Eunacom = Lector.GetString(4),
                            NacionalidadMed = Lector.GetString(5),
                            Especialidad = Lector.GetString(6),
                            horarios = Lector.GetString(7),
                            TarifaHr = Lector.GetInt32(8)
                        });
                    }
                }

                //Que es efectivo si es 200
                return StatusCode(200, medicos);
            }
        }
        catch (Exception ex)
        {
            // Traer el error 500, y el por que.
            return StatusCode(500, "No se pudo listar los registros por: " + ex);
        }
    }

    //Traer Medico por ID
    [HttpGet("{idMedico}")]
    public async Task<IActionResult> ObtenerMedico(int id)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                //Consulta a la base de datos
                string Consulta = "SELECT * FROM Medico WHERE idMedico = @id";

                //Guardar data
                Medico medico = new Medico();

                using (MySqlCommand command = new MySqlCommand(Consulta, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
}
                    using (var Lector = await command.ExecuteReaderAsync())
                    {
                        if (await Lector.ReadAsync())
                        {
                            //Rellenar datos segun medico
                            medico.idMedico = Lector.GetInt32(0);
                            medico.NombreMed = Lector.GetString(1);
                            medico.ApellidoMed = Lector.GetString(2);
                            medico.RunMed = Lector.GetString(3);
                            medico.Eunacom = Lector.GetString(4);
                            medico.NacionalidadMed = Lector.GetString(5);
                            medico.Especialidad = Lector.GetString(6);
                            medico.horarios = Lector.GetString(7);
                            medico.TarifaHr = Lector.GetInt32(8);

                            //Devuelve estado 200 (Correcto) y trae la data
                            return StatusCode(200, medico);
                        }
                        else
                        {
                            //Entrega 404 (Que no existe), y entrega mensaje de que no hay
                            return StatusCode(404, "No se encuentra el registro");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //Error 500, y da mensaje y problema
            return StatusCode(500, "No se puede realizar la petición por: " + ex);
        }
    }

    //Para crear un nuevo médico
    [HttpPost]
    public async Task<IActionResult> Nuevomedico([FromBody] Medico medico)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string Consulta = "INSERT INTO Medico (NombreMed, ApellidoMed, RunMed, Eunacom, NacionalidadMed, Especialidad, horarios, TarifaHr) VALUES (@NombreMed, @ApellidoMed, @RunMed, @Eunacom, @NacionalidadMed, @Especialidad, @horarios, @TarifaHr)";

                using (MySqlCommand command = new MySqlCommand(Consulta, connection))
                {
                    //Entregar codigos
                    command.Parameters.AddWithValue("@NombreMed", medico.NombreMed);
                    command.Parameters.AddWithValue("@ApellidoMed", medico.ApellidoMed);
                    command.Parameters.AddWithValue("@RunMed", medico.RunMed);
                    command.Parameters.AddWithValue("@Eunacom", medico.Eunacom);
                    command.Parameters.AddWithValue("@NacionalidadMed", medico.NacionalidadMed);
                    command.Parameters.AddWithValue("@Especialidad", medico.Especialidad);
                    command.Parameters.AddWithValue("@horarios", medico.horarios);
                    command.Parameters.AddWithValue("@TarifaHr", medico.TarifaHr);
                    await command.ExecuteNonQueryAsync();

                    //Trae Estado 201, y muestra
                    return StatusCode(201, $"Medico creado correctamente: {medico}");
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "No se pudo guardar el registro por: " + ex);
        }
    }

    // Para editar
    [HttpPut("{id}")]
    public async Task<IActionResult> ModificarMedico(int id, [FromBody] Medico medico)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string consulta = "UPDATE Medico SET NombreMed = @NombreMed, ApellidoMed = @ApellidoMed, RunMed = @RunMed, Eunacom = @Eunacom, NacionalidadMed = @NacionalidadMed, Especialidad = @Especialidad, horarios = @horarios, TarifaHr = @TarifaHr WHERE idMedico = @id";
                using (MySqlCommand command = new MySqlCommand(consulta, connection))
                {
                    command.Parameters.AddWithValue("@NombreMed", medico.NombreMed);
                    command.Parameters.AddWithValue("@ApellidoMed", medico.ApellidoMed);
                    command.Parameters.AddWithValue("@RunMed", medico.RunMed);
                    command.Parameters.AddWithValue("@Eunacom", medico.Eunacom);
                    command.Parameters.AddWithValue("@NacionalidadMed", medico.NacionalidadMed);
                    command.Parameters.AddWithValue("@Especialidad", medico.Especialidad);
                    command.Parameters.AddWithValue("@horarios", medico.horarios);
                    command.Parameters.AddWithValue("@TarifaHr", medico.TarifaHr);
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                    return StatusCode(200, "Registro editado correctamente");
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "No se pudo editar el medico por: " + ex);
        }
    }

    //Para eliminar
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarMedico(int id)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string Consulta = "DELETE FROM Medico WHERE idMedico = @id";

                using (MySqlCommand command = new MySqlCommand(Consulta, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    var ELim = await command.ExecuteNonQueryAsync();

                    if (ELim == 0)
                    {
                        return StatusCode(404, "Registro no encontrado!!!");
                    }
                    else
                    {
                        return StatusCode(200, $"Medico con el ID {id} eliminado correctamente");
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
