using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssemblerArm : MonoBehaviour
{
    public GameObject ikTarget;
    public GameObject ikPole;

    [NonSerialized]
    public Vector3 localRootPos;
    [NonSerialized]
    public Quaternion localRootRot;

    [NonSerialized]
    public SkinnedMeshRenderer targetMesh;
    [NonSerialized]
    public SkinnedMeshRenderer destinationMesh;
    [NonSerialized]
    public int step = -1;
   
    public Vector3 meshOffset;

    Vector3 targetLocalPos;
    Transform targetParent;
    Vector3 destinationLocalPos;
    Transform destinationParent;

    Quaternion rotDiff;

    public float max_speed = 2f;
    public float accel = 1f;
    public float kp = 1f;

    float speed = 0f;

    Animator anim;

    [NonSerialized]
    public GameObject piece; // The piece this robot arm is holding

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        localRootPos = ikTarget.transform.localPosition;
        step = -1;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int cc = 0;
    private void FixedUpdate()
    {
        if (step != -1)
        {
            cc++;
            if (cc > 100000) cc = 0;
            switch (step)
            {
                case 0: // Go towards the target
                    {
                        anim.SetBool("Grip", false);
                        anim.SetBool("Release", true);
                        if (targetLocalPos == null || targetParent == null || cc % 200 == 0)
                        {
                            targetParent = GetDominantBone(targetMesh);
                            /*
                            targetParent = targetMesh.gameObject.transform.parent;
                            targetLocalPos = targetParent.InverseTransformPoint(GetMeshPos(targetMesh));
                            Debug.Log("TargetParent = " + targetParent.name);
                            Debug.Log("TargetPos = " + GetMeshPos(targetMesh));
                            Debug.Log("TargetLocalPos = " + targetLocalPos);
                            */
                        }
                        //Vector3 diff = targetParent.TransformPoint(targetLocalPos) - ikTarget.transform.position;
                        
                        Vector3 diff = targetParent.transform.position - ikTarget.transform.position;
                        float tspeed = Mathf.Clamp((diff.magnitude + 0.2f) * kp, 0, max_speed);
                        if (speed != tspeed)
                        {
                            speed += Mathf.Min(tspeed - speed, accel * Time.fixedDeltaTime);
                        }
                        ikTarget.transform.position += diff.normalized * Mathf.Min(diff.magnitude, speed * Time.fixedDeltaTime);
                        ikTarget.transform.rotation = Quaternion.RotateTowards(ikTarget.transform.rotation, targetParent.transform.rotation, 50 * max_speed * Time.fixedDeltaTime);
                        if (diff.magnitude < speed * Time.fixedDeltaTime)
                        {
                            step = 1;
                            targetLocalPos = Vector3.zero;
                            targetParent = null;
                            destinationLocalPos = Vector3.zero;
                            destinationParent = null;
                        }
                        break;
                    }
                case 1: // Grab the target
                    anim.SetBool("Grip", true);
                    anim.SetBool("Release", false);
                    piece = Instantiate(new GameObject());
                    piece.transform.position = targetMesh.transform.position;
                    piece.transform.rotation = targetMesh.transform.rotation;
                    piece.transform.parent = targetMesh.transform.parent;
                    piece.transform.localPosition += meshOffset;
                    piece.transform.localScale = targetMesh.transform.localScale;
                    piece.transform.parent = ikTarget.transform;
                    MeshFilter mf = piece.AddComponent<MeshFilter>();
                    MeshRenderer mr = piece.AddComponent<MeshRenderer>();
                    mf.mesh = targetMesh.sharedMesh;
                    //mr.materials = smr.sharedMaterials;
                    mr.materials = targetMesh.materials;
                    mr.shadowCastingMode = targetMesh.shadowCastingMode;
                    mr.receiveShadows = targetMesh.receiveShadows;
                    mr.lightProbeUsage = targetMesh.lightProbeUsage;
                    mr.allowOcclusionWhenDynamic = targetMesh.allowOcclusionWhenDynamic;

                    targetMesh.enabled = false;
                    targetMesh.gameObject.SetActive(false);
                    step = 2;
                    break;
                case 2: // Move target to destination
                    {
                        if (destinationLocalPos == Vector3.zero || destinationParent == null || cc % 200 == 0)
                        {
                            destinationParent = GetDominantBone(destinationMesh);
                            /*
                            destinationParent = destinationMesh.gameObject.transform.parent;
                            destinationLocalPos = destinationParent.InverseTransformPoint(GetMeshPos(destinationMesh));
                            */
                        }
                        Vector3 diff = destinationParent.transform.position - ikTarget.transform.position;
                        float tspeed = Mathf.Clamp((diff.magnitude + 0.1f) * kp, 0, max_speed);
                        if (speed != tspeed)
                        {
                            speed += Mathf.Min(tspeed - speed, accel * Time.fixedDeltaTime);
                        }
                        //Vector3 diff = destinationParent.TransformPoint(destinationLocalPos) - ikTarget.transform.position;
                        ikTarget.transform.position += diff.normalized * Mathf.Min(diff.magnitude, speed * Time.fixedDeltaTime);
                        ikTarget.transform.rotation = Quaternion.RotateTowards(ikTarget.transform.rotation, destinationParent.transform.rotation, 50 * max_speed * Time.fixedDeltaTime);
                        if (diff.magnitude < speed * Time.fixedDeltaTime)
                        {
                            step = 3;
                            destinationLocalPos = Vector3.zero;
                            destinationParent = null;
                        }
                        break;
                    }
                case 3: // Place the target at the destination
                    {
                        anim.SetBool("Grip", false);
                        anim.SetBool("Release", false);
                        Destroy(piece);
                        destinationMesh.enabled = true;
                        step = -1;
                        break;
                    }
            }
        } else
        {
            Vector3 diff = transform.TransformPoint(localRootPos) - ikTarget.transform.position;
            float tspeed = Mathf.Clamp((diff.magnitude + 0.1f) * kp, 0, max_speed);
            if (speed != tspeed)
            {
                speed += Mathf.Min(tspeed - speed, accel * Time.fixedDeltaTime);
            }
            if (diff.magnitude > speed * Time.fixedDeltaTime)
            {
                ikTarget.transform.position += diff.normalized * Mathf.Min(diff.magnitude, speed * Time.fixedDeltaTime);
                ikTarget.transform.rotation = Quaternion.RotateTowards(ikTarget.transform.rotation, localRootRot, 50 * max_speed * Time.fixedDeltaTime);
            }
        }
    }

    Vector3 GetMeshPos(SkinnedMeshRenderer smr)
    {
        
        Mesh mesh = new Mesh();
        smr.BakeMesh(mesh);
        Vector3 avgPos = Vector3.zero;
        foreach (Vector3 v in mesh.vertices)
        {
            avgPos += v;
        }
        avgPos /= mesh.vertices.Length;
        //mesh.RecalculateBounds();
        //return mesh.bounds.center;
        Debug.Log("Mesh Center: " + avgPos);
        return smr.transform.parent.TransformPoint(avgPos);
        
    }
    Vector3 GetMeshPos(Mesh mesh)
    {
        Vector3 avgPos = Vector3.zero;
        foreach (Vector3 v in mesh.vertices)
        {
            avgPos += v;
        }
        avgPos /= mesh.vertices.Length;
        //mesh.RecalculateBounds();
        //return mesh.bounds.center;
        //Debug.Log("Mesh Center: " + avgPos);
        return avgPos;

    }

    Transform GetDominantBone(SkinnedMeshRenderer smr) {
        // Assume smr is your SkinnedMeshRenderer
        //Mesh mesh = new Mesh();
        //smr.BakeMesh(mesh);
        
        Mesh mesh = smr.sharedMesh; // or use BakeMesh for deformed positions
        BoneWeight[] weights = mesh.boneWeights;
        Vector3[] vertices = mesh.vertices;

        // Dictionary to accumulate total weight per bone index
        Dictionary<int, float> boneInfluence = new Dictionary<int, float>();

        // Also accumulate positions for vertices where each bone is dominant
        Dictionary<int, Vector3> boneVertexSum = new Dictionary<int, Vector3>();
        Dictionary<int, int> boneVertexCount = new Dictionary<int, int>();

        for (int i = 0; i < weights.Length; i++)
        {
            BoneWeight bw = weights[i];
            // Determine the bone with maximum influence for this vertex
            float maxWeight = bw.weight0;
            int dominantBone = bw.boneIndex0;
            if (bw.weight1 > maxWeight)
            {
                maxWeight = bw.weight1;
                dominantBone = bw.boneIndex1;
            }
            if (bw.weight2 > maxWeight)
            {
                maxWeight = bw.weight2;
                dominantBone = bw.boneIndex2;
            }
            if (bw.weight3 > maxWeight)
            {
                maxWeight = bw.weight3;
                dominantBone = bw.boneIndex3;
            }

            // Aggregate the weight for this bone
            if (!boneInfluence.ContainsKey(dominantBone))
            {
                boneInfluence[dominantBone] = 0;
                boneVertexSum[dominantBone] = Vector3.zero;
                boneVertexCount[dominantBone] = 0;
            }
            boneInfluence[dominantBone] += maxWeight;
            boneVertexSum[dominantBone] += vertices[i];
            boneVertexCount[dominantBone]++;
        }

        // Find the bone index with the highest total influence
        int dominantBoneOverall = boneInfluence.OrderByDescending(pair => pair.Value).First().Key;

        // Compute the average vertex position for that bone
        Vector3 averageVertexPos = boneVertexSum[dominantBoneOverall] / boneVertexCount[dominantBoneOverall];

        // Get the dominant bone's transform (from SkinnedMeshRenderer.bones)
        Transform dominantBoneTransform = smr.bones[dominantBoneOverall];

        // Compute the displacement between the bone and the mesh's average vertex position
        Vector3 displacement = averageVertexPos - dominantBoneTransform.localPosition;

        // (Optionally, convert to world space using skinnedRenderer.transform.TransformPoint)

        //Debug.Log($"Dominant Bone: {dominantBoneTransform.name}, Displacement: {displacement}");
        return dominantBoneTransform;
    }
}
