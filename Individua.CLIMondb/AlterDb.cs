using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Individua.CLIMondb
{
    class AlterDb
    {
        //在集合中找到第一个文档
        //var document = collection.Find(new BsonDocument()).FirstOrDefault();
        //var document = await collection.Find(new BsonDocument()).FirstOrDefaultAsync();// 异步

        //Console.WriteLine(document.ToString());
        // 构建文档
        // document = new BsonDocument {
        //    { "name", "MongoDB" },
        //    { "type", "Database" },
        //    { "count", 1 },
        //    { "info", new BsonDocument
        //        {
        //            { "x", 203 },
        //            { "y", 102 }
        //        }
        //    }
        //};

        //插入文档
        //collection.InsertOne(document);
        //await collection.InsertOneAsync(document);  // 异步

        // 插入多个文档
        //var documents = Enumerable.Range(0, 100).Select(i => new BsonDocument("counter", i));

        // 插入多个文档
        //collection.InsertMany(documents);
        // await collection.InsertManyAsync(documents);  // 异步

        //计数文件
        //var count = collection.CountAsync(new BsonDocument());
        //var count = await collection.CountAsync(new BsonDocument());// 异步

        //Console.WriteLine(count);

        //查找集合中的所有文档
        //var documents = collection.Find(new BsonDocument()).ToList();
        //var documents = await collection.Find(new BsonDocument()).ToListAsync(); //异步

        //如果文件数量预计会较大或可以迭代处理，ForEachAsync则会为每个返回的文档调用回调。
        //await collection.Find(new BsonDocument()).ForEachAsync(d => Console.WriteLine(d));


        //要使用同步API迭代返回的文档，请使用带有ToEnumerable适配器方法的C＃foreach语句：
        //var cursor = collection.Find(new BsonDocument()).ToCursor();
        //Console.WriteLine(cursor);
        //foreach (var documentOne in cursor.ToEnumerable())
        //{
        //    Console.WriteLine(documentOne);
        //}

        //获取一组带有过滤器的文档
        //var filter = Builders<BsonDocument>.Filter.Eq("counter", 71);
        //Console.WriteLine(filter);
        ////var documenttwo = collection.Find(filter).First();// 获取搜索出的第一个
        ////var document = await collection.Find(filter).FirstAsync();//异步
        //foreach (var documenttwo in collection.Find(filter).ToEnumerable())
        //{
        //    Console.WriteLine(documenttwo);
        //}

        //获取一组带有过滤器的文档
        //var filter = Builders<BsonDocument>.Filter.Gt("counter", 50);
        //// 范围，22 < i <= 34：
        //var filterBuilder = Builders<BsonDocument>.Filter;
        //var filter = filterBuilder.Gt("counter", 2) & filterBuilder.Lte("counter", 11);
        ////var cursor = collection.Find(filter).ToCursor();
        //Console.WriteLine(cursor.ToList().ToJson());
        //foreach (var documentthree in cursor.ToEnumerable())
        //{
        //    Console.WriteLine(documentthree);
        //}
        //await collection.Find(filter).ForEachAsync(document => Console.WriteLine(document));

        ////使用Exists过滤器构建器方法和Descending排序构建器方法来排序
        //var filter = Builders<BsonDocument>.Filter.Exists("num");
        //var sort = Builders<BsonDocument>.Sort.Descending("counter");
        //var documentfour = collection.Find(filter).Sort(sort).First();
        ////var documentfour = await collection.Find(filter).Sort(sort).FirstAsync();
        //Console.WriteLine(documentfour);

        //排除“_id”字段并输出第一个匹配

        //var projection = Builders<BsonDocument>.Projection.Exclude("_id");
        ////var cursor = collection.Find(new BsonDocument()).Project(projection).ToCursor();
        ////Console.WriteLine(cursor.ToList().ToJson());

        //var documentfive = collection.Find(new BsonDocument()).Project(projection).First();
        //var document = await collection.Find(new BsonDocument()).Project(projection).FirstAsync();
        //Console.WriteLine(documentfive.ToString());

        // 修改
        //var filter = Builders<BsonDocument>.Filter.Gt("counter", 80);
        //var update = Builders<BsonDocument>.Update.Inc("num", 24);
        //collection.UpdateOne(filter, update);

        // 删除
        //var filter = Builders<BsonDocument>.Filter.Eq("name", "MongoDB");
        //collection.DeleteOne(filter);
        //await collection.DeleteOneAsync(filter);
        //var filter = Builders<BsonDocument>.Filter.Gte("counter", 10);
        //var result = collection.DeleteMany(filter);
        //Console.WriteLine(result.DeletedCount);
    }
}
