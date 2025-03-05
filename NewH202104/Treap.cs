using Huangbo.AStarPetri;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tanis.Collections;

namespace NewH202104
{
    public class TreapNode
    {
        public TempList aStarNodes;//一个包含A*状态的队列
        public Array key;//某个A*状态队列的共有M
        public TreapNode left, right, parent;
        public double priority;//优先级，设定为节点中的第一个f值

        //新建树节点
        public TreapNode(AStarNode aStarNode,TreapNode left = null, TreapNode right = null, TreapNode parent = null)
        {
            this.left = left;
            this.right = right;
            this.parent = parent;
            aStarNodes=new TempList();
            aStarNodes.newElement(aStarNode);
            key = aStarNode.M;
            priority=getMinTotalCost();
        }

        //获取队列中最小f
        public double getMinTotalCost()
        {
            return aStarNodes.getMinTotalCost();
        }

        //清除对应树节点的队列
        void Clear()
        {
            aStarNodes.ClearList();
        }

        //队列节点数量
        public int CountContainNumber()
        {
            return aStarNodes.Count();
        }
    }

    public class Treap
    {
        public TreapNode root;//树堆根节点
        
        public bool IsEmpty()
        {
            return root == null;
        }

        //新增树堆节点
        //若节点为空，新增A*节点生成的队列作为树堆的根节点
        //若节点不为空，插入合适位置
        public void NewElement(AStarNode aStarNode)
        {
            if (IsEmpty())
            {
                root = new TreapNode(aStarNode);
            }
            else
            {
                TreapNode treapNode = NewElement(root, aStarNode);
                if (treapNode != null)
                    adjust(treapNode);
            }
        }

        //新增树堆节点
        //若新节点的M小于比对节点的M，与比对节点的左子节点进行比较
        //若新节点的M大于比对节点的M，与比对节点的右子结点进行比较
        //若新节点的M等于比对节点的M，则插入比对节点的队列中
        private TreapNode NewElement( TreapNode currentNode, AStarNode aStarNode)
        {
            int compare = orderCompare(currentNode.key, aStarNode.M);
            if (compare == 1)
            {
                if (currentNode.left == null)
                    return LockNewElement(currentNode, aStarNode);
                else 
                    return NewElement(currentNode.left, aStarNode);
            }
            else if (compare == -1)
            {
                if (currentNode.right == null)
                    return LockNewElement(currentNode, aStarNode);
                else
                    return NewElement(currentNode.right, aStarNode);
            }
            else
            {
                return LockNewElement(currentNode, aStarNode);
            }
        }

        //寻找合适的树堆节点位置，并进入该节点将A*节点插入到队列中的合适位置
        private TreapNode LockNewElement(TreapNode currentNode,AStarNode aStarNode)
        {
            int compare = orderCompare(currentNode.key, aStarNode.M);
            if (compare > 0)
            {
                if (currentNode.left == null)
                {
                    currentNode.left = new TreapNode(aStarNode,null,null,currentNode);
                    return currentNode.left;
                }
                else
                {
                    return LockNewElement( currentNode.left,  aStarNode);
                }
            }
            else if (compare < 0)
            {
                if (currentNode.right == null)
                {
                    currentNode.right = new TreapNode(aStarNode,null,null,currentNode);
                    return currentNode.right;
                }
                else
                {
                    return LockNewElement(currentNode.right, aStarNode);
                }
            }
            else
            {
                if (currentNode.aStarNodes.newElement(aStarNode))
                {
                    currentNode.priority=currentNode.getMinTotalCost();
                    return currentNode;
                }
                else
                {
                    return null;
                }
            }
        }

        //
        public AStarNode PopMinByPriority()
        {
            if (root == null)
                return null;
            AStarNode result = root.aStarNodes.popFront();
            if(!root.aStarNodes.isEmpty()){
                root.priority = root.getMinTotalCost();
                DownAdjust(root);
                return result;
            }
            if (root.left == null && root.right == null)
            {
                root = null;
            }
            else if (root.left == null && root.right != null)
            {
                root = root.right;
                root.parent = null;
            }
            else if(root.left != null && root.right == null)
            {
                root = root.left;
                root.parent = null;
            }
            else {
                /*
                 * 先找大M
                TreapNode replace = root.right;
                while (replace.left != null) {
                    replace=replace.left;
                }
                if (replace.parent.left == replace)
                {
                    replace.parent.left = replace.right;
                    if (replace.right != null)
                        replace.right.parent = replace.parent;
                }
                else
                {
                    replace.parent.right = replace.right;
                    if (replace.right != null)
                        replace.right.parent = replace.parent;
                }
                replace.left = root.left;
                replace.right = root.right;
                root.left.parent = replace;
                if (root.right != null)
                    root.right.parent = replace;
                root = replace;
                root.parent = null;
                */
                //先找小M
                TreapNode replace = root.left;
                while (replace.right != null)
                {
                    replace = replace.right;
                }
                if (replace.parent.right == replace)
                {
                    replace.parent.right = replace.left;
                    if (replace.left != null)
                        replace.left.parent = replace.parent;
                }
                else
                {
                    replace.parent.left = replace.left;
                    if (replace.left != null)
                        replace.left.parent = replace.parent;
                }
                replace.left = root.left;
                replace.right = root.right;
                root.right.parent = replace;
                if (root.left != null)
                    root.left.parent = replace;
                root = replace;
                root.parent = null;
                DownAdjust(root);
            }
            return result;
        }

