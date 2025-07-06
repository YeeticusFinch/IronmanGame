using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmorEquipper : Interactable
{
    public Armor armor;
    public GameObject rootPoseArmature; // Armature in the player's root pose
    public AssemblerArm[] arms;

    [NonSerialized]
    public GameObject fakeArmor;

    public override void interact(Player player)
    {
        base.interact(player);
        fakeArmor = GameObject.Instantiate(armor.armorVisual);
        fakeArmor.transform.position = armor.armorVisual.transform.position;
        fakeArmor.transform.rotation = armor.armorVisual.transform.rotation;
        fakeArmor.transform.localScale = armor.armorVisual.transform.lossyScale;
        //armor.EquipVisual(player, rootPoseArmature);
        foreach (SkinnedMeshRenderer mesh in armor.meshes)
        {
            mesh.enabled = false;
        }
        armor.EquipHidden(player, rootPoseArmature);
        StartCoroutine(EquipAnimation(player));
    }

    List<SkinnedMeshRenderer> smrs = new List<SkinnedMeshRenderer>();

    IEnumerator EquipAnimation(Player player)
    {
        bool notDone = true;
        while (notDone)
        {
            SkinnedMeshRenderer notDoneMesh = null;
            notDone = false;
            foreach (SkinnedMeshRenderer mesh in armor.meshes)
            {
                if (!mesh.enabled)
                {
                    notDone = true;
                    if (smrs.Contains(mesh))
                    {
                        // This mesh is already being dealt with
                    } else
                    {
                        notDoneMesh = mesh;
                        break;
                    }
                }
            }

            if (notDone && notDoneMesh != null)
            {
                int prevStep = 1;
                foreach (AssemblerArm arm in arms)
                {
                    if (arm.step == -1 && prevStep >= 1)
                    {
                        //arm.meshOffset = meshOffset;
                        smrs.Add(notDoneMesh);
                        GameObject fakeObj = PhysMath.FindBone(fakeArmor, notDoneMesh.gameObject.name);
                        arm.destinationMesh = notDoneMesh;
                        arm.targetMesh = fakeObj.GetComponent<SkinnedMeshRenderer>();
                        arm.step = 0;
                        break;
                    }
                    prevStep = arm.step;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        armor.FinishEquip(player, rootPoseArmature);
    }

    public override string getText()
    {
        return "Equip " + armor.name;
    }
}
