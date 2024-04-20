using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JamHub
{
    public class OrreryPlanet : MonoBehaviour
    {
        private static float distScale = 0.001f;
        private static float scaleScale = 0.004f;

        public OtherMod planet = null;
        private bool locked = false;

        /**
         * Every frame, move the orrery planet to match the location and scale of the actual
         */
        private void Update()
        {
            //If there's no planet, abort!
            if (planet == null || planet.Planet == null)
                return;

            //Calculate the true location of the planet
            Vector3 rawDist = planet.Planet.transform.position - CenterOfTheUniverse.s_instance._staticReferenceFrame.transform.position;

            //Update the location of the planet
            transform.localPosition = rawDist * distScale;

            //Update the scale of the planet
            //If we find a gravity well, get surface size from that. Otherwise, use a default size
            GravityVolume grav = planet.Planet.GetComponentInChildren<GravityVolume>();
            float scale = 1;
            if(grav != null)
                scale = grav._upperSurfaceRadius * scaleScale;
            transform.localScale = new Vector3(scale, scale, scale);

            //Update the material
            bool planetLocked = Locator.GetPlayerBody() != null && Locator.GetPlayerBody().gameObject.GetComponent<ReferenceFrameTracker>()._currentReferenceFrame ==
                planet.Planet.GetAttachedOWRigidbody().GetReferenceFrame();
            if (planetLocked && !locked)
            {
                gameObject.GetComponent<Renderer>().sharedMaterial = Orrery.selectedMat;
                locked = true;
            }
            if (!planetLocked && locked)
            {
                gameObject.GetComponent<Renderer>().sharedMaterial = Orrery.unselectedMat;
                locked = false;
            }
        }
    }
}
