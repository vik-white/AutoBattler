using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public class Bootstrap : MonoBehaviour
    {
        public ConfigsLoader Configs;
        
        void Awake()
        {
            new Setup()
                .Configs(Configs)
                .Add<LobbyEnvironment>(EnvironmentType.Lobby)
                .Add<BattleEnvironment>(EnvironmentType.Battle)
                .Start(EnvironmentType.Lobby);
        }
    }
}