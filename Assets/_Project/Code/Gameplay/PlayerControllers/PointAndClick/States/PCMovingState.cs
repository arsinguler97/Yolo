using UnityEngine;

namespace _Project.Code.Gameplay.PlayerControllers.PointAndClick.States
{
    public class PCMovingState : PCBaseState
    {
        public PCMovingState(PointAndClickController controller) : base(controller)
        {
        }

        public override void Update()
        {
            // Check if reached destination
            if (_controller.HasReachedDestination || !_controller.HasDestination)
            {
                _controller.ClearDestination();
                _stateMachine.TransitionTo<PCIdleState>();
                return;
            }

            // Adjust speed based on distance (slowdown near destination)
            var distanceToTarget = Vector3.Distance(_controller.transform.position, _controller.TargetPosition);

            float speed;
            if (distanceToTarget < _controller.MovementProfile.SlowdownDistance)
            {
                var speedMultiplier = Mathf.Clamp01(distanceToTarget / _controller.MovementProfile.SlowdownDistance);
                speed = _controller.MovementProfile.WalkSpeed * speedMultiplier;
            }
            else
            {
                speed = _controller.MovementProfile.WalkSpeed;
            }

            _controller.Motor.SetSpeed(speed);
        }
    }
}
