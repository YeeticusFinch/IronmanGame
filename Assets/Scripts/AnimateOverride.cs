using UnityEngine;

// Watch out, this ExecuteAlways means that code here will execute in the editor (like even when the game isn't running)
[ExecuteAlways]
public class AnimateOverride : MonoBehaviour
{
    [Tooltip("The name of the animation state to play.")]
    public string animAction;
    Player player;
    Animator animator;

    [Tooltip("Select the desired animation state.")]
    public AnimationState selectedState;

    // Define your animation states here.
    public enum AnimationState
    {
        Idle,
        Walk,
        Run,
        Jump
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying)
        {
            // Run when the application isn't playing (like when you are in the editor)
            if (player == null)
            {
                player = gameObject.GetComponent<Player>();
            }
            if (animator == null)
            {
                animator = gameObject.GetComponent<Animator>();
            }
            if (animAction != null && animAction.Trim().Length > 0)
            {
                animator.Play(animAction);
            }
        }
    }
}
