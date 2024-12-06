using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequests
{
    public static class Progressbar
    {
        public static int Progress = 0;
        public static void UpdateProgressBar(int total)
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1); // Set position at the last line
            Console.Write("[");

            int barWidth = Console.WindowWidth - 10; // Adjust for text and edges
            int filledWidth = (int)((double)Progress / total * barWidth);

            Console.Write(new string('=', filledWidth));
            Console.Write(new string(' ', barWidth - filledWidth));
            Console.Write($"] {Progress}/{total}");

            Console.Out.Flush(); // Ensure the output appears immediately
        }
    }
}
