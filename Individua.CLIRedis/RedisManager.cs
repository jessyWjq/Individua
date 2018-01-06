// =============================================================
// 功能：Redis管理
// 作者：徐德意
// Email：862860000@qq.com
// 时间：2017-11-16 22:00
// =============================================================
using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using Individua.Util.Extensions;
using System.Configuration;

namespace Individua.CLIRedis
{
    public static class RedisManager
    {
        public static string connectionString;
        public static int dbNum;
        public static string prefix;
        static ConnectionMultiplexer _redis;
        static object _locker = new object();
        static RedisManager()
        {
            var redis_config = ConfigurationManager.AppSettings["redis_config"].Split(',');
            connectionString = redis_config[0];
            dbNum = int.Parse(redis_config[1]);
            prefix = redis_config[2];
        }
        public static ConnectionMultiplexer Manager
        {
            get
            {
                if (_redis == null)
                {
                    lock (_locker)
                    {
                        if (_redis != null) return _redis;

                        _redis = GetManager();
                        return _redis;
                    }
                }
                return _redis;
            }
        }

        static ConnectionMultiplexer GetManager()
        {
            ConfigurationOptions connection = new ConfigurationOptions();
            connection.EndPoints.Add(connectionString);
            connection.AbortOnConnectFail = false;// 当为true时，当没有可用的服务器时则不会创建一个连接
            connection.ConnectRetry = 5; // 重试连接的次数
            connection.KeepAlive = 60; // 保存x秒的活动连接
            //connection.Ssl = true;
            connection.AllowAdmin = true;
            connection.SyncTimeout = 8000; //异步超时时间
            connection.ConnectTimeout = 20000; //超时时间
            return ConnectionMultiplexer.Connect(connection);
        }

        private static string KeyFormatting(string key)
        {
            if (key.StartsWith(prefix))
            {
                return key;
            }
            else
            {
                return $"{prefix}{key}";
            }
        }
        private static string KeyRemoveFormatting(string key)
        {
            if (key.StartsWith(prefix))
            {
                return key.Remove(0, prefix.Length);
            }
            else
            {
                return key;
            }
        }


        #region Get
        /// <summary>
        /// 获取Redis Key的Value
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="key">Key.</param>
        public static string Get(string key)
        {
            var db = Manager.GetDatabase(dbNum);
            return db.StringGet(KeyFormatting(key));
        }
        /// <summary>
        /// 获取Redis Key的Value,如果有就返回,没有就先添加再返回
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="key">Key.</param>
        /// <param name="acquire">Acquire.</param>
        /// <param name="expireMinutes">Expire minutes.</param>
        public static string Get(string key, Func<string> acquire, int expireMinutes = 0)
        {
            var db = Manager.GetDatabase(dbNum);
            if (db.KeyExists(KeyFormatting(key)))
            {
                return db.StringGet(KeyFormatting(key));
            }
            else
            {
                string value = acquire.Invoke();
                if (expireMinutes > 0)
                {
                    db.StringSet(KeyFormatting(key), value, TimeSpan.FromMinutes(expireMinutes));
                }
                else
                {
                    db.StringSet(KeyFormatting(key), value);
                }
                return value;
            }
        }
        /// <summary>
        /// 获取Redis Key的Value,如果有就返回,如果没有就先添加再返回
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="key">Key.</param>
        /// <param name="acquire">Acquire.</param>
        /// <param name="expireMinutes">Expire minutes.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Get<T>(string key, Func<T> acquire, int expireMinutes = 0)
        {
            var db = Manager.GetDatabase(dbNum);
            if (db.KeyExists(KeyFormatting(key)))
            {
                string value = db.StringGet(KeyFormatting(key));
                return value.ToObject<T>();
            }
            else
            {
                T t = acquire.Invoke();
                string value = t.ToJson();
                if (expireMinutes > 0)
                {
                    db.StringSet(KeyFormatting(key), value, TimeSpan.FromMinutes(expireMinutes));
                }
                else
                {
                    db.StringSet(KeyFormatting(key), value);
                }
                return t;
            }
        }
        public static string[] GetList(string key)
        {
            var db = Manager.GetDatabase(dbNum);
            return db.ListRange(KeyFormatting(key)).ToStringArray();
        }
        #endregion

