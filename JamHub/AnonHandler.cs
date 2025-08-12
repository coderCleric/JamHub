using NewHorizons;
using OWML.ModHelper;
using UnityEngine;

namespace JamHub
{
    public class AnonHandler : MonoBehaviour
    {
        [SerializeField]
        private GhostBrain anonBrain;
        [SerializeField]
        private CompoundLightSensor anonSensors;
        [SerializeField]
        private Transform triggerTransform;
        [SerializeField]
        private GameObject anonDialogue;
        private bool isAnonPissed;

        private void Start()
        {
            isAnonPissed = false;
            DialogueConditionManager.SharedInstance.SetConditionState("cOaDialogueDone", false);
            DialogueConditionManager.SharedInstance.SetConditionState("AnonPissed", false);
        }

        private void Update()
        {
            if (anonSensors.IsIlluminated() && !isAnonPissed)
            {
                isAnonPissed = true;
                DialogueConditionManager.SharedInstance.SetConditionState("AnonPissed", true);
                triggerTransform.localScale = Vector3.one;
                Destroy(anonDialogue);
            }
        }
    }
}
