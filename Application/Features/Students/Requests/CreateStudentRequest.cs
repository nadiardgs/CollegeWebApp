using Application.Features.Students.Responses;
using MediatR;

namespace Application.Features.Students.Requests;

public record CreateStudentRequest(string Name) : IRequest<UpsertStudentResponse>;