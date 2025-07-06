using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class Armor : MonoBehaviour
{
    public Collider standCollider;
    public Collider crouchCollider;
    ArmorPiece[] armorPieces;
    ArmorRoot root;
    public GameObject armorVisual;
    public GameObject meshPos; // Marks the position that the meshes detached from the skeleton should take

    Vector3 armorOffset;

    [System.NonSerialized]
    public List<SkinnedMeshRenderer> meshes = new List<SkinnedMeshRenderer>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        armorPieces = GetComponentsInChildren<ArmorPiece>();
        root = GetComponentInChildren<ArmorRoot>();
        armorOffset = meshPos.transform.localPosition;

        foreach (SkinnedMeshRenderer smr in armorVisual.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            //if (smr.gameObject.tag == "Assemble")
            //{
                meshes.Add(smr);
            //}
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void EquipHidden(Player player, GameObject rootPoseArmature)
    {
        //player.transform.position = transform.position;
        //player.transform.rotation = transform.rotation;
        //player.anim.SetBool("Root", true);
        //player.rootPoseArmature.transform.position = transform.position;
        //player.rootPoseArmature.transform.rotation = transform.rotation;
        rootPoseArmature.transform.position = rootPoseArmature.transform.position;
        rootPoseArmature.transform.rotation = rootPoseArmature.transform.rotation;
        root.Equip(player, rootPoseArmature);
        foreach (ArmorPiece piece in armorPieces)
        {
            piece.Equip(player, rootPoseArmature);
        }

        transform.parent = player.transform;

        
    }

    public void FinishEquip(Player player, GameObject rootPoseArmature)
    {
        List<Material> materials = new List<Material>();
        player.meshRenderer.GetMaterials(materials);
        for (int i = 0; i < materials.Count; i++)
        {
            materials[i] = player.invis;
        }
        player.meshRenderer.SetMaterials(materials);
        player.armor = this;
        //StartCoroutine(unroot(player));
    }

    public void Equip(Player player, GameObject rootPoseArmature)
    {
        EquipHidden(player, rootPoseArmature);
        FinishEquip(player, rootPoseArmature);
    }

    IEnumerator unroot(Player player)
    {
        yield return new WaitForSeconds(0.1f);
        player.anim.SetBool("Root", false);
    }
}
