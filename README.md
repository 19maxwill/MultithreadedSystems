This is an example of the Producer-Consumer problem.
In this system, there are several machines (M) that occasionally need to print
some text. there are also a smaller number of printers (N) that can print text
given to them. There are more machines than printers (M > N).
To enable printing, the system uses a print queue. When a machine has
something to print, it creates a print request for the document and inserts that
request into the print queue. When a printer becomes free, it retrieves the
document at the head of the queue, prints it, and removes it from the queue.
Because resources in a real system are finite, the queue is also limited. Thus 
a synchronization problem arises. This program demonstrates the ability to
solve this problem using semaphores.
