using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.ServiceProcess;
using System.Threading;
using System.Runtime.InteropServices;
using LineageConnector.WindowsAPI;
using LineageConnector.Properties;

namespace LineageConnector
{
    public partial class ConnectorMain : Form
    {
        private bool serverloading;
        private bool close_clicked;

        [DllImport("dxwnd.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr dxwnd_Init();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);


        public ConnectorMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (SystemTimeHelper.CheckPermission(DateTime.Now))
            {
                this.serverloading = true;
                ProcessHelper.LockManagement();
                new Thread(new ThreadStart(this.pre_process)).Start();
            } else
            {
                MessageBox.Show("시스템 날짜 변경 권한이 없습니다. 관리자 권한으로 실행하십시오.");
                this.close_clicked = true;
                Application.Exit();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs args)
        {
            if (!close_clicked)
            {
                // X 버튼 무효화.
                args.Cancel = true;
                MessageBox.Show("프로그램 닫기 버튼으로 종료하여 주십시오.");
            }
        }

        // 접속버튼 이벤트
        private void button1_Click(object sender, EventArgs e)
        {
            // TODO : 린빈파일 다운로드 받을 경로를 얻어온다.
            // TODO : 습득한 경로로부터 린빈파일을 다운로드
            // TODO : 접속 IP와 포트정보를 얻어온다.
            // TODO : 린빈에 IP, 포트정보를 조합하여 서버에 접속.
            // TODO : 통신 시, 암호화를 실시.
            // TODO : 애플리케이션에 대한 보안 강화 실장.
            // TODO : SystemTime.cs, SystemTimeHelper.cs, ProcessHelper.cs는 별도 dll로 작성
            // TODO : 각 서버에 대하여 사용 허가/불허를 컨트롤할 수 있도록. (접속기 인증서버)
            // TODO : 린투데이에 별도 후원페이지 작성하여 결제연동.
            // string exportFile = Path.GetDirectoryName(Application.ExecutablePath) + "\\lin.exe";
            string exportFile = Path.GetFullPath("D:\\lineage_kr_client_270\\") + "lin.exe";
            string ip = "122.222.80.14";
            string port = "2000";

            // 프로세스 생성
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = exportFile;
            startInfo.UseShellExecute = false;

            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = exportFile;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.StartInfo.Arguments = ip;

            // 프로세스 시작
            try
            {
                process.Start();
                this.serverloading = false;
                // Wait for the process to start
                while (process.MainWindowHandle == IntPtr.Zero)
                {
                    Thread.Sleep(100);
                    process.Refresh();
                }
                // Call dxwnd_Init to put the process in windowed mode
                IntPtr hWnd = dxwnd_Init();
                if (hWnd != IntPtr.Zero)
                {
                    SetParent(hWnd, process.MainWindowHandle);
                    MoveWindow(hWnd, 0, 0, 800, 600, true);
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 린빈파일 선택
        /// </summary>
        /// <returns>린빈저장경로</returns>
        private void button2_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 프로그램 닫기
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            if (SystemTimeHelper.CheckPermission(DateTime.Now))
            {
                // 작업관리자 활성화
                ProcessHelper.ReleaseManagement();
                // 시스템 시간 동기화
                SystemTimeHelper.SetRemoteSystemTime();
            }
            this.close_clicked = true;
            // 종료
            Application.Exit();
        }


        private void pre_process()
        {
            // 시스템 날짜 변경
            SystemTimeHelper.SetSystemTime(new DateTime(2004, 01, 01, 00, 00, 00));
            // 접속방해 서비스 검색
            // regdelete();
        }

        private void regdelete()
        {
            bool flag = false;
            RegistryKey registryKey = Registry.LocalMachine;
            registryKey = registryKey.OpenSubKey("SYSTEM\\ControlSet001\\Services\\SecurityClient", false);
            if (registryKey != null)
            {
                flag = true;
                Registry.LocalMachine.DeleteSubKey("SYSTEM\\\\ControlSet001\\\\Services\\\\SecurityClient", false);
                registryKey.Close();
            }
            RegistryKey registryKey2 = Registry.LocalMachine;
            registryKey2 = registryKey2.OpenSubKey("SYSTEM\\ControlSet001\\Services\\SecuritySession", false);
            if (registryKey2 != null)
            {
                flag = true;
                Registry.LocalMachine.DeleteSubKey("SYSTEM\\\\ControlSet001\\\\Services\\\\SecuritySession", false);
                registryKey2.Close();
            }
            RegistryKey registryKey3 = Registry.LocalMachine;
            registryKey3 = registryKey3.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\SecurityClient", false);
            if (registryKey3 != null)
            {
                flag = true;
                Registry.LocalMachine.DeleteSubKey("SYSTEM\\\\CurrentControlSet\\\\Services\\\\SecurityClient", false);
                registryKey3.Close();
            }
            RegistryKey registryKey4 = Registry.LocalMachine;
            registryKey4 = registryKey4.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\SecuritySession", false);
            if (registryKey4 != null)
            {
                flag = true;
                Registry.LocalMachine.DeleteSubKey("SYSTEM\\\\CurrentControlSet\\\\Services\\\\SecuritySession", false);
                registryKey4.Close();
            }
            RegistryKey registryKey5 = Registry.LocalMachine;
            registryKey5 = registryKey5.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\SecuritySystem", false);
            if (registryKey5 != null)
            {
                flag = true;
                Registry.LocalMachine.DeleteSubKey("SYSTEM\\\\CurrentControlSet\\\\Services\\\\SecuritySystem", false);
                registryKey5.Close();
            }
            RegistryKey registryKey6 = Registry.LocalMachine;
            registryKey6 = registryKey6.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\SecurityLogin", false);
            if (registryKey6 != null)
            {
                flag = true;
                Registry.LocalMachine.DeleteSubKey("SYSTEM\\\\CurrentControlSet\\\\Services\\\\SecurityLogin", false);
                registryKey6.Close();
            }
            ServiceController[] services = ServiceController.GetServices();
            for (int i = 0; i < services.Length; i++)
            {
                if ((services[i].ServiceName == "SecurityClient" || services[i].ServiceName == "SecuritySession" || services[i].ServiceName == "SecuritySystem" || services[i].ServiceName == "SecurityLogin") && services[i].Status == ServiceControllerStatus.Running)
                {
                    services[i].Stop();
                }
            }
            if (flag)
            {
                MessageBox.Show("접속방해 및 디도스관련 악성코드가 발견되어 삭제하였습니다. 접속이 불가능하면 컴퓨터 재부팅을 하시기 바랍니다.", "접속방해 서비스 검출");
            }
        }



        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }


    }
}
