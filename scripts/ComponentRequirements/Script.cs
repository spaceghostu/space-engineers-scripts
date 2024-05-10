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
namespace ComponentRequirements
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
        #region ComponentRequirements
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100; // Set the script to run every 100 ticks
        }

        public void Main(string argument, UpdateType updateSource)
        {
            var lcd = GridTerminalSystem.GetBlockWithName("LCD Panel") as IMyTextPanel;
            if (lcd == null)
            {
                Echo("No LCD Panel found!");
                return;
            }

            string customData = lcd.CustomData;
            var blockComponents = ParseCustomData(customData);

            // Display or use the parsed data
            foreach (var block in blockComponents)
            {
                Echo($"Block: {block.Key}");
                foreach (var component in block.Value)
                {
                    Echo($" - {component.Key}: {component.Value}");
                }
            }
        }

        private Dictionary<string, Dictionary<string, int>> ParseCustomData(string data)
        {
            var result = new Dictionary<string, Dictionary<string, int>>();
            var lines = data.Split(';');
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(':');
                if (parts.Length != 2)
                    continue;

                var blockName = parts[0].Trim();
                var components = parts[1].Trim().Split(',');
                var componentDict = new Dictionary<string, int>();

                foreach (var component in components)
                {
                    var compParts = component.Split('=');
                    if (compParts.Length != 2)
                        continue;

                    var componentName = compParts[0].Trim();
                    int count;
                    if (int.TryParse(compParts[1].Trim(), out count))
                    {
                        componentDict[componentName] = count;
                    }
                }

                if (componentDict.Count > 0)
                {
                    result[blockName] = componentDict;
                }
            }

            return result;
        }

        #endregion // ComponentRequirements
    }
}
