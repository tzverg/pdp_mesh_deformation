using UnityEngine;

public class MeshDeformerInput : MonoBehaviour
{
    [SerializeField] private float inputForce;
    [SerializeField] public float inputForceOffset = 0.1f;
    private Ray inputRay;
    private RaycastHit inputHit;
    private Vector3 inputForcePoint;
    private MeshDeformationController inputDeformer;

    #if UNITY_EDITOR
        void HandleInput()
        {
            inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
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
    #endif

    void Update()
    {
        #if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                HandleInput();
            }
        #endif
    }
}