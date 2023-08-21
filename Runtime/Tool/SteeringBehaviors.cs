using UnityEngine;

namespace GrazerCore.Tool
{
    public class SteeringBehaviors
    {
        public static Vector3 Seek(Vector3 selfPos, Vector3 targetPos, Vector3 currentVel, float maxSpeed, float maxForce)
        {
            var seekVel = currentVel;
            var desiredVelocity = Vector3.Normalize(targetPos - selfPos) * maxSpeed;
            var steering = desiredVelocity - seekVel;
            steering = Truncate(steering, maxForce);
            seekVel = Truncate(seekVel + steering, maxSpeed);
            return seekVel;
        }

        public static Vector3 Flee(Vector3 selfPos, Vector3 targetPos, Vector3 currentVel, float maxSpeed, float maxForce)
        {
            var fleeVel = currentVel;
            var desiredVelocity = Vector3.Normalize(selfPos - targetPos) * maxSpeed;
            var steering = desiredVelocity - fleeVel;
            steering = Truncate(steering, maxForce);
            fleeVel = Truncate(fleeVel + steering, maxSpeed);
            return fleeVel;
        }

        public static Vector3 Arrival(Vector3 selfPos, Vector3 targetPos, Vector3 currentVel, float maxSpeed, float maxForce, float slowingRadius)
        {
            var arrivalVel = currentVel;
            var desiredVelocity = targetPos - selfPos;
            var distance = desiredVelocity.magnitude;
            if (distance < slowingRadius)
            {
                desiredVelocity = desiredVelocity.normalized * maxSpeed * (distance / slowingRadius);
            }
            else
            {
                desiredVelocity = desiredVelocity.normalized * maxSpeed;
            }
            var steering = desiredVelocity - arrivalVel;
            steering = Truncate(steering, maxForce);
            arrivalVel = Truncate(arrivalVel + steering, maxSpeed);
            return arrivalVel;
        }

        public static Vector3 Truncate(Vector3 vel, float maxLength)
        {
            return Vector3.ClampMagnitude(vel, maxLength);
        }
    }
}
