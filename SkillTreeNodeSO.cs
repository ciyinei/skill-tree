using UnityEngine;

namespace SkillTree
{
    /// <summary>
    /// Defines the static data for a single node in a skill tree.
    /// Contains display information and an optional manual price override.
    /// The actual effect of unlocking this node is handled externally by a SkillTreeApplier.
    /// </summary>
    [CreateAssetMenu(menuName = "Skill Tree/Node", fileName = "SkillTreeNode")]
    public class SkillTreeNodeSO : ScriptableObject
    {
        [Header("Display")]
        [Tooltip("Icon shown in the skill tree UI.")]
        [SerializeField] private Sprite icon;

        [Tooltip("Short name displayed on the node.")]
        [SerializeField] private string title;

        [TextArea(2, 6)]
        [Tooltip("Description of what this upgrade does.")]
        [SerializeField] private string description;

        [Header("Pricing")]
        [Tooltip("When enabled, this node uses a fixed price instead of the tree's price curve.")]
        [SerializeField] private bool overridePrice;

        [Tooltip("Manual price used when Override Price is enabled.")]
        [SerializeField] private float manualPrice;

        public Sprite Icon => icon;
        public string Title => title;
        public string Description => description;
        public bool OverridePrice => overridePrice;
        public float ManualPrice => manualPrice;
    }
}