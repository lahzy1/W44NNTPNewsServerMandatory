using System.Windows;
using System.Windows.Controls;
using W44NntpNewsServerMandatory.Model;

namespace W44NntpNewsServerMandatory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NntpClient client;

        public MainWindow()
        {
            InitializeComponent();

            //fha
            client = new NntpClient();
            this.DataContext = client;
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            //fha
            //client = new NntpClient();
            client.Connect(serverTextBox.Text, int.Parse(portTextBox.Text), usernameTextBox.Text, passwordTextBox.Password);

            // If login succesful send the LIST command and display the list of newsgroups
            // in the newsGroups ListBox.
            if (client.IsAuthenticated)
            {
                //client.SaveNewsGroupsAsStrings();
                //client.PopulateListBox();
            }
        }

        private void newsGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void newsArticles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SafeUserData(string server, int port, string username)
        {
            // Saves the server name, port and user name to a file.
            
        }
    }
}
