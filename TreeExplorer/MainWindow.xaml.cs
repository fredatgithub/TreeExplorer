using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Npgsql;
using TreeExplorer.Helpers;
using TreeExplorer.Models;
using TreeExplorer.Properties;
using TreeExplorer.Services;

namespace TreeExplorer
{
  /// <summary>
  /// Logique d'interaction pour MainWindow.xaml
  /// </summary>
  public partial class MainWindow: Window
  {
    private readonly DatabaseService _databaseService;
    private readonly string _oracleCredentialsFileTemplate = "id_oracle-{profilName}.txt";
    private readonly string _pgCredentialsFileTemplate = "id_pg-{profilName}.txt";
    private string _oracleCredentialsFile = "id_oracle.txt";
    private string _pgCredentialsFile = "id_pg.txt";
    private readonly string _logFile = "log.txt";
    private const float DefaultWindowTop = 0.00f;
    private const float DefaultWindowLeft = 0.00f;

    public MainWindow()
    {
      InitializeComponent();
      btnTestPostgres.Click += BtnTestPostgres_Click;
      _databaseService = new DatabaseService();
      Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        // Charger les paramètres de connexion PostgreSQL au démarrage
        LoadPostgresConnectionSettings();

        // Vous pouvez ajouter ici le chargement des paramètres Oracle si nécessaire
        // LoadOracleConnectionSettings();
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Erreur lors du chargement des paramètres : {ex.Message}",
            "Erreur",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
      }
    }

    private async void BtnLoadModelTree_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        // Vérifier que les informations de connexion sont remplies
        if (string.IsNullOrWhiteSpace(txtPostgresServer.Text) ||
            string.IsNullOrWhiteSpace(txtPostgresPort.Text) ||
            string.IsNullOrWhiteSpace(txtPostgresDatabase.Text) ||
            string.IsNullOrWhiteSpace(txtPostgresUser.Text) ||
            string.IsNullOrWhiteSpace(pwdPostgresPassword.Password))
        {
          MessageBox.Show("Veuillez remplir tous les champs de connexion PostgreSQL.",
              "Champs manquants",
              MessageBoxButton.OK,
              MessageBoxImage.Warning);
          return;
        }

        // Afficher un indicateur de chargement
        Cursor = System.Windows.Input.Cursors.Wait;
        btnLoadModelTree.IsEnabled = false;
        btnLoadModelTree.Content = "Chargement...";

        // Configurer la connexion
        _databaseService.SetConnectionString(
            txtPostgresServer.Text,
            txtPostgresPort.Text,
            txtPostgresDatabase.Text,
            txtPostgresUser.Text,
            pwdPostgresPassword.Password);

        // Créer la racine de l'arborescence
        var root = new TreeNode(txtPostgresDatabase.Text);

        // Charger les tables de manière asynchrone
        var tablesNode = new TreeNode("Tables");
        var tables = await _databaseService.GetTablesAsync();
        foreach (var table in tables)
        {
          tablesNode.AddChild(new TreeNode(table));
        }
        root.AddChild(tablesNode);

        // Charger les vues de manière asynchrone
        var viewsNode = new TreeNode("Vues");
        var views = await _databaseService.GetViewsAsync();
        foreach (var view in views)
        {
          viewsNode.AddChild(new TreeNode(view));
        }
        root.AddChild(viewsNode);