        #region Set
        /// <summary>
        /// 写入Redis 如果存在Key就删除,并写入
        /// </summary>
        /// <returns>The set.</returns>
        /// <param name="key">Key.</param>
        /// <param name="acquire">Acquire.</param>
        /// <param name="expireMinutes">Expire minutes.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static bool Set<T>(string key, Func<T> acquire, int expireMinutes = 0)
        {
            var db = Manager.GetDatabase(dbNum);
            if (db.KeyExists(KeyFormatting(key)))
            {
                db.KeyDelete(KeyFormatting(key));
            }

            T t = acquire.Invoke();
            string value = t.ToJson();
            if (expireMinutes > 0)
            {
                return db.StringSet(KeyFormatting(key), value, TimeSpan.FromMinutes(expireMinutes));
            }
            else
            {
                return db.StringSet(KeyFormatting(key), value);
            }
        }

        /// <summary>
        /// 写入Redis 如果存在Key就删除,并写入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t">实体</param>
        /// <param name="expireMinutes"></param>
        /// <returns></returns>
        public static bool SetObj<T>(string key, T t, int expireMinutes = 0)
        {
            var db = Manager.GetDatabase(dbNum);
            if (db.KeyExists(KeyFormatting(key)))
            {
                db.KeyDelete(KeyFormatting(key));
            }

            //T t = acquire.Invoke();
            string value = t.ToJson();
            if (expireMinutes > 0)
            {
                return db.StringSet(KeyFormatting(key), value, TimeSpan.FromMinutes(expireMinutes));
            }
            else
            {
                return db.StringSet(KeyFormatting(key), value);
            }
        }

        /// <summary>
        /// 写入Redis 如果存在Key就删除,并写入
        /// </summary>
        /// <returns>The set.</returns>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <param name="expireMinutes">Expire minutes.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static bool Set<T>(string key, T value, int expireMinutes = 0)
        {
            var db = Manager.GetDatabase(dbNum);
            if (db.KeyExists(KeyFormatting(key)))
            {
                db.KeyDelete(KeyFormatting(key));
            }
            return db.StringSet(KeyFormatting(key), value.ToJson());
        }

        public static void SetList(string key, string[] values)
        {
            var db = Manager.GetDatabase(dbNum);
            for (int i = 0; i < values.Length; i++)
            {
                db.ListLeftPush(KeyFormatting(key), values[i]);
            }
        }
        public static void AddList(string key, string[] values)
        {
            var db = Manager.GetDatabase(dbNum);
            if (db.KeyExists(KeyFormatting(key)))
            {
                db.KeyDelete(KeyFormatting(key));
            }
            for (int i = 0; i < values.Length; i++)
            {
                db.ListLeftPush(KeyFormatting(key), values[i]);
            }
        }

        /// <summary>
        /// 添加 Value 到 Redis中
        /// </summary>
        /// <returns>The add.</returns>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <param name="expireMinutes">Expire minutes.</param>
        public static bool Add(string key, string value, int expireMinutes = 0)
        {
            var db = Manager.GetDatabase(dbNum);
            if (expireMinutes > 0)
            {
                db.StringSet(KeyFormatting(key), value, TimeSpan.FromMinutes(expireMinutes));
            }
            else
            {
                db.StringSet(KeyFormatting(key), value);
            }
            return db.StringSet(KeyFormatting(key), value);
        }
        public static void SetHash(string key, params CacheHash[] cacheHashArray)
        {
            var db = Manager.GetDatabase(dbNum);
            db.HashSet(KeyFormatting(key), Array.ConvertAll(cacheHashArray, item => new HashEntry(item.Key, item.Value)));
        }
        public static CacheHash[] GetHash(string key)
        {
            var db = Manager.GetDatabase(dbNum);

            var hashArray = db.HashGetAll(KeyFormatting(key));
            return Array.ConvertAll(hashArray, item => new CacheHash() { Key = item.Name, Value = item.Value });
        }
        #endregion

        /// <summary>
        /// 判断Key是否包含在Redis中
        /// </summary>
        /// <returns>The contains.</returns>
        /// <param name="key">Key.</param>
        public static bool Contains(string key)
        {
            var db = Manager.GetDatabase(dbNum);
            return db.KeyExists(KeyFormatting(key));
        }

        public static long ListLength(string key)
        {
            var db = Manager.GetDatabase(dbNum);
            return db.ListLength(KeyFormatting(key));
        }

