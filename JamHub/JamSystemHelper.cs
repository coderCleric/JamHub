using JamHub.orrery;
using NewHorizons;
using NewHorizons.Components.Orbital;
using NewHorizons.External;
using NewHorizons.Handlers;
using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.ModHelper;
using System.Collections.Generic;
using UnityEngine;

namespace JamHub
{
    public static class JamSystemHelper
    {
        private static string jsonStr =
            "{\r\n        \"parentPath\": \"Sector/jamplanet/computer_area/mod_computer/Capsule\",\r\n        \"rename\": \"computer_dialogue\",\r\n        \"isRelativeToParent\": true,\r\n        \"radius\": 0.5,\r\n        \"range\": 2\r\n      }";

        public static bool IsInValidSystem() => IsInValidSystem(JamHub.instance.newHorizons.GetCurrentStarSystem());
        public static bool IsInValidSystem(string s) => IsInJam3(s) || IsInJam5(s);

        public static bool IsInJam3() => IsInJam3(JamHub.instance.newHorizons.GetCurrentStarSystem());
        public static bool IsInJam3(string s) => s.Equals("Jam3");

        public static bool IsInJam5() => IsInJam5(JamHub.instance.newHorizons.GetCurrentStarSystem());
        public static bool IsInJam5(string s) => s.Equals("Jam5");

        public static bool IsFactRevealed(string id)
        {
            return (PlayerData.GetShipLogFactSave(id)?.revealOrder ?? -1) > -1;
        }

        public static void PrepSystem(string s)
        {
            if (IsInJam3(s))
            {
                JamHub.DebugPrint("Jam 3 system detected, doing prep");
                PrepJam3();
                return;
            }
            
            if (IsInJam5(s))
            {
                JamHub.DebugPrint("Jam 5 system detected, doing prep");
                PrepJam5();
                return;
            }
        }

        /**
         * Does all of the changes needed for the jam 3 version to work
         */
        private static void PrepJam3()
        {
            //Get the sector transform of our planet
            Transform sectorTF = GameObject.Find("ModJamHub_Body/Sector").transform;

            //Do all of the general prep
            GeneralPrep(sectorTF);

            //Go through each valid planet and make a mod object for it
            JamHub.instance.mods = new List<OtherMod>();
            foreach (NHAstroObject astroObj in Component.FindObjectsOfType<NHAstroObject>())
            {
                //Make sure it's a valid planet
                if (astroObj._type == AstroObject.Type.Planet && astroObj.gameObject.GetComponentInChildren<DarkBrambleRepelVolume>() == null &&
                    astroObj._primaryBody != null && astroObj._primaryBody._customName.Equals("Jam 3 Sun"))
                {
                    IModManifest manifest = JamHub.instance.ModHelper.Interaction.TryGetMod(astroObj.modUniqueName).ModHelper.Manifest;
                    JamHub.instance.mods.Add(new OtherMod(astroObj.modUniqueName, manifest.Name, manifest.Author, astroObj.gameObject));
                }
            }

            //Make the orrery
            Orrery orrery = sectorTF.Find("jamplanet/computer_area/Orrery").gameObject.AddComponent<Jam3Orrery>();

            //Make the computer work
            JamHub.instance.newHorizons.CreateDialogueFromXML("jam3hubcomputer", MakeComputerXML(JamHub.instance.mods), jsonStr, sectorTF.parent.gameObject);
        }

        /**
         * Does all of the changes needed for the jam 5 version to work
         */
        private static void PrepJam5()
        {
            //Get the sector transform of our planet
            Transform sectorTF = GameObject.Find("ModJamHub_Body/Sector").transform;

            //Do all of the general prep
            GeneralPrep(sectorTF);

            //Find the name of each mini-system's center
            List<NewHorizonsBody> bodyList = Main.BodyDict["Jam5"];
            List<string> nameList = new List<string>();
            foreach (NewHorizonsBody body in bodyList)
            {
                // stolen code from Heliostudy, which stole from the Jam5 mod
                var dict = new Dictionary<string, object>();
                if (body.Config.extras is JObject jObject)
                {
                    dict = jObject.ToObject<Dictionary<string, object>>();
                }

                if (dict.TryGetValue("isCenterOfMiniSystem", out var isCenter) && isCenter is bool isCenterBool && isCenterBool)
                {
                    nameList.Add(body.Config.name);
                }
            }

            //Compose a list of other mods
            JamHub.instance.mods = new List<OtherMod>(); 
            foreach (NHAstroObject astroObj in Component.FindObjectsOfType<NHAstroObject>())
            {
                if (nameList.Contains(astroObj._customName))
                {
                    IModManifest manifest = JamHub.instance.ModHelper.Interaction.TryGetMod(astroObj.modUniqueName).ModHelper.Manifest;
                    JamHub.instance.mods.Add(new OtherMod(astroObj.modUniqueName, manifest.Name, manifest.Author, astroObj.gameObject));
                }
            }

            //Make the orrery
            Orrery orrery = sectorTF.Find("jamplanet/computer_area/Orrery").gameObject.AddComponent<Jam5Orrery>();

            //Make the computer work
            JamHub.instance.newHorizons.CreateDialogueFromXML("jam5hubcomputer", MakeComputerXML(JamHub.instance.mods), jsonStr, sectorTF.parent.gameObject);
        }

