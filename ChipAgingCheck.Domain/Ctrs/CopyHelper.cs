using System;
using Newtonsoft.Json;

namespace ChipAgingCheck.Domain.Ctrs
{
    /// <summary>
    /// 深拷贝和 JSON 互转。用 Newtonsoft.Json，.NET Framework 4.7.2 没有 System.Text.Json。
    /// </summary>
    public static class CopyHelper
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
        };

        /// <summary>序列化再反序列化，拿到一份完全独立的副本。</summary>
        public static T DeepCopy<T>(this T obj) where T : class
        {
            string json = JsonConvert.SerializeObject(obj, Settings);
            T copy = JsonConvert.DeserializeObject<T>(json, Settings);

            if (copy == null)
            {
                throw new InvalidOperationException("DeepCopy 失败");
            }

            return copy;
        }

        public static string ToJson<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj, Settings);
        }

        /// <summary>解析失败返回 null，调用方自己决定要不要兜默认值。</summary>
        public static T FromJson<T>(this string json) where T : class
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json, Settings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("FromJson 失败：" + ex.Message);
                return null;
            }
        }
    }
}