using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ControlScreener
{
    public static class CaptureScreen
    {
        private static IntPtr m_HBitmap;

        private struct SIZE
        {
            public int cx;
            public int cy;
        }

        private class PlatformInvokeGDI32
        {
            public const int SRCCOPY = 13369376;

            [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
            public static extern IntPtr DeleteDC(IntPtr hDc);

            [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
            public static extern IntPtr DeleteObject(IntPtr hDc);

            [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
            public static extern bool BitBlt(IntPtr hdcDest, int xDest,
                int yDest, int wDest, int hDest, IntPtr hdcSource,
                int xSrc, int ySrc, int RasterOp);

            [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc,
                int nWidth, int nHeight);

            [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

            [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
            public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        }

        private class PlatformInvokeUSER32
        {

            public const int SM_CXSCREEN = 0;
            public const int SM_CYSCREEN = 1;

            [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
            public static extern IntPtr GetDesktopWindow();

            [DllImport("user32.dll", EntryPoint = "GetDC")]
            public static extern IntPtr GetDC(IntPtr ptr);

            [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
            public static extern int GetSystemMetrics(int abc);

            [DllImport("user32.dll", EntryPoint = "GetWindowDC")]
            public static extern IntPtr GetWindowDC(Int32 ptr);

            [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
        }

        public static Bitmap GetDesktopImage()
        {
            //В размер переменной мы должны сохранить размер экрана.
            SIZE size;

            //В размер переменной мы должны сохранить размер экрана.
            IntPtr hBitmap;

            //Здесь мы получаем дескриптор контекста устройства рабочего стола.
            IntPtr hDC = PlatformInvokeUSER32.GetDC
                          (PlatformInvokeUSER32.GetDesktopWindow());

            //Здесь мы делаем контекст устройства

            IntPtr hMemDC = PlatformInvokeGDI32.CreateCompatibleDC(hDC);

            ScreenScalingFactor scf = new ScreenScalingFactor();

            //Мы передаем SM_CXSCREEN константа GetSystemMetrics               // и получить X координаты экрана.
            size.cx = ((PlatformInvokeUSER32.GetSystemMetrics
                      (PlatformInvokeUSER32.SM_CXSCREEN) * scf.ScreenDPI) / scf.dpiBase);

            //Мы передаем SM_CYSCREEN константа GetSystemMetrics и получить Y координаты экрана.
            size.cy = ((PlatformInvokeUSER32.GetSystemMetrics
                      (PlatformInvokeUSER32.SM_CYSCREEN) * scf.ScreenDPI) / scf.dpiBase);

            //Мы создаем совместимое растровое изображение экрана с размером с помощью
            //контекст устройства экрана.
            hBitmap = PlatformInvokeGDI32.CreateCompatibleBitmap
                        (hDC, size.cx, size.cy);

            //Как hBitmap IntPtr, мы не можем проверить его значение null.
            //Для этой цели используется значение IntPtr.Zero.
            if (hBitmap != IntPtr.Zero)
            {
                //Здесь мы выбираем совместимое растровое изображение в памяти устройства
                //контекст и держит ссылку на старый битовый массив.
                IntPtr hOld = (IntPtr)PlatformInvokeGDI32.SelectObject
                                       (hMemDC, hBitmap);
                //Мы копируем Битовый массив к контексту устройства памяти.
                PlatformInvokeGDI32.BitBlt(hMemDC, 0, 0, size.cx, size.cy, hDC,
                                           0, 0, PlatformInvokeGDI32.SRCCOPY);
                //Мы выбираем старый битовый массив назад к контексту устройства памяти.
                PlatformInvokeGDI32.SelectObject(hMemDC, hOld);
                //Мы удаляем контекст устройства памяти.
                PlatformInvokeGDI32.DeleteDC(hMemDC);
                //Мы выпускаем контекст устройства экрана.
                PlatformInvokeUSER32.ReleaseDC(PlatformInvokeUSER32.
                                               GetDesktopWindow(), hDC);
                //Изображение создано и сохранено в локальную переменную

                Bitmap bmp = System.Drawing.Image.FromHbitmap(hBitmap);
                //Освободим память, чтобы избежать утечек памяти.
                PlatformInvokeGDI32.DeleteObject(hBitmap);
                //Вызовим сборщик мусора.
                GC.Collect();
                //Вернем изображение
                return bmp;
            }
            //Если hBitmap пустой, возвратите пустой указатель.
            return null;
        }
    }
}
