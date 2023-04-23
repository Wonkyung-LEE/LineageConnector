using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.ComponentModel;

namespace LineageConnector
{
    public class Downloader
    {
        public delegate void _ProgressChanged(long Total, long Current, string[] FileNames);

        const string MediafireHost = "mediafire.com";
        const string GoogleDriveHost = "drive.google.com";
        System.Windows.Forms.ProgressBar progressBar = null;

        private _ProgressChanged progressChanged = null;

        WebClient wc = new WebClient();
        public Downloader(System.Windows.Forms.ProgressBar bar, _ProgressChanged pc)
        {
            progressBar = bar;
            progressBar.Maximum = 100;
            wc.DownloadFileCompleted += DownloadFileCompletedCallback;
            wc.DownloadProgressChanged += DownloadProgressChanged;
            progressChanged = pc;
        }

        private string GetMediaFireDirectLink(string Link)
        {
            string URL = Link;

            string ParseKey1 = "href=\"https://";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            //request.Host = Host;
            request.KeepAlive = true;
            request.AllowAutoRedirect = true;
            //request.Headers.Add("accept-encoding", "gzip");
            request.Headers.Add("accept-language", "ko-KR,ko;q=0.9,en-US;q=0.8,en;q=0.7");
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            //request.ContentType = "*/*";
            request.UserAgent = PaymentServerRequest.UserAgent_Chrome;
            //request.Referer = "https://" + Host;
            request.Timeout = 10000; //10초안에 응답해라 !

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
                if (resultdata.Length < 20) return null;
                int ParseStartIndex = resultdata.IndexOf(ParseKey1) + ParseKey1.Length; //   href="https://  이거를 찾는것
                if (ParseStartIndex < ParseKey1.Length + 2) return null;

                int tempIndex = 0;
                for (int i = 0; i < ParseKey1.Length * 10; i++) //대충 수십번 이상 실행하라 이말
                {
                    if (resultdata.Length - ParseStartIndex < 400) return null; //실패 케이스
                    string TempResultData = resultdata.Substring(ParseStartIndex, 399);
                    tempIndex = TempResultData.IndexOf("GB)");
                    if (tempIndex == -1) tempIndex = resultdata.IndexOf("MB)"); //MB나 GB 수준의 인덱스까지는 찾아본다
                    if (tempIndex < 3) { ParseStartIndex = resultdata.IndexOf(ParseKey1, ParseStartIndex + 1) + ParseKey1.Length; continue; }

                    int doublequoteindex = TempResultData.IndexOf('\"', 1);

                    if (doublequoteindex - tempIndex < 380)
                    {
                        string DirectLink = "https://" + TempResultData.Substring(0, doublequoteindex).Replace("\"", "");
                        if (DirectLink.Contains('.') && DirectLink.Contains('/'))
                            return DirectLink;
                        return null;
                    }
                }
            }
            return null;
        }

        //구글 드라이브 개별 파일 다운로드는 99MB의 파일까지만 지원한다. 즉 분할압축해라
        private string GetGoolgDriveDirectLink(string Link) //너무 귀찮아서 하드코딩 향연. by. cm01 2022-01-08
        {
            string URL = Link;
            if (Link.Contains("/view"))
                Link = Link.Replace("?usp=sharing", "").Replace("?usp", "");

            int HostIndex = 0;
            if (Link.Contains(".google.com"))
                HostIndex = Link.IndexOf(".google.com") + 11; //.google.com의 길이
            string Host = Link.Substring(0, HostIndex).Replace("https://", "").Replace("/", "");

            string FileID = "";
            int MaxLength = 0;
            string[] Temp = Link.Replace("https://", "").Replace("google.com", "").Replace("file/d", "").Split('/'); //Replace("/view","") 이거는 날리지말자
            for (int i = 0; i < Temp.Length; i++)
                if (Temp[i] != null && Temp[i].Length > MaxLength) { MaxLength = Temp[i].Length; FileID = Temp[i]; }
            string ParseKey1 = "/" + FileID;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            //request.Host = Host;
            request.KeepAlive = true;
            request.AllowAutoRedirect = true;
            //request.Headers.Add("accept-encoding", "gzip");
            request.Headers.Add("accept-language", "ko-KR,ko;q=0.9,en-US;q=0.8,en;q=0.7");
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            //request.ContentType = "*/*";
            request.UserAgent = PaymentServerRequest.UserAgent_Chrome;
            //request.Referer = "https://" + Host;
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
            string URL2 = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var stream = new StreamReader(response.GetResponseStream()))
                    resultdata = stream.ReadToEnd();
                if (resultdata.Length < 20) return null;
                int ParseStartIndex = resultdata.IndexOf(ParseKey1) + ParseKey1.Length;
                if (ParseStartIndex < ParseKey1.Length + 2) return null;

