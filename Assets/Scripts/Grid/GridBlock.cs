using UnityEngine;

namespace Grid
{
    public class GridBlock : MonoBehaviour
    {
        public GridUnity.UnitType[] unitTypes = new  GridUnity.UnitType[9];
    
        private void Awake()
        {
            var gridUnits = GetComponentsInChildren<GridUnity>();
            if (gridUnits.Length < 9)
            {
                Debug.LogError(gridUnits.Length);
                return;
            }

            for (var i = 0; i < 9; i++)
            {
                gridUnits[i].CurrentUnitType = unitTypes[i];
            }
        }
    }
}
