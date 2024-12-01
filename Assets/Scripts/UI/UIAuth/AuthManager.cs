using Assets.Scripts.UI.UIAuth.Requests;
using Assets.Scripts.UI.UIAuth.Responses;
using Cysharp.Threading.Tasks;
using Assets.Scripts.Common.WebRequest;
using TMPro;
using UnityEngine;
using Zenject;
using Assets.Scripts.Common.Enumerations;
using System;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using Assets.Scripts.Common.WebRequest.JWT;

public class AuthManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _emailAuth, _passwordAuth;

    [SerializeField]
    private TMP_InputField _nicknameRegistration, _emailRegistration, _passwordRegistration, _repeatPasswordRegistration;

    [SerializeField]
    private TMP_InputField _emailRecoveryPassword;

    private FormsManager _formsManager;

    private const string _registrationUrl = "auth/sign-up";
    private const string _loginUrl = "auth/login";

    private const string _serverUnavailable = "The server is unavailable";

    private const string _multiPlayerScene = "MultiPlayer";

    [Inject]
    public void Construct()
    {
    }

    private void Start()
    {
        _formsManager = FormsManager.Instance;
    }

    public async void OnLoginClicked()
    {
        var login = new LoginRequest(_emailAuth.text, _passwordAuth.text);

        try
        {
            var result = await UniversalWebRequest.SendRequest<LoginRequest, LoginResponse>(_loginUrl, RequestType.POST, login);

            if (result != null && result.IsSuccess)
            {
                JwtToken.AccessToken = result.Payload.AccessToken;
                JwtToken.RefreshToken = result.Payload.RefreshToken;

                SceneManager.LoadScene(_multiPlayerScene);
            }
            else
            {
                _formsManager.ShowMessage(_serverUnavailable);
                Debug.LogError($"Unexpected error: {result.ToString()}");
            }
        }
        catch (UnityWebRequestException ex)
        {
            var error = Regex.Match(ex.Text, @"Detail=\\\""([^\\\""]*)\\\""");

            if (error.Success)
            {
                _formsManager.ShowMessage(error.Groups[1].Value);
            }
            else
            {
                _formsManager.ShowMessage(_serverUnavailable);
                Debug.LogError($"Unexpected error: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            _formsManager.ShowMessage(_serverUnavailable);
            Debug.LogError($"Unexpected error: {ex.Message}");
        }
    }

    public async void OnRegistrationClicked()
    {
        if (string.IsNullOrEmpty(_nicknameRegistration.text))
        {
            _formsManager.ShowMessage("Enter your nickname!");
            return;
        }
        if (_passwordRegistration.text != _repeatPasswordRegistration.text)
        {
            _formsManager.ShowMessage("Passwords must match!");
            return;
        }

        var registration = new RegistrationRequest(
            _nicknameRegistration.text,
            _emailRegistration.text,
            _passwordRegistration.text,
            _repeatPasswordRegistration.text);

        try
        {
            var result = await UniversalWebRequest.SendRequest<RegistrationRequest, RegistrationResponse>(_registrationUrl, RequestType.POST, registration);

            if (result != null && result.IsSuccess)
            {
                _formsManager.ShowMessage("Registration successful!");
            }
            else
            {
                _formsManager.ShowMessage(_serverUnavailable);
                Debug.LogError($"Unexpected error: {result.ToString()}");
            }
        }
        catch (UnityWebRequestException ex)
        {
            var error = Regex.Match(ex.Text, @"Detail=\\\""([^\\\""]*)\\\""");

            if (error.Success)
            {
                _formsManager.ShowMessage(error.Groups[1].Value);
            }
            else
            {
                _formsManager.ShowMessage(_serverUnavailable);
                Debug.LogError($"Unexpected error: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            _formsManager.ShowMessage(_serverUnavailable);
            Debug.LogError($"Unexpected error: {ex.Message}");
        }
    }

    //public async void RecoveryPassword()
    //{
    //    bool recoveryPasswordSuccessful = false;

    //    if (recoveryPasswordSuccessful)
    //    {
    //        _formsManager.ShowMessage("A message with instructions on password recovery has been sent to your email.");
    //    }
    //}
}
