using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Module
{
    public string name;
    public Mesh mesh;
    public int rotation;
    public bool flip;


    public string[] sockets = new string[6];
    public string bit;
    public static Dictionary<int, int> neighorSocket = new Dictionary<int, int>(){ {1,3}, {0, 2 }, { 2, 0}, { 3,1}, { 4, 5 }, { 5, 4 } };
    public Module(string _name,Mesh _mesh,int _r,bool _flip)
    {
        name=_name; mesh = _mesh;rotation = _r;flip = _flip;
        bit = _name.Substring(0, 8);

        if (name.Length != 8)
        {
            sockets[0] = name.Substring(9, 1);
            sockets[1] = name.Substring(10, 1);
            sockets[2] = name.Substring(11, 1);
            sockets[3] = name.Substring(12, 1);
            sockets[4] = name.Substring(13, 1);
            sockets[5] = name.Substring(14, 1);
        }
        else
        {
            sockets[0] = "a";
            sockets[1] = "a";
            sockets[2] = "a";
            sockets[3] = "a";
            sockets[4] = "a";
            sockets[5] = "a";
        }

    }
}
