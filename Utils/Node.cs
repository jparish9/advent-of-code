namespace AOC.Utils;

// base generic node class, don't instantiate directly (use Node if no other information needed)
public abstract class BaseNode<T> where T : BaseNode<T>
{
    public List<Edge<T>> Edges = [];

    public void AddEdge(T to, int weight=1)
    {
        Edges.Add(new Edge<T> { From = (T)this, To = to, Weight = weight });
    }
}

// edge class whose type will follow base or any derived node type
public class Edge<T> where T : BaseNode<T>
{
    public required T From { get; set; }
    public required T To { get; set; }
    public int Weight { get; set; }
}


// base usable Node class
public class Node : BaseNode<Node>
{
}

// named node
public class NamedNode : BaseNode<NamedNode>
{
    public required string Name { get; set; }
}