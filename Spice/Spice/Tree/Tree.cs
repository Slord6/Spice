using System;
using System.Collections.Generic;
using System.Text;

namespace Spice.Tree
{
    class Tree<T>
    {
        private Node<T> root;

        public Tree()
        {
            root = null;
        }

        public Node<T> Root
        {
            get
            {
                return root;
            }
            set
            {
                root = value;
            }
        }
    }
}
