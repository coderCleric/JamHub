using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JamHub.orrery
{
    public abstract class Orrery : MonoBehaviour
    {
        protected OrreryPlanet[] planets = null;
        protected GameObject sunSphere = null;
        public static Material selectedMat { get; protected set; } = null;
        public static Material unselectedMat { get; protected set; } = null;

        /**
         * Grab the sun sphere when we wake up
         */
        protected virtual void Start()
        {
            sunSphere = transform.Find("SunSphere").gameObject;
            unselectedMat = sunSphere.GetComponent<Renderer>().sharedMaterial;
            selectedMat = transform.Find("matholder").gameObject.GetComponent<Renderer>().sharedMaterial;
            Destroy(transform.Find("matholder").gameObject);
            MakePlanets(JamHub.instance.mods);
        }

        /**
         * Make the orrery planets from the given list of mods
         */
        public virtual void MakePlanets(List<OtherMod> mods)
        {
            //Initialize the empty array
            planets = new OrreryPlanet[mods.Count];

            //For each mod, make an orrery planet
            for(int i = 0; i < mods.Count; i++)
            {
                GameObject planetObj = Instantiate(sunSphere, transform);
                planets[i] = planetObj.AddComponent<OrreryPlanet>();
                planets[i].planet = mods[i];
            }
        }

        /**
         * Update the distance scale of the planets
         */
        protected void SetDistanceScale(float scale)
        {
            foreach(OrreryPlanet planet in planets)
                planet.distScale = scale;
        }

        /**
         * Update the size scale of the planets
         */
        protected void SetSizeScale(float scale)
        {
            foreach(OrreryPlanet planet in planets)
                planet.sizeScale = scale;
        }
    }
}
