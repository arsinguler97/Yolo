using UnityEngine;
using _Project.Code.Core.ServiceLocator;
using _Project.Code.Gameplay.Scenes;

namespace MyAssets.MyScripts
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private string firstSceneName = "MenuScene";
        [SerializeField] private bool loadAsync;

        private void Start()
        {
            var sceneService = ServiceLocator.Get<SceneService>();
            if (sceneService == null)
            {
                Debug.LogError("[BootstrapSceneLoader] SceneService not found! Make sure GameInitializer is present in this scene.");
                return;
            }

            if (sceneService.CurrentSceneName == firstSceneName)
            {
                Debug.Log("[BootstrapSceneLoader] Already in target scene, skipping load.");
                return;
            }

            Debug.Log($"[BootstrapSceneLoader] Loading first scene: {firstSceneName}");

            if (loadAsync)
                sceneService.LoadSceneAsync(firstSceneName);
            else
                sceneService.LoadScene(firstSceneName);
        }
    }
}