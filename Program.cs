using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using static System.Console;

public interface I_BST<TK,TV> where  TK:IComparable {
    int Count { get; }
    IDictionary<TK,TV> Items { get; }
    IEnumerable<TV> this[TK min, TK max] { get; }
    bool Remove(TK key);
    TV this[TK k] { get; set; }
    bool Contains(TK key);
    void printInorder();
    void printPreorder();
}

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


class BinSearchTree<TK, TV> : I_BST<TK,TV> where TK:IComparable {
    protected Node<TK, TV>? _root;
    protected int count;

    public BinSearchTree() {
        this._root = null;
        this.count = 0;
    }
    
    public int Count => this.count;
    
    public IDictionary<TK,TV> Items {
        get {
            SortedDictionary<TK, TV> dict = new SortedDictionary<TK, TV>();
            void help(Node<TK, TV>? n) {
                if (n == null)
                    return;
                dict.Add(n.Key, n.Val);
                help(n.Left);
                help(n.Right);
            }
            help(this._root);
            return dict;
        }
    }
    
    public IEnumerable<TV> this[TK min, TK max] {
        get {
            var list = new List<TV>();

            void help(Node<TK, TV>? n) {
                if (n == null) {
                    return;
                }
                if (n.Key.CompareTo(min) >= 0 && n.Key.CompareTo(max) <= 0) {
                    list.Add(n.Val);
                    help(n.Left);
                    help(n.Right);
                }
                else if (n.Key.CompareTo(min) < 0) {
                    help(n.Right);
                }
                else if (n.Key.CompareTo(max) > 0) {
                    help(n.Left);
                }
            }
            help(this._root);
            return list;
        }
    }
    
