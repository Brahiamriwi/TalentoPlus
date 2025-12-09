using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Infrastructure.Services;

public class PdfService : IPdfService
{
    public PdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<byte[]> GenerateResumeAsync(Employee employee)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(c => ComposeHeader(c, employee));
                page.Content().Element(c => ComposeContent(c, employee));
                page.Footer().Element(ComposeFooter);
            });
        });

        var pdfBytes = document.GeneratePdf();
        return Task.FromResult(pdfBytes);
    }

    private void ComposeHeader(IContainer container, Employee employee)
    {
        container.Column(column =>
        {
            column.Item().BorderBottom(2).BorderColor(Colors.Blue.Darken2).PaddingBottom(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text($"{employee.FirstName} {employee.LastName}")
                        .FontSize(24)
                        .Bold()
                        .FontColor(Colors.Blue.Darken2);
                    
                    col.Item().Text(employee.Position)
                        .FontSize(14)
                        .FontColor(Colors.Grey.Darken1);
                });
            });
        });
    }

    private void ComposeContent(IContainer container, Employee employee)
    {
        container.PaddingVertical(20).Column(column =>
        {
            column.Spacing(15);

            // Contact Information
            column.Item().Element(c => ComposeSection(c, "Información de Contacto", content =>
            {
                content.Item().Text($"📧 Email: {employee.Email}");
                content.Item().Text($"📱 Teléfono: {employee.Phone}");
                content.Item().Text($"📍 Dirección: {employee.Address}");
            }));

            // Personal Information
            column.Item().Element(c => ComposeSection(c, "Información Personal", content =>
            {
                content.Item().Text($"Fecha de Nacimiento: {employee.DateOfBirth:dd/MM/yyyy}");
                content.Item().Text($"Nivel Educativo: {GetEducationLevelDisplay(employee.EducationLevel)}");
            }));

            // Work Information
            column.Item().Element(c => ComposeSection(c, "Información Laboral", content =>
            {
                content.Item().Text($"Departamento: {employee.Department?.Name ?? "N/A"}");
                content.Item().Text($"Cargo: {employee.Position}");
                content.Item().Text($"Fecha de Ingreso: {employee.HireDate:dd/MM/yyyy}");
                content.Item().Text($"Estado: {GetStatusDisplay(employee.Status)}");
            }));

            // Professional Profile
            if (!string.IsNullOrWhiteSpace(employee.ProfessionalProfile))
            {
                column.Item().Element(c => ComposeSection(c, "Perfil Profesional", content =>
                {
                    content.Item().Text(employee.ProfessionalProfile).Justify();
                }));
            }
        });
    }

    private void ComposeSection(IContainer container, string title, Action<ColumnDescriptor> contentAction)
    {
        container.Column(column =>
        {
            column.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingBottom(5)
                .Text(title)
                .FontSize(14)
                .Bold()
                .FontColor(Colors.Blue.Darken2);
            
            column.Item().PaddingTop(8).Column(contentAction);
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.Span("Generado por TalentoPlus - ");
            text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
        });
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
}
