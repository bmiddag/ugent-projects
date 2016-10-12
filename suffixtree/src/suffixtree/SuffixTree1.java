package suffixtree;

import java.util.List;

/**
 * SuffixTree met HashMap
 * Zie verslag voor commentaar!
 * @author Bart Middag
 */
public class SuffixTree1 extends AbstractSuffixTree {

    @Override
    public void construct(List<Short> sequence) {
        super.construct(sequence);
        root = new TreeNode1(0, 0, -1);
        int size = values.size();
        for (int i = size - 2; i >= 0; i--) {
            addChild(new TreeNode1(i, size, i));
        }
    }

    public void addChild(TreeNode1 child) {
        TreeNode1 foundNode = null;
        TreeNode1 parent = (TreeNode1)root;
        int i = 0;
        int curBegin, curSize, minSize;
        int childBegin = child.getBegin();
        int childSize = child.getEnd() - childBegin;
        if (childSize > 0) {
            foundNode = (TreeNode1)quickGetChild(parent,values.get(childBegin));
        }
        while (foundNode != null) {
            curBegin = foundNode.getBegin();
            curSize = foundNode.getEnd() - curBegin;
            minSize = Math.min(curSize, childSize);
            for (i = 1; i < minSize; i++) {
                if (!values.get(childBegin + i).equals(values.get(curBegin + i))) {
                    break;
                }
            }
            if (i == curSize) {
                foundNode.setIndex(child.getIndex());
                child.setIndices(childBegin + i, child.getEnd());
                childBegin += i;
                childSize -= i;
                parent = foundNode;
                i = 0;
                if (childSize > 0) {
                    foundNode = (TreeNode1)quickGetChild(parent,values.get(childBegin));
                } else {
                    break;
                }
            } else {
                int begin = Math.min(foundNode.getBegin(), childBegin);
                TreeNode1 splitNode = new TreeNode1(begin, begin + i, child.getIndex());
                foundNode.setIndices(foundNode.getBegin() + i, foundNode.getEnd());
                child.setIndices(childBegin + i, child.getEnd());
                splitNode.quickPutChild(values.get(foundNode.getBegin()), foundNode);
                splitNode.quickPutChild(values.get(childBegin + i), child);
                parent.quickPutChild(values.get(begin), splitNode);
                break;
            }
        }
        if (i == 0) {
            parent.quickPutChild(values.get(childBegin), child);
        }
    }
    
    @Override
    public AbstractTreeNode quickGetChild(AbstractTreeNode parent, Short key) {
        try {
            return ((TreeNode1)parent).getChildMap().get(key);
        } catch(NullPointerException ex) {
            return null;
        }
    }
}
