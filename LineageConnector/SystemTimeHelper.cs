using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

/// <summary>
/// 시스템 시간 헬퍼
/// </summary>
public static class SystemTimeHelper
{
    //////////////////////////////////////////////////////////////////////////////////////////////////// Import
    ////////////////////////////////////////////////////////////////////////////////////////// Staitc
    //////////////////////////////////////////////////////////////////////////////// Public

    #region 시스템 시간 설정하기 - SetSystemTime(systemTime)

    /// <summary>
    /// 시스템 시간 설정하기
    /// </summary>
    /// <param name="systemTime">시스템 시간</param>
    /// <returns>처리 결과</returns>
    [DllImport("kernel32", SetLastError = true)]
    public static extern int SetSystemTime(ref SYSTEMTIME systemTime);

    #endregion

    #region 시스템 시간 구하기 - GetSystemTime(systemTime)

    /// <summary>
    /// 시스템 시간 구하기
    /// </summary>
    /// <param name="systemTime">시스템 시간</param>
    /// <returns>처리 결과</returns>
    [DllImport("kernel32", SetLastError = true)]
    public static extern int GetSystemTime(out SYSTEMTIME systemTime);

    #endregion

    //////////////////////////////////////////////////////////////////////////////////////////////////// Method
    ////////////////////////////////////////////////////////////////////////////////////////// Static
    //////////////////////////////////////////////////////////////////////////////// Public

    [DllImport("kernel32.dll")]
    public static extern bool SetLocalTime(ref SYSTEMTIME time);
    /// <summary>시스템 날짜/시간을 설정한다.</summary>
    /// <param name="dtNew">설정한 Date/Time</param>
    /// <returns>오류가 없는 경우 true를 응답하며,
    /// 그렇지 않은 경우 false를 응답한다.</returns>
    public static bool CheckPermission(DateTime dtNew)
    {
        bool bRtv = false;

        if (dtNew != DateTime.MinValue)
        {
            SYSTEMTIME st = new SYSTEMTIME();

            st.Year = (ushort)dtNew.Year;
            st.Month = (ushort)dtNew.Month;
            st.DayOfWeek = (ushort)dtNew.DayOfWeek;    // Set명령일 경우 이 값은 무시된다.
            st.Day = (ushort)dtNew.Day;
            st.Hour = (ushort)dtNew.Hour;
            st.Minute = (ushort)dtNew.Minute;
            st.Second = (ushort)dtNew.Second;
            bRtv = SetLocalTime(ref st); // 한국 시간대
        }
        return bRtv;
    }

    #region 시스템 시간 설정하기 - SetSystemTime(timeZone, sourceDateTime)

    /// <summary>
    /// 시스템 시간 설정하기
    /// </summary>
    /// <param name="timeZone">시간대</param>
    /// <param name="sourceDateTime">소스 일시</param>
    public static void SetSystemTime(int timeZone, DateTime sourceDateTime)
    {
        DateTime localDateTime;

        if (timeZone > 0)
        {
            localDateTime = sourceDateTime.AddHours(timeZone * -1);
        }
        else
        {
            localDateTime = sourceDateTime.AddHours(timeZone);
        }

        SYSTEMTIME systemTime = new SYSTEMTIME();

        systemTime.Year = Convert.ToUInt16(localDateTime.Year);
        systemTime.Month = Convert.ToUInt16(localDateTime.Month);
        systemTime.Day = Convert.ToUInt16(localDateTime.Day);
        systemTime.Hour = Convert.ToUInt16(localDateTime.Hour);
        systemTime.Minute = Convert.ToUInt16(localDateTime.Minute);
        systemTime.Second = Convert.ToUInt16(localDateTime.Second);

        SetSystemTime(ref systemTime);
    }

    #endregion
    #region 시스템 시간 설정하기 - SetSystemTime(sourceDateTime)

    /// <summary>
    /// 시스템 시간 설정하기
    /// </summary>
    /// <param name="sourceDateTime">설정 일시</param>
    public static void SetSystemTime(DateTime sourceDateTime)
    {
        SetSystemTime(9, sourceDateTime); // 한국 시간대
    }

    #endregion
    #region 시스템 시간 구하기 - GetSystemTime(timeZone)

    /// <summary>
    /// 시스템 시간 구하기
    /// </summary>
    /// <param name="timeZone">시간대</param>
    /// <returns>시스템 시간</returns>
    public static DateTime GetSystemTime(int timeZone)
    {
        SYSTEMTIME systemTime = new SYSTEMTIME();

        GetSystemTime(out systemTime);

        DateTime localDateTime = new DateTime
        (
            Convert.ToInt32(systemTime.Year),
            Convert.ToInt32(systemTime.Month),
            Convert.ToInt32(systemTime.Day),
            Convert.ToInt32(systemTime.Hour),
            Convert.ToInt32(systemTime.Minute),
            Convert.ToInt32(systemTime.Second),
            Convert.ToInt32(systemTime.Millisecond)
        );

        if (timeZone > 0)
        {
            return localDateTime.AddHours(timeZone);
        }
        else
        {
            return localDateTime.AddHours(timeZone * -1);
        }
    }

    #endregion
    #region 시스템 시간 구하기 - GetSystemTime()

    /// <summary>
    /// 시스템 시간 구하기
    /// </summary>
    /// <returns>시스템 시간</returns>
    public static DateTime GetSystemTime()
    {
        return GetSystemTime(9);
    }

    /// <summary>
    /// 시스템 시간을 time.nist.gov로부터 복구한다.
    /// </summary>
    public static void SetRemoteSystemTime()
    {
        Console.WriteLine(Process.GetCurrentProcess().Id);
        string responseText = null;

        try
        {
            using (var client = new TcpClient("time.nist.gov", 13))
            using (var streamReader = new StreamReader(client.GetStream()))
            {
                // 인터넷 시간 불러오기
                responseText = streamReader.ReadToEnd(); // "59442 21-08-16 14:28:19 50 0 0 585.3 UTC(NIST) *"
                var utcDateTimeString = responseText.Substring(7, 17);

                if (DateTime.TryParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime utcDateTime) == false)
                {
                    Console.WriteLine(responseText);
                }

                var localDateTime = utcDateTime.ToLocalTime();

                // PC에 적용하기
                SYSTEMTIME st = new SYSTEMTIME();
                st.Year = (ushort)localDateTime.Year;
                st.Month = (ushort)localDateTime.Month;
                st.Day = (ushort)localDateTime.Day;
                st.Hour = (ushort)localDateTime.Hour;
                st.Minute = (ushort)localDateTime.Minute;
                st.Second = (ushort)localDateTime.Second;
                st.Millisecond = (ushort)localDateTime.Millisecond;

                bool result = SetLocalTime(ref st);
                if (result == false)
                {
                    int lastError = Marshal.GetLastWin32Error();
                    Console.WriteLine(lastError);
                }

                Console.WriteLine($"Response: {responseText}, DateTime: {localDateTime}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message + "\n(" + responseText + ")");
        }
    }

    #endregion
}