using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Individua.CLIMondb
{
    //[BsonIgnoreExtraElements]
    class Bar
    {
        private ObjectId id;
        private string name;
        private string type;
        private int count;
        private int counter;
        private Info info;

        public ObjectId Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Type { get => type; set => type = value; }
        public int Count { get => count; set => count = value; }
        public Info Info { get => info; set => info = value; }
        public int Counter { get => counter; set => counter = value; }

        public Bar()
        {
        }

        public Bar(string name, string type, int count, Info info)
        {
            this.name = name;
            this.type = type;
            this.count = count;
            this.info = info;
        }
    }

    class Info
    {
        private int x;
        private int y;

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }

        public Info(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Info()
        {
        }
    }

}
