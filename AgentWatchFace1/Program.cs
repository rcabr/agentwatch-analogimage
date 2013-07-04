using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using System.IO;

namespace AgentWatchFace1
{
    public class Program
    {
        const int HandLengthHour = 35;
        const int HandLengthMinute = 55;
        const int HandWidthHour = 2;
        const int HandWidthMinute = 2;

        static readonly int _centerX = Bitmap.MaxWidth / 2;
        static readonly int _centerY = Bitmap.MaxHeight / 2;

        static Bitmap _display;
        static Timer _updateClockTimer;

        public static void Main()
        {
            // initialize our display buffer
            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            // display the time immediately
            UpdateTime(null);

            // obtain the current time
            DateTime currentTime = DateTime.Now;
            // set up timer to refresh time every minute
            TimeSpan dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond); // start timer at beginning of next minute
            TimeSpan period = new TimeSpan(0, 0, 1, 0, 0); // update time every minute
            _updateClockTimer = new Timer(UpdateTime, null, dueTime, period); // start our update timer

            // go to sleep; time updates will happen automatically every minute
            Thread.Sleep(Timeout.Infinite);
        }

        static void UpdateTime(object state)
        {
            // obtain the current time
            DateTime currentTime = DateTime.Now;
            // clear our display buffer
            _display.Clear();

            DrawBackground();

            DrawAnalogHands(currentTime);

            
            // flush the display buffer to the display
            _display.Flush();
        }

        private static void DrawBackground()
        {
            byte[] img = Resources.GetBytes(Resources.BinaryResources.Skull);
            Bitmap imgbmp = new Bitmap(img, Bitmap.BitmapImageType.Bmp);
            _display.DrawImage(0, 0, imgbmp, 0, 0, 128, 128);

            Random r = new Random();

            _display.DrawEllipse(Color.White, r.Next(127), r.Next(127), 1, 1);
        }

        private static void DrawAnalogHands(DateTime currentTime)
        {
            var hour = currentTime.Hour;
            var minute = currentTime.Minute;

            int hourAngle = (int)(0.5 * ((60 * hour) + minute)) - 90;
            int minuteAngle = minute * 6 - 90;
            double hourRadians = (hourAngle * System.Math.PI / 180);
            double minuteRadians = (minuteAngle * System.Math.PI / 180);

            var hourEndX = _centerX + HandLengthHour * System.Math.Cos(hourRadians);
            var hourEndY = _centerY + HandLengthHour * System.Math.Sin(hourRadians);

            var minuteEndX = _centerX + HandLengthMinute * System.Math.Cos(minuteRadians);
            var minuteEndY = _centerY + HandLengthMinute * System.Math.Sin(minuteRadians);

            _display.DrawLine(Color.Black, HandWidthHour, _centerX, _centerY, (int)hourEndX, (int)hourEndY);
            _display.DrawLine(Color.Black, HandWidthMinute, _centerX, _centerY, (int)minuteEndX, (int)minuteEndY);
            _display.DrawLine(Color.White, 1, _centerX, _centerY, (int)hourEndX, (int)hourEndY);
            _display.DrawLine(Color.White, 1, _centerX, _centerY, (int)minuteEndX, (int)minuteEndY);
        }


    }
}
