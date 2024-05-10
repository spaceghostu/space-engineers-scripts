using System;
using System;
using System.Collections.Generic;
using Sandbox.Game.EntityComponents;
// Space Engineers DLLs
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

/*
 * Must be unique per each script project.
 * Prevents collisions of multiple `class Program` declarations.
 * Will be used to detect the ingame script region, whose name is the same.
 */
namespace Stats
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
        #region Stats
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100; // Set the script to run every 100 game ticks
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // Find the LCD panel by name
            IMyTextPanel lcd =
                GridTerminalSystem.GetBlockWithName("(Mother) Stats LCD") as IMyTextPanel;
            if (lcd == null)
            {
                Echo("Error: LCD Panel not found");
                return;
            }

            // Retrieve all hydrogen tanks on the grid
            List<IMyGasTank> hydrogenTanks = new List<IMyGasTank>();
            GridTerminalSystem.GetBlocksOfType<IMyGasTank>(hydrogenTanks);
            float totalHydrogenCapacity = 0f;
            float currentHydrogen = 0f;

            foreach (var tank in hydrogenTanks)
            {
                totalHydrogenCapacity += (float)tank.Capacity;
                currentHydrogen += (float)(tank.FilledRatio * tank.Capacity);
            }

            // Retrieve all cargo containers on the grid
            List<IMyCargoContainer> cargoContainers = new List<IMyCargoContainer>();
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoContainers);
            float totalCargoCapacity = 0f;
            float currentCargo = 0f;

            foreach (var container in cargoContainers)
            {
                var inventory = container.GetInventory();
                totalCargoCapacity += (float)inventory.MaxVolume;
                currentCargo += (float)inventory.CurrentVolume;
            }

            // Calculate percentages for display
            int hydrogenPercent = (int)((currentHydrogen / totalHydrogenCapacity) * 100);
            int cargoPercent = (int)((currentCargo / totalCargoCapacity) * 100);

            // Generate progress bars for hydrogen and cargo
            string hydrogenBar = CreateProgressBar(hydrogenPercent, 30); // Set bar size to 30 characters
            string cargoBar = CreateProgressBar(cargoPercent, 30); // Set bar size to 30 characters

            // Display the information on the LCD
            lcd.WriteText(
                $"Hydrogen      {currentHydrogen:N0} / {totalHydrogenCapacity:N0}\n{hydrogenBar} {hydrogenPercent}%\n\n",
                false
            );
            lcd.WriteText(
                $"Cargo      {currentCargo:N0} / {totalCargoCapacity:N0}\n{cargoBar} {cargoPercent}%",
                true
            );
        }

        private string CreateProgressBar(int percent, int barSize)
        {
            int fillSize = (percent * barSize) / 100;
            return new string('|', fillSize) + new string('-', barSize - fillSize);
        }

        #endregion // Stats
    }
}
