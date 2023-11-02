using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

//fha callermembername
using System.Runtime.CompilerServices;

namespace W44NntpNewsServerMandatory.Model
{
    //fha hvad - nej?
    internal class NntpClient : INotifyPropertyChanged
    {
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;
        NetworkStream? stream;

        private ObservableCollection<string> _newsGroupInfos;

        public bool IsAuthenticated = false;

        #region Needed for the binding.
        public event PropertyChangedEventHandler? PropertyChanged;

        //fha see mine eksempler på Moodle, vi bruger CallerMemberName (hvorfor virtual?) // virtual blev bare foreslogt, da jeg ledte efter en løsning. Har ikke kigget nærmere på det keyword.
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<string> NewsGroupInfos
        {
            get { return _newsGroupInfos; }
            set
            {
                if (_newsGroupInfos != value)
                {
                    _newsGroupInfos = value;
                    //fha når setter kaldes så skal view informeres via et event
                    OnPropertyChanged(); // 
                }
            }
        } 
        #endregion

        public void Connect(string server, int port, string username, string password)
        {
            _client = new TcpClient(server, port);

            //var stream = _client.GetStream();
            stream = _client.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream) { AutoFlush = true };

            // Read the server's greeting
            string responseLine = _reader.ReadLine(); //fha der er en readlineAsync, vi har lige brugt 1,5 uger på at diskuttere multithreading og async!!
            Debug.WriteLine("Response: " + responseLine);

            // Send the username
            _writer.WriteLine("AUTHINFO USER " + username);
            responseLine = _reader.ReadLine();
            Debug.WriteLine("Response: " + responseLine);

            // Send the password
            _writer.WriteLine("AUTHINFO PASS " + password);
            responseLine = _reader.ReadLine();
            Debug.WriteLine("Response: " + responseLine);

            // Check if the login was successful
            if (responseLine.StartsWith("281"))
            {
                IsAuthenticated = true;
            }

            SaveNewsGroupsAsStrings();

            // Testing methods
            GroupCommand("dk.test"); // Returns all the newsgroups similar to LIST. Why?
            //GetNewsGroupArticles("dk.test"); // FIXME: It just lists more newsgroups. Why? It should just give the article numbers from the group.
        }

        public void SaveNewsGroupsAsStrings()
        {
            ObservableCollection<string> tempNewsGroupInfos = new ObservableCollection<string>();

            _writer.WriteLine("LIST");
            string responseLine;

            int counter = 0;

            while ((responseLine = _reader.ReadLine()) != null && counter < 10)
            {
                responseLine = _reader.ReadLine();
                tempNewsGroupInfos.Add(responseLine);
                Debug.WriteLine("Response: " + responseLine);
                counter++;
            }

            Debug.WriteLine("Number of newsgroups: " + counter);

            //_writer.Flush(); // Did not help to fix GroupCommand() and GetNewsGroupArticles().

            //fha det er faktisk en fin ide, så kan du kalde setter direkte
            NewsGroupInfos = tempNewsGroupInfos; // I used a temporary collection because I thought it might help with the binding performance.
        }

        public void GroupCommand(string newsGroup)
        {
            _writer.WriteLine("GROUP " + newsGroup);
            string responseLine = _reader.ReadLine();

            int counter = 0;
            while ((responseLine = _reader.ReadLine()) != null && counter < 10) // For testing. Remove later.
            {
                Debug.WriteLine("Response: " + responseLine);
                counter++;
            }
        }

        // Sends the LISTGROUP <group> command to the server.
        public void GetNewsGroupArticles(string newsGroup)
        {
            _writer.WriteLine("LISTGROUP " + newsGroup);
            string responseLine;

            int counter = 0;

            while ((responseLine = _reader.ReadLine()) != null && counter < 10)
            {
                responseLine = _reader.ReadLine();
                Debug.WriteLine("Response: " + responseLine);
                counter++;
            }

            Debug.WriteLine("Number of articles: " + counter);
        }

        // Sends the XHDR Subject <article range> command to the server.
        public void GetNewsGroupHeadlines(string newsGroup, int start, int end)
        {
            
        }


        /*public void PopulateListBox()
        {
            // Populate the newsGroupsListBox with the NewsGroupInfos List.
            foreach (string item in NewsGroupInfos)
            {
                newsGroupsListBox.Items.Add(item); // FIXME: This is not working. Why?
            }
        }*/
    }
}