                int tempIndex = 0;
                for (int i = 0; i < ParseKey1.Length; i++) //대충 수십번 이상 실행하라 이말
                {
                    tempIndex = resultdata.IndexOf("https://", ParseStartIndex + 1);
                    if (tempIndex - ParseStartIndex > 15) //대충 6~8글자 정도 차이나는듯 -> 찾으려는 문자열이 매우 가까이에 붙어있음
                    {
                        ParseStartIndex = resultdata.IndexOf(ParseKey1, tempIndex + 1) + ParseKey1.Length;
                        continue;
                    }
                    else if (tempIndex - ParseStartIndex < 1) return null;

                    int doublequoteindex = resultdata.IndexOf('\"', tempIndex + 5);
                    //여기까지 왔으면 거의 찾은것 같다.

                    URL2 = System.Text.RegularExpressions.Regex.Unescape(resultdata.Substring(tempIndex, doublequoteindex - tempIndex));
                    if (!URL2.Contains(ParseKey1.Replace("/", ""))) //마지막 확인 한번 더
                    {
                        ParseStartIndex = resultdata.IndexOf(ParseKey1, tempIndex + 1) + ParseKey1.Length;
                        continue;
                    }
                    resultdata = "";
                    result = false;
                    break;
                }
            }

            //return GetGoolgDriveDirectLink2(URL2, FileName);


            return URL2;
        }





        string[] DirectLinks = null;
        string[] DownloadFileNames = null;

        public bool DownloadClient(string Link, string SavePath)
        {
            if (progressBar != null) progressBar.Value = 0;
            if (Link.Contains(',')) //구글드라이브 분할 링크 확인 용도이며, 다른 곳의 링크는 이렇게 사용할 수 없다.           ※ 구글 다운로드는 테스트 안해봤음. 소스 수정 필요할 수 있음. by. cm01    2022-01-10
            {
                string[] Links = Link.Split(',');
                if (Links.Length < 2) Link.Trim(','); //이건 서버에서 오타냈을 수도 있으므로 그냥 ',' 없애주고 진행
                else //구글에 99MB로 zip를 분할해서 올려놨을 경우 전부 다운로드 하는 용도
                {
                    DirectLinks = new string[Links.Length];
                    DownloadFileNames = new string[Links.Length];
                    string FileName = Path.GetFileName(SavePath);
                    string FileNameWithoutExt = Path.GetFileNameWithoutExtension(FileName);
                    string Ext = Path.GetExtension(FileName);
                    if (Links[0].Contains("drive.google.com"))
                    {
                        DirectLinks[0] = GetGoolgDriveDirectLink(Links[0]);
                        DownloadFileNames[0] = FileName;
                    }

                    for (int i = 1; i < Links.Length; i++)
                    {
                        string TempLink = Links[i];
                        if (!TempLink.Contains("drive.google.com")) continue; //구글에 99MB로 zip를 분할해서 올려놨을 경우 (zip 파일 아니면 고장남)
                        DirectLinks[i] = GetGoolgDriveDirectLink(TempLink);
                        DownloadFileNames[i] = FileNameWithoutExt + ".z" + i.ToString("D2"); //z01, z02, z03 ...
                    }
                }
            }

            if (Link.Contains(GoogleDriveHost)) return false; //콤마도 없는데 구글 드라이브 링크만 달랑 올라왔다? 이건 대용량 파일이다. 따라서 다이렉트 다운로드 못하고 직접 브라우저로 띄워줘야함
            if (Link.Contains(MediafireHost)) Link = GetMediaFireDirectLink(Link); //미디어 파이어도 다이렉트 주소가 매번 바뀌기 때문에 이렇게 동적으로 찾아준다     ※테스트 완료, 정상작동 확인   by. cm01 2022-01-10


            if (DirectLinks == null)
                DirectLinks = new string[1] { Link };
            if (DownloadFileNames == null)
                DownloadFileNames = new string[1] { SavePath };

            if (DirectLinks != null && DirectLinks.Length > 0 && DirectLinks[0] != null) //다이렉트 링크를 잘 구해왔다면 다운로드 시작
            {
                for (int i = 0; i < DirectLinks.Length; i++)
                    if (DirectLinks[i].Length > 8)
                        wc.DownloadFileAsync(new Uri(DirectLinks[i]), Path.GetFileName(SavePath), (object)DownloadFileNames[i]);
                return true;
            }
            //다이렉트 링크를 못찾은 케이스. 유저가 직접 다운로드해야함
            else return false; //(구글 99MB 분할압축과 미디어파이어) 외에는 전부 링크 직접 띄워서 유저가 직접 다운로드하라고 해라
        }


        int itr = 0;
        long _CTBTR = 0;
        long _CBR = 0;
        public long CurrentTotalBytesToReceive { get { return _CTBTR; } }
        public long CurrentBytesReceived { get { return _CBR; } }
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            itr++;
            if (itr % 325 == 0)
            {
                _CTBTR = downloadProgressChangedEventArgs.TotalBytesToReceive;
                _CBR = downloadProgressChangedEventArgs.BytesReceived;
                double percentage = ((double)downloadProgressChangedEventArgs.BytesReceived / (double)_CTBTR) * 100.0;
                progressBar.Value = (int)percentage;
                if (progressChanged != null) progressChanged(_CTBTR, _CBR, null);
                return;
            }
            if (itr < 3)
            {
                _CTBTR = downloadProgressChangedEventArgs.TotalBytesToReceive;
                progressBar.Value = (int)(((double)downloadProgressChangedEventArgs.BytesReceived / (double)_CTBTR) * 100.0);
                if (progressChanged != null) progressChanged(_CTBTR, _CBR, null);
            }

        }

        private void DownloadFileCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            string FileName = (string)e.UserState;
            if (File.Exists(FileName))
                progressChanged(100, 100, DownloadFileNames);
        }

    }
}
