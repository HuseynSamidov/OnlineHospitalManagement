namespace Application.DTOs.CategoryDTOs.Create;

public class UpdateProcedureDto
{
    public Guid DepartmentId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}
