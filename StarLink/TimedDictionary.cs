using System;
using System.Collections.Generic;

namespace StarLink
{
    class TimedValue<T>
    {
        public TimedValue(T value, DateTime expiresAt)
        {
            Value = value;
            ExpiresAt = expiresAt;
        }

        public T Value { get; set; }
        public DateTime ExpiresAt { get; set; }

        public bool IsValid { get { return IsExpired(DateTime.Now) == false; } }

        public bool IsExpired(DateTime dateTime)
        {
            return dateTime > ExpiresAt;
        }
    }

    class TimedDictionarySettings
    {
        public int TickTime { get; set; } = 20000; // 20 seconds 
        public int TTL { get; set; } = 10000; // 10 seconds
        public bool RefreshTTLOnRead { get; set; } = false;
    }

    class TimedDictionary<K, V>
    {
        public TimedDictionary()
        {
            Settings = new TimedDictionarySettings();
            onExpire = (K key, V value) => { };
            data = new Dictionary<K, TimedValue<V>>();
            tickTime = DateTime.Now; 
        }

        public TimedDictionary(TimedDictionarySettings settings)
        {
            Settings = settings;
            onExpire = (K key, V value) => { };
            data = new Dictionary<K, TimedValue<V>>();
            tickTime = DateTime.Now;
        }

        public void Clear()
        {
            data.Clear();
        }
        public void Add(K key, V value)
        {
            data.Add(key, new TimedValue<V>(value, DateTime.Now.AddMilliseconds(Settings.TTL)));
        }

        public bool ContainsKey(K key)
        {
            if (data.ContainsKey(key))
            {
                TimedValue<V> timedValue = data[key];
                if (timedValue.IsValid)
                {
                    return true;
                }

                if (onExpire != null) onExpire.Invoke(key, timedValue.Value);
                data.Remove(key);
            }

            return false;
        }

        public void Remove(K key)
        {
            data.Remove(key);
        }

        public V Get(K key)
        {
            if (data.ContainsKey(key))
            {
                TimedValue<V> timedValue = data[key];
                if (timedValue.IsValid)
                {
                    if (Settings.RefreshTTLOnRead)
                    {
                        timedValue.ExpiresAt = DateTime.Now.AddMilliseconds(Settings.TTL);
                    }
                    return timedValue.Value;
                }

                if (onExpire != null) onExpire.Invoke(key, timedValue.Value);
                data.Remove(key);
            }

            return default(V);
        }

        public Dictionary<K, TimedValue<V>>.KeyCollection Keys { get { return data.Keys; } }
        public Dictionary<K, TimedValue<V>>.ValueCollection Values { get { return data.Values; } }

        public Dictionary<K, TimedValue<V>>.Enumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public V this[K key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Add(key, value);
            }
        }

        public void Tick()
        {
            DateTime now = DateTime.Now;
            if (now < tickTime)
            {
                return;
            }

            tickTime = now.AddMilliseconds(Settings.TickTime);

            foreach (var item in data)
            {
                if (item.Value.IsExpired(now))
                {
                    if (onExpire != null) onExpire.Invoke(item.Key, item.Value.Value);
                    data.Remove(item.Key);
                }
            }
        }

        public TimedDictionarySettings Settings { get; private set; }
        public Action<K, V> onExpire = (K key, V value) => { };
        private Dictionary<K, TimedValue<V>> data;
        private DateTime tickTime;
    }
}
