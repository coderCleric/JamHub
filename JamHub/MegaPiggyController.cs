using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JamHub
{
    public class MegaPiggyController : MonoBehaviour
    {
        private CharacterDialogueTree dialogue;

        /**
         * On awake, do some key setup
         */
        private void Awake()
        {
            dialogue = GetComponentInChildren<CharacterDialogueTree>();
            dialogue.OnStartConversation += CheckFacts;
            CheckFacts();
        }

        /**
         * Check Axiom and ARC Facts
         */
        private void CheckFacts()
        {
            var axiomLogRevealed = JamSystemHelper.IsFactRevealed("AXIOM_ESCAPE_POD_X2");
            DialogueConditionManager.SharedInstance.SetConditionState("AxiomEscapePod", axiomLogRevealed);

            var arcLogRevealed = JamSystemHelper.IsFactRevealed("ARC_FINAL_ROOM_X4");
            DialogueConditionManager.SharedInstance.SetConditionState("ARCBox", arcLogRevealed);
        }
    }
}
