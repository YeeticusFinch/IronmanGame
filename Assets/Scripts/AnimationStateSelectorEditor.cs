#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

[CustomEditor(typeof(AnimationStateSelector))]
public class AnimationStateSelectorEditor : Editor
{
    // Helper class to store state info.
    private class StateEntry
    {
        public string stateName;
        public int layerIndex;
        public StateEntry(string stateName, int layerIndex)
        {
            // Ensure the base layer is represented as 0.
            this.stateName = stateName;
            this.layerIndex = layerIndex < 0 ? 0 : layerIndex;
        }
    }

    public override void OnInspectorGUI()
    {
        // Draw default inspector fields.
        DrawDefaultInspector();

        AnimationStateSelector selector = (AnimationStateSelector)target;

        if (selector.animator == null)
        {
            EditorGUILayout.HelpBox("Please assign an Animator.", MessageType.Warning);
            return;
        }

        RuntimeAnimatorController rac = selector.animator.runtimeAnimatorController;
        if (rac == null)
        {
            EditorGUILayout.HelpBox("Animator does not have a Runtime Animator Controller.", MessageType.Warning);
            return;
        }

        AnimatorController ac = rac as AnimatorController;
        if (ac == null)
        {
            EditorGUILayout.HelpBox("Animator Controller is not of type AnimatorController.", MessageType.Warning);
            return;
        }

        // Collect all states from all layers.
        List<StateEntry> stateEntries = new List<StateEntry>();
        foreach (AnimatorControllerLayer layer in ac.layers)
        {
            foreach (ChildAnimatorState childState in layer.stateMachine.states)
            {
                // Only add unique state names.
                if (!stateEntries.Exists(x => x.stateName == childState.state.name))
                {
                    stateEntries.Add(new StateEntry(childState.state.name, layer.syncedLayerIndex < 0 ? 0 : layer.syncedLayerIndex));
                }
            }
        }

        if (stateEntries.Count == 0)
        {
            EditorGUILayout.HelpBox("No states found in the Animator Controller.", MessageType.Info);
            return;
        }

        // Build dropdown options with state name and layer info.
        string[] options = new string[stateEntries.Count];
        int currentIndex = 0;
        for (int i = 0; i < stateEntries.Count; i++)
        {
            options[i] = $"{stateEntries[i].stateName} (Layer {stateEntries[i].layerIndex})";
            if (stateEntries[i].stateName == selector.animationState)
            {
                currentIndex = i;
            }
        }

        // Display dropdown.
        int newIndex = EditorGUILayout.Popup("Animation State", currentIndex, options);
        if (newIndex != currentIndex)
        {
            // Update state and layer.
            selector.animationState = stateEntries[newIndex].stateName;
            selector.animationLayer = stateEntries[newIndex].layerIndex;

            // Immediately call animator.Play() so changes are visible in the Editor.
            selector.animator.Play(selector.animationState, selector.animationLayer < 0 ? 0 : selector.animationLayer, 0f);
            Debug.Log($"Editor changed state to: {selector.animationState} on layer: {selector.animationLayer}");

            EditorUtility.SetDirty(selector);

            selector.animator.Update(0.016f);  // Force a small time step update (approx. one frame at 60fps)
#if UNITY_EDITOR
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
#endif
        }
    }
}
#endif
