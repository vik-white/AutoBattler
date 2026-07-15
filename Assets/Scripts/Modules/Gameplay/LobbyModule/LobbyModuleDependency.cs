namespace vikwhite
{
    public class LobbyModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IStateMachine<ILobbyState>, StateMachine<ILobbyState>>();
            Register<IStateFactory<ILobbyState>, StateFactory<ILobbyState>>();
            Register<ILobbyStartState, LobbyStartState>();
            Register<ILobbyEndState, LobbyEndState>();
            
            Register<ILobbyWindow, LobbyWindow>();
            Register<LobbyWindowViewModel>();
            Register<LobbyWindowView>();

            Register<IRoomWindow, RoomWindow>();
            Register<RoomWindowViewModel>();
            Register<RoomWindowView>();

            Register<IRoomLineViewFactory, RoomLineViewFactory>();
            Register<RoomLineViewModel>();
            Register<RoomLineView>();
            
            Register<IRoomFactory, RoomFactory>();
            Register<Room>();
            Register<IRoomSelectionService, RoomSelectionService>();
            Register<IRoomsService, RoomsService>();
        }
    }
}
