using UnityEngine;

namespace vikwhite
{
    public class Bootstrap : MonoBehaviour
    {
        void Awake()
        {
            new Setup()
                .Add<LobbyEnvironment>(EnvironmentType.Lobby)
                .Add<BattleEnvironment>(EnvironmentType.Battle)
                .Start(EnvironmentType.Lobby);
        }
    }
}