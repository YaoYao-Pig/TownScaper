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
    [SerializeField] public ModuleLibrary moduleLibrary;
    [SerializeField] private Material moduleMaterial;
    [SerializeField] private float cellHeight;

    public List<Slot> slotList;
    private Grid grid;
    private GameManger gameManger;
    public WaveFunctionCpllapse waveFunctionCpllapse;
    void Awake()
    {
        grid = new Grid(maxRadius,cellSize, relaxTimes, maxYHeight, cellHeight);
        moduleLibrary=Instantiate(moduleLibrary);
        waveFunctionCpllapse = GetComponentInParent<WaveFunctionCpllapse>();

    }
    private void Start()
    {
        
    }
    private void Update()
    {
        /*foreach (var vc in grid.cubeVertexList)
        {
            if (vc.isActive && Vector3.Distance(vc.worldPosition, deactiveBall.position)<0.5f)
            {
                vc.isActive = false;
            }
            else if(!vc.isActive && Vector3.Distance(vc.worldPosition, activeBall.position) < 0.5f)
            {
                vc.isActive = true;
            }
        }*/
        

    }


    //当点击到某个CubeVertex时，根据新的状态修改CubeVe
    public void ToggleSlot(CubeVertex _cv)
    {
        _cv.isActive = !_cv.isActive;
        foreach (var cq in _cv.cubeQuadList)
        {
            CubeQuad.UpdateBit(cq);
            UpdateSlot(cq);
        }
    }

    public Grid GetGrid()
    {
        return grid;
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
                    slotList.Add(slot);
                    //更新Module
                    slot.UpdateModule(slot.possibleModule[0]);
                    waveFunctionCpllapse.resetSlot.Add(slot);
                    waveFunctionCpllapse.curCollapseSlots.Add(slot);
                }
                
            }
            else
            {
                Slot slot = slotGameObject.GetComponent<Slot>();
                if (_cq.bits=="00000000"|| _cq.bits == "11111111")
                {
                    slotList.Remove(slot);
                    if (waveFunctionCpllapse.resetSlot.Contains(slot))
                    {
                        waveFunctionCpllapse.resetSlot.Remove(slot);
                    }
                    if (waveFunctionCpllapse.curCollapseSlots.Contains(slot))
                    {
                        waveFunctionCpllapse.curCollapseSlots.Remove(slot);
                    }
                    Destroy(slotGameObject);
                    Resources.UnloadUnusedAssets();
                }
                else
                {
                    slotGameObject.GetComponent<Slot>().ResetSlot(moduleLibrary);
                    slotGameObject.GetComponent<Slot>().UpdateModule(slot.possibleModule[0]);


                    if (!waveFunctionCpllapse.resetSlot.Contains(slot))
                    {
                        waveFunctionCpllapse.resetSlot.Add(slot);
                    }
                    if (!waveFunctionCpllapse.curCollapseSlots.Contains(slot))
                    {
                        waveFunctionCpllapse.curCollapseSlots.Add(slot);
                    }
                }

            }

            
        }
        return;
        
    }
    private void OnDrawGizmos()
    {
        if (grid != null)
        {

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
