using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JamHub
{
    [HarmonyPatch]
    public static class Patches
    {
        /**
         * Listen for specific conditions to be set to true and lock onto the corresponding planet
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DialogueConditionManager), nameof(DialogueConditionManager.SetConditionState))]
        public static void DialogueLockOn(string conditionName, bool conditionState)
        {
            if(conditionState)
            {
                //Look for an ID matching the condition
                foreach(OtherMod mod in JamHub.instance.mods)
                {
                    if(conditionName.Equals(mod.ID))
                    {
                        OWRigidbody body = mod.planet.GetAttachedOWRigidbody();
                        if(body.IsTargetable())
                            Locator.GetPlayerBody().gameObject.GetComponent<ReferenceFrameTracker>().TargetReferenceFrame(body.GetReferenceFrame());

                        break;
                    }
                }
            }
        }
    }
}
