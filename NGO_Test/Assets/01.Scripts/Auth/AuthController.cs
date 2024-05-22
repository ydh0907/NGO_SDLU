using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    TimeOut,
    Error,
}

public class AuthController : MonoBehaviour
{
    public AuthState state;

    private void Start()
    {
        Initialize();
        TryAnonymouslySignIn();
    }

    private async void Initialize()
    {
        state = AuthState.NotAuthenticated;
        await UnityServices.InitializeAsync();
        Debug.Log(AuthenticationService.Instance.PlayerId);
    }

    private async void TryAnonymouslySignIn(int count = 5)
    {
        for (int i = 0; i < count; i++)
        {
            await AuthAnonymously();
            if (state == AuthState.Authenticated) break;
        }

        if (state == AuthState.TimeOut)
            Debug.LogError("time out on your auth, pls check your wifi or retry auth");
    }

    private async Task AuthAnonymously()
    {
        if (state == AuthState.Authenticated) return;
        if (state == AuthState.Authenticating) return;

        state = AuthState.Authenticating;

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (AuthenticationException e)
        {
            Debug.LogException(e);
            state = AuthState.Error;
        }
        catch (RequestFailedException e)
        {
            Debug.LogException(e);
            state = AuthState.Error;
        }

        if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
        {
            state = AuthState.Authenticated;
        }
        else if (state != AuthState.Error)
        {
            state = AuthState.TimeOut;
        }
    }
}
