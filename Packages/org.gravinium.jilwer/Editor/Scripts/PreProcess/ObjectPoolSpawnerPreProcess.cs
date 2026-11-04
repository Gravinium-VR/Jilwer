using System;
using Gravinium.Jilwer.VRC;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gravinium.Jilwer.Editor.PreProcess
{
    internal class ObjectPoolSpawnerPreProcess : IPreProcess
    {
        public void Process()
        {
            var spawners = Resources.FindObjectsOfTypeAll<ObjectPoolSpawner>();

            foreach (var spawner in spawners)
            {
                if (spawner.copies < 1)
                    throw new Exception($"[Jilwer] Copies must be 1 or greater: {spawner.gameObject.name}");
                if (!spawner.referenceObject)
                    throw new MissingFieldException($"[Jilwer] Reference object not found: {spawner.gameObject.name}");
                if (!spawner.objectPool)
                    throw new MissingFieldException($"[Jilwer] Object Pool not found: {spawner.gameObject.name}");
                
                GameObject obj = spawner.gameObject;

                var clones = new GameObject[spawner.copies];
                for (int i = 0; i < spawner.copies; i++)
                {
                    clones[i] = Object.Instantiate(spawner.referenceObject, obj.transform);
                }

                spawner.objectPool.Pool = clones;
                Object.Destroy(spawner);
                Object.Destroy(spawner.referenceObject);
            }
        }
    }
}