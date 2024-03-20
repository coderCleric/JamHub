using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JamHub
{
    public class Orrery : MonoBehaviour
    {
        public OrreryPlanet[] planets = null;
        private GameObject sunSphere = null;
        public static Material selectedMat = null;
        public static Material unselectedMat = null;

        /**
         * Grab the sun sphere when we wake up
         */
        private void Awake()
        {
            sunSphere = transform.Find("SunSphere").gameObject;
            unselectedMat = sunSphere.GetComponent<Renderer>().sharedMaterial;
            selectedMat = transform.Find("matholder").gameObject.GetComponent<Renderer>().sharedMaterial;
            Destroy(transform.Find("matholder").gameObject);
        }

        /**
         * Make the orrery planets from the given list of mods
         */
        public void MakePlanets(OtherMod[] mods)
        {
            //Initialize the empty array
            planets = new OrreryPlanet[mods.Length];

            //For each mod, make an orrery planet
            for(int i = 0; i < mods.Length; i++)
            {
                GameObject planetObj = GameObject.Instantiate(sunSphere, transform);
                planets[i] = planetObj.AddComponent<OrreryPlanet>();
                planets[i].planet = mods[i];
            }
        }
    }
}
