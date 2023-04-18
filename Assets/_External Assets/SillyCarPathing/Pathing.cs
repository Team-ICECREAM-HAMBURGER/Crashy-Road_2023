using UnityEngine;
using UnityEngine.AI;

namespace SillyCarPathing
{
    public class Pathing : MonoBehaviour
    {
        public Transform target;
        public NavMeshPath path;
        private float elapsed = 0.0f;
        void Start()
        {
            path = new NavMeshPath();
            elapsed = 0.0f;
        }
        // Update pathing periodically, currently about every 0.1 seconds
        void Update()
        {
            elapsed += Time.deltaTime;
            if (elapsed > 0.1f)
            {
                elapsed = 0.0f;
                NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
            }
            for (int i = 0; i < path.corners.Length - 1; i++)
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }
        private void OnDrawGizmos()
        {
            // Draw spheres at every corner for debugging
            if (path == null) return;
            for (int i = 0; i < path.corners.Length - 1; i++)
                Gizmos.DrawWireSphere(path.corners[i], 2);
        }
    }
}