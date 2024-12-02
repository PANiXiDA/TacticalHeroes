using Assets.Scripts.Infrastructure.Requests.AuthService;
using TMPro;
using UnityEngine;
using Zenject;
using UnityEngine.SceneManagement;
using Assets.Scripts.Common.WebRequest.JWT;
using Assets.Scripts.Common.Constants;
using Assets.Scripts.Services.Interfaces;
using Assets.Scripts.UI;
using Assets.Scripts.Common.Helpers;

public class AuthManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _emailAuth, _passwordAuth;

    [SerializeField]
    private TMP_InputField _nicknameRegistration, _emailRegistration, _passwordRegistration, _repeatPasswordRegistration;

    [SerializeField]
    private TMP_InputField _emailConfirmAccount, _emailRecoveryPassword;

    private FormsManager _formsManager;
    private PopupManager _popupManager;

    private IAuthService _authService;

    [Inject]
    public void Construct(IAuthService authService)
    {
        _authService = authService;
    }

    private void Start()
    {
        _formsManager = FormsManager.Instance;
        _popupManager = PopupManager.Instance;
    }

    public void OnLoginClicked()
    {
        var request = new LoginRequest(_emailAuth.text, _passwordAuth.text);

        TaskRunner.RunWithGlobalErrorHandling(async () =>
        {
            var response = await _authService.Login(request);

            JwtToken.AccessToken = response.AccessToken;
            JwtToken.RefreshToken = response.RefreshToken;

            SceneManager.LoadScene(SceneConstants.MultiPlayerLobbyScene);
        });
    }

    public void OnRegistrationClicked()
    {
        var request = new RegistrationRequest(
            _nicknameRegistration.text,
            _emailRegistration.text,
            _passwordRegistration.text,
            _repeatPasswordRegistration.text);

        TaskRunner.RunWithGlobalErrorHandling(async () =>
        {
            var response = await _authService.Registration(request);
            _popupManager.ShowPopup(SuccessConstants.Registration);
        });
    }

    public void ConfirmEmail()
    {
        var request = new ConfirmEmailRequest(_emailConfirmAccount.text);

        TaskRunner.RunWithGlobalErrorHandling(async () =>
        {
            var response = await _authService.ConfirmEmail(request);
            _popupManager.ShowPopup(SuccessConstants.ConfirmEmail);
        });
    }

    public void RecoveryPassword()
    {
        var request = new RecoveryPasswordRequest(_emailRecoveryPassword.text);

        TaskRunner.RunWithGlobalErrorHandling(async () =>
        {
            var response = await _authService.RecoveryPassword(request);
            _popupManager.ShowPopup(SuccessConstants.RecoveryPassword);
        });
    }
}
