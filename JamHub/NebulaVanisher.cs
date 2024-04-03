using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JamHub
{
    public class NebulaVanisher : MonoBehaviour
    {
        private CharacterDialogueTree dialogue;
        private bool sinking = false;

        /**
         * On awake, do some key setup
         */
        private void Awake()
        {
            dialogue = GetComponentInChildren<CharacterDialogueTree>();
            dialogue.OnEndConversation += Sink;
        }

        /**
         * Set the character to sink
         */
        private void Sink()
        {
            dialogue.gameObject.SetActive(false);
            sinking = true;
        }

        /**
         * If needed, make the character sink
         */
        private void Update()
        {
            if(sinking && transform.localPosition.y > -10)
            {
                float movement = -0.5f * Time.deltaTime;
                transform.Translate(new Vector3(0, movement, 0), Space.Self);
            }
        }
    }
}
