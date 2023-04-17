using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace LineageConnector
{
    internal static class Program
    {
        private static int process_count;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            ProcessHelper.ProtectProcess();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (IsExistProcess(false) == false)
            {
                process_count++;
                Application.Run(new ConnectorMain());
            } else
            {
                string message = String.Format("프로세스가 실행되어 있습니다.\r\n현재 프로세스를 종료하고 실행하시겠습니까?\r\n새로 실행하시려면 아니오를 선택하여 주십시오.");

                if (MessageBox.Show(message, "프로세스 확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    IsExistProcess(true);
                } else if (process_count < 3)
                {
                    process_count++;
                    Application.Run(new ConnectorMain());
                }
                else if (process_count > 3)
                {
                    MessageBox.Show("더 이상 다중실행할 수 없습니다.");
                }
            }
        }

        static bool IsExistProcess(bool bKillProcess)
        {
            bool exist = false;
            foreach (Process process in Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName))
            {
                // 현재 실행되는 프로세스인 경우는 스킵
                if (process.Id == Process.GetCurrentProcess().Id)
                {
                    continue;
                }

                exist = true;

                if (bKillProcess)
                {
                    // 다른 프로세스가 떠 있으면 강제 종료
                    exist = KillProcess(process);
                }
                if (!exist)
                {
                    // 프로세스 정상 종료 시, 재실행
                    Application.Run(new ConnectorMain());
                }
            }

            return exist;
        }

        static bool KillProcess(Process process)
        {
            try
            {
                // 강제종료
                process.Kill(); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return false;
        }
    }
}
