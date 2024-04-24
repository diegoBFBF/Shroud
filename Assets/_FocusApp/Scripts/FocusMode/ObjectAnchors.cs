using UnityEngine;

public class ObjectAnchors : MonoBehaviour{

    public Transform startPositionT;
    public Transform endPositionT;

    public Vector3 startPosition => startPositionT ? startPositionT.position : Vector3.zero; 
    public Vector3 endposition => endPositionT ? endPositionT.position : Vector3.zero; 

    [SerializeField]
    private Color color;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(startPosition,0.015f);
        Gizmos.DrawLine(startPosition,endposition);
        Gizmos.DrawSphere(endposition,0.015f);
    }

}