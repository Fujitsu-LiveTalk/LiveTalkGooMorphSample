/*
 * Copyright 2019 FUJITSU SOCIAL SCIENCE LABORATORY LIMITED
 * クラス名　：GooMorphModel
 * 概要      ：gooラボ形態素解析APIと連携
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LiveTalkGooMorphSample.Models
{
    internal class GooMorphModel
    {
        private string APIKey = "<<<API KEY>>>";
        private string Url = "https://labs.goo.ne.jp/api/morph";
        private string ProxyServer = "";    // PROXY経由なら proxy.hogehoge.jp:8080 のように指定
        private string ProxyId = "";        // 認証PROXYならIDを指定
        private string ProxyPassword = "";  // 認証PROXYならパスワードを指定

        public async Task<(List<string>, string)> TextToSpeechAsync(string text)
        {
            try
            {
                // パラメタ設定
                var param = new TReqest() {
                    app_id = APIKey,
                    sentence = text,
                    info_filter = "form"
                };

                // プロキシ設定
                var ch = new HttpClientHandler() { UseCookies = true };
                if (!string.IsNullOrEmpty(this.ProxyServer))
                {
                    var proxy = new System.Net.WebProxy(this.ProxyServer);
                    if (!string.IsNullOrEmpty(this.ProxyId) && !string.IsNullOrEmpty(this.ProxyPassword))
                    {
                        proxy.Credentials = new System.Net.NetworkCredential(this.ProxyId, this.ProxyPassword);
                    }
                    ch.Proxy = proxy;
                }
                else
                {
                    ch.Proxy = null;
                }

                // Web API呼び出し
                using (var client = new HttpClient(ch))
                {

                    using (var request = new HttpRequestMessage())
                    {
                        request.Method = HttpMethod.Post;
                        request.RequestUri = new Uri(this.Url);
                        var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(TReqest));
                        using (var ms = new MemoryStream())
                        {
                            serializer.WriteObject(ms, param);
                            var bytes = ms.ToArray();
                            var jsonString = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                        }
                        var response = await client.SendAsync(request);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            using (var json = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonString)))
                            {
                                var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(TResponse));
                                {
                                    var result = ser.ReadObject(json) as TResponse;
                                    var morph = new List<string>();
                                    for (var index = 0; index < result.word_list[0].Length; index++)
                                    {
                                        morph.Add(result.word_list[0][index][0]);
                                    }
                                    return (morph, string.Empty);
                                }
                            }
                        }
                        else
                        {
                            return (null, "Request Error:" + response.StatusCode.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (null, "Request Error:" + ex.Message);
            }
        }


        [DataContract]
        public class TReqest
        {
            [DataMember]
            public string app_id { get; set; }
            [DataMember]
            public string sentence { get; set; }
            [DataMember]
            public string info_filter { get; set; }
        }


        [DataContract]
        public class TResponse
        {
            [DataMember]
            public string info_filter { get; set; }
            [DataMember]
            public string request_id { get; set; }
            [DataMember]
            public string[][][] word_list { get; set; }
        }
    }
}
