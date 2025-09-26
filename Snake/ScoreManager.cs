using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ScoreManager
{
    private string filePath = "score.txt";

    public void SaveResult(Player player)
    {
        using (StreamWriter sw = new StreamWriter(filePath, true)) 
        {
            sw.WriteLine($"{player.Name}|{player.Score}");
        }
    }

    public List<Player> LoadResults()
    {
        List<Player> players = new List<Player>();

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length == 2)
                {
                    string name = parts[0];
                    int score = int.Parse(parts[1]);
                    players.Add(new Player(name, score));
                }
            }
        }

        return players.OrderByDescending(p => p.Score).ToList();
    }

    public void ShowTopResults(int topCount = 5)
    {
        var results = LoadResults();

        Console.WriteLine("\nТаблица рекордов:");
        foreach (var player in results.Take(topCount))
        {
            Console.WriteLine(player);
        }
    }
}
