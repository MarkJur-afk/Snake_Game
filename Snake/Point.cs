using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    class Point
    {
        public int x;
        public int y;
        public char sym;


        public Point()
        {
            Console.WriteLine("123");
        }

        public Point(int __x,  int __y, char __sym)
        {
            x = __x;
            y = __y;
            sym = __sym;
        }

        public void Draw()
        {
            Console.SetCursorPosition(x, y);
            Console.WriteLine(sym);
        }
    }
}
