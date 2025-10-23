using Huangbo.AStarPetri;
using System;
using System.Collections.Generic;

namespace NewH202104
{
    public class FMap
    {
        public Dictionary<decimal, ListNode> dictionary; //存储每个f值对应的最后出现的状态（我们优先扩展这个）
        
        public FMap()
        {
            dictionary = new Dictionary<decimal, ListNode>();
        }

        public void InsertToF(ListNode S)
        {
            if (dictionary.ContainsKey(S.aStarNode.fValue))
            {
                S.nextInF = dictionary[S.aStarNode.fValue];
                dictionary[S.aStarNode.fValue].preInF = S;
                dictionary[S.aStarNode.fValue] = S;
            }
            else
            {
                dictionary.Add(S.aStarNode.fValue, S);
            }
        }

        public void DeleteFromF(ListNode S)
        {
            if (dictionary[S.aStarNode.fValue] == S)
            {
                if (S.nextInF != null)
                {
                    S.nextInF.preInF = null;
                    dictionary[S.aStarNode.fValue] = S.nextInF;
                }
                else
                    dictionary.Remove(S.aStarNode.fValue);
            }
            else
            {
                if(S.nextInF != null)
                {
                    S.nextInF.preInF = S.preInF;
                }
                S.preInF.nextInF = S.nextInF;
            }
        }

        // 添加索引器以支持 dictionary[key] 语法
        public ListNode this[decimal key]
        {
            get { return dictionary[key]; }
            set { dictionary[key] = value; }
        }

    }

    public class TreapNode
    {
        public TempList aStarNodes;//一个包含A*状态的队列
        public Array key;//某个A*状态队列的共有M
        public TreapNode left, right, parent;
        public decimal priority;//优先级，设定为节点中的第一个f值

        //新建树节点
        public TreapNode(AStarNode aStarNode,TreapNode left = null, TreapNode right = null, TreapNode parent = null)
        {
            this.left = left;
            this.right = right;
            this.parent = parent;
            aStarNodes=new TempList(aStarNode);
            key = aStarNode.M;
            priority=getMinTotalCost();
        }

        //获取队列中最小f
        public decimal getMinTotalCost()
        {
            return aStarNodes.getMinTotalCost();
        }

        

        //队列节点数量
        public int CountContainNumber()
        {
            return aStarNodes.Count();
        }
    }

    public class Treap
    {
        int UP=0, DOWN=1;
        public TreapNode root;//树堆根节点
        public FMap fMap;//每个f值对应的最后出现的状态

        public Treap()
        {
            root = null;
            fMap = new FMap();
        }

        public bool IsEmpty()
        {
            return root == null;
        }

        public void Clear()
        {
            ClearNode(root);
            root = null;
        }


        public int CountTotalStates()
        {
            return CountTotalStatesInNode(root);
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
            }
        }
        
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

        internal void Insert(AStarNode S)
        {
            if (IsEmpty())
            {
                root = new TreapNode(S);
                fMap.InsertToF(root.aStarNodes.root.next);//程序无关行，更新firstMap
            }
            else
            {
                (int caseCode, TreapNode node, ListNode state) = Search(S);
                if(caseCode >= 0)
                {
                    if(caseCode == 1)
                    {
                        if (state.aStarNode.fValue <= S.fValue)
                            return;
                        else
                        {
                            fMap.DeleteFromF(state);
                            //N' = N' \ {S'}
                            state.pre.next = state.next;
                            if (state.next != null)
                                state.next.pre = state.pre;
                        }
                    }
                    //Add S into N′ in nondecreasing order of f -values;
                    ListNode stateExist, insertedState;
                    if (node.aStarNodes.root.next == null)
                    {
                        insertedState = new ListNode(S, node.aStarNodes.root);
                        node.aStarNodes.root.next = insertedState;
                    }
                    else
                    {
                        stateExist = node.aStarNodes.root.next;
                        while (stateExist.next != null)
                        {
                            if (stateExist.aStarNode.fValue < S.fValue)
                                stateExist = stateExist.next;
                            else
                                break;
                        }
                        if (stateExist.aStarNode.fValue < S.fValue)
                        {
                            insertedState = new ListNode(S, stateExist, stateExist.next);
                            if (stateExist.next != null)
                                stateExist.next.pre = insertedState;
                            stateExist.next = insertedState;
                        }
                        else
                        {
                            insertedState = new ListNode(S, stateExist.pre, stateExist);
                            stateExist.pre.next = insertedState;
                            stateExist.pre = insertedState;
                        }
                    }
                    node.priority = node.getMinTotalCost();
                    fMap.InsertToF(insertedState);//更新FMap
                }
                else
                {
                    if (orderCompare(S.M, node.key) == -1)//N.k<N_F.k  (S.M<node.k)
                    {
                        node.left = new TreapNode(S, null, null, node);
                        node = node.left;
                    }
                    else  //   N.k>N_F.k   (S.M>node.k)
                    {
                        node.right = new TreapNode(S, null, null, node);
                        node = node.right;
                    }
                    fMap.InsertToF(node.aStarNodes.root.next);  //更新FMap
                }
                UpdateTreap(node, UP);//从node开始自下而上调整Treap
            }
        }

