using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace TestClickByWinApi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("=================");
                Console.WriteLine("Enter to continue");
                Console.ReadLine();

                Console.WriteLine("Start after 3s. Move cursor to the windows");
                Thread.Sleep(3000);

                // Lấy tạo độ và handle đang di chuột trên màn hình
                GetCursorPos(out POINT lpPoint);
                var handle = WindowFromPoint(lpPoint);

                ScreenToClient(handle, ref lpPoint); // Chuyển đổi sang tạo độ của chuột trên màn hình => trên handle

                // Bấm chuột
                PostMessage(handle, WM_LBUTTONDOWN, 1, MakeLParam(lpPoint.X, lpPoint.Y)); // bấm chuột trái xuống. Firefox cứ chạy lệnh này là làm context menu bị ẩn
                PostMessage(handle, WM_LBUTTONUP, 1, MakeLParam(lpPoint.X, lpPoint.Y)); // bấm chuột trái lên

                Console.WriteLine($"Click to handle={FormatIntPtr(handle)}, pos={lpPoint.X}, {lpPoint.Y}");
            }
        }

        static int MakeLParam(int x, int y)
        {
            //ConsoleWriteLineIfDebug($"MakeLParam {((y << 16) | x)} vs {(y << 16) | (x & 0xFFFF)}");
            //return (int)((y << 16) | x);
            return (y << 16) | (x & 0xFFFF);
        }

        static string FormatIntPtr(IntPtr handle)
        {
            return "0x" + handle.ToString("X").PadLeft(IntPtr.Size * 2, '0');
        }

        #region Const
        private const int WM_LBUTTONDOWN = 0x0201; // Bấm chuột trái
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;
        #endregion

        #region Win api
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(POINT point);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }
        #endregion
    }
}
