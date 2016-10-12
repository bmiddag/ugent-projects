package test;

import java.lang.management.ManagementFactory;
import java.lang.management.ThreadMXBean;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import suffixtree.*;

/**
 * Testklasse voor de Suffixtree-algoritmen
 * @author Bart Middag
 */
public class Suffixtree {

    private Random random = new Random();
    private ArrayList<IntervalSearcher> tree = new ArrayList<IntervalSearcher>();
    private ArrayList<List<Short>> queries = new ArrayList<List<Short>>();
    private List<Short> list = new ArrayList();

    public void run(short max, int num_queries, int test_start, int test_stop, int test_step, boolean randomQueries, boolean minimalist) {
        long begin, end, time;
        int i, j, to_add, querySize, queryBegin;
        List<Short> query;

        if (randomQueries) {
            for (i = 0; i < num_queries; i++) {
                // Maak een query aan met random getallen.
                query = new ArrayList();
                querySize = random.nextInt(Math.max(0, test_start - test_step) + 1);
                for (j = 1; j < querySize; j++) {
                    query.add((short) random.nextInt(max));
                }
                //Voeg dit toe aan de lijst met queries.
                queries.add(query);
            }
        }


        for (int n = test_start; n < test_stop; n += test_step) {
            to_add = n - list.size();
            // Breid de sequence uit met random getallen.
            for (i = 0; i < to_add; i++) {
                list.add((short) random.nextInt(max));
            }

            if (randomQueries) {
                //Breid de queries uit met random getallen.
                for (i = 0; i < num_queries; i++) {
                    query = queries.get(i);
                    querySize = random.nextInt(test_step);
                    for (j = 0; j < querySize; j++) {
                        query.add((short) random.nextInt(max));
                    }
                }
            } else {
                //Maak nieuwe sublists aan van de lijst.
                queries.clear();
                for (i = 0; i < num_queries; i++) {
                    querySize = random.nextInt(list.size());
                    queryBegin = random.nextInt(list.size() - querySize);
                    queries.add(list.subList(queryBegin, queryBegin + querySize));
                }
            }

            //De arraylist met IntervalSearcher-objecten waarover we gaan itereren.
            tree.add(new SuffixTree1());
            tree.add(new SuffixTree2());
            tree.add(new AltIntervalSearcher());
            //We doen deze tests met ThreadMXBean ipv System.currentTimeMillis (zie verslag)
            ThreadMXBean bean = ManagementFactory.getThreadMXBean();
            if(!minimalist) System.out.printf("  ______________________________________________________________________%n");
            if(!minimalist) System.out.printf(" /%29s | CONSTRUCT | CONTAINS | COUNT | LOCATE \\%n", "N=" + n + " - S=" + max + " - Q=" + num_queries);
            for (i = 0; !tree.isEmpty(); i++) {
                if (tree.get(0) instanceof AbstractSuffixTree) {
                    if(!minimalist) System.out.printf(" |%29s |", "SuffixTree" + (i + 1));
                } else {
                    if(!minimalist) System.out.printf(" |%29s |", "AltIntervalSearcher");
                }
                //Voer garbage collection uit zodat dit de tijdsmetingen niet verstoort.
                System.gc();
                begin = bean.getCurrentThreadCpuTime();
                tree.get(0).construct(list);
                end = bean.getCurrentThreadCpuTime();
                time = (end - begin) / 1000000;
                if(!minimalist) {
                    System.out.printf("%10d |", time);
                } else {
                    System.out.print(time + " ");
                }

                begin = bean.getCurrentThreadCpuTime();
                for (j = 0; j < num_queries; j++) {
                    tree.get(0).contains(queries.get(j));
                }
                end = bean.getCurrentThreadCpuTime();
                time = (end - begin) / 1000000;
                if(!minimalist) {
                    System.out.printf("%9d |", time);
                } else {
                    System.out.print(time + " ");
                }

                begin = bean.getCurrentThreadCpuTime();
                for (j = 0; j < num_queries; j++) {
                    tree.get(0).count(queries.get(j));
                }
                end = bean.getCurrentThreadCpuTime();
                time = (end - begin) / 1000000;
                if(!minimalist) {
                    System.out.printf("%6d |", time);
                } else {
                    System.out.print(time + " ");
                }

                begin = bean.getCurrentThreadCpuTime();
                for (j = 0; j < num_queries; j++) {
                    tree.get(0).locate(queries.get(j));
                }
                end = bean.getCurrentThreadCpuTime();
                time = (end - begin) / 1000000;
                if(!minimalist) {
                    System.out.printf("%7d |%n", time);
                } else {
                    System.out.print(time);
                }

                //De huidige IntervalSearcher is niet meer nodig, zorg dat garbage collector dit kan verwijderen.
                tree.remove(0);
                
                if(minimalist && !tree.isEmpty()) System.out.print(" ");
            }

            if(!minimalist) {
                System.out.printf(" *----------------------------------------------------------------------*%n");
            } else {
                System.out.println();
            }

            //Roep garbage collection op zodat dit de tijdsmetingen niet verstoort.
            System.gc();
        }
    }

