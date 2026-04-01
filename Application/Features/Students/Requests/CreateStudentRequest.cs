using Application.Features.Students.Responses;
using MediatR;

namespace Application.Entities.Students.Requests;

public record CreateStudentRequest(string Name) : IRequest<UpsertStudentResponse>;