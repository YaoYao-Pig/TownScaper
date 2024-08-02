using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coord
{
    public int q;
    public int r;
    public int s;

    public Coord(int _q,int _r,int _s)
    {
        q = _q;
        r = _r;
        s = _s;
    }

    private static Coord[] directions = new Coord[]
    { 
        new Coord(0,1,-1),
        new Coord(-1,1,0),
        new Coord(-1,0,1),
        new Coord(0,-1,1),
        new Coord(1,-1,0),
        new Coord(1,0,-1)
    };

    private static Coord GetDirection(int _i)
    {
        return directions[_i];
    }
    //缩放coord
    private Coord Scale(int _r)
    {
        return new Coord(q * _r, r * _r, s * _r);
    }
    private Coord Neighbor(int direction)
    {
        return this+ GetDirection(direction);
    }
    public static List<Coord> GenerateSingleRingCoord(int _radius)
    {
        List<Coord> ringCoord = new List<Coord>();
        if (_radius == 0)
        {
            ringCoord.Add(new Coord(0, 0, 0));
            
        }
        else
        {
            //得到了radius层上的一个点的Coord坐标
            Coord hex = GetDirection(4).Scale(_radius);
            for (int i = 0; i < 6; ++i)
            {
                for (int j = 0; j < _radius; ++j)
                {

                    ringCoord.Add(hex);
                    //每条边上都添加然后前进一步
                    hex = hex.Neighbor(i);
                }
            }
        }
        return ringCoord;

    }

    public static Coord operator+(Coord _a,Coord _b)
    {
        return new Coord(_a.q + _b.q, _a.r + _b.r, _a.s + _b.s);
    }

    public override bool Equals(object _obj)
    {
        Coord tmp = (Coord)_obj;
        return q.Equals(tmp.q)&&
               r.Equals(tmp.r)&&
               s.Equals(tmp.s);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}
