using UnityEngine;

public class DashCollisionChecker : MonoBehaviour
{
    public bool IsPositionAvailable(Vector3 position, float radius, LayerMask layerMask)
    {
        if (Physics.CheckSphere(position, radius, layerMask))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}