        void adjust(TreapNode node)
        {
            UpAdjfust( node);
        }

        public void Clear()
        {
            ClearNode(root);
            root = null;
        }

        public int CountTreapNode()
        {
            return CountNode(root);
        }

        public int CountTotalStates()
        {
            return CountTotalStatesInNode(root);
        }

        //计算节点node（包含本身）所有下属树堆节点数量
        private int CountNode(TreapNode node)
        {
            if (node == null)
                return 0;
            else
                return 1+CountNode(node.left)+CountNode(node.right);
        }

        //计算节点node（包含本身）所有下属树堆节点中包含的A*节点数量
        private int CountTotalStatesInNode(TreapNode node=null)
        {
            if(node == null)
                return 0;
            int count = node.CountContainNumber(),
                countL = CountTotalStatesInNode(node.left),
                countR = CountTotalStatesInNode(node.right);
            return count+countL+countR;
        }

        private void ClearNode( TreapNode node)
        {
            if (node == null)
                return;
            else
            {
                ClearNode(node.left);
                ClearNode(node.right);
                node.aStarNodes.ClearList();
                node = null;
            }
        }

        //存疑3
        private void UpAdjfust( TreapNode node)
        {
            if(node.parent== null)
                return;
            if (node.parent.priority <= node.priority)
                return;
            Shift( node, node.parent.right == node);
            UpAdjfust(node);
        }

        private void Shift( TreapNode node, bool leftShift)
        {
            if (leftShift)//左旋
            {
                TreapNode nP = node.parent, nPP = nP.parent, nL = node.left;
                node.parent = nPP;
                if (nPP != null)
                {
                    if (nPP.right == nP)
                        nPP.right = node;
                    else
                        nPP.left = node;
                }
                node.left = nP;
                nP.parent = node;
                nP.right = nL;
                if (nL != null)
                    nL.parent = nP;

                if (node.parent == null)
                    root = node;
            }
            else//右旋
            {
                TreapNode nP = node.parent, nPP = nP.parent, nR = node.right;
                node.parent = nPP;
                if (nPP != null)
                {
                    if (nPP.right == nP)
                        nPP.right = node;
                    else
                        nPP.left = node;
                }
                node.right = nP;
                nP.parent = node;
                nP.left = nR;
                if (nR != null)
                {
                    nR.parent = nP;
                }
                if (node.parent == null)
                    root = node;
            }
        }

        private void DownAdjust( TreapNode root)
        {
            while (root.left != null||root.right!=null) {
                if (root.left != null && root.right != null)
                {
                    if (root.right.priority < root.left.priority)
                        if (root.right.priority < root.priority)
                            Shift(root.right, true);//左旋
                        else break;
                    else
                        if (root.left.priority <= root.priority)
                            Shift(root.left, false);//右旋
                        else break;
                }
                else if (root.left != null && root.right == null)
                {
                    if(root.priority>=root.left.priority)
                        Shift( root.left, false);
                    else break;
                }
                else
                {
                    if (root.priority > root.right.priority)
                        Shift(root.right, true);
                    else break;
                }
            }
        }

        /*向量过长可能会溢出
         * int orderCompare(Array arr1, Array arr2)// -1:<  0:=  1:>
        {
            for (int i = 0; i < arr1.Length; i++)
            {
                if ((int)arr1.GetValue(i) < (int)arr2.GetValue(i))
                    return -1;
                else if ((int)arr1.GetValue(i) > (int)arr2.GetValue(i))
                    return 1;
            }
            return 0;
        }
        */

        /*
         * 按照首位逐个比较
         */
        int orderCompare(Array arr1, Array arr2)// -1:<  0:=  1:>
        {
            for (int i = 0; i < arr1.Length; i++)
            {
                if ((int)arr1.GetValue(i) < (int)arr2.GetValue(i))
                    return -1;
                else if ((int)arr1.GetValue(i) > (int)arr2.GetValue(i))
                    return 1;
            }
            return 0;
        }