    public void test1(AbstractSuffixTree suffixTree) {
        List<Short> testList = new ArrayList();
        testList.add((short) 1);
        testList.add((short) 2);
        testList.add((short) 1);
        testList.add((short) 1);
        testList.add((short) 3);
        testList.add((short) 1);
        testList.add((short) 2);
        testList.add((short) 3);
        testList.add((short) 4);
        testList.add((short) 10);
        testList.add((short) 2);
        testList.add((short) 5);
        testList.add((short) 5);
        suffixTree.construct(testList);

        List<Short> list2 = new ArrayList();
        list2.add((short) 1);
        list2.add((short) 2);

        List<Short> list3 = new ArrayList();
        list3.add((short) 10);
        list3.add((short) 5);
        list3.add((short) 2);
        list3.add((short) 5);

        List<Short> list4 = new ArrayList();
        list4.add((short) 5);

        List<Short> list5 = new ArrayList();

        System.out.println(suffixTree.contains(testList) + " - " + suffixTree.count(testList));
        for (Integer lol : suffixTree.locate(testList)) {
            System.out.print(lol + " ");
        }
        System.out.println();
        System.out.println(suffixTree.contains(list2) + " - " + suffixTree.count(list2));
        for (Integer lol : suffixTree.locate(list2)) {
            System.out.print(lol + " ");
        }
        System.out.println();
        System.out.println(suffixTree.contains(list3) + " - " + suffixTree.count(list3));
        for (Integer lol : suffixTree.locate(list3)) {
            System.out.print(lol + " ");
        }
        System.out.println();
        System.out.println(suffixTree.contains(list4) + " - " + suffixTree.count(list4));
        for (Integer lol : suffixTree.locate(list4)) {
            System.out.print(lol + " ");
        }
        System.out.println();
        System.out.println(suffixTree.contains(list5) + " - " + suffixTree.count(list5));
        for (Integer lol : suffixTree.locate(list5)) {
            System.out.print(lol + " ");
        }
        System.out.println();

        suffixTree.printDetails();
    }

    public void test2(AbstractSuffixTree suffixTree) {
        List<Short> testList = new ArrayList();
        testList.add((short) 6);
        testList.add((short) 6);
        testList.add((short) 23);
        testList.add((short) 4);
        testList.add((short) 8);
        testList.add((short) 23);
        testList.add((short) 6);
        //list.add(Short.MAX_VALUE);
        suffixTree.construct(testList);

        List<Short> list2 = new ArrayList();
        list2.add((short) 6);
        list2.add((short) 23);

        List<Short> list3 = new ArrayList();
        list3.add((short) 6);

        List<Short> list4 = new ArrayList();
        list4.add((short) 5);

        List<Short> list5 = new ArrayList();

        System.out.println(suffixTree.contains(testList) + " - " + suffixTree.count(testList));
        for (Integer lol : suffixTree.locate(testList)) {
            System.out.print(lol + " ");
        }
        System.out.println();
        System.out.println(suffixTree.contains(list2) + " - " + suffixTree.count(list2));
        for (Integer lol : suffixTree.locate(list2)) {
            System.out.print(lol + " ");
        }
        System.out.println();
        System.out.println(suffixTree.contains(list3) + " - " + suffixTree.count(list3));
        for (Integer lol : suffixTree.locate(list3)) {
            System.out.print(lol + " ");
        }
        System.out.println();
        System.out.println(suffixTree.contains(list4) + " - " + suffixTree.count(list4));
        for (Integer lol : suffixTree.locate(list4)) {
            System.out.print(lol + " ");
        }
        System.out.println();
        System.out.println(suffixTree.contains(list5) + " - " + suffixTree.count(list5));
        for (Integer lol : suffixTree.locate(list5)) {
            System.out.print(lol + " ");
        }
        System.out.println();

        suffixTree.printDetails();
    }

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        Suffixtree test = new Suffixtree();
        //test.test1(new SuffixTree1());
        //test.test2(new SuffixTree2());

        //Voer de testmethode uit.
        //arg0: alfabetgrootte (s)
        //arg1: aantal queries (q)
        //arg2: min. grootte van de lijst
        //arg3: max. grootte van de lijst
        //arg4: grootte wordt hiermee verhoogd bij elke test
        //arg5: gebruik willekeurige queries (true), of gebruik sublists (false)
        //arg6: minimalistische prints om gemakkelijk grafiekjes mee te kunnen maken (true), duidelijke prints (false)
        test.run((short) Short.MAX_VALUE, 20, 500000, 40000000, 500000, true, false);
        System.gc();
    }
}
