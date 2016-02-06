using E_Mig2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace E_Mig2.Data
{
    public class IEmig
    {
        static string sessionId = null;
        static string sqlId = null;
        public static async Task<List<Vonat>> vonatBetoltes()
        {
            List<Vonat> vlist = new List<Vonat>();

            HttpClient client = new HttpClient();
            HttpRequestMessage req;
            HttpResponseMessage resp;

            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(sqlId))
            {

                //SESSION ID
                req = new HttpRequestMessage(HttpMethod.Post, "http://iemig.mav-trakcio.hu/netr/emig.aspx");
                req.Headers.Date = DateTime.Now.Subtract(new TimeSpan(10, 0, 0));
                req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.1; WOW64; rv:11.0) Gecko/20100101 Firefox/11.0");
                resp = await client.SendAsync(req);
                if (resp.Content != null)
                {
                    Match m = new Regex(@"gSessionId=(.*?);").Match(await resp.Content.ReadAsStringAsync());
                    if (m.Success)
                    {
                        sessionId = m.Groups[1].ToString();
                    }
                }

                //SQL ID
                req = new HttpRequestMessage(HttpMethod.Post, "http://iemig.mav-trakcio.hu/netr/emig.aspx?u=public&s=" + sessionId + "&t=publicsandr&q=Q5&lt=SqlCreate&w=null&c=null&o=null");
                req.Headers.Date = DateTime.Now.Subtract(new TimeSpan(10, 0, 0));
                req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.1; WOW64; rv:11.0) Gecko/20100101 Firefox/11.0");
                req.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                req.Headers.AcceptEncoding.ParseAdd("gzip, deflate");
                req.Headers.AcceptLanguage.ParseAdd("hu,en-us;q=0.7,en;q=0.3");
                req.Headers.Referrer = new Uri("http://iemig.mav-trakcio.hu/5.0/index.html");
                resp = await client.SendAsync(req);
                if (resp.Content != null)
                {
                    Match m = new Regex("<sqlid>(.*?)</sqlid>").Match(await resp.Content.ReadAsStringAsync());
                    if (m.Success)
                    {
                        sqlId = m.Groups[1].ToString();
                    }
                }

            }

            //DATABASE DOWNLOAD
            req = new HttpRequestMessage(HttpMethod.Post, "http://iemig.mav-trakcio.hu/netr/emig.aspx?u=public&s=" + sessionId + "&t=publicrspec&q=" + sqlId + "&f=publicmlist");
            req.Headers.Date = DateTime.Now.Subtract(new TimeSpan(10, 0, 0));
            req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.1; WOW64; rv:11.0) Gecko/20100101 Firefox/11.0");
            req.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            req.Headers.AcceptEncoding.ParseAdd("gzip, deflate");
            req.Headers.AcceptLanguage.ParseAdd("hu,en-us;q=0.7,en;q=0.3");
            req.Headers.Referrer = new Uri("http://iemig.mav-trakcio.hu/5.0/index.html");
            resp = await client.SendAsync(req);
            MatchCollection match = new Regex("<Mozdony (.*?)></Mozdony>").Matches(await resp.Content.ReadAsStringAsync());
            int mcount = 1;
            foreach(Match m in match)
            {
                string s = m.Groups[1].ToString();
                vlist.Add(new Vonat()
                {
                    Index = mcount,
                    Vonatszam = new Regex("vonatszam=\"(.*?)\"").Match(s).Groups[1].ToString(),
                    UIC = new Regex("uic=\"(.*?)\"").Match(s).Groups[1].ToString()
                });
                mcount++;
            }

            client = null;
            req = null;
            resp = null;
            match = null;
            

            return vlist;
        }
    }
}
