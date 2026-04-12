using System;
using System.Linq;

namespace vikwhite
{
    [Serializable]
    public class ParseFieldFloat
    {
        public string[] Id;
        public float[] Counts;
        public int Count => Id.Length;

        public ParseFieldFloat(string date) {
            if(date == "") return;
            string[] datas = date.Split(';').ToList().FindAll(e => e != "-").ToArray();
            Id = new string[datas.Length];
            Counts = new float[datas.Length];
            for(int i = 0; i < datas.Length; i++) {
                string[] row = datas[i].Split('-');
                Id[i] = row[0];
                Counts[i] = float.Parse(row[1].Replace(" ", ""));
            }
        }
    }
}