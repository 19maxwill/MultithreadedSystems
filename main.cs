using System;
using System.Collections.Generic;
using System.Threading;

class Assignment1
{
    // Simulation Initialisation
    private static int NUM_MACHINES = 50;   // Number of machines in the system that issue print requests
    private static int NUM_PRINTERS = 5;    // Number of printers in the system that print requests
    private static int SIMULATION_TIME = 30;
    private static int MAX_PRINTER_SLEEP = 3;
    private static int MAX_MACHINE_SLEEP = 5;
    private static volatile bool sim_active = true;

    // Create an empty list of print requests
    printList list = new printList();

    // Put any global variables here
        //semaphoreSlim is used since it is faster for single applications
    private SemaphoreSlim queueSlots = new SemaphoreSlim(5); //semaphore for the maximum print requests - stop machines from adding print requests to the list if list has 5 items
    private SemaphoreSlim queueMutex = new SemaphoreSlim(1); //semaphore for a mutex lock to stop multiple machines and printers from accessing the list


    public void startSimulation()
    {
        // ArrayList to keep for machine and printer threads
        List<Thread> mThreads = new List<Thread>();
        List<Thread> pThreads = new List<Thread>();

        // Create Machine and Printer threads
        for (int i = 0; i < NUM_PRINTERS; i++) {
            pThreads.Add(new Thread(new printerThread(this, i).SetPrinter));
        }
        for (int i = 0; i < NUM_MACHINES; i++) {
            mThreads.Add(new Thread(new machineThread(this, i).SetMachine));
        }
        
        // start the machine threads
        foreach( Thread mThread in mThreads) {
            mThread.Start();
        }
 
        //start the printer threads
        foreach( Thread pThread in pThreads) {
            pThread.Start();
        }

        // let the simulation run for some time
        sleep(SIMULATION_TIME);

        // finish simulation
        sim_active = false;

        // Wait until all printer threads finish by using the joining them


    }

    // Printer class
    public class printerThread
    {
        private readonly Assignment1 outer;
        private int printerID;

        public printerThread(Assignment1 parent, int id)
        {
            outer = parent;
            printerID = id;
        }

        public void SetPrinter()
        {
            while (sim_active)
            {
                // Simulate printer taking some time to print the document
                printerSleep();

                // Grab the request at the head of the queue and print it
                printDox(this.printerID);
            }
        }

        public void printerSleep()
        {
            int sleepSeconds = 1 + (int)(new Random(Guid.NewGuid().GetHashCode()).NextDouble() * MAX_PRINTER_SLEEP);

            try
            {
                Thread.Sleep(sleepSeconds * 1000);
            }
            catch (ThreadInterruptedException)
            {
                Console.WriteLine("Sleep Interrupted");
            }
        }

        public void printDox(int printerID)
        {
            Console.WriteLine("Printer ID:" + printerID + " : now available");

            // Write code here
            //entry section
            outer.queueMutex.Wait(); //stop other printers from getting the head of the list, and stop machines from adding to the list

            // print from the queue
            list.queuePrint(list, printerID);

            // Write code here
             // exit section
            outer.queueMutex.Release(); //allow printers and machines to access the list
            outer.queueSlots.Release(); //decrease the slots to let machines know a free slot for a print request is available

        }

        private printList list
        {
            get { return outer.list; }
        }
    }

    // Machine class
    public class machineThread
    {
        private readonly Assignment1 outer;
        private int machineID;

        public machineThread(Assignment1 parent, int id)
        {
            outer = parent;
            machineID = id;
        }

        public void SetMachine()
        {
            while (sim_active)
            {
                // machine sleeps for a random amount of time
                machineSleep();

                // machine wakes up and sends a print request
                isRequestSafe(this.machineID);
                printRequest(this.machineID);
                postRequest(this.machineID);
            }
        }

        public void isRequestSafe(int id)
        {
            Console.WriteLine("Machine " + id + " Checking availability");

            // Write code here:
            //entry section
            outer.queueSlots.Wait(); // wait for an empty slot in the print list as to not overwrite the print list
            outer.queueMutex.Wait(); //lock the queue to stop race conditions

            Console.WriteLine("Machine " + id + " will proceed");

            
        }

        public void printRequest(int id)
        {
            Console.WriteLine("Machine " + id + " Sending a print request");

            // Build a print document
            printDoc doc = new printDoc("My name is machine " + id, id);

            // Insert it in print queue
            outer.list = outer.list.queueInsert(outer.list, doc);
        }

        public void postRequest(int id)
        {
            Console.WriteLine("Machine " + id + " Releasing binary semaphore");

            // Write code here
            //exit section
            outer.queueMutex.Release(); //release the mutex so other machines can get in
        }

        public void machineSleep()
        {
            int sleepSeconds = 1 + (int)(new Random(Guid.NewGuid().GetHashCode()).NextDouble() * MAX_MACHINE_SLEEP);

            try
            {
                Thread.Sleep(sleepSeconds * 1000);
            }
            catch (ThreadInterruptedException)
            {
                Console.WriteLine("Sleep Interrupted");
            }
        }
    }

    private static void sleep(int s)
    {
        try
        {
            Thread.Sleep(s * 1000);
        }
        catch (ThreadInterruptedException)
        {
            Console.WriteLine("Sleep Interrupted");
        }
    }

    public static void Main(string[] args)
    {
        new Assignment1().startSimulation();
    }
}