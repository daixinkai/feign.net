using Feign.Formatting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Feign.Internal
{
    static class FeignClientUtils
    {
        #region PathVariable
        public static string ReplacePathVariable(string uri, string name, string value, bool urlEncode)
        {
            if (urlEncode && !string.IsNullOrEmpty(value))
            {
                value = System.Web.HttpUtility.UrlEncode(value);
            }
            name = "{" + name + "}";
            return uri.Replace(name, value);
        }

        public static string ReplacePathVariable<T>(ConverterCollection converters, string uri, string name, T value, bool urlEncode)
        {
            return ReplacePathVariable(uri, name, converters.ConvertValue<T, string>(value, true), urlEncode);
        }
        #endregion

        #region RequestQuery
        public static string ReplaceRequestQuery(string uri, string name, string value, bool urlEncode)
        {
            if (value == null)
            {
                return uri;
            }
            if (urlEncode && value.Length > 0)
            {
                value = System.Web.HttpUtility.UrlEncode(value);
            }
            if (uri.IndexOf("?") >= 0)
            {
                return uri + $"&{name}={value}";
            }
            else
            {
                return uri + $"?{name}={value}";
            }
        }
        public static string ReplaceRequestQuery<T>(ConverterCollection converters, NamingPolicy namingPolicy, string uri, string name, T value, bool urlEncode)
        {
            var typeCode = Type.GetTypeCode(typeof(T));
            if (typeCode == TypeCode.Object)
            {
                foreach (var item in GetObjectStringParameters(name, value, converters, namingPolicy))
                {
                    uri = ReplaceRequestQuery(uri, item.Key, item.Value, urlEncode);
                }
                return uri;
            }
            else
            {
                return ReplaceRequestQuery(uri, name, converters.ConvertValue<T, string>(value, true), urlEncode);
            }
        }
        #endregion


        public static IEnumerable<KeyValuePair<string, string>> GetObjectStringParameters<T>(string name, T value, ConverterCollection converters, NamingPolicy namingPolicy)
        {
            if (value == null)
            {
                yield break;
            }
            //Nullable<>
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                yield return new KeyValuePair<string, string>(name, converters.ConvertValue<T, string>(value, true));
                yield break;
            }

            if (typeof(IDictionary).IsAssignableFrom(typeof(T)))
            {
                IDictionary map = ((IDictionary)value);
                foreach (var item in map.Keys)
                {
                    if (map[item] == null)
                    {
                        continue;
                    }
                    yield return new KeyValuePair<string, string>(item.ToString(), converters.ConvertValue<string>(item, true));
                }
                yield break;
            }

            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                foreach (var item in value as IEnumerable)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    yield return new KeyValuePair<string, string>(name, converters.ConvertValue<string>(item, true));
                }
                yield break;
            }

            //TODO: ReplaceRequestQuery
            //foreach (var property in value.GetType().GetProperties())
            //{
            //    object propertyValue = property.GetValue(value);
            //    if (propertyValue == null)
            //    {
            //        continue;
            //    }
            //    if (propertyValue is IEnumerable&&propertyValue)
            //    {

            //    }
            //}

            // get properties

            foreach (var property in typeof(T).GetProperties())
            {
                if (property.GetMethod == null)
                {
                    continue;
                }
                object propertyValue = property.GetValue(value);
                if (propertyValue == null)
                {
                    continue;
                }
                if (propertyValue is string)
                {
                    yield return new KeyValuePair<string, string>(GetName(property, namingPolicy), propertyValue.ToString());
                    continue;
                }
                if (propertyValue is IEnumerable)
                {
                    foreach (var item in propertyValue as IEnumerable)
                    {
                        if (item == null)
                        {
                            continue;
                        }
                        yield return new KeyValuePair<string, string>(GetName(property, namingPolicy), converters.ConvertValue<string>(item, true));
                    }
                    continue;
                }
                yield return new KeyValuePair<string, string>(GetName(property, namingPolicy), converters.ConvertValue<string>(propertyValue, true));
            }
        }



        public static Encoding GetEncoding(MediaTypeHeaderValue mediaTypeHeaderValue)
        {
            string charset = mediaTypeHeaderValue?.CharSet;

            // If we do have encoding information in the 'Content-Type' header, use that information to convert
            // the content to a string.
            if (charset != null)
            {
                try
                {
                    // Remove at most a single set of quotes.
                    if (charset.Length > 2 &&
                        charset[0] == '\"' &&
                        charset[charset.Length - 1] == '\"')
                    {
                        return Encoding.GetEncoding(charset.Substring(1, charset.Length - 2));
                    }
                    else
                    {
                        return Encoding.GetEncoding(charset);
                    }
                }
                catch (ArgumentException e)
                {
                    throw new InvalidOperationException("The character set provided in ContentType is invalid. Cannot read content as string using an invalid character set.", e);
                }
            }
            return null;
        }


        public static string GetName(PropertyInfo property, NamingPolicy namingPolicy)
        {
            return namingPolicy.ConvertName(property.Name);
        }

        public static MultipartFormDataContent CreateMultipartFormDataContent(string boundary, bool quotedBoundary)
        {
            if (string.IsNullOrWhiteSpace(boundary))
            {
                //boundary = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")));
                boundary = Guid.NewGuid().ToString("N");
            }
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent(boundary);
            if (!quotedBoundary)
            {
                //multipartFormDataContent.Headers.ContentType = MediaTypeHeaderValue.Parse($"multipart/form-data; boundary={boundary}");
                multipartFormDataContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data")
                {
                    Parameters =
                    {
                        new NameValueHeaderValue("boundary", boundary)
                    }
                };
            }
            return multipartFormDataContent;
        }

    }
}
