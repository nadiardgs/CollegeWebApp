using System.Text.Json.Serialization;
using Application.Features.Students.Responses;
using MediatR;

namespace Application.Features.Students.Requests;

public class UpdateStudentRequest : IRequest<UpsertStudentResponse>
{
    [JsonIgnore]
    public int Id { get; set; } 
    public string Name { get; set; } = string.Empty;
}