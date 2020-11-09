using System;

namespace csAlgorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            Board board = new Board();
            Player player = new Player();
            board.Initialize(25, player);
            player.Initialize(1, 1, board);

            Console.CursorVisible = false;
            const int WAIT_TICK = 1000 / 30;

            int lastTick = 0;
            while (true)
            {
                int currentTick = System.Environment.TickCount;

                if (currentTick - lastTick < WAIT_TICK)
                    continue;
                int deltaTick = currentTick - lastTick;
                lastTick = currentTick;  //1000/30초 마다 코드 진행, 프레임 관리

                player.Update(deltaTick);

                //렌더링
                Console.SetCursorPosition(0, 0);
                board.Render();
            }
        }
    }
}
