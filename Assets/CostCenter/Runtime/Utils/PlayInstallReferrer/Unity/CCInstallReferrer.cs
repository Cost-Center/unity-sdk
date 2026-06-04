using System;
using System.Collections.Generic;
using UnityEngine;
using Ugi.PlayInstallReferrerPlugin;

namespace CostCenter {
    public static class CCInstallReferrer {
        private const string PREFS_KEY = "CC_InstallReferrer";

        private static string _cached;
        private static bool _fetchStarted;
        private static readonly List<Action<string>> _pending = new();

        // Full details from the last fetch this session (null if loaded from cache)
        public static PlayInstallReferrerDetails Details { get; private set; }

        // Cached referrer string — persists across sessions via PlayerPrefs
        public static string Value {
            get {
                if (_cached == null)
                    _cached = PlayerPrefs.GetString(PREFS_KEY, string.Empty);
                return string.IsNullOrEmpty(_cached) ? null : _cached;
            }
        }

        // Single entry point for all callers. If value is already cached, callback
        // fires synchronously. If a fetch is already in-flight, callback is queued.
        // Otherwise starts a fresh fetch.
        public static void Fetch(Action<string> callback = null) {
            if (!string.IsNullOrEmpty(Value)) {
                callback?.Invoke(Value);
                return;
            }
            if (callback != null) _pending.Add(callback);
            if (_fetchStarted) return;
            _fetchStarted = true;
            #if UNITY_ANDROID && !UNITY_EDITOR
            PlayInstallReferrerAndroid.GetInstallReferrerInfo(OnFetchComplete);
            #elif UNITY_EDITOR
            PlayInstallReferrerEditor.GetInstallReferrerInfo(OnFetchComplete);
            #else
            FlushPending(null);
            #endif
        }

        private static void OnFetchComplete(PlayInstallReferrerDetails details) {
            Details = details;
            if (details.Error == null && details.InstallReferrer != null) {
                _cached = details.InstallReferrer;
                PlayerPrefs.SetString(PREFS_KEY, _cached);
                PlayerPrefs.Save();
            }
            FlushPending(Value);
        }

        private static void FlushPending(string value) {
            var callbacks = new List<Action<string>>(_pending);
            _pending.Clear();
            foreach (var cb in callbacks) cb?.Invoke(value);
        }
    }
}
