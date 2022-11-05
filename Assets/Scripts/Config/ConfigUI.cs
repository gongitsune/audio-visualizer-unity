using System;
using System.Globalization;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Config
{
    public class ConfigUI : MonoBehaviour
    {
        private static ConfigUI _instance;

        [SerializeField] [Required] private Toggle useRemote;

        [SerializeField] [Required] private TMP_InputField colorSpeed,
            saturation,
            brightness,
            minSize,
            maxSize,
            minAmplitude,
            maxAmplitude,
            sensitivity,
            numOfParticle;

        private readonly Subject<Unit> _onUpdate = new();

        private float _colorSpeed,
            _saturation,
            _brightness,
            _minSize,
            _maxSize,
            _minAmplitude,
            _maxAmplitude,
            _sensitivity,
            _numOfParticle;

        private GameInput _input;
        [SerializeField] private Image panel;

        private RemoteConfigManager.RemoteSetting _remote;
        public IObservable<Unit> OnUpdate => _onUpdate;

        public static ConfigUI Instance
        {
            get
            {
                if (_instance is not null) return _instance;
                GameObject.FindWithTag("ConfigUI").TryGetComponent(out _instance);
                return _instance;
            }
        }

        public float ColorSpeed => useRemote.isOn ? _remote.ColorSpeed.Value : _colorSpeed;
        public float Saturation => useRemote.isOn ? _remote.Saturation.Value : _saturation;
        public float Brightness => useRemote.isOn ? _remote.Brightness.Value : _brightness;
        public float MinSize => useRemote.isOn ? _remote.MinSize.Value : _minSize;
        public float MaxSize => useRemote.isOn ? _remote.MaxSize.Value : _maxSize;
        public float MinAmplitude => useRemote.isOn ? _remote.MinAmplitude.Value : _minAmplitude;
        public float MaxAmplitude => useRemote.isOn ? _remote.MaxAmplitude.Value : _maxAmplitude;
        public float Sensitivity => useRemote.isOn ? _remote.Sensitivity.Value : _sensitivity;
        public float NumOfParticle => useRemote.isOn ? _remote.NumOfPartcile.Value : _numOfParticle;

        private void Start()
        {
            _input = new GameInput();
            _input.UI.Enable();
            
            var manager = RemoteConfigManager.Instance;
            _remote = manager.Setting;
            manager.OnUpdate.Subscribe(_ =>
            {
                if (!useRemote.isOn) return;

                colorSpeed.text = _remote.ColorSpeed.Value.ToString(CultureInfo.InvariantCulture);
                saturation.text = _remote.Saturation.Value.ToString(CultureInfo.InvariantCulture);
                brightness.text = _remote.Brightness.Value.ToString(CultureInfo.InvariantCulture);
                minSize.text = _remote.MinSize.Value.ToString(CultureInfo.InvariantCulture);
                maxSize.text = _remote.MaxSize.Value.ToString(CultureInfo.InvariantCulture);
                minAmplitude.text = _remote.MinAmplitude.Value.ToString(CultureInfo.InvariantCulture);
                maxAmplitude.text = _remote.MaxAmplitude.Value.ToString(CultureInfo.InvariantCulture);
                sensitivity.text = _remote.Sensitivity.Value.ToString(CultureInfo.InvariantCulture);
                numOfParticle.text = _remote.NumOfPartcile.Value.ToString(CultureInfo.InvariantCulture);

                _onUpdate.OnNext(Unit.Default);
            }).AddTo(this);

            _input.UI.Config.started += ToggleConfigUI;

            colorSpeed.onEndEdit.AddListener(v =>
            {
                _colorSpeed = (float)Convert.ToDouble(v);
                _onUpdate.OnNext(Unit.Default);
            });
            saturation.onEndEdit.AddListener(v =>
            {
                _saturation = (float)Convert.ToDouble(v);
                _onUpdate.OnNext(Unit.Default);
            });
            brightness.onEndEdit.AddListener(v =>
            {
                _brightness = (float)Convert.ToDouble(v);
                _onUpdate.OnNext(Unit.Default);
            });
            minSize.onEndEdit.AddListener(v =>
            {
                _minSize = (float)Convert.ToDouble(v);
                _onUpdate.OnNext(Unit.Default);
            });
            maxSize.onEndEdit.AddListener(v =>
            {
                _maxSize = (float)Convert.ToDouble(v);
                _onUpdate.OnNext(Unit.Default);
            });
            minAmplitude.onEndEdit.AddListener(v =>
            {
                _minAmplitude = (float)Convert.ToDouble(v);
                _onUpdate.OnNext(Unit.Default);
            });
            maxAmplitude.onEndEdit.AddListener(v =>
            {
                _maxAmplitude = (float)Convert.ToDouble(v);
                _onUpdate.OnNext(Unit.Default);
            });
            sensitivity.onEndEdit.AddListener(v =>
            {
                _sensitivity = (float)Convert.ToDouble(v);
                _onUpdate.OnNext(Unit.Default);
            });
            numOfParticle.onEndEdit.AddListener(v =>
            {
                _numOfParticle = (float)Convert.ToDouble(v);
                _onUpdate.OnNext(Unit.Default);
            });
        }

        private void ToggleConfigUI(InputAction.CallbackContext _)
        {
            panel.gameObject.SetActive(!panel.gameObject.activeSelf);
        }
    }
}