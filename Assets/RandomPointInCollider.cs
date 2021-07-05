using UnityEngine;

public class RandomPointInCollider
{
    Collider2D collider;
    Vector3 minBound;
    Vector3 maxBound;

    public RandomPointInCollider(Collider2D collider)
    {
      this.collider = collider;
      this.minBound = collider.bounds.min;
      this.maxBound = collider.bounds.max;
    }

    public Vector3 RandomPoint()
    {
      Vector3 randomPoint;

      do {
        randomPoint =
          new Vector3(
            Random.Range(minBound.x, maxBound.x),
            Random.Range(minBound.y, maxBound.y),
            Random.Range(minBound.z, maxBound.z)
          );
      } while(!collider.OverlapPoint(randomPoint));

      return randomPoint;
    }
}
