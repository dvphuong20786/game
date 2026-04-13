using System;
using System.Threading;

namespace FlappyBirdConsole
{
    class Program
    {
        static int width = 60;
        static int height = 20;

        static double birdY = height / 2.0;
        static double birdVelocity = 0;
        static double gravity = -0.5; // Kéo xuống (trục Y hướng xuống tăng hay giảm?)
        static double jumpForce = 1.5;

        static int pipeX = width - 10;
        static int gapY = 8;
        static int gapSize = 6;
        static int score = 0;
        static bool gameOver = false;

        static void Main(string[] args)
        {
            Console.Title = "Flappy Bird Terminal";
            Console.CursorVisible = false;
            Console.Clear();

            // Trọng lực ở console: y=0 là đỉnh, y=20 là đáy
            // Do đó gravity phải làm Y TĂNG lên (rơi xuống)
            gravity = 0.4;
            jumpForce = -1.5; // Bay lên là Y GIẢM
            birdVelocity = 0;

            while (!gameOver)
            {
                // Xử lý phím
                while (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Spacebar)
                    {
                        birdVelocity = jumpForce;
                    }
                }

                // Vật lý
                birdVelocity += gravity;
                birdY += birdVelocity;

                pipeX--;

                if (pipeX < 0)
                {
                    pipeX = width - 5;
                    Random rnd = new Random();
                    gapY = rnd.Next(2, height - gapSize - 2);
                    score++;
                }

                // Va chạm
                if (birdY < 0 || birdY >= height - 1)
                {
                    gameOver = true; // Chạm đất hoặc trần
                }

                // Va chạm cột ống nước (Chim luôn ở hoành độ x = 10)
                int birdX = 10;
                if (pipeX <= birdX && pipeX + 2 >= birdX) 
                {
                    if (birdY <= gapY || birdY >= gapY + gapSize)
                    {
                        gameOver = true;
                    }
                }

                // Vẽ màn hình
                DrawScreen(birdX);
                Thread.Sleep(50); // Tốc độ game (50ms mỗi khung hình)
            }

            Console.SetCursorPosition(width / 2 - 5, height / 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" GAME OVER! ");
            Console.SetCursorPosition(width / 2 - 6, height / 2 + 1);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($" SCORE: {score} ");
            Console.ForegroundColor = ConsoleColor.White;
            
            // Xóa bộ đệm phím thừa tránh kẹt terminal
            while (Console.KeyAvailable) Console.ReadKey(true);
            Console.SetCursorPosition(0, height + 1);
        }

        static void DrawScreen(int birdX)
        {
            // Thay vì xóa toàn bộ màn hình, chúng ta ghi đè bằng một chuỗi lớn để không bị nháy hình (flicker)
            char[] screenBuffer = new char[width * height];
            for (int i = 0; i < screenBuffer.Length; i++) screenBuffer[i] = ' ';

            // Vẽ sàn
            for (int x = 0; x < width; x++) screenBuffer[(height - 1) * width + x] = '=';

            // Vẽ cột ống nước
            for (int y = 0; y < height - 1; y++)
            {
                if (y < gapY || y >= gapY + gapSize)
                {
                    if (pipeX >= 0 && pipeX < width) screenBuffer[y * width + pipeX] = '|';
                    if (pipeX + 1 >= 0 && pipeX + 1 < width) screenBuffer[y * width + pipeX + 1] = '|';
                }
            }

            // Vẽ chim
            int bY = (int)Math.Round(birdY);
            if (bY >= 0 && bY < height)
            {
                 screenBuffer[bY * width + birdX] = '@';
            }

            // In ra console
            Console.SetCursorPosition(0, 0);
            
            // Chuyển buffer thành chuỗi có dấu xuống dòng
            string output = "";
            for(int y=0; y<height; y++)
            {
                output += new string(screenBuffer, y*width, width) + "\n";
            }
            Console.Write(output);
            
            // Điểm góc trên
            Console.SetCursorPosition(2, 0);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write($" Score: {score} ");
            Console.ResetColor();
        }
    }
}
