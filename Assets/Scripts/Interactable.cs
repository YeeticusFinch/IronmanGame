using UnityEngine;

public class Interactable : MonoBehaviour
{

    //Player player;
    public string text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void interact(Player player)
    {
        Player.selectedInteraction = null;
    }

    public virtual string getText()
    {
        return text;
    }
}
