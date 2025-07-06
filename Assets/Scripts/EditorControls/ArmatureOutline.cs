using UnityEngine;

[ExecuteInEditMode]
public class ArmatureOutline : MonoBehaviour
{
    public Color color = Color.red;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OutlineChildren(transform);
    }

    void OutlineChildren(Transform root)
    {
        if (root.childCount > 0)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                Debug.DrawLine(root.position, root.GetChild(i).position, color);
                OutlineChildren(root.GetChild(i));
            }
        }
    }
}
