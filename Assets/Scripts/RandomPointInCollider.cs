using System;
using UnityEngine;

public class RandomPointInCollider
{
    Collider2D collider;
    Vector3 minBound;
    Vector3 maxBound;

    int maxAttempts;

    public RandomPointInCollider(Collider2D collider, int maxAttempts = 100)
    {
      this.maxAttempts = maxAttempts;
      this.collider = collider;
      this.minBound = collider.bounds.min;
      this.maxBound = collider.bounds.max;
    }

    public Vector3 RandomPoint()
    {
      Vector3 randomPoint;
      int attemptsDone = 0;

      do {
        randomPoint =
          new Vector3(
            UnityEngine.Random.Range(minBound.x, maxBound.x),
            UnityEngine.Random.Range(minBound.y, maxBound.y),
            UnityEngine.Random.Range(minBound.z, maxBound.z)
          );
        attemptsDone ++;

        if(attemptsDone > maxAttempts)
          throw new InvalidOperationException("Max attempts reached: " + attemptsDone);

      } while(!collider.OverlapPoint(randomPoint));

      return randomPoint;
    }
}
