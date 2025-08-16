using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

namespace JamHub.orrery;

public class Jam5Orrery : Orrery
{
    protected override void Start()
    {
        base.Start();
        sunSphere.transform.localScale = Vector3.one;
        SetSizeScale(0.001f);
        SetDistanceScale(0.0005f);
    }
}
