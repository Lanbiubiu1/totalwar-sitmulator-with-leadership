using UnityEngine;

namespace NoOpArmy
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + nameof(T);
                            DontDestroyOnLoad(singleton);
                        }

                    }

                    return _instance;
                }
            }
        }

        private static bool applicationIsQuitting = false;

        protected void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(this.gameObject);
                Debug.LogWarning($"Only one instance of the type {nameof(T)} should exist!");
            }
        }

        protected void OnDestroy()
        {
            applicationIsQuitting = true;
        }
    }
}