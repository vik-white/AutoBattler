using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace vikwhite
{
    public interface IResourceService
    {
        void Initialize();
        Resource Get(ResourceType type);
        ReactiveProperty<int> GetAmount(ResourceType type);
        void Add(ResourceType type, int amount);
        void Spend(ResourceType type, int amount);
    }
    
    public class ResourceService : IResourceService
    {
        private readonly IProfileService _profile;
        private readonly IEventDispatcher _dispatcher;
        private readonly Dictionary<ResourceType, Resource> _resources = new ();

        public ResourceService(IProfileService profile, IEventDispatcher dispatcher)
        {
            _profile = profile;
            _dispatcher = dispatcher;
        }

        public void Initialize() {
            foreach (var resourceData in _profile.Data.Resources)
                _resources.Add(resourceData.Type, new Resource(resourceData.Type, resourceData.Amount));
        }

        public Resource Get(ResourceType type) => _resources[type];
        
        public ReactiveProperty<int> GetAmount(ResourceType type) => _resources[type].Amount;

        public void Add(ResourceType type, int amount) {
            if(amount <=0 ) return;
            _resources[type].Amount.Value += amount;
            Debug.Log($"Added {amount} to {_resources[type].Amount.Value}");
            _dispatcher.Dispatch(new ChangeResourceEvent(type, _resources[type].Amount.Value));
        }

        public void Spend(ResourceType type, int amount) {
            if(amount <=0 ) return;
            if (_resources[type].Amount.Value - amount >= 0) {
                _resources[type].Amount.Value -= amount;
                _dispatcher.Dispatch(new ChangeResourceEvent(type, _resources[type].Amount.Value));
            }
        }
    }
}