        /*
         * direction控制调整方向，node表明开始节点
         * **/
        private void UpdateTreap(TreapNode node, int direction)
        {
            if (direction==UP)
            {
                while (node.parent != null && node.parent.priority > node.priority)
                {
                    if (node.parent.left == node)
                        RightRotate(node);
                    else
                        LeftRotate(node);
                }
            }
            else//DOWN
            {
                while (node.left != null || node.right != null)
                {
                    if (node.left != null && node.right != null)//存在两个子节点
                    {
                        if (node.right.priority < node.left.priority && node.right.priority < node.priority)
                            LeftRotate(node.right);//左旋
                        else if (node.left.priority <= node.right.priority && node.left.priority < node.priority)
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
        }

        private void LeftRotate(TreapNode node)
        {
            TreapNode nTemp = node.parent, nPP = nTemp.parent, nL = node.left;

            nTemp.right = nL;
            if (nL != null)
                nL.parent = nTemp;
            
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

        /*
         * 取出Treap中具有最小f值的状态S
         * **/
        public AStarNode Pop()
        {
            AStarNode S = fMap[root.priority].aStarNode;//获取当前Treap中具有最小f值的状态S,相同f值优先取最深的节点。
            Delete(S);//移除S并调整Treap
            return S;
        }

        /*
         * 移除node中的S并更新移除后的Treap
         * **/
        public void Delete(AStarNode S)
        {
            TreapNode node = Search(S).node;
            //搜索N中的S
            ListNode stateExist= node.aStarNodes.root.next;
            while (stateExist.aStarNode != S)
            {
                stateExist = stateExist.next;
            }

            //删除N中S
            fMap.DeleteFromF(stateExist);  //更新fMap
            stateExist.pre.next = stateExist.next;
            if(stateExist.next!=null)
                stateExist.next.pre = stateExist.pre;

            if (!node.aStarNodes.isEmpty())
            {
                node.priority=node.getMinTotalCost();
                UpdateTreap(node, DOWN);
            }
            else
            {
                if (node.left == null && node.right == null)
                {
                    if (node.parent == null)
                        root = null;
                    else
                    {
                        if (node.parent.left == node)
                            node.parent.left = null;
                        else
                            node.parent.right = null;
                    }
                }
                else if(node.left != null && node.right == null)
                {
                    if(node.parent == null){
                        root = node.left;
                        node.left.parent = null;
                    }
                    else
                    {
                        node.left.parent = node.parent;
                        if (node.parent.left == node)
                            node.parent.left = node.left;
                        else
                            node.parent.right = node.left;
                    }
                }else if(node.left == null&& node.right != null)
                {
                    if(node.parent == null){
                        root = node.right;
                        node.right.parent = null;
                    }
                    else
                    {
                        node.right.parent = node.parent;
                        if (node.parent.left == node)
                            node.parent.left = node.right;
                        else
                            node.parent.right = node.right;
                    }
                }
                else
                {
                    //寻找左子树的最右节点
                    TreapNode rightmostNode = node.left;
                    while (rightmostNode.right != null)
                    {
                        rightmostNode = rightmostNode.right;
                    }

                    if (rightmostNode.parent.right != rightmostNode)//最右节点为左子树根结点
                    {
                        rightmostNode.right=node.right;
                        if(node.right!=null)
                            node.right.parent = rightmostNode;
                    }
                    else
                    {
                        rightmostNode.parent.right = rightmostNode.left;
                        if (rightmostNode.left != null)
                            rightmostNode.left.parent = rightmostNode.parent;

                        rightmostNode.left = node.left;
                        if (node.right != null)
                            node.right.parent = rightmostNode;

                        rightmostNode.right = node.right;
                        if (node.left != null)
                            node.left.parent = rightmostNode;
                    }
                    rightmostNode.parent = node.parent;
                    if (node.parent != null)
                    {
                        if (node.parent.left == node)
                            node.parent.left = rightmostNode;
                        else
                            node.parent.right = rightmostNode;
                    }
                    else
                    {
                        // 如果是根节点
                        root = rightmostNode;
                    }
                    UpdateTreap(rightmostNode, DOWN);
                }
            }
        }

        internal (int caseCode, TreapNode node,ListNode state) Search(AStarNode S)
        {
            if (IsEmpty())
                return (-1, null, null);
            TreapNode nodeExist = root;
            int compare = orderCompare(nodeExist.key, S.M);
            while (nodeExist != null)
            {
                if(compare == 0)  //N.k == S.M
                {
                    //if there exists S' in N such that S'.R=S.R
                    ListNode stateExist = nodeExist.aStarNodes.root.next;
                    while (stateExist != null)
                    {
                        if (stateExist.aStarNode.IsSameStateM_R(S))//寻找具有相同的mr的S
                        {
                            return (1, nodeExist, stateExist);
                        }
                        stateExist = stateExist.next;
                    }
                    //else
                    return (0, nodeExist, null);
                }
                else if(compare == -1)  //N.k < S.M
                {
                    if (nodeExist.right != null)
                    {
                        nodeExist = nodeExist.right;
                        compare = orderCompare(nodeExist.key, S.M);
                    }
                    else
                        return (-1, nodeExist, null);
                }
                else //compare = 1  N.k > S.M
                {
                    if (nodeExist.left != null)
                    {
                        nodeExist = nodeExist.left;
                        compare = orderCompare(nodeExist.key, S.M);
                    }
                    else
                        return (-1, nodeExist, null);
                }
            }
            return (-1, null, null);
        }
    }
}
