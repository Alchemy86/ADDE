using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;

namespace Cascade.DirectoryAgent
{

    public class ObjectChangedEventArgs : EventArgs
    {
        public ObjectChangedEventArgs(SearchResultEntry entry)
        {
            Result = entry;
        }
    
        public SearchResultEntry Result { get; set;}
    }

    public class ChangeNotifier : IDisposable
    {
        LdapConnection _connection;
        HashSet<IAsyncResult> _results = new HashSet<IAsyncResult>();
        
        public ChangeNotifier(LdapConnection connection)
        {
            _connection = connection;
            _connection.AutoBind = true;
        }
    
        public void Register(string dn, SearchScope scope)
        {
            SearchRequest request = new SearchRequest(
                dn, //root the search here
                "(objectclass=*)", //very inclusive
                scope, //any scope works
                new string[] { "sAMAccountName", "title", "description", "telephoneNumber", "isDeleted", "objectGUID", "uSNChanged" } //we are interested in all attributes
                );
    
            //register our search
            request.Controls.Add(new DirectoryNotificationControl() {
                IsCritical = true
            });
    
            IAsyncResult result = _connection.BeginSendRequest(
                request,
                PartialResultProcessing.ReturnPartialResultsAndNotifyCallback, 
                Notify, // Callback that processes responses
                null
            );

            //store the hash for disposal later
            _results.Add(result);
        }
    
        private void Notify(IAsyncResult result)
        {
            //since our search is long running, we don't want to use EndSendRequest
            PartialResultsCollection prc = _connection.GetPartialResults(result);
    
            foreach (SearchResultEntry entry in prc)
            {
                OnObjectChanged(new ObjectChangedEventArgs(entry));
            }
        }
    
        private void OnObjectChanged(ObjectChangedEventArgs args)
        {
            Console.WriteLine("Some kind of trigger... ooww");
            if (ObjectChanged != null)
            {
                ObjectChanged(this, args);
            }
        }
    
        public event EventHandler<ObjectChangedEventArgs> ObjectChanged;
    
        public void Dispose()
        {
            foreach (var result in _results)
            {
                //end each async search
                _connection.Abort(result);
            }
        }
    }
}