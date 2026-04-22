namespace vikwhite
{
    public interface ICardViewFactory : IPooledViewFactory<CardView, CardViewModel> { }
    
    public class CardViewFactory : PooledViewFactory<CardView, CardViewModel>, ICardViewFactory
    {
        public override string AssetName => "UI/Prefabs/SquadWindow/Card";
    }
}