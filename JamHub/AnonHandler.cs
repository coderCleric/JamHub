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
            anonBrain.enabled = true;
            anonBrain.OnEnterDreamWorld();
            anonBrain.EscalateThreatAwareness(GhostData.ThreatAwareness.SomeoneIsInHere);
            anonBrain.WakeUp();
            triggerTransform.localScale = Vector3.zero;
        }

        private void Update()
        {
            if (anonSensors.IsIlluminated() && !isAnonPissed)
            {
                isAnonPissed = true;
                triggerTransform.localScale = Vector3.one;
                Destroy(anonDialogue);
            }
        }
    }
}
