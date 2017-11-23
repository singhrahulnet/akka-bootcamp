using Akka.Actor;
using System.IO;
using System;

namespace WinTail
{
    public class FileValidatorActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;
        private readonly IActorRef _tailCoordinatorActor;

        public FileValidatorActor(IActorRef consolewriterActor, IActorRef tailCoordinatorActor)
        {
            _consoleWriterActor = consolewriterActor;
            _tailCoordinatorActor = tailCoordinatorActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;
            if (string.IsNullOrEmpty(msg))
            {
                _consoleWriterActor.Tell(new Messages.NullInputError("No input path"));
                Sender.Tell(new Messages.ContinueProcessing());
            }
            else
            {
                var valid = IsFileUri(msg);
                if (valid)
                {
                    _consoleWriterActor.Tell(new Messages.InputSuccess(
                        string.Format("Starting Processing for {0}", msg)));

                    _tailCoordinatorActor.Tell(new TailCoordinatorActor.StartTail(msg,_consoleWriterActor));                    
                }
                else
                {
                    _consoleWriterActor.Tell(new Messages.ValidationError("Invalid path"));
                    Sender.Tell(new Messages.ContinueProcessing());
                }
            }
        }

        private static bool IsFileUri(string path)
        {
            return File.Exists(path);
        }
    }
}