        public int Compare(AStarNode aStarNode1, AStarNode aStarNode2)
        {
            if (aStarNode1.fValue <= aStarNode2.fValue)
                return -1;
            else
                return 1;
        }
        //删除  没找到:-1  找到但是不删:0  成功删除:1
        public int delNode(AStarNode node)
        {
            if(IsEmpty())
            { return -1; }
            else
            {
                int cmp=orderCompare(root.key, node.M);
                TreapNode tempTN = root;
                while (cmp != 0)
                {
                    if (cmp == 1)
                    {
                        if (tempTN.left == null)
                            return -1;
                        else
                            tempTN = tempTN.left;
                    }
                    if (cmp == -1)
                    {
                        if (tempTN.right == null)
                            return -1;
                        else
                            tempTN = tempTN.right;
                    }
                    cmp=orderCompare(tempTN.key, node.M);
                }
                TempList tempList = tempTN.aStarNodes;
                ListNode current = tempList.root.next;
                //bool replaced = false;
                while (current != null)
                {
                    if (current.aStarNode.IsSameStateM_R(node))//寻找相同的mr
                    {
                        int cmp2 = Compare(current.aStarNode, node);
                        if (cmp2 == -1)
                        {
                            return 0;
                        }
                        if (cmp2 == 1)
                        {
                            current.pre.next = current.next;
                            if (current.pre.next != null)
                                current.pre.next.pre = current.pre;
                            /*
                            temp = current;
                            current = current.pre;
                            replaced = true;
                            */
                            tempTN.priority=tempTN.getMinTotalCost();
                            return 1;
                        }
                    }
                    current = current.next;
                    
                }
                /*
                fPos.next = new ListNode(node, fPos, fPos.next);
                if (fPos.next.next != null)
                {
                    fPos.next.next.pre = fPos.next;
                }
                */
                return 1;
            }
        }

        internal void InsertState(AStarNode S)
        {
            if (IsEmpty())
            {
                root = new TreapNode(S);
            }
            else
            {
                int compare = orderCompare(root.key, S.M);
                TreapNode Node = root;
                bool flag = true;
                while (compare != 0&&flag)
                {
                    switch (compare)
                    {
                        case 1:
                            if (Node.left != null)
                            {
                                Node = Node.left;
                                compare = orderCompare(Node.key, S.M);
                            }
                            else
                                flag = false;
                            break;
                        case -1:
                            if (Node.right != null)
                            {
                                Node = Node.right;
                                compare = orderCompare(Node.key, S.M);
                            }
                            else
                                flag = false;
                            break;
                        default:
                            break;
                    }
                }
                if (compare==0)//存在一个key=M的树堆节点
                {
                    ListNode current = Node.aStarNodes.root.next,
                    fPos = Node.aStarNodes.root;
                    bool replaced = false;
                    while (current != null&&!replaced)
                    {
                        if (current.aStarNode.fValue < S.fValue)
                            fPos = current;
                        if (current.aStarNode.IsSameStateM_R(S) && replaced == false)//寻找相同的mr
                        {
                            int cmp = Compare(current.aStarNode, S);
                            if (cmp == -1)//如果存在S'.R,M=S.R,M，但g(S')≤g(S)
                            {
                                return;
                            }
                            else //if (cmp == 1)  如果存在S'.R,M=S.R,M，且g(S')＞g(S)，删除S'
                            {
                                if (fPos == current)
                                    fPos = current.pre;
                                current.pre.next = current.next;
                                if (current.pre.next != null)
                                    current.pre.next.pre = current.pre;
                                current = current.pre;
                                replaced = true;
                            }
                        }
                        current = current.next;
                    }
                    //如果程序没有中途停止运行到这里，add S into treap
                    fPos.next = new ListNode(S, fPos, fPos.next);
                    if (fPos.next.next != null)
                        fPos.next.next.pre = fPos.next;
                    //到这里add完毕
                    Node.priority = Node.getMinTotalCost();
                    
                }
                else if (compare==1)//S.M<N.k
                {
                    Node.left = new TreapNode(S,null,null,Node);
                    Node=Node.left;
                }
                else//if (compare==-1)   S.M>N.k
                {
                    Node.right = new TreapNode(S,null,null,Node);
                    Node=Node.right;
                }
                UpdateTreap(Node, 1);//从Node开始自下而上调整Treap
                
            }
        }

        /*
         * direction控制调整方向，node表明开始节点
         * 1:BottomUp   0:UpDown
         * **/
        private void UpdateTreap(TreapNode node, int direction)
        {
            if (direction==0)//UpDown
            {
                while (node.left != null || node.right != null)
                {
                    if (node.left != null && node.right != null)//存在两个子节点
                    {
                        if (node.right.priority < node.left.priority)
                            if (node.right.priority < node.priority)
                                LeftRotate(node.right);//左旋
                            else break;
                        else
                            if (node.left.priority < node.priority)
                                RightRotate(node.left);//右旋
                            else break;
                    }
                    else if (node.left != null && node.right == null)//只有左子节点
                    {
                        if (node.left.priority < node.priority)
                            RightRotate(node.left);
                        else break;
                    }
                    else //只有右子节点
                    {
                        if (node.right.priority < node.priority)
                            LeftRotate(node.right);
                        else break;
                    }
                }
            }
            else//BottomUp
            {
                while (node.parent!=null&& node.parent.priority> node.priority)
                {
                    if(node.parent.right== node)
                        LeftRotate(node);
                    else
                        RightRotate(node);
                }
            }
        }          

