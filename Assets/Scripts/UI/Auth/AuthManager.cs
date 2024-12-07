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
using System;

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
            if (!Helpers.IsEmailValid(request.Email))
            {
                throw new ApplicationException(ErrorConstants.NotValidEmail);
            }
            if (!Helpers.IsPasswordValid(request.Password))
            {
                throw new ApplicationException(ErrorConstants.NotValidPassword);
            }

            var response = await _authService.Login(request);

            JwtTokenManager.AccessToken = response.AccessToken;
            JwtTokenManager.SaveRefreshToken(response.RefreshToken);

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
            if (!Helpers.IsEmailValid(request.Email))
            {
                throw new ApplicationException(ErrorConstants.NotValidEmail);
            }
            if (!Helpers.IsPasswordValid(request.Password))
            {
                throw new ApplicationException(ErrorConstants.NotValidPassword);
            }
            if (request.Password != request.RepeatPassword)
            {
                throw new ApplicationException(ErrorConstants.IncorrectRepeatPassword);
            }
            if (string.IsNullOrEmpty(request.NickName))
            {
                throw new ApplicationException(ErrorConstants.RequiredNickname);
            }

            var response = await _authService.Registration(request);
            _popupManager.ShowPopup(SuccessConstants.Registration);
        });
    }

    public void ConfirmEmail()
    {
        var request = new ConfirmEmailRequest(_emailConfirmAccount.text);

        TaskRunner.RunWithGlobalErrorHandling(async () =>
        {
            if (!Helpers.IsEmailValid(request.Email))
            {
                throw new ApplicationException(ErrorConstants.NotValidEmail);
            }

            var response = await _authService.ConfirmEmail(request);
            _popupManager.ShowPopup(SuccessConstants.ConfirmEmail);
        });
    }

    public void RecoveryPassword()
    {
        var request = new RecoveryPasswordRequest(_emailRecoveryPassword.text);

        TaskRunner.RunWithGlobalErrorHandling(async () =>
        {
            if (!Helpers.IsEmailValid(request.Email))
            {
                throw new ApplicationException(ErrorConstants.NotValidEmail);
            }

            var response = await _authService.RecoveryPassword(request);
            _popupManager.ShowPopup(SuccessConstants.RecoveryPassword);
        });
    }
}
