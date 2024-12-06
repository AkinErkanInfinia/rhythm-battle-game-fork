using InfiniaGamingCore.XMS;
using Managers;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UDPController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private List<GameObject> players;

    private int currentIndex = 0;

    private void Start()
    {
        RegisterToEvents();
    }

    private void RegisterToEvents()
    {
        MessageBroker.Default.Receive<PlayReceivedMessage>().Subscribe(GetPlayers).AddTo(this);
    }

    private void GetPlayers(PlayReceivedMessage message)
    {
        players = gameManager.GetPlayers();
        MessageBroker.Default.Receive<PlayerMovementMessage>().Subscribe(HandleMovement).AddTo(this);
    }

    private void HandleMovement(PlayerMovementMessage message)
    {
        PlayerData playerData = message.newPlayerData;

        players[currentIndex].transform.position = playerData.bodyPosition;

        if (currentIndex == players.Count - 1)
            currentIndex = 0;
        else
            currentIndex++;
    }
}