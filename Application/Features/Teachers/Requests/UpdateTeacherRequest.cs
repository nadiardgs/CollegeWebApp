using System.Text.Json.Serialization;
using Application.Features.Teachers.Responses;
using MediatR;

namespace Application.Features.Teachers.Requests;

public class UpdateTeacherRequest : IRequest<TeacherDto>
{
    [JsonIgnore]
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
}