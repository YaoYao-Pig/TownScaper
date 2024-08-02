using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    public Vertex a;
    public Vertex b;

    public Vertex midVertex;

    public List<Triangle> owners;
    private Quad qOwner;



    public int TriangleOwnersNumber() {  return owners.Count; }
    public List<Triangle> GetTriangleOwners() { return owners; }

    public void AddTriangleOwner(Triangle _t) { 
        if (owners.Count > 2) 
            throw new System.Exception("Edge::AddTriangleOwner"); 
        owners.Add(_t); 
    }


    public Edge(Vertex _a,Vertex _b)
    {
        a = _a;b = _b;
        owners = new List<Triangle>();
        qOwner = null;
        midVertex =Vertex.CreateMidVertex(a,b);
        Grid.midVertexList.Add(midVertex);

    }



    public void EraseTriangleOwner(Triangle _t)
    {
        Triangle e=null;
        foreach(var t in owners)
        {
            if (t.id == _t.id)
            {
                e = t;
                break;
            }
        }
        if (e != null) { owners.Remove(e); return; }
        throw new System.Exception("Edge::EraseTriangleOwner: Don't has TOwner");
    }

    public void AddQuadOwner(Quad _q)
    {
        if (qOwner == null) qOwner = _q;
    }

    public override int GetHashCode()
    {
        return a.GetHashCode() + b.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        Edge edge = (Edge)obj;
        return (a.Equals(edge.a) && b.Equals(edge.b)) || 
            (a.Equals(edge.b) && b.Equals(edge.a));
    }

}
