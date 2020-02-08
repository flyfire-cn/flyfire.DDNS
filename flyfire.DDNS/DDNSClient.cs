using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace flyfire.DDNS
{
    /// <summary>
    /// DDNS动态解析类
    /// 支持Oary、Dynu、NoIp
    /// https://www.cnblogs.com/flyfire-cn/
    /// </summary>
    public class DDNSClient
    {
        public string BaseAddress { get; set; }
        public string ApiUri { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        //[DefaultValue("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0")]
        public string Agent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="hostName">用户域名</param>
        /// <param name="userName">用户帐号</param>
        /// <param name="password">密码</param>
        /// <param name="baseAddress">服务器域名</param>
        /// <param name="apiUri">api接口地址</param>
        public DDNSClient(string hostName, string userName, string password, string baseAddress = "http://ddns.oray.com", string apiUri = "/ph/update")
        {
            BaseAddress = baseAddress;
            ApiUri = apiUri;
            UserName = userName;
            Password = password;
            HostName = hostName;
        }

        /// <summary>
        /// Get HttpClient
        /// </summary>
        /// <returns></returns>
        private HttpClient GetHttpClient()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            var services = serviceCollection.BuildServiceProvider();
            var clientFactory = services.GetService<IHttpClientFactory>();

            return clientFactory.CreateClient();
        }
        /// <summary>
        /// 获取公网IpV4地址
        /// 使用http://ddns.oray.com/checkip提供的服务
        /// </summary>
        /// <returns></returns>
        public virtual string GetInterNetIp()
        {
            var ipV4Str = "127.0.0.1";
            try
            {
                string Url = "http://ddns.oray.com";
                HttpClient client = GetHttpClient();
                client.BaseAddress = new Uri(Url);
                var response = client.GetAsync("/checkip");
                if (response.Result.IsSuccessStatusCode)
                {
                    var html = response.Result.Content.ReadAsStringAsync().Result;
                    //<html><head><title>Current IP Check</title></head><body>Current IP Address: 221.234.238.64</body></html>
                    if (html.Length == 0)
                    {
                        return ipV4Str;
                    }

                    var patten = @"\d+.\d+.\d+.\d+";
                    var ret = Regex.Match(html, patten, RegexOptions.IgnoreCase);
                    return ret.Value;
                }
                return ipV4Str;
            }
            catch (Exception)
            {
                return ipV4Str;
            }
        }
        /// <summary>
        /// 解析域名IpV4地址
        /// 失败时会返回0.0.0.0
        /// </summary>
        public string GetHostNameIp()
        {
            var ipV4Str = "0.0.0.0";
            try
            {
                IPHostEntry host = Dns.GetHostEntry(HostName);
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily.ToString() == "InterNetwork")
                    {
                        return ip.ToString();
                    }
                }
                return ipV4Str;
            }
            catch (Exception)
            {
                return ipV4Str;
            }
        }
        /// <summary>
        /// 检测域名与当前公网IP是否一致
        /// </summary>
        /// <returns>true:IpAddress发生变化，需要更新</returns>
        public bool IsIpAddressChanged()
        {
            var interNetIp = GetInterNetIp();
            var hostIp = GetHostNameIp();

            return !(interNetIp == hostIp);
        }
        /// <summary>
        /// 更新DDns
        /// </summary>
        /// <returns>更新结果</returns>
        public string UpdateDDns()
        {
            string ret = string.Empty;
            try
            {
                var client = GetHttpClient();
                client.BaseAddress = new Uri(BaseAddress);
                byte[] bytes = Encoding.Default.GetBytes($"{UserName}:{Password}");
                var base64 = Convert.ToBase64String(bytes);
                var authValue = $"Basic {base64}";
                client.DefaultRequestHeaders.Add("Authorization", authValue);
                client.DefaultRequestHeaders.Add("User-Agent", Agent);
                var response = client.GetAsync($"{ApiUri}?hostname={HostName}");
                if (response.Result.IsSuccessStatusCode)
                {
                    //ret = response.Result.ToString();
                    ret = response.Result.Content.ReadAsStringAsync().Result;//good 221.234.238.64
                }
                else
                {
                    ret = "service unavailable";
                }
                return ret;
            }
            catch (Exception ex)
            {
                ret = ex.Message;
                return ret;
            }
        }
    }
}
