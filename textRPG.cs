using System;
using System.Collections.Generic;
using System.Text;
/*
 * 파일 분류 작업 해보기
 * 필요한 객체 먼저 생각해보기 
 */
namespace csharp1
{
    class textRPG 
    {
        static void Main(string[] args)
        {
            game game = new game();
            while (true)
            {
                game.Process();
            }
        }
    }
}
