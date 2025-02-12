using Sirenix.OdinInspector;
using UnityEngine;

namespace _Common.BaseClasses
{
    public abstract class AResource<T> : SerializedScriptableObject where T : Object
    {
        public abstract string ResourcePath { get; }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public static TSub Load<TSub>() where TSub : AResource<T>
        {
            var instance = CreateInstance<TSub>();
            var path = instance.ResourcePath;
            DestroyImmediate(instance);
            
            var resource = UnityEngine.Resources.Load<T>(path) as TSub;
            if (!resource) Debug.LogWarning($"Resource of type {typeof(TSub)} not found at path {path}");
            return resource;
        }
    }
}