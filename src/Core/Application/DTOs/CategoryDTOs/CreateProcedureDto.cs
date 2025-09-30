namespace Application.DTOs.CategoryDTOs;

public class CreateProcedureDto
{
    public Guid DepartmentId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}
