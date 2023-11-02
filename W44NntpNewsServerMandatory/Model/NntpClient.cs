﻿using System;
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

namespace W44NntpNewsServerMandatory.Model
{
    internal class NntpClient : INotifyPropertyChanged, INotifyCollectionChanged
    {
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;
        NetworkStream? stream;

        private ObservableCollection<string> _newsGroupInfos;

        public bool IsAuthenticated = false;

        // Needed for the binding.
        public event PropertyChangedEventHandler? PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
        }

        //public List<string> newsGroupInfos; // Didn't work with a List.

        public ObservableCollection<string> NewsGroupInfos
        {
            get { return _newsGroupInfos; }
            set
            {
                if (_newsGroupInfos != value)
                {
                    _newsGroupInfos = value;
                    OnCollectionChanged(NotifyCollectionChangedAction.Reset);
                }
            }
        } 

        public void Connect(string server, int port, string username, string password)
        {
            _client = new TcpClient(server, port);

            //var stream = _client.GetStream();
            stream = _client.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream) { AutoFlush = true };

            // Read the server's greeting
            string responseLine = _reader.ReadLine();
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

/*            _writer.WriteLine("LIST");
            response = _reader.ReadLine();
            Debug.WriteLine("Response: " + response);

            response = _reader.ReadLine();
            Debug.WriteLine("Response: " + response);*/

            SaveNewsGroupsAsStrings();
        }

        public void SaveNewsGroupsAsStrings()
        {
            //newsGroupInfos = new List<string>();
            ObservableCollection<string> tempNewsGroupInfos = new ObservableCollection<string>();

            // Send the LIST command and saves the newsgroups returned from the server in the NewsGroups array.
            _writer.WriteLine("LIST");
            string responseLine;

            int counter = 0;

            while ((responseLine = _reader.ReadLine()) != null)
            {
                responseLine = _reader.ReadLine();
                tempNewsGroupInfos.Add(responseLine);
                Debug.WriteLine("Response: " + responseLine);
                counter++;
            }

            Debug.WriteLine("Number of newsgroups: " + counter);

            NewsGroupInfos = tempNewsGroupInfos; // I used a temporary collection because I thought it might help with the binding performance.
        }

/*        public void PopulateListBox()
        {
            // Populate the newsGroupsListBox with the NewsGroupInfos List.
            foreach (string item in NewsGroupInfos)
            {
                newsGroupsListBox.Items.Add(item); // FIXME: This is not working. Why?
            }
        }*/
    }
}
