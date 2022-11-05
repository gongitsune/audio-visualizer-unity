using UnityEngine;
using UnityEngine.VFX;

namespace Utils
{
    public readonly struct FloatVfxProp
    {
        private readonly VisualEffect _effect;
        private readonly int _id;

        public FloatVfxProp(in VisualEffect effect, in string name)
        {
            _effect = effect;
            _id = Shader.PropertyToID(name);
        }

        public float Value
        {
            get => _effect.GetFloat(_id);
            set => _effect.SetFloat(_id, value);
        }
    }
    
    public readonly struct IntVfxProp
    {
        private readonly VisualEffect _effect;
        private readonly int _id;

        public IntVfxProp(in VisualEffect effect, in string name)
        {
            _effect = effect;
            _id = Shader.PropertyToID(name);
        }

        public int Value
        {
            get => _effect.GetInt(_id);
            set => _effect.SetInt(_id, value);
        }
    }
    
    public readonly struct Vector4VfxProp
    {
        private readonly VisualEffect _effect;
        private readonly int _id;

        public Vector4VfxProp(in VisualEffect effect, in string name)
        {
            _effect = effect;
            _id = Shader.PropertyToID(name);
        }

        public Vector4 Value
        {
            get => _effect.GetVector4(_id);
            set => _effect.SetVector4(_id, value);
        }
    }
}