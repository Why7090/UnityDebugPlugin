﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityDebugPlugin
{
    /// <summary>
    /// Class that allows mods to register keybindings. They can be changed
    /// by the user in the keymapper window and are automatically saved and
    /// loaded again.
    /// </summary>
    /// <remarks>
    /// Keybindings consist of a modifier and a trigger. The modifier has to be
    /// held down while the trigger is pressed once in order for the keybinding
    /// to detect it was pressed. The modifier is usually something like Ctrl or
    /// Shift, while the trigger is often just one of the letter keys.
    /// </remarks>
    public static class Keybindings
    {
        private static Dictionary<string, Dictionary<string, Key>> keybindings
          = new Dictionary<string, Dictionary<string, Key>>();

        private static string modName = "UnityDebugPlugin";

        /// <summary>
        /// Registers a new keybinding with the specified name and default value.
        /// If the keybinding does already exist, possibly because it was loaded
        /// from configuration automatically, the default value is ignored and
        /// the existing Key is returned instead.
        /// </summary>
        /// <param name="name">Name of the keybinding</param>
        /// <param name="defaultBinding">Default value of the keybinding</param>
        /// <returns>The added keybinding</returns>
        public static Key AddKeybinding (string name, Key defaultBinding)
        {
            if (keybindings.ContainsKey(modName))
            {
                if (keybindings[modName].ContainsKey(name))
                {
                    return keybindings[modName][name];
                }
                else
                {
                    keybindings[modName][name]
                      = new Key(defaultBinding.Modifier, defaultBinding.Trigger);
                }
            }
            else
            {
                keybindings[modName] = new Dictionary<string, Key>();
                keybindings[modName][name]
                  = new Key(defaultBinding.Modifier, defaultBinding.Trigger);
            }

            return keybindings[modName][name];
        }

        /// <summary>
        /// Gets the keybinding associated with the specified name.
        /// If the name is not a registered keybinding, an InvalidOperationException
        /// is thrown.
        /// </summary>
        /// <param name="name">Name of the keybinding</param>
        /// <returns>The registered keybinding.</returns>
        public static Key Get (string name)
        {
            if (keybindings.ContainsKey(modName)
              && keybindings[modName].ContainsKey(name))
            {
                return keybindings[modName][name];
            }

            throw new ArgumentException("No such keybinding: " + name);
        }

        internal static void SaveToConfig ()
        {
            foreach (var modPair in keybindings)
            {
                foreach (var bindingPair in modPair.Value)
                {
                    Configuration.SetString("keybinding:" + modPair.Key + ":"
                      + bindingPair.Key + ":modifier", bindingPair.Value.Modifier.ToString());
                    Configuration.SetString("keybinding:" + modPair.Key + ":"
                      + bindingPair.Key + ":trigger", bindingPair.Value.Trigger.ToString());
                }
            }
        }

        internal static void LoadFromConfig ()
        {
            var keys = Configuration.GetKeys();
            foreach (var key in keys)
            {
                if (key.StartsWith("keybinding:", StringComparison.InvariantCulture))
                {
                    var parts = key.Split(':');
                    if (parts.Length != 4)
                    {
                        Debug.LogError("Invalid keybinding format in configuration!");
                        continue;
                    }
                    var mod = parts[1];
                    var name = parts[2];
                    var type = parts[3];

                    if (type == "modifier")
                    {
                        var modifier = Configuration.GetString("keybinding:" + mod + ":"
                          + name + ":modifier", null);
                        var trigger = Configuration.GetString("keybinding:" + mod + ":"
                          + name + ":trigger", null);

                        if (modifier == null || trigger == null)
                        {
                            Debug.LogError("Invalid keybinding in configuration!");
                            continue;
                        }

                        if (!keybindings.ContainsKey(mod))
                        {
                            keybindings[mod] = new Dictionary<string, Key>();
                        }

                        if (keybindings[mod].ContainsKey(name))
                        {
                            var tempKey = new Key(modifier, trigger);
                            keybindings[mod][name].Modifier = tempKey.Modifier;
                            keybindings[mod][name].Trigger = tempKey.Trigger;
                        }
                        else
                        {
                            keybindings[mod][name] = new Key(modifier, trigger);
                        }
                    }
                }
            }
        }

        internal static Dictionary<string, Dictionary<string, Key>>
          GetAllKeybindings ()
        {
            return keybindings;
        }

    }
}
