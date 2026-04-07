using UnityEngine;

namespace vikwhite
{
    public interface IViewFactory
    {
        TView CreateView<TView>(GameObject prefab, Transform parent, string name = null, bool activeOnStart = true) where TView : View;
        TView CreateView<TView, THierarchy>(THierarchy hierarchy) where TView : View<THierarchy> where THierarchy : MonoBehaviour;
    }
    
    public class ViewFactory : IViewFactory
    {
        private readonly DiContainer _container;

        public ViewFactory(DiContainer container)
        {
            _container = container;
        }

        public TView CreateView<TView>(GameObject prefab, Transform parent, string name = null, bool activeOnStart = true) where TView : View
        {
            GameObject gameObject = GameObject.Instantiate(prefab, parent);
            PreparePrefab(gameObject, parent, name, activeOnStart);
            return CreateView<TView>(gameObject);
        }

        public TView CreateView<TView, THierarchy>(THierarchy hierarchy) where TView : View<THierarchy> where THierarchy : MonoBehaviour
        {
            return CreateView<TView>(hierarchy.gameObject);
        }

        private TView CreateView<TView>(GameObject hierarchy) where TView : View
        {
            var view = _container.Resolve<TView, GameObject>(hierarchy);
            return view;
        }
        
        private void PreparePrefab(GameObject go, Transform parent, string name = null, bool activeOnStart = true)
        {
            if (!string.IsNullOrEmpty(name)) go.name = name;
            Transform transform = go.transform;
            if (parent != null) transform.SetParent(parent, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            go.SetActive(activeOnStart);
        }
    }
}