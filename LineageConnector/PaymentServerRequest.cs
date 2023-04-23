using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LineageConnector
{
    public enum DownloadLink
    {
        ClientZipFileName = 0,
        Link1,
        Link2,
        ClientZipPassword
    }

    public class PaymentServerRequest
    {
        string SERVER_ADDRESS;
        int SERVER_PORT;
        string SERVER_URL = "http://";
        public const string UserAgent_Chrome = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36";


        public PaymentServerRequest(string server_address, int server_port)
        {
            this.SERVER_ADDRESS = server_address; this.SERVER_PORT = server_port;
            SERVER_URL = SERVER_URL + server_address + ":" + server_port + "/";
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
        }

        public string GetHomepage()
        {
            string URL = SERVER_URL + "gethomepagelink";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

            request.Method = "GET";
            request.Host = SERVER_ADDRESS;
            request.KeepAlive = true;
            request.AllowAutoRedirect = true;
            request.Accept = "*/*";
            request.ContentType = "*/*";
            request.Referer = SERVER_URL;
            request.Timeout = 10000; //10초안에 응답해라 !



            bool result = false;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "err";
            }

            string resultdata = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var stream = new StreamReader(response.GetResponseStream()))
                    resultdata = stream.ReadToEnd();

                //if (responseString.Contains("Success")) result = true;
                result = true;
            }

            if (result == true)
                return resultdata;

            return "err";
        }

        public string[] GetDownloadLink()
        {
            string URL = SERVER_URL + "getdownloadlink";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

            request.Method = "GET";
            request.Host = SERVER_ADDRESS;
            request.KeepAlive = true;
            request.AllowAutoRedirect = true;
            request.Accept = "*/*";
            request.ContentType = "*/*";
            request.Referer = SERVER_URL;
            request.Timeout = 10000; //10초안에 응답해라 !



            bool result = false;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            string resultdata = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var stream = new StreamReader(response.GetResponseStream()))
                    resultdata = stream.ReadToEnd();

                //if (responseString.Contains("Success")) result = true;
                result = true;
            }

            if (result == true)
            {
                return resultdata.Split('\n');
            }

            return null;
        }

        public string[] GetBadProcessList()
        {
            string URL = SERVER_URL + "getbadprocesslist";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

            request.Method = "GET";
            request.Host = SERVER_ADDRESS;
            request.KeepAlive = true;
            request.AllowAutoRedirect = true;
            request.Accept = "*/*";
            request.ContentType = "*/*";
            request.Referer = SERVER_URL;
            request.Timeout = 10000; //10초안에 응답해라 !



            bool result = false;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                return null;
            }

            string resultdata = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var stream = new StreamReader(response.GetResponseStream()))
                    resultdata = stream.ReadToEnd();

                //if (responseString.Contains("Success")) result = true;
                result = true;
            }

            if (result == true)
            {
                if (resultdata == null || resultdata.Length == 0) return null;
                return resultdata.Split(',');
            }

            return null;
        }

        public string GetAPIServerWalletAddress(string ccID = "0", bool useSSL = false)
        {
            string http = "http://";
            if (useSSL) http = "https://";
            string URL = http + string.Format("{0}:{1}", SERVER_ADDRESS, SERVER_PORT);
            URL = URL + string.Format("/getserverwalletaddress?ccID={0}", ccID);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

            request.Method = "GET";
            request.Host = SERVER_ADDRESS;
            request.KeepAlive = true;
            request.AllowAutoRedirect = true;
            request.Accept = "*/*";
            request.ContentType = "*/*";
            request.Referer = SERVER_URL;
            request.Timeout = 25000; //25초안에 응답해라 !



            bool result = false;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "err";
            }

            string resultdata = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var stream = new StreamReader(response.GetResponseStream()))
                    resultdata = stream.ReadToEnd();
                result = true;
            }

            if (result == true)
                return resultdata;

            return "err";
        }

        public string RequestPayment(string account, string charname, string price, string password, string billsender, bool useSSL = false)
        {
            string http = "http://";
            if (useSSL) http = "https://";
            string URL = http + string.Format("{0}:{1}", SERVER_ADDRESS, SERVER_PORT);
            URL = URL + string.Format("/requestpayment?account={0}&charactername={1}&ccID=0&amount={2}&paymentpassword={3}&billSender={4}", account, charname, price, password, billsender);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

            request.Method = "GET";
            request.Host = SERVER_ADDRESS;
            request.KeepAlive = true;
            request.AllowAutoRedirect = true;
            request.Accept = "*/*";
            request.ContentType = "*/*";
            request.Referer = SERVER_URL;
            request.Timeout = 25000; //25초안에 응답해라 !



            bool result = false;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "err";
            }

            string resultdata = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var stream = new StreamReader(response.GetResponseStream()))
                    resultdata = stream.ReadToEnd();
                result = true;
            }

            if (result == true)
                return resultdata;

            return "err";
        }

        public string StartPayment(string account, string charname, string price, string password, string txID, bool useSSL = false)
        {
            string http = "http://";
            if (useSSL) http = "https://";
            string URL = http + string.Format("{0}:{1}", SERVER_ADDRESS, SERVER_PORT);
            URL = URL + string.Format("/startpayment?account={0}&charactername={1}&ccID=0&amount={2}&paymentpassword={3}&txID={4}", account, charname, price, password, txID);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

            request.Method = "GET";
            request.Host = SERVER_ADDRESS;
            request.KeepAlive = true;
            request.AllowAutoRedirect = true;
            request.Accept = "*/*";
            request.ContentType = "*/*";
            request.Referer = SERVER_URL;
            request.Timeout = 25000; //25초안에 응답해라 !



            bool result = false;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "err";
            }

            string resultdata = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var stream = new StreamReader(response.GetResponseStream()))
                    resultdata = stream.ReadToEnd();
                result = true;
            }

            if (result == true)
                return resultdata;

            return "err";
        }
    }
}
