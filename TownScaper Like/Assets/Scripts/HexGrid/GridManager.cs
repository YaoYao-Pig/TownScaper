using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int maxRadius;
    [SerializeField] private float cellSize;
    [SerializeField] private int relaxTimes;
    [SerializeField] private int maxYHeight;
    [SerializeField] private ModuleLibrary moduleLibrary;
    [SerializeField] private Material moduleMaterial;
    public Transform activeBall;
    public Transform deactiveBall;
    private Grid grid;
    void Awake()
    {
        grid = new Grid(maxRadius,cellSize, relaxTimes, maxYHeight);
        moduleLibrary=Instantiate(moduleLibrary);

    }
    private void Update()
    {
        foreach (var vc in grid.cubeVertexList)
        {
            if (vc.isActive && Vector3.Distance(vc.worldPosition, deactiveBall.position)<0.5f)
            {
                vc.isActive = false;
            }
            else if(!vc.isActive && Vector3.Distance(vc.worldPosition, activeBall.position) < 0.5f)
            {
                vc.isActive = true;
            }
        }
        
        foreach(var cq in grid.cubeQuadList)
        {
            CubeQuad.UpdateBit(cq);
            UpdateSlot(cq);
        }
    }

    //每一帧调用
    public void UpdateSlot(CubeQuad _cq)
    {
        if (_cq.pre_bits != _cq.bits)
        {

            GameObject slotGameObject = null;
            string slotName = "slot_" + grid.subQuadList.IndexOf(_cq.quad).ToString()+"_"+_cq.y;
            //看是否已经被创建
            if (gameObject.transform.Find(slotName))
            {
                slotGameObject = gameObject.transform.Find(slotName).gameObject;
            }
            

            if (slotGameObject == null)
            {

                if(_cq.bits != "00000000" && _cq.bits != "11111111")
                {
                    slotGameObject = new GameObject(slotName, typeof(Slot));
                    slotGameObject.transform.SetParent(transform);
                    slotGameObject.transform.localPosition = _cq.centerPosition;
                    Slot slot = slotGameObject.GetComponent<Slot>();
                    slot.Initialized(_cq, moduleLibrary, moduleMaterial);

                    //更新Module
                    slot.UpdateModule(slot.possibleModule[0]);
                }
                
            }
            else
            {
                Slot slot = slotGameObject.GetComponent<Slot>();
                if (_cq.bits=="00000000"|| _cq.bits == "11111111")
                {
                    Destroy(slotGameObject);
                    Resources.UnloadUnusedAssets();
                }
                else
                {
                    slotGameObject.GetComponent<Slot>().possibleModule = moduleLibrary.GetModule(_cq.bits);
                    slotGameObject.GetComponent<Slot>().UpdateModule(moduleLibrary.GetModule(_cq.bits)[0]);
                }

            }

            
        }
        return;
        
    }
    private void OnDrawGizmos()
    {
        if (grid != null)
        {
            if (maxYHeight == 1)
            {
                foreach (var sq in grid.subQuadList)
                {
                    Gizmos.DrawLine(sq.a.currentWorldPosition, sq.b.currentWorldPosition);
                    Gizmos.DrawLine(sq.b.currentWorldPosition, sq.c.currentWorldPosition);
                    Gizmos.DrawLine(sq.c.currentWorldPosition, sq.d.currentWorldPosition);
                    Gizmos.DrawLine(sq.d.currentWorldPosition, sq.a.currentWorldPosition);


                }
            }
            foreach (var vc in grid.cubeVertexList)
            {
                if (vc.isActive == false)
                    Gizmos.color = Color.gray;
                else
                    Gizmos.color = Color.red;
                Gizmos.DrawSphere(vc.worldPosition,0.2f);

            }
            
            foreach (var qc in grid.cubeQuadList)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(qc.cubeVertexList[0].worldPosition, qc.cubeVertexList[1].worldPosition);
                Gizmos.DrawLine(qc.cubeVertexList[1].worldPosition, qc.cubeVertexList[2].worldPosition);
                Gizmos.DrawLine(qc.cubeVertexList[2].worldPosition, qc.cubeVertexList[3].worldPosition);
                Gizmos.DrawLine(qc.cubeVertexList[3].worldPosition, qc.cubeVertexList[0].worldPosition);
                Gizmos.DrawLine(qc.cubeVertexList[4].worldPosition, qc.cubeVertexList[5].worldPosition);
                Gizmos.DrawLine(qc.cubeVertexList[5].worldPosition, qc.cubeVertexList[6].worldPosition);
                Gizmos.DrawLine(qc.cubeVertexList[6].worldPosition, qc.cubeVertexList[7].worldPosition);
                Gizmos.DrawLine(qc.cubeVertexList[7].worldPosition, qc.cubeVertexList[4].worldPosition);
                Gizmos.DrawLine(qc.cubeVertexList[0].worldPosition, qc.cubeVertexList[4].worldPosition);
                Gizmos.DrawLine(qc.cubeVertexList[1].worldPosition, qc.cubeVertexList[5].worldPosition);
                Gizmos.DrawLine(qc.cubeVertexList[2].worldPosition, qc.cubeVertexList[6].worldPosition);
                Gizmos.DrawLine(qc.cubeVertexList[3].worldPosition, qc.cubeVertexList[7].worldPosition);
                //Gizmos.color = Color.blue;
                //Gizmos.DrawSphere(qc.centerPosition, 0.1f);

                GUI.color = Color.black;
                Handles.Label(qc.centerPosition, qc.bits);
            }

        }

    }

}
