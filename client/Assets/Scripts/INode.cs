
using UnityEngine;

public interface INode
{
    string name { get; set; }
    Rect rect { get; set; }
    void Draw(ref Rect rect);
    bool LinkAble(INode node);
    void OnLink(INode node);
    void OnUnLink(INode node);
    INode Clone(INode node);
    Color GetColor();
}