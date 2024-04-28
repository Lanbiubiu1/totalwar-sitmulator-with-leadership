using NoOpArmy.WiseFeline.InfluenceMaps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NoOpArmy.WiseFeline.InfluenceMaps
{
    public class InfluenceMapCollection : NoOpArmy.Singleton<InfluenceMapCollection>
    {
        public Dictionary<string, InfluenceMapComponentBase> mapItems = new Dictionary<string, InfluenceMapComponentBase>();


        /// <summary>
        /// Gets a map by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public InfluenceMapComponentBase GetMap(string name)
        {
            if (mapItems.TryGetValue(name, out var value))
                return value;
            return null;
        }

        /// <summary>
        /// Registers a map in the collection. This is called automatically by the influence map component
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="influenceMap"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Register(string mapName, InfluenceMapComponentBase influenceMap)
        {
            if (!mapItems.ContainsKey(mapName))
                mapItems.Add(mapName, influenceMap);
        }

        /// <summary>
        /// Unregisters a map in the collection. This is called automatically by the influence map component
        /// </summary>
        /// <param name="mapName"></param>
        public void Unregister(string mapName)
        {
            mapItems.Remove(mapName);
        }
    }
}