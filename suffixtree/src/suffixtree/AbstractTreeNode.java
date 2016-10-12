package suffixtree;

import java.util.Collection;

/**
 * Abstracte TreeNode
 * @author Bart Middag
 */
public abstract class AbstractTreeNode implements TreeNode {
    protected int begin = -1;
    protected int end = -1;
    protected int index = -1;
    
    public AbstractTreeNode(int begin, int end, int index) {
        this.begin = begin;
        this.end = end;
        this.index = index;
    }
    
    @Override
    public boolean isRoot() {
        return index == -1;
    }
    
    @Override
    public int getIndex() {
        return index;
    }
    
    public void setIndex(int index) {
        this.index = index;
    }
    
    public int getBegin() {
        return begin;
    }
    
    public int getEnd() {
        return end;
    }

    public void setIndices(int begin, int end) {
        this.begin = begin;
        this.end = end;
    }
    
    @Override
    public abstract Collection<AbstractTreeNode> getChildren();
    
}
