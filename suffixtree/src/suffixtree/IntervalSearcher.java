package suffixtree;

import java.util.List;
import java.util.Set;

//////////////////////////////////////
// Do not alter this file in any way!
//////////////////////////////////////

/**
 * Inteface IntervalSearcher contains methods for finding patterns in lists of
 * shorts. 
 * 
 * The main purpose of an IntervalSearcher is to search for patterns 
 * (sublists/subintervals) within a given larger reference list/sequence. 
 * This specific IntervalSearcher is designed to work with lists of shorts.
 * 
 * Because reference is large and there are a lot of queries in a practical
 * example, reference is first indexed to speed up searches. This is done using
 * the construct method.
 * 
 * Example:
 * reference:
 * 1,2,35,10000, ..., -8, ... , 24, 10000, ... ,-8, ... , -20, -500, 1000, 7
 * | --------------- length = 10.000.000 (for example) --------------------|
 * query:
 * 10000,          ...           ,-8
 * |- length can be 100 for example |
 * 
 * First index (build suffix tree) for reference
 * Then use contains, count or locate functions on query 
 * (usually call for thousands or millions of queries)
 * 
 */
public interface IntervalSearcher {
    
    /**
     * Because suffix tree methods are sometimes easier to implement using a 
     * sentinel character (a character is appended to the sequence and that does
     * not occur anywhere else in the sequence), the character SENTINEL will never
     * occur in any sequence (reference nor query pattern)  for testing purposes. 
     * Thus, this character can be used for Suffix tree implementations.
     * 
     * !!!Note: appending the SENTINEL should be only used internally and no 
     * reference of the SENTINEL being appended to a reference sequence can be found
     * using the methods in the final implementation.
     * (Examples: the count of leaves should not differ from the length of the 
     * reference sequence, and there cannot be a leaf labeled SENTINEL that is 
     * a child of the root).
     * 
     * Usage of the SENTINEL for suffix tree implementations is strongly  recommended 
     * (but not obligatory).
     */
    public static final short SENTINEL = Short.MAX_VALUE;
    
    //NOTE: implementing classes should have a default constructor
    
    /**
     * Preprocess the reference string by building an index structure (suffix
     * tree) for the sequence.
     * Note: the search methods will never be called before the method construct
     * is called at least once.
     * @param sequence: a list of shorts that is considered the reference sequence.
     * This sequence is usually much larger than the pattern that are searched
     * in the sequence.
     */
    public void construct(List<Short> sequence);
    
    /**
     * search for the pattern query in the reference sequence.
     * @param query: list of shorts 
     * @return: true iff query is an exact match for at least one sublist/subinterval
     * of the reference sequence. 
     * This method is similar to the contains method of a string, but works 
     * with lists of shorts instead.
     * @return true for an empty list.
     */
    public boolean contains(List<Short> query);
    
    /**
     * search for the pattern query in the reference sequence.
     * @param query: list of shorts 
     * @return: the number of times query occurs as a sublist/subinterval
     * of the reference sequence. 
     * This method is similar to the locate method below, but only returns the 
     * number of times the pattern is found.
     * @return for an empty list: the number of suffixes in the reference sequence.
     */    
    public int count(List<Short> query);
    
    /**
     * Find all occurrences of the pattern query in the reference sequence.
     * Example: (i)   suffix tree for sequence 1,2,50,100,25,25,25,50,100,25,25 
     *          (ii)  query 25,25
     *          (iii) result {4,5,9}
     * @param query the query sequence (list of shorts)
     * @return a set of locations (index positions) in the sequence where 
     * the query occurs as a sublist/subinterval of the reference sequence.
     * This method is similar to iteratively using the indexOf method on strings,
     * untill all positions are found, but uses lists of shorts instead.
     * @return for an empty list: the indexes of all suffixes in the reference sequence.
     */
    public Set<Integer> locate(List<Short> query);
    
    /**
     * return the root of the suffix tree (only used for suffix tree methods, other 
     * alternative methods return null). 
     * @return a TreeNode (object from a class implementing TreeNode) representing
     * the root of the suffix tree. If the suffix tree has not been constructed,
     * this method should return null.
     */
    TreeNode getRoot();
    
}
