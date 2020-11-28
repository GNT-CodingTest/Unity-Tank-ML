using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpawnPointProvider : MonoBehaviour
{
    public float radius = 20f;

    public LayerMask detectableLayers;
    public LayerMask groundLayers;


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, Vector3.up, radius);
    }
#endif


    public Vector3 GetRandomSpawnPoint(float objectRadius)
    {
        for (var i = 0; i < 30; i++)
        {
            var randomPositionInCircle = Random.insideUnitCircle * radius;
            var randomPosition =
                transform.position + new Vector3(randomPositionInCircle.x, 0, randomPositionInCircle.y);

            var rayStartPosition = randomPosition + Vector3.up * 20f;

            if (Physics.SphereCast(rayStartPosition, objectRadius, Vector3.down, out var hit, 30f, detectableLayers))
            {
                if (groundLayers == (groundLayers | (1 << hit.collider.gameObject.layer)))
                {
                    return hit.point;
                }
            }
        }

        Debug.LogError("Can't find proper position");
        return transform.position;
    }
}