using System;
using System.Runtime.InteropServices;

namespace No_recoil_r
{
    public class MouseMover
    {
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);
        
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);
        
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }
          public void MoveMouseWithForce(double forceDown, double forceLeft, double forceRight)
        {
            GetCursorPos(out POINT currentPos);
            
            int deltaX = (int)((forceRight - forceLeft) * 0.5); 
            int deltaY = (int)(forceDown * 0.5); 
            
            int newX = currentPos.X + deltaX;
            int newY = currentPos.Y + deltaY;
            
            SetCursorPos(newX, newY);
        }
        
        public void MoveMouseDown(double force)
        {
            GetCursorPos(out POINT currentPos);
            int deltaY = (int)(force * 5);
            SetCursorPos(currentPos.X, currentPos.Y + deltaY);
        }
        
        public void MoveMouseLeft(double force)
        {
            GetCursorPos(out POINT currentPos);
            int deltaX = (int)(force * 5);
            SetCursorPos(currentPos.X - deltaX, currentPos.Y);
        }
        
        public void MoveMouseRight(double force)
        {
            GetCursorPos(out POINT currentPos);
            int deltaX = (int)(force * 5);
            SetCursorPos(currentPos.X + deltaX, currentPos.Y);
        }
    }
}