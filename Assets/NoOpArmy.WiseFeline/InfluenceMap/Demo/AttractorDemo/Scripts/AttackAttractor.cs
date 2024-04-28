using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoOpArmy.WiseFeline.InfluenceMaps.Demo
{
    public class AttackAttractor : MonoBehaviour
    {
        public string mapName = "";
        private InfluenceMapComponentBase map;

        IEnumerator Start()
        {
            map = InfluenceMapCollection.Instance.GetMap(mapName);
            while (true)
            {
                float val = map.SearchForHighestValueWithRandomStartingPoint(transform.position, 40, out var _);
                if (val < 0.7f)
                {
                    map.SearchForHighestValueWithRandomStartingPoint(transform.position, 300, out var res);
                    res.y = transform.position.y;
                    GetComponent<IAIMovement>().MoveToPosition(res);
                }
                else
                {
                    var success = map.SearchForValueWithRandomStartingPoint(0.4f, SearchCondition.Less, transform.position, 400, out var res);
                    res.y = transform.position.y;
                    GetComponent<IAIMovement>().MoveToPosition(res);
                }
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}
