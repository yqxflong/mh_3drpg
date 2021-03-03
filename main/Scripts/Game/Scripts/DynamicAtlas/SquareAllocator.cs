using System.Collections.Generic;

public class SquareAllocator
{
    public class Node
    {
        public int   X = 0;
        public int   Y = 0;
        public int   Size = 0;

        public Node   Parent = null;
        public Node[] Children = new Node[4];
        
        bool mMergeable = true;
        public bool Mergeable
        {
            get
            {
                return mMergeable;
            }

            set
            {
                if (mMergeable != value)
                {
                    mMergeable = value;

                    if (Parent != null)
                    {
                        // Recursive
                        Parent.Mergeable = Parent.CalculateMergeability();
                    }
                }
            }
        }
        
        public Node()
        {

        }

        public void Set(int x, int y, int size, Node parent)
        {
            X = x;
            Y = y;
            Size = size;
            Parent = parent;
        }

        public bool Split()
        {
            if (Size <= 1)
            {
                return false;
            }

            int size = Size >> 1;

            if (Children[0] == null) Children[0] = new Node();
            if (Children[1] == null) Children[1] = new Node();
            if (Children[2] == null) Children[2] = new Node();
            if (Children[3] == null) Children[3] = new Node();

            Children[0].Set(X,        Y,        size, this);
            Children[1].Set(X + size, Y,        size, this);
            Children[2].Set(X + size, Y + size, size, this);
            Children[3].Set(X,        Y + size, size, this);

            return true;
        }

        bool CalculateMergeability()
        {
            return
                (Children[0] == null || Children[0].mMergeable) &&
                (Children[1] == null || Children[1].mMergeable) &&
                (Children[2] == null || Children[2].mMergeable) &&
                (Children[3] == null || Children[3].mMergeable);
        }
    }

    Node       mRoot = null;
    List<Node> mFreeList = new List<Node>();
    int        mSize = 0;

    public SquareAllocator(int size)
    {
        mSize = Floor(size);
        mRoot = new Node();
        mRoot.Set(0, 0, mSize, null);
        mFreeList.Add(mRoot);
    }

    public Node Allocate(int w, int h)
    {
        int size = CalculateSquareSize(w, h);

        EB.Debug.Log("[SquareAllocator]Allocate: size ={0}", size.ToString());

        Node node = FindFreeNode(size);
        if (node != null)
        {
            mFreeList.Remove(node);

            bool split = false;
            while (node.Size > size)
            {
                if (node.Split())
                {
                    split = true;

                    mFreeList.Add(node.Children[1]);
                    mFreeList.Add(node.Children[2]);
                    mFreeList.Add(node.Children[3]);

                    node = node.Children[0];
                }
                else
                {
                    break;
                }
            }

            if (split)
            {
                mFreeList.Sort(FreeListCompare);
            }

            node.Mergeable = false;
        }
        return node;
    }

    public void Free(Node node)
    {
        if (node != null)
        {
            node.Mergeable = true;

            while (node.Parent != null && node.Parent.Mergeable)
            {
                node = node.Parent;

                for (int i = 0; i < 4; ++i)
                {
                    mFreeList.Remove(node.Children[i]);
                }
            }

            mFreeList.Add(node);
            mFreeList.Sort(FreeListCompare);
        }
    }

    public int GetSize()
    {
        return mSize;
    }

    static int FreeListCompare(Node a, Node b)
    {
        int result = a.Size - b.Size;
        if (result == 0)
        {
            result = a.X - b.X;
            if (result == 0)
            {
                result = a.Y - b.Y;
            }
        }
        return result;
    }

    Node FindFreeNode(int size)
    {
        List<Node>.Enumerator iterator = mFreeList.GetEnumerator();
        while (iterator.MoveNext())
        {
            Node current = iterator.Current;
            if (current.Size >= size)
            {
                return current;
            }
        }
        return null;
    }

    int CalculateSquareSize(int w, int h)
    {
        int size = System.Math.Max(w, h);
        return Ceil(size);
    }

    public static int Ceil(int val)
    {
        int length = 1;
        while (length < val)
        {
            length = length << 1;
        }
        return length;
    }

    public static int Floor(int val)
    {
        int length = 1;
        while((length << 1) <= val)
        {
            length = length << 1;
        }
        return length;
    }
}