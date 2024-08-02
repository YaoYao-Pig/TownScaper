using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int maxRadius;
    [SerializeField] private float cellSize;
    [SerializeField] private int relaxTimes;
    private Grid grid;
    void Awake()
    {
        grid = new Grid(maxRadius,cellSize, relaxTimes);

    }

    private void OnDrawGizmos()
    {
        if (grid != null)
        {
            /*            Gizmos.color = Color.red;
                        foreach (var vertex in grid.vertexList)
                        {
                            Gizmos.DrawSphere(vertex.initeWorldPosition, 0.5f);
                        }*/
            /*            Gizmos.color = Color.white;
                        foreach (var e in Grid.edgeList)
                        {
                            Gizmos.DrawLine(e.a.initeWorldPosition,e.b.initeWorldPosition);
                        }*/

            /*            Gizmos.color = Color.blue;
                        foreach (var q in grid.quadList)
                        {
                            Gizmos.DrawLine(q.a.initeWorldPosition, q.b.initeWorldPosition);
                            Gizmos.DrawLine(q.b.initeWorldPosition, q.c.initeWorldPosition);
                            Gizmos.DrawLine(q.c.initeWorldPosition, q.d.initeWorldPosition);
                            Gizmos.DrawLine(q.d.initeWorldPosition, q.a.initeWorldPosition);
                        }*/
/*            Gizmos.color = Color.gray;
            foreach (var t in grid.triangleList)
            {
                Gizmos.DrawLine(t.a.initeWorldPosition, t.b.initeWorldPosition);
                Gizmos.DrawLine(t.b.initeWorldPosition, t.c.initeWorldPosition);
                Gizmos.DrawLine(t.c.initeWorldPosition, t.a.initeWorldPosition);
                Gizmos.DrawSphere(t.ab.midVertex.initeWorldPosition, 0.1f);
                Gizmos.DrawSphere(t.bc.midVertex.initeWorldPosition, 0.1f);
                Gizmos.DrawSphere(t.ca.midVertex.initeWorldPosition, 0.1f);
            }

            Gizmos.color = Color.white;
            foreach (var v in Grid.midVertexList)
            {
                Gizmos.DrawSphere(v.initeWorldPosition, 0.1f);

            }

            Gizmos.color = Color.black;
            foreach (var v in Grid.centerVertexList)
            {
                Gizmos.DrawSphere(v.initeWorldPosition, 0.1f);

            }*/

            Gizmos.color = Color.yellow;
            foreach (var sq in grid.subQuadList)
            {
                Gizmos.DrawLine(sq.a.currentWorldPosition, sq.b.currentWorldPosition);
                Gizmos.DrawLine(sq.b.currentWorldPosition, sq.c.currentWorldPosition);
                Gizmos.DrawLine(sq.c.currentWorldPosition, sq.d.currentWorldPosition);
                Gizmos.DrawLine(sq.d.currentWorldPosition, sq.a.currentWorldPosition);

            }

        }

    }

}
