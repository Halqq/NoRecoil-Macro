using System;
using System.Runtime.InteropServices;

namespace No_recoil_r
{
    public class MouseMover
    {
        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);
        
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public uint Type;
            public MOUSEINPUT MouseInput;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        
        private const uint INPUT_MOUSE = 0;
        private const uint MOUSEEVENTF_MOVE = 0x0001;
        
        private void SimulateMouseMove(int deltaX, int deltaY)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].Type = INPUT_MOUSE;
            inputs[0].MouseInput.dx = deltaX;
            inputs[0].MouseInput.dy = deltaY;
            inputs[0].MouseInput.mouseData = 0;
            inputs[0].MouseInput.dwFlags = MOUSEEVENTF_MOVE;
            inputs[0].MouseInput.time = 0;
            inputs[0].MouseInput.dwExtraInfo = IntPtr.Zero;
            
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
        
        public void MoveMouseWithForce(double forceDown, double forceLeft, double forceRight)
        {
            int deltaX = (int)((forceRight - forceLeft) * 0.5); 
            int deltaY = (int)(forceDown * 0.5); 
            
            SimulateMouseMove(deltaX, deltaY);
        }
        
        public void MoveMouseDown(double force)
        {
            int deltaY = (int)(force * 5);
            SimulateMouseMove(0, deltaY);
        }
        
        public void MoveMouseLeft(double force)
        {
            int deltaX = (int)(force * 5);
            SimulateMouseMove(-deltaX, 0);
        }
        
        public void MoveMouseRight(double force)
        {
            int deltaX = (int)(force * 5);
            SimulateMouseMove(deltaX, 0);
        }
    }
}