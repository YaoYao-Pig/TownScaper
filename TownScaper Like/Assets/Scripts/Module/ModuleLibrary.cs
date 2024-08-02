using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="My/ModuleLibrary")]
public class ModuleLibrary : ScriptableObject
{
    public void Awake()
    {
        ImportMoudle();
    }
    public GameObject allMoudles;

    public Dictionary<string, List<Module>> moduleDic=new Dictionary<string, List<Module>>();


    public void ImportMoudle()
    {
        //初始化dic
        for(int i = 1; i < 256; ++i)
        {
            moduleDic.Add(Convert.ToString(i, 2).PadLeft(8, '0'), new List<Module>());
        }

        //导入
        foreach(Transform child in allMoudles.transform)
        {
            Mesh mesh = child.GetComponent<MeshFilter>().sharedMesh;
            string name = child.name;
            moduleDic[name].Add(new Module(name, mesh, 0, false));


            //是否存在旋转后的不同模型
            if (!IsEqualRotate90(name))
            {
                string r90Name = RotateName(name, 1);
                moduleDic[r90Name].Add(new Module(r90Name, mesh, 1, false));
                if (!IsEqualRotate180(name))
                {
                    string r180Name = RotateName(name, 2);
                    moduleDic[r180Name].Add(new Module(r180Name, mesh, 2, false));
                    string r270Name = RotateName(name, 3);
                    moduleDic[r270Name].Add(new Module(r270Name, mesh, 3, false));
                    if (!FlipRotationEqualCheck(name))
                    {
                        string flipName = FlipName(name);
                        moduleDic[flipName].Add(new Module(flipName,mesh,0,true));
                        moduleDic[RotateName(flipName, 1)].Add(new Module(RotateName(flipName, 1), mesh, 1, true));
                        moduleDic[RotateName(flipName, 2)].Add(new Module(RotateName(flipName, 2), mesh, 2, true));
                        moduleDic[RotateName(flipName, 3)].Add(new Module(RotateName(flipName, 3), mesh, 3, true));
                    }
                }
            }


        }

        

    }

    public string FlipName(string _name)
    {
        return _name[3].ToString() + _name[2] + _name[1] + _name[0] + _name[7] + _name[6] + _name[5] + _name[4];
    }

    public bool FlipRotationEqualCheck(string _name)
    {
        string symetry_vertical = _name[3].ToString() + _name[2] + _name[1] + _name[0] + _name[7] + _name[6] + _name[5] + _name[4];

        string symetry_horizontal = _name[1].ToString() + _name[0] + _name[3] + _name[2] + _name[5] + _name[4] + _name[7] + _name[6];

        string symetry_02 = _name[0].ToString() + _name[3] + _name[2] + _name[1] + _name[4] + _name[7] + _name[6] + _name[5];

        string symetry_03 = _name[2].ToString() + _name[1] + _name[0] + _name[3] + _name[6] + _name[5] + _name[4] + _name[7];

        return _name == symetry_vertical || _name == symetry_horizontal || _name == symetry_02 || _name == symetry_03;
    }

    private string RotateName(string _name,int _times)
    {
        string result = _name;
        for (int i = 0; i < _times; ++i)
        {
            result = result[3] + result.Substring(0, 3) + result[7] + result.Substring(4, 3);
        }
        return result;
    }
    private bool IsEqualRotate90(string _name)
    {
        return _name[0] == _name[1] &&
            _name[1] == _name[2] &&
            _name[2] == _name[3] &&
            _name[3] == _name[1] &&
            _name[4] == _name[5] &&
            _name[5] == _name[6] &&
            _name[6] == _name[7] &&
            _name[7] == _name[4];
    }
    private bool IsEqualRotate180(string _name)
    {
        return _name[0] == _name[2] && _name[1] == _name[3] &&
             _name[4] == _name[6] && _name[5] == _name[7]; 
    }
   public List<Module> GetModule(string _name)
    {

        List<Module> result = null;
        moduleDic.TryGetValue(_name, out result);
        if (result == null)
        {
            Debug.Log(_name);
            throw new Exception("ModuleLibrary::GetModule -> _name can't match");
        }
        return result;
    }
}
