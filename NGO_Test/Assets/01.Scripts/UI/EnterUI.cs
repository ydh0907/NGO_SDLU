using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;
using Unity.Networking.Transport.Relay;

public class EnterUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    private static string joinCode = string.Empty;
    private Allocation allocation;
    private JoinAllocation joinAllocation;

    private void Awake()
    {
        hostButton.onClick.AddListener(HandleHostStartEvent);
        clientButton.onClick.AddListener(HandleClientStartEvent);
        NetworkManager.Singleton.ConnectionApprovalCallback += Approval;
    }

    private void Approval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = true;
    }

    private async void HandleClientStartEvent()
    {
        joinCode = ipInput.text;
        try
        {
            joinAllocation = await Relay.Instance.JoinAllocationAsync(joinCode);
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async void HandleHostStartEvent()
    {
        while (true)
        {
            try
            {
                allocation = await Relay.Instance.CreateAllocationAsync(4);
                break;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                continue;
            }
        }

        while (true)
        {
            try
            {
                joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log(joinCode);
                break;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                continue;
            }
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayData);

        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene
            (SceneEnum.GameScene.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
