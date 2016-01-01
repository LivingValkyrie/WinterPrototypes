using System;

namespace EventTesting {
    class Program {
        static void Main(string[] args) {
            EventRaiser eventRaiser = new EventRaiser();
            eventRaiser.testEventHandler += SubscriberEventMathod;
            eventRaiser.testEventHandler += eventRaiser.EventMethod;
            eventRaiser.testEventHandler(eventRaiser, EventArgs.Empty);
        }

        public static void SubscriberEventMathod(object sender, EventArgs e) {
            Console.WriteLine("subscriber called");
        }

    }

    class EventRaiser {
        public EventHandler testEventHandler;

        public void EventMethod(object sender, EventArgs e) {
            Console.WriteLine("raiser event called");
        }

    }

    class EventSubscriber {

        public void EventMethod() {}
    }

    class CustomEventArgs : EventArgs {}
}