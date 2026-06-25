using UnityEngine;

namespace vikwhite
{
    public interface IBattleGridService
    {
        void SetVisible(bool visible);
    }

    public class BattleGridService : IBattleGridService
    {
        private GameObject _grid;

        public void SetVisible(bool visible)
        {
            if(_grid == null) _grid = GameObject.FindFirstObjectByType<HexGrid>().gameObject;
            _grid.SetActive(visible);
        }
    }
}