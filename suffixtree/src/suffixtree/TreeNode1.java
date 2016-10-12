package suffixtree;

import java.util.Collection;
import java.util.HashMap;

/**
 * TreeNode met HashMap
 * @author Bart Middag
 */
public final class TreeNode1 extends AbstractTreeNode {

    private HashMap<Short,AbstractTreeNode> children;

    public TreeNode1(int begin, int end, int index) {
        super(begin,end,index);
    }

    @Override
    public boolean isLeaf() {
        try {
            return (children.isEmpty());
        } catch(NullPointerException ex) {
            return true;
        }
    }

    @Override
    public Collection<AbstractTreeNode> getChildren() {
        try {
            return children.values();
        } catch(NullPointerException ex) {
            return null;
        }
    }
    
    public void quickPutChild(Short key, TreeNode1 child) {
        try {
            children.put(key, child);
        } catch(NullPointerException ex) {
            //We maken de HashMap pas aan bij het toevoegen van een kind
            //met capaciteit 1 - zo sparen we een beetje geheugen
            children = new HashMap<Short,AbstractTreeNode>(1);
            children.put(key, child);
        }
    }
    
    public HashMap<Short,AbstractTreeNode> getChildMap() {
        try {
            return children;
        } catch(NullPointerException ex) {
            return null;
        }
    }
}
