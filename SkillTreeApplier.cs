using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SkillTree
{
    /// <summary>
    /// Connects a <see cref="SkillTreeModel"/> to gameplay systems by applying
    /// the effect of each node when it is unlocked. <br/><br/>
    ///
    /// Nodes (abilities, inventory, etc.) are handled in <see cref="ApplyCustomNode"/>,
    /// which is where game-specific logic should be added. Add one case per custom node type. <br/><br/>
    ///
    /// </summary>
    public class SkillTreeApplier : MonoBehaviour
    {
        [Header("Skill Tree")]
        [Tooltip("The config whose nodes this applier will react to.")]
        [SerializeField] private SkillTreeSO skillTreeConfig;

        [Header("Events (Optional)")]
        [Tooltip("Raised after any node effect is applied. Useful for triggering UI feedback.")]
        [SerializeField] private UnityEvent onUpgradeApplied;
        
        private SkillTreeModel _model;

        /// <summary>
        /// Initializes the applier with a runtime model and subscribes to its unlock events.
        /// Call this after constructing your <see cref="SkillTreeModel"/>.
        /// Already-unlocked nodes (e.g. from a loaded save) are replayed immediately.
        /// </summary>
        /// <param name="model">The runtime model to listen to.</param>
        /// <param name="playerStats">
        /// The player's stat model. Required only if the tree contains
        /// <see cref="StatSkillTreeNodeSO"/> nodes. Pass null for trees with no stat nodes.
        /// </param>
        public void Initialize(SkillTreeModel model)
        {
            _model = model;
            _model.OnNodeUnlocked += ApplyNode;
            ReapplyAll();
        }

        private void OnDestroy()
        {
            if (_model != null)
                _model.OnNodeUnlocked -= ApplyNode;
        }

        /// <summary>
        /// Applies the effect for the given node index.
        /// Stat nodes are handled generically; all other nodes route to <see cref="ApplyCustomNode"/>.
        /// </summary>
        private void ApplyNode(int nodeIndex)
        {
            var node = _model.Config.Nodes[nodeIndex];
            ApplyCustomNode(nodeIndex, node);
            onUpgradeApplied?.Invoke();
        }
        
        /// <summary>
        /// Handles non-stat node effects. Add one case per custom node type.
        /// The switch is on node type rather than index — stable against reordering nodes in the SO.
        ///
        /// NOTE: each new custom node type requires a case here. For trees with many node types
        /// or frequent additions, consider the ISkillTreeContext pattern instead — see the
        /// skill tree documentation for details.
        /// </summary>
        private void ApplyCustomNode(int nodeIndex, SkillTreeNodeSO node)
        {
            switch (node)
            {
                // Example: unlock an ability via a separate system.
                // case DashNodeSO dashNode:
                //     abilityManager.Unlock(AbilityType.Dash);
                //     break;

                // Example: increase max inventory size.
                // case InventoryNodeSO inventoryNode:
                //     inventoryManager.IncreaseCapacity(inventoryNode.ExtraSlots);
                //     break;

                default:
                    Debug.LogWarning($"[SkillTreeApplier] No custom effect defined for node type {node.GetType().Name} ({node.Title}).");
                    break;
            }
        }
        
        /// <summary>
        /// Replays all already-unlocked nodes on initialization.
        /// Ensures effects are correctly applied when loading from a save.
        /// </summary>
        private void ReapplyAll()
        {
            for (int i = 0; i < _model.NextUnlockIndex; i++)
                ApplyNode(i);
        }

        #region Stat Integration Example

        // Applies a stat modifier from a StatSkillTreeNodeSO to the player's stats.
        // Stores the modifier reference for potential future removal.

        // private void ApplyStatNode(int nodeIndex, StatSkillTreeNodeSO statNode)
        // {
        //     if (_playerStats == null)
        //     {
        //         Debug.LogError($"[SkillTreeApplier] Node {nodeIndex} ({statNode.Title}) is a stat node but no StatsModel was passed to Initialize().");
        //         return;
        //     }
        //
        //     var modifier = statNode.CreateModifier(this);
        //     _playerStats.GetStat(statNode.StatType).AddModifier(modifier);
        //     _appliedModifiers[nodeIndex] = modifier;
        // }
        
        // Removes the stat modifier applied by a specific node, if one exists.
        // Useful if a refund or reset system is added later.

        // public void RemoveStatModifier(int nodeIndex)
        // {
        //     if (!_appliedModifiers.TryGetValue(nodeIndex, out var modifier)) return;
        //
        //     var node = _model.Config.Nodes[nodeIndex] as StatSkillTreeNodeSO;
        //     if (node == null) return;
        //
        //     _playerStats.GetStat(node.StatType).RemoveModifier(modifier);
        //     _appliedModifiers.Remove(nodeIndex);
        // }

        #endregion
    }
}