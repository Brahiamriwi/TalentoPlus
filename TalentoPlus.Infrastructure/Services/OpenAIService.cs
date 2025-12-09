using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Infrastructure.Services;

public class OpenAIService : IOpenAIService
{
    private readonly string _apiKey;

    public OpenAIService(IConfiguration configuration)
    {
        _apiKey = configuration["OpenAI:ApiKey"] ?? string.Empty;
    }

    public async Task<string> ProcessNaturalLanguageQueryAsync(string query, List<Employee> employees)
    {
        if (string.IsNullOrEmpty(_apiKey))
            return "Error: La API Key de OpenAI no está configurada.";

        try
        {
            var client = new ChatClient("gpt-4o-mini", _apiKey);

            var employeesData = BuildEmployeesContext(employees);
            
           var systemPrompt = @"Eres un asistente de Recursos Humanos para el sistema TalentoPlus. 
Tu función es analizar consultas sobre empleados y responder ÚNICAMENTE basándote en los datos proporcionados.

Reglas importantes:
1. Solo responde con información que esté presente en los datos de empleados proporcionados.
2. Si la información solicitada no está disponible en los datos, indica que no tienes esa información.
3. Responde siempre en español de manera clara y profesional.
4. Puedes hacer cálculos como promedios, conteos, sumas, etc. basándote en los datos.
5. Si te preguntan por un empleado específico, busca coincidencias por nombre, apellido o email.
6. Los salarios están en pesos colombianos (COP).
7. **IMPORTANTE: Sé breve y directo. No muestres el proceso de cálculo ni listes todos los datos. Solo da la respuesta final.**
8. **Máximo 2-3 oraciones en tu respuesta.**

Datos de empleados disponibles:
" + employeesData;
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(query)
            };

            var response = await client.CompleteChatAsync(messages);

            return response.Value.Content[0].Text ?? "No se pudo generar una respuesta.";
        }
        catch (Exception ex)
        {
            return $"Error al procesar la consulta: {ex.Message}";
        }
    }

    private static string BuildEmployeesContext(List<Employee> employees)
    {
        if (employees == null || employees.Count == 0)
            return "No hay empleados registrados en el sistema.";

        var sb = new StringBuilder();
        sb.AppendLine($"Total de empleados: {employees.Count}");
        sb.AppendLine();

        foreach (var emp in employees)
        {
            sb.AppendLine($"- Nombre: {emp.FirstName} {emp.LastName}");
            sb.AppendLine($"  Email: {emp.Email}");
            sb.AppendLine($"  Cargo: {emp.Position}");
            sb.AppendLine($"  Departamento: {emp.Department?.Name ?? "Sin asignar"}");
            sb.AppendLine($"  Salario: ${emp.Salary:N0} COP");
            sb.AppendLine($"  Fecha de Ingreso: {emp.HireDate:dd/MM/yyyy}");
            sb.AppendLine($"  Estado: {GetStatusDisplay(emp.Status)}");
            sb.AppendLine($"  Nivel Educativo: {GetEducationLevelDisplay(emp.EducationLevel)}");
            sb.AppendLine($"  Teléfono: {emp.Phone}");
            sb.AppendLine($"  Fecha de Nacimiento: {emp.DateOfBirth:dd/MM/yyyy}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string GetStatusDisplay(Core.Enums.EmployeeStatus status)
    {
        return status switch
        {
            Core.Enums.EmployeeStatus.Active => "Activo",
            Core.Enums.EmployeeStatus.Inactive => "Inactivo",
            Core.Enums.EmployeeStatus.OnVacation => "En Vacaciones",
            _ => status.ToString()
        };
    }

    private static string GetEducationLevelDisplay(Core.Enums.EducationLevel level)
    {
        return level switch
        {
            Core.Enums.EducationLevel.HighSchool => "Bachiller",
            Core.Enums.EducationLevel.Technical => "Técnico",
            Core.Enums.EducationLevel.Technologist => "Tecnólogo",
            Core.Enums.EducationLevel.Bachelor => "Profesional",
            Core.Enums.EducationLevel.Specialization => "Especialización",
            Core.Enums.EducationLevel.Master => "Maestría",
            Core.Enums.EducationLevel.Doctorate => "Doctorado",
            _ => level.ToString()
        };
    }
}
