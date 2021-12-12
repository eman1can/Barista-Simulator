using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class VertexNode<T> {
    public T Value;
    public VertexNode<T> Next = null;
    public VertexNode<T> Last = null;

    public VertexNode(T value) {
        Value = value;
    }

    public VertexNode<T> First() {
        if (Last == null)
            return this;
        return Last._First(Value);
    }

    private VertexNode<T> _First(T root) {
        if (Value.Equals(root))
            return this;
        if (Last == null)
            return this;
        return Last._First(Value);
    }
}
