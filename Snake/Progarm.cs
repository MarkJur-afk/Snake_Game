using System;
using System.Threading;
using Snake;

class Program
{
    static void Main(string[] args)
    {

        Console.SetWindowSize(80, 25);
        Console.SetBufferSize(80, 25);

        Walls walls = new Walls(80, 25);
        walls.Draw();

        Point p = new Point(4, 5, '*');
        SnakeGame snake = new SnakeGame(p, 4, Direction.RIGHT);

        FoodCreator foodCreator = new FoodCreator(80, 25, '$');
        Point food = foodCreator.CreateFood();
        food.Draw();

        Sounds sounds = new Sounds();
        sounds.PlayBackground();

        int score = 0;

        while (true)
        {
            if (snake.Eat(food))
            {
                score++;
                sounds.PlayEat();
                food = foodCreator.CreateFood();
                food.Draw();
            }
            else
            {
                snake.Move();
            }

            if (walls.IsHit(snake) || snake.IsHitTail())
            {
                sounds.PlayDead();
                sounds.StopBackground();
                break;
            }

            Thread.Sleep(100);

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                snake.HandleKey(key.Key);
            }
        }

        Console.Clear();
        Console.WriteLine("Koik!");
        Console.WriteLine($"Teie result: {score}");

        string name;
        do
        {
            Console.Write("Kirjuta oma nimi (min 3 sumbols): ");
            name = Console.ReadLine();
        } while (string.IsNullOrWhiteSpace(name) || name.Length < 3);

        Player player = new Player(name, score);
        ScoreManager manager = new ScoreManager();
        manager.SaveResult(player);
        manager.ShowTopResults(5);

        Console.WriteLine("\nZmak klavish...");
        Console.ReadKey();
    }
}
