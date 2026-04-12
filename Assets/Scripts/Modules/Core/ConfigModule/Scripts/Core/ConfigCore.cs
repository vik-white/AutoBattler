using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace vikwhite.Data
{
    [Serializable]
    public class ConfigCore
    {
        public static FieldInfo[] Fields => _fields = _fields ?? typeof(ConfigsLoader).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).ToList().FindAll(e => e.FieldType.IsSubclassOf(typeof(ConfigCore))).ToArray();
        [SerializeField]public List<string> IDS; //[HideInInspector]
        private static FieldInfo[] _fields;
        protected List<Dictionary<string, string>> _json;

        public delegate void LoadSheetsDelegate(int count);
        public virtual IEnumerator Load(string sheetName, string ID, string APIKey, LoadSheetsDelegate callback) {
            yield return null;
        }

        public virtual void ConnectData<L, U>(Config<L, U> config) where L : class where U : class { }

        public virtual void ConnectData(ConfigsLoader configs) { }

        public virtual void ConnectParseField<L>() where L : class {}

        public virtual void ConnectParseField(ConfigsLoader configs, FieldInfo field) { }
    }
}