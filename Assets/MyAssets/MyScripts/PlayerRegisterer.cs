using UnityEngine;
using _Project.Code.Core.ServiceLocator;
using _Project.Code.Gameplay.Player;

namespace MyAssets.MyScripts
{
    public class PlayerRegisterer : MonoBehaviour
    {
        private void Start()
        {
            var playerService = ServiceLocator.Get<PlayerService>();
            if (playerService == null)
            {
                Debug.LogError("[PlayerRegisterer] PlayerService not found!");
                return;
            }

            playerService.RegisterPlayer(null);
            playerService.GetType()
                .GetField("_activePlayer",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(playerService, transform);

            Debug.Log($"[PlayerRegisterer] Registered {gameObject.name} as active player.");
        }
    }
}    