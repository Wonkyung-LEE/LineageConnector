using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
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
    /// 시스템 시간을 NST Server로부터 복구한다.
    /// </summary>
    public static void SetRemoteSystemTime()
    {
        // 인터넷 시간 서버에서 현재 UTC 시간을 가져온다.
        string ntpServer = "pool.ntp.org";
        IPAddress[] addresses = Dns.GetHostEntry(ntpServer).AddressList;
        IPAddress ipAddress = addresses[0];
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 123);
        byte[] ntpData = new byte[48];
        ntpData[0] = 0x1B;
        var socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        socket.Connect(ipEndPoint);
        socket.Send(ntpData);
        socket.Receive(ntpData);
        socket.Close();

        // NTP 패킷에서 시간을 가져온다.
        ulong intpart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
        ulong fractpart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];
        ulong milliseconds = (intpart * 1000) + ((fractpart * 1000) / 0x100000000L);

        // 1970년 1월 1일 0시 0분 0초 UTC 기준으로부터 현재 UTC 시간까지의 경과 시간을 계산한다.
        DateTime epoch = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime networkDateTime = epoch.AddMilliseconds((long)milliseconds);

        // 시스템 시간.
        DateTime systemDateTime = DateTime.Now;

        // 시스템 시간과 인터넷 시간의 차이를 계산한다.
        TimeSpan timeDiff = networkDateTime - systemDateTime;

        // 시스템 시간을 인터넷 시간과 동기화.
        SYSTEMTIME st = new SYSTEMTIME();
        st.Year = (ushort)networkDateTime.Year;
        st.Month = (ushort)networkDateTime.Month;
        st.Day = (ushort)networkDateTime.Day;
        st.Hour = (ushort)networkDateTime.Hour;
        st.Minute = (ushort)networkDateTime.Minute;
        st.Second = (ushort)networkDateTime.Second;
        SetSystemTime(ref st);

        Console.WriteLine("시스템 시간을 인터넷 시간과 동기화하였습니다.");
    }

    #endregion
}