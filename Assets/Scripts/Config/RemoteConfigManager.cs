using System;
using System.Threading.Tasks;
using UniRx;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

namespace Config
{
    public class RemoteConfigManager : MonoBehaviour
    {
        private static RemoteConfigManager _instance;
        private readonly Subject<Unit> _onUpdate = new();
        public IObservable<Unit> OnUpdate => _onUpdate;

        [SerializeField] private float updateSpan = 10;

        [SerializeField] private RemoteSetting setting;
        private float _elapsed;

        public static RemoteConfigManager Instance
        {
            get
            {
                if (_instance is not null) return _instance;
                GameObject.FindWithTag("RemoteConfigManager").TryGetComponent(out _instance);
                return _instance;
            }
        }

        public RemoteSetting Setting => setting;

        private async void Awake()
        {
            if (Utilities.CheckForInternetConnection()) await InitializeRemoteConfigAsync();

            RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
            RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new AppAttributes());
        }

        private void FixedUpdate()
        {
            _elapsed += Time.fixedDeltaTime;
            if (_elapsed < updateSpan) return;
            _elapsed = 0;
            RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new AppAttributes());
        }

        private static async Task InitializeRemoteConfigAsync()
        {
            // initialize handlers for unity game services
            await UnityServices.InitializeAsync();

            // options can be passed in the initializer, e.g if you want to set analytics-user-id or an environment-name use the lines from below:
            // var options = new InitializationOptions()
            //   .SetOption("com.unity.services.core.analytics-user-id", "my-user-id-1234")
            //   .SetOption("com.unity.services.core.environment-name", "production");
            // await UnityServices.InitializeAsync(options);

            // remote config requires authentication for managing environment information
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private void ApplyRemoteSettings(ConfigResponse response)
        {
            switch (response.requestOrigin)
            {
                case ConfigOrigin.Default:
                    Debug.Log("No settings loaded this session; using default values.");
                    break;
                case ConfigOrigin.Cached:
                    Debug.Log("No settings loaded this session; using cached values from a previous session.");
                    break;
                case ConfigOrigin.Remote:
                    Debug.Log("New settings loaded this session; update values accordingly.");
                    setting.Update();
                    _onUpdate.OnNext(Unit.Default);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private struct UserAttributes
        {
        }

        private struct AppAttributes
        {
        }

        [Serializable]
        public class RemoteSetting
        {
            [SerializeField] public FloatConfigValue ColorSpeed = new("ColorSpeed"),
                Saturation = new("Saturation"),
                Brightness = new("Brightness"),
                MinSize = new("MinSize"),
                MaxSize = new("MaxSize"),
                MinAmplitude = new("MinAmplitude"),
                MaxAmplitude = new("MaxAmplitude"),
                Sensitivity = new("Sensitivity"),
                NumOfPartcile = new("NumOfParticle");

            public void Update()
            {
                ColorSpeed.Update();
                Saturation.Update();
                Brightness.Update();
                MinSize.Update();
                MaxSize.Update();
                MinAmplitude.Update();
                MaxAmplitude.Update();
                Sensitivity.Update();
                NumOfPartcile.Update();
            }
        }

        [Serializable]
        public class FloatConfigValue
        {
            [SerializeField] private float value;
            private readonly string _key;

            public FloatConfigValue(string key)
            {
                _key = key;
            }

            public float Value
            {
                get => value;
                private set => this.value = value;
            }

            public void Update()
            {
                Value = RemoteConfigService.Instance.appConfig.GetFloat(_key);
            }
        }
    }
}