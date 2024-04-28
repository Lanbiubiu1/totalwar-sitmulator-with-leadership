using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoOpArmy.WiseFeline.InfluenceMaps.Demo
{
    public class DefendAttractor : MonoBehaviour
    {
        public Transform attractor;
        public string mapName = "";
        private InfluenceMapComponentBase map;

        IEnumerator Start()
        {
            map = InfluenceMapCollection.Instance.GetMap(mapName);
            while (true)
            {
                map.SearchForHighestValueClosestToCenter(transform.position, 40, out var res);
                GetComponent<IAIMovement>().MoveToPosition(res);
                yield return new WaitForSeconds(1f);
            }
        }


    }
}
