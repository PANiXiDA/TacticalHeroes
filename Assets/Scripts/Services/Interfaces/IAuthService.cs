using Assets.Scripts.Infrastructure.Requests.AuthService;
using Assets.Scripts.Infrastructure.Responses;
using Assets.Scripts.Infrastructure.Responses.AuthService;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Services.Interfaces
{
    public interface IAuthService
    {
        UniTask<EmptyResponse> Registration(RegistrationRequest request);
        UniTask<LoginResponse> Login(LoginRequest request);
        UniTask<EmptyResponse> ConfirmEmail(ConfirmEmailRequest request);
        UniTask<EmptyResponse> RecoveryPassword(RecoveryPasswordRequest request);
    }
}