        /**
         * Does the prep that will be needed in every system with our planet
         */
        private static void GeneralPrep(Transform sectorTF)
        {
            //Make the plaques look at the actual displays
            //Get an array of all the roots
            Transform[] platforms = {
                sectorTF.Find("jamplanet/pop_mods_area/showroom/building"),
                sectorTF.Find("jamplanet/pop_mods_area/showroom_storymods/building"),
                sectorTF.Find("jamplanet/memorial_area/showroom/building"),
                //sectorTF.Find("jamplanet/pop_nh_area/showroom/building"),
                sectorTF.Find("jamplanet/upcoming_nh_area/showroom/building"),
                sectorTF.Find("jamplanet/prev_jam_area/showroom_jam1/building"),
                sectorTF.Find("jamplanet/prev_jam_area/showroom_jam2/building"),
                sectorTF.Find("jamplanet/prev_jam_area/showroom_jam3/building"),
                sectorTF.Find("jamplanet/prev_jam_area/showroom_jam4/building"),
                sectorTF.Find("jamplanet/prev_jam_area/showroom_jam5/building")
            };

            //Loop through each, looking for names with "display"
            foreach (Transform platform in platforms)
            {
                foreach (Transform tf in platform)
                {
                    if (tf.name.Contains("display"))
                    {
                        //We found one, set the dialogue to look where we want
                        tf.gameObject.GetComponentInChildren<CharacterDialogueTree>()._attentionPoint = tf.Find("image");
                    }
                }
            }

            //Change all of the animator speeds for the modder NPC's so its offset
            foreach (Animator animator in sectorTF.Find("jamplanet/modder_shack_area/moddershack/building/modders").gameObject.GetComponentsInChildren<Animator>())
            {
                float newSpeed = Random.Range(0.9f, 1.1f);
                int invScale = Random.Range(0, 2);

                //Set the random speed
                animator.speed = newSpeed;

                //Randomly swap hands
                int xScale = 1;
                if (invScale == 1)
                    xScale = -1;
                animator.transform.localScale = new Vector3(xScale, 1, 1);

                //If it's trifid, set up their controller
                if (animator.transform.parent.name.Equals("Trifid"))
                {
                    animator.transform.parent.gameObject.AddComponent<TrifidController>();
                }

                //If it's piggy, set up their controller
                if (animator.transform.parent.name.Equals("MegaPiggy"))
                {
                    animator.transform.parent.gameObject.AddComponent<MegaPiggyController>();
                }
            }
        }

