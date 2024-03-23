using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JamHub
{
    public class ErnestoController : MonoBehaviour
    {
        private Animator animator = null;
        private CharacterDialogueTree[] dialogues = null;
        private int conIndex = 0;

        /**
         * On awake grab the components and link to the dialogues
         */
        private void Awake()
        {
            //Grab the components
            animator = GetComponent<Animator>();
            dialogues = GetComponentsInChildren<CharacterDialogueTree>();

            //Disable all but the first dialogue
            for(int i = 1; i < dialogues.Length; i++)
                dialogues[i].gameObject.SetActive(false);

            //Set up the event on all but the last dialogue
            for (int i = 0; i < dialogues.Length - 1; i++)
                dialogues[i].OnEndConversation += OnDialogueEnd;

            //Set up the actual fish animator to work right
            Animator ernestoAnim = transform.Find("ernesto/angler_model").GetComponent<Animator>();
            ernestoAnim.runtimeAnimatorController =
                GameObject.Find("DB_SmallNest_Body/Sector_SmallNestDimension/Interactables_SmallNestDimension/Anglerfish_Body/Beast_Anglerfish")
                .GetComponent<Animator>().runtimeAnimatorController;
            ernestoAnim.SetFloat("MoveSpeed", 0);
        }

        /**
         * Trigger Ernesto to move to the next point
         */
        private void OnDialogueEnd()
        {
            animator.SetTrigger("proceed");

            //Switch what dialogue is active
            dialogues[conIndex].gameObject.SetActive(false);
            conIndex++;
            dialogues[conIndex].gameObject.SetActive(true);
        }

        /**
         * On destroy, unlink the events
         */
        private void OnDestroy()
        {
            for (int i = 0; i < dialogues.Length - 1; i++)
                dialogues[i].OnEndConversation -= OnDialogueEnd;
        }
    }
}
