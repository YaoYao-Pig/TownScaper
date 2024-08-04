using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quad : Shape
{
    public Vertex a;
    public Vertex b;
    public Vertex c;
    public Vertex d;

    public Edge ab;
    public Edge bc;
    public Edge cd;
    public Edge da;


    private static int globalId = 0;

    public List<CubeQuad> yQuadList = new List<CubeQuad>();

    private List<Vertex> SortEdge(Vertex _a, Vertex _b, Vertex _c,Vertex _d)
    {
        //根据x和z坐标排序
        List<Vertex> result = new List<Vertex>();

        List<Vertex> tmp = new List<Vertex>() { _a, _b, _c, _d };
        
        for(int i=0;i<tmp.Count-1;++i)
        {
            Vertex t;
            for(int j=0;j< tmp.Count - i - 1; ++j)
            {
                if (tmp[j].initeWorldPosition.x < tmp[j + 1].initeWorldPosition.x)
                {
                    t = tmp[j];
                    tmp[j] =tmp[j + 1];
                    tmp[j + 1] = t;
                }
            }
            
        }

        Vertex left_bottom;
        Vertex left_top;
        if (tmp[2].initeWorldPosition.z < tmp[3].initeWorldPosition.z)
        {
            left_bottom = tmp[2];
            left_top = tmp[3];
        }
        else { left_bottom = tmp[3];
            left_top = tmp[2];
        }

        Vertex right_bottom;
        Vertex right_top;
        if (tmp[0].initeWorldPosition.z < tmp[1].initeWorldPosition.z)
        {
            right_bottom = tmp[0];
            right_top = tmp[1];
        }
        else
        {
            right_bottom = tmp[1];
            right_top = tmp[0];
        }
        result.Add(right_top);
        result.Add(right_bottom);
        result.Add(left_bottom);
        result.Add(left_top);

        return result;
    }

    public Quad(Vertex _a,Vertex _b,Vertex _c,Vertex _d,List<Edge> _es)
    {
        List<Vertex> vs = SortEdge(_a, _b, _c, _d);
        a = vs[0];b = vs[1]; c = vs[2]; d = vs[3];
        vertexs.Add(a);
        vertexs.Add(b);
        vertexs.Add(c);
        vertexs.Add(d);


        id = globalId++;
        centerVertex = Vertex.CreateCenterVertex(vertexs);
        Grid.centerVertexList.Add(centerVertex);

        ab = Grid.GetEdge(_a,_b,_es);
        bc = Grid.GetEdge(_b,_c,_es);
        cd = Grid.GetEdge(_c,_d,_es);
        da = Grid.GetEdge(_d,_a, _es);

    }

    public Quad(HashSet<Vertex> _vs,List<Edge> _es)
    {
        
        List<Vertex> tmp = new List<Vertex>();
        foreach(var v in _vs)
        {
            tmp.Add(v);
        }
        List<Vertex> vs = SortEdge(tmp[0], tmp[1], tmp[2], tmp[3]);
        a = vs[0]; b = vs[1]; c = vs[2]; d = vs[3];
        vertexs.Add(a);
        vertexs.Add(b);
        vertexs.Add(c);
        vertexs.Add(d);

        id = globalId++;
        centerVertex = Vertex.CreateCenterVertex(vertexs);
        Grid.centerVertexList.Add(centerVertex);

        ab = Grid.GetEdge(a, b,_es);
        bc = Grid.GetEdge(b, c,_es);
        cd = Grid.GetEdge(c, d,_es);
        da = Grid.GetEdge(d, a, _es);
    }

    public void CaculateRelax()
    {
        Vector3 center = (a.currentWorldPosition + b.currentWorldPosition + c.currentWorldPosition + d.currentWorldPosition) / 4;

        Vector3 vectorNew_a = (a.currentWorldPosition
            + Quaternion.AngleAxis(-90, Vector3.up) * (b.currentWorldPosition - center) + center
            + Quaternion.AngleAxis(-180, Vector3.up) * (c.currentWorldPosition - center) + center
               + Quaternion.AngleAxis(-270, Vector3.up) * (d.currentWorldPosition - center) + center
            ) / 4;
        Vector3 vectorNew_b = Quaternion.AngleAxis(90, Vector3.up) * (vectorNew_a - center) + center;
        Vector3 vectorNew_c = Quaternion.AngleAxis(180, Vector3.up) * (vectorNew_a - center) + center;
        Vector3 vectorNew_d = Quaternion.AngleAxis(270, Vector3.up) * (vectorNew_a - center) + center;

        a.offset += (vectorNew_a - a.currentWorldPosition) * 0.1f;
        b.offset += (vectorNew_b - b.currentWorldPosition) * 0.1f;
        c.offset += (vectorNew_c - c.currentWorldPosition) * 0.1f;
        d.offset += (vectorNew_d - d.currentWorldPosition) * 0.1f;

    }
}

public class CubeQuad
{
    public CubeVertex[] cubeVertexList = new CubeVertex[8];

    public string bits="";
    public string pre_bits = "";
    public int y;

    public Vector3 centerPosition = Vector3.zero;
    public Quad quad;
    public CubeQuad(Quad _q,int _y)
    {
        if (_y >= Grid.maxY) throw new System.Exception("CubeQuad::CubeQuad -> _y should less than maxY");
        quad = _q;
        y = _y;
        cubeVertexList[0] = _q.a.yVertexList[_y + 1];
        cubeVertexList[1] = _q.b.yVertexList[_y + 1];
        cubeVertexList[2] = _q.c.yVertexList[_y + 1];
        cubeVertexList[3] = _q.d.yVertexList[_y + 1];
        cubeVertexList[4] = _q.a.yVertexList[_y];
        cubeVertexList[5] = _q.b.yVertexList[_y];
        cubeVertexList[6] = _q.c.yVertexList[_y];
        cubeVertexList[7] = _q.d.yVertexList[_y];


        foreach(var v in cubeVertexList)
        {
            centerPosition += v.worldPosition;
            v.cubeQuadList.Add(this);
        }


        centerPosition /= 8;
        
    }

    /// <summary>
    /// 根据CubeQuad 每一个CubeVertex的Active状态，更新其bit值
    /// </summary>
    /// <param name="_c"></param>
    public static void UpdateBit(CubeQuad _c)
    {
        _c.pre_bits = _c.bits;
        _c.bits = "";
        for(int i = 0; i < 8; ++i)
        {
            char tmp = '0';
            if (_c.cubeVertexList[i].isActive)
            {
                tmp = '1';
            }
            _c.bits += tmp;
        }
    }
}
