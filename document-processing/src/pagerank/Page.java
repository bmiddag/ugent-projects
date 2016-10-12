// Bart Middag

package pagerank;

import java.io.Serializable;
import java.net.URL;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import org.apache.commons.math3.fraction.BigFraction;

/**
 * Page class - includes all page properties
 * @author Bart Middag
 */
public class Page implements Comparable<Page>, Serializable {
    private final URL url;
    private transient final List<Page> links; // No need to save these into the file
    private transient int incomingLinks;
    private final Set<String> keywords;
    private BigFraction pageRank;
    private BigFraction previousRank;
    
    public Page(URL url) {
        this.url = url;
        links = new ArrayList<>();
        keywords = new HashSet<>();
        pageRank = new BigFraction(0);
        previousRank = new BigFraction(0);
    }
    
    /**
     * Adds page as a link.
     * Also adds an incoming link to that page.
     * @param page  The Page this Page links to
     */
    public void addLink(Page page) {
        links.add(page);
        page.addIncomingLink();
    }
    
    public void addIncomingLink() {
        incomingLinks++;
    }
    
    public int getIncomingLinks() {
        return incomingLinks;
    }
    
    public URL getURL() {
        return url;
    }
    
    public Set<String> getKeywords() {
        return keywords;
    }
    
    public void addKeyword(String keyword) {
        keywords.add(keyword);
    }
    
    public List<Page> getLinks() {
        return links;
    }
    
    public void setPageRank(BigFraction pageRank) {
        this.pageRank = pageRank;
    }
    
    /**
     * Add a BigFraction to the current PageRank.
     * The PageRank is computed by summing probabilities.
     * @param probability   The probability to add to the current PageRank
     */
    public void addPageRank(BigFraction probability) {
        pageRank = pageRank.add(probability);
    }
    
    public BigFraction getPageRank() {
        return pageRank;
    }
    
    /**
     * Start the next iteration in the PageRank computing process.
     */
    public void nextStep() {
        previousRank = pageRank;
        pageRank = new BigFraction(0);
    }
    
    public BigFraction getPreviousRank() {
        return previousRank;
    }
    
    public void setPreviousRank(BigFraction previousRank) {
        this.previousRank = previousRank;
    }

    /**
     * Compare Page to another Page for order.
     * Used to sort search results.
     * @param p Page to compare to
     * @return  1, 0 or -1 if bigger, equal or smaller PageRank
     */
    @Override
    public int compareTo(Page p) {
        BigFraction otherRank = p.getPageRank();
        int comparedRank = pageRank.compareTo(otherRank);
        if(comparedRank == 0) {
            return url.toString().compareTo(p.getURL().toString());
        } else return comparedRank;
    }
}
