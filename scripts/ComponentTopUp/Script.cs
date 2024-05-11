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
namespace ComponentTopUp
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
        #region ComponentTopUp
        // public Program()
        // {
        //     Runtime.UpdateFrequency = UpdateFrequency.Update100; // Run the script every 100 updates (approx. every 1.6 seconds)
        // }
        public void Main(string argument, UpdateType updateSource)
        {
            var targetItemAmount = 100;
            IMyCargoContainer cargoContainer =
                GridTerminalSystem.GetBlockWithName("Cargo Container Name") as IMyCargoContainer;
            IMyAssembler assembler =
                GridTerminalSystem.GetBlockWithName("Assembler Name") as IMyAssembler;

            if (cargoContainer == null)
            {
                Echo("Error: Cargo container not found!");
                return;
            }

            if (assembler == null)
            {
                Echo("Error: Assembler not found!");
                return;
            }

            Echo("Both cargo container and assembler found.");

            var inventory = cargoContainer.GetInventory();
            var items = new List<MyInventoryItem>();
            inventory.GetItems(items);
            Echo($"Found {items.Count} items in the container.");

            // Gather currently queued items
            List<MyProductionItem> currentQueue = new List<MyProductionItem>();
            assembler.GetQueue(currentQueue);
            var currentlyQueued = new HashSet<string>();
            foreach (var queuedItem in currentQueue)
            {
                currentlyQueued.Add(NormalizeName(queuedItem.BlueprintId.SubtypeName));
                Echo($"Already in queue: {NormalizeName(queuedItem.BlueprintId.SubtypeName)}");
            }

            Dictionary<string, int> neededComponents = new Dictionary<string, int>
            {
                { "ComputerComponent", targetItemAmount },
                { "Display", targetItemAmount },
                { "GirderComponent", targetItemAmount },
                { "InteriorPlate", targetItemAmount },
                { "LargeTube", targetItemAmount },
                { "MetalGrid", targetItemAmount },
                { "MotorComponent", targetItemAmount },
                { "SmallTube", targetItemAmount },
                { "SteelPlate", targetItemAmount },
            };

            // Check current stock and decide how much to queue
            foreach (var item in items)
            {
                string itemType = NormalizeName(item.Type.SubtypeId.ToString());
                Echo($"Checking item: {itemType}, Amount: {item.Amount}");

                if (
                    neededComponents.ContainsKey(
                        itemType + (itemType.EndsWith("Component") ? "" : "Component")
                    ) && !currentlyQueued.Contains(itemType)
                )
                {
                    int needed = targetItemAmount - (int)item.Amount;
                    if (needed > 0)
                    {
                        Echo($"Queueing {needed} of {itemType}");
                        MyDefinitionId blueprintId = MyDefinitionId.Parse(
                            "MyObjectBuilder_BlueprintDefinition/"
                                + itemType
                                + (itemType.EndsWith("Component") ? "" : "Component")
                        );
                        assembler.AddQueueItem(blueprintId, (VRage.MyFixedPoint)needed);
                    }
                    neededComponents[
                        itemType + (itemType.EndsWith("Component") ? "" : "Component")
                    ] = Math.Max(0, needed);
                }
            }

            // Queue items not present in the cargo at all and not already queued
            foreach (var component in neededComponents)
            {
                if (
                    component.Value == targetItemAmount
                    && !currentlyQueued.Contains(component.Key.Replace("Component", ""))
                )
                {
                    MyDefinitionId blueprintId = MyDefinitionId.Parse(
                        "MyObjectBuilder_BlueprintDefinition/" + component.Key
                    );
                    Echo($"Queueing {component.Value} of {component.Key} not present in cargo.");
                    assembler.AddQueueItem(blueprintId, (VRage.MyFixedPoint)component.Value);
                }
            }

            Echo("Script execution completed successfully.");
        }

        // Helper method to normalize item names
        private string NormalizeName(string name)
        {
            return name.Replace("Component", "");
        }
        #endregion // ComponentTopUp
    }
}
