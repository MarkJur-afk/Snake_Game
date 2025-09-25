using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NAudio.Wave;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            var soundManager = new SoundManager();
            var scoreManager = new ScoreManager("scores.csv");

            var game = new Game(soundManager, scoreManager);
            game.Run();
        }
    }

    // ==================== GAME ====================
    class Game
    {
        private readonly SoundManager _soundManager;
        private readonly ScoreManager _scoreManager;
        private int _score = 0;
        private string _playerName = "";

        public Game(SoundManager soundManager, ScoreManager scoreManager)
        {
            _soundManager = soundManager;
            _scoreManager = scoreManager;
        }

        public void Run()
        {
            // показать топ
            _scoreManager.ShowTopScores();

            // имя игрока
            Console.Write("\nSisestage oma nimi: ");
            _playerName = Console.ReadLine();
            if (string.IsNullOrEmpty(_playerName))
                _playerName = "Anonymous";

            // консоль
            Console.Clear();
            Console.SetWindowSize(80, 25);
            Console.SetBufferSize(80, 25);

            ShowScore();
            Walls walls = new Walls(80, 25);
            walls.Draw();

            Point p = new Point(4, 5, '*');
            Snake snake = new Snake(p, 4, Direction.RIGHT);
            snake.Draw();

            FoodCreator foodCreator = new FoodCreator(80, 25, '$');
            Point food = foodCreator.CreateFood();
            food.Draw();

            _soundManager.PlayBackgroundMusic();

            while (true)
            {
                if (walls.IsHit(snake) || snake.IsHitTail())
                    break;

                if (snake.Eat(food))
                {
                    _soundManager.PlayEatSound();
                    _score += 10;
                    ShowScore();

                    food = foodCreator.CreateFood();
                    food.Draw();
                }
                else
                {
                    snake.Move();
                }

                Thread.Sleep(100);

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true); // исправил
                    snake.HandleKey(key.Key);
                }
            }

            _soundManager.StopBackgroundMusic();
            _soundManager.PlayDeathSound();

            _scoreManager.SaveScore(_playerName, _score);

            WriteGameOver();
            Console.ReadLine();
        }

        private void ShowScore()
        {
            Console.SetCursorPosition(2, 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Mängija: {_playerName} | Skoor: {_score}");
            Console.ResetColor();
        }

        private void WriteGameOver()
        {
            int xOffset = 25;
            int yOffset = 8;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(xOffset, yOffset++);
            WriteText("============================", xOffset, yOffset++);
            WriteText("Mäng on Lõpetatud", xOffset + 1, yOffset++);
            WriteText($"Teie tulemus: {_score} punkti", xOffset + 1, yOffset++);
            yOffset++;
            WriteText("Автор: Mark Jurgen", xOffset + 2, yOffset++);
            WriteText("============================", xOffset, yOffset++);
            WriteText("Tulemus on salvestatud!", xOffset + 1, yOffset++);
            Thread.Sleep(4000);
            Console.Clear();
        }

        private void WriteText(string text, int xOffset, int yOffset)
        {
            Console.SetCursorPosition(xOffset, yOffset);
            Console.WriteLine(text);
        }
    }

    // ==================== SOUND MANAGER ====================
    class SoundManager : IDisposable
    {
        private IWavePlayer _backgroundPlayer;
        private AudioFileReader _backgroundMusic;
        private IWavePlayer _eatPlayer;
        private AudioFileReader _eatSound;
        private IWavePlayer _deathPlayer;
        private AudioFileReader _deathSound;

        public SoundManager()
        {
            try
            {
                if (File.Exists("Main.mp3"))
                {
                    _backgroundMusic = new AudioFileReader("Main.mp3");
                    _backgroundPlayer = new WaveOutEvent();
                }

                if (File.Exists("eat.mp3"))
                {
                    _eatSound = new AudioFileReader("eat.mp3");
                    _eatPlayer = new WaveOutEvent();
                }

                if (File.Exists("dead.mp3"))
                {
                    _deathSound = new AudioFileReader("dead.mp3");
                    _deathPlayer = new WaveOutEvent();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Viga helide laadimisel: " + ex.Message);
            }
        }

        public void PlayBackgroundMusic()
        {
            if (_backgroundMusic != null && _backgroundPlayer != null)
            {
                _backgroundMusic.Position = 0;
                _backgroundPlayer.Init(_backgroundMusic);
                _backgroundPlayer.Play();
                _backgroundPlayer.PlaybackStopped += (s, e) =>
                {
                    _backgroundMusic.Position = 0;
                    _backgroundPlayer.Play();
                };
            }
        }

        public void StopBackgroundMusic()
        {
            _backgroundPlayer?.Stop();
        }

        public void PlayEatSound()
        {
            if (_eatSound != null && _eatPlayer != null)
            {
                _eatSound.Position = 0;
                _eatPlayer.Init(_eatSound);
                _eatPlayer.Play();
            }
        }

        public void PlayDeathSound()
        {
            if (_deathSound != null && _deathPlayer != null)
            {
                _deathSound.Position = 0;
                _deathPlayer.Init(_deathSound);
                _deathPlayer.Play();
            }
        }

        public void Dispose()
        {
            _backgroundPlayer?.Dispose();
            _backgroundMusic?.Dispose();
            _eatPlayer?.Dispose();
            _eatSound?.Dispose();
            _deathPlayer?.Dispose();
            _deathSound?.Dispose();
        }
    }

    // ==================== SCORE MANAGER ====================
    class ScoreManager
    {
        private readonly string _filePath;

        public ScoreManager(string filePath)
        {
            _filePath = filePath;
        }

        public void SaveScore(string playerName, int score)
        {
            try
            {
                string entry = $"{DateTime.Now:dd.MM.yyyy HH:mm};{playerName};{score}";
                File.AppendAllLines(_filePath, new[] { entry });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Viga tulemuste salvestamisel: " + ex.Message);
            }
        }

        public void ShowTopScores()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== PARIMAD TULEMUSED ===");
            Console.ResetColor();

            if (!File.Exists(_filePath))
            {
                Console.WriteLine("Tulemusi pole veel!");
                Console.WriteLine("========================");
                return;
            }

            try
            {
                var lines = File.ReadAllLines(_filePath);
                var scores = new List<(string name, int score, string date)>();

                foreach (var line in lines)
                {
                    var parts = line.Split(';');
                    if (parts.Length == 3 && int.TryParse(parts[2], out int s))
                        scores.Add((parts[1], s, parts[0]));
                }

                var top = scores.OrderByDescending(x => x.score).Take(3).ToList();

                if (top.Count == 0)
                    Console.WriteLine("Tulemusi pole veel!");
                else
                {
                    for (int i = 0; i < top.Count; i++)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"{i + 1}. ");
                        Console.ResetColor();
                        Console.Write($"{top[i].name}: ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write($"{top[i].score} punkti ");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine($"({top[i].date})");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Viga tulemuste lugemisel: " + ex.Message);
            }

            Console.ResetColor();
            Console.WriteLine("========================");
        }
    }
}
