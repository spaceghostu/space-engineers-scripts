using System;
using Sandbox.Game.EntityComponents;
// Space Engineers DLLs
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

/*
 * Must be unique per each script project.
 * Prevents collisions of multiple `class Program` declarations.
 * Will be used to detect the ingame script region, whose name is the same.
 */
namespace Prefixer
{
    /*
     * Do not change this declaration because this is the game requirement.
     */
    public sealed class Program : MyGridProgram
    {
        /*
         * Must be same as the namespace. Will be used for automatic script export.
         * The code inside this region is the ingame script.
         */
        #region Prefixer
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100; // Set the script to run every 100 game ticks
        }

        public void Main(string argument)
        {
            // Check if the argument is not empty
            if (string.IsNullOrWhiteSpace(argument))
            {
                Echo("No prefix provided. Please provide a prefix as an argument.");
                return;
            }

            // Get all blocks on the grid
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocks(blocks);

            // Prefix to be added
            string prefix = $"({argument}) ";

            // Iterate over all blocks
            foreach (var block in blocks)
            {
                // Check if the current name already has the prefix to avoid duplication
                if (!block.CustomName.StartsWith(prefix))
                {
                    block.CustomName = prefix + block.CustomName;
                }
            }

            Echo($"Prefix '{argument}' added to {blocks.Count} blocks.");
        }

        #endregion // Prefixer
    }
}
