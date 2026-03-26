using System;
using UnityEngine;

namespace SkillTree
{
    /// <summary>
    /// Runtime state for a skill tree. Created from a <see cref="SkillTreeSO"/> config
    /// and tracks which nodes have been unlocked.
    /// Currency is managed externally via <see cref="ICurrencyManager"/>.
    /// Does not modify the source SO — each instance is independent.
    /// </summary>
    public class SkillTreeModel
    {
        /// <summary>The config this model was created from.</summary>
        public SkillTreeSO Config { get; }

        /// <summary>The currency type used to pay for unlocks in this tree.</summary>
        public CurrencyType CurrencyType { get; }

        /// <summary>Index of the next node available to unlock. Equals node count when the tree is complete.</summary>
        public int NextUnlockIndex { get; private set; }

        /// <summary>True when all nodes in the tree have been unlocked.</summary>
        public bool IsComplete => NextUnlockIndex >= Config.Nodes.Length;

        /// <summary>
        /// Raised when a node is successfully unlocked.
        /// Returns the index of the unlocked node.
        /// </summary>
        public event Action<int> OnNodeUnlocked;

        private readonly ICurrencyManager _currencyManager;

        /// <param name="config">The SO that defines this tree's nodes and pricing.</param>
        /// <param name="currencyManager">The currency system used to pay for unlocks.</param>
        /// <param name="currencyType">The currency type this tree costs.</param>
        /// <param name="unlockedCount">
        /// Number of nodes already unlocked (e.g. loaded from a save).
        /// </param>
        public SkillTreeModel(SkillTreeSO config, ICurrencyManager currencyManager, int unlockedCount = 0)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            _currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));
            CurrencyType = config.CurrencyType;
            NextUnlockIndex = Mathf.Clamp(unlockedCount, 0, config.Nodes.Length);
        }

        /// <summary>
        /// Returns the price of the next node to unlock, or 0 if the tree is complete.
        /// </summary>
        public float GetNextPrice() => IsComplete ? 0f : Config.GetPrice(NextUnlockIndex);

        /// <summary>
        /// Returns true if the currency manager has enough balance to unlock the next node.
        /// </summary>
        public bool CanUnlockNext() => !IsComplete && _currencyManager.CanAfford(CurrencyType, GetNextPrice());

        /// <summary>
        /// Attempts to unlock the next node in the tree.
        /// Delegates the affordability check and currency deduction to <see cref="ICurrencyManager"/>.
        /// </summary>
        /// <returns>True if the node was unlocked, false if insufficient funds or tree is complete.</returns>
        public bool TryUnlockNext()
        {
            if (!CanUnlockNext()) return false;

            _currencyManager.SpendCurrency(CurrencyType, GetNextPrice());

            int unlockedIndex = NextUnlockIndex;
            NextUnlockIndex++;
            OnNodeUnlocked?.Invoke(unlockedIndex);

            return true;
        }

        /// <summary>
        /// Returns true if the node at the given index has been unlocked.
        /// </summary>
        public bool IsUnlocked(int nodeIndex) => nodeIndex < NextUnlockIndex;
    }
}