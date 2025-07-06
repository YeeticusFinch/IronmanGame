using UnityEngine;

public class UI : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject interaction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UIButton(string btnName)
    {
        switch (btnName)
        {
            case "play":
                GameManager.instance.resume();
                break;
        }
    }
}
