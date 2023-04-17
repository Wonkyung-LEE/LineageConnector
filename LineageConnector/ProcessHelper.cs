using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;

namespace LineageConnector
{
    internal class ProcessHelper
    {
        /// <summary>
        /// 프로세스 헬퍼
        /// </summary>
        #region 프로세스 접근 권한 - ProcessAccessRights

        /// <summary>
        /// 프로세스 접근 권한
        /// </summary>
        [Flags]
        public enum ProcessAccessRights
        {
            /// <summary>
            /// PROCESS_CREATE_PROCESS
            /// </summary>
            PROCESS_CREATE_PROCESS = 0x0080,

            /// <summary>
            /// PROCESS_CREATE_THREAD
            /// </summary>
            PROCESS_CREATE_THREAD = 0x0002,

            /// <summary>
            /// PROCESS_DUP_HANDLE
            /// </summary>
            PROCESS_DUP_HANDLE = 0x0040,

            /// <summary>
            /// PROCESS_QUERY_INFORMATION
            /// </summary>
            PROCESS_QUERY_INFORMATION = 0x0400,

            /// <summary>
            /// PROCESS_QUERY_LIMITED_INFORMATION
            /// </summary>
            PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,

            /// <summary>
            /// PROCESS_SET_INFORMATION
            /// </summary>
            PROCESS_SET_INFORMATION = 0x0200,

            /// <summary>
            /// PROCESS_SET_QUOTA
            /// </summary>
            PROCESS_SET_QUOTA = 0x0100,

            /// <summary>
            /// PROCESS_SUSPEND_RESUME
            /// </summary>
            PROCESS_SUSPEND_RESUME = 0x0800,

            /// <summary>
            /// PROCESS_TERMINATE
            /// </summary>
            PROCESS_TERMINATE = 0x0001,

            /// <summary>
            /// PROCESS_VM_OPERATION
            /// </summary>
            PROCESS_VM_OPERATION = 0x0008,

            /// <summary>
            /// PROCESS_VM_READ
            /// </summary>
            PROCESS_VM_READ = 0x0010,

            /// <summary>
            /// PROCESS_VM_WRITE
            /// </summary>
            PROCESS_VM_WRITE = 0x0020,

            /// <summary>
            /// DELETE
            /// </summary>
            DELETE = 0x00010000,

            /// <summary>
            /// READ_CONTROL
            /// </summary>
            READ_CONTROL = 0x00020000,

            /// <summary>
            /// SYNCHRONIZE
            /// </summary>
            SYNCHRONIZE = 0x00100000,

            /// <summary>
            /// WRITE_DAC
            /// </summary>
            WRITE_DAC = 0x00040000,

            /// <summary>
            /// WRITE_OWNER
            /// </summary>
            WRITE_OWNER = 0x00080000,

            /// <summary>
            /// STANDARD_RIGHTS_REQUIRED
            /// </summary>
            STANDARD_RIGHTS_REQUIRED = 0x000f0000,

            /// <summary>
            /// PROCESS_ALL_ACCESS
            /// </summary>
            PROCESS_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFF)
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Import
        ////////////////////////////////////////////////////////////////////////////////////////// Static
        //////////////////////////////////////////////////////////////////////////////// Private

        #region 현재 프로세스 구하기 - GetCurrentProcess()

        /// <summary>
        /// 현재 프로세스 구하기
        /// </summary>
        /// <returns>현재 프로세스 핸들</returns>
        [DllImport("kernel32")]
        private static extern IntPtr GetCurrentProcess();

        #endregion
        #region 커널 객체 보안 구하기 - GetKernelObjectSecurity(handle, securityInformation, securityDescriptor, length, lengthNeeded)

        /// <summary>
        /// 커널 객체 보안 구하기
        /// </summary>
        /// <param name="handle">커널 객체 핸들</param>
        /// <param name="securityInformation">보안 정보</param>
        /// <param name="securityDescriptor">보안 설명자</param>
        /// <param name="length">길이</param>
        /// <param name="lengthNeeded">필요 길이</param>
        /// <returns>처리 결과</returns>
        [DllImport("advapi32", SetLastError = true)]
        private static extern bool GetKernelObjectSecurity
        (
            IntPtr handle,
            int securityInformation,
            [Out] byte[] securityDescriptor,
            uint length,
            out uint lengthNeeded
        );

        #endregion
        #region 커널 객체 보안 설정하기 - SetKernelObjectSecurity(handle, securityInformation, securityDescriptor)

        /// <summary>
        /// 커널 객체 보안 설정하기
        /// </summary>
        /// <param name="handle">커널 객체 핸들</param>
        /// <param name="securityInformation">보안 정보</param>
        /// <param name="securityDescriptor">보안 설명자</param>
        /// <returns>처리 결과</returns>
        [DllImport("advapi32", SetLastError = true)]
        private static extern bool SetKernelObjectSecurity
        (
            IntPtr handle,
            int securityInformation,
            [In] byte[] securityDescriptor
        );

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Field
        ////////////////////////////////////////////////////////////////////////////////////////// Private

