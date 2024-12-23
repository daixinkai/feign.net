using Feign.Formatting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace Feign.Internal
{
    internal static class FeignClientUtils
    {
        #region PathVariable

        public static bool ContainsPathVariable(string? uri, string? name)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                return false;
            }
            name = "{" + name + "}";
            return uri.Contains(name);
        }

        public static string ReplacePathVariable(string uri, string name, string? value, bool urlEncode)
        {
            if (urlEncode && !string.IsNullOrEmpty(value))
            {
                value = Uri.EscapeDataString(value);
            }
            name = "{" + name + "}";
            return uri.Replace(name, value);
        }

        public static string ReplacePathVariable<T>(ConverterCollection converters, string uri, string name, T value, bool urlEncode)
        {
            return ReplacePathVariable(uri, name, converters.ConvertStringValue(value, true), urlEncode);
        }
        #endregion

        #region RequestQuery
        public static string ReplaceRequestQuery(string uri, string name, string? value, bool urlEncode)
        {
            if (value == null)
            {
                return uri;
            }
            if (urlEncode && value.Length > 0)
            {
                value = Uri.EscapeDataString(value);
            }
#if NETCOREAPP3_0_OR_GREATER
            if (uri.Contains('?'))
#else
            if (uri.Contains("?"))
#endif
            {
                return uri + $"&{name}={value}";
            }
            else
            {
                return uri + $"?{name}={value}";
            }
        }
        public static string ReplaceRequestQuery<T>(string uri, string name, T value, IFeignOptions options)
        {
            var typeCode = Type.GetTypeCode(typeof(T));
            bool urlEncode = options.Request.UseUrlEncode;
            if (typeCode == TypeCode.Object)
            {
                foreach (var item in GetObjectStringParameters(name, value, options))
                {
                    uri = ReplaceRequestQuery(uri, item.Key, item.Value, urlEncode);
                }
                return uri;
            }
            else
            {
                return ReplaceRequestQuery(uri, name, options.Converters.ConvertStringValue(value, true), urlEncode);
            }
        }
        #endregion


        public static IEnumerable<KeyValuePair<string, string?>> GetObjectStringParameters<T>(string name, T value, IFeignOptions options)
        {
            if (value == null)
            {
                yield break;
            }
            //Nullable<>
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                yield return new KeyValuePair<string, string?>(name, options.Converters.ConvertStringValue(value, true));
                yield break;
            }

            //if (typeof(IDictionary).IsAssignableFrom(typeof(T)))
            //{
            //    IDictionary map = (IDictionary)value;
            //    foreach (var item in map.Keys)
            //    {
            //        var mapValue = map[item!];
            //        if (mapValue == null)
            //        {
            //            continue;
            //        }
            //        yield return new KeyValuePair<string, string?>(item!.ToString()!, converters.ConvertValue<string>(mapValue, true));
            //    }
            //    yield break;
            //}

            //if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            //{
            //    foreach (var item in (IEnumerable)value)
            //    {
            //        if (item == null)
            //        {
            //            continue;
            //        }
            //        yield return new KeyValuePair<string, string?>(name, converters.ConvertValue<string>(item, true));
            //    }
            //    yield break;
            //}

            // get properties

            foreach (var item in new ObjectQueryMap<T>(name, value, options.PropertyNamingPolicy, options.Converters).GetStringParameters(options.Request.IncludeRootParameterName))
            {
                yield return item;
            }

        }

        public static Encoding? GetEncoding(MediaTypeHeaderValue? mediaTypeHeaderValue)
        {
            string? charset = mediaTypeHeaderValue?.CharSet;

            if (charset == null)
            {
                return null;
            }

            // If we do have encoding information in the 'Content-Type' header, use that information to convert
            // the content to a string.

            if (charset.Equals("utf-8", StringComparison.OrdinalIgnoreCase) || charset.Equals("utf8", StringComparison.OrdinalIgnoreCase))
            {
                return Encoding.UTF8;
            }
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


        public static MultipartFormDataContent CreateMultipartFormDataContent(string? boundary, bool quotedBoundary)
        {
            if (string.IsNullOrWhiteSpace(boundary))
            {
                //boundary = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")));
                boundary = Guid.NewGuid().ToString("N");
            }
            MultipartFormDataContent multipartFormDataContent = new(boundary);
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
