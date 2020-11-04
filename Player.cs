using System;
using System.Collections.Generic;
using System.Text;

namespace csharp1
{
    enum PlayerType
    {
        None = 0,
        Knight = 1,
        Archer = 2,
        Mage = 3
    }
    class Player : Creature
    {
        protected PlayerType type = PlayerType.None;
        protected Player(PlayerType type) : base(CreatureType.Player)
        {
            this.type = type;
        } //인자가 없는 경우 사용할 수 없도록 만들어 줌, protected로 설정하여 자체로 생성할 수 없음

        public PlayerType GetPlayerType() { return type; }
    }

    class Knight : Player
    {
        public Knight() : base(PlayerType.Knight) //부모 클래스의 생성자 호출
        {
            SetInfo(100, 10);
        }
    }

    class Mage: Player
    {
        public Mage() : base(PlayerType.Mage)
        {
            SetInfo(75, 12);
        }
    }

    class Archer : Player
    {
        public Archer() : base(PlayerType.Archer)
        {
            SetInfo(50, 15);
        }
    }
}
