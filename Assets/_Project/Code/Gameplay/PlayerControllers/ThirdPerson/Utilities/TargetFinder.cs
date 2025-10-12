using UnityEngine;
using _Project.Code.Gameplay.Combat;

namespace _Project.Code.Gameplay.PlayerControllers.ThirdPerson.Utilities
{
    public static class TargetFinder
    {
        public static Transform FindNearestTarget(Transform origin, float radius, LayerMask layers)
        {
            var colliders = Physics.OverlapSphere(origin.position, radius, layers);

            Transform nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (var collider in colliders)
            {
                if (collider.transform == origin) continue;

                var targetable = collider.GetComponent<ITargetable>();
                if (targetable == null || !targetable.IsTargetable) continue;

                float distance = Vector3.Distance(origin.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = collider.transform;
                }
            }

            return nearest;
        }
    }
}