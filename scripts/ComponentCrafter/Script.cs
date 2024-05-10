using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
namespace ComponentCrafter
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
        #region ComponentCrafter
        // Names of the assembler and cargo container
        private string assemblerName = "Assembler";
        private string cargoName = "Cargo";

        // Minimum stock levels for components
        private Dictionary<string, int> minStockLevels = new Dictionary<string, int>()
        {
            { "SteelPlate", 100 },
        };

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100; // Run the script every 100 ticks
        }

        public void Main(string argument, UpdateType updateSource)
        {
            var assembler = GridTerminalSystem.GetBlockWithName(assemblerName) as IMyAssembler;
            if (assembler == null)
            {
                Echo("Assembler not found!");
                return;
            }

            var cargo = GridTerminalSystem.GetBlockWithName(cargoName) as IMyCargoContainer;
            if (cargo == null)
            {
                Echo("Cargo container not found!");
                return;
            }

            var inventoryCargo = cargo.GetInventory();
            var inventoryAsm = assembler.OutputInventory;

            Dictionary<string, int> currentStock = new Dictionary<string, int>();

            // Sum up items in cargo and assembler storage
            SumUpInventoryItems(inventoryCargo, currentStock);
            SumUpInventoryItems(inventoryAsm, currentStock);

            // Account for items in assembler's queue
            List<MyProductionItem> queueItems = new List<MyProductionItem>();
            assembler.GetQueue(queueItems);
            foreach (var item in queueItems)
            {
                string itemName = item
                    .BlueprintId.ToString()
                    .Substring(item.BlueprintId.ToString().LastIndexOf('/') + 1);
                if (currentStock.ContainsKey(itemName))
                {
                    currentStock[itemName] += (int)item.Amount;
                }
                else
                {
                    currentStock[itemName] = (int)item.Amount;
                }
            }

            // Check and queue missing items
            foreach (var kvp in minStockLevels)
            {
                int currentAmount = currentStock.ContainsKey(kvp.Key) ? currentStock[kvp.Key] : 0;
                if (currentAmount < kvp.Value)
                {
                    int missingAmount = kvp.Value - currentAmount;
                    MyDefinitionId blueprintId = MyDefinitionId.Parse(
                        "MyObjectBuilder_BlueprintDefinition/" + kvp.Key
                    );
                    // Explicitly cast the integer to MyFixedPoint
                    MyFixedPoint amountToQueue = (MyFixedPoint)missingAmount;
                    assembler.AddQueueItem(blueprintId, amountToQueue);
                    Echo($"Queued {missingAmount} of {kvp.Key}");
                }
            }
        }

        // Helper method to sum up items from a given inventory
        private void SumUpInventoryItems(
            IMyInventory inventory,
            Dictionary<string, int> stockDictionary
        )
        {
            List<MyInventoryItem> items = new List<MyInventoryItem>();
            inventory.GetItems(items);

            foreach (var item in items)
            {
                string typeName = item.Type.SubtypeId.ToString();
                if (stockDictionary.ContainsKey(typeName))
                {
                    stockDictionary[typeName] += (int)item.Amount;
                }
                else
                {
                    stockDictionary[typeName] = (int)item.Amount;
                }
            }
        }

        #endregion // ComponentCrafter
    }
}
