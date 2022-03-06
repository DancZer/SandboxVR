using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class RuntimeTreeSplitAreaGenerator : MonoBehaviour
{
    public int BigHorizontalSliceCount = 6;
    public int SmallHorizontalSliceCount = 3;
    public float BigSplitAngle = 20;
    public float SmallSplitAngle = 20;
    private int planeIndex = 0;
    public GameObject Tree;
    public Material CrossSectionMaterial;

    
    private GameObject NextHullObj;
    private GameObject BigHullObj;
    private GameObject SmallHullObj;

    public void SliceNext(){
        GameObject obj = GetGameObject();

        if(obj != null){
            Vector3 normal = GetSliceNormal();

            GameObject[] shatters = obj.SliceInstantiate(this.transform.position, normal, new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f), CrossSectionMaterial);

            if(shatters!=null && shatters.Length == 2){
                if(planeIndex == 1){
                    NextHullObj = shatters[0];    
                }else if(planeIndex == 2){
                    NextHullObj = shatters[0];
                    BigHullObj = shatters[1];
                }else if(planeIndex == 3){
                    NextHullObj = shatters[0];
                    SmallHullObj = shatters[1];
                }else if(planeIndex < BigHorizontalSliceCount + 3){ 
                    BigHullObj = shatters[0];
                }else if(planeIndex < BigHorizontalSliceCount + SmallHorizontalSliceCount + 3){
                    SmallHullObj = shatters[0];
                }
                
                Destroy(obj);
            }else{
                Debug.Log($"Shatters: {shatters?.Length}");
            }
        }
        
        planeIndex++;
    }

    private Vector3 GetSliceNormal(){

        if(planeIndex < 1){
            return Vector3.zero;
        }else if(planeIndex == 1){
            return Vector3.up;
        }else if(planeIndex == 2){
            return Quaternion.AngleAxis(BigSplitAngle, Vector3.forward) * Vector3.up;
        }else if(planeIndex == 3){
            return Quaternion.AngleAxis(SmallSplitAngle, Vector3.back) * Vector3.up;
        }else if(planeIndex < BigHorizontalSliceCount + 3){
            var chunkIndex = planeIndex-3;

            return GetChunkSlicePlaneNormal(chunkIndex, BigHorizontalSliceCount, 0);
        }else if(planeIndex < BigHorizontalSliceCount + SmallHorizontalSliceCount + 3){
            var chunkIndex = planeIndex - BigHorizontalSliceCount - 3;

            return GetChunkSlicePlaneNormal(chunkIndex, SmallHorizontalSliceCount, 180);
        }else{
            return Vector3.zero;
        }
    }

    private Vector3 GetChunkSlicePlaneNormal(int chunkIndex, int chunkCount, float angleOffset){
        var chunkSliceAngle = 180f/chunkCount;
        
        return Quaternion.AngleAxis(angleOffset+chunkIndex*chunkSliceAngle, Vector3.down) * Vector3.right;
    }

    private GameObject GetGameObject(){
        if(planeIndex < 1){
            return null;
        }else if(planeIndex == 1){
            return Tree;
        }else if(planeIndex == 2){
            return NextHullObj;
        }else if(planeIndex == 3){
            return NextHullObj;
        }else if(planeIndex < BigHorizontalSliceCount + 3){
            return BigHullObj;
        }else if(planeIndex < BigHorizontalSliceCount + SmallHorizontalSliceCount + 3){
            return SmallHullObj;
        }else{
            return null;
        }
    }
    private void OnDrawGizmos() {
        DrawPlane(this.transform.position, GetSliceNormal());
    }
    private void DrawPlane(Vector3 position, Vector3 normal)
    {
        Vector3 v3;
 
        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;;
            
        var corner0 = (position + v3);
        var corner2 = (position - v3);
        var q = Quaternion.AngleAxis(90f, normal);
        v3 = q * v3;
        var corner1 = (position + v3);
        var corner3 = (position - v3);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(corner0, corner2);
        Gizmos.DrawLine(corner1, corner3);
        Gizmos.DrawLine(corner0, corner1);
        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner2, corner3);
        Gizmos.DrawLine(corner3, corner0);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(position, normal);
    }
}
