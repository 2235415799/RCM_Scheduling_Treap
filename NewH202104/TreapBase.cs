using Huangbo.AStarPetri;
using System;

namespace NewH202104
{
    public class ListNode
    {
        public AStarNode aStarNode;
        public ListNode pre;
        public ListNode next;
        public ListNode preInF = null;
        public ListNode nextInF = null;
        public ListNode(AStarNode aStarNode=null, ListNode pre = null, ListNode next = null)
        {
            this.aStarNode = aStarNode;
            this.pre = pre;
            this.next = next;
        }
    }
    public class TempList
    {
        public ListNode root;   
        public TempList(AStarNode aStarNode)
        {
            root = new ListNode();
            root.next = new ListNode(aStarNode, root, null);
        }

        internal void ClearList()
        {
            ListNode current = root.next;
            ListNode temp;
            while (current != null)
            {
                temp = current;
                current = current.next;
                temp.aStarNode = null;
                temp = null;
            }
            this.root = null;
        }//清理队列

        internal int Count()//计算队列节点数量
        {
            if(root == null)
                return 0;
            ListNode current = root.next;
            int count = 0;
            while (current != null)
            {
                count++;
                current = current.next;
            }
            return count;
        }

        internal decimal getMinTotalCost()//获取队列节点中最小的f值
        {
            if (isEmpty())
                return 1000000000;
            else
                return root.next.aStarNode.fValue;
        }

        internal bool isEmpty()
        {
            return root.next == null;
        }

        //首位出栈
        internal AStarNode popFront()
        {
            if(isEmpty())
                return null;
            else
            {
                AStarNode result = root.next.aStarNode;
                root.next = root.next.next;
                if(root.next != null)
                    root.next.pre=root;
                return result;
            }
        }
    }
}