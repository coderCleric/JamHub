using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JamHub
{
    /**
     * Because the self NPC's like to get really big for no reason
     */
    public class SizeFixer : MonoBehaviour
    {
        [SerializeField] private float size = 1;
        private void Update()
        {
            transform.localScale = new Vector3(size, size, size);
            Destroy(this);
        }
    }
}