        /**
         * Make the dialogue xml for the computer from the list of planets
         */
        private static string MakeComputerXML(List<OtherMod> mods)
        {
            //Figure out how many full pages we have, and how many are on the partial
            int modsPerPage = 3;
            int fullPages = (int)Mathf.Floor(mods.Count / modsPerPage);
            int partialPage = mods.Count - (fullPages * modsPerPage);

            //Construct the header
            string retstr = "<DialogueTree xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"https://raw.githubusercontent.com/xen-42/outer-wilds-new-horizons/main/NewHorizons/Schemas/dialogue_schema.xsd\">\n";
            retstr += "<NameField>Orrery Computer</NameField>\n";

            //Construct the intro dialogue
            retstr += "<DialogueNode>\r\n    <Name>INITIAL</Name>\r\n    <EntryCondition>DEFAULT</EntryCondition>\r\n    <Dialogue>\r\n      <Page>Hey there! I'm a computer dedicated to scanning and tracking all of the different mods in this system.</Page>\r\n      <Page>You can use my orrery above us to track the relative positions of different bodies.</Page>\r\n      <Page>I can even give you a bit of information on the different mods that are installed, and lock on to the main bodies of each mod!</Page>\r\n    </Dialogue>\r\n    <DialogueTarget>PAGE0</DialogueTarget>\r\n <RevealFacts><FactID>COMPUTER_DETAILS_FACT_CCL</FactID></RevealFacts> <SetCondition>ORRERY_COMPUTER_TALKED</SetCondition> </DialogueNode>\n";

            //Construct the full pages
            int page;
            for(page = 0; page < fullPages; page++)
            {
                //Make the header for the dialogue node
                retstr += "<DialogueNode>\n";
                retstr += "<Name>PAGE" + page + "</Name>\n";
                if(page == 0)
                    retstr += "<EntryCondition>ORRERY_COMPUTER_TALKED</EntryCondition>";

                //Make the dialogue line
                retstr += "<Dialogue>\n";
                retstr += "<Page>Page " + (page + 1) + "</Page>\n";
                retstr += "</Dialogue>\n";
                retstr += "<DialogueOptionsList>\n";

                //Make a dialogue option for each mod
                for(int i = 0; i < modsPerPage; i++)
                {
                    retstr += "<DialogueOption>\n";
                    retstr += "<Text>" + mods[(page * modsPerPage) + i].Name + "</Text>\n";
                    retstr += "<DialogueTarget>" + mods[(page * modsPerPage) + i].ID + "</DialogueTarget>\n";
                    retstr += "</DialogueOption>";
                }

                //Put the closing dialogue options
                if (page > 0)
                    retstr += "<DialogueOption>\r\n        <Text>Previous Page</Text>\r\n        <DialogueTarget>PAGE" + (page - 1) + "</DialogueTarget>\r\n      </DialogueOption>\n";
                if (page < fullPages - 1 || partialPage > 0)
                    retstr += "<DialogueOption>\r\n        <Text>Next Page</Text>\r\n        <DialogueTarget>PAGE" + (page + 1) + "</DialogueTarget>\r\n      </DialogueOption>\n";
                retstr += "<DialogueOption>\r\n        <Text>Exit</Text>\r\n      </DialogueOption>\n";
                retstr += "</DialogueOptionsList>\r\n\t</DialogueNode>\n";
            }

            //Construct the partial page
            if(partialPage > 0)
            {
                //Make the header for the dialogue node
                retstr += "<DialogueNode>\n";
                retstr += "<Name>PAGE" + page + "</Name>\n";
                if (fullPages == 0)
                    retstr += "<EntryCondition>ORRERY_COMPUTER_TALKED</EntryCondition>";

                //Make the dialogue line
                retstr += "<Dialogue>\n";
                retstr += "<Page>Page " + (page + 1) + "</Page>\n";
                retstr += "</Dialogue>\n";
                retstr += "<DialogueOptionsList>\n";

                //Make a dialogue option for each mod
                for (int i = 0; i < partialPage; i++)
                {
                    retstr += "<DialogueOption>\n";
                    retstr += "<Text>" + mods[(page * modsPerPage) + i].Name + "</Text>\n";
                    retstr += "<DialogueTarget>" + mods[(page * modsPerPage) + i].ID + "</DialogueTarget>\n";
                    retstr += "</DialogueOption>";
                }

                //Put the closing dialogue options
                if (page > 0)
                    retstr += "<DialogueOption>\r\n        <Text>Previous Page</Text>\r\n        <DialogueTarget>PAGE" + (page - 1) + "</DialogueTarget>\r\n      </DialogueOption>\n";
                retstr += "<DialogueOption>\r\n        <Text>Exit</Text>\r\n      </DialogueOption>\n";
                retstr += "</DialogueOptionsList>\r\n\t</DialogueNode>\n";
            }

            //Make the nodes for each mod
            int count = 0;
            foreach(OtherMod mod in mods)
            {
                retstr += "<DialogueNode>\n";
                retstr += "<Name>" + mod.ID + "</Name>\n";
                retstr += "<Dialogue>\n";
                retstr += "<Page>Mod Name: " + mod.Name + "\nMod Author: " + mod.Author + "\nBody Name: " + TranslationHandler.GetTranslation(mod.Planet.GetComponent<AstroObject>()._customName, TranslationHandler.TextType.UI) + "\nUnique ID: " + mod.ID + "</Page>";
                retstr += "</Dialogue>\r\n    <DialogueOptionsList>\r\n      <DialogueOption>\r\n        <Text>Lock on to body.</Text>\r\n<ConditionToSet>" + mod.ID + "</ConditionToSet>\n";
                retstr += "</DialogueOption>\r\n      <DialogueOption>\r\n        <Text>Cancel.</Text>\r\n<DialogueTarget>PAGE" + (int)Mathf.Floor(count/modsPerPage) + "</DialogueTarget>\r\n      </DialogueOption>\r\n    </DialogueOptionsList>\r\n  </DialogueNode>\n";
                count++;
            }

            //Construct the footer
            retstr += "</DialogueTree>\n";

            return retstr.Replace("&", "&amp;");
        }
    }
}
