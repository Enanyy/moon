using System;
using System.Collections.Generic;
using UnityEngine;
public interface ITreeNode
{
    string name { get; set; }
    Rect rect { get; set; }
    List<ITreeNode> children { get; }
    void OnDraw(ref Rect rect);
    bool ConnectableTo(ITreeNode node);
    void OnConnect(ITreeNode node);
    void OnDisconnect(ITreeNode node);
    ITreeNode Clone(ITreeNode node);
    Action<ITreeNode> OnAddChild { get; set; }
    Color GetConnectionColor();
}

