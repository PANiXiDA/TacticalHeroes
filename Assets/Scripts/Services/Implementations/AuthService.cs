using Assets.Scripts.Services.Interfaces;
using Assets.Scripts.Infrastructure.Requests.AuthService;
using Assets.Scripts.Infrastructure.Responses.AuthService;
using Cysharp.Threading.Tasks;
using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Common.WebRequest;
using Assets.Scripts.Common.Constants;
using Assets.Scripts.Infrastructure.Responses;

namespace Assets.Scripts.Services.Implementations
{
    public class AuthService : IAuthService
    {
        public AuthService() { }

        public async UniTask<EmptyResponse> Registration(RegistrationRequest request)
        {
            var result = await UniversalWebRequest.SendRequest<RegistrationRequest, EmptyResponse>(
                ApiEndpointsConstants.RegistrationEndpoint,
                RequestType.POST,
                request);

            return result.EnsureSuccess();
        }

        public async UniTask<LoginResponse> Login(LoginRequest request)
        {
            var result = await UniversalWebRequest.SendRequest<LoginRequest, LoginResponse>(
                ApiEndpointsConstants.LoginEndpoint,
                RequestType.POST,
                request);

            return result.EnsureSuccess();
        }

        public async UniTask<EmptyResponse> ConfirmEmail(ConfirmEmailRequest request)
        {
            var result = await UniversalWebRequest.SendRequest<ConfirmEmailRequest, EmptyResponse>(
                ApiEndpointsConstants.EmailConfirmEndpoint,
                RequestType.POST,
                request);

            return result.EnsureSuccess();
        }

        public async UniTask<EmptyResponse> RecoveryPassword(RecoveryPasswordRequest request)
        {
            var result = await UniversalWebRequest.SendRequest<RecoveryPasswordRequest, EmptyResponse>(
                ApiEndpointsConstants.RecoveryPasswordEndpoint,
                RequestType.POST,
                request);

            return result.EnsureSuccess();
        }
    }
}
