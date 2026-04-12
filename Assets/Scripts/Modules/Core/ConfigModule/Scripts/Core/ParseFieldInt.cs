using System;
using System.Linq;

namespace vikwhite
{
    [Serializable]
    public class ParseFieldInt
    {
        public string[] Id;
        public int[] Counts;
        public int Count => Id.Length;

        public ParseFieldInt(string date) {
            if(date == "") return;
            string[] datas = date.Split(';').ToList().FindAll(e => e != "-").ToArray();
            Id = new string[datas.Length];
            Counts = new int[datas.Length];
            for(int i = 0; i < datas.Length; i++) {
                string[] row = datas[i].Split('-');
                Id[i] = row[0];
                Counts[i] = int.Parse(row[1].Replace(" ", ""));
            }
        }
    }
}