        #region Field

        /// <summary>
        /// DACL_SECURITY_INFORMATION
        /// </summary>
        private const int DACL_SECURITY_INFORMATION = 0x00000004;

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Method
        ////////////////////////////////////////////////////////////////////////////////////////// Static
        //////////////////////////////////////////////////////////////////////////////// Public

        #region 프로세스 보호하기 - ProtectProcess()

        /// <summary>
        /// 프로세스 보호하기
        /// </summary>
        public static void ProtectProcess()
        {
            IntPtr processHandle = GetCurrentProcess();

            RawSecurityDescriptor descriptor = GetProcessSecurityDescriptor(processHandle);

            for (int i = descriptor.DiscretionaryAcl.Count - 1; i > 0; i--)
            {
                descriptor.DiscretionaryAcl.RemoveAce(i);
            }

            descriptor.DiscretionaryAcl.InsertAce
            (
                0,
                new CommonAce
                (
                    AceFlags.None,
                    AceQualifier.AccessDenied,
                    (int)ProcessAccessRights.PROCESS_ALL_ACCESS,
                    new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    false,
                    null
                )
            );

            SetProcessSecurityDescriptor(processHandle, descriptor);
        }

        /// <summary>
        /// 작업관리자 제어 - Lock
        /// </summary>
        /// <returns>레지스트리 등록</returns>
        public static void LockManagement()
        {
            // 작업관리자 비활성화
            RegistryKey regkey;
            string keyValueInt = "1";
            string subKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";

            try
            {
                regkey = Registry.CurrentUser.CreateSubKey(subKey);
                regkey.SetValue("DisableTaskMgr", keyValueInt);
                regkey.Close();
                MessageBox.Show("작업관리자 실행을 비활성화 하였습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MessageBox.Show("작업관리자 제어에 필요한 권한이 부족합니다. 관리자 권한으로 실행하십시오.");
            }
        }

        /// <summary>
        /// 작업관리자 제어 - Release
        /// </summary>
        /// <returns>레지스트리 삭제</returns>
        public static void ReleaseManagement()
        {
            // 작업관리자 활성화
            RegistryKey regkey;
            string subKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";

            try
            {
                regkey = Registry.CurrentUser.OpenSubKey(subKey, true);
                if (regkey.ValueCount== 0)
                {
                    return;
                }
                regkey.DeleteValue("DisableTaskMgr");
                regkey.Close();
                MessageBox.Show("작업관리자 실행을 활성화 하였습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MessageBox.Show("작업관리자 제어에 필요한 권한이 부족합니다. 관리자 권한으로 실행하십시오.");
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////// Private

        #region 프로세스 보안 설명자 구하기 - GetProcessSecurityDescriptor(processHandle)

        /// <summary>
        /// 프로세스 보안 설명자 구하기
        /// </summary>
        /// <param name="processHandle">프로세스 핸들</param>
        /// <returns>RAW 보안 설명자</returns>
        private static RawSecurityDescriptor GetProcessSecurityDescriptor(IntPtr processHandle)
        {
            byte[] byteArray = new byte[0];
            uint bufferSizeNeeded;

            GetKernelObjectSecurity
            (
                processHandle,
                DACL_SECURITY_INFORMATION,
                byteArray,
                0,
                out bufferSizeNeeded
            );

            if (bufferSizeNeeded < 0 || bufferSizeNeeded > short.MaxValue)
            {
                throw new Win32Exception();
            }

            if
            (
                !GetKernelObjectSecurity
                (
                    processHandle,
                    DACL_SECURITY_INFORMATION,
                    byteArray = new byte[bufferSizeNeeded],
                    bufferSizeNeeded,
                    out bufferSizeNeeded
                )
            )
            {
                throw new Win32Exception();
            }

            return new RawSecurityDescriptor(byteArray, 0);
        }

        #endregion
        #region 프로세스 보안 설명자 설정하기 - SetProcessSecurityDescriptor(processHandle, descriptor)

        /// <summary>
        /// 프로세스 보안 설명자 설정하기
        /// </summary>
        /// <param name="processHandle">프로세스 핸들</param>
        /// <param name="descriptor">RAW 보안 설명자</param>
        private static void SetProcessSecurityDescriptor(IntPtr processHandle, RawSecurityDescriptor descriptor)
        {
            byte[] byteArray = new byte[descriptor.BinaryLength];

            descriptor.GetBinaryForm(byteArray, 0);

            if (!SetKernelObjectSecurity(processHandle, DACL_SECURITY_INFORMATION, byteArray))
            {
                throw new Win32Exception();
            }
        }
        #endregion
    }
}
