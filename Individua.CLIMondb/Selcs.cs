using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Individua.CLIMondb
{
    class Selcs
    {
        /// <summary>
        /// 查询 表信息   异步
        /// </summary>
        /// <param name="mongodbHost"></param>
        /// <returns></returns>
        public string SelUserMsg(MongodbHost mongodbHost) {
            //根据条件查询集合
            var list = new List<FilterDefinition<Bar>>
            {
                //Builders<Bar>.Filter.Lte("counter", 9),
                Builders<Bar>.Filter.Lt("Count", 20)
            };
            var filter = Builders<Bar>.Filter.And(list);
            //2.查询字段
            var field = new[] { "Name", "Count", "Counter", "Type", "Info" };
            //3.排序字段
            var sort = Builders<Bar>.Sort.Descending("Count");
            //List<Bar> res =  MongoDbHelper<Bar>.FindList(mongodbHost, filter, field, sort);
            var res = MongoDbHelper<Bar>.FindListAsync(mongodbHost, filter, field, sort);
            Console.WriteLine(res.Result.Count);
            Console.WriteLine(res.Result.First().Name);
            Console.WriteLine(res.Id);
            Console.WriteLine(res.Result.First().Info.X);
            Console.WriteLine(res.Result.ToArray().ToJson());
            return "Success";
        }

        /// <summary>
        /// 修改一条信息
        /// </summary>
        /// <param name="mongodbHost"></param>
        /// <returns></returns>
        public string UpdUserMsg(MongodbHost mongodbHost)
        {
            //根据条件查询集合
            var list = new List<FilterDefinition<Bar>>
            {
                //Builders<Bar>.Filter.Lte("counter", 9),
                Builders<Bar>.Filter.Lt("Count", 20)
            };
            var filter = Builders<Bar>.Filter.And(list);
            //2.查询字段
            var field = new[] { "Name", "Count", "Counter", "Type", "Info" };
            //3.排序字段
            var sort = Builders<Bar>.Sort.Descending("Count");
            //List<Bar> res =  MongoDbHelper<Bar>.FindList(mongodbHost, filter, field, sort);
            var res = MongoDbHelper<Bar>.FindListAsync(mongodbHost, filter, field, sort);
            Bar bar = res.Result.First();
            bar.Type = "admin222";
            var ress = MongoDbHelper<Bar>.UpdateAsync(mongodbHost, bar,bar.Id.ToString());
            Console.WriteLine(ress.Id);
            Console.WriteLine(ress.Result.UpsertedId);
            Console.WriteLine(ress.Result.MatchedCount);
            Console.WriteLine(res.Result.ToJson());
            return "Success";
        }

        

    }
}
