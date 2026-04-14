using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Reflection;
using System.Globalization;

namespace vikwhite.Data
{
    public interface IConfig<I> where I : class
    {
        I Get(string id);
        I Get(int index = 0);
        List<I> GetAll();
        bool Contains(string id);
    }

    [Serializable]
    public class Config<T, I> : ConfigCore, IConfig<I> where T : class where I : class
    {
        public List<T> Data;
        public List<I> IData;
        private Dictionary<string, T> _dictionary;

        public override IEnumerator Load(string sheetName, string ID, string APIKey, LoadSheetsDelegate callback) {
            string url = $"https://sheets.googleapis.com/v4/spreadsheets/{ID}/values/{sheetName}?key={APIKey}";
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            bool isError = www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError;
            callback(!isError ? LoadJSON(www.downloadHandler.text) : 0);
        }

        public override void ConnectData<L, U>(Config<L, U> config) where L : class where U : class {
            foreach(FieldInfo field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public)) {
                if(field.FieldType.Equals(typeof(L)) || typeof(L).IsSubclassOf(field.FieldType)) {
                    for(int i = 0; i < Data.Count; i++) {
                        string id = _json[i][field.Name];
                        if(id != "") {
                            field.SetValue(Data[i], config.GetItem(id));
                        }
                    }
                }
            }
        }

        public override void ConnectData(ConfigsLoader configs) {
            foreach(FieldInfo field in Fields) {
                (field.GetValue(configs) as ConfigCore).ConnectData<T, I>(this);
            }
        }

