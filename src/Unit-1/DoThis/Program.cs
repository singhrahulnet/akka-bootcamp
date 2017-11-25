using System;
using Akka.Actor;

namespace WinTail
{
    #region Program
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
            // initialize MyActorSystem
            MyActorSystem = ActorSystem.Create("MyActorSystem");

            //var consoleWriterActor = MyActorSystem.ActorOf(Props.Create(() =>
            //new ConsoleWriterActor()));
            //var consoleReaderActor = MyActorSystem.ActorOf(Props.Create(() =>
            //new ConsoleReaderActor(consoleWriterActor)));

            Props consoleWriterProps = Props.Create<ConsoleWriterActor>();
            IActorRef consoleWriterActor = MyActorSystem.ActorOf(consoleWriterProps, "consoleWriterActor");


            //Props validationActorProps = Props.Create(() => new ValidationActor(consoleWriterActor));
            //IActorRef validationActor = MyActorSystem.ActorOf(validationActorProps, "validationActor");

            // make tailCoordinatorActor
            Props tailCoordinatorProps = Props.Create(() => new TailCoordinatorActor());
            IActorRef tailCoordinatorActor = MyActorSystem.ActorOf(tailCoordinatorProps, "tailCoordinatorActor");

            // pass tailCoordinatorActor to fileValidatorActorProps (just adding one extra arg)
            Props fileValidatorActorProps = Props.Create(() => new FileValidatorActor(consoleWriterActor));

            IActorRef validationActor = MyActorSystem.ActorOf(fileValidatorActorProps, "validationActor");
           

            Props consoleReaderProps = Props.Create<ConsoleReaderActor>();
            IActorRef consoleReaderActor = MyActorSystem.ActorOf(consoleReaderProps, "consoleReaderActor");




            // tell console reader to begin
            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            // blocks the main thread from exiting until the actor system is shut down
            MyActorSystem.WhenTerminated.Wait();
        }

    }
    #endregion
}
