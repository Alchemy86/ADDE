using System;
using System.DirectoryServices.Protocols;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Cascade.DirectoryAgent
{
    public class HostedService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var identifier = new LdapDirectoryIdentifier(null);
          
            using (LdapConnection connect = new LdapConnection(identifier, null, AuthType.Basic))
            {
                connect.AutoBind = true;
                    connect.SessionOptions.ProtocolVersion = 3;
                using (ChangeNotifier notifier = new ChangeNotifier(connect))
                {
                    //register some objects for notifications (limit 5)
                    notifier.Register("CN=Users,dc=poc,dc=local", SearchScope.OneLevel);
        
                    notifier.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(notifier_ObjectChanged);
        
                    Console.WriteLine("Waiting for changes...");
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        
                    }

                    return Task.CompletedTask;
                }
            }
        }

        static void notifier_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            Console.WriteLine("EPPPP");
            Console.WriteLine(e.Result.DistinguishedName);
            foreach (string attrib in e.Result.Attributes.AttributeNames)
            {
                foreach (var item in e.Result.Attributes[attrib].GetValues(typeof(string)))
                {
                    Console.WriteLine("\t{0}: {1}", attrib, item);
                }
            }
            Console.WriteLine();
            Console.WriteLine("====================");
            Console.WriteLine();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("DYIUNG");
            return Task.CompletedTask;
        }
    }
}