using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roadrunner._3DDiffusion
{
    internal class LiquidModel
    {
        private float[] state;

        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint Depth { get; set; }
        public float Consistency { get; set; }

        public LiquidModel(uint width, uint height, uint depth, float consistency)
        {
            Width = width;
            Height = height;
            Depth = depth;

            state = new float[Width * Height * Depth];

            Consistency = consistency;
        }

        private uint ToIndex(uint x, uint y, uint z)
        {
            return (Height * Width * z) + (Width * y) + x;
        }

        public float GetState(uint x, uint y, uint z)
        {
            return GetState(state, x, y, z);
        }

        private float GetState(float[] array, uint x, uint y, uint z)
        {
            uint index = ToIndex(x, y, z);
            return array[index];
        }

        public void SetState(uint x, uint y, uint z, float newState)
        {
            SetState(state, x, y, z, newState);
        }

        private void SetState(float[] array, uint x, uint y, uint z, float newState)
        {
            uint index = ToIndex(x, y, z);
            array[index] = newState;
        }

        public void Update(double timeStep)
        {
            float[] stateCopy = new float[state.Length];
            Array.Copy(state, stateCopy, state.Length);

            float consistency = Consistency;

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    for (int k = 0; k < Depth; k++)
                    {
                        float value = GetState(stateCopy, (uint)i, (uint)j, (uint)k);
                        ICollection<float> diffsToAverage = new List<float>();
                        for (int iDiff = -1; iDiff <= 1; iDiff++)
                        {
                            for (int jDiff = -1; jDiff <= 1; jDiff++)
                            {
                                for (int kDiff = -1; kDiff <= 1; kDiff++)
                                {
                                    int iNew = i + iDiff;
                                    int jNew = j + jDiff;
                                    int kNew = k + kDiff;
                                    if (iNew < 0 || iNew >= Width ||
                                        jNew < 0 || jNew >= Height ||
                                        kNew < 0 || kNew >= Depth ||
                                        (iNew == i && jNew == j && kNew == k))
                                    {
                                        continue;
                                    }

                                    float nearValue = GetState(stateCopy, (uint)iNew, (uint)jNew, (uint)kNew);
                                    float valueDiff = (nearValue - value);
                                    diffsToAverage.Add(valueDiff);
                                }
                            }
                        }

                        float newValue = value + ((diffsToAverage.Count > 0 ? diffsToAverage.Average() : 0.0f) * (float)timeStep * consistency);
                        if (newValue < 0.0f || newValue > 1000.0f ||float.IsInfinity(newValue))
                        {
                            newValue = 0.0f;
                        }
                        SetState((uint)i, (uint)j, (uint)k, newValue);
                    }
                }
            }
        }
    }
}
