using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everest.PuzzleGame
{
    // An AVL tree node 
    public class Node
    {
        public int key, height;
        public Node left, right;
        public int size; // size of the tree rooted with this Node 
    };

    public class AVLTree
    {
        private Node m_Root = null;

        private int GetNodeHeight(Node node) => node == null ? 0 : node.height;

        private int GetNodeSize(Node node) => node == null ? 0 : node.size;

        private Node CreateNode(int key)
        {
            var node = new Node();
            node.key = key;
            node.left = node.right = null;
            node.height = node.size = 1;
            return node;
        }

        private Node RotateRight(Node node)
        {
            Node x = node.left;
            Node T2 = x.right;

            // Perform rotation 
            x.right = node;
            node.left = T2;

            // Update heights 
            node.height = Math.Max(GetNodeHeight(node.left), GetNodeHeight(node.right)) + 1;
            x.height = Math.Max(GetNodeHeight(x.left), GetNodeHeight(x.right)) + 1;

            // Update sizes 
            node.size = GetNodeSize(node.left) + GetNodeSize(node.right) + 1;
            x.size = GetNodeSize(x.left) + GetNodeSize(x.right) + 1;

            // Return new root 
            return x;
        }

        private Node RotateLeft(Node x)
        {
            Node y = x.right;
            Node T2 = y.left;

            // Perform rotation 
            y.left = x;
            x.right = T2;

            //  Update heights 
            x.height = Math.Max(GetNodeHeight(x.left), GetNodeHeight(x.right)) + 1;
            y.height = Math.Max(GetNodeHeight(y.left), GetNodeHeight(y.right)) + 1;

            // Update sizes 
            x.size = GetNodeSize(x.left) + GetNodeSize(x.right) + 1;
            y.size = GetNodeSize(y.left) + GetNodeSize(y.right) + 1;

            // Return new root 
            return y;
        }

        // Get Balance factor of Node N 
        private int GetBalanceFactor(Node N)
        {
            if (N == null)
                return 0;
            return GetNodeHeight(N.left) - GetNodeHeight(N.right);
        }

        public Node Insert(Node node, int key, out int result)
        {
            if (node == null)
            {
                result = 0;
                return CreateNode(key);
            }

            if (key < node.key)
            {
                node.left = Insert(node.left, key, out result);
                result = result + GetNodeSize(node.right) + 1;
            }
            else
            {
                node.right = Insert(node.right, key, out result);
            }

            /* 2. Update height and size of this ancestor node */
            node.height = Math.Max(GetNodeHeight(node.left),
                               GetNodeHeight(node.right)) + 1;
            node.size = GetNodeSize(node.left) + GetNodeSize(node.right) + 1;

            /* 3. Get the balance factor of this ancestor node to 
                  check whether this node became unbalanced */
            int balance = GetBalanceFactor(node);

            // If this node becomes unbalanced, then there are 
            // 4 cases 

            // Left Left Case 
            if (balance > 1 && key < node.left.key)
                return RotateRight(node);

            // Right Right Case 
            if (balance < -1 && key > node.right.key)
                return RotateLeft(node);

            // Left Right Case 
            if (balance > 1 && key > node.left.key)
            {
                node.left = RotateLeft(node.left);
                return RotateRight(node);
            }

            // Right Left Case 
            if (balance < -1 && key < node.right.key)
            {
                node.right = RotateRight(node.right);
                return RotateLeft(node);
            }

            /* return the (unchanged) node pointer */
            return node;
        }

        public Node GetInversionCount(Node node, int key, out int result)
        {
            if (key == node.key)
            {
                result = 0;
                return node;
            }

            if (key < node.key)
            {
                node.left = GetInversionCount(node.left, key, out result);

                // UPDATE COUNT OF GREATE ELEMENTS FOR KEY 
                result = result + GetNodeSize(node.right) + 1;
            }
            else
            {
                node.right = Insert(node.right, key, out result);
            }

            return node;
        }

        public Node Find(int key, Node root)
        {
            if (key < root.key)
            {
                if (key == root.key)
                {
                    return root;
                }
                else
                    return Find(key, root.left);
            }
            else
            {
                if (key == root.key)
                {
                    return root;
                }
                else
                    return Find(key, root.right);
            }

        }

    }
}
