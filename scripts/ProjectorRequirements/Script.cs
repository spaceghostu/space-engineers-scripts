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

namespace ProjectorRequirements
{
    public sealed class Program : MyGridProgram
    {
        #region ProjectorRequirements

        public void Save() { }

        // public Program()
        // {
        //     Runtime.UpdateFrequency = UpdateFrequency.Update100; // Adjust as necessary for your setup
        // }

        void Main()
        {
            string blockTypeName = "LandingGear";
            var blockType = (MyCubeBlockDefinition)
                MyDefinitionManager.Static.GetDefinition(
                    new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), blockTypeName)
                );

            if (blockType == null)
            {
                Echo("Block type not found.");
                return;
            }

            Echo("Components required for " + blockTypeName + ":");

            foreach (var component in blockType.Components)
            {
                Echo(component.Definition.DisplayNameText + ": " + component.Count);
            }
        }

        #endregion // ProjectorRequirements
    }
}
