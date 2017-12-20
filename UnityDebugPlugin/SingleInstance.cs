﻿using UnityEngine;

namespace UnityDebugPlugin
{
    /// <summary>
    /// Singleton for MonoBehaviours.
    /// Creates a game object for your component if no instance is found.
    /// </summary>
    /// <typeparam name="T">The class you want to make a singleton</typeparam>
    public abstract class SingleInstance<T> : MonoBehaviour
      where T : SingleInstance<T>
    {
        /// <summary>
        /// Name of the game object this will be attached to.
        /// </summary>
        public abstract string Name { get; }

        private static T _instance;

        /// <summary>
        /// Return the single instance.
        /// If none exists, a new game object is created and the component is added
        /// to it.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = CreateOrFind();
                return _instance;
            }
        }

        /// <summary>
        /// Create an instance if none exists already.
        /// </summary>
        public static void Initialize ()
        {
            if (_instance == null)
                _instance = CreateOrFind();
        }

        private static T CreateOrFind ()
        {
            T[] instances = FindObjectsOfType<T>();

            if (instances.Length > 1)
            {
                Debug.LogWarning("Too many instances of " + typeof(T).Name + "!");
            }

            if (instances.Length > 0)
            {
                return instances[0];
            }

            // No instance exists yet, create one
            var t = new GameObject("SingleInstance temp").AddComponent<T>();
            t.gameObject.name = t.Name;
            return t;
        }
    }
}
