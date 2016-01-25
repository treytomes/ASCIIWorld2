using System.Runtime.InteropServices;

namespace DirectCanvas
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Color4
    {
        public Color4(float a, float r, float g, float b)
        {
            InternalColor4 = new SlimDX.Color4();

            Alpha = a;
            Red = r;
            Green = g;
            Blue = b;
        }

        [FieldOffset(0)]
        public float Red;
        [FieldOffset(4)]
        public float Green;
        [FieldOffset(8)]
        public float Blue;
        [FieldOffset(12)]
        public float Alpha;
        
        [FieldOffset(0)]
        internal SlimDX.Color4 InternalColor4;
    }
}
