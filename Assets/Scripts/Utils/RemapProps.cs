using System;
using Unity.Mathematics;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public class RemapProps
    {
        [SerializeField] private float toMin, toMax, fromMin, fromMax;

        public float Remap(in float x)
        {
            return math.remap(fromMin, fromMax, toMin, toMax, x);
        }
        
        public RemapProps(float toMin, float toMax, float fromMin, float fromMax)
        {
            this.toMin = toMin;
            this.toMax = toMax;
            this.fromMin = fromMin;
            this.fromMax = fromMax;
        }
    }
}