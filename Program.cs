using System;
using System.Xml.Serialization;

namespace csharp1
{
    class Program
    {
        enum ClassType
        {
            None = 0,
            Knight = 1,
            Archer = 2,
            Mage = 3
        }

        enum MonsterType
        {
            None = 0,
            Slime = 1,
            Orc = 2,
            Skeleton = 3
        }

        struct Player //다양한 데이터 담는 구조체
        {
            public int hp;
            public int attack;
        }

        struct Monster
        {
            public int hp;
            public int attack;
        }

        static ClassType ChooseClass() //코드를 분리해서 관리 할 수 있게 됨 -> 직업 고르기
        {
            ClassType choice = ClassType.None;
            Console.WriteLine("직업을 선택하세요");
            Console.WriteLine("[1] 기사");
            Console.WriteLine("[2] 궁수");
            Console.WriteLine("[3] 법사");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    choice = ClassType.Knight;
                    break;
                case "2":
                    choice = ClassType.Archer;
                    break;
                case "3":
                    choice = ClassType.Mage;
                    break;
            }
            return choice; //반환한 값을 외부에서도 사용 하능하게 된다.
        }
        static void CreatePlayer(ClassType choice, out Player player)
        {
            switch (choice)
            {
                case ClassType.Knight:
                    player.hp = 100;
                    player.attack = 10;
                    break;
                case ClassType.Archer:
                    player.hp = 75;
                    player.attack = 12;
                    break;
                case ClassType.Mage:
                    player.hp = 50;
                    player.attack = 15;
                    break;
                default: //이외의 경우도 정의해줘야한다.
                    player.hp = 0;
                    player.attack = 0;
                    break;
            }
        }

        static void CreateRandomMonster(out Monster monster)
        {
            Random rand = new Random();
            int randMonster = rand.Next(1, 4);
            switch (randMonster)
            {
                case (int)MonsterType.Slime:
                    Console.WriteLine("슬라임이 스폰되었습니다!");
                    monster.hp = 20;
                    monster.attack = 2;
                    break;
                case (int)MonsterType.Orc:
                    Console.WriteLine("오크가 스폰되었습니다!");
                    monster.hp = 30;
                    monster.attack = 3;
                    break;
                case (int)MonsterType.Skeleton:
                    Console.WriteLine("스켈레톤이 스폰되었습니다!");
                    monster.hp = 40;
                    monster.attack = 4;
                    break;
                default:
                    monster.hp = 0;
                    monster.attack = 0;
                    break;
            }
        }

        static void Fight(ref Player player, ref Monster monster)
        {
            while (true)
            {
                monster.hp -= player.attack;
                if (monster.hp <= 0)
                {
                    Console.WriteLine("승리했습니다!");
                    Console.WriteLine($"남은 체력: {player.hp}");
                    break;
                }

                player.hp -= monster.attack;
                if (player.hp <= 0)
                {
                    Console.WriteLine("패배했습니다!");
                    break;
                }
            }
        }
        static void EnterField(ref Player player)
        {
            while (true)
            {
                Console.WriteLine("필드에 접속했습니다.");
                Monster monster;
                CreateRandomMonster(out monster);

                Console.WriteLine("[1] 전투 모드로 돌입 ");
                Console.WriteLine("[2] 일정 확률로 마을로 도망 ");

                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        Fight(ref player, ref monster);
                        break;
                    case "2":
                        Random rand = new Random();
                        int randVal = rand.Next(0, 101);
                        if (randVal <= 33)
                        {
                            Console.WriteLine("도망치는데 성공했습니다!");
                            break;
                        }
                        else
                        {
                            Fight(ref player, ref monster);
                        }
                        break;
                    default:
                        break;
                }
            }

        }
        static void EnterGame(ref Player player)
        {
            Console.WriteLine("게임에 접속했습니다.");
            Console.WriteLine("[1] 필드로 가기");
            Console.WriteLine("[2] 로비로 돌아가기");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    EnterField(ref player);
                    break;
                case "2":
                    return;
            }
        }

        static void Main(string[] args)
        {
            while (true)
            {
                ClassType choice = ChooseClass();
                if (choice != ClassType.None)
                {
                    //캐릭터 생성 -> 2개 반환
                    Player player; //캐릭터 생성 구조체
                    CreatePlayer(choice, out player);
                    EnterGame(ref player);
                }
            }
        }
    }
}
