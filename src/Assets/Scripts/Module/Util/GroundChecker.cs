using UnityEngine;

namespace Module.Utile
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] private Transform groundCheckPosition;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayer;

        public bool CheckGroundedByTag()
        {
            Collider[] colliders = Physics.OverlapSphere(groundCheckPosition.position, groundCheckRadius, groundLayer);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Ground") || collider.CompareTag("ScaleObject"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}