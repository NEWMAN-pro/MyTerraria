using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type : byte
{
    Block = 0,
    Other = 1
}

public class Item
{
    // ��ƷID
    public byte ID;
    // ��Ʒ����
    public Type type;
    // ��Ʒ����
    public int count;

}
