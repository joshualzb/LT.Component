using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace LT.Component.Web.Url
{
    /// <summary>
    /// Rewirter 通用类
    /// </summary>
    public class Rewirter : IHttpModule
    {
        public void Dispose()
        {
            
        }

        public void Init(HttpApplication context)
        {
            context.AuthorizeRequest += new EventHandler(context_AuthorizeRequest);
        }

        void context_AuthorizeRequest(object sender, EventArgs e)
        {
            RewriteUrl((HttpApplication)sender);
        }

        void RewriteUrl(HttpApplication context)
        {
            Uri uri = context.Request.Url;
            string absolutePath = uri.AbsolutePath;
            string urlPath = uri.PathAndQuery;

            string[] paths = Rules.GetPaths();
            int count = paths.Length;

            string path = null;
            Regex re = null;

            for (int i = 0; i < count; i++)
            {
                re = new Regex("^" + paths[i], RegexOptions.IgnoreCase);
                if (re.IsMatch(absolutePath))
                {
                    path = paths[i];
                    break;
                }
            }

            if (path != null)
            {
                string from = null;
                string test = null;
                List<RuleModel> rules = Rules.Get(path);

                foreach (RuleModel rule in rules)
                {
                    from = "^" + path + rule.From + "$";
                    re = new Regex(from, RegexOptions.IgnoreCase);
                    test = (rule.OP == "abs") ? absolutePath : urlPath;

                    if (re.IsMatch(test))
                    {
                        string sendToUrl = re.Replace(absolutePath, rule.To);
                        int queryPos = sendToUrl.IndexOf('?');

                        if (context.Request.QueryString.Count > 0)
                        {
                            if (queryPos != -1)
                            {
                                sendToUrl += "&" + context.Request.QueryString.ToString();
                            }
                            else
                            {
                                sendToUrl += "?" + context.Request.QueryString.ToString();
                            }
                        }

                        string queryString = String.Empty;
                        if (queryPos > 0)
                        {
                            queryString = sendToUrl.Substring(queryPos + 1);
                            sendToUrl = sendToUrl.Substring(0, queryPos);
                        }

                        context.Context.RewritePath(sendToUrl, String.Empty, queryString);
                        break;
                    }
                }
            }
        }
    }
}
