using static System.Console;

class Node<TK, TV> {
    public TK Key;
    public TV Val;
    public Node<TK, TV>? Left;
    public Node<TK, TV>? Right;
    public int height;

    public Node(TK key, TV val) {
        this.Key = key;
        this.Val = val;
        this.Left = null;
        this.Right = null;
        this.height = 1;
    }
}

class AVL_TREE<TK, TV> where TK:IComparable
{
    public Node<TK, TV>? _root;

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
            Write($"{n.Key}:{n.height} ");
            help(n.Right);
        }
        help(this._root);
        WriteLine();
    }
    
    public void printPreorder() {
        void help(Node<TK, TV> n) {
            if (n == null) {
                return;
            }
            Write($"{n.Key}:{n.height} ");
            help(n.Left);
            help(n.Right);
        }
        help(this._root);
        WriteLine();
    }
    
    private static int get_max_height(Node<TK, TV>? a, Node<TK, TV>? b) {
        if (a == null && b == null) {
            return 0;
        }
        else if (a == null && b != null) {
            return b.height;
        }
        else if (b == null && a != null) {
            return a.height;
        }
        else {
            return Math.Max(a.height, b.height);
        }
    }

    public static Node<TK, TV> RightRotate(Node<TK, TV> a)
    {
        var new_r = a.Left;
        a.Left = new_r.Right;
        new_r.Right = a;

        new_r.Right.height = get_max_height(new_r.Right.Left, new_r.Right.Right) + 1;
        new_r.height = get_max_height(new_r.Left, new_r.Right) + 1;
        return new_r;
    }
    
    public static Node<TK, TV> LeftRotate(Node<TK, TV> a)
    {
        var new_r = a.Right;
        a.Right = new_r.Left;
        new_r.Left = a;
        
        new_r.Left.height = get_max_height(new_r.Left.Left, new_r.Left.Right) + 1;
        new_r.height = get_max_height(new_r.Left, new_r.Right) + 1;
        return new_r;
    }

    private static int CalcBalance(Node<TK, TV> n)
    {
        if (n == null)
        {
            return 0;
        }
        int l = n.Left == null ? 0 : n.Left.height;
        int r = n.Right == null ? 0 : n.Right.height;
        return l - r;
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
        
        n.height = get_max_height(n.Left, n.Right) + 1;
        int balanceFactor = CalcBalance(n);

        if (balanceFactor > 1 && key.CompareTo(n.Left.Key) < 0) {
            return RightRotate(n);
        }
        if (balanceFactor < -1 && key.CompareTo(n.Right.Key) > 0) {
            return LeftRotate(n);
        }
        if (balanceFactor > 1 && key.CompareTo(n.Left.Key) > 0) {
            n.Left = LeftRotate(n.Left);
            return RightRotate(n);
        }
        if (balanceFactor < -1 && key.CompareTo(n.Right.Key) < 0) {
            n.Right = RightRotate(n.Right);
            return LeftRotate(n);
        }
        return n;
    }

    public void Add(TK key, TV val) {
        this._root = add_help(this._root, key, val);
    }

    private Node<TK, TV> remove_help(Node<TK, TV> n, TK key) {
        if (n == null) {
            WriteLine(key);
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
                n = null;
            else if (n.Left == null && n.Right != null)
                n = n.Right;
            else if (n.Left != null && n.Right == null)
                n = n.Left;
            else
            {
                Node<TK, TV> temp_node = n.Right;
                while (temp_node.Left != null)
                    temp_node = temp_node.Left;
                TK temp_key = temp_node.Key;
                TV temp_val = temp_node.Val;
                n.Right = remove_help(n.Right, temp_node.Key);
                n.Key = temp_key;
                n.Val = temp_val;
            }
        }

        if (n == null) {
            return n;
        }
        else {
            n.height = get_max_height(n.Left, n.Right) + 1;
        }
        int balanceFactor = CalcBalance(n);

        if (balanceFactor > 1 && CalcBalance(n.Left) >= 0) {
            return RightRotate(n);
        }
        if (balanceFactor > 1 && CalcBalance(n.Left) < 0) {
            n.Left = LeftRotate(n.Left);
            return RightRotate(n);
        }
        if (balanceFactor < -1 && CalcBalance(n.Right) <= 0) {
            return LeftRotate(n);
        }
        
        if (balanceFactor < -1 && CalcBalance(n.Right) > 0) {
            n.Right = RightRotate(n.Right);
            return LeftRotate(n);
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
        test_6();
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
        // tree.Remove(30);
        tree.printPreorder();
    }
    // General test for Add, Remove with 50 random elements
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
    
    // Right rotate test
    static void test_3()
    {
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>();
        tree.Add(50, "hello");
        tree.Add(30, "hello");
        tree.Add(70, "hello");
        tree.Add(20, "hello");
        tree.Add(40, "hello");
        tree.Add(15, "hello");
        tree.Add(25, "hello");
        tree.printPreorder();
        // tree._root = tree.RightRotate(tree._root);
        tree.printPreorder();
    }
    
    // Left rotate test
    static void test_4() {
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>();
        tree.Add(50, "hello");
        tree.Add(30, "hello");
        tree.Add(70, "hello");
        tree.Add(60, "hello");
        tree.Add(80, "hello");
        tree.Add(75, "hello");
        tree.Add(85, "hello");
        tree.printPreorder();
        // tree._root = tree.LeftRotate(tree._root);
        tree.printPreorder();
    }
    
    // Add with balancing testing
    static void test_5()
    {
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>();
        tree.Add(20, "hello");
        tree.Add(30, "hello");
        tree.Add(40, "hello");
        tree.Add(50, "hello");
        tree.Add(60, "hello");
        tree.Add(70, "hello");
        tree.Add(80, "hello");
        tree.Add(65, "hello");
        
        tree.printInorder();
        tree.printPreorder();
        WriteLine(" ");
        
        
        tree = new AVL_TREE<int, string>();
        tree.Add(80, "hello");
        tree.Add(70, "hello");
        tree.Add(60, "hello");
        tree.Add(50, "hello");
        tree.Add(40, "hello");
        tree.Add(30, "hello");
        tree.Add(20, "hello");
        tree.Add(65, "hello");
        
        tree.printInorder();
        tree.printPreorder();
        WriteLine(" ");
        
        
        tree = new AVL_TREE<int, string>();
        tree.Add(50, "hello");
        tree.Add(30, "hello");
        tree.Add(40, "hello");
        tree.printInorder();
        tree.printPreorder();
        WriteLine(" ");
        
        tree.Add(35, "hello");
        tree.Add(32, "hello");
        tree.printInorder();
        tree.printPreorder();
    }

    static void test_6()
    {
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>();
        Random r = new Random(200);
        for (int i = 0; i < 200; i++)
        {
            int num = r.Next(1000);
            tree.Add(num, "hello");
        }
        tree.printInorder();
        WriteLine(tree._root.height);
        int[] to_remove = {1, 120, 122, 507, 655, 663, 721, 722, 735, 59, 44, 110, 761, 19, 891, 900, 
            901, 914, 920, 966, 995, 946, 295, 547, 792, 720};
        foreach (var i in to_remove)
        {
            tree.Remove(i);
        }
        tree.printInorder();
        WriteLine(tree._root.height);
    }
}