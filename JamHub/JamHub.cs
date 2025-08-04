using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace JamHub
{
    public class JamHub : ModBehaviour
    {
        public INewHorizons newHorizons = null;
        public List<OtherMod> mods;

        //Static
        public static JamHub instance;

        private void Start()
        {

            // Get the New Horizons API and load configs
            newHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            newHorizons.LoadConfigs(this);

            //Set ourselves up to do stuff when the system loads
            UnityEvent<string> loadCompleteEvent = newHorizons.GetStarSystemLoadedEvent();
            loadCompleteEvent.AddListener(JamSystemHelper.PrepSystem);

            //Make all of the patches
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            //Set up things to happen once the player wakes up
            GlobalMessenger.AddListener("WakeUp", OnceAwake);

            // Say that we're done
            instance = this;
            ModHelper.Console.WriteLine($"My mod {nameof(JamHub)} is loaded!", MessageType.Success);
        }

        public bool IsInValidSystem()
        {
            if (newHorizons.GetCurrentStarSystem().Equals("Jam3") || newHorizons.GetCurrentStarSystem().Equals("Jam5"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * Set up Trifid's dialogue stuff
         */
        private void OnceAwake()
        {
            //Return early if in the wrong system
            if (IsInValidSystem())
                return;

            //Trigger the condition
            if (Locator.GetShipLogManager().IsFactRevealed("EH_PHOSPHORS_X1"))
                PlayerData._currentGameSave.SetPersistentCondition("ECHO_HIKE_DONE", true);
            JamHub.DebugPrint("sanity check");
        }

        public static void DebugPrint(string message)
        {
            instance.ModHelper.Console.WriteLine(message);
        }
    }
}
