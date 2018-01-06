using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Linq;

namespace Individua.CLIMondb
{
    class Program
    {
        private readonly AlterDb alterDb = new AlterDb();
        static void Main(string[] args)
        {
            // MongoClient 连接
            //var client = new MongoClient("mongodb://192.168.1.172:27017");
            ////获取数据库
            //var database = client.GetDatabase("test");
            //// 获取集合
            //var collection = database.GetCollection<BsonDocument>("bar");
            MongodbHost mongodbHost = new MongodbHost
            {
                Connection = "mongodb://192.168.1.167:27017",
                DataBase = "Test",
                Table = "users"
            };

            Selcs selcs = new Selcs();
            selcs.UpdUserMsg(mongodbHost);

            //for (int i=5;i>0;i--) {
            //    Console.WriteLine(ObjectId.GenerateNewId());
            //}


            //int num = MongoDbHelper<Bar>.Add(mongodbHost, new Bar("zhangsan", "lisi", 34, new Info(111, 222)));
            //Console.WriteLine(num);

            //var list = new Bar
            //{
            //    Id = ObjectId.GenerateNewId(),
            //    Name = "zhangsan",
            //    Count = 22,
            //    Counter = 23,
            //    Type = "user",
            //    Info = new Info(22, 23)
            //};
            //var list1 = new Bar
            //{
            //    Id = ObjectId.GenerateNewId(),
            //    Name = "lisi",
            //    Count = 11,
            //    Counter = 33,
            //    Type = "teacher",
            //    Info = new Info(11, 33)
            //};
            //var list2 = new Bar
            //{
            //    Id = ObjectId.GenerateNewId(),             
            //    Name = "wangwu",
            //    Count = 15,
            //    Counter = 25,
            //    Type = "user",
            //    Info = new Info(15, 25)
            //};
            //var list3 = new Bar
            //{
            //    Id = ObjectId.GenerateNewId(),
            //    Name = "zhaoliu",
            //    Count = 14,
            //    Counter = 53,
            //    Type = "user",
            //    Info = new Info(14, 53)
            //};
            //var list4 = new Bar
            //{
            //    Id = ObjectId.GenerateNewId(),
            //    Name = "chenqiq",
            //    Count = 23,
            //    Counter = 32,
            //    Type = "user",
            //    Info = new Info(23, 32)
            //};
            //List<Bar> documents = new List<Bar>
            //{
            //    list,list1,list2,list3,list4
            //};
            //int num = MongoDbHelper<Bar>.InsertMany(mongodbHost, documents);
            //Console.WriteLine(num);

            ////1.批量修改,修改的条件
            //var time = DateTime.Now;
            //var list = new List<FilterDefinition<Bar>>
            //{
            //    Builders<Bar>.Filter.Lte("counter", 9),
            //    Builders<Bar>.Filter.Gte("counter", 1)
            //};
            //var filter = Builders<Bar>.Filter.And(list);

            ////2.要修改的字段内容
            //var dic = new Dictionary<string, string>
            //{
            //    { "UseAge", "168" },
            //    { "Name", "朝阳" }
            //};
            ////3.批量修改
            //var kk = MongoDbHelper<Bar>.UpdateManay(mongodbHost, dic, filter);

            //Console.WriteLine(kk.IsAcknowledged);
            //Console.WriteLine(kk.IsModifiedCountAvailable);
            //Console.WriteLine(kk.MatchedCount);
            //Console.WriteLine(kk.ModifiedCount);


            ////根据条件查询集合
            //var list = new List<FilterDefinition<Bar>>
            //{
            //    //Builders<Bar>.Filter.Lte("counter", 9),
            //    Builders<Bar>.Filter.Lt("Count", 20)
            //};
            //var filter = Builders<Bar>.Filter.And(list);
            ////2.查询字段
            //var field = new[] { "Name", "Count", "Counter", "Type" ,"Info"};
            ////3.排序字段
            //var sort = Builders<Bar>.Sort.Descending("Count");
            //List<Bar> res = MongoDbHelper<Bar>.FindList(mongodbHost, filter, null, sort);
            //Console.WriteLine(res.Count);
            //Console.WriteLine(res.First().Info.X);
            //Console.WriteLine(res.ToArray().ToJson());


            ////分页查询，查询条件
            //var time = DateTime.Now;
            //var list = new List<FilterDefinition<PhoneEntity>>();
            //list.Add(Builders<PhoneEntity>.Filter.Lt("AddTime", time.AddDays(400)));
            //list.Add(Builders<PhoneEntity>.Filter.Gt("AddTime", time));
            //var filter = Builders<PhoneEntity>.Filter.And(list);
            //long count = 0;
            ////排序条件
            //var sort = Builders<PhoneEntity>.Sort.Descending("AddTime");
            //var res = TMongodbHelper<PhoneEntity>.FindListByPage(host, filter, 2, 10, out count, null, sort);

            Console.ReadKey();
        }
    }
}