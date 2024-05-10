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
namespace HydrogenEmergency
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
        #region HydrogenEmergency

        /*
         * The constructor, called only once every session and always before any
         * other method is called. Use it to initialize your script.
         *
         * The constructor is optional and can be removed if not needed.
         *
         * It's recommended to set RuntimeInfo.UpdateFrequency here, which will
         * allow your script to run itself without a timer block.
         */
        public Program() { }

        /*
         * Called when the program needs to save its state. Use this method to save
         * your state to the Storage field or some other means.
         *
         * This method is optional and can be removed if not needed.
         */
        public void Save() { }

        public void Main(string argument, UpdateType updateSource)
        {
            // Get the main grid (the grid where this programmable block is located)
            var mainGrid = Me.CubeGrid;

            // Define the threshold for low hydrogen warning
            double hydrogenThresholdPercentage = 10.0;

            // Find all hydrogen tanks on the main grid
            List<IMyGasTank> hydrogenTanks = new List<IMyGasTank>();
            GridTerminalSystem.GetBlocksOfType<IMyGasTank>(
                hydrogenTanks,
                tank =>
                    tank.BlockDefinition.SubtypeName.Contains("Hydrogen")
                    && tank.CubeGrid == mainGrid
            );

            // Calculate total hydrogen storage capacity and current hydrogen volume
            double totalHydrogenCapacity = 0;
            double currentHydrogenVolume = 0;
            foreach (var tank in hydrogenTanks)
            {
                totalHydrogenCapacity += tank.Capacity;
                currentHydrogenVolume += tank.FilledRatio * tank.Capacity;
            }
            double currentHydrogenPercentage =
                (currentHydrogenVolume / totalHydrogenCapacity) * 100; // Convert to percentage

            // Find all lights on the main grid that contain "HydrogenSignal" in their name
            List<IMyInteriorLight> lights = new List<IMyInteriorLight>();
            GridTerminalSystem.GetBlocksOfType<IMyInteriorLight>(
                lights,
                light => light.CubeGrid == mainGrid && light.CustomName.Contains("HydrogenSignal")
            );

            // Define the color for low hydrogen (Red)
            Color lowHydrogenColor = Color.Red;

            // Update lights based on total hydrogen level
            foreach (var light in lights)
            {
                if (currentHydrogenPercentage < hydrogenThresholdPercentage)
                {
                    light.Color = lowHydrogenColor;
                    light.BlinkIntervalSeconds = 2; // Optional: Make lights blink
                    light.BlinkLength = 50; // Optional: Blink length percentage
                    light.Enabled = true; // Optional: Ensure the light is on
                }
                else
                {
                    light.Color = Color.White; // Resets to default color when hydrogen level is normal
                    light.BlinkIntervalSeconds = 0; // Stops blinking
                }
            }

            // Optional: Log or display the current hydrogen level
            Echo("Current Hydrogen Level: " + currentHydrogenPercentage.ToString("F2") + "%");
        }

        #endregion // HydrogenEmergency
    }
}
