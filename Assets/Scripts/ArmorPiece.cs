using System.Collections;
using UnityEngine;

// Watch out, this ExecuteAlways means that code here will execute in the editor (like even when the game isn't running)
[ExecuteAlways]
public class ArmorPiece : MonoBehaviour
{
    //public GameObject mesh;
    public string bone;
    public float maxDurability = 50;

    [System.NonSerialized]
    public float durability;

    Transform ogParent;

    Transform parent;
    Vector3 ogLocalPosition;
    Quaternion ogLocalRotation;
    Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        durability = maxDurability;
        ogParent = transform.parent;
        ogLocalPosition = transform.localPosition;
        ogLocalRotation = transform.localRotation;
    }

    public void Equip(Player player, GameObject rootArmature)
    {
        foreach (Player.Bone bone in player.bones)
        {
            if (bone.name.Equals(this.bone))
            {
                parent = bone.bone.transform;
                GameObject rootBone = PhysMath.FindBone(rootArmature, bone.name);
                if (rootBone != null)
                {
                    offset = transform.localPosition - PhysMath.FindBone(rootArmature, bone.name).transform.localPosition;
                }
                //offset = transform.localPosition - bone.rootPosBone.transform.localPosition;
                //transform.parent = bone.bone.transform;
                //Debug.Log("Found bone " + bone);
                break;
            }
        }
    }

    IEnumerator GoToPosition()
    {
        yield return new WaitForSeconds(0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
        {
            // This will only run if the game is running
            if (parent != null)
            {
                transform.localRotation = parent.localRotation;
                transform.localPosition = parent.localPosition + offset;
            }
        }

        if (bone == null || bone.Length == 0)
        {
            bone = name;
        }
    }
}
