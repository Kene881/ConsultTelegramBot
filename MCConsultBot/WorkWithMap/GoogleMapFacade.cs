using GoogleMaps.LocationServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MCConsultBot.WorkWithMap
{
    class GoogleMapFacade
    {
        /////ae3c1ca254321914669d54f2d2e53057
        /// <summary>
        /// http://api.positionstack.com/v1/forward?access_key=ae3c1ca254321914669d54f2d2e53057&query=Stavanger,Norway
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static float[] GetLatitudeAndLongtude(string address)
        {
            float[] LongLat = new float[2];

            var uri = $"http://api.positionstack.com/v1/forward?access_key=ae3c1ca254321914669d54f2d2e53057&query={address}";
            var httpClient = new HttpClient();
            var json = httpClient.GetStringAsync(uri).Result;

            var deserialize = JObject.Parse(json);


            LongLat[0] = (float)deserialize["data"][0]["latitude"];
            LongLat[1] = (float)deserialize["data"][0]["longitude"];

            return LongLat;
        }
    }
}
