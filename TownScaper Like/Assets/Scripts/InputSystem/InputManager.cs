using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    private GridManager gridManager;

    private CubeVertex targetVertex;
    private CubeVertex selectedVertex;
    public Cursor cursor;

    public SlotCollider slotCollider;

    private RaycastType raycastType;

    private GameObject slideObject;

    public WaveFunctionCpllapse waveFunctionCpllapse;

    private void Awake()
    {
        gridManager = GetComponentInParent<GameManger>().GetComponentInChildren<GridManager>();
        waveFunctionCpllapse = GetComponentInParent<WaveFunctionCpllapse>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Build.Enable();
        playerInputActions.Build.Add.performed += Add;
        playerInputActions.Build.Delete.performed += Delete;
    }



    public void FixedUpdate()
    {
        FindTarget();
        //selectedVertex = targetVertex;
        if (targetVertex != null)
        {
            UpdateCursor();
        }
    }



    public float maxRayRange;
    [SerializeField] private LayerMask layerMask;


    public void UpdateCursor()
    {
        cursor.UpdateCursor(targetVertex, selectedVertex,raycastType, slideObject);
    }


    private void FindTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        raycastType = RaycastType.NONE;
        if (Physics.Raycast(ray, out hit, maxRayRange, layerMask))
        {
            GroundColliderQuad groundColliderQuad = hit.transform.GetComponent<GroundColliderQuad>();
            
            if (groundColliderQuad)
            {
                selectedVertex = null;

                raycastType = RaycastType.GROUND;

                Vector3 aim = hit.point;

                Quad sq = groundColliderQuad.subQuad;
                Vector3 a = sq.a.currentWorldPosition;
                Vector3 b = sq.b.currentWorldPosition;
                Vector3 c = sq.c.currentWorldPosition;
                Vector3 d = sq.d.currentWorldPosition;
                Vector3 ab = (a + b) / 2;
                Vector3 bc = (b + c) / 2;
                Vector3 cd = (c + d) / 2;
                Vector3 da = (d + a) / 2;


                float ab_cd=(aim.z - ab.z)*(aim.x - cd.x) - (aim.z - cd.z)*(aim.x - ab.x);
                float bc_da=(aim.z -bc.z)*(aim.x- da.x) - (aim.z - da.z)*(aim.x-bc.x);

                float a_ab_cd = (a.z - ab.z)*(a.x-cd.x) - (a.z - cd.z) * (a.x-ab.x);
                float a_bc_da = (a.z -bc.z)*(a.x-da.x)-(a.z - da.z)*(a.x-bc.x);
                bool on_ad_side=ab_cd*a_ab_cd >= 0;
                bool on_ab_side=bc_da*a_bc_da >= 0;
                if (on_ab_side && on_ad_side)
                {
                    targetVertex = sq.a.yVertexList[1];
                }
                else if (on_ad_side && !on_ab_side)
                {
                    targetVertex = sq.d.yVertexList[1];
                }
                else if (!on_ad_side && on_ab_side)
                {
                    targetVertex = sq.b.yVertexList[1];
                }
                else
                {
                    targetVertex = sq.c.yVertexList[1];
                }
            }
            else
            {
                SlotRoot slotRoot = hit.transform.GetComponentInParent<SlotRoot>();
                //SlotSlideType type = hit.transform.GetComponent<SlotSlideType>();
                selectedVertex = slotRoot.cubeVertex;
                string type = hit.transform.GetComponent<Transform>().name;
                Debug.Log(type.ToString());
                if(type == "TOP")
                {

                    if (selectedVertex.y < Grid.maxY-1)
                    {
                        raycastType = RaycastType.TOP;
                        targetVertex = selectedVertex.vertex.yVertexList[selectedVertex.y + 1];
                    }
                    else
                    {
                        targetVertex = null;
                        raycastType = RaycastType.NONE;
                    }
                    
                }
                else if (type == "SLIDE0"|| type == "SLIDE1"|| type == "SLIDE2"|| type == "SLIDE3"|| type == "SLIDE4")
                {
                    raycastType = RaycastType.SILDE;
                    slideObject = hit.transform.GetComponent<Transform>().gameObject;
                    targetVertex = hit.transform.GetComponent<SlotSlide>().neighor;
                }
                
            }
        }
    }




    public void Add(InputAction.CallbackContext ctx)
    {
        if(targetVertex != null&&!targetVertex.isActive)
        {
            gridManager.ToggleSlot(targetVertex);
            slotCollider.CreateSlotCollider(targetVertex);
            waveFunctionCpllapse.WFC();
        }
    }

    public void Delete(InputAction.CallbackContext ctx)
    {
        if(selectedVertex!=null&& selectedVertex.isActive)
        {
            gridManager.ToggleSlot(selectedVertex);
            slotCollider.DestroyCollider(selectedVertex);
            waveFunctionCpllapse.WFC();
        }
    }


    private void OnDrawGizmos()
    {
        if (targetVertex != null)
        {
            Gizmos.DrawSphere(targetVertex.worldPosition, 0.2f);
        }
    }










