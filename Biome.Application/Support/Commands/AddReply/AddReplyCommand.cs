using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Support.Commands.AddReply;

public sealed record AddReplyCommand(Guid TicketId, Guid UserId, string Content, bool IsFromCustomer) : IRequest<Result>;
