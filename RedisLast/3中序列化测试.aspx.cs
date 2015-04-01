using fastJSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedisLast
{
    public partial class _3中序列化测试 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var person = new Person
            {
                Id = 1,
                Name = "Vision Ding",
                Age = 28,
                Gender = true,
                Address = "广东省广州市天河区",
                IDCard = "42XXXX198XXXXXXXXX"
            };
            for (var i = 0; i < 10; i++)
            {
                Response.Write("第" + i + 1 + "次执行：");
                // Console.WriteLine("第{0}次执行：", i + 1);
                PerformanceTest(person, "Json.NET", 100000, p => JsonConvert.SerializeObject(p));
                PerformanceTest(person, "fastJSON", 100000, p => JSON.ToJSON(p));
                PerformanceTest(person, "ServiceStack", 100000, p => ServiceStack.Text.JsonSerializer.SerializeToString(p));
            }
        }

        /// <summary> 
        /// 循环执行序列化操作 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="entity">需要执行序列化操作的实体</param> 
        /// <param name="testName">库的名称</param> 
        /// <param name="count">循环测试</param> 
        /// <param name="func">序列化方法的委托</param> 
        private void PerformanceTest<T>(T entity, string testName, int count, Action<T> func) where T : class
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var i = 0; i < count; i++)
            {
                func.Invoke(entity);
            }
            stopwatch.Stop();
            //Console.WriteLine("{0}: {1}", testName, stopwatch.ElapsedMilliseconds); 
            Response.Write("" + testName + ":" + stopwatch.ElapsedMilliseconds);
        }
    }

    [Serializable]
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public bool Gender { get; set; }
        public string Address { get; set; }
        public string IDCard { get; set; }
    }
}



//第01次执行：Json.NET:728fastJSON:1141ServiceStack:310
//第11次执行：Json.NET:226fastJSON:386ServiceStack:281
//第21次执行：Json.NET:224fastJSON:390ServiceStack:288
//第31次执行：Json.NET:233fastJSON:402ServiceStack:315
//第41次执行：Json.NET:278fastJSON:388ServiceStack:383
//第51次执行：Json.NET:228fastJSON:423ServiceStack:313
//第61次执行：Json.NET:254fastJSON:411ServiceStack:362
//第71次执行：Json.NET:234fastJSON:423ServiceStack:320
//第81次执行：Json.NET:230fastJSON:403ServiceStack:302
//第91次执行：Json.NET:250fastJSON:524ServiceStack:306


#region API ServiceStack.Text
//string JsonSerializer.SerializeToString<T>(T value);
//void JsonSerializer.SerializeToWriter<T>(T value, TextWriter writer);
//void JsonSerializer.SerializeToStream<T>(T value, Stream stream);
//T JsonSerializer.DeserializeFromString<T>(string value);
//T JsonSerializer.DeserializeFromReader<T>(TextReader reader);
//T JsonSerializer.DeserializeFromStream<T>(Stream stream);
#endregion