        public override void ConnectParseField<L>() where L : class {
            foreach(FieldInfo field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public)) {
                if(field.FieldType.Equals(typeof(ParseFieldInt)) || field.FieldType.IsSubclassOf(typeof(ParseFieldInt))) {
                    for(int i = 0; i < Data.Count; i++) {
                        if(_json[i].ContainsKey(field.Name)) {
                            field.SetValue(Data[i], CreateDataObjectInt(field.FieldType, _json[i][field.Name]));
                        }
                    }
                }
                if(field.FieldType.Equals(typeof(ParseFieldFloat)) || field.FieldType.IsSubclassOf(typeof(ParseFieldFloat))) {
                    for(int i = 0; i < Data.Count; i++) {
                        if(_json[i].ContainsKey(field.Name)) {
                            field.SetValue(Data[i], CreateDataObjectFloat(field.FieldType, _json[i][field.Name]));
                        }
                    }
                }
            }
        }

        public override void ConnectParseField(ConfigsLoader configs, FieldInfo field) {
            (field.GetValue(configs) as ConfigCore).ConnectParseField<T>();
        }

        public T Add(string id, T data) {
            if(IDS == null) IDS = new List<string>();
            if(_dictionary == null) _dictionary = new Dictionary<string, T>();
            IDS.Add(id);
            Data.Add(data);
            _dictionary.Add(id, data);
            return _dictionary[id];
        }

        public void Delete(string id) {
            T data = GetItem(id);
            if(data == null) return;
            _dictionary.Remove(id);
            Data.Remove(data);
            IDS.Remove(id);
        }

        public void Rename(string oldID, string newID) {
            T data = GetItem(oldID);
            if(data == null) return;
            int index = Data.IndexOf(data);
            Data.RemoveAt(index);
            Data.Insert(index, data);
            IDS[index] = newID;
            _dictionary.Remove(oldID);
            _dictionary.Add(newID, data);
        }

        public T GetItem(string id) {
            if(_dictionary == null) InitDictionary();
            if(!_dictionary.ContainsKey(id)) return null;
            return _dictionary[id];
        }

        public I Get(string id) {
            if(_dictionary == null) InitDictionary();
            if(!_dictionary.ContainsKey(id)) return null;
            return _dictionary[id] as I;
        }

        public I Get(int index = 0) {
            return Data[index] as I;
        }

        public List<I> GetAll() {
            if(IData == null) {
                IData = new List<I>();
                foreach(var item in Data)
                    IData.Add(item as I);
            }
            return IData;
        }

        public bool Contains(string id) {
            return GetItem(id) != null;
        }

        private int LoadJSON(string jsonText) {
            _dictionary = null;
            Data = new List<T>();
            IDS = new List<string>();
            if (typeof(T).GetCustomAttribute<OneRowConfigAttribute>() != null) _json = ParseOneRowJSON(jsonText);
            else _json = ParseJSON(jsonText);
            foreach(Dictionary<string, string> row in _json) {
                Data.Add(CreateData(row));
                if(row.ContainsKey("ID")) IDS.Add(row["ID"]);
            }
            return Data.Count;
        }

        private List<Dictionary<string, string>> ParseJSON(string text) {
            var json = JSON.Parse(text);

            List<string> titles = new List<string>();
            foreach(var item in json["values"][0]) {
                titles.Add(item.Value);
            }

            List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
            for(int i = 1; i < json["values"].Count; i++) {
                Dictionary<string, string> row = new Dictionary<string, string>();
                for(int n = 0; n < titles.Count; n++) {
                    if(titles[n] != "" && !row.ContainsKey(titles[n])) row.Add(titles[n], json["values"][i][n].Value);
                }
                data.Add(row);
            }
            return data;
        }

        private List<Dictionary<string, string>> ParseOneRowJSON(string text) {
            var json = JSON.Parse(text);

            List<string> titles = new List<string>();
            List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
            Dictionary<string, string> row = new Dictionary<string, string>();
            data.Add(row);
            for(int i = 1; i < json["values"].Count; i++) {
                row.Add(json["values"][i][0].Value, json["values"][i][1].Value);
            }
            return data;
        }

        private T CreateData(Dictionary<string, string> row) {
            T data = CreateDataObject();
            foreach(FieldInfo field in typeof(T).GetFields()) {
                if(row.ContainsKey(field.Name)) {
                    if(field.FieldType == typeof(string)) field.SetValue(data, row[field.Name]);
                    else if(field.FieldType == typeof(int)) field.SetValue(data, row[field.Name] != "" ? int.Parse(row[field.Name].Replace(" ", "")) : 0);
                    else if (field.FieldType == typeof(float))
                    {
                        var value = row[field.Name].Replace(",", ".");
                        field.SetValue(data, value != "" ? float.Parse(value, CultureInfo.InvariantCulture) : 0f);
                    } 
                    else if(field.FieldType == typeof(bool)) field.SetValue(data, row[field.Name] != "" ? row[field.Name] == "TRUE" : false);
                    //else if(field.FieldType == typeof(GameObject)) field.SetValue(data, ResourcesEx.Load<GameObject>(Constants.Directories.Prefabs.Paths, row[field.Name]));
                    //else if(field.FieldType == typeof(Sprite)) field.SetValue(data, ResourcesEx.Load<Sprite>(Constants.Directories.Images.Paths, row[field.Name]));
                    //else if(field.FieldType == typeof(Material)) field.SetValue(data, ResourcesEx.Load<Material>(Constants.Directories.Materials.Paths, row[field.Name]));
                    //else if(field.FieldType == typeof(Color)) field.SetValue(data, row[field.Name] != "" ? ColorUtils.FromHex(row[field.Name]) : Color.white);
                    else if(field.FieldType.IsEnum) field.SetValue(data, (int)Enum.Parse(field.FieldType, row[field.Name]));
                }
            }
            if (data is ICustomJsonParser parser) parser.Parse(row);
            return data;
        }

        private T CreateDataObject() {
            ConstructorInfo info = typeof(T).GetConstructor(new Type[] { });
            return info.Invoke(new object[] { }) as T;
        }

        private ParseFieldInt CreateDataObjectInt(Type type, string str) {
            ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(string) }, null);
            return info.Invoke(new object[] { str }) as ParseFieldInt;
        }

        private ParseFieldFloat CreateDataObjectFloat(Type type, string str) {
            ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(string) }, null);
            return info.Invoke(new object[] { str }) as ParseFieldFloat;
        }

        private void InitDictionary() {
            _dictionary = new Dictionary<string, T>();
            if(IDS == null) IDS = new List<string>();
            for(int i = 0; i < Data.Count; i++) {
                if(!_dictionary.ContainsKey(IDS[i]))
                    _dictionary.Add(IDS[i], Data[i]);
            }
        }
    }
}