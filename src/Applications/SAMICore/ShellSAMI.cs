using System;
using System.Threading;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.IOInterfaces.TextOutput;

namespace SAMI.Application
{
    internal class ShellSAMI : SAMIBase
    {
        public ShellSAMI(IInternalConfigurationManager configManager) : 
            base(configManager)
        {
            foreach(ITextOutputController controller in ConfigManager.FindAllComponentsOfType<ITextOutputController>())
            {
                controller.SpeechOutputAdded += Controller_SpeechOutputAdded;
            }
        }

        internal void Controller_SpeechOutputAdded(object sender, SpeechOutputAddedEventArgs e)
        {
            Console.WriteLine(e.TextOutput);
        }

        public override void CheckForInputs()
        {
            // Wait for a bit so that if a response comes back immediately, we get the event.
            Thread.Sleep(100);
            Console.Write("> ");
            String input = Console.ReadLine();
            if (input.Equals("exit"))
            {
                _shouldStop = true;
            }
            else
            {
                foreach (IVoiceSensor kinect in ConfigManager.FindAllComponentsOfType<IVoiceSensor>())
                {
                    kinect.SimulateInput(input);
                }
                if (!input.StartsWith("Sammie"))
                {
                    input = "Sammie " + input;
                }
                foreach (IVoiceSensor kinect in ConfigManager.FindAllComponentsOfType<IVoiceSensor>())
                {
                    kinect.SimulateInput(input);
                }
            }
        }
    }
}
