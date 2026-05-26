using System;
using UnityEngine;

namespace SkillTree
{
    /// <summary>
    /// Runtime state for a skill tree. Created from a <see cref="SkillTreeSO"/> config
    /// and tracks which nodes have been unlocked.
    /// Currency logic is injected via delegates.
    /// Does not modify the source SO — each instance is independent.
    /// </summary>
    public class SkillTreeModel
    {
        /// <summary>The config this model was created from.</summary>
        public SkillTreeSO Config { get; }

        /// <summary>Index of the next node available to unlock. Equals node count when the tree is complete.</summary>
        public int NextUnlockIndex { get; private set; }

        /// <summary>True when all nodes in the tree have been unlocked.</summary>
        public bool IsComplete => NextUnlockIndex >= Config.Nodes.Length;

        /// <summary>
        /// Raised when a node is successfully unlocked.
        /// Returns the index of the unlocked node.
        /// </summary>
        public event Action<int> OnNodeUnlocked;

        private readonly Func<float, bool> CanAfford;
        private readonly Action<float> OnSpendCurrency;

        /// <param name="config">The SO that defines this tree's nodes and pricing.</param>
        /// <param name="canAfford">
        /// Returns true if the player can afford the given amount.
    
        /// </param>
        /// <param name="spendCurrency">
        /// Deducts the given amount from the player's balance.
        /// </param>
        /// <param name="unlockedCount">
        /// Number of nodes already unlocked (e.g. loaded from a save).
        /// </param>
        public SkillTreeModel(
            SkillTreeSO config,
            Func<float, bool> canAfford,
            Action<float> spendCurrency,
            int unlockedCount = 0)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            CanAfford = canAfford ?? throw new ArgumentNullException(nameof(canAfford));
            OnSpendCurrency = spendCurrency ?? throw new ArgumentNullException(nameof(spendCurrency));
            NextUnlockIndex = Mathf.Clamp(unlockedCount, 0, config.Nodes.Length);
        }

        /// <summary>
        /// Returns the price of the next node to unlock, or 0 if the tree is complete.
        /// </summary>
        public float GetNextPrice() => IsComplete ? 0f : Config.GetPrice(NextUnlockIndex);

        /// <summary>
        /// Returns true if the injected affordability check passes for the next node's price.
        /// </summary>
        public bool CanUnlockNext() => !IsComplete && CanAfford(GetNextPrice());

        /// <summary>
        /// Attempts to unlock the next node in the tree.
        /// Delegates affordability check and currency deduction to the injected functions.
        /// </summary>
        /// <returns>True if the node was unlocked, false if insufficient funds or tree is complete.</returns>
        public bool TryUnlockNext()
        {
            if (!CanUnlockNext()) return false;

            OnSpendCurrency(GetNextPrice());

            int unlockedIndex = NextUnlockIndex;
            NextUnlockIndex++;
            OnNodeUnlocked?.Invoke(unlockedIndex);

            return true;
        }

        /// <summary>
        /// Returns true if the node at the given index has been unlocked.
        /// </summary>
        public bool IsUnlocked(int nodeIndex) => nodeIndex < NextUnlockIndex;
        
        public float GetNodePrice(int nodeIndex) => Config.GetPrice(nodeIndex);
    }
}