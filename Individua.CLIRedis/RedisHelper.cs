using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace Individua.CLIRedis
{
    /// <summary>
    /// RedisHelper
    /// 一个复杂的的场景中可能包含有主从复制 ， 对于这种情况，只需要指定所有地址在连接字符串中（它将会自动识别出主服务器）
    ///  ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("server1:6379,server2:6379");
    /// </summary>
    public static class RedisHelper
    {
        /// <summary>
        /// 序列化方式
        /// </summary>
        public enum SerializationStyle
        {
            //.Net对象序列化
            Binary = 1,
            //Json序列化
            Json = 2,
        }

        /// <summary>
        /// redis数据类型
        /// </summary>
        public enum Redistype
        {
            /// <summary>
            /// key不存在返回
            /// </summary>
            None = 0,
            /// <summary>
            /// 字符串类型
            /// </summary>
            String = 1,
            /// <summary>
            /// 列表
            /// </summary>
            List = 2,
            /// <summary>
            /// 无序集合
            /// </summary>
            Set = 3,
            /// <summary>
            /// 有序集合
            /// </summary>
            SortedSet = 4,
            /// <summary>
            /// hash表
            /// </summary>
            Hash = 5,
            /// <summary>
            /// 客户端库无法识别数据类型
            /// </summary>
            Unknown = 6
        }

        private static readonly string Connectstr;
        public static readonly string redisallkeys = "redisallkeys";

        /// <summary>
        /// 共享应用程序中的 ConnectionMultiplexer 实例的一个方法是，拥有返回连接示例的静态属性（与下列示例类似）。 
        /// 这种线程安全方法，可仅初始化单一连接的 ConnectionMultiplexer 实例。 
        /// Redis 缓存连接，也可成功调用。 ConnectionMultiplexer 的一个关键功能是，
        /// 一旦还原网络问题和其他原因，它将自动还原缓存连接。
        /// Lazy<T> 对象初始化默认是线程安全的，在多线程环境下，第一个访问 Lazy<T> 对象的 Value 属性的线程将初始化 Lazy<T> 对象;
        /// 以后访问的线程都将使用第一次初始化的数据。
        /// </summary>
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(Connectstr);
        });

        public static ConnectionMultiplexer Instance
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        static RedisHelper()
        {
            //var builder = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory + "Redis")
            //    .AddJsonFile("Cache.json", optional: true, reloadOnChange: true);
            //var configroot = builder.Build();
            //var serversection = configroot.GetSection("servers");
            //var serversections = serversection.GetChildren();
            List<string> endpoints = new List<string>
            {
                //foreach (var serse in serversections)
                //{
                //    string host = serse["host"];
                //    string port = serse["port"];
                //    string attr = serse["attr"];
                //    if (!string.IsNullOrEmpty(host))
                //    {
                //        var endpoint = host;
                //        if (!string.IsNullOrEmpty(port))
                //        {
                //            endpoint += ":" + port;
                //        }
                //        if (!string.IsNullOrEmpty(attr))
                //        {
                //            endpoint += "," + attr;
                //        }
                //        endpoints.Add(endpoint);
                //    }
                //}
                "127.0.0.1:6379"
            };
            if (endpoints.Any())
            {
                Connectstr = string.Join(",", endpoints);
            }
            else
            {
                throw new Exception("配置连接失败");
            }
        }

        /// <summary>
        /// 获取缓存实例
        /// </summary>
        /// <returns></returns>
        public static IDatabase CacheDatabase
        {
            get
            {
                return Instance.GetDatabase(15);
            }
        }

        #region 缓存键操作
        /// <summary>
        /// 根据key返回存储的Redis类型
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Redistype KeyType(string key)
        {
            RedisType rtype = CacheDatabase.KeyType(key);
            return (Redistype)Enum.ToObject(typeof(Redistype), rtype.GetHashCode());
        }

        /// <summary>
        /// 根据key删除缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Del(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            // 从所有redis的key有序集合中移除
            ListRemoveAsync(redisallkeys, key);
            return CacheDatabase.KeyDelete(key);
        }

        /// <summary>
        /// 异步根据key删除缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Task<bool> DelAsync(string key)
        {
            return Task.Factory.StartNew(() =>
            {
                return Del(key);
            });
        }

        /// <summary>
        /// 判断在缓存中是否存在该key的缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            return CacheDatabase.KeyExists(key);
        }

        #endregion

        #region 字符串
        /// <summary>
        /// 根据key获取缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static T Get<T>(string key, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default(T);
            }
            RedisValue val;
            val = CacheDatabase.StringGet(key);
            if (!val.HasValue)
            {
                return default(T);
            }
            if (sstyle == SerializationStyle.Json)
            {
                return JsonConvert.DeserializeObject<T>(val);
            }
            return Deserialize<T>(val);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        public static bool Set<T>(string key, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            RedisValue val;
            if (sstyle == SerializationStyle.Json)
            {
                val = JsonConvert.SerializeObject(value);
            }
            else
            {
                val = Serialize(value);
            }
            // 添加到所有redis的key有序集合中
            ListAddAsync(redisallkeys, key);
            return CacheDatabase.StringSet(key, val);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="seconds"></param>
        /// <param name="sstyle"></param>
        public static bool Set<T>(string key, T value, int seconds, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            RedisValue val;
            if (sstyle == SerializationStyle.Json)
            {
                val = JsonConvert.SerializeObject(value);
            }
            else
            {
                val = Serialize(value);
            }
            // 添加到所有redis的key有序集合中
            ListAddAsync(redisallkeys, key);
            return CacheDatabase.StringSet(key, val, new TimeSpan(0, 0, 0, seconds));
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ts"></param>
        /// <param name="sstyle"></param>
        public static bool Set<T>(string key, T value, TimeSpan ts, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            RedisValue val;
            if (sstyle == SerializationStyle.Json)
            {
                val = JsonConvert.SerializeObject(value);
            }
            else
            {
                val = Serialize(value);
            }
            // 添加到所有redis的key有序集合中
            ListAddAsync(redisallkeys, key);
            return CacheDatabase.StringSet(key, val, ts);
        }

        /// <summary>
        /// 异步设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        public static Task<bool> SetAsync<T>(string key, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                return Set(key, value, sstyle);
            });
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="seconds"></param>
        /// <param name="sstyle"></param>
        public static Task<bool> SetAsync<T>(string key, T value, int seconds, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                return Set(key, value, seconds, sstyle);
            });
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ts"></param>
        /// <param name="sstyle"></param>
        public static Task<bool> SetAsync<T>(string key, T value, TimeSpan ts, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                return Set(key, value, ts, sstyle);
            });
        }

        /// <summary>
        /// 根据key获取缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<T> GetAsync<T>(string key, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                return Get<T>(key, sstyle);
            });
        }

        /// <summary>
        /// 实现递增
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long Increment(string key)
        {
            //三种命令模式
            //Sync,同步模式会直接阻塞调用者，但是显然不会阻塞其他线程。
            //Async,异步模式直接走的是Task模型。
            //Fire - and - Forget,就是发送命令，然后完全不关心最终什么时候完成命令操作。
            //即发即弃：通过配置 CommandFlags 来实现即发即弃功能，在该实例中该方法会立即返回，如果是string则返回null 如果是int则返回0.这个操作将会继续在后台运行，一个典型的用法页面计数器的实现：
            return CacheDatabase.StringIncrement(key, flags: CommandFlags.FireAndForget);
        }

        /// <summary>
        /// 实现递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long Decrement(string key, string value)
        {
            return CacheDatabase.HashDecrement(key, value, flags: CommandFlags.FireAndForget);
        }

        #endregion

        #region 无序集合

        /// <summary>
        /// 设置添加无序集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        public static bool SetAdd<T>(string key, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            RedisValue val;
            if (sstyle == SerializationStyle.Json)
            {
                val = JsonConvert.SerializeObject(value);
            }
            else
            {
                val = Serialize(value);
            }
            // 添加到所有redis的key有序集合中
            ListAddAsync(redisallkeys, key);
            return CacheDatabase.SetAdd(key, val);
        }

        /// <summary>
        /// 获取无序集合中元素的数量
        /// </summary>
        /// <param name="key"></param>
        public static long SetCount(string key)
        {
            return CacheDatabase.SetLength(key);
        }

        /// <summary>
        /// 异步设置添加无序集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        public static Task<bool> SetAddAsync<T>(string key, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                return SetAdd(key, value, sstyle);
            });
        }

        /// <summary>
        /// 获取key对应的无序集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static List<T> GetMembers<T>(string key, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            List<T> results = new List<T>();
            RedisValue[] vals;
            vals = CacheDatabase.SetMembers(key);
            if (vals == null || !vals.Any())
            {
                return new List<T>();
            }
            foreach (RedisValue val in vals)
            {
                if (sstyle == SerializationStyle.Json)
                {
                    results.Add(JsonConvert.DeserializeObject<T>(val));
                }
                else
                {
                    results.Add(Deserialize<T>(val));
                }
            }
            return results;
        }

        /// <summary>
        /// 异步获取的无序集合的所有元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<List<T>> GetMembersAsync<T>(string key, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                return GetMembers<T>(key, sstyle);
            });
        }


        /// <summary>
        /// 异步判断数据是否存在无序集合中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        public static Task<bool> SetExistsAsync<T>(string key, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                RedisValue val;
                if (sstyle == SerializationStyle.Json)
                {
                    val = JsonConvert.SerializeObject(value);
                }
                else
                {
                    val = Serialize(value);
                }
                return CacheDatabase.SetContains(key, val);
            });
        }

        /// <summary>
        /// 设置集合移动
        /// </summary>
        /// <param name="sourcekey">原始key</param>
        /// <param name="destinationkey">目标key</param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        public static bool SetMove<T>(string sourcekey, string destinationkey, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            RedisValue val;
            if (sstyle == SerializationStyle.Json)
            {
                val = JsonConvert.SerializeObject(value);
            }
            else
            {
                val = Serialize(value);
            }
            return CacheDatabase.SetMove(sourcekey, destinationkey, val);
        }

        /// <summary>
        /// 异步设置集合移动
        /// </summary>
        /// <param name="sourcekey">原始key</param>
        /// <param name="destinationkey">目标key</param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        public static Task<bool> SetMoveAsync<T>(string sourcekey, string destinationkey, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                return SetMove(sourcekey, destinationkey, value, sstyle);
            });
        }

        /// <summary>
        /// 移除集合中的一个或多个成员元素，不存在的成员元素会被忽略。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<bool> SetRemoveAsync<T>(string key, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                RedisValue val;
                if (sstyle == SerializationStyle.Json)
                {
                    val = JsonConvert.SerializeObject(value);
                }
                else
                {
                    val = Serialize(value);
                }
                return CacheDatabase.SetRemove(key, val);
            });
        }

        /// <summary>
        /// 移除集合中的一个或多个成员元素，不存在的成员元素会被忽略。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<bool> SetRemoveAsync<T>(string key, T[] values, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                if (sstyle == SerializationStyle.Json)
                {
                    foreach (object value in values)
                    {
                        RedisValue val = JsonConvert.SerializeObject(value);
                        CacheDatabase.SetRemove(key, val);
                    }
                }
                else
                {
                    foreach (object value in values)
                    {
                        RedisValue val = Serialize(value);
                        CacheDatabase.SetRemove(key, val);
                    }
                }
                return true;
            });
        }
        #endregion

        #region 有序集合

        /// <summary>
        /// 设置添加到有序集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        public static bool SortSetAdd<T>(string key, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            RedisValue val;
            if (sstyle == SerializationStyle.Json)
            {
                val = JsonConvert.SerializeObject(value);
            }
            else
            {
                val = Serialize(value);
            }
            // 添加到所有redis的key有序集合中
            ListAddAsync(redisallkeys, key);
            long score = DateTime.Now.Ticks - new DateTime(2016, 10, 1).Ticks;
            return CacheDatabase.SortedSetAdd(key, val, score);
        }

        /// <summary>
        /// 设置添加到有序集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        /// <param name="sstyle"></param>
        public static bool SortSetAdd<T>(string key, T value, int score, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            RedisValue val;
            if (sstyle == SerializationStyle.Json)
            {
                val = JsonConvert.SerializeObject(value);
            }
            else
            {
                val = Serialize(value);
            }
            // 添加到所有redis的key有序集合中
            ListAddAsync(redisallkeys, key);
            return CacheDatabase.SortedSetAdd(key, val, score);
        }

        /// <summary>
        /// 异步设置添加到有序集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        public static Task<bool> SortSetAddAsync<T>(string key, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                return SortSetAdd(key, value, sstyle);
            });
        }

        /// <summary>
        /// 异步设置添加到有序集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        /// <param name="sstyle"></param>
        public static Task<bool> SortSetAddAsync<T>(string key, T value, int score, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                return SortSetAdd(key, value, score);
            });
        }

        /// <summary>
        /// 异步设置添加到有序集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="sstyle"></param>
        public static Task<bool> SortSetAddArrayAsync<T>(string key, T[] values, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                foreach (var value in values)
                {
                    SortSetAdd(key, value, sstyle);
                }
                return true;
            });
        }

        /// <summary>
        /// 异步覆盖到有序集合:value存在时覆盖，value不存在时添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        public static Task<bool> SortSetCoverAsync<T>(string key, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            long score = DateTime.Now.Ticks - new DateTime(2016, 10, 1).Ticks;
            return SortSetCoverAsync(key, value, score, sstyle);
        }

        /// <summary>
        /// 异步覆盖到有序集合:value存在时覆盖，value不存在时添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        /// <param name="sstyle"></param>
        public static Task<bool> SortSetCoverAsync<T>(string key, T value, long score, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                RedisValue val;
                if (sstyle == SerializationStyle.Json)
                {
                    val = JsonConvert.SerializeObject(value);
                }
                else
                {
                    val = Serialize(value);
                }
                //删除已存在的数据
                CacheDatabase.SortedSetRemove(key, val);
                //向有序集合中新增
                return CacheDatabase.SortedSetAdd(key, val, score);
            });
        }

        /// <summary>
        /// 移除有序集合key里的value值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        public static bool SortSetRemove<T>(string key, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            RedisValue val;
            if (sstyle == SerializationStyle.Json)
            {
                val = JsonConvert.SerializeObject(value);
            }
            else
            {
                val = Serialize(value);
            }
            return CacheDatabase.SortedSetRemove(key, val);
        }

        /// <summary>
        /// 异步移除有序集合key里的value值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        public static Task<bool> SortSetRemoveAsync<T>(string key, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                return SortSetRemove(key, value, sstyle);
            });
        }

        /// <summary>
        /// 异步移除有序集合key里的索引index的项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        public static Task<bool> SortSetRemoveByBankAsync(string key, long index)
        {
            return Task.Factory.StartNew(() =>
            {
                return CacheDatabase.SortedSetRemoveRangeByRank(key, index, index + 1) > 0;
            });
        }

        /// <summary>
        /// 有序集合的数量
        /// </summary>
        /// <param name="key"></param>
        public static long SortSetCount(string key)
        {
            return CacheDatabase.SortedSetLength(key);
        }

        /// <summary>
        /// 返回有序集合中指定成员的索引
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        public static long? SortSetRank<T>(string key, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            RedisValue val;
            if (sstyle == SerializationStyle.Json)
            {
                val = JsonConvert.SerializeObject(value);
            }
            else
            {
                val = Serialize(value);
            }
            return CacheDatabase.SortedSetRank(key, val);
        }

        /// <summary>
        /// 返回有序集合中指定成员的分数值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        public static double? SortSetScore<T>(string key, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            RedisValue val;
            if (sstyle == SerializationStyle.Json)
            {
                val = JsonConvert.SerializeObject(value);
            }
            else
            {
                val = Serialize(value);
            }
            return CacheDatabase.SortedSetScore(key, val);
        }

        /// <summary>
        /// 异步根据索引的起始终止值获取有序递增排列集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static List<T> SortSetRange<T>(string key, long start, long end, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            RedisValue[] vals;
            vals = CacheDatabase.SortedSetRangeByRank(key, start, end);
            if (vals == null || !vals.Any())
            {
                return new List<T>();
            }
            List<T> results = new List<T>();
            if (sstyle == SerializationStyle.Json)
            {
                foreach (RedisValue val in vals)
                {
                    results.Add(JsonConvert.DeserializeObject<T>(val));
                }
            }
            else
            {
                foreach (RedisValue val in vals)
                {
                    results.Add(Deserialize<T>(val));
                }
            }
            return results;
        }


        /// <summary>
        /// 异步根据索引的起始终止值获取有序递增排列集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<List<T>> SortSetRangeAsync<T>(string key, long start, long end, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                return SortSetRange<T>(key, start, end, sstyle);
            });
        }

        /// <summary>
        /// 异步根据索引值的起始终止值删除有序递增排列集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Task<long> SortSetRemoveRangeAsync(string key, long start, long end)
        {
            return Task.Factory.StartNew(() =>
            {
                return CacheDatabase.SortedSetRemoveRangeByValue(key, start, end);
            });
        }

        /// <summary>
        /// 异步根据索引值的起始终止值删除有序递增排列集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<bool> SortSetResetAsync<T>(string key, List<T> values, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                //删除有序集合中所有元素
                CacheDatabase.SortedSetRemoveRangeByValue(key, 0, -1);
                foreach (var value in values)
                {
                    SortSetAdd(key, value, sstyle);
                }
                return true;
            });
        }
        /// <summary>
        /// 异步根据键删除有序集合中所有元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Task<long> SortSetClearAsync(string key)
        {
            return Task.Factory.StartNew(() =>
            {
                // 删除redis的key有序集合中的键值
                ListRemoveAsync(redisallkeys, key);
                return CacheDatabase.SortedSetRemoveRangeByValue(key, 0, -1);
            });
        }

        /// <summary>
        /// 异步获取有序递增排列集合所有数据并删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<List<T>> SortSetALLReadRemoveAsync<T>(string key, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                RedisValue[] vals;
                vals = CacheDatabase.SortedSetRangeByRank(key);
                if (vals != null && vals.Any())
                {
                    // 移除集合中的数据
                    CacheDatabase.SortedSetRemoveRangeByRank(key, 0, vals.Length);
                    // 从所有redis的key有序集合中删除键
                    ListRemoveAsync(redisallkeys, key);
                }
                if (vals == null || !vals.Any())
                {
                    return new List<T>();
                }
                List<T> results = new List<T>();
                if (sstyle == SerializationStyle.Json)
                {
                    foreach (RedisValue val in vals)
                    {
                        results.Add(JsonConvert.DeserializeObject<T>(val));
                    }
                }
                else
                {
                    foreach (RedisValue val in vals)
                    {
                        results.Add(Deserialize<T>(val));
                    }
                }
                return results;
            });
        }

        /// <summary>
        /// 异步返回集合的所有元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<List<T>> SortSetALLReadAsync<T>(string key, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                List<T> result = new List<T>();
                RedisValue[] vals;
                vals = CacheDatabase.SortedSetRangeByRank(key);
                if (vals == null || !vals.Any())
                {
                    return new List<T>();
                }
                if (sstyle == SerializationStyle.Json)
                {
                    foreach (RedisValue val in vals)
                    {
                        result.Add(JsonConvert.DeserializeObject<T>(val));
                    }
                }
                else
                {
                    foreach (var val in vals)
                    {
                        result.Add(Deserialize<T>(val));
                    }
                }
                return result;
            });
        }
        #endregion

        #region 列表

        /// <summary>
        /// 异步将一个或多个值插入到列表的尾部
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<bool> ListAddAsync<T>(string key, T[] values, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                // 添加到所有redis的key有序集合中
                if (key != redisallkeys)
                {
                    // 添加到所有redis的key有序集合中
                    ListAddAsync(redisallkeys, key);
                }
                if (sstyle == SerializationStyle.Json)
                {
                    foreach (object value in values)
                    {
                        RedisValue val = JsonConvert.SerializeObject(value);
                        CacheDatabase.ListRightPush(key, val);
                    }
                }
                else
                {
                    foreach (object value in values)
                    {
                        RedisValue val = Serialize(value);
                        CacheDatabase.ListRightPush(key, val);
                    }
                }
                return true;
            });
        }

        /// <summary>
        /// 异步将一个或多个值插入到列表的尾部
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<bool> ListAddAsync<T>(string key, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                RedisValue val;
                if (sstyle == SerializationStyle.Json)
                {
                    val = JsonConvert.SerializeObject(value);
                }
                else
                {
                    val = Serialize(value);
                }
                // 添加到所有redis的key有序集合中
                if (key != redisallkeys)
                {
                    // 添加到所有redis的key有序集合中
                    ListAddAsync(redisallkeys, key);
                }
                return CacheDatabase.ListRightPush(key, val) > 0;
            });
        }

        /// <summary>
        /// 异步将一个或多个值插入到列表的尾部
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<bool> ListSetByIndexAsync<T>(string key, long index, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                RedisValue val;
                if (sstyle == SerializationStyle.Json)
                {
                    val = JsonConvert.SerializeObject(value);
                }
                else
                {
                    val = Serialize(value);
                }
                // 添加到所有redis的key有序集合中
                if (key != redisallkeys)
                {
                    // 添加到所有redis的key有序集合中
                    ListAddAsync(redisallkeys, key);
                }
                CacheDatabase.ListSetByIndex(key, index, val);
                return true;
            });
        }
        /// <summary>
        /// 异步将一个或多个值插入到列表的顶部
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<bool> ListAddTopAsync<T>(string key, T[] values, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                if (sstyle == SerializationStyle.Json)
                {
                    foreach (object value in values)
                    {
                        RedisValue val = JsonConvert.SerializeObject(value);
                        CacheDatabase.ListLeftPush(key, val);
                    }
                }
                else
                {
                    foreach (object value in values)
                    {
                        RedisValue val = Serialize(value);
                        CacheDatabase.ListLeftPush(key, val);
                    }
                }
                // 添加到所有redis的key有序集合中
                if (key != redisallkeys)
                {
                    // 添加到所有redis的key有序集合中
                    ListAddAsync(redisallkeys, key);
                }
                return true;
            });
        }

        /// <summary>
        /// 异步将一个或多个值插入到列表的尾部
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<bool> ListAddTopAsync<T>(string key, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                RedisValue val;
                if (sstyle == SerializationStyle.Json)
                {
                    val = JsonConvert.SerializeObject(value);
                }
                else
                {
                    val = Serialize(value);
                }
                // 添加到所有redis的key有序集合中
                if (key != redisallkeys)
                {
                    // 添加到所有redis的key有序集合中
                    ListAddAsync(redisallkeys, key);
                }
                return CacheDatabase.ListLeftPush(key, val) > 0;
            });
        }

        /// <summary>
        /// 返回列表的总元素数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long ListCount(string key)
        {
            return CacheDatabase.ListLength(key);
        }

        /// <summary>
        /// 异步移除并返回列表的第一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<T> ListPopAsync<T>(string key, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                RedisValue val;
                val = CacheDatabase.ListLeftPop(key);
                if (!val.HasValue)
                {
                    return default(T);
                }
                if (sstyle == SerializationStyle.Json)
                {
                    return JsonConvert.DeserializeObject<T>(val);
                }
                else
                {
                    return Deserialize<T>(val);
                }
            });
        }

        /// <summary>
        /// 异步通过索引获取列表中的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<T> ListIndexAsync<T>(string key, long index, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                RedisValue val;
                val = CacheDatabase.ListGetByIndex(key, index);
                if (!val.HasValue)
                {
                    return default(T);
                }
                if (sstyle == SerializationStyle.Json)
                {
                    return JsonConvert.DeserializeObject<T>(val);
                }
                else
                {
                    return Deserialize<T>(val);
                }
            });
        }

        /// <summary>
        /// 异步将value元素插入列表中的pivot元素之后
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<long> ListInsertAfterAsync<T>(string key, T pivot, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                RedisValue pivotval;
                RedisValue insertval;
                if (sstyle == SerializationStyle.Json)
                {
                    pivotval = JsonConvert.SerializeObject(pivot);
                    insertval = JsonConvert.SerializeObject(value);
                }
                else
                {
                    pivotval = Serialize(pivot);
                    insertval = Serialize(value);
                }
                return CacheDatabase.ListInsertAfter(key, pivotval, insertval);
            });
        }

        /// <summary>
        /// 异步将value元素插入列表中的pivot元素之前
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<long> ListInsertBeforeAsync<T>(string key, T pivot, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                RedisValue pivotval;
                RedisValue insertval;
                if (sstyle == SerializationStyle.Json)
                {
                    pivotval = JsonConvert.SerializeObject(pivot);
                    insertval = JsonConvert.SerializeObject(value);
                }
                else
                {
                    pivotval = Serialize(pivot);
                    insertval = Serialize(value);
                }
                return CacheDatabase.ListInsertBefore(key, pivotval, insertval);
            });
        }

        /// <summary>
        /// 异步返回列表的所有元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<List<T>> ListALLAsync<T>(string key, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                List<T> result = new List<T>();
                RedisValue[] vals;
                vals = CacheDatabase.ListRange(key);
                if (vals == null || !vals.Any())
                {
                    return new List<T>();
                }
                if (sstyle == SerializationStyle.Json)
                {
                    foreach (var redisval in vals)
                    {
                        result.Add(JsonConvert.DeserializeObject<T>(redisval));
                    }
                }
                else
                {
                    foreach (var redisval in vals)
                    {
                        result.Add(Deserialize<T>(redisval));
                    }
                }
                return result;
            });
        }

        /// <summary>
        /// 异步移除列表中与参数val相等的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task<bool> ListRemoveAsync<T>(string key, T value, SerializationStyle sstyle = SerializationStyle.Binary)
        {
            return Task.Factory.StartNew(() =>
            {
                RedisValue val;
                if (sstyle == SerializationStyle.Json)
                {
                    val = JsonConvert.SerializeObject(value);
                }
                else
                {
                    val = Serialize(value);
                }
                return CacheDatabase.ListRemove(key, val) > 0;
            });
        }

        /// <summary>
        /// 异步裁剪列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sstyle"></param>
        /// <returns></returns>
        public static Task ListTrimAsync(string key, long satrt, long stop)
        {
            return Task.Factory.StartNew(() =>
            {
                CacheDatabase.ListTrim(key, satrt, stop);
            });
        }

        #endregion

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        static byte[] Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, o);
                byte[] objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        static T Deserialize<T>(byte[] stream)
        {
            if (!stream.Any())
            {
                return default(T);
            }
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream(stream))
                {
                    T result = (T)binaryFormatter.Deserialize(memoryStream);
                    return result;
                }
            }
            catch (Exception )
            {
                return default(T);
            }
        }

        #region  当作消息代理中间件使用 一般使用更专业的消息队列来处理这种业务场景
        /// <summary>
        /// 当作消息代理中间件使用
        /// 消息组建中,重要的概念便是生产者,消费者,消息中间件。
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static long Publish(string channel, string message)
        {
            ISubscriber sub = Instance.GetSubscriber();
            return sub.Publish(channel, message);
        }

        /// <summary>
        /// 在消费者端得到该消息并输出
        /// </summary>
        /// <param name="channelFrom"></param>
        /// <returns></returns>
        public static void Subscribe(string channelFrom)
        {
            ISubscriber sub = Instance.GetSubscriber();
            sub.Subscribe(channelFrom, (channel, message) =>
            {
                Console.WriteLine(message);
            });
        }
        #endregion

        /// <summary>
        /// GetServer方法会接收一个EndPoint类或者一个唯一标识一台服务器的键值对
        /// 有时候需要为单个服务器指定特定的命令
        /// 使用IServer可以使用所有的shell命令，比如：
        /// DateTime lastSave = server.LastSave();
        /// ClientInfo[] clients = server.ClientList();
        /// 如果报错在连接字符串后加 ,allowAdmin=true;
        /// </summary>
        /// <returns></returns>
        public static IServer GetServer(string host, int port)
        {
            IServer server = Instance.GetServer(host, port);
            return server;
        }

        /// <summary>
        /// 获取全部终结点
        /// </summary>
        /// <returns></returns>
        public static EndPoint[] GetEndPoints()
        {
            EndPoint[] endpoints = Instance.GetEndPoints();
            return endpoints;
        }
    }
}