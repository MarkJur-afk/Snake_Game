using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    class Progarm
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(80, 25);
            Console.SetBufferSize(80, 25);



            HorizontalLine upLine = new HorizontalLine(0, 78, 0, '+');
            HorizontalLine downLine = new HorizontalLine(0, 78, 24, '+');
            VerticalLine leftLine = new VerticalLine(0, 24, 0, '+');
            VerticalLine rightLine = new VerticalLine(0, 24, 78, '+');
            upLine.Draw();
            downLine.Draw();
            leftLine.Draw();
            rightLine.Draw();



            Point p1 = new Point(1, 3, '*');
            //p1.x = 1;
            //p1.y = 3;
            //p1.sym = '*';
            p1.Draw();

            Point p2 = new Point(4, 5, '#');
            p2.Draw();



            List<Point> pList = new List<Point>();
            pList.Add(p1);
            pList.Add(p2);


            HorizontalLine line = new HorizontalLine(5, 10, 8, '*');
            line.Draw();


            
            //int x1 = 1;
            //int y1 = 3;
            //char sym1 = '*';

            
            //Draw(x1, y1, sym1 );

            //int x2 = 4;
            //int y2 = 5;
            //char sym2 = '#';

            //Console.SetCursorPosition(x2, y2);
            //Console.WriteLine(sym2);

            //Draw(x2, y2, sym2);
            Console.ReadLine();

        }

    }
}
