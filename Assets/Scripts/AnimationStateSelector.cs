using UnityEngine;

[ExecuteAlways]
public class AnimationStateSelector : MonoBehaviour
{
    [Tooltip("Assign the Animator component here.")]
    public Animator animator;

    [Tooltip("Name of the animation state to play.")]
    public string animationState;

    [Tooltip("Layer index of the selected animation state (base layer should be 0).")]
    public int animationLayer;

    // Store last state to detect changes.
    [SerializeField, HideInInspector]
    private string lastAnimationState = "";

    private void OnValidate()
    {
        // Only update when not playing and if an animator is assigned.
        if (animator != null && !Application.isPlaying)
        {
            // If the state has changed or if the layer is negative, update.
            if (animationState != lastAnimationState)
            {
                lastAnimationState = animationState;
                int layer = animationLayer < 0 ? 0 : animationLayer;
                animator.Play(animationState, layer, 0f);
                Debug.Log($"Playing state: {animationState} on layer: {layer}");
                animator.Update(0.016f);  // Force a small time step update (approx. one frame at 60fps)
#if UNITY_EDITOR
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
#endif
            }
        }
    }
}