/*    private void FindTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxRayRange, layerMask))
        {
            GroundColliderQuad groundColliderQuad = hit.transform.GetComponent<GroundColliderQuad>();

            if (groundColliderQuad)
            {
                Vector3 x = hit.point;

                Vector3 a = groundColliderQuad.subQuad.a.currentWorldPosition;
                Vector3 b = groundColliderQuad.subQuad.b.currentWorldPosition;
                Vector3 c = groundColliderQuad.subQuad.c.currentWorldPosition;
                Vector3 d = groundColliderQuad.subQuad.d.currentWorldPosition;
                Vector3[] vextecs = new Vector3[] { a, b, c, d };

                Vector3 ab = groundColliderQuad.subQuad.ab.midVertex.currentWorldPosition;
                Vector3 bc = groundColliderQuad.subQuad.bc.midVertex.currentWorldPosition;
                Vector3 cd = groundColliderQuad.subQuad.cd.midVertex.currentWorldPosition;
                Vector3 da = groundColliderQuad.subQuad.da.midVertex.currentWorldPosition;
                Vector3[] mids = new Vector3[] { ab, bc, cd, da };

                Vector3 center = groundColliderQuad.subQuad.centerVertex.currentWorldPosition;

                int flag = 0;
                for (int i = 0; i < 4; ++i)
                {
                    Vector3 a_ab = mids[i] - vextecs[i];
                    Vector3 ab_center = center - mids[i];
                    Vector3 center_ad = mids[(3 + i) % 4] - center;
                    Vector3 ad_a = vextecs[i] - mids[(3 + i) % 4];

                    Vector3 a_x = x - vextecs[i];
                    Vector3 ab_x = x - mids[i];
                    Vector3 center_x = x - center;
                    Vector3 ad_x = x - mids[(3 + i) % 4];

                    a_ab = Vector3.Cross(a_ab, a_x);
                    ab_center = Vector3.Cross(ab_center, ab_x);
                    center_ad = Vector3.Cross(center_ad, center_x);
                    ad_a = Vector3.Cross(ad_a, ad_x);

                    float a1 = Vector3.Dot(a_ab, ab_center);
                    float a2 = Vector3.Dot(a_ab, center_ad);
                    float a3 = Vector3.Dot(a_ab, ad_a);
                    if (a1 <= 0 || a2 <= 0 || a3 <= 0) continue;
                    flag = i; break;
                }

                if (flag == 0) targetVertex = groundColliderQuad.subQuad.a.yVertexList[1];
                else if (flag == 1) targetVertex = groundColliderQuad.subQuad.b.yVertexList[1];
                else if (flag == 2) targetVertex = groundColliderQuad.subQuad.c.yVertexList[1];
                else if (flag == 3) targetVertex = groundColliderQuad.subQuad.d.yVertexList[1];
            }
            else
            {
                SlotCollider slotCollider = hit.transform.GetComponentInParent<Transform>().GetComponentInParent<SlotCollider>();
                //SlotSlideType type = hit.transform.GetComponent<SlotSlideType>();
                string type = hit.transform.GetComponent<Transform>().name;
                CubeVertex cubeVertex = slotCollider.cubeVertex;
                Debug.Log(type.ToString());
                if (type == "TOP")
                {
                    if (cubeVertex.y < Grid.maxY - 1)
                    {
                        targetVertex = selectedVertex.vertex.yVertexList[cubeVertex.y + 1];


                    }
                    else
                    {
                        targetVertex = null;
                    }

                }
                else if (type == "SLIDE0" || type == "SLIDE1" || type == "SLIDE2" || type == "SLIDE3" || type == "SLIDE4")
                {
                    targetVertex = hit.transform.GetComponent<SlotSlide>().neighor;
                }

            }
        }
    }*/

}
