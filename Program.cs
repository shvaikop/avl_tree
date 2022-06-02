using static System.Console;

class Node<TK, TV> {
    public TK Key;
    public TV Val;
    public Node<TK, TV>? Left;
    public Node<TK, TV>? Right;

    public Node(TK key, TV val) {
        this.Key = key;
        this.Val = val;
        this.Left = null;
        this.Right = null;
    }
}

class AVL_TREE<TK, TV> where TK:IComparable
{
    private Node<TK, TV>? _root;

    public AVL_TREE() {
        this._root = null;
    }

    public void printInorder()
    {
        void help(Node<TK, TV> n)
        {
            if (n == null) {
                return;
            }
            help(n.Left);
            Write(n.Key + " ");
            help(n.Right);
        }
        help(this._root);
        WriteLine();
    }

    private static Node<TK,TV> add_help(Node<TK, TV> n, TK key, TV val) {
        if (n == null) {
            return new Node<TK, TV>(key, val);
        }

        if (key.CompareTo(n.Key) < 0) {
            n.Left = add_help(n.Left, key, val);
        }
        else if (key.CompareTo(n.Key) > 0) {
            n.Right = add_help(n.Right, key, val);
        }
        else {
            n.Val = val;
        }
        return n;
    }

    public void Add(TK key, TV val) {
        this._root = add_help(this._root, key, val);
    }
    private void Replace(Node<TK,TV> parent, Node<TK,TV> n, Node<TK,TV> novy)
    {
        if (parent == null)
            this._root = novy;
        else if (parent.Left == n)
            parent.Left = novy;
        else if (parent.Right == n)
            parent.Right = novy;
        else
            throw new System.Exception("Not a child");
    }

    private Node<TK, TV> remove_help(Node<TK, TV> n, TK key) {
        if (n == null) {
            throw new System.Exception("Key to be deleted does not exist");
        }
        if (key.CompareTo(n.Key) < 0) {
            n.Left = this.remove_help(n.Left, key);
        }
        else if (key.CompareTo(n.Key) > 0) {
            n.Right = this.remove_help(n.Right, key);
        }
        else
        {
            if (n.Left == null && n.Right == null)
                return null;
            else if (n.Left == null && n.Right != null)
                return n.Right;
            else if (n.Left != null && n.Right == null)
                return n.Left;
            else
            {
                Node<TK, TV> temp_node = n.Right;
                while (temp_node.Left != null)
                    temp_node = temp_node.Left;
                TK temp_key = temp_node.Key;
                TV temp_val = temp_node.Val;
                this.Remove(temp_node.Key);
                n.Key = temp_key;
                n.Val = temp_val;
            }
        }
        return n;
    }

    public bool Remove(TK key)
    {
        // first call contains to see if key exists
        this._root = this.remove_help(this._root, key);
        return true;
    }
}

class Program
{
    static void Main()
    {
        // test_1();
        
    }
    
    // recursive bst deletion
    static void test_1() {
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>();
        tree.Add(50, "hello");
        tree.Add(30, "hello");
        tree.Add(70, "hello");
        tree.Add(20, "hello");
        tree.Add(40, "hello");
        tree.Add(60, "hello");
        tree.Add(80, "hello");
        tree.Add(65, "hello");
        
        tree.printInorder();
        tree.Remove(30);
        tree.printInorder();
    }

    static void test_2() {
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>();
        Random r = new Random(200);
        for (int i = 0; i < 50; i++)
        {
            int num = r.Next(1000);
            tree.Add(num, "hello");
        }
        tree.printInorder();
        tree.Remove(22);
        tree.Remove(122);
        tree.Remove(246);
        tree.Remove(720);
        tree.Remove(934);
        tree.printInorder();
    }
}