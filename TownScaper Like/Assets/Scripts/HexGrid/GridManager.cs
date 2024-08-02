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
    public Transform activeBall;
    public Transform deactiveBall;
    private Grid grid;
    void Awake()
    {
        grid = new Grid(maxRadius,cellSize, relaxTimes, maxYHeight);

    }
    private void Update()
    {
        foreach (var vc in grid.cubeVertexList)
        {
            if (vc.isActive && Vector3.Distance(vc.worldPosition, deactiveBall.position)<2f)
            {
                vc.isActive = false;
            }
            else if(!vc.isActive && Vector3.Distance(vc.worldPosition, activeBall.position) < 2f)
            {
                vc.isActive = true;
            }
        }
        
        foreach(var qc in grid.cubeQuadList)
        {
            CubeQuad.UpdateBit(qc);
        }
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
