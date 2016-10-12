package suffixtree;

import java.util.List;
import java.util.TreeSet;

/**
 * SuffixTree met TreeSet
 * Zie verslag voor commentaar!
 * @author Bart Middag
 */
public class SuffixTree2 extends AbstractSuffixTree {

    @Override
    public void construct(List<Short> sequence) {
        super.construct(sequence);
        root = new TreeNode2(0, 0, -1, (short) 0);
        int size = values.size();
        for (int i = size - 2; i >= 0; i--) {
            addChild(new TreeNode2(i, size, i, values.get(i)));
        }
    }

    public void addChild(TreeNode2 child) {
        TreeNode2 foundNode = null;
        TreeNode2 parent = (TreeNode2)root;
        int i = 0;
        int curBegin, curSize, minSize;
        int childBegin = child.getBegin();
        int childSize = child.getEnd() - childBegin;
        if (childSize > 0) {
            foundNode = (TreeNode2)quickGetChild(parent, values.get(childBegin));
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
                childBegin += i;
                childSize -= i;
                child.setIndices(childBegin, childBegin+childSize);
                try {
                    child.setValue(values.get(childBegin));
                } catch(IndexOutOfBoundsException ex) {
                    child.setValue(Short.MAX_VALUE);
                }
                parent = foundNode;
                i = 0;
                if (childSize > 0) {
                    foundNode = (TreeNode2)quickGetChild(parent, values.get(childBegin));
                } else {
                    break;
                }
            } else {
                int begin = Math.min(foundNode.getBegin(), childBegin);
                TreeNode2 splitNode = new TreeNode2(begin, begin + i, child.getIndex(), values.get(begin));
                foundNode.setIndices(foundNode.getBegin() + i, foundNode.getEnd());
                parent.quickRemoveChild(foundNode);
                foundNode.setValue(values.get(foundNode.getBegin()));
                child.setIndices(childBegin + i, child.getEnd());
                child.setValue(values.get(childBegin + i));
                splitNode.quickAddChild(foundNode);
                splitNode.quickAddChild(child);
                parent.quickAddChild(splitNode);
                break;
            }
        }
        if (i == 0) {
            parent.quickAddChild(child);
        }
    }
    
    public List<Short> getValues() {
        return values;
    }

    @Override
    public AbstractTreeNode quickGetChild(AbstractTreeNode parent, Short key) {
        TreeNode2 node = (TreeNode2)parent;
        TreeSet<AbstractTreeNode> children = node.getChildren();
        short value = node.getValue();
        node.setValue(key);
        try {
            if (children.contains(parent)) {
                TreeNode2 child = (TreeNode2)children.floor(parent);
                node.setValue(value);
                return child;
            }
        } catch (NullPointerException ex) {}
        node.setValue(value);
        return null;
    }
}