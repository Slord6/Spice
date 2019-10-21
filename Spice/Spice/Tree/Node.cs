using System;
using System.Collections.Generic;
using System.Text;

namespace Spice.Tree
{
    class Node<T>
    {
        private T value;
        private List<Node<T>> children;

        public Node(T value)
        {
            this.value = value;
            this.children = new List<Node<T>>();
        }

        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        public List<Node<T>> Children
        {
            get
            {
                return children;
            }
        }

        public override string ToString()
        {
            return "Node: " + value.ToString();
        }
    }
}
