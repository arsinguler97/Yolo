using UnityEngine;
using _Project.Code.Core.Events;
using _Project.Code.Gameplay.Triggers;

namespace _Project.Code.Gameplay.CameraSystems.Triggers
{
    public class CameraZoneTrigger : TriggerZone
    {
        [Header("Camera Settings")]
        [SerializeField] private CameraProfile _cameraProfile;
        [SerializeField] private float _blendTime = 1f;
        [SerializeField] private int _priority = 10;

        [Header("Exit Behavior")]
        [SerializeField] private bool _exitResetsCamera = true;
        [SerializeField] private CameraProfile _exitProfile;

        protected override void OnZoneEntered(GameObject obj)
        {
            if (_cameraProfile != null)
            {
                EventBus.Instance.Publish(new CameraZoneEnteredEvent
                {
                    Profile = _cameraProfile,
                    BlendTime = _blendTime,
                    Priority = _priority
                });

                Debug.Log($"Entered camera zone: {_cameraProfile.name}");
            }
        }

        protected override void OnZoneExited(GameObject obj)
        {
            if (_exitResetsCamera && _exitProfile != null)
            {
                EventBus.Instance.Publish(new CameraZoneEnteredEvent
                {
                    Profile = _exitProfile,
                    BlendTime = _blendTime,
                    Priority = 0
                });
            }

            EventBus.Instance.Publish(new CameraZoneExitedEvent
            {
                Profile = _cameraProfile
            });

            Debug.Log($"Exited camera zone: {_cameraProfile?.name}");
        }

        protected override Color GetGizmoColor()
        {
            return new Color(0, 1, 1, 0.3f);
        }
    }
}
