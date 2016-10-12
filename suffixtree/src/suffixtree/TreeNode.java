package suffixtree;

import java.util.Collection;

//////////////////////////////////////
// Do not alter this file in any way!
//////////////////////////////////////

/**
 * Interface representing a node in a suffix tree.
 * This data structure is returned by the SuffixTree interface method getRoot().
 * The functions in this interface have to be implemented and are required to 
 * function properly for every implementation SuffixTree[N]. 
 * 
 * These functions were are required for correctness tests of the implementatinos,
 * but can be used to implement the required functions in SuffixTree and 
 * IntervalSearcher.
 * 
 * Additional fields and functions can be added to classes implementing this 
 * interface.
 */
public interface TreeNode {
    
    /**
     * This method checks if the current TreeNode is the root of a SuffixTree.
     * @return true if this TreeNode is the root of the tree, false otherwise.
     */
    boolean isRoot();
    
    /**
     * This method checks if the current TreeNode is a leaf.
     * @return true if this TreeNode is a leaf, false otherwise.
     */
    boolean isLeaf();
    
    /**
     * returns the 0-based index of the (a) corresponding suffix.
     * @return 
     * For a leaf: return the 0-based index of THE suffix that corresponds to this
     * leaf, according to the definition of a suffix tree. 
     * For an internal node: return a 0-based index of a suffix that corresponds 
     * to A LEAF in the subtree defined by the TreeNode (the subtree with this 
     * TreeNode as the root).
     */
    int getIndex();
    
    /**
     * A method that returns a collection of all direct children of the current
     * TreeNode.
     * @param <T> The implementing class of TreeNode of which this object is a 
     * member
     * @return A collection of all direct children of this T object, where T is 
     * an implementing class of TreeNode. The direct children of this TreeNode 
     * are the children that are connected with a single edge with this object
     * (so do not include the grandchildren etc.)
     * 
     * For a leaf, this function should return an empty collection !
     */
    <T extends TreeNode> Collection<T> getChildren();
    
}