    private Node<TK,TV> add_help(Node<TK, TV>? n, TK key, TV val) {
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
            this.count--;
        }
        return n;
    }
    
    private Node<TK, TV>? remove_help(Node<TK, TV>? n, TK key) {
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
                Node<TK, TV>? temp_node = n.Right;
                if (temp_node == null)
                {
                    throw new Exception("Null node where it shouldn't be");
                }
                while (temp_node.Left != null)
                    temp_node = temp_node.Left;
                TK temp_key = temp_node.Key;
                TV temp_val = temp_node.Val;
                n.Right = remove_help(n.Right, temp_node.Key);
                n.Key = temp_key;
                n.Val = temp_val;
            }
        }
        return n;
    }

    public virtual bool Remove(TK key) {
        this._root = this.remove_help(this._root, key);
        return true;
    }
    
    protected TV Get(TK key) {
        var n = _root;
        while (n != null) {
            if (key.CompareTo(n.Key) == 0)
                return n.Val;
            if (key.CompareTo(n.Key) < 0)
                n = n.Left;
            else
                n = n.Right;
        }
        throw new System.Exception("Trying to get value of key which does not exist");
    }

    public virtual TV this[TK k] {
        set {
            this._root = add_help(this._root, k, value);
            this.count++;
        }
        get { return this.Get(k); }
    }
    
    public bool Contains(TK key) {
        var n = _root;
        while (n != null) {
            if (key.CompareTo(n.Key) == 0)
                return true;
            if (key.CompareTo(n.Key) < 0)
                n = n.Left;
            else
                n = n.Right;
        }
        return false;
    }
    
    public void printInorder() {
        void help(Node<TK, TV>? n) {
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
        void help(Node<TK, TV>? n) {
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
}

class AVL_TREE<TK, TV> : BinSearchTree<TK, TV> , I_BST<TK,TV> where TK:IComparable
{
    public AVL_TREE() : base() { }
    
    private int get_max_height(Node<TK, TV>? a, Node<TK, TV>? b) {
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
            if (a == null || b == null)
            {
                throw new Exception("Null nodes where they shouldn't be");
            }
            return Math.Max(a.height, b.height);
        }
    }

    private Node<TK, TV> RightRotate(Node<TK, TV> a) {
        var new_r = a.Left;
        if (new_r == null) {
            throw new Exception("Null in right rotate");
        }
        a.Left = new_r.Right;
        new_r.Right = a;

        new_r.Right.height = get_max_height(new_r.Right.Left, new_r.Right.Right) + 1;
        new_r.height = get_max_height(new_r.Left, new_r.Right) + 1;
        return new_r;
    }
    
    private Node<TK, TV> LeftRotate(Node<TK, TV> a) {
        var new_r = a.Right;
        if (new_r == null) {
            throw new Exception("Null in left rotate");
        }
        a.Right = new_r.Left;
        new_r.Left = a;
        
        new_r.Left.height = get_max_height(new_r.Left.Left, new_r.Left.Right) + 1;
        new_r.height = get_max_height(new_r.Left, new_r.Right) + 1;
        return new_r;
    }

    private int CalcBalance(Node<TK, TV>? n) {
        if (n == null) {
            return 0;
        }
        int l = n.Left == null ? 0 : n.Left.height;
        int r = n.Right == null ? 0 : n.Right.height;
        return l - r;
    }

    private Node<TK,TV> add_help(Node<TK, TV>? n, TK key, TV val) {
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
            this.count--;
        }
        
        n.height = get_max_height(n.Left, n.Right) + 1;
        int balanceFactor = CalcBalance(n);

        if (balanceFactor > 1 && key.CompareTo(n.Left!.Key) < 0) {
            return RightRotate(n);
        }
        if (balanceFactor < -1 && key.CompareTo(n.Right!.Key) > 0) {
            return LeftRotate(n);
        }
        if (balanceFactor > 1 && key.CompareTo(n.Left!.Key) > 0) {
            n.Left = LeftRotate(n.Left);
            return RightRotate(n);
        }
        if (balanceFactor < -1 && key.CompareTo(n.Right!.Key) < 0) {
            n.Right = RightRotate(n.Right);
            return LeftRotate(n);
        }
        return n;
    }

    private Node<TK, TV>? remove_help(Node<TK, TV>? n, TK key) {
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
        else {
            if (n.Left == null && n.Right == null)
                n = null;
            else if (n.Left == null && n.Right != null)
                n = n.Right;
            else if (n.Left != null && n.Right == null)
                n = n.Left;
            else {
                if (n.Right == null) {
                    throw new Exception("Null where it should not be");
                }
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
            n.Left = LeftRotate(n.Left!);
            return RightRotate(n);
        }
        if (balanceFactor < -1 && CalcBalance(n.Right) <= 0) {
            return LeftRotate(n);
        }
        
        if (balanceFactor < -1 && CalcBalance(n.Right) > 0) {
            n.Right = RightRotate(n.Right!);
            return LeftRotate(n);
        }
        return n;
    }

    public override bool Remove(TK key) {
        this._root = this.remove_help(this._root, key);
        this.count--;
        return true;
    }
    
    public override TV this[TK k] {
        set {
            this._root = add_help(this._root, k, value);
            this.count++;
        }
        get { return this.Get(k); }
    }
}



class Program {
    static void Main() {
        experiment_1();
        experiment_11();
        experiment_2();
        experiment_3();
        test_1();
        test_2();
        test_5();
        test_6();
        test_7();
        test_8();
    }
    
    // Inserting 5_000 elements into the 3 trees
    static void experiment_1() {
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>();
        SortedDictionary<int, string> dict = new SortedDictionary<int, string>();
        BinSearchTree<int, string> bin_tree = new BinSearchTree<int, string>();
        DateTime start = DateTime.Now;
        for (int i = 0; i < 5_000; i++) {
            tree[i] = "hello";
        }
        DateTime end = DateTime.Now;
        WriteLine($"Time to insert 5,000 ascending order elements " +
                  $"into my AVL tree is {(end - start).TotalMilliseconds} milliseconds");
        
        start = DateTime.Now;
        for (int i = 0; i < 5_000; i++) {
            dict.Add(i, "hello");
        }
        end = DateTime.Now;
        WriteLine($"Time to insert 5,000 ascending order" +
                  $" elements into c# SortedDictionary is {(end - start).TotalMilliseconds} milliseconds");
        
        start = DateTime.Now;
        for (int i = 0; i < 5_000; i++) {
            bin_tree[i] = "hello";
        }
        end = DateTime.Now;
        WriteLine($"Time to insert 5,000 ascending order" +
                  $" elements into Simple BST is {(end - start).TotalMilliseconds} milliseconds");
    }
    
    // inserting 10_000 to 100_000, elements into AVL tree and C# SortedDictionary
    // to see the trend of growth
    static void experiment_11()
    {
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>();
        SortedDictionary<int, string> dict = new SortedDictionary<int, string>();
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
        for (int n = 10_000; n <= 100_000; n = n + 10_000)
        {
            tree = new AVL_TREE<int, string>();
            dict = new SortedDictionary<int, string>();
            
            start = DateTime.Now;
            for (int i = 0; i < n; i++) {
                tree[i] = "hello";
            }
            end = DateTime.Now;
            WriteLine($"Time to insert {n} ascending order elements " +
                      $"into my AVL tree is {(end - start).TotalMilliseconds} milliseconds");
        
            start = DateTime.Now;
            for (int i = 0; i < n; i++) {
                dict.Add(i, "hello");
            }
            end = DateTime.Now;
            WriteLine($"Time to insert {n} ascending order" +
                      $" elements into c# SortedDictionary is {(end - start).TotalMilliseconds} milliseconds");
        }
        
    }
    
    // Inserting 100_000 elements
    static void experiment_2() {
        BinSearchTree<int, string> bin_tree = new BinSearchTree<int, string>();
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>();
        SortedDictionary<int, string> dict = new SortedDictionary<int, string>();
        Random r = new Random(200);
        DateTime start = DateTime.Now;
        for (int i = 0; i < 100_000; i++) {
            tree[r.Next(100_000)] = "hello";
        }
        DateTime end = DateTime.Now;
        WriteLine($"Time to insert 100,000 elements in random order" +
                  $" into my AVL tree is {(end - start).TotalMilliseconds} milliseconds");
        
        start = DateTime.Now;
        for (int i = 0; i < 100_000; i++) {
            int num = r.Next(100_000);
            try {
                dict.Add(num, "hello");
            }
            catch (ArgumentException) {
                continue;
            }
        }
        end = DateTime.Now;
        WriteLine($"Time to insert 100,000 elements in random order" +
                  $" into c# SortedDictionary is {(end - start).TotalMilliseconds} milliseconds");
        
        start = DateTime.Now;
        for (int i = 0; i < 100_000; i++) {
            bin_tree[r.Next(100_000)] = "hello";
        }
        end = DateTime.Now;
        WriteLine($"Time to insert 100,000 in random order" +
                  $" elements into Simple BST is {(end - start).TotalMilliseconds} milliseconds");
    }

    static void experiment_3() {
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>();
        SortedDictionary<int, string> dict = new SortedDictionary<int, string>();
        BinSearchTree<int, string> bin_tree = new BinSearchTree<int, string>();
        
        List<int> ls = new List<int>();     // list of elements to add
        for (int i = 0; i < 100_000; i++) {
            ls.Add(i);
        }
        Random r = new Random(999);
        var shuffled = ls.OrderBy(_ => r.Next()).ToList();  // shuffle the list
        foreach (var x in shuffled) {     // add elements to each tree
            bin_tree[x] = "hello";
            tree[x] = "hello";
            dict.Add(x, "hello");
        }
        
        r = new Random(211);                    // Get elements to Get
        List<int> vals_to_get = new List<int>();
        for (int i = 0; i < 100_000; i++) {
            vals_to_get.Add(r.Next(100_000));
        }
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
        for (int n = 10_000; n <= 100_000; n = n + 10_000)
        {
            start = DateTime.Now;
            for (int i = 0; i < n; i++) {
                var y = tree[vals_to_get[i]];
            }
            end = DateTime.Now;
            WriteLine($"Time to get {n} random elements out of 100,000 element from" +
                      $" my AVL tree is {(end - start).TotalMilliseconds} milliseconds");
        
            start = DateTime.Now;
            for (int i = 0; i < n; i++) {
                var y = bin_tree[vals_to_get[i]];
            }
            end = DateTime.Now;
            WriteLine($"Time to get {n} random elements out of 100,000 element from" +
                      $" Simple BST is {(end - start).TotalMilliseconds} milliseconds");
        
            start = DateTime.Now;
            for (int i = 0; i < n; i++) {
                var y = dict[vals_to_get[i]];
            }
            end = DateTime.Now;
            WriteLine($"Time to get {n} random elements out of 100,000 element from" +
                      $" C# SortedDict is {(end - start).TotalMilliseconds} milliseconds");
        }
    }
    
    // recursive bst deletion
    static void test_1() {
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>();
        tree[50] = "hello";
        tree[30] = "hello";
        tree[70] = "hello";
        tree[20] = "hello";
        tree[40] = "hello";
        tree[60] = "hello";
        tree[80] = "hello";
        tree[65] = "hello";
        
        tree.printInorder();
        tree.Remove(30);
        tree.printPreorder();
    }
    // General test for Add, Remove with 50 random elements
    static void test_2() {
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>();
        Random r = new Random(200);
        for (int i = 0; i < 50; i++) {
            int num = r.Next(1000);
            tree[num] = "hello";
        }
        IDictionary<int, string> dict = tree.Items;
        WriteLine($"dict contains {dict.Count} items, tree contains {tree.Count}");
        WriteLine($"{dict.ContainsKey(122)}"); //True
        WriteLine($"{dict.ContainsKey(-100)}"); //False
        WriteLine(tree.Count);
        tree[122] = "hi";
        WriteLine(tree.Count);
        tree.printInorder();
        tree.Remove(22);
        tree.Remove(122);
        tree.Remove(246);
        tree.Remove(720);
        tree.Remove(934);
        WriteLine(tree.Count);
        tree.printInorder();
        
        tree = new AVL_TREE<int, string>();
        dict = tree.Items;
        WriteLine($"dict contains {dict.Count} items");
        WriteLine($"{dict.ContainsKey(122)}");
        WriteLine($"{dict.ContainsKey(-100)}");
    }
    
    // Right rotate test
    static void test_3() {
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>();
        tree[50] = "hello";
        tree[30] = "hello";
        tree[70] = "hello";
        tree[20] = "hello";
        tree[40] = "hello";
        tree[15] = "hello";
        tree[25] = "hello";
        tree.printPreorder();
        // tree._root = tree.RightRotate(tree._root);
        tree.printPreorder();
    }
    
    // Left rotate test
    static void test_4() {
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>();
        tree[50] = "hello";
        tree[30] = "hello";
        tree[70] = "hello";
        tree[60] = "hello";
        tree[80] = "hello";
        tree[75] = "hello";
        tree[85] = "hello";
        tree.printPreorder();
        // tree._root = tree.LeftRotate(tree._root);
        tree.printPreorder();
    }
    
    // Add with balancing testing
    static void test_5() {
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>(); // accending order
        tree[20] = "hello";
        tree[30] = "hello";
        tree[40] = "hello";
        tree[50] = "hello";
        tree[60] = "hello";
        tree[70] = "hello";
        tree[80] = "hello";
        tree[65] = "hello";
        
        tree.printInorder();
        tree.printPreorder();
        WriteLine(" ");
        
        
        tree = new AVL_TREE<int, string>();     // deccending order
        tree[80] = "hello";
        tree[70] = "hello";
        tree[60] = "hello";
        tree[50] = "hello";
        tree[40] = "hello";
        tree[30] = "hello";
        tree[20] = "hello";
        tree[65] = "hello";
        
        tree.printInorder();
        tree.printPreorder();
        WriteLine(" ");
        
        // testing double rotations
        tree = new AVL_TREE<int, string>();
        tree[50] = "hello";
        tree[30] = "hello";
        tree[40] = "hello";
        tree.printInorder();
        tree.printPreorder();
        WriteLine(" ");
        
        tree[35] = "hello";
        tree[32] = "hello";
        tree.printInorder();
        tree.printPreorder();
    }
    
    // Tests removing with rotations
    static void test_6() {    // adding 200 elements, deleting a bunch, testing that height <= 1.44 * log2(n)
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>();
        Random r = new Random(200);
        for (int i = 0; i < 200; i++) {
            int num = r.Next(1000);
            tree[num] = "hello";
        }
        tree.printInorder();
        // WriteLine(tree._root.height);   // _root field should be made public!!!
        int[] to_remove = {1, 120, 122, 507, 655, 663, 721, 722, 735, 59, 44, 110, 761, 19, 891, 900, 
            901, 914, 920, 966, 995, 946, 295, 547, 792, 720};
        foreach (var i in to_remove) {
            tree.Remove(i);
        }
        tree.printInorder();
        // WriteLine(tree._root.height);
    }
    
    // general test, Contains, Remove
    static void test_7() {
        AVL_TREE<int, string> tree = new AVL_TREE<int, string>();
        tree[3] = "hi";
        tree[-100] = "bye";
        WriteLine(tree.Contains(3));    //true
        WriteLine(tree.Contains(20));   //false
        WriteLine(tree.Contains(-100));     //true
        tree[50] = "hello";
        WriteLine(tree[-100]);
        tree[-100] = "new";
        WriteLine(tree[-100]);
        WriteLine(tree[3]);
        tree.Remove(50);
        try {
            WriteLine(tree[50]); // should result in an exception!!!
        }
        catch (Exception) {
            WriteLine("all good");
        }
        
        // WriteLine(tree[70]);
        tree.printInorder();
    }

    // Tests the indexer which returns an IEnumerable set of values between min and max
    static void test_8() {
        AVL_TREE<int, int> tree = new AVL_TREE<int, int>();
        Random r = new Random(200);
        for (int i = 0; i < 50; i++) {
            int num = r.Next(1000);
            tree[num] = num;
        }
        tree.printInorder();
        IEnumerable<int> set = tree[0, 500];
        foreach (var x in set) {
            Write($"{x} ");
        }
        WriteLine();
    }
}