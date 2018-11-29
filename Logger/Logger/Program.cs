using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainOfResponsibility
{
    [Flags]
    public enum LogLevel
    {
        None = 0,  //don't use for binary escape
        Info = 1,   //1
        Warning = 2,  //10
        Error = 4,    //100
        All = 7       //111
    }

    public abstract class Logger
    {
        protected LogLevel level;

        // The next Handler in the chain
        protected Logger next;

        public Logger(LogLevel mask)
        {
            this.level = mask;
        }

        /// <summary>
        /// Sets the Next logger to make a list/chain of Handlers.
        /// </summary>
        public Logger SetNext(Logger nextlogger)
        {
            next = nextlogger;
            return nextlogger;
        }

        public void Message(string msg, LogLevel severity)
        {
            if ((severity & level) != 0)//True only if all logMask bits are set in severity
            {
                WriteMessage(msg);
            }
            if (next != null)
            {
                next.Message(msg, severity);
            }
        }

        abstract protected void WriteMessage(string msg);
    }

    public class ConsoleLogger : Logger
    {
        public ConsoleLogger(LogLevel mask)
            : base(mask)
        { }

        protected override void WriteMessage(string msg)
        {
            Console.WriteLine("Writing to console: " + msg);
        }
    }

    public class EmailLogger : Logger
    {
        public EmailLogger(LogLevel mask)
            : base(mask)
        { }

        protected override void WriteMessage(string msg)
        {
            // Placeholder for mail send logic, usually the email configurations are saved in config file.
            Console.WriteLine("Sending via email: " + msg);
        }
    }

    class FileLogger : Logger
    {
        public FileLogger(LogLevel mask)
            : base(mask)
        { }

        protected override void WriteMessage(string msg)
        {
            // Placeholder for File writing logic
            Console.WriteLine("Writing to Log File: " + msg);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            // Build the chain of responsibility
            Logger logger, logger1, logger2;
            logger = new ConsoleLogger(LogLevel.All);
            logger1 = logger.SetNext(new FileLogger(LogLevel.Warning));
            logger2 = logger1.SetNext(new EmailLogger(LogLevel.Error));

            // Handled by ConsoleLogger since the console has a loglevel of all
            logger.Message("Order record retrieved.", LogLevel.Info);

            // Handled by FileLogger since filelogger implements Warning
            logger.Message("Customer Address details missing in Branch DataBase.", LogLevel.Warning);

            // Handled by EmailLogger
            logger.Message("Customer Address details missing in Organization DataBase.", LogLevel.Error);

            Console.ReadLine();
        }
    }
}
