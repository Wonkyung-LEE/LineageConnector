using System;
using System.Runtime.InteropServices;

/// <summary>
/// 시스템 시간
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SYSTEMTIME
{
    //////////////////////////////////////////////////////////////////////////////////////////////////// Field
    ////////////////////////////////////////////////////////////////////////////////////////// Public

    #region Field

    /// <summary>
    /// 연도
    /// </summary>
    public ushort Year;

    /// <summary>
    /// 월
    /// </summary>
    public ushort Month;

    /// <summary>
    /// 요일
    /// </summary>
    public ushort DayOfWeek;

    /// <summary>
    /// 일
    /// </summary>
    public ushort Day;

    /// <summary>
    /// 시
    /// </summary>
    public ushort Hour;

    /// <summary>
    /// 분
    /// </summary>
    public ushort Minute;

    /// <summary>
    /// 초
    /// </summary>
    public ushort Second;

    /// <summary>
    /// 밀리초
    /// </summary>
    public ushort Millisecond;

    #endregion
}