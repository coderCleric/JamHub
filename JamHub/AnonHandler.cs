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
        [SerializeField]
        private DreamLanternItem obligatoryDreamLantern; // we need to place a random dream lantern in the scene to get inhabitants working

        private void Start()
        {
            DialogueConditionManager.SharedInstance.SetConditionState("cOaDialogueDone", false);
            DialogueConditionManager.SharedInstance.SetConditionState("AnonPissed", false);
        }

        public void InitiateGhost()
        {
            Locator.GetDreamWorldController()._dreamBody.gameObject.SetActive(true);
            var relativeLocation = new RelativeLocationData(Vector3.up * 2 + Vector3.forward * 2, Quaternion.identity, Vector3.zero);

            //nessesary for owlks, lets them work somehow????????
            var location = DreamArrivalPoint.Location.Zone3;
            var arrivalPoint = Locator.GetDreamArrivalPoint(location);
            var dreamCampfire = Locator.GetDreamCampfire(location);
            if (Locator.GetToolModeSwapper().GetItemCarryTool().GetHeldItemType() != ItemType.DreamLantern)
            {
                obligatoryDreamLantern.OnEnterDreamWorld();
                Locator.GetDreamWorldController()._playerLantern = obligatoryDreamLantern;
            };

            //Puts player into a semi-dreamworld state
            Locator.GetDreamWorldController().EnterDreamWorld(dreamCampfire, arrivalPoint, relativeLocation);

            SingleLightSensor[] sensors = anonBrain.GetComponentsInChildren<SingleLightSensor>();
            SingleLightSensor[] ghostLightSensors = new SingleLightSensor[sensors.Length];

            anonBrain.enabled = true;
            for (int i = 0; i < sensors.Length; i++)
            {
                ghostLightSensors[i] = sensors[i];
            }
            anonBrain.enabled = true;
            anonBrain.WakeUp();
            anonBrain.OnEnterDreamWorld();
            anonBrain.EscalateThreatAwareness(GhostData.ThreatAwareness.SomeoneIsInHere);
            triggerTransform.localScale = Vector3.zero;
        }

        private void Update()
        {
            if (anonSensors.IsIlluminated() && !IsAnonPissed())
            {
                DialogueConditionManager.SharedInstance.SetConditionState("AnonPissed", true);
                triggerTransform.localScale = Vector3.one;
                Destroy(anonDialogue);
            }
        }

        private bool IsAnonPissed()
        {
            return DialogueConditionManager.SharedInstance.GetConditionState("AnonPissed");
        }
    }
}
