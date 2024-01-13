using JetBrains.Annotations;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    NetworkVariable<ushort> score = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public static NetworkVariable<FixedString32Bytes> winnerName = new(new FixedString32Bytes("  "), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private string clientName;

    [CanBeNull] public static event System.Action GameOverEvent;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;
        score.OnValueChanged += OnScoreChanged;
        winnerName.OnValueChanged += OnWinnerSelected;
        clientName = GameObject.Find("InputName").GetComponent<TMP_InputField>().text;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            IncreaseScoreServerRpc(clickPos);
            Debug.Log(score.Value);
        }

    }

    private void OnScoreChanged(ushort prev, ushort current)
    {
        if (current >= 8)
        {
            DeclareWinnerServerRpc(new FixedString32Bytes(clientName));
        }
    }

    private void OnWinnerSelected(FixedString32Bytes prev, FixedString32Bytes current)
    {
        UIGameOver.winnerName = current.Value.ToString();
        GameOverEvent.Invoke();
    }

    [ServerRpc]
    private void IncreaseScoreServerRpc(Vector3 clickPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(clickPos, -Vector2.up, 0.001f);
        if (hit && hit.collider.gameObject.CompareTag("Box"))
        {
            hit.collider.GetComponent<NetworkObject>().Despawn();
            score.Value += 1;

        }

    }

    [ServerRpc]
    private void DeclareWinnerServerRpc(FixedString32Bytes winner)
    {
        winnerName.Value = winner;
    }

}