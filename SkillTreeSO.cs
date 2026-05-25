using UnityEngine;

namespace SkillTree
{
    /// <summary>
    /// Defines the structure, pricing rules, and currency type of a linear skill tree.
    /// Acts as a config asset that designers can edit in the Inspector.
    /// At runtime, pass this to a <see cref="SkillTreeModel"/> to create
    /// a stateful instance that tracks unlock progress.
    /// </summary>
    [CreateAssetMenu(menuName = "Skill Tree/Tree", fileName = "SkillTree")]
    public class SkillTreeSO : ScriptableObject
    {
        [Tooltip("Ordered list of nodes. Nodes are unlocked from index 0 upward.")]
        [SerializeField] private SkillTreeNodeSO[] nodes;

        [Header("Currency")]
        [Tooltip("The currency type spent to unlock nodes in this tree.")]
        [SerializeField] private CurrencyTokenSO currencyToken;

        [Header("Pricing")]
        [Tooltip("When enabled, node prices are calculated from the curve below. " +
                 "Individual nodes can still override this with their own manual price.")]
        [SerializeField] private bool usePriceCurve;

        [Tooltip("Maps node index (x) to price (y). Only used when Use Price Curve is enabled.")]
        [SerializeField] private AnimationCurve priceCurve = AnimationCurve.Linear(0, 10, 10, 100);

        [Tooltip("Scales the curve output. Final price = curve(index) * multiplier.")]
        [SerializeField] private float priceMultiplier = 1f;

        public SkillTreeNodeSO[] Nodes => nodes;
        public CurrencyTokenSO CurrencyToken => currencyToken;

        /// <summary>
        /// Returns the price for a node at the given index.
        /// Respects per-node manual overrides, then falls back to the curve or the node's manual price.
        /// </summary>
        public float GetPrice(int nodeIndex)
        {
            if (nodes == null || nodeIndex < 0 || nodeIndex >= nodes.Length)
                return 0f;

            var node = nodes[nodeIndex];

            if (node.OverridePrice)
                return node.ManualPrice;

            if (usePriceCurve)
                return priceCurve.Evaluate(nodeIndex) * priceMultiplier;

            return node.ManualPrice;
        }
    }
}
