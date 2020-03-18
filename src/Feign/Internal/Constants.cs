using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    static class Constants
    {
        public static class MediaTypes
        {

            public const string TEXT_XML = "text/xml";
            public const string APPLICATION_XML = "application/xml";

            public const string TEXT_JSON = "text/json";
            public const string APPLICATION_JSON = "application/json";
            public const string APPLICATION_ANY_JSON_SYNTAX = "application/*+json";

            public const string FORMDATA = "form-data";
            public const string APPLICATION_FORM_URLENCODED = "application/x-www-form-urlencoded";
            public const string APPLICATION_STREAM = "application/octet-stream";

            public const string MULTIPART_FORMDATA = "multipart/form-data";

        }

    }
}
