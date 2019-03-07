using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace LT.Component.Utility
{
    /// <summary>
    /// Spider 通用类
    /// </summary>
    public class Spider
    {
        /// <summary>
        /// 创建访问URL的Response
        /// </summary>
        private HttpWebResponse getWebResponse(string uri, Cookie cookie)
        {
            HttpWebRequest request;
            HttpWebResponse response = null;

            try
            {
                //请求
                request = WebRequest.Create(uri) as HttpWebRequest;
                CookieContainer myCookieContainer = new CookieContainer();

                if (request != null)
                {
                    request.KeepAlive = true;
                    request.AllowAutoRedirect = true;
                    request.AllowWriteStreamBuffering = true;
                    request.Referer = "http://www.lanxe.net/";
                    request.Timeout = 20000;
                    request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0; .NET CLR 1.1.4322; .NET CLR 2.0.50215; fqSpider)";
                    request.CookieContainer = myCookieContainer;

                    //加入Cookie
                    if (cookie != null)
                    {
                        myCookieContainer.Add(cookie);
                    }

                    //回应
                    response = request.GetResponse() as HttpWebResponse;
                    if (response != null) response.Cookies = myCookieContainer.GetCookies(request.RequestUri);
                }
                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    return response;
                }
            }
            catch
            {

            }

            //返回
            return null;
        }

        public WebImageModel GetImage(string uri, Cookie cookie)
        {
            /*
             * 在这里需要实现多线程获取图片
             * 暂不处理
            */

            HttpWebResponse response = getWebResponse(uri, cookie);
            if (response == null)
            {
                return null;
            }

            try
            {
                //声明容器
                WebImageModel model = new WebImageModel();

                //判断是否为图片流类型
                string contentType = response.Headers["Content-Type"].ToLower();
                if (Regexs.ContentTypeImageRegex.IsMatch(contentType))
                {
                    Match m = Regexs.ContentTypeImageRegex.Match(contentType);

                    //获取图片后缀名
                    model.Extention = m.Groups[1].Value;

                    // 得到图片资料
                    long length = response.ContentLength;
                    Stream stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        Image image = Image.FromStream(stream, true);

                        model.Size = length;
                        model.Stream = image;
                    }
                }

                return model;
            }
            catch
            {

            }

            return null;
        }

        /// <summary>
        /// 获取网页源代码
        /// </summary>
        public string GetFullHtml(string uri, string encodeName, Cookie cookie)
        {
            HttpWebResponse response = getWebResponse(uri, cookie);
            if (response == null)
            {
                return null;
            }

            try
            {
                //HTML容器
                StringBuilder outs = new StringBuilder();

                //判断文档是否为Text形式
                string contentType = response.Headers["Content-Type"].ToLower();
                if (Regexs.ContentTypeTextRegex.IsMatch(contentType))
                {
                    //网页编码
                    Encoding encode;
                    if (encodeName.ToLower() == "default")
                    {
                        encode = Encoding.Default;
                    }
                    else
                    {
                        encode = Encoding.GetEncoding(encodeName);
                    }

                    Stream receiveStream = response.GetResponseStream();
                    if (receiveStream != null)
                    {
                        StreamReader reader = new StreamReader(receiveStream, encode);

                        // 每次读取1024
                        char[] read = new char[1024];
                        int count = reader.Read(read, 0, 1024);
                        while (count > 0)
                        {
                            outs.Append(new String(read, 0, count));
                            count = reader.Read(read, 0, 1024);
                        }

                        // 读取完毕
                        reader.Close();
                    }
                    if (receiveStream != null) receiveStream.Close();
                }

                return outs.ToString();
            }
            catch (Exception)
            {

            }

            return null;
        }

        public string MakeFullUrl(string webPath, string url)
        {
            Match m = Regexs.UrlReg.Match(webPath);

            string server = m.Groups["SERVER"].Value;
            string path = "/" + m.Groups["PATH"].Value.Trim('/');
            string port = m.Groups["PORT"].Value;
            string protocol = m.Groups["PROTOCOL"].Value;

            if (url.ToLower().IndexOf("http://", System.StringComparison.Ordinal) != 0 && url.Length != 0)
            {
                if (url.Substring(0, 1) == "/")
                {
                    if (port.Length == 0 || port == "80")
                    {
                        url = string.Format("{0}://{1}{2}", protocol, server, url);
                    }
                    else
                    {
                        url = string.Format("{0}://{1}:{2}{3}", protocol, server, port, url);
                    }
                }
                else
                {
                    if (port.Length == 0 || port == "80")
                    {
                        url = string.Format("{0}://{1}{2}/{3}", protocol, server, path, url);
                    }
                    else
                    {
                        url = string.Format("{0}://{1}:{2}{3}/{4}", protocol, server, port, path, url);
                    }
                }
            }

            return url;
        }
    }

    public class WebImageModel
    {
        private string _extention;
        private long _size;
        private System.Drawing.Image _stream;

        /// <summary>
        /// 图片后缀名
        /// </summary>
        public string Extention
        {
            set { _extention = value; }
            get { return _extention; }
        }

        /// <summary>
        /// 图片大小
        /// </summary>
        public long Size
        {
            set { _size = value; }
            get { return _size; }
        }

        /// <summary>
        /// 图片文件数据流
        /// </summary>
        public System.Drawing.Image Stream
        {
            set { _stream = value; }
            get { return _stream; }
        }
    }
}
