using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Reflection;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace JamHub
{
    public class JamHub : ModBehaviour
    {
        public INewHorizons newHorizons = null;
        public OtherMod[] mods = new OtherMod[0];

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

        /**
         * Generate the list of NH addons
         */
        public void GenModList()
        {
            //Retrieve the ID's of all the NH addons
            string[] idList = newHorizons.GetInstalledAddons();
            mods = new OtherMod[idList.Length - 2];
            int ignored = 0;

            //Loop through the retrieved ID's
            for (int i = 0; i < idList.Length; i++)
            {
                //Blacklist NH and the Jam3 mod
                if (idList[i].Equals("xen.NewHorizons") || idList[i].Equals("xen.ModJam3"))
                {
                    ignored++;
                    continue;
                }

                //Make a new entry in the array
                IModManifest manifest = ModHelper.Interaction.TryGetMod(idList[i]).ModHelper.Manifest;
                mods[i - ignored] = new OtherMod(idList[i], manifest.Name, manifest.Author);
            }
        }

        /**
         * Set up Trifid's dialogue stuff
         */
        private void OnceAwake()
        {
            //Return early if in the wrong system
            if (!newHorizons.GetCurrentStarSystem().Equals("Jam3"))
                return;

            //Trigger the condition
            if (Locator.GetShipLogManager().IsFactRevealed("EH_PHOSPHORS_X1"))
                PlayerData._currentGameSave.SetPersistentCondition("ECHO_HIKE_DONE", true);
            JamHub.DebugPrint("sanity check");
        }

        private void Update()
        {
            if (Keyboard.current[Key.N].wasPressedThisFrame)
            {
                foreach(OtherMod mod in mods)
                {
                    ModHelper.Console.WriteLine(mod.Name + " : " + mod.Author);
                }
            }
        }

        public static void DebugPrint(string message)
        {
            instance.ModHelper.Console.WriteLine(message);
        }
    }
}