        #region Remove
        /// <summary>
        /// 移除Redis中的Key
        /// </summary>
        /// <returns>The remove.</returns>
        /// <param name="key">Key.</param>
        public static bool Remove(string key)
        {
            var db = Manager.GetDatabase(dbNum);
            return db.KeyDelete(KeyFormatting(key));
        }
        public static void Remove(string[] keys)
        {
            var db = Manager.GetDatabase(dbNum);
            db.KeyDelete(Array.ConvertAll(keys, item => (RedisKey)KeyFormatting(item)));
        }
        public static void RemoveAll()
        {
            var server = Manager.GetServer(connectionString);
            var keys = server.Keys(dbNum, KeyFormatting("*")).ToArray();
            var db = Manager.GetDatabase(dbNum);
            db.KeyDelete(Array.ConvertAll(keys, item => (RedisKey)(item)));
        }
        public static void RemoveStartWithKey(string key)
        {
            string[] keys = GetKeysStartWith(key);
            Remove(keys);
        }
        public static void RemoveEndWithKey(string key)
        {
            string[] keys = GetKeysEndWith(key);
            Remove(keys);
        }
        public static void RemoveContaions(string key)
        {
            string[] keys = GetKeysContains(key);
            Remove(keys);
        }
        #endregion

        #region Keys
        public static string[] GetKeys()
        {
            var server = Manager.GetServer(connectionString);
            var keys = server.Keys(dbNum, KeyFormatting("*")).ToArray();
            return Array.ConvertAll(keys, k => KeyRemoveFormatting(k.ToString()));
        }
        public static string[] GetKeysStartWith(string key)
        {
            var server = Manager.GetServer(connectionString);
            var keys = server.Keys(dbNum, KeyFormatting($"{key}*")).ToArray();
            return Array.ConvertAll(keys, k => KeyRemoveFormatting(k.ToString()));
        }
        public static string[] GetKeysEndWith(string key)
        {
            var server = Manager.GetServer(connectionString);
            var keys = server.Keys(dbNum, KeyFormatting($"*{key}")).ToArray();
            return Array.ConvertAll(keys, k => KeyRemoveFormatting(k.ToString()));
        }
        public static string[] GetKeysContains(string key)
        {
            var server = Manager.GetServer(connectionString);
            var keys = server.Keys(dbNum, KeyFormatting($"*{key}*")).ToArray();
            return Array.ConvertAll(keys, k => KeyRemoveFormatting(k.ToString()));
        }
        #endregion

        #region Queue
        /// <summary>
        /// 出列队
        /// </summary>
        /// <returns>The dequeue.</returns>
        /// <param name="key">Key.</param>
        public static string Dequeue(string key)
        {
            var db = Manager.GetDatabase(dbNum);
            var value = db.ListRightPop(KeyFormatting(key), 0);
            if (value.IsNullOrEmpty)
            {
                return "";
            }
            else
            {
                return value;
            }
        }
        /// <summary>
        /// 出列队
        /// </summary>
        /// <returns>The dequeue.</returns>
        /// <param name="key">Key.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Dequeue<T>(string key)
        {
            var db = Manager.GetDatabase(dbNum);
            var value = db.ListRightPop(KeyFormatting(key), 0);
            if (value.IsNullOrEmpty)
            {
                return default(T);
            }
            else
            {
                return value.ToString().ToObject<T>();
            }
        }
        /// <summary>
        /// 入列队
        /// </summary>
        /// <returns>The enqueue.</returns>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static void Enqueue(string key, string value)
        {
            var db = Manager.GetDatabase(dbNum);
            db.ListLeftPush(KeyFormatting(key), value, 0, 0);
        }
        /// <summary>
        /// 入列队
        /// </summary>
        /// <returns>The enqueue.</returns>
        /// <param name="key">Key.</param>
        /// <param name="values">Values.</param>
        public static void Enqueue(string key, string[] values)
        {
            var db = Manager.GetDatabase(dbNum);
            for (int i = 0; i < values.Length; i++)
            {
                db.ListLeftPush(KeyFormatting(key), values[i], 0, 0);
            }
        }
        /// <summary>
        /// 入列队
        /// </summary>
        /// <returns>The enqueue.</returns>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Enqueue<T>(string key, T value)
        {
            var db = Manager.GetDatabase(dbNum);
            db.ListLeftPush(KeyFormatting(key), value.ToJson(), 0, 0);
        }
        #endregion
    }
}
