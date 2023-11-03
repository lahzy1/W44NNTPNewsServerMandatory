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
using System.Net.Http;

namespace W44NntpNewsServerMandatory.Model
{
    //fha hvad - nej?
    internal class NntpClient : INotifyPropertyChanged
    {
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;
        private NetworkStream? _stream;

        private ObservableCollection<string> _newsGroupInfos;

        public bool IsAuthenticated = false;

        #region Binding to the ListBox
        public event PropertyChangedEventHandler? PropertyChanged;

        //fha see mine eksempler på Moodle, vi bruger CallerMemberName (hvorfor virtual?) // virtual var i forslaget, da jeg ledte efter en løsning. Har ikke kigget nærmere på det keyword.
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
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        /// <summary>
        /// Takes the server name, port, username and password from the user input in the UI and connects to the server.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public async void Connect(string server, int port, string username, string password)
        {
            Debug.WriteLine("Start of Connect()");
            _client = new TcpClient(server, port);

            //var stream = _client.GetStream();
            _stream = _client.GetStream();
            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream) { AutoFlush = true };

            // Read the server's greeting
            string responseLine = await _reader.ReadLineAsync(); //fha der er en readlineAsync, vi har lige brugt 1,5 uger på at diskuttere multithreading og async!!
            Debug.WriteLine("Response: " + responseLine);

            // Send the username
            await _writer.WriteLineAsync("AUTHINFO USER " + username);
            responseLine = await _reader.ReadLineAsync();
            Debug.WriteLine("Response: " + responseLine);

            // Send the password
            await _writer.WriteLineAsync("AUTHINFO PASS " + password);
            responseLine = await _reader.ReadLineAsync();
            Debug.WriteLine("Response: " + responseLine);

            // Check if the login was successful
            if (responseLine.StartsWith("281"))
            {
                IsAuthenticated = true;
            }

            //await _writer.FlushAsync();
            await SaveNewsGroupsAsStringsAsync(); // FIXME: Noget i denne metode gør, at GroupCommand() ikke giver det forventede resultat. Hvorfor?


            // Testing methods
            await GroupCommand("dk.test"); // Returns all the newsgroups similar to LIST. Why?
            //await GetNewsGroupArticles("dk.test"); // FIXME: It just lists more newsgroups. Why? It should just give the article numbers from the group.
        }

        /// <summary>
        /// Sends a LIST command to the server, takes the response and saves it as strings in the NewsGroupInfos collection.
        /// </summary>
        /// <returns></returns>
        public async Task SaveNewsGroupsAsStringsAsync()
        {
            ObservableCollection<string> tempNewsGroupInfos = new ObservableCollection<string>();
            await _writer.WriteLineAsync("LIST");
            string responseLine;

            int counter = 0;

            while ((responseLine = await _reader.ReadLineAsync()) != null && counter < 10) // For testing. Remove later.
            {
                responseLine = await _reader.ReadLineAsync();
                tempNewsGroupInfos.Add(responseLine);
                Debug.WriteLine("Response: " + responseLine);
                counter++;
            }

            Debug.WriteLine("Number of newsgroups: " + counter);

            //fha det er faktisk en fin ide, så kan du kalde setter direkte
            NewsGroupInfos = tempNewsGroupInfos; // I used a temporary collection because I thought it might help with the binding performance.
        }

        /// <summary>
        /// Takes the name of a newsgroup and sends a GROUP command to the server.
        /// </summary>
        /// <param name="newsGroup"></param>
        /// <returns></returns>
        public async Task GroupCommand(string newsGroup)
        {
            Debug.WriteLine("Start of GroupCommand()");
            await _writer.WriteLineAsync("GROUP " + newsGroup);
            string responseLine = await _reader.ReadLineAsync();

            Debug.WriteLine("Response: " + responseLine);

            /*int counter = 0;
            while ((responseLine = await _reader.ReadLineAsync()) != null && counter < 10) // For testing. Remove later.
            {
                Debug.WriteLine("Response: " + responseLine);
                counter++;
            }*/
        }

        // Sends the LISTGROUP <group> command to the server.
        public async Task GetNewsGroupArticles(string newsGroup)
        {
            Debug.WriteLine("Start of GetNewsGroupArticles()");
            await _writer.WriteLineAsync("LISTGROUP " + newsGroup);
            string responseLine;

            int counter = 0;

            while ((responseLine = await _reader.ReadLineAsync()) != null && counter < 10) // For testing. Remove later.
            {
                responseLine = await _reader.ReadLineAsync();
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
