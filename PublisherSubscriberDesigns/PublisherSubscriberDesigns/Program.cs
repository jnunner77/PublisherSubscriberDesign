using System;
using System.Collections.Generic;
using System.Linq;
namespace PublisherSubscriberDesigns {
    /// <summary>
    /// This class is used to set the event argument type and to define it as a generic type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyEventArgs<T> : EventArgs {
        // Getter/setter for the generic value type.
        public T Value { get; set; }
        // Constructor sets the value for the generic type.
        public MyEventArgs(T value) {
            Value = value;
        }
    }
    /// <summary>
    /// This class defines the publisher, event handler and what happens when the event is raised.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Publisher<T> {
        // This generic EventHandler defines the type of argument we want to send while raising our event. The delegate
        // is used to keep track of which subscriber's callback methods are being referenced.
        public event EventHandler<MyEventArgs<T>> onChange = delegate { };
        // This function is responsible for what happens when an event is raised.
        public void Raise() {
            // A new argument is created.
            MyEventArgs<T> eventArgs = new MyEventArgs<T>((T)new Object { });
            // A list of exceptions keeps track of what callback method it came from.
            List<Exception> exceptions = new List<Exception>();
            // We loop through all of our callback methods to see if one of the callback methods either passes or fails.
            // In the event of a pass, the data is passed through the parameters to the method. If it fails, an exception 
            // is raised.
            foreach(Delegate handler in onChange.GetInvocationList()) {
                try {
                    // Unless we are using delegates, a DynamicInvoke should be avoided at all costs.
                    handler.DynamicInvoke(this, eventArgs);
                } catch(Exception e) {
                    exceptions.Add(e);
                }
            }
            // If there were any exceptions in the callback method calls, raise them now.
            if(exceptions.Any()) {
                throw new AggregateException(exceptions);
            }
        }
    }
    /// <summary>
    /// This class is used to subscribe members to a particular event that has been previously defined.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Program<T> {
        static void Main(string[] args) {
            // We define a Publisher class object.
            Publisher<T> p = new Publisher<T>();
            // The subscriber is registered for this particular event.
            p.onChange += (sender, e) => Console.WriteLine("Event 1 works." + e.Value.ToString());
            p.onChange += (sender, e) => { throw new Exception(); };
            // We raise the event and invoke all of the subscriber callback methods.
            p.Raise();
            Console.ReadLine();
        }
    }
}