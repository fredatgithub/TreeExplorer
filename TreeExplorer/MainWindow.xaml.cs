using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using TreeExplorer.Helpers;
using TreeExplorer.Models;
using TreeExplorer.Properties;

namespace TreeExplorer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _oracleCredentialsFileTemplate = "id_oracle-{profilName}.txt";
        private readonly string _pgCredentialsFileTemplate = "id_pg-{profilName}.txt";
        private string _oracleCredentialsFile = "id_oracle.txt";
        private string _pgCredentialsFile = "id_pg.txt";
        private readonly string _logFile = "log.txt";
        //private IOracleService _oracleService;
        //private IPostgresService _postgresService;
        private const float DefaultWindowTop = 0.00f;
        private const float DefaultWindowLeft = 0.00f;

        public MainWindow()
        {
            InitializeComponent();
            btnTestPostgres.Click += BtnTestPostgres_Click;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // nothing for now
        }

        private void InitializeSampleTree()
        {
            // Création de la racine de l'arborescence
            var root = new TreeNode("Base de données");

            // Ajout de tables
            var tablesNode = new TreeNode("Tables");
            var table1 = new TreeNode("Utilisateurs");
            var table2 = new TreeNode("Commandes");
            var table3 = new TreeNode("Produits");
            
            tablesNode.AddChildren(new[] { table1, table2, table3 });

            // Ajout de vues
            var vuesNode = new TreeNode("Vues");
            var vue1 = new TreeNode("Vue_Utilisateurs_Actifs");
            var vue2 = new TreeNode("Vue_Commandes_EnCours");
            
            vuesNode.AddChildren(new[] { vue1, vue2 });

            // Ajout de procédures stockées
            var proceduresNode = new TreeNode("Procédures");
            var proc1 = new TreeNode("sp_CreerUtilisateur");
            var proc2 = new TreeNode("sp_SupprimerUtilisateur");
            
            proceduresNode.AddChildren(new[] { proc1, proc2 });

            // Ajout de tous les nœuds à la racine
            root.AddChildren(new[] { tablesNode, vuesNode, proceduresNode });

            // Création d'une collection observable pour la liaison de données
            var treeData = new ObservableCollection<TreeNode> { root };

            // Liaison des données au TreeView
            treeView.ItemsSource = treeData;
        }

        private void BtnLoadModelTree_Click(object sender, RoutedEventArgs e)
        {
            // Cette méthode sera appelée lors du clic sur le bouton "Load tree"
            // Vous pouvez y ajouter la logique pour charger les données réelles depuis la base de données
            InitializeSampleTree(); // Pour l'instant, on recharge l'exemple
        }

        private void BtnTestPostgres_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Handle tree view item selection change here
            if (e.NewValue is TreeViewItem selectedItem)
            {
                // Do something with the selected item
                MessageBox.Show($"Selected item: {selectedItem.Header}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize the tree view with some items
            TreeViewItem rootItem = new TreeViewItem { Header = "Root" };
            TreeViewItem childItem1 = new TreeViewItem { Header = "Child 1" };
            TreeViewItem childItem2 = new TreeViewItem { Header = "Child 2" };
            rootItem.Items.Add(childItem1);
            rootItem.Items.Add(childItem2);
            //MyTreeView.Items.Add(rootItem);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Save window position and size
            var settings = Settings.Default;

            if (WindowState == WindowState.Normal)
            {
                settings.WindowTop = Top;
                settings.WindowLeft = Left;
                settings.WindowHeight = Height;
                settings.WindowWidth = Width;
            }
            else
            {
                settings.WindowTop = RestoreBounds.Top;
                settings.WindowLeft = RestoreBounds.Left;
                settings.WindowHeight = RestoreBounds.Height;
                settings.WindowWidth = RestoreBounds.Width;
            }

            settings.WindowState = WindowState;
            settings.Save();

            // Save ID
            SaveCredentials();

            // Save logs
            SaveLogs();
        }

        private void SaveCredentials()
        {
            try
            {
                // Save Oracle ID
                if (chkSaveOracle.IsChecked == true && cboOracleConnectionProfile.SelectedIndex != -1)
                {
                    var oracleCredentials = new DbCredentials
                    {
                        Server = txtOracleServer.Text,
                        Port = txtOraclePort.Text,
                        Database = txtOracleServiceName.Text,
                        Username = txtOracleUser.Text,
                        Password = pwdOraclePassword.Password
                    };

                    var jsonOracle = JsonConvert.SerializeObject(oracleCredentials);
                    var encryptedOracle = EncryptionHelper.Encrypt(jsonOracle);
                    File.WriteAllText(GetSelectedProfilforOracle(cboOracleConnectionProfile.SelectedValue.ToString()), encryptedOracle);
                    Settings.Default.OracleSelectedProfil = cboOracleConnectionProfile.SelectedValue.ToString();
                    Settings.Default.Save();
                }

                // Save PostgreSQL ID
                if (chkSavePostgres.IsChecked == true && cboPostgresqlConnectionProfile.SelectedIndex != -1)
                {
                    var pgCredentials = new DbCredentials
                    {
                        Server = txtPostgresServer.Text,
                        Port = txtPostgresPort.Text,
                        Database = txtPostgresDatabase.Text,
                        Schema = txtPostgresSchema.Text,
                        Username = txtPostgresUser.Text,
                        Password = pwdPostgresPassword.Password
                    };

                    var jsonPg = JsonConvert.SerializeObject(pgCredentials);
                    var encryptedPg = EncryptionHelper.Encrypt(jsonPg);
                    File.WriteAllText(GetSelectedProfilforPostgresql(cboPostgresqlConnectionProfile.SelectedValue.ToString()), encryptedPg);
                    Settings.Default.PostgresqlSelectedProfil = cboPostgresqlConnectionProfile.SelectedValue.ToString();
                    Settings.Default.Save();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Error saving credentials: {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetSelectedProfilforPostgresql(string profilName)
        {
            _pgCredentialsFile = _pgCredentialsFileTemplate.Replace("{profilName}", profilName);
            return _pgCredentialsFile;
        }

        private string GetSelectedProfilforOracle(string profilName)
        {
            _oracleCredentialsFile = _oracleCredentialsFileTemplate.Replace("{profilName}", profilName);
            return _oracleCredentialsFile;
        }

        private void BtnLoadOracleConnection_Click(object sender, RoutedEventArgs e)
        {
            // Load Oracle connection settings
            //LoadOracleConnectionSettings();
        }

        private void SaveLogs()
        {
            try
            {
                File.WriteAllText(_logFile, txtLogs.Text);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Error saving logs: {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChkSaveOracle_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLoadPostgresqlConnection_Click(object sender, RoutedEventArgs e)
        {
            if (cboPostgresConnectionProfileFile.SelectedIndex == -1)
            {
                MessageBox.Show("You have to select a profile name for the PostgreSQL connection", "No profile chosen", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }

            var profileName = cboPostgresConnectionProfileFile.SelectedValue.ToString();
            profileName = ChangeProfileNameToProfileFilenameForPostgresql(profileName);
            var encryptedPg = File.ReadAllText(profileName);
            var decryptedPg = EncryptionHelper.Decrypt(encryptedPg);
            if (!string.IsNullOrEmpty(decryptedPg))
            {
                var pgCredentials = JsonConvert.DeserializeObject<DbCredentials>(decryptedPg);
                txtPostgresServer.Text = pgCredentials.Server;
                txtPostgresPort.Text = pgCredentials.Port;
                txtPostgresDatabase.Text = pgCredentials.Database;
                txtPostgresSchema.Text = pgCredentials.Schema;
                txtPostgresUser.Text = pgCredentials.Username;
                pwdPostgresPassword.Password = pgCredentials.Password;
                chkSavePostgres.IsChecked = true;
            }
        }

        private string ChangeProfileNameToProfileFilenameForPostgresql(string profileName)
        {
            return _pgCredentialsFileTemplate.Replace("{profilName}", profileName);
        }

        private void ChkSavePostgres_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLoadResultTree_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
