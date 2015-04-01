using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ServiceStack.Redis;

namespace RedisLast
{
    /// <summary>
    /// RedisBase 基础类  待扩展
    /// </summary>
    public class RedisBase
    {
        static readonly RedisClient redis = (RedisClient)RedisManager.GetClient();// new RedisClient("localhost");
        #region --Item--
        public static bool Item_Set<T>(string key, T t)
        {
            try
            {
                return redis.Set<T>(key, t, new TimeSpan(1, 0, 0));
            }
            catch (Exception ex)
            {
                //loginfi
            }
            return false;
        }

        public static T Item_Get<T>(string key) where T : class
        {
            return redis.Get<T>(key);
        }

        public static bool Item_Remove(string key)
        {
            return redis.Remove(key);
        }
        #endregion

        #region --list--
        public static void List_Add<T>(string key, T t)
        {
            var redisTypedClient = redis.As<T>();
            redisTypedClient.AddItemToList(redisTypedClient.Lists[key], t);
        }

        public static bool List_Remove<T>(string key, T t)
        {
            var redisTypedClient = redis.As<T>();
            return redisTypedClient.RemoveItemFromList(redisTypedClient.Lists[key], t) > 0;
        }

        public static void List_RemoveAll<T>(string key)
        {
            var redisTypedClient = redis.As<T>();
            redisTypedClient.Lists[key].RemoveAll();
        }
        public static int List_Count(string key)
        {
            return redis.GetListCount(key);
        }
        public static List<T> List_GetRange<T>(string key, int start, int count)
        {
            var c = redis.As<T>();
            return c.Lists[key].GetRange(start, start + count - 1);
        }

        public static List<T> List_GetList<T>(string key)
        {
            var c = redis.GetTypedClient<T>();
            return c.Lists[key].GetRange(0, c.Lists[key].Count);
        }

        public static List<T> List_GetList<T>(string key, int pageIndex, int pageSize)
        {
            int start = pageSize * (pageIndex - 1);
            return List_GetRange<T>(key, start, pageSize);
        }
        #endregion

        #region -- Set --
        public static void Set_Add<T>(string key, T t)
        {
            var redisTypedClient = redis.GetTypedClient<T>();
            redisTypedClient.Sets[key].Add(t);
        }

        public static bool Set_Contains<T>(string key, T t)
        {

            var redisTypedClient = redis.GetTypedClient<T>();
            return redisTypedClient.Sets[key].Contains(t);

        }
        public static bool Set_Remove<T>(string key, T t)
        {
            var redisTypedClient = redis.GetTypedClient<T>();
            return redisTypedClient.Sets[key].Remove(t);
        }
        #endregion

        #region -- Hash --
        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool Hash_Exist<T>(string key, string dataKey)
        {
            return redis.HashContainsEntry(key, dataKey);
        }

        public static List<string> Hash_GetHashFields(string hashId)
        {
            return redis.GetHashKeys(hashId);
        }
        public static string Hash_GetHashFieldValues(string hashId, string field)
        {
            return redis.GetValueFromHash(hashId, field);
        }
        public static List<string> Hash_GetHashFieldValue(string hashId, params string[] fields)
        {
            return redis.GetValuesFromHash(hashId, fields);
        }
        public static List<string> Hash_GetHashValues(string hashId)
        {
            return redis.GetHashValues(hashId);
        }

        public static void SetHashField(string key, string field, string value)
        {
            redis.SetEntryInHash(key, field, value);
        }
        public static void SetHashIncr(string key, string field, int incre)
        {
            redis.IncrementValueInHash(key, field, incre);
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool Hash_Set<T>(string key, string dataKey, T t)
        {

            string value = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
            return redis.SetEntryInHash(key, dataKey, value);

        }
        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool Hash_Remove(string key, string dataKey)
        {

            return redis.RemoveEntryFromHash(key, dataKey);

        }
        /// <summary>
        /// 移除整个hash
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool Hash_Remove(string key)
        {

            return redis.Remove(key);

        }
        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static T Hash_Get<T>(string key, string dataKey)
        {

            string value = redis.GetValueFromHash(key, dataKey);
            return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(value);

        }
        /// <summary>
        /// 获取整个hash的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<T> Hash_GetAll<T>(string key)
        {

            var list = redis.GetHashValues(key);
            if (list != null && list.Count > 0)
            {
                List<T> result = new List<T>();
                foreach (var item in list)
                {
                    var value = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(item);
                    result.Add(value);
                }
                return result;
            }
            return null;

        }

        #endregion

        #region -- SortedSet --
        /// <summary>
        ///  添加数据到 SortedSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="score"></param>
        public static bool SortedSet_Add<T>(string key, T t, double score)
        {

            string value = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
            return redis.AddItemToSortedSet(key, value, score);

        }
        /// <summary>
        /// 移除数据从SortedSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool SortedSet_Remove<T>(string key, T t)
        {

            string value = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
            return redis.RemoveItemFromSortedSet(key, value);

        }
        /// <summary>
        /// 修剪SortedSet
        /// </summary>
        /// <param name="key"></param>
        /// <param name="size">保留的条数</param>
        /// <returns></returns>
        public static int SortedSet_Trim(string key, int size)
        {

            return redis.RemoveRangeFromSortedSet(key, size, 9999999);

        }
        /// <summary>
        /// 获取SortedSet的长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int SortedSet_Count(string key)
        {

            return redis.GetSortedSetCount(key);

        }

        /// <summary>
        /// 获取SortedSet的分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<T> SortedSet_GetList<T>(string key, int pageIndex, int pageSize)
        {

            var list = redis.GetRangeFromSortedSet(key, (pageIndex - 1) * pageSize, pageIndex * pageSize - 1);
            if (list != null && list.Count > 0)
            {
                List<T> result = new List<T>();
                foreach (var item in list)
                {
                    var data = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(item);
                    result.Add(data);
                }
                return result;
            }

            return null;
        }


        /// <summary>
        /// 获取SortedSet的全部数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<T> SortedSet_GetListALL<T>(string key)
        {

            var list = redis.GetRangeFromSortedSet(key, 0, 9999999);
            if (list != null && list.Count > 0)
            {
                List<T> result = new List<T>();
                foreach (var item in list)
                {
                    var data = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(item);
                    result.Add(data);
                }
                return result;
            }

            return null;
        }
        #endregion

        #region --common--
        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datetime"></param>
        public static void SetExpire(string key, DateTime datetime)
        {
            redis.ExpireEntryAt(key, datetime);
        }

        public static string GetValueString(string key)
        {
            return redis.GetValue(key);
        }
        public static byte[] GetValueByte(string key)
        {
            return redis.Get(key);
        }
        #endregion
    }
}