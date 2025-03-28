using Frelance.Application.Mediatr.Commands.Users;
using Frelance.Contracts.Dtos;

namespace Frelance.Application.Repositories;

public interface IAccountRepository
{
    Task RegisterAsync(CreateUserCommand createUserCommand);
    Task<UserDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);
    Task LockAccountAsync(BlockAccountCommand command);
    Task DeleteAccountAsync(DeleteAccountCommand command);
}