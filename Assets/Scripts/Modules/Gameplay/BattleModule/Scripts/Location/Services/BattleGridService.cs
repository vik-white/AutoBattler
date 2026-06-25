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
            if (_grid == null)
            {
                var grid = Object.FindAnyObjectByType<HexGrid>(FindObjectsInactive.Include);
                _grid = grid != null ? grid.gameObject : null;
            }

            if (_grid != null)
                _grid.SetActive(visible);
        }
    }
}