        private void RightRotate(TreapNode node)
        {
            TreapNode nTemp = node.parent, nPP = nTemp.parent, nR = node.right;
            nTemp.left = nR;
            if (nR != null)
                nR.parent = nTemp;
            node.right = nTemp;
            nTemp.parent = node;
            if (nPP != null)
            {
                if (nPP.right == node.parent)
                    nPP.right = node;
                else
                    nPP.left = node;
                node.parent = nPP;
            }
            else
            {
                node.parent = null;
                root = node;
            }
        }

        private void LeftRotate(TreapNode node)
        {
            TreapNode nTemp = node.parent, nPP = nTemp.parent, nL = node.left;
            nTemp.right = nL;
            if (nL != null)
                nL.parent=nTemp;
            node.left = nTemp;
            nTemp.parent = node;
            if (nPP != null)
            {
                if (nPP.right == node.parent)
                    nPP.right = node;
                else
                    nPP.left = node;
                node.parent = nPP;
            }
            else
            { 
                node.parent = null;
                root = node; 
            }
        }

        /*
         * 取出Treap中具有最小f值的状态S
         * **/
        public AStarNode PopState()
        {
            AStarNode S = root.aStarNodes.root.next.aStarNode;//获取当前Treap中具有最小f值的状态S
            DeleteState(ref root, S);//移除S并调整Treap
            return S;
        }


        /*
         * 移除node中的S并更新移除后的Treap
         * **/
        public void DeleteState(ref TreapNode node, AStarNode s)
        { 
            ListNode listNode= node.aStarNodes.root.next;
            while (listNode.aStarNode != s)
            {
                listNode = listNode.next;
            }
            listNode.pre.next = listNode.next;//删除N中S
            if(listNode.next!=null)
                listNode.next.pre = listNode.pre;
            if (!node.aStarNodes.isEmpty())
            {
                node.priority=node.getMinTotalCost();
                UpdateTreap(node, 0);
            }
            else
            {
                if (node.left == null && node.right == null)
                {
                    node=null;
                }else if(node.left != null && node.right == null)
                {
                    node.left.parent = node.parent;
                    node = node.left;
                }else if(node.left == null&& node.right != null)
                {
                    node.right.parent = node.parent;
                    node = node.right;
                }else
                {
                    //寻找左子树的最右节点
                    TreapNode rightmostNode = node.left;
                    while (rightmostNode.right != null)
                    {
                        rightmostNode = rightmostNode.right;
                    }

                    if (rightmostNode.parent.right != rightmostNode)//最右节点为左子树根结点
                    {
                        rightmostNode.parent.left = rightmostNode.left;
                        if (rightmostNode.left != null)
                            rightmostNode.left.parent = rightmostNode.parent;
                    }
                    else
                    {
                        rightmostNode.parent.right = rightmostNode.left;
                        if (rightmostNode.left != null)
                            rightmostNode.left.parent = rightmostNode.parent;
                    }
                    rightmostNode.left = node.left;
                    rightmostNode.right = node.right;
                    node.right.parent = rightmostNode;
                    if (node.left != null)
                        node.left.parent = rightmostNode;
                    rightmostNode.parent = node.parent;
                    node = rightmostNode;
                    UpdateTreap(node,0);
                }
            }
        }

        internal (TreapNode TreapNode,AStarNode AStarNode) SearchState(AStarNode S)
        {
            if (IsEmpty())
            {
                return (null,null);
            }
            else
            {
                int compare = orderCompare(root.key, S.M);
                TreapNode Node = root;
                while (compare != 0)
                {
                    switch (compare)
                    {
                        case 1:
                            if (Node.left != null)
                            {
                                Node = Node.left;
                                compare = orderCompare(Node.key, S.M);
                            }
                            else
                                return (null, null);
                            break;
                        case -1:
                            if (Node.right != null)
                            {
                                Node = Node.right;
                                compare = orderCompare(Node.key, S.M);
                            }
                            else
                                return (null, null);
                            break;
                        default:
                            break;
                    }
                }
                ListNode current = Node.aStarNodes.root.next;
                while (current != null)
                {
                    if (current.aStarNode.IsSameStateM_R(S))//寻找具有相同的mr的S
                    {
                        return (Node,current.aStarNode);
                    }
                    current = current.next;
                }
                return (null, null);

            }
        }
    }
}
