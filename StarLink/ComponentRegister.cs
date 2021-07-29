using System;
using System.Collections.Concurrent;

namespace StarLink
{
    public class ComponentRegister<T>
        where T : Component
    {
        public ComponentRegister()
        {
            _components = new ConcurrentDictionary<string, T>();
        }

        public void Add(T component)
        {
            if (_components.TryAdd(component.Id, component))
            {
                component.Initialize();
            }
        }

        public bool TryGet(string id, out T component)
        {
            return _components.TryGetValue(id, out component);
        }

        public bool TryGet<K>(out K component)
            where K : T
        {
            component = default;
            foreach (var comp in _components.Values)
            {
                if (comp.GetType() == typeof(K))
                {
                    component = (K)comp;
                    return true;
                }
            }
            return false;
        }

        private ConcurrentDictionary<string, T> _components;
    }
}
