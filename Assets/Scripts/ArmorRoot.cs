using UnityEngine;

public class ArmorRoot : MonoBehaviour
{
    Transform parent;
    Vector3 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Equip(Player player, GameObject rootArmature)
    {
        parent = player.rootBone.transform;
        GameObject rootBone = PhysMath.FindBone(rootArmature, "root");
        if (rootBone != null)
            offset = transform.localPosition - player.rootBone.transform.localPosition;
        //offset = transform.localPosition - player.rootPoseArmature.transform.localPosition;
        //offset = transform.position - parent.position; 
    }

    // Update is called once per frame
    void Update()
    {
        if (parent != null)
        {
            transform.position = parent.position;
            transform.rotation = parent.rotation;
        }
    }
}
