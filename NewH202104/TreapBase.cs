using Huangbo.AStarPetri;
using System;

namespace NewH202104
{
    public class ListNode
    {
        public AStarNode aStarNode;
        public ListNode pre;
        public ListNode next;
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
        public TempList()
        {
            root = new ListNode();
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

        internal double getMinTotalCost()//获取队列节点中最小的f值
        {
            if (isEmpty())
                return 1000000000;
            else
                return (double)root.next.aStarNode.fValue;
        }

        internal bool isEmpty()
        {
            return root.next == null;
        }

        /*
         * false:插入失败，因为插入节点并不优于原有节点
         * true:插入成功
         * **/
        internal bool newElement(AStarNode aStarNode)
        {
            ListNode current = root.next,
                fPos=root;
            bool replaced = false;
            while (current != null)
            {
                if (current.aStarNode.fValue < aStarNode.fValue)
                    fPos = current;
                if (current.aStarNode.IsSameStateM_R(aStarNode) && replaced == false)//寻找相同的mr
                {
                    int cmp=Compare(current.aStarNode, aStarNode);
                    if (cmp == -1)
                    {
                        return false;
                    }
                    else if (cmp == 1)
                    {
                        if(fPos == current)
                            fPos = current.pre;
                        current.pre.next=current.next;
                        if(current.pre.next != null)
                            current.pre.next.pre = current.pre;
                        current = current.pre;
                        replaced = true;
                    }
                }
                current=current.next;
            }
            fPos.next=new ListNode(aStarNode,fPos,fPos.next);
            if(fPos.next.next != null)
                fPos.next.next.pre = fPos.next;
            return true;
        }//插入或替换成更优节点

        private int Compare(AStarNode aStarNode1, AStarNode aStarNode2)
        {
            if (aStarNode1.IsSameStateM_R(aStarNode2))
            {
                if(aStarNode1.fValue <= aStarNode2.fValue)
                    return -1;
                else
                    return 1;
            }
            return 0;
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