        // Mettre à jour l'interface utilisateur
        var treeData = new ObservableCollection<TreeNode> { root };
        treeView.ItemsSource = treeData;
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Erreur lors du chargement des données : {ex.Message}",
            "Erreur",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
      }
      finally
      {
        // Réactiver le bouton et restaurer le curseur
        btnLoadModelTree.IsEnabled = true;
        btnLoadModelTree.Content = "Load tree";
        Cursor = System.Windows.Input.Cursors.Arrow;
      }
    }

    private async void BtnTestPostgres_Click(object sender, RoutedEventArgs e)
    {
      // Vérifier que les champs obligatoires sont remplis
      if (string.IsNullOrWhiteSpace(txtPostgresServer.Text) ||
          string.IsNullOrWhiteSpace(txtPostgresPort.Text) ||
          string.IsNullOrWhiteSpace(txtPostgresDatabase.Text) ||
          string.IsNullOrWhiteSpace(txtPostgresUser.Text) ||
          string.IsNullOrWhiteSpace(pwdPostgresPassword.Password))
      {
        MessageBox.Show("Veuillez remplir tous les champs de connexion.",
            "Champs manquants",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
        return;
      }

      // Afficher un indicateur de chargement
      var originalContent = btnTestPostgres.Content;
      btnTestPostgres.Content = "Connexion en cours...";
      btnTestPostgres.IsEnabled = false;
      Cursor = System.Windows.Input.Cursors.Wait;

      try
      {
        // Tester la connexion de manière asynchrone
        bool isConnected = await Task.Run(() =>
        {
          try
          {
            using (var conn = new NpgsqlConnection(
                        $"Host={txtPostgresServer.Text};" +
                        $"Port={txtPostgresPort.Text};" +
                        $"Database={txtPostgresDatabase.Text};" +
                        $"Username={txtPostgresUser.Text};" +
                        $"Password={pwdPostgresPassword.Password};" +
                        "Command Timeout=15;"))
            {
              conn.Open();
              return true;
            }
          }
          catch
          {
            return false;
          }
        });

        if (isConnected)
        {
          // Sauvegarder les paramètres de connexion
          SavePostgresConnectionSettings();

          MessageBox.Show("Connexion réussie à la base de données PostgreSQL !\n\n" +
                        "Les paramètres de connexion ont été sauvegardés.",
              "Succès",
              MessageBoxButton.OK,
              MessageBoxImage.Information);
        }
        else
        {
          throw new Exception("Impossible de se connecter à la base de données. Vérifiez vos paramètres de connexion.");
        }
      }
      catch (PostgresException ex)
      {
        string errorMessage = ex.Message;
        if (ex.SqlState == "28P01") // Code d'erreur pour échec d'authentification
        {
          errorMessage = "Échec de l'authentification. Vérifiez votre nom d'utilisateur et mot de passe.";
        }
        MessageBox.Show($"Erreur de connexion : {errorMessage}",
            "Erreur de connexion",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Erreur lors de la connexion : {ex.Message}",
            "Erreur",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
      }
      finally
      {
        // Restaurer l'interface utilisateur
        btnTestPostgres.Content = originalContent;
        btnTestPostgres.IsEnabled = true;
        Cursor = System.Windows.Input.Cursors.Arrow;
      }
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
      try
      {
        if (chkSavePostgres.IsChecked == true)
        {
          // Si la case est cochée, sauvegarder les paramètres actuels
          SavePostgresConnectionSettings();
        }
        else
        {
          // Si la case est décochée, supprimer le fichier de configuration s'il existe
          if (File.Exists(_pgCredentialsFile))
          {
            File.Delete(_pgCredentialsFile);
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Erreur lors de la gestion des paramètres : {ex.Message}",
            "Erreur",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
      }
    }

    private void SavePostgresConnectionSettings()
    {
      try
      {
        // Créer un objet avec les informations de connexion
        var connectionInfo = new
        {
          Server = txtPostgresServer.Text,
          Port = txtPostgresPort.Text,
          Database = txtPostgresDatabase.Text,
          Username = txtPostgresUser.Text,
          Password = pwdPostgresPassword.Password,
          Schema = txtPostgresSchema.Text,
          LastConnection = DateTime.Now
        };

        // Sérialiser en JSON
        string json = JsonConvert.SerializeObject(connectionInfo, Formatting.Indented);

        // Chiffrer le contenu
        string encryptedContent = EncryptionHelper.Encrypt(json);

        // Créer le répertoire s'il n'existe pas
        string directory = Path.GetDirectoryName(_pgCredentialsFile);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
          Directory.CreateDirectory(directory);
        }

        // Écrire dans le fichier
        File.WriteAllText(_pgCredentialsFile, encryptedContent);

        // Mettre à jour l'interface utilisateur
        chkSavePostgres.IsChecked = true;
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Erreur lors de la sauvegarde des paramètres : {ex.Message}",
            "Erreur",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
      }
    }

    private bool LoadPostgresConnectionSettings()
    {
      try
      {
        if (!File.Exists(_pgCredentialsFile))
        {
          return false;
        }

        // Lire le contenu chiffré
        string encryptedContent = File.ReadAllText(_pgCredentialsFile);

        // Décrypter le contenu
        string json = EncryptionHelper.Decrypt(encryptedContent);

        if (string.IsNullOrEmpty(json))
        {
          return false;
        }

        // Désérialiser le JSON
        var connectionInfo = JsonConvert.DeserializeAnonymousType(json, new
        {
          Server = "",
          Port = "",
          Database = "",
          Username = "",
          Password = "",
          Schema = "public",
          LastConnection = DateTime.MinValue
        });

        if (connectionInfo == null)
        {
          return false;
        }

        // Mettre à jour l'interface utilisateur
        txtPostgresServer.Text = connectionInfo.Server;
        txtPostgresPort.Text = connectionInfo.Port;
        txtPostgresDatabase.Text = connectionInfo.Database;
        txtPostgresUser.Text = connectionInfo.Username;
        pwdPostgresPassword.Password = connectionInfo.Password;
        txtPostgresSchema.Text = !string.IsNullOrEmpty(connectionInfo.Schema) ?
            connectionInfo.Schema : "public";

        // Cocher la case de sauvegarde
        chkSavePostgres.IsChecked = true;

        return true;
      }
      catch
      {
        // En cas d'erreur, on ne fait rien
        return false;
      }
    }

    private void BtnLoadResultTree_Click(object sender, RoutedEventArgs e)
    {

    }
  }
}
