using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    Slim,
    Zombie,
    Skeleton,
    Bat
}

public class MonsterFactory
{

    public Monster CreateMonster(MonsterType type)
    {
        switch (type)
        {
            case MonsterType.Slim:
                return new Slim();
            case MonsterType.Zombie:
                return new Zombie();
            default:
                return null;
        }
    }
}
