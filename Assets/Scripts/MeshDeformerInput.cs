using UnityEngine;

public class MeshDeformerInput : MonoBehaviour
{
    [SerializeField] private float inputForce;
    [SerializeField] public float inputForceOffset = 0.1f;
    private Ray inputRay;
    private RaycastHit inputHit;
    private Vector3 inputForcePoint;
    private MeshDeformationController inputDeformer;

    void HandleInput(Vector3 inputPoint)
    {
        inputRay = Camera.main.ScreenPointToRay(inputPoint);

        if (Physics.Raycast(inputRay, out inputHit)) 
        {
            inputDeformer = inputHit.collider.GetComponent<MeshDeformationController>();
            
            if (inputDeformer) 
            {
                inputForcePoint = inputHit.point;
                inputForcePoint += inputHit.normal * inputForceOffset;
                inputDeformer.AddDeformingForce(inputForcePoint, inputForce);
            }
        }
    }

    void Update()
    {
        #if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                HandleInput(Input.mousePosition);
            }
        #elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0)
            {
                HandleInput(Input.GetTouch(0).position);
            }
        #endif
    }
}