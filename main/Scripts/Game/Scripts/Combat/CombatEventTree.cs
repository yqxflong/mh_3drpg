using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Combat
{
    public enum CombatEventTreeState
    {
        INVALID = -1,
        INITED,
        RUNNING,
        DONE
    }

    public class CombatEventTree
    {
        private static Dictionary<int, CombatEventTree> s_nodeCache = new Dictionary<int, CombatEventTree>(200);

        protected CombatEventTree m_parent = null;
        protected CombatEvent m_event = null;
        protected CombatEventTreeState m_treeState = CombatEventTreeState.INVALID;
        protected CombatEventTreeState m_nodeState = CombatEventTreeState.INVALID;
        protected Dictionary<eCombatEventTiming, List<CombatEventTree>> m_children = new Dictionary<eCombatEventTiming, List<CombatEventTree>>(CombatEventTimingComparer.Default);

        public CombatEventTree Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

        public CombatEvent Event
        {
            get { return m_event; }
            set { m_event = value; }
        }

        public CombatEventTreeState NodeState
        {
            get { return m_nodeState; }
            set { m_nodeState = value; }
        }

        public CombatEventTreeState TreeState
        {
            get { return m_treeState; }
            set { m_treeState = value; }
        }

        public CombatEventTree()
        {

        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj == this)
            {
                return true;
            }

            if (obj is CombatEventTree == false)
            {
                return false;
            }

            CombatEventTree cmp_tree = obj as CombatEventTree;
            return m_event.GetUniqueCode() == cmp_tree.Event.GetUniqueCode();
        }

        public override int GetHashCode()
        {
            return m_event.GetUniqueCode();
        }

        public bool Equals(CombatEventTree cmp_tree)
        {
            if (cmp_tree == null)
            {
                return false;
            }

            if (cmp_tree == this)
            {
                return true;
            }

            return m_event.GetUniqueCode() == cmp_tree.Event.GetUniqueCode();
        }

        public static CombatEventTree Parse(CombatEvent evt)
        {
            CombatEventTree root = new CombatEventTree();
            root.Event = evt;
            root.NodeState = CombatEventTreeState.INITED;
            root.TreeState = CombatEventTreeState.INITED;

            if (evt.Children == null)
            {
                return root;
            }

            // children are CombatEffectEvent
            for (int i = 0, cnt = evt.Children.Count; i < cnt; ++i)
            {
                CombatEffectEvent effect = evt.Children[i] as CombatEffectEvent;
                CombatEventTree sub_tree = Parse(effect);

                if (effect.Parent < 0)
                {
                    root.AddChild(sub_tree);
                }
                else
                {
                    CombatEvent parent_effect = evt.Children[effect.Parent];
                    CombatEventTree parent = root.FindInChildren(parent_effect);
                    parent.AddChild(sub_tree);
                }
            }

            return root;
        }

        public bool AddChild(CombatEventTree sub_tree)
        {
            if (sub_tree.Parent != null)
            {
                EB.Debug.LogError("AddChild: parent is set");
                return false;
            }

            eCombatEventTiming timing = sub_tree.Event.Timing;
            if (!HasChildren(timing))
            {
                m_children[timing] = new List<CombatEventTree>();
            }

            List<CombatEventTree> children = GetChildren(timing);
            if (ILRDefine.UNITY_EDITOR && children.Contains(sub_tree))
            {
                EB.Debug.LogError("AddChild: child exists");
                return false;
            }

            sub_tree.Parent = this;
            children.Add(sub_tree);

            if (ILRDefine.UNITY_EDITOR && s_nodeCache.ContainsKey(sub_tree.GetHashCode()))
            {
                EB.Debug.LogError("AddChild: cache exists");
            }

            s_nodeCache[sub_tree.GetHashCode()] = sub_tree;

            if (ILRDefine.UNITY_EDITOR && timing == eCombatEventTiming.AUTO && children.Count > 1)
            {
                EB.Debug.LogError("AddChild: auto children great than one");
            }

            return true;
        }

        public bool RemoveChild(CombatEventTree sub_tree)
        {
            if (sub_tree.Parent != this)
            {
                EB.Debug.LogError("RemoveChild: parent not match, this = {0}, target = {1}", Event.GetLogId(), sub_tree.Parent != null ? sub_tree.Parent.Event.GetLogId() : null);
                return false;
            }

            eCombatEventTiming timing = sub_tree.Event.Timing;
            List<CombatEventTree> children = GetChildren(timing);
            if (children == null)
            {
                EB.Debug.LogError("RemoveChild: timing not match, {0}", timing);
                return false;
            }

            if (!children.Remove(sub_tree))
            {
                EB.Debug.LogError("RemoveChild: child not found, {0}", sub_tree.Event.GetLogId());
                return false;
            }

            if (children.Count == 0 && !m_children.Remove(timing))
            {
                EB.Debug.LogError("RemoveChild: remove empty timing failed, {0}", timing);
                return false;
            }

            sub_tree.Parent = null;
            if (ILRDefine.UNITY_EDITOR && !s_nodeCache.ContainsKey(sub_tree.GetHashCode()))
            {
                EB.Debug.LogError("RemoveChild: cache not exists");
            }
            s_nodeCache.Remove(sub_tree.GetHashCode());

            return true;
        }

        public void RemoveAllChildren()
        {
            var iter = m_children.GetEnumerator();
            while (iter.MoveNext())
            {
                for (int i = 0; i < iter.Current.Value.Count; ++i)
                {
                    iter.Current.Value[i].Parent = null;
                    if (ILRDefine.UNITY_EDITOR && !s_nodeCache.ContainsKey(iter.Current.Value[i].GetHashCode()))
                    {
                        EB.Debug.LogError("RemoveAllChildren: cache not exists");
                    }
                    s_nodeCache.Remove(iter.Current.Value[i].GetHashCode());
                }
                //iter.Current.Value.Clear();
            }
            iter.Dispose();
            m_children.Clear();
        }

        public bool RemoveChildren(eCombatEventTiming timing)
        {
            if (m_children.ContainsKey(timing))
            {
                List<CombatEventTree> children = GetChildren(timing);

                int len = children.Count;
                for(int i = 0; i < len; i++)
                {
                    CombatEventTree child = children[i];
                    child.Parent = null;
                    if (ILRDefine.UNITY_EDITOR && !s_nodeCache.ContainsKey(child.GetHashCode()))
                    {
                        EB.Debug.LogError("RemoveChildren: cache not exists");
                    }
                    s_nodeCache.Remove(child.GetHashCode());
                }
                //children.Clear();
                return m_children.Remove(timing);
            }

            return true;
        }

        public bool IsChild(CombatEventTree sub_tree)
        {
            if (sub_tree.Parent != this)
            {
                return false;
            }

            eCombatEventTiming timing = sub_tree.Event.Timing;
            List<CombatEventTree> children = GetChildren(timing);
            if (children == null)
            {
                return false;
            }

            return children.Contains(sub_tree);
        }

        public bool IsAncestor(CombatEventTree ancestor_tree)
        {
            CombatEventTree iter = Parent;
            while (iter != null)
            {
                if (iter == ancestor_tree)
                {
                    return true;
                }

                iter = iter.Parent;
            }
            return false;
        }

        private CombatEventTree[] childrenBuffer = new CombatEventTree[0];
        public System.ArraySegment<CombatEventTree> GetChildren()
        {
            int bufferLength = childrenBuffer.Length;
            int len = 0;
            var iter = m_children.GetEnumerator();
            while (iter.MoveNext())
            {
                var pair = iter.Current;
                List<CombatEventTree> tree_list = pair.Value;
                for (int i = 0, cnt = tree_list.Count; i < cnt; ++i)
                {
                    CombatEventTree tree = tree_list[i];

                    // check buffer size
                    if (len == bufferLength)
                    {
                        var reallocLength = bufferLength > 0 ? bufferLength * 2 : 1;
                        var realloc = new CombatEventTree[reallocLength];
                        if (bufferLength > 0)
                        {
                            System.Array.Copy(childrenBuffer, realloc, bufferLength);
                        }
                        childrenBuffer = realloc;
                        bufferLength = reallocLength;
                    }
                    childrenBuffer[len++] = tree;
                }
            }
            iter.Dispose();

            return new System.ArraySegment<CombatEventTree>(childrenBuffer, 0, len);
        }

        public IEnumerator<CombatEventTree> GetChildrenNonAlloc()
        {
            var iter = m_children.GetEnumerator();
            while (iter.MoveNext())
            {
                var pair = iter.Current;
                List<CombatEventTree> tree_list = pair.Value;
                for (int i = 0, cnt = tree_list.Count; i < cnt; ++i)
                {
                    yield return tree_list[i];
                }
            }
            iter.Dispose();
        }

        public bool HasChildren(eCombatEventTiming timing)
        {
            return m_children.ContainsKey(timing);
        }

        public List<CombatEventTree> GetChildren(eCombatEventTiming timing)
        {
            if (m_children.ContainsKey(timing))
            {
                return m_children[timing];
            }

            return null;
        }

        public CombatEventTree FindInChildren(CombatEvent evt)
        {
            // speed up
            CombatEventTree found = null;
            if (s_nodeCache.TryGetValue(evt.GetHashCode(), out found))
            {
                if (found.IsAncestor(this))
                {
                    return found;
                }
            }
            EB.Debug.LogWarning("CombatEventTree.FindInChildren: cache miss");

            var iter = GetChildrenNonAlloc();
            while (iter.MoveNext())
            {
                CombatEventTree child = iter.Current;

                if (child.Event.Equals(evt))
                {
                    iter.Dispose();
                    return child;
                }

                found = child.FindInChildren(evt);
                if (found != null)
                {
                    iter.Dispose();
                    return found;
                }
            }
            iter.Dispose();

            return null;
        }

        private CombatEventTree[] parentBuffer = new CombatEventTree[0];
        public System.ArraySegment<CombatEventTree> GetParents()
        {
            int count = 0;
            int bufferLength = parentBuffer.Length;
            var iter = m_parent;
            while (iter != null)
            {
                if (count == bufferLength)
                {
                    int reallocLength = bufferLength > 0 ? bufferLength * 2 : 1;
                    var realloc = new CombatEventTree[reallocLength];
                    if (bufferLength > 0)
                    {
                        System.Array.Copy(parentBuffer, realloc, bufferLength);
                    }
                    parentBuffer = realloc;
                    bufferLength = reallocLength;
                }

                parentBuffer[count++] = iter;
                iter = iter.Parent;
            }

            return new System.ArraySegment<CombatEventTree>(parentBuffer, 0, count);
        }

        public IEnumerator<CombatEventTree> GetParentsNonAlloc()
        {
            var iter = m_parent;
            while (iter != null)
            {
                yield return iter;

                iter = iter.Parent;
            }
        }

        public CombatEventTree FindInParents(CombatEvent evt)
        {
            if (m_parent == null)
            {
                return null;
            }

            if (m_parent.Event.Equals(evt))
            {
                return m_parent;
            }

            return m_parent.FindInParents(evt);
        }

        public IEnumerable<CombatEventTree> GetSiblingsNonAlloc()
        {
            if (m_parent == null)
            {
                yield return null;
            }

            var iter = m_parent.GetChildrenNonAlloc();
            while (iter.MoveNext())
            {
                CombatEventTree sibling = iter.Current;

                if (sibling == this)
                {
                    continue;
                }

                yield return sibling;
            }
            iter.Dispose();
        }

        public CombatEventTree FindInSiblings(CombatEvent evt)
        {
            if (m_parent == null)
            {
                return null;
            }

            // speed up
            CombatEventTree found = null;
            if (s_nodeCache.TryGetValue(evt.GetHashCode(), out found))
            {
                if (found.IsAncestor(m_parent) && found != this)
                {
                    return found;
                }
            }
            EB.Debug.LogWarning("CombatEventTree.FindInSiblings: cache miss");

            var iter = m_parent.GetChildrenNonAlloc();
            while (iter.MoveNext())
            {
                CombatEventTree sibling = iter.Current;

                if (sibling == this)
                {
                    continue;
                }

                if (sibling.Event.Equals(evt))
                {
                    iter.Dispose();
                    return sibling;
                }
            }
            iter.Dispose();

            iter = m_parent.GetChildrenNonAlloc();
            while (iter.MoveNext())
            {
                CombatEventTree sibling = iter.Current;

                if (sibling == this)
                {
                    continue;
                }

                found = sibling.FindInChildren(evt);
                if (found != null)
                {
                    iter.Dispose();
                    return found;
                }
            }
            iter.Dispose();

            return null;
        }

        public CombatEventTree Find(CombatEvent evt)
        {
            // speed up
            CombatEventTree found = null;
            if (s_nodeCache.TryGetValue(evt.GetHashCode(), out found))
            {
                if (found == this || found.IsAncestor(this))
                {
                    return found;
                }
            }

            if (m_event.Equals(evt))
            {
                return this;
            }

            EB.Debug.LogWarning("CombatEventTree.Find: cache miss");

            var iter = m_children.GetEnumerator();
            while (iter.MoveNext())
            {
                var pair = iter.Current;
                List<CombatEventTree> tree_list = pair.Value;
                for (int i = 0, cnt = tree_list.Count; i < cnt; ++i)
                {
                    found = tree_list[i].Find(evt);
                    if (found != null)
                    {
                        iter.Dispose();
                        return found;
                    }
                }
            }
            iter.Dispose();

            return null;
        }

        // reduce GC Alloc
        private static List<CombatEventTree> queueBuffer = new List<CombatEventTree>();
        private static int queueBufferTail = 0;

        private static void ReleaseTree(CombatEventTree tree)
        {
            // clean buffer
            queueBuffer.Clear();
            queueBufferTail = 0;

            //EB.Debug.LogError("ReleaseTree: queueBuffer start");
            queueBuffer.Add(tree);

            while (queueBuffer.Count - queueBufferTail > 0)
            {
                CombatEventTree front = queueBuffer[queueBufferTail++];

                var iter = front.m_children.GetEnumerator();
                while (iter.MoveNext())
                {
                    var pair = iter.Current;
                    List<CombatEventTree> tree_list = pair.Value;
                    for (int i = 0, cnt = tree_list.Count; i < cnt; ++i)
                    {
                        queueBuffer.Add(tree_list[i]);
                    }
                }
                iter.Dispose();
            }

            for (int i = queueBuffer.Count - 1; i >= 0; --i)
            {
                CombatEventTree last = queueBuffer[i];
                if (last.Parent != null)
                {
                    last.Parent.RemoveChild(last);
                }
                last.RemoveAllChildren();
                if (last.Event.Children != null)
                {
                    last.Event.Children.Clear();
                    last.Event = null;
                }
            }

            //EB.Debug.LogError("ReleaseTree: queueBuffer end");
            queueBuffer.Clear();
            queueBufferTail = 0;
        }

        public void ReleaseTree()
        {
            ReleaseTree(this);
        }

        private static IEnumerator<CombatEventTree> TreeToListNonAlloc(CombatEventTree tree, CombatEventTree cut)
        {
            queueBuffer.Clear();
            queueBufferTail = 0;

            //EB.Debug.LogError("TreeToListNonAlloc: queueBuffer start");
            queueBuffer.Add(tree);

            while (queueBuffer.Count - queueBufferTail > 0)
            {
                CombatEventTree front = queueBuffer[queueBufferTail++];

                if (front == cut)
                {
                    continue;
                }

                var iter = front.m_children.GetEnumerator();
                while (iter.MoveNext())
                {
                    var pair = iter.Current;
                    List<CombatEventTree> tree_list = pair.Value;
                    for (int i = 0, cnt = tree_list.Count; i < cnt; ++i)
                    {
                        queueBuffer.Add(tree_list[i]);
                    }
                }
                iter.Dispose();

                yield return front;
            }

            //EB.Debug.LogError("TreeToListNonAlloc: queueBuffer end");
            queueBuffer.Clear();
            queueBufferTail = 0;
        }

        public IEnumerator<CombatEventTree> TreeToListNonAlloc()
        {
            var iter = TreeToListNonAlloc(this, null);
            while (iter.MoveNext())
            {
                yield return iter.Current;
            }
            iter.Dispose();
        }

        private static List<CombatEventTree> TreeToList(CombatEventTree tree, CombatEventTree cut)
        {
            List<CombatEventTree> list = new List<CombatEventTree>();

            queueBuffer.Clear();
            queueBufferTail = 0;

            //EB.Debug.LogError("TreeToList: queueBuffer start");
            queueBuffer.Add(tree);

            while (queueBuffer.Count - queueBufferTail > 0)
            {
                CombatEventTree front = queueBuffer[queueBufferTail++];

                if (front == cut)
                {
                    continue;
                }

                var iter = front.m_children.GetEnumerator();
                while (iter.MoveNext())
                {
                    var pair = iter.Current;
                    List<CombatEventTree> tree_list = pair.Value;
                    for (int i = 0, cnt = tree_list.Count; i < cnt; ++i)
                    {
                        queueBuffer.Add(tree_list[i]);
                    }
                }
                iter.Dispose();

                list.Add(front);
            }

            //EB.Debug.LogError("TreeToList: queueBuffer end");
            queueBuffer.Clear();
            queueBufferTail = 0;
            return list;
        }

        public List<CombatEventTree> TreeToList()
        {
            return TreeToList(this, null);
        }

        public List<CombatEventTree> ChildrenToList()
        {
            List<CombatEventTree> list = new List<CombatEventTree>();
            var iter = GetChildrenNonAlloc();
            while (iter.MoveNext())
            {
                CombatEventTree child = iter.Current;
                list.AddRange(TreeToList(child, null));
            }
            iter.Dispose();
            return list;
        }

        public IEnumerator<CombatEventTree> ChildrenToListNonAlloc()
        {
            var iter = GetChildrenNonAlloc();
            while (iter.MoveNext())
            {
                CombatEventTree child = iter.Current;

                var children = TreeToListNonAlloc(child, null);
                while (children.MoveNext())
                {
                    yield return children.Current;
                }
                children.Dispose();
            }
            iter.Dispose();
        }

        public List<CombatEventTree> GetParentsAndSiblings()
        {
            return GetParentsAndSiblings(null);
        }

        public List<CombatEventTree> GetParentsAndSiblings(CombatEventTree cut)
        {
            List<CombatEventTree> list = new List<CombatEventTree>();

            if (m_parent == null)
            {
                return list;
            }

            list = TreeToList(m_parent, this);

            CombatEventTree from = m_parent;
            var iter = m_parent.GetParentsNonAlloc();
            while (iter.MoveNext())
            {
                CombatEventTree ancestor = iter.Current;
                if (ancestor == cut)
                {
                    break;
                }

                List<CombatEventTree> ancestors = TreeToList(ancestor, from);
                list.AddRange(ancestors);

                from = ancestor;
            }
            iter.Dispose();

            return list;
        }

        public IEnumerator<CombatEventTree> GetParentsAndSiblingsNonAlloc(CombatEventTree cut)
        {
            if (m_parent == null)
            {
                yield break;
            }

            IEnumerator<CombatEventTree> list = TreeToListNonAlloc(m_parent, this);
            while (list.MoveNext())
            {
                yield return list.Current;
            }
            list.Dispose();

            CombatEventTree from = m_parent;
            var iter = m_parent.GetParentsNonAlloc();
            while (iter.MoveNext())
            {
                CombatEventTree ancestor = iter.Current;
                if (ancestor == cut)
                {
                    break;
                }

                IEnumerator<CombatEventTree> ancestors = TreeToListNonAlloc(ancestor, from);
                while (ancestors.MoveNext())
                {
                    yield return ancestors.Current;
                }
                ancestors.Dispose();

                from = ancestor;
            }
            iter.Dispose();
        }

        public bool IsParentsAndSiblingsDone(CombatEventTree cut)
        {
            if (m_parent == null)
            {
                return true;
            }

            if (!IsTreeDone(m_parent, this))
            {
                return false;
            }

            CombatEventTree from = m_parent;
            var iter = m_parent.GetParentsNonAlloc();
            while (iter.MoveNext())
            {
                CombatEventTree ancestor = iter.Current;
                if (ancestor == cut)
                {
                    break;
                }

                if (!IsTreeDone(ancestor, from))
                {
                    return false;
                }

                from = ancestor;
            }
            iter.Dispose();
            return true;
        }

        public static bool IsTreeDone(CombatEventTree tree, CombatEventTree cut)
        {
            // clean buffer
            queueBuffer.Clear();
            queueBufferTail = 0;

            //EB.Debug.LogError("IsTreeDone: queueBuffer start");
            queueBuffer.Add(tree);

            while (queueBuffer.Count - queueBufferTail > 0)
            {
                CombatEventTree front = queueBuffer[queueBufferTail++];

                if (front == cut)
                {
                    continue;
                }

                var iter = front.m_children.GetEnumerator();
                while (iter.MoveNext())
                {
                    var pair = iter.Current;
                    List<CombatEventTree> tree_list = pair.Value;
                    for (int i = 0, cnt = tree_list.Count; i < cnt; ++i)
                    {
                        queueBuffer.Add(tree_list[i]);
                    }
                }
                iter.Dispose();

                if (front.NodeState != CombatEventTreeState.DONE)
                {
                    //EB.Debug.LogError("IsTreeDone: queueBuffer end");
                    queueBuffer.Clear();
                    queueBufferTail = 0;
                    return false;
                }
            }

            //EB.Debug.LogError("IsTreeDone: queueBuffer end");
            queueBuffer.Clear();
            queueBufferTail = 0;
            return true;
        }

        public static void ReleaseBufferAndCache()
        {
            queueBuffer.Clear();
            queueBufferTail = 0;
            s_nodeCache.Clear();
        }
    }
}