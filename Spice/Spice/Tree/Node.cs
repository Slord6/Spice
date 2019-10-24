using System;
using System.Collections.Generic;
using System.Linq;
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
            StringBuilder builder = new StringBuilder();

            builder.Append(value.ToString());

            if (children.Count > 0)
            {
                builder.Append(Environment.NewLine);
                builder.Append("With children: ");
                builder.Append(String.Join(", ", children.Select(c => c.ToString())));
            }

            return builder.ToString();
        }
    }
}
